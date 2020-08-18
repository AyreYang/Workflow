using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using Database.Implements.SQLite;
using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestConsole.Models;
using WorkFlow.Interfaces.Entities;
using WorkFlowEngine;
using WorkFlowEngine.Entities;

namespace TestConsole
{
    class Program
    {
        private const string DBSRC = @"E:\01_Workspace\01_VS\05_WorkFlow\99_Temp\WorkFlow.s3db";
        private static IDatabaseAccessor accessor = new DatabaseAccessor(DBSRC);
        private static DBContext dbcontext = new DBContext(accessor);
        static void Main(string[] args)
        {
            var model = new Model2(accessor);

            //Create workflow
            //model.CreateWorkFlow();

            var id = model.CreateSendTask("E0350802", "TST-M2-002");
            //var id = model.CreateApprovalTask("E0300084", Guid.Parse("e36125ff-08af-4cb0-b312-c990befd040d"), 2, "approved by Lee!");
            //var id = model.CreateApprovalTask("E0344652", Guid.Parse("d204e1c7-a4b8-441b-80ac-0611fe8c766d"), 2, "approved by Cui!");
            //var id = model.CreateApprovalTask("E0299598", Guid.Parse("62ff75ce-1e40-4f6f-8953-e6f76d7ab7a7"), 2, "approved by Rita!");
            //var id = model.CreateApprovalTask("E01234567", Guid.Parse("c963049d-4ca8-45dd-a65b-e011bd9edef5"), 3, "rejected by Someone!");

            //var id = model.CreateApprovalTask("E0350802", Guid.Parse("558fa123-7764-48be-8b69-90b11a27ae64"), 2, "resubmitted by Ayre!");
            //var id = model.CreateApprovalTask("E0300084", Guid.Parse("6c02a9de-a1ee-48a8-9b9c-a1a9963cd0be"), 2, "approved by Lee!");
            //var id = model.CreateApprovalTask("E0344652", Guid.Parse("eab753e1-2013-414c-9ec5-46bb47731b7b"), 2, "approved by Cui!");
            //var id = model.CreateApprovalTask("E0299598", Guid.Parse("10edd29a-4b41-42ec-80d1-519a7b6a0bc1"), 3, "rejected by Rita!");
            //var id = model.CreateApprovalTask("E01234567", Guid.Parse("6c7cafe9-cd79-409e-afa6-beb7adc2e25a"), 2, "approved by Someone!");

            var tasks = GetTask();
            if (tasks != null && tasks.Length > 0)
            {
                ProcessTask(tasks.First());
            }


            model.DeleteTask(id);
            return;
        }

        private static void ProcessTask(WF_VW_TASK task)
        {
            if (task == null) return;

            var workflow = WorkFlowFactory.Inst.CreateWorkFlow(task.WorkFlowId);
            workflow.ProcessTask(task as ITask);
        }

        private static WF_VW_TASK[] GetTask()
        {
            var list = dbcontext.Retrieve<WF_VW_TASK>();
            return list.Entities;
        }
    }
}
