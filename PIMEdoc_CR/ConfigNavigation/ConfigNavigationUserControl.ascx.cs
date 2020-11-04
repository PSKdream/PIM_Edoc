using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;


namespace PIMEdoc_CR.ConfigNavigation
{
    public partial class ConfigNavigationUserControl : UserControl
    {
        private static string ListNavigation = "Navigation";
        private static string[] sArColumnNavigation = new string[] {
            "NodeID", "ParentNodeID", "Title", "Url", "Level", "AudienceName","Sequence","Head"};
        public class MST_Menu_Entity
        {
            public struct FieldName
            {
                public const String MenuOrder = "MenuOrder";
                public const String DisplayName = "DisplayName";
                public const String SubURL = "SubURL";
                public const String PageName = "PageName";
                public const String IsHeader = "IsHeader";
                public const String Audience = "Audience";
                public const String IsActive = "IsActive";
            }
            public int MenuOrder { get; set; }
            public String DisplayName { get; set; }
            public String SubURL { get; set; }
            public String PageName { get; set; }
            public Boolean IsHeader { get; set; }
            public String Audience { get; set; }
            public Boolean IsActive { get; set; }
        }
        public static string SiteUrl = SPContext.Current.Site.Url;
        public bool IsHeader
        {
            get
            {
                if (ViewState["IsHeader"] == null)
                {
                    return false;
                }
                return (bool)ViewState["IsHeader"];
            }
            set
            {
                ViewState["IsHeader"] = value;
            }
        }
        public bool IsDelete
        {
            get
            {
                if (ViewState["IsDelete"] == null)
                {
                    return false;
                }
                return (bool)ViewState["IsDelete"];
            }
            set
            {
                ViewState["IsDelete"] = value;
            }
        }
        public string vsNodeID
        {
            get
            {
                if (ViewState["vsNodeID"] == null) { return string.Empty; }
                return ViewState["vsNodeID"].ToString();
            }
            set
            {
                ViewState["vsNodeID"] = value;
            }
        }
        [Serializable]
        public class DeleteByID
        {
            public int iID { get; set; }
        }
        public List<DeleteByID> oListID
        {
            get
            {
                if (ViewState["oListID"] == null)
                {
                    ViewState["oListID"] = new List<DeleteByID>();
                }
                return (List<DeleteByID>)ViewState["oListID"];
            }
            set
            {
                this.ViewState["oListID"] = value;
            }
        }
        private bool _overLevel = false;
        //I Property for check user to live in
        private bool bGroupOwner;
        //I Property for ParentNodeID of current treeview node
        private string sParentNodeID = "";
        //I Level number used for Move up and down node of treeview
        private int iCurrentLevel = -1;


        

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {

                lblTitle.Text = "";
                lblUrl.Text = "";
                lblDescript.Text = "";
                hdfNodeValue.Value = "";
                hdfNodeIdForAddHeader.Value = "";
                DataTable dt = GetDataNavigation();
                if (!DataTableIsNullOrEmpty(dt))
                {
                    DataTable dtTree = DelDupRowByNodeId(dt, sArColumnNavigation, "NodeID");
                    string filter = "Level = 1 AND ISNULL(ParentNodeID,'') = ''";
                    procMenuRender(dtTree, TreeView1.Nodes[0].ChildNodes, filter);
                    recursiveSetImageTreeView(TreeView1.Nodes[0]);

                    #region  " Check Permission for Menu "
                    /*
                    CheckUserOwner();
                    dt = dt.SelectToTable("Level < 4");
                    if (bGroupOwner)
                    {
                        //I In case current user live in "SPIMPortal Owners"
                        dt = dt.SelectToTable("AudienceName = '' OR AudienceName is null OR AudienceName = 'SPIMPortal Visitors' OR AudienceName = 'SPIMPortal Owners'");
                    }
                    else
                    {
                        //I In case current user none live in "SPIMPortal Owners"
                        //I Get all node with under level
                        //I Get the node to contain : "SPIMPortal Owners"
                        DataTable tmpDt = dt.SelectToTable("AudienceName = 'SPIMPortal Owners'");
                        //I Check valid data
                        if (tmpDt != null)
                        {
                            //I Incase valid
                            for (int i1 = 0; i1 < tmpDt.Rows.Count; i1++)
                            {
                                dt = DeleteTree(dt, tmpDt.Rows[i1]["NodeID"].ToString());
                            }
                        }
                    }
                    dt = dt.Sort("Sequence ASC");
                    */
                    #endregion Check Permission for Menu --- End
                    //  TODO: 01 - Fix for view menu.
                    //I procMenuRender(dt, Menu1.Items, filter);
                }
            }

            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Error", "setHeaderImg();", true);
            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Error", "alert('test');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Alert", "DelTagTd();", true);
        }

        private DataTable DelDupRowByNodeId(DataTable argDt, string[] argColName, string argSortCol)
        {
            try
            {

                #region  " New DataTable "
                DataTable dtReturn = new DataTable();
                for (int i1 = 0; i1 < argColName.Count(); i1++)
                {
                    dtReturn.Columns.Add(argColName[i1]);
                }
                #endregion New DataTable --- End

                #region  " Delete duplicate "
                for (int i1 = 0; i1 < argDt.Rows.Count; i1++)
                {
                    DataRow dr = argDt.Rows[i1];
                    DataRow drTmp = dtReturn.NewRow();
                    for (int i2 = 0; i2 < argColName.Count(); i2++)
                    {
                        drTmp[argColName[i2]] = dr[argColName[i2]];
                    }
                    if (i1 == 0)
                    {
                        dtReturn.Rows.Add(drTmp);
                        continue;
                    }
                    DataRow[] drArr = dtReturn.Select(argSortCol + " = '" + dr[argSortCol].ToString() + "'");
                    if (drArr.Count() > 0)
                    {
                        continue;
                    }
                    dtReturn.Rows.Add(drTmp);
                }
                #endregion Delete duplicate --- End

                return dtReturn;
            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
                return null;
            }
        }

        private DataTable GetDataNavigation()
        {
            SPSite oSPsite;
            DataTable dt_group = new DataTable();
            dt_group.Columns.Add("GroupID");
            dt_group.Columns.Add("GroupName");

            string SharedPointSite = SPContext.Current.Site.Url;
           // PIMClass.TCBClass.LogWriter.Write(SharedPointSite, Page.Request.PhysicalPath);
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (oSPsite = new SPSite(SharedPointSite))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        for (int i = 0; i < oSPWeb.Groups.Count; i++)
                        {
                            SPGroup Group = oSPWeb.Groups[i];
                            dt_group.Rows.Add(Group.ID, Group.Name);
                        }
                    }
                }
            });

            string filter = string.Empty;
            foreach (DataRow dr_group in dt_group.Rows)
            {
                if (IsMemberOfGroup(dr_group["GroupName"].ToString(), GetLogOnName()))
                {
                    filter += string.Format("'{0}',", dr_group["GroupID"].ToString());
                }
            }

            if (!string.IsNullOrEmpty(filter)) { filter = filter.Remove(filter.Length - 1, 1); }

            DataTable dt = GetDataTable(ListNavigation, "", "Sequence ASC");

            if (!DataTableIsNullOrEmpty(dt))
            {
                dt = dt.Copy().DefaultView.ToTable(true, sArColumnNavigation);
            }
            return dt;
        }

        public string GetLogOnName()
        {
            string logon = string.Empty;
            if (string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) { return string.Empty; }
            string[] user = HttpContext.Current.User.Identity.Name.Split('\\');
            if (user.Length != 2) { return string.Empty; }
            logon = user[1];
            return logon;
        }
        private bool IsMemberOfGroup(string GroupName, string LogOnName)
        {
            bool b = false;
            string s_user = string.Empty;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                {
                    SPGroup oGroup = SPContext.Current.Web.Groups[GroupName];
                    foreach (SPUser user in oGroup.Users)
                    {
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


            return b;
        }
        private DataTable GetDataTable(string ListName, string Condition, string Sort)
        {
            try
            {
                SPSite oSPsite;
                string SharedPointSite = SPContext.Current.Site.Url;
                DataTable dt = new DataTable();
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (oSPsite = new SPSite(SharedPointSite))
                    {
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {
                            oSPWeb.AllowUnsafeUpdates = true;
                            SPList oList = oSPWeb.Lists[ListName];
                            SPQuery myquery = new SPQuery();
                            SPListItemCollection ListSPListItem = oList.GetItems();
                            dt = ListSPListItem.GetDataTable();

                            if (!DataTableIsNullOrEmpty(dt))
                            {
                                DataView dv = new DataView();
                                if (!string.IsNullOrEmpty(Condition.Trim()))
                                {
                                    dv = dt.Copy().DefaultView;
                                    dv.RowFilter = Condition;
                                    dt = dv.ToTable();
                                }
                                if (!string.IsNullOrEmpty(Sort.Trim()))
                                {
                                    dv = dt.Copy().DefaultView;
                                    dv.Sort = Sort;
                                    dt = dv.ToTable();
                                }
                            }

                            oSPWeb.AllowUnsafeUpdates = false;
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
        private bool DataTableIsNullOrEmpty(DataTable dt)
        {
            if (dt == null) { return true; }
            if (dt.Rows.Count == 0) { return true; }
            return false;
        }
        private void procMenuRender(DataTable dt, TreeNodeCollection itms, string filter)
        {
            //  TODO: 1.1 - Test set icon in treeview
            #region  " Test area "
            string s = "";
            if (Request.QueryString["test"] != null)
            {
                if (!Request.QueryString["test"].ToString().Equals(""))
                {
                    s = Request.QueryString["test"].ToString();
                }
            }
            #endregion Test area --- End
            DataRow[] drs = dt.Select(filter, "");
            foreach (DataRow dr in drs)
            {
                TreeNode itm = new TreeNode();
                itm.Text = dr["Title"].ToString();
                itm.Value = dr["NodeID"].ToString();
                //  TODO: 1.2 - Test set icon in treeview
                #region  " Test area "
                if (s != "")
                {
                    itm.ImageToolTip = s;
                }
                else
                {
                    itm.ImageToolTip = dr["Head"].ToString();
                }
                #endregion Test area --- End
                //itm.ImageUrl = "../Img/treeFold.png";
                //itm.ImageToolTip = dr["Url"].ToString();
                //I Set for image of level 1 (Header)
                itm.ToolTip = dr["Level"].ToString();
                itms.Add(itm);

                int Level = int.Parse(dr["Level"].ToString()) + 1;
                filter = "Level = '" + Level + "' AND ParentNodeID = '" + dr["NodeID"] + "'";
                procMenuRender(dt, itm.ChildNodes, filter);
            }
        }
        private void procMenuRender(DataTable dt, MenuItemCollection itms, string filter)
        {
            DataRow[] drs = dt.Select(filter, "");
            foreach (DataRow dr in drs)
            {
                MenuItem itm = new MenuItem();
                //I itm.NavigateUrl = Convert.ToString(dr["Url"]);

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
                }
                else
                {
                    if (dr["Url"].ToString().Trim().Equals("@OP"))
                    {
                        //bCheckUrlNoneLink = true;
                       // itm.NavigateUrl = SharedBusinessRule.GetConfigInterface("HomeDisplay", this.Page);
                        itm.Target = "_blank";
                    }
                    else
                    {
                        itm.NavigateUrl = Convert.ToString(dr["Url"]);
                    }
                }

                itm.Text = "<span class=\"submenu-button\"></span>" + dr["Title"].ToString() + "<b class=\"caret\"></b>";
                itm.Value = dr["NodeID"].ToString();
                itms.Add(itm);

                int Level = int.Parse(dr["Level"].ToString()) + 1;
                filter = "Level = '" + Level + "' AND ParentNodeID = '" + dr["NodeID"] + "'";
                procMenuRender(dt, itm.ChildItems, filter);
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {

            if (oListID != null)
            {
                if (oListID.Count > 0)
                {
                    oListID.Clear();
                }
            }
            oListID = new List<DeleteByID>();

            IsHeaderOrNot();
            if (!string.IsNullOrEmpty(vsNodeID))
            {
                if (vsNodeID != "0000")
                {
                    using (SPSite oSite = new SPSite(SiteUrl))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            SPList list = oWeb.Lists[ListNavigation];
                            SPListItemCollection oList = GetDataFromDocLib(ListNavigation, @"<Where><Eq><FieldRef Name='NodeID' /><Value Type='Text'>" + vsNodeID + @"</Value></Eq></Where>");
                            if (oList != null)
                            {
                                if (oList.Count > 0)
                                {
                                    foreach (SPListItem item in oList)
                                    {
                                        DeleteByID NewList = new DeleteByID();
                                        NewList.iID = item.ID;
                                        oListID.Add(NewList);
                                    }
                                }
                            }
                        }
                    }

                    #region  " Delete node and update sequence "
                    if (!IsHeader)
                    {
                        if (vsNodeID.Trim().Equals(""))
                        {
                            //I Some thing wrong
                            Alert("Delete incomplate", "");
                            return;
                        }
                        GetParentNodeID();
                        if (sParentNodeID.Equals(""))
                        {
                            //I Some thing wrong
                            Alert("Delete incomplate", "");
                            return;
                        }
                    }
                    DeleteItemByID(oListID);
                    if (IsDelete)
                    {
                        if (IsHeader)
                        {
                            ReOrderSequenceHead();
                        }
                        else
                        {
                            ReOrderSequenceChild(sParentNodeID);
                        }

                        #region  " Delete child node of current node "
                        if (!vsNodeID.Trim().Equals("0000"))
                        {
                            string str = GetAllIdWithParentNodeIdByNodeID(vsNodeID);
                            if (!str.Trim().Equals(""))
                            {
                                str = str.Remove(0, 1);
                                string[] arNodeID = str.Split(',');
                                if (arNodeID == null)
                                {
                                    //I Incase error
                                    Alert("Delete incomplate", "");
                                    return;
                                }
                                if (arNodeID.Count() < 1)
                                {
                                    //I Incase error
                                    Alert("Delete incomplate", "");
                                    return;
                                }
                                List<DeleteByID> lDel = new List<DeleteByID>();
                                for (int i2 = 0; i2 < arNodeID.Count(); i2++)
                                {
                                    lDel.Add(new DeleteByID { iID = Convert.ToInt32(arNodeID[i2]) });
                                }
                                IsDelete = false;
                                DeleteItemByID(lDel);
                                if (!IsDelete)
                                {
                                    //I Incase error
                                    Alert("Delete incomplate", "");
                                    return;
                                }
                            }
                        }
                        #endregion Delete child node of current node --- End

                        Alert("Delete complate", "");
                        Render();
                    }
                    #endregion Delete node and update sequence --- End

                }
                else
                {
                    Alert("Can not delete the root", "");
                }
            }
        }
        public SPListItemCollection GetDataFromDocLib(string FolderName, string Query)
        {
            using (SPSite oSite = new SPSite(SiteUrl))
            {
                using (SPWeb oWeb = oSite.OpenWeb())
                {
                    SPList oList = oWeb.Lists[FolderName];
                    SPQuery que = new SPQuery();
                    que.Query = Query;
                    SPListItemCollection ColList = oList.GetItems(que);
                    return ColList;
                }
            }
        }
        protected void TextBoxClick_TextChanged(object sender, EventArgs e)
        {

        }
        public void Alert(String Message, string sURLTask = "")
        {

            string sScriptRedirectURL = "";
            if (sURLTask != "")
            {
                sScriptRedirectURL = string.Format("window.location.href = '{0}'", sURLTask);
            }

            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Alert",
                String.Format("alert('{0}');{1};", Message, sScriptRedirectURL), true);

        }
        public void DeleteItemByID(List<DeleteByID> oList)
        {
            using (SPSite oSite = new SPSite(SiteUrl))
            {
                using (SPWeb oWeb = oSite.OpenWeb())
                {
                    SPList list = oWeb.Lists[ListNavigation];
                    foreach (var itemID in oList)
                    {
                        int iItemID = Convert.ToInt16(itemID.iID.ToString());
                        list.Items.DeleteItemById(iItemID);
                        IsDelete = true;
                    }
                }
            }
        }
        public void IsHeaderOrNot()
        {
            //string[] sSeparators = new string[] { "\\\\" };
            char[] cSeparator = new char[] { '/' };
            //string[] sNodeIDWithoutBackSlash = null;
            string[] sNodeIDWithoutSlash = null;
            //I Change statement for check header by pattern "0000/nnnn".
            string[] sCheck = hdfNodeValue.Value.Split(cSeparator, StringSplitOptions.None);
            int iCheck = 0;
            if (sCheck != null)
            {
                iCheck = sCheck.Count();
            }
            if (iCheck > 2)
            {
                sNodeIDWithoutSlash = hdfNodeValue.Value.ToString().Split(cSeparator, StringSplitOptions.None);
                if (sNodeIDWithoutSlash.Length > 0)
                {
                    vsNodeID = sNodeIDWithoutSlash[sNodeIDWithoutSlash.Length - 1];
                    IsHeader = false;
                }
            }
            else
            {
                if (iCheck == 2)
                {
                    iCheck = 1;
                }
                else
                {
                    iCheck = 0;
                }
                if (string.IsNullOrEmpty(sCheck[iCheck]))
                {
                    vsNodeID = "0000";
                    IsHeader = true;
                }
                else
                {
                    vsNodeID = sCheck[iCheck];
                    IsHeader = true;
                }
            }
        }
        protected void btnMoveUp_Click(object sender, EventArgs e)
        {
            IsHeaderOrNot();
            if (IsHeader)
            {
                HeaderMoveUp();
                return;
            }
            IsOverLevel();
            if (_overLevel)
            {
                //I In case error. because over 3 level.
                return;
            }
            int NodeLevel = 0;
            int NodeSequence = 0;
            int NewNodeSequence = 0;
            int NodeBefore = 0;
            string sNewParentNodeID = "";
            DataTable drRowTop = new DataTable();
            SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
            DataTable dt = oListNavigation.GetDataTable();
            if (oListNavigation != null)
            {
                if (oListNavigation.Count > 0)
                {
                    //I Get target row for move up
                    DataRow[] drCurrentRow = dt.Select(string.Format("NodeID = '{0}'", vsNodeID));
                    //I Get level of target node
                    NodeLevel = Convert.ToInt16(drCurrentRow[0]["Level"].ToString());
                    //I Get current sequence of target node
                    NodeSequence = Convert.ToInt16(drCurrentRow[0]["Sequence"].ToString());
                    //I Get list item site of target node
                    SPListItem sItemCurrent = oListNavigation.GetItemById(Convert.ToInt16(drCurrentRow[0]["ID"].ToString()));
                    //I Set sequence of target node
                    //sItemCurrent["Sequence"] = NodeSequence - 1;
                    //I Set default parent nodeId
                    sNewParentNodeID = drCurrentRow[0]["ParentNodeID"].ToString();
                    //I Incase target node to be top child
                    if (NodeSequence == 1)
                    {
                        NewNodeSequence = 1;
                        #region  " Old statement "
                        /*
                        NodeBefore = Convert.ToInt16(drCurrentRow[0]["ParentNodeID"].ToString());
                        DataTable dtSequenceOfParentNode = dt.SelectToTable(
                            string.Format("NodeID = {0}", NodeBefore.ToString()));
                        #region  " Incase top of the top "
                        if (dtSequenceOfParentNode.Rows[0]["Sequence"].ToString() == "1")
                        {
                            sItemCurrent["Sequence"] = 1;
                            NewNodeSequence = 1;
                            sNewParentNodeID = drCurrentRow[0]["ParentNodeID"].ToString();
                            return;
                        }
                        #endregion Incase top of the top --- End
                        else
                        {
                            #region  " Incase none top group "

                            #region  " Check for move group "
                            DataTable dtParentNodeSequence = null;
                            if (iCurrentLevel < 3)
                            {
                                //I Incase move child node.
                                dtParentNodeSequence =
                                  dt.SelectToTable(string.Format
                                  ("NodeID <> {0} AND Sequence < {1} AND Level = {2}"
                                  , NodeBefore
                                  , dtSequenceOfParentNode.Rows[0]["Sequence"].ToString()
                                  , dtSequenceOfParentNode.Rows[0]["Level"].ToString()));

                            }
                            else
                            {
                                //I Incase move last level node.
                                //I Move to new group.
                                //I You must query with NodeID of Header node.
                                dtParentNodeSequence =
                                  dt.SelectToTable(string.Format
                                  ("NodeID <> {0} AND Sequence < {1} AND Level = {2} AND ParentNodeID = {3}"
                                  , NodeBefore
                                  , dtSequenceOfParentNode.Rows[0]["Sequence"].ToString()
                                  , dtSequenceOfParentNode.Rows[0]["Level"].ToString()
                                  , dtSequenceOfParentNode.Rows[0]["ParentNodeID"].ToString()));

                            }
                            dtParentNodeSequence = dtParentNodeSequence.Sort("Sequence DESC");
                            #endregion Check for move group --- End

                            if (!DataTableIsNullOrEmpty(dtParentNodeSequence))
                            {
                                dtParentNodeSequence = dtParentNodeSequence.Sort("Sequence DESC");
                                DataTable dtSequenceChild = dt.SelectToTable(string.Format(
                                    "ParentNodeID = {0}"
                                    , dtParentNodeSequence.Rows[0]["NodeID"]));
                                if (DataTableIsNullOrEmpty(dtSequenceChild))
                                {
                                    sItemCurrent["Sequence"] = 1;
                                    NewNodeSequence = 1;
                                }
                                else
                                {
                                    dtSequenceChild = dtSequenceChild.Sort("Sequence DESC");
                                    sItemCurrent["Sequence"] =
                                        Convert.ToInt16(dtSequenceChild.Rows[0]["Sequence"].ToString()) + 1;
                                    NewNodeSequence = Convert.ToInt16(dtSequenceChild.Rows[0]["Sequence"].ToString()) + 1;
                                }
                                sItemCurrent["ParentNodeID"] =
                                    Convert.ToInt16(dtParentNodeSequence.Rows[0]["NodeID"].ToString());
                                sNewParentNodeID = dtParentNodeSequence.Rows[0]["NodeID"].ToString();
                            }
                            else
                            {
                                //I Something error
                                sItemCurrent["ParentNodeID"] = NodeBefore;
                                sItemCurrent["Sequence"] = 1;
                                NewNodeSequence = 1;
                                sNewParentNodeID = NodeBefore.ToString();
                            }
                            #endregion Incase none top group --- End
                        }
                        */
                        #endregion Old statement --- End

                    }
                    else
                    {
                        NewNodeSequence = NodeSequence - 1;
                    }

                    #region  " Update target node "
                    if (drCurrentRow.Count() == 1)
                    {
                        sItemCurrent["Sequence"] = NewNodeSequence;
                        sItemCurrent.Update();
                    }
                    else
                    {
                        //I Multi row
                        for (int i1 = 0; i1 < drCurrentRow.Count(); i1++)
                        {
                            SPListItem item =
                                oListNavigation.GetItemById(Convert.ToInt16(
                                drCurrentRow[i1]["ID"].ToString()));
                            item["Sequence"] = NewNodeSequence;
                            //item["ParentNodeID"] = sNewParentNodeID;
                            item.Update();
                        }
                    }
                    #endregion Update target node --- End

                    // - Sequence
                    if (NodeSequence == 1)
                    {
                        //drRowTop = dt.SelectToTable(string.Format("ParentNodeID = '{0}'", NodeBefore));
                        drRowTop = null;
                    }
                    else
                    {
                        drRowTop = dt.SelectToTable(string.Format("Level = '{0}' AND Sequence = {1} AND ParentNodeID = '{2}'", NodeLevel, NodeSequence - 1, drCurrentRow[0]["ParentNodeID"].ToString()));
                    }

                    if (!DataTableIsNullOrEmpty(drRowTop))
                    {
                        foreach (DataRow item in drRowTop.Rows)
                        {
                            if (item["NodeID"].ToString() != vsNodeID)
                            {
                                SPListItem sItemTop = oListNavigation.GetItemById(Convert.ToInt16(item["ID"].ToString()));
                                if (NodeSequence == 1)
                                {
                                    sItemTop["Sequence"] = Convert.ToInt16(item["Sequence"].ToString()) - 1;
                                }
                                else
                                {
                                    sItemTop["Sequence"] = Convert.ToInt16(item["Sequence"].ToString()) + 1;
                                }

                                sItemTop.Update();
                            }
                        }
                    }

                }

            }
            Render();
            return;
        }
        protected void btnMoveDown_Click(object sender, EventArgs e)
        {
            IsHeaderOrNot();
            if (IsHeader)
            {
                HeaderMoveDown();
                return;
            }
            IsOverLevel();
            if (_overLevel)
            {
                //I In case error. because over 3 level.
                return;
            }
            int NodeLevel = 0;
            int NodeSequence = 0;
            int NodeBefore = 0;
            int NewNodeSequence = 0;
            string strNextParentNodeID = "-";
            string sNewParentNodeID = "";
            DataTable drRowTop = new DataTable();
            SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
            DataTable dt = oListNavigation.GetDataTable();
            if (oListNavigation != null)
            {
                if (oListNavigation.Count > 0)
                {
                    //I Get current node
                    DataRow[] drCurrentRow = dt.Select(string.Format("NodeID = '{0}'", vsNodeID));
                    //I Get current level
                    NodeLevel = Convert.ToInt16(drCurrentRow[0]["Level"].ToString());
                    //I Get current sequence
                    NodeSequence = Convert.ToInt16(drCurrentRow[0]["Sequence"].ToString());
                    //I Get current parent
                    sNewParentNodeID = drCurrentRow[0]["ParentNodeID"].ToString();
                    //I Get all node in current parent
                    DataTable dtMaxRow = dt.SelectToTable(string.Format("Level = '{0}' AND ParentNodeID = '{1}'", NodeLevel, drCurrentRow[0]["ParentNodeID"].ToString()));
                    strNextParentNodeID = drCurrentRow[0]["ParentNodeID"].ToString();
                    dtMaxRow = dtMaxRow.Sort("Sequence DESC");
                    bool bdtMaxRowEmpty = false;
                    if (dtMaxRow == null)
                    {
                        bdtMaxRowEmpty = true;
                    }
                    else
                    {
                        if (dtMaxRow.Rows.Count == 0)
                        {
                            bdtMaxRowEmpty = true;
                        }
                    }
                    if (!bdtMaxRowEmpty)
                    {
                        //I Sorting for bubble most value of Sequence in group
                        dtMaxRow = dtMaxRow.Sort("Sequence DESC");
                    }

                    SPListItem sItemCurrent = oListNavigation.GetItemById(Convert.ToInt16(drCurrentRow[0]["ID"].ToString()));
                    sItemCurrent["Sequence"] = NodeSequence + 1;
                    NewNodeSequence = NodeSequence + 1;
                    int iLastSeqNum = -1;
                    if (!bdtMaxRowEmpty) { iLastSeqNum = Convert.ToInt16(dtMaxRow.Rows[0]["Sequence"].ToString()); }
                    //I Incase last node in parent
                    if (NodeSequence >= iLastSeqNum)
                    {

                        NewNodeSequence = NodeSequence;
                        #region  " Old statement "
                        /*
                        NodeBefore = Convert.ToInt16(drCurrentRow[0]["ParentNodeID"].ToString());
                        DataTable dtParentNode = dt.SelectToTable(string.Format("NodeID = {0}", drCurrentRow[0]["ParentNodeID"].ToString()));
                        DataTable dtSequenceInLevelOfParentNode;
                        if (dtParentNode.Rows[0]["ParentNodeID"] == null)
                        {
                            dtSequenceInLevelOfParentNode = dt.SelectToTable
                                (string.Format("Level = {0} AND ISNULL(ParentNodeID,'') = ''", (NodeLevel - 1).ToString()));
                        }
                        else
                        {
                            if (dtParentNode.Rows[0]["ParentNodeID"].ToString().Equals(""))
                            {
                                dtSequenceInLevelOfParentNode = dt.SelectToTable
                                    (string.Format("Level = {0} AND ISNULL(ParentNodeID,'') = ''", (NodeLevel - 1).ToString()));
                            }
                            else
                            {
                                dtSequenceInLevelOfParentNode = dt.SelectToTable
                                    (string.Format("Level = {0} AND NodeID = '{1}'"
                                    , (NodeLevel - 2).ToString(), dtParentNode.Rows[0]["ParentNodeID"].ToString()));
                            }
                        }

                        //DataTable dtParentOfParentNode = dt.SelectToTable(string.Format("NodeID = '{1}'"
                        //, NodeLevel, dtParentNode.Rows[0]["ParentNodeID"].ToString()));
                        dtSequenceInLevelOfParentNode = dtSequenceInLevelOfParentNode.Sort("Sequence DESC");
                        if (dtSequenceInLevelOfParentNode.Rows[0]["Sequence"].ToString()
                            == (dtParentNode.Rows[0]["Sequence"]).ToString())
                        {
                            //I Incase sequence of parent node to be last sequence
                            //I in level of parent node.
                            //I and sequence of current node to be last sequence
                            //I in group.
                            sItemCurrent["Sequence"] = NodeSequence;
                            NewNodeSequence = NodeSequence;
                            return;
                        }
                        else
                        {
                            //I Incase sequence of parent node to not last sequence
                            //I in level of parent node.
                            //I but sequence of current node to be last sequence
                            //I in group.
                            //I Get next parent node for assign to current node
                            //I for update current node.

                            #region  " Check for move group "
                            DataTable dtNextParentNode = null;
                            if (iCurrentLevel < 3)
                            {
                                //I Incase child node.
                                dtNextParentNode =
                                  dt.SelectToTable(string.Format
                                  ("NodeID <> {0} AND Sequence > {1} AND Level = {2}"
                                  , NodeBefore
                                  , dtParentNode.Rows[0]["Sequence"].ToString()
                                  , dtParentNode.Rows[0]["Level"].ToString()));
                            }
                            else
                            {
                                //I Incase last level node.
                                dtNextParentNode =
                                  dt.SelectToTable(string.Format
                                  ("NodeID <> {0} AND Sequence > {1} AND Level = {2} AND ParentNodeID = {3}"
                                  , NodeBefore
                                  , dtParentNode.Rows[0]["Sequence"].ToString()
                                  , dtParentNode.Rows[0]["Level"].ToString()
                                  , dtParentNode.Rows[0]["ParentNodeID"].ToString()));
                            }
                            dtNextParentNode = dtNextParentNode.Sort("Sequence ASC");
                            #endregion Check for move group --- End

                            if (!DataTableIsNullOrEmpty(dtNextParentNode))
                            {
                                //I Sorting for bubble top
                                dtNextParentNode = dtNextParentNode.Sort("Sequence ASC");
                                sItemCurrent["Sequence"] = 1;
                                sItemCurrent["ParentNodeID"] =
                                    dtNextParentNode.Rows[0]["NodeID"];
                                NewNodeSequence = 1;
                                strNextParentNodeID = dtNextParentNode.Rows[0]["NodeID"].ToString();
                            }
                            else
                            {
                                //I Something error
                                sItemCurrent["Sequence"] = NodeSequence;
                                sItemCurrent["ParentNodeID"] = NodeBefore;
                                NewNodeSequence = NodeSequence;
                                strNextParentNodeID = NodeBefore.ToString();
                            }
                        }
                        */
                        #endregion Old statement --- End

                    }
                    else
                    {
                        NewNodeSequence = NodeSequence + 1;
                    }
                    #region  " Update target node "
                    if (drCurrentRow.Count() == 1)
                    {
                        sItemCurrent["Sequence"] = NewNodeSequence;
                        sItemCurrent.Update();
                    }
                    else
                    {
                        //I Multi row
                        for (int i1 = 0; i1 < drCurrentRow.Count(); i1++)
                        {
                            SPListItem item =
                                oListNavigation.GetItemById(Convert.ToInt16(
                                drCurrentRow[i1]["ID"].ToString()));
                            item["Sequence"] = NewNodeSequence;
                            //item["ParentNodeID"] = strNextParentNodeID;
                            item.Update();
                        }
                    }
                    //sItemCurrent.Update();
                    #endregion Update target node --- End

                    // - Sequence
                    if (NodeSequence >= Convert.ToInt16(dtMaxRow.Rows[0]["Sequence"].ToString()))
                    {
                        //drRowTop = dt.SelectToTable(string.Format("ParentNodeID = '{0}' AND  Level = '{1}'", strNextParentNodeID, NodeLevel));
                        drRowTop = null;
                    }
                    else
                    {
                        drRowTop = dt.SelectToTable(string.Format("Level = '{0}' AND Sequence = '{1}' AND ParentNodeID = '{2}'",
                            NodeLevel, NodeSequence + 1, drCurrentRow[0]["ParentNodeID"].ToString()));
                    }

                    if (!DataTableIsNullOrEmpty(drRowTop))
                    {
                        foreach (DataRow item in drRowTop.Rows)
                        {
                            if (item["NodeID"].ToString() != vsNodeID)
                            {
                                SPListItem sItemTop = oListNavigation.GetItemById(Convert.ToInt16(item["ID"].ToString()));
                                if (NodeSequence == Convert.ToInt16(dtMaxRow.Rows[0]["Sequence"].ToString()))
                                {
                                    sItemTop["Sequence"] = Convert.ToInt16(item["Sequence"].ToString()) + 1;
                                }
                                else
                                {
                                    sItemTop["Sequence"] = Convert.ToInt16(item["Sequence"].ToString()) - 1;
                                }
                                sItemTop.Update();
                            }
                        }
                    }
                }
            }
            Render();
            return;
        }
        public DataTable ToTable(DataRow[] drs)
        {
            if (drs == null) { return null; }
            if (drs.Length == 0) { return null; }

            DataTable dt = drs[0].Table.Copy();
            dt.Clear();
            foreach (DataRow dr in drs)
            {
                dt.ImportRow(dr);
            }
            return dt;
        }
        public DataTable Sort(DataTable dt, string Sort = "")
        {
            if (!dt.IsNullOrEmpty())
            {
                DataView dv = dt.Copy().DefaultView;
                dv.Sort = Sort;
                dt = dv.ToTable().Copy();
            }
            return dt;
        }
        private void BindInfoTreeview(DataRow argDr)
        {
            try
            {
                #region  " Validate input "
                lblTitle.Text = "";
                lblDescript.Text = "";
                lblUrl.Text = "";
                string Parent = "Head";
                string Description = "";
                string sUrl = "";
                bool bHead = true;
                if (argDr == null) { return; }
                //  Sequence,Title,ParentNodeID,NodeID,Level,Url,Description
                if (argDr["ParentNodeID"] != null)
                {
                    if (!argDr["ParentNodeID"].ToString().Trim().Equals(""))
                    {
                        Parent = argDr["ParentNodeID"].ToString().Trim();
                        bHead = false;
                    }
                }
                if (argDr["Sequence"] == null) { return; }
                if (argDr["Sequence"].ToString().Trim().Equals("")) { return; }
                if (argDr["Title"] == null) { return; }
                if (argDr["Title"].ToString().Trim().Equals("")) { return; }
                if (argDr["NodeID"] == null) { return; }
                if (argDr["NodeID"].ToString().Trim().Equals("")) { return; }
                if (argDr["Level"] == null) { return; }
                if (argDr["Level"].ToString().Trim().Equals("")) { return; }
                if (argDr["Url"] == null)
                {
                    sUrl = "";
                }
                if (argDr["Url"].ToString().Trim().Equals(""))
                {
                    sUrl = "";
                }
                else
                {
                    sUrl = argDr["Url"].ToString().Trim();
                }
                if (argDr["Description"] != null)
                {
                    Description = argDr["Description"].ToString().Trim();
                }
                #endregion Validate input --- End

                #region  " Get Header row "
                if (!bHead)
                {
                    DataRow drHead = GetInfoTreeview(Parent);
                    if (drHead == null) { return; }
                    Parent = drHead["Title"].ToString().Trim();
                }
                #endregion Get Header row --- End

                #region  " Set information of current node of treeview "
                lblTitle.Text = argDr["Title"].ToString().Trim();
                lblUrl.Text = sUrl;
                lblDescript.Text = Description;
                lblLevel.Text = argDr["Level"].ToString().Trim();
                lblParent.Text = Parent;
                lblSequence.Text = argDr["Sequence"].ToString().Trim();
                #endregion Set information of current node of treeview --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
            }
        }
        private DataRow GetInfoTreeview(string argNodeID)
        {
            try
            {
                #region  " Get all data and validate "
                SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
                DataTable dt = oListNavigation.GetDataTable();
                DataRow[] dr = dt.Select(string.Format("NodeID = {0}", argNodeID));
                if (dr == null) { return null; }
                if (dr.Count() < 1) { return null; }
                #endregion Get all data and validate --- End

                return dr[0];

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
            }
            return null;
        }
        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            {

                #region  " Get all data and validate "

                //I None change.
                //I Because "TreeView1.SelectedNode.Value" return NodeID value.
                if (TreeView1.SelectedNode.Value == null) { return; }
                if (TreeView1.SelectedNode.Value.ToString().Equals("")) { return; }
                string str = TreeView1.SelectedNode.Value.ToString().Trim();
                //Alert(str, "");
                #region  " Set display root node "
                if (str.Trim().Equals("0000"))
                {

                    #region  " Set information of current node of treeview "
                    lblTitle.Text = "Root";
                    lblUrl.Text = "";
                    lblDescript.Text = "";
                    lblLevel.Text = "0";
                    lblParent.Text = "";
                    lblSequence.Text = "0";
                    hdfNodeIdForAddHeader.Value = "0000";
                    //Alert(hdfNodeIdForAddHeader.Value,"");
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Alert", "DelTagTd();", true);
                    #endregion Set information of current node of treeview --- End

                    return;
                }
                #endregion Set display root node --- End

                //I Change statement for assign valuePath direct to hdfNodeValue
                hdfNodeValue.Value = TreeView1.SelectedNode.ValuePath;
                hdfNodeIdForAddHeader.Value = "Test message";
                string sId = "";
                if (TreeView1.SelectedNode.ValuePath.IndexOf("/") == -1)
                {
                    hdfNodeIdForAddHeader.Value = sId = TreeView1.SelectedNode.ValuePath;
                }
                else
                {
                    sId = TreeView1.SelectedNode.ValuePath.Substring(TreeView1.SelectedNode.ValuePath.LastIndexOf("/") + 1);
                    hdfNodeIdForAddHeader.Value = sId;
                }
                hdfIsHeaderForAddLinkOrAddHeader.Value = TreeView1.SelectedNode.ImageToolTip;
                //Alert(sId, "");
                //string sId = TreeView1.SelectedNode.ValuePath.Substring(
                //    TreeView1.SelectedNode.ValuePath.LastIndexOf("\\"), TreeView1.SelectedNode.ValuePath.Length);
                //hdfNodeIdForAddHeader.Value = sId;
                //Alert(sId);
                DataRow drMain = GetInfoTreeview(str);
                if (drMain == null) { return; }
                BindInfoTreeview(drMain);
                //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Alert", "DelTagTd();", true);

                #endregion Get all data and validate --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
                Alert(str);
            }
        }

        #region  " Move head node "

        //I Create property for (bool) keep action of level to support
        //I Method for check level of node
        private void IsOverLevel()
        {
            try
            {
                int i = Regex.Matches(hdfNodeValue.Value.ToString(), "/").Count;
                _overLevel = true;
                switch (i)
                {
                    case 0:
                        _overLevel = false;
                        iCurrentLevel = 0;
                        break;
                    case 1:
                        _overLevel = false;
                        iCurrentLevel = 1;
                        break;
                    case 2:
                        _overLevel = false;
                        iCurrentLevel = 2;
                        break;
                    case 3:
                        _overLevel = false;
                        iCurrentLevel = 3;
                        break;
                    case 4:
                        _overLevel = false;
                        iCurrentLevel = 4;
                        break;
                    case 5:
                        _overLevel = false;
                        iCurrentLevel = 5;
                        break;
                    case 6:
                        _overLevel = false;
                        iCurrentLevel = 6;
                        break;
                    default:
                        _overLevel = true;
                        iCurrentLevel = 7;
                        break;
                }
                return;
            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
                _overLevel = true;
            }
        }
        //I Create method for move up header
        private void HeaderMoveUp()
        {
            try
            {

                #region  " Get current node "
                SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
                DataTable dt = oListNavigation.GetDataTable();
                int sequence = -1;
                if (DataTableIsNullOrEmpty(dt))
                {
                    //I Error
                    return;
                }
                DataTable dtCurrentNode = dt.SelectToTable(string.Format(
                    "NodeID = {0}", vsNodeID));
                if (DataTableIsNullOrEmpty(dtCurrentNode))
                {
                    //I Error
                    return;
                }
                SPListItem sItemCurrentUpdate =
                    oListNavigation.GetItemById(Convert.ToInt16(dtCurrentNode.Rows[0]["ID"].ToString()));
                if (sItemCurrentUpdate == null)
                {
                    //I Error
                    return;
                }
                int NodeLevel = Convert.ToInt16(
                    dtCurrentNode.Rows[0]["Level"].ToString());
                int NodeSequence = Convert.ToInt16(
                    dtCurrentNode.Rows[0]["Sequence"].ToString());
                #endregion Get current node --- End

                #region  " Check top sequence "
                sequence = Convert.ToInt32(dtCurrentNode.Rows[0]["Sequence"].ToString());
                if (sequence == 1)
                {
                    //I In case node to move to be top sequence
                    return;
                }
                #endregion Check top sequence --- End

                #region  " Check sequence more 1 "
                if (sequence < 2)
                {
                    //I Error
                    return;
                }
                sItemCurrentUpdate["Sequence"] = NodeSequence - 1;
                #endregion Check sequence more 1 --- End

                #region  " Get affected node "

                DataTable dtAffectNode = dt.SelectToTable(string.Format(
                    "NodeID <> {0} AND Level = {1} AND Sequence = {2}",
                    vsNodeID, NodeLevel.ToString(),
                    (NodeSequence - 1).ToString()));
                if (dtAffectNode == null)
                {
                    //I Error
                    return;
                }

                #endregion Get affected node --- End

                #region  " Get item of affect node and update "

                SPListItem sItemAffect =
                    oListNavigation.GetItemById(Convert.ToInt16(
                    dtAffectNode.Rows[0]["ID"].ToString()));
                if (sItemAffect == null)
                {
                    //I Error
                    return;
                }
                sItemAffect["Sequence"] = NodeSequence;

                //I Update data
                if (dtCurrentNode.Rows.Count > 1)
                {
                    for (int i1 = 0; i1 < dtCurrentNode.Rows.Count; i1++)
                    {
                        SPListItem item =
                            oListNavigation.GetItemById(Convert.ToInt16(
                            dtCurrentNode.Rows[i1]["ID"].ToString()));
                        item["Sequence"] = NodeSequence - 1;
                        item.Update();
                    }
                }
                else
                {
                    sItemCurrentUpdate.Update();
                }
                if (dtAffectNode.Rows.Count > 1)
                {
                    for (int i1 = 0; i1 < dtAffectNode.Rows.Count; i1++)
                    {
                        SPListItem item =
                            oListNavigation.GetItemById(Convert.ToInt16(
                            dtAffectNode.Rows[i1]["ID"].ToString()));
                        item["Sequence"] = NodeSequence;
                        item.Update();
                    }
                    //sItemAffect.Update();
                }
                else
                {
                    sItemAffect.Update();
                }
                Render();
                return;

                #endregion Get item of affect node and update --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
                return;
            }
        }
        //I Create method for move down header
        private void HeaderMoveDown()
        {
            try
            {

                #region  " Get current node "

                SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
                DataTable dt = oListNavigation.GetDataTable();
                int sequence = -1, lastSequence = -1;
                if (DataTableIsNullOrEmpty(dt))
                {
                    //I Error
                    return;
                }
                DataTable dtCurrentNode = dt.SelectToTable(string.Format(
                    "NodeID = {0}", vsNodeID));
                if (DataTableIsNullOrEmpty(dtCurrentNode))
                {
                    //I Error
                    return;
                }
                SPListItem sItemCurrentUpdate =
                    oListNavigation.GetItemById(Convert.ToInt16(dtCurrentNode.Rows[0]["ID"].ToString()));
                if (sItemCurrentUpdate == null)
                {
                    //I Error
                    return;
                }
                int NodeLevel = Convert.ToInt16(
                    dtCurrentNode.Rows[0]["Level"].ToString());
                int NodeSequence = Convert.ToInt16(
                    dtCurrentNode.Rows[0]["Sequence"].ToString());

                #endregion Get current node --- End

                #region  " Check last sequence "
                sequence = Convert.ToInt32(dtCurrentNode.Rows[0]["Sequence"].ToString());
                lastSequence = oListNavigation.GetDataTable().SelectToTable(string.Format(
                    "Level = {0}", NodeLevel.ToString())).Rows.Count;
                if (lastSequence == -1)
                {
                    //I Error
                    return;
                }
                if (sequence == lastSequence)
                {
                    //I In case node to move to be last sequence
                    return;
                }
                #endregion Check last sequence --- End

                #region  " Check sequence to less last sequence "

                if (sequence < lastSequence)
                {
                    //I Update sequence
                    sItemCurrentUpdate["Sequence"] = NodeSequence + 1;
                }
                else
                {
                    //I Error
                    return;
                }

                #endregion Check sequence to less last sequence --- End

                #region  " Get affected node "

                DataTable dtAffectNode = dt.SelectToTable(string.Format(
                    "NodeID <> {0} AND Level = {1} AND Sequence = {2}",
                    vsNodeID, NodeLevel.ToString(),
                    (NodeSequence + 1).ToString()));
                if (dtAffectNode == null)
                {
                    //I Error
                    return;
                }

                #endregion Get affected node --- End

                #region  " Get item of affect node and update "

                SPListItem sItemAffect =
                    oListNavigation.GetItemById(Convert.ToInt16(
                    dtAffectNode.Rows[0]["ID"].ToString()));
                if (sItemAffect == null)
                {
                    //I Error
                    return;
                }
                sItemAffect["Sequence"] = NodeSequence;

                //I Update data
                if (dtCurrentNode.Rows.Count > 1)
                {
                    for (int i1 = 0; i1 < dtCurrentNode.Rows.Count; i1++)
                    {
                        SPListItem item =
                            oListNavigation.GetItemById(Convert.ToInt16(
                            dtCurrentNode.Rows[i1]["ID"].ToString()));
                        item["Sequence"] = NodeSequence + 1;
                        item.Update();
                    }
                }
                else
                {
                    sItemCurrentUpdate.Update();
                }
                if (dtAffectNode.Rows.Count > 1)
                {
                    for (int i1 = 0; i1 < dtAffectNode.Rows.Count; i1++)
                    {
                        SPListItem item =
                            oListNavigation.GetItemById(Convert.ToInt16(
                            dtAffectNode.Rows[i1]["ID"].ToString()));
                        item["Sequence"] = NodeSequence;
                        item.Update();
                    }
                    //sItemAffect.Update();
                }
                else
                {
                    sItemAffect.Update();
                }
                //sItemCurrentUpdate.Update();
                //sItemAffect.Update();
                Render();
                return;

                #endregion Get item of affect node and update --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
                return;
            }
        }

        #endregion Move head node --- End

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
        #endregion Check permission method --- End

        #region  " Delete node of treeview "
        //I Get all item in target and convert to DataTable
        private DataTable GetAllListToDataTable()
        {
            try
            {

                #region  " Get all item of current list and validate "
                SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
                if (oListNavigation == null)
                {
                    //I Incase null
                    return null;
                }
                if (oListNavigation.Count == 0)
                {
                    //I Incase empty
                    return null;
                }
                #endregion Get all item of current list and validate --- End

                #region  " Covert to dataTable and return "
                return oListNavigation.GetDataTable();
                #endregion Covert to dataTable and return --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
            }
            return null;
        }
        //I Get ParentNodeID of current node
        private void GetParentNodeID()
        {
            try
            {

                #region  " Check header "
                IsHeaderOrNot();
                if (IsHeader)
                {
                    sParentNodeID = "";
                    return;
                }
                #endregion Check header --- End

                #region  " Get all list and convert to dataTable , and validate "
                DataTable dt = GetAllListToDataTable();
                if (dt == null)
                {
                    //I Incase get none
                    sParentNodeID = "";
                }
                if (dt.Rows.Count == 0)
                {
                    //I Incase get none
                    sParentNodeID = "";
                }
                #endregion Get all list and convert to dataTable , and validate --- End

                #region  " Get ParentNodeID "
                if (vsNodeID == "")
                {
                    //I Error
                    sParentNodeID = "";
                }
                DataRow[] dr = dt.Select(string.Format("NodeID = '{0}'", vsNodeID));
                if (dr == null)
                {
                    //I Error
                    sParentNodeID = "";
                }
                if (dr.Count() == 0)
                {
                    //I Error
                    sParentNodeID = "";
                }
                if (dr.Count() != 1)
                {
                    //I Error
                    sParentNodeID = "";
                }
                if (dr[0]["ParentNodeID"] == null)
                {
                    //I Error
                    sParentNodeID = "";
                }
                if (dr[0]["ParentNodeID"].ToString().Equals(""))
                {
                    //I Error
                    sParentNodeID = "";
                }
                sParentNodeID = dr[0]["ParentNodeID"].ToString();
                #endregion Get ParentNodeID --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
                sParentNodeID = "";
            }
        }
        //I Re order of sequence of header node
        private void ReOrderSequenceHead()
        {
            try
            {

                #region  " Get all list to datatable and validate "
                DataTable dt = GetAllListToDataTable();
                if (dt == null)
                {
                    //I Incase get none
                    return;
                }
                if (dt.Rows.Count == 0)
                {
                    //I Incase get none
                    return;
                }
                #endregion Get all list to datatable and validate --- End

                #region  " Select target item by level and validate "
                dt = dt.SelectToTable("Level = 1");
                if (dt.IsNullOrEmpty())
                {
                    //I Incase some thing wrong or empty level header
                    return;
                }
                #endregion Select target item by level and validate --- End

                #region  " Sorting and validate "
                dt = dt.Sort("Sequence ASC");
                if (dt.IsNullOrEmpty())
                {
                    //I Incase some thing wrong or empty level header
                    return;
                }
                #endregion Sorting and validate --- End

                #region  " Update item list by looping "
                dt = dt.Sort("Sequence ASC");
                SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
                int iSequence = 1;
                for (int i1 = 0; i1 < dt.Rows.Count; i1++)
                {
                    DataTable dt3 = dt.SelectToTable("NodeID = '" + dt.Rows[i1]["NodeID"].ToString() + "'");
                    if (dt3.Rows.Count > 1)
                    {
                        for (int i2 = 0; i2 < dt3.Rows.Count; i2++)
                        {
                            SPListItem spItemUpdate = oListNavigation.
                                GetItemById(Convert.ToInt16(dt3.Rows[i2]["ID"].ToString()));
                            spItemUpdate["Sequence"] = iSequence;
                            spItemUpdate.Update();
                        }
                        i1 = (i1 + (dt3.Rows.Count - 1));
                    }
                    else
                    {
                        SPListItem spItemUpdate = oListNavigation.
                            GetItemById(Convert.ToInt16(dt.Rows[i1]["ID"].ToString()));
                        spItemUpdate["Sequence"] = iSequence;
                        spItemUpdate.Update();
                    }
                    iSequence++;
                }
                #endregion Update item list by looping --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
            }
        }
        //I Re order of sequence of child node
        private void ReOrderSequenceChild(string argParentNodeID)
        {
            try
            {

                #region  " Get all current list and validate "
                DataTable dt = GetAllListToDataTable();
                if (dt.IsNullOrEmpty())
                {
                    //I Some thing wrong
                    return;
                }
                #endregion Get all current list and validate --- End

                #region  " Select row by current parent node id and validate "
                dt = dt.SelectToTable(string.Format("ParentNodeID = '{0}'", argParentNodeID));
                if (dt.IsNullOrEmpty())
                {
                    //I Parent none child.
                    return;
                }
                #endregion Select row by current parent node id and validate --- End

                #region  " Sorting row by sequence and validate "
                dt = dt.Sort("Sequence ASC");
                if (dt.IsNullOrEmpty())
                {
                    //I Some thing wrong
                    return;
                }
                #endregion Sorting row by sequence and validate --- End

                #region  " Update sequence by looping "
                dt = dt.Sort("Sequence ASC");
                SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
                int iSequence = 1;
                for (int i1 = 0; i1 < dt.Rows.Count; i1++)
                {
                    DataTable dt3 = dt.SelectToTable("NodeID = '" + dt.Rows[i1]["NodeID"].ToString() + "'");
                    if (dt3.Rows.Count > 1)
                    {
                        for (int i2 = 0; i2 < dt3.Rows.Count; i2++)
                        {
                            SPListItem spItemUpdate = oListNavigation.
                                GetItemById(Convert.ToInt16(dt3.Rows[i2]["ID"].ToString()));
                            spItemUpdate["Sequence"] = iSequence;
                            spItemUpdate.Update();
                        }
                        i1 = (i1 + (dt3.Rows.Count - 1));
                    }
                    else
                    {
                        SPListItem spItemUpdate = oListNavigation.
                            GetItemById(Convert.ToInt16(dt.Rows[i1]["ID"].ToString()));
                        spItemUpdate["Sequence"] = iSequence;
                        spItemUpdate.Update();
                    }
                    iSequence++;
                }
                /*
                SPListItemCollection oListNavigation = GetDataFromDocLib(ListNavigation, "");
                for (int i1 = 0; i1 < dt.Rows.Count; i1++)
                {
                    SPListItem spItemUpdate = oListNavigation.
                        GetItemById(Convert.ToInt16(dt.Rows[i1]["ID"].ToString()));
                    spItemUpdate["Sequence"] = (i1 + 1);
                    spItemUpdate.Update();
                }
                */
                #endregion Update sequence by looping --- End

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
            }
        }
        //I Get all Id tocontain Parent node id by NodeID (recursive)
        private string GetAllIdWithParentNodeIdByNodeID(string argNodeID)
        {
            string sReturn = "";
            try
            {

                #region  " Get all NodeID to have ParentNodeID by argument NodeID "
                if (argNodeID.Trim().ToString().Equals(""))
                {
                    //I Incase error
                    return sReturn;
                }
                DataTable dt = GetAllListToDataTable();
                if (dt.IsNullOrEmpty())
                {
                    //I Incase error
                    return sReturn;
                }
                dt = dt.SelectToTable(string.Format("ParentNodeID = '{0}'", argNodeID));
                if (dt.IsNullOrEmpty())
                {
                    //I Incase none child
                    return sReturn;
                }
                #endregion Get all NodeID to have ParentNodeID by argument NodeID --- End

                #region  " Call recursive "
                string sTempNodeID = "";
                for (int i1 = 0; i1 < dt.Rows.Count; i1++)
                {
                    sReturn += GetAllIdWithParentNodeIdByNodeID(dt.Rows[i1]["NodeID"].ToString());
                    sTempNodeID += ("," + dt.Rows[i1]["ID"].ToString());
                }
                sReturn = (sTempNodeID + sReturn);
                #endregion Call recursive --- End

                //I Return string
                return sReturn;

            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
            }
            return "";
        }
        #endregion Delete node of treeview --- End

        #region  " Update image of treeview "
        //I Display treeview and navigation link.
        private void Render()
        {

            TreeView1.Nodes[0].ChildNodes.Clear();
            DataTable dt = GetDataNavigation();
            if (!DataTableIsNullOrEmpty(dt))
            {
                DataTable dtTree = DelDupRowByNodeId(dt, sArColumnNavigation, "NodeID");
                string filter = "Level = 1 AND ISNULL(ParentNodeID,'') = ''";
                procMenuRender(dtTree, TreeView1.Nodes[0].ChildNodes, filter);
                TreeView1.ExpandAll();
                recursiveSetImageTreeView(TreeView1.Nodes[0]);

                #region  " Check Permission "
                /*
                CheckUserOwner();
                dt = dt.SelectToTable("Level < 4");
                if (bGroupOwner)
                {
                    //I In case current user live in "SPIMPortal Owners"
                    dt = dt.SelectToTable("AudienceName = '' OR AudienceName is null OR AudienceName = 'SPIMPortal Visitors' OR AudienceName = 'SPIMPortal Owners'");
                }
                else
                {
                    //I In case current user none live in "SPIMPortal Owners"
                    //I Get all node with under level
                    //I Get the node to contain : "SPIMPortal Owners"
                    DataTable tmpDt = dt.SelectToTable("AudienceName = 'SPIMPortal Owners'");
                    //I Check valid data
                    if (tmpDt != null)
                    {
                        //I Incase valid
                        for (int i1 = 0; i1 < tmpDt.Rows.Count; i1++)
                        {
                            dt = DeleteTree(dt, tmpDt.Rows[i1]["NodeID"].ToString());
                        }
                    }
                }
                dt = dt.Sort("Sequence ASC");
                */
                #endregion Check Permission --- End

                //  TODO: 01 - Fix for view menu.
                //I Menu1.Items.Clear();
                //I procMenuRender(dt, Menu1.Items, filter);
            }
        }

        #endregion Update image of treeview --- End

        #region  " Refrash treeview and navigation link "
        protected void btnRefrash_Click(object sender, EventArgs e)
        {
            try
            {
                Render();
            }
            catch (Exception ex)
            {
                string str = "Error : " + ex.Message;
            }
        }
        #endregion Refrash treeview and navigation link --- End

        void recursiveSetImageTreeView(TreeNode argTn)
        {
            foreach (TreeNode Node in argTn.ChildNodes)
            {

                #region  " Set image of treeview new 1 "
                if (!Node.ToolTip.ToString().Equals("1"))
                {
                    if (Node.ChildNodes.Count == 0)
                    {
                        if (Node.ImageToolTip.Equals("1"))
                        {
                            //Node.ImageUrl = "../../../../Img/treeFold.png";
                            Node.ImageUrl = "../../../../Img/treeExpand.gif";
                            Node.Text = "<span style='display:none;' class='clsRemoveTd'></span>" + Node.Text;
                        }
                        else
                        {
                            Node.Text = "<span style='display:none;' class='clsRemoveTd'></span>" + Node.Text;
                            Node.ImageUrl = "../../../../Img/link.gif";
                        }
                        //Node.ImageToolTip = "-1";
                    }
                }
                #endregion Set image of treeview new 1 --- End

                #region  " Set image of treeview : Old statement 1 "
                /*
                if (!Node.ToolTip.ToString().Equals("1"))
                {
                    if (Node.ChildNodes.Count == 0)
                    {
                        if (Node.ImageToolTip.Equals(""))
                        {
                            //Node.ImageUrl = "../../../../Img/treeFold.png";
                            Node.ImageUrl = "../../../../Img/treeExpand.gif";
                            Node.Text = "<span style='display:none;' class='clsRemoveTd'></span>" + Node.Text;
                        }
                        else
                        {
                            Node.Text = "<span style='display:none;' class='clsRemoveTd'></span>" + Node.Text;
                            Node.ImageUrl = "../../../../Img/link.gif";
                        }
                    }
                }
                */
                #endregion Set image of treeview : Old statement 1 --- End
                /*
                if (Node.ChildNodes.Count == 0) {
                    if (Node.ImageToolTip.Equals("")) {
                        Node.ImageUrl = "../../../../Img/treeFold.png";
                    }
                    else {
                        Node.ImageUrl = "../../../../Img/link.gif";
                    }
                }
                */
                recursiveSetImageTreeView(Node);
            }
        }


    }
}
