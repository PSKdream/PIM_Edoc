using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Data;
using System.Linq;
using System.Web;
using System.Collections.Generic;

using PIMEdoc_CR;
using PIMEdoc_CR.Default.Rule;

namespace PIMEdoc_CR.ControlTemplates.PIMEdoc_CR
{
    public partial class Menu : UserControl
    {
        //I Property for check user to live in
        private bool bGroupOwner;
        private static string siteUrl = SPContext.Current.Web.Url;
        private SPSite oSPsite;

        protected string sElementOfMobile
        {
            get
            {
                if (ViewState["_sElementOfMobile"] == null)
                {
                    ViewState["_sElementOfMobile"] = string.Empty;
                }
                return (string)ViewState["_sElementOfMobile"];
            }
            set
            {
                ViewState["_sElementOfMobile"] = value;
            }
        }
        protected string sDefaultLang
        {
            get
            {
                if (ViewState["_sDefaultLang"] == null)
                {
                    ViewState["_sDefaultLang"] = "EN";
                }
                return (string)ViewState["_sDefaultLang"];
            }
            set
            {
                ViewState["_sDefaultLang"] = value;
            }
        }

        public string GetDefaultLang(string sLowerUser)
        {
            try
            {
                SPListItemCollection ListSPListItem = null;
                string sDefaultLang = "EN";
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (oSPsite = new SPSite(siteUrl))
                    {
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {
                            oSPWeb.AllowUnsafeUpdates = true;
                            SPList oList = oSPWeb.Lists["Config_DefaultLanguage"];
                            SPQuery myquery = new SPQuery();
                            myquery.Query = string.Format("<Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query>", sLowerUser);
                            ListSPListItem = oList.GetItems(myquery);
                            if (ListSPListItem != null)
                            {
                                if (ListSPListItem.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(ListSPListItem[0]["DefaultLang"].ToString()))
                                    {
                                        sDefaultLang = ListSPListItem[0]["DefaultLang"].ToString();
                                    }
                                }
                                else
                                {
                                    SPListItem NewItem = oList.Items.Add();
                                    NewItem["Title"] = sLowerUser;
                                    NewItem["DefaultLang"] = "EN";
                                    NewItem.Update();
                                    sDefaultLang = "EN";
                                }
                            }
                            oSPWeb.AllowUnsafeUpdates = false;
                        }
                    }
                });
                return sDefaultLang;
            }
            catch (Exception ex)
            {
                return "EN";
            }
        }
        public void SetDefaultLang(string sLowerUser)
        {
            try
            {
                SPListItemCollection ListSPListItem = null;                
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (oSPsite = new SPSite(siteUrl))
                    {
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {
                            oSPWeb.AllowUnsafeUpdates = true;
                            SPList oList = oSPWeb.Lists["Config_DefaultLanguage"];
                            SPQuery myquery = new SPQuery();
                            myquery.Query = string.Format("<Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query>", sLowerUser);
                            ListSPListItem = oList.GetItems(myquery);
                            if (ListSPListItem != null)
                            {
                                if (ListSPListItem.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(ListSPListItem[0]["DefaultLang"].ToString()))
                                    {
                                        SPListItem NewItem = ListSPListItem[0];
                                        string sLang =  ListSPListItem[0]["DefaultLang"].ToString() == "EN" ? "TH" : "EN";
                                        NewItem["DefaultLang"] = sLang;
                                        NewItem.Update();
                                        sDefaultLang = sLang;                                       
                                    }
                                }
                            }
                            oSPWeb.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
            }
        }
        private DataTable DeleteDuplicateBy()
        {
            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    sDefaultLang = GetDefaultLang(SPContext.Current.Web.CurrentUser.LoginName);
                    SetCssButtonByLang();
                    PopulateMenu();
                    setK2WorkCount();
                }
                ScriptManager.RegisterStartupScript(this.Page, typeof(System.Web.UI.Page),
                    "ftPageLoad_Menu", "ftPageLoad_Menu();", true);
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }



        }
        private void PopulateMenu()
        {
            //I Get target data of list (folder in sharepoint) in format DataTable
            DataTable dt = GetDataNavigation();
            DataTable dtGroup = GetGroupByUserNameAndDomainName();
            //I Query for select target data
            string filter = "AudienceID = '' OR AudienceID is null";

            #region  " Check Permission "

            DataTable dtReturn;

            #region  " Validate "
            if (dt.IsNullOrEmpty())
            {
                //I Error
                return;
            }
            if (dtGroup.IsNullOrEmpty())
            {
                //I Error
                return;
            }
            #endregion Validate --- End

            #region  " Check permission "
            for (int i1 = 0; i1 < dtGroup.Rows.Count; i1++)
            {
                /*
                if (i1 > 0) {
                    filter = (filter + " OR ");
                }
                */
                filter = (filter + " OR ");
                filter = (filter + "AudienceID = '" + dtGroup.Rows[i1]["ID"].ToString() + "'");
            }
            dtReturn = dt.SelectToTable(filter);
            #endregion Check permission --- End

            #endregion Check Permission --- End

            dtReturn = dtReturn.Sort("Sequence ASC");
            dtReturn = DeleteDupOfNavigation(dtReturn);
            filter = "Level = 1 AND ISNULL(ParentNodeID,'') = ''";
            sElementOfMobile = "<ul class=\"nav nav-pills nav-stacked\" id=\"idMainList\">";
            Menu1.Items.Clear();
            procMenuRender(dtReturn, Menu1.Items, filter);
            sElementOfMobile = (sElementOfMobile + "<ul>");
        }
        #region  " New Element "
        //I Delete duplicate
        private DataTable DeleteDupOfNavigation(DataTable argDt)
        {
            DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add("NodeID");
            dtReturn.Columns.Add("ParentNodeID");
            dtReturn.Columns.Add("Title");
            dtReturn.Columns.Add("Url");
            dtReturn.Columns.Add("Level");
            dtReturn.Columns.Add("AudienceName");
            dtReturn.Columns.Add("Sequence");
            dtReturn.Columns.Add("AudienceID");
            try
            {
                for (int i1 = 0; i1 < argDt.Rows.Count; i1++)
                {
                    DataTable dtTest = argDt.SelectToTable("NodeID = '" + argDt.Rows[i1]["NodeID"].ToString() + "'");
                    if (dtTest != null)
                    {
                        if (dtTest.Rows.Count > 0)
                        {
                            if (dtTest.Rows.Count == 1)
                            {
                                //DataRow dr = argDt.NewRow();
                                DataRow dr = dtReturn.NewRow();
                                dr["NodeID"] = (dtTest.Rows[0]["NodeID"] == null) ? "" : dtTest.Rows[0]["NodeID"].ToString();
                                dr["ParentNodeID"] = (dtTest.Rows[0]["ParentNodeID"] == null) ? "" : dtTest.Rows[0]["ParentNodeID"].ToString();
                                dr["Title"] = (dtTest.Rows[0]["Title"] == null) ? "" : dtTest.Rows[0]["Title"].ToString();
                                dr["Url"] = (dtTest.Rows[0]["Url"] == null) ? "" : dtTest.Rows[0]["Url"].ToString();
                                dr["Level"] = (dtTest.Rows[0]["Level"] == null) ? "" : dtTest.Rows[0]["Level"].ToString();
                                dr["AudienceName"] = (dtTest.Rows[0]["AudienceName"] == null) ? "" : dtTest.Rows[0]["AudienceName"].ToString();
                                dr["Sequence"] = (dtTest.Rows[0]["Sequence"] == null) ? "" : dtTest.Rows[0]["Sequence"].ToString();
                                dr["AudienceID"] = (dtTest.Rows[0]["AudienceID"] == null) ? "" : dtTest.Rows[0]["AudienceID"].ToString();
                                dtReturn.Rows.Add(dr);
                            }
                            else
                            {
                                DataTable dtTest2 = dtReturn.SelectToTable("NodeID = '" + dtTest.Rows[0]["NodeID"].ToString() + "'");
                                if (dtTest2 != null)
                                {
                                    if (dtTest2.Rows.Count > 0)
                                    {
                                        continue;
                                    }
                                }
                                //DataRow dr = argDt.NewRow();
                                DataRow dr = dtReturn.NewRow();
                                dr["NodeID"] = (dtTest.Rows[0]["NodeID"] == null) ? "" : dtTest.Rows[0]["NodeID"].ToString();
                                dr["ParentNodeID"] = (dtTest.Rows[0]["ParentNodeID"] == null) ? "" : dtTest.Rows[0]["ParentNodeID"].ToString();
                                dr["Title"] = (dtTest.Rows[0]["Title"] == null) ? "" : dtTest.Rows[0]["Title"].ToString();
                                dr["Url"] = (dtTest.Rows[0]["Url"] == null) ? "" : dtTest.Rows[0]["Url"].ToString();
                                dr["Level"] = (dtTest.Rows[0]["Level"] == null) ? "" : dtTest.Rows[0]["Level"].ToString();
                                dr["AudienceName"] = (dtTest.Rows[0]["AudienceName"] == null) ? "" : dtTest.Rows[0]["AudienceName"].ToString();
                                dr["Sequence"] = (dtTest.Rows[0]["Sequence"] == null) ? "" : dtTest.Rows[0]["Sequence"].ToString();
                                dr["AudienceID"] = (dtTest.Rows[0]["AudienceID"] == null) ? "" : dtTest.Rows[0]["AudienceID"].ToString();
                                for (int i2 = 1; i2 < dtTest.Rows.Count; i2++)
                                {
                                    dr["AudienceID"] = (dr["AudienceID"].ToString() + ";" + dtTest.Rows[i2]["AudienceID"].ToString());
                                    dr["AudienceName"] = (dr["AudienceName"].ToString() + ";"
                                        + dtTest.Rows[i2]["AudienceName"].ToString());
                                }
                                dtReturn.Rows.Add(dr);
                                /*
                                 "NodeID", "ParentNodeID", "Title", "Url", "Level",
                                 "AudienceName","Sequence","AudienceID"
                                */
                            }
                        }
                    }
                }
                return dtReturn;
            }
            catch (Exception ex)
            {
                return null;
            }           
        }
        #endregion New Element --- End
        //I Method for checking current user to logon.
        ///
        //I To be member of current group name to passed.
        private void SetCssButtonByLang()
        {
            btnLang.CssClass = string.Format("IconLang{0}", sDefaultLang);
        }
        private bool IsMemberOfGroup(string GroupName, string LogOnName)
        {
            bool b = false;
            string s_user = string.Empty;
            //I Unknow
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                //I New object of sharepoint by current url site
                using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                {
                    //I Maybe get current web group information by group name
                    SPGroup oGroup = SPContext.Current.Web.Groups[GroupName];
                    //I Foreach by user in group
                    foreach (SPUser user in oGroup.Users)
                    {
                        //I Maybe used "LoginName" or "Name" for checking
                        string[] users = user.LoginName.Split('\\');
                        string[] names = user.Name.Split('\\');
                        if (users.Length == 2)
                        {
                            if (users[1].Trim().ToLower() != "system")
                            {
                                if (users[1].Trim().ToLower() == LogOnName.ToLower())
                                {
                                    b = true;
                                }
                            }
                        }
                        else
                        {
                            if (names.Length == 2)
                            {
                                if (names[1].Trim().ToLower() != "system")
                                {
                                    if (names[1].Trim().ToLower() == "domain users")
                                    {
                                        b = true;
                                    }
                                }
                            }
                            else
                            {
                                if (user.Name.ToLower() == "everyone")
                                {
                                    b = true;
                                }
                            }
                        }
                    }
                }
            });

            //I Return result for checking.
            //I Current user name to logon.
            //I To be member of current group.
            return b;
        }
        //I Get data in format DataTable for navigation bar
        private DataTable GetDataNavigation()
        {

            #region     Unused statement

            //I Class site of sharepoint
            SPSite oSPsite;
            //I Object of data for return
            DataTable dt_group = new DataTable();
            //I Setting column
            dt_group.Columns.Add("GroupID");
            dt_group.Columns.Add("GroupName");
            //I Url of sharepoint site

            //I Unknow statement
            //I Get group id block
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                string SharedPointSite = SPContext.Current.Site.Url;
                //I New object of sharepoint site
                using (oSPsite = new SPSite(SharedPointSite))
                {
                    //I Maybe open target web site
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        //I Unknow
                        for (int i = 0; i < oSPWeb.Groups.Count; i++)
                        {
                            SPGroup Group = oSPWeb.Groups[i];
                            dt_group.Rows.Add(Group.ID, Group.Name);
                        }
                    }
                }
            });
            //I Maybe string for keep group id of current user
            string filter = string.Empty;
            //I Get row in any group
            foreach (DataRow dr_group in dt_group.Rows)
            {
                //I Check current user to be member of group in current row
                if (IsMemberOfGroup(dr_group["GroupName"].ToString(), GetLogOnName()))
                {
                    //I Assign group id to filter string
                    filter += string.Format("'{0}',", dr_group["GroupID"].ToString());
                }
            }
            //I Remove last comma in case filter valid
            if (!string.IsNullOrEmpty(filter)) { filter = filter.Remove(filter.Length - 1, 1); }

            #endregion  Unused statement

            #region   Used for get data
            //I Get information in format DataTable of target list of sharepoint
            DataTable dt = GetDataTable("Navigation", "", "Sequence ASC");
            //I Check empty
            if (!DataTableIsNullOrEmpty(dt))
            {
                //I Maybe forming DataTable to target column
                dt = dt.Copy().DefaultView.ToTable(true, new string[] { "NodeID", "ParentNodeID", "Title", "Url", "Level",
                "AudienceName","Sequence","AudienceID"});
            }
            //I Return result
            return dt;
            #endregion

        }
        //I Method for validate and get current user name to log on
        public string GetLogOnName()
        {
            string logon = string.Empty;
            //I Check current user name
            if (string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) { return string.Empty; }
            //I Maybe split current user name
            string[] user = HttpContext.Current.User.Identity.Name.Split('\\');
            //I Validate user name to splited
            if (user.Length != 2) { return string.Empty; }
            //I Get log on user name
            logon = user[1];
            //I Return data [maybe user name]
            return logon;
        }
        //I Get DataTable information of target list (folder in sharepoint)
        private DataTable GetDataTable(string ListName, string Condition, string Sort)
        {
            try
            {
                SPSite oSPsite;
                string SharedPointSite = SPContext.Current.Site.Url;
                DataTable dt = new DataTable();
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    //I New sharepoint site object by current sharepoint site url
                    using (oSPsite = new SPSite(SharedPointSite))
                    {
                        //I New object sharepoint web by method open web
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {
                            //I Unknow
                            oSPWeb.AllowUnsafeUpdates = true;
                            //I Get List information by target list name (folder in sharepoint)
                            SPList oList = oSPWeb.Lists[ListName];
                            //I New object of query for get target data from target list of sharepoint
                            //I Unused
                            SPQuery myquery = new SPQuery();
                            //I Maybe class of data type for get data from target list of sharepoint
                            SPListItemCollection ListSPListItem = oList.GetItems();
                            //I Maybe get item of target list to DataTable
                            dt = ListSPListItem.GetDataTable();
                            //I Check dataTable to null or empty
                            if (!DataTableIsNullOrEmpty(dt))
                            {
                                //I Maybe used for dummy dataTable for convert data in dataTable
                                DataView dv = new DataView();
                                //I Current condition to be "" (empty)
                                if (!string.IsNullOrEmpty(Condition.Trim()))
                                {
                                    //I Incase condition valid
                                    //I Unknow
                                    dv = dt.Copy().DefaultView;
                                    dv.RowFilter = Condition;
                                    dt = dv.ToTable();
                                }
                                //I Check sorting
                                if (!string.IsNullOrEmpty(Sort.Trim()))
                                {
                                    //I Incase sorting valid
                                    dv = dt.Copy().DefaultView;
                                    dv.Sort = Sort;
                                    dt = dv.ToTable();
                                }
                            }
                            //I Unknow
                            oSPWeb.AllowUnsafeUpdates = false;
                        }
                    }
                });
                //I Return dataTable
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //I Method for check null or empty of dataTable
        private bool DataTableIsNullOrEmpty(DataTable dt)
        {
            if (dt == null) { return true; }
            if (dt.Rows.Count == 0) { return true; }
            return false;
        }
        //I Method set data to gotted to target MenuItem (Recursive)
        private void procMenuRender(DataTable dt, MenuItemCollection itms, string filter)
        {
            string sUlTag = "";
            string sUrlMobile = "";
            string sTarget = "";
            //I Split target DataTable to DataRow by filter string
            DataRow[] drs = dt.Select(filter, "");
            foreach (DataRow dr in drs)
            {
                //I New object of MenuItem for binding data to control
                MenuItem itm = new MenuItem();
                //I Assign url in current row

                sElementOfMobile = (sElementOfMobile + "<li class=\"dropdown\" style=\"list-style:none;\">");

                bool bCheckUrlNoneLink = (dr["Url"] == null) ? true : false;
                if (!bCheckUrlNoneLink)
                {
                    if (dr["Url"].ToString().Trim().Equals(""))
                    {
                        bCheckUrlNoneLink = true;
                    }
                    if (dr["Url"].ToString().Trim().Equals("-"))
                    {
                        bCheckUrlNoneLink = true;
                    }
                }
                if (bCheckUrlNoneLink)
                {
                    //I itm.NavigateUrl = SharedBusinessRule.GetConfigInterface("HomeDisplay",this.Page);
                    //I itm.Target = "_blank";
                    itm.NavigateUrl = "javascript:void(0);";
                    sUrlMobile = "javascript:void(0);";
                    sTarget = "";
                }
                else
                {
                    itm.NavigateUrl = Convert.ToString(dr["Url"]);
                    sUrlMobile = itm.NavigateUrl;
                    sTarget = " target=\"_blank\" ";
                }

                string[] ArrTitle = dr["Title"].ToString().Split('|');
                string sLng = sDefaultLang;
                string sTitleByLang = "";
                if (ArrTitle.Length > 1)
                {
                    if (sLng == "EN")
                    {
                        sTitleByLang = ArrTitle[0];
                    }
                    else
                    {
                        sTitleByLang = ArrTitle[1];
                    }
                }
                else
                {
                    sTitleByLang = ArrTitle[0];
                }

                #region  " Test link or header "
                DataRow[] drTest = dt.Select("ParentNodeID = '" + dr["NodeID"] +
                    "' AND ISNULL(ParentNodeID,'') <> ''", "");
                if (drTest != null)
                {
                    if (drTest.Length > 0)
                    {
                        //I Header
                        sElementOfMobile = (sElementOfMobile + "<a class=\"dropdown\" href=\"javascript:void(0);\""
                            + "onclick=\"ftToggleMenu(this);\">" +
                            ((sTitleByLang == "") ? "None" : sTitleByLang)
                            + " <span class=\"dropdown\">"
                            + "<span class=\"caret\"></span></span></a><ul class=\"dropdown\" style=\"display:none;\">");
                        sUlTag = "</ul>";
                    }
                    else
                    {
                        //I None head
                        sElementOfMobile = (sElementOfMobile + "<a class=\"dropdown\" href=\"" + sUrlMobile + "\""
                            + sTarget + ">" + ((sTitleByLang == "") ? "None" : sTitleByLang) + "</a>");
                        sUlTag = "";
                    }
                }
                else
                {
                    //I None head
                    sElementOfMobile = (sElementOfMobile + "<a class=\"dropdown\" href=\"" + sUrlMobile + "\""
                        + sTarget + ">" + sTitleByLang + "</a>");
                    sUlTag = "";
                }
                #endregion Test link or header --- End

                //I Assign text by title in current row

                itm.Text = sTitleByLang;
                //I Assign value by NodeID in current row
                itm.Value = dr["NodeID"].ToString();
                //I Add dummy data of MenuItem to target MenuItem control
                itms.Add(itm);
                //I Get number of level by field Level
                int Level = int.Parse(dr["Level"].ToString()) + 1;
                //I Set string filter by "Level" and "ParentNodeID" for Recursive
                filter = "Level = '" + Level + "' AND ParentNodeID = '" + dr["NodeID"] + "'";
                //I Recursive call
                procMenuRender(dt, itm.ChildItems, filter);

                sElementOfMobile = (sElementOfMobile + sUlTag + "</li>");
            }
        }

        #region  " Check permission method "
        //I Method for check current user to live in  "SPIMPortal Owners"
        private void CheckUserOwner()
        {
            try
            {
                bGroupOwner = false;
                string currentUser = GetLogOnName();
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                    {
                        SPGroup oGroup = SPContext.Current.Web.Groups["SPIMPortal Owners"];
                        foreach (SPUser user in oGroup.Users)
                        {
                            string[] users = user.LoginName.Split('\\');
                            if (users.Length == 2)
                            {
                                if (users[1].Trim().ToLower() != "system")
                                {
                                    if (users[1] == currentUser)
                                    {
                                        bGroupOwner = true;
                                    }
                                }
                            }
                        }
                    }
                });
                return;
            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
                bGroupOwner = false;
            }
        }
        //I Delete tree in dataTable with NodeID
        private DataTable DeleteTree(DataTable argTableIncome, string argNodeID)
        {
            try
            {
                #region  " Delete target node and check valid data "
                DataTable dtReturn = argTableIncome.SelectToTable(string.Format("NodeID <> {0}", argNodeID));
                if (dtReturn == null)
                {
                    //I Incase error or empty
                    return null;
                }
                if (dtReturn.Rows.Count == 0)
                {
                    //I Incase empty
                    return null;
                }
                #endregion Delete target node and check valid data --- End

                #region  " Get child node with NodeID and check valid data "
                DataTable dtDel = dtReturn.SelectToTable(string.Format("ParentNodeID = '{0}'", argNodeID));
                if (dtDel == null)
                {
                    //I Incase none child
                    return dtReturn;
                }
                if (dtDel.Rows.Count == 0)
                {
                    //I Incase none child
                    return dtReturn;
                }
                #endregion Get child node with NodeID and check valid data --- End

                #region  " Loop for delete tree by NodeID [Recursive] "
                for (int i1 = 0; i1 < dtDel.Rows.Count; i1++)
                {
                    dtReturn = DeleteTree(dtReturn, dtDel.Rows[i1]["NodeID"].ToString());
                }
                #endregion Loop for delete tree by NodeID [Recursive] --- End

                #region  " Return data "
                //I Return data to deleted
                return dtReturn;
                #endregion Return data --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
            }
            //I Incase error
            return null;
        }
        //I Get all group in sharepoint
        private DataTable GetAllGroup()
        {
            DataTable dt = new DataTable();
            SPSite oSPsite;
            SPWeb oSPWeb;
            try
            {
                dt.Columns.Add("Name");
                dt.Columns.Add("ID");
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (oSPsite = new SPSite(SPContext.Current.Site.Url))
                    {
                        using (oSPWeb = oSPsite.OpenWeb())
                        {
                            foreach (SPGroup group in oSPWeb.Groups)
                            {
                                if (group.Name != "Style Resource Readers")
                                {
                                    dt.Rows.Add(group.Name.ToString(), group.ID.ToString());
                                }
                            }
                        }
                    }
                });
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //I Get all group from user name
        private DataTable GetGroupByUserNameAndDomainName()
        {
            DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add("Name");
            dtReturn.Columns.Add("ID");
            try
            {

                #region  " Get and validate data "

                string Uname = GetLogOnName();
                string DomainName = GetDomainName();

                #region  " Validate current domain and user name "
                if (Uname == null)
                {
                    //I Error
                    return null;
                }
                if (Uname.Trim().Equals(""))
                {
                    //I Error
                    return null;
                }
                if (DomainName == null)
                {
                    //I Error
                    return null;
                }
                #endregion Validate current domain and user name --- End

                DataTable dtAllGroup = GetAllGroup();
                if (dtAllGroup == null)
                {
                    //I Error
                    return null;
                }
                if (dtAllGroup.Rows.Count < 1)
                {
                    //I Error
                    return null;
                }

                #endregion Get and validate data --- End

                #region  " Get group from current user name "
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                    {

                        #region  " For by all group in sharepoint "
                        for (int i1 = 0; i1 < dtAllGroup.Rows.Count; i1++)
                        {
                            SPGroup oGroup = SPContext.Current.Web.Groups[dtAllGroup.Rows[i1]["Name"].ToString()];
                            if (dtAllGroup.Rows[i1]["Name"].ToString().Contains("spimportal visitors"))
                            {
                                SPUser user = oGroup.Users[0];
                                string str = user.Name;
                            }
                            foreach (SPUser user in oGroup.Users)
                            {

                                if (user.Name.ToUpper().Contains("DOMAIN USERS"))
                                {
                                    string[] users = user.Name.Split('\\');
                                    if (users.Length == 2)
                                    {
                                        if (users != null)
                                        {
                                            if (!users[0].Trim().Equals(""))
                                            {
                                                if (users[0].Trim().ToUpper().Equals(DomainName.ToUpper().Trim()))
                                                {
                                                    dtReturn.Rows.Add(dtAllGroup.Rows[i1]["Name"].ToString(),
                                                        dtAllGroup.Rows[i1]["ID"].ToString());
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string[] users = user.LoginName.Split('\\');
                                    if (users.Length == 2)
                                    {
                                        if (users[1].Trim().ToLower() != "system")
                                        {
                                            if (users[1].ToString().ToUpper().Equals(Uname.ToUpper()))
                                            {
                                                dtReturn.Rows.Add(dtAllGroup.Rows[i1]["Name"].ToString(),
                                                    dtAllGroup.Rows[i1]["ID"].ToString());
                                                break;
                                            }
                                        }
                                    }
                                }

                            }

                        }
                        #endregion For by all group in sharepoint --- End

                    }
                });
                #endregion Get group from current user name --- End

                return dtReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //I Method for get domain of current user
        public string GetDomainName()
        {
            string domain = string.Empty;
            //I Check current user name
            if (string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) { return string.Empty; }
            //I Maybe split current user name
            string[] user = HttpContext.Current.User.Identity.Name.Split('\\');
            //I Validate user name to splited
            if (user.Length != 2) { return string.Empty; }

            if (user[0] == null) { return string.Empty; }
            if (user[0].Trim().Equals("")) { return string.Empty; }
            //I Split for only domain
            if (user[0].IndexOf("|") != -1)
            {
                string[] str = user[0].Split('|');
                if (str == null) { return string.Empty; }
                if (str.Length != 2) { return string.Empty; }
                if (str[1] == null) { return string.Empty; }
                if (str[1].Trim().Equals("")) { return string.Empty; }
                user[0] = str[1];
            }

            //I Get current domain name
            domain = user[0];
            //I Return data current domain name
            return domain;
        }
        #endregion Check permission method --- End

        protected void btnLang_Click(object sender, EventArgs e)
        {
            SetDefaultLang(SPContext.Current.Web.CurrentUser.LoginName);
            SetCssButtonByLang();
            PopulateMenu();
        }

        private void setK2WorkCount()
        {

            string _ConnectionString = "";
            string logonName = SharedRules.LogonName();

            string currentUserID = SharedRules.FindUserID(logonName, this.Page);
            if (string.IsNullOrEmpty(currentUserID))
            {
                currentUserID = Request.QueryString["USERID"] != null
                    ? Request.QueryString["USERID"].ToString()
                    : "5050108";
            }

            DataTable dtDb = Extension.GetDataTable("SiteSetting");
            {
                if (!dtDb.DataTableIsNullOrEmpty())
                {
                    _ConnectionString = dtDb.Rows[0]["Value"].ToString();
                }
            }
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(_ConnectionString);
            List<TRNDocument> listDocument = db.TRNDocuments.Where(x => x.WaitingFor.Contains(currentUserID)).ToList();
            string counter = (listDocument.Count() > 999 ? 999 : listDocument.Count()).ToString();
            lbl_WorklistCounter.Text = counter;
        }

        protected void btnK2Task_Click(object sender, EventArgs e)
        {
            Extension.Redirect(Page, "Worklist.aspx");
        }

    }
}
