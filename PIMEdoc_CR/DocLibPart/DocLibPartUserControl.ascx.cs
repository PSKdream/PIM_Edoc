using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace PIMEdoc_CR.DocLibPart
{
    public partial class DocLibPartUserControl : UserControl
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
        private DataTable vs_EmployeeDataList
        {
            get
            {
                return (DataTable)ViewState["vs_EmployeeDataList"];
            }
            set
            {
                ViewState["vs_EmployeeDataList"] = value;
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

                    vs_CurrentUserLoginName = SharedRules.LogonName();
                    vs_CurrentUserID = SharedRules.FindUserID(vs_CurrentUserLoginName, this.Page);
                    if (string.IsNullOrEmpty(vs_CurrentUserID))
                    {
                        vs_CurrentUserID = Request.QueryString["USERID"] != null
                            ? Request.QueryString["USERID"].ToString()
                            : "5050108";
                    }

                    vs_CurrentPane = "centre";
                    vs_CurrentDocType = "P";
                    vs_isCentreAdminSarabun = false;
                    vs_isCentreAdmin = false;

                    gv_CentreDocList.DataSource = new List<string>();
                    gv_CentreDocList.DataBind();
                    vs_EmployeeDataList = Extension.GetEmployeeData(this.Page);
                    FilterByDocType();
                    cs = Page.ClientScript;
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                Extension.MessageBox(this.Page, ex.Message);
            }
        }


        #region | Document Filter |
        private void FilterByDocType()
        {
            try
            {
                DataClassesDataAccessDataContext dataContext = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                List<TRNDocument> ListDocument = dataContext.TRNDocuments.Where(x => (x.DocTypeCode.Equals("P") || x.DocTypeCode.Equals("C"))
                                    && x.Category.Equals("centre")
                                    && x.PermissionType.Equals("Public")
                                    &&
                                        (x.Status.ToLower().Equals("completed")
                                         || (x.Status.ToLower().Equals("cancelled") && !x.DocNo.Equals("Auto Generate")))
                                    )
                                .OrderByDescending(x => x.ModifiedDate)
                    .Take(5).ToList();

                vs_DocumentList = Extension.ListToDataTable<TRNDocument>(ListDocument);
                BindViewStateToCentre();
            }
            catch (Exception ex)
            {
                Extension.MessageBox(this.Page, ex.Message.Replace('\'', ' '));
            }
        }

        private void BindViewStateToCentre()
        {
            gv_CentreDocList.DataSource = vs_DocumentList;
            gv_CentreDocList.DataBind();
        }

        protected void gv_DocList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string target = vs_CurrentPane;
            if (e.Row.RowType == DataControlRowType.DataRow)
            { 
                //e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_CentreDocList, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';this.style.cursor='pointer';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                e.Row.ToolTip = "Click for selecting this row.";

                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    //add onclick except first cell
                    e.Row.Cells[i].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_CentreDocList, "Select$" + e.Row.RowIndex);
                }


                #region | Document Attach |
                Label lbl_DocomentAttachName = (Label)e.Row.FindControl("lbl_DocomentAttachName");
                if (lbl_DocomentAttachName != null)
                {
                    DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                    int ID = 0;
                    ID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "DocID"));

                    List<TRNAttachFileDoc> objAttachDoc = db.TRNAttachFileDocs.Where(x => x.DocID == ID).ToList();
                    if (objAttachDoc != null && objAttachDoc.Count > 0)
                    {
                        foreach (TRNAttachFileDoc doc in objAttachDoc)
                        {
                            lbl_DocomentAttachName.Text = doc.AttachFile;
                        }
                    }
                    else
                    {
                        lbl_DocomentAttachName.Text = "-";
                    }
                }
                #endregion

                #region | Document No |
                Label lbl_DocNo = (Label)e.Row.FindControl("lbl_DocNo");
                if (lbl_DocNo != null)
                {
                    object objDocNo = DataBinder.Eval(e.Row.DataItem, "DocNo");
                    if (objDocNo != null)
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

                #region | To Department |
                //Label lbl_toDepartment = (Label)e.Row.FindControl("lbl_toDepartmentName");
                //if (lbl_toDepartment != null)
                //{
                //    object objDeptTo = DataBinder.Eval(e.Row.DataItem, "ToDepartmentName");
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

                #region | Created Date |
                Label lbl_CreatedDate = (Label)e.Row.FindControl("lbl_CreatedDate");
                if (lbl_CreatedDate != null)
                {
                    object objDateData = DataBinder.Eval(e.Row.DataItem, "CreatedDate");
                    if (objDateData != null && objDateData != DBNull.Value)
                    {
                        string date = ((DateTime)objDateData).ToString("dd/MM/yyyy", _ctli);
                        lbl_CreatedDate.Text = date;
                    }
                }
                #endregion

                #region | Modified Date |
                //Label lbl_ModifiedDate = (Label)e.Row.FindControl("lbl_ModifiedDate");
                //if (lbl_ModifiedDate != null)
                //{
                //    object objModifiedData = DataBinder.Eval(e.Row.DataItem, "ModifiedDate");
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
                //    object objModifiedData = DataBinder.Eval(e.Row.DataItem, "ModifiedBy");
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
                //    object objDeadline = DataBinder.Eval(e.Row.DataItem, "Deadline");
                //    if (objDeadline != null && objDeadline != DBNull.Value)
                //    {
                //        string date = ((DateTime)objDeadline).ToString("dd/MM/yyyy", _ctli);
                //        lbl_Deadline.Text = date;
                //    }
                //}
                #endregion
                
                #region | Document Type |
                Label lbl_DocType = (Label)e.Row.FindControl("lbl_DocType");
                if (lbl_DocType != null)
                {
                    object objDocType = DataBinder.Eval(e.Row.DataItem, "DocTypeCode");
                    if (objDocType != null && objDocType != DBNull.Value)
                    {
                        DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", "<View><Query><RowLimit>100000</RowLimit></Query></View>");
                        if (!dtDocumentType.DataTableIsNullOrEmpty())
                        {
                            foreach (DataRow item in dtDocumentType.Rows)
                            {
                                if (item["DocTypeCode"].ToString().Equals(objDocType.ToString()))
                                {
                                    lbl_DocType.Text = item["DocTypeName"].ToString();
                                }
                            }

                        }

                    }
                }
                #endregion

                #region | Creator |
                Label lbl_Creator = (Label)e.Row.FindControl("lbl_Creator");
                if (lbl_Creator != null)
                {                    
                    if (vs_EmployeeDataList != null && vs_EmployeeDataList.Rows.Count > 0)
                    {
                        object objCreator = DataBinder.Eval(e.Row.DataItem, "RequestorID");
                        if (objCreator != null)
                        {
                            var QueryRequestorID = string.Format("EMPLOYEEID = '{0}'", objCreator.ToString());
                            var dr = vs_EmployeeDataList.Select(QueryRequestorID);
                            if (dr != null)
                            {
                                if (dr.Count() > 0)
                                {
                                    lbl_Creator.Text = dr.FirstOrDefault()["EmployeeName_TH"].ToString();
                                } 
                            }
                            else
                            {
                                Extension.MessageBox(this.Page, "");
                            }
                        }

                    }
                }
                #endregion

                #region | Priority |
                //Label lbl_Priority = (Label)e.Row.FindControl("lbl_Priority");
                //if (lbl_Priority != null)
                //{
                //    object priority = DataBinder.Eval(e.Row.DataItem, "Priority");
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

                #region | Status |
                //Label lbl_Status = (Label)e.Row.FindControl("lbl_Status");
                //if (lbl_Status != null)
                //{
                //    object status = DataBinder.Eval(e.Row.DataItem, "Status");
                //    lbl_Status.Text = status.ToString().Equals("Completed") ? "เวียนหนังสือแล้ว" : status.ToString();
                //}
                #endregion 
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
                        Extension.Redirect(this.Page, url);
                    }
                }
            }
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
            BindViewStateToCentre();
        }
        #endregion
    }
}
