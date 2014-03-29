using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.Collections;

using System.Diagnostics;


namespace TeamAllocation.Tasks
{
    public partial class TasksUserControl : UserControl
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
        static List<string> F_approved = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            LabelF1.Text = "";

            if (!Page.IsPostBack)
            {
                try
                {
                    if (WebShips == null)
                        WebShips = SPContext.Current.Site.RootWeb;
                    //  WebShips = getSubSiteURL("IncomingShips"); //CHANGE THIS: get the parent site
            
                    DropDownListF.Items.Add(new ListItem("All", "All"));
                    SPList list = WebShips.Lists["Shipment Schedule"]; //get the list from the site
             

                    foreach (SPListItem item in list.Items)
                    {
                        string F = Convert.ToString(item["F"]);

                        if (!DropDownListF.Items.Contains(new ListItem(F)))
                        {
                            DropDownListF.Items.Add(new ListItem(F, F));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LabelF1.Text = "Error: " + ex.Message;
                }
            }
        }
        protected void DropDownListF_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WebShips == null)
                WebShips = SPContext.Current.Site.RootWeb;
            //  WebShips = getSubSiteURL("IncomingShips"); //CHANGE THIS: get the parent site
            
            SPList list = WebShips.Lists["Shipment Schedule"]; //get the list from the site
            
            string selectedValue = DropDownListF.SelectedItem.Value;
            try
            {
                m_shipList.Clear();
                foreach (SPListItem item in list.Items)
                {
                    if (DropDownListF.SelectedIndex == 0 || string.Compare(Convert.ToString(item["F"]), selectedValue) == 0)
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

                GridViewF.DataSource = m_shipList;
                GridViewF.DataBind();

            }
            catch (Exception ex)
            {
                LabelF1.Text = "Error: " + ex.Message;
            }
        }
        protected void GridViewF_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelF2.Text = "";

            Ship ship = m_shipList[GridViewF.SelectedIndex];

            LabelF2.Text = "Incoming ship: " + ship.Title + ". Docking time: " + ship.Atime;

          
            if (GridViewF.SelectedIndex >= 0)
            {
                if (ship.F_Approved.CompareTo("Yes") == 0)
                    return;
            }
        }

        protected void ButtonS_Click(object sender, EventArgs e)
        {


            if (GridViewF.SelectedIndex != -1)
            {
                Ship ship = m_shipList[GridViewF.SelectedIndex];

                if (RadioButtonF1.Checked)
                ship.F_Approved = "Yes";

                if (RadioButtonF2.Checked)
                ship.F_Approved = "No";
                
                m_shipList[GridViewF.SelectedIndex] = ship;

                GridViewF.DataSource = m_shipList;
                GridViewF.DataBind();

                if (WebShips == null)
                    WebShips = SPContext.Current.Site.RootWeb;
                //  WebShips = getSubSiteURL("IncomingShips"); //CHANGE THIS: get the parent site
                
                SPList list = WebShips.Lists["Shipment Schedule"]; //get the list from the site
             
                foreach (SPListItem item in list.Items)
                {
                    string title = Convert.ToString(item["Ship Name"]);
                    if (string.Compare(Convert.ToString(item["F"]), DropDownListF.SelectedItem.Value) == 0 &&
                        string.Compare(title, ship.Title) == 0
                        )
                    {
                        item["F_Approved"] = ship.F_Approved;
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
