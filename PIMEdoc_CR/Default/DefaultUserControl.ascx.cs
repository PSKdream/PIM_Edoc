using PIMEdoc_CR.Default.Rule;
using PIMEdoc_CR.Helper;
using PIMEdoc_CR.Rule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PIMEdoc_CR.Default
{
    public partial class DefaultUserControl : UserControl
    {
        private string vs_CurrentPane
        {
            get
            {
                return (string)ViewState["vs_CurrentPane"];
            }
            set
            {
                ViewState["vs_CurrentPane"] = value;
            }
        }
        private string vs_Lang
        {
            get
            {
                return (string)ViewState["vs_Lang"];
            }
            set
            {
                ViewState["vs_Lang"] = value;
            }
        }
        private string vs_CurrentUserLoginName
        {
            get
            {
                return (string)ViewState["vs_CurrentUserLoginName"];
            }
            set
            {
                ViewState["vs_CurrentUserLoginName"] = value;
            }
        }
        public int CurrentPageNumber
        {
            get
            {
                if (ViewState["PageNumber"] != null)
                {
                    return Convert.ToInt32(ViewState["PageNumber"]);
                }
                else
                {
                    return 0;
                }
            }
            set { ViewState["PageNumber"] = value; }
        }
        public int ItemPerPage
        {
            get
            {
                if (ViewState["ItemPerPage"] != null)
                {
                    return Convert.ToInt32(ViewState["ItemPerPage"]);
                }
                else
                {
                    return 10;
                }
            }
            set { ViewState["ItemPerPage"] = value; }
        }
        private string vs_CurrentUserID
        {
            get
            {
                return (string)ViewState["vs_CurrentUserID"];
            }
            set
            {
                ViewState["vs_CurrentUserID"] = value;
            }
        }
        private string vs_CurrentDocType
        {
            get
            {
                return (string)ViewState["vs_CurrentDocType"];
            }
            set
            {
                ViewState["vs_CurrentDocType"] = value;
            }
        }
        private bool vs_isCentreAdmin
        {
            get
            {
                return (bool)ViewState["vs_isCentreAdmin"];
            }
            set
            {
                ViewState["vs_isCentreAdmin"] = value;
            }
        }
        private bool vs_isCentreAdminSarabun
        {
            get
            {
                return (bool)ViewState["vs_isCentreAdminSarabun"];
            }
            set
            {
                ViewState["vs_isCentreAdminSarabun"] = value;
            }
        }
        private bool vs_isITAdmin
        {
            get
            {
                return (bool)ViewState["vs_isITAdmin"];
            }
            set
            {
                ViewState["vs_isITAdmin"] = value;
            }
        }
        private bool vs_isViewAllDOc
        {
            get
            {
                return (bool)ViewState["vs_isViewAllDOc"];
            }
            set
            {
                ViewState["vs_isViewAllDOc"] = value;
            }
        }
        private bool vs_isSearch
        {
            get
            {
                if (ViewState["vs_isSearch"] == null)
                {
                    return false;
                }
                return (bool)ViewState["vs_isSearch"];
            }
            set
            {
                ViewState["vs_isSearch"] = value;
            }
        }
        private DataTable vs_DocumentList
        {
            get
            {
                return (DataTable)ViewState["vs_DocumentList"];
            }
            set
            {
                ViewState["vs_DocumentList"] = value;
            }
        }
        private List<DocumentLibraryGV.RESULT> vs_GVCentre
        {
            get
            {
                return (List<DocumentLibraryGV.RESULT>)ViewState["vs_GVCentre"];
            }
            set
            {
                ViewState["vs_GVCentre"] = value;
            }
        }


        private ClientScriptManager cs;
        readonly System.Globalization.CultureInfo _ctli = Extension._ctli;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    vs_Lang = "TH";

                    vs_CurrentUserLoginName = Rule.SharedRules.LogonName();
                    //vs_CurrentUserID = Rule.SharedRules.FindUserID(vs_CurrentUserLoginName, this.Page);
                    //if (string.IsNullOrEmpty(vs_CurrentUserID))
                    //{
                    //    vs_CurrentUserID = Request.QueryString["USERID"] != null
                    //        ? Request.QueryString["USERID"].ToString()
                    //        : "5050108";
                    //}

                    if (Request.QueryString["USERID"] != null)
                    {
                        vs_CurrentUserID = Request.QueryString["USERID"].ToString();
                    }
                    else
                    {
                        vs_CurrentUserID = Rule.SharedRules.FindUserID(vs_CurrentUserLoginName, this.Page);
                    }

                    vs_CurrentPane = "centre";
                    vs_CurrentDocType = "P";
                    vs_isCentreAdminSarabun = false;
                    vs_isCentreAdmin = false;
                    vs_isITAdmin = false;
                    vs_isViewAllDOc = false;
                    vs_isSearch = false;
                    InitialData();
                    FilterByDocType(vs_CurrentDocType, vs_CurrentPane);
                    cs = Page.ClientScript;
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                Extension.MessageBox(this.Page, ex.Message);
            }
        }

        private void InitialData()
        {
            DataTable dtCentreAdmin = Extension.GetDataTable("MstAdminCentre");
            if (!dtCentreAdmin.DataTableIsNullOrEmpty())
            {
                foreach (DataRow row in dtCentreAdmin.Rows)
                {
                    //if (row["EmpID"].ToString().Trim().Split(',').Contains(vs_CurrentUserID) )
                    if (row["UserName"].ToString().Trim().Split(',').Contains(vs_CurrentUserLoginName) || row["EmpID"].ToString().Trim().Split(',').Contains(vs_CurrentUserID))
                    {
                        vs_isCentreAdmin = true;
                        if (row["DeptID"].ToString() == "10")
                        {
                            vs_isCentreAdminSarabun = true;
                        }
                    }
                }
            }

            string spGroup = "Admin_ITEdoc";
            if (!string.IsNullOrEmpty(spGroup))
            {
                List<string> userList = Rule.SharedRules.GetAllUserInGroup(spGroup);
                DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
                if (!dtEmp.DataTableIsNullOrEmpty())
                {
                    foreach (string user in userList)
                    {
                        if (vs_CurrentUserID == SharedRules.FindUserID(user, this.Page))
                        {
                            vs_isITAdmin = true;
                            break;
                        }
                    }
                }
            }

            spGroup = "All_ViewDocument";
            if (!string.IsNullOrEmpty(spGroup))
            {
                List<string> userList = Rule.SharedRules.GetAllUserInGroup(spGroup);
                DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
                if (!dtEmp.DataTableIsNullOrEmpty())
                {
                    foreach (string user in userList)
                    {
                        if (vs_CurrentUserID == SharedRules.FindUserID(user, this.Page))
                        {
                            vs_isViewAllDOc = true;
                            break;
                        }
                    }
                }
            }
            DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", "<View><Query><Where><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq></Where><RowLimit>100000</RowLimit></Query></View>");
            if (!dtDocumentType.DataTableIsNullOrEmpty())
            {
                foreach (DataRow dr in dtDocumentType.Rows)
                {
                    switch (dr["Value"].ToString())
                    {
                        case "P":
                            btn_Annouce.CommandName = "P";
                            txt_Annouce.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        case "C":
                            btn_Command.CommandName = "C";
                            txt_Command.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        case "R":
                            btn_Rule.CommandName = "R";
                            txt_Rule.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        case "L":
                            btn_Order.CommandName = "L";
                            txt_Order.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        case "Im":
                            btn_Recieve.CommandName = "Im";
                            txt_Recieve.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        case "M":
                            btn_M.CommandName = "M";
                            txt_M.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        case "Ex":
                            btn_Ex.CommandName = "Ex";
                            txt_Ex.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        case "ExEN":
                            btn_ExEn.CommandName = "ExEN";
                            txt_ExEn.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        case "Other":
                            btn_Other.CommandName = "Other";
                            txt_Other.Text = string.Format("{0}<br/>{1}", dr["DocTypeName"].ToString(), dr["DocTypeNameEN"].ToString());
                            break;
                        default: break;
                    }
                }
            }
        }

        #region | Button Action |
        protected void btn_Command_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            //disable all button
            btn_Annouce.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_Annouce.CommandName));
            btn_Annouce.Attributes.CssStyle.Add("background-size", "contain !important");
            btn_Command.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_Command.CommandName));
            btn_Command.Attributes.CssStyle.Add("background-size", "contain !important");
            btn_Rule.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_Rule.CommandName));
            btn_Rule.Attributes.CssStyle.Add("background-size", "contain !important");
            btn_Order.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_Order.CommandName));
            btn_Order.Attributes.CssStyle.Add("background-size", "contain !important");
            btn_Ex.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_Ex.CommandName));
            btn_Ex.Attributes.CssStyle.Add("background-size", "contain !important");
            btn_ExEn.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_Ex.CommandName));
            btn_ExEn.Attributes.CssStyle.Add("background-size", "contain !important");
            btn_Recieve.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_Recieve.CommandName));
            btn_Recieve.Attributes.CssStyle.Add("background-size", "contain !important");
            btn_M.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_M.CommandName));
            btn_M.Attributes.CssStyle.Add("background-size", "contain !important");
            btn_Other.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}_none.png')  !important", Extension.GetSPSite(), btn_Other.CommandName));
            btn_Other.Attributes.CssStyle.Add("background-size", "contain !important");
            //enable selected button
            btn.Attributes.CssStyle.Add("background", string.Format("url('{0}/Img/{1}.png') !important", Extension.GetSPSite(), btn.CommandName));
            btn.Attributes.CssStyle.Add("background-size", "contain !important");

            vs_CurrentDocType = btn.CommandName;
            FilterByDocType(btn.CommandName, vs_CurrentPane);
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true); 

        }
        protected void btn_CentreReset_Click(object sender, EventArgs e)
        {
            ClearFilter();
        }
        protected void btn_CentreSearch_Click(object sender, EventArgs e)
        {
            FilterByDocType(vs_CurrentDocType, vs_CurrentPane);
        }
        #endregion

        #region | Document Filter |
        protected void ClearFilter()
        {
            txt_CentreSearch.Text = "";
            txt_CentreCreateFrom.Text = "";
            txt_CentreCreateTo.Text = "";

            FilterByDocType(vs_CurrentDocType, vs_CurrentPane);
        }

        private void FilterByDocType(string docType, string category)
        {
            try
            {

                vs_isSearch = false;
                string searchTitle = txt_CentreSearch.Text.ToLower();

                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                string status = ddl_Status.SelectedValue;
                string Department = "";
                SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(Page, vs_CurrentUserID);
                if (empData != null)
                {
                    List<SpecificEmployeeData.RESULT> empResult = empData.RESULT.ToList();
                    List<string> listDept = new List<string>();
                    foreach (SpecificEmployeeData.RESULT item in empResult)
                    {
                        if (!string.IsNullOrWhiteSpace(item.DEPARTMENT_ID))
                        {
                            if (!string.IsNullOrWhiteSpace(Department))
                            {
                                Department += string.Format(",'{0}'", item.DEPARTMENT_ID);
                            }
                            else
                            {
                                Department += string.Format("'{0}'", item.DEPARTMENT_ID);
                            }
                        }
                    }
                }
                string mess = "";
                string DateForm = Extension.ConvertTextToDateFormat(txt_CentreCreateFrom.Text);
                string DateTo = Extension.ConvertTextToDateFormat(txt_CentreCreateTo.Text);
                bool isAdmin = (vs_isITAdmin || vs_isViewAllDOc || vs_isCentreAdmin) ? true : false;
                mess = string.Format("docType = {0}\r\n category = {1}\r\n status = {2}\r\n searchTitle = {3}\r\n Department = {4}\r\n DateForm = {5}\r\n DateTo = {6}\r\n isAdmin = {7}\r\n ",docType, category, status, searchTitle, Department, DateForm, DateTo, isAdmin);
                List<TRNDocument> ListDocument = db.SP_DocumentLibrary(docType, category, status, searchTitle, Department, DateForm, DateTo, isAdmin).ToList();

                if (ListDocument == null)
                {
                    Extension.LogWriter.Write(new Exception("(ListDocument == null)"));
                    mess += string.Format("ListDocument = {0}", "null");
                }
                else
                {
                    mess += string.Format("ListDocument = {0}", ListDocument.Count());
                }
                
                
                if (docType == "Other")
                {
                    gv_CentreDocList.Columns[2].Visible = true;
                }
                else
                {
                    gv_CentreDocList.Columns[2].Visible = false;
                }
                #region เช็คที่ stored แทน เปลี่ยนจาก (เอกสารภายในหน่วยงาน บุคคลทั่วไปเห็นแค่ ประกาศ + คำสั่ง) --> เห็นได้ทุกประเภท ยกเว้น ที่เป็น ความลับ
                //if (vs_CurrentPane == "internal")
                //{
                //    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);

                //    //เอกสารภายในหน่วยงาน บุคคลทั่วไปเห็นแค่ ประกาศ + คำสั่ง
                //    if ((!(docType == "P" || docType == "C")) && !(vs_isITAdmin || vs_isViewAllDOc || vs_isCentreAdmin))
                //    {
                //        vs_DocumentList = new DataTable();
                //        BindViewStateToCentre(true);
                //        return;
                //    }
                //    if (vs_isITAdmin || vs_isViewAllDOc)
                //    {
                //        vs_DocumentList = Extension.ListToDataTable<TRNDocument>(ListDocument);//FilterGridViewByCreatedDAte(ListDocument, searchCreateFrom, searchCreateTo);
                //        BindViewStateToCentre(true);
                //        return;
                //    }
                //}
                //else
                //{

                //    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);

                //    if (vs_isITAdmin || vs_isViewAllDOc)
                //    {
                //        vs_DocumentList = Extension.ListToDataTable<TRNDocument>(ListDocument);//FilterGridViewByCreatedDAte(ListDocument, searchCreateFrom, searchCreateTo);
                //        BindViewStateToCentre(true);
                //        return;
                //    }
                //}
                #endregion

                vs_DocumentList = Extension.ListToDataTable<TRNDocument>(ListDocument);//FilterGridViewByCreatedDAte(ListDocument, searchCreateFrom, searchCreateTo);
                BindViewStateToCentre(true);
                //Extension.MessageBox(this.Page, mess);
                //if (!string.IsNullOrWhiteSpace(searchTitle))
                //{ 
                //    SearchGridviewTable(searchTitle.ToLower(), gv_CentreDocList); 
                //}
            }
            catch (Exception ex)
            {
                Extension.MessageBox(this.Page, ex.Message.Replace('\'', ' '));
                Extension.LogWriter.Write(ex);
            }
        }
        private DataTable FilterGridViewByCreatedDAte(List<TRNDocument> ListDocument, DateTime searchCreateFrom, DateTime searchCreateTo)
        {
            if (string.IsNullOrWhiteSpace(txt_CentreCreateFrom.Text) && string.IsNullOrWhiteSpace(txt_CentreCreateTo.Text))
            {
                return Extension.ListToDataTable<TRNDocument>(ListDocument);
            }
            searchCreateTo = searchCreateTo.AddDays(1);
            return Extension.ListToDataTable<TRNDocument>(
                        ListDocument
                            .Where(x => x.CreatedDate.Value.CompareTo(searchCreateFrom) >= 0 && x.CreatedDate.Value.CompareTo(searchCreateTo) <= 0)
                            .ToList()
                    );


            DataTable dtDocument = Extension.ListToDataTable<TRNDocument>(ListDocument);
            DataView dvDocument = dtDocument.DefaultView;
            if (dtDocument.Rows.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(txt_CentreCreateFrom.Text) && string.IsNullOrWhiteSpace(txt_CentreCreateTo.Text))
                {
                    dvDocument.RowFilter = string.Format(@"CreatedDate >= #01/01/{0} 00:00:00#", DateTime.Now.Year);
                    dvDocument = dvDocument.ToTable().DefaultView;
                }
                if (!string.IsNullOrWhiteSpace(txt_CentreCreateFrom.Text))
                {
                    dvDocument.RowFilter = string.Format(@"CreatedDate >= #{0}#", searchCreateFrom.ToString("dd/MM/yyyy 00:00:00", new CultureInfo("th-TH")));
                    dvDocument = dvDocument.ToTable().DefaultView;
                }
                if (!string.IsNullOrWhiteSpace(txt_CentreCreateTo.Text))
                {
                    dvDocument.RowFilter = string.Format(@"CreatedDate <= #{0}#", searchCreateTo.ToString("dd/MM/yyyy 23:59:59"), new CultureInfo("th-TH"));
                    dvDocument = dvDocument.ToTable().DefaultView;
                }
            }
            return dvDocument.ToTable();
        }

        private void SearchGridviewTable(string searchTitle, GridView gv)
        {
            BindViewStateToCentre(false); 
        }

        private void BindViewStateToCentre(bool allowPaging)
        {
            BindRepeaterWithPaging(true); return;
            gv_CentreDocList.AllowPaging = allowPaging;
            if (vs_isSearch)
            {
                gv_CentreDocList.DataSource = vs_GVCentre;
            }
            else
            {
                gv_CentreDocList.DataSource = vs_DocumentList;
            }
            gv_CentreDocList.DataBind();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);
        }

        protected void gv_DocList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string target = vs_CurrentPane;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                //e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_CentreDocList, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';this.style.cursor='pointer';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";

                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    if (i != 5)
                    {
                        //add onclick except first cell and fifth cell;
                        e.Row.Cells[i].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_CentreDocList, "Select$" + e.Row.RowIndex);
                    }
                }


                #region | Document No |
                Label lbl_DocNo = (Label)e.Row.FindControl("lbl_DocNo");
                if (lbl_DocNo != null)
                {
                    object objDocNo = DataBinder.Eval(e.Row.DataItem, "DocNo");
                    if (objDocNo != null)
                    {
                        #region | Default |
                        if (!vs_isSearch)
                        {
                            object objDocType = DataBinder.Eval(e.Row.DataItem, "DocTypeCode");
                            if (objDocType != null && objDocType != DBNull.Value)
                            {
                                if (objDocType.ToString() != "R" && objDocType.ToString() != "L")
                                {
                                    lbl_DocNo.Text = objDocNo.ToString();
                                }
                            }
                        }
                        #endregion
                        #region | Search |
                        else
                        {
                            lbl_DocNo.Text = objDocNo.ToString();
                        }
                        #endregion
                    }
                }
                #endregion

                #region | Title |
                Label lbl_title = (Label)e.Row.FindControl("lbl_title");
                if (lbl_title != null)
                {
                    object objTitle = DataBinder.Eval(e.Row.DataItem, "title");
                    if (objTitle != null && objTitle != DBNull.Value)
                    {
                        lbl_title.Text = objTitle.ToString();
                    }
                    object objSubTitle = DataBinder.Eval(e.Row.DataItem, "SubTitle");
                    if (objSubTitle != null && objSubTitle != DBNull.Value)
                    {
                        if (!string.IsNullOrEmpty(objSubTitle.ToString()))
                            lbl_title.Text = string.Format("{0} ({1})", objTitle.ToString(), objSubTitle.ToString());
                    }
                }
                #endregion

                #region | From Department |
                Label lbl_fromDepartment = (Label)e.Row.FindControl("lbl_fromDepartment");
                if (lbl_fromDepartment != null)
                {
                    object objDeptFrom = DataBinder.Eval(e.Row.DataItem, "FromDepartmentName");
                    if (objDeptFrom != null && objDeptFrom != DBNull.Value)
                    {
                        string sResult = objDeptFrom.ToString();
                        if (objDeptFrom.ToString().Length > 15)
                        {
                            //sResult = string.Format("{0}...", sResult.Substring(0, 15));
                        }
                        lbl_fromDepartment.Text = sResult;
                    }
                }
                #endregion

                #region | Created Date |
                Label lbl_CreatedDate = (Label)e.Row.FindControl("lbl_CreatedDate");
                if (lbl_CreatedDate != null)
                {
                    object objDateData = DataBinder.Eval(e.Row.DataItem, "CreatedDate");
                    if (objDateData != null && objDateData != DBNull.Value)
                    {
                        #region |Default|
                        if (!vs_isSearch)
                        {
                            string date = ((DateTime)objDateData).ToString("dd/MM/yyyy", _ctli);
                            lbl_CreatedDate.Text = date;
                        }
                        #endregion
                        #region | Search |
                        else
                        {
                            lbl_CreatedDate.Text = objDateData.ToString();
                        }
                        #endregion
                    }
                }
                #endregion

                #region | Document Attach |
                LinkButton lbtnAttachment = (LinkButton)e.Row.FindControl("lbtnDocumentAttachName");
                //Label lbl_DocumentAttachName = (Label)e.Row.FindControl("lbl_DocumentAttachName");
                if (lbtnAttachment != null)
                {
                    string docID = DataBinder.Eval(e.Row.DataItem, "DocID").ToString();
                    #region | Default |
                    if (!vs_isSearch)
                    {
                        TRNAttachFileDoc attachDoc = db.TRNAttachFileDocs.FirstOrDefault(x => x.DocID.ToString().Equals(docID));
                        if (attachDoc != null)
                        {
                            //lbtnAttachment.Text = attachDoc.AttachFile;
                            lbtnAttachment.CommandArgument = docID;
                        }
                        else
                        {
                            lbtnAttachment.Text = "-";
                        }
                    }
                    #endregion
                    #region | Search |
                    else
                    {
                        object attachFile = DataBinder.Eval(e.Row.DataItem, "DocumentFile");
                        if (attachFile != null && attachFile != DBNull.Value)
                        {
                            //lbtnAttachment.Text = attachFile.ToString();
                            lbtnAttachment.CommandArgument = docID;

                        }
                    }
                    #endregion
                }

                #region | Backup Label |
                //Label lbl_DocomentAttachName = (Label)e.Row.FindControl("lbl_DocomentAttachName");
                //if (lbl_DocomentAttachName != null)
                //{
                //    #region | Default |
                //    if (!vs_isSearch)
                //    {
                //        List<TRNAttachFileDoc> objAttachDoc = db.TRNAttachFileDocs.Where(x => x.DocID == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "DocID").ToString())).ToList();
                //        if (objAttachDoc != null && objAttachDoc.Count > 0)
                //        {
                //            lbl_DocomentAttachName.Text = objAttachDoc[0].AttachFile;
                //        }
                //        else
                //        {
                //            lbl_DocomentAttachName.Text = "-";
                //        }
                //    }
                //    #endregion
                //    #region | Search |
                //    else
                //    {
                //        var objAttachFile = DataBinder.Eval(e.Row.DataItem, "DocumentFile");
                //        if (objAttachFile != null && objAttachFile != DBNull.Value)
                //        {
                //            lbl_DocomentAttachName.Text = objAttachFile.ToString();
                //        }
                //    }
                //    #endregion
                //} 
                #endregion
                #endregion

                #region | Creator |
                Label lbl_Creator = (Label)e.Row.FindControl("lbl_Creator");
                if (lbl_Creator != null)
                {
                    try
                    {
                        #region | Default |
                        if (!vs_isSearch)
                        {
                            DataTable dtEmp = Extension.GetEmployeeData(this.Page);
                            if (dtEmp != null && dtEmp.Rows.Count > 0)
                            {
                                object objCreator = DataBinder.Eval(e.Row.DataItem, "RequestorID");
                                DataRow dr = dtEmp.Select(string.Format("EMPLOYEEID = {0}", objCreator)).First();
                                if (dr != null)
                                {
                                    lbl_Creator.Text = dr["EmployeeName_TH"].ToString();
                                }
                                else
                                {
                                    Extension.MessageBox(this.Page, "");
                                }
                            }
                        }
                        #endregion
                        #region | Search |
                        else
                        {
                            object objCreator = DataBinder.Eval(e.Row.DataItem, "Creator");
                            if (objCreator != null && objCreator != DBNull.Value)
                            {
                                lbl_Creator.Text = objCreator.ToString();
                            }
                        }
                        #endregion
                    }
                    catch (Exception)
                    {
                    }
                    #endregion

                    #region | Description |
                    Label lbl_Description = (Label)e.Row.FindControl("lbl_Desctiption");
                    if (lbl_Description != null)
                    {
                        object objDes = DataBinder.Eval(e.Row.DataItem, "Description");
                        if (objDes != null && objDes != DBNull.Value)
                        {
                            lbl_Description.Text = objDes.ToString();
                            e.Row.ToolTip = "Description: " + objDes.ToString();
                        }
                    }
                    #endregion

                    #region | Status |
                    Label lbl_Status = (Label)e.Row.FindControl("lbl_Status");
                    if (lbl_Status != null)
                    {
                        object status = DataBinder.Eval(e.Row.DataItem, "Status");
                        lbl_Status.Text = status.ToString().Replace("Completed", "เวียนหนังสือแล้ว").Replace("Cancelled", "เอกสารยกเลิก");//   .Equals("Completed") ? "" : status.ToString();
                    }
                    #endregion

                    #region | To Department |
                    //Label lbl_toDepartment = (Label)e.Row.FindControl("lbl_toDepartmentName");
                    //if (lbl_toDepartment != null)
                    //{
                    //    var objDeptTo = DataBinder.Eval(e.Row.DataItem, "ToDepartmentName");
                    //    if (objDeptTo != null && objDeptTo != DBNull.Value)
                    //    {
                    //        string sResult = objDeptTo.ToString();
                    //        if (objDeptTo.ToString().Length > 15)
                    //        {
                    //            sResult = string.Format("{0}...", sResult.Substring(0, 15));
                    //        }
                    //        lbl_toDepartment.Text = sResult;
                    //    }
                    //}
                    #endregion

                    #region | Modified Date |
                    //Label lbl_ModifiedDate = (Label)e.Row.FindControl("lbl_ModifiedDate");
                    //if (lbl_ModifiedDate != null)
                    //{
                    //    var objModifiedData = DataBinder.Eval(e.Row.DataItem, "ModifiedDate");
                    //    if (objModifiedData != null && objModifiedData != DBNull.Value)
                    //    {
                    //        string date = ((DateTime)objModifiedData).ToString("dd/MM/yyyy", _ctli);
                    //        lbl_ModifiedDate.Text = date;
                    //    }
                    //}
                    #endregion

                    #region | Modified Name |
                    //Label lbl_ModifiedName = (Label)e.Row.FindControl("lbl_ModifiedName");
                    //if (lbl_ModifiedName != null)
                    //{
                    //    var objModifiedData = DataBinder.Eval(e.Row.DataItem, "ModifiedBy");
                    //    if (objModifiedData != null && objModifiedData != DBNull.Value)
                    //    {
                    //        SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(Page, objModifiedData.ToString());
                    //        if (empData != null)
                    //        {
                    //            string nameTH = string.Format("{0}{1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH, empData.LASTNAME_TH);
                    //            string nameEN = string.Format("{0}{1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN, empData.LASTNAME_EN);
                    //            lbl_ModifiedName.Text = vs_Lang == "TH" ? nameTH : nameEN;
                    //        }
                    //    }
                    //}
                    #endregion

                    #region | Deadline |
                    //Label lbl_Deadline = (Label)e.Row.FindControl("lbl_Deadline");
                    //if (lbl_Deadline != null)
                    //{
                    //    var objDeadline = DataBinder.Eval(e.Row.DataItem, "Deadline");
                    //    if (objDeadline != null && objDeadline != DBNull.Value)
                    //    {
                    //        string date = ((DateTime)objDeadline).ToString("dd/MM/yyyy", _ctli);
                    //        lbl_Deadline.Text = date;
                    //    }
                    //}
                    #endregion

                    #region | Document Type |
                    //Label lbl_DocType = (Label)e.Row.FindControl("lbl_DocType");
                    //if (lbl_DocType != null)
                    //{
                    //    var objDocType = DataBinder.Eval(e.Row.DataItem, "DocTypeCode");
                    //    if (objDocType != null && objDocType != DBNull.Value)
                    //    {
                    //        DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", "<View><Query><RowLimit>100000</RowLimit></Query></View>");
                    //        if (!dtDocumentType.DataTableIsNullOrEmpty())
                    //        {
                    //            foreach (DataRow item in dtDocumentType.Rows)
                    //            {
                    //                if (item["DocTypeCode"].ToString().Equals(objDocType.ToString()))
                    //                {
                    //                    lbl_DocType.Text = item["DocTypeName"].ToString();
                    //                }
                    //            }

                    //        }

                    //    }
                    //}
                    #endregion

                    #region | SubOtherType |
                    Label lbl_SubOther = (Label)e.Row.FindControl("lbl_SubOtherType");
                    if (lbl_SubOther != null)
                    {
                        object objOtherType = DataBinder.Eval(e.Row.DataItem, "OtherDocType");
                        if (objOtherType != null && objOtherType != DBNull.Value)
                        {
                            DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", "<View><Query><RowLimit>100000</RowLimit></Query></View>");
                            if (!dtDocumentType.DataTableIsNullOrEmpty())
                            {
                                foreach (DataRow item in dtDocumentType.Rows)
                                {
                                    if (item["Value"].ToString().Equals(objOtherType.ToString()))
                                    {
                                        lbl_SubOther.Text = item["DocTypeName"].ToString();
                                    }
                                }

                            }

                        }
                    }
                    #endregion

                    #region | Priority |
                    //Label lbl_Priority = (Label)e.Row.FindControl("lbl_Priority");
                    //if (lbl_Priority != null)
                    //{
                    //    var priority = DataBinder.Eval(e.Row.DataItem, "Priority");
                    //    string result = priority.ToString();
                    //    switch (result)
                    //    {
                    //        case "Normal": result = "ทั่วไป"; break;
                    //        case "Fast": result = "ด่วน"; break;
                    //        case "Faster": result = "ด่วนมาก"; break;
                    //        case "Fastest": result = "ด่วนที่สุด"; break;
                    //        default: result = ""; break;
                    //    }
                    //    lbl_Priority.Text = result;
                    //}
                    #endregion

                }
            }
        }
        protected void gv_CentreDocList_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow gv_row in gv_CentreDocList.Rows)
            {
                if (gv_row.RowIndex == gv_CentreDocList.SelectedIndex)
                {
                    Label lbl_DocID = (Label)gv_row.Cells[0].FindControl("lbl_DocID");
                    if (lbl_DocID != null)
                    {
                        string url = string.Format("e-Document.aspx?PK={0}", lbl_DocID.Text);
                        Extension.RedirectNewTab(this.Page, url);
                    }
                }
            }
        }
        protected void gv_CentreDocList_Init(object sender, EventArgs e)
        {

        }
        protected override void Render(HtmlTextWriter writer)
        {
            foreach (GridViewRow r in gv_CentreDocList.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation(gv_CentreDocList.UniqueID, "Select$" + r.RowIndex);
                }
            }
            base.Render(writer);
        }
        protected void gv_CentreDocList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_CentreDocList.PageIndex = e.NewPageIndex;
            BindViewStateToCentre(true);
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);
        }

        #region | Custom Paging |

        protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            CurrentPageNumber = Convert.ToInt32(e.CommandArgument) - 1;
            BindRepeaterWithPaging(true);
        }
        protected void btn_Previous_Command(object sender, CommandEventArgs e)
        {
            CurrentPageNumber = CurrentPageNumber - 1;
            BindRepeaterWithPaging(true);
        }

        protected void btn_Next_Command(object sender, CommandEventArgs e)
        {
            CurrentPageNumber = CurrentPageNumber + 1;
            BindRepeaterWithPaging(true);
        }
        private void BindRepeaterWithPaging(bool allowPaging)
        {

            gv_CentreDocList.AllowPaging = allowPaging;

            //Create the PagedDataSource that will be used in paging
            PagedDataSource pgitems = new PagedDataSource();
            if (vs_isSearch)
            {
                pgitems.DataSource = vs_GVCentre;
            }
            else
            {
                pgitems.DataSource = vs_DocumentList.DataTableToList<TRNDocument>();
            }
            pgitems.AllowPaging = true;

            //Control page size from here 
            ItemPerPage = 10;
            pgitems.PageSize = ItemPerPage;
            pgitems.CurrentPageIndex = CurrentPageNumber;

            btn_Previous.Enabled = CurrentPageNumber > 0;
            btn_Next.Enabled = CurrentPageNumber < pgitems.PageCount - 1;

            if (pgitems.PageCount > 1)
            {
                rptPaging.Visible = true;
                ArrayList pages = new ArrayList();
                for (int i = 0; i <= pgitems.PageCount - 1; i++)
                {

                    if (pgitems.PageCount == 1)
                    {
                        pages.Add("1");
                    }
                    else if (i >= CurrentPageNumber)
                    {
                        if ((i > CurrentPageNumber - 2 && i < CurrentPageNumber + 3) || i == pgitems.PageCount - 1)
                        {
                            pages.Add((i + 1).ToString());
                        }
                        else if (i > CurrentPageNumber && i - 4 < CurrentPageNumber)
                        {
                            pages.Add("...");
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            pages.Add((i + 1).ToString());
                        }
                        else if (i + 2 == CurrentPageNumber)
                        {
                            pages.Add("...");
                        }
                        else if (i + 3 > CurrentPageNumber)
                        {
                            pages.Add((i + 1).ToString());
                        }
                    }



                }
                rptPaging.DataSource = pages;
                rptPaging.DataBind();

                RepeaterItemCollection lstPage = rptPaging.Items;
                foreach (RepeaterItem item in lstPage)
                {
                    LinkButton selectedPage = ((LinkButton)item.FindControl("btnPage"));
                    if (selectedPage.CommandArgument == (CurrentPageNumber + 1).ToString())
                    {
                        selectedPage.CssClass += " active";
                        break;
                    }
                }
            }
            else
            {
                rptPaging.Visible = false;
                btn_Previous.Visible = false;
                btn_Next.Visible = false;
            }

            //Finally, set the datasource of the repeater
            try
            {
                gv_CentreDocList.DataSource = pgitems;
                gv_CentreDocList.DataBind();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);
            }
            catch (Exception ex)
            {
                pgitems.CurrentPageIndex = 0;
                gv_CentreDocList.DataSource = pgitems;
                gv_CentreDocList.DataBind();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);
            }
        }
        #endregion
        #endregion

        #region | Validate DateTime |
        protected bool IsDateTimeValid(ref string sMessege)
        {
            if (!string.IsNullOrEmpty(txt_CentreCreateFrom.Text) && !string.IsNullOrEmpty(txt_CentreCreateTo.Text))
            {
                try
                {
                    DateTime? createFrom = Extension.ConvertTextToDate(txt_CentreCreateFrom.Text);
                    DateTime? createTo = Extension.ConvertTextToDate(txt_CentreCreateTo.Text);

                    if ((createFrom == null ? DateTime.MinValue : createFrom) > (createTo == null ? DateTime.MinValue : createTo))
                    {
                        sMessege = "กรุณาใส่วันที่มากกว่าวันที่เริ่มต้น";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    sMessege = "Invalid Format DateTime";
                    return false;
                }
            }
            return true;
        }
        protected void OnDateTimeChanged(object sender, EventArgs e)
        {
            string sMessege = string.Empty;
            TextBox txt_dateTime = (TextBox)sender;
            //DateTime? dateValue = Extension.ConvertTextToDate(txt_dateTime.Text);
            //if (dateValue == null)
            //{ 
            //    txt_dateTime.Text = "";
            //    Extension.MessageBox(this.Page, "Invalid Format DateTime");
            //}

            if (!IsDateTimeValid(ref sMessege))
            {
                txt_dateTime.Text = "";
                Extension.MessageBox(this.Page, sMessege);
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);

        }
        #endregion

        protected void linkBtn_InternalPane_Click(object sender, EventArgs e)
        {
            vs_CurrentPane = "internal";
            FilterByDocType(vs_CurrentDocType, vs_CurrentPane);
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);
        }
        protected void linkBtn_CentrePane_Click(object sender, EventArgs e)
        {
            vs_CurrentPane = "centre";
            FilterByDocType(vs_CurrentDocType, vs_CurrentPane);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", string.Format("selectTab('{0}');", vs_CurrentPane), true);
        }

        protected void ddl_Status_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterByDocType(vs_CurrentDocType, vs_CurrentPane);
        }

        protected void lbtnDocumentAttachName_Click(object sender, EventArgs e)
        {
            LinkButton linkButton = (LinkButton)sender;
            int docID = -1;
            if (Int32.TryParse(linkButton.CommandArgument, out docID))
            {
                //Check authority
                using (DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString()))
                {
                    TRNDocument document = db.TRNDocuments.SingleOrDefault(x => x.DocID.ToString().Equals(docID));
                    if (document == null) lblAuthorizeResult.Text = "Cannot retrieve authorize data.";

                    //if public permission => pass authorize.
                    if (document.PermissionType.Equals("Public"))
                    {
                        lblAuthorizeResult.Text = "Authorized!";
                        Extension.RedirectNewTab(Page, document.AttachFilePath);
                        return;
                    }



                    List<string> authorizedID = new List<string>();

                    #region | Creater && requester |
                    //Creatre & requester
                    authorizedID.Add(document.CreatorID ?? "");
                    authorizedID.Add(document.RequestorID ?? "");
                    #endregion

                    #region | Approval |
                    //Approval
                    List<TRNApprover> listApproval = db.TRNApprovers.Where(x => x.DocID == docID).ToList();
                    if (listApproval != null)
                    {
                        foreach (TRNApprover item in listApproval)
                        {
                            authorizedID.Add(item.EmpID ?? "");
                        }
                    }
                    #endregion

                    #region | Assign |
                    //Assign
                    List<TRNAssign> listAssign = db.TRNAssigns.Where(x => x.DocID == docID).ToList();
                    if (listAssign != null)
                    {
                        foreach (TRNAssign item in listAssign)
                        {
                            authorizedID.Add(item.AssignToID ?? "");
                        }
                    }
                    #endregion

                    #region | History |
                    //History
                    List<TRNHistory> listHistory = db.TRNHistories.Where(x => x.DocID == docID).ToList();
                    if (listHistory != null)
                    {
                        foreach (TRNHistory item in listHistory)
                        {
                            authorizedID.Add(item.EmpID ?? "");

                        }
                    }
                    #endregion

                    #region | Permission |
                    //Permission
                    List<TRNPermission> listPermission = db.TRNPermissions.Where(x => x.DocID == docID).ToList();
                    if (listPermission != null)
                    {
                        foreach (TRNPermission item in listPermission)
                        {
                            authorizedID.Add(item.EmpID ?? "");
                        }
                    }
                    #endregion

                    if (authorizedID.Distinct().ToList().Contains(vs_CurrentUserID))
                    {
                        lblAuthorizeResult.Text = "Authorized!";
                        Extension.RedirectNewTab(Page, document.AttachFilePath);
                    }
                    else
                    {
                        lblAuthorizeResult.Text = "Not Authorized!";
                        JsHelper.togglePopup(Page, modalAuthorize.ClientID, true);
                    }

                }
            }
        }
    }
}
