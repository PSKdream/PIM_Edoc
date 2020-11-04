
using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace PIMEdoc_CR.Default.e_Document.Worklist
{
    public partial class WorklistUserControl : UserControl
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
        private string vs_CurrentUserDepID
        {
            get
            {
                return (string)ViewState["vs_CurrentUserDepID"];
            }
            set
            {
                ViewState["vs_CurrentUserDepID"] = value;
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

        private string vs_Role
        {
            get
            {
                return (string)ViewState["vs_Role"];
            }
            set
            {
                ViewState["vs_Role"] = value;
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
        private DataClassesDataAccessDataContext db;

        #region | Page Load & Initial |
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    //vs_CurrentUserID = Request.QueryString["USERID"] != null ? Request.QueryString["USERID"].ToString() : "5050108";
                    //vs_PK = Request.QueryString["PK"] != null ? Request.QueryString["PK"].ToString() : string.Empty;

                    string logonName = Rule.SharedRules.LogonName();
                    vs_CurrentUserID = Rule.SharedRules.FindUserID(logonName, this.Page);
                    if (!string.IsNullOrEmpty(Request.QueryString["USERID"]))
                    {
                        vs_CurrentUserID = Request.QueryString["USERID"].ToString();
                    }
                    if (!string.IsNullOrWhiteSpace(vs_CurrentUserID))
                    {
                        try
                        {
                            SpecificEmployeeData.RootObject emp = Extension.GetSpecificEmployeeFromTemp(this.Page, vs_CurrentUserID);
                            vs_CurrentUserDepID = "";
                            if (emp != null)
                            {
                                if (emp.RESULT != null)
                                {
                                    vs_CurrentUserDepID = emp.RESULT.FirstOrDefault().DEPARTMENT_ID;
                                }
                            }
                        }
                        catch
                        {

                        }

                    }

                    initialData();

                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                Extension.MessageBox(this.Page, ex.Message + string.Format("emp : {0} | depid : {1}", vs_CurrentUserID, vs_CurrentUserDepID));
            }

        }
        protected void initialData()
        {
            vs_CurrentPane = "centre";
            vs_Lang = "TH";

            string spGroup = "Admin_ITEdoc";
            if (!string.IsNullOrEmpty(spGroup))
            {
                List<string> userList = Rule.SharedRules.GetAllUserInGroup(spGroup);
                DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
                if (!dtEmp.DataTableIsNullOrEmpty())
                {
                    foreach (string user in userList)
                    {
                        try
                        {
                            if (vs_CurrentUserID == SharedRules.FindUserID(user, this.Page))
                            {
                                vs_Role = "ITAdmin";
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Extension.LogWriter.Write(ex);
                        }
                    }
                }
            }

            DataTable dtDb = Extension.GetDataTable("SiteSetting");
            {
                if (!dtDb.DataTableIsNullOrEmpty())
                {
                    vs_ConnectionString = dtDb.Rows[0]["Value"].ToString();
                }
            }
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(vs_ConnectionString);

            DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
            if (!dtDocType.DataTableIsNullOrEmpty())
            {
                ddl_DocType.DataSource = dtDocType;
                ddl_DocType.DataTextField = "DocTypeName";
                ddl_DocType.DataValueField = "Value";
                ddl_DocType.DataBind();
                ddl_DocType.Items.Insert(0, new ListItem("ทั้งหมด", ""));
            }


            #region | Backup |
            //List<TRNDocument> ListDocument = db.TRNDocuments.ToList();
            //string docType = ddl_DocType.SelectedValue;
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
            //           (x.Status.Equals(Extension._Draft) ||
            //            x.Status.Equals(Extension._Rework) ||
            //            x.Status.Equals(Extension._WaitForRequestorReview) ||
            //            x.Status.Equals(Extension._WaitForComment) ||
            //            x.Status.Equals(Extension._WaitForApprove) ||
            //            x.Status.Equals(Extension._WaitForAdminCentre)
            //            )
            //          )
            //        .OrderByDescending(x => x.CreatedDate)
            //        .ToList();

            //if (!ddl_DocType.SelectedValue.Equals(""))
            //{
            //    ListDocument = ListDocument.Where(x => x.DocTypeCode.Equals(docType)).OrderByDescending(x => x.CreatedDate).ToList();
            //}


            //vs_dtDocument = Extension.ListToDataTable<TRNDocument>(ListDocument);
            //BindRepeaterWithPaging();
            #endregion

            btnSearch_Click(null, new EventArgs());

            //lbl_ItemCounter.Text = ListDocument.Count.ToString() + (ListDocument.Count > 1 ? " items" : " item");
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

        #region | Validate DateTime |
        protected bool IsDateTimeValid(ref string sMessege)
        {
            if (!string.IsNullOrEmpty(txt_RequestDateFrom.Text) && !string.IsNullOrEmpty(txt_RequestDateTo.Text))
            {
                try
                {
                    DateTime requestFrom = DateTime.ParseExact(txt_RequestDateFrom.Text, "dd/MM/yyyy",
                        new CultureInfo("en-GB"));
                    DateTime requestTo = DateTime.ParseExact(txt_RequestDateTo.Text, "dd/MM/yyyy",
                        new CultureInfo("en-GB"));
                    if (requestFrom > requestTo)
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
            DateTime dateValue;
            if (!string.IsNullOrEmpty(txt_dateTime.Text))
            {
                if (!DateTime.TryParseExact(txt_dateTime.Text, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out dateValue))
                {
                    txt_dateTime.Text = "";
                    Extension.MessageBox(this.Page, "Invalid Format DateTime");
                }
                if (!IsDateTimeValid(ref sMessege))
                {
                    txt_dateTime.Text = "";
                    Extension.MessageBox(this.Page, sMessege);
                }
            }
            else
            {

            }
        }
        #endregion

        #region | WorkList Gridview |
        protected void gv_WorkList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_WorkList, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';this.style.cursor='pointer';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                //e.Row.ToolTip = "Click for selecting this row.";


                for (int i = 1; i < e.Row.Cells.Count - 1; i++)
                {
                    //add onclick except first, second and last cell
                    if (i != 7)
                    {
                        e.Row.Cells[i].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_WorkList, "Select$" + e.Row.RowIndex);
                    }
                }
                db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

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

                #region | Description |
                Label lbl_Description = (Label)e.Row.FindControl("lbl_Description");
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
            if (e.CommandName != "ShowHistory") return;

            vs_DocID = e.CommandArgument.ToString();
            DataClassesDataAccessDataContext dataContext = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
            List<TRNHistory> listHistory = dataContext.TRNHistories.Where(x => x.DocID == Convert.ToInt32(vs_DocID)).ToList();
            if (listHistory.Count > 0)
            {
                gv_History.DataSource = listHistory;
                gv_History.DataBind();
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popHistory", "$('#showHistory').modal('show');", true);
        }


        private void BindingGv(DataTable dtDocument)
        {
            gv_WorkList.DataSource = dtDocument;
            gv_WorkList.DataBind();
        }
        #endregion

        #region | Search Panel |
        #region | Back up Search Panel |
        //protected void btnSearch_Click_(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        db = new DataClassesDataAccessDataContext(vs_ConnectionString);
        //        List<TRNDocument> ListDocument = new List<TRNDocument>();

        //        #region | Filter TaskGroup |
        //        if (ddl_TaskGroup.SelectedValue.Equals("To Do List"))
        //        {
        //            #region | Find Delegation |
        //            List<TRNDocument> listDelegateDocument = new List<TRNDocument>();
        //            List<TRNDelegate> listDelegate = db.TRNDelegates
        //                .Where(
        //                    x => (x.DelegateToID.Equals(vs_CurrentUserID)) &&
        //                         ((x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now) || object.Equals(x.DateFrom, x.DateTo))
        //                    )
        //                .ToList();
        //            if (listDelegate != null && listDelegate.Count > 0)
        //            {

        //                List<v_TRNDelegateDetail> listDelegateDetail = db.v_TRNDelegateDetails.Where(x => x.IsActive ?? false).ToList();
        //                foreach (TRNDelegate @delegate in listDelegate)
        //                {
        //                    #region | By DocID |
        //                    List<string> delegateDocID = listDelegateDetail.Where(x => x.DelegateID == @delegate.DelegateID && !(x.IsByDocType ?? false)).Select(x => x.DocID).ToList();
        //                    if (delegateDocID != null && delegateDocID.Count > 0)
        //                    {
        //                        List<TRNDocument> documentByID = db.TRNDocuments.Where(x => delegateDocID.Contains(x.DocID.ToString()) && x.WaitingFor.Equals(@delegate.ApproverID)).ToList();
        //                        listDelegateDocument = listDelegateDocument.Union(documentByID).ToList();
        //                    }
        //                    #endregion

        //                    #region | By DocType |
        //                    List<string> delegateDocType = listDelegateDetail.Where(x => x.DelegateID == @delegate.DelegateID && (x.IsByDocType ?? false)).Select(x => x.DocType).ToList();
        //                    if (delegateDocType != null && delegateDocType.Count > 0)
        //                    {
        //                        List<TRNDocument> documentByDocType = db.TRNDocuments.Where(x => delegateDocType.Contains(x.DocTypeCode)).ToList();
        //                        documentByDocType = documentByDocType.Where(x => x.WaitingFor.Split(',').Contains(@delegate.ApproverID)).ToList();
        //                        listDelegateDocument = listDelegateDocument.Union(documentByDocType).ToList();
        //                    }
        //                    #endregion
        //                }

        //            }
        //            #endregion

        //            #region | Get To Do Document |
        //            ListDocument = db.TRNDocuments
        //                       .Where(x =>
        //                               x.Status.Equals(Extension._Draft) ||
        //                               x.Status.Equals(Extension._Rework) ||
        //                               x.Status.Equals(Extension._WaitForRequestorReview) ||
        //                               x.Status.Equals(Extension._WaitForComment) ||
        //                               x.Status.Equals(Extension._WaitForApprove) ||
        //                               x.Status.Equals(Extension._WaitForAdminCentre)
        //                             )
        //                       .ToList();
        //            ListDocument = ListDocument
        //                .Where(x => x.WaitingFor.Replace(" ", "").Split(',').Contains(vs_CurrentUserID))
        //                .ToList();
        //            #endregion

        //            #region | Combine Delegation with To Do Document |
        //            if (listDelegateDocument != null && listDelegateDocument.Count > 0)
        //            {
        //                listDelegateDocument = listDelegateDocument.Distinct().ToList();
        //                ListDocument = ListDocument.Union(listDelegateDocument).ToList();
        //            }
        //            #endregion
        //        }
        //        else if (ddl_TaskGroup.SelectedValue.Equals("In Process"))
        //        {
        //            ListDocument = db.TRNDocuments
        //                .Where(
        //                         x => (
        //                                x.RequestorID == vs_CurrentUserID ||
        //                                x.CreatorID == vs_CurrentUserID
        //                              ) &&
        //                              (
        //                                x.Status.Equals(Extension._WaitForRequestorReview) ||
        //                                x.Status.Equals(Extension._WaitForComment) ||
        //                                x.Status.Equals(Extension._WaitForApprove) ||
        //                                x.Status.Equals(Extension._WaitForAdminCentre) ||
        //                                x.Status.Equals(Extension._RequestCancel)
        //                              )
        //                      )
        //                .ToList();
        //        }
        //        else if (ddl_TaskGroup.SelectedValue.Equals("Completed"))
        //        {
        //            ListDocument = db.TRNDocuments
        //                .Where(
        //                         x => (x.RequestorID == vs_CurrentUserID ||
        //                              x.CreatorID == vs_CurrentUserID) &&
        //                              x.Status.Equals(Extension._Completed)
        //                      )
        //                .ToList();
        //        }
        //        else if (ddl_TaskGroup.SelectedValue.Equals("Rejected"))
        //        {
        //            ListDocument = db.TRNDocuments
        //                .Where(
        //                         x => (x.RequestorID == vs_CurrentUserID &&
        //                              x.Status.Equals(Extension._Rejected)) ||
        //                              (x.CreatorID == vs_CurrentUserID &&
        //                              x.Status.Equals(Extension._Rejected))
        //                      )
        //                .ToList();
        //        }
        //        else if (ddl_TaskGroup.SelectedValue.Equals("Cancelled"))
        //        {
        //            ListDocument = db.TRNDocuments
        //                .Where(
        //                         x => (x.RequestorID == vs_CurrentUserID &&
        //                              x.Status.Equals(Extension._Cancelled)) ||
        //                              (x.CreatorID == vs_CurrentUserID &&
        //                              x.Status.Equals(Extension._Cancelled))
        //                      )
        //                .ToList();
        //        }
        //        else if (ddl_TaskGroup.SelectedValue.Equals("History"))
        //        {
        //            if (vs_Role != "ITAdmin")
        //            {
        //                List<TRNApprover> ListApprover = db.TRNApprovers.Where(x => x.EmpID == vs_CurrentUserID).ToList();
        //                List<TRNHistory> listHistory = db.TRNHistories.Where(x => x.EmpID == vs_CurrentUserID).ToList();
        //                List<int?> ListDocIDParticipate = new List<int?>();
        //                if (ListApprover != null)
        //                {
        //                    if (ListApprover.Count() > 0)
        //                    {
        //                        ListDocIDParticipate.AddRange(ListApprover.Select(x => x.DocID).ToList());
        //                    }
        //                    Extension.LogWriter.Write(new Exception(string.Format("ListApprover Count : {0} ", ListApprover.Count.ToString())));
        //                }
        //                if (listHistory != null)
        //                {
        //                    if (listHistory != null)
        //                    {
        //                        ListDocIDParticipate.AddRange(listHistory.Select(x => x.DocID).ToList());
        //                    }
        //                    Extension.LogWriter.Write(new Exception(string.Format("listHistory Count : {0} ", listHistory.Count.ToString())));
        //                }
        //                ListDocIDParticipate = ListDocIDParticipate.Distinct().ToList();
        //                Extension.LogWriter.Write(new Exception(string.Format("ListDocIDParticipate Count : {0} ", ListDocIDParticipate.Count.ToString())));


        //                ListDocument = db.TRNDocuments.Where(x => ListDocIDParticipate.Contains(x.DocID) && !x.WaitingFor.Contains(vs_CurrentUserID)).ToList();
        //                //(from m in db.TRNDocuments
        //                //where ListDocIDParticipate.Contains(m.DocID) && m.WaitingFor != vs_CurrentUserID
        //                //select m).Take(20).ToList();
        //            }
        //            else
        //            {
        //                ListDocument = db.TRNDocuments.ToList();
        //            }
        //        }
        //        else if (ddl_TaskGroup.SelectedValue.Equals("Assign"))
        //        {
        //            List<Int32> listAssign = db.TRNAssigns.Where(x => x.AssignToID == vs_CurrentUserID).Select(x => x.DocID ?? 0).ToList();
        //            ListDocument = db.TRNDocuments.Where(x => listAssign.Contains(x.DocID)).Distinct().ToList();
        //        }
        //        #endregion

        //        #region | Filter Panel |
        //        if (!string.IsNullOrWhiteSpace(txt_RequestDateFrom.Text))
        //        {
        //            string[] fromDate = txt_RequestDateFrom.Text.Split('/');
        //            int year = int.Parse(fromDate[2]) - 543;
        //            int month = int.Parse(fromDate[1]);
        //            int day = int.Parse(fromDate[0]);
        //            DateTime date = new DateTime(year, month, day);
        //            ListDocument = ListDocument.Where(x => x.CreatedDate != null && x.CreatedDate.Value.Date >= date).ToList();
        //        }
        //        if (!string.IsNullOrWhiteSpace(txt_RequestDateTo.Text))
        //        {
        //            string[] fromDate = txt_RequestDateTo.Text.Split('/');
        //            int year = int.Parse(fromDate[2]) - 543;
        //            int month = int.Parse(fromDate[1]);
        //            int day = int.Parse(fromDate[0]);
        //            DateTime date = new DateTime(year, month, day);// DateTime.ParseExact(txt_RequestDateFrom.Text, "dd/MM/yyyy", new CultureInfo("en-US")).Date;
        //            ListDocument = ListDocument.Where(x => x.CreatedDate != null && x.CreatedDate.Value.Date <= date).ToList();
        //        }
        //        if (!string.IsNullOrWhiteSpace(txt_Search.Text))
        //        {
        //            ListDocument = ListDocument.Where(x => x.DocNo.ToLower().Contains(txt_Search.Text.ToLower())
        //                || x.Title.ToLower().Contains((txt_Search.Text.ToLower()))
        //                || x.SubTitle.ToLower().Contains((txt_Search.Text.ToLower()))
        //                || x.DocumentSource.ToLower().Contains((txt_Search.Text.ToLower()))
        //                || x.Description.Contains(txt_Search.Text))
        //                    .ToList();
        //        }
        //        if (ddl_DocType.SelectedIndex > 0)
        //        {
        //            ListDocument = ListDocument.Where(x => x.DocTypeCode.Equals(ddl_DocType.SelectedValue)).ToList();
        //        }
        //        if (ddl_Categoty.SelectedIndex > 0)
        //        {
        //            ListDocument = ListDocument.Where(x => x.Category.Equals(ddl_Categoty.SelectedValue)).ToList();
        //        }
        //        if (ddl_Status.SelectedIndex > 0)
        //        {
        //            ListDocument = ListDocument.Where(x => x.Status.Equals(ddl_Status.SelectedValue)).ToList();
        //        }
        //        #endregion

        //        ListDocument = ListDocument.OrderByDescending(x => x.ModifiedDate).ToList();

        //        vs_dtDocument = Extension.ListToDataTable<TRNDocument>(ListDocument);
        //        BindRepeaterWithPaging();
        //        lbl_ItemCounter.Text = ListDocument.Count.ToString() + (ListDocument.Count > 1 ? " items" : " item");
        //    }
        //    catch (Exception ex)
        //    {
        //        Extension.LogWriter.Write(ex);
        //    }
        //}
        #endregion
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                db = new DataClassesDataAccessDataContext(vs_ConnectionString);
                List<TRNDocument> ListDocument = new List<TRNDocument>();
                string DateForm = Extension.ConvertTextToDateFormat(txt_RequestDateFrom.Text);
                string DateTo = Extension.ConvertTextToDateFormat(txt_RequestDateTo.Text);

                #region | Filter TaskGroup |
                string currentUser = vs_CurrentUserID;
                if (vs_Role == "ITAdmin")
                {
                    currentUser = "ITAdmin";
                }
                ListDocument = db.SP_Worklist(ddl_TaskGroup.SelectedValue, vs_CurrentUserID, vs_CurrentUserDepID
                    , ddl_DocType.SelectedValue
                        , DateForm, DateTo
                        , txt_Search.Text
                        , ddl_Categoty.SelectedValue
                        , ddl_Status.SelectedValue).ToList();
                //if (ddl_TaskGroup.SelectedValue.Equals("To Do List"))
                //{
                //    ListDocument = db.SP_Worklist_ToDoList(vs_CurrentUserID
                //        , ddl_DocType.SelectedValue
                //        , DateForm, DateTo
                //        , txt_Search.Text
                //        , ddl_Categoty.SelectedValue
                //        , ddl_Status.SelectedValue).ToList();
                //}
                //else if (ddl_TaskGroup.SelectedValue.Equals("In Process"))
                //{
                //    ListDocument = db.SP_Worklist_InProcess(vs_CurrentUserID
                //         , ddl_DocType.SelectedValue
                //         , DateForm, DateTo
                //         , txt_Search.Text
                //         , ddl_Categoty.SelectedValue
                //         , ddl_Status.SelectedValue).ToList();
                //}
                //else if (ddl_TaskGroup.SelectedValue.Equals("Completed"))
                //{
                //    ListDocument = db.SP_Worklist_Completed(vs_CurrentUserID
                //        , ddl_DocType.SelectedValue
                //        , DateForm, DateTo
                //        , txt_Search.Text
                //        , ddl_Categoty.SelectedValue
                //        , ddl_Status.SelectedValue).ToList();
                //}
                //else if (ddl_TaskGroup.SelectedValue.Equals("Rejected"))
                //{
                //    ListDocument = db.SP_Worklist_Rejected(vs_CurrentUserID
                //        , ddl_DocType.SelectedValue
                //        , DateForm, DateTo
                //        , txt_Search.Text
                //        , ddl_Categoty.SelectedValue
                //        , ddl_Status.SelectedValue).ToList();
                //}
                //else if (ddl_TaskGroup.SelectedValue.Equals("Cancelled"))
                //{
                //    ListDocument = db.SP_Worklist_Cancelled(vs_CurrentUserID
                //         , ddl_DocType.SelectedValue
                //         , DateForm, DateTo
                //         , txt_Search.Text
                //         , ddl_Categoty.SelectedValue
                //         , ddl_Status.SelectedValue).ToList();
                //}
                //else if (ddl_TaskGroup.SelectedValue.Equals("History"))
                //{
                //    if (vs_Role != "ITAdmin")
                //    {
                //        ListDocument = db.SP_Worklist_History(vs_CurrentUserID
                //        , ddl_DocType.SelectedValue
                //        , DateForm, DateTo
                //        , txt_Search.Text
                //        , ddl_Categoty.SelectedValue
                //        , ddl_Status.SelectedValue).ToList();
                //    }
                //    else
                //    {
                //        ListDocument = db.SP_Worklist_History("ITAdmin"
                //        , ddl_DocType.SelectedValue
                //        , DateForm, DateTo
                //        , txt_Search.Text
                //        , ddl_Categoty.SelectedValue
                //        , ddl_Status.SelectedValue).ToList();
                //    }
                //}
                //else if (ddl_TaskGroup.SelectedValue.Equals("Assign"))
                //{
                //    ListDocument = db.SP_Worklist_Assign(vs_CurrentUserID
                //       , ddl_DocType.SelectedValue
                //       , DateForm, DateTo
                //       , txt_Search.Text
                //       , ddl_Categoty.SelectedValue
                //       , ddl_Status.SelectedValue).ToList();
                //}
                #endregion 
                vs_dtDocument = Extension.ListToDataTable<TRNDocument>(ListDocument);
                BindRepeaterWithPaging();
                lbl_ItemCounter.Text = ListDocument.Count.ToString() + (ListDocument.Count > 1 ? " items" : " item");
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            Extension.Redirect(this.Page, "Worklist.aspx");
        }

        protected void ddl_TaskGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_TaskGroup.SelectedValue == "History")
            {
                txt_RequestDateFrom.Text = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy", _clti);
                txt_RequestDateTo.Text = DateTime.Now.ToString("dd/MM/yyyy", _clti);
            }
            btnSearch_Click(sender, e);
        }
        #endregion
        #region | Custom Paging |

        protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            CurrentPageNumber = Convert.ToInt32(e.CommandArgument) - 1;
            BindRepeaterWithPaging();
        }
        protected void btn_Previous_Command(object sender, CommandEventArgs e)
        {
            CurrentPageNumber = CurrentPageNumber - 1;
            BindRepeaterWithPaging();
        }
        protected void btn_Next_Command(object sender, CommandEventArgs e)
        {
            CurrentPageNumber = CurrentPageNumber + 1;
            BindRepeaterWithPaging();
        }
        private void BindRepeaterWithPaging()
        {
            gv_WorkList.AllowPaging = true;

            //Create the PagedDataSource that will be used in paging
            PagedDataSource pgitems = new PagedDataSource();
            pgitems.DataSource = vs_dtDocument.DataTableToList<TRNDocument>();
            pgitems.AllowPaging = true;

            //Control page size from here 
            ItemPerPage = 10;
            pgitems.PageSize = ItemPerPage;
            pgitems.CurrentPageIndex = CurrentPageNumber;

            btn_Previous.Enabled = CurrentPageNumber > 0;
            btn_Next.Enabled = CurrentPageNumber < pgitems.PageCount - 1;

            if (pgitems.PageCount > 1)
            {
                btn_Previous.Visible = true;
                btn_Next.Visible = true;
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
                gv_WorkList.DataSource = pgitems;
                gv_WorkList.DataBind();
            }
            catch (Exception ex)
            {
                pgitems.CurrentPageIndex = 0;
                gv_WorkList.DataSource = pgitems;
                gv_WorkList.DataBind();
            }
        }
        #endregion
        #region | History Modal |
        protected void btn_closeBtn_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "searchApproverModal", "$('#searchApproverModal').modal('hide');", true);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "searchApproverModal", "$('#searchApproverModal').modal();", true);
        }

        protected void gv_History_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";

                try
                {
                    object objActionBy = DataBinder.Eval(e.Row.DataItem, "EmpID");
                    SpecificEmployeeData.RootObject objEmp = Extension.GetSpecificEmployeeFromTemp(this.Page, objActionBy.ToString());

                    Label lblActionBy = (Label)e.Row.FindControl("lbl_ActionBy");
                    if (lblActionBy != null)
                    {
                        if (objActionBy != DBNull.Value)
                        {
                            if (objEmp != null)
                            {
                                string name_TH = string.Format("{0}{1} {2}", objEmp.PREFIX_TH, objEmp.FIRSTNAME_TH, objEmp.LASTNAME_TH);
                                string name_EN = string.Format("{0}{1} {2}", objEmp.PREFIX_EN, objEmp.FIRSTNAME_EN, objEmp.LASTNAME_EN);
                                lblActionBy.Text = vs_Lang == "TH" ? name_TH : name_EN;
                            }
                        }
                    }
                    Label lblActionDate = (Label)e.Row.FindControl("lbl_ActionDate");
                    object objActionDate = DataBinder.Eval(e.Row.DataItem, "ActionDate");
                    if (lblActionDate != null)
                        if (objActionDate != null && objActionDate != DBNull.Value) lblActionDate.Text = ((DateTime)objActionDate).ToString("dd/MM/yyyy HH:mm:ss");


                    Label lblPosition = (Label)e.Row.FindControl("lbl_Position");
                    object objPosition = DataBinder.Eval(e.Row.DataItem, "PositionID");
                    if (lblPosition == null) return;
                    if (objPosition == null || objPosition == DBNull.Value || objEmp == null) return;
                    SpecificEmployeeData.RESULT Dept = objEmp.RESULT.First(x => x.POSITION_TD == objPosition.ToString());
                    lblPosition.Text = vs_Lang == "TH" ? Dept.POSTION_NAME_TH : Dept.POSTION_NAME_EN;
                }
                catch (Exception ex)
                {
                    Extension.LogWriter.Write(ex);
                }

            }
        }

        protected void btn_closePopup_OnClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popHistory", "$('#showHistory').modal('hide');", true);
        }
        #endregion

        protected string getDoctype(string input)
        {
            string docType = "";
            switch (ddl_DocType.SelectedValue)
            {
                case "annouce": docType = "ประกาศ"; break;
                case "command": docType = "คำสั่ง"; break;
                case "rule": docType = "ข้อบังคับ"; break;
                case "order": docType = "ระเบียบ"; break;
                case "recieve": docType = "หนังสือเข้า"; break;
                case "send": docType = "หนังสือออก"; break;
                default: break;
            }
            return docType;
        }

        #region | Check Box |

        protected void chkHeadWorkList_OnCheckedChanged(object sender, EventArgs e)
        {
            CheckBox headBox = (CheckBox)sender;
            vs_SelectedItems = 0;

            foreach (GridViewRow row in gv_WorkList.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("chkWorkList");
                if (checkBox != null)
                {
                    if (headBox.Checked)
                    {
                        vs_SelectedItems++;
                    }
                    checkBox.Checked = headBox.Checked;
                }
            }
            if (headBox.Checked && ddl_TaskGroup.SelectedValue == "To Do List")
            {
                div_approvement.Visible = true;
                btn_Approve.OnClientClick = string.Format("return confirm('คุณต้องการอนุมัติ {0} รายการ ใช่หรือไม่?');", vs_SelectedItems);
            }
            else
            {

                div_approvement.Visible = false;
                btn_Approve.OnClientClick = "";
            }
        }

        protected void chkWorkList_OnCheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            CheckBox headerBox = (CheckBox)gv_WorkList.HeaderRow.FindControl("chkHeadWorkList");
            if (!checkBox.Checked)
            {
                headerBox.Checked = false;
                vs_SelectedItems--;
                div_approvement.Visible = false;
                if (vs_SelectedItems > 0 && ddl_TaskGroup.SelectedValue == "To Do List")
                {
                    div_approvement.Visible = true;
                    btn_Approve.OnClientClick = string.Format("return confirm('คุณต้องการอนุมัติ {0} รายการ ใช่หรือไม่?');", vs_SelectedItems);
                }
            }
            else
            {
                vs_SelectedItems++;
                if (ddl_TaskGroup.SelectedValue == "To Do List")
                {
                    div_approvement.Visible = true;
                    btn_Approve.OnClientClick = string.Format("return confirm('คุณต้องการอนุมัติ {0} รายการ ใช่หรือไม่?');", vs_SelectedItems);
                }
                if (vs_SelectedItems == gv_WorkList.Rows.Count)
                {
                    headerBox.Checked = true;
                }
            }
        }
        #endregion
        protected void btn_Approve_Click(object sender, EventArgs e)
        {
            string sMessage = "";
            List<string> sMessageFileWord = new List<string>();
            int failedCounter = 0;
            int successCounter = 0;
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
            if (db.Connection.State == ConnectionState.Open)
            {
                db.Connection.Close();
                db.Connection.Open();
            }
            else
            {
                db.Connection.Open();
                System.Data.Common.DbTransaction dbTrabs = db.Connection.BeginTransaction();
                db.Transaction = dbTrabs;
                string DocID = string.Empty;
                try
                {


                    foreach (GridViewRow gv_row in gv_WorkList.Rows)
                    {
                        try
                        {
                            DocID = "";
                            if (((CheckBox)gv_row.FindControl("chkWorkList")).Checked)
                            {
                                string pk = "0";
                                Label lbl_DocID = (Label)gv_row.Cells[0].FindControl("lbl_DocID");
                                if (lbl_DocID != null) pk = lbl_DocID.Text;
                                DocID = pk;
                                if (!string.IsNullOrWhiteSpace(pk) && pk != "0")
                                {
                                    TRNDocument objDocument = db.TRNDocuments.SingleOrDefault(x => x.DocID == Convert.ToInt32(pk));
                                    if (objDocument != null)
                                    {
                                        string oDocLib = objDocument.DocLib;
                                        string oDocSet = objDocument.DocSet;
                                        #region | get approval |
                                        List<TRNApprover> listApprover = db.TRNApprovers.Where(x => x.DocID == Convert.ToInt32(pk)).OrderBy(x => x.ApproverID).ToList();

                                        #endregion

                                        #region | Wait For Approve |
                                        if (objDocument.Status.Equals("Wait for Approve") && objDocument.WaitingFor.Trim(' ').Split(',').Contains(vs_CurrentUserID))
                                        {
                                            if (objDocument.CurrentApprovalLevel == listApprover.Count)
                                            {
                                                //Last Approval
                                                #region | Update ObjDocument *Last Approval* |
                                                objDocument.ApproveDate = DateTime.Now;
                                                if (objDocument.DocTypeCode != "M" && objDocument.DocTypeCode != "Im")
                                                {
                                                    DataTable dtAdmin = Extension.GetDataTable("MstAdminCentre");
                                                    string waitingFor = string.Empty;
                                                    string waitingForDeptID = string.Empty;
                                                    if (!dtAdmin.DataTableIsNullOrEmpty())
                                                    {
                                                        DataTable oResult = new DataTable();
                                                        if (objDocument.Category.Equals("centre"))
                                                        {
                                                            oResult = dtAdmin.AsEnumerable()
                                                                  .Where(r => r.Field<String>("DeptID").Equals("10")).ToList().CopyToDataTable();
                                                        }
                                                        else
                                                        {
                                                            oResult = dtAdmin.AsEnumerable()
                                                                  .Where(r => r.Field<String>("DeptID").Equals(objDocument.FromDepartmentID.ToString())).ToList().CopyToDataTable();
                                                        }

                                                        foreach (DataRow row in oResult.Rows)
                                                        {
                                                            string[] empLoginName = row["UserName"].ToString().Split(',');
                                                            if (empLoginName != null)
                                                            {
                                                                for (int i = 0; i < empLoginName.Count(); i++)
                                                                {

                                                                    if (string.IsNullOrEmpty(waitingFor))
                                                                    {
                                                                        waitingFor = SharedRules.ConvertLoginNameToUserID(empLoginName[i], Page);
                                                                    }
                                                                    else
                                                                    {
                                                                        waitingFor += string.Format(",{0}", SharedRules.ConvertLoginNameToUserID(empLoginName[i], Page));
                                                                    }
                                                                }
                                                            }
                                                            waitingForDeptID = row["DeptID"].ToString();
                                                            //if (string.IsNullOrEmpty(waitingFor))
                                                            //{
                                                            //    waitingFor = row["EmpID"].ToString();
                                                            //    waitingForDeptID = row["DeptID"].ToString();
                                                            //}
                                                            //else
                                                            //{
                                                            //    waitingFor += string.Format(",{0}", row["EmpID"].ToString());
                                                            //    waitingForDeptID += string.Format(",{0}", row["DeptID"].ToString());
                                                            //}
                                                        }
                                                    }
                                                    objDocument.WaitingFor = waitingFor;
                                                    objDocument.WaitingForDeptID = waitingForDeptID;
                                                    objDocument.Status = "Wait for Admin Centre";
                                                }
                                                else
                                                {
                                                    objDocument.WaitingFor = "";
                                                    objDocument.WaitingForDeptID = "";
                                                    objDocument.Status = "Completed";
                                                }
                                                #endregion

                                                #region | Generate Document No |
                                                string runningNo = "";
                                                string newRunningNo = "0001";
                                                string projectYear = (DateTime.Now.Year).ToString();
                                                projectYear = objDocument.DocTypeCode == "ExEN"
                                                        ? projectYear.ConvertToAD()
                                                        : projectYear.ConvertToBE();

                                                //generate RunningNo
                                                List<ControlRunning> listControlRunning = new List<ControlRunning>();
                                                listControlRunning = db.ControlRunnings.ToList();

                                                int sDepID = Convert.ToInt32(objDocument.RequestorDepartmentID);
                                                string sCategory = "", sDocType = "", sDepCode = "";
                                                string sDepName = "";
                                                DataTable dtDept = Extension.GetDepartmentData(this.Page);
                                                if (dtDept.Rows.Count > 0)
                                                {
                                                    sDepName = dtDept.Rows[0]["Department_Name_TH"].ToString();
                                                    sDepCode = dtDept.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString();
                                                    if (objDocument.DocTypeCode == "ExEN")
                                                    {
                                                        sDepName = dtDept.Rows[0]["Department_Name_EN"].ToString();
                                                        sDepCode = dtDept.Rows[0]["DEPARTMENT_ACRONYM_EN"].ToString();
                                                    }
                                                }
                                                listControlRunning = listControlRunning.Where(x => x.DocType == objDocument.DocTypeCode
                                                                                            && x.CreatedYear == projectYear
                                                                                            && x.DepID == (objDocument.Category == "centre" ? 0 : sDepID)
                                                                                            && x.RunningNo >= 1)
                                                                                            .OrderByDescending(x => x.RunningNo)
                                                                                            .ToList();

                                                //string institudeName = "สถาบันการจัดการปัญญาภิวัฒน์";
                                                //runningNo = Extension.GetDocumentNo(objDocument.Category, objDocument.DocTypeCode, sDocType, sDepName, sDepCode, institudeName, newRunningNo, projectYear);
                                                if (listControlRunning.Count > 0)
                                                {
                                                    foreach (ControlRunning cr in listControlRunning)
                                                    {
                                                        newRunningNo = cr.RunningNo > 9999
                                                            ? cr.RunningNo.ToString()
                                                            : string.Format("{0:D4}", (cr.RunningNo + 1));
                                                        cr.RunningNo += 1;
                                                        objDocument.DocNo = string.Format("{0}/{1}", newRunningNo, projectYear);
                                                        db.SubmitChanges();
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    //Insert data to MSTControlRunning
                                                    ControlRunning objControlRunning = new ControlRunning();
                                                    objControlRunning.DocType = objDocument.DocTypeCode;
                                                    objControlRunning.CreatedYear = projectYear;
                                                    objControlRunning.DepID = objDocument.Category == "internal" ? sDepID : 0;
                                                    objControlRunning.RunningNo = Convert.ToInt32(newRunningNo);

                                                    objDocument.DocNo = string.Format("{0}/{1}", newRunningNo, projectYear);

                                                    if (string.IsNullOrEmpty(objDocument.DocID.ToString()))
                                                    {
                                                        db.TRNDocuments.InsertOnSubmit(objDocument);
                                                        db.SubmitChanges();
                                                    }

                                                    objControlRunning.DocID = objDocument.DocID;

                                                    db.ControlRunnings.InsertOnSubmit(objControlRunning);
                                                    db.SubmitChanges();
                                                }
                                                #endregion

                                                #region | Update MS Word + Upload to DocSet |
                                                if (!string.IsNullOrWhiteSpace(objDocument.AttachWordPath))
                                                {
                                                    string oDocName = string.Format("PIMEdocumentTemplate-[{0}].docx", objDocument.DocTypeCode);
                                                    TRNApprover objApprover = db.TRNApprovers.Where(x => x.DocID == objDocument.DocID).OrderByDescending(o => o.ApproverID).First();
                                                    if (objDocument.DocTypeCode == "ExEN")
                                                    {
                                                        SpecificEmployeeData.RootObject specAppEN = Extension.GetSpecificEmployeeFromTemp(Page, objApprover.EmpID.ToString());
                                                        DataTable specDeptEN = Extension.GetSpecificDepartmentDataFromTemp(Page, objApprover.DepartmentID.ToString());
                                                        DataTable specPosEN = Extension.GetSpecificPositionDataFromTemp(Page, objApprover.PositionID.ToString());
                                                        objDocument.FromDepartmentName = Extension.GetSpecificDepartmentDataFromTemp(Page, objDocument.FromDepartmentID.ToString()).Rows[0]["DEPARTMENT_NAME_EN"].ToString();
                                                        objApprover.EmployeeName = string.Format("{0} {1} {2}", specAppEN.PREFIX_EN, specAppEN.FIRSTNAME_EN, specAppEN.LASTNAME_EN);
                                                        objApprover.DepartmentName = specDeptEN.Rows[0]["DEPARTMENT_NAME_EN"].ToString();
                                                        objApprover.PositionName = specPosEN.Rows[0]["POSITION_NAME_EN"].ToString();
                                                    }
                                                    List<TRNReferenceDoc> listRefDoc = db.TRNReferenceDocs.Where(x => x.DocID == Convert.ToInt32(pk)).ToList();
                                                    string[] referenceDocument = new string[listRefDoc.Count];
                                                    for (int i = 0; i < listRefDoc.Count; i++)
                                                    {
                                                        try
                                                        {
                                                            TRNDocument objRefDoc = db.TRNDocuments.First(x => x.DocID == (listRefDoc[i].DocID ?? 0));
                                                            referenceDocument[i] = Extension.GenerateDocumentNo(objRefDoc.DocNo, Convert.ToInt32(objRefDoc.FromDepartmentID), objRefDoc.Category, objRefDoc.DocTypeCode, Page);
                                                        }
                                                        catch (Exception ex) { }

                                                    }
                                                    //Update MSWord
                                                    byte[] data = Extension.getMSWord(objDocument.AttachWordPath);
                                                    byte[] bArr = Extension.UpdateMSWord(Page, data, objDocument, objApprover, referenceDocument);
                                                    SharedRules.UploadFileIntoDocumentSet(oDocLib, oDocSet, oDocName, new MemoryStream(bArr), "", "");

                                                    //Convert to PDF
                                                    Extension.ChangeTypeToPDF(objDocument.AttachWordPath, "TempDocument", oDocSet, Page);
                                                }
                                                #endregion

                                                #region | Generate PDF + Move Temp to DocLib |
                                                DataTable dtAttachTable = new DataTable();
                                                string tempDocSet = objDocument.DocSet;
                                                if ((objDocument.Status == "Wait for Admin Centre" && objDocument.Type != "Save") ||
                                            (objDocument.Status == "Completed" && objDocument.Type == "Save") ||
                                            (objDocument.Status == "Completed" && (objDocument.DocTypeCode == "M" || objDocument.DocTypeCode == "Im") && objDocument.Type != "Save"))
                                                {
                                                    if (!(objDocument.Status == "Completed" && objDocument.Type == "Save"))
                                                    {
                                                        TRNEDocumentQueue eDocQ = new TRNEDocumentQueue();
                                                        eDocQ.DocID = objDocument.DocID;
                                                        eDocQ.UserLoginName = vs_CurrentUserID;
                                                        eDocQ.IsActive = true;
                                                        db.TRNEDocumentQueues.InsertOnSubmit(eDocQ);
                                                        db.SubmitChanges();
                                                    }
                                                    try
                                                    {
                                                        string DocLibName = "Document_Library";
                                                        //Stream stream = new MemoryStream(Extension.PrintMemoDetail("Upload", db, objDocument.DocID, Page));
                                                        string DocTypeCode = "DocTypeCode";
                                                        DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
                                                        if (!dtDocType.DataTableIsNullOrEmpty())
                                                        {
                                                            DataTable sDocTypeResult = dtDocType.AsEnumerable().Where(r => r.Field<String>("Value").Equals(objDocument.DocTypeCode)).ToList().CopyToDataTable();
                                                            if (!sDocTypeResult.DataTableIsNullOrEmpty())
                                                            {
                                                                DocTypeCode = sDocTypeResult.Rows[0]["DocTypeCode"].ToString();
                                                            }
                                                        }


                                                        //string DocSet = lbl_DocumentNo.Text.Replace("/", "_");
                                                        DateTime createDate = DateTime.Parse(objDocument.CreatedDate.ToString());
                                                        //string DocSet = string.Format("{0}_{1}/{2}_{3}", DocTypeCode, newRunningNo, projectYear, string.Format("{0}{1}{2}", createDate.Day, createDate.Month.ToString("D2"), createDate.Year.ToString().ConvertToBE()));
                                                        string DocSet = string.Format("{0}_{1}/{2}{3}_{4}", DocTypeCode, newRunningNo, projectYear, objDocument.Category == "internal" ? string.Format("_{0}", objDocument.RequestorDepartmentID) : "", string.Format("{0}{1}{2}", createDate.Day.ToString("D2"), createDate.Month.ToString("D2"), createDate.Year.ToString().ConvertToBE()));
                                                        DocSet = DocSet.Replace("/", "_");
                                                        objDocument.DocSet = DocSet;
                                                        objDocument.DocLib = DocLibName;

                                                        SharedRules.CreateDocumentSet(DocLibName, DocSet, null);
                                                        //SharedRules.UploadFileIntoDocumentSet(DocLibName, DocSet, string.Format("{0}{1}", DocSet, ".pdf"), stream, "", SharedRules.LogonName());

                                                        if ((objDocument.DocTypeCode != "Im" && objDocument.Type != "Save") ||
                                                             (objDocument.Status == "Completed" && (objDocument.DocTypeCode == "M" || objDocument.DocTypeCode == "Im") && objDocument.Type != "Save"))
                                                        {
                                                            dtAttachTable.Columns.Add("Sequence");
                                                            dtAttachTable.Columns.Add("AttachFile");
                                                            dtAttachTable.Columns.Add("FileName");
                                                            dtAttachTable.Columns.Add("ActorName");
                                                            dtAttachTable.Columns.Add("AttachDate");
                                                            dtAttachTable.Columns.Add("AttachFilePath");
                                                            dtAttachTable.Columns.Add("ActorID");
                                                            dtAttachTable.Columns.Add("DocSetName");
                                                            dtAttachTable.Columns.Add("DocLibName");
                                                            dtAttachTable.Columns.Add("IsPrimary");

                                                            DataRow dr = dtAttachTable.NewRow();

                                                            dr["Sequence"] = 1;
                                                            dr["AttachFile"] = DocSet + ".pdf";
                                                            dr["ActorName"] = "Generated by E-Document System";
                                                            dr["AttachDate"] = DateTime.Now;
                                                            dr["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, DocSet + ".pdf");

                                                            string DocPDFPath = dr["AttachFilePath"].ToString();
                                                            objDocument.AttachFilePath = DocPDFPath;

                                                            if (!string.IsNullOrWhiteSpace(objDocument.AttachWordPath))
                                                            {
                                                                objDocument.AttachWordPath = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, string.Format("PIMEdocumentTemplate-[{0}].docx", objDocument.DocTypeCode));

                                                            }
                                                            dr["DocSetName"] = DocSet;
                                                            dr["DocLibName"] = "Document_Library";
                                                            dr["ActorID"] = "-";
                                                            dr["IsPrimary"] = "Y";
                                                            dtAttachTable.Rows.Add(dr);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        throw ex;
                                                        //Extension.LogWriter.Write(ex);
                                                    }
                                                    #region | Move File to DocLib |

                                                    SharedRules.CopyFileToAnotherDocSet("TempDocument", oDocSet, "Document_Library", objDocument.DocSet);

                                                    DataTable vs_attachFileTable = Extension.ListToDataTable<TRNAttachFileDoc>(db.TRNAttachFileDocs.Where(x => x.DocID == objDocument.DocID).ToList());
                                                    foreach (DataRow fileRow in vs_attachFileTable.Rows)
                                                    {
                                                        fileRow["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), "Document_Library", objDocument.DocSet, fileRow["AttachFile"]);
                                                        fileRow["DocSetName"] = objDocument.DocSet;
                                                        fileRow["DocLibName"] = "Document_Library";
                                                    }
                                                    db.SubmitChanges();
                                                    #endregion
                                                }
                                                #endregion

                                                #region | Update/Insert Attach Document |
                                                //update or insert Attach Document
                                                if (!string.IsNullOrEmpty(objDocument.DocID.ToString()))
                                                {
                                                    List<TRNAttachFileDoc> objListTRNAttachDoc = new List<TRNAttachFileDoc>();
                                                    objListTRNAttachDoc = db.TRNAttachFileDocs.ToList();
                                                    IEnumerable<TRNAttachFileDoc> queryAD = (from TRNAttachFileDoc attachDoc in objListTRNAttachDoc
                                                                                             where attachDoc.DocID == objDocument.DocID
                                                                                             select attachDoc);
                                                    db.TRNAttachFileDocs.DeleteAllOnSubmit(queryAD);
                                                    db.SubmitChanges();
                                                }

                                                List<TRNAttachFileDoc> listAttachDocument = new List<TRNAttachFileDoc>();
                                                if (!dtAttachTable.DataTableIsNullOrEmpty())
                                                {

                                                    foreach (DataRow dr in dtAttachTable.Rows)
                                                    {
                                                        TRNAttachFileDoc objAttachDocument = new TRNAttachFileDoc();
                                                        objAttachDocument.DocID = objDocument.DocID;
                                                        //objAttachDocument.ActorID = Convert.ToInt32(dr["ActorID"].ToString());
                                                        objAttachDocument.ActorName = dr["ActorName"].ToString();
                                                        objAttachDocument.AttachDate = DateTime.Parse(dr["AttachDate"].ToString());
                                                        objAttachDocument.AttachFile = dr["AttachFile"].ToString();
                                                        objAttachDocument.AttachFIlePath = dr["AttachFilePath"].ToString();
                                                        objAttachDocument.DocSetName = dr["DocSetName"].ToString();
                                                        objAttachDocument.DocLibName = dr["DocLibName"].ToString();
                                                        objAttachDocument.IsPrimary = dr["IsPrimary"].ToString();

                                                        listAttachDocument.Add(objAttachDocument);
                                                    }


                                                }
                                                if (listAttachDocument.Count > 0)
                                                {
                                                    db.TRNAttachFileDocs.InsertAllOnSubmit(listAttachDocument);
                                                    db.SubmitChanges();
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                //Generic Approval
                                                int currentApproveLevel = Convert.ToInt32(objDocument.CurrentApprovalLevel);
                                                objDocument.WaitingFor = listApprover[currentApproveLevel].EmpID.ToString();
                                                objDocument.WaitingForDeptID = listApprover[currentApproveLevel].DepartmentID.ToString();
                                                objDocument.CurrentApprovalLevel = objDocument.CurrentApprovalLevel + 1;
                                            }

                                            #region | Update/Insert History |
                                            //Update History
                                            TRNHistory objActionHistory = new TRNHistory();
                                            objActionHistory.DocID = objDocument.DocID;
                                            objActionHistory.EmpID = vs_CurrentUserID;
                                            objActionHistory.ActionName = "Approve";
                                            objActionHistory.ActionDate = DateTime.Now;
                                            objActionHistory.StatusBefore = "Wait for Approve";
                                            db.TRNHistories.InsertOnSubmit(objActionHistory);
                                            db.SubmitChanges();
                                            #endregion

                                            #region | Send Email |
                                            if ((objDocument.Status == "Rejected" || objDocument.Status == "Completed"))
                                            {
                                            }
                                            else if (objDocument.Status == "Wait for Admin Centre")
                                            {
                                                Extension.SendEmailTemplate(objDocument.Status, objDocument.WaitingFor.ToString(), objDocument.WaitingForDeptID.ToString(), "Approve", "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                            }
                                            else
                                            {
                                                Extension.SendEmailTemplate(objDocument.Status, objDocument.WaitingFor.ToString(), objDocument.WaitingForDeptID.ToString(), "Approve", "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);

                                            }
                                            #endregion
                                            successCounter++;
                                        }
                                        else
                                        {
                                            failedCounter++;
                                        }
                                        #endregion


                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            failedCounter++;
                            Extension.LogWriter.Write(ex);
                            sMessageFileWord.Add(string.Format("\\n DocID : {0} {1}", DocID, ReplaceMessage(ex.Message.ToString())));

                        }
                    }
                }
                catch (Exception ex)
                {
                    Extension.LogWriter.Write(ex);
                    dbTrabs.Rollback();
                }
                finally
                {
                    if (failedCounter == 0)
                    {
                        dbTrabs.Commit();
                    }
                    if (db.Connection.State == System.Data.ConnectionState.Open)
                    {
                        db.Connection.Close();
                    }
                }
            }
             
            if (failedCounter > 0)
            {
                if (!string.IsNullOrWhiteSpace(sMessage))
                {
                    sMessage += "\\n";
                }
                sMessage += string.Format("{0} งานที่ไม่สามารถอนุมัติผ่านหน้านี้ได้", failedCounter);
                if (sMessageFileWord.Count() > 0)
                {
                    foreach (string msg in sMessageFileWord)
                    {
                        sMessage += "\\n";
                        sMessage += string.Format("{0}", msg);
                    }
                }
            }
            else
            {
                if (successCounter > 0)
                {
                    sMessage = string.Format("{0} งาน ดำเนินการอนุมิติแล้ว ", successCounter);
                }
            }

            if (!string.IsNullOrWhiteSpace(sMessage))
            {
                Extension.MessageBox(Page, sMessage);
            }
            div_approvement.Visible = false;
            btnSearch_Click(sender, e);
            //Extension.Redirect(Page, "WorkList.aspx");
        }

        private string ReplaceMessage(string Message)
        {
            if (Message.Contains("locked for shared"))
            {
                string user = Message.Split(' ').Last();
                if (!string.IsNullOrWhiteSpace(user))
                {
                    if (user.Contains("\\"))
                    {
                        user = Extension.ToUpperFirstLetter(user.Split('\\')[1]);
                    }
                    Message = string.Format("The document is editing by {0}, Please close all editing documents. (กรุณาปิดไฟล์ Word)", user.Trim().Replace(".", ""));
                }
                else
                {
                    Message = "This Document is in editing mode, Please close all editing documents. (กรุณาปิดไฟล์ Word)";
                }
            }
            return Message;
        }
        #region |back up|

        //protected void btn_Approve_Click(object sender, EventArgs e)
        //{
        //    int failedCounter = 0;
        //    int successCounter = 0;
        //    DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
        //    foreach (GridViewRow gv_row in gv_WorkList.Rows)
        //    {
        //        try
        //        {

        //            if (((CheckBox)gv_row.FindControl("chkWorkList")).Checked)
        //            {
        //                string pk = "0";
        //                Label lbl_DocID = (Label)gv_row.Cells[0].FindControl("lbl_DocID");
        //                if (lbl_DocID != null) pk = lbl_DocID.Text;
        //                if (!string.IsNullOrWhiteSpace(pk) && pk != "0")
        //                {
        //                    TRNDocument objDocument = db.TRNDocuments.SingleOrDefault(x => x.DocID == Convert.ToInt32(pk));
        //                    if (objDocument != null)
        //                    {
        //                        string oDocLib = objDocument.DocLib;
        //                        string oDocSet = objDocument.DocSet;
        //                        #region | get approval |
        //                        List<TRNApprover> listApprover = db.TRNApprovers.Where(x => x.DocID == Convert.ToInt32(pk)).OrderBy(x => x.ApproverID).ToList();

        //                        #endregion

        //                        #region | Wait For Approve |
        //                        if (objDocument.Status.Equals("Wait for Approve") && objDocument.WaitingFor.Trim(' ').Split(',').Contains(vs_CurrentUserID))
        //                        {
        //                            if (objDocument.CurrentApprovalLevel == listApprover.Count)
        //                            {
        //                                //Last Approval
        //                                #region | Update ObjDocument *Last Approval* |
        //                                objDocument.ApproveDate = DateTime.Now;
        //                                if (objDocument.DocTypeCode != "M" && objDocument.DocTypeCode != "Im")
        //                                {
        //                                    DataTable dtAdmin = Extension.GetDataTable("MstAdminCentre");
        //                                    string waitingFor = string.Empty;
        //                                    string waitingForDeptID = string.Empty;
        //                                    if (!dtAdmin.DataTableIsNullOrEmpty())
        //                                    {
        //                                        DataTable oResult = new DataTable();
        //                                        if (objDocument.Category.Equals("centre"))
        //                                        {
        //                                            oResult = dtAdmin.AsEnumerable()
        //                                                  .Where(r => r.Field<String>("DeptID").Equals("10")).ToList().CopyToDataTable();
        //                                        }
        //                                        else
        //                                        {
        //                                            oResult = dtAdmin.AsEnumerable()
        //                                                  .Where(r => r.Field<String>("DeptID").Equals(objDocument.FromDepartmentID.ToString())).ToList().CopyToDataTable();
        //                                        }

        //                                        foreach (DataRow row in oResult.Rows)
        //                                        {
        //                                            string[] empLoginName = row["UserName"].ToString().Split(',');
        //                                            if (empLoginName != null)
        //                                            {
        //                                                for (int i = 0; i < empLoginName.Count(); i++)
        //                                                {

        //                                                    if (string.IsNullOrEmpty(waitingFor))
        //                                                    {
        //                                                        waitingFor = SharedRules.ConvertLoginNameToUserID(empLoginName[i], Page);
        //                                                    }
        //                                                    else
        //                                                    {
        //                                                        waitingFor += string.Format(",{0}", SharedRules.ConvertLoginNameToUserID(empLoginName[i], Page));
        //                                                    }
        //                                                }
        //                                            }
        //                                            waitingForDeptID = row["DeptID"].ToString();
        //                                            //if (string.IsNullOrEmpty(waitingFor))
        //                                            //{
        //                                            //    waitingFor = row["EmpID"].ToString();
        //                                            //    waitingForDeptID = row["DeptID"].ToString();
        //                                            //}
        //                                            //else
        //                                            //{
        //                                            //    waitingFor += string.Format(",{0}", row["EmpID"].ToString());
        //                                            //    waitingForDeptID += string.Format(",{0}", row["DeptID"].ToString());
        //                                            //}
        //                                        }
        //                                    }
        //                                    objDocument.WaitingFor = waitingFor;
        //                                    objDocument.WaitingForDeptID = waitingForDeptID;
        //                                    objDocument.Status = "Wait for Admin Centre";
        //                                }
        //                                else
        //                                {
        //                                    objDocument.WaitingFor = "";
        //                                    objDocument.WaitingForDeptID = "";
        //                                    objDocument.Status = "Completed";
        //                                }
        //                                #endregion

        //                                #region | Generate Document No |
        //                                string runningNo = "";
        //                                string newRunningNo = "0001";
        //                                string projectYear = (DateTime.Now.Year).ToString();
        //                                projectYear = objDocument.DocTypeCode == "ExEN"
        //                                        ? projectYear.ConvertToAD()
        //                                        : projectYear.ConvertToBE();

        //                                //generate RunningNo
        //                                List<ControlRunning> listControlRunning = new List<ControlRunning>();
        //                                listControlRunning = db.ControlRunnings.ToList();

        //                                int sDepID = Convert.ToInt32(objDocument.RequestorDepartmentID);
        //                                string sCategory = "", sDocType = "", sDepCode = "";
        //                                string sDepName = "";
        //                                DataTable dtDept = Extension.GetDepartmentData(this.Page);
        //                                if (dtDept.Rows.Count > 0)
        //                                {
        //                                    sDepName = dtDept.Rows[0]["Department_Name_TH"].ToString();
        //                                    sDepCode = dtDept.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString();
        //                                    if (objDocument.DocTypeCode == "ExEN")
        //                                    {
        //                                        sDepName = dtDept.Rows[0]["Department_Name_EN"].ToString();
        //                                        sDepCode = dtDept.Rows[0]["DEPARTMENT_ACRONYM_EN"].ToString();
        //                                    }
        //                                }
        //                                listControlRunning = listControlRunning.Where(x => x.DocType == objDocument.DocTypeCode
        //                                                                            && x.CreatedYear == projectYear
        //                                                                            && x.DepID == (objDocument.Category == "centre" ? 0 : sDepID)
        //                                                                            && x.RunningNo >= 1)
        //                                                                            .OrderByDescending(x => x.RunningNo)
        //                                                                            .ToList();

        //                                //string institudeName = "สถาบันการจัดการปัญญาภิวัฒน์";
        //                                //runningNo = Extension.GetDocumentNo(objDocument.Category, objDocument.DocTypeCode, sDocType, sDepName, sDepCode, institudeName, newRunningNo, projectYear);
        //                                if (listControlRunning.Count > 0)
        //                                {
        //                                    foreach (ControlRunning cr in listControlRunning)
        //                                    {
        //                                        newRunningNo = cr.RunningNo > 9999
        //                                            ? cr.RunningNo.ToString()
        //                                            : string.Format("{0:D4}", (cr.RunningNo + 1));
        //                                        cr.RunningNo += 1;
        //                                        objDocument.DocNo = string.Format("{0}/{1}", newRunningNo, projectYear);
        //                                        db.SubmitChanges();
        //                                        break;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    //Insert data to MSTControlRunning
        //                                    ControlRunning objControlRunning = new ControlRunning();
        //                                    objControlRunning.DocType = objDocument.DocTypeCode;
        //                                    objControlRunning.CreatedYear = projectYear;
        //                                    objControlRunning.DepID = objDocument.Category == "internal" ? sDepID : 0;
        //                                    objControlRunning.RunningNo = Convert.ToInt32(newRunningNo);

        //                                    objDocument.DocNo = string.Format("{0}/{1}", newRunningNo, projectYear);

        //                                    if (string.IsNullOrEmpty(objDocument.DocID.ToString()))
        //                                    {
        //                                        db.TRNDocuments.InsertOnSubmit(objDocument);
        //                                        db.SubmitChanges();
        //                                    }

        //                                    objControlRunning.DocID = objDocument.DocID;

        //                                    db.ControlRunnings.InsertOnSubmit(objControlRunning);
        //                                    db.SubmitChanges();
        //                                }
        //                                #endregion

        //                                #region | Update MS Word + Upload to DocSet |
        //                                if (!string.IsNullOrWhiteSpace(objDocument.AttachWordPath))
        //                                {
        //                                    string oDocName = string.Format("PIMEdocumentTemplate-[{0}].docx", objDocument.DocTypeCode);
        //                                    TRNApprover objApprover = db.TRNApprovers.Where(x => x.DocID == objDocument.DocID).OrderByDescending(o => o.ApproverID).First();
        //                                    if (objDocument.DocTypeCode == "ExEN")
        //                                    {
        //                                        SpecificEmployeeData.RootObject specAppEN = Extension.GetSpecificEmployeeFromTemp(Page, objApprover.EmpID.ToString());
        //                                        DataTable specDeptEN = Extension.GetSpecificDepartmentDataFromTemp(Page, objApprover.DepartmentID.ToString());
        //                                        DataTable specPosEN = Extension.GetSpecificPositionDataFromTemp(Page, objApprover.PositionID.ToString());
        //                                        objDocument.FromDepartmentName = Extension.GetSpecificDepartmentDataFromTemp(Page, objDocument.FromDepartmentID.ToString()).Rows[0]["DEPARTMENT_NAME_EN"].ToString();
        //                                        objApprover.EmployeeName = string.Format("{0} {1} {2}", specAppEN.PREFIX_EN, specAppEN.FIRSTNAME_EN, specAppEN.LASTNAME_EN);
        //                                        objApprover.DepartmentName = specDeptEN.Rows[0]["DEPARTMENT_NAME_EN"].ToString();
        //                                        objApprover.PositionName = specPosEN.Rows[0]["POSITION_NAME_EN"].ToString();
        //                                    }
        //                                    List<TRNReferenceDoc> listRefDoc = db.TRNReferenceDocs.Where(x => x.DocID == Convert.ToInt32(pk)).ToList();
        //                                    string[] referenceDocument = new string[listRefDoc.Count];
        //                                    for (int i = 0; i < listRefDoc.Count; i++)
        //                                    {
        //                                        try
        //                                        {
        //                                            TRNDocument objRefDoc = db.TRNDocuments.First(x => x.DocID == (listRefDoc[i].DocID ?? 0));
        //                                            referenceDocument[i] = Extension.GenerateDocumentNo(objRefDoc.DocNo, Convert.ToInt32(objRefDoc.FromDepartmentID), objRefDoc.Category, objRefDoc.DocTypeCode, Page);
        //                                        }
        //                                        catch (Exception ex) { }

        //                                    }
        //                                    //Update MSWord
        //                                    byte[] data = Extension.getMSWord(objDocument.AttachWordPath);
        //                                    byte[] bArr = Extension.UpdateMSWord(Page, data, objDocument, objApprover, referenceDocument);
        //                                    SharedRules.UploadFileIntoDocumentSet(oDocLib, oDocSet, oDocName, new MemoryStream(bArr), "", "");
        //                                    //Convert to PDF
        //                                    Extension.ChangeTypeToPDF(objDocument.AttachWordPath, "TempDocument", oDocSet, Page);
        //                                }
        //                                #endregion

        //                                #region | Generate PDF + Move Temp to DocLib |
        //                                DataTable dtAttachTable = new DataTable();
        //                                string tempDocSet = objDocument.DocSet;
        //                                if ((objDocument.Status == "Wait for Admin Centre" && objDocument.Type != "Save") ||
        //                            (objDocument.Status == "Completed" && objDocument.Type == "Save") ||
        //                            (objDocument.Status == "Completed" && (objDocument.DocTypeCode == "M" || objDocument.DocTypeCode == "Im") && objDocument.Type != "Save"))
        //                                {
        //                                    if (!(objDocument.Status == "Completed" && objDocument.Type == "Save"))
        //                                    {
        //                                        TRNEDocumentQueue eDocQ = new TRNEDocumentQueue();
        //                                        eDocQ.DocID = objDocument.DocID;
        //                                        eDocQ.UserLoginName = vs_CurrentUserID;
        //                                        eDocQ.IsActive = true;
        //                                        db.TRNEDocumentQueues.InsertOnSubmit(eDocQ);
        //                                        db.SubmitChanges();
        //                                    }
        //                                    try
        //                                    {
        //                                        string DocLibName = "Document_Library";
        //                                        //Stream stream = new MemoryStream(Extension.PrintMemoDetail("Upload", db, objDocument.DocID, Page));
        //                                        string DocTypeCode = "DocTypeCode";
        //                                        DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
        //                                        if (!dtDocType.DataTableIsNullOrEmpty())
        //                                        {
        //                                            DataTable sDocTypeResult = dtDocType.AsEnumerable().Where(r => r.Field<String>("Value").Equals(objDocument.DocTypeCode)).ToList().CopyToDataTable();
        //                                            if (!sDocTypeResult.DataTableIsNullOrEmpty())
        //                                            {
        //                                                DocTypeCode = sDocTypeResult.Rows[0]["DocTypeCode"].ToString();
        //                                            }
        //                                        }


        //                                        //string DocSet = lbl_DocumentNo.Text.Replace("/", "_");
        //                                        DateTime createDate = DateTime.Parse(objDocument.CreatedDate.ToString());
        //                                        //string DocSet = string.Format("{0}_{1}/{2}_{3}", DocTypeCode, newRunningNo, projectYear, string.Format("{0}{1}{2}", createDate.Day, createDate.Month.ToString("D2"), createDate.Year.ToString().ConvertToBE()));
        //                                        string DocSet = string.Format("{0}_{1}/{2}{3}_{4}", DocTypeCode, newRunningNo, projectYear, objDocument.Category == "internal" ? string.Format("_{0}", objDocument.RequestorDepartmentID) : "", string.Format("{0}{1}{2}", createDate.Day.ToString("D2"), createDate.Month.ToString("D2"), createDate.Year.ToString().ConvertToBE()));
        //                                        DocSet = DocSet.Replace("/", "_");
        //                                        objDocument.DocSet = DocSet;
        //                                        objDocument.DocLib = DocLibName;

        //                                        SharedRules.CreateDocumentSet(DocLibName, DocSet, null);
        //                                        //SharedRules.UploadFileIntoDocumentSet(DocLibName, DocSet, string.Format("{0}{1}", DocSet, ".pdf"), stream, "", SharedRules.LogonName());

        //                                        if ((objDocument.DocTypeCode != "Im" && objDocument.Type != "Save") ||
        //                                             (objDocument.Status == "Completed" && (objDocument.DocTypeCode == "M" || objDocument.DocTypeCode == "Im") && objDocument.Type != "Save"))
        //                                        {
        //                                            dtAttachTable.Columns.Add("Sequence");
        //                                            dtAttachTable.Columns.Add("AttachFile");
        //                                            dtAttachTable.Columns.Add("FileName");
        //                                            dtAttachTable.Columns.Add("ActorName");
        //                                            dtAttachTable.Columns.Add("AttachDate");
        //                                            dtAttachTable.Columns.Add("AttachFilePath");
        //                                            dtAttachTable.Columns.Add("ActorID");
        //                                            dtAttachTable.Columns.Add("DocSetName");
        //                                            dtAttachTable.Columns.Add("DocLibName");
        //                                            dtAttachTable.Columns.Add("IsPrimary");

        //                                            DataRow dr = dtAttachTable.NewRow();

        //                                            dr["Sequence"] = 1;
        //                                            dr["AttachFile"] = DocSet + ".pdf";
        //                                            dr["ActorName"] = "Generated by E-Document System";
        //                                            dr["AttachDate"] = DateTime.Now;
        //                                            dr["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, DocSet + ".pdf");

        //                                            string DocPDFPath = dr["AttachFilePath"].ToString();
        //                                            objDocument.AttachFilePath = DocPDFPath;

        //                                            if (!string.IsNullOrWhiteSpace(objDocument.AttachWordPath))
        //                                            {
        //                                                objDocument.AttachWordPath = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, string.Format("PIMEdocumentTemplate-[{0}].docx", objDocument.DocTypeCode));

        //                                            }
        //                                            dr["DocSetName"] = DocSet;
        //                                            dr["DocLibName"] = "Document_Library";
        //                                            dr["ActorID"] = "-";
        //                                            dr["IsPrimary"] = "Y";
        //                                            dtAttachTable.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                    catch (Exception ex)
        //                                    {
        //                                        throw ex;
        //                                        //Extension.LogWriter.Write(ex);
        //                                    }
        //                                    #region | Move File to DocLib |

        //                                    SharedRules.CopyFileToAnotherDocSet("TempDocument", oDocSet, "Document_Library", objDocument.DocSet);

        //                                    DataTable vs_attachFileTable = Extension.ListToDataTable<TRNAttachFileDoc>(db.TRNAttachFileDocs.Where(x => x.DocID == objDocument.DocID).ToList());
        //                                    foreach (DataRow fileRow in vs_attachFileTable.Rows)
        //                                    {
        //                                        fileRow["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), "Document_Library", objDocument.DocSet, fileRow["AttachFile"]);
        //                                        fileRow["DocSetName"] = objDocument.DocSet;
        //                                        fileRow["DocLibName"] = "Document_Library";
        //                                    }
        //                                    db.SubmitChanges();
        //                                    #endregion
        //                                }
        //                                #endregion

        //                                #region | Update/Insert Attach Document |
        //                                //update or insert Attach Document
        //                                if (!string.IsNullOrEmpty(objDocument.DocID.ToString()))
        //                                {
        //                                    List<TRNAttachFileDoc> objListTRNAttachDoc = new List<TRNAttachFileDoc>();
        //                                    objListTRNAttachDoc = db.TRNAttachFileDocs.ToList();
        //                                    IEnumerable<TRNAttachFileDoc> queryAD = (from TRNAttachFileDoc attachDoc in objListTRNAttachDoc
        //                                                                             where attachDoc.DocID == objDocument.DocID
        //                                                                             select attachDoc);
        //                                    db.TRNAttachFileDocs.DeleteAllOnSubmit(queryAD);
        //                                    db.SubmitChanges();
        //                                }

        //                                List<TRNAttachFileDoc> listAttachDocument = new List<TRNAttachFileDoc>();
        //                                if (!dtAttachTable.DataTableIsNullOrEmpty())
        //                                {

        //                                    foreach (DataRow dr in dtAttachTable.Rows)
        //                                    {
        //                                        TRNAttachFileDoc objAttachDocument = new TRNAttachFileDoc();
        //                                        objAttachDocument.DocID = objDocument.DocID;
        //                                        //objAttachDocument.ActorID = Convert.ToInt32(dr["ActorID"].ToString());
        //                                        objAttachDocument.ActorName = dr["ActorName"].ToString();
        //                                        objAttachDocument.AttachDate = DateTime.Parse(dr["AttachDate"].ToString());
        //                                        objAttachDocument.AttachFile = dr["AttachFile"].ToString();
        //                                        objAttachDocument.AttachFIlePath = dr["AttachFilePath"].ToString();
        //                                        objAttachDocument.DocSetName = dr["DocSetName"].ToString();
        //                                        objAttachDocument.DocLibName = dr["DocLibName"].ToString();
        //                                        objAttachDocument.IsPrimary = dr["IsPrimary"].ToString();

        //                                        listAttachDocument.Add(objAttachDocument);
        //                                    }


        //                                }
        //                                if (listAttachDocument.Count > 0)
        //                                {
        //                                    db.TRNAttachFileDocs.InsertAllOnSubmit(listAttachDocument);
        //                                    db.SubmitChanges();
        //                                }
        //                                #endregion
        //                            }
        //                            else
        //                            {
        //                                //Generic Approval
        //                                int currentApproveLevel = Convert.ToInt32(objDocument.CurrentApprovalLevel);
        //                                objDocument.WaitingFor = listApprover[currentApproveLevel].EmpID.ToString();
        //                                objDocument.WaitingForDeptID = listApprover[currentApproveLevel].DepartmentID.ToString();
        //                                objDocument.CurrentApprovalLevel = objDocument.CurrentApprovalLevel + 1;
        //                            }

        //                            #region | Update/Insert History |
        //                            //Update History
        //                            TRNHistory objActionHistory = new TRNHistory();
        //                            objActionHistory.DocID = objDocument.DocID;
        //                            objActionHistory.EmpID = vs_CurrentUserID;
        //                            objActionHistory.ActionName = "Approve";
        //                            objActionHistory.ActionDate = DateTime.Now;
        //                            objActionHistory.StatusBefore = "Wait for Approve";
        //                            db.TRNHistories.InsertOnSubmit(objActionHistory);
        //                            db.SubmitChanges();
        //                            #endregion

        //                            #region | Send Email |
        //                            if ((objDocument.Status == "Rejected" || objDocument.Status == "Completed"))
        //                            { }
        //                            else if (objDocument.Status == "Wait for Admin Centre")
        //                            {
        //                                Extension.SendEmailTemplate(objDocument.Status, objDocument.WaitingFor.ToString(), objDocument.WaitingForDeptID.ToString(), "Approve", "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
        //                            }
        //                            else
        //                            {
        //                                Extension.SendEmailTemplate(objDocument.Status, objDocument.WaitingFor.ToString(), objDocument.WaitingForDeptID.ToString(), "Approve", "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);

        //                            }
        //                            #endregion
        //                            successCounter++;
        //                        }
        //                        else
        //                        {
        //                            failedCounter++;
        //                        }
        //                        #endregion
        //                    }
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Extension.LogWriter.Write(ex);

        //        }
        //    }
        //    string sMessage = "";
        //    if (successCounter > 0)
        //    {
        //        sMessage = string.Format("{0} งาน ดำเนินการอนุมิติแล้ว ", successCounter);
        //        //Extension.MessageBox(Page, string.Format("ดำเนินการอนุมิติแล้ว {0} งาน", successCounter));
        //    }
        //    if (failedCounter > 0)
        //    {
        //        if (!string.IsNullOrWhiteSpace(sMessage))
        //        {
        //            sMessage += "\\n";
        //        }
        //        sMessage += string.Format("{0} งานที่ไม่สามารถอนุมัติผ่านหน้านี้ได้", failedCounter);
        //        //Extension.MessageBox(Page, string.Format("{0} งานที่ไม่สามารถอนุมัติผ่านหน้านี้ได้", failedCounter));
        //    }
        //    if (!string.IsNullOrWhiteSpace(sMessage))
        //    {
        //        Extension.MessageBox(Page, sMessage);
        //    }
        //    div_approvement.Visible = false;
        //    btnSearch_Click(sender, e);
        //    //Extension.Redirect(Page, "WorkList.aspx");
        //}

        #endregion
    }
}
