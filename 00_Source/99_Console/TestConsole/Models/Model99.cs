using Database.Interfaces;
using System;
using WorkFlowEntities.Entities;

namespace TestConsole.Models
{
    public class Model99 : Model
    {
        public struct Param99
        {
            public string Reason { get; set; }
            public decimal Money { get; set; }
        }

        public Model99(IDatabaseAccessor accessor) : base(accessor, Guid.Parse("cfc08f08-c211-4b7a-a117-422ea399f6a0"))
        {
        }

        public override void CreateWorkFlow()
        {
            var user = "E0350802";
            
            var ent_wf = new WF_DEF_Workflow();
            ent_wf.ID = Guid.NewGuid();
            ent_wf.Name = "工作流模型99-eApproval";
            ent_wf.Code = "WorkFlowModel99-eApproval";
            ent_wf.IsDeleted = false;
            ent_wf.CreatedBy = user;
            ent_wf.CreatedOn = DateTime.Now;
            ent_wf.LastModifiedBy = user;
            ent_wf.LastModifiedOn = DateTime.Now;

            #region 节点
            #region 发起节点
            var ent_ndStart = ent_wf.Nodes.NewEntity();
            ent_ndStart.ID = Guid.NewGuid();
            ent_ndStart.WorkflowID = ent_wf.ID;
            ent_ndStart.Name = "发起节点";
            ent_ndStart.Code = "Start";
            ent_ndStart.Type = 0;
            ent_ndStart.IsDeleted = false;
            ent_ndStart.CreatedBy = user;
            ent_ndStart.CreatedOn = DateTime.Now;
            ent_ndStart.LastModifiedBy = user;
            ent_ndStart.LastModifiedOn = DateTime.Now;
            #endregion

            #region DIT节点
            var ent_ndDIT = ent_wf.Nodes.NewEntity();
            ent_ndDIT.ID = Guid.NewGuid();
            ent_ndDIT.WorkflowID = ent_wf.ID;
            ent_ndDIT.Name = "DIT节点";
            ent_ndDIT.Code = "DIT";
            ent_ndDIT.Type = 2;
            ent_ndDIT.IsDeleted = false;
            ent_ndDIT.CreatedBy = user;
            ent_ndDIT.CreatedOn = DateTime.Now;
            ent_ndDIT.LastModifiedBy = user;
            ent_ndDIT.LastModifiedOn = DateTime.Now;
            #endregion

            #region DIT_Order_Manager节点
            var ent_ndDITOrderMgr = ent_wf.Nodes.NewEntity();
            ent_ndDITOrderMgr.ID = Guid.NewGuid();
            ent_ndDITOrderMgr.WorkflowID = ent_wf.ID;
            ent_ndDITOrderMgr.Name = "DIT_Order_Manager节点";
            ent_ndDITOrderMgr.Code = "DIT_Order_Manager";
            ent_ndDITOrderMgr.Type = 1;
            ent_ndDITOrderMgr.IsDeleted = false;
            ent_ndDITOrderMgr.CreatedBy = user;
            ent_ndDITOrderMgr.CreatedOn = DateTime.Now;
            ent_ndDITOrderMgr.LastModifiedBy = user;
            ent_ndDITOrderMgr.LastModifiedOn = DateTime.Now;
            #endregion

            #region DIT_Return_Manager节点
            var ent_ndDITReturnMgr = ent_wf.Nodes.NewEntity();
            ent_ndDITReturnMgr.ID = Guid.NewGuid();
            ent_ndDITReturnMgr.WorkflowID = ent_wf.ID;
            ent_ndDITReturnMgr.Name = "DIT_Return_Manager节点";
            ent_ndDITReturnMgr.Code = "DIT_Return_Manager";
            ent_ndDITReturnMgr.Type = 1;
            ent_ndDITReturnMgr.IsDeleted = false;
            ent_ndDITReturnMgr.CreatedBy = user;
            ent_ndDITReturnMgr.CreatedOn = DateTime.Now;
            ent_ndDITReturnMgr.LastModifiedBy = user;
            ent_ndDITReturnMgr.LastModifiedOn = DateTime.Now;
            #endregion

            #region DIT_Head_of_SupplyChain节点
            var ent_ndDITHeadOfSupplyChain = ent_wf.Nodes.NewEntity();
            ent_ndDITHeadOfSupplyChain.ID = Guid.NewGuid();
            ent_ndDITHeadOfSupplyChain.WorkflowID = ent_wf.ID;
            ent_ndDITHeadOfSupplyChain.Name = "DIT_Head_of_SupplyChain节点";
            ent_ndDITHeadOfSupplyChain.Code = "DIT_Head_of_SupplyChain";
            ent_ndDITHeadOfSupplyChain.Type = 1;
            ent_ndDITHeadOfSupplyChain.IsDeleted = false;
            ent_ndDITHeadOfSupplyChain.CreatedBy = user;
            ent_ndDITHeadOfSupplyChain.CreatedOn = DateTime.Now;
            ent_ndDITHeadOfSupplyChain.LastModifiedBy = user;
            ent_ndDITHeadOfSupplyChain.LastModifiedOn = DateTime.Now;
            #endregion


            #region DOA1节点
            var ent_ndDOA1 = ent_wf.Nodes.NewEntity();
            ent_ndDOA1.ID = Guid.NewGuid();
            ent_ndDOA1.WorkflowID = ent_wf.ID;
            ent_ndDOA1.Name = "DOA1节点";
            ent_ndDOA1.Code = "DOA1";
            ent_ndDOA1.Type = 1;
            ent_ndDOA1.IsDeleted = false;
            ent_ndDOA1.CreatedBy = user;
            ent_ndDOA1.CreatedOn = DateTime.Now;
            ent_ndDOA1.LastModifiedBy = user;
            ent_ndDOA1.LastModifiedOn = DateTime.Now;
            #endregion

            #region DOA2节点
            var ent_ndDOA2 = ent_wf.Nodes.NewEntity();
            ent_ndDOA2.ID = Guid.NewGuid();
            ent_ndDOA2.WorkflowID = ent_wf.ID;
            ent_ndDOA2.Name = "DOA2节点";
            ent_ndDOA2.Code = "DOA2";
            ent_ndDOA2.Type = 1;
            ent_ndDOA2.IsDeleted = false;
            ent_ndDOA2.CreatedBy = user;
            ent_ndDOA2.CreatedOn = DateTime.Now;
            ent_ndDOA2.LastModifiedBy = user;
            ent_ndDOA2.LastModifiedOn = DateTime.Now;
            #endregion

            #region DOA3节点
            var ent_ndDOA3 = ent_wf.Nodes.NewEntity();
            ent_ndDOA3.ID = Guid.NewGuid();
            ent_ndDOA3.WorkflowID = ent_wf.ID;
            ent_ndDOA3.Name = "DOA3节点";
            ent_ndDOA3.Code = "DOA3";
            ent_ndDOA3.Type = 1;
            ent_ndDOA3.IsDeleted = false;
            ent_ndDOA3.CreatedBy = user;
            ent_ndDOA3.CreatedOn = DateTime.Now;
            ent_ndDOA3.LastModifiedBy = user;
            ent_ndDOA3.LastModifiedOn = DateTime.Now;
            #endregion

            #region Distributor节点
            var ent_ndDistributor = ent_wf.Nodes.NewEntity();
            ent_ndDistributor.ID = Guid.NewGuid();
            ent_ndDistributor.WorkflowID = ent_wf.ID;
            ent_ndDistributor.Name = "Distributor节点";
            ent_ndDistributor.Code = "Distributor";
            ent_ndDistributor.Type = 2;
            ent_ndDistributor.IsDeleted = false;
            ent_ndDistributor.CreatedBy = user;
            ent_ndDistributor.CreatedOn = DateTime.Now;
            ent_ndDistributor.LastModifiedBy = user;
            ent_ndDistributor.LastModifiedOn = DateTime.Now;
            #endregion


            #region 结束节点
            var ent_nd9 = ent_wf.Nodes.NewEntity();
            ent_nd9.ID = Guid.NewGuid();
            ent_nd9.WorkflowID = ent_wf.ID;
            ent_nd9.Name = "结束节点";
            ent_nd9.Code = "EndNode";
            ent_nd9.Type = 9;
            ent_nd9.IsDeleted = false;
            ent_nd9.CreatedBy = user;
            ent_nd9.CreatedOn = DateTime.Now;
            ent_nd9.LastModifiedBy = user;
            ent_nd9.LastModifiedOn = DateTime.Now;
            #endregion
            #endregion


            #region Mappings
            #region DIT连线
            var ent_mappingDIT = ent_ndDIT.Parents.NewEntity();
            ent_mappingDIT.ID = Guid.NewGuid();
            ent_mappingDIT.NodeID = ent_ndDIT.ID;
            ent_mappingDIT.WorkflowID = ent_wf.ID;
            ent_mappingDIT.ParentID = ent_ndStart.ID;
            ent_mappingDIT.IsDeleted = false;
            ent_mappingDIT.CreatedBy = user;
            ent_mappingDIT.CreatedOn = DateTime.Now;
            ent_mappingDIT.LastModifiedBy = user;
            ent_mappingDIT.LastModifiedOn = DateTime.Now;
            #endregion
            #region DITOrderMgr连线
            var ent_mappingDITOrderMgr = ent_ndDITOrderMgr.Parents.NewEntity();
            ent_mappingDITOrderMgr.ID = Guid.NewGuid();
            ent_mappingDITOrderMgr.NodeID = ent_ndDITOrderMgr.ID;
            ent_mappingDITOrderMgr.WorkflowID = ent_wf.ID;
            ent_mappingDITOrderMgr.ParentID = ent_ndDIT.ID;
            ent_mappingDITOrderMgr.IsDeleted = false;
            ent_mappingDITOrderMgr.CreatedBy = user;
            ent_mappingDITOrderMgr.CreatedOn = DateTime.Now;
            ent_mappingDITOrderMgr.LastModifiedBy = user;
            ent_mappingDITOrderMgr.LastModifiedOn = DateTime.Now;
            #endregion
            #region DITReturnMgr连线
            var ent_mappingDITReturnMgr = ent_ndDITReturnMgr.Parents.NewEntity();
            ent_mappingDITReturnMgr.ID = Guid.NewGuid();
            ent_mappingDITReturnMgr.NodeID = ent_ndDITReturnMgr.ID;
            ent_mappingDITReturnMgr.WorkflowID = ent_wf.ID;
            ent_mappingDITReturnMgr.ParentID = ent_ndDIT.ID;
            ent_mappingDITReturnMgr.IsDeleted = false;
            ent_mappingDITReturnMgr.CreatedBy = user;
            ent_mappingDITReturnMgr.CreatedOn = DateTime.Now;
            ent_mappingDITReturnMgr.LastModifiedBy = user;
            ent_mappingDITReturnMgr.LastModifiedOn = DateTime.Now;
            #endregion
            #region DITHeadOfSupplyChain连线
            var ent_mappingDITHeadOfSupplyChain1 = ent_ndDITHeadOfSupplyChain.Parents.NewEntity();
            ent_mappingDITHeadOfSupplyChain1.ID = Guid.NewGuid();
            ent_mappingDITHeadOfSupplyChain1.NodeID = ent_ndDITHeadOfSupplyChain.ID;
            ent_mappingDITHeadOfSupplyChain1.WorkflowID = ent_wf.ID;
            ent_mappingDITHeadOfSupplyChain1.ParentID = ent_ndDITOrderMgr.ID;
            ent_mappingDITHeadOfSupplyChain1.IsDeleted = false;
            ent_mappingDITHeadOfSupplyChain1.CreatedBy = user;
            ent_mappingDITHeadOfSupplyChain1.CreatedOn = DateTime.Now;
            ent_mappingDITHeadOfSupplyChain1.LastModifiedBy = user;
            ent_mappingDITHeadOfSupplyChain1.LastModifiedOn = DateTime.Now;

            var ent_mappingDITHeadOfSupplyChain2 = ent_ndDITHeadOfSupplyChain.Parents.NewEntity();
            ent_mappingDITHeadOfSupplyChain2.ID = Guid.NewGuid();
            ent_mappingDITHeadOfSupplyChain2.NodeID = ent_ndDITHeadOfSupplyChain.ID;
            ent_mappingDITHeadOfSupplyChain2.WorkflowID = ent_wf.ID;
            ent_mappingDITHeadOfSupplyChain2.ParentID = ent_ndDITReturnMgr.ID;
            ent_mappingDITHeadOfSupplyChain2.IsDeleted = false;
            ent_mappingDITHeadOfSupplyChain2.CreatedBy = user;
            ent_mappingDITHeadOfSupplyChain2.CreatedOn = DateTime.Now;
            ent_mappingDITHeadOfSupplyChain2.LastModifiedBy = user;
            ent_mappingDITHeadOfSupplyChain2.LastModifiedOn = DateTime.Now;
            #endregion


            #region DOA1连线
            var ent_mappingDOA1 = ent_ndDOA1.Parents.NewEntity();
            ent_mappingDOA1.ID = Guid.NewGuid();
            ent_mappingDOA1.NodeID = ent_ndDOA1.ID;
            ent_mappingDOA1.WorkflowID = ent_wf.ID;
            ent_mappingDOA1.ParentID = ent_ndStart.ID;
            ent_mappingDOA1.IsDeleted = false;
            ent_mappingDOA1.CreatedBy = user;
            ent_mappingDOA1.CreatedOn = DateTime.Now;
            ent_mappingDOA1.LastModifiedBy = user;
            ent_mappingDOA1.LastModifiedOn = DateTime.Now;
            #endregion
            #region DOA2连线
            var ent_mappingDOA2 = ent_ndDOA2.Parents.NewEntity();
            ent_mappingDOA2.ID = Guid.NewGuid();
            ent_mappingDOA2.NodeID = ent_ndDOA2.ID;
            ent_mappingDOA2.WorkflowID = ent_wf.ID;
            ent_mappingDOA2.ParentID = ent_ndDOA1.ID;
            ent_mappingDOA2.IsDeleted = false;
            ent_mappingDOA2.CreatedBy = user;
            ent_mappingDOA2.CreatedOn = DateTime.Now;
            ent_mappingDOA2.LastModifiedBy = user;
            ent_mappingDOA2.LastModifiedOn = DateTime.Now;
            #endregion
            #region DOA3连线
            var ent_mappingDOA3 = ent_ndDOA3.Parents.NewEntity();
            ent_mappingDOA3.ID = Guid.NewGuid();
            ent_mappingDOA3.NodeID = ent_ndDOA3.ID;
            ent_mappingDOA3.WorkflowID = ent_wf.ID;
            ent_mappingDOA3.ParentID = ent_ndDOA2.ID;
            ent_mappingDOA3.IsDeleted = false;
            ent_mappingDOA3.CreatedBy = user;
            ent_mappingDOA3.CreatedOn = DateTime.Now;
            ent_mappingDOA3.LastModifiedBy = user;
            ent_mappingDOA3.LastModifiedOn = DateTime.Now;
            #endregion
            #region Distributor连线
            var ent_mappingDistributor1 = ent_ndDistributor.Parents.NewEntity();
            ent_mappingDistributor1.ID = Guid.NewGuid();
            ent_mappingDistributor1.NodeID = ent_ndDistributor.ID;
            ent_mappingDistributor1.WorkflowID = ent_wf.ID;
            ent_mappingDistributor1.ParentID = ent_ndDOA1.ID;
            ent_mappingDistributor1.IsDeleted = false;
            ent_mappingDistributor1.CreatedBy = user;
            ent_mappingDistributor1.CreatedOn = DateTime.Now;
            ent_mappingDistributor1.LastModifiedBy = user;
            ent_mappingDistributor1.LastModifiedOn = DateTime.Now;

            var ent_mappingDistributor2 = ent_ndDistributor.Parents.NewEntity();
            ent_mappingDistributor2.ID = Guid.NewGuid();
            ent_mappingDistributor2.NodeID = ent_ndDistributor.ID;
            ent_mappingDistributor2.WorkflowID = ent_wf.ID;
            ent_mappingDistributor2.ParentID = ent_ndDOA2.ID;
            ent_mappingDistributor2.IsDeleted = false;
            ent_mappingDistributor2.CreatedBy = user;
            ent_mappingDistributor2.CreatedOn = DateTime.Now;
            ent_mappingDistributor2.LastModifiedBy = user;
            ent_mappingDistributor2.LastModifiedOn = DateTime.Now;

            var ent_mappingDistributor3 = ent_ndDistributor.Parents.NewEntity();
            ent_mappingDistributor3.ID = Guid.NewGuid();
            ent_mappingDistributor3.NodeID = ent_ndDistributor.ID;
            ent_mappingDistributor3.WorkflowID = ent_wf.ID;
            ent_mappingDistributor3.ParentID = ent_ndDOA3.ID;
            ent_mappingDistributor3.IsDeleted = false;
            ent_mappingDistributor3.CreatedBy = user;
            ent_mappingDistributor3.CreatedOn = DateTime.Now;
            ent_mappingDistributor3.LastModifiedBy = user;
            ent_mappingDistributor3.LastModifiedOn = DateTime.Now;
            #endregion
            #endregion


            #region 规则
            #region 发起节点-输出规则
            var ent_ruleStartOutput = ent_ndStart.Rules.NewEntity();
            ent_ruleStartOutput.ID = Guid.NewGuid();
            ent_ruleStartOutput.NodeID = ent_ndStart.ID;
            ent_ruleStartOutput.Type = 2;
            ent_ruleStartOutput.Flag = 99;
            ent_ruleStartOutput.Name = "输出规则";
            ent_ruleStartOutput.Code = "Output";
            ent_ruleStartOutput.Script = @"
            var reason = parameter != null && parameter.Reason != null ? (string)parameter.Reason : string.Empty;
            switch (reason.Trim().ToUpper())
            {
                case ""DIT"":
                    return nodes.Where(n => n.Code == ""DIT"").ToArray();
                case ""NORMAL"":
                    return nodes.Where(n => n.Code == ""DOA1"").ToArray();
                default:
                    return nodes.Where(n => n.Code == ""DOA1"").ToArray();
            }";
            ent_ruleStartOutput.IsDeleted = false;
            ent_ruleStartOutput.CreatedBy = user;
            ent_ruleStartOutput.CreatedOn = DateTime.Now;
            ent_ruleStartOutput.LastModifiedBy = user;
            ent_ruleStartOutput.LastModifiedOn = DateTime.Now;
            ent_ndStart.RL_Output = ent_ruleStartOutput.ID;
            #endregion
            #region DOA1节点-输出规则
            var ent_ruleDOA1Output = ent_ndDOA1.Rules.NewEntity();
            ent_ruleDOA1Output.ID = Guid.NewGuid();
            ent_ruleDOA1Output.NodeID = ent_ndDOA1.ID;
            ent_ruleDOA1Output.Type = 2;
            ent_ruleDOA1Output.Flag = 99;
            ent_ruleDOA1Output.Name = "输出规则";
            ent_ruleDOA1Output.Code = "Output";
            ent_ruleDOA1Output.Script = @"
            switch (status)
            {
                case ""Approved"":
                    var money = parameter != null && parameter.Money != null ? (decimal)parameter.Money : 0M;
                    if (money > 390000M)
                    {
                        return nodes.Where(n => n.Code == ""DOA2"").ToArray();
                    }
                    else
                    {
                        return nodes.Where(n => n.Code == ""Distributor"").ToArray();
                    }
                case ""Rejected"":
                    return nodes.Where(n => n.Type == 0).ToArray();
                default:
                    return null;
            }";
            ent_ruleDOA1Output.IsDeleted = false;
            ent_ruleDOA1Output.CreatedBy = user;
            ent_ruleDOA1Output.CreatedOn = DateTime.Now;
            ent_ruleDOA1Output.LastModifiedBy = user;
            ent_ruleDOA1Output.LastModifiedOn = DateTime.Now;
            ent_ndDOA1.RL_Output = ent_ruleDOA1Output.ID;
            #endregion
            #region DOA2节点-输出规则
            var ent_ruleDOA2Output = ent_ndDOA2.Rules.NewEntity();
            ent_ruleDOA2Output.ID = Guid.NewGuid();
            ent_ruleDOA2Output.NodeID = ent_ndDOA2.ID;
            ent_ruleDOA2Output.Type = 2;
            ent_ruleDOA2Output.Flag = 99;
            ent_ruleDOA2Output.Name = "输出规则";
            ent_ruleDOA2Output.Code = "Output";
            ent_ruleDOA2Output.Script = @"
            switch (status)
            {
                case ""Approved"":
                    var money = parameter != null && parameter.Money != null ? (decimal)parameter.Money : 0M;
                    if (money > 780000M)
                    {
                        return nodes.Where(n => n.Code == ""DOA3"").ToArray();
                    }
                    else
                    {
                        return nodes.Where(n => n.Code == ""Distributor"").ToArray();
                    }
                case ""Rejected"":
                    return nodes.Where(n => n.Type == 0).ToArray();
                default:
                    return null;
            }";
            ent_ruleDOA2Output.IsDeleted = false;
            ent_ruleDOA2Output.CreatedBy = user;
            ent_ruleDOA2Output.CreatedOn = DateTime.Now;
            ent_ruleDOA2Output.LastModifiedBy = user;
            ent_ruleDOA2Output.LastModifiedOn = DateTime.Now;
            ent_ndDOA2.RL_Output = ent_ruleDOA2Output.ID;
            #endregion

            #region End节点-输入规则
            var ent_ruleEndInput = ent_nd9.Rules.NewEntity();
            ent_ruleEndInput.ID = Guid.NewGuid();
            ent_ruleEndInput.NodeID = ent_nd9.ID;
            ent_ruleEndInput.Type = 1;
            ent_ruleEndInput.Flag = 1;
            ent_ruleEndInput.Name = "输入规则";
            ent_ruleEndInput.Code = "Input";
            ent_ruleEndInput.Script = null;
            ent_ruleEndInput.IsDeleted = false;
            ent_ruleEndInput.CreatedBy = user;
            ent_ruleEndInput.CreatedOn = DateTime.Now;
            ent_ruleEndInput.LastModifiedBy = user;
            ent_ruleEndInput.LastModifiedOn = DateTime.Now;
            ent_nd9.RL_Input = ent_ruleEndInput.ID;
            #endregion

            #region DIT_Order_Manager节点-找人规则
            var ent_ruleDITOrderMgrApprovers = ent_ndDITOrderMgr.Rules.NewEntity();
            ent_ruleDITOrderMgrApprovers.ID = Guid.NewGuid();
            ent_ruleDITOrderMgrApprovers.NodeID = ent_ndDITOrderMgr.ID;
            ent_ruleDITOrderMgrApprovers.Type = 0;
            ent_ruleDITOrderMgrApprovers.Flag = 99;
            ent_ruleDITOrderMgrApprovers.Name = "找人规则";
            ent_ruleDITOrderMgrApprovers.Code = "Approvers";
            ent_ruleDITOrderMgrApprovers.Script = @"return new string[]{""DITOrderManager""};";
            ent_ruleDITOrderMgrApprovers.IsDeleted = false;
            ent_ruleDITOrderMgrApprovers.CreatedBy = user;
            ent_ruleDITOrderMgrApprovers.CreatedOn = DateTime.Now;
            ent_ruleDITOrderMgrApprovers.LastModifiedBy = user;
            ent_ruleDITOrderMgrApprovers.LastModifiedOn = DateTime.Now;
            ent_ndDITOrderMgr.RL_Approvers = ent_ruleDITOrderMgrApprovers.ID;
            #endregion
            #region DIT_Return_Manager节点-找人规则
            var ent_ruleDITReturnMgrApprovers = ent_ndDITReturnMgr.Rules.NewEntity();
            ent_ruleDITReturnMgrApprovers.ID = Guid.NewGuid();
            ent_ruleDITReturnMgrApprovers.NodeID = ent_ndDITReturnMgr.ID;
            ent_ruleDITReturnMgrApprovers.Type = 0;
            ent_ruleDITReturnMgrApprovers.Flag = 99;
            ent_ruleDITReturnMgrApprovers.Name = "找人规则";
            ent_ruleDITReturnMgrApprovers.Code = "Approvers";
            ent_ruleDITReturnMgrApprovers.Script = @"return new string[]{""DITReturnManager""};";
            ent_ruleDITReturnMgrApprovers.IsDeleted = false;
            ent_ruleDITReturnMgrApprovers.CreatedBy = user;
            ent_ruleDITReturnMgrApprovers.CreatedOn = DateTime.Now;
            ent_ruleDITReturnMgrApprovers.LastModifiedBy = user;
            ent_ruleDITReturnMgrApprovers.LastModifiedOn = DateTime.Now;
            ent_ndDITReturnMgr.RL_Approvers = ent_ruleDITReturnMgrApprovers.ID;
            #endregion
            #region DIT_Head_of_SupplyChain节点-找人规则
            var ent_ruleDITHeadOfSupplyChainApprovers = ent_ndDITHeadOfSupplyChain.Rules.NewEntity();
            ent_ruleDITHeadOfSupplyChainApprovers.ID = Guid.NewGuid();
            ent_ruleDITHeadOfSupplyChainApprovers.NodeID = ent_ndDITHeadOfSupplyChain.ID;
            ent_ruleDITHeadOfSupplyChainApprovers.Type = 0;
            ent_ruleDITHeadOfSupplyChainApprovers.Flag = 99;
            ent_ruleDITHeadOfSupplyChainApprovers.Name = "找人规则";
            ent_ruleDITHeadOfSupplyChainApprovers.Code = "Approvers";
            ent_ruleDITHeadOfSupplyChainApprovers.Script = @"return new string[]{""DITHeadOfSupplyChain""};";
            ent_ruleDITHeadOfSupplyChainApprovers.IsDeleted = false;
            ent_ruleDITHeadOfSupplyChainApprovers.CreatedBy = user;
            ent_ruleDITHeadOfSupplyChainApprovers.CreatedOn = DateTime.Now;
            ent_ruleDITHeadOfSupplyChainApprovers.LastModifiedBy = user;
            ent_ruleDITHeadOfSupplyChainApprovers.LastModifiedOn = DateTime.Now;
            ent_ndDITHeadOfSupplyChain.RL_Approvers = ent_ruleDITHeadOfSupplyChainApprovers.ID;
            #endregion

            #region DOA1节点-找人规则
            var ent_ruleDOA1Approvers = ent_ndDOA1.Rules.NewEntity();
            ent_ruleDOA1Approvers.ID = Guid.NewGuid();
            ent_ruleDOA1Approvers.NodeID = ent_ndDOA1.ID;
            ent_ruleDOA1Approvers.Type = 0;
            ent_ruleDOA1Approvers.Flag = 99;
            ent_ruleDOA1Approvers.Name = "找人规则";
            ent_ruleDOA1Approvers.Code = "Approvers";
            ent_ruleDOA1Approvers.Script = @"return new string[]{""DOA1""};";
            ent_ruleDOA1Approvers.IsDeleted = false;
            ent_ruleDOA1Approvers.CreatedBy = user;
            ent_ruleDOA1Approvers.CreatedOn = DateTime.Now;
            ent_ruleDOA1Approvers.LastModifiedBy = user;
            ent_ruleDOA1Approvers.LastModifiedOn = DateTime.Now;
            ent_ndDOA1.RL_Approvers = ent_ruleDOA1Approvers.ID;
            #endregion
            #region DOA2节点-找人规则
            var ent_ruleDOA2Approvers = ent_ndDOA2.Rules.NewEntity();
            ent_ruleDOA2Approvers.ID = Guid.NewGuid();
            ent_ruleDOA2Approvers.NodeID = ent_ndDOA2.ID;
            ent_ruleDOA2Approvers.Type = 0;
            ent_ruleDOA2Approvers.Flag = 99;
            ent_ruleDOA2Approvers.Name = "找人规则";
            ent_ruleDOA2Approvers.Code = "Approvers";
            ent_ruleDOA2Approvers.Script = @"return new string[]{""DOA2""};";
            ent_ruleDOA2Approvers.IsDeleted = false;
            ent_ruleDOA2Approvers.CreatedBy = user;
            ent_ruleDOA2Approvers.CreatedOn = DateTime.Now;
            ent_ruleDOA2Approvers.LastModifiedBy = user;
            ent_ruleDOA2Approvers.LastModifiedOn = DateTime.Now;
            ent_ndDOA2.RL_Approvers = ent_ruleDOA2Approvers.ID;
            #endregion
            #region DOA3节点-找人规则
            var ent_ruleDOA3Approvers = ent_ndDOA3.Rules.NewEntity();
            ent_ruleDOA3Approvers.ID = Guid.NewGuid();
            ent_ruleDOA3Approvers.NodeID = ent_ndDOA3.ID;
            ent_ruleDOA3Approvers.Type = 0;
            ent_ruleDOA3Approvers.Flag = 99;
            ent_ruleDOA3Approvers.Name = "找人规则";
            ent_ruleDOA3Approvers.Code = "Approvers";
            ent_ruleDOA3Approvers.Script = @"return new string[]{""DOA3""};";
            ent_ruleDOA3Approvers.IsDeleted = false;
            ent_ruleDOA3Approvers.CreatedBy = user;
            ent_ruleDOA3Approvers.CreatedOn = DateTime.Now;
            ent_ruleDOA3Approvers.LastModifiedBy = user;
            ent_ruleDOA3Approvers.LastModifiedOn = DateTime.Now;
            ent_ndDOA3.RL_Approvers = ent_ruleDOA3Approvers.ID;
            #endregion
            #endregion

            ent_wf.Save(Accessor);
            
        }
    }
}
