
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace TeamAllocation.Task_O
{
    public partial class Task_OUserControl : UserControl
    {
        SPWeb WebShips = null;
        SPWeb getSubSiteURL (string subsiteTitle){
            for(int i = 0; i< SPContext.Current.Site.AllWebs.Count;i++)
            {
                if(SPContext.Current.Site.AllWebs[i].Name.Contains(subsiteTitle))
                {
                    return SPContext.Current.Site.AllWebs[i];
                }
            } 
        return null;}
        
        static List<Ship> m_shipList = new List<Ship>();
        static List<string> O_approved = new List<string>();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "";

            if (!Page.IsPostBack)
            {
                try
                {
                    if (WebShips == null)
                        WebShips = getSubSiteURL("IncomingShips");

                    DropDownList1.Items.Add(new ListItem("All", "All"));
                    SPList list = WebShips.Lists["Shipment Schedule"]; //get the list from the site
             
                    foreach (SPListItem item in list.Items)
                    {
                        string O = Convert.ToString(item["O"]);

                        if (!DropDownList1.Items.Contains(new ListItem(O)))
                        {
                            DropDownList1.Items.Add(new ListItem(O, O));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Label1.Text = "Error: " + ex.Message;
                }
            }
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WebShips == null)
                WebShips = getSubSiteURL("IncomingShips");
            SPList list = WebShips.Lists["Shipment Schedule"]; //get the list from the site
            
            string selectedValue = DropDownList1.SelectedItem.Value;
            try
            {
                m_shipList.Clear();
                foreach (SPListItem item in list.Items)
                {
                    if (DropDownList1.SelectedIndex == 0 || 
                        string.Compare(Convert.ToString(item["O"]), selectedValue) == 0)
                    {
                        Ship ship = new Ship();
                        ship.Berth = Convert.ToString(item["Berth ID"]);
                        ship.Title = Convert.ToString(item["Ship Name"]);
                        ship.Atime = Convert.ToString(item["Docking Time"]);
                        ship.Assigned = Convert.ToBoolean(item["Assigned"]);
                        ship.F = Convert.ToString(item["F"]);
                        ship.O = Convert.ToString(item["O"]);
                        ship.S = Convert.ToString(item["S"]);
                        ship.F_Approved = Convert.ToBoolean(item["F_Approved"]);
                        ship.O_Approved = Convert.ToBoolean(item["O_Approved"]);
                        ship.S_Approved = Convert.ToBoolean(item["S_Approved"]);

                        m_shipList.Add(ship);
                    } 
                  
                }

                GridView1.DataSource = m_shipList;
                GridView1.DataBind();

            }
            catch (Exception ex)
            {
                Label1.Text = "Error: " + ex.Message;
            }
        }
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label2.Text = "";
            
            Ship ship = m_shipList[GridView1.SelectedIndex];

            Label2.Text = "Incoming ship: " + ship.Title + ". Docking time: " + ship.Atime;
                
            if (ship.F != null && ship.F.Length != 0 && !O_approved.Contains(ship.F))
                O_approved.Add(ship.F);
            
            if (GridView1.SelectedIndex >= 0)
            {
                if (ship.O_Approved)
                    return;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (GridView1.SelectedIndex != -1)
            {
                Ship ship = m_shipList[GridView1.SelectedIndex];

                if (RadioButton1.Checked)
                    ship.S_Approved = true;

                if (RadioButton2.Checked)
                    ship.S_Approved = false;

                m_shipList[GridView1.SelectedIndex] = ship;
                
                GridView1.DataSource = m_shipList;
                GridView1.DataBind();

                if (WebShips == null)
                    WebShips = getSubSiteURL("IncomingShips");//get the site
                SPList list = WebShips.Lists["Shipment Schedule"]; //get the list from the site
             
                foreach (SPListItem item in list.Items)
                {

                    string title = Convert.ToString(item["Ship Name"]);
                    if (string.Compare(Convert.ToString(item["S"]), DropDownList1.SelectedItem.Value) == 0 &&
                        string.Compare(title, ship.Title) == 0
                        )
                    {

                        item["O_Approved"] = ship.S_Approved;
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
        public bool F_Approved { get; set; }
        public bool O_Approved { get; set; }
        public bool S_Approved { get; set; }

    }
}
