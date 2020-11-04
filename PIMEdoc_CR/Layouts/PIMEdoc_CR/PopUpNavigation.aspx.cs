using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
//using Microsoft.SharePoint
using System.Data;
using System.Data.OleDb;
using System.Web.UI;
using System.Collections.Generic;
using System.Linq;

namespace PIMEdoc_CR.Layouts.PIMEdoc_CR
{
    public partial class PopUpNavigation : LayoutsPageBase
    {
        public string SiteURL = SPContext.Current.Web.Url;
        private static string ListNavigation = "Navigation";
        private static string LevelOfHeader = "1";
        private static string ParentNodeIdOfHeader = "";
        private static string fldHead = "";
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
        public string sParentNodeID
        {
            get
            {
                if (ViewState["sParentNodeID"] == null) { return string.Empty; }
                return ViewState["sParentNodeID"].ToString();
            }
            set
            {
                ViewState["sParentNodeID"] = value;
            }
        }
        public string vsAction
        {
            get
            {
                if (ViewState["Action"] == null) { return string.Empty; }
                return ViewState["Action"].ToString();
            }
            set
            {
                ViewState["Action"] = value;
            }
        }
        public string IsHeader
        {
            get
            {
                if (ViewState["IsHeader"] == null) { return string.Empty; }
                return ViewState["IsHeader"].ToString();
            }
            set
            {
                ViewState["IsHeader"] = value;
            }
        }
        public bool IsEdit
        {
            get
            {
                if (ViewState["IsEdit"] == null)
                {
                    return false;
                }
                return (bool)ViewState["IsEdit"];
            }
            set
            {
                ViewState["IsEdit"] = value;
            }
        }
        private static string SharedPointSite = SPContext.Current.Site.Url;
        private static SPSite oSPsite = new SPSite(SharedPointSite);
        private static SPWeb web = oSPsite.OpenWeb();

        public static string SiteUrl = SPContext.Current.Site.Url;
        public List<ClassIDDelete> IdForDelete
        {
            get
            {
                if (ViewState["IdForDelete"] == null)
                {
                    ViewState["IdForDelete"] = new List<ClassIDDelete>();
                }
                return (List<ClassIDDelete>)ViewState["IdForDelete"];
            }
            set
            {
                this.ViewState["IdForDelete"] = value;
            }
        }
        [Serializable]
        public class ClassIDDelete
        {
            public int iID { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                #region  " New statement 1 "
                if (Request.QueryString["NodeID"] != null)
                {
                    string[] sSeparators = new string[] { "\\\\" };
                    string[] sNodeIDWithBackSlash = null;
                    if (Request.QueryString["NodeID"].Contains("\\\\"))
                    {
                        sNodeIDWithBackSlash = Request.QueryString["NodeID"].ToString().Split(sSeparators, StringSplitOptions.None);
                        if (sNodeIDWithBackSlash.Length > 0)
                        {
                            vsNodeID = sNodeIDWithBackSlash[sNodeIDWithBackSlash.Length - 1];
                            sParentNodeID = sNodeIDWithBackSlash[sNodeIDWithBackSlash.Length - 2];
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Request.QueryString["NodeID"]))
                        {
                            vsNodeID = "0000";
                        }
                        else
                        {
                            vsNodeID = Request.QueryString["NodeID"];
                        }
                    }
                    /*
                    if (string.IsNullOrEmpty(Request.QueryString["NodeID"]))
                    {
                        vsNodeID = "0000";
                    }
                    else
                    {
                        vsNodeID = Request.QueryString["NodeID"];
                    }
                    */
                }
                if (Request.QueryString["Head"] != null)
                {
                    if (string.IsNullOrEmpty(Request.QueryString["Head"]))
                    {
                        IsHeader = "No";
                        fldHead = "0";
                    }
                    else
                    {
                        if (Request.QueryString["Head"].ToString().Equals("0"))
                        {
                            IsHeader = "No";
                            fldHead = "0";
                        }
                        else
                        {
                            IsHeader = "Yes";
                            fldHead = "1";
                        }
                    }
                }
                #endregion New statement 1 --- End

                #region  " Old statement 1 "
                /*
                if (Request.QueryString["NodeID"] != null)
                {
                    string[] sSeparators = new string[] { "\\\\" };
                    string[] sNodeIDWithoutBackSlash = null;
                    if (Request.QueryString["NodeID"].Contains("\\\\"))
                    {
                        sNodeIDWithoutBackSlash = Request.QueryString["NodeID"].ToString().Split(sSeparators, StringSplitOptions.None);
                        if (sNodeIDWithoutBackSlash.Length > 0)
                        {
                            vsNodeID = sNodeIDWithoutBackSlash[sNodeIDWithoutBackSlash.Length - 1];
                            sParentNodeID = sNodeIDWithoutBackSlash[sNodeIDWithoutBackSlash.Length - 2];
                            IsHeader = "No";
                            fldHead = "0";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Request.QueryString["NodeID"]))
                        {
                            vsNodeID = "0000";
                        }
                        else
                        {
                            vsNodeID = Request.QueryString["NodeID"];
                        }

                        IsHeader = "Yes";
                        fldHead = "1";
                    }
                }
                */
                #endregion Old statement 1 --- End

                vsAction = Page.Request.QueryString["Action"];
                if (vsAction == "Edit")
                {
                    IsEdit = true;
                    DataTable dt = GetDataTable(ListNavigation, "NodeID = '" + vsNodeID + "'", "");
                    if (!DataTableIsNullOrEmpty(dt))
                    {
                        BindData(dt);
                    }
                }
            }
        }
        protected void btnOK_Click(object sender, EventArgs e)
        {
            string Result = string.Empty;
            if (!PassValidate(ref Result, vsAction))
            {

                DataTable dt;
                DataTable dt_node;
                string ParentNodeID;
                vsAction = Page.Request.QueryString["Action"];
                ParentNodeID = string.Empty;
                dt = new DataTable();
                dt_node = new DataTable();
                string sSequenceOfHeader = "";

                if (vsAction == "Edit")
                {
                    using (SPSite oSite = new SPSite(SiteUrl))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {

                            #region  " Get sequence of header for edit "
                            DataTable dtTmp = GetDataTable(ListNavigation, "NodeID = '" + vsNodeID + "'", "NodeID DESC");
                            if (DataTableIsNullOrEmpty(dtTmp))
                            {
                                sSequenceOfHeader = "1";
                            }
                            else
                            {
                                sSequenceOfHeader = dtTmp.Rows[0]["Sequence"].ToString();
                            }
                            #endregion Get sequence of header for edit --- End

                            SPList list;
                            SPListItemCollection oList;
                            list = oWeb.Lists[ListNavigation];
                            oList = GetDataFromDocLib(ListNavigation, @"<Where><Eq><FieldRef Name='NodeID' /><Value Type='Text'>" + vsNodeID + @"</Value></Eq></Where>");
                            if (oList != null)
                            {
                                if (oList.Count > 0)
                                {
                                    foreach (SPListItem item in oList)
                                    {
                                        ClassIDDelete NewList = new ClassIDDelete();
                                        NewList.iID = item.ID;
                                        IdForDelete.Add(NewList);
                                    }
                                }
                            }
                            if (IsHeader == "Yes")
                            {
                                //vsAction = "AddHeading";
                                vsAction = "EditHeading";
                            }
                            else
                            {
                                vsAction = "AddLink";
                            }
                        }
                    }
                }
                if (vsAction == "EditHeading")
                {
                    //  Sequence , NodeID
                    dt = EditHeaderLevel1OrMore(vsNodeID, sSequenceOfHeader);
                }
                if (vsAction == "AddHeading")
                {
                    //Alert(vsNodeID, "");
                    if (vsNodeID == "0000")
                    {
                        //Alert("Add node level 1 outer : " + vsNodeID, "");
                        dt = AddNodeLevel1(vsNodeID);
                    }
                    else
                    {
                        if (vsNodeID.Trim().Equals(""))
                        {
                            //Alert("Add node level 1 inner : " + ParentNodeID, "");
                            dt = AddNodeLevel1("0000");
                        }
                        else
                        {
                            //Alert("Add node level 1 child : " + vsNodeID, "");
                            dt = AddChildNode(vsNodeID);
                        }
                        //Alert("Error [Mark Add header not 0000]", "");
                        /*
                        dt_node = GetDataTable(ListNavigation, "NodeID = '" + vsNodeID + "'", "NodeID DESC");
                        if (!DataTableIsNullOrEmpty(dt_node)) {
                            ParentNodeID = dt_node.Rows[0]["ParentNodeID"].ToString();
                            //Alert(ParentNodeID, "");
                            if (string.IsNullOrEmpty(ParentNodeID)) {
                                Alert("Add node level 1 inner : " + ParentNodeID, "");
                                dt = AddNodeLevel1(vsNodeID);
                            }
                            else {
                                Alert("Add node level 1 child : " + ParentNodeID, "");
                                dt = AddChildNode(ParentNodeID);
                            }
                        }
                        */
                    }
                }
                else if (vsAction == "AddLink")
                {
                    if (IsEdit)
                    {
                        dt = AddChildNode(sParentNodeID);
                    }
                    else
                    {
                        dt = AddChildNode(vsNodeID);
                    }
                }

                Insert(dt);

                if (IsEdit)
                {
                    DeleteItemByID(IdForDelete);
                }

                ScriptManager.RegisterStartupScript(Page, typeof(System.Web.UI.Page), "ClosePopUp", "ClosePopUp();", true);
            }
            else
            {
                Alert(Result, "");
            }

        }
        private DataTable EditHeaderLevel1OrMore(string NodeID, string argSequence)
        {
            //int Level = 1;
            DataTable dt_audience = GetSelectdAudience();
            DataTable dt = GenerateColumn();

            if (!DataTableIsNullOrEmpty(dt_audience))
            {
                //Alert("Get row by audience","");
                foreach (DataRow dr_audience in dt_audience.Rows)
                {
                    /*
                    dt.Rows.Add(NodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl, LevelOfHeader,
                        argSequence, dr_audience["AudienceID"].ToString(), dr_audience["AudienceName"].ToString(),
                        txtDescription.Text, fldHead);
                    */
                    dt.Rows.Add(NodeID.ToString(), ParentNodeIdOfHeader, txtTitle.Text, AssetUrlSelector1.AssetUrl, LevelOfHeader,
                        argSequence, dr_audience["AudienceID"].ToString(), dr_audience["AudienceName"].ToString(),
                        txtDescription.Text, fldHead);
                }
            }
            else
            {
                dt.Rows.Add(NodeID.ToString(), ParentNodeIdOfHeader, txtTitle.Text, AssetUrlSelector1.AssetUrl,
                    LevelOfHeader, argSequence, "", "", txtDescription.Text, fldHead);
                //Alert("Get row by none audience", "");
                /*
                dt.Rows.Add(NodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl,
                    LevelOfHeader, argSequence, "", "", txtDescription.Text, fldHead);
                */
            }
            /*
            int NewNodeID = 0;
            int Sequence = 1;
            DataTable dt_node = new DataTable();


            if (NodeID.Equals("0000")) {
                #region  " Generate data for add header only "
                dt_node = GetDataTable(ListNavigation, "Level = '1'", "NodeID DESC");
                DataTable dtSequence = GetDataTable(ListNavigation, "Level = '1'", "Sequence DESC");
                if ((!DataTableIsNullOrEmpty(dt_node)) && (!DataTableIsNullOrEmpty(dtSequence))) {
                    NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                    Sequence = int.Parse(dtSequence.Rows[0]["Sequence"].ToString()) + 1;
                }
                else {
                    if ((!DataTableIsNullOrEmpty(dt_node))) {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                        Sequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString()) + 1;
                    }
                    else {
                        NewNodeID = 1001;
                        Sequence = 1;
                    }
                }
                #endregion Generate data for add header only --- End
            }
            else {
                #region  " Generate or get data for add or edit "
                dt_node = GetDataTable(ListNavigation, "NodeID = '" + NodeID + "'", "NodeID DESC");
                if (!DataTableIsNullOrEmpty(dt_node)) {
                    if (IsEdit) {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString());
                        Sequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString());
                    }
                    else {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                        Sequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString()) + 1;
                    }

                }
                else {
                    dt_node = GetDataTable(ListNavigation, "Level = '1'", "NodeID DESC");
                    if (!DataTableIsNullOrEmpty(dt_node)) {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                        Sequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString()) + 1;
                    }
                    else {
                        NewNodeID = 1001;
                        Sequence = 1;
                    }
                }
                #endregion Generate or get data for add or edit --- End
            }

            if (!DataTableIsNullOrEmpty(dt_audience)) {
                foreach (DataRow dr_audience in dt_audience.Rows) {
                    dt.Rows.Add(NewNodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl, Level, Sequence, dr_audience["AudienceID"].ToString(), dr_audience["AudienceName"].ToString(), txtDescription.Text);
                }
            }
            else {
                dt.Rows.Add(NewNodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl, Level, Sequence, "", "", txtDescription.Text);
            }
            */

            return dt;
        }
        private DataTable GetSelectdAudience()
        {
            try
            {
                DataTable dt_audience = new DataTable();
                dt_audience.Columns.Add("AudienceID");
                dt_audience.Columns.Add("AudienceName");
                if (PeopleEditor1.ResolvedEntities.Count > 0)
                {
                    for (int i = 0; i < PeopleEditor1.ResolvedEntities.Count; i++)
                    {
                        PickerEntity user = (PickerEntity)PeopleEditor1.ResolvedEntities[i];
                        string uname = user.EntityData["AccountName"].ToString();
                        SPSite site = new SPSite(SiteURL);
                        SPWeb web = site.OpenWeb();
                        if (web.Groups.OfType<SPGroup>().Where(g => g.Name == uname).Count() > 0)
                        {
                            SPGroup oGroup = web.SiteGroups[uname];
                            dt_audience.Rows.Add(oGroup.ID, oGroup.Name);
                        }

                    }
                }
                return dt_audience;
            }
            catch (Exception ex)
            {
                lbllog.Text = ex.ToString();
                throw ex;
            }


        }
        private DataTable AddNodeLevel1(string NodeID)
        {
            int Level = 1;
            int NewNodeID = 0;
            int Sequence = 1;
            DataTable dt_audience = GetSelectdAudience();
            DataTable dt_node = new DataTable();
            DataTable dt = GenerateColumn();


            if (NodeID.Equals("0000"))
            {
                #region  " Generate data for add header only "
                dt_node = GetDataTable(ListNavigation, "Level = '1'", "NodeID DESC");
                DataTable dtSequence = GetDataTable(ListNavigation, "Level = '1'", "Sequence DESC");
                if ((!DataTableIsNullOrEmpty(dt_node)) && (!DataTableIsNullOrEmpty(dtSequence)))
                {
                    NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                    Sequence = int.Parse(dtSequence.Rows[0]["Sequence"].ToString()) + 1;
                }
                else
                {
                    if ((!DataTableIsNullOrEmpty(dt_node)))
                    {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                        Sequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString()) + 1;
                    }
                    else
                    {
                        NewNodeID = 1001;
                        Sequence = 1;
                    }
                }
                #endregion Generate data for add header only --- End
            }
            else
            {
                #region  " Generate or get data for add or edit "
                dt_node = GetDataTable(ListNavigation, "NodeID = '" + NodeID + "'", "NodeID DESC");
                if (!DataTableIsNullOrEmpty(dt_node))
                {
                    if (IsEdit)
                    {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString());
                        Sequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString());
                    }
                    else
                    {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                        Sequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString()) + 1;
                    }

                }
                else
                {
                    dt_node = GetDataTable(ListNavigation, "Level = '1'", "NodeID DESC");
                    if (!DataTableIsNullOrEmpty(dt_node))
                    {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                        Sequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString()) + 1;
                    }
                    else
                    {
                        NewNodeID = 1001;
                        Sequence = 1;
                    }
                }
                #endregion Generate or get data for add or edit --- End
            }

            if (!DataTableIsNullOrEmpty(dt_audience))
            {
                foreach (DataRow dr_audience in dt_audience.Rows)
                {
                    dt.Rows.Add(NewNodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl, Level, Sequence, dr_audience["AudienceID"].ToString(), dr_audience["AudienceName"].ToString(), txtDescription.Text, fldHead);
                }
            }
            else
            {
                dt.Rows.Add(NewNodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl, Level, Sequence, "", "", txtDescription.Text, fldHead);
            }

            return dt;
        }
        private DataTable AddNode(string ParentNodeID)
        {
            int Level = 1;
            int NewNodeID = 0;
            DataTable dt_audience = GetSelectdAudience();
            DataTable dt_node = new DataTable();
            DataTable dt = GenerateColumn();
            if (!DataTableIsNullOrEmpty(dt_audience))
            {
                dt_node = GetDataTable(ListNavigation, "NodeID = '" + ParentNodeID + "'", "NodeID DESC");
                if (!DataTableIsNullOrEmpty(dt_node))
                {
                    dt_node = GetDataTable(ListNavigation, "Level = " + int.Parse(dt_node.Rows[0]["Level"].ToString()), "NodeID DESC");
                    NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                    Level = int.Parse(dt_node.Rows[0]["Level"].ToString());
                }
                foreach (DataRow dr_audience in dt_audience.Rows)
                {
                    dt.Rows.Add(NewNodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl, Level, dt_audience.Rows.IndexOf(dr_audience) + 1, dr_audience["AudienceID"].ToString(), dr_audience["AudienceName"].ToString(), txtDescription.Text);
                }
            }
            else
            {
                dt.Rows.Add(NewNodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl, Level, "", "", "", txtDescription.Text);
            }
            return dt;
        }
        private DataTable AddChildNode(string NodeID)
        {
            int Level = 1;
            int NewNodeID = 0;
            int iSequence = 0;
            DataTable dt_audience = GetSelectdAudience();
            DataTable dt_node = new DataTable();
            DataTable dt = GenerateColumn();

            dt_node = GetDataTable(ListNavigation, "NodeID = '" + NodeID + "'", "NodeID DESC");
            if (!DataTableIsNullOrEmpty(dt_node))
            {
                //Alert("Add valid parent", "");
                int CurrentLevel = int.Parse(dt_node.Rows[0]["Level"].ToString());
                iSequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString());
                dt_node = GetDataTable(ListNavigation, string.Format("Level = '{0}' AND ParentNodeID = '{1}'", CurrentLevel + 1, NodeID), "NodeID DESC");
                if (!DataTableIsNullOrEmpty(dt_node))
                {
                    //Alert("Valid child in parent", "");
                    if (IsEdit)
                    {
                        DataRow[] dr = dt_node.Select(string.Format("NodeID ='{0}'", vsNodeID));
                        iSequence = int.Parse(dr[0]["Sequence"].ToString());
                        NewNodeID = int.Parse(dr[0]["NodeID"].ToString());
                    }
                    else
                    {
                        DataTable dtNewLevel = GetDataTable(ListNavigation, "Level = '" + dt_node.Rows[0]["Level"].ToString() + "'", "NodeID DESC");
                        if (!DataTableIsNullOrEmpty(dtNewLevel))
                        {
                            NewNodeID = int.Parse(dtNewLevel.Rows[0]["NodeID"].ToString()) + 1;

                        }

                        DataTable dtNodeSequence = dtNewLevel.SelectToTable(string.Format("ParentNodeID = '{0}'", vsNodeID));
                        if (!DataTableIsNullOrEmpty(dtNewLevel))
                        {
                            dtNodeSequence = dtNodeSequence.Sort("Sequence DESC");
                            iSequence = int.Parse(dtNodeSequence.Rows[0]["Sequence"].ToString()) + 1;
                        }
                        //Alert("Sequence of valid child in parent : " + iSequence.ToString(), "");
                    }
                }
                else
                {
                    //Alert("new child in parent", "");
                    dt_node = GetDataTable(ListNavigation, string.Format("Level = '{0}' AND ParentNodeID ='{1}'", CurrentLevel + 1, NodeID), "NodeID DESC");
                    if (!DataTableIsNullOrEmpty(dt_node))
                    {
                        NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                        iSequence = int.Parse(dt_node.Rows[0]["Sequence"].ToString()) + 1;
                        //Alert("Sequence of invalid child in parent : " + iSequence.ToString(), "");
                    }
                    else
                    {
                        dt_node = GetDataTable(ListNavigation, string.Format("Level = '{0}'", CurrentLevel + 1), "NodeID DESC");
                        if (!DataTableIsNullOrEmpty(dt_node))
                        {
                            NewNodeID = int.Parse(dt_node.Rows[0]["NodeID"].ToString()) + 1;
                            iSequence = 1;
                        }
                        else
                        {
                            NewNodeID = int.Parse((CurrentLevel + 1).ToString() + "001");
                            iSequence = 1;
                        }
                        //Alert("Sequence of new child in parent : " + iSequence.ToString(), "");
                    }
                }
                Level = CurrentLevel + 1;

                if (!DataTableIsNullOrEmpty(dt_audience))
                {
                    //I Incase get audience
                    //I 
                    foreach (DataRow dr_audience in dt_audience.Rows)
                    {
                        dt.Rows.Add(NewNodeID.ToString(), NodeID, txtTitle.Text, AssetUrlSelector1.AssetUrl, Level, iSequence, dr_audience["AudienceID"].ToString(), dr_audience["AudienceName"].ToString(), txtDescription.Text, fldHead);
                    }
                }
                else
                {
                    //I For add header none require audience field
                    //I (within value or without value)
                    dt.Rows.Add(NewNodeID.ToString(), NodeID, txtTitle.Text, AssetUrlSelector1.AssetUrl,
                        Level, iSequence, "", "", txtDescription.Text, fldHead);
                }

            }
            else
            {
                //Alert("Invalid parent", "");
                dt.Rows.Add(NewNodeID.ToString(), string.Empty, txtTitle.Text, AssetUrlSelector1.AssetUrl,
                    Level, iSequence + 1, "", "", txtDescription.Text, fldHead);
            }

            return dt;
        }
        private DataTable GenerateColumn()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NodeID");
            dt.Columns.Add("ParentNodeID");
            dt.Columns.Add("Title");
            dt.Columns.Add("Url");
            dt.Columns.Add("Level");
            dt.Columns.Add("Sequence");
            dt.Columns.Add("AudienceID");
            dt.Columns.Add("AudienceName");
            dt.Columns.Add("Description");
            dt.Columns.Add("Head");
            return dt;
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
        public void Insert(DataTable dt)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (oSPsite = new SPSite(SharedPointSite))
                    {

                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                oSPWeb.AllowUnsafeUpdates = true;
                                SPList PTList = oSPWeb.Lists[ListNavigation];
                                if (vsAction != "Edit")
                                {
                                    SPListItem PTListItem = PTList.Items.Add();
                                    for (int i = 0; i < dr.ItemArray.Length; i++)
                                    {
                                        PTListItem[dt.Columns[i].ColumnName] = dr[i].ToString();
                                        PTListItem.Update();
                                    }
                                }
                                else
                                {
                                    SPListItemCollection items = PTList.GetItems(new SPQuery()
                                    {
                                        Query = @"<Where><Eq><FieldRef Name='NodeID' /><Value Type='Text'>" + vsNodeID + "</Value></Eq></Where>"
                                    });
                                    for (int i = 0; i < dr.ItemArray.Length; i++)
                                    {
                                        foreach (SPListItem item in items)
                                        {
                                            item[dt.Columns[i].ColumnName] = dr[i].ToString();
                                            item.Update();
                                        }
                                    }
                                }
                                oSPWeb.AllowUnsafeUpdates = false;

                            }
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                lbllog.Text = ex.ToString();
            }

        }
        public void BindData(DataTable dt)
        {
            if (!DataTableIsNullOrEmpty(dt))
            {
                txtTitle.Text = dt.Rows[0]["Title"].ToString();
                AssetUrlSelector1.AssetUrl = dt.Rows[0]["Url"].ToString();
                txtDescription.Text = dt.Rows[0]["Description"].ToString();
                LevelOfHeader = dt.Rows[0]["Level"].ToString();
                ParentNodeIdOfHeader = dt.Rows[0]["ParentNodeID"].ToString();
                DataTable dtAudience = GetDataTable(ListNavigation, "NodeID = '" + vsNodeID + "'", "");
                PeopleEditor1.CommaSeparatedAccounts = MakeAuienceName(dtAudience);
                PeopleEditor1.Validate();
            }
        }
        private bool DataTableIsNullOrEmpty(DataTable dt)
        {
            if (dt == null) { return true; }
            if (dt.Rows.Count == 0) { return true; }
            return false;
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
        public string MakeAuienceName(DataTable dtAudience)
        {
            string sReturn = string.Empty;
            if (!DataTableIsNullOrEmpty(dtAudience))
            {
                foreach (DataRow item in dtAudience.Rows)
                {
                    if (item["AudienceName"] != null)
                    {
                        if (!string.IsNullOrEmpty(item["AudienceName"].ToString()))
                        {
                            if (string.IsNullOrEmpty(sReturn))
                            {
                                sReturn = item["AudienceName"].ToString();
                            }
                            else
                            {
                                sReturn += ", " + item["AudienceName"].ToString();
                            }
                        }
                    }
                }
            }
            if (sReturn != string.Empty) if (sReturn.EndsWith(";")) sReturn = sReturn.Substring(0, sReturn.Length - 1);
            return sReturn;
        }
        public void DeleteItemByID(List<ClassIDDelete> oList)
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
                    }
                }
            }
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
        public bool PassValidate(ref string sResult, string vsAction)
        {
            bool bl = false;
            if (vsAction == "AddLink")
            {
                #region  " Validate add link "
                if (string.IsNullOrEmpty(txtTitle.Text))
                {
                    sResult = "กรุณากรอก Title";
                    bl = true;
                }

                if (PeopleEditor1.Entities != null)
                {
                    bool bNotEmpty = false;
                    /*
                    if (PeopleEditor1.Entities.Count > 0)
                    {
                        for (int i3 = 0; i3 < PeopleEditor1.Entities.Count; i3++)
                        {
                            PickerEntity pe = (PickerEntity)PeopleEditor1.Entities[i3];
                            string strAudience = pe.DisplayText.Trim();
                            //Alert(strAudience, "");
                            if (!strAudience.Trim().Equals(""))
                            {
                                bNotEmpty = true;
                                break;
                            }
                        }
                        if (!bNotEmpty)
                        {
                            sResult = "กรุณาเลือก GroupSharepoint";
                            bl = true;
                        }
                    }
                    else
                    {
                        sResult = "กรุณาเลือก GroupSharepoint";
                        bl = true;
                    }
                    */
                }
                else
                {
                    sResult = "กรุณาเลือก GroupSharepoint";
                    bl = true;
                }
                /*
                DataTable dtAudience = GetDataTable(ListNavigation, "NodeID = '" + vsNodeID + "'", "");
                if (DataTableIsNullOrEmpty(dtAudience)) {
                    sResult = "กรุณาเลือก GroupSharepoint";
                    bl = true;
                }
                if (string.IsNullOrEmpty(AssetUrlSelector1.AssetUrl)) {
                    sResult = "กรุณาเลือก URL";
                    bl = true;
                }
                if (PeopleEditor1.ResolvedEntities.Count == 0) {
                    sResult = "กรุณาเลือก GroupSharepoint";
                    bl = true;
                }
                */
                #endregion Validate add link --- End
            }
            else
            {
                #region  " Validate add head "
                if (string.IsNullOrEmpty(txtTitle.Text))
                {
                    sResult = "กรุณากรอก Title";
                    bl = true;
                }
                /*
                if (string.IsNullOrEmpty(AssetUrlSelector1.AssetUrl)) {
                    sResult = "กรุณาเลือก URL";
                    bl = true;
                }
                */
                #endregion Validate add head --- End
            }
            return bl;
        }
        public void Alert(String Message, string Url)
        {
            string sUrl = string.Empty;
            if (!string.IsNullOrEmpty(Url))
            {
                sUrl = string.Format("window.location.href = '{0}'", Url);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Error", string.Format("alert('{0}');{1};", Message, sUrl), true);
            }
        }
    }
}
