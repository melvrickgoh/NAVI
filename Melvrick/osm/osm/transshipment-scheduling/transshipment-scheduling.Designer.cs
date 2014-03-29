using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace osm.transshipment_scheduling
{
    public sealed partial class transshipment_scheduling
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind4 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind5 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind7 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken2 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind6 = new System.Workflow.ComponentModel.ActivityBind();
            this.logToHistoryListActivity1 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.onScheduleChange = new Microsoft.SharePoint.WorkflowActions.OnTaskChanged();
            this.createTransshipmentSchedulingTask = new Microsoft.SharePoint.WorkflowActions.CreateTask();
            this.incomingShipCreation = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // logToHistoryListActivity1
            // 
            this.logToHistoryListActivity1.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity1.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity1.HistoryDescription = "";
            this.logToHistoryListActivity1.HistoryOutcome = "";
            this.logToHistoryListActivity1.Name = "logToHistoryListActivity1";
            this.logToHistoryListActivity1.OtherData = "";
            this.logToHistoryListActivity1.UserId = -1;
            // 
            // onScheduleChange
            // 
            activitybind1.Name = "transshipment_scheduling";
            activitybind1.Path = "onSchedulingTaskChange_AfterProperties1";
            activitybind2.Name = "transshipment_scheduling";
            activitybind2.Path = "onSchedulingTaskChange_BeforeProperties1";
            correlationtoken1.Name = "TransshipmentToken";
            correlationtoken1.OwnerActivityName = "transshipment_scheduling";
            this.onScheduleChange.CorrelationToken = correlationtoken1;
            this.onScheduleChange.Executor = null;
            this.onScheduleChange.Name = "onScheduleChange";
            activitybind3.Name = "transshipment_scheduling";
            activitybind3.Path = "onSchedulingTaskChange_TaskId1";
            this.onScheduleChange.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onScheduleChange_Invoked);
            this.onScheduleChange.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.AfterPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            this.onScheduleChange.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.BeforePropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.onScheduleChange.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.TaskIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            // 
            // createTransshipmentSchedulingTask
            // 
            this.createTransshipmentSchedulingTask.CorrelationToken = correlationtoken1;
            this.createTransshipmentSchedulingTask.ListItemId = -1;
            this.createTransshipmentSchedulingTask.Name = "createTransshipmentSchedulingTask";
            this.createTransshipmentSchedulingTask.SpecialPermissions = null;
            activitybind4.Name = "transshipment_scheduling";
            activitybind4.Path = "createAssignShipToBerthTask_TaskId1";
            activitybind5.Name = "transshipment_scheduling";
            activitybind5.Path = "createAssignShipToBerthTask_TaskProperties1";
            this.createTransshipmentSchedulingTask.MethodInvoking += new System.EventHandler(this.createTransshipmentSchedulingTask_MethodInvoking);
            this.createTransshipmentSchedulingTask.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind5)));
            this.createTransshipmentSchedulingTask.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind4)));
            activitybind7.Name = "transshipment_scheduling";
            activitybind7.Path = "workflowId";
            // 
            // incomingShipCreation
            // 
            correlationtoken2.Name = "workflowToken";
            correlationtoken2.OwnerActivityName = "transshipment_scheduling";
            this.incomingShipCreation.CorrelationToken = correlationtoken2;
            this.incomingShipCreation.EventName = "OnWorkflowActivated";
            this.incomingShipCreation.Name = "incomingShipCreation";
            activitybind6.Name = "transshipment_scheduling";
            activitybind6.Path = "workflowProperties";
            this.incomingShipCreation.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.incomingShipCreation_Invoked);
            this.incomingShipCreation.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind7)));
            this.incomingShipCreation.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind6)));
            // 
            // transshipment_scheduling
            // 
            this.Activities.Add(this.incomingShipCreation);
            this.Activities.Add(this.createTransshipmentSchedulingTask);
            this.Activities.Add(this.onScheduleChange);
            this.Activities.Add(this.logToHistoryListActivity1);
            this.Name = "transshipment_scheduling";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.SharePoint.WorkflowActions.OnTaskChanged onScheduleChange;

        private Microsoft.SharePoint.WorkflowActions.CreateTask createTransshipmentSchedulingTask;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity1;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated incomingShipCreation;










































































    }
}
