
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
            LabelS1.Text = "";

            if (!Page.IsPostBack)
            {
                try
                {
                    if (WebShips == null)
                        WebShips = getSubSiteURL("IncomingShips");

                    DropDownListS.Items.Add(new ListItem("All", "All"));
                    SPList list = WebShips.Lists["Ships"];


                    foreach (SPListItem item in list.Items)
                    {
                        string S = Convert.ToString(item["S"]);

                        if (!DropDownListS.Items.Contains(new ListItem(S)))
                        {
                            DropDownListS.Items.Add(new ListItem(S, S));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LabelS1.Text = "Error: " + ex.Message;
                }
            }
        }
        protected void DropDownListS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WebShips == null)
                WebShips = getSubSiteURL("IncomingShips");
            SPList list = WebShips.Lists["Ships"];
            string selectedValue = DropDownListS.SelectedItem.Value;
            try
            {
                m_shipList.Clear();
                foreach (SPListItem item in list.Items)
                {
                    if (DropDownListS.SelectedIndex == 0 || string.Compare(Convert.ToString(item["S"]), selectedValue) == 0)
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

                GridViewS.DataSource = m_shipList;
                GridViewS.DataBind();

            }
            catch (Exception ex)
            {
                LabelS1.Text = "Error: " + ex.Message;
            }
        }
        protected void GridViewS_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelS2.Text = "";

            Ship ship = m_shipList[GridViewS.SelectedIndex];

            LabelS2.Text = "Incoming ship: " + ship.Title + ". Arrival time: " + ship.Atime;

            if (ship.F != null && ship.F.Length != 0 && !S_approved.Contains(ship.F))
                S_approved.Add(ship.F);

            if (GridViewS.SelectedIndex >= 0)
            {
                if (ship.S_Approved)
                    return;
            }
        }

        protected void ButtonS_Click(object sender, EventArgs e)
        {


            if (GridViewS.SelectedIndex != -1)
            {
                Ship ship = m_shipList[GridViewS.SelectedIndex];

                if (RadioButtonS1.Checked)
                    RadioButtonS2.Checked = false;
                    ship.S_Approved = true;
                if (RadioButtonS2.Checked)
                    RadioButtonS1.Checked = false;
                    ship.S_Approved = false;
                m_shipList[GridViewS.SelectedIndex] = ship;

                GridViewS.DataSource = m_shipList;
                GridViewS.DataBind();

                if (WebShips == null)
                    WebShips = getSubSiteURL("IncomingShips");//get the site
                SPList list = WebShips.Lists["Ships"];//get the list from the site

                foreach (SPListItem item in list.Items)
                {
                    string title = Convert.ToString(item["Title"]);
                    if (string.Compare(Convert.ToString(item["S"]), DropDownListS.SelectedItem.Value) == 0 &&
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
        public bool F_Approved { get; set; }
        public bool O_Approved { get; set; }
        public bool S_Approved { get; set; }

    }
}
