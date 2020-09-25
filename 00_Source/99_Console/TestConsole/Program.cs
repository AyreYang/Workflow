using Database.Entity;
using Database.Implements.SQLite;
using Database.Interfaces;
using Newtonsoft.Json;
using System;
using System.Linq;
using TestConsole.Models;
using WorkFlow.Interfaces.Entities;
using WorkFlowEngine;
using WorkFlowEntities.Views;

namespace TestConsole
{
    class Program
    {
        private const string DBSRC = @"E:\01_Workspace\01_VS\05_WorkFlow\99_Temp\WorkFlow.s3db";
        private static IDatabaseAccessor accessor = new DatabaseAccessor(DBSRC);
        private static DBContext dbcontext = new DBContext(accessor);
        static void Main(string[] args)
        {
            //TestRule4Approvers();

            //TestRule4Input();
            //TestRule4Input0();

            //TestRule4Status();


            //var rule = new Rule(@"
            //bool? result = null;
            //if (nodes != null && nodes.Length > 0)
            //{
            //    result = nodes.Any(n => (bool)n.status);
            //}

            //return result;");

            //var rule = new Rule(@"return parameter.flag;");
            //var param = JsonConvert.DeserializeObject("{\"flag\":2}");
            //var result = rule.Test(param);
            //return;

            var model = new Model99(accessor);
            //model.AddRules("E0350802");

            //Create workflow
            //model.CreateWorkFlow();

            Model99.Param99 parameter = new Model99.Param99();
            parameter.Reason = "DIT";
            parameter.Money = 780000.001M;

            //var id = model.CreateSendTask("E0350802", "TST-M99-001", JsonConvert.SerializeObject(parameter));
            //var id = model.CreateApprovalTask("DOA1", Guid.Parse("0245b00a-2e90-4da9-9fd2-dfdf9e20a023"), 2, "approved by DOA1!", JsonConvert.SerializeObject(parameter));
            //var id = model.CreateApprovalTask("DOA2", Guid.Parse("f37f801b-d2d8-451e-b575-e0f06cfa22ca"), 2, "approved by DOA2!", JsonConvert.SerializeObject(parameter));
            //var id = model.CreateApprovalTask("DOA3", Guid.Parse("8340efb8-442c-436d-b25d-a353e6087bfb"), 2, "approved by DOA3!", JsonConvert.SerializeObject(parameter));

            //var id = model.CreateApprovalTask("DITOrderManager", Guid.Parse("a33f8822-ddee-4a60-ad66-f3bb777fd611"), 2, "approved by DITOrderManager!", JsonConvert.SerializeObject(parameter));
            //var id = model.CreateApprovalTask("DITReturnManager", Guid.Parse("66123b54-ea08-4fc7-a58d-0e640754b6ba"), 2, "approved by DITReturnManager!", JsonConvert.SerializeObject(parameter));
            var id = model.CreateApprovalTask("DITHeadOfSupplyChain", Guid.Parse("71848176-b0ea-48ab-9d93-f52ce1c19369"), 2, "approved by DITHeadOfSupplyChain!", JsonConvert.SerializeObject(parameter));

            //var id = model.CreateApprovalTask("E0350802", Guid.Parse("0058047e-16e6-4d82-94f9-c06c5c219561"), 2, "resubmitted by Ayre!");
            //var id = model.CreateApprovalTask("E0300084", Guid.Parse("9c2a6948-e9bd-45b9-811b-1ad8d69edf0f"), 2, "approved by Lee!");
            //var id = model.CreateApprovalTask("E0344652", Guid.Parse("cbf64e6e-48de-440b-b672-7b2b77325507"), 2, "approved by Cui!");
            //var id = model.CreateApprovalTask("E0299598", Guid.Parse("b4d8ea2c-c309-400e-bdfa-2e19ef0ea69f"), 2, "approved by Rita!");
            //var id = model.CreateApprovalTask("E0123456", Guid.Parse("9666b80f-3ba5-4c1a-99af-41d7dfad2855"), 2, "approved by Someone!");

            //var id = model.CreateApprovalTask("E0350802", Guid.Parse("558fa123-7764-48be-8b69-90b11a27ae64"), 2, "resubmitted by Ayre!");
            //var id = model.CreateApprovalTask("E0300084", Guid.Parse("6c02a9de-a1ee-48a8-9b9c-a1a9963cd0be"), 2, "approved by Lee!");
            //var id = model.CreateApprovalTask("E0344652", Guid.Parse("eab753e1-2013-414c-9ec5-46bb47731b7b"), 2, "approved by Cui!");
            //var id = model.CreateApprovalTask("E0299598", Guid.Parse("10edd29a-4b41-42ec-80d1-519a7b6a0bc1"), 3, "rejected by Rita!");
            //var id = model.CreateApprovalTask("E01234567", Guid.Parse("6c7cafe9-cd79-409e-afa6-beb7adc2e25a"), 2, "approved by Someone!");

            try
            {
                var tasks = GetTask();
                if (tasks != null && tasks.Length > 0)
                {
                    ProcessTask(tasks.First());
                }
            }
            catch (Exception err) { }
            finally
            {
                model.DeleteTask(id);
            }

            return;
        }

        
        private static void ProcessTask(WF_VW_Task task)
        {
            if (task == null) return;

            var workflow = WorkFlowFactory.Inst.CreateWorkFlow(task.WorkflowID);
            if (!workflow.Ready) throw new ApplicationException();
            workflow.ProcessTask(task as IRTask);
        }

        private static WF_VW_Task[] GetTask()
        {
            var list = dbcontext.Retrieve<WF_VW_Task>();
            return list.Entities;
        }
        

        /*
        private static void TestRule4Approvers()
        {
            var parameter = "{\"division\":\"CHC\"}";
            var script = @"
                string[] approvers = null;
                string division = (string)parameter.division;
                switch(division.ToUpper())
                {
                    case ""CHC"":
                        approvers = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
                        break;
                    default:
                        approvers = null;
                        break;
                }
                return approvers;
            ";
            
            var rule = new Rule4Approvers(script);
            var approvers = rule.GetApprovers(JsonConvert.DeserializeObject(parameter));
            return;
        }

        private static void TestRule4Input()
        {
            var parameter = "{\"division\":\"db\"}";
            var script = @"
                var result = false;
                string division = (string)parameter.division;
                switch(division.ToUpper())
                {
                    case ""CHC"":
                        result = nodes.Any(n => n.iStatus == 1) ? false : true;
                        break;
                    case ""DB"":
                        result = nodes.Any(n => n.iStatus == 2) ? true : false;
                        break;
                    default:
                        result = false;
                        break;
                }
                return result;
            ";

            var rule = new Rule4Input(script);

            var nodes = new List<INode>();
            nodes.Add(new TNode(Guid.NewGuid(), NodeStatus.Processing));
            nodes.Add(new TNode(Guid.NewGuid(), NodeStatus.Approved));
            
            var can = rule.CanInput(nodes.ToArray(), JsonConvert.DeserializeObject(parameter));
            return;
        }
        private static void TestRule4Input0()
        {
            object parameter = null;

            var rule = new Rule4Input(1);

            var nodes = new List<INode>();
            nodes.Add(new TNode(Guid.NewGuid(), NodeStatus.Processing));
            nodes.Add(new TNode(Guid.NewGuid(), NodeStatus.Approved));

            var can = rule.CanInput(nodes.ToArray(), parameter);
            return;
        }

        private static void TestRule4Status()
        {
            var parameter = "{\"division\":\"db\"}";
            var script = @"
                var result = 0;
                string division = (string)parameter.division;
                switch(division.ToUpper())
                {
                    case ""CHC"":
                        result = details.Any(d => d.iStatus == 1) ? 1 : 2;
                        break;
                    case ""DB"":
                        result = details.Any(d => d.iStatus == 2) ? 2 : 1;
                        break;
                    default:
                        result = 0;
                        break;
                }
                return result;
            ";

            var rule = new Rule4Status(script);

            var details = new List<IPNodeDetail>();
            details.Add(new TDetail(Guid.NewGuid(), Guid.NewGuid(), NodeStatus.Processing));
            details.Add(new TDetail(Guid.NewGuid(), Guid.NewGuid(), NodeStatus.Approved));

            var status = rule.GetStatus(details.ToArray(), JsonConvert.DeserializeObject(parameter));
            return;
        }
        */
    }

    /*
    public class Rule
    {
        private const string TEMPLATE = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;

            public class Express
            {
                public int Test(dynamic parameter)
                {
                    {body}
                }
            }";
        private object host { get; set; }
        private MethodInfo rule { get; set; }

        public Rule(string script)
        {
            if (string.IsNullOrWhiteSpace(script)) throw new ArgumentNullException("script");
            CreateRule(script.Trim());
        }
        private void CreateRule(string script)
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
            //paramters.ReferencedAssemblies.Add("System.Data.dll");
            //paramters.ReferencedAssemblies.Add("System.Xml.dll");
            //paramters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            //编译代码
            var source = TEMPLATE.Replace("{body}", script);
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
                host = result.CompiledAssembly.CreateInstance("Express");
                rule = host.GetType().GetMethod("Test");
                //objMI.Invoke(objHelloWorld, null);
            }
        }
        public int? Test(dynamic parameter)
        {
            var result = rule.Invoke(host, new object[] { parameter });
            return result != null ? (int?)result : null;
        }

        //private bool? Test1(dynamic parameters)
        //{
        //    bool? result = null;
        //    if (nodes != null && nodes.Length > 0)
        //    {
        //        result = nodes.Any(n => n.Value);
        //    }
            
        //    return result;
        //}
    }

    public class Param
    {
        public Guid id { get; set; }
        public bool status { get; set; }
    }

    public class TNode : INode
    {
        public Guid Id { get; private set; }

        public NodeStatus Status { get; set; }
        public int iStatus
        {
            get
            {
                return (int)Status;
            }
        }

        public bool HasParents { get; set; }

        public bool HasChildren { get; set; }

        public TNode(Guid id, NodeStatus status)
        {
            Id = id;
            Status = status;

            HasParents = false;
            HasChildren = false;
        }

        public INode FetchNode(Guid node)
        {
            return null;
        }

        public INode[] FetchParents()
        {
            return null;
        }

        public INode FetchSender()
        {
            return null;
        }
    }

    public class TDetail : IPNodeDetail
    {
        public Guid Id { get; private set; }

        public Guid Nid { get; private set; }

        public NodeStatus Status { get; set; }

        public int iStatus => (int)Status;

        public TDetail(Guid id, Guid nid, NodeStatus status)
        {
            Id = id;
            Nid = nid;
            Status = status;
        }
    }
    */
}
