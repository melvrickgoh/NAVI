using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;

namespace osm.transshipment_scheduling
{
    public class Ship
    {
        public string ShipName { get; set; }
        public string Destinations { get; set; }
        public string ArrivalTime { get; set; }
        public string Capacity { get; set; }
        public string CurrentGoods { get; set; }
        public string CurrentCapacity { get; set; }
        public string ShipID { get; set; }
    }

    public sealed partial class transshipment_scheduling : SequentialWorkflowActivity
    {
        //class variables
        public Guid createAssignShipToBerthTask_TaskId1 = default(System.Guid);
        public Guid onSchedulingTaskChange_TaskId1 = default(System.Guid);

        //initiating task properties
        public SPWorkflowTaskProperties createAssignShipToBerthTask_TaskProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        //initiating aftertask properties (i.e. properties which a task will have after its execution)
        public SPWorkflowTaskProperties createAssignShipToBerthTask_TaskProperties1_AfterProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties onSchedulingTaskChange_AfterProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties onSchedulingTaskChange_BeforeProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();

        //loop for whether workflow is approved
        public bool isWorkFlowApproved = true;

        public transshipment_scheduling()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        public Ship ship = new Ship();

        private void createTransshipmentSchedulingTask_MethodInvoking(object sender, EventArgs e)
        {
            SPListItem item = workflowProperties.Item;
            ship.ShipName = Convert.ToString(item["Ship Name"]);
            ship.Destinations = Convert.ToString(item["Destinations"]);
            ship.ArrivalTime = Convert.ToString(item["Arrival Timing"]);
            ship.Capacity = Convert.ToString(item["Capacity"]);
            ship.CurrentGoods = Convert.ToString(item["Current Goods"]);
            ship.CurrentCapacity = Convert.ToString(item["Current Capacity"]);
            ship.ShipID = Convert.ToString(item["Ship ID"]);

            item["Workflow URL"] = workflowProperties.ItemUrl;
            item.Update();

            SPGroup group = workflowProperties.Web.Groups["Ship Schedulers"];
            SPFieldUserValue groupValue = new SPFieldUserValue(workflowProperties.Web, group.ID, group.Name);
            createAssignShipToBerthTask_TaskId1 = Guid.NewGuid();
            createAssignShipToBerthTask_TaskProperties1.StartDate = DateTime.Now;
            createAssignShipToBerthTask_TaskProperties1.DueDate = DateTime.Now.AddDays(1.0);
            createAssignShipToBerthTask_TaskProperties1.Title = ship.ShipName + " has arrived. Please assign a berth to it";
            createAssignShipToBerthTask_TaskProperties1.Description = "Please prepare " + ship.ShipName + " for transshipment processing";
            createAssignShipToBerthTask_TaskProperties1.AssignedTo = groupValue.LookupValue;

            createAssignShipToBerthTask_TaskProperties1.ExtendedProperties["Assign Ship"] = "http://win-jau77jek513:1001/Transshipment/default.aspx";
            createAssignShipToBerthTask_TaskProperties1.ExtendedProperties["Ship"] = ship.ShipName;
            LogComment("Creation of ship task");
        }

        private void incomingShipCreation_Invoked(object sender, ExternalDataEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("workflow is invoked");
            System.Diagnostics.Debug.WriteLine(e.InstanceId);
        }

        private void onShipAssignToBerth_Invoked(object sender, ExternalDataEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("change to task is occuring is invoked");

        }

        private Boolean isTaskCompleted()
        {
            //string statusFieldID = this.workflowProperties.TaskList.GetItemById(onSchedulingTaskChange_AfterProperties1.TaskItemId)["Status"].ToString();
            //System.Diagnostics.Debug.WriteLine("loop status > " + statusFieldID);
            // if (statusFieldID.ToLower().Contains("cleared"))
            //{
            //for exiting the loop after completion of task
            return false;
            //}
            //else
            //{
            //for staying in the loop after task change
            //   return true;
            //}
        }

        private void isWorkflowPending(object sender, ConditionalEventArgs e)
        {
            e.Result = isWorkFlowApproved;
        }

        private void checkValue1(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Workflow Approval 1 >>> " + isWorkFlowApproved);
        }

        private void checkValue2(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Workflow Approval 2 >>> " + isWorkFlowApproved);
        }

        private void checkValue3(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Workflow Approval 3 >>> " + isWorkFlowApproved);
        }

        private void LogComment(string logMessage)
        {
            SPWorkflow.CreateHistoryEvent(workflowProperties.Web, this.WorkflowInstanceId, 0, workflowProperties.Web.CurrentUser, new TimeSpan(), "Update", logMessage, string.Empty);
        }

        private void onScheduleChange_Invoked(object sender, ExternalDataEventArgs e)
        {

        }

    }
}
