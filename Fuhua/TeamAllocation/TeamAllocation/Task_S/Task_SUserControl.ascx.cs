
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace TeamAllocation.Task_S
{
    public partial class Task_SUserControl : UserControl
    {
        SPWeb WebShips = null;
        SPWeb getSubSiteURL(string subsiteTitle)
        {
            for (int i = 0; i < SPContext.Current.Site.AllWebs.Count; i++)
            {
                if (SPContext.Current.Site.AllWebs[i].Name.Contains(subsiteTitle))
                {
                    return SPContext.Current.Site.AllWebs[i];
                }
            }
            return null;
        }

        static List<Ship> m_shipList = new List<Ship>();
        static List<string> S_approved = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Label_dropdown_errormsg.Text = "";

            if (!Page.IsPostBack)
            {
                try
                {
                   if (WebShips == null)
                       WebShips = SPContext.Current.Site.RootWeb;
                     //   WebShips = getSubSiteURL("IncomingShips"); //CHANGE THIS: get the parent site

                   DropDownList_safety_name.Items.Add(new ListItem("All", "All")); //add items into dropdown list
                    SPList list = WebShips.Lists["Shipment Schedule"]; // get list from site
                    

                    foreach (SPListItem item in list.Items)
                    {
                        string S = Convert.ToString(item["S"]);

                        if (!DropDownList_safety_name.Items.Contains(new ListItem(S)))
                        {
                            DropDownList_safety_name.Items.Add(new ListItem(S, S));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Label_dropdown_errormsg.Text = "Error: " + ex.Message;
                }
            }
        }
        protected void DropDownListS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WebShips == null)
                //WebShips = getSubSiteURL("IncomingShips"); //get the site //CHANGE THIS: get the parent site
                WebShips = SPContext.Current.Site.RootWeb;
            
            SPList list = WebShips.Lists["Shipment Schedule"]; //get the list from the site
            string selectedValue = DropDownList_safety_name.SelectedItem.Value;

            try
            {
                m_shipList.Clear();
                foreach (SPListItem item in list.Items)
                {

                    if (DropDownList_safety_name.SelectedIndex == 0 ||
                        string.Compare(Convert.ToString(item["S"]), selectedValue) == 0)
                    {
                        Ship ship = new Ship();
                        ship.Berth = Convert.ToString(item["Berth ID"]);
                        ship.Title = Convert.ToString(item["Ship Name"]);
                        ship.Atime = Convert.ToString(item["Docking Time"]);
                        ship.Assigned = Convert.ToBoolean(item["Assigned"]);
                        ship.F = Convert.ToString(item["F"]);
                        ship.O = Convert.ToString(item["O"]);
                        ship.S = Convert.ToString(item["S"]);
                        ship.F_Approved = Convert.ToString(item["F_Approved"]);

                        ship.O_Approved = Convert.ToString(item["O_Approved"]);

                        ship.S_Approved = Convert.ToString(item["S_Approved"]);
                        
                        m_shipList.Add(ship);
                    } 
                  
                }

                GridView_safety_tasklist.DataSource = m_shipList;
                GridView_safety_tasklist.DataBind();

            }
            catch (Exception ex)
            {
                Label_dropdown_errormsg.Text = "Error: " + ex.Message;
            }
        }
        protected void GridViewS_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label_safety_selected_task.Text = "";

            Ship ship = m_shipList[GridView_safety_tasklist.SelectedIndex];

            Label_safety_selected_task.Text = "Incoming ship: " + ship.Title + ". Docking time: " + ship.Atime;

            if (ship.F != null && ship.F.Length != 0 && !S_approved.Contains(ship.F))
                S_approved.Add(ship.F);

            if (GridView_safety_tasklist.SelectedIndex >= 0)
            {
                if (ship.S_Approved.CompareTo("Yes") == 0)
                    return;
            }
        }

        protected void ButtonS_Click(object sender, EventArgs e)
        {
            //THE RADIO BUTTON: SHOULD NOT BE ABLE TOMM CLICK ON BOTH

            if (GridView_safety_tasklist.SelectedIndex != -1)
            {
                Ship ship = m_shipList[GridView_safety_tasklist.SelectedIndex];

                if (RadioButton_safety_approve.Checked)
                    ship.S_Approved = "Yes"; //CHANGE THIS TO STRING: APPROVED

                if (RadioButton_safety_reject.Checked)
                    ship.S_Approved = "NO"; //CHANGE THIS TO STRING: REJECTED

                m_shipList[GridView_safety_tasklist.SelectedIndex] = ship;

                GridView_safety_tasklist.DataSource = m_shipList;
                GridView_safety_tasklist.DataBind();

                if (WebShips == null)
                    //WebShips = getSubSiteURL("IncomingShips"); //get the site //CHANGE THIS: get the parent site
                      WebShips = SPContext.Current.Site.RootWeb;
                SPList list = WebShips.Lists["Shipment Schedule"]; //get the list from the site
             
                foreach (SPListItem item in list.Items)
                {
                    string title = Convert.ToString(item["Ship Name"]);
                    if (string.Compare(Convert.ToString(item["S"]), DropDownList_safety_name.SelectedItem.Value) == 0 &&
                        string.Compare(title, ship.Title) == 0
                        )
                    {

                        item["S_Approved"] = ship.S_Approved;
                        item.Update();
                        Debug.WriteLine("ITEM UPDATED");
                    }
                }
            }
        }
    }
    class Ship
    {

        public string Berth { get; set; }
        public string Title { get; set; }
        public string Atime { get; set; }
        public bool Assigned { get; set; }
        public string F { get; set; }
        public string O { get; set; }
        public string S { get; set; }
        public string F_Approved { get; set; }
        public string O_Approved { get; set; }
        public string S_Approved { get; set; }
    }
}
