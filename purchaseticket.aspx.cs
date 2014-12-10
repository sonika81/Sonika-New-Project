using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
namespace BCEC
{//kiran
    public partial class control_admin_purchaseticket : System.Web.UI.Page
    {
        Event objEvent;
        DataTable dtTickets;

        orderclass objOrder = new orderclass();
        DataTable dtAttendee;
        decimal ClubAdmin_Surcharge = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState["TICKETS"] != null)
                dtTickets = (DataTable)ViewState["TICKETS"];
            else
                dtTickets = CreateTickets();
            if (!IsPostBack)
            {
                ((Literal)this.Master.FindControl("LtrBreadCrumb")).Text =
                    "<a href='desktop.aspx'>Desktop</a> &nbsp;&raquo;&nbsp;Manage Offline Sales";
                ((Literal)this.Master.FindControl("LtrPageTitle")).Text = "Manage Offline Sales";
                
                ShowDirectDeposit();
                fillEvents();
              //  FillInvoiceDetails();
                ListItem li = new ListItem("No Invoice Found", "0");
                ddlInvoiceNo.Items.Insert(0, li);
                ddlInvoiceNo.SelectedIndex = 0;
                btnAdd.Visible = false;
                BindPOSPaymentOption();
                trinvstatus.Visible = false;
            }
            showHideTables();
        }

        private void ShowDirectDeposit()
        {
            loginscreen objcontent = new loginscreen();
            objcontent.GetInvoiceContentDetails();
            litDirectDeposit.Text = objcontent.ScreenText;
        }

        public void showHideTables()
        {
            if (grdTickets.Rows.Count > 0)
            {
                btnAdd.Visible = true;
                tableTickets.Visible = true;
            }
            else
            {
                btnAdd.Visible = false;
                tableTickets.Visible = false;
            }
            if (grdTempCart.Rows.Count > 0)
            {
                imbAddToCat.Visible = true;
                tableCart.Visible = true;
                tblCheckoutMethod.Visible = true;
            }
            else
            {
                imbAddToCat.Visible = false;
                tableCart.Visible = false;
                tblCheckoutMethod.Visible = false;
            }
        }
        private DataTable CreateTickets()
        {
            DataTable dt = new DataTable("TICKETS");
            DataColumn dc1 = new DataColumn("TICKET_ID", typeof(int));
            dc1.AutoIncrement = true;
            dc1.AutoIncrementSeed = 1;
            dc1.AutoIncrementStep = 1;
            DataColumn dc6 = new DataColumn("EVENT_NAME");
            DataColumn dc2 = new DataColumn("SEATING_AREA_ID", typeof(int));
            dc2.DefaultValue = 0;
            DataColumn dc3 = new DataColumn("SEATING_AREA_NAME");
            DataColumn dc4 = new DataColumn("TICKET_TYPE_ID", typeof(int));
            dc4.DefaultValue = 0;
            DataColumn dc5 = new DataColumn("TICKET_TYPE_NAME");
            DataColumn dc7 = new DataColumn("START_DATETIME");
            DataColumn dc8 = new DataColumn("DATE_ID");
            DataColumn dc9 = new DataColumn("PRICE", typeof(decimal));
            dc9.DefaultValue = 0;
            DataColumn dc10 = new DataColumn("QUANTITY", typeof(int));
            dc10.DefaultValue = 0;
            DataColumn dc11 = new DataColumn("TYPE");
            dc11.DefaultValue = "INDIVIDUAL";
            DataColumn dc12 = new DataColumn("TOTAL_PRICE", typeof(decimal));
            DataColumn dc13 = new DataColumn("TABLE_TYPE_ID", typeof(int));
            dc13.DefaultValue = 0;
            DataColumn dc14 = new DataColumn("TABLE_CAPACITY", typeof(int));
            dc14.DefaultValue = 0;


            DataColumn dc15 = new DataColumn("TOTAL_DISCOUNT_PERCENT", typeof(decimal));
            dc15.DefaultValue = 0;

            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            dt.Columns.Add(dc7);
            dt.Columns.Add(dc8);
            dt.Columns.Add(dc9);
            dt.Columns.Add(dc10);
            dt.Columns.Add(dc11);
            dt.Columns.Add(dc12);
            dt.Columns.Add(dc13);
            dt.Columns.Add(dc14);
            dt.Columns.Add(dc15);
            ViewState["TICKETS"] = (DataTable)dt;
            return dt;
        }
        private void fillEvents()
        {

            objEvent = new Event();
            DataTable dt = objEvent.GetEventListing();
            DataView dv = dt.DefaultView;
            dv.RowFilter = "EVENT_TYPE='Public'";

            if (dv.Count > 0)
            {
                ddlEvent.DataSource = dv;
                ddlEvent.DataTextField = "EVENT_NAME";
                ddlEvent.DataValueField = "EVENT_ID";
                ddlEvent.DataBind();
                ListItem li = new ListItem("Select", "0");
                ddlEvent.Items.Insert(0, li);
            }
            else
            {
                ListItem li = new ListItem("No Events Found", "0");
                ddlEvent.Items.Insert(0, li);
                ddlEvent.SelectedIndex = 0;
            }
        }
        private void BindSeatingArea(int eid)
        {
            objEvent = new Event();
            EventSeatingSubSeating objSeating = new EventSeatingSubSeating();
            objSeating.EventID = eid;
            //objEvent.SeatingArea = objSeating.GetSeatingAreas(eid);
            objEvent.SeatingArea = objSeating.GetActivatedSeatingAreas();
            ddlSeatingArea.DataSource = objEvent.SeatingArea;
            ddlSeatingArea.DataTextField = "SEATING_AREA_NAME";
            ddlSeatingArea.DataValueField = "SEATING_AREA_ID";
            ddlSeatingArea.DataBind();
            ListItem li = new ListItem("Select", "0");
            ddlSeatingArea.Items.Insert(0, li);
        }
        private void BindEventDates(int eventID)
        {
            objEvent = new Event();
            EvenStartDateClass objDate = new EvenStartDateClass();
            objEvent.EventDates = objDate.GetEventDates(eventID).Tables[0];
            ddlDate.DataSource = objEvent.EventDates;
            DataColumn dc = new DataColumn("START_DATETIME");
            objEvent.EventDates.Columns.Add(dc);
            DateTime eventStartDate = DateTime.MinValue;

            foreach (DataRow dr in objEvent.EventDates.Rows)
            {
                dr["START_DATETIME"] = Convert.ToDateTime(dr["START_DATE"]).ToString("dd/MM/yyyy") + " " + dr["START_TIME"].ToString();
            }
            ddlDate.DataTextField = "START_DATETIME";
            ddlDate.DataValueField = "DATE_ID";
            ddlDate.DataBind();
        }
        private void FillCountries()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("../../includes/Country.xml"));
            XmlNodeList nodeList = xmlDoc.SelectNodes("Countries/Country");

            foreach (XmlNode node in nodeList)
            {
                ddlCountry.Items.Add(new ListItem(node.SelectSingleNode("CountryName").InnerText, node.SelectSingleNode("CountryName").InnerText));
                ddlCountryMailing.Items.Add(new ListItem(node.SelectSingleNode("CountryName").InnerText, node.SelectSingleNode("CountryName").InnerText));
            }
            xmlDoc = null;
        }
        #region FILL STATES
        /// <summary>
        /// function to Fill States in dropdownlist
        /// </summary>
        private void FillStates()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("../../includes/States.xml"));
            XmlNodeList nodeList = xmlDoc.SelectNodes("States/State");

            foreach (XmlNode node in nodeList)
            {
                ddlBillState.Items.Add(new ListItem(node.SelectSingleNode("StateName").InnerText, node.SelectSingleNode("StateName").InnerText));
                ddlBillStateMailing.Items.Add(new ListItem(node.SelectSingleNode("StateName").InnerText, node.SelectSingleNode("StateName").InnerText));
            }
            nodeList = null;
            xmlDoc = null;
            xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("../../includes/austates.xml"));
            nodeList = xmlDoc.SelectNodes("States/State");

            foreach (XmlNode node in nodeList)
            {
                ddlAuStates.Items.Add(new ListItem(node.SelectSingleNode("StateName").InnerText, node.SelectSingleNode("StateName").InnerText));
                ddlAuStatesMailing.Items.Add(new ListItem(node.SelectSingleNode("StateName").InnerText, node.SelectSingleNode("StateName").InnerText));
            }

            xmlDoc = null;
        }
        #endregion

  
        protected void ddlEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEvent.SelectedIndex > 0)
            {
                ddlInvoiceNo.Enabled = true;
                int eventid = Convert.ToInt32(ddlEvent.SelectedValue);
                objEvent = new Event();
                objEvent.EventID = eventid;
                objEvent.FetchEventDetails();
                ViewState["LAST_TICKETS_MESSAGE"] = objEvent.LastTicketsMessage;
                hdnAdditionalDiscount.Value = objEvent.AdditionalTicketDiscount.ToString();
                hdnIsFoodBeverages.Value = objEvent.FNB.ToString().ToLower();
                if (string.IsNullOrEmpty(objEvent.BeverageMessage))
                    litBeverageMessage.Text = configclass.sBeveragesMessage;
                else
                    litBeverageMessage.Text = objEvent.BeverageMessage.Trim();
                hdnAllowNamesLater.Value = objEvent.AllowNamesLater.ToString();
                BindSeatingArea(eventid);
                BindEventDates(eventid);
                ddlDate_SelectedIndexChanged(sender, e);
                if (ddlSeatingArea.Items.Count > 0)
                    FillTickets(Convert.ToInt32(ddlEvent.SelectedValue), Convert.ToInt32(ddlSeatingArea.SelectedValue));

            }
            showHideTables();
        }

        protected void ddlSeatingArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(ViewState["FromInv"]) != "Yes")
            {
                ddlInvoiceNo.SelectedIndex = 0;
                //trinvnumber.Visible = false;
                trinvstatus.Visible = false;
            }
            else
            {

             //   trinvnumber.Visible = true;
                trinvstatus.Visible = true;
            }
            ViewState["FromInv"] = "";
            //if (ddlSeatingArea.SelectedIndex == 0)
            //{
            //    trinvnumber.Visible = true;

            //}
            int eventid = Convert.ToInt32(ddlEvent.SelectedValue);
            int seatingid = Convert.ToInt32(ddlSeatingArea.SelectedValue);
            FillTickets(eventid, seatingid);
            BindCart();
            showHideTables();
            //ddlInvoiceNo.ClearSelection();
            //ddlInvoiceNo.Enabled = false;
        }
        private void FillTickets(int eventID, int seatingID)
        {
            EventSeatingSubSeating objSeating = new EventSeatingSubSeating();
            DataTable dt = objSeating.getSeatingTickets(seatingID);
            DataView dv = dt.DefaultView;
            dv.RowFilter = "RANGE_NAME='1-9'";
            grdTickets.DataSource = dv;
            grdTickets.DataBind();
            if (dv.Count == 0)
            {
                btnAdd.Visible = false;
            }
            else
            {
                btnAdd.Visible = true;
            }

        }
        private void BindCart()
        {
            grdTempCart.DataSource = dtTickets;
            grdTempCart.DataBind();
            if (grdTempCart.Rows.Count == 0)
                imbAddToCat.Visible = false;
            else
                imbAddToCat.Visible = true;
        }
        private void JSAlert(string message)
        {
            ScriptManager.RegisterStartupScript(Page, this.GetType(),
                   "Msg", "alert('" + message + "')", true);
        }
        //protected void ddlDate_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //}
        protected void imbAdd_Click(object sender, ImageClickEventArgs e)
        {
            objEvent = new Event();
            bool added = false;
            foreach (GridViewRow gvr in grdTickets.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("chk");
                if (chk.Checked)
                {
                    if (string.IsNullOrEmpty(((TextBox)gvr.FindControl("txtQuantity")).Text.Trim()))
                    {
                       
                        lblTickets.Text = "Please enter Quantity";
                        return;
                    }
                    int qty;
                    qty = Convert.ToInt32(((TextBox)gvr.FindControl("txtQuantity")).Text);
                    if (qty < 1)
                    {
                        lblTickets.Text = "Please enter Quantity greater than 0";
                        return;
                    }

                    DataRow dr = dtTickets.NewRow();

                    dr["EVENT_NAME"] = ddlEvent.SelectedItem.Text;
                    dr["SEATING_AREA_ID"] = Convert.ToInt32(ddlSeatingArea.SelectedValue);
                    dr["SEATING_AREA_NAME"] = ddlSeatingArea.SelectedItem.Text;
                    dr["TICKET_TYPE_ID"] = ((HiddenField)gvr.FindControl("hdnTicketTypeID")).Value;
                    dr["TICKET_TYPE_NAME"] = ((Literal)gvr.FindControl("litTicketTypeName")).Text;
                    dr["START_DATETIME"] = ddlDate.SelectedItem.Text;
                    dr["DATE_ID"] = ddlDate.SelectedValue;
                    dr["QUANTITY"] = ((TextBox)gvr.FindControl("txtQuantity")).Text;

                    if (((DropDownList)gvr.FindControl("ddlType")).SelectedValue == "TABLE")
                    {
                        //dr["PRICE"] = Convert.ToInt32(((Literal)gvr.FindControl("litTablePrice")).Text);
                        dr["PRICE"] = Convert.ToDecimal(((Literal)gvr.FindControl("litTablePrice")).Text.Substring(1, ((Literal)gvr.FindControl("litTablePrice")).Text.Length - 1));
                        dr["TYPE"] = "TABLE";
                        dr["TABLE_TYPE_ID"] = Convert.ToInt32(((DropDownList)gvr.FindControl("ddlTableType")).SelectedValue);
                        dr["TABLE_CAPACITY"] = Convert.ToInt32(((Literal)gvr.FindControl("litTableSeats")).Text);

                        // Updating Old Prices
                        bool isIndividualOld = true;
                        foreach (DataRow myrow in dtTickets.Rows)
                        {
                            if (myrow["TYPE"].ToString().ToUpper() == "TABLE" && myrow["TICKET_TYPE_ID"].ToString() == dr["TICKET_TYPE_ID"].ToString())
                                isIndividualOld = false;
                        }

                        if (isIndividualOld)
                        {
                            foreach (DataRow dr2 in dtTickets.Rows)
                            {
                                if (dr2["TYPE"].ToString().ToUpper() == "INDIVIDUAL" && dr2["TICKET_TYPE_ID"].ToString() == dr["TICKET_TYPE_ID"].ToString())
                                {
                                    decimal discount = 0;
                                    discount = Convert.ToDecimal(hdnAdditionalDiscount.Value);
                                    dr2["TOTAL_DISCOUNT_PERCENT"] = discount;
                                    decimal prc = Convert.ToDecimal(dr2["PRICE"]);
                                    prc = (prc - ((discount / 100) * prc));
                                    dr2["PRICE"] = prc;
                                    dr2["TOTAL_PRICE"] = Convert.ToDecimal(dr2["PRICE"]) * Convert.ToInt32(dr2["QUANTITY"]);
                                }
                            }
                        }

                    }
                    else
                    {
                        bool availDiscount = false;
                        decimal discount = 0;
                        foreach (DataRow myrow in dtTickets.Rows)
                        {
                            if (myrow["TYPE"].ToString().ToUpper() == "TABLE" && myrow["TICKET_TYPE_ID"].ToString() == dr["TICKET_TYPE_ID"].ToString())
                                availDiscount = true;
                        }
                        decimal prc = Convert.ToDecimal(((Literal)gvr.FindControl("litPrice")).Text);
                        decimal dicount = Convert.ToDecimal(hdnAdditionalDiscount.Value);
                        if (availDiscount)
                            prc = (prc - ((dicount / 100) * prc));

                        dr["PRICE"] = prc;
                        dr["TYPE"] = "INDIVIDUAL";
                        if (availDiscount)
                            dr["TOTAL_DISCOUNT_PERCENT"] = dicount;
                    }
                    dr["TOTAL_PRICE"] = Convert.ToDecimal(dr["PRICE"]) * Convert.ToInt32(dr["QUANTITY"]);
                    DataRow[] existing = dtTickets.Select("TICKET_TYPE_ID=" + dr["TICKET_TYPE_ID"].ToString() + " AND SEATING_AREA_ID=" + dr["SEATING_AREA_ID"].ToString() + " AND DATE_ID=" + dr["DATE_ID"].ToString() + " AND TABLE_TYPE_ID='" + dr["TABLE_TYPE_ID"].ToString() + "'");
                    if (existing.Length > 0)
                    {
                        lblTickets.Text = "Ticket already added";
                        return;
                    }
                    int extQty = 0;
                    DataRow[] existingSameTT = dtTickets.Select("TICKET_TYPE_ID=" + dr["TICKET_TYPE_ID"].ToString() + " AND SEATING_AREA_ID=" + dr["SEATING_AREA_ID"].ToString() + " AND DATE_ID=" + dr["DATE_ID"].ToString());
                    foreach (DataRow drExt in existingSameTT)
                    {
                        if (drExt["TYPE"].ToString().ToUpper() == "TABLE")
                            extQty += Convert.ToInt32(drExt["QUANTITY"]) * Convert.ToInt32(drExt["TABLE_CAPACITY"]);
                        else
                            extQty += Convert.ToInt32(drExt["QUANTITY"]);
                    }


                    int maxPurchase = objEvent.GetMaxPurchaseQuantity(Convert.ToInt32(((HiddenField)gvr.FindControl("hdnTicketTypeID")).Value));
                    if (maxPurchase > 0)
                        if (Convert.ToInt32(((TextBox)gvr.FindControl("txtQuantity")).Text) > maxPurchase)
                        {
                            lblTickets.Text = "Maximum number of " + maxPurchase.ToString() + " tickets of this type can be purchased at a time.";
                            return;
                        }
                    objEvent.GetAvailaibleQuantity(Convert.ToInt32(ddlDate.SelectedValue), Convert.ToInt32(ddlSeatingArea.SelectedValue), Convert.ToInt32(((HiddenField)gvr.FindControl("hdnTicketTypeID")).Value), Convert.ToInt32(ddlEvent.SelectedValue));

                    if (objEvent.PendingTickets == 0)
                    {
                        lblTickets.Text = "No Tickets Available";
                        return;
                    }
                    if (dr["TYPE"].ToString().ToUpper() == "TABLE")
                    {

                        if ((Convert.ToInt32(((TextBox)gvr.FindControl("txtQuantity")).Text) * Convert.ToInt32(((Literal)gvr.FindControl("litTableSeats")).Text) + extQty) > objEvent.PendingTickets)
                        {
                            lblTickets.Text = ViewState["LAST_TICKETS_MESSAGE"].NullSafeToString();
                            return;
                        }

                    }
                    else
                    {
                        if (Convert.ToInt32(((TextBox)gvr.FindControl("txtQuantity")).Text) + extQty > objEvent.PendingTickets)
                        {
                            lblTickets.Text = ViewState["LAST_TICKETS_MESSAGE"].NullSafeToString();
                            return;
                        }

                    }
                    added = true;
                    dtTickets.Rows.Add(dr);
                }
                ViewState["TICKETS"] = dtTickets;
                BindCart();

            }
            if (added == false)
            {
                lblTickets.Text = "Please select tickets";
            }
            showHideTables();
        }
        protected void grdTempCart_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdTempCart.EditIndex = e.NewEditIndex;
            grdTempCart.DataSource = dtTickets;
            grdTempCart.DataBind();
        }
        protected void grdTempCart_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdTempCart.EditIndex = -1;
            grdTempCart.DataSource = dtTickets;
            grdTempCart.DataBind();
        }
        protected void grdTempCart_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int ticketId = Convert.ToInt32(grdTempCart.DataKeys[e.RowIndex][0].ToString());
            HiddenField LitticketTypeId = (HiddenField)grdTempCart.Rows[e.RowIndex].FindControl("hdnTicketID");
            DataRow[] dr = dtTickets.Select("TICKET_ID=" + ticketId);
            dtTickets.Rows.Remove(dr[0]);
            ViewState["TICKETS"] = dtTickets;
            // Updating Old Prices
            bool isIndividualOld = true;
            foreach (DataRow myrow in dtTickets.Rows)
            {
                if (myrow["TYPE"].ToString().ToUpper() == "TABLE" && myrow["TICKET_TYPE_ID"].ToString() == LitticketTypeId.Value.Trim())
                    isIndividualOld = false;
            }

            if (isIndividualOld)
            {
                foreach (DataRow dr2 in dtTickets.Rows)
                {
                    if (dr2["TYPE"].ToString().ToUpper() == "INDIVIDUAL" && dr2["TICKET_TYPE_ID"].ToString() == LitticketTypeId.Value.Trim())
                    {
                        dr2["TOTAL_DISCOUNT_PERCENT"] = 0;
                        dr2["PRICE"] = commonclass.CalculateActualCost(Convert.ToDecimal(dr2["PRICE"]), Convert.ToDecimal(hdnAdditionalDiscount.Value));
                        dr2["TOTAL_PRICE"] = Convert.ToDecimal(dr2["PRICE"]) * Convert.ToInt32(dr2["QUANTITY"]);
                    }
                }
            }

            grdTempCart.DataSource = dtTickets;
            grdTempCart.DataBind();
        }
        protected void grdTempCart_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int ticketId = Convert.ToInt32(grdTempCart.DataKeys[e.RowIndex][0].ToString());
            DataRow[] dr = dtTickets.Select("TICKET_ID=" + ticketId);
            TextBox txt = ((TextBox)(grdTempCart.Rows[e.RowIndex].FindControl("txtQuantity")));
            int quantity = 0;
            try
            {
                quantity = Convert.ToInt32(txt.Text.Trim());
            }
            catch
            {
                lblCart.Text = "Please enter a valid quantity.";
                return;
            }
            if (quantity == 0)
            {
                lblCart.Text = "Quantity can't be 0.";
                return;
            }
            int NoOfTickets = quantity;
            if (dr[0]["TYPE"].ToString().ToUpper() == "TABLE")
            {
                //NoOfTickets *= 10;
                NoOfTickets *= Convert.ToInt32(((HiddenField)(grdTempCart.Rows[e.RowIndex].FindControl("hdnTableCapacity"))).Value);
            }
            objEvent = new Event();
            int maxPurchase = objEvent.GetMaxPurchaseQuantity(Convert.ToInt32(dr[0]["TICKET_TYPE_ID"]));
            if (maxPurchase > 0)
                if (NoOfTickets > maxPurchase)
                {
                    lblCart.Text = "Maximum number of " + maxPurchase.ToString() + " tickets of this type can be purchased at a time.";
                    return;
                }
            objEvent.GetAvailaibleQuantity(Convert.ToInt32(dr[0]["DATE_ID"]), Convert.ToInt32(dr[0]["SEATING_AREA_ID"]), Convert.ToInt32(dr[0]["TICKET_TYPE_ID"]), Convert.ToInt32(ddlEvent.SelectedValue));

            int qtyExt = 0;
            foreach (DataRow drExt in dtTickets.Rows)
            {
                if (Convert.ToInt32(drExt["TICKET_ID"]) == ticketId)
                    continue;
                else if (Convert.ToInt32(drExt["TICKET_TYPE_ID"]) == Convert.ToInt32(dr[0]["TICKET_TYPE_ID"]))
                {
                    if (drExt["TYPE"].ToString().ToUpper() == "TABLE")
                        qtyExt += Convert.ToInt32(drExt["QUANTITY"]) * Convert.ToInt32(drExt["TABLE_CAPACITY"]);
                    else
                        qtyExt += Convert.ToInt32(drExt["QUANTITY"]);
                }
            }
            if ((NoOfTickets + qtyExt) > objEvent.PendingTickets)
            {
                lblCart.Text = ViewState["LAST_TICKETS_MESSAGE"].NullSafeToString();
                return;
            }
            dr[0]["QUANTITY"] = quantity;
            dr[0]["TOTAL_PRICE"] = Convert.ToDecimal(dr[0]["PRICE"]) * Convert.ToInt32(dr[0]["QUANTITY"]);
            ViewState["TICKETS"] = dtTickets;
            grdTempCart.EditIndex = -1;
            grdTempCart.DataSource = dtTickets;
            grdTempCart.DataBind();
        }
        protected void imbAddTocart_Click(object sender, ImageClickEventArgs e)
        {
            //if ( Convert.ToString( ViewState["inv"]) == "Invoice")
            //{
            //  //  SetCheckoutMethod();
            //    return;
            //}

            if (Convert.ToString(ViewState["inv"]) == "Invoice")
            {
                dtAttendee = (DataTable)ViewState["ATTENDEE"];
                dtAttendee.Clear();
                //ViewState["ATTENDEE"] = dtAttendee;
                ViewState["ATTENDEE"] = null;
                //  SetCheckoutMethod();
                //return;
            }

             ViewState["AddToCart"] ="Yes";
            if (dtTickets.Rows.Count == 0)
            {
                ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName-Please Select Tickets");
                return;
            }
            else
            {
                //SetCheckoutMethod();
                SetCheckoutMethodInvoice();
                mvcheckout.SetActiveView(vwCompletePayment);
                tableCart.Disabled = true;
                grdTickets.Enabled = false;
                grdTempCart.Enabled = false;
                tableTickets.Disabled = true;
                ddlEvent.Enabled = false;
                
                ddlSeatingArea.Enabled = false;
                ddlDate.Enabled = false;
                ddlPOSPaymentOption.Enabled = false;
                rdoDetailed.Enabled = false;
                rdoSimple.Enabled = false;
                rdoStraight.Enabled = false;
                if (hdnCheckoutMethod.Value.Trim().ToLower() == "simple")
                {
                    cmTrBillingInfo.Visible = false;
                    cmTrInfo.Visible = false;
                }
                else
                {
                    cmTrBillingInfo.Visible = true;
                    cmTrInfo.Visible = true;
                }
                checkout();
                if (hdnCheckoutMethod.Value.Trim().ToLower() == "straight")
                {
                    litErrorMsg.Text = "";
                    imbeditBilling.Visible = false;
                    ShowReciept();
                }
                imbAddToCat.Enabled = false;
            }
        }

        private void SetCheckoutMethod()
        {
            if (rdoSimple.Checked)
                hdnCheckoutMethod.Value = "simple";
            else if (rdoStraight.Checked)
                hdnCheckoutMethod.Value = "straight";
            else
                hdnCheckoutMethod.Value = "detailed";


            SetCheckoutMethodView(hdnCheckoutMethod.Value);
            if (Convert.ToBoolean(hdnAllowNamesLater.Value))
            {
                cmTrChkAttendee.Visible = true;
            }
            else
            {
                cmTrChkAttendee.Visible = false;
                chkAttendeeListLater.Checked = false;
            }
        }
        private void SetCheckoutMethodInvoice()
        {
            if (rdoSimple.Checked)
                hdnCheckoutMethod.Value = "simple";
            else if (rdoStraight.Checked)
                hdnCheckoutMethod.Value = "straight";
            else
                hdnCheckoutMethod.Value = "detailed";


            SetCheckoutMethodViewInvoice(hdnCheckoutMethod.Value);
            if (Convert.ToBoolean(hdnAllowNamesLater.Value))
            {
                cmTrChkAttendee.Visible = true;
            }
            else
            {
                cmTrChkAttendee.Visible = false;
                chkAttendeeListLater.Checked = false;
            }
        }

        private void SetCheckoutMethodViewInvoice(string checkoutmethod)
        {
            switch (checkoutmethod)
            {
                case "straight":
                    txtFirstName.Text = "offline sale – straight through checkout";
                    txtLastName.Text = "offline sale – straight through checkout";
                    cmTrBillingInfo.Visible = false;
                    cmTrInfo.Visible = false;
                    cmTrAttendee.Visible = false;
                    trInfo1.Visible = false;
                    trInfo2.Visible = false;
                    break;
                case "simple":
                    //txtFirstName.Text = "";
                    //txtLastName.Text = "";
                    cmTrBillingInfo.Visible = true;
                    cmTrInfo.Visible = true;
                    cmTrAttendee.Visible = false;
                    trInfo1.Visible = true;
                    trInfo2.Visible = true;
                    break;
                case "detailed":
                    //txtFirstName.Text = "";
                    //txtLastName.Text = "";
                    cmTrBillingInfo.Visible = true;
                    cmTrInfo.Visible = true;
                    CheckIsPatronDetail();
                    //cmTrAttendee.Visible = true;
                    trInfo1.Visible = true;
                    trInfo2.Visible = true;
                    break;
            }

        }

        private void SetCheckoutMethodView(string checkoutmethod)
        {
            switch (checkoutmethod)
            {
                case "straight":
                    txtFirstName.Text = "offline sale – straight through checkout";
                    txtLastName.Text = "offline sale – straight through checkout";
                    cmTrBillingInfo.Visible = false;
                    cmTrInfo.Visible = false;
                    cmTrAttendee.Visible = false;
                    trInfo1.Visible = false;
                    trInfo2.Visible = false;
                    break;
                case "simple":
                    txtFirstName.Text = "";
                    txtLastName.Text = "";
                    cmTrBillingInfo.Visible = true;
                    cmTrInfo.Visible = true;
                    cmTrAttendee.Visible = false;
                    trInfo1.Visible = true;
                    trInfo2.Visible = true;
                    break;
                case "detailed":
                    txtFirstName.Text = "";
                    txtLastName.Text = "";
                    cmTrBillingInfo.Visible = true;
                    cmTrInfo.Visible = true;
                    CheckIsPatronDetail();
                    //cmTrAttendee.Visible = true;
                    trInfo1.Visible = true;
                    trInfo2.Visible = true;
                    break;
            }

        }

        private void checkout()
        {
            if (ViewState["ATTENDEE"] == null)
                dtAttendee = CreateList();
            else
                dtAttendee = (DataTable)ViewState["ATTENDEE"];

            mvcheckout.SetActiveView(vwCompletePayment);
            BindCart2();
            FillCartCharges();
            litDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            FillCountries();
            FillStates();
        }
        private DataTable CreateList()
        {
            DataTable dt = new DataTable("ATTENDEE");
            DataColumn dc1 = new DataColumn("TICKET_ID", typeof(int));
            dc1.AutoIncrement = true;
            dc1.AutoIncrementSeed = 1;
            dc1.AutoIncrementStep = 1;
            DataColumn dc2 = new DataColumn("TICKET_TYPE_NAME");
            DataColumn dc3 = new DataColumn("START_DATETIME");
            DataColumn dc4 = new DataColumn("TICKET_TYPE_ID", typeof(int));
            dc4.DefaultValue = 0;
            DataColumn dc5 = new DataColumn("DATE_ID", typeof(int));
            DataColumn dc6 = new DataColumn("EVENT_NAME");
            dc5.DefaultValue = 0;
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            ViewState["ATTENDEE"] = dt;
            return dt;
        }
        private void BindCart2()
        {
            DataTable dt = dtTickets;

            foreach (DataRow dr in dt.Rows)
            {
                int qty = Convert.ToInt32(dr["QUANTITY"]);
                if (dr["TYPE"].ToString() == "TABLE")
                {

                    //qty = qty * 10;
                    qty = qty * Convert.ToInt32(dr["TABLE_CAPACITY"]);
                }
                for (int i = 0; i < qty; i++)
                {
                    DataRow drAtt = dtAttendee.NewRow();
                    drAtt["TICKET_TYPE_ID"] = Convert.ToInt32(dr["TICKET_TYPE_ID"]);
                    drAtt["TICKET_TYPE_NAME"] = Convert.ToString(dr["TICKET_TYPE_NAME"]);
                    drAtt["DATE_ID"] = Convert.ToInt32(dr["DATE_ID"]);
                    drAtt["START_DATETIME"] = dr["START_DATETIME"].ToString();
                    drAtt["EVENT_NAME"] = dr["EVENT_NAME"].ToString();
                    dtAttendee.Rows.Add(drAtt);
                }
            }
            ViewState["ATTENDEE"] = dtAttendee;
            dlistAttendee.DataSource = dtAttendee;
            dlistAttendee.DataBind();
            grdCart.DataSource = dt;
            grdCart.DataBind();
            dlCart.DataSource = dt;
            dlCart.DataBind();
        }
        protected void dlistAttendee_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            dietinformationclass dtClass = new dietinformationclass();

            CheckBoxList chkList = ((CheckBoxList)e.Item.FindControl("chkDietary"));
            DataTable dt = dtClass.fetchDietaryInformationDetail("");
            DataView dv = dt.DefaultView;
            dv.RowFilter = "IS_ACTIVE='TRUE'";
            chkList.DataSource = dv;
            chkList.DataTextField = "DIET_NAME";
            chkList.DataValueField = "DIET_ID";
            chkList.DataBind();

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (hdnCheckoutMethod.Value.Trim().ToLower() == "straight" || hdnCheckoutMethod.Value.Trim().ToLower() == "simple")
                {
                    ((TextBox)e.Item.FindControl("txtFirstName")).Text = txtFirstName.TrimmedText();
                    ((TextBox)e.Item.FindControl("txtLastName")).Text = txtLastName.TrimmedText();
                }
                else if (e.Item.ItemIndex == 0)
                {
                    ((TextBox)e.Item.FindControl("txtFirstName")).Text = txtFirstName.TrimmedText();
                    ((TextBox)e.Item.FindControl("txtLastName")).Text = txtLastName.TrimmedText();

                }

                if (hdnIsFoodBeverages.Value.ToString() == "false")
                {
                    ((HtmlTableCell)e.Item.FindControl("tdDietHead")).Visible = false;
                    ((HtmlTableCell)e.Item.FindControl("tdDiet")).Visible = false;
                    //trBeveragesMessage.Visible = false;
                }
                else
                {
                    //trBeveragesMessage.Visible = true;
                    ((Literal)e.Item.FindControl("litFNBMsg")).Text = litBeverageMessage.Text.Trim();
                }
            }
        }
        protected void dlistAttendee_ItemCommand(object source, DataListCommandEventArgs e)
        {
            CheckBoxList chkList = (CheckBoxList)e.Item.FindControl("chkDietary");
            chkList.Visible = true;
            ((LinkButton)e.CommandSource).Visible = false;
        }
        protected void imbeditBilling_Click(object sender, ImageClickEventArgs e)
        {
            litErrorMsg.Text = string.Empty;
            //mvcheckout.ActiveViewIndex -= 1;
            mvcheckout.ActiveViewIndex = 0;
        }
        protected void FillCartCharges()
        {
            hdnTotalCommission.Value = "0";
            hdnAgentCommission.Value = "0";
            hdnSuperAdminCommission.Value = "0";
            hdnAdditionalSurcharge.Value = "0";
            hdnEventAdminAmount.Value = "0";
            hdnClubAdminAmount.Value = "0";
            hdnGrandTotal.Value = "0";
            hdnActualFee.Value = "0";
            hdnTransactionFee.Value = "0";

            decimal totalCommission = 0;
            decimal agentPercentage = 0;
            decimal additionalSurcharge = 0;
            decimal paypaltransactionFee = 0;
            decimal actualFee = 0;
            decimal totalCost = 0;

            decimal totalPaypalFee = 0;
            decimal TotaltotalCost = 0;


            Event objEvent = new Event();
            decimal tmptotalCost = 0;

            for (int cnt = 0; cnt < grdCart.Rows.Count; cnt++)
            {
                string eventID = ddlEvent.SelectedValue;
                string DatetID = ((Literal)grdCart.Rows[cnt].FindControl("litDateID")).Text;
                string qty = ((Literal)grdCart.Rows[cnt].FindControl("litQty")).Text;
                string ticketPrice = ((Literal)grdCart.Rows[cnt].FindControl("litPrice")).Text;
                string ticketTypeID = ((Literal)grdCart.Rows[cnt].FindControl("litTicketTypeID")).Text;
                string TicketTotalPrice = ((Literal)grdCart.Rows[cnt].FindControl("litTotalPrice")).Text;
                //hdnNoOfTickets.Value = qty;
                string Ticket_Type = ((Literal)grdCart.Rows[cnt].FindControl("litType")).Text.Trim();
                int TableCapacity = 0;
                if (Ticket_Type.ToLower() == "group")
                {
                    TableCapacity = Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableCapacity")).Value);
                    hdnNoOfTickets.Value = Convert.ToString(Convert.ToInt32(qty) * TableCapacity);
                }
                else
                    hdnNoOfTickets.Value = qty;
                objOrder.EventId = Convert.ToInt32(eventID);
                objEvent.EventID = objOrder.EventId;

                objEvent.FetchEventDetails();
                litIsBasicEvent.Text = objEvent.IsBasicEvent.ToString();
                //TotaltotalCost += Math.Round(Convert.ToInt32(hdnNoOfTickets.Value) * Convert.ToDecimal(ticketPrice), 2);
                //totalCost = Math.Round(Convert.ToInt32(hdnNoOfTickets.Value) * Convert.ToDecimal(ticketPrice), 2);
                if (Ticket_Type.ToLower() == "group")
                {
                    //TotaltotalCost += Math.Round(Convert.ToInt32(((Convert.ToDecimal(hdnNoOfTickets.Value)) / (Convert.ToDecimal(10)))) * Convert.ToDecimal(ticketPrice), 2);
                    //totalCost = Math.Round(Convert.ToInt32(((Convert.ToDecimal(hdnNoOfTickets.Value)) / (Convert.ToDecimal(10)))) * Convert.ToDecimal(ticketPrice), 2);
                    TotaltotalCost += Math.Round(Convert.ToInt32(((Convert.ToDecimal(hdnNoOfTickets.Value)) / (Convert.ToDecimal(TableCapacity)))) * Convert.ToDecimal(ticketPrice), 2);
                    totalCost = Math.Round(Convert.ToInt32(((Convert.ToDecimal(hdnNoOfTickets.Value)) / (Convert.ToDecimal(TableCapacity)))) * Convert.ToDecimal(ticketPrice), 2);

                }
                else
                {
                    TotaltotalCost += Math.Round(Convert.ToInt32(hdnNoOfTickets.Value) * Convert.ToDecimal(ticketPrice), 2);
                    totalCost = Math.Round(Convert.ToInt32(hdnNoOfTickets.Value) * Convert.ToDecimal(ticketPrice), 2);
                }

                hdnTotal.Value = totalCost.ToString();

                adminusersclass objUsers = new adminusersclass();
                decimal tmptotalCommission = 0;
                tmptotalCommission = objUsers.GetSurchargeAmount();
                totalCommission += tmptotalCommission = Math.Round((Convert.ToInt32(hdnNoOfTickets.Value) * tmptotalCommission), 2);
                hdnTotalCommission.Value = totalCommission.ToString();

                //additional surcharge
                decimal tmpadditionalSurcharge = 0;
                additionalSurcharge += tmpadditionalSurcharge = Math.Round(Convert.ToDecimal(Convert.ToInt32(hdnNoOfTickets.Value) * objEvent.GetAdditionalSurcharge()), 2);
                hdnAdditionalSurcharge.Value = additionalSurcharge.ToString();
                ClubAdmin_Surcharge += Math.Round(Convert.ToDecimal(Convert.ToInt32(hdnNoOfTickets.Value) * objEvent.ClubAdminSurcharge), 2);
            }

            if (objEvent.IsAbsorb)
            {
                totalPaypalFee = 0; // Math.Round(((Convert.ToDecimal(2.40) * TotaltotalCost) / 100) + Convert.ToDecimal(0.30), 2);
            }
            else
            {
                totalPaypalFee = 0; // Math.Round(((Convert.ToDecimal(2.40) * (TotaltotalCost + totalCommission + additionalSurcharge)) / 100) + Convert.ToDecimal(0.30), 2);
            }
            // hdnTransactionFee.Value = totalPaypalFee.ToString();

            if (objEvent.IsAbsorb)
            {
                litTotal.Text = TotaltotalCost.ToString();
                litTotal1.Text = TotaltotalCost.ToString();
                LitPaypalFee.Text = "0.00";
                LitSurcharge.Text = "0";
            }
            else
            {
                //litTotal.Text = Convert.ToString(TotaltotalCost + totalPaypalFee + additionalSurcharge + totalCommission);
                //LitPaypalFee.Text = totalPaypalFee.ToString();
                //LitSurcharge.Text = Convert.ToString(totalCommission + additionalSurcharge);
                litTotal.Text = Convert.ToString(TotaltotalCost + totalPaypalFee + additionalSurcharge + totalCommission + ClubAdmin_Surcharge);
                litTotal1.Text = Convert.ToString(TotaltotalCost + totalPaypalFee + additionalSurcharge + totalCommission + ClubAdmin_Surcharge);

                if (chkIsApplyPosPayment.Checked)
                {//apply payment
                    LitPaypalFee.Text = totalPaypalFee.ToString();
                }
                else
                {
                    LitPaypalFee.Text = "0.00";
                }
              
                LitSurcharge.Text = Convert.ToString(totalCommission + additionalSurcharge + ClubAdmin_Surcharge);
            }
            if (!objEvent.IsAbsorb)
            {
                //Calculate Pos Charges and add to total cost
                clsPosPayment objPosPayment = new clsPosPayment();
                BindPOSPaymentDetail(ref objPosPayment, Convert.ToInt32(ddlPOSPaymentOption.SelectedValue));
                //litTotal.Text = Convert.ToString(Math.Round((Convert.ToDecimal(litTotal.Text) + (((objPosPayment.Percent_Charges * Convert.ToDecimal(litTotal.Text)) / 100) + objPosPayment.Additional_Charge)), 2));
                hdnTransactionFee.Value = Convert.ToString(Math.Round((((objPosPayment.Percent_Charges * Convert.ToDecimal(litTotal.Text)) / 100) + objPosPayment.Additional_Charge), 2));
                litTotal.Text = Convert.ToString(Math.Round(Convert.ToDecimal(litTotal.Text) + Convert.ToDecimal(hdnTransactionFee.Value), 2));
                litTotal1.Text = Convert.ToString(Math.Round(Convert.ToDecimal(litTotal1.Text) + Convert.ToDecimal(hdnTransactionFee.Value), 2));
                objPosPayment = null;
            }
            else
            {
                hdnTransactionFee.Value = "0";
                litTotal.Text = Convert.ToString(Math.Round(Convert.ToDecimal(litTotal.Text) + Convert.ToDecimal(hdnTransactionFee.Value), 2));
                litTotal1.Text = Convert.ToString(Math.Round(Convert.ToDecimal(litTotal1.Text) + Convert.ToDecimal(hdnTransactionFee.Value), 2));
            }
            if (chkIsApplyPosPayment.Checked)
            {//apply payment
                LitPaypalFee.Text = hdnTransactionFee.Value;
            }
            else
            {
                LitPaypalFee.Text = "0.00";
            }

           
         

            hdnTotalCommission.Value = LitSurcharge.Text;
            ClubAdmin_Surcharge = 0;
            litTotalCost.Text = TotaltotalCost.ToString("c");
            litIncludeGst.Text = commonclass.CalculateGST(Convert.ToDouble(TotaltotalCost), configclass.GSTPrice, Event.CheckGstIncluded(Convert.ToInt32(ddlEvent.SelectedValue))).ToString("c");
            string temp;
            temp=commonclass.CalculateGST(Convert.ToDouble(TotaltotalCost), configclass.GSTPrice, Event.CheckGstIncluded(Convert.ToInt32(ddlEvent.SelectedValue))).ToString();
            litTotal.Text = Convert.ToString(Convert.ToDecimal(litTotal.Text) + Convert.ToDecimal(temp));
            litTotal1.Text = litTotal.Text;
        }

        protected void imbConfirm_Click(object sender, EventArgs e)
        {
            litErrorMsg.Text = string.Empty;
            if (txtPOS.Text.Trim() == "")
            {
                ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName-Please Enter POS Code");
                return;
            }
            PlaceOrder();

        }
        private void PlaceOrder()
        {
            orderclass objOrder = new orderclass();
            if (Session["UID"] == null)
                objOrder.MemberId = 0;
            else
                objOrder.MemberId = 0;
            objOrder.EventId = Convert.ToInt32(ddlEvent.SelectedValue);
            objOrder.BillFName = txtFirstName.Text.Trim();
            //objOrder.BillMName = txtBillMiddleName.Text.Trim();
            objOrder.BillLName = txtLastName.Text.Trim();
            objOrder.BillEmail = txtEmail.Text.Trim();
            objOrder.BillAddress1 = txtAddress1.Text.Trim();
            objOrder.BillAddress2 = txtAddress2.Text.Trim();
            objOrder.BillCountry = ddlCountry.SelectedItem.Value;
            if (ddlCountry.SelectedValue == "United States")
            {
                objOrder.BillState = ddlBillState.SelectedValue;
            }
            else if (ddlCountry.SelectedValue == "Australia")
            {
                objOrder.BillState = ddlAuStates.SelectedValue;
            }
            else
            {
                objOrder.BillState = txtBillState.Text.Trim();
            }

            objOrder.BillCity = txtCity.Text.Trim();
            objOrder.BillPhone = txtPhone.Text.Trim();
            objOrder.BillZipCode = txtPostal.Text.Trim();

            objOrder.CompanyName = txtCompany.Text;
            objOrder.Facsimile = txtFax.Text;
            objOrder.EventCost = 0;
            hdnGrandTotal.Value = litTotal.Text;
            objOrder.GrandTotal = Convert.ToDecimal(hdnGrandTotal.Value);
            objOrder.TicketType = "individual";
            objOrder.NoOfTickets = dlistAttendee.Items.Count;
            objOrder.POSCode = txtPOS.Text.Trim();
            objOrder.Is_Attendee = chkAttendeeListLater.Checked;
            objOrder.PaymentMode = ddlPOSPaymentOption.SelectedItem.Value;
            //if (rdbCash.Checked)
            //    objOrder.PaymentMode = "Cash";
            //else
            //    objOrder.PaymentMode = "Cheque";
            decimal totalGrandToPaypal = Convert.ToDecimal(hdnTotal.Value);
            objOrder.Offline_OrderComments = txtComments.Text.Trim();


            clsPosPayment objPosPayment = new clsPosPayment();
            BindPOSPaymentDetail(ref objPosPayment, Convert.ToInt32(ddlPOSPaymentOption.SelectedValue));
            objOrder.POS_Percentage = objPosPayment.Percent_Charges;
            objOrder.POS_Charges = objPosPayment.Additional_Charge;
            objOrder.GST_Cost = Convert.ToDecimal(litIncludeGst.Text.Trim().Substring(1, (litIncludeGst.Text.Trim().Length - 1)));
          //  objOrder.GST_Cost = commonclass.CalculateGST(Convert.ToDouble(objOrder.GrandTotal), configclass.GSTPrice, Event.CheckGstIncluded(Convert.ToInt32(ddlEvent.SelectedValue)));
            objPosPayment = null;
            if (ddlInvoiceNo.SelectedIndex > 0)
                objOrder.InvoiceNo = ddlInvoiceNo.SelectedItem.Value;
            else
                objOrder.InvoiceNo = "";
          
            if (ValidateTicket())
            {
                if (objOrder.AddEventOrder("ORDER") == true)
                {
                    int c = 0;
                    int x = 0;
                    if (ddlInvoiceNo.SelectedIndex > 0)
                    {
                        objOrder.AddEventOrder_Invoice_Status(ddlInvoiceNo.SelectedItem.Value,ddlInvoiceStatus.SelectedItem.Value);
                    }
                    for (int cnt = 0; cnt < grdCart.Rows.Count; cnt++)
                    {
                        string eventID = ddlEvent.SelectedValue;
                        string eventDateID = ((Literal)grdCart.Rows[cnt].FindControl("litDateID")).Text;
                        string qty = ((Literal)grdCart.Rows[cnt].FindControl("litQty")).Text;
                        string ticketPrice = ((Literal)grdCart.Rows[cnt].FindControl("litPrice")).Text;
                        string ticketType = ((Literal)grdCart.Rows[cnt].FindControl("litTicketTypeID")).Text;
                        string TicketTotalPrice = ((Literal)grdCart.Rows[cnt].FindControl("litTotalPrice")).Text;
                        string SeatingAreaID = ((Literal)grdCart.Rows[cnt].FindControl("litSeatingAreaID")).Text;
                        string tableType = ((Literal)grdCart.Rows[cnt].FindControl("litType")).Text;
                        if (tableType.ToString().Trim().ToUpper() == "GROUP")
                        {
                            //qty = (Convert.ToInt32(qty) * 10).ToString();
                            qty = (Convert.ToInt32(qty) * Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableCapacity")).Value)).ToString();
                        }
                        ViewState["TableType"] = tableType;
                        hdnNoOfTickets.Value = qty;

                        objOrder.Table_Type_ID = Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableTypeID")).Value);
                        objOrder.Table_Capacity = Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableCapacity")).Value);
                        objOrder.EventId = Convert.ToInt32(eventID);
                        objOrder.SeatingAreaID = Convert.ToInt32(SeatingAreaID);
                        GetEventDetail(objOrder.EventId.ToString(), ticketPrice, Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableCapacity")).Value));

                        objOrder.IsAbsorb = Convert.ToBoolean(hdnIsAbsorb.Value);

                        //----for getting the startno and endNo for a order
                        Event objEvent = new Event();
                        objEvent.EventID = objOrder.EventId;
                        int endNo = objEvent.GetMaxEndNo();
                        objEvent = null;

                        objOrder.NoOfTickets = Convert.ToInt32(qty);//quantity of each ticket
                        objOrder.StartNo = endNo + 1;
                        objOrder.EndNo = endNo + objOrder.NoOfTickets;


                        objOrder.EventTicketType = ticketType;

                        objOrder.TicketType = "individual";

                        objOrder.TicketPrice = Convert.ToDecimal(ticketPrice);
                        objOrder.EventDateTicketID = Convert.ToInt32(eventDateID);
                        objOrder.Total = Convert.ToDecimal(TicketTotalPrice);

                        objOrder.BarcodeDrawType = hdnBarcodeDrawType.Value;
                        objOrder.BarcodeType = hdnBarcodeType.Value;

                        objOrder.TransactionFees = Convert.ToDecimal(hdnTransactionFee.Value);
                        objOrder.SuperAdminCommission = Convert.ToDecimal(hdnSuperAdminCommission.Value);
                        objOrder.AgentCommission = Convert.ToDecimal(hdnAgentCommission.Value);
                        objOrder.TotalCommission = Convert.ToDecimal(hdnTotalCommission.Value);
                        objOrder.AdditionalSurcharge = Convert.ToDecimal(hdnAdditionalSurcharge.Value);
                        objOrder.EventAdminAmount = Convert.ToDecimal(hdnEventAdminAmount.Value);
                        objOrder.ClubAdminAmount = Convert.ToDecimal(hdnClubAdminAmount.Value);
                        objOrder.ActualFee = Convert.ToDecimal(hdnActualFee.Value);
                        objOrder.GrandTotal = Convert.ToDecimal(litTotal.Text);
                        //totalGrandToPaypal += objOrder.GrandTotal;
                        totalGrandToPaypal = objOrder.GrandTotal;
                        if (tableType.ToString().Trim().ToUpper() == "GROUP")
                        {
                            objOrder.IS_Table = true;
                        }
                        else
                        {
                            objOrder.IS_Table = false;
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(ViewState["CLubAdminSurcharge"])))
                        {
                            objOrder.ClubAdmin_Surcharge = Convert.ToDecimal(ViewState["CLubAdminSurcharge"]);
                        }
                        objOrder.Total_Discount_Percent = Convert.ToDecimal(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTotal_Discount_Percent")).Value);
                        objOrder.AddEventOrderDetail("ORDER_DETAIL");
                        x = x + Convert.ToInt32(qty);
                        AddTicket(objOrder.OrderId, objOrder.OrderDetailID, objOrder.StartNo, c, x);
                        c = c + Convert.ToInt32(qty);
                    }
                    Session["CustomerMail"] = txtEmail.Text.Trim();
                    Session["CustomerName"] = txtFirstName.Text.Trim();
                    Response.Redirect("purchasecomplete.aspx?orderid=" + objOrder.OrderId + "&amt=" + totalGrandToPaypal + "&mode=event&chained1=true");
                }
                else
                    ExceptionHandlingClass.PrintError(litErrorMsg, "ERROR_ADD,@fieldName-Order");

            }
            else
            {
                mvcheckout.ActiveViewIndex = 1;
            }


            objOrder = null;
        }
        private void AddTicket(int orderId, int orderDetailID, int startNo, int c, int x)
        {
            Event objEvent = new Event();


            // for (int i = 0; i < Convert.ToInt32(hdnNoOfTickets.Value); i++)
            for (int i = c; i < Convert.ToInt32(x); i++)
            {
                orderclass obj = new orderclass();
                obj.OrderId = orderId;
                obj.OrderDetailID = orderDetailID;

                int seats = Convert.ToInt32(0);
                //if (dlistAttendee.Items.Count == Convert.ToInt32(seats))
                if (chkAttendeeListLater.Checked)
                {
                    obj.BillFName = "Pending";
                    obj.BillLName = "Pending";
                    obj.CompanyName = "Pending";
                    obj.DietaryInfo = "-1";
                    obj.Comments = "Pending";
                    obj.SeatingPreference = "Pending";

                }
                else
                {
                    if (dlistAttendee.Items.Count > 0)
                    {
                        TextBox txtDFirstName = (TextBox)dlistAttendee.Items[i].FindControl("txtFirstName");
                        TextBox txtDLastName = (TextBox)dlistAttendee.Items[i].FindControl("txtLastName");
                        TextBox txtCompany = (TextBox)dlistAttendee.Items[i].FindControl("txtCompany");
                        CheckBoxList chkList = (CheckBoxList)dlistAttendee.Items[i].FindControl("chkDietary");
                        TextBox txtOthers = (TextBox)dlistAttendee.Items[i].FindControl("txtOtherRequirements");
                        TextBox txtSeatingPreference = (TextBox)dlistAttendee.Items[i].FindControl("txtSeatingPreference");
                        //                    TextBox txtDOB = (TextBox)dlistAttendee.Items[i].FindControl("txtDOB");
                        if (txtDFirstName.Text != "")
                        {
                            obj.BillFName = txtDFirstName.Text.Trim();
                            if (txtDLastName.Text != "")
                            {

                                obj.BillLName = txtDLastName.Text.Trim();
                            }
                            else
                            {
                                obj.BillLName = Session["LNAME"].ToString();
                            }
                        }
                        else
                        {
                            if (Session["FNAME"] != null)
                                obj.BillFName = Session["FNAME"].ToString();
                            if (Session["LNAME"] != null)
                                obj.BillLName = Session["LNAME"].ToString();
                        }
                        obj.CompanyName = txtCompany.Text.Trim();
                        foreach (ListItem li in chkList.Items)
                        {
                            if (li.Selected)
                                obj.DietaryInfo += li.Value + ",";
                        }
                        obj.Comments = txtOthers.Text.Trim();
                        obj.SeatingPreference = txtSeatingPreference.Text.Trim();
                    }
                    else
                    {
                        if (Session["FNAME"] != null)
                            obj.BillFName = Session["FNAME"].ToString();
                        if (Session["LNAME"] != null)
                            obj.BillLName = Session["LNAME"].ToString();
                    }
                }
                int ticketNo = 0;
                int noOfTicket = 0;

                noOfTicket = i + 1;
                //start No is Nth ticket
                ticketNo = startNo + i;

                obj.NoOfTickets = x - c;

                obj.TicketNo = i + 1;
                obj.AddTicket();

                obj = null;
            }

        }
        private void GetEventDetail(string eventID, string ticketPrice, int intTableCapacity)
        {
            Event objEvent = new Event();
            //  objEvent.EventId = Convert.ToInt32(hdnEventId.Value);

            objEvent.EventID = Convert.ToInt32(eventID);
            objEvent.FetchEventDetails();
            litIsBasicEvent.Text = objEvent.IsBasicEvent.ToString();


            //        lblEventName.Text = objEvent.EventName;
            //lblEventCost.Text = Convert.ToString(objEvent.EventCost);

            // lblNoOfTickets.Text = hdnNoOfTickets.Value;

            hdnBarcodeDrawType.Value = objEvent.BarcodeDrawType;
            hdnBarcodeType.Value = objEvent.BarcodeType;

            decimal totalCost = 0;
            //totalCost = Math.Round(Convert.ToInt32(hdnNoOfTickets.Value) * Convert.ToDecimal(ticketPrice), 2);
            if (!string.IsNullOrEmpty(Convert.ToString(ViewState["TableType"])) && Convert.ToString(ViewState["TableType"]).ToLower() == "group")
                totalCost = Math.Round(Convert.ToDecimal(((Convert.ToDecimal(hdnNoOfTickets.Value)) / (Convert.ToDecimal(intTableCapacity)))) * Convert.ToDecimal(ticketPrice), 2);
            else
                totalCost = Math.Round(Convert.ToDecimal(hdnNoOfTickets.Value) * Convert.ToDecimal(ticketPrice), 2);
            hdnTotal.Value = totalCost.ToString();
            hdnIsAbsorb.Value = objEvent.IsAbsorb.ToString();


            //calculate additional charges
            adminusersclass objUsers = new adminusersclass();
            decimal totalCommission = 0;
            totalCommission = objUsers.GetSurchargeAmount();
            totalCommission = Math.Round((Convert.ToInt32(hdnNoOfTickets.Value) * totalCommission), 2);
            hdnTotalCommission.Value = Convert.ToString(totalCommission);

            //agent share
            objUsers.GetAgentDetails();
            decimal agentPercentage = 0;
            agentPercentage = objUsers.Commission;
            agentPercentage = Math.Round((agentPercentage * Convert.ToDecimal(hdnTotalCommission.Value)) / 100, 2);
            hdnAgentCommission.Value = agentPercentage.ToString();
            //super admin share
            hdnSuperAdminCommission.Value = Convert.ToString(totalCommission - agentPercentage);


            //additional surcharge
            decimal additionalSurcharge = 0;
            additionalSurcharge = Math.Round(Convert.ToInt32(hdnNoOfTickets.Value) * objEvent.GetAdditionalSurcharge(), 2);
            hdnAdditionalSurcharge.Value = additionalSurcharge.ToString();
            ClubAdmin_Surcharge = Math.Round(Convert.ToInt32(hdnNoOfTickets.Value) * objEvent.ClubAdminSurcharge, 2);

            decimal paypaltransactionFee = 0;
            decimal actualFee = 0;

            if (objEvent.IsAbsorb)
            {
                /////// paypaltransactionFee = ((Convert.ToDecimal(2.40) * totalCost) / 100) + Convert.ToDecimal(0.30);
                paypaltransactionFee = Convert.ToDecimal(LitPaypalFee.Text);
                paypaltransactionFee = Math.Round(paypaltransactionFee, 2);
                actualFee = paypaltransactionFee;
                hdnEventAdminAmount.Value = "0";
                //hdnClubAdminAmount.Value = Convert.ToString(totalCost - (totalCommission + actualFee));

                hdnClubAdminAmount.Value = Convert.ToString(totalCost - (totalCommission + actualFee) - ClubAdmin_Surcharge);
                hdnGrandTotal.Value = Convert.ToString(totalCost);

                hdnActualFee.Value = actualFee.ToString();

            }
            else
            {
                //// paypaltransactionFee = ((Convert.ToDecimal(2.40) * (totalCost + totalCommission + additionalSurcharge)) / 100) + Convert.ToDecimal(0.30);
                paypaltransactionFee = Convert.ToDecimal(LitPaypalFee.Text);
                paypaltransactionFee = Math.Round(paypaltransactionFee, 2);
                //  actualFee = ((Convert.ToDecimal(2.40) * (totalCost + totalCommission + additionalSurcharge + paypaltransactionFee)) / 100) + Convert.ToDecimal(0.30);
                actualFee = paypaltransactionFee;
                hdnEventAdminAmount.Value = "0";
                //hdnClubAdminAmount.Value = Convert.ToString(totalCost - (actualFee - paypaltransactionFee));
                //hdnGrandTotal.Value = Convert.ToString(totalCost + (totalCommission + additionalSurcharge + paypaltransactionFee));
                hdnClubAdminAmount.Value = Convert.ToString(totalCost);
                hdnGrandTotal.Value = Convert.ToString(totalCost + (totalCommission + additionalSurcharge + paypaltransactionFee) + ClubAdmin_Surcharge);
            }
            hdnTransactionFee.Value = paypaltransactionFee.ToString();
            hdnActualFee.Value = actualFee.ToString();

            trSurcharge.Style.Add("display", "");
            trPaypal.Style.Add("display", "");
            trTotal.Style.Add("display", "");


            if (objEvent.IsAbsorb)
            {
                ////////// lblTotalAmount.Text = hdnGrandTotal.Value;
                LitPaypalFee.Text = "0.00";
                LitSurcharge.Text = "0";

            }
            else
            {
                /////// lblTotalAmount.Text = hdnGrandTotal.Value;
                LitPaypalFee.Text = hdnTransactionFee.Value;
                LitSurcharge.Text = Convert.ToString(totalCommission + additionalSurcharge+ ClubAdmin_Surcharge);
            }

            //Calculate Pos Charges and add to total cost
            clsPosPayment objPosPayment = new clsPosPayment();
            BindPOSPaymentDetail(ref objPosPayment, Convert.ToInt32(ddlPOSPaymentOption.SelectedValue));
            // hdnGrandTotal.Value = Convert.ToString(Math.Round((Convert.ToDecimal(hdnGrandTotal.Value) + (((objPosPayment.Percent_Charges * Convert.ToDecimal(hdnGrandTotal.Value)) / 100) + objPosPayment.Additional_Charge)), 2));
            //  hdnTransactionFee.Value = Convert.ToString(Math.Round((((objPosPayment.Percent_Charges * Convert.ToDecimal(hdnGrandTotal.Value)) / 100) + objPosPayment.Additional_Charge), 2));

            hdnGrandTotal.Value = Convert.ToString(Math.Round((Convert.ToDecimal(hdnGrandTotal.Value) + (Math.Round(Convert.ToDecimal(hdnGrandTotal.Value) + Convert.ToDecimal(hdnTransactionFee.Value), 2))), 2));
            objPosPayment = null;

            ViewState["CLubAdminSurcharge"] = ClubAdmin_Surcharge;
            ClubAdmin_Surcharge = 0;
        }
        private bool ValidateTicket()
        {
            bool status = true;
            if (!chkAttendeeListLater.Checked)
            {
                orderclass obj = new orderclass();
                for (int i = 0; i < dlistAttendee.Items.Count; i++)
                {
                    TextBox txtDFirstName = (TextBox)dlistAttendee.Items[i].FindControl("txtFirstName");
                    TextBox txtDLastName = (TextBox)dlistAttendee.Items[i].FindControl("txtLastName");
                    //                TextBox txtDOB = (TextBox)dlistAttendee.Items[i].FindControl("txtDOB");

                    //if (txtDOB.Text.Trim() == "")
                    //{
                    //    obj.ArrErrorMsgs.Add("CUSTOM_MSG,@fieldName-Please enter DOB for Ticket " + Convert.ToString(i + 1));
                    //    status = false;
                    //}

                    if (txtDFirstName.Text.Trim() == "")
                    {
                        obj.ArrErrorMsgs.Add("CUSTOM_MSG,@fieldName-Please enter First Name for Ticket " + Convert.ToString(i + 1));
                        status = false;
                    }
                    if (txtDLastName.Text.Trim() == "")
                    {
                        obj.ArrErrorMsgs.Add("CUSTOM_MSG,@fieldName-Please enter Last Name for Ticket " + Convert.ToString(i + 1));
                        status = false;
                    }
                }
                ExceptionHandlingClass.PrintError(litErrorMsg, obj.ArrErrorMsgs);
                obj = null;
            }
            return status;
        }
        protected void imbProceed_Click(object sender, ImageClickEventArgs e)
        {
            //loginscreen objScreen = new loginscreen();
            //objScreen.GetInvoiceContentDetails();
            //txtContent.Text = objScreen.ScreenText;
            if (ddlInvoiceNo.SelectedIndex > 0)
            {
                litInvoiceNumber.Text = ddlInvoiceNo.SelectedItem.Value;
            }
            if  (Convert.ToString(ViewState["AddToCart"]) =="No")
            {
                imbAddTocart_Click(sender, e);
            }
            if (ddlPOSPaymentOption.SelectedItem.Value == "13")
            {
                PlaceOrder_Invoice();
            }
            
            else
            {

                litErrorMsg.Text = "";
                dtAttendee = (DataTable)ViewState["ATTENDEE"];
                dlistAttendee.DataSource = dtAttendee;
                dlistAttendee.DataBind();
                ShowReciept();
            }
        }
        private void ShowReciept()
        {
            objOrder.BillFName = txtFirstName.Text.Trim();
            objOrder.BillLName = txtLastName.Text.Trim();
            objOrder.BillEmail = txtEmail.Text.Trim();
            objOrder.BillAddress1 = txtAddress1.Text.Trim();
            objOrder.BillAddress2 = txtAddress2.Text.Trim();
            objOrder.BillCountry = ddlCountry.SelectedItem.Value;
            if (ddlCountry.SelectedValue == "United States")
            {
                objOrder.BillState = ddlBillState.SelectedItem.Value;
            }
            else if (ddlCountry.SelectedValue == "Australia")
            {
                objOrder.BillState = ddlAuStates.SelectedItem.Value;
            }
            else
            {
                objOrder.BillState = txtBillState.Text.Trim();
            }
            objOrder.BillCity = txtCity.Text.Trim();
            objOrder.BillPhone = txtPhone.Text.Trim();
            objOrder.BillZipCode = txtPostal.Text.Trim();

            objOrder.CompanyName = txtCompany.Text;
            objOrder.Facsimile = txtFax.Text;

            mvcheckout.ActiveViewIndex += 1;
            litDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            litUserName.Text = objOrder.BillFName;
            litUserName2.Text = objOrder.BillFName;
            litUserName3.Text = objOrder.BillFName;
            litAddress.Text = objOrder.BillAddress1 + "<br/>" + objOrder.BillCity + "<br/>" + objOrder.BillState + "<br/>" + objOrder.BillCountry;
        }
        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {

            GridViewRow gvr = (GridViewRow)(((Control)sender).NamingContainer);
            DropDownList ddlType = (DropDownList)gvr.FindControl("ddlType");
            Literal litTablePrice = (Literal)gvr.FindControl("litTablePrice");
            DropDownList ddlTableType = (DropDownList)gvr.FindControl("ddlTableType");
            Literal litTableSeats = (Literal)gvr.FindControl("litTableSeats");
            if (Convert.ToString(ddlType.SelectedValue) == "INDIVIDUAL")
            {
                litTablePrice.Text = "NA";
                ddlTableType.Visible = false;
                litTableSeats.Text = "NA";
                ((Literal)gvr.FindControl("litSign")).Visible = true;
                ((Literal)gvr.FindControl("litPrice")).Text = Convert.ToString(((HiddenField)gvr.FindControl("hdnPrice")).Value);
            }
            else
            {
                HiddenField HiddenTicketType_ID = (HiddenField)gvr.FindControl("hdnTicketTypeID");
                TableType objTableType = new TableType();
                objTableType.Seating_AreaID = Convert.ToInt32(ddlSeatingArea.SelectedValue);
                objTableType.TickeType_Id = Convert.ToInt32(HiddenTicketType_ID.Value);
                objTableType.EventId = Convert.ToInt32(ddlEvent.SelectedValue);
                objTableType.Page_Code = "TABLEDETAIL";
                BindddlTableType(ddlTableType, objTableType, litTablePrice, litTableSeats);
                ((Literal)gvr.FindControl("litPrice")).Text = "NA";
                ((Literal)gvr.FindControl("litSign")).Visible = false;
            }
        }
        protected void ddlTableType_SelectedIndexChanged(object sender, EventArgs e)
        {

            GridViewRow gvr = (GridViewRow)(((Control)sender).NamingContainer);
            DropDownList ddlType = (DropDownList)gvr.FindControl("ddlType");
            Literal litTablePrice = (Literal)gvr.FindControl("litTablePrice");
            DropDownList ddlTableType = (DropDownList)gvr.FindControl("ddlTableType");
            Literal litTableSeats = (Literal)gvr.FindControl("litTableSeats");
            if (Convert.ToString(ddlType.SelectedValue) == "INDIVIDUAL")
            {
                litTablePrice.Text = "NA";
                ddlTableType.Visible = false;
                litTableSeats.Text = "NA";
            }
            else
            {
                HiddenField HiddenTicketType_ID = (HiddenField)gvr.FindControl("hdnTicketTypeID");
                TableType objTableType = new TableType();
                objTableType.Seating_AreaID = Convert.ToInt32(ddlSeatingArea.SelectedValue);
                objTableType.TickeType_Id = Convert.ToInt32(HiddenTicketType_ID.Value);
                objTableType.EventId = Convert.ToInt32(ddlEvent.SelectedValue);
                objTableType.TableTypeID = Convert.ToInt32(ddlTableType.SelectedValue);
                DataTable dtTableType = objTableType.GetTablePriceDetail();
                litTablePrice.Text = "$" + Convert.ToString(dtTableType.Rows[0]["TABLE_PRICE"]);
                litTableSeats.Text = Convert.ToString(dtTableType.Rows[0]["TABLE_CAPACITY"]);
                objTableType = null;
            }
        }
        private void BindddlTableType(DropDownList ddlTableType, TableType objTableType, Literal litTablePrice, Literal litTableSeats)
        {

            DataTable dtTableType = objTableType.GetTablePriceDetail();
            if (dtTableType != null && dtTableType.Rows.Count > 0)
            {
                ddlTableType.DataSource = dtTableType;
                ddlTableType.DataTextField = "TABLE_TYPE_NAME";
                ddlTableType.DataValueField = "TABLE_TYPE_ID";
                ddlTableType.DataBind();
                litTablePrice.Text = "$" + Convert.ToString(dtTableType.Rows[0]["TABLE_PRICE"]);
                litTableSeats.Text = Convert.ToString(dtTableType.Rows[0]["TABLE_CAPACITY"]);
                ddlTableType.Visible = true;
            }
            objTableType = null;

        }
        protected void grdTickets_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlType = (DropDownList)e.Row.FindControl("ddlType");
                Literal litTablePrice = (Literal)e.Row.FindControl("litTablePrice");
                Literal litTableSeats = (Literal)e.Row.FindControl("litTableSeats");
                Literal litRemQty = (Literal)e.Row.FindControl("litRemQty");
                CheckBox chk = (CheckBox)e.Row.FindControl("chk");
                TextBox txtqty= (TextBox)e.Row.FindControl("txtQuantity");
                objEvent = new Event();
                DataTable dt1 = objEvent.GetInvoiceListingTicket(ddlInvoiceNo.SelectedItem.Value, Convert.ToInt32(((HiddenField)e.Row.FindControl("hdnTicketTypeID")).Value));
                if (dt1.Rows.Count>0)
                {
                    txtqty.Text = Convert.ToString(dt1.Rows[0]["TOTAL_TICKETS"]);
                chk.Checked = true;
                }
                
                if (Convert.ToString(ddlType.SelectedValue) == "INDIVIDUAL")
                {
                    litTablePrice.Text = "NA";
                    litTableSeats.Text = "NA";
                }
                if (litRemQty != null)
                {
                    objEvent = new Event();
                    objEvent.GetAvailaibleQuantity(Convert.ToInt32(ddlDate.SelectedValue), Convert.ToInt32(ddlSeatingArea.SelectedValue), Convert.ToInt32(((HiddenField)e.Row.FindControl("hdnTicketTypeID")).Value), Convert.ToInt32(ddlEvent.SelectedValue));
                    litRemQty.Text = objEvent.PendingTickets.ToString();
                    //if (objEvent.PendingTickets <= 0)
                    //    e.Row.Visible = false;
                }


            }
        }
        protected void chkAttendeeListLater_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAttendeeListLater.Checked)
                tblAttendeeOption.Visible = false;
            else
                tblAttendeeOption.Visible = true;

        }

        private void BindPOSPaymentOption()
        {
            clsPosPayment objPosPayment = new clsPosPayment();
            DataTable dt = objPosPayment.GetPosPaymentListing(string.Empty);
            if (dt.Rows.Count > 0)
            {
                ddlPOSPaymentOption.DataSource = dt;
                ddlPOSPaymentOption.DataTextField = "PAYMENTOPTION";
                ddlPOSPaymentOption.DataValueField = "PAYMENT_ID";
                ddlPOSPaymentOption.DataBind();
            }
            dt.Clear();
            objPosPayment = null;
        }
        protected void ddlPOSPaymentOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            //checkout();
            //clsPosPayment objPosPayment = new clsPosPayment();
            ////BindPOSPaymentDetail(ref objPosPayment, Convert.ToInt32(ddlPOSPaymentOption.SelectedValue));
            //objPosPayment = null;
        }

        private void BindPOSPaymentDetail(ref clsPosPayment objPosPayment, int payment_id)
        {

            objPosPayment.PaymentId = payment_id;
            objPosPayment.GetPosPaymentDetail();

        }
        /// <summary>
        /// method to check whether to display the patron detail later option or not
        /// </summary>
        private void CheckIsPatronDetail()
        {
            if (ddlEvent.SelectedIndex > 0)
            {
                objEvent = new Event();
                objEvent.EventID = Convert.ToInt32(ddlEvent.SelectedValue);
                DataTable dtDays = objEvent.GetMinEmailNotificationDay();
                if (dtDays.Rows.Count > 0)
                {
                    EvenStartDateClass objEventStartDate = new EvenStartDateClass();
                    DataSet ds = objEventStartDate.GetEventAttendanceDates(Convert.ToInt32(ddlEvent.SelectedValue), "MINSTARTDATE");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DateTime sDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["START_DATE"]);
                        if (DateTime.Now.Date == sDate.AddDays(-1 * Convert.ToInt32(dtDays.Rows[0]["NO_DAY"])).Date)
                        {
                            cmTrAttendee.Visible = false;
                        }
                        else
                        {
                            cmTrAttendee.Visible = true;
                        }
                    }
                    objEventStartDate = null;
                }

            }
        }

        #region FillInvoice Details
        private void PlaceOrder_Invoice()
        {
            orderclass objOrder = new orderclass();
            if (Session["UID"] == null)
                objOrder.MemberId = 0;
            else
                objOrder.MemberId = 0;
            objOrder.EventId = Convert.ToInt32(ddlEvent.SelectedValue);
            objOrder.BillFName = txtFirstName.Text.Trim();
            litUserName3.Text = objOrder.BillFName;
            //objOrder.BillMName = txtBillMiddleName.Text.Trim();
            objOrder.BillLName = txtLastName.Text.Trim();
            objOrder.BillEmail = txtEmail.Text.Trim();
            objOrder.BillAddress1 = txtAddress1.Text.Trim();
            objOrder.BillAddress2 = txtAddress2.Text.Trim();
            objOrder.BillCountry = ddlCountry.SelectedItem.Value;
            if (ddlCountry.SelectedValue == "United States")
            {
                objOrder.BillState = ddlBillState.SelectedValue;
            }
            else if (ddlCountry.SelectedValue == "Australia")
            {
                objOrder.BillState = ddlAuStates.SelectedValue;
            }
            else
            {
                objOrder.BillState = txtBillState.Text.Trim();
            }

            objOrder.BillCity = txtCity.Text.Trim();
            objOrder.BillPhone = txtPhone.Text.Trim();
            objOrder.BillZipCode = txtPostal.Text.Trim();

            objOrder.CompanyName = txtCompany.Text;
            objOrder.Facsimile = txtFax.Text;
            objOrder.EventCost = 0;
            hdnGrandTotal.Value = litTotal.Text;
            objOrder.GrandTotal = Convert.ToDecimal(hdnGrandTotal.Value);
            objOrder.TicketType = "individual";
            objOrder.NoOfTickets = dlistAttendee.Items.Count;
            objOrder.POSCode = txtPOS.Text.Trim();
            //objOrder.SeatingAreaID=
            objOrder.Is_Attendee = chkAttendeeListLater.Checked;
            objOrder.PaymentMode = ddlPOSPaymentOption.SelectedItem.Value;
            //if (rdbCash.Checked)
            //    objOrder.PaymentMode = "Cash";
            //else
            //    objOrder.PaymentMode = "Cheque";
            decimal totalGrandToPaypal = Convert.ToDecimal(hdnTotal.Value);
            objOrder.Offline_OrderComments = txtComments.Text.Trim();


            clsPosPayment objPosPayment = new clsPosPayment();
            BindPOSPaymentDetail(ref objPosPayment, Convert.ToInt32(ddlPOSPaymentOption.SelectedValue));
            objOrder.POS_Percentage = objPosPayment.Percent_Charges;
            objOrder.POS_Charges = objPosPayment.Additional_Charge;
            objOrder.GST_Cost = Convert.ToDecimal(litIncludeGst.Text.Trim().Substring(1, (litIncludeGst.Text.Trim().Length - 1)));
            objPosPayment = null;

            if (ddlCountryMailing.SelectedValue == "United States")
            {
                objOrder.Mailing_State = ddlBillStateMailing.SelectedValue;
            }
            else if (ddlCountry.SelectedValue == "Australia")
            {
                objOrder.Mailing_State = ddlAuStatesMailing.SelectedValue;
            }
            else
            {
                objOrder.Mailing_State = txtBillStateMailing.Text.Trim();
            }


            objOrder.Country = ddlCountry.SelectedItem.Value;
          //  objOrder.Mailing_State = ddlAuStatesMailing.SelectedItem.Value;
            objOrder.Mailing_Country = ddlCountryMailing.SelectedItem.Value;
            objOrder.Mailing_Zipcode = txtPostalMailing.Text;
            objOrder.Mailing_City = txtCityMailing.Text;
            objOrder.Mailing_Address2 = txtAddressMailing2.Text;
            objOrder.Mailing_Address1 = txtAddressMailing1.Text;
            objOrder.BillingAbnacn = txtAbnAcn.Text;
            objOrder.BillingMobile = txtMobile.Text;

            //if (ValidateTicket())
            if (true==true)
            {
                if (rdoSimple.Checked)
                    objOrder.CheckOutType = "simple";
                else if (rdoStraight.Checked)
                    objOrder.CheckOutType = "straight";
                else
                    objOrder.CheckOutType = "detailed";
                objOrder.OrderDate = ddlDate.SelectedItem.Value;
                if (objOrder.AddEventOrder_Invoice("ORDER") == true)
                {
                    int c = 0;
                    int x = 0;
                    for (int cnt = 0; cnt < grdCart.Rows.Count; cnt++)
                    {
                        string eventID = ddlEvent.SelectedValue;
                        string eventDateID = ((Literal)grdCart.Rows[cnt].FindControl("litDateID")).Text;
                        string qty = ((Literal)grdCart.Rows[cnt].FindControl("litQty")).Text;
                        string ticketPrice = ((Literal)grdCart.Rows[cnt].FindControl("litPrice")).Text;
                        string ticketType = ((Literal)grdCart.Rows[cnt].FindControl("litTicketTypeID")).Text;
                        string TicketTotalPrice = ((Literal)grdCart.Rows[cnt].FindControl("litTotalPrice")).Text;
                        string SeatingAreaID = ((Literal)grdCart.Rows[cnt].FindControl("litSeatingAreaID")).Text;
                        string tableType = ((Literal)grdCart.Rows[cnt].FindControl("litType")).Text;
                        if (tableType.ToString().Trim().ToUpper() == "GROUP")
                        {
                            //qty = (Convert.ToInt32(qty) * 10).ToString();
                            qty = (Convert.ToInt32(qty) * Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableCapacity")).Value)).ToString();
                        }
                        ViewState["TableType"] = tableType;
                        hdnNoOfTickets.Value = qty;

                        objOrder.Table_Type_ID = Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableTypeID")).Value);
                        objOrder.Table_Capacity = Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableCapacity")).Value);
                        objOrder.EventId = Convert.ToInt32(eventID);
                        objOrder.SeatingAreaID = Convert.ToInt32(SeatingAreaID);
                        GetEventDetail(objOrder.EventId.ToString(), ticketPrice, Convert.ToInt32(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTableCapacity")).Value));

                        objOrder.IsAbsorb = Convert.ToBoolean(hdnIsAbsorb.Value);

                        //----for getting the startno and endNo for a order
                        Event objEvent = new Event();
                        objEvent.EventID = objOrder.EventId;
                        int endNo = objEvent.GetMaxEndNo();
                        objEvent = null;

                        objOrder.NoOfTickets = Convert.ToInt32(qty);//quantity of each ticket
                        objOrder.StartNo = endNo + 1;
                        objOrder.EndNo = endNo + objOrder.NoOfTickets;


                        objOrder.EventTicketType = ticketType;

                        objOrder.TicketType = "individual";

                        objOrder.TicketPrice = Convert.ToDecimal(ticketPrice);
                        objOrder.EventDateTicketID = Convert.ToInt32(eventDateID);
                        objOrder.Total = Convert.ToDecimal(TicketTotalPrice);

                        objOrder.BarcodeDrawType = hdnBarcodeDrawType.Value;
                        objOrder.BarcodeType = hdnBarcodeType.Value;

                        objOrder.TransactionFees = Convert.ToDecimal(hdnTransactionFee.Value);
                        objOrder.SuperAdminCommission = Convert.ToDecimal(hdnSuperAdminCommission.Value);
                        objOrder.AgentCommission = Convert.ToDecimal(hdnAgentCommission.Value);
                        objOrder.TotalCommission = Convert.ToDecimal(hdnTotalCommission.Value);
                        objOrder.AdditionalSurcharge = Convert.ToDecimal(hdnAdditionalSurcharge.Value);
                        objOrder.EventAdminAmount = Convert.ToDecimal(hdnEventAdminAmount.Value);
                        objOrder.ClubAdminAmount = Convert.ToDecimal(hdnClubAdminAmount.Value);
                        objOrder.ActualFee = Convert.ToDecimal(hdnActualFee.Value);
                        objOrder.GrandTotal = Convert.ToDecimal(litTotal.Text);
                        //totalGrandToPaypal += objOrder.GrandTotal;
                        totalGrandToPaypal = objOrder.GrandTotal;
                        if (tableType.ToString().Trim().ToUpper() == "GROUP")
                        {
                            objOrder.IS_Table = true;
                        }
                        else
                        {
                            objOrder.IS_Table = false;
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(ViewState["CLubAdminSurcharge"])))
                        {
                            objOrder.ClubAdmin_Surcharge = Convert.ToDecimal(ViewState["CLubAdminSurcharge"]);
                        }

                      


                        objOrder.Total_Discount_Percent = Convert.ToDecimal(((HiddenField)grdCart.Rows[cnt].FindControl("hdnTotal_Discount_Percent")).Value);
                        objOrder.AddEventOrderDetail_Invoice("ORDER_DETAIL");
                        x = x + Convert.ToInt32(qty);
                      //  AddTicket(objOrder.OrderId, objOrder.OrderDetailID, objOrder.StartNo, c, x);
                        c = c + Convert.ToInt32(qty);
                    }
                    string s;
                    s = Receipt;
                    litInvoiceNumber.Text = objOrder.InvoiceNo;
                  
                    SendUSerMail(objOrder.InvoiceNo);
                    Session["CustomerMail"] = txtEmail.Text.Trim();
                    Session["CustomerName"] = txtFirstName.Text.Trim();
                    Response.Redirect("purchasecomplete.aspx?orderid=" + objOrder.OrderId + "&amt=" + totalGrandToPaypal + "&mode=event&chained1=true&frommode=inv&invno=" + objOrder.InvoiceNo);
                }
                else
                    ExceptionHandlingClass.PrintError(litErrorMsg, "ERROR_ADD,@fieldName-Order");

            }
            else
            {
                mvcheckout.ActiveViewIndex = 1;
            }


            objOrder = null;
        }

        private void SendUSerMail(string Invnumber)
        {

            Event objEvent = new Event();
            orderclass objOrder = new orderclass();
            
            Hashtable htblEmail = new Hashtable();
            htblEmail["@username"] = txtFirstName.Text+" "+txtLastName.Text;
            htblEmail["@message"] = "<b>Invoice Number: " + Invnumber;
            htblEmail["@details"] = Receipt;
            htblEmail["@path"] = configclass.siteUrl;
            htblEmail["@sitename"] = configclass.siteName;

            //--------------------------
            //htblEmail["@soldtickets"] = ticketsPurchased;
            //htblEmail["@tickets"] = noOfTicket.ToString();

            bool status = MailClass.SendMail(txtEmail.Text, configclass.siteName + "-Event Invoice Booking", configclass.emailTemplatePath, "EventInvoice.htm", htblEmail);


        }
        
       
       
        protected void ddlInvoiceNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            trinvstatus.Visible = false;
            if (ddlInvoiceNo.SelectedIndex == 0)
            {
                ViewState["ATTENDEE"] = dtAttendee;
                ddlSeatingArea.Enabled = true;
                grdTickets.DataBind();
                trinvstatus.Style.Add("display", "none");
            }
            else
            {
                trinvstatus.Style.Add("display", "");
               // ddlSeatingArea.Enabled = false;
                ddlInvoiceStatus.Focus();
            }
            mvcheckout.Visible = true;
                trinvstatus.Visible = true;
                FillInvoiceStatus();
                objEvent = new Event();
                if (dtAttendee!=null)
                dtAttendee.Clear();
                if (dtTickets != null)
                dtTickets.Clear();
                ViewState["ATTENDEE"] = dtAttendee;
               
                //ddlEvent.SelectedItem = ddlInvoiceNo.SelectedItem.Value;


                DataTable dt1 = objEvent.GetInvoiceListingMain(ddlInvoiceNo.SelectedItem.Value);
                DataTable dt = objEvent.GetInvoiceListing(ddlInvoiceNo.SelectedItem.Value);

                if (dt1.Rows.Count>0)
                {
                    if (Convert.ToString(dt1.Rows[0]["CHECKOUT_METHOD"]) == "detailed")
                    {
                        rdoDetailed.Checked = true;
                        rdoSimple.Checked = false;
                        rdoStraight.Checked = false;
                    }
                    else if (Convert.ToString(dt1.Rows[0]["CHECKOUT_METHOD"]) == "simple")
                    {
                        rdoSimple.Checked = true;
                        rdoDetailed.Checked = false;
                        rdoStraight.Checked = false;
                    }
                    else if (Convert.ToString(dt1.Rows[0]["CHECKOUT_METHOD"]) == "straight")
                    {
                        rdoStraight.Checked = true;
                        rdoSimple.Checked = false;
                        rdoDetailed.Checked = false;
                    }
                 
                }

                if (dt.Rows.Count > 0)
                {

                    BindEventDates(Convert.ToInt32(dt.Rows[0]["EVENT_ID"]));
                    ddlDate.ClearSelection();
                    try
                    {
                        ddlDate.Items.FindByValue(Convert.ToString(dt1.Rows[0]["EVENT_DATE_VALUE"])).Selected = true;
                    }
                    catch { }



                    //ddlEvent.ClearSelection();

                    //ddlEvent.Items.FindByValue(Convert.ToString(dt.Rows[0]["EVENT_ID"])).Selected = true;
                    //ddlEvents_SelectedIndexChanged(sender, e);
                    //ddlDate.ClearSelection();
                    //try
                    //{
                    //    ddlDate.Items.FindByValue(Convert.ToString(dt1.Rows[0]["EVENT_DATE_VALUE"])).Selected = true;
                    //}
                    //catch { }
                    ViewState["FromInv"] = "Yes";

                    ddlSeatingArea.ClearSelection();
                    ddlSeatingArea.Items.FindByValue(Convert.ToString(dt.Rows[0]["SEATING_AREA_ID"])).Selected = true;
                    ddlSeatingArea_SelectedIndexChanged(sender, e);
                    ddlDate.Enabled = true;

                    //  Button btn;
                    ImageClickEventArgs e1 = new ImageClickEventArgs(0, 0);
                    imbAdd_Click(sender, e1);

                    SetCheckoutMethod();
                    //  imbAddTocart_Click(sender, e1);
                    if (hdnCheckoutMethod.Value.Trim().ToLower() == "simple")
                    {
                        cmTrBillingInfo.Visible = false;
                        cmTrInfo.Visible = false;
                    }
                    else
                    {
                        cmTrBillingInfo.Visible = true;
                        cmTrInfo.Visible = true;
                    }
                    checkout();
                    if (hdnCheckoutMethod.Value.Trim().ToLower() == "straight")
                    {
                        litErrorMsg.Text = "";
                        imbeditBilling.Visible = false;
                        ShowReciept();
                    }


                    //////
                    ViewState["inv"] = "Invoice";
                    ViewState["AddToCart"] = "No";
                    FillDetailedInfo(dt1);
                    ddlPOSPaymentOption.Enabled = true;
                    BindPOSPaymentOptionInv();
                    imbAddToCat.Enabled = true;
                    rdoSimple.Enabled = true;
                    rdoStraight.Enabled = true;
                    rdoDetailed.Enabled = true;

                    ddlDate.ClearSelection();
                    try
                    {
                        ddlDate.Items.FindByValue(Convert.ToString(dt1.Rows[0]["EVENT_DATE_VALUE"])).Selected = true;
                    }
                    catch { }
                }
                else
                {
                    ViewState["FromInv"] = "Yes";

                    ddlSeatingArea.ClearSelection();
                    ddlSeatingArea.Items.FindByValue("0").Selected = true;
                    ddlSeatingArea_SelectedIndexChanged(sender, e);
                    ddlDate.Enabled = true;
                    rdoDetailed.Checked = false;
                    rdoSimple.Checked = false;
                    rdoStraight.Checked = false;
                    mvcheckout.Visible = false;
                    SetCheckoutMethod();

                }
                //ddlSeatingArea.Enabled = false;
                //ddlInvoiceStatus.Focus();
           // }
         //   ddlSeatingArea.Enabled = true;
        }

        private void FillDetailedInfo(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                txtFirstName.Text = Convert.ToString(dt.Rows[0]["BILLING_FIRST_NAME"]);
                txtLastName.Text = Convert.ToString(dt.Rows[0]["BILLING_LAST_NAME"]);
                txtEmail.Text = Convert.ToString(dt.Rows[0]["BILLING_EMAIL"]);
                txtPhone.Text = Convert.ToString(dt.Rows[0]["BILLING_PHONE"]);
                txtFax.Text = Convert.ToString(dt.Rows[0]["BILLING_FACSIMILE"]);
                txtCompany.Text = Convert.ToString(dt.Rows[0]["BILLING_COMPANY"]);
                txtAddress1.Text = Convert.ToString(dt.Rows[0]["BILLING_ADDRESS1"]);
                txtAddress2.Text = Convert.ToString(dt.Rows[0]["BILLING_ADDRESS2"]);
                txtCity.Text = Convert.ToString(dt.Rows[0]["BILLING_CITY"]);
                txtPostal.Text = Convert.ToString(dt.Rows[0]["BILLING_ZIPCODE"]);
                txtBillState.Text = Convert.ToString(dt.Rows[0]["BILLING_STATE"]);

                ddlCountry.ClearSelection();
                try
                {
                    ddlCountry.Items.FindByValue(Convert.ToString(dt.Rows[0]["COUNTRY"])).Selected = true;
                }
                catch { }

                if (ddlCountry.SelectedValue == "United States")
                {
                    ddlBillState.ClearSelection();
                    try
                    {
                        ddlBillState.Style.Add("Display", "");
                        ddlAuStates.Style.Add("Display", "none");
                        txtBillState.Style.Add("Display", "none");
                        ddlBillState.Items.FindByValue(Convert.ToString(dt.Rows[0]["BILLING_STATE"])).Selected = true;
                    }
                    catch { }
                }
                else if (ddlCountry.SelectedValue == "Australia")
                {
                    ddlAuStates.ClearSelection();
                    try
                    {
                        ddlBillState.Style.Add("Display", "none");
                        ddlAuStates.Style.Add("Display", "");
                        txtBillState.Style.Add("Display", "none");
                        ddlAuStates.Items.FindByValue(Convert.ToString(dt.Rows[0]["BILLING_STATE"])).Selected = true;
                    }
                    catch { }

                }
                else
                {

                    ddlBillState.Style.Add("Display", "none");
                    ddlAuStates.Style.Add("Display", "none");
                    txtBillState.Style.Add("Display", "");
                    txtBillState.Text = Convert.ToString(dt.Rows[0]["BILLING_STATE"]);

                }


                ddlCountryMailing.ClearSelection();
                try
                {
                    ddlCountryMailing.Items.FindByValue(Convert.ToString(dt.Rows[0]["MAILING_COUNTRY"])).Selected = true;
                }
                catch { }

                if (ddlCountryMailing.SelectedValue == "United States")
                {
                    ddlBillState.ClearSelection();
                    try
                    {
                        ddlBillStateMailing.Style.Add("Display", "");
                        ddlAuStatesMailing.Style.Add("Display", "none");
                        txtBillStateMailing.Style.Add("Display", "none");
                        ddlBillStateMailing.Items.FindByValue(Convert.ToString(dt.Rows[0]["MAILING_STATE"])).Selected = true;
                    }
                    catch { }
                }
                else if (ddlCountryMailing.SelectedValue == "Australia")
                {
                    ddlBillState.ClearSelection();
                    try
                    {
                        ddlBillStateMailing.Style.Add("Display", "none");
                        ddlAuStatesMailing.Style.Add("Display", "");
                        txtBillStateMailing.Style.Add("Display", "none");
                        ddlAuStatesMailing.Items.FindByValue(Convert.ToString(dt.Rows[0]["MAILING_STATE"])).Selected = true;
                    }
                    catch { }
                }
                else
                {
                    ddlBillStateMailing.Style.Add("Display", "none");
                    ddlAuStatesMailing.Style.Add("Display", "none");
                    txtBillStateMailing.Style.Add("Display", "");
                    txtBillStateMailing.Text = Convert.ToString(dt.Rows[0]["MAILING_STATE"]);
                }

                ddlAuStatesMailing.ClearSelection();
                try {
                    ddlAuStatesMailing.Items.FindByValue(Convert.ToString(dt.Rows[0]["MAILING_STATE"])).Selected = true;
                }
                catch { }
               

                txtPostalMailing.Text = Convert.ToString(dt.Rows[0]["MAILING_ZIPCODE"]);
                txtCityMailing.Text = Convert.ToString(dt.Rows[0]["MAILING_CITY"]);
                txtAddressMailing2.Text = Convert.ToString(dt.Rows[0]["MAILING_ADDRESS2"]);
                txtAddressMailing1.Text = Convert.ToString(dt.Rows[0]["MAILING_ADDRESS1"]);
                txtAbnAcn.Text = Convert.ToString(dt.Rows[0]["BILLING_ABNACN"]);
                txtMobile.Text = Convert.ToString(dt.Rows[0]["BILLING_MOBILE"]);

              //  objOrder.Mailing_State = ddlAuStatesMailing.SelectedItem.Value;
              //  objOrder.Mailing_Country = ddlCountryMailing.SelectedItem.Value;
                //objOrder.Mailing_Zipcode = txtPostalMailing.Text;
                //objOrder.Mailing_City = txtCityMailing.Text;
                //objOrder.Mailing_Address2 = txtAddressMailing2.Text;
                //objOrder.Mailing_Address1 = txtAddressMailing1.Text;
                //objOrder.BillingAbnacn = txtAbnAcn.Text;
                //objOrder.BillingMobile = txtMobile.Text;

                //objOrder.BillFName = txtFirstName.Text.Trim();
                //objOrder.BillLName = txtLastName.Text.Trim();
                //objOrder.BillEmail = txtEmail.Text.Trim();
                //objOrder.BillPhone = txtPhone.Text.Trim();
                //objOrder.Facsimile = txtFax.Text;
                //objOrder.CompanyName = txtCompany.Text;
                //objOrder.BillAddress1 = txtAddress1.Text.Trim();
                //objOrder.BillAddress2 = txtAddress2.Text.Trim();
                //objOrder.BillCity = txtCity.Text.Trim();


                //objOrder.BillCountry = ddlCountry.SelectedItem.Value;
                //if (ddlCountry.SelectedValue == "United States")
                //{
                //    objOrder.BillState = ddlBillState.SelectedValue;
                //}
                //else if (ddlCountry.SelectedValue == "Australia")
                //{
                //    objOrder.BillState = ddlAuStates.SelectedValue;
                //}


                //else
                //{
                //    objOrder.BillState = txtBillState.Text.Trim();
                //}
               
              //  objOrder.BillZipCode = txtPostal.Text.Trim();
                
            }
        
        }
        private void BindPOSPaymentOptionInv()
        {
            clsPosPayment objPosPayment = new clsPosPayment();
            DataTable dt = objPosPayment.GetPosPaymentListing(string.Empty);
            DataView dv = dt.DefaultView;
            dv.RowFilter = "PAYMENT_ID<>13";
            if (dv.Count > 0)
            {
                ddlPOSPaymentOption.DataSource = dv;
                ddlPOSPaymentOption.DataTextField = "PAYMENTOPTION";
                ddlPOSPaymentOption.DataValueField = "PAYMENT_ID";
                ddlPOSPaymentOption.DataBind();
            }
            dt.Clear();
            objPosPayment = null;
        }
        //public override void VerifyRenderingInServerForm(Control control)
        //{

        //}
        public string Receipt
        {
            get
            {
                //string email = lblCardNumber.Text;
                //if (lblCardNumber.Text != "")
                //{
                //    lblCardNumber.Text = "************" + lblCardNumber.Text.Substring(lblCardNumber.Text.Length - 4, 4);
                //    ;
                //}
              
                StringBuilder stringBuilder = new StringBuilder();
                StringWriter stringWriter = new StringWriter(stringBuilder);
                HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

                tblInvoice.RenderControl(htmlWriter);
                //((HtmlTable)Page.FindControl("tblInvoice")).RenderControl(htmlWriter);
              //  lblCardNumber.Text = email;
                ltrStart.Text = "";
                ltrEnd.Text = "";
                return stringBuilder.ToString();
            }
        }
        protected void ddlDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            int eventid = Convert.ToInt32(ddlEvent.SelectedValue);
            int dateid = Convert.ToInt32(ddlDate.SelectedValue);
            FillInvoiceDetails(eventid, dateid);
            ddlInvoiceNo.Enabled = true;
        }
        private void FillInvoiceDetails(int EventID,int DateID)
        {

            objEvent = new Event();
            DataTable dt = objEvent.GetInvoiceListing(EventID,DateID);
            DataView dv = dt.DefaultView;
            //if (ddlInvoiceNo.Items.Count == 1)
            //    ddlInvoiceNo.Items.RemoveAt(0);
            ddlInvoiceNo.Items.Clear();
            dv.RowFilter = "ORDER_ID>0";

            if (dv.Count > 0)
            {
                ddlInvoiceNo.DataSource = dv;
                ddlInvoiceNo.DataTextField = "INVOICE_NO";
                ddlInvoiceNo.DataValueField = "INVOICE_NO";
                ddlInvoiceNo.DataBind();
                ListItem li = new ListItem("Select Invoice No", "0");
                ddlInvoiceNo.Items.Insert(0, li);
            }
            else
            {
                ListItem li = new ListItem("No Invoice Found", "0");
                ddlInvoiceNo.Items.Insert(0, li);
                ddlInvoiceNo.SelectedIndex = 0;
            }
        }

        private void FillInvoiceStatus()
        {

            objEvent = new Event();
            DataTable dt = objEvent.GetInvoicestatus();
        

            if (dt.Rows.Count > 0)
            {
                ddlInvoiceStatus.DataSource = dt;
                ddlInvoiceStatus.DataTextField = "INV_STATUS_NAME";
                ddlInvoiceStatus.DataValueField = "INV_STATUS_VALUE";
                ddlInvoiceStatus.DataBind();
                //ListItem li = new ListItem("Select Invoice No", "0");
                //ddlInvoiceNo.Items.Insert(0, li);
            }
            else
            {
                ListItem li = new ListItem("No Invoice Status Found", "0");
                ddlInvoiceNo.Items.Insert(0, li);
                ddlInvoiceNo.SelectedIndex = 0;
            }
        }


     


#endregion
    }
}