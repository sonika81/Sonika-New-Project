using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using SSO.Controls;
/// <summary>
// created on : 14 dec,2010
// created by : Vikas Rana
// Purpose : To show quick order form          
/// </summary>
namespace SSO
{
    public partial class quickorder : System.Web.UI.Page
    {
        #region DECLARATION
        ProductClass objProduct = new ProductClass();
        OrderClass objOrder = new OrderClass();
        ProductBundleClass objBundle = new ProductBundleClass();
        ProductAttributeClass objAtt = new ProductAttributeClass();
        ShoppingCartClass objCart = new ShoppingCartClass();
        ProductBundleCart objBundleCart = new ProductBundleCart();
        protected int index;
        DateTime takeDate;
        EWService.EWservice objEWService = new EWService.EWservice();
        protected bool isTuckShop = false; 
        protected List<int> stocks;
        public class BundleCart
        {
            private ProductBundleCart _Cart;
            public ProductBundleCart Cart
            {
                get
                {
                    if (_Cart == null)
                        _Cart = new ProductBundleCart();
                    return _Cart;
                }
                set { _Cart = value; }
            }
            private List<ProductBundleCartDetail> _Detail;
            public List<ProductBundleCartDetail> Detail
            {
                get
                {
                    if (_Detail == null)
                        _Detail = new List<ProductBundleCartDetail>();
                    return _Detail;
                }
                set { _Detail = value; }
            }
        }
        #endregion

        #region PAGE LOAD
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = SSO.CommonClass.SetPageTitle("Quick Order Form");
            if (!IsPostBack)
            {
                BindStudent();
                BindMealSession();
                BindData();
               
            }
            if (Request.QueryString["upqty"] != null)
            {
                      if (Convert.ToString(Request.QueryString["upqty"]) == "yes")
                      {
                                ExceptionHandlingClass.PrintMsg(litMsg, "CUSTOM_MSG,@fieldName-Product quantity updated successfully.");
                                tdError.Visible = true;
                                tdError.Attributes.Add("class", "passed");
                      }
                      if (Convert.ToString(Request.QueryString["upqty"]) == "invalid")
                      {
                                ExceptionHandlingClass.PrintMsg(litMsg, "CUSTOM_MSG,@fieldName-Product quantity must be greater than 0.");
                                tdError.Visible = true;
                                tdError.Attributes.Add("class", "error");
                      }
                      if (Convert.ToString(Request.QueryString["upqty"]) == "rchdmax")
                      {
                                if (Convert.ToString(Request.QueryString["max"]) != null)
                                {
                                          string qty = "";
                                          qty = Convert.ToString(Request.QueryString["max"]);
                                          ExceptionHandlingClass.PrintMsg(litMsg, "CUSTOM_MSG,@fieldName-Maximum Quantity allowed for product is :" + qty);
                                          tdError.Visible = true;
                                          tdError.Attributes.Add("class", "error");
                                }

                      }


            }
            if (ConfigClass.Heading.ToLower() == "tuckshop")
                isTuckShop = true;
            SetStudentBreadCrumb();
            GetEwalletAccontBalance();
        }
        private void SetStudentBreadCrumb()
        {
            trStudent.Style.Add("display", "none");
            if (ConfigClass.Heading.ToLower() == "tuckshop")
            {
                trStudent.Style.Add("display", "block");
                var studentName = "";
                var mealType = "";
                var date = "";
                litStuBredCrumb.Text = "";
                ContentPlaceHolder chpContent = (ContentPlaceHolder)this.Master.Master.FindControl("chpContent");
                DropDownList ddlStudent = (DropDownList)(chpContent.FindControl("ddlStudent"));
                SSO.Controls.SimpleTextBox txtDate = (SSO.Controls.SimpleTextBox)(chpContent.FindControl("txtDate"));
                DropDownList ddlMealType = (DropDownList)(chpContent.FindControl("ddlMealType"));
                if (Session["tuckshopstuname"] != null)
                {
                    studentName = Convert.ToString(Session["tuckshopstuname"]);
                }
                if (Session["tuckshopmealtype"] != null)
                {
                    mealType = Convert.ToString(Session["tuckshopmealtype"]);
                }
                if (Session["tuckshoptakedate"] != null)
                {
                    try
                    {
                        date = ((DateTime.ParseExact(Convert.ToString(Session["tuckshoptakedate"]), "dd/MM/yyyy", null))).ToString("dd/MM/yyyy");
                    }
                    catch (Exception)
                    {
                    }
                }



                if (studentName != "")
                {
                    litStuBredCrumb.Text += studentName;
                }

                if (mealType != "")
                {
                    if (studentName != "")
                    {
                        litStuBredCrumb.Text += " << ";
                    }
                    litStuBredCrumb.Text += mealType;
                }


                if (date != "")
                {
                    if (studentName != "" || mealType != "")
                    {
                        litStuBredCrumb.Text += " << ";
                    }
                    litStuBredCrumb.Text += date;
                }
            }

        }
        #endregion

        #region BIND DATA
        private void BindData()
        {
            stocks = new List<int>();
            Schools objSchool = new Schools();
            objSchool.SchoolId = Convert.ToInt32(Session["SchoolID"]);
            objSchool.GetSchoolDetail();
            if (!objSchool.IsAbsorb)
            {
                ViewState["surchargeMin"] = objSchool.EstoreMinSurcharge;
                ViewState["surchargePer"] = objSchool.EstorePercentSurcharge;
                ViewState["surchargeMax"] = objSchool.EstoreMaxSurcharge;
            }
            else
            {
                ViewState["surchargeMin"] = ViewState["surchargePer"] = ViewState["surchargeMax"] = 0;
                ViewState["ABSORB_STYLE"] = "style='display:none;'";
            }

            if (objSchool.PaypalAbsorb)
            {
                ViewState["PaypalFees"] = 0;
                ViewState["PAYPAL_STYLE"] = "style='display:none;'";
            }
            else
                ViewState["PaypalFees"] = ConfigClass.PaypalFees;

            ViewState["PAYPAL_STYLE"] = "style='display:none;'";
            ViewState["PaypalFees"] = 0;
            index = 0;
            objOrder.SchoolID = objSchool.SchoolId;
            DataSet ds = objOrder.QuickOrderList();
             if (ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count > 0)
             {
                rptCategories.DataSource = ds;
                rptCategories.DataBind();
            }
            else
            {
                rptCategories.Visible = false;
                //  divCategories.Style.Add("display", "none");
            }

            ds = objOrder.QuickOrderBundles();
            if (ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count > 0)
            {
                rptBundles.DataSource = ds;
                rptBundles.DataBind();
            }
            else
            {
                rptBundles.Visible = false;
                // divBundle.Style.Add("display", "none");
            }
        }
        private bool BindProductAttribute(RepeaterItem item)
        {
            Label lblStatus = (item.FindControl("lblStatus") ?? GetRepeaterItem(item).FindControl("lblAvailable")) as Label;
            Literal lit1Text = item.FindControl("lit1Text") as Literal;
            Literal lit2Text = item.FindControl("lit2Text") as Literal;
            DropDownList ddl1Value = item.FindControl("ddl1Value") as DropDownList;
            DropDownList ddl2Value = item.FindControl("ddl2Value") as DropDownList;
            var txtQuantity = item.FindControl("ddl2Value") as SSO.Controls.NumericTextBox;
            bool flag = false;
            objAtt.ProductID = objProduct.ProductId;
            DataView view = objAtt.GetAllProductAttribute(string.Empty).Tables[0].DefaultView;
            view.RowFilter = "IS_ACTIVE = 1 AND ((PREORDER = 0 AND QUANTITY > 0) OR (PREORDER = 1 AND (PREORDER_LIMIT + QUANTITY) > 0))";
            if (view.Count == 0)
            {
                if (lblStatus != null)
                    lblStatus.Text = "(Out of Stock)";
                flag = false;
            }
            view.RowFilter = "IS_ACTIVE = 1";
            flag = view.Count > 0;
            view.Sort = "DISPLAY_ORDER";
        //    view.Sort = "LABEL1VALUE";
            if (view.Count > 0)
            {
                string value = string.Empty;
                foreach (DataRowView row in view)
                {
                    value = Convert.ToString(row["LABEL1VALUE"]);
                    if (ddl1Value.Items.FindByValue(value) == null)
                        ddl1Value.Items.Add(new ListItem(value, value));
                }
                Session["ProductDetail"] = view;
                lit1Text.Text = Convert.ToString(view[0]["LABEL1TEXT"]);
                if (Convert.ToString(view[0]["LABEL2TEXT"])!="")
                lit2Text.Text = Convert.ToString(view[0]["LABEL2TEXT"])+":";

                //view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND ((PREORDER = 0 AND QUANTITY > 0) OR (PREORDER = 1 AND (PREORDER_LIMIT + QUANTITY) > 0))", ddl1Value.SelectedValue);
                view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL2VALUE <>'' AND LABEL1VALUE = '{0}'", ddl1Value.SelectedValue.Replace("'", "''"));
                view.Sort = "LABEL2VALUE";
                if (view.Count < 1)
                {
                    ddl2Value.Visible = false;
                    lit2Text.Visible = false;
                    lit2Text.Text = "";
                }
                else
                {
                    lit2Text.Text = view[0]["LABEL2TEXT"].ToString() + ":";
                    ddl2Value.Visible = true;
                    lit2Text.Visible = true;
                }
                ddl2Value.DataSource = view;
                ddl2Value.DataBind();
                view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND LABEL2VALUE = '{1}' AND ((PREORDER = 0 AND QUANTITY > 0) OR (PREORDER = 1 AND (PREORDER_LIMIT + QUANTITY) > 0))", ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
                if (view.Count == 0)
                {
                    if (lblStatus != null)
                        lblStatus.Text = "(Out of Stock)";
                    flag = false;
                }
            }
            if (!string.IsNullOrEmpty(lblStatus.Text))
                stocks.Add(objProduct.ProductId);
            return flag;
        }
        #endregion

        #region COMMON FUNCTION
        public string getPrice(object absorb, object price)
        {
            return ConfigClass.GetPriceExcludingGST(Convert.ToBoolean(absorb), Convert.ToDecimal(price)).ToString("F2");
        }

        public string getGST(object absorb, object price)
        {
            return ConfigClass.GetGST(Convert.ToBoolean(absorb), Convert.ToDecimal(price)).ToString("F2");
        }

        public string getProductPrice(object absorb, decimal salePrice, object retailPrice)
        {
            return salePrice > 0 ? getPrice(absorb, salePrice) : getPrice(absorb, retailPrice);
        }

        public string getProductGST(object absorb, decimal salePrice, object retailPrice)
        {
            return salePrice > 0 ? getGST(absorb, salePrice) : getGST(absorb, retailPrice);
        }

        private string GenerateProductCode(string size, string color)
        {
            string productCode = (objProduct.ProductCode ?? "").Trim() + "_" + objProduct.CategoryId;
            if (color != "" && size == "")
                productCode = productCode + "_" + color.Substring(0, 1).ToUpper();
            else if (color == "" && size != "")
                productCode = productCode + "_" + size;
            else if (color != "" && size != "")
                productCode = productCode + "_" + color.Substring(0, 1).ToUpper() + "_" + size;
            return productCode;
        }

        protected void ddl1ValueProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl1Value = sender as DropDownList;
            RepeaterItem item = GetRepeaterItem(ddl1Value);
            DropDownList ddl2Value = item.FindControl("ddl2Value") as DropDownList;
            Literal lit2Text = item.FindControl("lit2Text") as Literal;

            objAtt.ProductID = Convert.ToInt32((item.FindControl("hdnProductID") as HiddenField).Value);
            DataView view = objAtt.GetAllProductAttribute(string.Empty).Tables[0].DefaultView;
            view.RowFilter = string.Format("IS_ACTIVE = 1 and LABEL2VALUE<>'' AND LABEL1VALUE = '{0}'", ddl1Value.SelectedValue.Replace("'", "''"));
            view.Sort = "LABEL2VALUE";
            if (view.Count < 1)
            {
                ddl2Value.Visible = false;
                lit2Text.Visible = false;
                lit2Text.Text = "";
            }
            else
            {
                ddl2Value.Visible = true;
                lit2Text.Text = view[0]["LABEL2TEXT"].ToString()+":";
                lit2Text.Visible = true;
            }
            ddl2Value.DataSource = view;
            ddl2Value.DataBind();
            ddl2Value.Focus();
            view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND LABEL2VALUE = '{1}' AND ((PREORDER = 0 AND QUANTITY > 0) OR (PREORDER = 1 AND PREORDER_LIMIT + QUANTITY > 0))", ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
            SetStockMessage(view.Count, item);
        }

        protected void ddl2ValueProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl2Value = sender as DropDownList;
            RepeaterItem item = GetRepeaterItem(ddl2Value);
            DropDownList ddl1Value = item.FindControl("ddl1Value") as DropDownList;

            objAtt.ProductID = Convert.ToInt32((item.FindControl("hdnProductID") as HiddenField).Value);
            DataView view = objAtt.GetAllProductAttribute(string.Empty).Tables[0].DefaultView;
            view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND LABEL2VALUE = '{1}' AND ((PREORDER = 0 AND QUANTITY > 0) OR (PREORDER = 1 AND PREORDER_LIMIT + QUANTITY > 0))", ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
            SetStockMessage(view.Count, item);
        }

        private void SetStockMessage(int count, RepeaterItem item)
        {
            int index = Convert.ToInt32(((item.FindControl("hdnQuantity") ?? item.FindControl("hdnBundleQuantity")) as HiddenField).Value);
            var txtQuantity = (item.FindControl("txtQuantity") ?? item.FindControl("txtBundleQuantity")) as NumericTextBox;
            if (count == 0)
            {
                txtQuantity.Enabled = false;
                txtQuantity.Value = 0;
                ((item.FindControl("lblStatus") ?? item.FindControl("lblAvailable")) as Label).Text = "(Out of Stock)";
            }
            else
            {
                txtQuantity.Enabled = true;
                txtQuantity.Value = 1;
                ((item.FindControl("lblStatus") ?? item.FindControl("lblAvailable")) as Label).Text = string.Empty;
            }
            
            ScriptManager.RegisterClientScriptBlock(UpdatePanel1, UpdatePanel1.GetType(), "calPrice", string.Format("calPrice({0},{1});", index, txtQuantity.Value), true);
        }

        protected void ddl1ValueBundle_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl1Value = sender as DropDownList;
            Repeater rptBundleProducts = GetRepeater(ddl1Value);
            RepeaterItem rptBundleProductItem = GetRepeaterItem(ddl1Value);
            RepeaterItem rptBundleItem = GetRepeaterItem(rptBundleProducts);
            DropDownList ddl2Value = rptBundleProductItem.FindControl("ddl2Value") as DropDownList;

            objAtt.ProductID = Convert.ToInt32((rptBundleProductItem.FindControl("hdnProductID") as HiddenField).Value);
            DataView view = objAtt.GetAllProductAttribute(string.Empty).Tables[0].DefaultView;
            view.RowFilter = string.Format("IS_ACTIVE = 1 and LABEL2VALUE<>'' AND LABEL1VALUE = '{0}'", ddl1Value.SelectedValue.Replace("'", "''"));
            view.Sort = "LABEL2VALUE";
            if (view.Count < 1)
            {
                ddl2Value.Visible = false;
                lit2Text.Visible = false;
                lit2Text.Text = "";
            }
            else
            {
                ddl2Value.Visible = true;
                lit2Text.Text = view[0]["LABEL2TEXT"].ToString() + ":";
                lit2Text.Visible = true;
            }

            ddl2Value.DataSource = view;
            ddl2Value.DataBind();
            ddl2Value.Focus();
            view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND LABEL2VALUE = '{1}' AND ((PREORDER = 0 AND QUANTITY > 0) OR (PREORDER = 1 AND PREORDER_LIMIT + QUANTITY > 0))", ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
            SetStockMessage(view.Count, rptBundleItem);

        }

        protected void ddl2ValueBundle_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl2Value = sender as DropDownList;
            Repeater rptBundleProducts = GetRepeater(ddl2Value);
            RepeaterItem rptBundleProductItem = GetRepeaterItem(ddl2Value);
            RepeaterItem rptBundleItem = GetRepeaterItem(rptBundleProducts);
            DropDownList ddl1Value = rptBundleProductItem.FindControl("ddl1Value") as DropDownList;

            objAtt.ProductID = Convert.ToInt32((rptBundleProductItem.FindControl("hdnProductID") as HiddenField).Value);
            DataView view = objAtt.GetAllProductAttribute(string.Empty).Tables[0].DefaultView;
            view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND LABEL2VALUE = '{1}' AND ((PREORDER = 0 AND QUANTITY > 0) OR (PREORDER = 1 AND PREORDER_LIMIT + QUANTITY > 0))", ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
            SetStockMessage(view.Count, rptBundleItem);
        }

        private Repeater GetRepeater(Control ddl)
        {
            if (ddl.Parent != null)
                if ((ddl.Parent as Repeater) != null)
                    return ddl.Parent as Repeater;
                else
                    return GetRepeater(ddl.Parent);
            else return null;
        }

        private RepeaterItem GetRepeaterItem(Control ddl)
        {
            if (ddl.Parent != null)
                if ((ddl.Parent as RepeaterItem) != null)
                    return ddl.Parent as RepeaterItem;
                else
                    return GetRepeaterItem(ddl.Parent);
            else return null;
        }
        #endregion

        #region REPEATER EVENTS
        protected void rptCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    DataRowView row = e.Item.DataItem as DataRowView;
                    Repeater rptProducts = e.Item.FindControl("rptProducts") as Repeater;
                    DataSet ds = objOrder.QuickOrderProducts(Convert.ToInt32(row["CATEGORY_ID"]));
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        rptProducts.DataSource = ds;
                        rptProducts.DataBind();
                    }
                    else
                    {
                       HtmlAnchor aCategory = e.Item.FindControl("aCategory") as HtmlAnchor;
                       aCategory.Style.Add("display", "none");
                    }
                    break;
            }
        }

        protected void rptProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    stocks.Clear();
                    var txtQuantity = e.Item.FindControl("txtQuantity") as SSO.Controls.NumericTextBox;
                    if (!string.IsNullOrEmpty(Convert.ToString(DataBinder.Eval(e.Item.DataItem, "AVAILABLE"))))
                    {
                              txtQuantity.Enabled = false;
                              txtQuantity.Text = "0";
                    }
                    txtQuantity.Attributes.Add("onchange", string.Format("calPrice({0},this.value);", index));
                    index++;
                    objProduct.ProductId = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "PRODUCT_ID"));
                    objProduct.GetProductDetails();
                    if (objProduct.HasAttribute)
                    {
                        (e.Item.FindControl("tblAttribute") as HtmlTable).Visible = true;
                        BindProductAttribute(e.Item);
                        if (stocks.Count > 0)
                        {
                                  txtQuantity.Enabled = false;
                                  txtQuantity.Text = "0";
                        }
                    }
                    break;
            }
        }

        protected void rptBundles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    stocks.Clear();
                    var txtQuantity = e.Item.FindControl("txtBundleQuantity") as SSO.Controls.NumericTextBox;
                    txtQuantity.Attributes.Add("onchange", string.Format("calPrice({0},this.value);", index));
                    index++;
                    objBundle.BundleID = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "BUNDLE_ID"));
                    var rptBundleProducts = e.Item.FindControl("rptBundleProducts") as Repeater;
                    rptBundleProducts.DataSource = objBundle.ProductBundleSelectedProducts();
                    rptBundleProducts.DataBind();
                    if (stocks.Count > 0)
                    {
                              txtQuantity.Enabled = false;
                              txtQuantity.Text = "0";
                    }
                    break;
            }
        }

        protected void rptBundleProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    objProduct.ProductId = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "PRODUCT_ID"));
                    objProduct.GetProductDetails();
                    if (objProduct.HasAttribute)
                    {
                        (e.Item.FindControl("tblAttribute") as HtmlTable).Visible = true;
                        BindProductAttribute(e.Item);
                    }
                    break;
            }
        }
        #endregion

        #region SUBMIT
        protected void lnkSubmit_Click(object sender, EventArgs e)
        {
                  ConfigClass.Redirect("shoppingcart.aspx");
            ////
            if (ConfigClass.Heading.ToLower() == "tuckshop")
            {
                ContentPlaceHolder chpContent = (ContentPlaceHolder)this.Master.Master.FindControl("chpContent");
                DropDownList ddlStudent = (DropDownList)(chpContent.FindControl("ddlStudent"));
                SSO.Controls.SimpleTextBox txtDate = (SSO.Controls.SimpleTextBox)(chpContent.FindControl("txtDate"));
                DropDownList ddlMealType = (DropDownList)(chpContent.FindControl("ddlMealType"));

                if (ddlStudent.SelectedValue == "" || txtDate.Text == "" || ddlMealType.SelectedValue == "")
                {
                    ExceptionHandlingClass.PrintError(litMsg, "STUDENT_INFO,@fieldName-");
                    tdError.Visible = true;
                    tdError.Attributes.Add("class", "error");
                    return;
                }

                if (DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", null) < DateTime.Now.Date)
                {
                    ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName- Date cannot be less than current date");
                    tdError.Visible = true;
                    tdError.Attributes.Add("class", "error");
                    return;

                }
                try
                {
                    takeDate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", null);
                }
                catch (Exception ex)
                {
                    ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName- Please enter valid Date");
                    tdError.Visible = true;
                    tdError.Attributes.Add("class", "error");
                    return;
                }

                Session["tuckshopstuid"] = ddlStudent.SelectedValue;
                Session["tuckshopmealtype"] = ddlMealType.SelectedValue;
                Session["tuckshoptakedate"] = txtDate.Text;
                Session["tuckshopstuname"] = ddlStudent.SelectedItem.Text;

            }
            //////

            int quantity;
            int addedQuantity;
            int totalQuantity;
            int quantityToAdd;
            ShoppingCartClass cart;
            List<ShoppingCartClass> lstCart = new List<ShoppingCartClass>();
            DropDownList ddl1Value = null, ddl2Value = null;
            foreach (RepeaterItem i in rptCategories.Items)
                foreach (RepeaterItem item in (i.FindControl("rptProducts") as Repeater).Items)
                {
                    quantity = (item.FindControl("txtQuantity") as SSO.Controls.NumericTextBox).Value;
                    if (quantity > 0)
                    {
                        addedQuantity = totalQuantity = quantityToAdd = 0;
                        cart = new ShoppingCartClass();

                        cart.MemberId = Convert.ToInt32(Session["MemberID"]);
                        DataSet ds = cart.FetchItemsForCart();

                        objAtt.ProductID = objProduct.ProductId = Convert.ToInt32((item.FindControl("hdnProductID") as HiddenField).Value);
                        objProduct.SchoolId = Convert.ToInt32(Session["SchoolID"]);
                        objProduct.GetProductDetails();

                        ddl1Value = item.FindControl("ddl1Value") as DropDownList;
                        ddl2Value = item.FindControl("ddl2Value") as DropDownList;

                        if (objProduct.HasAttribute)
                        {
                            DataView view = objAtt.GetAllProductAttribute(string.Empty).Tables[0].DefaultView;
                            view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND LABEL2VALUE = '{1}'", ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
                            if (Convert.ToBoolean(view[0]["PREORDER"]))
                                quantityToAdd = Convert.ToInt32(view[0]["QUANTITY"]) + Convert.ToInt32(view[0]["PREORDER_LIMIT"]);
                            else
                                quantityToAdd = Convert.ToInt32(view[0]["QUANTITY"]);

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                view = ds.Tables[0].DefaultView;
                                view.RowFilter = string.Format("PRODUCT_ID = {0} AND LABEL1VALUE = '{1}' AND LABEL2VALUE = '{2}'", objProduct.ProductId, ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
                                if (view.Count > 0)
                                    addedQuantity = Convert.ToInt32(view[0]["PRODUCT_QTY"]);
                            }
                        }
                        else
                        {
                            quantityToAdd = objProduct.OrderableQuantity;
                            DataView view = ds.Tables[0].DefaultView;
                            view.RowFilter = "PRODUCT_ID = " + objProduct.ProductId;
                            if (view.Count > 0)
                                addedQuantity = Convert.ToInt32(view[0]["PRODUCT_QTY"]);
                        }
                        if (addedQuantity == 0)
                        {
                            totalQuantity = quantity;
                            if (totalQuantity > quantityToAdd)
                            {
                                ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objProduct.ProductName + "' is " + quantityToAdd);
                                tdError.Visible = true;
                                tdError.Attributes.Add("class", "error");
                                return;
                            }
                        }
                        else
                        {
                            totalQuantity = quantity + addedQuantity;
                            if (totalQuantity > quantityToAdd)
                            {
                                ExceptionHandlingClass.PrintMsg(litMsg, string.Format(@"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objProduct.ProductName + "' is {0}. You have already added {1}.", quantityToAdd, addedQuantity));
                                tdError.Attributes.Add("class", "error");
                                tdError.Visible = true;
                                return;
                            }
                        }
                        cart.CustomerId = Convert.ToInt32(Session["MemberID"]);
                        cart.SessionId = Session.SessionID;
                        cart.Label1Value = ddl1Value.SelectedValue;
                        cart.Label2Value = ddl2Value.SelectedValue;
                        cart.ProductId = objProduct.ProductId;
                        cart.Barcode = objProduct.Barcode;
                        cart.Gender = Convert.ToChar(objProduct.Gender.Substring(0, 1));
                        cart.Quantity = quantity;
                        cart.ProductCode = GenerateProductCode(cart.Label2Value, cart.Label1Value);
                        //
                         if (ConfigClass.Heading.ToLower() == "tuckshop")
                         { 
                             cart.StudentId = Convert.ToInt32(Session["tuckshopstuid"]);
                             cart.Mealtype  = Convert.ToString(Session["tuckshopmealtype"]);
                             cart.TakeDate =takeDate ;
                         }
                        //

                        ConfigClass.CartPriceWithGST(objProduct, ref cart);
                        lstCart.Add(cart);
                    }
                }

            List<BundleCart> pCarts = new List<BundleCart>();
            foreach (RepeaterItem item in rptBundles.Items)
            {
                quantity = (item.FindControl("txtBundleQuantity") as SSO.Controls.NumericTextBox).Value;
                if (quantity > 0)
                {
                    objBundleCart = new ProductBundleCart();
                    BundleCart pCart = new BundleCart();
                    addedQuantity = totalQuantity = quantityToAdd = 0;

                    objBundle.BundleID = Convert.ToInt32((item.FindControl("hdnBundleID") as HiddenField).Value);
                    objBundle.SchoolID = Convert.ToInt32(Session["SchoolID"]);
                    objBundle.ProductBundleDetails(true);
                    objBundleCart.CustomerID = Convert.ToInt32(Session["MemberID"]);
                    DataSet dsBundleCart = objBundleCart.FetchItemsForCart();
                    DataView dvBundleCart = new DataView(dsBundleCart.Tables[1]);
                    dvBundleCart.RowFilter = "BUNDLE_ID = " + objBundle.BundleID;
                    if (dvBundleCart.Count > 0)
                        addedQuantity = Convert.ToInt32(dvBundleCart[0]["PRODUCT_QTY"]);
                    totalQuantity = addedQuantity + quantity;

                    if (totalQuantity > objBundle.OrderableQuantity)
                    {
                        if (addedQuantity > 0)
                        {
                            ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objBundle.Name + "' is " + objBundle.OrderableQuantity + ". You have already added " + addedQuantity + " .");
                            tdError.Visible = true;
                            tdError.Attributes.Add("class", "error");
                            return;
                        }
                        else
                        {
                            ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objBundle.Name + "' is " + objBundle.OrderableQuantity);
                            tdError.Visible = true;
                            tdError.Attributes.Add("class", "error");
                            return;
                        }
                    }
                    List<ProductBundleCartDetail> cartItems = new List<ProductBundleCartDetail>();

                    foreach (RepeaterItem item1 in (item.FindControl("rptBundleProducts") as Repeater).Items)
                    {
                        HiddenField hdnQuantity = item1.FindControl("hdnQuantity") as HiddenField;
                        quantityToAdd = Convert.ToInt32(hdnQuantity.Value) * quantity;
                        addedQuantity = 0;
                        totalQuantity = 0;
                        //availableQunatity = 0;

                        objProduct.SchoolId = Convert.ToInt32(Session["SchoolID"]);
                        objProduct.ProductId = Convert.ToInt32((item1.FindControl("hdnProductID") as HiddenField).Value);
                        objProduct.GetProductDetails();
                        ProductBundleCartDetail objCartDetail = new ProductBundleCartDetail() { ProductID = objProduct.ProductId, Code = objProduct.ProductCode, ProductQuantity = Convert.ToInt32(hdnQuantity.Value) };

                        if (objProduct.HasAttribute)
                        {
                            ddl1Value = item1.FindControl("ddl1Value") as DropDownList;
                            ddl2Value = item1.FindControl("ddl2Value") as DropDownList;
                            objCartDetail.Label1Value = ddl1Value.SelectedValue;
                            objCartDetail.Label2Value = ddl2Value.SelectedValue;
                        }
                        else
                            objCartDetail.Label1Value = objCartDetail.Label2Value = null;
                        cartItems.Add(objCartDetail);
                    }

                    dvBundleCart.RowFilter = "BUNDLE_ID = " + objBundle.BundleID;
                    dvBundleCart = new DataView(dsBundleCart.Tables[0]);
                    DataView dvAddedProducts;
                    int totalItems = 0;
                    bool isNew = true;
                    quantityToAdd = quantity;
                    ProductBundleCartDetail objCartDetail1 = new ProductBundleCartDetail();
                    if (dvBundleCart.Count > 0) // check for similar bundles
                        foreach (DataRowView drvCartItem in dvBundleCart)
                        {
                            totalItems = 0;
                            objCartDetail1.CartID = Convert.ToInt32(drvCartItem["CART_ID"]);
                            dvAddedProducts = new DataView(objCartDetail1.FetchCartDetails().Tables[0]);
                            foreach (DataRowView drvAddedProduct in dvAddedProducts)
                                foreach (ProductBundleCartDetail cartItem in cartItems)
                                {
                                    if (cartItem.ProductQuantity == Convert.ToInt32(drvAddedProduct["QTY"]) &&
                                        CommonClass.CompareIgnoreCase(cartItem.ProductID, drvAddedProduct["PRODUCT_ID"]) &&
                                            CommonClass.CompareIgnoreCase(cartItem.Label1Value, drvAddedProduct["COLOR"]) &&
                                            CommonClass.CompareIgnoreCase(cartItem.Label2Value, drvAddedProduct["SIZE"]))
                                    {
                                        totalItems++;
                                        break;
                                    }
                                }
                            if (cartItems.Count == totalItems && dvAddedProducts.Count == totalItems)
                            {
                                isNew = false;
                                objBundleCart.CartID = objCartDetail1.CartID;
                                quantityToAdd += Convert.ToInt32(drvCartItem["QUANTITY"]);
                                break;
                            }
                        }
                    objBundleCart.CustomerID = Convert.ToInt32(Session["MemberID"]);
                    objBundleCart.Quantity = quantityToAdd;
                    objBundleCart.BundleID = objBundle.BundleID;
                    objBundleCart.Code = objBundle.Code;
                    objBundleCart.Gender = objBundle.Gender;
                    //
                    if (ConfigClass.Heading.ToLower() == "tuckshop")
                    {
                        objBundleCart.StudentId = Convert.ToInt32(Session["tuckshopstuid"]);
                        objBundleCart.Mealtype = Convert.ToString(Session["tuckshopmealtype"]);
                        objBundleCart.TakeDate = takeDate;
                    }
                    //
                    ConfigClass.BundleCartPriceWithGST(objBundle, ref objBundleCart);
                    pCart.Cart = objBundleCart;
                    if (isNew)
                        pCart.Detail = cartItems;
                    pCarts.Add(pCart);
                }
            }

            //save product cart
            foreach (var item in lstCart)
                item.AddCart();

            //save bundle cart
            foreach (var item in pCarts)
            {
                var bundleCart = item.Cart;
                if (bundleCart.AddCart())
                    foreach (var detail in item.Detail)
                    {
                        var bundleDetail = detail;
                        bundleDetail.CartID = bundleCart.CartID;
                        detail.AddToCart();
                    }
            }
            if (lstCart.Count > 0 || pCarts.Count > 0)
                ConfigClass.Redirect("shoppingcart.aspx");
            else
            {
                ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-Please select at least one Product");
                tdError.Visible = true;
                tdError.Attributes.Add("class", "error");
            }
        }
        #endregion

       
        private void BindStudent()
        {
            if (ConfigClass.Heading.ToLower() == "tuckshop")
            {
                trStudent.Style.Add("display", "");
                Student objStudents = new Student();
                objStudents.ParentID = Convert.ToInt32(Session["MemberID"]);
                DataSet ds = objStudents.GetStudents(string.Empty);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlStudent.DataSource = ds;
                    ddlStudent.DataTextField = "FULL_NAME";
                    ddlStudent.DataValueField = "STUDENTID";

                    ddlStudent.DataBind();
                    if (ddlStudent.Items.Count > 1)
                        ddlStudent.Items.Insert(0, new ListItem("Select Student", string.Empty));
                }
                else
                {
                    ddlStudent.Items.Insert(0, new ListItem("Select Student", string.Empty));
                }
                if (Convert.ToString(Session["tuckshopstuid"]) != "")
                    ddlStudent.SelectedValue = Convert.ToString(Session["tuckshopstuid"]);
                if (Convert.ToString(Session["tuckshopmealtype"]) != "")
                    ddlMealType.SelectedValue = Convert.ToString(Session["tuckshopmealtype"]);
                if (Convert.ToString(Session["tuckshoptakedate"]) != "")
                    txtDate.Text = Convert.ToString(Session["tuckshoptakedate"]);
            }
            else
            {
                trStudent.Style.Add("display", "none");
            }
        }

        private void BindMealSession()
        {
            if (ConfigClass.Heading.ToLower() == "tuckshop")
            {
                txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                trStudent.Style.Add("display", "");
                mealsessionclass objmeal = new mealsessionclass();

                DataSet ds = objmeal.GetActiveMealSessionListing(string.Empty);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlMealType.DataSource = ds;
                    ddlMealType.DataTextField = "MEAL_SESSION_NAME";
                    ddlMealType.DataValueField = "MEAL_SESSION_NAME";// "MID";
                    ddlMealType.DataBind();

                    if (ddlMealType.Items.Count > 1)
                        ddlMealType.Items.Insert(0, new ListItem("Select Meal Session", string.Empty));
                }
                else
                {
                    ddlMealType.Items.Insert(0, new ListItem("Select Meal Session", string.Empty));
                }

                if (Convert.ToString(Session["tuckshopstuid"]) != "")
                    ddlStudent.SelectedValue = Convert.ToString(Session["tuckshopstuid"]);
                if (Convert.ToString(Session["tuckshopmealtype"]) != "")
                    ddlMealType.SelectedValue = Convert.ToString(Session["tuckshopmealtype"]);
                if (Convert.ToString(Session["tuckshoptakedate"]) != "")
                    txtDate.Text = Convert.ToString(Session["tuckshoptakedate"]);
            }
            else
            {
                trStudent.Style.Add("display", "none");
            }
        }

        private void SetStudentSectionValues()
        {
            ContentPlaceHolder chpContent = (ContentPlaceHolder)this.Master.Master.FindControl("chpContent");
            DropDownList ddlMealTypeMaster = (DropDownList)(chpContent.FindControl("ddlMealType"));
            DropDownList ddlStudentMaster = (DropDownList)(chpContent.FindControl("ddlStudent"));
            SSO.Controls.SimpleTextBox txtDateMaster = (SSO.Controls.SimpleTextBox)(chpContent.FindControl("txtDate"));


            if (ddlStudent.SelectedValue != "")
            {
                ddlStudentMaster.SelectedValue = ddlStudent.SelectedValue;
                Session["tuckshopstuid"] = ddlStudent.SelectedValue;
                Session["tuckshopstuname"] = ddlStudent.SelectedItem.Text;
            }
            else
            {
                ddlStudentMaster.SelectedValue = ddlStudent.SelectedValue;
                Session["tuckshopstuid"] = "";
                Session["tuckshopstuname"] ="";
            }
            if (ddlMealType.SelectedValue != "")
            {                
                ddlMealTypeMaster.SelectedValue = ddlMealType.SelectedValue;
                Session["tuckshopmealtype"] = ddlMealType.SelectedValue;
            }
            else
            {
                ddlMealTypeMaster.SelectedValue = ddlMealType.SelectedValue;
                Session["tuckshopmealtype"] ="";
            }
            if (txtDate.Text != "")
            {
                if (DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", null) < DateTime.Now.Date)
                {
                    ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName- Date cannot be less than current date");
                    tdError.Visible = true;
                    tdError.Attributes.Add("class", "error");
                    return;

                }
                try
                {
                    takeDate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", null);
                }
                catch (Exception ex)
                {
                    ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName- Please enter valid Date");
                    tdError.Visible = true;
                    tdError.Attributes.Add("class", "error");
                    return;
                }
                
                txtDateMaster.Text = txtDate.Text;
                Session["tuckshoptakedate"] = txtDate.Text;
            }
            else
            {
                txtDateMaster.Text = txtDate.Text;
                Session["tuckshoptakedate"] = "";
            }
            SetStudentBreadCrumb();
        }
        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStudentSectionValues();
        }
        protected void ddlMealType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStudentSectionValues();
        }
        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            SetStudentSectionValues();
        }
        private void GetEwalletAccontBalance()
        {
            decimal balance = 0;
            decimal minimumbalance = 0;
            string accNum = "";
            if (Session["MemberID"] != null)
            {
                var obj = objEWService.GetMinBalance_SSOL(Convert.ToString(Session["MemberID"]));

                foreach (var m in obj)
                {
                    balance = Convert.ToDecimal(m.Balance);
                    minimumbalance = Convert.ToDecimal(m.MinBalance);
                    accNum = Convert.ToString(m.AccountNumber);
                    litEWBalance.Text = balance.ToString();

                   litEWNum.Text = accNum;
                }
            }
            if (accNum == "")
                trEWalletBalance.Style.Add("display", "none");
            else
                trEWalletBalance.Style.Add("display", "");
        }
        protected void rptBundles_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

                  ////
                  if (ConfigClass.Heading.ToLower() == "tuckshop")
                  {
                            ContentPlaceHolder chpContent = (ContentPlaceHolder)this.Master.Master.FindControl("chpContent");
                            DropDownList ddlStudent = (DropDownList)(chpContent.FindControl("ddlStudent"));
                            SSO.Controls.SimpleTextBox txtDate = (SSO.Controls.SimpleTextBox)(chpContent.FindControl("txtDate"));
                            DropDownList ddlMealType = (DropDownList)(chpContent.FindControl("ddlMealType"));

                            if (ddlStudent.SelectedValue == "" || txtDate.Text == "" || ddlMealType.SelectedValue == "")
                            {
                                      ExceptionHandlingClass.PrintError(litMsg, "STUDENT_INFO,@fieldName-");
                                      tdError.Visible = true;
                                      tdError.Attributes.Add("class", "error");
                                      return;
                            }

                            if (DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", null) < DateTime.Now.Date)
                            {
                                      ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName- Date cannot be less than current date");
                                      tdError.Visible = true;
                                      tdError.Attributes.Add("class", "error");
                                      return;

                            }
                            try
                            {
                                      takeDate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", null);
                            }
                            catch (Exception ex)
                            {
                                      ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName- Please enter valid Date");
                                      tdError.Visible = true;
                                      tdError.Attributes.Add("class", "error");
                                      return;
                            }

                            Session["tuckshopstuid"] = ddlStudent.SelectedValue;
                            Session["tuckshopmealtype"] = ddlMealType.SelectedValue;
                            Session["tuckshoptakedate"] = txtDate.Text;
                            Session["tuckshopstuname"] = ddlStudent.SelectedItem.Text;

                  }
                  //////

                  int quantity;
                  int addedQuantity;
                  int totalQuantity;
                  int quantityToAdd;
                  ShoppingCartClass cart;
                  List<ShoppingCartClass> lstCart = new List<ShoppingCartClass>();
                  DropDownList ddl1Value = null, ddl2Value = null;
                  #region commented
                  
                  ////foreach (RepeaterItem i in rptCategories.Items)
                  ////          foreach (RepeaterItem item in (i.FindControl("rptProducts") as Repeater).Items)
                  ////          {
                  ////                    quantity = (item.FindControl("txtQuantity") as SSO.Controls.NumericTextBox).Value;
                  ////                    if (quantity > 0)
                  ////                    {
                  ////                              addedQuantity = totalQuantity = quantityToAdd = 0;
                  ////                              cart = new ShoppingCartClass();

                  ////                              cart.MemberId = Convert.ToInt32(Session["MemberID"]);
                  ////                              DataSet ds = cart.FetchItemsForCart();

                  ////                              objAtt.ProductID = objProduct.ProductId = Convert.ToInt32((item.FindControl("hdnProductID") as HiddenField).Value);
                  ////                              objProduct.SchoolId = Convert.ToInt32(Session["SchoolID"]);
                  ////                              objProduct.GetProductDetails();

                  ////                              ddl1Value = item.FindControl("ddl1Value") as DropDownList;
                  ////                              ddl2Value = item.FindControl("ddl2Value") as DropDownList;

                  ////                              if (objProduct.HasAttribute)
                  ////                              {
                  ////                                        DataView view = objAtt.GetAllProductAttribute(string.Empty).Tables[0].DefaultView;
                  ////                                        view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND LABEL2VALUE = '{1}'", ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
                  ////                                        if (Convert.ToBoolean(view[0]["PREORDER"]))
                  ////                                                  quantityToAdd = Convert.ToInt32(view[0]["QUANTITY"]) + Convert.ToInt32(view[0]["PREORDER_LIMIT"]);
                  ////                                        else
                  ////                                                  quantityToAdd = Convert.ToInt32(view[0]["QUANTITY"]);

                  ////                                        if (ds.Tables[0].Rows.Count > 0)
                  ////                                        {
                  ////                                                  view = ds.Tables[0].DefaultView;
                  ////                                                  view.RowFilter = string.Format("PRODUCT_ID = {0} AND LABEL1VALUE = '{1}' AND LABEL2VALUE = '{2}'", objProduct.ProductId, ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
                  ////                                                  if (view.Count > 0)
                  ////                                                            addedQuantity = Convert.ToInt32(view[0]["PRODUCT_QTY"]);
                  ////                                        }
                  ////                              }
                  ////                              else
                  ////                              {
                  ////                                        quantityToAdd = objProduct.OrderableQuantity;
                  ////                                        DataView view = ds.Tables[0].DefaultView;
                  ////                                        view.RowFilter = "PRODUCT_ID = " + objProduct.ProductId;
                  ////                                        if (view.Count > 0)
                  ////                                                  addedQuantity = Convert.ToInt32(view[0]["PRODUCT_QTY"]);
                  ////                              }
                  ////                              if (addedQuantity == 0)
                  ////                              {
                  ////                                        totalQuantity = quantity;
                  ////                                        if (totalQuantity > quantityToAdd)
                  ////                                        {
                  ////                                                  ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objProduct.ProductName + "' is " + quantityToAdd);
                  ////                                                  tdError.Visible = true;
                  ////                                                  tdError.Attributes.Add("class", "error");
                  ////                                                  return;
                  ////                                        }
                  ////                              }
                  ////                              else
                  ////                              {
                  ////                                        totalQuantity = quantity + addedQuantity;
                  ////                                        if (totalQuantity > quantityToAdd)
                  ////                                        {
                  ////                                                  ExceptionHandlingClass.PrintMsg(litMsg, string.Format(@"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objProduct.ProductName + "' is {0}. You have already added {1}.", quantityToAdd, addedQuantity));
                  ////                                                  tdError.Attributes.Add("class", "error");
                  ////                                                  tdError.Visible = true;
                  ////                                                  return;
                  ////                                        }
                  ////                              }
                  ////                              cart.CustomerId = Convert.ToInt32(Session["MemberID"]);
                  ////                              cart.SessionId = Session.SessionID;
                  ////                              cart.Label1Value = ddl1Value.SelectedValue;
                  ////                              cart.Label2Value = ddl2Value.SelectedValue;
                  ////                              cart.ProductId = objProduct.ProductId;
                  ////                              cart.Barcode = objProduct.Barcode;
                  ////                              cart.Gender = Convert.ToChar(objProduct.Gender.Substring(0, 1));
                  ////                              cart.Quantity = quantity;
                  ////                              cart.ProductCode = GenerateProductCode(cart.Label2Value, cart.Label1Value);
                  ////                              //
                  ////                              if (ConfigClass.Heading.ToLower() == "tuckshop")
                  ////                              {
                  ////                                        cart.StudentId = Convert.ToInt32(Session["tuckshopstuid"]);
                  ////                                        cart.Mealtype = Convert.ToString(Session["tuckshopmealtype"]);
                  ////                                        cart.TakeDate = takeDate;
                  ////                              }
                  ////                              //

                  ////                              ConfigClass.CartPriceWithGST(objProduct, ref cart);
                  ////                              lstCart.Add(cart);
                  ////                    }
                  ////          }
                  #endregion

                  List<BundleCart> pCarts = new List<BundleCart>();
                  //foreach (RepeaterItem item in rptBundles.Items)
                  //{
                            quantity = (e.Item.FindControl("txtBundleQuantity") as SSO.Controls.NumericTextBox).Value;
                            if (quantity > 0)
                            {
                                      objBundleCart = new ProductBundleCart();
                                      BundleCart pCart = new BundleCart();
                                      addedQuantity = totalQuantity = quantityToAdd = 0;

                                      objBundle.BundleID = Convert.ToInt32((e.Item.FindControl("hdnBundleID") as HiddenField).Value);
                                      objBundle.SchoolID = Convert.ToInt32(Session["SchoolID"]);
                                      objBundle.ProductBundleDetails(true);
                                      objBundleCart.CustomerID = Convert.ToInt32(Session["MemberID"]);
                                      DataSet dsBundleCart = objBundleCart.FetchItemsForCart();
                                      DataView dvBundleCart = new DataView(dsBundleCart.Tables[1]);
                                      dvBundleCart.RowFilter = "BUNDLE_ID = " + objBundle.BundleID;
                                      if (dvBundleCart.Count > 0)
                                                addedQuantity = Convert.ToInt32(dvBundleCart[0]["PRODUCT_QTY"]);
                                      totalQuantity = addedQuantity + quantity;

                                      if (totalQuantity > objBundle.OrderableQuantity)
                                      {
                                                if (addedQuantity > 0)
                                                {
                                                          ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objBundle.Name + "' is " + objBundle.OrderableQuantity + ". You have already added " + addedQuantity + " .");
                                                          tdError.Visible = true;
                                                          tdError.Attributes.Add("class", "error");
                                                          return;
                                                }
                                                else
                                                {
                                                          ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objBundle.Name + "' is " + objBundle.OrderableQuantity);
                                                          tdError.Visible = true;
                                                          tdError.Attributes.Add("class", "error");
                                                          return;
                                                }
                                      }
                                      List<ProductBundleCartDetail> cartItems = new List<ProductBundleCartDetail>();

                                      foreach (RepeaterItem item1 in (e.Item.FindControl("rptBundleProducts") as Repeater).Items)
                                      {
                                                HiddenField hdnQuantity = item1.FindControl("hdnQuantity") as HiddenField;
                                                quantityToAdd = Convert.ToInt32(hdnQuantity.Value) * quantity;
                                                addedQuantity = 0;
                                                totalQuantity = 0;
                                                //availableQunatity = 0;

                                                objProduct.SchoolId = Convert.ToInt32(Session["SchoolID"]);
                                                objProduct.ProductId = Convert.ToInt32((item1.FindControl("hdnProductID") as HiddenField).Value);
                                                objProduct.GetProductDetails();
                                                ProductBundleCartDetail objCartDetail = new ProductBundleCartDetail() { ProductID = objProduct.ProductId, Code = objProduct.ProductCode, ProductQuantity = Convert.ToInt32(hdnQuantity.Value) };

                                                if (objProduct.HasAttribute)
                                                {
                                                          ddl1Value = item1.FindControl("ddl1Value") as DropDownList;
                                                          ddl2Value = item1.FindControl("ddl2Value") as DropDownList;
                                                          objCartDetail.Label1Value = ddl1Value.SelectedValue;
                                                          objCartDetail.Label2Value = ddl2Value.SelectedValue;
                                                }
                                                else
                                                          objCartDetail.Label1Value = objCartDetail.Label2Value = null;
                                                cartItems.Add(objCartDetail);
                                      }

                                      dvBundleCart.RowFilter = "BUNDLE_ID = " + objBundle.BundleID;
                                      dvBundleCart = new DataView(dsBundleCart.Tables[0]);
                                      DataView dvAddedProducts;
                                      int totalItems = 0;
                                      bool isNew = true;
                                      quantityToAdd = quantity;
                                      ProductBundleCartDetail objCartDetail1 = new ProductBundleCartDetail();
                                      if (dvBundleCart.Count > 0) // check for similar bundles
                                                foreach (DataRowView drvCartItem in dvBundleCart)
                                                {
                                                          totalItems = 0;
                                                          objCartDetail1.CartID = Convert.ToInt32(drvCartItem["CART_ID"]);
                                                          dvAddedProducts = new DataView(objCartDetail1.FetchCartDetails().Tables[0]);
                                                          foreach (DataRowView drvAddedProduct in dvAddedProducts)
                                                                    foreach (ProductBundleCartDetail cartItem in cartItems)
                                                                    {
                                                                              if (cartItem.ProductQuantity == Convert.ToInt32(drvAddedProduct["QTY"]) &&
                                                                                  CommonClass.CompareIgnoreCase(cartItem.ProductID, drvAddedProduct["PRODUCT_ID"]) &&
                                                                                      CommonClass.CompareIgnoreCase(cartItem.Label1Value, drvAddedProduct["COLOR"]) &&
                                                                                      CommonClass.CompareIgnoreCase(cartItem.Label2Value, drvAddedProduct["SIZE"]))
                                                                              {
                                                                                        totalItems++;
                                                                                        break;
                                                                              }
                                                                    }
                                                          if (cartItems.Count == totalItems && dvAddedProducts.Count == totalItems)
                                                          {
                                                                    isNew = false;
                                                                    objBundleCart.CartID = objCartDetail1.CartID;
                                                                    quantityToAdd += Convert.ToInt32(drvCartItem["QUANTITY"]);
                                                                    break;
                                                          }
                                                }
                                      objBundleCart.CustomerID = Convert.ToInt32(Session["MemberID"]);
                                      objBundleCart.Quantity = quantityToAdd;
                                      objBundleCart.BundleID = objBundle.BundleID;
                                      objBundleCart.Code = objBundle.Code;
                                      objBundleCart.Gender = objBundle.Gender;
                                      //
                                      if (ConfigClass.Heading.ToLower() == "tuckshop")
                                      {
                                                objBundleCart.StudentId = Convert.ToInt32(Session["tuckshopstuid"]);
                                                objBundleCart.Mealtype = Convert.ToString(Session["tuckshopmealtype"]);
                                                objBundleCart.TakeDate = takeDate;
                                      }
                                      //
                                      ConfigClass.BundleCartPriceWithGST(objBundle, ref objBundleCart);
                                      pCart.Cart = objBundleCart;
                                      if (isNew)
                                                pCart.Detail = cartItems;
                                      pCarts.Add(pCart);
                            }
                            else
                            {
                                      ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-Quantity must be greater than 0.");
                                      tdError.Visible = true;
                                      tdError.Attributes.Add("class", "error");
                                      return;
                            
                            }
                 // }

                  //save product cart
                  //foreach (var item in lstCart)
                  //          item.AddCart();

                  //save bundle cart
                  foreach (var item in pCarts)
                  {
                            var bundleCart = item.Cart;
                            if (bundleCart.AddCart())
                                      foreach (var detail in item.Detail)
                                      {
                                                var bundleDetail = detail;
                                                bundleDetail.CartID = bundleCart.CartID;
                                                detail.AddToCart();
                                      }
                  }
                  if (pCarts.Count > 0)
                  {
                            ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-Product Bundle added successfully.");
                            tdError.Visible = true;
                            tdError.Attributes.Add("class", "passed");
                         //   return;
                  }
                  ((ssonested2)this.Page.Master).BindCart();
                  tdError.Visible = false;

                  //          ConfigClass.Redirect("shoppingcart.aspx");
                  //else
                  //{
                  //          ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-Please select at least one Product");
                  //          tdError.Visible = true;
                  //          tdError.Attributes.Add("class", "error");
                  //}

        }
        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

                  ////
                  if (ConfigClass.Heading.ToLower() == "tuckshop")
                  {
                            ContentPlaceHolder chpContent = (ContentPlaceHolder)this.Master.Master.FindControl("chpContent");
                            DropDownList ddlStudent = (DropDownList)(chpContent.FindControl("ddlStudent"));
                            SSO.Controls.SimpleTextBox txtDate = (SSO.Controls.SimpleTextBox)(chpContent.FindControl("txtDate"));
                            DropDownList ddlMealType = (DropDownList)(chpContent.FindControl("ddlMealType"));

                            if (ddlStudent.SelectedValue == "" || txtDate.Text == "" || ddlMealType.SelectedValue == "")
                            {
                                      ExceptionHandlingClass.PrintError(litMsg, "STUDENT_INFO,@fieldName-");
                                      tdError.Visible = true;
                                      tdError.Attributes.Add("class", "error");
                                      return;
                            }

                            if (DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", null) < DateTime.Now.Date)
                            {
                                      ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName- Date cannot be less than current date");
                                      tdError.Visible = true;
                                      tdError.Attributes.Add("class", "error");
                                      return;

                            }
                            try
                            {
                                      takeDate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", null);
                            }
                            catch (Exception ex)
                            {
                                      ExceptionHandlingClass.PrintError(litMsg, "CUSTOM_MSG,@fieldName- Please enter valid Date");
                                      tdError.Visible = true;
                                      tdError.Attributes.Add("class", "error");
                                      return;
                            }

                            Session["tuckshopstuid"] = ddlStudent.SelectedValue;
                            Session["tuckshopmealtype"] = ddlMealType.SelectedValue;
                            Session["tuckshoptakedate"] = txtDate.Text;
                            Session["tuckshopstuname"] = ddlStudent.SelectedItem.Text;

                  }
                  //////

                  int quantity;
                  int addedQuantity;
                  int totalQuantity;
                  int quantityToAdd;
                  ShoppingCartClass cart;
                  List<ShoppingCartClass> lstCart = new List<ShoppingCartClass>();
                  DropDownList ddl1Value = null, ddl2Value = null;
                  #region commented

                  //foreach (RepeaterItem i in rptCategories.Items)
                            //foreach (RepeaterItem item in (i.FindControl("rptProducts") as Repeater).Items)
                            //{
                                      quantity = (e.Item.FindControl("txtQuantity") as SSO.Controls.NumericTextBox).Value;
                                      if (quantity > 0)
                                      {
                                                addedQuantity = totalQuantity = quantityToAdd = 0;
                                                cart = new ShoppingCartClass();

                                                cart.MemberId = Convert.ToInt32(Session["MemberID"]);
                                                DataSet ds = cart.FetchItemsForCart();

                                                objAtt.ProductID = objProduct.ProductId = Convert.ToInt32((e.Item.FindControl("hdnProductID") as HiddenField).Value);
                                                objProduct.SchoolId = Convert.ToInt32(Session["SchoolID"]);
                                                objProduct.GetProductDetails();

                                                ddl1Value = e.Item.FindControl("ddl1Value") as DropDownList;
                                                ddl2Value = e.Item.FindControl("ddl2Value") as DropDownList;

                                                if (objProduct.HasAttribute)
                                                {
                                                          DataView view = objAtt.GetAllProductAttribute(string.Empty).Tables[0].DefaultView;
                                                          view.RowFilter = string.Format("IS_ACTIVE = 1 AND LABEL1VALUE = '{0}' AND LABEL2VALUE = '{1}'", ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
                                                          if (Convert.ToBoolean(view[0]["PREORDER"]))
                                                                    quantityToAdd = Convert.ToInt32(view[0]["QUANTITY"]) + Convert.ToInt32(view[0]["PREORDER_LIMIT"]);
                                                          else
                                                                    quantityToAdd = Convert.ToInt32(view[0]["QUANTITY"]);

                                                          if (ds.Tables[0].Rows.Count > 0)
                                                          {
                                                                    view = ds.Tables[0].DefaultView;
                                                                    view.RowFilter = string.Format("PRODUCT_ID = {0} AND LABEL1VALUE = '{1}' AND LABEL2VALUE = '{2}'", objProduct.ProductId, ddl1Value.SelectedValue.Replace("'", "''"), ddl2Value.SelectedValue.Replace("'", "''"));
                                                                    if (view.Count > 0)
                                                                              addedQuantity = Convert.ToInt32(view[0]["PRODUCT_QTY"]);
                                                          }
                                                }
                                                else
                                                {
                                                          quantityToAdd = objProduct.OrderableQuantity;
                                                          DataView view = ds.Tables[0].DefaultView;
                                                          view.RowFilter = "PRODUCT_ID = " + objProduct.ProductId;
                                                          if (view.Count > 0)
                                                                    addedQuantity = Convert.ToInt32(view[0]["PRODUCT_QTY"]);
                                                }
                                                if (addedQuantity == 0)
                                                {
                                                          totalQuantity = quantity;
                                                          if (totalQuantity > quantityToAdd)
                                                          {
                                                                    ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objProduct.ProductName + "' is " + quantityToAdd);
                                                                    tdError.Visible = true;
                                                                    tdError.Attributes.Add("class", "error");
                                                                    return;
                                                          }
                                                }
                                                else
                                                {
                                                          totalQuantity = quantity + addedQuantity;
                                                          if (totalQuantity > quantityToAdd)
                                                          {
                                                                    ExceptionHandlingClass.PrintMsg(litMsg, string.Format(@"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objProduct.ProductName + "' is {0}. You have already added {1}.", quantityToAdd, addedQuantity));
                                                                    tdError.Attributes.Add("class", "error");
                                                                    tdError.Visible = true;
                                                                    return;
                                                          }
                                                }
                                                cart.CustomerId = Convert.ToInt32(Session["MemberID"]);
                                                cart.SessionId = Session.SessionID;
                                                cart.Label1Value = ddl1Value.SelectedValue;
                                                cart.Label2Value = ddl2Value.SelectedValue;
                                                cart.ProductId = objProduct.ProductId;
                                                cart.Barcode = objProduct.Barcode;
                                                cart.Gender = Convert.ToChar(objProduct.Gender.Substring(0, 1));
                                                cart.Quantity = quantity;
                                                cart.ProductCode = GenerateProductCode(cart.Label2Value, cart.Label1Value);
                                                //
                                                if (ConfigClass.Heading.ToLower() == "tuckshop")
                                                {
                                                          cart.StudentId = Convert.ToInt32(Session["tuckshopstuid"]);
                                                          cart.Mealtype = Convert.ToString(Session["tuckshopmealtype"]);
                                                          cart.TakeDate = takeDate;
                                                }
                                                //

                                                ConfigClass.CartPriceWithGST(objProduct, ref cart);
                                               // (e.Item.FindControl("txtQuantity") as SSO.Controls.NumericTextBox).Value=0;
                                                lstCart.Add(cart);
                                      }
                                      else
                                      {
                                                ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-Quantity must be greater than 0.");
                                                tdError.Visible = true;
                                                tdError.Attributes.Add("class", "error");
                                                return;

                                      }
                         //   }
                  #endregion
                  //////#region bundle
                  
                  //////List<BundleCart> pCarts = new List<BundleCart>();
                  ////////foreach (RepeaterItem item in rptBundles.Items)
                  ////////{
                  //////quantity = (e.Item.FindControl("txtBundleQuantity") as SSO.Controls.NumericTextBox).Value;
                  //////if (quantity > 0)
                  //////{
                  //////          objBundleCart = new ProductBundleCart();
                  //////          BundleCart pCart = new BundleCart();
                  //////          addedQuantity = totalQuantity = quantityToAdd = 0;

                  //////          objBundle.BundleID = Convert.ToInt32((e.Item.FindControl("hdnBundleID") as HiddenField).Value);
                  //////          objBundle.SchoolID = Convert.ToInt32(Session["SchoolID"]);
                  //////          objBundle.ProductBundleDetails(true);
                  //////          objBundleCart.CustomerID = Convert.ToInt32(Session["MemberID"]);
                  //////          DataSet dsBundleCart = objBundleCart.FetchItemsForCart();
                  //////          DataView dvBundleCart = new DataView(dsBundleCart.Tables[1]);
                  //////          dvBundleCart.RowFilter = "BUNDLE_ID = " + objBundle.BundleID;
                  //////          if (dvBundleCart.Count > 0)
                  //////                    addedQuantity = Convert.ToInt32(dvBundleCart[0]["PRODUCT_QTY"]);
                  //////          totalQuantity = addedQuantity + quantity;

                  //////          if (totalQuantity > objBundle.OrderableQuantity)
                  //////          {
                  //////                    if (addedQuantity > 0)
                  //////                    {
                  //////                              ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objBundle.Name + "' is " + objBundle.OrderableQuantity + ". You have already added " + addedQuantity + " .");
                  //////                              tdError.Visible = true;
                  //////                              tdError.Attributes.Add("class", "error");
                  //////                              return;
                  //////                    }
                  //////                    else
                  //////                    {
                  //////                              ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-The maximum quantity allowed for '" + objBundle.Name + "' is " + objBundle.OrderableQuantity);
                  //////                              tdError.Visible = true;
                  //////                              tdError.Attributes.Add("class", "error");
                  //////                              return;
                  //////                    }
                  //////          }
                  //////          List<ProductBundleCartDetail> cartItems = new List<ProductBundleCartDetail>();

                  //////          foreach (RepeaterItem item1 in (e.Item.FindControl("rptBundleProducts") as Repeater).Items)
                  //////          {
                  //////                    HiddenField hdnQuantity = item1.FindControl("hdnQuantity") as HiddenField;
                  //////                    quantityToAdd = Convert.ToInt32(hdnQuantity.Value) * quantity;
                  //////                    addedQuantity = 0;
                  //////                    totalQuantity = 0;
                  //////                    //availableQunatity = 0;

                  //////                    objProduct.SchoolId = Convert.ToInt32(Session["SchoolID"]);
                  //////                    objProduct.ProductId = Convert.ToInt32((item1.FindControl("hdnProductID") as HiddenField).Value);
                  //////                    objProduct.GetProductDetails();
                  //////                    ProductBundleCartDetail objCartDetail = new ProductBundleCartDetail() { ProductID = objProduct.ProductId, Code = objProduct.ProductCode, ProductQuantity = Convert.ToInt32(hdnQuantity.Value) };

                  //////                    if (objProduct.HasAttribute)
                  //////                    {
                  //////                              ddl1Value = item1.FindControl("ddl1Value") as DropDownList;
                  //////                              ddl2Value = item1.FindControl("ddl2Value") as DropDownList;
                  //////                              objCartDetail.Label1Value = ddl1Value.SelectedValue;
                  //////                              objCartDetail.Label2Value = ddl2Value.SelectedValue;
                  //////                    }
                  //////                    else
                  //////                              objCartDetail.Label1Value = objCartDetail.Label2Value = null;
                  //////                    cartItems.Add(objCartDetail);
                  //////          }

                  //////          dvBundleCart.RowFilter = "BUNDLE_ID = " + objBundle.BundleID;
                  //////          dvBundleCart = new DataView(dsBundleCart.Tables[0]);
                  //////          DataView dvAddedProducts;
                  //////          int totalItems = 0;
                  //////          bool isNew = true;
                  //////          quantityToAdd = quantity;
                  //////          ProductBundleCartDetail objCartDetail1 = new ProductBundleCartDetail();
                  //////          if (dvBundleCart.Count > 0) // check for similar bundles
                  //////                    foreach (DataRowView drvCartItem in dvBundleCart)
                  //////                    {
                  //////                              totalItems = 0;
                  //////                              objCartDetail1.CartID = Convert.ToInt32(drvCartItem["CART_ID"]);
                  //////                              dvAddedProducts = new DataView(objCartDetail1.FetchCartDetails().Tables[0]);
                  //////                              foreach (DataRowView drvAddedProduct in dvAddedProducts)
                  //////                                        foreach (ProductBundleCartDetail cartItem in cartItems)
                  //////                                        {
                  //////                                                  if (cartItem.ProductQuantity == Convert.ToInt32(drvAddedProduct["QTY"]) &&
                  //////                                                      CommonClass.CompareIgnoreCase(cartItem.ProductID, drvAddedProduct["PRODUCT_ID"]) &&
                  //////                                                          CommonClass.CompareIgnoreCase(cartItem.Label1Value, drvAddedProduct["COLOR"]) &&
                  //////                                                          CommonClass.CompareIgnoreCase(cartItem.Label2Value, drvAddedProduct["SIZE"]))
                  //////                                                  {
                  //////                                                            totalItems++;
                  //////                                                            break;
                  //////                                                  }
                  //////                                        }
                  //////                              if (cartItems.Count == totalItems && dvAddedProducts.Count == totalItems)
                  //////                              {
                  //////                                        isNew = false;
                  //////                                        objBundleCart.CartID = objCartDetail1.CartID;
                  //////                                        quantityToAdd += Convert.ToInt32(drvCartItem["QUANTITY"]);
                  //////                                        break;
                  //////                              }
                  //////                    }
                  //////          objBundleCart.CustomerID = Convert.ToInt32(Session["MemberID"]);
                  //////          objBundleCart.Quantity = quantityToAdd;
                  //////          objBundleCart.BundleID = objBundle.BundleID;
                  //////          objBundleCart.Code = objBundle.Code;
                  //////          objBundleCart.Gender = objBundle.Gender;
                  //////          //
                  //////          if (ConfigClass.Heading.ToLower() == "tuckshop")
                  //////          {
                  //////                    objBundleCart.StudentId = Convert.ToInt32(Session["tuckshopstuid"]);
                  //////                    objBundleCart.Mealtype = Convert.ToString(Session["tuckshopmealtype"]);
                  //////                    objBundleCart.TakeDate = takeDate;
                  //////          }
                  //////          //
                  //////          ConfigClass.BundleCartPriceWithGST(objBundle, ref objBundleCart);
                  //////          pCart.Cart = objBundleCart;
                  //////          if (isNew)
                  //////                    pCart.Detail = cartItems;
                  //////          pCarts.Add(pCart);
                  //////}
                  //////#endregion
                  // }

                  //save product cart
                  foreach (var item in lstCart)
                            item.AddCart();

                  //save bundle cart
                  //foreach (var item in pCarts)
                  //{
                  //          var bundleCart = item.Cart;
                  //          if (bundleCart.AddCart())
                  //                    foreach (var detail in item.Detail)
                  //                    {
                  //                              var bundleDetail = detail;
                  //                              bundleDetail.CartID = bundleCart.CartID;
                  //                              detail.AddToCart();
                  //                    }
                  //}

                  if (lstCart.Count > 0 )
                  {
                            ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-Product added successfully.");
                            tdError.Visible = true;
                            tdError.Attributes.Add("class", "passed");
                           // return;
                  }
                  ((ssonested2)this.Page.Master).BindCart();
                  tdError.Visible = false;

                  //          ConfigClass.Redirect("shoppingcart.aspx");
                  //else
                  //{
                  //          ExceptionHandlingClass.PrintMsg(litMsg, @"CUSTOM_MSG,@fieldName-Please select at least one Product");
                  //          tdError.Visible = true;
                  //          tdError.Attributes.Add("class", "error");
                  //}

        }
}
}