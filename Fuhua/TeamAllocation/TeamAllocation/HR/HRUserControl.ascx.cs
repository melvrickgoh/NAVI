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
        SPWeb Finance = null;
        SPWeb Operations = null;
        SPWeb Safety = null;

        static List<Ship> m_shipList = new List<Ship>(); //list of ships
        static List<string> FPeopleAssigned = new List<string>(); //list of people assigned
        static List<string> OPeopleAssigned = new List<string>();
        static List<string> SPeopleAssigned = new List<string>();
        
        //static string[] FPeople = new string[] { "Siu Ngee", "YG", "Leon", "Jasmine" }; //list of people in each department
        //static string[] OPeople = new string[] { "Dex", "Law", "FH", "Mel" };
        //static string[] SPeople = new string[] { "YY", "WT", "Delphine", "YC" };

        static string[] FPeople;
        static string[] OPeople;
        static string[] SPeople;

        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "";

            if (Finance == null)
                Finance = getSubSiteURL("Finance"); //get the site
            SPList Flist = Finance.Lists["HR"]; // get list from site
            
            if (Operations == null)
                Operations = getSubSiteURL("Ops"); //get the site
            SPList Olist = Operations.Lists["HR"]; // get list from site
            
            if (Safety == null)
                Safety = getSubSiteURL("Safety"); //get the site
            SPList Slist = Safety.Lists["HR"]; // get list from site

            FPeople = new string[Flist.ItemCount];
            for (int i = 0; i < Flist.ItemCount; i++)
                FPeople[i] = Convert.ToString(Flist.Items[i]["Title"]);
            OPeople = new string[Olist.ItemCount];
            for (int i = 0; i < Olist.ItemCount; i++)
                OPeople[i] = Convert.ToString(Olist.Items[i]["Title"]);
            SPeople = new string[Slist.ItemCount];
            for (int i = 0; i < Slist.ItemCount; i++)
                SPeople[i] = Convert.ToString(Slist.Items[i]["Title"]);


            if (!Page.IsPostBack)
            {
                try
                {
                    
                    DropDownList1.Items.Add(new ListItem("All", "All")); //add items into dropdown list
                    //SPList list = WebShips.Lists["Shipment Schedule"]; //get list from site
                    SPList list = SPContext.Current.Web.Lists["Shipment Schedule"]; //at parent site: transshipment


                    foreach (SPListItem item in list.Items)
                    {
                        string time = Convert.ToString(item["Docking Time"]); //get column from list

                        if (!DropDownList1.Items.Contains(new ListItem(time)))
                        {
                            DropDownList1.Items.Add(new ListItem(time, time)); //add items into dropdown from list
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
            //if (WebShips == null)
              //  WebShips = getSubSiteURL("IncomingShips"); //get the site

           
            SPList list = SPContext.Current.Web.Lists["Shipment Schedule"]; //at parent site: transshipment
            string selectedValue = DropDownList1.SelectedItem.Value;
            
            try
            {
                m_shipList.Clear();
                foreach (SPListItem item in list.Items)
                {
                    if (DropDownList1.SelectedIndex == 0 ||
                        string.Compare(Convert.ToString(item["Docking Time"]), selectedValue) == 0)
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

                        int count = m_shipList.Count;
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
                //for each ship at selected time
                //if F is not empty
                //if F is not empty
                //if list of peopel assigned for F contains 

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

            if (GridView1.SelectedIndex >= 0) //if selected ship has been assigned, do nothing
            {
                Ship ship = m_shipList[GridView1.SelectedIndex];
                if (ship.Assigned)
                    return;
            }

            for (int i = 0; i < FPeople.Length; i++) //display unassigned people at radio button for selection
            {
                if (!FPeopleAssigned.Contains(FPeople[i]))
                    RadioButtonList1.Items.Add(FPeople[i]);
            }
            for (int i = 0; i < OPeople.Length; i++) //display unassigned people at radio button for selection
            {
                if (!OPeopleAssigned.Contains(OPeople[i]))
                    RadioButtonList2.Items.Add(OPeople[i]);
            }
            for (int i = 0; i < SPeople.Length; i++) //display unassigned people at radio button for selection
            {
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
                //if (WebShips == null)
                  //  WebShips = getSubSiteURL("IncomingShips");//get the site
                SPList list = SPContext.Current.Web.Lists["Shipment Schedule"]; //at parent site: transshipment

                foreach (SPListItem item in list.Items)
                {
                    string title = Convert.ToString(item["Ship Name"]);
                    if (string.Compare(Convert.ToString(item["Docking Time"]), DropDownList1.SelectedItem.Value) == 0 &&
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
