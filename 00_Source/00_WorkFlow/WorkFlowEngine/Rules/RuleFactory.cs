using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using WorkFlow.Components;
using WorkFlow.Enums;
using WorkFlow.Interfaces;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEngine.Rules
{
    internal struct DllInfo
    {
        public string Assembly { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
    }
    internal struct WebInfo
    {
        public string Url { get; set; }
        public string Method { get; set; }
    }

    internal class RuleFactory
    {
        private const string RX_URL = @"^(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]$";
        private static object _lock = new object();
        private static RuleFactory _inst = null;
        private List<KeyValuePair<string, ProxyObject>> assemblys = null;
        internal static RuleFactory Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_lock)
                    {
                        if (_inst == null) _inst = new RuleFactory();
                    }
                }
                return _inst;
            }
        }

        private RuleFactory()
        {
            assemblys = new List<KeyValuePair<string, ProxyObject>>();
        }

        private string LoadAssembly(Guid workflow, string dll)
        {
            if (string.IsNullOrWhiteSpace(dll)) throw new ArgumentNullException("dll");
            if (!File.Exists(dll.Trim())) throw new FileNotFoundException("The dll file doesn`t exist!", dll);
            var file = new FileInfo(dll.Trim());
            var key = file.FullName.Trim().ToUpper();
            if (!assemblys.Any(d => d.Key.Equals(key)))
            {
                var domain = AppDomain.CreateDomain(file.Name);
                var assembly = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
                var type = typeof(ProxyObject).FullName;
                var proxy = (ProxyObject)domain.CreateInstanceFromAndUnwrap(assembly, type);
                proxy.domain = domain;
                proxy.LoadAssembly(workflow, dll.Trim());

                assemblys.Add(new KeyValuePair<string, ProxyObject>(key, proxy));
            }
            return key;
        }

        private string Post(string url, string json)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");
            if (!Regex.IsMatch(url.Trim(), RX_URL)) throw new ArgumentException(string.Format("url({0}) is illegal!", url), "url");

            var request = (HttpWebRequest)WebRequest.Create(url.Trim());
            request.Method = "POST";

            if (!string.IsNullOrWhiteSpace(json))
            {
                request.ContentType = "application/json;charset=UTF-8";
                byte[] data = Encoding.UTF8.GetBytes(json.Trim());
                int length = data.Length;
                request.ContentLength = length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, length);
                }
            }

            string result = null;
            using (StreamReader reader = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        private string Get(string url, params KeyValuePair<string, object>[] parameters)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");
            if (!Regex.IsMatch(url.Trim(), RX_URL)) throw new ArgumentException(string.Format("url({0}) is illegal!", url), "url");
            var sbUrl = new StringBuilder();
            sbUrl.Append(url.Trim());
            if (parameters != null && parameters.Length > 0)
            {
                sbUrl.Append("?");
                int i = 0;
                foreach (var item in parameters)
                {
                    if (i++ > 0) sbUrl.Append("&");
                    sbUrl.AppendFormat("{0}={1}", item.Key, item.Value);
                }
            }

            var request = (HttpWebRequest)WebRequest.Create(sbUrl.ToString());
            request.Method = "GET";
            string result = null;
            using (StreamReader reader = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        private T Post<T>(string url, object parameter)
        {
            T result = default(T);
            var json = parameter != null ? JsonConvert.SerializeObject(parameter) : null;
            var str = Post(url, json);
            if(str != null)
            {
                result = JsonConvert.DeserializeObject<T>(str.Trim());
            }
            return result;
        }
        private T Get<T>(string url, params KeyValuePair<string, object>[] parameters)
        {
            T result = default(T);
            var str = Get(url, parameters);
            if (str != null)
            {
                result = JsonConvert.DeserializeObject<T>(str.Trim());
            }
            return result;
        }

        internal void ReleaseAssembly(Guid workflow)
        {
            assemblys.ForEach(a =>
            {
                a.Value.ReleaseAssembly(workflow);
                if(a.Value.count <= 0) AppDomain.Unload(a.Value.domain);
            });

            assemblys.RemoveAll(a => a.Value.count <= 0);
        }

        internal Func<KeyValuePair<string, object>[], string[]> FetchRule4Approvers(Guid workflow, int type, string content)
        {
            Func<KeyValuePair<string, object>[], string[]> func = null;
            //处理类型
            switch (type)
            {
                case 0: //明文
                    func = (parameters) =>
                    {
                        var approvers = new List<string>();
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            var array = JsonConvert.DeserializeObject<string[]>(content.Trim());
                            if (array != null) array.ToList().ForEach(item =>
                              {
                                  var user = item.Trim().ToUpper();
                                  if (!string.IsNullOrWhiteSpace(user) && !approvers.Any(a => a.Equals(user))) approvers.Add(user);
                              });
                        }
                        return approvers.ToArray();
                    };
                    break;
                case 1: //dll
                    var dll = JsonConvert.DeserializeObject<DllInfo>(content.Trim());
                    var key = LoadAssembly(workflow, dll.Assembly);
                    var proxy = assemblys.First(p => p.Key.Equals(key)).Value;
                    func = (parameters) =>
                    {
                        string[] approvers = null;
                        var obj = proxy.Invoke(dll.Class, dll.Method, parameters);
                        if (obj != null && obj is string[])
                        {
                            approvers = (string[])obj;
                        }
                        return approvers;
                    };
                    break;
                case 2: //webservice/webapi
                    var web = JsonConvert.DeserializeObject<WebInfo>(content.Trim());
                    func = (parameters) =>
                    {
                        string[] approvers =
                        !string.IsNullOrWhiteSpace(web.Method) && "POST".Equals(web.Method.Trim().ToUpper()) ?
                        Post<string[]>(web.Url, parameters) : Get<string[]>(web.Url, parameters);
                        return approvers;
                    };
                    break;
            }
            return func;
        }
        internal Func<INode[], bool> FetchRule4Input(Guid workflow, int type, string content)
        {
            Func<INode[], bool> func = null;
            //处理类型
            switch (type)
            {
                case 0: //明文
                    func = (nodes) =>
                    {
                        var result = false;
                        bool.TryParse(content ?? string.Empty, out result);
                        return result;
                    };
                    break;
                case 1: //dll
                    var info = JsonConvert.DeserializeObject<DllInfo>(content);
                    var key = LoadAssembly(workflow, info.Assembly);
                    var proxy = assemblys.First(p => p.Key.Equals(key)).Value;
                    func = (nodes) =>
                    {
                        var result = false;
                        var obj = proxy.Invoke(info.Class, info.Method, nodes);
                        if (obj != null && obj is bool)
                        {
                            result = (bool)obj;
                        }
                        return result;
                    };
                    break;
            }
            return func;
        }
        internal Func<IPNodeDetail[], INode[], OPResult> FetchRule4Output(Guid workflow, int type, string content)
        {
            Func<IPNodeDetail[], INode[], OPResult> func = null;
            //处理类型
            switch (type)
            {
                case 0: //明文
                    func = (details, nodes) =>
                    {
                        var result = default(OPResult);
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            var json = (JObject)JsonConvert.DeserializeObject(content.Trim());
                            if (json.ContainsKey("Command")) 
                            {
                                if (string.IsNullOrWhiteSpace(json["Command"].ToString()))
                                {
                                    result.Command = OPCommand.none;
                                }
                                else
                                {
                                    var command = OPCommand.none;
                                    if(Enum.TryParse(json["Command"].ToString().Trim().ToLower(), out command))
                                    {
                                        result.Command = command;
                                    }
                                }
                            }
                            if (json.ContainsKey("Nodes"))
                            {
                                var array = json["Nodes"].ToObject<string[]>();
                                if (array != null) array.ToList().ForEach(item =>
                                {
                                    var id = Guid.Empty;
                                    if (Guid.TryParse(item.Trim(), out id)
                                    && nodes.Any(node => node.Id.Equals(id)))
                                        result.AddNodes(nodes.First(node => node.Id.Equals(id)));
                                });
                            }
                            
                        }
                        return result;
                    };
                    break;
                case 1: //dll
                    var info = JsonConvert.DeserializeObject<DllInfo>(content);
                    var key = LoadAssembly(workflow, info.Assembly);
                    var proxy = assemblys.First(p => p.Key.Equals(key)).Value;
                    func = (details, nodes) =>
                    {
                        var result = default(OPResult);
                        var obj = proxy.Invoke(info.Class, info.Method, details, nodes);
                        if (obj != null && obj is OPResult)
                        {
                            result = obj as OPResult;
                        }
                        return result;
                    };
                    break;
            }
            return func;
        }
    }

    internal class ProxyObject : MarshalByRefObject
    {
        private bool ready = false;
        public AppDomain domain = null;
        private Assembly assembly = null;
        private List<Guid> list = new List<Guid>();
        public int count
        {
            get { return list.Count; }
        }

        internal void LoadAssembly(Guid id, string dll)
        {
            if(!ready) assembly = Assembly.LoadFile(dll);
            if (!list.Any(i => i.Equals(id))) list.Add(id);
            ready = true;
        }
        internal void ReleaseAssembly(Guid id)
        {
            if (list.Any(i => i.Equals(id))) list.Remove(id);
        }
        public object Invoke(string fullClassName, string methodName, params Object[] args)
        {
            if (!ready) throw new ApplicationException("Proxy is not ready!");
            if (string.IsNullOrWhiteSpace(fullClassName)) throw new ArgumentNullException("fullClassName");
            if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentNullException("methodName");

            var classnm = fullClassName.Trim();
            Type tp = assembly.GetType(classnm);
            if (tp == null) throw new ApplicationException(string.Format("Proxy({0}) doesn`t contains class({1})!", assembly.FullName, classnm));

            var methodnm = methodName.Trim();
            MethodInfo method = tp.GetMethod(methodnm);
            if (method == null) throw new ApplicationException(string.Format("Class({0}) doesn`t contains method({1}) in proxy({2})!", classnm, methodnm, assembly.FullName));

            Object obj = Activator.CreateInstance(tp);
            return method.Invoke(obj, args);
        }
    }
}
