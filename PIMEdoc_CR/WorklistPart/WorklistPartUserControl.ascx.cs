
using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


namespace PIMEdoc_CR.WorklistPart
{
    public partial class WorklistPartUserControl : UserControl
    {
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
        private string vs_DocID
        {
            get
            {
                return (string)ViewState["vs_DocID"];
            }
            set
            {
                ViewState["vs_DocID"] = value;
            }
        }
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
        private string vs_ConnectionString
        {
            get
            {
                return (string)ViewState["vs_ConnectionString"];
            }
            set
            {
                ViewState["vs_ConnectionString"] = value;
            }
        }
        private int vs_SelectedItems
        {
            get
            {
                return ViewState["vs_SelectedItems"] == null ? 0 : (int)ViewState["vs_SelectedItems"];
            }
            set
            {
                ViewState["vs_SelectedItems"] = value;
            }
        }

        private readonly CultureInfo _clti = Extension._ctli;
        private DataTable vs_dtDocument
        {
            get
            {
                return (DataTable)ViewState["vs_dtDocument"];
            }
            set
            {
                ViewState["vs_dtDocument"] = value;
            }
        }

        #region | Page Load & Initial |
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    string logonName = SharedRules.LogonName();
                    vs_CurrentUserID = SharedRules.FindUserID(logonName, this.Page);
                    if (string.IsNullOrEmpty(vs_CurrentUserID))
                    {
                        vs_CurrentUserID = Request.QueryString["USERID"] != null ? Request.QueryString["USERID"].ToString() : "5050108";
                    }
                    gv_WorkList.DataSource = new List<string>();
                    gv_WorkList.DataBind();

                    initialData();

                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                Extension.MessageBox(this.Page, ex.Message);
            }

        }
        protected void initialData()
        {
            vs_Lang = "TH";
            DataTable dtDb = Extension.GetDataTable("SiteSetting");
            {
                if (!dtDb.DataTableIsNullOrEmpty())
                {
                    vs_ConnectionString = dtDb.Rows[0]["Value"].ToString();
                }
            }
            using (DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(vs_ConnectionString))
            {

                #region | Find Delegation |
                List<TRNDocument> listDelegateDocument = new List<TRNDocument>();
                List<TRNDelegate> listDelegate = db.TRNDelegates
                    .Where(
                        x => (x.DelegateToID.Equals(vs_CurrentUserID)) &&
                             ((x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now) || object.Equals(x.DateFrom, x.DateTo))
                        )
                    .ToList();
                if (listDelegate != null && listDelegate.Count > 0)
                {

                    List<v_TRNDelegateDetail> listDelegateDetail = db.v_TRNDelegateDetails.Where(x => x.IsActive ?? false).ToList();
                    foreach (TRNDelegate @delegate in listDelegate)
                    {
                        #region | By DocID |
                        List<string> delegateDocID = listDelegateDetail.Where(x => x.DelegateID == @delegate.DelegateID && !(x.IsByDocType ?? false)).Select(x => x.DocID).ToList();
                        if (delegateDocID != null && delegateDocID.Count > 0)
                        {
                            List<TRNDocument> documentByID = db.TRNDocuments.Where(x => delegateDocID.Contains(x.DocID.ToString()) && x.WaitingFor.Equals(@delegate.ApproverID)).ToList();
                            listDelegateDocument = listDelegateDocument.Union(documentByID).ToList();
                        }
                        #endregion

                        #region | By DocType |
                        List<string> delegateDocType = listDelegateDetail.Where(x => x.DelegateID == @delegate.DelegateID && (x.IsByDocType ?? false)).Select(x => x.DocType).ToList();
                        if (delegateDocType != null && delegateDocType.Count > 0)
                        {
                            List<TRNDocument> documentByDocType = db.TRNDocuments.Where(x => delegateDocType.Contains(x.DocTypeCode)).ToList();
                            documentByDocType = documentByDocType.Where(x => x.WaitingFor.Split(',').Contains(@delegate.ApproverID)).ToList();
                            listDelegateDocument = listDelegateDocument.Union(documentByDocType).ToList();
                        }
                        #endregion
                    }

                }
                #endregion
                #region | Get relate document |
                List<TRNDocument> ListDocument = db.TRNDocuments
                            .Where(x =>
                                    x.Status.Equals(Extension._Draft) ||
                                    x.Status.Equals(Extension._Rework) ||
                                    x.Status.Equals(Extension._WaitForRequestorReview) ||
                                    x.Status.Equals(Extension._WaitForComment) ||
                                    x.Status.Equals(Extension._WaitForApprove) ||
                                    x.Status.Equals(Extension._WaitForAdminCentre)
                                  )
                            .ToList();
                ListDocument = ListDocument
                    .Where(x => x.WaitingFor.Replace(" ", "").Split(',').Contains(vs_CurrentUserID))
                    .ToList();
                #endregion
                #region | Union Releated Document with Delegation |
                if (listDelegateDocument != null && listDelegateDocument.Count > 0)
                {
                    listDelegateDocument = listDelegateDocument.Distinct().ToList();
                    ListDocument = ListDocument.Union(listDelegateDocument).ToList();
                }
                #endregion


                #region | Backup |
                //List<TRNDocument> ListDocument = db.TRNDocuments.ToList();
                //foreach (var item in ListDocument)
                //{
                //    List<v_TRNDelegateDetail> listDelegate = new List<v_TRNDelegateDetail>();
                //    listDelegate = db.v_TRNDelegateDetails
                //        .Where(x => x.DelegateToID == vs_CurrentUserID).ToList();
                //    listDelegate = listDelegate
                //        .Where(x => item.WaitingFor.Replace(" ", "").Split(',').Contains(x.ApproverID)).ToList();
                //    listDelegate = listDelegate
                //        .Where(x => x.DepartmentID == item.WaitingForDeptID).ToList();
                //    listDelegate = listDelegate
                //        .Where(x => Convert.ToBoolean(x.IsActive)).ToList();
                //    listDelegate = listDelegate.Where(x =>
                //    {
                //        var i = Convert.ToBoolean(x.IsByDocType);
                //        return i ? x.DocType == item.DocTypeCode : x.DocID == item.DocID.ToString();
                //    }
                //        ).ToList();
                //    listDelegate = listDelegate
                //        .Where(x => (DateTime.Now >= x.DateFrom && (x.DateTo == null || DateTime.Now <= x.DateTo))).ToList();
                //    if (listDelegate.Count > 0)
                //    {
                //        item.WaitingFor = vs_CurrentUserID;
                //    }
                //}

                //ListDocument = ListDocument
                //    .Where(x => x.WaitingFor.Split(',').Contains(vs_CurrentUserID) &&
                //           (x.Status.Equals("Draft") ||
                //            x.Status.Equals("Rework") ||
                //            x.Status.Equals("Wait for Requestor Review") ||
                //            x.Status.Equals("Wait for Comment") ||
                //            x.Status.Equals("Wait for Approve") ||
                //            x.Status.Equals("Wait for Admin Centre"))
                //          )
                //        .OrderByDescending(x => x.CreatedDate)
                //        .ToList(); 
                #endregion



                vs_dtDocument = Extension.ListToDataTable<TRNDocument>(ListDocument);
            }
            BindingGv(vs_dtDocument);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            foreach (GridViewRow r in gv_WorkList.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation(gv_WorkList.UniqueID, "Select$" + r.RowIndex);
                }
            }
            base.Render(writer);
        }

        #endregion

        #region | WorkList Gridview |
        protected void gv_WorkList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_WorkList, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';this.style.cursor='pointer';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                e.Row.ToolTip = "Click for selecting this row.";

                using (DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString()))
                {
                    #region | Attach Doc |
                    HyperLink hpl_AttachDoc = (HyperLink)e.Row.FindControl("hpl_AttachDoc");
                    Label lbl_DocID = (Label)e.Row.FindControl("lbl_DocID");
                    if (hpl_AttachDoc != null && lbl_DocID != null)
                    {
                        object docID = DataBinder.Eval(e.Row.DataItem, "DocID");

                        List<TRNAttachFileDoc> listAttachDoc =
                            db.TRNAttachFileDocs.Where(x => x.DocID == Convert.ToInt32(docID.ToString()) && x.IsPrimary == "Y").ToList();
                        if (listAttachDoc.Count > 0)
                        {
                            TRNAttachFileDoc attachDoc = listAttachDoc[0];
                            if (attachDoc != null)
                            {
                                hpl_AttachDoc.Text = attachDoc.AttachFile.ToString();
                                hpl_AttachDoc.NavigateUrl = attachDoc.AttachFIlePath.ToString();
                            }
                        }


                    }
                    #endregion

                    #region | Document Type |
                    Label lbl_DocType = (Label)e.Row.FindControl("lbl_DocType");
                    if (lbl_DocType != null)
                    {
                        object objDocType = DataBinder.Eval(e.Row.DataItem, "DocTypeCode");
                        if (objDocType != null && objDocType != DBNull.Value)
                        {
                            DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", "<Where><And><Eq><FieldRef Name='Level' /><Value Type='Number'>0</Value></Eq><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq></And></Where>");
                            if (!dtDocumentType.DataTableIsNullOrEmpty())
                            {
                                foreach (DataRow item in dtDocumentType.Rows)
                                {
                                    if (item["Value"].ToString().Equals(objDocType.ToString()))
                                    {
                                        lbl_DocType.Text = item["DocTypeName"].ToString();
                                    }
                                }

                            }

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
                        object objTitle = DataBinder.Eval(e.Row.DataItem, "Title");
                        if (objTitle != null)
                        {
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
                    }
                    #endregion

                    #region | From Department |
                    Label lbl_FromDepartment = (Label)e.Row.FindControl("lbl_FromDepartment");
                    if (lbl_FromDepartment != null)
                    {
                        object objFromDeptName = DataBinder.Eval(e.Row.DataItem, "FromDepartmentName");
                        if (objFromDeptName != null && objFromDeptName != DBNull.Value)
                        {
                            lbl_FromDepartment.Text = objFromDeptName.ToString().Split(',')[0];
                            if (objFromDeptName.ToString().Split(',').Length > 1)
                            {
                                lbl_FromDepartment.Text += ", ...";
                            }
                        }
                    }
                    #endregion

                    #region | To Department |
                    Label lbl_ToDepartment = (Label)e.Row.FindControl("lbl_ToDepartment");
                    if (lbl_ToDepartment != null)
                    {
                        object objToDeptName = DataBinder.Eval(e.Row.DataItem, "ToDepartmentName");
                        if (objToDeptName != null && objToDeptName != DBNull.Value)
                        {
                            lbl_ToDepartment.Text = objToDeptName.ToString().Split(',')[0];
                            if (objToDeptName.ToString().Split(',').Length > 1)
                            {
                                lbl_ToDepartment.Text += ", ...";
                            }
                        }
                    }
                    #endregion

                    #region | Category |
                    Label lblCategory = (Label)e.Row.FindControl("lbl_Category");
                    if (lblCategory != null)
                    {
                        object objType = DataBinder.Eval(e.Row.DataItem, "Category");
                        if (objType != null && objType != DBNull.Value)
                        {
                            DataTable dtCategory = Extension.GetDataTable("MstCategory");
                            if (!dtCategory.DataTableIsNullOrEmpty())
                            {
                                foreach (DataRow item in dtCategory.Rows)
                                {
                                    if (item["Value"].ToString().Equals(objType.ToString()))
                                    {
                                        lblCategory.Text = item["CategoryName"].ToString();
                                    }
                                }

                            }

                        }
                    }
                    #endregion

                    #region | Create Date |
                    Label lblCreateDate = (Label)e.Row.FindControl("lbl_CreatedDate");
                    if (lblCreateDate != null)
                    {
                        object objDateData = DataBinder.Eval(e.Row.DataItem, "CreatedDate");
                        if (objDateData != null && objDateData != DBNull.Value)
                        {
                            string date = ((DateTime)objDateData).ToString("dd/MM/yyyy", _clti);
                            lblCreateDate.Text = date;
                        }
                    }
                    #endregion

                    #region | Requestor |
                    Label lblRequeestor = (Label)e.Row.FindControl("lbl_Requestor");
                    if (lblRequeestor != null)
                    {
                        object objReq = DataBinder.Eval(e.Row.DataItem, "RequestorID");
                        if (objReq != null && objReq != DBNull.Value)
                        {
                            SpecificEmployeeData.RootObject emp = Extension.GetSpecificEmployeeFromTemp(this.Page, objReq.ToString());
                            if (emp != null)
                            {
                                string nameTH = string.Format("{0}{1} {2}", emp.PREFIX_TH, emp.FIRSTNAME_TH, emp.LASTNAME_TH);
                                string nameEN = string.Format("{0}{1} {2}", emp.PREFIX_EN, emp.FIRSTNAME_EN, emp.LASTNAME_EN);
                                lblRequeestor.Text = vs_Lang == "EN" ? nameEN : nameTH;
                            }
                        }
                    }
                    #endregion

                    #region | Waiting For |
                    Label lblWaitingFor = (Label)e.Row.FindControl("lbl_WaitingFor");
                    if (lblWaitingFor != null)
                    {
                        object objWaiting = DataBinder.Eval(e.Row.DataItem, "WaitingFor");
                        if (objWaiting != null && objWaiting != DBNull.Value)
                        {
                            string waitingFor = "";
                            foreach (string waiting in objWaiting.ToString().Trim(' ').Split(','))
                            {
                                SpecificEmployeeData.RootObject emp = Extension.GetSpecificEmployeeFromTemp(this.Page, waiting);
                                if (emp != null)
                                {
                                    string ex = "<br/>";
                                    if (string.IsNullOrWhiteSpace(waitingFor))
                                    {
                                        ex = "";
                                    }
                                    string nameTH = string.Format("{0}{1} {2}", emp.PREFIX_TH, emp.FIRSTNAME_TH, emp.LASTNAME_TH);
                                    string nameEN = string.Format("{0}{1} {2}", emp.PREFIX_EN, emp.FIRSTNAME_EN, emp.LASTNAME_EN);
                                    if (!string.IsNullOrWhiteSpace(nameTH))
                                    {
                                        waitingFor += ex;
                                        waitingFor += vs_Lang == "EN" ? nameEN : string.Format("{0} ({1})", nameTH, emp.EMPLOYEEID);
                                    }
                                }

                            }
                            lblWaitingFor.Text = waitingFor;
                        }
                    }
                    #endregion
                }
            }


        }

        protected void gv_WorkList_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow gv_row in gv_WorkList.Rows)
            {
                if (gv_row.RowIndex == gv_WorkList.SelectedIndex)
                {
                    Label lbl_DocID = (Label)gv_row.Cells[0].FindControl("lbl_DocID");
                    if (lbl_DocID != null) Response.Redirect(string.Format("e-Document.aspx?PK={0}", lbl_DocID.Text));
                }
            }
        }

        protected void gv_WorkList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_WorkList.PageIndex = e.NewPageIndex;
            BindingGv(vs_dtDocument);
        }

        protected void gv_WorkList_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
        }

        private void BindingGv(DataTable dtDocument)
        {
            gv_WorkList.DataSource = dtDocument;
            gv_WorkList.DataBind();
            if (dtDocument.Rows.Count == 0)
            {
                panel_Container.Visible = false;
            }
        }
        #endregion
    }
}
