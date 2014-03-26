using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace TeamAllocation.HR
{

    public partial class HRUserControl : UserControl
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
        static List<string> FPeopleAssigned = new List<string>();
        static List<string> OPeopleAssigned = new List<string>();
        static List<string> SPeopleAssigned = new List<string>();
        static string[] FPeople = new string[] { "Siu Ngee", "YG", "Leon", "Jasmine" };
        static string[] OPeople = new string[] { "Dex", "Law", "FH", "Mel" };
        static string[] SPeople = new string[] { "YY", "WT", "Delphine", "YC" };

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
                    SPList list = WebShips.Lists["Ships"];


                    foreach (SPListItem item in list.Items)
                    {
                        string time = Convert.ToString(item["Arrival Time"]);

                        if (!DropDownList1.Items.Contains(new ListItem(time)))
                        {
                            DropDownList1.Items.Add(new ListItem(time, time));
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
                WebShips = getSubSiteURL("IncomingShips");//get the site
            SPList list = WebShips.Lists["Ships"];//get the list from the site
            string selectedValue = DropDownList1.SelectedItem.Value;
            try
            {
                m_shipList.Clear();
                foreach (SPListItem item in list.Items)
                {
                    if (DropDownList1.SelectedIndex == 0 || string.Compare(Convert.ToString(item["Arrival Time"]), selectedValue) == 0)
                    {
                        Ship ship = new Ship();
                        ship.Berth = Convert.ToString(item["Berth"]);
                        ship.Title = Convert.ToString(item["Title"]);
                        ship.Atime = Convert.ToString(item["Arrival Time"]);
                        ship.Assigned = Convert.ToBoolean(item["Assigned"]);
                        ship.F = Convert.ToString(item["F"]);
                        ship.O = Convert.ToString(item["O"]);
                        ship.S = Convert.ToString(item["S"]);
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
            FPeopleAssigned.Clear();
            OPeopleAssigned.Clear();
            SPeopleAssigned.Clear();
            foreach (Ship ship in m_shipList)
            {
                if (ship.F != null && ship.F.Length != 0 && !FPeopleAssigned.Contains(ship.F))
                    FPeopleAssigned.Add(ship.F);
                if (ship.O != null && ship.O.Length != 0 && !OPeopleAssigned.Contains(ship.O))
                    OPeopleAssigned.Add(ship.O);
                if (ship.S != null && ship.S.Length != 0 && !SPeopleAssigned.Contains(ship.S))
                    SPeopleAssigned.Add(ship.S);
            }
            RadioButtonList1.Items.Clear();
            RadioButtonList2.Items.Clear();
            RadioButtonList3.Items.Clear();

            if (GridView1.SelectedIndex >= 0)
            {
                Ship ship = m_shipList[GridView1.SelectedIndex];
                if (ship.Assigned)
                    return;
            }

            for (int i = 0; i < 4; i++)
            {
                if (!FPeopleAssigned.Contains(FPeople[i]))
                    RadioButtonList1.Items.Add(FPeople[i]);
                if (!OPeopleAssigned.Contains(OPeople[i]))
                    RadioButtonList2.Items.Add(OPeople[i]);
                if (!SPeopleAssigned.Contains(SPeople[i]))
                    RadioButtonList3.Items.Add(SPeople[i]);
            }
        }

        protected void AssignTeam_Click(object sender, EventArgs e)
        {
            if (GridView1.SelectedIndex != -1)
            {
                Ship ship = m_shipList[GridView1.SelectedIndex];
                ship.Assigned = true;
                ship.F = RadioButtonList1.SelectedValue;
                ship.O = RadioButtonList2.SelectedValue;
                ship.S = RadioButtonList3.SelectedValue;
                m_shipList[GridView1.SelectedIndex] = ship;

                //get the data on the selected radio button for FOS
                //update the data to the FOS columns onto the 'incoming ships' list
                
                GridView1.DataSource = m_shipList;
                GridView1.DataBind();
                if (WebShips == null)
                    WebShips = getSubSiteURL("IncomingShips");//get the site
                SPList list = WebShips.Lists["Ships"];//get the list from the site
                foreach (SPListItem item in list.Items)
                {
                    string title = Convert.ToString(item["Title"]);
                    if (string.Compare(Convert.ToString(item["Arrival Time"]), DropDownList1.SelectedItem.Value) == 0 &&
                        string.Compare(title, ship.Title) == 0
                        )
                    {
                        item["Assigned"] = ship.Assigned;
                        item["F"] = ship.F;
                        item["O"] = ship.O;
                        item["S"] = ship.S;
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
