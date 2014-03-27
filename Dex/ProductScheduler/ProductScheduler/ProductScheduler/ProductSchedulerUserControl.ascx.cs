using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections;
using System.Collections.Generic;

namespace ProductScheduler.ProductScheduler
{
    public partial class ProductSchedulerUserControl : UserControl
    {
        SPWeb objweb = null;

        SPWeb GetSubSiteURL(string subsiteTitle)
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

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            if (!IsPostBack)
            {
                try
                {
                    SPList spShipLists = SPContext.Current.Web.Lists["Shipment Schedule"];

                    ddlDockingTime.Items.Add(new ListItem("All", "All"));

                    List<Ship> ShipList = new List<Ship>();

                    foreach (SPListItem item in spShipLists.Items)
                    {
                        Ship s = new Ship();
                        s.ShipID = Convert.ToString(item["Ship ID"]);
                        s.ShipName = Convert.ToString(item["Ship Name"]);
                        s.DockingTime = Convert.ToString(item["Docking Time"]);
                        s.Status = Convert.ToString(item["Status"]);
                        s.ShipmentID = Convert.ToString(item["Shipment ID"]);
                        s.CurrentCapacity = Convert.ToString(item["Current Capacity"]);
                        s.NewCapacity = Convert.ToString(item["New Capacity"]);

                        ShipList.Add(s);

                        //Populate GridView for Display All Ships
                        displayGV.DataSource = ShipList;
                        displayGV.DataBind();

                        //Populate the Docking Time dropdownlist with ships that already have been assigned.
                        //Populate dropdown list with list of docking timing
                        string sTime = s.DockingTime;
                        if (!ddlDockingTime.Items.Contains(new ListItem(sTime)))
                        {
                            string confirmed = s.Status;
                            if (confirmed == "New" || confirmed == "Missed") //New ships or ships that have missed the schedule.
                            {
                                ddlDockingTime.Items.Add(new ListItem(sTime, sTime));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                }
            }
        }

        protected void ddlDockingTime_SelectedIndexChanged1(object sender, EventArgs e)
        {
            SPList spShipLists = SPContext.Current.Web.Lists["Shipment Schedule"];
            string selectedDockingTiming = ddlDockingTime.SelectedItem.Value;

            try
            {
                List<Ship> ShipList = new List<Ship>();

                foreach (SPListItem item in spShipLists.Items)
                {
                    //Match destination && Status is New/Missed
                    if (ddlDockingTime.SelectedIndex == 0 ||
                        selectedDockingTiming.Equals(Convert.ToString(item["Docking Time"]))
                        && (Convert.ToString(item["Status"]) == "New" || Convert.ToString(item["Status"]) == "Missed"))
                    {
                        Ship s = new Ship();
                        s.ShipID = Convert.ToString(item["Ship ID"]);
                        s.ShipName = Convert.ToString(item["Ship Name"]);
                        s.DockingTime = Convert.ToString(item["Docking Time"]);
                        s.Status = Convert.ToString(item["Status"]);
                        s.ShipmentID = Convert.ToString(item["Shipment ID"]);
                        s.CurrentCapacity = Convert.ToString(item["Current Capacity"]);
                        s.NewCapacity = Convert.ToString(item["New Capacity"]);

                        ShipList.Add(s);
                    }
                }
                displayGV.DataSource = ShipList;
                displayGV.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void chkShip_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkStatus = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkStatus.NamingContainer;
        }

        protected void chkHeader_CheckedChanged1(object sender, EventArgs e)
        {
            CheckBox chkheader = (CheckBox)displayGV.HeaderRow.FindControl("chkHeader");

            foreach (GridViewRow row in displayGV.Rows)
            {
                CheckBox chkRow = (CheckBox)row.FindControl("chkShip");

                if (chkheader.Checked == true)
                {
                    chkRow.Checked = true;
                }
                else
                {
                    chkRow.Checked = false;
                }
            }
        }

        protected void btnScheduleProduct_Click(object sender, EventArgs e)
        {
            //Retrieve incoming ship list that contains destinations
            SPList spShipLists = SPContext.Current.Web.Lists["Incoming Ships"];
            SPList spSelectedShipLists = SPContext.Current.Web.Lists["Shipment Schedule"];

            //For updating/adding new shipment
            SPWeb warehouseweb = GetSubSiteURL("Warehouse");
            SPListItemCollection listItems = warehouseweb.Lists["Shipment Details"].Items;
            SPList spShipmentList = warehouseweb.Lists["Shipment Details"];

            Boolean anySelected = false;
            List<string> selectShipNames = new List<string>();
            List<SelectedShip> selectedShipList = new List<SelectedShip>();

            //Loop thru the list of selected ships to archive destination of each ship for comparison
            foreach (GridViewRow row in displayGV.Rows)
            {
                CheckBox chkRow = (CheckBox)row.FindControl("chkShip");
                if (chkRow.Checked == true) //Ship is selected.
                {
                    anySelected = true; //Trigger to indicate there is something selected
                    string shipID = row.Cells[1].Text;
                    string shipName = row.Cells[2].Text;
                    string destinations = "";    //Store retrieved list of destinations of each ship
                    string[] destinationArray;

                    foreach (SPListItem item in spShipLists.Items) //For each ships, retrieve its destination from incoming ship list
                    {
                        if (shipID.Equals(Convert.ToString(item["Ship ID"])))
                        {
                            destinations = Convert.ToString(item["Destinations"]);
                            destinationArray = destinations.Split(','); //Split destinations ','

                            Hashtable destinationHashTable = new Hashtable();   //Create each ship HashTable [Destination,index number] for quick check later
                            int index = 0;

                            foreach (string d in destinationArray)
                            {
                                destinationHashTable.Add(d, index);
                                index++;
                            }//end of destination list of each ship

                            foreach (SPListItem selectedShip in spSelectedShipLists.Items)
                            {
                                if (shipID.Equals(Convert.ToString(selectedShip["Ship ID"])))
                                {
                                    SelectedShip sS = new SelectedShip();
                                    sS.ShipID = Convert.ToString(selectedShip["Ship ID"]);
                                    sS.ShipName = Convert.ToString(selectedShip["Ship Name"]);
                                    sS.DockingTime = Convert.ToString(selectedShip["Docking Time"]);
                                    sS.Status = Convert.ToString(selectedShip["Status"]);
                                    sS.ShipmentID = Convert.ToString(selectedShip["Shipment ID"]);
                                    sS.CurrentCapacity = Convert.ToString(selectedShip["Current Capacity"]);
                                    sS.NewCapacity = Convert.ToString(selectedShip["New Capacity"]);
                                    sS.Destinations = destinationHashTable;
                                    selectedShipList.Add(sS);
                                }
                            }
                        }
                    }
                }
            }

            if (!anySelected)
            {
                lblMessage.Text = "No ship was selected. Please select at least 1 to schedule products onto the ship.";
            }
            else
            {
                //Retrieve the list of partner freight product list
                List<Product> productList = getProductList();

                if (productList != null)    //Proceeds only if there are items still waiting to be shipped
                {
                    //Compare checks against partner freight product list
                    foreach (Product p in productList)
                    {
                        //Get the ship that will reach that destination fastest & has capacity
                        SelectedShip chosenShip = getBestShipChoice(p, selectedShipList);

                        if (chosenShip == null)
                        {
                            //do nothing as no ship selected is going to the destination the product is scheduled for.
                        }
                        else
                        {
                            /*Best fit of ship found!
                            **store the product to ship
                            **Update ships new current capacity into the list of ships
                            */
                            for (int i = 0; i < selectedShipList.Count; i++)
                            {
                                if (selectedShipList[i].ShipName.Equals(chosenShip.ShipName))
                                {
                                    //lblMessage.Text += " " + selectedShipList[i].ShipName + " product " + p.ProductName + selectedShipList[i].CurrentCapacity + " " + chosenShip.CurrentCapacity.ToString();
                                    selectedShipList[i] = chosenShip; //Replace item with updated values     

                                    //See if ship already has an shipment id 
                                    string shipmentID = "";
                                    if (string.IsNullOrEmpty(selectedShipList[i].ShipmentID)) //New Shipment
                                    {
                                        //Get a new Shipment number
                                        Guid guid = Guid.NewGuid();
                                        shipmentID = selectedShipList[i].ShipName + "-" + guid.ToString();
                                        selectedShipList[i].ShipmentID = shipmentID;
                                    }
                                    else
                                    {
                                        //Addon to existing shipment
                                        shipmentID = selectedShipList[i].ShipmentID;
                                        foreach (SPListItem shipmentItem in spShipmentList.Items)
                                        {
                                            //Store the product to list associated to ship in Shipment Detail
                                            if (shipmentID.Equals(Convert.ToString(shipmentItem["Shipment ID"]))) //Shipment ID found
                                            {
                                                //Addon to list
                                                //shipmentItem["Shipment ID"] = shipmentID;
                                                //shipmentItem.Update();
                                                //lblMessage.Text += " works ";
                                            }
                                        }
                                    }

                                    //Create new shipment list in Shipment Details in warehouse
                                    SPListItem item = listItems.Add();
                                    item["Product Name"] = p.ProductName;
                                    item["Shipment ID"] = shipmentID;
                                    item["TEU"] = Convert.ToInt32(p.TEU);
                                    item["Price"] = Convert.ToInt32(p.Price);
                                    item["Product ID"] = Convert.ToInt32(p.ProductID);
                                    item["Ship Name"] = selectedShipList[i].ShipName;

                                    item.Update();
                                    //Switch status of product to Pending for Inspection in Client Shipping List
                                    //Updates Ship's status to Pending Inspection in Shipment Schedule
                                    //Updates ship's New Capacity in Shipment Schedule
                                    //Update ship shipment id in Shipment Schedule
                                }
                            }
                        }
                        //Update/create a list for law to reference to.
                    }   //stop once all ships no longer has capacity or reach end of product list.
                }
                else
                {
                    lblMessage.Text = "There are no product to be shipped out.";
                }
            }
        }

        protected void unscheduleProducts(string shipName)
        {
            //Ships that are rejected
            //update partner product list 
            //remove shipment details from list
            //change ship status to missed
            /*
             * 
             * SPWeb mySite = SPContext.Current.Web;
                SPListItemCollection listItems = mySite.Lists[TextBox1.Text].Items;
                int itemCount = listItems.Count;

                for (int k=0; k<itemCount; k++)
                {
                    SPListItem item = listItems[k];

                    if (TextBox2.Text==item["Shipment ID"].ToString())
                    {
                        listItems.Delete(k);
                    }
                }

             * */
        }

        private List<Product> getProductList()
        {
            if (objweb == null)
            {
                objweb = GetSubSiteURL("Partner Freight");
            }

            SPList spProductList = objweb.Lists["Client Shipping List"];
            List<Product> productList = new List<Product>();

            foreach (SPListItem item in spProductList.Items)
            {
                //Retrieve product that is of status new or missed
                if (Convert.ToString(item["Status"]).Equals("New") || Convert.ToString(item["Status"]).Equals("Missed"))
                {
                    Product p = new Product();
                    p.Client = Convert.ToString(item["Client"]);
                    p.ProductID = Convert.ToString(item["Product ID"]);
                    p.ProductName = Convert.ToString(item["Product Name"]);
                    p.TEU = Convert.ToInt32(item["TEU"]);
                    p.Destination = Convert.ToString(item["Destination"]);
                    p.Price = Convert.ToInt32(item["Price"]);
                    p.Status = Convert.ToString(item["Status"]);

                    productList.Add(p);
                }
            }
            return productList;
        }

        private SelectedShip getBestShipChoice(Product p, List<SelectedShip> selectedShipList)
        {
            //Saves the ship with best destination index
            //Destination index
            SelectedShip chosenShip = new SelectedShip();
            int destinationBestIndex = -1;

            //Loop through list of hashtables
            foreach (SelectedShip sS in selectedShipList)
            {
                Hashtable shipHashtable = sS.Destinations;
                //Check if destination exist in a given ship's total destination list
                if (shipHashtable.Contains(p.Destination)) //Exists
                {
                    //If have capacity
                    if (p.TEU <= Convert.ToInt32(sS.CurrentCapacity))
                    {
                        //Capture if index is better
                        //-1 = no record yet, first attempt on comparison
                        //Store the product information to the best ship choice
                        if (destinationBestIndex == -1 || (int)shipHashtable[p.Destination] < destinationBestIndex)
                        {
                            destinationBestIndex = (int)shipHashtable[p.Destination];
                            chosenShip = sS;
                        }
                    }
                }
            }

            if (chosenShip != null)
            {
                //Updates chosenShips current capacity
                int currentCapacity = 0;
                currentCapacity = Convert.ToInt32(chosenShip.CurrentCapacity);
                currentCapacity -= p.TEU;
                chosenShip.CurrentCapacity = currentCapacity.ToString();
            }

            return chosenShip;
        }

        class Ship
        {
            public string ShipID { get; set; }
            public string ShipName { get; set; }
            public string DockingTime { get; set; }
            public string Status { get; set; }
            public string ShipmentID { get; set; }
            public string CurrentCapacity { get; set; }
            public string NewCapacity { get; set; }
        }

        class SelectedShip
        {
            public string ShipID { get; set; }
            public string ShipName { get; set; }
            public string DockingTime { get; set; }
            public string Status { get; set; }
            public string ShipmentID { get; set; }
            public string CurrentCapacity { get; set; }
            public string NewCapacity { get; set; }
            public Hashtable Destinations { get; set; }
        }

        class Product
        {
            public string Client { get; set; }
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public int TEU { get; set; }
            public string Destination { get; set; }
            public int Price { get; set; }
            public string Status { get; set; }
        }
    }
}
