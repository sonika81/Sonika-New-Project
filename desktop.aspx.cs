using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;


/// <summary>
/// Created By:vikas
/// Created On:15 oct 2010
/// Purpose: manage desktop
/// </summary>
namespace SSO
{
    public partial class control_admin_desktop : System.Web.UI.Page
    {
        #region DECLARATION
        int intuserType;
        int SchoolID { get { return Convert.ToInt32(Session["SchoolID"]); } }
        int UserType { get { return Convert.ToInt32(Session["USER_TYPE"]); } }
        protected int TotalAdminUsers;
        protected int TotalSchoolAdmin;
        protected int TotalAgent;
        protected int TotalRegisteredMembers;
        protected int TotalSubscribers;
        protected int TotalEventAdmin;
        protected int TotalAdminPersonnel;
        protected string LastLoginIP;
        protected string LastLoginTime;
        protected int count = 0;
        #endregion

        #region PAGE LOAD
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UID"] == null || Session["SchoolID"] == null)
                ConfigClass.Redirect("index.aspx");

            if (Convert.ToInt32(Session["USER_TYPE"]) == 1 && !string.IsNullOrEmpty(Request.QueryString["ID"]))
                Session["SchoolID"] = Request.QueryString["ID"];

            ((Literal)this.Master.FindControl("ltrBreadCrumb")).Text = "Desktop";
            ((Literal)this.Master.FindControl("ltrPageTitle")).Text = "Desktop";
            ((HtmlAnchor)this.Master.FindControl("ackback")).Visible = false;
            ShowDesktop();
            FillDashBord();
            ShowCordiNote();
            //if (Session["USER_TYPE"].ToString() == "10")
            //{
                BindDemoTrainingMaterial();
            //}
            //else
            //{
            //    tblDemoTrainingMaterial.Style.Add("display", "none");
            //}
        }
        #endregion

        #region FUNCTION FOR FILLING DASHBOARD

        private void BindDemoTrainingMaterial()
        {
            bool docExists = false;
            demoTraningMaterialclass objdemo = new demoTraningMaterialclass();

            if (Session["SchoolID"] != null)
            {
                objdemo.SchoolID = Convert.ToInt32(Session["SchoolID"]);
            }

            DataSet dsDemo = objdemo.[();
            if (dsDemo != null && dsDemo.Tables.Count > 0 && dsDemo.Tables[0].Rows.Count > 0)
            {
                docExists = true;
                repDemo.DataSource = dsDemo;
                repDemo.DataBind();

            }
            else
            {
                litDemoMsg.Text = "&nbsp;&nbsp;No records found";
            }
            DataSet ds = objdemo.GetTraningMaterialListing();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                docExists = true;
                count = 0;
                repTrainingMat.DataSource = ds;
                repTrainingMat.DataBind();
            }
            else
            {
                litTrainingMat.Text = "&nbsp;&nbsp;No records found";
            }
            //if (!docExists)
            //{
            //    tblDemoTrainingMaterial.Style.Add("display", "none");
            //}
        }


        private void FillDashBord()
        {
            DesktopClass objDesktop = new DesktopClass();
            if (objDesktop.GetDashBoardInfo(Request.UserHostAddress))
            {
                TotalAdminUsers = objDesktop.TotalAdminUsers;
                TotalSchoolAdmin = objDesktop.TotalSchoolAdmin;
                TotalAgent = objDesktop.TotalAgent;
                TotalRegisteredMembers = objDesktop.TotalRegisteredMembers;
                TotalSubscribers = objDesktop.TotalSubscribers;
                TotalEventAdmin = objDesktop.TotalEventAdmin;
                TotalAdminPersonnel = objDesktop.TotalAdminPersonnel;
                LastLoginIP = objDesktop.LastLoginIP;
                LastLoginTime = objDesktop.LastLoginTime;
            }

            intuserType = Convert.ToInt32(Session["USER_TYPE"]);
            switch (intuserType)
            {
                case 0:
                case 1://Super admin
                    trLastLogin.Visible = trClubAdmin.Visible = true;
                    DataView view = new Schools().GetSchools(string.Empty).Tables[0].DefaultView;
                    view.RowFilter = "IS_ACTIVE = True";
                    rptSchools.DataSource = view;
                    rptSchools.DataBind();
                    break;
                case 3://Event Admin
                case 4://Admin Personnel
                    trLastLogin.Visible = true;
                    break;
                case 2://School admin
                case 5://Agent
                default:
                    trLastLogin.Visible = trClubAdmin.Visible = true;
                    break;
            }

            if (Session["USER_TYPE"] != null)
            {
                DashboardContent objDash = new DashboardContent();
                string userType = Convert.ToString(Session["USER_TYPE"]);
                objDash.UserType = Convert.ToInt32(userType);
                objDash.SchoolID = Convert.ToInt32(Session["SchoolID"]);
                objDash.GetDetailByType();
                if (objDash.IsActive)
                {
                    //downloadable Manual PDF
                    if (!string.IsNullOrEmpty(objDash.ManualLink))
                    {
                        trManual.Style.Add("display", "");
                        litManual.Text = "<a href='../../" + ConfigClass.uploadsFolderPath + "mannual/" + objDash.ManualLink + "' target='_blank'><b>Download Manual</b></a><br/>&nbsp;";
                    }
                    else
                        trManual.Style.Add("display", "none");

                    //announcements
                    if (!string.IsNullOrEmpty(objDash.Announcements))
                    {
                        tdAnn.Style.Add("display", "");
                        litAnn.Text = objDash.Announcements;
                    }
                    else
                        tdAnn.Style.Add("display", "none");

                    //dashboar content
                    if (!string.IsNullOrEmpty(objDash.DashboarContent))
                    {
                        tdDashboarContent.Style.Add("display", "");
                        litDashBoardContent.Text = objDash.DashboarContent.ToString();
                    }
                    else
                        tdDashboarContent.Style.Add("display", "none");
                }
                else
                {
                    trManual.Style.Add("display", "none");
                    tdAnn.Style.Add("display", "none");
                    tdDashboarContent.Style.Add("display", "none");
                }
            }
        }
        #endregion
        private void ShowCordiNote()
        {
                  if (Session["USER_TYPE"].ToString() != "1" )
                  {
                            tdmsg.Style.Add("display", "");
                            //FavouriteToolClass objNote = new FavouriteToolClass();
                            //try
                            //{
                            //          objNote.NoteSuperadmintoCordi();
                            //          //if (objNote.SchoolId != ConfigClass.SchoolID)
                            //          //    ConfigClass.Redirect("index.aspx");
                            //        //  litCordinatorNote.Text = objNote.Comment;

                            //}
                            //catch (Exception ex)
                            //{
                            //          ExceptionHandlingClass.HandleException(ex);
                            //}
                            //finally
                            //{
                            //          objNote = null;
                            //}
                  }
                  else
                            tdmsg.Style.Add("display", "none");
        }
        #region SHOW DESKTOP
        /// <summary>
        /// Show hide desktop icons according to user Type       
        /// </summary>
        private void ShowDesktop()
        {
           // rptDesktop.DataSource = objModule.GetDesktop(Convert.ToInt32(Session["USER_TYPE"]));
            rptDesktop.DataSource = objModule.GetDesktop_New(Convert.ToInt32(Session["USER_TYPE"]), Convert.ToInt32(Session["SchoolID"]));
          
            rptDesktop.DataBind();
        }

        ModuleClass objModule = new ModuleClass();
        protected void rptDesktop_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    Repeater rptItems = e.Item.FindControl("rptItems") as Repeater;
                    HiddenField headername = e.Item.FindControl("hidname") as HiddenField;
                    if (rptItems != null)
                    {
                        string schools = "0,";
                        schools += SchoolID > 0 ? "2" : "1";
                        if (SchoolID > 0)
                        {
                                  DataSet ds1 = new DataSet();
                                  ds1 = objModule.CheckEventModule(SchoolID);
                                  if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0 && headername.Value=="Manage Events")
                                  {
                                            if (Convert.ToBoolean(ds1.Tables[0].Rows[0]["EVENT_MODULE"]) == false)
                                            {
                                                      e.Item.Visible = false;
                                                      //      break;
                                            }
                                            else
                                            {
                                                      rptItems.DataSource = objModule.GetDesktopItems(UserType, Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "DESKTOP_ID")), schools);
                                                      rptItems.DataBind();
                                                      if (rptItems.Items.Count < 1)
                                                      {
                                                                e.Item.Visible = false;
                                                      }

                                            }
                                  }
                                  else
                                  {
                                            rptItems.DataSource = objModule.GetDesktopItems(UserType, Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "DESKTOP_ID")), schools);
                                            rptItems.DataBind();
                                            if (rptItems.Items.Count < 1)
                                            {
                                                      e.Item.Visible = false;
                                            }

                                  }

                        }
                        else
                        {
                                  rptItems.DataSource = objModule.GetDesktopItems(UserType, Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "DESKTOP_ID")), schools);
                                  rptItems.DataBind();
                                  if (rptItems.Items.Count < 1)
                                  {
                                            e.Item.Visible = false;
                                  }
                        }
                    }
                    break;
            }
        }
        #endregion

        protected void grdFiles_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            //if (e.Item.ItemType == DataControlItemType.DataItem)//run the code for grid's item template only
            //{
            //string Filename = DataBinder.Eval(e.Item.DataItem, "FILES_NAME").ToString();
            //switch (e.Item.ItemType)
            //{

            //  case ListItemType.Item:

            try
            {
                if (count == 0)
                    ((Literal)e.Item.FindControl("litsep")).Text = "";
                else
                    ((Literal)e.Item.FindControl("litsep")).Text = ",";
                count = count + 1;


                string Filename = ((HiddenField)e.Item.FindControl("hdFileName")).Value;
                if (Filename.Contains(".flv") || Filename.Contains(".swf"))
                {
                    ((HyperLink)e.Item.FindControl("filelink")).NavigateUrl = "playvideo.aspx?type=6&name=" + Filename;
                    ((HyperLink)e.Item.FindControl("filelink")).Target = "blank";

                }
                else if (Filename.Contains(".webm"))
                {
                    ((HyperLink)e.Item.FindControl("filelink")).NavigateUrl = "playvideo.aspx?type=7&name=" + Filename;
                    ((HyperLink)e.Item.FindControl("filelink")).Target = "blank";

                }
                else if (Filename.Contains(".dat") || Filename.Contains(".wmv") || Filename.Contains(".mp3") || Filename.Contains(".wma") || Filename.Contains(".wav") || Filename.Contains(".mpeg"))
                {
                    ((HyperLink)e.Item.FindControl("filelink")).NavigateUrl = "playvideo.aspx?type=8&name=" + Filename;
                    ((HyperLink)e.Item.FindControl("filelink")).Target = "blank";

                }
                else
                {
                    ((HyperLink)e.Item.FindControl("filelink")).NavigateUrl = "../../uploads/DemoTrainingMaterial/" + Filename;
                    ((HyperLink)e.Item.FindControl("filelink")).Target = "blank";
                }
                //    break;
                //}

                //}
            }
            catch { }
            //switch (e.Item.ItemType)
            //{
            //    case ListItemType.Footer:
            //        ((Literal)e.Item.FindControl("litsep")).Text = "";
            //        break;
            //}
        }
      

    }
}