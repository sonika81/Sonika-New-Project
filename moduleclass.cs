using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Xml;


/// <summary>
// created by : vikas
// Created On:19 Oct 2010
// Purpose : This class has various functions required to Get the List of modules.
/// </summary>
namespace SSO
{
    public class ModuleClass
    {
        #region private members

        private string PageAccess = string.Empty;
        ArrayList _arrErrorMsgs = new ArrayList();

        #endregion

        #region public properties


        public ArrayList arrErrorMsgs
        {
            get
            {
                return _arrErrorMsgs;
            }
            set
            {
                _arrErrorMsgs = value;
            }
        }
        #endregion

        #region public methods
        public DataSet GetDesktop(int userType)
        {
            return SqlHelper.ExecuteDataset(ConfigClass.DbConn, CommandType.StoredProcedure,
               ConfigClass.storedProcPrefix + "DESKTOP_GET", new SqlParameter[1] { new SqlParameter("@USER_TYPE", userType) });
        }

        public DataSet GetDesktop_New(int userType, int Schoolid)
        {
            SqlParameter[] objParam = new SqlParameter[2];
            objParam[0] = new SqlParameter("@USER_TYPE", userType);
            objParam[1] = new SqlParameter("@SCHOOLID", Schoolid);
            return SqlHelper.ExecuteDataset(ConfigClass.DbConn, CommandType.StoredProcedure,
               ConfigClass.storedProcPrefix + "DESKTOP_GET_NEW", objParam);


            //return SqlHelper.ExecuteDataset(ConfigClass.DbConn, CommandType.StoredProcedure,
            //   ConfigClass.storedProcPrefix + "DESKTOP_GET", new SqlParameter[1] { new SqlParameter("@USER_TYPE", userType) });
        }

        public DataSet GetDesktopItems(int userType, int desktopID, string schoolMode)
        {
            SqlParameter[] objParam = new SqlParameter[3];
            objParam[0] = new SqlParameter("@USER_TYPE", userType);
            objParam[1] = new SqlParameter("@DESKTOP_ID", desktopID);
            objParam[2] = new SqlParameter("@SCHOOL", schoolMode);
            return SqlHelper.ExecuteDataset(ConfigClass.DbConn, CommandType.StoredProcedure,
               ConfigClass.storedProcPrefix + "DESKTOP_ITEMS_GET", objParam);
        }

        public DataSet CheckEventModule(Int32 Schoolid)
        {
                  SqlParameter[] objParam = new SqlParameter[3];
                  objParam[2] = new SqlParameter("@SCHOOLID", Schoolid);
                  return SqlHelper.ExecuteDataset(ConfigClass.DbConn, CommandType.StoredProcedure,
                     ConfigClass.storedProcPrefix + "CHECK_EVENT_MODULE", objParam);
        }
        public DataSet ListAllModules()
        {
            DataSet ds = new DataSet();
            ds = SqlHelper.ExecuteDataset(ConfigClass.DbConn, CommandType.StoredProcedure,
               ConfigClass.storedProcPrefix + "LIST_MODULES");
            return ds;
        }

        public void BindModules(GridView gvModules, Literal litMsg)
        {
            DataSet ds = ListAllModules();
            if (ds.Tables[0].Rows.Count > 0)
            {
                gvModules.DataSource = ds;
                gvModules.DataBind();
            }
            else
                ExceptionHandlingClass.PrintError(litMsg, "NO_RECORD,@fieldName- Modules");
            ds.Dispose();
        }


        #region GET ACTIVE MODULES
        /// <summary>
        /// function to GetActiveModules
        /// Author: Mdhillon
        /// </summary>
        /// it will set the pageIds variable with comma seapareted string of Page Names</returns>
        public DataSet GetActiveModules()
        {
            DataSet ds = new DataSet();

            try
            {
                ds = SqlHelper.ExecuteDataset(ConfigClass.DbConn, CommandType.StoredProcedure,
                             ConfigClass.storedProcPrefix + "GET_ACTIVE_MODULES");
            }
            catch (Exception ex)
            {
                ExceptionHandlingClass.HandleException(ex);
            }

            return ds;
        }

        private void GetCommaSeparatedActiveModules()
        {
            DataSet ds = GetActiveModules();
            if (ds.Tables[0].Rows.Count > 0)
            {
                string userType = string.Empty;
                if (HttpContext.Current.Session["USER_TYPE"] != null)
                    userType = HttpContext.Current.Session["USER_TYPE"].ToString();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["MODULE_CODE"].ToString().ToLower() == "dashboardcontent" || row["MODULE_CODE"].ToString().ToLower() == "faqlisting" || row["MODULE_CODE"].ToString().ToLower() == "loginscreen" || row["MODULE_CODE"].ToString().ToLower() == "paidfreeevents")
                    {
                        if (userType == "0" || userType == "1")
                        {//super admin
                            if (PageAccess.Length > 0)
                                PageAccess += ",";
                            PageAccess += row["MODULE_CODE"].ToString();
                        }
                    }
                    else
                    {
                        if (PageAccess.Length > 0)
                            PageAccess += ",";
                        PageAccess += row["MODULE_CODE"].ToString();
                    }
                }
            }
            ds.Dispose();
        }

        /// <summary>
        /// Get Module list + sub pages to show on general side
        /// </summary>
        /// <returns></returns>
        public DataSet GetMainMenu()
        {
            DataSet ds = new DataSet();

            try
            {
                ds = SqlHelper.ExecuteDataset(ConfigClass.DbConn, CommandType.StoredProcedure,
                             ConfigClass.storedProcPrefix + "GET_MAIN_MENU");
            }
            catch (Exception ex)
            {
                ExceptionHandlingClass.HandleException(ex);
            }

            return ds;
        }


        #region Desktop Icons
        /// <summary>
        /// Function to generate Hashtable of all icons
        /// Written By : vikas
        /// </summary>
        /// <returns>Hashtable</returns>
        private Hashtable GetDesktopIcons()
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["SchoolID"] == null)
                return null;
            Hashtable objHash = new Hashtable();
            objHash["users"] = "<a href='listusers.aspx'>"
                                       + "<img alt='' src=\"images/icons/user2.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\" "
                                       + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                       + "Manage Users</a>";

            objHash["dynamic"] = string.Format("<a href=\"{0}\">", Convert.ToInt32(HttpContext.Current.Session["SchoolID"]) > 0 ? "listdynamicpages.aspx" : "listadmindynamicpages.aspx")
                                        + "<img alt='' src=\"images/icons/notice.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                        + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                        + string.Format("Manage{0} Dynamic Pages</a>", Convert.ToInt32(HttpContext.Current.Session["SchoolID"]) > 0 ? string.Empty : " Admin");

            objHash["hbanner"] = "<a href=\"listhomeimages.aspx\">"
                                       + "<img alt='' src=\"images/icons/photogallery.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                       + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                       + "Manage Home Page Banner Images</a>";

            objHash["contact"] = "<a href=\"contactuslisting.aspx\">"
                                        + "<img alt='' src=\"images/icons/contact.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                        + "onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                        + "Manage Contact Us</a>";

            objHash["subscribers"] = "<a href=\"listcustomers.aspx\">"
                                        + "<img alt='' src=\"images/icons/user3.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                        + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                        + "Manage Subscribers</a>";

            objHash["news"] = "<a href=\"listnews.aspx\">"
                                        + "<img alt='' src=\"images/icons/manageorder.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                        + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                        + "Manage News</a>";

            objHash["stickets"] = "<a href=\"supportticketlisting.aspx\">"
                                      + "<img alt='' src=\"images/icons/misc17.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                      + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                      + "Manage Support Tickets</a>";

            objHash["sitehome"] = " <a href=\"../../index.aspx\" target=\"_blank\">"
                                 + "<img alt='' src=\"images/icons/home.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                 + "  onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                 + "Site Home</a>";

            objHash["logout"] = "<a href=\"logout.aspx\">"
                             + "<img alt='' src=\"images/icons/logout.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                             + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                             + "Logout</a>";

            if (HttpContext.Current.Session["USER_TYPE"] != null)
            {
                if (HttpContext.Current.Session["USER_TYPE"].ToString() == "0" || HttpContext.Current.Session["USER_TYPE"].ToString() == "1")
                {
                    objHash["dashboardcontent"] = "<a href=\"dashboadmanage.aspx\">"
                                    + "<img alt='' src=\"images/icons/tool2.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                    + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                    + "Manage Dashboard Content</a>";

                    objHash["library"] = "<a href=\"manageagent.aspx\">"
                                                    + "<img alt='' src=\"images/icons/manageorder.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                                    + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                                    + "Manage Agents</a>";

                    objHash["schools"] = "<a href=\"schools.aspx\">"
                                               + "<img alt='' src=\"images/icons/home.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                               + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                               + "Manage Schools</a>";
                    objHash["faqlisting"] = "<a href=\"listfaq.aspx\">"
                                   + "<img alt='' src=\"images/icons/search.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                   + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                   + "Manage FAQ</a>";

                    //objHash["loginscreen"] = "<a href=\"addscreenicon.aspx\">"
                    //               + "<img alt='' src=\"images/icons/snapshot.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                    //               + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                    //               + "Add Login Screen Content</a>";
                }
                else
                {
                    objHash["estore"] = "<a href=\"listestore.aspx\">"
                               + "<img alt='' src=\"images/icons/e-store.gif\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                               + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                               + "Manage eStore</a>";

                    objHash["fees"] = "<a href=\"listfees.aspx\">"
                              + "<img alt='' src=\"images/icons/fees.gif\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                              + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                              + "Manage Fees</a>";

                    objHash["resources"] = "<a href=\"listresources.aspx\">"
                              + "<img alt='' src=\"images/icons/resources.gif\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                              + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                              + "Manage Resources</a>";

                    objHash["faq"] = "<a href=\"faqdisplay.aspx\">"
                                    + "<img alt='' src=\"images/icons/misc17.png\" border=\"0\" onmouseover=\"this.className = 'pressDown'\""
                                    + " onmouseout=\"this.className = 'pressNo'\" class='pressNo' /><br />"
                                    + "FAQ</a>";

                }
            }


            return objHash;
        }

        /// <summary>
        /// Function to generate Hashtable of all icons
        /// Written By : vikas
        /// </summary>
        /// <returns>Hashtable</returns>
        private Hashtable GetShortCutDesktopIcons()
        {
            Hashtable objHash = new Hashtable();

            objHash["events"] = "<span class='shortcut_icon'><a href='listevents.aspx'>"
                                      + "<img src='images/icons/misc13_small.png' border='0' align='middle' class='top_icon'"
                                          + "alt='' /><br />"
                                     + "Manage Events</a></span>";
            objHash["users1"] = "<span class=\"shortcut_icon\"><a href=\"desktop.aspx\">"
                                        + "<img src=\"images/icon_home.gif\" border=\"0\" align=\"middle\" class=\"top_icon\" alt=\"\" /><br />"
                                        + "Desktop</a></span>";

            objHash["users"] = "<span class=\"shortcut_icon\"><a href=\"listusers.aspx\">"
                                        + "<img src=\"images/icon_manage_users.gif\" border=\"0\" align=\"middle\" class=\"top_icon\" alt=\"\" /><br />"
                                        + "Manage Users</a></span>";


            objHash["product"] = "<span class=\"shortcut_icon\"><a href=\"listproducts.aspx\">"
                                        + "<img src=\"images/misc13_small.png\" border=\"0\" align=\"middle\" class=\"top_icon\" alt=\"\" /><br />"
                                        + "Manage Products</a></span>";



            objHash["dynamic"] = "<span class='shortcut_icon'><a href='listdynamicpages.aspx'>"
                                        + "<img src='images/icons/notice_small.png' border='0' align='middle' class='top_icon'"
                                            + "alt='' /><br />"
                                       + "Manage Dynamic Pagess</a></span>";

            objHash["gallery"] = "<span class='shortcut_icon'><a href='listgalleryphotos.aspx'>"
                                        + "<img src='images/icons/misc13_small.png' border='0' align='middle' class='top_icon'"
                                            + "alt='' /><br />"
                                       + "Manage Gallery</a></span>";

            objHash["banner"] = "<span class='shortcut_icon'><a href='listbanners.aspx'>"
                                        + "<img src='images/icons/misc13_small.png' border='0' align='middle' class='top_icon'"
                                            + "alt='' /><br />"
                                       + "Manage Banner</a></span>";

            objHash["videos"] = "<span class='shortcut_icon'><a href='listvideos.aspx'>"
                                        + "<img src='images/icons/misc13_small.png' border='0' align='middle' class='top_icon'"
                                            + "alt='' /><br />"
                                       + "Manage Videos</a></span>";

            objHash["subscribers"] = "<span class='shortcut_icon'><a href='listcustomers.aspx'>"
                                        + "<img src='images/icons/misc13_small.png' border='0' align='middle' class='top_icon'"
                                            + "alt='' /><br />"
                                       + "Newsletter Subscribers</a></span>";

            objHash["customers"] = "<span class='shortcut_icon'><a href='listcustomers.aspx'>"
                                        + "<img src='images/icons/misc13_small.png' border='0' align='middle' class='top_icon'"
                                            + "alt='' /><br />"
                                       + "Manage Members</a></span>";

            objHash["blog"] = "<span class='shortcut_icon'><a href='listblogs.aspx'>"
                                        + "<img src='images/icons/misc13_small.png' border='0' align='middle' class='top_icon'"
                                            + "alt='' /><br />"
                                       + "Manage Blogs</a></span>";
            objHash["sTickets"] = "<span class='shortcut_icon'><a href='supportticketlisting.aspx'>"
                                       + "<img src='images/icons/misc13_small.png' border='0' align='middle' class='top_icon'"
                                           + "alt='' /><br />"
                                      + "Support Tickets</a></span>";

            return objHash;
        }

        #endregion

        #region Generate Deskop Icons
        /// <summary>
        /// Function to draw Desktop Icons dynamically according to page Access Rights
        /// Written By :Mdhillon
        /// </summary>
        public string UserDesktop()
        {
            string userDesktop = string.Empty;

            if (HttpContext.Current.Session["USER_TYPE"] != null)
            {
                Hashtable objHash = GetDesktopIcons();
                int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
                int count = 0;
                int rowStart = 0;
                string moduleCode = string.Empty;
                string temp = string.Empty;

                GetCommaSeparatedActiveModules();

                //If User is Regular Admin or Super Admin
                if (userType < 3)
                {
                    //for super admin only
                    if (userType <= 1)
                        PageAccess += ",module,agent,surcharge";


                    ////if (userType <= 1)//super admin
                    ////    PageAccess += ",module,eventreport,patronreport2,accesscontrol,eventstatus";
                    ////else if (userType <= 2)//super admin,club
                    ////    PageAccess += ",module,eventreport";
                    ////else if (userType <= 1)
                    ////    PageAccess += ",module,patronreport2";

                }
                else if (userType == 3)//If User is EVent Admin
                {
                    temp = string.Empty;
                    if (PageAccess.ToLower().Contains("users"))
                        temp = "users";
                    if (temp.Length > 0)
                        temp += ",";
                    if (PageAccess.ToLower().Contains("events"))
                        temp += "events";
                    if (temp.Length > 0)
                        temp += ",";
                    if (PageAccess.ToLower().Contains("eorder"))
                        temp += "eorder";
                    if (temp.Length > 0)
                        temp += ",";
 if (PageAccess.ToLower().Contains("ecategory"))
                        temp += "ecategory";
                    if (temp.Length > 0)
                        temp += ",";

                    if (PageAccess.ToLower().Contains("stickets"))
                        temp += "stickets";


                    PageAccess = temp;
                }
                else if (userType == 5)//If User is Agent
                {
                    temp = string.Empty;

                    if (PageAccess.ToLower().Contains("eorder"))
                        temp += "eorder";
                    if (temp.Length > 0)
                        temp += ",";
                    if (PageAccess.ToLower().Contains("stickets"))
                        temp += "stickets";

                    PageAccess = temp;
                }
                else if (userType == 4)//If User is Admin Personnel
                {
                    temp = string.Empty;
                    //if (PageAccess.ToLower().Contains("eorder"))
                    //    temp += "eorder";
                    if (temp.Length > 0)
                        temp += ",";
                    if (PageAccess.ToLower().Contains("stickets"))
                        temp += "stickets";
                    PageAccess = temp;
                }

                //Common for all user types
                PageAccess += ",faq,sitehome,logout";

                string[] arrPages = PageAccess.Split(',');

                if (arrPages.Length > 0)
                {
                    userDesktop = "<table width=\"460\" cellpadding=\"0\" cellspacing=\"1\" bgcolor=\"#cccccc\" align=\"left\""
                                     + " class=\"box\">\n";

                    for (int index = 0; index < arrPages.Length; index++)
                    {
                        moduleCode = Convert.ToString(arrPages[index]).ToLower();

                        if (objHash.ContainsKey(moduleCode) == true)
                        {
                            //open row
                            if (rowStart == 0)
                            {
                                userDesktop += "<tr bgcolor=\"#FFFFFF\">\n";


                                rowStart = 1;
                            }

                            //Add td

                            userDesktop += "<td width=\"13%\" valign=\"middle\" align=\"center\" height=\"100\" class=\"my_icon\" onmouseover=\"this.filters.alpha.opacity='100';this.className='my_icon_hover'\""
                                            + " onmousedown=\"this.filters.alpha.opacity='70'\"  onmouseout=\"this.filters.alpha.opacity='85'\">\n";

                            userDesktop += objHash[moduleCode].ToString();

                            userDesktop += "</td>\n";

                            //increment td
                            count++;

                            //////add white boxes at the end
                            ////if (arrPages.Length % 4 != 0 && index == arrPages.Length - 1)
                            ////{
                            ////    for (int i = 0; i < (4 - (count % 4)); i++)
                            ////    {
                            ////        userDesktop += "<td width=\"13%\" valign=\"middle\" align=\"center\" height=\"100\" class=\"my_icon\" onmouseover=\"this.filters.alpha.opacity='100';this.className='my_icon_hover'\""
                            ////                        + " onmousedown=\"this.filters.alpha.opacity='70'\"  onmouseout=\"this.filters.alpha.opacity='85'\">";
                            ////        userDesktop += "sdfsdf</td>";
                            ////    }
                            ////}

                            //close row
                            if (count % 4 == 0)
                            {
                                userDesktop += "</tr>\n";
                                rowStart = 0;
                            }
                        }
                    }
                    userDesktop += "</table>";


                }
            }
            return userDesktop;
        }

        public string UserShortCutDesktop()
        {
            string userDesktop = string.Empty;

            if (HttpContext.Current.Session["USER_TYPE"] != null)
            {
                Hashtable objHash = GetShortCutDesktopIcons();
                int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
                int count = 0;
                int rowStart = 0;
                string moduleCode = string.Empty;
                string temp = string.Empty;
                string temp1 = string.Empty;

                GetCommaSeparatedActiveModules();

                if (PageAccess.ToLower().Contains("users"))
                {
                    temp1 = "users";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                if (PageAccess.ToLower().Contains("product"))
                {
                    temp1 += "product";
                }
                if (temp1.Length > 0)
                    temp1 += ",";
                if (PageAccess.ToLower().Contains("dynamic"))
                {
                    temp1 += "dynamic";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                if (PageAccess.ToLower().Contains("gallery"))
                {
                    temp1 += "gallery";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                if (PageAccess.ToLower().Contains("banner"))
                {
                    temp1 += "banner";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                if (PageAccess.ToLower().Contains("videos"))
                {
                    temp1 += "videos";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                if (PageAccess.ToLower().Contains("events"))
                {
                    temp1 += "events";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                if (PageAccess.ToLower().Contains("subscribers"))
                {
                    temp1 = "subscribers";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                if (PageAccess.ToLower().Contains("customers"))
                {
                    temp1 += "customers";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                if (PageAccess.ToLower().Contains("blog"))
                {
                    temp1 += "blog";
                }
                if (temp1.Length > 0)
                    temp1 += ",";

                PageAccess = temp1;

                //If User is Regular Admin or Super Admin
                if (userType < 3)
                {
                    //for super admin only
                    if (userType <= 1)
                        PageAccess += ",module";
                }
                else if (userType == 3)//If User is EVent Admin
                {
                    temp = string.Empty;
                    if (PageAccess.ToLower().Contains("users"))
                        temp = "users";
                    if (temp.Length > 0)
                        temp += ",";
                    if (PageAccess.ToLower().Contains("events"))
                        temp += "events";
                    if (temp.Length > 0)
                        temp += ",";
                    if (PageAccess.ToLower().Contains("eorder"))
                        temp += "eorder";

                    PageAccess = temp;
                }
                else if (userType == 4)//If User is Admin Personnel
                {
                    temp = string.Empty;
                    if (PageAccess.Contains("eorder"))
                        temp += "eorder";
                    PageAccess = temp;
                }

                //Common for all user types
                PageAccess += ",sitehome,logout";

                string[] arrPages = PageAccess.Split(',');

                if (arrPages.Length > 0)
                {
                    userDesktop = "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" id=\"shortcut_icons_table\" width=\"100%\">\n<tr>\n";

                    for (int index = 0; index < arrPages.Length; index++)
                    {
                        moduleCode = Convert.ToString(arrPages[index]).ToLower();

                        if (objHash.ContainsKey(moduleCode) == true)
                        {


                            //Add td
                            userDesktop += "<td class=\"id_shortcut_icon_td\" nowrap=\"nowrap\">\n";

                            userDesktop += objHash[moduleCode].ToString();

                            userDesktop += "</td>\n";

                        }
                    }
                    userDesktop += "</tr></table>";


                }
            }
            return userDesktop;
        }
        #endregion



        /// <summary>
        ///WRITTEN BY : MD
        ///Check Page Access Rights for Admin Side 
        /// </summary>
        /// <param name="litAdminMenu"></param>
        public void CheckPageAccess(Literal litAdminMenu)
        {
            int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
            //if the user is super admin he has access to all the pages
            if (Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]) >= 0)
            {
                string strName = string.Empty;

                GetCommaSeparatedActiveModules();

                //default access to desktop for all users
                if (PageAccess != string.Empty &&
                    (HttpContext.Current.Request.RawUrl.ToLower().IndexOf("unauthorized") < 0 ||
                    HttpContext.Current.Request.RawUrl.ToLower().IndexOf("desktop") < 0))
                {

                    //get the Page Keyword
                    strName = GetPageKeyword("pagename.xml");

                    //Check the user has access to current page request
                    //Match the Keyword with the Access Keywords
                    if (strName.Length > 0 && this.PageAccess.ToUpper().IndexOf(strName) < 0)
                        ConfigClass.Redirect("unauthorized.aspx");
                    else
                    {
                        string temp = string.Empty;
                        if (userType < 3)
                        {
                            //super admin/School Administrator
                            //for super admin only
                            if (userType <= 1)//super admin
                                PageAccess += ",module,eventreport,patronreport1,patronreport2,accesscontrol,eventstatus,performancereport,transactionreport,paymentreport,instructions,schools,product,pcategory,porder,eorder,admindynamix,ecategory,events,eventbasic,elocation,treportall,library,pactivityreport,producttransactionreport,orderreport,productreport";
                            else if (userType <= 2)//super admin,SCHOOL ADMIN
                                PageAccess += ",module,patronreport1,patronreport2,eventreport,performancereport,transactionreport,paymentreport,instructions,product,pcategory,porder,parents,events,eventbasic,elocation,eorder,ecategory,treportall,library,pactivityreport,producttransactionreport,orderreport";
                            else if (userType > 2)
                                PageAccess += ",module,patronreport1,patronreport2,eventreport,performancereport,transactionreport,paymentreport,instructions,eventbasic,ecategory,eorder";
                        }
                        else if (userType == 3)//If User is Event Admin
                        {
                            temp = string.Empty;
                            if (PageAccess.ToLower().Contains("users"))
                                temp = "users";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("events"))
                                temp += "events";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("eventbasic"))
                                temp += "eventbasic";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("eorder"))
                                temp += "eorder";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("stickets"))
                                temp += "stickets";
                            if (temp.Length > 0)
                                temp += ",";
                            temp += "events,eventbasic,eventreport,patronreport1,patronreport2,performancereport,transactionreport,paymentreport,eorder";


                            PageAccess = temp;
                        }
                        else if (userType == 5)//If User is Agent
                        {
                            temp = string.Empty;

                            if (PageAccess.ToLower().Contains("eorder"))
                                temp += "eorder";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("stickets"))
                                temp += "stickets";

                            PageAccess = temp;
                        }
                        else if (userType == 4)//If User is Admin Personnel
                        {
                            temp = string.Empty;
                            if (PageAccess.Contains("eorder"))
                                temp += "eorder";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("stickets"))
                                temp += "stickets";

                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("stickets"))
                                temp += "";

                            if (temp.Length > 0)
                                temp += ",";
                            temp += "eventbasic";


                            PageAccess = temp;
                        }
                        //else if (userType == 0 || userType == 1)//If User is super admin
                        //{
                        //    PageAccess += "dashboardcontent,faqlisting,loginscreen";
                        //}
                        if (userType != 5 && userType != 4)
                        {
                            PageAccess += "surveylist,";
                        }
                        //Common for all user types
                        PageAccess += "sitehome,logout";

                        //Write JS Menu 
                        litAdminMenu.Text = WriteScriptMenu(PageAccess);
                    }
                }
                ////else
                ////    HttpContext.Current.ConfigClass.Redirect("unauthorized.aspx");

            }
            else
            {
                //JS Menu
                litAdminMenu.Text = "<script type=\"text/javascript\" src=\"includes/script/menu.js\"></script>";
            }

        }


        /// <summary>
        ///WRITTEN BY : vikas
        ///Check Page Access Rights for General  Side 
        /// </summary>
        /// <param name="litAdminMenu"></param>
        public void CheckPageAccess()
        {
            string strName = string.Empty;
            GetCommaSeparatedActiveModules();

            //get the Page Keyword
            strName = GetPageKeyword("pagenamegeneral.xml");

            //Check the user has access to current page request
            //Match the Keyword with the Access Keywords
            if (strName.Length > 0 && this.PageAccess.ToUpper().IndexOf(strName) < 0)
                ConfigClass.Redirect("error.aspx");
        }

        //Match the Current Page Request with the Page Names Mentioned in the xml file 
        //and if matches then get the keyword for that page
        //Because in the database only kwywords are stored
        private string GetPageKeyword(string xmlFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNodeList xmlNodeList;
            string strName = string.Empty;

            xmlDoc.Load(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath +
                "/" + ConfigClass.XmlMessageFilePath + xmlFileName));
            xmlNodeList = xmlDoc.SelectNodes("/PAGE/NODE");

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (HttpContext.Current.Request.RawUrl.ToUpper().
                    IndexOf(xmlNode.SelectSingleNode("PAGE_NAME").InnerText.ToUpper()) > -1)
                {
                    strName = xmlNode.SelectSingleNode("PAGE_KEYWORD").InnerText;
                    break;
                }
            }
            return strName.ToUpper();
        }




        #region Javascript Menu
        /// <summary>
        /// Function to generate javascript menu dynamically
        /// Written By: Mdhillon
        /// </summary>
        /// <param name="PageAccess">string containing Page Names</param>
        /// 
        private string WriteScriptMenu(string pageAccess)
        {
            int schoolid = 0;
            int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["SchoolID"] == null)
                return string.Empty;
            else
                schoolid = Convert.ToInt32(HttpContext.Current.Session["SchoolID"]);
            Schools school = new Schools();
            school.SchoolId = schoolid;
            school.GetSchoolDetail();
            if (userType <= 1)
            {
                //Super admin has access to on/off module
                pageAccess += ",module";
            }
            //Javascript Menu
            pageAccess = pageAccess.ToLower();
            string script = "";
            if (schoolid == 0)
            {
                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],";
////			                    _cmSplit,
//			                    [null,'Site',null,null,'Site Management',"
//                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
//                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','../../index.aspx','_blank','Website Homepage'],"
//                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','../../index.aspx',null,'Inline View of Website Homepage'],"
//                                 + "],"
//                                 + "],";
            }
            else
            {
                 string rehdetails = "";
                      string Reg_Name = "";
                      string Reg_uniqueid = "";
                      posUserClass objUsers = new posUserClass();
                      objUsers.SchoolID = schoolid;
                      DataSet ds=new DataSet();
                      ds = objUsers.PosRegisterforPreview();
                      if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                      {
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                          Reg_Name = "";
                                          Reg_uniqueid = "";
                                          Reg_Name = Convert.ToString(ds.Tables[0].Rows[i]["REG_NAME"]);
                                          Reg_uniqueid = Convert.ToString(ds.Tables[0].Rows[i]["REG_UNIQUE_VAL"]);
                                          if (Reg_Name.Length > 0 && Reg_uniqueid.Length > 0)
                                          {
                                                    if (rehdetails.Length == 0)
                                                              rehdetails = "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview POS Front', null, null, 'Preview',";
                                                    rehdetails = rehdetails + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','" + Reg_Name + "','" + ConfigClass.POSSite + "Login/LogIn/" + school.SubDomain + "/" + Reg_uniqueid + "','_blank','Website Homepage'],";                 
                                          }
                                }
                                if (rehdetails.Length > 0)
                                {
                                          rehdetails = rehdetails + "],";
                                }
                      }

                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],
			                    _cmSplit,
			                    [null,'Site',null,null,'Site Management',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','/" + school.SubDomain + "/index.aspx','_blank','Website Homepage'],"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','/" + school.SubDomain + "/index.aspx',null,'Inline View of Website Homepage'],"
                                + "],"
                                + rehdetails
                                + "],";
            }
            //----------------------------- ----------USERS-----------------------------------------------
            if (userType != 5 && userType != 4)//5 : agent   4 : admin personnel
            {
                script += "[null,'Manage',null,null,'Module Management',";
                if (schoolid > 0 && pageAccess.IndexOf("parents") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listparents.aspx',null,'Manage Parents']";
                    //if (schoolid == 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    //script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditparent.aspx',null,'Add/Edit Parents']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                else
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                if (userType == 1 && schoolid < 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Parents Homepage Alert', 'homepagealertfront.aspx', null, 'FAQ'],";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Coordinators Alert', 'homepagealerttocordi.aspx', null, 'FAQ'],";
                }

                if (pageAccess.IndexOf("users") > -1)
                {
                    if (schoolid > 0)
                    {
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listusers.aspx', null, 'Manage Users'],";

                        //----Register start
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Register', null, null, 'Manage Library',"
                                + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','registerassigalist.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditRegister.aspx',null,'Add/Edit Library']";
                        if (userType <= 1)
                            script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/View Register to Schools','registermaster.aspx',null,'Add/Edit Library']";
                        script += "],";
                        // Register End
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Pos Users', 'listPosUsers.aspx', null, 'Manage Pos Users'],";

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Membership Type', null, null, 'Manage Membership Type',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listMemtype.aspx',null,'Manage Membership Type']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditMemtype.aspx',null,'Add/Edit Membership Type']";
                        script += "],";


                        //----Product Discount start
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Discount', null, null, 'Manage Discount',"
                                + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listDiscount.aspx',null,'Manage Discount']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','adddiscount.aspx',null,'Add/Edit Discount']";
                        script += "],";
                        // Product Discount  End

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Left Banner', null, null, 'Manage Left Banner',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listhomeimages.aspx',null,'Manage Left Banner']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addedithomebanner.aspx',null,'Add/Edit Left Banner']";
                        script += "],";



                    }
                    else
                    {
                        // if (Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                        if (userType == 1)
                        {
                            script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listadminusers.aspx', null, 'Manage Users'],";
                        }
                    }


                }

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries'],";
                
                }
                if (schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Delivery Option Appearance', 'manageDeliveryOPTApperance.aspx', null, 'Manage Delivery Option Appearance'],";
                }

                if (pageAccess.IndexOf("dynamic") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Dynamic Content', 'listdynamicpages.aspx', null, 'View Dynamic Content'],";
                }
                if (schoolid == 0)
                {
                    if (pageAccess.IndexOf("admindynamix") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Admin Dynamic Content', 'listadmindynamicpages.aspx', null, 'View Admin Dynamic Content'],";
                    }
                }
                if (userType > 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ'],";
                }
                if (userType <= 4 && pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }
                if (userType <= 2 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Class', 'listclasses.aspx', null, 'Manage Class',";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage CheckOut Content', 'addeditcheckoutcontent.aspx', null, 'Manage CheckOut Content',";
                    script += "],";
                    if (!school.TuckshopModule)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Enroll Comment', 'addeditenrollcomment.aspx', null, 'Manage Enroll Comment',";
                        script += "],";
                    }
                }
                if (userType <= 1)
                {//super admin
                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', 'schools.aspx', null, 'Manage Schools'],";
                    if (schoolid == 0 && pageAccess.IndexOf("schools") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Order Label', 'addeditcheckoutlabels.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Event Order Status', null, null, 'Manage Event Order Status',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','eventlistorderstatus.aspx',null,'Manage Event Order Status']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','eventorderstatusaddedit.aspx',null,'Add Event Order Status']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', null, null, 'Manage Schools',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','schools.aspx',null,'Manage Schools']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditschool.aspx',null,'Add School']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage e-Merchant', null, null, 'Manage e-Merchant',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listtranstype.aspx',null,'Manage e-Merchant']";
                        script += "],";

                        //---email template
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Email Template', null, null, 'Manage Email Template',"
                           + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','emailtemplatelisting.aspx',null,'Manage Email Template']";
                        script += "],";
                        //---custom message
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Custom Messages', null, null, 'Manage Custom Messages',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','custommsglist.aspx',null,'Manage Custom Messages']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Edit Financial History Notes', 'editsuperadminmessages.aspx', null, 'Manage Checkout Label',";
                        script += "],";

                    }

                    if (schoolid > 0 && pageAccess.IndexOf("library") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Library', null, null, 'Manage Library',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listlibrary.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditlibrary.aspx',null,'Add/Edit Library']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Notes', 'addedittooptip.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("news") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'News', null, null, 'News',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View News','listnews.aspx',null,'Manage News']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add News','addeditnews.aspx',null,'Add News']";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("subscribers") > -1)
                    {
                        //script += "_cmSplit,"
                        //        + "[null,'Newsletter Subscribers',null,null,'Newsletter Subscribers',";
                        //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Subscribers','listcustomers.aspx',null,'View Subscribers']"
                        //        + "],";

                        //script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Newsletter Subscribers, 'listcustomers.aspx', null, 'Newsletter Subscribers'],";
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'View Subscribers', 'listcustomers.aspx', null, 'View Subscribers',";
                        script += "],";


                    }



                    //if (pageAccess.IndexOf("paidfreeevents") > -1)
                    //{
                    //    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Paid Free Events', 'managepaidfreeevents.aspx', null, 'Manage Paid Free Events',";
                    //    script += "],";
                    //}
                }


                script += " ],";

            }

            if (userType == 4)
            {//admin personnel
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',"
                         + "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', null, null, 'Manage Users',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listusers.aspx',null,'Manage Users']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                script += "],";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries',";
                    script += "],";
                }

                if (pageAccess.IndexOf("dynamic") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Dynamic Content', 'listdynamicpages.aspx', null, 'Manage Dynamic Content',";
                    script += "],";
                }

                if (pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }

                script += " ],";
            }
            if (userType == 5)
            {//agent
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                script += " ],";
            }
            //----------------------------Super Admin Setting-------------------------------------------
            if (schoolid == 0 || pageAccess.IndexOf("faqlisting") > -1 || pageAccess.IndexOf("dashboardcontent") > -1 || pageAccess.IndexOf("loginscreen") > -1 || pageAccess.IndexOf("stickets") > -1)
            {
                script += "_cmSplit,[null,'Manage Settings',null,null,'Manage Settings',";
                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facility GST','faFacilityManageGST.aspx',null,'Manage Facility GST']";
                }


                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Parameters','parameterlist.aspx',null,'Manage Parameters']";
                }
                if (pageAccess.IndexOf("dashboardcontent") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Dashboard Content','dashboadmanage.aspx',null,'Manage Dashboard Content']";
                }
                if (schoolid == 0 && pageAccess.IndexOf("faqlisting") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage FAQ','listfaq.aspx',null,'Manage FAQ']";
                }
                if (pageAccess.IndexOf("loginscreen") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Login Screen Content','addscreenicon.aspx',null,'Manage Login Screen Content']";
                }
                if (pageAccess.IndexOf("stickets") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Support Ticket','supportticketlisting.aspx',null,'Manage Support Ticket']";
                }
                if (userType > 1 && schoolid > 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Home Page Alert','homepagealert.aspx',null,'Manage Support Ticket']";
                //script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Home Page Alert', 'homepagealert.aspx', null, 'Manage Home Page Alert'],";


                script += " ],";
            }
            //----------------------------PRODUCT CATEGORIES-------------------------------------------
            if (school.TuckshopModule && pageAccess.IndexOf("pcategory") > -1 && schoolid > 0)
            {
                script += "_cmSplit,[null,'Tuckshop Categories',null,null,'Tuckshop Categories',";
                //Category
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Categories', null, null, 'Manage Product Categories',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productcategorieslist.aspx',null,'Manage Product Categories']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productcategoryaddedit.aspx',null,'Add/Edit Product Categories']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Sub Categories', null, null, 'Manage Product Sub Categories',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productsubcategorylist.aspx',null,'Manage Product Sub Categories']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productsubcategoryaddedit.aspx',null,'Add/Edit Product Sub Categories']";
                script += "], ],";
            }
            else
            {
                if (school.EStoreModule && pageAccess.IndexOf("pcategory") > -1 && schoolid > 0)
                {
                    script += "_cmSplit,[null,'eStore Categories',null,null,'eStore Categories',";
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Categories', null, null, 'Manage Product Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productcategorieslist.aspx',null,'Manage Product Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productcategoryaddedit.aspx',null,'Add/Edit Product Categories']";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Sub Categories', null, null, 'Manage Product Sub Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productsubcategorylist.aspx',null,'Manage Product Sub Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productsubcategoryaddedit.aspx',null,'Add/Edit Product Sub Categories']";
                    script += "], ],";
                }
            }
            //-----------------------------PRODUCTS----------------------------------------------
            //////if (school.TuckshopModule && schoolid > 0 && (pageAccess.IndexOf("product") > -1 || pageAccess.IndexOf("pcategory") > -1 || pageAccess.IndexOf("porder") > -1))
            //////{
            //////    script += "_cmSplit,[null,'Tuckshop',null,null,'Tuckshop',";
            //////}
            //////else if (school.EStoreModule && schoolid > 0 && (pageAccess.IndexOf("product") > -1 || pageAccess.IndexOf("pcategory") > -1 || pageAccess.IndexOf("porder") > -1))
            //////{
            //////    script += "_cmSplit,[null,'eStore',null,null,'eStore',";
            //////}
            if ((school.EStoreModule || school.TuckshopModule) && schoolid > 0 && (pageAccess.IndexOf("product") > -1 || pageAccess.IndexOf("pcategory") > -1 || pageAccess.IndexOf("porder") > -1))
            {
                script += "_cmSplit,[null,'" + ConfigClass.Heading + "',null,null,'" + ConfigClass.Heading + "',";

                if (school.TuckshopModule)
                {
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Meal Session', null, null, 'Manage Meal Session',"
                    + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','tuckshopmealsessionlisting.aspx',null,'Manage Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','tuckshopmealsession.aspx',null,'Add/Edit Meal Sessions']";
                    script += "],";
                }
                if (pageAccess.IndexOf("product") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products', null, null, 'Manage Products',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlist.aspx',null,'Manage Products']";

                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddedit.aspx',null,'Add/Edit Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Import/Export Products','productimport.aspx',null,'Import/Export Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Import/Export Product Attributes','productattributeimport.aspx',null,'Import/Export Product Attributes']";
                    script += "],";

                    //event product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Food', null, null, 'Manage Event Food',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlistfood.aspx',null,'Manage Products']";

                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddeditnewfood.aspx',null,'Add/Edit Products']";
                    script += "],";
                    //Product Bundle
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Bundles', null, null, 'Manage Product Bundles',"
                        + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productbundlelist.aspx',null,'Manage Product Bundles']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productbundleaddedit.aspx',null,'Add/Edit Product Bundles']";
                    script += "],";
                }

                if (schoolid > 0 && pageAccess.IndexOf("porder") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                    script += "],";

                    //script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Bundle Orders', null, null, 'Manage Products Bundle Orders',";
                    //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductbundleorders.aspx',null,'View Orders']";
                    //script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Order Status', null, null, 'Manage Order Status',";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listorderstatus.aspx',null,'Manage Order Status']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/Edit Order Status','orderstatusaddedit.aspx',null,'Add/Edit Order Status']";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Order Financial Status', null, null, 'Manage Order Financial Status',";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','orderfinalcialstatuslist.aspx',null,'Manage Order Financial Status']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/Edit Order Financial Status','orderfinalcialstatus.aspx',null,'Add/Edit Order Financial Status']";
                    script += "],";
                }
                script += " ],";
            }
            //-----------------------------Gallery----------------------------------------------
            if (pageAccess.IndexOf("gallery") > -1 || pageAccess.IndexOf("gcategory") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Gallery',null,null,'Manage Gallery',";

                if (pageAccess.IndexOf("gcategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Categories', null, null, 'Manage Gallery Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgallerycategories.aspx',null,'Manage Gallery Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgallerycategory.aspx',null,'Add/Edit Gallery Categories']";
                    script += "],";
                }

                if (pageAccess.IndexOf("gallery") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Photos', null, null, 'Manage Gallery Photos',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgalleryphotos.aspx',null,'Manage Gallery Photos']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgalleryphotos.aspx',null,'Add/Edit Gallery Photos']";
                    script += "],";
                }
                script += " ],";

            }
            //-----------------------------Events----------------------------------------------
            if (school.EventModule && schoolid > 0 && (pageAccess.IndexOf("events") > -1 || pageAccess.IndexOf("eorder") > -1 || pageAccess.IndexOf("surveylist") > -1))
            {

                script += "_cmSplit,"
                       + "[null,'Events',null,null,'Events',";

                if (pageAccess.IndexOf("events") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Paid Events', null, null, 'Manage Paid Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";
                    script += "],";

                }


                if (pageAccess.IndexOf("events") > -1 || pageAccess.IndexOf("eorder") > -1)
                {
                    //event product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Food Items', null, null, 'Manage Event Food Items',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlistfood.aspx',null,'Manage Event Food Itemss']";

                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddeditnewfood.aspx',null,'Add/Edit Event Food Items']";
                    script += "],";

                }



                if (pageAccess.IndexOf("eventbasic") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Basic Events', null, null, 'Manage Basic Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventsbasic.aspx',null,'Manage Basic Events']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventbasic.aspx',null,'Add/Edit Basic Events']";
                    script += "],";
                }
                if (pageAccess.IndexOf("eorder") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Orders', null, null, 'Manage Event Orders',"
                             + ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','View Order','vieweventorders.aspx',null,'View Order']";
                    script += "],";
                }
                //if (pageAccess.IndexOf("surveylist") > -1)
                //{
                //    //Product
                //    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Survey', null, null, 'Manage Survey',"
                //           + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','#listsurvey.aspx',null,'Manage Survey']";
                //    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','#addeditsurvey.aspx',null,'Add Survey']";
                //    script += "],";
                //}
                if (pageAccess.IndexOf("ecategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event category', null, null, 'Manage Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventcategories.aspx',null,'Manage Event category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventcategory.aspx',null,'Add/Edit Event category']";
                    script += "],";
                }
                if (pageAccess.IndexOf("elocation") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Location category', null, null, 'Manage Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventlocation.aspx',null,'Manage Location category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventlocation.aspx',null,'Add/Edit Location category']";
                    script += "],";
                }

                script += " ],";

            }

            //------------------------------------DYNAMIC CONTENT------------------------
            ////if (PageAccess.IndexOf("dynamic") > -1)
            ////{
            ////    script += "_cmSplit,"
            ////           + "[null,'Manage Dynamic Content',null,null,'Manage Dynamic Content',";
            ////    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listdynamicpages.aspx',null,'View Dynamic Pages']"
            ////            + "],";
            ////}

            //------------------------------------Contact Us list------------------------
            ////if (PageAccess.IndexOf("contact") > -1)
            ////{
            ////    script += "_cmSplit,"
            ////           + "[null,'Manage Contact Us',null,null,'Manage Contact Us',";
            ////    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Contact Us Queries','contactuslisting.aspx',null,'View Contact Us Queries']"
            ////           + "],";
            ////}

            //------------------------------------NEWS------------------------
            //-------16 jan
            ////////////if (schoolid > 0 && pageAccess.IndexOf("news") > -1)
            ////////////{
            ////////////    script += "[null, 'News', null, null, 'News',"
            ////////////        + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View News','listnews.aspx',null,'Manage News']";
            ////////////    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add News','addeditnews.aspx',null,'Add News']";
            ////////////    script += "],";
            ////////////}
            //------------------------------------Newsletter Subscribers------------------------
            //------- 16 jan
            //////////////if (pageAccess.IndexOf("subscribers") > -1)
            //////////////{
            //////////////    script += "_cmSplit,"
            //////////////            + "[null,'Newsletter Subscribers',null,null,'Newsletter Subscribers',";
            //////////////    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Subscribers','listcustomers.aspx',null,'View Subscribers']"
            //////////////            + "],";
            //////////////}
            //------------------------------------Registerred Members------------------------
            if (pageAccess.IndexOf("customers") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Members',null,null,'Manage Members',";
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Registered Users','listmembers.aspx',null,'View Registered Users']"
                       + "],";
            }

            if (userType == 4)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Report',null,null,'Manage Report',";
                int used = 0;
                if (used == 1)
                {
                    script += ",";
                }
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx',null,'Patron Report 2']";
                used = 1;
                script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {//for supera admin
                //////if (school.TuckshopModule)
                //////{
                //////    script += "_cmSplit,"
                //////           + "[null,'Tuckshop Reports',null,null,'Tuckshop Reports',";
                //////}
                //////else //if (school.EStoreModule)
                //////{
                //////    script += "_cmSplit,"
                //////          + "[null,'eStore Reports',null,null,'eStore Reports',";
                //////}
                script += "_cmSplit,"
                    + "[null,'" + ConfigClass.Heading + "',null,null,'" + ConfigClass.Heading + "',";
                int used = 0;
                if (pageAccess.IndexOf("producttransactionreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Transaction Report','producttransactionreport.aspx',null,'Product Transaction Report']";
                    used = 1;
                }
                if (ConfigClass.UserType == 1 && pageAccess.IndexOf("orderreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Absorbed','orderreport.aspx?absorb=11',null,'All Fees Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreport.aspx?absorb=00',null,'All Fees Not Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Surcharge Absorbed','orderreport.aspx?absorb=10',null,'Surcharge Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreport.aspx?absorb=01',null,'Paypal Fees Absorbed']";
                    // script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report','orderreportall.aspx',null,'Financial Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report For Live School','orderreportall.aspx?demo=no',null,'Financial Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report For Demo School','orderreportall.aspx?demo=yes',null,'Financial Report']";

                    used = 1;
                }
                script += " ],";
            }
            else if (ConfigClass.UserType < 3)
            {
                ////////if (school.TuckshopModule)
                ////////{
                ////////    script += "_cmSplit,"
                ////////           + "[null,'Tuckshop Reports',null,null,'Tuckshop Reports',";
                ////////}
                ////////else //if (school.EStoreModule)
                ////////{
                ////////    script += "_cmSplit,"
                ////////          + "[null,'eStore Reports',null,null,'eStore Reports',";
                ////////}
                script += "_cmSplit,"
                       + "[null,'" + ConfigClass.Heading + " Reports',null,null,'" + ConfigClass.Heading + " Reports',";
                int used = 0;
                if (used == 1)
                {
                    script += ",";
                }
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Attribute Sale Report','productAttributeSalesreport.aspx',null,'Product Attribute Sale Report']";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Wholesale Report','productWholeSalesreport.aspx',null,'Product Wholesale Report']";
                used = 1;

                if (pageAccess.IndexOf("treportall") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report All','transactionreportall.aspx',null,'Transaction Report All']";
                    used = 1;
                }

               

                if (pageAccess.Contains("pactivityreport"))
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Activity Report','productactivityreport.aspx',null,'Product Activity Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Stock Report','stockreport.aspx',null,'Stock Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Production Report','estoreproductionreport.aspx',null,'eStore Production Report']";

                    if (ConfigClass.UserType == 2)
                    {
                        script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report','ordersreportall.aspx',null,'Financial Report']";
                        script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Order Report','viewproductordersreport.aspx',null,'Product Order Report']";
                        //    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreportcordi.aspx',null,'Paypal Fees Absorbed']";
                        //    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreportcordina.aspx',null,'All Fees Not Absorbed']";
                    }
                    used = 1;
                }
                script += " ],";
            }



            if (schoolid > 0 && ConfigClass.UserType == 2)
            {
                script += "_cmSplit,"
                       + "[null,'Financial Reports',null,null,'Financial Reports'";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreportcordi.aspx',null,'Paypal Fees Absorbed']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreportcordina.aspx',null,'All Fees Not Absorbed']";

                // script += " ],";
                //demo
                script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {
                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";

                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Report','productreports.aspx',null,'Product Report']";
                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Report','estorereports.aspx',null,'eStore Report']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','eventreports.aspx',null,'Event Report']";

                script += " ],";
                //demo
                script += "_cmSplit,"
                       + "[null,'Manuals',null,null,'Manuals'";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Demo and Training Material','demoTrainingMaterialfiles.aspx',null,'Manage Demo and Training Material']";
                script += " ],";
            }
            else
            {

                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";

                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Report','productreports.aspx',null,'Product Report']";

                //if (school.TuckshopModule)
                //    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Tuckshop Report','estorereports.aspx',null,'Tuckshop Report']";
                //else if (school.EStoreModule)
                //    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Report','estorereports.aspx',null,'eStore Report']";
                if (school.EventModule)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','eventreports.aspx',null,'Event Report']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Parent Login History','customerloginHistory.aspx',null,'Parent Login History']";

                script += " ],";
            }

            if (school.EventModule)
            {
                if (schoolid > 0 && (pageAccess.IndexOf("eventreport") > -1 || pageAccess.IndexOf("patronreport2") > -1 || pageAccess.IndexOf("accesscontrol") > -1 || pageAccess.IndexOf("eventstatus") > -1 || pageAccess.IndexOf("attreport") > -1))
                {
                    script += "_cmSplit,"
                           + "[null,'Event Reports',null,null,'Event Reports',";
                    int used = 0;

                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Paid Events', null, null, 'Paid Events',";
                    //+ "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                    // script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";



                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paid Event','reportevent.aspx',null,'Event Report']";
                    if (pageAccess.IndexOf("eventreport") > -1)
                    {

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','reportevent.aspx?etype=p',null,'Event Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport1") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 1','patronreport1.aspx?etype=p',null,'Patron Report 1']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport2") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx?etype=p',null,'Patron Report 2']";
                        script += ",";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 3','patronreport3.aspx?etype=p',null,'Patron Report 3']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("accesscontrol") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Access Control Download Report','reportaccesscontrol.aspx?etype=p',null,'Access Control Download Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("eventstatus") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Status Report','eventstatusreport.aspx?etype=p',null,'Event Status Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("performancereport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Performance Report','performancereport.aspx?etype=p',null,'Performance Report']";
                        used = 1;
                    }

                    if (pageAccess.IndexOf("transactionreport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report','transactionreport.aspx?etype=p',null,'Transaction Report']";
                        used = 1;
                    }

                    if (pageAccess.IndexOf("paymentreport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Payment Report','paymentreport.aspx?etype=p',null,'Payment Report']";
                        used = 1;
                    }
                    script += "],";


                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Basic Events', null, null, 'Paid Events',";
                    //+ "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                    // script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";



                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paid Event','reportevent.aspx',null,'Event Report']";
                    if (pageAccess.IndexOf("eventreport") > -1)
                    {

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','reportevent.aspx?etype=b',null,'Event Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport1") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 1','patronreport1.aspx?etype=b',null,'Patron Report 1']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport2") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx?etype=b',null,'Patron Report 2']";
                        script += ",";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 3','patronreport3.aspx?etype=b',null,'Patron Report 3']";

                        used = 1;
                    }
                    if (pageAccess.IndexOf("accesscontrol") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Access Control Download Report','reportaccesscontrol.aspx?etype=b',null,'Access Control Download Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("eventstatus") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Status Report','eventstatusreport.aspx?etype=b',null,'Event Status Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("performancereport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Performance Report','performancereport.aspx?etype=b',null,'Performance Report']";
                        used = 1;
                    }

                    if (pageAccess.IndexOf("transactionreport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report','transactionreport.aspx?etype=b',null,'Transaction Report']";
                        used = 1;
                    }

                    //if (pageAccess.IndexOf("paymentreport") > -1)
                    //{
                    //          if (used == 1)
                    //          {
                    //                    script += ",";
                    //          }
                    //          script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Payment Report','paymentreport.aspx?etype=b',null,'Payment Report']";
                    //          used = 1;
                    //}
                    script += "],";


                    script += " ],";
                }
            }
            if (school.FacilityModule)
            {
                //if (schoolid > 0 && (pageAccess.IndexOf("eventreport")>-1||pageAccess.IndexOf("patronreport2") > -1)
                if (schoolid > 0)
                { 
                    //Manage Resource
                    script += "_cmSplit,[null,'Manage Facilities',null,null,'Manage Facilities',";

                    //term
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Term', null, null, 'Term Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilityTermlisting.aspx',null,'Term Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityTermaddedit.aspx',null,'Add/Edit Term']";
                    script += "],";
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facility Category', null, null, 'Manage Facility Category',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilityCategorylisting.aspx',null,'Manage Facility Category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityCategoryaddedit.aspx',null,'Add/Edit Facilities Category']";
                    script += "],";
                    //locaton
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Location', null, null, 'Location Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilityLocationlisting.aspx',null,'Location Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityLocationaddedit.aspx',null,'Add/Edit Location']";
                    script += "],";

                    //sub location
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Sub Location', null, null, 'Sub Location Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilitySubLocationlisting.aspx',null,'Sub Location Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilitySubLocationaddedit.aspx',null,'Add/Edit Sub Location']";
                    script += "],";
                    //resource Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Resource Category', null, null, 'Resource Category Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACResourceCategorylisting.aspx',null,'Resource Category Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACResourceCategoryaddedit.aspx',null,'Add/Edit Resource Categories']";
                    script += "],";
                    //resource 
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Resource', null, null, 'Resource Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilityResourcelisting.aspx',null,'Resource Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityResourceaddedit.aspx',null,'Add/Edit Resource']";
                    script += "],";
                    //Facilities
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facilities', null, null, 'Facilities Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilitylisting.aspx',null,'Facilities Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityaddedit.aspx',null,'Add/Edit Facilities']";
                    script += "],";
                    //Facilities booking
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage booked Facilities', null, null, 'Facilities Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faBookedFacility.aspx',null,'Booked Facilities']";
                    script += "],";


                    //Facilities notes
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facility Help Notes', null, null, 'Facility Help Notes',"
                    + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Add Facility Help Notes','Facilitynotes.aspx',null,'Facility Help Notes']";
                    script += "],";
                    
                   
                  
                   
                    //Facilities parameter
                    //script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facilities Parameter', null, null, 'Manage Facilities Parameter',"
                    // + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facilities Cut Off Time                                                     ','faCutofftime.aspx',null,'Manage Facilities Cut Off Time']"
                    //   + ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Cancel Fee','faFacilityManageCancelFee.aspx',null,'Manage Cancel Fee']";
                    //script += "],";


                    //  script += " ],";

                    //----------manage resource
                    // script += "_cmSplit,[null,'Manage Resource',null,null,'Manage Resource',";
                   
                 


                    script += " ],";

                }
            }
            #region Parent Teacher module
            ////////if (school.ParentTeacherModule)
            ////////{
            ////////          //if (schoolid > 0 && (pageAccess.IndexOf("eventreport")>-1||pageAccess.IndexOf("patronreport2") > -1)
            ////////          if (schoolid > 0)
            ////////          {
            ////////                    //Manage Resource
            ////////                    script += "_cmSplit,[null,'Manage Parent-Teacher',null,null,'Manage Parent-Teacher',";
            ////////                    //Category
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Parent-Teacher Category Listing', null, null, 'Parent-Teacher Category Listing',"
            ////////                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','ptmCategoryListing.aspx',null,'Parent-Teacher Category Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','ptmCategoryaddedit.aspx',null,'Add/Edit Parent-Teacher Categories']";
            ////////                    script += "],";
            ////////                    //locaton
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Location Listing', null, null, 'Location Listing',"
            ////////                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faFacilityLocationListing.aspx',null,'Location Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faFacilityLocationaddedit.aspx',null,'Add/Edit Location']";
            ////////                    script += "],";
            ////////                    //sub location
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Sub Location Listing', null, null, 'Sub Location Listing',"
            ////////                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faSubLocationListing.aspx',null,'Sub Location Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faSubLocationaddedit.aspx',null,'Add/Edit Sub Location']";
            ////////                    script += "],";
            ////////                    //Facility
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Facility Listing', null, null, 'Facility Listing',"
            ////////                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faFacilityListing.aspx',null,'Facility Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faFacilityaddedit.aspx',null,'Add/Edit Facility']";
            ////////                    script += "],";
            ////////                    //Facility booking
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage booked Facility', null, null, 'Facility Listing',"
            ////////                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faBookedFacility.aspx',null,'Booked Facility']";
            ////////                    script += "],";
            ////////                    //Facility parameter
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facility Parameter', null, null, 'Manage Facility Parameter',"
            ////////                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facility Cut Off Time                                                     ','faCutofftime.aspx',null,'Manage Facility Cut Off Time']"
            ////////                       + ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Cancel Fee','faFacilityManageCancelFee.aspx',null,'Manage Cancel Fee']";
            ////////                    script += "],";


            ////////                    script += " ],";

            ////////                    //----------manage resource
            ////////                    script += "_cmSplit,[null,'Manage Resource',null,null,'Manage Resource',";
            ////////                    //resource Category
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Resource Category Listing', null, null, 'Resource Category Listing',"
            ////////                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faResourceCategoryListing.aspx',null,'Resource Category Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faResourceCategoryaddedit.aspx',null,'Add/Edit Resource Categories']";
            ////////                    script += "],";
            ////////                    //resource 
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Resource Listing', null, null, 'Resource Listing',"
            ////////                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faResourceListing.aspx',null,'Resource Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faResourceaddedit.aspx',null,'Add/Edit Resource']";
            ////////                    script += "],";


            ////////                    script += " ],";

            ////////          }
            ////////}
            #endregion

            script += "];"
                          + "cmDraw ('myMenuID', myMenu, 'hbr', cmThemeOffice, 'ThemeOffice');";

            return "<script type=\"text/javascript\">" + script + "</script>";

        }


        private string WriteScriptMenu_31Mar2014(string pageAccess)
        {
            int schoolid = 0;
            int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["SchoolID"] == null)
                return string.Empty;
            else
                schoolid = Convert.ToInt32(HttpContext.Current.Session["SchoolID"]);
            Schools school = new Schools();
            school.SchoolId = schoolid;
            school.GetSchoolDetail();
            if (userType <= 1)
            {
                //Super admin has access to on/off module
                pageAccess += ",module";
            }
            //Javascript Menu
            pageAccess = pageAccess.ToLower();
            string script = "";
            if (schoolid == 0)
            {
                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],
			                    _cmSplit,
			                    [null,'Site',null,null,'Site Management',"
                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','../../index.aspx','_blank','Website Homepage'],"
                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','../../index.aspx',null,'Inline View of Website Homepage'],"
                                 + "],"
                                 + "],";
            }
            else
            {
                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],
			                    _cmSplit,
			                    [null,'Site',null,null,'Site Management',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','/" + school.SubDomain + "/index.aspx','_blank','Website Homepage'],"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','/" + school.SubDomain + "/index.aspx',null,'Inline View of Website Homepage'],"
                                + "],"
                                + "],";
            }
            //----------------------------- ----------USERS-----------------------------------------------
            if (userType != 5 && userType != 4)//5 : agent   4 : admin personnel
            {
                script += "[null,'Manage',null,null,'Module Management',";
                if (schoolid > 0 && pageAccess.IndexOf("parents") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listparents.aspx',null,'Manage Parents']";
                    //if (schoolid == 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    //script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditparent.aspx',null,'Add/Edit Parents']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                else
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                if (userType == 1 && schoolid < 1)
                {
                          script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Parents Homepage Alert', 'homepagealertfront.aspx', null, 'FAQ'],";
                          script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Coordinators Alert', 'homepagealerttocordi.aspx', null, 'FAQ'],";
                }

                if (pageAccess.IndexOf("users") > -1)
                {
                    if (schoolid > 0)
                    {
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listusers.aspx', null, 'Manage Users'],";

                     


                    }
                    else
                    {
                       // if (Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                        if (userType == 1)
                        {
                            script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listadminusers.aspx', null, 'Manage Users'],";
                        }
                    }


                }

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries'],";
                }

                if (pageAccess.IndexOf("dynamic") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Dynamic Content', 'listdynamicpages.aspx', null, 'View Dynamic Content'],";
                }
                if (schoolid == 0)
                {
                    if (pageAccess.IndexOf("admindynamix") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Admin Dynamic Content', 'listadmindynamicpages.aspx', null, 'View Admin Dynamic Content'],";
                    }
                }
                if (userType > 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ'],";
                }
                if (userType <= 4 && pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }
                if (userType <= 2 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Class', 'listclasses.aspx', null, 'Manage Class',";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage CheckOut Content', 'addeditcheckoutcontent.aspx', null, 'Manage CheckOut Content',";
                    script += "],";
                    if (!school.TuckshopModule)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Enroll Comment', 'addeditenrollcomment.aspx', null, 'Manage Enroll Comment',";
                        script += "],";
                    }
                }
                if (userType <= 1)
                {//super admin
                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', 'schools.aspx', null, 'Manage Schools'],";
                    if (schoolid == 0 && pageAccess.IndexOf("schools") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Order Label', 'addeditcheckoutlabels.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Event Order Status', null, null, 'Manage Event Order Status',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','eventlistorderstatus.aspx',null,'Manage Event Order Status']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','eventorderstatusaddedit.aspx',null,'Add Event Order Status']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', null, null, 'Manage Schools',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','schools.aspx',null,'Manage Schools']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditschool.aspx',null,'Add School']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage e-Merchant', null, null, 'Manage e-Merchant',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listtranstype.aspx',null,'Manage e-Merchant']";
                        script += "],";

                        //---email template
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Email Template', null, null, 'Manage Email Template',"
                           + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','emailtemplatelisting.aspx',null,'Manage Email Template']";
                        script += "],";
                        //---custom message
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Custom Messages', null, null, 'Manage Custom Messages',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','custommsglist.aspx',null,'Manage Custom Messages']";
                        script += "],";
                        
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Edit Financial History Notes', 'editsuperadminmessages.aspx', null, 'Manage Checkout Label',";
                        script += "],";

                    }

                    if (schoolid > 0 && pageAccess.IndexOf("library") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Library', null, null, 'Manage Library',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listlibrary.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditlibrary.aspx',null,'Add/Edit Library']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Notes', 'addedittooptip.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("news") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'News', null, null, 'News',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View News','listnews.aspx',null,'Manage News']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add News','addeditnews.aspx',null,'Add News']";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("subscribers") > -1)
                    {
                        //script += "_cmSplit,"
                        //        + "[null,'Newsletter Subscribers',null,null,'Newsletter Subscribers',";
                        //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Subscribers','listcustomers.aspx',null,'View Subscribers']"
                        //        + "],";

                        //script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Newsletter Subscribers, 'listcustomers.aspx', null, 'Newsletter Subscribers'],";
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'View Subscribers', 'listcustomers.aspx', null, 'View Subscribers',";
                        script += "],";


                    }



                    //if (pageAccess.IndexOf("paidfreeevents") > -1)
                    //{
                    //    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Paid Free Events', 'managepaidfreeevents.aspx', null, 'Manage Paid Free Events',";
                    //    script += "],";
                    //}
                }


                script += " ],";

            }

            if (userType == 4)
            {//admin personnel
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',"
                         + "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', null, null, 'Manage Users',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listusers.aspx',null,'Manage Users']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                script += "],";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries',";
                    script += "],";
                }

                if (pageAccess.IndexOf("dynamic") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Dynamic Content', 'listdynamicpages.aspx', null, 'Manage Dynamic Content',";
                    script += "],";
                }

                if (pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }

                script += " ],";
            }
            if (userType == 5)
            {//agent
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                script += " ],";
            }
            //----------------------------Super Admin Setting-------------------------------------------
            if (schoolid == 0 || pageAccess.IndexOf("faqlisting") > -1 || pageAccess.IndexOf("dashboardcontent") > -1 || pageAccess.IndexOf("loginscreen") > -1 || pageAccess.IndexOf("stickets") > -1)
            {
                script += "_cmSplit,[null,'Manage Settings',null,null,'Manage Settings',";
                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facility GST','faFacilityManageGST.aspx',null,'Manage Facility GST']";
                }


                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Parameters','parameterlist.aspx',null,'Manage Parameters']";
                }
                if (pageAccess.IndexOf("dashboardcontent") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Dashboard Content','dashboadmanage.aspx',null,'Manage Dashboard Content']";
                }
                if (schoolid == 0 && pageAccess.IndexOf("faqlisting") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage FAQ','listfaq.aspx',null,'Manage FAQ']";
                }
                if (pageAccess.IndexOf("loginscreen") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Login Screen Content','addscreenicon.aspx',null,'Manage Login Screen Content']";
                }
                if (pageAccess.IndexOf("stickets") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Support Ticket','supportticketlisting.aspx',null,'Manage Support Ticket']";
                }
                if (userType > 1 && schoolid > 0)
                          script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Home Page Alert','homepagealert.aspx',null,'Manage Support Ticket']";
                //script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Home Page Alert', 'homepagealert.aspx', null, 'Manage Home Page Alert'],";

               
                script += " ],";
            }
            //----------------------------PRODUCT CATEGORIES-------------------------------------------
            if (school.TuckshopModule && pageAccess.IndexOf("pcategory") > -1 && schoolid > 0)
            {
                script += "_cmSplit,[null,'Tuckshop Categories',null,null,'Tuckshop Categories',";
                //Category
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Categories', null, null, 'Manage Product Categories',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productcategorieslist.aspx',null,'Manage Product Categories']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productcategoryaddedit.aspx',null,'Add/Edit Product Categories']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Sub Categories', null, null, 'Manage Product Sub Categories',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productsubcategorylist.aspx',null,'Manage Product Sub Categories']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productsubcategoryaddedit.aspx',null,'Add/Edit Product Sub Categories']";
                script += "], ],";
            }
            else
            {
                if (school.EStoreModule && pageAccess.IndexOf("pcategory") > -1 && schoolid > 0)
                {
                    script += "_cmSplit,[null,'eStore Categories',null,null,'eStore Categories',";
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Categories', null, null, 'Manage Product Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productcategorieslist.aspx',null,'Manage Product Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productcategoryaddedit.aspx',null,'Add/Edit Product Categories']";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Sub Categories', null, null, 'Manage Product Sub Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productsubcategorylist.aspx',null,'Manage Product Sub Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productsubcategoryaddedit.aspx',null,'Add/Edit Product Sub Categories']";
                    script += "], ],";
                }
            }
            //-----------------------------PRODUCTS----------------------------------------------
            //////if (school.TuckshopModule && schoolid > 0 && (pageAccess.IndexOf("product") > -1 || pageAccess.IndexOf("pcategory") > -1 || pageAccess.IndexOf("porder") > -1))
            //////{
            //////    script += "_cmSplit,[null,'Tuckshop',null,null,'Tuckshop',";
            //////}
            //////else if (school.EStoreModule && schoolid > 0 && (pageAccess.IndexOf("product") > -1 || pageAccess.IndexOf("pcategory") > -1 || pageAccess.IndexOf("porder") > -1))
            //////{
            //////    script += "_cmSplit,[null,'eStore',null,null,'eStore',";
            //////}
            if ((school.EStoreModule||school.TuckshopModule) && schoolid > 0 && (pageAccess.IndexOf("product") > -1 || pageAccess.IndexOf("pcategory") > -1 || pageAccess.IndexOf("porder") > -1))
            {
               script += "_cmSplit,[null,'"+ConfigClass.Heading+"',null,null,'"+ConfigClass.Heading+"',";

               if (school.TuckshopModule)
               {
                   script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Meal Session', null, null, 'Manage Meal Session',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','tuckshopmealsessionlisting.aspx',null,'Manage Products']";
                   script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','tuckshopmealsession.aspx',null,'Add/Edit Meal Sessions']";
                   script += "],";
               }
                if (pageAccess.IndexOf("product") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products', null, null, 'Manage Products',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlist.aspx',null,'Manage Products']";
                    
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddedit.aspx',null,'Add/Edit Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Import/Export Products','productimport.aspx',null,'Import/Export Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Import/Export Product Attributes','productattributeimport.aspx',null,'Import/Export Product Attributes']";
                    script += "],";
                    
                    //event product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Food', null, null, 'Manage Event Food',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlistfood.aspx',null,'Manage Products']";

                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddeditnewfood.aspx',null,'Add/Edit Products']";
                    script += "],";
                    //Product Bundle
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Bundles', null, null, 'Manage Product Bundles',"
                        + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productbundlelist.aspx',null,'Manage Product Bundles']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productbundleaddedit.aspx',null,'Add/Edit Product Bundles']";
                    script += "],";
                }

                if (schoolid > 0 && pageAccess.IndexOf("porder") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                    script += "],";

                    //script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Bundle Orders', null, null, 'Manage Products Bundle Orders',";
                    //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductbundleorders.aspx',null,'View Orders']";
                    //script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Order Status', null, null, 'Manage Order Status',";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listorderstatus.aspx',null,'Manage Order Status']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/Edit Order Status','orderstatusaddedit.aspx',null,'Add/Edit Order Status']";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Order Financial Status', null, null, 'Manage Order Financial Status',";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','orderfinalcialstatuslist.aspx',null,'Manage Order Financial Status']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/Edit Order Financial Status','orderfinalcialstatus.aspx',null,'Add/Edit Order Financial Status']";
                    script += "],";
                }
                script += " ],";
            }
            //-----------------------------Gallery----------------------------------------------
            if (pageAccess.IndexOf("gallery") > -1 || pageAccess.IndexOf("gcategory") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Gallery',null,null,'Manage Gallery',";

                if (pageAccess.IndexOf("gcategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Categories', null, null, 'Manage Gallery Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgallerycategories.aspx',null,'Manage Gallery Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgallerycategory.aspx',null,'Add/Edit Gallery Categories']";
                    script += "],";
                }

                if (pageAccess.IndexOf("gallery") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Photos', null, null, 'Manage Gallery Photos',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgalleryphotos.aspx',null,'Manage Gallery Photos']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgalleryphotos.aspx',null,'Add/Edit Gallery Photos']";
                    script += "],";
                }
                script += " ],";

            }
            //-----------------------------Events----------------------------------------------
            if (school.EventModule && schoolid > 0 && (pageAccess.IndexOf("events") > -1 || pageAccess.IndexOf("eorder") > -1 || pageAccess.IndexOf("surveylist") > -1))
            {

                script += "_cmSplit,"
                       + "[null,'Events',null,null,'Events',";

                if (pageAccess.IndexOf("events") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Paid Events', null, null, 'Manage Paid Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";
                    script += "],";

                }

                
  if (pageAccess.IndexOf("events") > -1 || pageAccess.IndexOf("eorder") > -1)
{
//event product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Food Items', null, null, 'Manage Event Food Items',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlistfood.aspx',null,'Manage Event Food Itemss']";

                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddeditnewfood.aspx',null,'Add/Edit Event Food Items']";
                    script += "],";

  }

               

                if (pageAccess.IndexOf("eventbasic") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Basic Events', null, null, 'Manage Basic Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventsbasic.aspx',null,'Manage Basic Events']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventbasic.aspx',null,'Add/Edit Basic Events']";
                    script += "],";
                }
                if (pageAccess.IndexOf("eorder") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Orders', null, null, 'Manage Event Orders',"
                             + ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','View Order','vieweventorders.aspx',null,'View Order']";
                    script += "],";

                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event category', null, null, 'Manage Events',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventcategories.aspx',null,'Manage Event category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventcategory.aspx',null,'Add/Edit Event category']";
                    script += "],";
                }
                //if (pageAccess.IndexOf("surveylist") > -1)
                //{
                //    //Product
                //    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Survey', null, null, 'Manage Survey',"
                //           + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','#listsurvey.aspx',null,'Manage Survey']";
                //    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','#addeditsurvey.aspx',null,'Add Survey']";
                //    script += "],";
                //}
                if (pageAccess.IndexOf("ecategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event category', null, null, 'Manage Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventcategories.aspx',null,'Manage Event category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventcategory.aspx',null,'Add/Edit Event category']";
                    script += "],";
                }
                if (pageAccess.IndexOf("elocation") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Location category', null, null, 'Manage Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventlocation.aspx',null,'Manage Location category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventlocation.aspx',null,'Add/Edit Location category']";
                    script += "],";
                }

                script += " ],";

            }

            //------------------------------------DYNAMIC CONTENT------------------------
            ////if (PageAccess.IndexOf("dynamic") > -1)
            ////{
            ////    script += "_cmSplit,"
            ////           + "[null,'Manage Dynamic Content',null,null,'Manage Dynamic Content',";
            ////    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listdynamicpages.aspx',null,'View Dynamic Pages']"
            ////            + "],";
            ////}

            //------------------------------------Contact Us list------------------------
            ////if (PageAccess.IndexOf("contact") > -1)
            ////{
            ////    script += "_cmSplit,"
            ////           + "[null,'Manage Contact Us',null,null,'Manage Contact Us',";
            ////    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Contact Us Queries','contactuslisting.aspx',null,'View Contact Us Queries']"
            ////           + "],";
            ////}

            //------------------------------------NEWS------------------------
            //-------16 jan
            ////////////if (schoolid > 0 && pageAccess.IndexOf("news") > -1)
            ////////////{
            ////////////    script += "[null, 'News', null, null, 'News',"
            ////////////        + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View News','listnews.aspx',null,'Manage News']";
            ////////////    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add News','addeditnews.aspx',null,'Add News']";
            ////////////    script += "],";
            ////////////}
            //------------------------------------Newsletter Subscribers------------------------
            //------- 16 jan
            //////////////if (pageAccess.IndexOf("subscribers") > -1)
            //////////////{
            //////////////    script += "_cmSplit,"
            //////////////            + "[null,'Newsletter Subscribers',null,null,'Newsletter Subscribers',";
            //////////////    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Subscribers','listcustomers.aspx',null,'View Subscribers']"
            //////////////            + "],";
            //////////////}
            //------------------------------------Registerred Members------------------------
            if (pageAccess.IndexOf("customers") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Members',null,null,'Manage Members',";
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Registered Users','listmembers.aspx',null,'View Registered Users']"
                       + "],";
            }

            if (userType == 4)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Report',null,null,'Manage Report',";
                int used = 0;
                if (used == 1)
                {
                    script += ",";
                }
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx',null,'Patron Report 2']";
                used = 1;
                script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {//for supera admin
                //////if (school.TuckshopModule)
                //////{
                //////    script += "_cmSplit,"
                //////           + "[null,'Tuckshop Reports',null,null,'Tuckshop Reports',";
                //////}
                //////else //if (school.EStoreModule)
                //////{
                //////    script += "_cmSplit,"
                //////          + "[null,'eStore Reports',null,null,'eStore Reports',";
                //////}
                script += "_cmSplit,"
                    + "[null,'"+ConfigClass.Heading+"',null,null,'"+ConfigClass.Heading+"',";
                int used = 0;
                if (pageAccess.IndexOf("producttransactionreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Transaction Report','producttransactionreport.aspx',null,'Product Transaction Report']";
                    used = 1;
                }
                if (ConfigClass.UserType == 1 && pageAccess.IndexOf("orderreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Absorbed','orderreport.aspx?absorb=11',null,'All Fees Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreport.aspx?absorb=00',null,'All Fees Not Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Surcharge Absorbed','orderreport.aspx?absorb=10',null,'Surcharge Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreport.aspx?absorb=01',null,'Paypal Fees Absorbed']";
                   // script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report','orderreportall.aspx',null,'Financial Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report For Live School','orderreportall.aspx?demo=no',null,'Financial Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report For Demo School','orderreportall.aspx?demo=yes',null,'Financial Report']";

                    used = 1;
                }
                script += " ],";
            }
            else if (ConfigClass.UserType < 3)
            {
                ////////if (school.TuckshopModule)
                ////////{
                ////////    script += "_cmSplit,"
                ////////           + "[null,'Tuckshop Reports',null,null,'Tuckshop Reports',";
                ////////}
                ////////else //if (school.EStoreModule)
                ////////{
                ////////    script += "_cmSplit,"
                ////////          + "[null,'eStore Reports',null,null,'eStore Reports',";
                ////////}
                script += "_cmSplit,"
                       + "[null,'"+ConfigClass.Heading+" Reports',null,null,'"+ConfigClass.Heading+" Reports',";
                int used = 0;
                if (pageAccess.IndexOf("treportall") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report All','transactionreportall.aspx',null,'Transaction Report All']";
                    used = 1;
                }

                if (pageAccess.Contains("pactivityreport"))
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Activity Report','productactivityreport.aspx',null,'Product Activity Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Stock Report','stockreport.aspx',null,'Stock Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Production Report','estoreproductionreport.aspx',null,'eStore Production Report']";
                    
                    if (ConfigClass.UserType == 2)
                    {
                        script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report','ordersreportall.aspx',null,'Financial Report']";
                        script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Order Report','viewproductordersreport.aspx',null,'Product Order Report']";
                    //    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreportcordi.aspx',null,'Paypal Fees Absorbed']";
                    //    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreportcordina.aspx',null,'All Fees Not Absorbed']";
                    }
                    used = 1;
                }
                script += " ],";
            }

        

            if (schoolid > 0 && ConfigClass.UserType == 2)
            {
                      script += "_cmSplit,"
                             + "[null,'Financial Reports',null,null,'Financial Reports'";

                      script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreportcordi.aspx',null,'Paypal Fees Absorbed']";
                      script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreportcordina.aspx',null,'All Fees Not Absorbed']";
                   
                     // script += " ],";
                      //demo
                      script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {
                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";

                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Report','productreports.aspx',null,'Product Report']";
                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Report','estorereports.aspx',null,'eStore Report']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','eventreports.aspx',null,'Event Report']";
                 
                script += " ],";
                //demo
                script += "_cmSplit,"
                       + "[null,'Manuals',null,null,'Manuals'";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Demo and Training Material','demoTrainingMaterialfiles.aspx',null,'Manage Demo and Training Material']";
                script += " ],";
            }
            else
            {

                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";

                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Report','productreports.aspx',null,'Product Report']";

                //if (school.TuckshopModule)
                //    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Tuckshop Report','estorereports.aspx',null,'Tuckshop Report']";
                //else if (school.EStoreModule)
                //    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Report','estorereports.aspx',null,'eStore Report']";
                if (school.EventModule)
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','eventreports.aspx',null,'Event Report']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Parent Login History','customerloginHistory.aspx',null,'Parent Login History']";
              
                script += " ],";
            }

            if (school.EventModule)
            {
                if (schoolid > 0 && (pageAccess.IndexOf("eventreport") > -1 || pageAccess.IndexOf("patronreport2") > -1 || pageAccess.IndexOf("accesscontrol") > -1 || pageAccess.IndexOf("eventstatus") > -1 || pageAccess.IndexOf("attreport") > -1))
                {
                    script += "_cmSplit,"
                           + "[null,'Event Reports',null,null,'Event Reports',";
                    int used = 0;

                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Paid Events', null, null, 'Paid Events',";
                 //+ "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                   // script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";
                   


                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paid Event','reportevent.aspx',null,'Event Report']";
                    if (pageAccess.IndexOf("eventreport") > -1)
                    {

                              script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','reportevent.aspx?etype=p',null,'Event Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport1") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 1','patronreport1.aspx?etype=p',null,'Patron Report 1']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport2") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx?etype=p',null,'Patron Report 2']";
                        script += ",";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 3','patronreport3.aspx?etype=p',null,'Patron Report 3']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("accesscontrol") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Access Control Download Report','reportaccesscontrol.aspx?etype=p',null,'Access Control Download Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("eventstatus") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Status Report','eventstatusreport.aspx?etype=p',null,'Event Status Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("performancereport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Performance Report','performancereport.aspx?etype=p',null,'Performance Report']";
                        used = 1;
                    }

                    if (pageAccess.IndexOf("transactionreport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report','transactionreport.aspx?etype=p',null,'Transaction Report']";
                        used = 1;
                    }

                    if (pageAccess.IndexOf("paymentreport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Payment Report','paymentreport.aspx?etype=p',null,'Payment Report']";
                        used = 1;
                    }
                           script += "],";


                           script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Basic Events', null, null, 'Paid Events',";
                           //+ "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                           // script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";



                           //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paid Event','reportevent.aspx',null,'Event Report']";
                           if (pageAccess.IndexOf("eventreport") > -1)
                           {

                                     script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','reportevent.aspx?etype=b',null,'Event Report']";
                                     used = 1;
                           }
                           if (pageAccess.IndexOf("patronreport1") > -1)
                           {
                                     if (used == 1)
                                     {
                                               script += ",";
                                     }
                                     script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 1','patronreport1.aspx?etype=b',null,'Patron Report 1']";
                                     used = 1;
                           }
                           if (pageAccess.IndexOf("patronreport2") > -1)
                           {
                                     if (used == 1)
                                     {
                                               script += ",";
                                     }
                                     script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx?etype=b',null,'Patron Report 2']";
                                     script += ",";
                                     script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 3','patronreport3.aspx?etype=b',null,'Patron Report 3']";
                 
                                     used = 1;
                           }
                           if (pageAccess.IndexOf("accesscontrol") > -1)
                           {
                                     if (used == 1)
                                     {
                                               script += ",";
                                     }
                                     script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Access Control Download Report','reportaccesscontrol.aspx?etype=b',null,'Access Control Download Report']";
                                     used = 1;
                           }
                           if (pageAccess.IndexOf("eventstatus") > -1)
                           {
                                     if (used == 1)
                                     {
                                               script += ",";
                                     }
                                     script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Status Report','eventstatusreport.aspx?etype=b',null,'Event Status Report']";
                                     used = 1;
                           }
                           if (pageAccess.IndexOf("performancereport") > -1)
                           {
                                     if (used == 1)
                                     {
                                               script += ",";
                                     }
                                     script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Performance Report','performancereport.aspx?etype=b',null,'Performance Report']";
                                     used = 1;
                           }

                           if (pageAccess.IndexOf("transactionreport") > -1)
                           {
                                     if (used == 1)
                                     {
                                               script += ",";
                                     }
                                     script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report','transactionreport.aspx?etype=b',null,'Transaction Report']";
                                     used = 1;
                           }

                           //if (pageAccess.IndexOf("paymentreport") > -1)
                           //{
                           //          if (used == 1)
                           //          {
                           //                    script += ",";
                           //          }
                           //          script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Payment Report','paymentreport.aspx?etype=b',null,'Payment Report']";
                           //          used = 1;
                           //}
                           script += "],";

                          
                          script += " ],";
                }
            }
            if (school.FacilityModule)
            {
              //if (schoolid > 0 && (pageAccess.IndexOf("eventreport")>-1||pageAccess.IndexOf("patronreport2") > -1)
                if (schoolid > 0 )
                {
                    //Manage Resource
                    script += "_cmSplit,[null,'Manage Facilities',null,null,'Manage Facilities',";
                    //Facilities
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facilities', null, null, 'Manage Facilities',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faFacilityListing.aspx',null,'Manage Facilities']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faFacilityaddedit.aspx',null,'Add/Edit Facilities']";
                    script += "],";
                    //Facilities booking
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage booked Facilities', null, null, 'Facilities Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faBookedFacility.aspx',null,'Booked Facilities']";
                    script += "],";
                    //resource 
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Resource', null, null, 'Resource Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faResourceListing.aspx',null,'Resource Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faResourceaddedit.aspx',null,'Add/Edit Resource']";
                    script += "],";
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facility Category', null, null, 'Manage Facility Category',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faFacilityCategoryListing.aspx',null,'Manage Facility Category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faFacilityCategoryaddedit.aspx',null,'Add/Edit Facilities Category']";
                    script += "],";
                    //locaton
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Location', null, null, 'Location Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faFacilityLocationListing.aspx',null,'Location Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faFacilityLocationaddedit.aspx',null,'Add/Edit Location']";
                    script += "],";
                    //sub location
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Sub Location', null, null, 'Sub Location Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faSubLocationListing.aspx',null,'Sub Location Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faSubLocationaddedit.aspx',null,'Add/Edit Sub Location']";
                    script += "],";
                    
                  
                    //Facilities parameter
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facilities Parameter', null, null, 'Manage Facilities Parameter',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facilities Cut Off Time                                                     ','faCutofftime.aspx',null,'Manage Facilities Cut Off Time']"
                       + ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Cancel Fee','faFacilityManageCancelFee.aspx',null,'Manage Cancel Fee']";
                    script += "],";


                  //  script += " ],";

                    //----------manage resource
                   // script += "_cmSplit,[null,'Manage Resource',null,null,'Manage Resource',";
                    //resource Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Resource Category', null, null, 'Resource Category Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faResourceCategoryListing.aspx',null,'Resource Category Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faResourceCategoryaddedit.aspx',null,'Add/Edit Resource Categories']";
                    script += "],";
                    


                    script += " ],";

                }
            }
            #region Parent Teacher module
            ////////if (school.ParentTeacherModule)
            ////////{
            ////////          //if (schoolid > 0 && (pageAccess.IndexOf("eventreport")>-1||pageAccess.IndexOf("patronreport2") > -1)
            ////////          if (schoolid > 0)
            ////////          {
            ////////                    //Manage Resource
            ////////                    script += "_cmSplit,[null,'Manage Parent-Teacher',null,null,'Manage Parent-Teacher',";
            ////////                    //Category
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Parent-Teacher Category Listing', null, null, 'Parent-Teacher Category Listing',"
            ////////                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','ptmCategoryListing.aspx',null,'Parent-Teacher Category Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','ptmCategoryaddedit.aspx',null,'Add/Edit Parent-Teacher Categories']";
            ////////                    script += "],";
            ////////                    //locaton
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Location Listing', null, null, 'Location Listing',"
            ////////                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faFacilityLocationListing.aspx',null,'Location Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faFacilityLocationaddedit.aspx',null,'Add/Edit Location']";
            ////////                    script += "],";
            ////////                    //sub location
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Sub Location Listing', null, null, 'Sub Location Listing',"
            ////////                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faSubLocationListing.aspx',null,'Sub Location Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faSubLocationaddedit.aspx',null,'Add/Edit Sub Location']";
            ////////                    script += "],";
            ////////                    //Facility
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Facility Listing', null, null, 'Facility Listing',"
            ////////                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faFacilityListing.aspx',null,'Facility Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faFacilityaddedit.aspx',null,'Add/Edit Facility']";
            ////////                    script += "],";
            ////////                    //Facility booking
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage booked Facility', null, null, 'Facility Listing',"
            ////////                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faBookedFacility.aspx',null,'Booked Facility']";
            ////////                    script += "],";
            ////////                    //Facility parameter
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facility Parameter', null, null, 'Manage Facility Parameter',"
            ////////                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facility Cut Off Time                                                     ','faCutofftime.aspx',null,'Manage Facility Cut Off Time']"
            ////////                       + ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Cancel Fee','faFacilityManageCancelFee.aspx',null,'Manage Cancel Fee']";
            ////////                    script += "],";


            ////////                    script += " ],";

            ////////                    //----------manage resource
            ////////                    script += "_cmSplit,[null,'Manage Resource',null,null,'Manage Resource',";
            ////////                    //resource Category
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Resource Category Listing', null, null, 'Resource Category Listing',"
            ////////                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faResourceCategoryListing.aspx',null,'Resource Category Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faResourceCategoryaddedit.aspx',null,'Add/Edit Resource Categories']";
            ////////                    script += "],";
            ////////                    //resource 
            ////////                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Resource Listing', null, null, 'Resource Listing',"
            ////////                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faResourceListing.aspx',null,'Resource Listing']";
            ////////                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','faResourceaddedit.aspx',null,'Add/Edit Resource']";
            ////////                    script += "],";


            ////////                    script += " ],";

            ////////          }
            ////////}
            #endregion

            script += "];"
                          + "cmDraw ('myMenuID', myMenu, 'hbr', cmThemeOffice, 'ThemeOffice');";

            return "<script type=\"text/javascript\">" + script + "</script>";

        }

        private string WriteScriptMenu1(string pageAccess)
        {
            int schoolid = 0;
            int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["SchoolID"] == null)
                return string.Empty;
            else
                schoolid = Convert.ToInt32(HttpContext.Current.Session["SchoolID"]);
            Schools school = new Schools();
            school.SchoolId = schoolid;
            school.GetSchoolDetail();
            if (userType <= 1)
            {
                //Super admin has access to on/off module
                pageAccess += ",module";
            }
            //Javascript Menu
            pageAccess = pageAccess.ToLower();
            string script = "";
            if (schoolid == 0)
            {
                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],
			                    _cmSplit,
			                    [null,'Site',null,null,'Site Management',"
                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','../../index.aspx','_blank','Website Homepage'],"
                                 + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','../../index.aspx',null,'Inline View of Website Homepage'],"
                                 + "],"
                                 + "],";
            }
            else
            {
                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],
			                    _cmSplit,
			                    [null,'Site',null,null,'Site Management',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','/" + school.SubDomain + "/index.aspx','_blank','Website Homepage'],"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','/" + school.SubDomain + "/index.aspx',null,'Inline View of Website Homepage'],"
                                + "],"
                                + "],";
            }
            //----------------------------- ----------USERS-----------------------------------------------
            if (userType != 5 && userType != 4)//5 : agent   4 : admin personnel
            {
                script += "[null,'Manage',null,null,'Module Management',";
                if (schoolid > 0 && pageAccess.IndexOf("parents") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listparents.aspx',null,'Manage Parents']";
                    //if (schoolid == 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    //script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditparent.aspx',null,'Add/Edit Parents']";
                    script += "],";
                }
                else
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    script += "],";
                }
                if (pageAccess.IndexOf("users") > -1)
                {
                    if (schoolid > 0)
                    {
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listusers.aspx', null, 'Manage Users'],";
                    }
                    else
                    {
                        if (Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                        {
                            script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listadminusers.aspx', null, 'Manage Users'],";
                        }
                    }


                }

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries'],";
                }

                if (pageAccess.IndexOf("dynamic") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Dynamic Content', 'listdynamicpages.aspx', null, 'View Dynamic Content'],";
                }
                if (schoolid == 0)
                {
                    if (pageAccess.IndexOf("admindynamix") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Admin Dynamic Content', 'listadmindynamicpages.aspx', null, 'View Admin Dynamic Content'],";
                    }
                }
                if (userType > 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ'],";
                }
                if (userType <= 4 && pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }
                if (userType <= 2 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Class', 'listclasses.aspx', null, 'Manage Class',";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage CheckOut Content', 'addeditcheckoutcontent.aspx', null, 'Manage CheckOut Content',";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Enroll Comment', 'addeditenrollcomment.aspx', null, 'Manage Enroll Comment',";
                    script += "],";
                }
                if (userType <= 1)
                {//super admin
                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', 'schools.aspx', null, 'Manage Schools'],";
                    if (schoolid == 0 && pageAccess.IndexOf("schools") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Order Label', 'addeditcheckoutlabels.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Event Order Status', null, null, 'Manage Event Order Status',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','eventlistorderstatus.aspx',null,'Manage Event Order Status']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','eventorderstatusaddedit.aspx',null,'Add Event Order Status']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', null, null, 'Manage Schools',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','schools.aspx',null,'Manage Schools']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditschool.aspx',null,'Add School']";
                        script += "],";
                    }

                    if (schoolid > 0 && pageAccess.IndexOf("library") > -1)
                    {                        
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Library', null, null, 'Manage Library',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listlibrary.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditlibrary.aspx',null,'Add/Edit Library']";
                        script += "],";
                    }
                    //if (pageAccess.IndexOf("paidfreeevents") > -1)
                    //{
                    //    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Paid Free Events', 'managepaidfreeevents.aspx', null, 'Manage Paid Free Events',";
                    //    script += "],";
                    //}
                }
               

                script += " ],";

            }

            if (userType == 4)
            {//admin personnel
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',"
                         + "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', null, null, 'Manage Users',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listusers.aspx',null,'Manage Users']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                script += "],";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries',";
                    script += "],";
                }

                if (pageAccess.IndexOf("dynamic") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Dynamic Content', 'listdynamicpages.aspx', null, 'Manage Dynamic Content',";
                    script += "],";
                }

                if (pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }

                script += " ],";
            }
            if (userType == 5)
            {//agent
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                script += " ],";
            }
            //----------------------------Super Admin Setting-------------------------------------------
            if (schoolid == 0 || pageAccess.IndexOf("faqlisting") > -1 || pageAccess.IndexOf("dashboardcontent") > -1 || pageAccess.IndexOf("loginscreen") > -1 || pageAccess.IndexOf("stickets") > -1)
            {
                script += "_cmSplit,[null,'Manage Settings',null,null,'Manage Settings',";
                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Parameters','parameterlist.aspx',null,'Manage Parameters']";
                }
                if (pageAccess.IndexOf("dashboardcontent") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Dashboard Content','dashboadmanage.aspx',null,'Manage Dashboard Content']";
                }
                if (schoolid == 0 && pageAccess.IndexOf("faqlisting") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage FAQ','listfaq.aspx',null,'Manage FAQ']";
                }
                if (pageAccess.IndexOf("loginscreen") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Login Screen Content','addscreenicon.aspx',null,'Manage Login Screen Content']";
                }
                if (pageAccess.IndexOf("stickets") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Support Ticket','supportticketlisting.aspx',null,'Manage Support Ticket']";
                }
                script += " ],";
            }
            //----------------------------PRODUCT CATEGORIES-------------------------------------------
            if (school.EStoreModule && pageAccess.IndexOf("pcategory") > -1 && schoolid > 0)
            {
                script += "_cmSplit,[null,'eStore Categories',null,null,'eStore Categories',";
                //Category
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Categories', null, null, 'Manage Product Categories',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productcategorieslist.aspx',null,'Manage Product Categories']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productcategoryaddedit.aspx',null,'Add/Edit Product Categories']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Sub Categories', null, null, 'Manage Product Sub Categories',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productsubcategorylist.aspx',null,'Manage Product Sub Categories']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productsubcategoryaddedit.aspx',null,'Add/Edit Product Sub Categories']";
                script += "], ],";
            }
            //-----------------------------PRODUCTS----------------------------------------------
            if (school.EStoreModule && schoolid > 0 && (pageAccess.IndexOf("product") > -1 || pageAccess.IndexOf("pcategory") > -1 || pageAccess.IndexOf("porder") > -1))
            {
                script += "_cmSplit,[null,'eStore',null,null,'eStore',";
                if (pageAccess.IndexOf("product") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products', null, null, 'Manage Products',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlist.aspx',null,'Manage Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddedit.aspx',null,'Add/Edit Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Import/Export Products','productimport.aspx',null,'Import/Export Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Import/Export Product Attributes','productattributeimport.aspx',null,'Import/Export Product Attributes']";
                    script += "],";

                    //Product Bundle
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Bundles', null, null, 'Manage Product Bundles',"
                        + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productbundlelist.aspx',null,'Manage Product Bundles']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productbundleaddedit.aspx',null,'Add/Edit Product Bundles']";
                    script += "],";
                }

                if (schoolid > 0 && pageAccess.IndexOf("porder") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                    script += "],";
                    //script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Bundle Orders', null, null, 'Manage Products Bundle Orders',";
                    //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductbundleorders.aspx',null,'View Orders']";
                    //script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Order Status', null, null, 'Manage Order Status',";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listorderstatus.aspx',null,'Manage Order Status']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/Edit Order Status','orderstatusaddedit.aspx',null,'Add/Edit Order Status']";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Order Financial Status', null, null, 'Manage Order Financial Status',";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','orderfinalcialstatuslist.aspx',null,'Manage Order Financial Status']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/Edit Order Financial Status','orderfinalcialstatus.aspx',null,'Add/Edit Order Financial Status']";
                    script += "],";
                }
                script += " ],";
            }
            //-----------------------------Gallery----------------------------------------------
            if (pageAccess.IndexOf("gallery") > -1 || pageAccess.IndexOf("gcategory") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Gallery',null,null,'Manage Gallery',";

                if (pageAccess.IndexOf("gcategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Categories', null, null, 'Manage Gallery Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgallerycategories.aspx',null,'Manage Gallery Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgallerycategory.aspx',null,'Add/Edit Gallery Categories']";
                    script += "],";
                }

                if (pageAccess.IndexOf("gallery") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Photos', null, null, 'Manage Gallery Photos',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgalleryphotos.aspx',null,'Manage Gallery Photos']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgalleryphotos.aspx',null,'Add/Edit Gallery Photos']";
                    script += "],";
                }
                script += " ],";

            }
            //-----------------------------Events----------------------------------------------
            if (school.EventModule && schoolid > 0 && (pageAccess.IndexOf("events") > -1 || pageAccess.IndexOf("eorder") > -1 || pageAccess.IndexOf("surveylist") > -1))
            {

                script += "_cmSplit,"
                       + "[null,'Events',null,null,'Events',";

                if (pageAccess.IndexOf("events") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Paid Events', null, null, 'Manage Paid Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";
                    script += "],";
                }
                if (pageAccess.IndexOf("eventbasic") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Basic Events', null, null, 'Manage Basic Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventsbasic.aspx',null,'Manage Basic Events']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventbasic.aspx',null,'Add/Edit Basic Events']";
                    script += "],";
                }
                if (pageAccess.IndexOf("eorder") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Orders', null, null, 'Manage Event Orders',"
                             + ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','View Order','vieweventorders.aspx',null,'View Order']";
                    script += "],";
                }
                //if (pageAccess.IndexOf("surveylist") > -1)
                //{
                //    //Product
                //    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Survey', null, null, 'Manage Survey',"
                //           + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','#listsurvey.aspx',null,'Manage Survey']";
                //    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','#addeditsurvey.aspx',null,'Add Survey']";
                //    script += "],";
                //}
                if (pageAccess.IndexOf("ecategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event category', null, null, 'Manage Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventcategories.aspx',null,'Manage Event category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventcategory.aspx',null,'Add/Edit Event category']";
                    script += "],";
                }
                if (pageAccess.IndexOf("elocation") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Location category', null, null, 'Manage Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventlocation.aspx',null,'Manage Location category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventlocation.aspx',null,'Add/Edit Location category']";
                    script += "],";
                }

                script += " ],";

            }

            //------------------------------------DYNAMIC CONTENT------------------------
            ////if (PageAccess.IndexOf("dynamic") > -1)
            ////{
            ////    script += "_cmSplit,"
            ////           + "[null,'Manage Dynamic Content',null,null,'Manage Dynamic Content',";
            ////    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listdynamicpages.aspx',null,'View Dynamic Pages']"
            ////            + "],";
            ////}

            //------------------------------------Contact Us list------------------------
            ////if (PageAccess.IndexOf("contact") > -1)
            ////{
            ////    script += "_cmSplit,"
            ////           + "[null,'Manage Contact Us',null,null,'Manage Contact Us',";
            ////    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Contact Us Queries','contactuslisting.aspx',null,'View Contact Us Queries']"
            ////           + "],";
            ////}

            //------------------------------------NEWS------------------------
            if (schoolid > 0 && pageAccess.IndexOf("news") > -1)
            {
                script += "[null, 'News', null, null, 'News',"
                    + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View News','listnews.aspx',null,'Manage News']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add News','addeditnews.aspx',null,'Add News']";
                script += "],";
            }
            //------------------------------------Newsletter Subscribers------------------------
            if (pageAccess.IndexOf("subscribers") > -1)
            {
                script += "_cmSplit,"
                        + "[null,'Newsletter Subscribers',null,null,'Newsletter Subscribers',";
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Subscribers','listcustomers.aspx',null,'View Subscribers']"
                        + "],";
            }
            //------------------------------------Registerred Members------------------------
            if (pageAccess.IndexOf("customers") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Members',null,null,'Manage Members',";
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Registered Users','listmembers.aspx',null,'View Registered Users']"
                       + "],";
            }

            if (userType == 4)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Report',null,null,'Manage Report',";
                int used = 0;
                if (used == 1)
                {
                    script += ",";
                }
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx',null,'Patron Report 2']";
                used = 1;
                script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {
                //script += "_cmSplit,"
                //       + "[null,'eStore Reports',null,null,'eStore Reports',";
                int used = 0;
                if (pageAccess.IndexOf("producttransactionreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Transaction Report','producttransactionreport.aspx',null,'Product Transaction Report']";
                    used = 1;
                }
                if (ConfigClass.UserType == 1 && pageAccess.IndexOf("orderreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Absorbed','orderreport.aspx?absorb=11',null,'All Fees Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreport.aspx?absorb=00',null,'All Fees Not Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Surcharge Absorbed','orderreport.aspx?absorb=10',null,'Surcharge Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreport.aspx?absorb=01',null,'Paypal Fees Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report','orderreportall.aspx',null,'Financial Report']";

                    used = 1;
                }
                script += " ],";
            }
            else if (ConfigClass.UserType < 3)
            {
                //script += "_cmSplit,"
                //       + "[null,'eStore Reports',null,null,'eStore Reports',";
                int used = 0;
                if (pageAccess.IndexOf("treportall") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report All','transactionreportall.aspx',null,'Transaction Report All']";
                    used = 1;
                }

                if (pageAccess.Contains("pactivityreport"))
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Activity Report','productactivityreport.aspx',null,'Product Activity Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Stock Report','stockreport.aspx',null,'Stock Report']";
                    if (ConfigClass.UserType == 2)
                    {
                        script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report','ordersreportall.aspx',null,'Financial Report']";
                        script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Order Report','viewproductordersreport.aspx',null,'Product Order Report']";
                    }
                    used = 1;
                }
                script += " ],";
            }

            //if (schoolid == 0 && ConfigClass.UserType == 1)
            {
                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";

                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Report','productreports.aspx',null,'Product Report']";
                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Report','estorereports.aspx',null,'eStore Report']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','eventreports.aspx',null,'Event Report']";
                script += " ],";
            }

            if (schoolid > 0 && (pageAccess.IndexOf("eventreport") > -1 || pageAccess.IndexOf("patronreport2") > -1 || pageAccess.IndexOf("accesscontrol") > -1 || pageAccess.IndexOf("eventstatus") > -1 || pageAccess.IndexOf("attreport") > -1))
            {
                script += "_cmSplit,"
                       + "[null,'Event Reports',null,null,'Event Reports',";
                int used = 0;
                if (pageAccess.IndexOf("eventreport") > -1)
                {

                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','reportevent.aspx',null,'Event Report']";
                    used = 1;
                }
                if (pageAccess.IndexOf("patronreport1") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 1','patronreport1.aspx',null,'Patron Report 1']";
                    used = 1;
                }
                if (pageAccess.IndexOf("patronreport2") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx',null,'Patron Report 2']";
                    used = 1;
                }
                if (pageAccess.IndexOf("accesscontrol") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Access Control Download Report','reportaccesscontrol.aspx',null,'Access Control Download Report']";
                    used = 1;
                }
                if (pageAccess.IndexOf("eventstatus") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Status Report','eventstatusreport.aspx',null,'Event Status Report']";
                    used = 1;
                }
                if (pageAccess.IndexOf("performancereport") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Performance Report','performancereport.aspx',null,'Performance Report']";
                    used = 1;
                }

                if (pageAccess.IndexOf("transactionreport") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report','transactionreport.aspx',null,'Transaction Report']";
                    used = 1;
                }

                if (pageAccess.IndexOf("paymentreport") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Payment Report','paymentreport.aspx',null,'Payment Report']";
                    used = 1;
                }

                script += " ],";
            }

            script += "];"
                          + "cmDraw ('myMenuID', myMenu, 'hbr', cmThemeOffice, 'ThemeOffice');";

            return "<script type=\"text/javascript\">" + script + "</script>";

        }
        #endregion
        #endregion


        #region new desktop

        public void CheckPageAccess(Literal litAdminMenu,string desktopType)
        {
            int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
            //if the user is super admin he has access to all the pages
            if (Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]) >= 0)
            {
                string strName = string.Empty;

                GetCommaSeparatedActiveModules();

                //default access to desktop for all users
                if (PageAccess != string.Empty &&
                    (HttpContext.Current.Request.RawUrl.ToLower().IndexOf("unauthorized") < 0 ||
                    HttpContext.Current.Request.RawUrl.ToLower().IndexOf("desktop") < 0))
                {

                    //get the Page Keyword
                    strName = GetPageKeyword("pagename.xml");

                    //Check the user has access to current page request
                    //Match the Keyword with the Access Keywords
                    if (strName.Length > 0 && this.PageAccess.ToUpper().IndexOf(strName) < 0)
                        ConfigClass.Redirect("unauthorized.aspx");
                    else
                    {
                        string temp = string.Empty;
                        if (userType < 3)
                        {
                            //super admin/School Administrator
                            //for super admin only
                            if (userType <= 1)//super admin
                                PageAccess += ",module,eventreport,patronreport1,patronreport2,accesscontrol,eventstatus,performancereport,transactionreport,paymentreport,instructions,schools,product,pcategory,porder,eorder,admindynamix,ecategory,events,eventbasic,elocation,treportall,library,pactivityreport,producttransactionreport,orderreport,productreport";
                            else if (userType <= 2)//super admin,SCHOOL ADMIN
                                PageAccess += ",module,patronreport1,patronreport2,eventreport,performancereport,transactionreport,paymentreport,instructions,product,pcategory,porder,parents,events,eventbasic,elocation,eorder,ecategory,treportall,library,pactivityreport,producttransactionreport,orderreport";
                            else if (userType > 2)
                                PageAccess += ",module,patronreport1,patronreport2,eventreport,performancereport,transactionreport,paymentreport,instructions,eventbasic,eorder";
                        }
                        else if (userType == 3)//If User is Event Admin
                        {
                            temp = string.Empty;
                            if (PageAccess.ToLower().Contains("users"))
                                temp = "users";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("events"))
                                temp += "events";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("eventbasic"))
                                temp += "eventbasic";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("eorder"))
                                temp += "eorder";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("stickets"))
                                temp += "stickets";
                            if (temp.Length > 0)
                                temp += ",";
                            temp += "events,eventbasic,eventreport,patronreport1,patronreport2,performancereport,transactionreport,paymentreport,eorder";


                            PageAccess = temp;
                        }
                        else if (userType == 5)//If User is Agent
                        {
                            temp = string.Empty;

                            if (PageAccess.ToLower().Contains("eorder"))
                                temp += "eorder";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("stickets"))
                                temp += "stickets";

                            PageAccess = temp;
                        }
                        else if (userType == 4)//If User is Admin Personnel
                        {
                            temp = string.Empty;
                            if (PageAccess.Contains("eorder"))
                                temp += "eorder";
                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("stickets"))
                                temp += "stickets";

                            if (temp.Length > 0)
                                temp += ",";
                            if (PageAccess.ToLower().Contains("stickets"))
                                temp += "";

                            if (temp.Length > 0)
                                temp += ",";
                            temp += "eventbasic";


                            PageAccess = temp;
                        }
                        //else if (userType == 0 || userType == 1)//If User is super admin
                        //{
                        //    PageAccess += "dashboardcontent,faqlisting,loginscreen";
                        //}
                        if (userType != 5 && userType != 4)
                        {
                            PageAccess += "surveylist,";
                        }
                        //Common for all user types
                        PageAccess += "sitehome,logout";

                        //Write JS Menu 
                        if (desktopType.ToUpper() == "ESTORE")
                            litAdminMenu.Text = WriteScriptMenuESTORE(PageAccess);
                        else if (desktopType.ToUpper() == "EVENT")
                            litAdminMenu.Text = WriteScriptMenuEVENT(PageAccess);
                        else if (desktopType.ToUpper() == "FACILITY")
                            litAdminMenu.Text = WriteScriptMenuFACILITY(PageAccess);
                        else
                            litAdminMenu.Text = WriteScriptMenu(PageAccess);
                    }
                }
                ////else
                ////    HttpContext.Current.ConfigClass.Redirect("unauthorized.aspx");

            }
            else
            {
                //JS Menu
                litAdminMenu.Text = "<script type=\"text/javascript\" src=\"includes/script/menu.js\"></script>";
            }

        }


        private string WriteScriptMenuESTORE(string pageAccess)
        {
            int schoolid = 0;
            int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["SchoolID"] == null)
                return string.Empty;
            else
                schoolid = Convert.ToInt32(HttpContext.Current.Session["SchoolID"]);
            Schools school = new Schools();
            school.SchoolId = schoolid;
            school.GetSchoolDetail();
            if (userType <= 1)
            {
                //Super admin has access to on/off module
                pageAccess += ",module";
            }
            //Javascript Menu
            pageAccess = pageAccess.ToLower();
            string script = "";
            if (schoolid == 0)
            {
                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],";
                                //_cmSplit,
                                //[null,'Site',null,null,'Site Management',"
                                // + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                // + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','../../index.aspx','_blank','Website Homepage'],"
                                // + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','../../index.aspx',null,'Inline View of Website Homepage'],"
                                // + "],"
                                // + "],";
            }
            else
            {
                string rehdetails = "";
                string Reg_Name = "";
                string Reg_uniqueid = "";
                posUserClass objUsers = new posUserClass();
                objUsers.SchoolID = schoolid;
                DataSet ds = new DataSet();
                ds = objUsers.PosRegisterforPreview();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Reg_Name = "";
                        Reg_uniqueid = "";
                        Reg_Name = Convert.ToString(ds.Tables[0].Rows[i]["REG_NAME"]);
                        Reg_uniqueid = Convert.ToString(ds.Tables[0].Rows[i]["REG_UNIQUE_VAL"]);
                        if (Reg_Name.Length > 0 && Reg_uniqueid.Length > 0)
                        {
                            if (rehdetails.Length == 0)
                                rehdetails = "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview POS Front', null, null, 'Preview',";
                            rehdetails = rehdetails + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','" + Reg_Name + "','" + ConfigClass.POSSite + "Login/LogIn/" + school.SubDomain + "/" + Reg_uniqueid + "','_blank','Website Homepage'],";
                        }
                    }
                    if (rehdetails.Length > 0)
                    {
                        rehdetails = rehdetails + "],";
                    }
                }

                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],
			                    _cmSplit,
			                    [null,'Site',null,null,'Site Management',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','/" + school.SubDomain + "/index.aspx','_blank','Website Homepage'],"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','/" + school.SubDomain + "/index.aspx',null,'Inline View of Website Homepage'],"
                                + "],"
                                 + rehdetails
                                + "],";
            }
            //----------------------------- ----------USERS-----------------------------------------------
            if (userType != 5 && userType != 4)//5 : agent   4 : admin personnel
            {
                script += "[null,'Manage',null,null,'Module Management',";
                if (schoolid > 0 && pageAccess.IndexOf("parents") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listparents.aspx',null,'Manage Parents']";
                    //if (schoolid == 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                else
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                if (userType == 1 && schoolid < 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Parents Homepage Alert', 'homepagealertfront.aspx', null, 'FAQ'],";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Coordinators Alert', 'homepagealerttocordi.aspx', null, 'FAQ'],";
                }

                if (pageAccess.IndexOf("users") > -1)
                {
                    if (schoolid > 0)
                    {
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listusers.aspx', null, 'Manage Users'],";

                        //----Register start
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Register', null, null, 'Manage Library',"
                                + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','registerassigalist.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditRegister.aspx',null,'Add/Edit Library']";
                        if (userType <= 1)
                            script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/View Register to Schools','registermaster.aspx',null,'Add/Edit Library']";
                        script += "],";
                        // Register End
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Pos Users', 'listPosUsers.aspx', null, 'Manage Pos Users'],";


                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Membership Type', null, null, 'Manage Membership Type',"
                               + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listMemtype.aspx',null,'Manage Membership Type']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditMemtype.aspx',null,'Add/Edit Membership Type']";
                        script += "],";


                        //----Product Discount start
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Discount', null, null, 'Manage Discount',"
                                + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listDiscount.aspx',null,'Manage Discount']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','adddiscount.aspx',null,'Add/Edit Discount']";
                        script += "],";
                        // Product Discount  End

                    }
                    else
                    {
                        // if (Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                        if (userType == 1)
                        {
                            script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listadminusers.aspx', null, 'Manage Users'],";
                        }
                    }
                }
                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries'],";
                }

                if (schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Delivery Option Appearance', 'manageDeliveryOPTApperance.aspx', null, 'Manage Delivery Option Appearance'],";
                }


                if (pageAccess.IndexOf("dynamic") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Dynamic Content', 'listdynamicpages.aspx', null, 'View Dynamic Content'],";
                }
                if (schoolid == 0)
                {
                    if (pageAccess.IndexOf("admindynamix") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Admin Dynamic Content', 'listadmindynamicpages.aspx', null, 'View Admin Dynamic Content'],";
                    }
                }
                if (userType > 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ'],";
                }
                if (userType <= 4 && pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }
                if (userType <= 2 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Class', 'listclasses.aspx', null, 'Manage Class',";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage CheckOut Content', 'addeditcheckoutcontent.aspx', null, 'Manage CheckOut Content',";
                    script += "],";
                    if (!school.TuckshopModule)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Enroll Comment', 'addeditenrollcomment.aspx', null, 'Manage Enroll Comment',";
                        script += "],";
                    }
                }
                if (userType <= 1)
                {//super admin
                    if (schoolid == 0 && pageAccess.IndexOf("schools") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Order Label', 'addeditcheckoutlabels.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Event Order Status', null, null, 'Manage Event Order Status',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','eventlistorderstatus.aspx',null,'Manage Event Order Status']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','eventorderstatusaddedit.aspx',null,'Add Event Order Status']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', null, null, 'Manage Schools',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','schools.aspx',null,'Manage Schools']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditschool.aspx',null,'Add School']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage e-Merchant', null, null, 'Manage e-Merchant',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listtranstype.aspx',null,'Manage e-Merchant']";
                        script += "],";

                        //---email template
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Email Template', null, null, 'Manage Email Template',"
                           + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','emailtemplatelisting.aspx',null,'Manage Email Template']";
                        script += "],";
                        //---custom message
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Custom Messages', null, null, 'Manage Custom Messages',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','custommsglist.aspx',null,'Manage Custom Messages']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Edit Financial History Notes', 'editsuperadminmessages.aspx', null, 'Manage Checkout Label',";
                        script += "],";

                    }

                    if (schoolid > 0 && pageAccess.IndexOf("library") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Library', null, null, 'Manage Library',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listlibrary.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditlibrary.aspx',null,'Add/Edit Library']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Notes', 'addedittooptip.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("news") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'News', null, null, 'News',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View News','listnews.aspx',null,'Manage News']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add News','addeditnews.aspx',null,'Add News']";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("subscribers") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'View Subscribers', 'listcustomers.aspx', null, 'View Subscribers',";
                        script += "],";


                    }
                }


                script += " ],";

            }

            if (userType == 4)
            {//admin personnel
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',"
                         + "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', null, null, 'Manage Users',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listusers.aspx',null,'Manage Users']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                script += "],";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries',";
                    script += "],";
                }

                if (pageAccess.IndexOf("dynamic") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Dynamic Content', 'listdynamicpages.aspx', null, 'Manage Dynamic Content',";
                    script += "],";
                }

                if (pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }

                script += " ],";
            }
            if (userType == 5)
            {//agent
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                script += " ],";
            }
            //----------------------------Super Admin Setting-------------------------------------------
            if (schoolid == 0 || pageAccess.IndexOf("faqlisting") > -1 || pageAccess.IndexOf("dashboardcontent") > -1 || pageAccess.IndexOf("loginscreen") > -1 || pageAccess.IndexOf("stickets") > -1)
            {
                script += "_cmSplit,[null,'Manage Settings',null,null,'Manage Settings',";
                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facility GST','faFacilityManageGST.aspx',null,'Manage Facility GST']";
                }


                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Parameters','parameterlist.aspx',null,'Manage Parameters']";
                }
                if (pageAccess.IndexOf("dashboardcontent") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Dashboard Content','dashboadmanage.aspx',null,'Manage Dashboard Content']";
                }
                if (schoolid == 0 && pageAccess.IndexOf("faqlisting") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage FAQ','listfaq.aspx',null,'Manage FAQ']";
                }
                if (pageAccess.IndexOf("loginscreen") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Login Screen Content','addscreenicon.aspx',null,'Manage Login Screen Content']";
                }
                if (pageAccess.IndexOf("stickets") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Support Ticket','supportticketlisting.aspx',null,'Manage Support Ticket']";
                }
                if (userType > 1 && schoolid > 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Home Page Alert','homepagealert.aspx',null,'Manage Support Ticket']";
                //script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Home Page Alert', 'homepagealert.aspx', null, 'Manage Home Page Alert'],";


                script += " ],";
            }
            //----------------------------PRODUCT CATEGORIES-------------------------------------------

            if (pageAccess.IndexOf("pcategory") > -1 && schoolid > 0)
            {
                script += "_cmSplit,[null,'eStore Categories',null,null,'eStore Categories',";
                //Category
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Categories', null, null, 'Manage Product Categories',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productcategorieslist.aspx',null,'Manage Product Categories']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productcategoryaddedit.aspx',null,'Add/Edit Product Categories']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Sub Categories', null, null, 'Manage Product Sub Categories',"
                   + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productsubcategorylist.aspx',null,'Manage Product Sub Categories']";
                script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productsubcategoryaddedit.aspx',null,'Add/Edit Product Sub Categories']";
                script += "], ],";
            }
            
            if (schoolid > 0 && (pageAccess.IndexOf("product") > -1 || pageAccess.IndexOf("pcategory") > -1 || pageAccess.IndexOf("porder") > -1))
            {
                script += "_cmSplit,[null,'" + ConfigClass.Heading + "',null,null,'" + ConfigClass.Heading + "',";

                if (pageAccess.IndexOf("product") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products', null, null, 'Manage Products',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlist.aspx',null,'Manage Products']";

                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddedit.aspx',null,'Add/Edit Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Import/Export Products','productimport.aspx',null,'Import/Export Products']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Import/Export Product Attributes','productattributeimport.aspx',null,'Import/Export Product Attributes']";
                    script += "],";

                    
                    //Product Bundle
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Product Bundles', null, null, 'Manage Product Bundles',"
                        + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productbundlelist.aspx',null,'Manage Product Bundles']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productbundleaddedit.aspx',null,'Add/Edit Product Bundles']";
                    script += "],";
                }

                if (schoolid > 0 && pageAccess.IndexOf("porder") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Order Status', null, null, 'Manage Order Status',";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listorderstatus.aspx',null,'Manage Order Status']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/Edit Order Status','orderstatusaddedit.aspx',null,'Add/Edit Order Status']";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Order Financial Status', null, null, 'Manage Order Financial Status',";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','orderfinalcialstatuslist.aspx',null,'Manage Order Financial Status']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/Edit Order Financial Status','orderfinalcialstatus.aspx',null,'Add/Edit Order Financial Status']";
                    script += "],";
                }
                script += " ],";
            }
            //-----------------------------Gallery----------------------------------------------
            if (pageAccess.IndexOf("gallery") > -1 || pageAccess.IndexOf("gcategory") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Gallery',null,null,'Manage Gallery',";

                if (pageAccess.IndexOf("gcategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Categories', null, null, 'Manage Gallery Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgallerycategories.aspx',null,'Manage Gallery Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgallerycategory.aspx',null,'Add/Edit Gallery Categories']";
                    script += "],";
                }

                if (pageAccess.IndexOf("gallery") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Photos', null, null, 'Manage Gallery Photos',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgalleryphotos.aspx',null,'Manage Gallery Photos']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgalleryphotos.aspx',null,'Add/Edit Gallery Photos']";
                    script += "],";
                }
                script += " ],";

            }
            //------------------------------------Registerred Members------------------------
            if (pageAccess.IndexOf("customers") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Members',null,null,'Manage Members',";
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Registered Users','listmembers.aspx',null,'View Registered Users']"
                       + "],";
            }

            if (userType == 4)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Report',null,null,'Manage Report',";
                int used = 0;
                if (used == 1)
                {
                    script += ",";
                }
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx',null,'Patron Report 2']";
                used = 1;
                script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {//for supera admin
                script += "_cmSplit,"
                    + "[null,'" + ConfigClass.Heading + "',null,null,'" + ConfigClass.Heading + "',";
                int used = 0;
                if (pageAccess.IndexOf("producttransactionreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Transaction Report','producttransactionreport.aspx',null,'Product Transaction Report']";
                    used = 1;
                }
                if (ConfigClass.UserType == 1 && pageAccess.IndexOf("orderreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Absorbed','orderreport.aspx?absorb=11',null,'All Fees Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreport.aspx?absorb=00',null,'All Fees Not Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Surcharge Absorbed','orderreport.aspx?absorb=10',null,'Surcharge Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreport.aspx?absorb=01',null,'Paypal Fees Absorbed']";

                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report For Live School','orderreportall.aspx?demo=no',null,'Financial Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report For Demo School','orderreportall.aspx?demo=yes',null,'Financial Report']";

                    used = 1;
                }
                script += " ],";
            }
            else if (ConfigClass.UserType < 3)
            {
                script += "_cmSplit,"
                       + "[null,'" + ConfigClass.Heading + " Reports',null,null,'" + ConfigClass.Heading + " Reports',";
                int used = 0;
                if (used == 1)
                {
                    script += ",";
                }
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Attribute Sale Report','productAttributeSalesreport.aspx',null,'Product Attribute Sale Report']";


                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Wholesale Report','productWholeSalesreport.aspx',null,'Product Wholesale Report']";

                used = 1;

                if (pageAccess.IndexOf("treportall") > -1)
                {
                    if (used == 1)
                    {
                        script += ",";
                    }
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report All','transactionreportall.aspx',null,'Transaction Report All']";
                    used = 1;
                }



                if (pageAccess.Contains("pactivityreport"))
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Activity Report','productactivityreport.aspx',null,'Product Activity Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Stock Report','stockreport.aspx',null,'Stock Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Production Report','estoreproductionreport.aspx',null,'eStore Production Report']";

                    if (ConfigClass.UserType == 2)
                    {
                        script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report','ordersreportall.aspx',null,'Financial Report']";
                        script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Order Report','viewproductordersreport.aspx',null,'Product Order Report']";
                    }
                    used = 1;
                }
                script += " ],";
            }



            if (schoolid > 0 && ConfigClass.UserType == 2)
            {
                script += "_cmSplit,"
                       + "[null,'Financial Reports',null,null,'Financial Reports'";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreportcordi.aspx',null,'Paypal Fees Absorbed']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreportcordina.aspx',null,'All Fees Not Absorbed']";

                // script += " ],";
                //demo
                script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {
                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";

                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Report','productreports.aspx',null,'Product Report']";
                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Report','estorereports.aspx',null,'eStore Report']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','eventreports.aspx',null,'Event Report']";

                script += " ],";
                //demo
                script += "_cmSplit,"
                       + "[null,'Manuals',null,null,'Manuals'";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Demo and Training Material','demoTrainingMaterialfiles.aspx',null,'Manage Demo and Training Material']";
                script += " ],";
            }
            else
            {

                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";

                //script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','eStore Report','estorereports.aspx',null,'eStore Report']";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Parent Login History','customerloginHistory.aspx',null,'Parent Login History']";

                script += " ],";
            }

            script += "];"
                          + "cmDraw ('myMenuID', myMenu, 'hbr', cmThemeOffice, 'ThemeOffice');";

            return "<script type=\"text/javascript\">" + script + "</script>";

        }

        private string WriteScriptMenuEVENT(string pageAccess)
        {
            int schoolid = 0;
            int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["SchoolID"] == null)
                return string.Empty;
            else
                schoolid = Convert.ToInt32(HttpContext.Current.Session["SchoolID"]);
            Schools school = new Schools();
            school.SchoolId = schoolid;
            school.GetSchoolDetail();
            if (userType <= 1)
            {
                //Super admin has access to on/off module
                pageAccess += ",module";
            }
            //Javascript Menu
            pageAccess = pageAccess.ToLower();
            string script = "";
            if (schoolid == 0)
            {
                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],";
                                ////////_cmSplit,
                                ////////[null,'Site',null,null,'Site Management',"
                                //////// + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                //////// + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','../../index.aspx','_blank','Website Homepage'],"
                                //////// + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','../../index.aspx',null,'Inline View of Website Homepage'],"
                                //////// + "],"
                                //////// + "],";
            }
            else
            {
                string rehdetails = "";
                string Reg_Name = "";
                string Reg_uniqueid = "";
                posUserClass objUsers = new posUserClass();
                objUsers.SchoolID = schoolid;
                DataSet ds = new DataSet();
                ds = objUsers.PosRegisterforPreview();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Reg_Name = "";
                        Reg_uniqueid = "";
                        Reg_Name = Convert.ToString(ds.Tables[0].Rows[i]["REG_NAME"]);
                        Reg_uniqueid = Convert.ToString(ds.Tables[0].Rows[i]["REG_UNIQUE_VAL"]);
                        if (Reg_Name.Length > 0 && Reg_uniqueid.Length > 0)
                        {
                            if (rehdetails.Length == 0)
                                rehdetails = "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview POS Front', null, null, 'Preview',";
                            rehdetails = rehdetails + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','" + Reg_Name + "','" + ConfigClass.POSSite + "Login/LogIn/" + school.SubDomain + "/" + Reg_uniqueid + "','_blank','Website Homepage'],";
                        }
                    }
                    if (rehdetails.Length > 0)
                    {
                        rehdetails = rehdetails + "],";
                    }
                }

                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],
			                    _cmSplit,
			                    [null,'Site',null,null,'Site Management',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','/" + school.SubDomain + "/index.aspx','_blank','Website Homepage'],"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','/" + school.SubDomain + "/index.aspx',null,'Inline View of Website Homepage'],"
                                + "],"
                                  + rehdetails
                                + "],";
            }
            //----------------------------- ----------USERS-----------------------------------------------
            if (userType != 5 && userType != 4)//5 : agent   4 : admin personnel
            {
                script += "[null,'Manage',null,null,'Module Management',";
                if (schoolid > 0 && pageAccess.IndexOf("parents") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listparents.aspx',null,'Manage Parents']";
                    //if (schoolid == 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    //script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditparent.aspx',null,'Add/Edit Parents']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                else
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                if (userType == 1 && schoolid < 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Parents Homepage Alert', 'homepagealertfront.aspx', null, 'FAQ'],";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Coordinators Alert', 'homepagealerttocordi.aspx', null, 'FAQ'],";
                }

                if (pageAccess.IndexOf("users") > -1)
                {
                    if (schoolid > 0)
                    {
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listusers.aspx', null, 'Manage Users'],";

                        //----Register start
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Register', null, null, 'Manage Library',"
                                + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','registerassigalist.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditRegister.aspx',null,'Add/Edit Library']";
                        if (userType <= 1)
                            script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/View Register to Schools','registermaster.aspx',null,'Add/Edit Library']";
                        script += "],";
                        // Register End
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Pos Users', 'listPosUsers.aspx', null, 'Manage Pos Users'],";


                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Membership Type', null, null, 'Manage Membership Type',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listMemtype.aspx',null,'Manage Membership Type']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditMemtype.aspx',null,'Add/Edit Membership Type']";
                        script += "],";


                        //----Product Discount start
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Discount', null, null, 'Manage Discount',"
                                + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listDiscount.aspx',null,'Manage Discount']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','adddiscount.aspx',null,'Add/Edit Discount']";
                        script += "],";
                        // Product Discount  End
                    }
                    else
                    {
                        // if (Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                        if (userType == 1)
                        {
                            script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listadminusers.aspx', null, 'Manage Users'],";
                        }
                    }


                }

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries'],";
                }
                if (schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Delivery Option Appearance', 'manageDeliveryOPTApperance.aspx', null, 'Manage Delivery Option Appearance'],";
                }
                if (pageAccess.IndexOf("dynamic") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Dynamic Content', 'listdynamicpages.aspx', null, 'View Dynamic Content'],";
                }
                if (schoolid == 0)
                {
                    if (pageAccess.IndexOf("admindynamix") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Admin Dynamic Content', 'listadmindynamicpages.aspx', null, 'View Admin Dynamic Content'],";
                    }
                }
                if (userType > 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ'],";
                }
                if (userType <= 4 && pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }
                if (userType <= 2 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Class', 'listclasses.aspx', null, 'Manage Class',";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage CheckOut Content', 'addeditcheckoutcontent.aspx', null, 'Manage CheckOut Content',";
                    script += "],";
                    if (!school.TuckshopModule)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Enroll Comment', 'addeditenrollcomment.aspx', null, 'Manage Enroll Comment',";
                        script += "],";
                    }
                }
                if (userType <= 1)
                {//super admin
                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', 'schools.aspx', null, 'Manage Schools'],";
                    if (schoolid == 0 && pageAccess.IndexOf("schools") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Order Label', 'addeditcheckoutlabels.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Event Order Status', null, null, 'Manage Event Order Status',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','eventlistorderstatus.aspx',null,'Manage Event Order Status']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','eventorderstatusaddedit.aspx',null,'Add Event Order Status']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', null, null, 'Manage Schools',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','schools.aspx',null,'Manage Schools']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditschool.aspx',null,'Add School']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage e-Merchant', null, null, 'Manage e-Merchant',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listtranstype.aspx',null,'Manage e-Merchant']";
                        script += "],";

                        //---email template
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Email Template', null, null, 'Manage Email Template',"
                           + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','emailtemplatelisting.aspx',null,'Manage Email Template']";
                        script += "],";
                        //---custom message
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Custom Messages', null, null, 'Manage Custom Messages',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','custommsglist.aspx',null,'Manage Custom Messages']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Edit Financial History Notes', 'editsuperadminmessages.aspx', null, 'Manage Checkout Label',";
                        script += "],";

                    }

                    if (schoolid > 0 && pageAccess.IndexOf("library") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Library', null, null, 'Manage Library',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listlibrary.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditlibrary.aspx',null,'Add/Edit Library']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Notes', 'addedittooptip.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("news") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'News', null, null, 'News',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View News','listnews.aspx',null,'Manage News']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add News','addeditnews.aspx',null,'Add News']";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("subscribers") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'View Subscribers', 'listcustomers.aspx', null, 'View Subscribers',";
                        script += "],";
                    }
                }


                script += " ],";

            }

            if (userType == 4)
            {//admin personnel
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',"
                         + "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', null, null, 'Manage Users',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listusers.aspx',null,'Manage Users']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                script += "],";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries',";
                    script += "],";
                }

                if (pageAccess.IndexOf("dynamic") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Dynamic Content', 'listdynamicpages.aspx', null, 'Manage Dynamic Content',";
                    script += "],";
                }

                if (pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }

                script += " ],";
            }
            if (userType == 5)
            {//agent
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                script += " ],";
            }
            //----------------------------Super Admin Setting-------------------------------------------
            if (schoolid == 0 || pageAccess.IndexOf("faqlisting") > -1 || pageAccess.IndexOf("dashboardcontent") > -1 || pageAccess.IndexOf("loginscreen") > -1 || pageAccess.IndexOf("stickets") > -1)
            {
                script += "_cmSplit,[null,'Manage Settings',null,null,'Manage Settings',";
                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facility GST','faFacilityManageGST.aspx',null,'Manage Facility GST']";
                }


                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Parameters','parameterlist.aspx',null,'Manage Parameters']";
                }
                if (pageAccess.IndexOf("dashboardcontent") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Dashboard Content','dashboadmanage.aspx',null,'Manage Dashboard Content']";
                }
                if (schoolid == 0 && pageAccess.IndexOf("faqlisting") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage FAQ','listfaq.aspx',null,'Manage FAQ']";
                }
                if (pageAccess.IndexOf("loginscreen") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Login Screen Content','addscreenicon.aspx',null,'Manage Login Screen Content']";
                }
                if (pageAccess.IndexOf("stickets") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Support Ticket','supportticketlisting.aspx',null,'Manage Support Ticket']";
                }
                if (userType > 1 && schoolid > 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Home Page Alert','homepagealert.aspx',null,'Manage Support Ticket']";
                //script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Home Page Alert', 'homepagealert.aspx', null, 'Manage Home Page Alert'],";


                script += " ],";
            }
            //-----------------------------Gallery----------------------------------------------
            if (pageAccess.IndexOf("gallery") > -1 || pageAccess.IndexOf("gcategory") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Gallery',null,null,'Manage Gallery',";

                if (pageAccess.IndexOf("gcategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Categories', null, null, 'Manage Gallery Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgallerycategories.aspx',null,'Manage Gallery Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgallerycategory.aspx',null,'Add/Edit Gallery Categories']";
                    script += "],";
                }

                if (pageAccess.IndexOf("gallery") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Photos', null, null, 'Manage Gallery Photos',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgalleryphotos.aspx',null,'Manage Gallery Photos']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgalleryphotos.aspx',null,'Add/Edit Gallery Photos']";
                    script += "],";
                }
                script += " ],";

            }
            //-----------------------------Events----------------------------------------------
            if (school.EventModule && schoolid > 0 && (pageAccess.IndexOf("events") > -1 || pageAccess.IndexOf("eorder") > -1 || pageAccess.IndexOf("surveylist") > -1))
            {

                script += "_cmSplit,"
                       + "[null,'Events',null,null,'Events',";

                if (pageAccess.IndexOf("events") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Paid Events', null, null, 'Manage Paid Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";
                    script += "],";

                }
                if (pageAccess.IndexOf("events") > -1 || pageAccess.IndexOf("eorder") > -1)
                {
                    //event product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Food Items', null, null, 'Manage Event Food Items',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','productlistfood.aspx',null,'Manage Event Food Itemss']";

                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','productaddeditnewfood.aspx',null,'Add/Edit Event Food Items']";
                    script += "],";

                }
                if (pageAccess.IndexOf("eventbasic") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Basic Events', null, null, 'Manage Basic Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventsbasic.aspx',null,'Manage Basic Events']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventbasic.aspx',null,'Add/Edit Basic Events']";
                    script += "],";
                }
                if (pageAccess.IndexOf("eorder") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event Orders', null, null, 'Manage Event Orders',"
                             + ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','View Order','vieweventorders.aspx',null,'View Order']";
                    script += "],";
                }
                if (pageAccess.IndexOf("ecategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Event category', null, null, 'Manage Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventcategories.aspx',null,'Manage Event category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventcategory.aspx',null,'Add/Edit Event category']";
                    script += "],";
                }
                if (pageAccess.IndexOf("elocation") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Location category', null, null, 'Manage Events',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listeventlocation.aspx',null,'Manage Location category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addediteventlocation.aspx',null,'Add/Edit Location category']";
                    script += "],";
                }

                script += " ],";

            }
            //------------------------------------Registerred Members------------------------
            if (pageAccess.IndexOf("customers") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Members',null,null,'Manage Members',";
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Registered Users','listmembers.aspx',null,'View Registered Users']"
                       + "],";
            }

            if (userType == 4)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Report',null,null,'Manage Report',";
                int used = 0;
                if (used == 1)
                {
                    script += ",";
                }
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx',null,'Patron Report 2']";
                used = 1;
                script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {//for supera admin               
                script += "_cmSplit,"
                    + "[null,'" + ConfigClass.Heading + "',null,null,'" + ConfigClass.Heading + "',";
                int used = 0;
                if (pageAccess.IndexOf("producttransactionreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Product Transaction Report','producttransactionreport.aspx',null,'Product Transaction Report']";
                    used = 1;
                }
                if (ConfigClass.UserType == 1 && pageAccess.IndexOf("orderreport") > -1)
                {
                    if (used == 1)
                        script += ",";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Absorbed','orderreport.aspx?absorb=11',null,'All Fees Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreport.aspx?absorb=00',null,'All Fees Not Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Surcharge Absorbed','orderreport.aspx?absorb=10',null,'Surcharge Absorbed']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreport.aspx?absorb=01',null,'Paypal Fees Absorbed']";
                    // script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report','orderreportall.aspx',null,'Financial Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report For Live School','orderreportall.aspx?demo=no',null,'Financial Report']";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Financial Report For Demo School','orderreportall.aspx?demo=yes',null,'Financial Report']";

                    used = 1;
                }
                script += " ],";
            }

            if (schoolid > 0 && ConfigClass.UserType == 2)
            {
                script += "_cmSplit,"
                       + "[null,'Financial Reports',null,null,'Financial Reports'";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paypal Fees Absorbed','orderreportcordi.aspx',null,'Paypal Fees Absorbed']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','All Fees Not Absorbed','orderreportcordina.aspx',null,'All Fees Not Absorbed']";

                // script += " ],";
                //demo
                script += " ],";
            }

            if (schoolid == 0 && ConfigClass.UserType == 1)
            {
                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','eventreports.aspx',null,'Event Report']";

                script += " ],";
                //demo
                script += "_cmSplit,"
                       + "[null,'Manuals',null,null,'Manuals'";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Demo and Training Material','demoTrainingMaterialfiles.aspx',null,'Manage Demo and Training Material']";
                script += " ],";
            }
            else
            {

                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";

                if (school.EventModule)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','eventreports.aspx',null,'Event Report']";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Parent Login History','customerloginHistory.aspx',null,'Parent Login History']";

                script += " ],";
            }

            if (school.EventModule)
            {
                if (schoolid > 0 && (pageAccess.IndexOf("eventreport") > -1 || pageAccess.IndexOf("patronreport2") > -1 || pageAccess.IndexOf("accesscontrol") > -1 || pageAccess.IndexOf("eventstatus") > -1 || pageAccess.IndexOf("attreport") > -1))
                {
                    script += "_cmSplit,"
                           + "[null,'Event Reports',null,null,'Event Reports',";
                    int used = 0;

                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Paid Events', null, null, 'Paid Events',";
                    //+ "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                    // script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";



                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paid Event','reportevent.aspx',null,'Event Report']";
                    if (pageAccess.IndexOf("eventreport") > -1)
                    {

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','reportevent.aspx?etype=p',null,'Event Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport1") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 1','patronreport1.aspx?etype=p',null,'Patron Report 1']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport2") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx?etype=p',null,'Patron Report 2']";
                        script += ",";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 3','patronreport3.aspx?etype=p',null,'Patron Report 3']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("accesscontrol") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Access Control Download Report','reportaccesscontrol.aspx?etype=p',null,'Access Control Download Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("eventstatus") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Status Report','eventstatusreport.aspx?etype=p',null,'Event Status Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("performancereport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Performance Report','performancereport.aspx?etype=p',null,'Performance Report']";
                        used = 1;
                    }

                    if (pageAccess.IndexOf("transactionreport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report','transactionreport.aspx?etype=p',null,'Transaction Report']";
                        used = 1;
                    }

                    if (pageAccess.IndexOf("paymentreport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Payment Report','paymentreport.aspx?etype=p',null,'Payment Report']";
                        used = 1;
                    }
                    script += "],";


                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Basic Events', null, null, 'Paid Events',";
                    //+ "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listevents.aspx',null,'Manage Paid Events']";
                    // script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditevent.aspx',null,'Add/Edit Paid Events']";



                    //script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Paid Event','reportevent.aspx',null,'Event Report']";
                    if (pageAccess.IndexOf("eventreport") > -1)
                    {

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Report','reportevent.aspx?etype=b',null,'Event Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport1") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 1','patronreport1.aspx?etype=b',null,'Patron Report 1']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("patronreport2") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 2','patronreport2.aspx?etype=b',null,'Patron Report 2']";
                        script += ",";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Patron Report 3','patronreport3.aspx?etype=b',null,'Patron Report 3']";

                        used = 1;
                    }
                    if (pageAccess.IndexOf("accesscontrol") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Access Control Download Report','reportaccesscontrol.aspx?etype=b',null,'Access Control Download Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("eventstatus") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Event Status Report','eventstatusreport.aspx?etype=b',null,'Event Status Report']";
                        used = 1;
                    }
                    if (pageAccess.IndexOf("performancereport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Performance Report','performancereport.aspx?etype=b',null,'Performance Report']";
                        used = 1;
                    }

                    if (pageAccess.IndexOf("transactionreport") > -1)
                    {
                        if (used == 1)
                        {
                            script += ",";
                        }
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Transaction Report','transactionreport.aspx?etype=b',null,'Transaction Report']";
                        used = 1;
                    }

                    //if (pageAccess.IndexOf("paymentreport") > -1)
                    //{
                    //          if (used == 1)
                    //          {
                    //                    script += ",";
                    //          }
                    //          script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Payment Report','paymentreport.aspx?etype=b',null,'Payment Report']";
                    //          used = 1;
                    //}
                    script += "],";


                    script += " ],";
                }
            }

            script += "];"
                          + "cmDraw ('myMenuID', myMenu, 'hbr', cmThemeOffice, 'ThemeOffice');";

            return "<script type=\"text/javascript\">" + script + "</script>";

        }


        private string WriteScriptMenuFACILITY(string pageAccess)
        {
            int schoolid = 0;
            int userType = Convert.ToInt32(HttpContext.Current.Session["USER_TYPE"]);
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["SchoolID"] == null)
                return string.Empty;
            else
                schoolid = Convert.ToInt32(HttpContext.Current.Session["SchoolID"]);
            Schools school = new Schools();
            school.SchoolId = schoolid;
            school.GetSchoolDetail();
            if (userType <= 1)
            {
                //Super admin has access to on/off module
                pageAccess += ",module";
            }
            //Javascript Menu
            pageAccess = pageAccess.ToLower();
            string script = "";
            if (schoolid == 0)
            {
                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],";
                                //////_cmSplit,
                                //////[null,'Site',null,null,'Site Management',"
                                ////// + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                ////// + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','../../index.aspx','_blank','Website Homepage'],"
                                ////// + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','../../index.aspx',null,'Inline View of Website Homepage'],"
                                ////// + "],"
                                ////// + "],";
            }
            else
            {
                string rehdetails = "";
                string Reg_Name = "";
                string Reg_uniqueid = "";
                posUserClass objUsers = new posUserClass();
                objUsers.SchoolID = schoolid;
                DataSet ds = new DataSet();
                ds = objUsers.PosRegisterforPreview();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Reg_Name = "";
                        Reg_uniqueid = "";
                        Reg_Name = Convert.ToString(ds.Tables[0].Rows[i]["REG_NAME"]);
                        Reg_uniqueid = Convert.ToString(ds.Tables[0].Rows[i]["REG_UNIQUE_VAL"]);
                        if (Reg_Name.Length > 0 && Reg_uniqueid.Length > 0)
                        {
                            if (rehdetails.Length == 0)
                                rehdetails = "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview POS Front', null, null, 'Preview',";
                            rehdetails = rehdetails + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','" + Reg_Name + "','" + ConfigClass.POSSite + "Login/LogIn/" + school.SubDomain + "/" + Reg_uniqueid + "','_blank','Website Homepage'],";
                        }
                    }
                    if (rehdetails.Length > 0)
                    {
                        rehdetails = rehdetails + "],";
                    }
                }

                script = @"var myMenu =
		                        [
			                    [null,'Home','desktop.aspx',null,'Control Panel'],
			                    _cmSplit,
			                    [null,'Site',null,null,'Site Management',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />', 'Preview', null, null, 'Preview',"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','In New Window','/" + school.SubDomain + "/index.aspx','_blank','Website Homepage'],"
                                + "['<img src=\"images/ThemeOffice/preview.png\" alt=\"\" />','Inline','/" + school.SubDomain + "/index.aspx',null,'Inline View of Website Homepage'],"
                                + "],"
                               + rehdetails
                                + "],";
            }
            //----------------------------- ----------USERS-----------------------------------------------
            if (userType != 5 && userType != 4)//5 : agent   4 : admin personnel
            {
                script += "[null,'Manage',null,null,'Module Management',";
                if (schoolid > 0 && pageAccess.IndexOf("parents") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listparents.aspx',null,'Manage Parents']";
                    //if (schoolid == 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                else
                {
                    script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Parents', 'listparents.aspx', null, 'Manage Parents'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','parentsearch.aspx',null,'Manage Parents History']";
                    script += "],";

                    script += ",['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Students', null, null, 'Manage Students'";
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Details','studentlist.aspx',null,'Manage Students']";
                    script += "],";
                }
                if (userType == 1 && schoolid < 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Parents Homepage Alert', 'homepagealertfront.aspx', null, 'FAQ'],";
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Coordinators Alert', 'homepagealerttocordi.aspx', null, 'FAQ'],";
                }

                if (pageAccess.IndexOf("users") > -1)
                {
                    if (schoolid > 0)
                    {
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listusers.aspx', null, 'Manage Users'],";

                        //----Register start
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Register', null, null, 'Manage Library',"
                                + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','registerassigalist.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditRegister.aspx',null,'Add/Edit Library']";
                        if (userType <= 1)
                            script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add/View Register to Schools','registermaster.aspx',null,'Add/Edit Library']";
                        script += "],";
                        // Register End
                        script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Pos Users', 'listPosUsers.aspx', null, 'Manage Pos Users'],";


                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Membership Type', null, null, 'Manage Membership Type',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listMemtype.aspx',null,'Manage Membership Type']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditMemtype.aspx',null,'Add/Edit Membership Type']";
                        script += "],";

                        //----Product Discount start
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Discount', null, null, 'Manage Discount',"
                                + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listDiscount.aspx',null,'Manage Discount']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','adddiscount.aspx',null,'Add/Edit Discount']";
                        script += "],";
                        // Product Discount  End
                    }
                    else
                    {
                        // if (Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                        if (userType == 1)
                        {
                            script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', 'listadminusers.aspx', null, 'Manage Users'],";
                        }
                    }
                }
                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries'],";
                }
                if (schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Delivery Option Appearance', 'manageDeliveryOPTApperance.aspx', null, 'Manage Delivery Option Appearance'],";
                }

                if (pageAccess.IndexOf("dynamic") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Dynamic Content', 'listdynamicpages.aspx', null, 'View Dynamic Content'],";
                }
                if (schoolid == 0)
                {
                    if (pageAccess.IndexOf("admindynamix") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Admin Dynamic Content', 'listadmindynamicpages.aspx', null, 'View Admin Dynamic Content'],";
                    }
                }
                if (userType > 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ'],";
                }
                if (userType <= 4 && pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }
                if (userType <= 2 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Class', 'listclasses.aspx', null, 'Manage Class',";
                    script += "],";
                    script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage CheckOut Content', 'addeditcheckoutcontent.aspx', null, 'Manage CheckOut Content',";
                    script += "],";
                    if (!school.TuckshopModule)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Enroll Comment', 'addeditenrollcomment.aspx', null, 'Manage Enroll Comment',";
                        script += "],";
                    }
                }
                if (userType <= 1)
                {//super admin
                    if (schoolid == 0 && pageAccess.IndexOf("schools") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Order Label', 'addeditcheckoutlabels.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Event Order Status', null, null, 'Manage Event Order Status',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','eventlistorderstatus.aspx',null,'Manage Event Order Status']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','eventorderstatusaddedit.aspx',null,'Add Event Order Status']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Schools', null, null, 'Manage Schools',"
                              + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','schools.aspx',null,'Manage Schools']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditschool.aspx',null,'Add School']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage e-Merchant', null, null, 'Manage e-Merchant',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listtranstype.aspx',null,'Manage e-Merchant']";
                        script += "],";

                        //---email template
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Email Template', null, null, 'Manage Email Template',"
                           + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','emailtemplatelisting.aspx',null,'Manage Email Template']";
                        script += "],";
                        //---custom message
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Custom Messages', null, null, 'Manage Custom Messages',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','custommsglist.aspx',null,'Manage Custom Messages']";
                        script += "],";

                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Edit Financial History Notes', 'editsuperadminmessages.aspx', null, 'Manage Checkout Label',";
                        script += "],";

                    }

                    if (schoolid > 0 && pageAccess.IndexOf("library") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Library', null, null, 'Manage Library',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listlibrary.aspx',null,'Manage Library']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditlibrary.aspx',null,'Add/Edit Library']";
                        script += "],";
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'Manage Notes', 'addedittooptip.aspx', null, 'Manage Checkout Label',";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("news") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'News', null, null, 'News',"
                            + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View News','listnews.aspx',null,'Manage News']";
                        script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add News','addeditnews.aspx',null,'Add News']";
                        script += "],";
                    }
                    if (schoolid > 0 && pageAccess.IndexOf("subscribers") > -1)
                    {
                        script += "['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />', 'View Subscribers', 'listcustomers.aspx', null, 'View Subscribers',";
                        script += "],";


                    }
                }


                script += " ],";

            }

            if (userType == 4)
            {//admin personnel
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',"
                         + "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Users', null, null, 'Manage Users',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listusers.aspx',null,'Manage Users']";
                script += "],";
                script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Products Orders', null, null, 'Manage Products Orders',"
                         + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','viewproductorders.aspx',null,'View Orders']";
                script += "],";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                if (pageAccess.IndexOf("contact") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'View Contact Us Queries', 'contactuslisting.aspx', null, 'View Contact Us Queries',";
                    script += "],";
                }

                if (pageAccess.IndexOf("dynamic") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Manage Dynamic Content', 'listdynamicpages.aspx', null, 'Manage Dynamic Content',";
                    script += "],";
                }

                if (pageAccess.IndexOf("listticket") > -1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'Tickets Listing', 'listtickets.aspx', null, 'Tickets Listing',";
                    script += "],";
                }

                script += " ],";
            }
            if (userType == 5)
            {//agent
                script += "_cmSplit,"
                         + "[null,'Manage',null,null,'Module Management',";


                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />', 'FAQ', 'faqdisplay.aspx', null, 'FAQ',";
                script += "],";

                script += " ],";
            }
            //----------------------------Super Admin Setting-------------------------------------------
            if (schoolid == 0 || pageAccess.IndexOf("faqlisting") > -1 || pageAccess.IndexOf("dashboardcontent") > -1 || pageAccess.IndexOf("loginscreen") > -1 || pageAccess.IndexOf("stickets") > -1)
            {
                script += "_cmSplit,[null,'Manage Settings',null,null,'Manage Settings',";
                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facility GST','faFacilityManageGST.aspx',null,'Manage Facility GST']";
                }


                if (schoolid == 0 && Convert.ToInt32(HttpContext.Current.Session["UID"]) == 1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Parameters','parameterlist.aspx',null,'Manage Parameters']";
                }
                if (pageAccess.IndexOf("dashboardcontent") > -1 && schoolid > 0)
                {
                    script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Dashboard Content','dashboadmanage.aspx',null,'Manage Dashboard Content']";
                }
                if (schoolid == 0 && pageAccess.IndexOf("faqlisting") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage FAQ','listfaq.aspx',null,'Manage FAQ']";
                }
                if (pageAccess.IndexOf("loginscreen") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Login Screen Content','addscreenicon.aspx',null,'Manage Login Screen Content']";
                }
                if (pageAccess.IndexOf("stickets") > -1)
                {
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Support Ticket','supportticketlisting.aspx',null,'Manage Support Ticket']";
                }
                if (userType > 1 && schoolid > 0)
                    script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Home Page Alert','homepagealert.aspx',null,'Manage Support Ticket']";
                //script += "['<img src=\"images/ThemeOffice/users_add.png\" alt=\"\" />', 'Manage Home Page Alert', 'homepagealert.aspx', null, 'Manage Home Page Alert'],";


                script += " ],";
            }
           
            //-----------------------------Gallery----------------------------------------------
            if (pageAccess.IndexOf("gallery") > -1 || pageAccess.IndexOf("gcategory") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Gallery',null,null,'Manage Gallery',";

                if (pageAccess.IndexOf("gcategory") > -1)
                {
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Categories', null, null, 'Manage Gallery Categories',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgallerycategories.aspx',null,'Manage Gallery Categories']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgallerycategory.aspx',null,'Add/Edit Gallery Categories']";
                    script += "],";
                }

                if (pageAccess.IndexOf("gallery") > -1)
                {
                    //Product
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Gallery Photos', null, null, 'Manage Gallery Photos',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','listgalleryphotos.aspx',null,'Manage Gallery Photos']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','addeditgalleryphotos.aspx',null,'Add/Edit Gallery Photos']";
                    script += "],";
                }
                script += " ],";

            }
            //------------------------------------Registerred Members------------------------
            if (pageAccess.IndexOf("customers") > -1)
            {
                script += "_cmSplit,"
                       + "[null,'Manage Members',null,null,'Manage Members',";
                script += "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Registered Users','listmembers.aspx',null,'View Registered Users']"
                       + "],";
            }


            if (schoolid == 0 && ConfigClass.UserType == 1)
            {
                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";
                //demo
                script += "_cmSplit,"
                       + "[null,'Manuals',null,null,'Manuals'";

                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Demo and Training Material','demoTrainingMaterialfiles.aspx',null,'Manage Demo and Training Material']";
                script += " ],";
            }
            else
            {

                script += "_cmSplit,"
                       + "[null,'My Reports',null,null,'My Reports'";
                script += ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Parent Login History','customerloginHistory.aspx',null,'Parent Login History']";

                script += " ],";
            }

            
            if (school.FacilityModule)
            {
                //if (schoolid > 0 && (pageAccess.IndexOf("eventreport")>-1||pageAccess.IndexOf("patronreport2") > -1)
                if (schoolid > 0)
                {
                    //Manage Resource
                    script += "_cmSplit,[null,'Manage Facilities',null,null,'Manage Facilities',";

                    //term
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Term', null, null, 'Term Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilityTermlisting.aspx',null,'Term Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityTermaddedit.aspx',null,'Add/Edit Term']";
                    script += "],";
                    //Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facility Category', null, null, 'Manage Facility Category',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilityCategorylisting.aspx',null,'Manage Facility Category']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityCategoryaddedit.aspx',null,'Add/Edit Facilities Category']";
                    script += "],";
                    //locaton
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Location', null, null, 'Location Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilityLocationlisting.aspx',null,'Location Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityLocationaddedit.aspx',null,'Add/Edit Location']";
                    script += "],";

                    //sub location
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Sub Location', null, null, 'Sub Location Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilitySubLocationlisting.aspx',null,'Sub Location Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilitySubLocationaddedit.aspx',null,'Add/Edit Sub Location']";
                    script += "],";
                    //resource Category
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Resource Category', null, null, 'Resource Category Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACResourceCategorylisting.aspx',null,'Resource Category Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACResourceCategoryaddedit.aspx',null,'Add/Edit Resource Categories']";
                    script += "],";
                    //resource 
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Resource', null, null, 'Resource Listing',"
                       + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilityResourcelisting.aspx',null,'Resource Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityResourceaddedit.aspx',null,'Add/Edit Resource']";
                    script += "],";
                    //Facilities
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facilities', null, null, 'Facilities Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','FACFacilitylisting.aspx',null,'Facilities Listing']";
                    script += ",['<img src=\"images/ThemeOffice/add_section.png\" alt=\"\" />','Add New','FACFacilityaddedit.aspx',null,'Add/Edit Facilities']";
                    script += "],";
                    //Facilities booking
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage booked Facilities', null, null, 'Facilities Listing',"
                     + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','View Listing','faBookedFacility.aspx',null,'Booked Facilities']";
                    script += "],";
                    //Facilities parameter
                    //script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facilities Parameter', null, null, 'Manage Facilities Parameter',"
                    // + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Facilities Cut Off Time                                                     ','faCutofftime.aspx',null,'Manage Facilities Cut Off Time']"
                    //   + ",['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Manage Cancel Fee','faFacilityManageCancelFee.aspx',null,'Manage Cancel Fee']";
                    //script += "],";

                    //Facilities notes
                    script += "['<img src=\"images/ThemeOffice/messaging_config.png\" alt=\"\" />', 'Manage Facility Help Notes', null, null, 'Facility Help Notes',"
                    + "['<img src=\"images/ThemeOffice/content.png\" alt=\"\" />','Add Facility Help Notes','Facilitynotes.aspx',null,'Facility Help Notes']";
                    script += "],";

                    script += " ],";

                }
            }

            script += "];"
                          + "cmDraw ('myMenuID', myMenu, 'hbr', cmThemeOffice, 'ThemeOffice');";

            return "<script type=\"text/javascript\">" + script + "</script>";

        }


        #endregion
        #endregion

    }
}
