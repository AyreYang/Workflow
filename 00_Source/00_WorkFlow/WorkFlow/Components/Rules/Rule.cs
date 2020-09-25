using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;

namespace WorkFlow.Components.Rules
{
    public abstract class Rule
    {
        private object host { get; set; }
        public int Flag { get; set; }
        private MethodInfo rule { get; set; }
        public bool Available { get; private set; }

        protected abstract string BuildSource(string script, out string className, out string methodName);

        protected Rule()
        {
            Flag = 0;
            Available = false;
        }
        protected Rule(string script):this()
        {
            if (string.IsNullOrWhiteSpace(script)) throw new ArgumentNullException("script");
            string className, methodName;
            var source = BuildSource(script, out className, out methodName);
            if (string.IsNullOrWhiteSpace(source)) throw new ApplicationException("source is null or empty!");
            CreateRule(source, className, methodName);
        }
        protected void Initialize(object host, MethodInfo rule)
        {
            if (host == null) throw new ArgumentNullException("host");
            if (rule == null) throw new ArgumentNullException("rule");
            
            this.host = host;
            this.rule = rule;
            Available = true;
        }

        private void CreateRule(string source, string className, string methodName)
        {
            var provider = new CSharpCodeProvider();
            var paramters = new CompilerParameters();
            paramters.CompilerOptions = string.Empty;
            paramters.GenerateExecutable = false;
            paramters.GenerateInMemory = true;
            paramters.CompilerOptions += "/optimize";
            //添加需要引用的dll
            paramters.ReferencedAssemblies.Add("System.dll");
            paramters.ReferencedAssemblies.Add("System.Core.dll");
            paramters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");

            //编译代码
            var result = provider.CompileAssemblyFromSource(paramters, source);

            if (result.Errors.HasErrors)
            {
                var message = new StringBuilder();
                message.AppendLine(string.Format("CompilingErrors({0}):", result.Errors.Count));
                for (var i = 0; i < result.Errors.Count; i++) message.AppendLine(string.Format("line({0}):{1}", result.Errors[i].Line, result.Errors[i].ErrorText));
                throw new ApplicationException(message.ToString());
            }
            else
            {
                var inst = result.CompiledAssembly.CreateInstance(className);
                var method = inst.GetType().GetMethod(methodName);
                Initialize(inst, method);
            }
        }
        protected object Test(params object[] parameters)
        {
            return rule.Invoke(host, parameters);
        }
        protected void SetFlag(int flag)
        {
            if (flag < 0 || flag > 99) throw new ArgumentException(string.Format("Argument(flag:{0}=>[0-99]) is invalid!", flag, "flag"));
            Flag = flag;
        }
    }
}
