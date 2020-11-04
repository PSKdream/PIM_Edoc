using Microsoft.SharePoint;
using PIMEdoc_CR.Default.Rule;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using File = Microsoft.SharePoint.Client.File;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace PIMEdoc_CR.Default.e_Document
{
    public partial class e_DocumentUserControl : UserControl
    {
        #region | ViewState |
        private string vs_PK
        {
            get
            {
                return (string)ViewState["vs_PK"];
            }
            set
            {
                ViewState["vs_PK"] = value;
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
        private string vs_Status
        {
            get
            {
                return (string)ViewState["vs_Status"];
            }
            set
            {
                ViewState["vs_Status"] = value;
            }
        }
        private string vs_WaitingFor
        {
            get
            {
                return (string)ViewState["vs_WaitingFor"];
            }
            set
            {
                ViewState["vs_WaitingFor"] = value;
            }
        }
        private bool vs_isAttach
        {
            get
            {
                return (bool)ViewState["sAttach"];
            }
            set
            {
                ViewState["sAttach"] = value;
            }
        }
        private bool vs_isDocAttach
        {
            get
            {
                if (ViewState["vs_isDocAttach"] == null)
                {
                    return false;
                }
                return (bool)ViewState["vs_isDocAttach"];
            }
            set
            {
                ViewState["vs_isDocAttach"] = value;
            }
        }
        private bool vs_isSecret
        {
            get
            {
                return (bool)ViewState["vs_isSecret"];
            }
            set
            {
                ViewState["vs_isSecret"] = value;
            }
        }
        private bool vs_isExternalUpload
        {
            get
            {
                if (ViewState["vs_isExternalUpload"] == null)
                {
                    return false;
                }
                return (bool)ViewState["vs_isExternalUpload"];
            }
            set
            {
                ViewState["vs_isExternalUpload"] = value;
            }
        }
        private bool vs_isLastApproval
        {
            get
            {
                return (bool)ViewState["vs_isLastApproval"];
            }
            set
            {
                ViewState["vs_isLastApproval"] = value;
            }
        }
        private bool vs_isAssignable
        {
            get
            {
                if (ViewState["vs_isAssignable"] == null)
                {
                    return false;
                }
                return (bool)ViewState["vs_isAssignable"];
            }
            set
            {
                ViewState["vs_isAssignable"] = value;
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
        private string vs_DocPDFPath
        {
            get
            {
                return (string)ViewState["vs_DocPDFPath"];
            }
            set
            {
                ViewState["vs_DocPDFPath"] = value;
            }
        }
        private string vs_DocWordPath
        {
            get
            {
                if (ViewState["vs_DocWordPath"] == null)
                {
                    return "";
                }
                return (string)ViewState["vs_DocWordPath"];
            }
            set
            {
                ViewState["vs_DocWordPath"] = value;
            }
        }
        private string vs_popUpTarget
        {
            get
            {
                return (string)ViewState["sPopupTarget"];
            }
            set
            {
                ViewState["sPopupTarget"] = value;
            }
        }
        private string vs_DeptTarget
        {
            get
            {
                return (string)ViewState["vs_DeptTarget"];
            }
            set
            {
                ViewState["vs_DeptTarget"] = value;
            }
        }
        private string vs_CreatorID
        {
            get
            {
                if (ViewState["vs_CreatorID"] == null) { return ""; }
                return (string)ViewState["vs_CreatorID"];
            }
            set
            {
                ViewState["vs_CreatorID"] = value;
            }
        }
        private string vs_RequestorID
        {
            get
            {
                if (ViewState["vs_RequestorID"] == null) { return ""; }
                return (string)ViewState["vs_RequestorID"];
            }
            set
            {
                ViewState["vs_RequestorID"] = value;
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
        private string vs_CurrentUser_DepID
        {
            get
            {
                return (string)ViewState["vs_CurrentUser_DepID"];
            }
            set
            {
                ViewState["vs_CurrentUser_DepID"] = value;
            }
        }
        private string vs_CurrentUserPosID
        {
            get
            {
                return (string)ViewState["vs_CurrentUserPosID"];
            }
            set
            {
                ViewState["vs_CurrentUserPosID"] = value;
            }
        }
        private string vs_CurrentRunningNo
        {
            get
            {
                return (string)ViewState["vs_CurrentRunningNo"];
            }
            set
            {
                ViewState["vs_CurrentRunningNo"] = value;
            }
        }
        private bool vs_isDelegate
        {
            get
            {
                return (bool)ViewState["vs_isDelegate"];
            }
            set
            {
                ViewState["vs_isDelegate"] = value;
            }
        }
        private bool vs_isDocSetCreated
        {
            get
            {
                return (bool)ViewState["sDocSet"];
            }
            set
            {
                ViewState["sDocSet"] = value;
            }
        }
        private bool vs_isShowAllDept
        {
            get
            {
                if (ViewState["vs_isShowAllDept"] == null)
                {
                    return false;
                }
                return (bool)ViewState["vs_isShowAllDept"];
            }
            set
            {
                ViewState["vs_isShowAllDept"] = value;
            }
        }
        private bool vs_isOccurBySubDepartment
        {
            get
            {
                if (ViewState["vs_isOccurBySubDepartment"] == null)
                {
                    return false;
                }
                return (bool)ViewState["vs_isOccurBySubDepartment"];
            }
            set
            {
                ViewState["vs_isOccurBySubDepartment"] = value;
            }
        }
        private bool startConversion
        {
            get
            {
                return (bool)ViewState["startConversion"];
            }
            set
            {
                ViewState["startConversion"] = value;
            }
        }
        private DataTable vs_DtRefDoc
        {
            get
            {
                return (DataTable)ViewState["vs_DtRefDoc"];
            }
            set
            {
                ViewState["vs_DtRefDoc"] = value;
            }
        }
        private DataTable vs_ApprovalList
        {
            get
            {
                return (DataTable)ViewState["sApprovalList"];
            }
            set
            {
                ViewState["sApprovalList"] = value;
            }
        }
        private DataTable vs_Assign
        {
            get
            {
                return (DataTable)ViewState["vs_Assign"];
            }
            set
            {
                ViewState["vs_Assign"] = value;
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
        private DataTable vs_PermissionList
        {
            get
            {
                return (DataTable)ViewState["sPermissionList"];
            }
            set
            {
                ViewState["sPermissionList"] = value;
            }
        }
        private DataTable vs_ToDepartment
        {
            get
            {
                if (ViewState["vs_ToDepartment"] == null)
                {
                    DataTable dt_ToDepartment = new DataTable();
                    dt_ToDepartment.Columns.Add("Sequence", typeof(String));
                    dt_ToDepartment.Columns.Add("DeptID", typeof(String));
                    dt_ToDepartment.Columns.Add("DEPARTMENT_CODE", typeof(String));
                    dt_ToDepartment.Columns.Add("DEPARTMENT_NAME_TH", typeof(String));
                    dt_ToDepartment.Columns.Add("DEPARTMENT_NAME_EN", typeof(String));
                    return dt_ToDepartment;
                }
                return (DataTable)ViewState["vs_ToDepartment"];
            }
            set
            {
                ViewState["vs_ToDepartment"] = value;
            }
        }
        private DataTable vs_GroupMailList
        {
            get
            {
                return (DataTable)ViewState["sGroupMailList"];
            }
            set
            {
                ViewState["sGroupMailList"] = value;
            }
        }
        private DataTable vs_attachFileTable
        {
            get
            {
                return (DataTable)ViewState["attachFileTable"];
            }
            set
            {
                ViewState["attachFileTable"] = value;
            }
        }
        private DataTable vs_attachDocTable
        {
            get
            {
                return (DataTable)ViewState["vs_attachDocTable"];
            }
            set
            {
                ViewState["vs_attachDocTable"] = value;
            }
        }
        private DataTable vs_DataTestDepartment
        {
            get
            {
                return (DataTable)ViewState["sDataTestDepartment"];
            }
            set
            {
                ViewState["sDataTestDepartment"] = value;
            }
        }

        readonly CultureInfo _ctli = Extension._ctli;

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
        #endregion
        private DataClassesDataAccessDataContext db;
        #region | PageLoad & Initial & Load Data |
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Disable buttono", "DisableButton();", true);
                ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Disable Enter", "DisableEnter();", true);

                if (!Page.IsPostBack)
                {
                    vs_Role = Request.QueryString["Role"] != null ? Request.QueryString["Role"].ToString() : "Creator";
                    vs_PK = Request.QueryString["PK"] != null ? Request.QueryString["PK"].ToString() : string.Empty;

                    string logonName = Rule.SharedRules.LogonName();
                    vs_CurrentUserLoginName = logonName;
                    if (Request.QueryString["USERID"] != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Request.QueryString["USERID"].ToString()))
                        {
                            vs_CurrentUserID = Request.QueryString["USERID"].ToString();
                        }
                        else
                        {
                            vs_CurrentUserID = "No user";
                        }
                    }
                    else
                    {
                        vs_CurrentUserID = Rule.SharedRules.FindUserID(logonName, this.Page);
                    }

                    //vs_CurrentUserID = Rule.SharedRules.FindUserID(logonName, this.Page);
                    //if (string.IsNullOrEmpty(vs_CurrentUserID))
                    //{
                    //    vs_CurrentUserID = Request.QueryString["USERID"] != null
                    //        ? Request.QueryString["USERID"].ToString()
                    //        : "No user";
                    //}
                    SpecificEmployeeData.RootObject emp = Extension.GetSpecificEmployeeFromTemp(Page, vs_CurrentUserID);
                    vs_CurrentUser_DepID = "";
                    if (emp != null)
                    {
                        if (emp.RESULT != null)
                        {
                            vs_CurrentUser_DepID = emp.RESULT.FirstOrDefault().DEPARTMENT_ID;
                        }
                    }
                    vs_WaitingFor = vs_CurrentUserID;
                    InitialData();

                    DataTable dtDb = Extension.GetDataTable("SiteSetting");
                    {
                        if (!dtDb.DataTableIsNullOrEmpty())
                        {
                            vs_ConnectionString = dtDb.Rows[0]["Value"].ToString();
                        }
                    }
                    LoadData();
                    if (!string.IsNullOrEmpty(vs_PK))
                    {
                        UserAuthorize();
                    }
                }
                lblRole.Text = vs_Role;
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                Extension.MessageBox(this.Page, ex.Message);
            }
            finally
            {

                //ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Enable buttono", "EnableButton();", true);
            }
        }
        private void LoadData()
        {
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(vs_ConnectionString);
            if (!string.IsNullOrWhiteSpace(vs_PK))
            {
                TRNDocument objDocument = db.TRNDocuments.SingleOrDefault(x => x.DocID == Convert.ToInt32(vs_PK));
                if (objDocument != null)
                {
                    List<TRNHistory> listHistory = db.TRNHistories.Where(x => x.DocID == objDocument.DocID).ToList();
                    List<TRNAttachFileDoc> listAttach = db.TRNAttachFileDocs.Where(x => x.DocID == objDocument.DocID).ToList();
                    List<TRNAssign> listAssign = db.TRNAssigns.Where(x => x.DocID == objDocument.DocID).ToList();
                    try
                    {
                        DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();

                        vs_isShowAllDept = !(objDocument.Status.Equals(Extension._Draft) || objDocument.Status.Equals(Extension._Rework));
                        #region | Creator |

                        vs_CreatorID = objDocument.CreatorID;
                        int sCreatorDeptID = Convert.ToInt32(objDocument.CreatorDepartmentID);
                        int sCreatorSubDeptID = Convert.ToInt32(objDocument.CreatorSubDepartmentID);
                        int sCreatorPosID = Convert.ToInt32(objDocument.CreatorPositionID);
                        //                  var sResult = dtEmp.Select(string.Format(@"EMPLOYEEID='{0}' AND Department_ID ='{1}' AND SubDepartment_ID ='{2}' AND Position_TD ='{3}'", sCreatorID, sCreatorDeptID, sCreatorSubDeptID, sCreatorPosID)).SingleOrDefault();
                        string lang = "TH";
                        SpecificEmployeeData.RootObject objCreator = Extension.GetSpecificEmployeeFromTemp(this.Page, vs_CreatorID);
                        if (objCreator != null)
                        {
                            //Creator Info
                            lbl_RequestorID.Text = lbl_CreatorID.Text = objDocument.CreatorID;
                            lbl_RequestorName.Text = lbl_CreatorName.Text = lang == "TH" ?
                                string.Format("{0}{1} {2}", objCreator.PREFIX_TH, objCreator.FIRSTNAME_TH, objCreator.LASTNAME_TH) :
                                string.Format("{0}{1} {2}", objCreator.PREFIX_EN, objCreator.FIRSTNAME_EN, objCreator.LASTNAME_EN); // sResult[string.Format("EmployeeName_{0}", lang)].ToString();
                            lbl_RequestorTel.Text = lbl_CreatorTel.Text = objCreator.TELEPHONE;// sResult["TELEPHONE"].ToString();

                            CreatorDeptChange(objCreator);
                            if (ddl_CreatorDepartment.Items.FindByValue(sCreatorDeptID.ToString()) != null) { ddl_CreatorDepartment.SelectedValue = sCreatorDeptID.ToString(); }

                            CreatorSubDeptChange(objCreator);
                            if (ddl_CreatorSubDepartment.Items.FindByValue(sCreatorSubDeptID.ToString()) != null) { ddl_CreatorSubDepartment.SelectedValue = sCreatorSubDeptID.ToString(); }

                            CreatorPosChange(objCreator);
                            if (ddl_CreatorPosition.Items.FindByValue(sCreatorPosID.ToString()) != null) { ddl_CreatorPosition.SelectedValue = sCreatorPosID.ToString(); }
                        }
                        else { throw new Exception("Missing Creator information"); }

                        #endregion

                        #region | Requestor |

                        vs_RequestorID = objDocument.RequestorID;
                        int sRequestorDeptID = Convert.ToInt32(objDocument.RequestorDepartmentID);
                        int sRequestorSubDeptID = Convert.ToInt32(objDocument.RequestorSubDepartmentID);
                        int sRequestorPosID = Convert.ToInt32(objDocument.RequestorPositionID);
                        //                  var sResult = dtEmp.Select(string.Format(@"EMPLOYEEID='{0}' AND Department_ID ='{1}' AND SubDepartment_ID ='{2}' AND Position_TD ='{3}'", sRequestorID, sRequestorDeptID, sRequestorSubDeptID, sRequestorPosID)).SingleOrDefault();
                        SpecificEmployeeData.RootObject objRequestor = Extension.GetSpecificEmployeeFromTemp(this.Page, vs_RequestorID);

                        if (objRequestor != null)
                        {
                            InitialData();

                            //Requestor Info
                            lbl_RequestorID.Text = objDocument.RequestorID.ToString();
                            lbl_RequestorName.Text = lang == "TH" ?
                                string.Format("{0}{1} {2}", objRequestor.PREFIX_TH, objRequestor.FIRSTNAME_TH, objRequestor.LASTNAME_TH) :
                                string.Format("{0}{1} {2}", objRequestor.PREFIX_EN, objRequestor.FIRSTNAME_EN, objRequestor.LASTNAME_EN); //sResult[string.Format("EmployeeName_{0}", lang)].ToString();
                            lbl_RequestorTel.Text = objRequestor.TELEPHONE;//sResult["TELEPHONE"].ToString();

                            RequestorDeptChange(objRequestor);
                            ddl_RequestorDepartment.SelectedValue = sRequestorDeptID.ToString();
                            RequestorSubDeptChange(objRequestor);
                            ddl_RequestorSubDepartment.SelectedValue = sRequestorSubDeptID.ToString();
                            RequestorPosChange(objRequestor);
                            ddl_RequestorPosition.SelectedValue = sRequestorPosID.ToString();

                        }
                        else { throw new Exception("Missing Requestor information"); }
                        #endregion

                        #region | Information Panel |
                        vs_Status = lbl_Status.Text = objDocument.Status.ToString();
                        if (objDocument.DocSet != null) { lbl_docSet.Text = objDocument.DocSet; vs_isDocSetCreated = true; }
                        if (objDocument.DocNo != null) { lbl_DocumentNo.Text = objDocument.DocNo.ToString(); }
                        if (!string.IsNullOrWhiteSpace(objDocument.ApproveDate.ToString()))
                        {
                            hdn_ApproveDate.Value = ((DateTime)objDocument.ApproveDate).ToString("dd/MM/yyyy", _ctli);
                        }
                        txt_CreateDate.Text = ((DateTime)objDocument.CreatedDate).ToString("dd/MM/yyyy", _ctli);
                        txt_DocDescription.Text = objDocument.Description.ToString();
                        txt_FromDepartment.Text = objDocument.FromDepartmentName.ToString();
                        hdn_FromDepartmentID.Text = objDocument.FromDepartmentID.ToString();
                        txt_ToDepartment.Text = objDocument.ToDepartmentName.ToString();
                        hdn_ToDepartment.Text = objDocument.ToDepartmentID.ToString();


                        vs_DocWordPath = objDocument.AttachWordPath ?? "";
                        if (!string.IsNullOrWhiteSpace(vs_DocWordPath))
                        {
                            chk_isAttachWord.Checked = true;
                            chk_IsAttahcWordCheckedChanged();
                            btn_DownloadTemplate.Text = "Edit Document";
                            btn_DownloadTemplate.CommandName = "Edit";
                            //btn_DownloadTemplate.PostBackUrl = vs_DocWordPath; 
                        }

                        txt_to.Text = objDocument.To;
                        txt_CC.Text = objDocument.CC;
                        txt_Attachment.Text = objDocument.Attachment;
                        txt_RecieveDocNo.Text = objDocument.RecieveDocumentNo;
                        txt_Source.Text = objDocument.DocumentSource;
                        if (objDocument.SendToID != null)
                        {
                            lbl_sendToID.Text = objDocument.SendToID;
                            SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(Page, lbl_sendToID.Text);
                            if (empData != null)
                            {
                                string nameTH = string.Format("{0}{1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH, empData.LASTNAME_TH);
                                string nameEN = string.Format("{0}{1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN, empData.LASTNAME_EN);
                                txt_SendTo.Text = vs_Lang == "TH" ? nameTH : nameEN;
                            }
                        }
                        if (objDocument.DocumentDate != null)
                        {
                            DateTime documentDate = DateTime.Parse(objDocument.DocumentDate.ToString());
                            txt_DocumentDate.Text = documentDate.SetDateTimeFormat();
                        }
                        if (objDocument.DocumentRecieveDate != null)
                        {
                            DateTime receiveDate = DateTime.Parse(objDocument.DocumentRecieveDate.ToString());
                            txt_DocumentRecieve.Text = receiveDate.SetDateTimeFormat();
                        }


                        if (objDocument.Deadline != null)
                        {
                            DateTime deadline = (DateTime)objDocument.Deadline;
                            txt_deadline.Text = deadline.SetDateTimeFormat();
                        }

                        chk_DOA.Checked = objDocument.DOA.ToString().Equals("Y") ? true : false;
                        chk_AutoStamp.Checked = objDocument.AutoStamp.ToString().Equals("Y") ? true : false;
                        chk_InternalStamp.Checked = objDocument.InternalOnlyStamp ?? false;
                        rdb_Type.SelectedValue = objDocument.Type.ToString();
                        if (rdb_Type.SelectedValue == "Submit")
                        {
                            DivButtonForSave.Visible = false;
                            DivButtonForSubmit.Visible = true;
                            panel_Approval.Visible = true;
                            panel_attachDoc.Visible = false;
                            PanelInfoExtend.Visible = true;
                            divContent.Visible = true;
                            if (chk_DOA.Checked)
                            {
                                tblAmount.Visible = true;
                                txt_Amount.Text = objDocument.Amount != null ? objDocument.Amount.ToString() : "";
                                lbl_CostCenter.Text = objDocument.CostCenter != null
                                    ? objDocument.CostCenter.ToString()
                                    : "";
                            }
                            //ddl_ApprovalMatrix.SelectedValue = objDocument.ApprovalMatrix == null
                            //    ? ""
                            //    : objDocument.ApprovalMatrix.ToString();

                            //if (ddl_ApprovalMatrix.SelectedIndex > 0)
                            //{
                            //    div_approvalMatrix.Enabled = true;
                            //}
                        }
                        else
                        {
                            DivButtonForSave.Visible = true;
                            DivButtonForSubmit.Visible = false;
                            panel_Approval.Visible = false;
                            panel_attachDoc.Visible = true;
                            PanelInfoExtend.Visible = false;
                            divContent.Visible = false;
                        }
                        ddl_Permission.SelectedValue = objDocument.PermissionType.ToString();
                        if (objDocument.PermissionType.ToString().Equals("Secret"))
                        {
                            vs_isSecret = true;
                            div_permission.Visible = true;
                            div_permissionPublic.Visible = false;
                        }
                        rdb_Category.SelectedValue = objDocument.Category.ToString();
                        if (rdb_Category.SelectedValue == "centre")
                        {
                            rcbCategoryChanged();
                        }
                        ddl_DocType.SelectedValue = objDocument.DocTypeCode.ToString();
                        ddl_DocType_SelectedIndexChanged(null, new EventArgs());
                        vs_isOccurBySubDepartment = chkDocNoBySubDepartment.Checked = objDocument.IsOccurBySubDepartment ?? false;
                        #region | Ex.Condition BY DocType |
                        if (ddl_DocType.SelectedValue == "Other")
                        {
                            chk_AutoStamp.Visible = true;
                            info_OtherDoctype.Visible = true;
                            ddl_otherDocType.SelectedValue = objDocument.OtherDocType;
                        }
                        else if (ddl_DocType.SelectedValue == "Ex" || ddl_DocType.SelectedValue == "ExEN")
                        {
                            info_extend.Visible = true;
                            panel_Attachment.Visible = true;
                            txt_to.Text = objDocument.To;
                            txt_CC.Text = objDocument.CC;
                            txt_Attachment.Text = objDocument.Attachment;
                        }
                        else if (ddl_DocType.SelectedValue == "Im")
                        {
                            vs_DocPDFPath = objDocument.AttachFilePath;
                            chk_isAttachWord.Visible = false;
                            btn_DownloadTemplate.Visible = false;

                            info_extend.Visible = true;
                            panel_To.Visible = false;
                            panel_Cc.Visible = false;

                            panel_attachFile.Visible = false;
                            //panel_summerNote.Visible = false;
                            panel_RecieveDocNo.Visible = true;
                            panel_Source.Visible = true;
                            panel_RecieveDate.Visible = true;
                            chk_AutoStamp.Visible = true;
                            if (lbl_Status.Text == "Completed")
                            {
                                //info_extend.Visible = false;
                            }
                        }
                        else if (ddl_DocType.SelectedValue == "M")
                        {
                            chk_AutoStamp.Visible = true;
                            memo_extend.Visible = true;
                        }
                        #endregion

                        ddl_Priority.SelectedValue = objDocument.Priority.ToString();

                        txt_title.Text = objDocument.Title.ToString();
                        txt_subtitle.Text = objDocument.SubTitle;
                        #endregion

                        #region | To Department |
                        if (!string.IsNullOrWhiteSpace(hdn_ToDepartment.Text))
                        {
                            string[] toDept = hdn_ToDepartment.Text.Split(',');
                            int counter = 1;
                            DataTable dt = vs_ToDepartment;
                            foreach (string item in toDept)
                            {
                                DataTable objDept = Extension.GetSpecificDepartmentDataFromTemp(Page, item);
                                if (!objDept.DataTableIsNullOrEmpty())
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["Sequence"] = counter;
                                    dr["DeptID"] = item;
                                    dr["DEPARTMENT_CODE"] = objDept.Rows[0]["DEPARTMENT_CODE"];
                                    dr["DEPARTMENT_NAME_TH"] = objDept.Rows[0]["DEPARTMENT_NAME_TH"];
                                    dr["DEPARTMENT_NAME_EN"] = objDept.Rows[0]["DEPARTMENT_NAME_EN"];
                                    dt.Rows.Add(dr);
                                }
                                counter++;
                            }
                            vs_ToDepartment = dt;
                            gv_ToDepartment.DataSource = vs_ToDepartment;
                            gv_ToDepartment.DataBind();
                        }
                        #endregion

                        #region | Reference Document |
                        List<TRNReferenceDoc> listRefDocument = db.TRNReferenceDocs.Where(x => x.DocID == objDocument.DocID).ToList();
                        if (listRefDocument.Count > 0)
                        {
                            DataTable dtRefDoc = Extension.ListToDataTable<TRNReferenceDoc>(listRefDocument);
                            dtRefDoc.Columns.Add("Sequence");
                            int iSequence = 1;
                            foreach (DataRow objRefDoc in dtRefDoc.Rows)
                            {
                                objRefDoc["Sequence"] = iSequence;
                                iSequence++;
                            }

                            vs_DtRefDoc = dtRefDoc;
                            gv_ReferenceDocument.DataSource = vs_DtRefDoc;
                            gv_ReferenceDocument.DataBind();

                        }
                        #endregion

                        #region | Permission & Group Mail |
                        if (ddl_Permission.SelectedValue.Equals("Secret"))
                        {
                            List<TRNPermission> listPermission =
                                db.TRNPermissions.Where(x => x.DocID == objDocument.DocID).ToList();

                            if (listPermission.Count > 0)
                            {
                                DataTable dtPermission = Extension.ListToDataTable<TRNPermission>(listPermission);
                                int iSequence = 1;
                                foreach (DataRow objPermission in dtPermission.Rows)
                                {
                                    objPermission["Sequence"] = iSequence;
                                    iSequence++;
                                }
                                vs_PermissionList = dtPermission;
                                gv_Permission.DataSource = vs_PermissionList;
                                gv_Permission.DataBind();

                            }
                        }
                        else
                        {
                            List<TRNGroupMail> listGroupMail =
                                db.TRNGroupMails.Where(x => x.DocID == objDocument.DocID.ToString()).ToList();

                            if (listGroupMail.Count > 0)
                            {
                                DataTable dtGroupMail = Extension.ListToDataTable<TRNGroupMail>(listGroupMail);
                                int iSequence = 1;
                                foreach (DataRow objGroupMail in dtGroupMail.Rows)
                                {
                                    objGroupMail["Sequence"] = iSequence;
                                    iSequence++;
                                }
                                vs_GroupMailList = dtGroupMail;
                                gv_GroupEmail.DataSource = vs_GroupMailList;
                                gv_GroupEmail.DataBind();

                            }
                        }
                        #endregion

                        #region | Approval List |
                        if (rdb_Type.SelectedValue.Equals("Submit"))
                        {
                            List<TRNApprover> listApprover = db.TRNApprovers.Where(x => x.DocID == objDocument.DocID).ToList();
                            if (listApprover.Count > 0)
                            {
                                DataTable dtApprovor = Extension.ListToDataTable<TRNApprover>(listApprover);
                                int iSequence = 1;
                                foreach (DataRow drApprovor in dtApprovor.Rows)
                                {
                                    drApprovor["Sequence"] = iSequence;
                                    iSequence++;
                                }
                                vs_ApprovalList = dtApprovor;
                                gvApprovelList.DataSource = vs_ApprovalList;
                                gvApprovelList.DataBind();
                            }
                        }
                        #endregion

                        #region | Attachment |
                        if (listAttach != null && listAttach.Count > 0)
                        {
                            if (listAttach.Count(x => x.IsPrimary == "Y") > 0)
                            {
                                panel_attachDoc.Visible = true;
                                vs_DocPDFPath = listAttach.First(x => x.IsPrimary == "Y").AttachFIlePath;
                                //panel_attachDoc.Enabled = false;
                            }


                            DataTable dtDocAttach = Extension.ListToDataTable<TRNAttachFileDoc>(listAttach.Where(x => x.IsPrimary == "Y"));
                            if (!dtDocAttach.DataTableIsNullOrEmpty())
                            {
                                dtDocAttach.Columns.Add("Sequence");
                                for (int i = 0; i < dtDocAttach.Rows.Count; i++)
                                {
                                    dtDocAttach.Rows[i]["Sequence"] = i + 1;
                                }
                                vs_attachDocTable = dtDocAttach;
                                gv_AttachDocumentFile.DataSource = dtDocAttach;
                                gv_AttachDocumentFile.DataBind();
                            }

                            if (dtDocAttach.Rows.Count > 0)
                            {
                                //vs_DocPDFPath = dtDocAttach.Rows[0]["AttachFilePath"].ToString();
                            }

                            DataTable dtFileAttach = Extension.ListToDataTable<TRNAttachFileDoc>(listAttach.Where(x => x.IsPrimary == "N"));
                            if (!dtFileAttach.DataTableIsNullOrEmpty())
                            {
                                dtFileAttach.Columns.Add("Sequence");
                                for (int i = 0; i < dtFileAttach.Rows.Count; i++)
                                {
                                    dtFileAttach.Rows[i]["Sequence"] = i + 1;
                                }
                                vs_attachFileTable = dtFileAttach;
                                gv_AttachFile.DataSource = dtFileAttach;
                                gv_AttachFile.DataBind();

                            }

                            if (!dtDocAttach.DataTableIsNullOrEmpty())
                            {
                                if (dtDocAttach.Rows[0]["DocSetName"] != null)
                                {
                                    lbl_docSet.Text = dtDocAttach.Rows[0]["DocSetName"].ToString();
                                    vs_isDocSetCreated = true;
                                }
                            }
                            if (!dtFileAttach.DataTableIsNullOrEmpty())
                            {
                                if (dtFileAttach.Rows[0]["DocSetName"] != null)
                                {
                                    lbl_docSet.Text = dtFileAttach.Rows[0]["DocSetName"].ToString();
                                    vs_isDocSetCreated = true;
                                }
                            }

                        }
                        #endregion

                        #region | History |
                        if (listHistory != null && listHistory.Count > 0)
                        {
                            divHistory.Visible = true;
                            gv_ActionHistory.DataSource = listHistory;
                            gv_ActionHistory.DataBind();
                        }
                        #endregion

                        #region | Assign |
                        if (listAssign != null && listAssign.Count > 0)
                        {
                            foreach (TRNAssign item in listAssign)
                            {
                                if (!item.Acknowledge && item.AssignToID == vs_CurrentUserID)
                                {
                                    item.Acknowledge = true;
                                    //Send Email To Requestor
                                    Extension.SendEmailTemplate("Accept Acknowledge", item.ActorID.ToString(), "", "Acknowledge", "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);

                                    //SendEmailTemplate("Acknowledge", item.ActorID.ToString(), "", "Acknowledge", "", objDocument.DocID.ToString());
                                }
                            }
                            db.SubmitChanges();
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Extension.MessageBox(this.Page, ex.Message.ToString());
                        Extension.LogWriter.Write(ex);

                    }

                }
                else { Extension.MessageBox(this.Page, string.Format("Missing Document {0}, Please contact administrator.", vs_PK), "Worklist.aspx"); }
            }

            List<TRNDocument> data = new List<TRNDocument>();
            //data = db.TRNDocuments.Where(x => x.Status == "Completed").ToList();
            DataTable dtDocuement = Extension.ListToDataTable(data);
            vs_DocumentList = dtDocuement;


        }
        private void InitialData()
        {
            vs_Lang = "TH";
            vs_Status = Extension._NewRequest;
            vs_isAttach = false;
            vs_isDocSetCreated = false;
            vs_isSecret = false;
            startConversion = false;
            vs_isLastApproval = false;
            vs_isCentreAdminSarabun = false;
            vs_isCentreAdmin = false;
            vs_isDelegate = false;

            lbl_docSet.Text = Guid.NewGuid().ToString().Replace("-", "");

            if (!string.IsNullOrWhiteSpace(vs_RequestorID))
            {
                lbl_CreatorID.Text = vs_CreatorID;
            }
            else
            {
                lbl_CreatorID.Text = vs_CreatorID = vs_CurrentUserID;
            }
            if (!string.IsNullOrWhiteSpace(vs_RequestorID))
            {
                lbl_RequestorID.Text = vs_RequestorID;
            }
            else
            {
                lbl_RequestorID.Text = vs_RequestorID = vs_CreatorID;
            }



            DataTable dtCentreAdmin = Extension.GetDataTable("MstAdminCentre");
            if (!dtCentreAdmin.DataTableIsNullOrEmpty())
            {
                foreach (DataRow row in dtCentreAdmin.Rows)
                {
                    //if (row["EmpID"].ToString() == vs_CurrentUserID)
                    if (row["UserName"].ToString().Trim().Split(',').Contains(SharedRules.FindUserNameByID(vs_RequestorID, Page)) || row["EmpID"].ToString().Trim().Split(',').Contains(vs_RequestorID))
                    {
                        vs_isCentreAdmin = true;
                        if (row["DeptID"].ToString() == "10")
                        {
                            vs_isCentreAdminSarabun = true;
                        }
                    }
                }

            }


            SpecificEmployeeData.RootObject creatorData = Extension.GetSpecificEmployeeFromTemp(this.Page, vs_CreatorID);
            CreatorDeptChange(creatorData);
            CreatorSubDeptChange(creatorData);
            CreatorPosChange(creatorData);

            lbl_CreatorName.Text = string.Format("{0}{1} {2}", creatorData.PREFIX_TH, creatorData.FIRSTNAME_TH, creatorData.LASTNAME_TH);
            lbl_CreatorTel.Text = creatorData.TELEPHONE.ToString();

            SpecificEmployeeData.RootObject requesterData = Extension.GetSpecificEmployeeFromTemp(this.Page, vs_RequestorID);
            RequestorDeptChange(requesterData);
            RequestorSubDeptChange(requesterData);
            RequestorPosChange(requesterData);

            lbl_RequestorName.Text = string.Format("{0}{1} {2}", requesterData.PREFIX_TH, requesterData.FIRSTNAME_TH, requesterData.LASTNAME_TH);
            lbl_RequestorTel.Text = requesterData.TELEPHONE.ToString();

            txt_FromDepartment.Text = ddl_RequestorDepartment.SelectedItem.Text;
            hdn_FromDepartmentID.Text = ddl_RequestorDepartment.SelectedValue;

            txt_CreateDate.Text = DateTime.Now.ToString("dd/MM/yyyy", _ctli);
            //rdb_Type.SelectedValue = "Submit";
            //rdb_Category.SelectedValue = "internal";

            btn_DocumentFileUpload.Attributes.Add("Style", "Display:none");
            doc_upload.Attributes["onchange"] = "UploadDoc(this)";
            btn_AttachUpload.Attributes.Add("Style", "Display:none");
            file_upload.Attributes["onchange"] = "UploadFile(this)";

            DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", @"<Where><And><Eq><FieldRef Name='Level' /><Value Type='Number'>0</Value></Eq><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq></And></Where>");

            rdb_Type_BindingData(false);

            if (dtDocumentType.Rows.Count > 0)
            {

                ddlDocumentType.DataSource = dtDocumentType;
                ddlDocumentType.DataTextField = vs_Lang == "TH" ? "DocTypeName" : "DocTypeNameEN";
                ddlDocumentType.DataValueField = "Value";
                ddlDocumentType.DataBind();
                ddlDocumentType.Items.Insert(0, new ListItem("-- Please Select --", ""));

                //Check isInternal
                dtDocumentType = Extension.GetDocTypeByCategory("isInternal", vs_isCentreAdmin);

                ddl_DocType.DataSource = dtDocumentType;
                ddl_DocType.DataTextField = vs_Lang == "TH" ? "DocTypeName" : "DocTypeNameEN";
                ddl_DocType.DataValueField = "Value";
                ddl_DocType.DataBind();
                ddl_DocType.Items.Insert(0, new ListItem("-- Please Select --", ""));

            }


            DataTable dtOtherDocType = SharedRules.GetList("MstDocumentType", "<Where><And><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq><And><Eq><FieldRef Name='Level' /><Value Type='Number'>1</Value></Eq><Eq><FieldRef Name='isInternal' /><Value Type='Boolean'>1</Value></Eq></And></And></Where>");
            if (!dtOtherDocType.DataTableIsNullOrEmpty())
            {
                ddl_otherDocType.DataSource = dtOtherDocType;
                ddl_otherDocType.DataTextField = vs_Lang == "TH" ? "DocTypeName" : "DocTypeNameEN";
                ddl_otherDocType.DataValueField = "Value";
                ddl_otherDocType.DataBind();
            }
            Extension.GetDepartmentData(Page);


            btn4 = btn4.SetUpActionButton("Save Draft", "", true, "CssButton custom-btn-Info", string.Format("OnPreventDoubleClick({0}, 'Saving...');", btn4.ClientID));
            btn5 = btn5.SetUpActionButton("Submit", "", true, "CssButton custom-btn-Success", string.Format("OnPreventDoubleClick({0}, 'Submiting...');", btn5.ClientID));

            #region "Initial DataTable"
            vs_DtRefDoc = new DataTable();
            vs_DtRefDoc.Columns.Add("Sequence");
            vs_DtRefDoc.Columns.Add("DocNo");
            vs_DtRefDoc.Columns.Add("Category");
            vs_DtRefDoc.Columns.Add("DocumentType");
            vs_DtRefDoc.Columns.Add("Title");
            vs_DtRefDoc.Columns.Add("RefDocID");

            vs_PermissionList = new DataTable();
            vs_PermissionList.Columns.Add("Sequence");
            vs_PermissionList.Columns.Add("EmpID");
            vs_PermissionList.Columns.Add("EmployeeName");
            vs_PermissionList.Columns.Add("DepartmentID");
            vs_PermissionList.Columns.Add("DepartmentName");
            vs_PermissionList.Columns.Add("SubDepartmentID");
            vs_PermissionList.Columns.Add("SubDepartmentName");
            vs_PermissionList.Columns.Add("PositionID");
            vs_PermissionList.Columns.Add("PositionName");

            vs_GroupMailList = new DataTable();
            vs_GroupMailList.Columns.Add("Sequence");
            vs_GroupMailList.Columns.Add("DepartmentID");
            vs_GroupMailList.Columns.Add("DepartmentName");
            vs_GroupMailList.Columns.Add("DepartmentGroupMail");

            vs_ApprovalList = new DataTable();
            vs_ApprovalList.Columns.Add("Sequence");
            vs_ApprovalList.Columns.Add("EmpID");
            vs_ApprovalList.Columns.Add("EmployeeName");
            vs_ApprovalList.Columns.Add("DepartmentID");
            vs_ApprovalList.Columns.Add("DepartmentName");
            vs_ApprovalList.Columns.Add("SubDepartmentID");
            vs_ApprovalList.Columns.Add("SubDepartmentName");
            vs_ApprovalList.Columns.Add("PositionID");
            vs_ApprovalList.Columns.Add("PositionName");

            vs_Assign = new DataTable();
            vs_Assign.Columns.Add("Sequence");
            vs_Assign.Columns.Add("EmpID");
            vs_Assign.Columns.Add("EmployeeName");
            vs_Assign.Columns.Add("DepartmentID");
            vs_Assign.Columns.Add("Email");
            vs_Assign.Columns.Add("Tel");

            DataTable dtAttachDoc = new DataTable();
            dtAttachDoc.Columns.Add("Sequence");
            dtAttachDoc.Columns.Add("AttachFile");
            dtAttachDoc.Columns.Add("FileName");
            dtAttachDoc.Columns.Add("ActorName");
            dtAttachDoc.Columns.Add("AttachDate");
            dtAttachDoc.Columns.Add("AttachFilePath");
            dtAttachDoc.Columns.Add("ActorID");
            dtAttachDoc.Columns.Add("DocSetName");
            dtAttachDoc.Columns.Add("DocLibName");
            dtAttachDoc.Columns.Add("IsPrimary");
            vs_attachDocTable = dtAttachDoc;

            DataTable dtAttachFile = new DataTable();
            dtAttachFile.Columns.Add("Sequence");
            dtAttachFile.Columns.Add("AttachFile");
            dtAttachFile.Columns.Add("FileName");
            dtAttachFile.Columns.Add("ActorName");
            dtAttachFile.Columns.Add("AttachDate");
            dtAttachFile.Columns.Add("AttachFilePath");
            dtAttachFile.Columns.Add("ActorID");
            dtAttachFile.Columns.Add("DocSetName");
            dtAttachFile.Columns.Add("DocLibName");
            dtAttachFile.Columns.Add("IsPrimary");
            vs_attachFileTable = dtAttachFile;
            vs_isAttach = true;


            gv_Assign.DataSource = new List<String>();
            gv_Assign.DataBind();

            gv_ToDepartment.DataSource = new List<string>();
            gv_ToDepartment.DataBind();
            #endregion
        }
        #endregion

        #region | Information |
        protected void chk_DOA_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_Type.SelectedValue != "Save")
            {
                tblAmount.Visible = chk_DOA.Checked;
                //div_approvalMatrix.Enabled = chk_DOA.Checked;
                DataTable dtDept = Extension.GetSpecificDepartmentData(this.Page, ddl_RequestorDepartment.SelectedValue);
                if (!dtDept.DataTableIsNullOrEmpty())
                {
                    lbl_CostCenter.Text = dtDept.Rows[0]["UL"].ToString();
                }
                if (!chk_DOA.Checked)
                {
                    //ddl_ApprovalMatrix.SelectedIndex = 0;
                    lbl_CostCenter.Text = "";
                }
            }
        }

        protected void ddl_DocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            chk_AutoStamp.Visible = false;
            chk_AutoStamp.Checked = false;
            chk_AutoStamp.Enabled = true;
            info_extend.Visible = false;
            info_OtherDoctype.Visible = false;
            div_ToDepartment.Visible = true;

            memo_extend.Visible = false;
            panel_Attachment.Visible = false;
            panel_RecieveDate.Visible = false;
            panel_RecieveDocNo.Visible = false;
            panel_Source.Visible = false;
            chk_isAttachWord.Visible = false;
            btn_DownloadTemplate.Visible = true;

            SetChkDocNoBySubDepartment();

            if (rdb_Type.SelectedValue != "Save")
            {
                panel_attachDoc.Visible = false;
            }
            if (vs_attachDocTable.Rows.Count > 0 && rdb_Type.SelectedValue != "Save")
            {
                vs_attachDocTable.Rows.RemoveAt(0);
                gv_AttachDocumentFile.DataSource = vs_attachDocTable;
                gv_AttachDocumentFile.DataBind();
                gv_AttachDocumentFile.Visible = false;
                vs_DocPDFPath = "";
            }

            if (ddl_DocType.SelectedValue.ToString() == "Ex" || ddl_DocType.SelectedValue.ToString() == "ExEN")
            {
                info_extend.Visible = true;
                div_ToDepartment.Visible = false;
                txt_ToDepartment.Text = "";
                hdn_ToDepartment.Text = "";
                panel_To.Visible = true;
                panel_Cc.Visible = true;
                panel_Attachment.Visible = true;
            }
            else if (ddl_DocType.SelectedValue.ToString() == "Im")
            {
                info_extend.Visible = true;
                panel_To.Visible = false;
                panel_Cc.Visible = false;
                panel_Attachment.Visible = false;
                panel_RecieveDate.Visible = true;
                txt_DocumentRecieve.Text = DateTime.Now.ToString("dd/MM/yyyy", Extension._ctliTH);

                panel_RecieveDocNo.Visible = true;
                panel_Source.Visible = true;
                panel_attachDoc.Visible = true;
                panel_attachFile.Visible = false;
                //panel_summerNote.Visible = false;
                //PanelInfoExtend.Visible = false;
                chk_AutoStamp.Visible = true;
                chk_AutoStamp.Checked = true;
                chk_AutoStamp.Enabled = false;

                chk_isAttachWord.Checked = false;
                chk_isAttachWord.Visible = false;
                btn_DownloadTemplate.Visible = false;

                //ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Disable Text Editing", " disableTextEditing();", true);
            }
            else if (ddl_DocType.SelectedValue.ToString().Equals("Other"))
            {
                info_OtherDoctype.Visible = true;
                ddl_otherDocType.SelectedIndex = 0;
                chk_AutoStamp.Visible = true;
                chk_AutoStamp.Checked = false;


            }
            else if (ddl_DocType.SelectedValue.ToString() == "M")
            {
                chk_AutoStamp.Visible = true;
                chk_AutoStamp.Checked = false;
                memo_extend.Visible = true;

            }
        }
        private void SetChkDocNoBySubDepartment()
        {
            //Available only M / Ex / ExEN
            string subID = ddl_RequestorSubDepartment.SelectedValue;
            DataTable subDept = Extension.GetSpecificDepartmentDataFromTemp(Page, subID);
            if (!subDept.DataTableIsNullOrEmpty() && subDept.Rows[0]["PRIMARY"].Equals("1")
                    && (ddl_DocType.SelectedValue.ToString() == "M" || ddl_DocType.SelectedValue.ToString() == "Ex" || ddl_DocType.SelectedValue.ToString() == "ExEN"))
            {
                chkDocNoBySubDepartment.Visible = true;
            }
            else
            {
                chkDocNoBySubDepartment.Visible = false;
                chkDocNoBySubDepartment.Checked = false;
            }
        }
        protected void rdb_Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            rcbCategoryChanged();

        }
        protected void rcbCategoryChanged()
        {
            if (rdb_Type.SelectedValue == "Save")
            {
                if (lbl_Status.Text == Extension._NewRequest || lbl_Status.Text == Extension._Draft)
                {
                    if (!(vs_isCentreAdmin || vs_isCentreAdminSarabun))
                    {
                        rdb_Category.SelectedIndex = -1;
                        Extension.MessageBox(this.Page, "ท่านไม่มีสิทธิ์เข้าถึงการจัดเก็บเอกสาร กรุณาติดต่อไปยังเจ้าหน้าที่ที่เกี่ยวข้อง");
                        return;
                    }
                    if (rdb_Category.SelectedValue == "centre")
                    {
                        if (!vs_isCentreAdminSarabun)
                        {
                            rdb_Category.SelectedIndex = -1;
                            Extension.MessageBox(this.Page, "ท่านไม่มีสิทธิ์เข้าถึงการจัดเก็บเอกสารส่วนกลาง กรุณาติดต่อไปยังเจ้าหน้าที่ที่เกี่ยวข้อง");
                            return;
                        }
                    }

                }
            }


            if (ddl_DocType.SelectedIndex > 0)
            {
                ListItem item = ddl_DocType.Items.FindByValue(ddl_DocType.SelectedValue);
                if (item != null)
                {
                    ddl_DocType.Items.FindByValue(ddl_DocType.SelectedValue).Selected = true;
                }
                else
                {
                    chk_AutoStamp.Visible = false;
                    chk_AutoStamp.Checked = false;
                }
            }
            string tempDDL = ddl_DocType.SelectedValue;
            if (rdb_Category.SelectedValue == "centre")
            {
                //Check isCentre
                DataTable dtDocumentType = Extension.GetDocTypeByCategory("isCentre", vs_isCentreAdmin);

                ddl_DocType.DataSource = dtDocumentType;
                ddl_DocType.DataTextField = vs_Lang == "TH" ? "DocTypeName" : "DocTypeNameEN";
                ddl_DocType.DataValueField = "Value";
                ddl_DocType.DataBind();
                ddl_DocType.Items.Insert(0, new ListItem("-- Please Select --", ""));

                ListItem item = ddl_DocType.Items.FindByValue(tempDDL);
                if (item != null)
                {
                    ddl_DocType.Items.FindByValue(tempDDL).Selected = true;
                }

                DataTable dtOtherDocType = SharedRules.GetList("MstDocumentType", "<Where><And><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq><And><Eq><FieldRef Name='Level' /><Value Type='Number'>1</Value></Eq><Eq><FieldRef Name='isCentre' /><Value Type='Boolean'>1</Value></Eq></And></And></Where>");
                if (!dtOtherDocType.DataTableIsNullOrEmpty())
                {
                    ddl_otherDocType.DataSource = dtOtherDocType;
                    ddl_otherDocType.DataTextField = vs_Lang == "TH" ? "DocTypeName" : "DocTypeNameEN";
                    ddl_otherDocType.DataValueField = "Value";
                    ddl_otherDocType.DataBind();
                }

                chk_InternalStamp.Visible = false;
                chk_InternalStamp.Checked = false;
            }
            else
            {
                //Check isInternal
                DataTable dtDocumentType = Extension.GetDocTypeByCategory("isInternal", vs_isCentreAdmin);

                ddl_DocType.DataSource = dtDocumentType;
                ddl_DocType.DataTextField = vs_Lang == "TH" ? "DocTypeName" : "DocTypeNameEN";
                ddl_DocType.DataValueField = "Value";
                ddl_DocType.DataBind();
                ddl_DocType.Items.Insert(0, new ListItem("-- Please Select --", ""));

                ListItem item = ddl_DocType.Items.FindByValue(tempDDL);
                if (item != null)
                {
                    ddl_DocType.Items.FindByValue(tempDDL).Selected = true;
                }

                DataTable dtOtherDocType = SharedRules.GetList("MstDocumentType", "<Where><And><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq><And><Eq><FieldRef Name='Level' /><Value Type='Number'>1</Value></Eq><Eq><FieldRef Name='isInternal' /><Value Type='Boolean'>1</Value></Eq></And></And></Where>");
                if (!dtOtherDocType.DataTableIsNullOrEmpty())
                {
                    ddl_otherDocType.DataSource = dtOtherDocType;
                    ddl_otherDocType.DataTextField = vs_Lang == "TH" ? "DocTypeName" : "DocTypeNameEN";
                    ddl_otherDocType.DataValueField = "Value";
                    ddl_otherDocType.DataBind();
                }

                chk_InternalStamp.Visible = true;
            }
        }
        protected void txt_deadline_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_deadline.Text))
            {
                try
                {
                    DateTime dtReqDateTo = DateTime.ParseExact(txt_deadline.Text, "MM/dd/yyyy", _ctli);
                    txt_deadline.Text = dtReqDateTo.ToString("dd/MM/yyyy", _ctli);
                }
                catch (Exception ex)
                {
                    txt_deadline.Text = "";
                }

            }
        }
        protected void txt_Amount_TextChanged(object sender, EventArgs e)
        {
            txt_Amount.Text = Extension.SetDecimalFormat(txt_Amount.Text);
        }

        protected void txt_RecieveDocNo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_RecieveDocNo.Text))
            {
                string sResult = CheckReceiveDocumentNoUsed(txt_RecieveDocNo.Text);
                if (!string.IsNullOrWhiteSpace(sResult))
                {
                    txt_RecieveDocNo.Text = "";
                    Extension.MessageBox(Page, sResult);
                }
            }
        }
        private string CheckReceiveDocumentNoUsed(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                int pk = 0;
                if (!string.IsNullOrWhiteSpace(vs_PK))
                {
                    pk = Convert.ToInt32(vs_PK);
                }
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(vs_ConnectionString);

                List<TRNDocument> listDocument = db.TRNDocuments
                    .Where(x => x.DocTypeCode == "Im" && x.RecieveDocumentNo == input
                                && (x.Status != Extension._Cancelled && x.Status != Extension._Rejected && x.Status != Extension._Draft)
                                && x.DocID != pk)
                    .ToList();
                if (listDocument != null && listDocument.Count > 0)
                {
                    return "This Recieve Document No was already uesd.";
                }
            }
            return "";
        }

        protected void lkbtnDeadline_Click(object sender, EventArgs e)
        {
            txt_deadline.Focus();
        }


        protected void chk_isAttachWord_CheckedChanged(object sender, EventArgs e)
        {
            chk_IsAttahcWordCheckedChanged();
        }
        private void chk_IsAttahcWordCheckedChanged()
        {
            bool isCheck = chk_isAttachWord.Checked;
            //btn_DownloadTemplate.Visible = isCheck;
            //panel_summerNote.Visible = !isCheck;
            if (isCheck)
            {

                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "disableTextEditing();", true);
            }
            else
            {
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "enableTextEditing();", true);
            }
        }

        protected void btn_addReferenceDocument_Click(object sender, EventArgs e)
        {
            OpenSearchDocPopup();
        }

        private void rdb_Type_BindingData(bool isAddAll = false)
        {
            List<KeyValuePair<string, string>> ListOfType = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("","--Please Select--"),
                new KeyValuePair<string, string>("Submit","ส่งเอกสารขออนุมัติ")
            };
            if (vs_isCentreAdmin || isAddAll)
            {
                ListOfType.Add(new KeyValuePair<string, string>("Save", "จัดเก็บเอกสาร"));
            }
            rdb_Type.DataSource = ListOfType;
            rdb_Type.DataTextField = "value";
            rdb_Type.DataValueField = "key";
            rdb_Type.DataBind();
        }
        protected void rdb_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdb_Type.SelectedValue == "Save")
            {
                if (!vs_isCentreAdmin)
                {
                    //rdb_Type.SelectedValue = "Submit";
                    rdb_Type.SelectedIndex = -1;
                    Extension.MessageBox(this.Page, "ท่านไม่มีสิทธิ์เข้าถึงการจัดเก็บเอกสาร กรุณาติดต่อไปยังเจ้าหน้าที่ที่เกี่ยวข้อง");
                    return;
                }
                if (rdb_Category.SelectedValue == "centre")
                {
                    if (!vs_isCentreAdminSarabun)
                    {
                        //rdb_Type.SelectedValue = "Submit";
                        rdb_Type.SelectedIndex = -1;
                        Extension.MessageBox(this.Page, "ท่านไม่มีสิทธิ์เข้าถึงการจัดเก็บเอกสารส่วนกลาง กรุณาติดต่อไปยังเจ้าหน้าที่ที่เกี่ยวข้อง");
                        return;
                    }
                }
                DivButtonForSave.Visible = true;
                DivButtonForSubmit.Visible = false;
                panel_Approval.Visible = false;
                PanelInfoExtend.Visible = false;
                divContent.Visible = false;
                tblAmount.Visible = false;
                panel_attachDoc.Visible = true;
                chk_AutoStamp.Visible = false;
                chk_AutoStamp.Checked = false;
            }
            else
            {
                DivButtonForSave.Visible = false;
                DivButtonForSubmit.Visible = true;
                panel_Approval.Visible = true;
                PanelInfoExtend.Visible = true;
                divContent.Visible = true;
                panel_attachDoc.Visible = false;
                if (chk_DOA.Checked)
                {
                    tblAmount.Visible = true;
                }
                if (ddl_DocType.SelectedValue == "M" || ddl_DocType.SelectedValue == "Im")
                {
                    chk_AutoStamp.Visible = true;
                }
            }
            //UserAuthorize();
        }

        protected void gv_ReferenceDocument_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                //e.Row.ToolTip = "Click for selecting this row.";
                //e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_ReferenceDocument, "Select$" + e.Row.RowIndex);

                Label lblReferenceDocumentNo = (Label)e.Row.FindControl("lblReferenceDocumentNo");
                object objDocNo = DataBinder.Eval(e.Row.DataItem, "DocNo");
                if (lblReferenceDocumentNo != null)
                    if (objDocNo != null && objDocNo != DBNull.Value) lblReferenceDocumentNo.Text = objDocNo.ToString();

                Label lblReferenceCategory = (Label)e.Row.FindControl("lblReferenceCategory");
                object objCategory = DataBinder.Eval(e.Row.DataItem, "Category");
                if (lblReferenceCategory != null)
                {
                    if (objCategory != null && objCategory != DBNull.Value)
                    {
                        string categoryName = string.Empty;
                        DataTable dtCategory = Extension.GetDataTable("MstCategory");
                        if (!dtCategory.DataTableIsNullOrEmpty())
                        {
                            DataTable oResult = dtCategory.AsEnumerable()
                                .Where(r => r.Field<String>("Value").Equals(objCategory.ToString()))
                                .ToList()
                                .CopyToDataTable();
                            categoryName = oResult.Rows[0]["CategoryName"].ToString();
                        }
                        lblReferenceCategory.Text = categoryName;
                    }

                }


                Label lblReferenceDocumentType = (Label)e.Row.FindControl("lblReferenceDocumentType");
                object objLDocType = DataBinder.Eval(e.Row.DataItem, "DocumentType");
                if (lblReferenceDocumentType != null)
                    if (objLDocType != null && objLDocType != DBNull.Value)
                    {
                        string docName = "";
                        DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
                        if (!dtDocType.DataTableIsNullOrEmpty())
                        {
                            DataTable oResult = dtDocType.AsEnumerable()
                                .Where(r => r.Field<String>("Value").Equals(objLDocType.ToString())).ToList().CopyToDataTable();
                            docName = oResult.Rows[0]["DocTypeName"].ToString();
                        }
                        //lblReferenceDocumentType.Text = objLDocType.ToString();
                        lblReferenceDocumentType.Text = docName;
                    }

            }
        }

        protected void gv_ReferenceDocument_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dtRefDoc = vs_DtRefDoc;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dtRefDoc != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    dtRefDoc.Rows[iRowIndex].Delete();
                    int iSequence = 1;
                    foreach (DataRow dr in dtRefDoc.Rows)
                    {
                        dr["Sequence"] = iSequence;
                        iSequence++;
                    }

                    btn_addReferenceDocument.Enabled = true;
                }
            }
            DataView dv = dtRefDoc.DefaultView;
            dv.Sort = "Sequence ASC";
            DataTable sortedDT = dv.ToTable();
            vs_DtRefDoc = sortedDT;

            gv_ReferenceDocument.DataSource = vs_DtRefDoc;
            gv_ReferenceDocument.DataBind();
        }
        protected void ddl_Category_SelectedIndexChanged1(object sender, EventArgs e)
        {

        }

        #region | Creator Department |
        private void CreatorDeptChange(SpecificEmployeeData.RootObject empData)
        {
            if (vs_isShowAllDept)
            {
                ddl_CreatorDepartment.DataSource = Extension.GetDepartmentData(Page).Copy();
            }
            else
            {
                ddl_CreatorDepartment.DataSource = empData.RESULT.Distinct();
            }
            ddl_CreatorDepartment.DataTextField = "DEPARTMENT_NAME_TH";
            ddl_CreatorDepartment.DataValueField = "DEPARTMENT_ID";
            ddl_CreatorDepartment.DataBind();
        }

        private void CreatorSubDeptChange(SpecificEmployeeData.RootObject empData)
        {
            if (vs_isShowAllDept)
            {
                ddl_CreatorSubDepartment.DataSource = Extension.GetDepartmentData(Page).Copy();
                ddl_CreatorSubDepartment.DataTextField = "DEPARTMENT_NAME_TH";//"SUBDEPARTMENT_NAME_TH";
                ddl_CreatorSubDepartment.DataValueField = "DEPARTMENT_ID";// "SUBDEPARTMENT_ID";
            }
            else
            {
                ddl_CreatorSubDepartment.DataSource = empData.RESULT.Where(x => x.DEPARTMENT_ID.Equals(ddl_CreatorDepartment.SelectedValue.ToString())).ToList().Distinct();
                ddl_CreatorSubDepartment.DataTextField = "SUBDEPARTMENT_NAME_TH";
                ddl_CreatorSubDepartment.DataValueField = "SUBDEPARTMENT_ID";
            }
            ddl_CreatorSubDepartment.DataBind();
        }

        private void CreatorPosChange(SpecificEmployeeData.RootObject empData)
        {
            if (vs_isShowAllDept)
            {
                ddl_CreatorPosition.DataSource = Extension.GetPositionData(Page).Copy();
                ddl_CreatorPosition.DataTextField = "POSITION_NAME_TH";
                ddl_CreatorPosition.DataValueField = "POSITION_ID";
            }
            else
            {
                ddl_CreatorPosition.DataSource = empData.RESULT.Where(x => x.DEPARTMENT_ID.Equals(ddl_CreatorDepartment.SelectedValue.ToString()) && x.SUBDEPARTMENT_ID.Equals(ddl_CreatorSubDepartment.SelectedValue.ToString())).ToList().Distinct();
                ddl_CreatorPosition.DataTextField = "POSTION_NAME_TH";
                ddl_CreatorPosition.DataValueField = "POSITION_TD";
            }
            ddl_CreatorPosition.DataBind();
        }

        protected void ddl_CreatorDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreatorSubDeptChange(Extension.GetSpecificEmployeeFromTemp(this.Page, vs_CreatorID));
        }

        protected void ddl_CreatorSubDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreatorPosChange(Extension.GetSpecificEmployeeFromTemp(this.Page, vs_CreatorID));
        }
        #endregion

        #region | Requestor Department |
        protected void chkDocNoBySubDepartment_CheckedChanged(object sender, EventArgs e)
        {
            vs_isOccurBySubDepartment = chkDocNoBySubDepartment.Checked;
        }
        private void RequestorDeptChange(SpecificEmployeeData.RootObject empData)
        {
            if (vs_isShowAllDept)
            {
                ddl_RequestorDepartment.DataSource = Extension.GetDepartmentData(Page);
            }
            else
            {
                ddl_RequestorDepartment.DataSource = empData.RESULT.Distinct();
            }
            ddl_RequestorDepartment.DataTextField = "DEPARTMENT_NAME_TH";
            ddl_RequestorDepartment.DataValueField = "DEPARTMENT_ID";
            ddl_RequestorDepartment.DataBind();


        }
        private void RequestorSubDeptChange(SpecificEmployeeData.RootObject empData)
        {
            if (vs_isShowAllDept)
            {
                ddl_RequestorSubDepartment.DataSource = Extension.GetDepartmentData(Page);
                ddl_RequestorSubDepartment.DataTextField = "DEPARTMENT_NAME_TH";//"SUBDEPARTMENT_NAME_TH";
                ddl_RequestorSubDepartment.DataValueField = "DEPARTMENT_ID";// "SUBDEPARTMENT_ID";
            }
            else
            {
                ddl_RequestorSubDepartment.DataSource = empData.RESULT.Where(x => x.DEPARTMENT_ID.Equals(ddl_RequestorDepartment.SelectedValue.ToString())).ToList();
                ddl_RequestorSubDepartment.DataTextField = "SUBDEPARTMENT_NAME_TH";
                ddl_RequestorSubDepartment.DataValueField = "SUBDEPARTMENT_ID";

            }
            ddl_RequestorSubDepartment.DataBind();
            SetChkDocNoBySubDepartment();
        }

        private void RequestorPosChange(SpecificEmployeeData.RootObject empData)
        {
            if (vs_isShowAllDept)
            {
                ddl_RequestorPosition.DataSource = Extension.GetPositionData(Page);
                ddl_RequestorPosition.DataTextField = "POSITION_NAME_TH";
                ddl_RequestorPosition.DataValueField = "POSITION_ID";
            }
            else
            {
                ddl_RequestorPosition.DataSource = empData.RESULT.Where(x => x.DEPARTMENT_ID.Equals(ddl_RequestorDepartment.SelectedValue.ToString()) && x.SUBDEPARTMENT_ID.Equals(ddl_RequestorSubDepartment.SelectedValue.ToString())).ToList();
                ddl_RequestorPosition.DataTextField = "POSTION_NAME_TH";
                ddl_RequestorPosition.DataValueField = "POSITION_TD";
            }
            ddl_RequestorPosition.DataBind();
        }

        protected void ddl_RequestorDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            RequestorSubDeptChange(Extension.GetSpecificEmployeeFromTemp(this.Page, lbl_RequestorID.Text));
            txt_FromDepartment.Text = ddl_RequestorDepartment.SelectedItem.ToString();
            hdn_FromDepartmentID.Text = ddl_RequestorDepartment.SelectedValue;
            if (chk_DOA.Checked)
            {
                DataTable dtDept = Extension.GetSpecificDepartmentData(this.Page, ddl_RequestorDepartment.SelectedValue);
                if (!dtDept.DataTableIsNullOrEmpty())
                {
                    lbl_CostCenter.Text = dtDept.Rows[0]["UL"].ToString();
                }
            }
        }

        protected void ddl_RequestorSubDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            RequestorPosChange(Extension.GetSpecificEmployeeFromTemp(this.Page, lbl_RequestorID.Text));
        }
        #endregion

        #region | Permission |
        protected void ddl_Permission_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_Permission.SelectedValue.Equals("Secret"))
            {
                div_permission.Visible = true;
                div_permissionPublic.Visible = false;
                //vs_GroupMailList = null;
                vs_isSecret = true;
            }
            else
            {
                div_permission.Visible = false;
                div_permissionPublic.Visible = true;
                //vs_PermissionList = null;
                vs_isSecret = false;
            }
        }
        protected void btn_addPermission_Click(object sender, EventArgs e)
        {

        }

        protected void gv_GroupEmail_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dt_GroupMail = vs_GroupMailList;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dt_GroupMail != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    dt_GroupMail.Rows[iRowIndex].Delete();
                    int iSequence = 1;
                    foreach (DataRow dr in dt_GroupMail.Rows)
                    {
                        dr["Sequence"] = iSequence;
                        iSequence++;
                    }
                }
            }
            DataView dv = dt_GroupMail.DefaultView;
            dv.Sort = "Sequence ASC";
            DataTable sortedDT = dv.ToTable();
            vs_GroupMailList = sortedDT;
            BinddtGroupMailViewStateToGridview();
        }
        #endregion

        #region | Action History |

        protected void gv_ActionHistory_RowDataBound(object sender, GridViewRowEventArgs e)
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
                        if (objActionBy != null && objActionBy != DBNull.Value)
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
                    if (lblPosition != null)
                    {
                        if (objPosition != null && objPosition != DBNull.Value)
                        {
                            if (objEmp != null)
                            {
                                SpecificEmployeeData.RESULT Dept = objEmp.RESULT.First(x => x.POSITION_TD == objPosition.ToString());
                                if (Dept == null) throw new ArgumentNullException("Dept");
                                lblPosition.Text = vs_Lang == "TH" ? Dept.POSTION_NAME_TH : Dept.POSTION_NAME_EN;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Extension.LogWriter.Write(ex);
                }

            }
        }
        #endregion

        #endregion

        #region | Button Action & Submit Data |
        protected void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Text == "Assign & Forward") { btnAssign_Click(sender, e); return; }

            string sResult = SubmitData(btn.Text);

            if (string.IsNullOrEmpty(sResult))
            {
                DivButtonForSubmit.Visible = false;
                Extension.MessageBox(this.Page, btn.Text + " Completed", "Worklist.aspx");
            }
            else
            {
                Extension.MessageBox(this.Page, sResult);
            }
        }
        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (vs_Status == "Completed")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "PopReqComment", "$('#searchAssign').modal();", true);
            }
            else
            {
                Extension.MessageBox(this.Page, "You have to submit first!");
            }
        }
        protected void btnRequestComment_Click(object sender, EventArgs e)
        {
            if (!(lbl_Status.Text.Equals(Extension._NewRequest) || lbl_Status.Text.Equals(Extension._Draft) || lbl_Status.Text.Equals(Extension._Rework)))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "PopReqComment", "$('#searchAdvisor').modal();", true);
            }
            else
            {
                Extension.MessageBox(this.Page, "You have to submit first!");
            }
        }

        protected void btnSubmitToAssign_Click(object sender, EventArgs e)
        {
            if (!vs_Assign.DataTableIsNullOrEmpty() && vs_Assign.Rows.Count > 0)
            {
                //DataClassesDataAccessDataContext DataContext = new DataClassesDataAccessDataContext(vs_ConnectionString);
                //DbTransaction dbTrabs = null;
                //if (DataContext.Connection.State == ConnectionState.Open)
                //{
                //    DataContext.Connection.Close();
                //    DataContext.Connection.Open();
                //}
                //else
                //{
                //    DataContext.Connection.Open();
                //    dbTrabs = DataContext.Connection.BeginTransaction();
                //    DataContext.Transaction = dbTrabs;
                //    try
                //    {
                //        List<TRNAssign> listAssign = new List<TRNAssign>();
                //        foreach (DataRow dr in vs_Assign.Rows)
                //        {
                //            TRNAssign objAssign = new TRNAssign();
                //            objAssign.DocID = Convert.ToInt32(vs_PK);
                //            objAssign.ActorID = Convert.ToInt32(vs_CurrentUserID);
                //            objAssign.AssignToID = Convert.ToInt32(dr["EmpID"].ToString());
                //            objAssign.ActionDate = DateTime.Now;

                //            listAssign.Add(objAssign);
                //        }
                //        DataContext.TRNAssigns.InsertAllOnSubmit(listAssign);
                //        DataContext.SubmitChanges();

                //        dbTrabs.Commit();

                //        Extension.MessageBox(this.Page, "Assign Completed.");
                //    }
                //    catch (Exception ex)
                //    {
                //        Extension.LogWriter.Write(ex);
                //        dbTrabs.Rollback();
                //    }
                //    finally
                //    {
                //        if (DataContext.Connection.State == System.Data.ConnectionState.Open)
                //        {
                //            DataContext.Connection.Close();
                //        }
                //    }
                //}

                string sResult = SubmitData("Assign & Forward");

                if (string.IsNullOrEmpty(sResult))
                {
                    Extension.MessageBox(this.Page, "Assign & Forward Completed", "Worklist.aspx");
                    //Extension.MessageBox(this.Page, "Submit Completed", "Default.aspx");
                }
                else
                {
                    Extension.MessageBox(this.Page, sResult);
                }
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "HideAssign", "$('#searchAssign').modal('hide');", true);
            }
            else
            {
                Extension.MessageBox(this.Page, "You have to add Employee first!");
            }
        }
        protected void lkbtnSearchAdvisor_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "HideReqComment", "$('#searchAdvisor').modal('hide');", true);
            //PopupSearchEmployeePicker.Is_SingleMode = true;
            //PopupSearchEmployeePicker.vs_sUserDefaultLang = vs_sUserDefaultLang;
            //PopupSearchEmployeePicker.vs_TargetControlTextbox = txt_RequestComment.ID;
            //PopupSearchEmployeePicker.vs_TargetControlEmpIDTextbox = txt_RequestCommentEMPID.ID;
            //PopupSearchEmployeePicker.OpenPopup();
            OpenPopup(sender, e);
        }
        protected void btnSubmitToRequestComment_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_RequestCommentEMPID.Text))
            {
                string sResult = SubmitData("Request Comment");
                if (string.IsNullOrEmpty(sResult))
                {
                    Extension.MessageBox(this.Page, "Request Comment Completed", "Worklist.aspx");
                }
                else
                {
                    Extension.MessageBox(this.Page, sResult);
                }
            }
            else
            {
                Extension.MessageBox(this.Page, "Please Select Advisor");
            }

        }
        protected void btnCancelRequestComment_Click(object sender, EventArgs e)
        {

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "HideReqComment", "$('#searchAdvisor').modal('hide');", true);

        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            string sScript = "window.parent.location = '" + "Worklist.aspx" + "'; ";
            ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "AlertMessage", sScript, true);
        }

        private string SubmitData(string sAction)
        {
            try
            {
                string sValidtionMsg = string.Empty;
                if (IsPassValidate(ref sValidtionMsg))
                {
                    DataClassesDataAccessDataContext DataContext = new DataClassesDataAccessDataContext(vs_ConnectionString);
                    if (DataContext.Connection.State == ConnectionState.Open)
                    {
                        DataContext.Connection.Close();
                        DataContext.Connection.Open();
                    }
                    else
                    {
                        DataContext.Connection.Open();
                        System.Data.Common.DbTransaction dbTrabs = DataContext.Connection.BeginTransaction();
                        DataContext.Transaction = dbTrabs;
                        try
                        {
                            DivButtonForSubmit.Visible = false;

                            #region | validate waitingFor |

                            if (!string.IsNullOrWhiteSpace(vs_PK) && lbl_Status.Text != "Completed")
                            {
                                TRNDocument ValidateDocument = DataContext.TRNDocuments.First(x => x.DocID == Convert.ToInt32(vs_PK));
                                if (vs_isDelegate)
                                {
                                    List<v_TRNDelegateDetail> listDelegate = new List<v_TRNDelegateDetail>();
                                    listDelegate = DataContext.v_TRNDelegateDetails
                                        .Where(x => x.DelegateToID == vs_CurrentUserID).ToList();
                                    listDelegate = listDelegate
                                        .Where(x => ValidateDocument.WaitingFor.Replace(" ", "").Split(',').Contains(x.ApproverID)).ToList();
                                    listDelegate = listDelegate
                                        .Where(x => x.DepartmentID == ValidateDocument.WaitingForDeptID).ToList();
                                    listDelegate = listDelegate
                                        .Where(x => Convert.ToBoolean(x.IsActive)).ToList();
                                    listDelegate = listDelegate.Where(x =>
                                    {
                                        bool i = Convert.ToBoolean(x.IsByDocType);
                                        return i ? x.DocType == ValidateDocument.DocTypeCode : x.DocID == vs_PK;
                                    }
                                        ).ToList();
                                    listDelegate = listDelegate
                                        .Where(x => (DateTime.Now >= x.DateFrom && (x.DateTo == null || DateTime.Now <= x.DateTo))).ToList();

                                    if (listDelegate.Count > 0)
                                    {
                                        ValidateDocument.WaitingFor = vs_CurrentUserID;
                                        ValidateDocument.WaitingForDeptID = vs_CurrentUserDepID;
                                    }
                                }
                                if (ValidateDocument != null)
                                {
                                    vs_WaitingFor = ValidateDocument.WaitingFor;
                                    if (vs_Role != "ITAdmin")
                                    {
                                        if (!vs_WaitingFor.Trim(' ').Split(',').Contains(vs_CurrentUserID) || lbl_Status.Text.Equals("Completed"))
                                        {
                                            Extension.MessageBox(Page, "เอกสารมีการดำเนินการไปแล้ว", string.Format("e-Document.aspx?pk={0}", vs_PK));
                                            return "";
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region | Update TRNDocument Data |
                            #region | Data |

                            TRNDocument objDocument = new TRNDocument();
                            if (!string.IsNullOrEmpty(vs_PK))
                            {
                                objDocument = DataContext.TRNDocuments.SingleOrDefault(x => x.DocID.ToString().Equals(vs_PK));
                                Debug.Assert(objDocument != null, "objDocument != null");
                                vs_Status = objDocument.Status.ToString();
                            }
                            else
                            {
                                objDocument.DocLib = "TempDocument";
                            }
                            string docType = ddl_DocType.SelectedValue;
                            if (vs_isDocSetCreated)
                            {
                                objDocument.DocSet = lbl_docSet.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(vs_DocWordPath))
                            {
                                objDocument.AttachWordPath = vs_DocWordPath;
                            }
                            if (docType == "Other")
                            {
                                objDocument.OtherDocType = ddl_otherDocType.SelectedValue;
                            }
                            else if (docType == "Im")
                            {
                                objDocument.To = txt_to.Text;
                                objDocument.CC = txt_CC.Text;
                                objDocument.RecieveDocumentNo = txt_RecieveDocNo.Text;
                                objDocument.DocumentSource = txt_Source.Text;
                                if (!string.IsNullOrWhiteSpace(txt_DocumentDate.Text))
                                {
                                    objDocument.DocumentDate = DateTime.Parse(txt_DocumentDate.Text, Extension._ctliEN);
                                    if (objDocument.DocumentDate.Value.Year > 2500)
                                    {
                                        objDocument.DocumentDate = DateTime.Parse(txt_DocumentDate.Text, Extension._ctliTH);
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(txt_DocumentRecieve.Text))
                                {
                                    objDocument.DocumentRecieveDate = DateTime.Parse(txt_DocumentRecieve.Text, Extension._ctliEN);
                                    if (objDocument.DocumentRecieveDate.Value.Year > 2500)
                                    {
                                        objDocument.DocumentRecieveDate = DateTime.Parse(txt_DocumentRecieve.Text, Extension._ctliTH);
                                    }
                                }
                                objDocument.AttachFilePath = vs_DocPDFPath;
                            }
                            else if (docType == "M")
                            {
                                if (!string.IsNullOrWhiteSpace(lbl_sendToID.Text))
                                {
                                    objDocument.SendToID = lbl_sendToID.Text;
                                }
                            }
                            if (lbl_DocumentNo.Text == "Auto Generate")
                            {
                                objDocument.DocNo = lbl_DocumentNo.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(txt_DocDescription.Text) && txt_DocDescription.Text.Count() > 500)
                            {
                                throw new Exception("รายละเอียดของเอกสาร(โดยย่อ) ยาวเกินที่กำหนด");
                            }
                            objDocument.Description = txt_DocDescription.Text;
                            objDocument.DocTypeCode = ddl_DocType.SelectedValue;
                            if (!string.IsNullOrEmpty(lbl_CreatorID.Text.Trim()))
                            {
                                objDocument.CreatorID = lbl_CreatorID.Text.Trim();
                            }
                            else
                            {
                                objDocument.CreatorID = "";
                            }

                            if (!string.IsNullOrEmpty(ddl_CreatorDepartment.SelectedValue))
                            {
                                objDocument.CreatorDepartmentID = Convert.ToInt32(ddl_CreatorDepartment.SelectedValue);
                            }
                            else
                            {
                                objDocument.CreatorDepartmentID = 0;
                            }

                            if (!string.IsNullOrEmpty(ddl_CreatorSubDepartment.SelectedValue))
                            {
                                objDocument.CreatorSubDepartmentID = Convert.ToInt32(ddl_CreatorSubDepartment.SelectedValue);
                            }
                            else
                            {
                                objDocument.CreatorSubDepartmentID = 0;
                            }

                            objDocument.CreatorPositionID = Convert.ToInt32(ddl_CreatorPosition.SelectedValue);
                            objDocument.RequestorID = lbl_RequestorID.Text.Trim();
                            objDocument.RequestorDepartmentID = Convert.ToInt32(ddl_RequestorDepartment.SelectedValue);
                            objDocument.RequestorSubDepartmentID = Convert.ToInt32(ddl_RequestorSubDepartment.SelectedValue);
                            objDocument.RequestorPositionID = Convert.ToInt32(ddl_RequestorPosition.SelectedValue);
                            objDocument.Type = rdb_Type.SelectedValue.ToString();
                            objDocument.Category = rdb_Category.SelectedValue.ToString();
                            objDocument.To = txt_to.Text;
                            objDocument.CC = txt_CC.Text;
                            objDocument.Attachment = txt_Attachment.Text;
                            objDocument.Title = txt_title.Text;
                            objDocument.SubTitle = txt_subtitle.Text;
                            objDocument.FromDepartmentID = Convert.ToInt32(hdn_FromDepartmentID.Text);
                            objDocument.FromDepartmentName = txt_FromDepartment.Text;
                            objDocument.ToDepartmentID = hdn_ToDepartment.Text;
                            objDocument.ToDepartmentName = txt_ToDepartment.Text;
                            objDocument.Priority = ddl_Priority.SelectedValue;
                            objDocument.ModifiedBy = vs_CurrentUserID;
                            objDocument.ModifiedDate = DateTime.Now;
                            if (!string.IsNullOrWhiteSpace(txt_deadline.Text))
                            {
                                objDocument.Deadline = DateTime.Parse(txt_deadline.Text, Extension._ctliEN);
                                if (objDocument.Deadline.Value.Year > 2500)
                                {
                                    objDocument.Deadline = DateTime.Parse(txt_deadline.Text, Extension._ctliTH);
                                }
                            }
                            objDocument.DOA = chk_DOA.Checked ? "Y" : "N";
                            objDocument.AutoStamp = chk_AutoStamp.Checked ? "Y" : "N";
                            objDocument.InternalOnlyStamp = chk_InternalStamp.Checked;
                            objDocument.IsOccurBySubDepartment = chkDocNoBySubDepartment.Checked;
                            if (vs_isLastApproval && sAction == "Approve")
                            {
                                objDocument.ApproveDate = DateTime.Now;
                            }

                            if (chk_DOA.Checked)
                            {
                                if (objDocument.Type.ToLower().Equals("submit"))
                                {
                                    objDocument.CostCenter = lbl_CostCenter.Text;
                                    objDocument.Amount = Convert.ToDecimal(txt_Amount.Text);
                                    //objDocument.ApprovalMatrix = ddl_ApprovalMatrix.SelectedValue;
                                }
                                else
                                {
                                    objDocument.CostCenter = "";
                                    objDocument.Amount = 0;
                                    //objDocument.ApprovalMatrix = "";
                                }
                            }
                            objDocument.PermissionType = ddl_Permission.SelectedValue;
                            objDocument.Comment = txt_AdditionalComment.Text;

                            #endregion
                            #region | Workflow |
                            int currentApprovalLevel = 1;

                            if (sAction == "Approve" || sAction == "Submit" || sAction == "Reply")
                            {
                                if (sAction == "Approve")
                                {
                                    if (objDocument.Status.Equals(Extension._RequestCancel))
                                    {
                                        vs_Status = objDocument.Status;
                                    }
                                    else if (!objDocument.Status.Equals(Extension._WaitForRequestorReview))
                                    {
                                        currentApprovalLevel = Convert.ToInt32(objDocument.CurrentApprovalLevel) + 1;
                                    }
                                }
                                else if (sAction == "Submit")
                                {
                                    objDocument.CreatedDate = DateTime.Now;
                                    objDocument.CurrentApprovalLevel = currentApprovalLevel;

                                    //if creator != Requester -> sent to Requester
                                    if ((!lbl_CreatorID.Text.Equals(lbl_RequestorID.Text))
                                        && (vs_Status.Equals(Extension._NewRequest) || vs_Status.Equals(Extension._Draft) || vs_Status.Equals(Extension._Rework)))
                                    {
                                        objDocument.Status = Extension._WaitForRequestorReview;
                                        objDocument.WaitingFor = objDocument.RequestorID.ToString();
                                        objDocument.WaitingForDeptID = objDocument.RequestorDepartmentID.ToString();
                                        vs_Status = "";
                                    }
                                    else if ((lbl_CreatorID.Text.Equals(lbl_RequestorID.Text))
                                        && (vs_Status.Equals(Extension._NewRequest) || vs_Status.Equals(Extension._Draft) || vs_Status.Equals(Extension._WaitForRequestorReview)))
                                    {
                                        objDocument.Status = "Wait for Approve";
                                    }
                                }
                                else if (sAction == "Reply")
                                {
                                    if (string.IsNullOrWhiteSpace(txt_AdditionalComment.Text))
                                    {
                                        //Extension.MessageBox(this.Page, "");
                                        txt_AdditionalComment.Focus();
                                        return "กรุณาแสดงความคิดเห็น";
                                    }
                                    currentApprovalLevel = Convert.ToInt32(objDocument.CurrentApprovalLevel);
                                }

                                DataTable dt = vs_ApprovalList;
                                DataRow[] dr = null;
                                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                string msgCheck = "";

                                if (dt.Rows.Count == 0)
                                {
                                    //Upload E-doc into SP

                                    if (vs_attachDocTable.DataTableIsNullOrEmpty())
                                    {
                                        if (!(lbl_Status.Text.Equals(Extension._WaitForRequestorReview) && vs_Status == ""))
                                        {
                                            objDocument.Status = Extension._Draft;
                                            objDocument.CreatedDate = DateTime.Now;
                                            objDocument.WaitingFor = vs_CurrentUserID;
                                            objDocument.WaitingForDeptID = hdn_FromDepartmentID.Text;
                                            vs_Status = "";
                                        }
                                    }
                                    else if (objDocument.Status.Equals(Extension._WaitForRequestorReview) && vs_CurrentUserID != objDocument.RequestorID)
                                    {
                                        //keep status 'Waiting For Requester Review'
                                    }
                                    else
                                    {
                                        if (ddl_RequestorDepartment.SelectedValue == "161" && ddl_DocType.SelectedValue == "M")
                                        {
                                            msgCheck += string.Format("เข้าเคส Else แล้ว Complete <br/> vs_attachDocTable : {0}", serializer.Serialize(vs_attachDocTable));
                                        }
                                        objDocument.Status = Extension._Completed;
                                        objDocument.CreatedDate = DateTime.Now;
                                        objDocument.ApproveDate = DateTime.Now;
                                        objDocument.WaitingFor = "";
                                        objDocument.WaitingForDeptID = "";
                                        objDocument.DocLib = "Document_Library";
                                        vs_Status = "";
                                    }
                                }
                                else
                                {
                                    dr = dt.Select(string.Format("Sequence = {0} ", currentApprovalLevel));
                                }

                                #region มีผู้อนุมัติถัดไป if have Approval list -> Go to next Approval
                                if (dr != null && dr.Length > 0)
                                {
                                    objDocument.CurrentApprovalLevel = currentApprovalLevel;

                                    if ((vs_Status.Equals("") && objDocument.Status.Equals(Extension._WaitForRequestorReview)) && currentApprovalLevel == 1)
                                    {
                                        vs_Status = objDocument.Status;
                                    }
                                    else
                                    {
                                        objDocument.Status = "Wait for Approve";
                                        objDocument.WaitingFor = dr[0]["EmpID"].ToString();
                                        objDocument.WaitingForDeptID = dr[0]["DepartmentID"].ToString();
                                        vs_Status = objDocument.Status;
                                    }
                                }
                                #endregion
                                #region else Document to approved -> sent to AdminCentre or Completed
                                else
                                {
                                    if (vs_Status.Equals(Extension._RequestCancel))
                                    {
                                        objDocument.WaitingFor = "";
                                        objDocument.WaitingForDeptID = "";
                                        objDocument.Status = Extension._Cancelled;

                                        TRNEDocumentQueue eDocQ = DataContext.TRNEDocumentQueues.SingleOrDefault(x => x.DocID == objDocument.DocID);
                                        if (eDocQ != null)
                                        {
                                            eDocQ.IsActive = true;
                                            DataContext.SubmitChanges();
                                        }
                                    }
                                    else if (vs_Status != "" && rdb_Type.SelectedValue != "Save")
                                    {
                                        //  ต้องไม่เท่ากับ หนังสือรับจากภายนอก (Import Letter) 
                                        if (ddl_DocType.SelectedValue != "Im") //ddl_DocType.SelectedValue != "M" && 
                                        {
                                            DataTable dtAdmin = Extension.GetDataTable("MstAdminCentre");
                                            string waitingFor = string.Empty;
                                            string waitingForDeptID = string.Empty;

                                            if (!dtAdmin.DataTableIsNullOrEmpty())
                                            {
                                                if (ddl_RequestorDepartment.SelectedValue == "161" && ddl_DocType.SelectedValue == "M")
                                                {
                                                    msgCheck += string.Format("<br/> dtAdmin != null : {0}", serializer.Serialize(dtAdmin));
                                                }
                                                DataTable oResult = new DataTable();
                                                try
                                                {
                                                    if (rdb_Category.SelectedValue == "centre")
                                                    {
                                                        oResult = dtAdmin.AsEnumerable().Where(r => r.Field<String>("DeptID").Equals("10")).ToList().CopyToDataTable();
                                                    }
                                                    else
                                                    {
                                                        string depcheck = vs_isOccurBySubDepartment ? objDocument.RequestorSubDepartmentID.ToString() : objDocument.RequestorDepartmentID.ToString();
                                                        oResult = dtAdmin.AsEnumerable()
                                                              .Where(r => r.Field<String>("DeptID").Equals(depcheck)).ToList().CopyToDataTable();
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    throw new Exception("AdminCentre Not Found.");
                                                }
                                                foreach (DataRow row in oResult.Rows)
                                                {
                                                    //if (string.IsNullOrEmpty(waitingFor))
                                                    //{
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
                                                }
                                            }

                                            objDocument.WaitingFor = waitingFor;
                                            objDocument.WaitingForDeptID = waitingForDeptID;
                                            objDocument.DocLib = "Document_Library";
                                            objDocument.Status = Extension._WaitForAdminCentre;
                                        }
                                        else
                                        {
                                            objDocument.WaitingFor = "";
                                            objDocument.WaitingForDeptID = "";
                                            objDocument.Status = Extension._Completed;
                                            objDocument.DocLib = "Document_Library";
                                        }
                                    }
                                }
                                #endregion
                                vs_Status = objDocument.Status;
                                if (!string.IsNullOrWhiteSpace(msgCheck))
                                { 
                                    Extension.LogWriter.Write(msgCheck);
                                }
                            }
                            else if (sAction == "Confirm")
                            {
                                objDocument.WaitingFor = "";
                                objDocument.WaitingForDeptID = "";
                                objDocument.Status = Extension._Completed;
                                vs_Status = objDocument.Status;

                            }
                            else if (sAction == "Save Draft")
                            {
                                objDocument.Status = Extension._Draft;
                                vs_Status = Extension._Draft;
                                objDocument.CreatedDate = DateTime.Now;

                                objDocument.WaitingFor = objDocument.CreatorID.ToString();
                                objDocument.WaitingForDeptID = objDocument.CreatorDepartmentID.ToString();
                            }
                            else if (sAction == "Rework")
                            {
                                objDocument.Status = Extension._Rework;
                                vs_Status = Extension._Rework;

                                objDocument.WaitingFor = objDocument.CreatorID.ToString();
                                objDocument.WaitingForDeptID = objDocument.CreatorDepartmentID.ToString();
                            }
                            else if (sAction == "Cancel")
                            {
                                if (objDocument.Status.Equals(Extension._Completed))
                                {
                                    if (string.IsNullOrWhiteSpace(txt_AdditionalComment.Text))
                                    {
                                        txt_AdditionalComment.Focus();
                                        return "กรุณาแสดงความคิดเห็น";
                                    }
                                    TRNEDocumentQueue eDocQ = DataContext.TRNEDocumentQueues.SingleOrDefault(x => x.DocID == objDocument.DocID);
                                    if (eDocQ != null)
                                    {
                                        eDocQ.IsActive = true;
                                        DataContext.SubmitChanges();
                                    }
                                }
                                objDocument.Status = "Cancelled";
                                vs_Status = "Cancelled";

                                objDocument.WaitingFor = "";
                                objDocument.WaitingForDeptID = "";
                            }
                            else if (sAction == "Reject")
                            {
                                if (vs_Status.Equals(Extension._RequestCancel))
                                {

                                    objDocument.Status = Extension._Completed;
                                    vs_Status = Extension._Completed;

                                    objDocument.WaitingFor = "";
                                    objDocument.WaitingForDeptID = "";
                                }
                                else
                                {
                                    objDocument.Status = "Rejected";
                                    vs_Status = "Rejected";

                                    objDocument.WaitingFor = "";
                                    objDocument.WaitingForDeptID = "";
                                }
                            }
                            else if (sAction == "Request Comment")
                            {
                                objDocument.Status = Extension._WaitForComment;
                                vs_Status = Extension._WaitForComment;
                                if (!string.IsNullOrWhiteSpace(txt_RequestCommentEMPID.Text))
                                {
                                    objDocument.WaitingFor = txt_RequestCommentEMPID.Text;
                                    objDocument.WaitingForDeptID = txt_RequestCommentDEPID.Text;
                                }
                            }
                            else if (sAction == "Request Cancel")
                            {
                                objDocument.Status = Extension._RequestCancel;
                                vs_Status = Extension._RequestCancel;
                                SpecificEmployeeData.RootObject req = Extension.GetSpecificEmployeeFromTemp(Page, lbl_RequestorID.Text);
                                objDocument.WaitingFor = string.IsNullOrWhiteSpace(req.MANAGERID) ? "9000007" : req.MANAGERID;
                                objDocument.WaitingForDeptID = string.IsNullOrWhiteSpace(req.MANAGERID) ? "1" : (objDocument.RequestorDepartmentID ?? 0).ToString();
                            }
                            #endregion

                            if (string.IsNullOrEmpty(vs_PK))
                            {
                                DataContext.TRNDocuments.InsertOnSubmit(objDocument);
                                DataContext.SubmitChanges();
                            }
                            #endregion

                            #region | Update/Insert Assign |
                            // Update or Insert Assign
                            if (!vs_Assign.DataTableIsNullOrEmpty() && vs_Assign.Rows.Count > 0)
                            {
                                List<TRNAssign> listAssign = new List<TRNAssign>();
                                foreach (DataRow dr in vs_Assign.Rows)
                                {
                                    TRNAssign objAssign = new TRNAssign();
                                    objAssign.DocID = Convert.ToInt32(vs_PK);
                                    objAssign.ActorID = vs_CurrentUserID;
                                    objAssign.AssignToDeptID = dr["DepartmentID"].ToString();
                                    objAssign.AssignToID = dr["EmpID"].ToString();
                                    objAssign.ActionDate = DateTime.Now;
                                    objAssign.Acknowledge = false;
                                    objAssign.Comment = txt_AdditionalComment.Text;

                                    listAssign.Add(objAssign);
                                }
                                DataContext.TRNAssigns.InsertAllOnSubmit(listAssign);
                                DataContext.SubmitChanges();
                            }
                            #endregion

                            #region | Update/Insert History |
                            //Update History
                            TRNHistory objActionHistory = new TRNHistory();
                            objActionHistory.DocID = objDocument.DocID;
                            objActionHistory.EmpID = vs_CurrentUserID;
                            objActionHistory.ActionName = sAction;
                            objActionHistory.ActionDate = DateTime.Now;

                            string oDelegateText = "[อนุมัติงานแทน]";
                            string oComment = vs_isDelegate ? string.Format("{0} {1}", oDelegateText, txt_AdditionalComment.Text) : txt_AdditionalComment.Text;
                            objActionHistory.Comment = oComment;

                            objActionHistory.StatusBefore = lbl_Status.Text;
                            //objActionHistory.SignatureWording = signatureID;
                            DataContext.TRNHistories.InsertOnSubmit(objActionHistory);
                            DataContext.SubmitChanges();
                            #endregion

                            #region | Update/Insert Reference Document |
                            //update or insert Reference Document
                            if (!string.IsNullOrEmpty(vs_PK))
                            {
                                List<TRNReferenceDoc> objListTRNRefDoc = new List<TRNReferenceDoc>();
                                objListTRNRefDoc = DataContext.TRNReferenceDocs.ToList();
                                IEnumerable<TRNReferenceDoc> queryRD = (from TRNReferenceDoc refDoc in objListTRNRefDoc
                                                                        where refDoc.DocID == Convert.ToInt32(vs_PK)
                                                                        select refDoc);
                                DataContext.TRNReferenceDocs.DeleteAllOnSubmit(queryRD);
                                DataContext.SubmitChanges();
                            }
                            DataTable dtRefDoc = vs_DtRefDoc;

                            if (dtRefDoc != null)
                            {
                                List<TRNReferenceDoc> listRefDocument = new List<TRNReferenceDoc>();
                                foreach (DataRow dr in vs_DtRefDoc.Rows)
                                {
                                    TRNReferenceDoc objRefDocument = new TRNReferenceDoc();
                                    objRefDocument.DocID = objDocument.DocID;
                                    objRefDocument.DocNo = dr["DocNo"].ToString();
                                    objRefDocument.Title = dr["Title"].ToString();
                                    objRefDocument.DocumentType = dr["DocumentType"].ToString();
                                    objRefDocument.Category = dr["Category"].ToString();
                                    objRefDocument.RefDocID = Convert.ToInt32(dr["RefDocID"].ToString());

                                    listRefDocument.Add(objRefDocument);
                                }
                                DataContext.TRNReferenceDocs.InsertAllOnSubmit(listRefDocument);
                                DataContext.SubmitChanges();


                            }
                            #endregion

                            #region | Update/Insert Specific Permission |
                            //Update or Insert Specific Permission
                            if (!string.IsNullOrEmpty(vs_PK))
                            {
                                List<TRNPermission> objListTRNPermission = new List<TRNPermission>();
                                objListTRNPermission = DataContext.TRNPermissions.ToList();
                                IEnumerable<TRNPermission> queryPm = (from TRNPermission permission in objListTRNPermission
                                                                      where permission.DocID == Convert.ToInt32(vs_PK)
                                                                      select permission);
                                DataContext.TRNPermissions.DeleteAllOnSubmit(queryPm);
                                DataContext.SubmitChanges();
                            }
                            DataTable dtPermission = vs_PermissionList;
                            if (dtPermission != null)
                            {
                                if (ddl_Permission.SelectedValue.Equals("Secret"))
                                {
                                    List<TRNPermission> listPermission = new List<TRNPermission>();
                                    foreach (DataRow dr in vs_PermissionList.Rows)
                                    {
                                        TRNPermission objPermission = new TRNPermission();
                                        objPermission.DocID = objDocument.DocID;
                                        objPermission.EmpID = dr["EmpID"].ToString();
                                        objPermission.EmployeeName = dr["EmployeeName"].ToString();
                                        objPermission.DepartmentID = Convert.ToInt32(dr["DepartmentID"].ToString());
                                        objPermission.DepartmentName = dr["DepartmentName"].ToString();
                                        objPermission.SubDepartmentID = Convert.ToInt32(dr["SubDepartmentID"].ToString());
                                        objPermission.SubDepartmentName = dr["SubDepartmentName"].ToString();
                                        objPermission.PositionID = Convert.ToInt32(dr["PositionID"].ToString());
                                        objPermission.PositionName = dr["PositionName"].ToString();
                                        listPermission.Add(objPermission);
                                    }
                                    DataContext.TRNPermissions.InsertAllOnSubmit(listPermission);
                                    DataContext.SubmitChanges();
                                }

                            }
                            #endregion

                            #region | Update/Insert GroupMail |
                            //Update or Insert Specific Permission
                            if (!string.IsNullOrEmpty(vs_PK))
                            {
                                List<TRNGroupMail> objListTRNGroupMail = new List<TRNGroupMail>();
                                objListTRNGroupMail = DataContext.TRNGroupMails.ToList();
                                IEnumerable<TRNGroupMail> queryPm = (from TRNGroupMail groupMail in objListTRNGroupMail
                                                                     where groupMail.DocID == vs_PK
                                                                     select groupMail);
                                DataContext.TRNGroupMails.DeleteAllOnSubmit(queryPm);
                                DataContext.SubmitChanges();
                            }
                            if (vs_GroupMailList != null)
                            {
                                if (ddl_Permission.SelectedValue.Equals("Public"))
                                {
                                    List<TRNGroupMail> listPermission = new List<TRNGroupMail>();
                                    foreach (DataRow dr in vs_GroupMailList.Rows)
                                    {
                                        TRNGroupMail objPermission = new TRNGroupMail();
                                        objPermission.DocID = objDocument.DocID.ToString();
                                        objPermission.DepartmentID = dr["DepartmentID"].ToString();
                                        objPermission.DepartmentName = dr["DepartmentName"].ToString();
                                        objPermission.DepartmentGroupMail = dr["DepartmentGroupMail"].ToString();
                                        listPermission.Add(objPermission);
                                    }
                                    DataContext.TRNGroupMails.InsertAllOnSubmit(listPermission);
                                    DataContext.SubmitChanges();
                                }

                            }
                            #endregion

                            #region | Update/Insert Specific Approval |
                            //Update or Insert Specific Approval
                            if (!string.IsNullOrEmpty(vs_PK))
                            {
                                List<TRNApprover> objListTRNApprovor = new List<TRNApprover>();
                                objListTRNApprovor = DataContext.TRNApprovers.ToList();
                                IEnumerable<TRNApprover> queryApp = (from TRNApprover app in objListTRNApprovor
                                                                     where app.DocID == Convert.ToInt32(vs_PK)
                                                                     select app);
                                DataContext.TRNApprovers.DeleteAllOnSubmit(queryApp);
                                DataContext.SubmitChanges();
                            }
                            string signatureID = "";
                            DataTable dtApprroval = vs_ApprovalList;
                            if (dtApprroval != null)
                            {
                                if (rdb_Type.SelectedValue.Equals("Submit"))
                                {
                                    List<TRNApprover> listApprover = new List<TRNApprover>();
                                    foreach (DataRow dr in vs_ApprovalList.Rows)
                                    {
                                        TRNApprover objApprover = new TRNApprover();
                                        objApprover.DocID = objDocument.DocID;
                                        objApprover.EmpID = dr["EmpID"].ToString();
                                        objApprover.EmployeeName = dr["EmployeeName"].ToString();
                                        objApprover.DepartmentID = Convert.ToInt32(dr["DepartmentID"].ToString());
                                        objApprover.DepartmentName = dr["DepartmentName"].ToString();
                                        objApprover.SubDepartmentID = Convert.ToInt32(dr["SubDepartmentID"].ToString());
                                        objApprover.SubDepartmentName = dr["SubDepartmentName"].ToString();
                                        objApprover.PositionID = Convert.ToInt32(dr["PositionID"].ToString());
                                        objApprover.PositionName = dr["PositionName"].ToString();
                                        listApprover.Add(objApprover);
                                    }
                                    DataContext.TRNApprovers.InsertAllOnSubmit(listApprover);
                                    DataContext.SubmitChanges();
                                }
                            }
                            #endregion


                            if (sAction != "Confirm" && sAction != "Assign & Forward" && sAction != "Update" && !(sAction == "Cancel" && vs_Status == "Cancelled"))
                            {
                                #region | Generate Document No |
                                //Gen DocNo + update objDocument DocNo + Update objRunningNo DocNo
                                string runningNo = lbl_DocumentNo.Text;
                                string newRunningNo = "0001";
                                string projectYear = (DateTime.Now.Year).ToString();
                                projectYear = ddl_DocType.SelectedValue == "ExEN"
                                        ? projectYear.ConvertToAD()
                                        : projectYear.ConvertToBE();
                                if ((objDocument.Status.Equals(Extension._WaitForAdminCentre) && runningNo == "Auto Generate") ||
                                    (objDocument.Status.Equals(Extension._Completed) && runningNo == "Auto Generate") ||
                                    (runningNo == "Auto Generate" && rdb_Type.SelectedValue == "Save") ||
                                    (ddl_DocType.SelectedValue.Equals("Im") && objDocument.Status.Equals(Extension._WaitForApprove) && runningNo == "Auto Generate"))
                                {
                                    //generate RunningNo
                                    List<ControlRunning> listControlRunning = new List<ControlRunning>();
                                    listControlRunning = DataContext.ControlRunnings.ToList();

                                    int sDepID = Convert.ToInt32(vs_isOccurBySubDepartment ? ddl_RequestorSubDepartment.SelectedValue : ddl_RequestorDepartment.SelectedValue);


                                    listControlRunning = listControlRunning.Where(x => x.DocType == ddl_DocType.SelectedValue.ToString()
                                                                                    && x.CreatedYear == projectYear
                                                                                    && x.DepID == (rdb_Category.SelectedValue == "centre" ? 0 : sDepID)
                                                                                    && x.RunningNo >= 1)
                                                                                    .OrderByDescending(x => x.RunningNo)
                                                                                    .ToList();
                                    if (listControlRunning.Count > 0)
                                    {
                                        foreach (ControlRunning cr in listControlRunning)
                                        {
                                            newRunningNo = cr.RunningNo > 9999
                                                ? cr.RunningNo.ToString()
                                                : string.Format("{0:D4}", (cr.RunningNo + 1));
                                            cr.RunningNo += 1;
                                            objDocument.DocNo = string.Format("{0}/{1}", newRunningNo, projectYear);
                                            lbl_DocumentNo.Text = objDocument.DocNo;
                                            DataContext.SubmitChanges();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        //Insert data to MSTControlRunning
                                        ControlRunning objControlRunning = new ControlRunning();
                                        objControlRunning.DocType = ddl_DocType.SelectedValue.ToString();
                                        objControlRunning.CreatedYear = projectYear;
                                        objControlRunning.DepID = rdb_Category.SelectedValue == "internal" ? sDepID : 0;
                                        objControlRunning.RunningNo = Convert.ToInt32(newRunningNo);

                                        objDocument.DocNo = string.Format("{0}/{1}", newRunningNo, projectYear);

                                        lbl_DocumentNo.Text = Extension.GenerateDocumentNo(newRunningNo, (int)objControlRunning.DepID, rdb_Category.SelectedValue, ddl_DocType.SelectedValue, Page);
                                        //lbl_DocumentNo.Text = objDocument.DocNo;
                                        objControlRunning.DocID = objDocument.DocID;

                                        DataContext.ControlRunnings.InsertOnSubmit(objControlRunning);
                                        DataContext.SubmitChanges();
                                    }
                                    lbl_DocumentNo.Text = objDocument.DocNo.ToString();
                                }
                                else
                                {
                                    newRunningNo = lbl_DocumentNo.Text;
                                    DataContext.SubmitChanges();
                                }
                                #endregion
                                #region | Update MSWord |
                                if (objDocument.DocTypeCode != "Im" && objDocument.Type != "Save" && chk_isAttachWord.Checked)
                                {

                                    //เปลี่ยน Vs-Status หลบการเช็ค Status ของChangeTypeToPDF จังหวะ last Approval approved.
                                    string tempStatus = vs_Status;
                                    vs_Status = "New";
                                    if (lbl_DocumentNo.Text != "Auto Generate")
                                    {
                                        vs_Status = "LastApp";
                                    }
                                    string msg = UpdateMSWord(false, objDocument.DocNo);
                                    if (!string.IsNullOrWhiteSpace(msg))
                                    {
                                        dbTrabs.Rollback();
                                        return msg;
                                    }
                                    //fix เป็นค่าก่อนย้ายได้เลย เพราะเข้าเคสนี้เฉพาะก่อนเอกสารจะถูกย้ายไป Document_Library
                                    Extension.ChangeTypeToPDF(vs_DocWordPath, "TempDocument", lbl_docSet.Text, Page);
                                    vs_Status = tempStatus;
                                }
                                #endregion
                                #region | Generate PDF |

                                if ((vs_Status == Extension._WaitForAdminCentre && rdb_Type.SelectedValue != "Save") ||
                                    (vs_Status == "Completed" && rdb_Type.SelectedValue == "Save") ||
                                    (vs_Status == "Completed" && ddl_DocType.SelectedValue == "Im" && rdb_Type.SelectedValue != "Save"))
                                {
                                    //Send to generate PDF behind
                                    //in case of Submit Completed
                                    if (!(vs_Status == "Completed" && rdb_Type.SelectedValue == "Save"))
                                    {
                                        TRNEDocumentQueue eDocQ = new TRNEDocumentQueue();
                                        eDocQ.DocID = objDocument.DocID;
                                        eDocQ.UserLoginName = vs_CurrentUserID;
                                        eDocQ.IsActive = true;
                                        DataContext.TRNEDocumentQueues.InsertOnSubmit(eDocQ);
                                        DataContext.SubmitChanges();
                                    }

                                    string DocSet = "";
                                    try
                                    {
                                        string DocLibName = "Document_Library";
                                        string DocTypeCode = "DocTypeCode";
                                        DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
                                        if (!dtDocType.DataTableIsNullOrEmpty())
                                        {
                                            DataTable sDocTypeResult = dtDocType.AsEnumerable().Where(r => r.Field<String>("Value").Equals(ddl_DocType.SelectedValue)).ToList().CopyToDataTable();
                                            if (!sDocTypeResult.DataTableIsNullOrEmpty())
                                            {
                                                DocTypeCode = sDocTypeResult.Rows[0]["DocTypeCode"].ToString();
                                            }
                                        }
                                        DateTime createDate = DateTime.Parse(objDocument.CreatedDate.ToString());
                                        if (ddl_DocType.SelectedValue.Equals("Im") && !lbl_DocumentNo.Text.Equals("Auto Generate"))
                                        {
                                            //Type 'Im' has been change to generate running No after Requester submitted the Document
                                            //So it has to be get current Running No here
                                            newRunningNo = lbl_DocumentNo.Text.Split('/')[0];

                                        }


                                        DocSet = string.Format("{0}_{1}/{2}{3}_{4}", DocTypeCode, newRunningNo, projectYear, objDocument.Category == "internal" ? string.Format("_{0}", objDocument.RequestorDepartmentID) : "", string.Format("{0}{1}{2}", createDate.Day.ToString("D2"), createDate.Month.ToString("D2"), createDate.Year.ToString().ConvertToBE()));
                                        DocSet = DocSet.Replace("/", "_");
                                        SharedRules.CreateDocumentSet(DocLibName, DocSet, null);
                                        objDocument.DocSet = DocSet;

                                        if ((ddl_DocType.SelectedValue != "Im" && rdb_Type.SelectedValue != "Save")
                                            || (chk_isAttachWord.Checked && btn_DownloadTemplate.Visible))
                                        {
                                            DataRow dr = vs_attachDocTable.NewRow();

                                            dr["Sequence"] = 1;
                                            dr["AttachFile"] = DocSet + ".pdf";
                                            dr["ActorName"] = "Generated by E-Document System";
                                            dr["AttachDate"] = DateTime.Now;
                                            dr["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, DocSet + ".pdf");

                                            vs_DocPDFPath = dr["AttachFilePath"].ToString();
                                            objDocument.AttachFilePath = vs_DocPDFPath;
                                            if (chk_isAttachWord.Checked)
                                            {
                                                vs_DocWordPath = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, string.Format("PIMEdocumentTemplate-[{0}].docx", objDocument.DocTypeCode));
                                                objDocument.AttachWordPath = vs_DocWordPath;
                                            }

                                            dr["DocSetName"] = DocSet;
                                            dr["DocLibName"] = "Document_Library";
                                            dr["ActorID"] = "-";
                                            dr["IsPrimary"] = "Y";
                                            vs_isDocAttach = true;
                                            vs_attachDocTable.Rows.Add(dr);
                                        }
                                        else
                                        {
                                            vs_DocPDFPath = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, vs_attachDocTable.Rows[0]["AttachFile"]); //string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, vs_attachDocTable.Rows[0]["AttachFile"]);
                                            vs_attachDocTable.Rows[0]["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}.pdf", Extension.GetSPSite(), DocLibName, DocSet, DocSet);//string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, DocSet + ".pdf");
                                            if (rdb_Type.SelectedValue == "Save")
                                            {
                                                vs_attachDocTable.Rows[0]["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, vs_attachDocTable.Rows[0]["AttachFile"]);//string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, DocSet, vs_attachDocTable.Rows[0]["AttachFile"]);
                                            }
                                            vs_attachDocTable.Rows[0]["AttachFile"] = DocSet + ".pdf";
                                            vs_attachDocTable.Rows[0]["DocSetName"] = DocSet;
                                            vs_attachDocTable.Rows[0]["DocLibName"] = "Document_Library";
                                            vs_isDocAttach = true;

                                            objDocument.AttachFilePath = vs_DocPDFPath;
                                            DataContext.SubmitChanges();
                                        }
                                        DataContext.SubmitChanges();

                                    }
                                    catch (Exception ex)
                                    {
                                        Extension.LogWriter.Write(ex);
                                    }
                                    #region | Move File to DocLib |
                                    SharedRules.CopyFileToAnotherDocSet("TempDocument", lbl_docSet.Text, "Document_Library", DocSet);
                                    if ((vs_Status == "Completed" && rdb_Type.SelectedValue == "Save") || (ddl_DocType.SelectedValue == "Im" && rdb_Type.SelectedValue != "Save") || (chk_isAttachWord.Checked))
                                    {
                                        //string sAttachDocName = ((HyperLink)gv_AttachDocumentFile.Rows[0].FindControl("hpl_AttachDocFile")).Text;
                                    }
                                    foreach (DataRow fileRow in vs_attachFileTable.Rows)
                                    {
                                        fileRow["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), "Document_Library", DocSet, fileRow["AttachFile"]);
                                        fileRow["DocSetName"] = DocSet;
                                        fileRow["DocLibName"] = "Document_Library";
                                    }
                                    SharedRules.DeleteDocSet("TempDocument", lbl_docSet.Text);
                                    #endregion
                                }
                                if (sAction == "Confirm" || (vs_Status == "Completed"))
                                {
                                    //SharedRules.MoveFileToAnotherDocSet("TempDocument", lbl_docSet.Text, "Document_Library", lbl_docSet.Text);
                                }
                                #endregion
                            }
                            else
                            {
                                //Update ? -> Do Nothing
                                //Confirm ? -> isExternalUpload ?  -> Gen new PDF  : do nothing
                                //Assign & Forward ? -> Gen new PDF 
                                if (sAction != "Update")
                                {
                                    if (sAction == "Assign & Forward" || (sAction == "Confirm" && vs_isExternalUpload) || sAction == "Cancel")
                                        //Generate new Document
                                        try
                                        {
                                            Stream stream = new MemoryStream(PrintMemoDetail(sAction, DataContext));
                                            SharedRules.UploadFileIntoDocumentSet(vs_Status == "Cancelled" || vs_Status == "Completed" ? "Document_Library" : "TempDocument", lbl_docSet.Text, string.Format("{0}{1}", lbl_docSet.Text, ".pdf"), stream, "", SharedRules.LogonName());
                                        }
                                        catch (Exception ex)
                                        {
                                            Extension.LogWriter.Write(ex);
                                        }
                                }

                            }


                            #region | Update/Insert Attach Document |
                            //update or insert Attach Document
                            if (!string.IsNullOrEmpty(vs_PK))
                            {
                                List<TRNAttachFileDoc> objListTRNAttachDoc = new List<TRNAttachFileDoc>();
                                objListTRNAttachDoc = DataContext.TRNAttachFileDocs.ToList();
                                IEnumerable<TRNAttachFileDoc> queryAD = (from TRNAttachFileDoc attachDoc in objListTRNAttachDoc
                                                                         where attachDoc.DocID == Convert.ToInt32(vs_PK)
                                                                         select attachDoc);
                                DataContext.TRNAttachFileDocs.DeleteAllOnSubmit(queryAD);
                                DataContext.SubmitChanges();
                            }
                            DataTable dtAttachDoc = vs_attachDocTable;
                            DataTable dtAttachFile = vs_attachFileTable;
                            List<TRNAttachFileDoc> listAttachDocument = new List<TRNAttachFileDoc>();
                            if (!dtAttachDoc.DataTableIsNullOrEmpty())
                            {

                                foreach (DataRow dr in dtAttachDoc.Rows)
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
                            if (!dtAttachFile.DataTableIsNullOrEmpty())
                            {
                                foreach (DataRow dr in dtAttachFile.Rows)
                                {
                                    TRNAttachFileDoc objAttachDocument = new TRNAttachFileDoc();
                                    objAttachDocument.DocID = objDocument.DocID;
                                    objAttachDocument.ActorID = dr["ActorID"].ToString();
                                    objAttachDocument.ActorName = dr["ActorName"].ToString();
                                    objAttachDocument.AttachDate = DateTime.Parse(dr["AttachDate"].ToString());
                                    objAttachDocument.AttachFile = dr["AttachFile"].ToString();
                                    objAttachDocument.AttachFIlePath = dr["AttachFilePath"].ToString();
                                    objAttachDocument.DocSetName = dr["DocSetName"].ToString();
                                    objAttachDocument.IsPrimary = dr["IsPrimary"].ToString();

                                    listAttachDocument.Add(objAttachDocument);
                                }
                            }
                            if (listAttachDocument.Count > 0)
                            {
                                DataContext.TRNAttachFileDocs.InsertAllOnSubmit(listAttachDocument);
                                DataContext.SubmitChanges();
                            }
                            #endregion 

                            dbTrabs.Commit();

                            if (false) // Enable for GenerateFull Preview PDF in every step and include it into email
                            {
                                //for make sure that fullPreviewPDF.pdf always Generate
                                vs_PK = objDocument.DocID.ToString();
                                byte[] fullPDF = PrintMemoDetail("", DataContext);
                                Extension.UploadFileToSP(objDocument.DocLib, objDocument.DocSet, "fullPreviewPDF.pdf", fullPDF);
                            }

                            #region | Send Email |

                            if (sAction.Equals("Update"))
                            {
                            }
                            else if (vs_Status == "Cancelled" || vs_Status == "Rejected" || vs_Status == "Reworked")
                            {
                                #region | Send mail to Creator & Requester |
                                Extension.SendEmailTemplate(objDocument.Status, objDocument.CreatorID.ToString(), objDocument.RequestorDepartmentID.ToString(), sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                if (objDocument.CreatorID != objDocument.RequestorID)
                                {
                                    Extension.SendEmailTemplate(objDocument.Status, objDocument.RequestorID.ToString(), objDocument.RequestorDepartmentID.ToString(), sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                }
                                #endregion
                            }
                            else if (sAction != "Save Draft" && vs_Status != Extension._Draft && sAction != "Assign & Forward")
                            {
                                if (vs_Status == "Completed")
                                {
                                    if (!(objDocument.DocTypeCode.Equals("M") || objDocument.DocTypeCode.Equals("Im")))
                                    {

                                        if (vs_Status == "Completed")
                                        {
                                            #region | Send mail to Permisison |
                                            if (objDocument.PermissionType == "Secret")
                                            {
                                                List<TRNPermission> listPermission = DataContext.TRNPermissions.Where(x => x.DocID == objDocument.DocID).ToList();
                                                foreach (TRNPermission objPermission in listPermission)
                                                {
                                                    Extension.SendEmailTemplate("NotifyToPermission", objPermission.EmpID.ToString(), objPermission.DepartmentID.ToString(), "Approve", "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                                }
                                            }
                                            else
                                            {
                                                List<TRNGroupMail> listGroupMail = DataContext.TRNGroupMails.Where(x => x.DocID == objDocument.DocID.ToString()).ToList();
                                                foreach (TRNGroupMail objGroupMail in listGroupMail)
                                                {
                                                    Extension.SendEmailTemplate("NotifyToPermission", "", "", "Approve", "", objGroupMail.DepartmentGroupMail, objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                                    //SendEmailTemplate(objDocument.Status, objGroupMail.DepartmentGroupMail, "Approve", "", objDocument.DocID.ToString(), DocSet, objEdocQ.UserLoginName);
                                                }
                                            }
                                            #endregion
                                        }
                                        if (!(sAction == "Send Email" && vs_Status == "Completed"))
                                        {
                                            #region | Send mail to Creator & Requester |
                                            Extension.SendEmailTemplate(objDocument.Status, objDocument.CreatorID.ToString(), objDocument.RequestorDepartmentID.ToString(), sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                            if (objDocument.CreatorID != objDocument.RequestorID)
                                            {
                                                Extension.SendEmailTemplate(objDocument.Status, objDocument.RequestorID.ToString(), objDocument.RequestorDepartmentID.ToString(), sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                else if (vs_Status == Extension._WaitForAdminCentre)
                                {
                                    //Change to send after PDF Generated
                                    //Extension.SendEmailTemplate("Update Document", objDocument.WaitingFor.ToString(), objDocument.WaitingForDeptID.ToString(), sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                }
                                else
                                {

                                    if (sAction.Equals("Reply"))
                                    {
                                        Extension.SendEmailTemplate("ReplyComment", objDocument.WaitingFor.ToString(), objDocument.WaitingForDeptID.ToString(), sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                    }
                                    else
                                    {
                                        //Send To Next Actor (Waiting For)
                                        #region | Send to Delegate User |
                                        List<v_TRNDelegateDetail> listDelegate = new List<v_TRNDelegateDetail>();
                                        listDelegate = DataContext.v_TRNDelegateDetails
                                            .Where(x => x.DelegateToID == vs_CurrentUserID).ToList();
                                        listDelegate = listDelegate
                                            .Where(x => objDocument.WaitingFor.Replace(" ", "").Split(',').Contains(x.ApproverID)).ToList();
                                        listDelegate = listDelegate
                                            .Where(x => x.DepartmentID == objDocument.WaitingForDeptID).ToList();
                                        listDelegate = listDelegate
                                            .Where(x => Convert.ToBoolean(x.IsActive)).ToList();
                                        listDelegate = listDelegate.Where(x =>
                                        {
                                            bool i = Convert.ToBoolean(x.IsByDocType);
                                            return i ? x.DocType == objDocument.DocTypeCode : x.DocID == vs_PK;
                                        }
                                            ).ToList();
                                        listDelegate = listDelegate
                                            .Where(x => (DateTime.Now >= x.DateFrom && (x.DateTo == null || DateTime.Now <= x.DateTo))).ToList();

                                        foreach (v_TRNDelegateDetail item in listDelegate)
                                        {
                                            Extension.SendEmailTemplate(objDocument.Status, item.ApproverID, item.DepartmentID, sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);

                                        }
                                        #endregion
                                        Extension.SendEmailTemplate(objDocument.Status, objDocument.WaitingFor.ToString(), objDocument.WaitingForDeptID.ToString(), sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                    }

                                }
                            }
                            else if (sAction == "Assign & Forward")
                            {
                                foreach (DataRow row in vs_Assign.Rows)
                                {
                                    Extension.SendEmailTemplate("Assign", row["EmpID"].ToString(), row["DepartmentID"].ToString(), sAction, "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                                }
                            }
                            #endregion

                        }
                        catch (Exception ex)
                        {
                            Extension.LogWriter.Write(ex);
                            dbTrabs.Rollback();
                            DivButtonForSubmit.Visible = true;
                            return ex.Message.ToString();
                        }
                        finally
                        {
                            if (DataContext.Connection.State == System.Data.ConnectionState.Open)
                            {
                                DataContext.Connection.Close();
                            }
                        }
                    }
                    return "";
                }
                else
                {
                    return sValidtionMsg;
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return ex.Message.ToString();
            }
        }
        #endregion

        #region | Popup Search |
        #region "Popup Requestor"
        public void OpenPopup(object sender, EventArgs e)
        {
            txt_searchBox.Text = "";
            LinkButton linkBtn = new LinkButton();
            Button btn = new Button();

            if (sender is LinkButton)
            {
                linkBtn = (LinkButton)sender;
            }
            else if (sender is Button)
            {
                btn = (Button)sender;
            }
            else
            {
                return;
            }

            switch (sender is LinkButton ? linkBtn.ID : btn.ID)
            {
                case "btnSearchEmployee":
                    vs_popUpTarget = "Requestor";
                    break;
                case "Btn_Add_Spec_Approver":
                    vs_popUpTarget = "Approver";
                    break;
                case "btn_addPermission":
                    vs_popUpTarget = "Permission";
                    break;
                case "lkbtnSearchAdvisor":
                    vs_popUpTarget = "Advisor";
                    txt_searchBox.Text = txt_RequestComment.Text;
                    break;
                case "lkbtnSearchAssign":
                    vs_popUpTarget = "Assign";
                    break;
                case "btn_searchSendTo":
                    vs_popUpTarget = "SendTo";
                    break;
                default:
                    vs_popUpTarget = ""; break;
            }
            LoadDataEmpToCache(txt_searchBox.Text);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popSearchToModal1", "$('#searchEmpReqModal').modal('show');", true);

        }
        protected override void Render(HtmlTextWriter writer)
        {
            #region "GridView"
            foreach (GridViewRow r in gv_searchEmpReqTable.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation(gv_searchEmpReqTable.UniqueID, "Select$" + r.RowIndex);
                }
            }
            foreach (GridViewRow r in gv_searchDocTable.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation(gv_searchDocTable.UniqueID, "Select$" + r.RowIndex);
                }
            }
            foreach (GridViewRow r in gv_SearchDepartment.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation(gv_SearchDepartment.UniqueID, "Select$" + r.RowIndex);
                }
            }
            if (gv_Assign.FooterRow != null)
            {
                if (gv_Assign.FooterRow.RowType == DataControlRowType.Footer)
                {
                    gv_Assign.FooterRow.Cells[0].ColumnSpan = gv_Assign.FooterRow.Cells.Count;
                    for (int i = gv_Assign.FooterRow.Cells.Count - 1; i > 0; i--)
                    {
                        gv_Assign.FooterRow.Cells.RemoveAt(i);
                    }
                }
            }

            #endregion
            base.Render(writer);
        }
        protected void searchBtn_Click(object sender, EventArgs e)
        {
            LoadDataEmpToCache(txt_searchBox.Text);
            txt_searchBox.Focus();
        }
        private void LoadDataEmpToCache(string sFilterKeyword)
        {
            DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
            if (vs_popUpTarget == "Approver" || vs_popUpTarget == "Assign")
            {
                try
                {

                    //Merge Table
                    DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                    List<TRNEmployeeExtension> listEmpEx = db.TRNEmployeeExtensions.ToList();
                    if (listEmpEx.Count > 0)
                    {
                        List<TRNEmployeeExtension> listEmp = Extension.DataTableToList<TRNEmployeeExtension>(dtEmp);
                        if (listEmp.Count > 0)
                        {
                            foreach (TRNEmployeeExtension item in listEmpEx)
                            {
                                listEmp.Add(item);
                            }
                            listEmp = listEmp.OrderBy(x => x.EMPLOYEEID).ToList();
                            dtEmp = Extension.ListToDataTable<TRNEmployeeExtension>(listEmp);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            if (!string.IsNullOrEmpty(sFilterKeyword))
            {
                DataView dvFilter = dtEmp.Copy().DefaultView;
                dvFilter.RowFilter = string.Format(@"
				DEPARTMENT_NAME_TH LIKE '%{0}%'
				OR SUBDEPARTMENT_NAME_TH LIKE '%{0}%'
				OR POSTION_NAME_TH LIKE '%{0}%'
				OR EmployeeID LIKE '%{0}%' 
				OR FIRSTNAME_TH LIKE '%{0}%' 
				OR LASTNAME_TH LIKE '%{0}%' 
				OR PREFIX_TH LIKE '%{0}%' 
				OR FIRSTNAME_EN LIKE '%{0}%' 
				OR LASTNAME_EN LIKE '%{0}%' 
				OR PREFIX_EN LIKE '%{0}%' 
				OR Email LIKE '%{0}%'
                OR EmployeeName_TH LIKE '%{0}%'
                OR EmployeeName_EN LIKE '%{0}%'", sFilterKeyword);
                dtEmp = dvFilter.ToTable();
            }
            gv_searchEmpReqTable.DataSource = dtEmp;
            gv_searchEmpReqTable.DataBind();
        }
        protected void searchEmpTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                e.Row.ToolTip = "Click for selecting this row.";
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_searchEmpReqTable, "Select$" + e.Row.RowIndex);

                Label lblEmpName = (Label)e.Row.FindControl("lblEmpName");
                object objPrefixData = DataBinder.Eval(e.Row.DataItem, "PREFIX_TH");
                object objNameData = DataBinder.Eval(e.Row.DataItem, "FIRSTNAME_TH");
                object objLNameData = DataBinder.Eval(e.Row.DataItem, "LASTNAME_TH");
                if (lblEmpName != null)
                    if (objNameData != null && objNameData != DBNull.Value) lblEmpName.Text = string.Format("{0} {1} {2}", objPrefixData, objNameData, objLNameData);

                Label lblPosition = (Label)e.Row.FindControl("lblPosition");
                object objPositionData = DataBinder.Eval(e.Row.DataItem, "POSTION_NAME_TH");
                if (lblPosition != null)
                    if (objPositionData != null && objPositionData != DBNull.Value) lblPosition.Text = (string)objPositionData;

                Label lblDepartment = (Label)e.Row.FindControl("lblDepartment");
                object objDepartmentData = DataBinder.Eval(e.Row.DataItem, "DEPARTMENT_NAME_TH");
                if (lblDepartment != null)
                    if (objDepartmentData != null && objDepartmentData != DBNull.Value) lblDepartment.Text = objDepartmentData.ToString();

                Label lblSubDepartment = (Label)e.Row.FindControl("lblSubDepartment");
                object objSubDepartmentData = DataBinder.Eval(e.Row.DataItem, "SUBDEPARTMENT_NAME_TH");
                if (lblSubDepartment != null)
                    if (objSubDepartmentData != null && objSubDepartmentData != DBNull.Value) lblSubDepartment.Text = objSubDepartmentData.ToString();

            }
        }
        protected void searchEmpTable_IndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow gv_row in gv_searchEmpReqTable.Rows)
            {
                if (gv_row.RowIndex == gv_searchEmpReqTable.SelectedIndex)
                {
                    string sEmpID = gv_row.Cells[0].Text;
                    DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
                    if (dtEmp != null)
                    {
                        DataView dvFilter = dtEmp.Copy().DefaultView;
                        dvFilter.RowFilter = string.Format("EmployeeID = '{0}'", sEmpID);
                        if (dvFilter.ToTable().Rows.Count > 0)
                        {
                            string sEmpName = dvFilter.ToTable().Rows[0]["EmployeeName"].ToString();
                            if (vs_popUpTarget.Equals("Approval"))
                            {
                                gvApprovelList.DataSource = dtEmp;
                                gvApprovelList.DataBind();
                            }
                        }
                    }
                }
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchEmpModal", "$('#searchEmpModal').modal('hide');", true);

        }
        protected void btnClosePopup_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchCCModal1", "$('#searchEmpReqModal').modal('hide');", true);
        }

        protected void gv_searchEmpReqTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_searchEmpReqTable.PageIndex = e.NewPageIndex;
            LoadDataEmpToCache(txt_searchBox.Text);
        }

        protected void btn_resetBtn_Click(object sender, EventArgs e)
        {
            txt_searchBox.Text = "";
            LoadDataEmpToCache("");
        }

        protected void gv_searchEmpReqTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow gv_row in gv_searchEmpReqTable.Rows)
                {
                    if (gv_row.RowIndex == gv_searchEmpReqTable.SelectedIndex)
                    {
                        HiddenField hdnEmpID = (HiddenField)gv_row.FindControl("hdnEmpID");
                        HiddenField hdnDeptID = (HiddenField)gv_row.FindControl("hdnDeptID");
                        HiddenField hdnSubDeptID = (HiddenField)gv_row.FindControl("hdnSubDeptID");
                        HiddenField hdnPosID = (HiddenField)gv_row.FindControl("hdnPosID");
                        string sEmpID = gv_row.Cells[0].Text; ;
                        string sDeptID = hdnDeptID.Value.ToString();
                        string sSubDeptID = hdnSubDeptID.Value.ToString();
                        string sPosID = hdnPosID.Value.ToString();

                        if (!string.IsNullOrWhiteSpace(lbl_RequestorID.Text))
                        {
                            SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(this.Page, sEmpID);

                            DataView dvFilter = Extension.GetEmployeeData(this.Page).Copy().DefaultView;
                            //dvFilter.RowFilter = string.Format("EmployeeID = '{0}'", sEmpID);
                            dvFilter.RowFilter = string.Format(@"
				EmployeeID = '{0}'
				AND DEPARTMENT_ID = '{1}'
				AND SUBDEPARTMENT_ID = '{2}' 
				AND POSITION_TD = '{3}' ", sEmpID, sDeptID, sSubDeptID, sPosID);
                            DataTable dtEmp = dvFilter.ToTable();
                            if (empData != null)
                            {
                                string empNameTH = string.Format("{0} {1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH, empData.LASTNAME_TH);
                                string empNameEN = string.Format("{0} {1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN, empData.LASTNAME_EN);
                                switch (vs_popUpTarget)
                                {
                                    case "Requestor":
                                        vs_RequestorID = sEmpID;
                                        InitialData();


                                        lbl_RequestorID.Text = sEmpID;
                                        lbl_RequestorName.Text = empNameTH;
                                        lbl_RequestorTel.Text = empData.TELEPHONE.ToString();
                                        RequestorDeptChange(empData);
                                        ddl_RequestorDepartment.SelectedValue = empData.RESULT.First(x => x.DEPARTMENT_ID.Equals(sDeptID)).DEPARTMENT_ID;
                                        RequestorSubDeptChange(empData);
                                        ddl_RequestorSubDepartment.SelectedValue = empData.RESULT.First(x => x.SUBDEPARTMENT_ID.Equals(sSubDeptID)).SUBDEPARTMENT_ID;
                                        RequestorPosChange(empData);
                                        ddl_RequestorPosition.SelectedValue = empData.RESULT.First(x => x.POSITION_TD.Equals(sPosID)).POSITION_TD;
                                        txt_FromDepartment.Text = empData.RESULT.First(x => x.DEPARTMENT_ID.Equals(sDeptID)).DEPARTMENT_NAME_TH;
                                        hdn_FromDepartmentID.Text = empData.RESULT.First(x => x.DEPARTMENT_ID.Equals(sDeptID)).DEPARTMENT_ID;

                                        break;
                                    case "Approver":
                                        DataTable dtApproval = vs_ApprovalList;
                                        DataRow drApproval = dtApproval.NewRow();
                                        drApproval["Sequence"] = dtApproval.Rows.Count + 1;
                                        drApproval["EmpID"] = sEmpID;
                                        drApproval["EmployeeName"] = empNameTH;
                                        DataTable dtDept = Extension.GetSpecificDepartmentDataFromTemp(Page, sDeptID);
                                        if (!dtDept.DataTableIsNullOrEmpty())
                                        {
                                            drApproval["DepartmentID"] = sDeptID;
                                            drApproval["DepartmentName"] = dtDept.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                                            //drApproval["DepartmentName"] = dtDept.AsEnumerable().Where(r => r.Field<string>("DEPARTMENT_ID") == sDeptID).Single().Field<string>("DEPARTMENT_NAME_TH");
                                            //drApproval["DepartmentName"] = empData.RESULT.First(x => x.DEPARTMENT_ID == sDeptID).POSTION_NAME_TH;
                                        }
                                        dtDept = Extension.GetSpecificDepartmentDataFromTemp(Page, sDeptID);
                                        if (!dtDept.DataTableIsNullOrEmpty())
                                        {
                                            drApproval["SubDepartmentID"] = sSubDeptID;
                                            drApproval["SubDepartmentName"] = dtDept.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                                            //drApproval["SubDepartmentName"] = dtDept.AsEnumerable().Where(r => r.Field<string>("DEPARTMENT_ID") == sSubDeptID).Single().Field<string>("DEPARTMENT_NAME_TH");
                                            //drApproval["SubDepartmentName"] = empData.RESULT.First(x => x.DEPARTMENT_ID == sDeptID && x.SUBDEPARTMENT_ID == sSubDeptID).POSTION_NAME_TH;
                                        }
                                        DataTable dtPosition = Extension.GetPositionData(Page);
                                        if (!dtPosition.DataTableIsNullOrEmpty())
                                        {
                                            drApproval["PositionID"] = sPosID;
                                            drApproval["PositionName"] = dtPosition.AsEnumerable().Where(r => r.Field<string>("POSITION_ID") == sPosID).Single().Field<string>("POSITION_NAME_TH");
                                            //drApproval["PositionName"] = empData.RESULT.First(x => x.DEPARTMENT_ID == sDeptID && x.SUBDEPARTMENT_ID == sSubDeptID && x.POSITION_TD == sPosID).POSTION_NAME_TH;
                                        }
                                        dtApproval.Rows.Add(drApproval);
                                        vs_ApprovalList = dtApproval;
                                        BinddtApproverViewStateToGridview();
                                        break;
                                    case "Permission":
                                        DataTable dtPermission = vs_PermissionList;
                                        DataRow drPermission = dtPermission.NewRow();
                                        drPermission["Sequence"] = dtPermission.Rows.Count + 1;
                                        drPermission["EmpID"] = empData.EMPLOYEEID;
                                        drPermission["EmployeeName"] = empNameTH;
                                        drPermission["DepartmentID"] = sDeptID;
                                        drPermission["DepartmentName"] = empData.RESULT.First(x => x.DEPARTMENT_ID == sDeptID).POSTION_NAME_TH;
                                        drPermission["SubDepartmentID"] = sSubDeptID;
                                        drPermission["SubDepartmentName"] = empData.RESULT.First(x => x.DEPARTMENT_ID == sDeptID && x.SUBDEPARTMENT_ID == sSubDeptID).POSTION_NAME_TH;
                                        drPermission["PositionID"] = sPosID;
                                        drPermission["PositionName"] = empData.RESULT.First(x => x.DEPARTMENT_ID == sDeptID && x.SUBDEPARTMENT_ID == sSubDeptID && x.POSITION_TD == sPosID).POSTION_NAME_TH;
                                        dtPermission.Rows.Add(drPermission);
                                        vs_PermissionList = dtPermission;
                                        BinddtPermissionViewStateToGridview();
                                        break;
                                    case "Assign":
                                        DataTable dtAssign = vs_Assign;
                                        DataRow drAssign = dtAssign.NewRow();
                                        drAssign["Sequence"] = dtAssign.Rows.Count + 1;
                                        drAssign["EmpID"] = empData.EMPLOYEEID;
                                        drAssign["EmployeeName"] = vs_Lang == "TH" ? empNameTH : empNameEN;
                                        drAssign["DepartmentID"] = sDeptID;
                                        drAssign["Email"] = empData.EMAIL;
                                        drAssign["Tel"] = empData.TELEPHONE;
                                        dtAssign.Rows.Add(drAssign);
                                        vs_Assign = dtAssign;
                                        BinddtAssignViewStateToGridview();

                                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "PopReqComment", "$('#searchAssign').modal();", true);
                                        break;
                                    case "Advisor":
                                        txt_RequestCommentEMPID.Text = sEmpID;
                                        txt_RequestCommentDEPID.Text = sDeptID;
                                        txt_RequestComment.Text = vs_Lang == "TH" ? empNameTH : empNameEN;
                                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "PopReqComment", "$('#searchAdvisor').modal();", true);
                                        break;
                                    case "SendTo":
                                        lbl_sendToID.Text = sEmpID;
                                        txt_SendTo.Text = vs_Lang == "TH" ? empNameTH : empNameEN;
                                        break;
                                    default:
                                        break;
                                }

                            }
                        }

                    }
                }
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchEmpModal", "$('#searchEmpReqModal').modal('hide');", true);
            }
            catch (Exception ex)
            {

                Extension.LogWriter.Write(ex);

            }

        }
        #endregion

        #region | Search Reference Document |

        private void OpenSearchDocPopup()
        {
            txt_searchDoc.Text = "";
            txt_CreateDateFrom.Text = "";
            txt_CreateDateTo.Text = "";
            gv_searchDocTable.PageIndex = 0;

            BindDataDocFromGV();

            DataTable dtDepartment = Extension.GetDepartmentData(this.Page);
            List<ListItem> ltDepartment = new List<ListItem>();

            ddlToDepartment.DataSource = dtDepartment;
            ddlToDepartment.DataTextField = "DEPARTMENT_NAME_TH";
            ddlToDepartment.DataValueField = "DEPARTMENT_ID";
            ddlToDepartment.DataBind();
            ddlToDepartment.Items.Insert(0, new ListItem("-- Please Select --", ""));
            ddlToDepartment.SelectedIndex = 0;


            ddlFromDepartment.DataSource = dtDepartment;
            ddlFromDepartment.DataTextField = "DEPARTMENT_NAME_TH";
            ddlFromDepartment.DataValueField = "DEPARTMENT_ID";
            ddlFromDepartment.DataBind();
            ddlFromDepartment.Items.Insert(0, new ListItem("-- Please Select --", ""));
            ddlFromDepartment.SelectedIndex = 0;

            ddlDocumentType.SelectedIndex = 0;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popSearchModal", "$('#searchDocModal').modal('show');", true);
        }
        protected void btnClosePopupSearchDoc_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchModal", "$('#searchDocModal').modal('hide');", true);
        }
        protected void gv_searchDocTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_searchDocTable.PageIndex = e.NewPageIndex;
            BindDataDocFromGV();

        }
        private void BindDataDocFromGV()
        {
            if (vs_DocumentList == null)
            {
                List<TRNDocument> data = new List<TRNDocument>();
                vs_DocumentList = Extension.ListToDataTable(data);
            }
            gv_searchDocTable.DataSource = vs_DocumentList;
            gv_searchDocTable.DataBind();
        }
        protected void gv_searchDocTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                e.Row.ToolTip = "Click for selecting this row.";
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_searchDocTable, "Select$" + e.Row.RowIndex);

                HiddenField hdnDocNo = (HiddenField)e.Row.FindControl("hdnDocNo");
                object objLDocNo = DataBinder.Eval(e.Row.DataItem, "DocID");
                if (objLDocNo != null)
                    if (objLDocNo != null && objLDocNo != DBNull.Value) hdnDocNo.Value = objLDocNo.ToString();


                Label lblDocType = (Label)e.Row.FindControl("lblDocType");
                object objLDocType = DataBinder.Eval(e.Row.DataItem, "DocTypeCode");
                if (lblDocType != null)
                    if (objLDocType != null && objLDocType != DBNull.Value)
                    {
                        //DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", "<View><Query><Where><And><Eq><FieldRef Name='DocTypeCode' /><Value Type='text'>" + objLDocType.ToString() + "</Value></Eq><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq></And></Where><RowLimit>100000</RowLimit></Query></View>");
                        DataTable dtDocumentType = Extension.GetDataTable("MstDocumentType");
                        if (!dtDocumentType.DataTableIsNullOrEmpty())
                        {
                            DataTable oResult = dtDocumentType.AsEnumerable()
                                .Where(r => r.Field<String>("Value").Equals(objLDocType.ToString())).ToList().CopyToDataTable();
                            if (!oResult.DataTableIsNullOrEmpty())
                            {
                                lblDocType.Text = oResult.Rows[0]["DocTypeName"].ToString();
                            }
                        }
                    }

                Label lblDocNo = (Label)e.Row.FindControl("lblDocNo");
                object objDocNo = DataBinder.Eval(e.Row.DataItem, "DocNo");
                if (lblDocNo != null && objDocNo != null && objDocNo != DBNull.Value && !objLDocType.Equals("L") && !objLDocType.Equals("R"))
                {
                    lblDocNo.Text = objDocNo.ToString();
                }

                Label lblCreateDate = (Label)e.Row.FindControl("lblCreateDate");
                object objCreateDate = DataBinder.Eval(e.Row.DataItem, "CreatedDate");
                if (lblCreateDate != null)
                    if (objCreateDate != null && objCreateDate != DBNull.Value)
                    {
                        DateTime createdDate = DateTime.Parse(objCreateDate.ToString());
                        lblCreateDate.Text = createdDate.ToString("dd/MM/yyyy");

                    }

                Label lblCreator = (Label)e.Row.FindControl("lblCreator");
                object objCreator = DataBinder.Eval(e.Row.DataItem, "CreatorID");
                if (lblCreator != null)
                    if (objCreator != null && objCreator != DBNull.Value)
                    {
                        DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(vs_ConnectionString);
                        DataTable dtEmp = Extension.GetEmployeeData(this.Page);
                        DataView dvFilter = dtEmp.Copy().DefaultView;
                        dvFilter.RowFilter = string.Format("EmployeeID = '{0}'", Convert.ToInt32(objCreator));
                        if (dvFilter.ToTable().Rows.Count > 0)
                        {
                            lblCreator.Text = dvFilter.ToTable().Rows[0]["EmployeeName_TH"].ToString();
                        }
                        //SpecificEmployeeData.RootObject sCreator = Extension.GetSpecificEmployeeData(this.Page, objCreator.ToString());
                        //lblCreator.Text = string.Format("{0}{1} {2}", sCreator.PREFIX_TH, sCreator.FIRSTNAME_TH, sCreator.LASTNAME_TH);
                    }

                Label lblFromDepartment = (Label)e.Row.FindControl("lblFromDepartment");
                object objFromDepartment = DataBinder.Eval(e.Row.DataItem, "FromDepartmentName");
                if (lblFromDepartment != null)
                    if (objFromDepartment != null && objFromDepartment != DBNull.Value) lblFromDepartment.Text = objFromDepartment.ToString();

                Label lbl_ToDepartment = (Label)e.Row.FindControl("lblToDepartment");
                object objToDepartment = DataBinder.Eval(e.Row.DataItem, "ToDepartmentName");
                if (lbl_ToDepartment != null)
                    if (objToDepartment != null && objToDepartment != DBNull.Value)
                    {
                        lbl_ToDepartment.Text = objToDepartment.ToString().Split(',')[0];
                        if (objToDepartment.ToString().Split(',').Length > 1)
                        {
                            lbl_ToDepartment.Text += ", ...";
                        }
                    }

                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                object objStatus = DataBinder.Eval(e.Row.DataItem, "Status");
                if (lblStatus != null)
                    if (objStatus != null && objStatus != DBNull.Value) lblStatus.Text = objStatus.ToString();

            }
        }

        protected void gv_searchDocTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow gv_row in gv_searchDocTable.Rows)
            {
                if (gv_row.RowIndex == gv_searchDocTable.SelectedIndex)
                {
                    HiddenField hdnDocNo = (HiddenField)gv_row.FindControl("hdnDocNo");

                    if (!string.IsNullOrWhiteSpace(hdnDocNo.Value))
                    {
                        int sDocNo = Convert.ToInt32(hdnDocNo.Value);

                        DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(vs_ConnectionString);
                        TRNDocument objDocument = db.TRNDocuments.SingleOrDefault(x => x.DocID == sDocNo);

                        DataTable dtRefDoc = vs_DtRefDoc;
                        int iCurrentRowCount = dtRefDoc != null ? dtRefDoc.Rows.Count : 0;

                        DataRow dr = dtRefDoc.NewRow();
                        dr["Sequence"] = iCurrentRowCount + 1;



                        dr["DocNo"] = objDocument.DocNo;
                        dr["Title"] = objDocument.Title;
                        dr["Category"] = objDocument.Category;
                        dr["DocumentType"] = objDocument.DocTypeCode;
                        dr["RefDocID"] = objDocument.DocID;
                        vs_DtRefDoc.Rows.Add(dr);
                        gv_ReferenceDocument.DataSource = dtRefDoc;
                        gv_ReferenceDocument.DataBind();

                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchModal", "$('#searchDocModal').modal('hide');", true);
                    }
                }
            }
        }

        protected void btn_searchDoc_Click(object sender, EventArgs e)
        {
            try
            {
                DataClassesDataAccessDataContext DataContext = new DataClassesDataAccessDataContext(vs_ConnectionString);
                IQueryable<TRNDocument> queryDoc = DataContext.TRNDocuments.Where(x => x.Status == Extension._Completed);
                string searchText = txt_searchDoc.Text;
                DateTime? searchDateFrom = Extension.ConvertTextToDate(txt_CreateDateFrom.Text);
                DateTime? searchDateTo = Extension.ConvertTextToDate(txt_CreateDateTo.Text);
                string searchDepartmentFrom = ddlFromDepartment.SelectedValue;
                string searchDepartmentTo = ddlToDepartment.SelectedValue;
                string searchDocType = ddlDocumentType.SelectedValue;

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    queryDoc = queryDoc.Where(x => x.Title.Contains(searchText) || x.DocNo.Contains(searchText) || x.Description.Contains(searchText));

                }
                if (searchDateFrom != null)
                {
                    queryDoc = queryDoc.Where(x => x.CreatedDate.HasValue && x.CreatedDate.Value.Date >= searchDateFrom.Value.Date);
                }

                if (searchDateTo != null)
                {
                    queryDoc = queryDoc.Where(x => x.CreatedDate.HasValue && x.CreatedDate.Value.Date <= searchDateTo.Value.Date);
                }

                if (!string.IsNullOrWhiteSpace(searchDepartmentFrom))
                {
                    int id = int.Parse(searchDepartmentFrom);
                    queryDoc = queryDoc.Where(x => x.FromDepartmentID == id);
                }

                if (!string.IsNullOrWhiteSpace(searchDepartmentTo))
                {
                    queryDoc = queryDoc.Where(x => x.ToDepartmentID == searchDepartmentTo);
                }

                if (!string.IsNullOrWhiteSpace(searchDocType))
                {
                    queryDoc = queryDoc.Where(x => x.DocTypeCode == searchDocType);
                }

                DataTable dtDocuement = Extension.ListToDataTable(queryDoc.ToList());

                vs_DocumentList = dtDocuement;
                BindDataDocFromGV();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        protected void btn_ResetDoc_OnClick(object sender, EventArgs e)
        {
            OpenSearchDocPopup();
        }
        #endregion

        #region "Popup Search Approval"
        protected void BinddtApproverViewStateToGridview()
        {
            gvApprovelList.DataSource = vs_ApprovalList;
            gvApprovelList.DataBind();
        }
        protected void ddl_ApprovorDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApprovorDDLChanged(sender, "DepartmentID");
        }

        protected void ddl_ApprovorSubDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApprovorDDLChanged(sender, "SubDepartmentID");
        }

        protected void ddl_ApprovorPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApprovorDDLChanged(sender, "PositionID");
        }
        protected void ApprovorDDLChanged(object sender, string target)
        {
            DropDownList ddl = (DropDownList)sender;
            if (ddl != null)
            {
                Panel pnl = (Panel)ddl.Parent;
                if (pnl != null)
                {
                    Label lbl = (Label)pnl.FindControl("lblApprovorSequence");
                    if (lbl != null)
                    {
                        string sSeq = lbl.Text;
                        DataTable dt_SpecificApproval = vs_ApprovalList;
                        if (dt_SpecificApproval != null)
                        {
                            foreach (DataRow dr in dt_SpecificApproval.Rows)
                            {
                                if (dr["Sequence"].ToString() == sSeq)
                                {
                                    dr[target] = Convert.ToInt32(ddl.SelectedValue);
                                    break;
                                }
                            }
                            vs_ApprovalList = dt_SpecificApproval;
                            BinddtApproverViewStateToGridview();
                        }
                    }
                }
            }
        }

        protected void gvApprovelList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblApproverName = (Label)e.Row.FindControl("lblApproverName");
                DropDownList ddlApproverDepartment = (DropDownList)e.Row.FindControl("ddl_ApprovorDepartment");
                DropDownList ddlApproverSubDepartment = (DropDownList)e.Row.FindControl("ddl_ApprovorSubDepartment");
                DropDownList ddlApproverPosition = (DropDownList)e.Row.FindControl("ddl_ApprovorPosition");
                if (ddlApproverSubDepartment != null && ddlApproverSubDepartment != null && ddlApproverPosition != null && lblApproverName != null)
                {
                    object EmpID = DataBinder.Eval(e.Row.DataItem, "EmpID");
                    object DeptID = DataBinder.Eval(e.Row.DataItem, "DepartmentID");
                    object SubDeptID = DataBinder.Eval(e.Row.DataItem, "SubDepartmentID");
                    object PosID = DataBinder.Eval(e.Row.DataItem, "PositionID");
                    if (EmpID != null && EmpID != DBNull.Value)
                    {
                        if (DeptID != null && DeptID != DBNull.Value)
                        {
                            if (SubDeptID != null && SubDeptID != DBNull.Value)
                            {
                                if (PosID != null && PosID != DBNull.Value)
                                {
                                    SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(this.Page, EmpID.ToString());

                                    if (empData != null)
                                    {
                                        string empNameTH = string.Format("{0}{1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH, empData.LASTNAME_TH);
                                        string empNameEN = string.Format("{0}{1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN, empData.LASTNAME_EN);

                                        lblApproverName.Text = vs_Lang == "TH" ? empNameTH : empNameEN;
                                        DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                                        try
                                        {
                                            //Add Employee Extension
                                            List<TRNEmployeeExtension> listEmpEx = db.TRNEmployeeExtensions.Where(x => x.EMPLOYEEID == EmpID.ToString()).ToList();
                                            if (listEmpEx.Count > 0)
                                            {
                                                foreach (TRNEmployeeExtension item in listEmpEx)
                                                {
                                                    SpecificEmployeeData.RESULT result = new SpecificEmployeeData.RESULT();
                                                    result.DEPARTMENT_ID = item.DEPARTMENT_ID;
                                                    result.DEPARTMENT_NAME_EN = item.DEPARTMENT_NAME_EN;
                                                    result.DEPARTMENT_NAME_TH = item.DEPARTMENT_NAME_TH;
                                                    result.SUBDEPARTMENT_ID = item.SUBDEPARTMENT_ID;
                                                    result.SUBDEPARTMENT_NAME_EN = item.SUBDEPARTMENT_NAME_EN;
                                                    result.SUBDEPARTMENT_NAME_TH = item.SUBDEPARTMENT_NAME_TH;
                                                    result.POSITION_TD = item.POSITION_TD;
                                                    result.POSTION_NAME_EN = item.POSTION_NAME_EN;
                                                    result.POSTION_NAME_TH = item.POSTION_NAME_TH;
                                                    result.PRIMARY_POSITION = item.PRIMARY_POSITION;
                                                    empData.RESULT.Add(result);


                                                }
                                                if (string.IsNullOrWhiteSpace(empNameEN) || string.IsNullOrWhiteSpace(empNameTH))
                                                {
                                                    TRNEmployeeExtension objEmp = listEmpEx.First();
                                                    SpecificEmployeeData.RootObject root = new SpecificEmployeeData.RootObject();
                                                    root.EMPLOYEEID = objEmp.EMPLOYEEID;
                                                    root.USERNAME = objEmp.USERNAME;
                                                    root.PREFIX_TH = objEmp.PREFIX_TH;
                                                    root.PREFIX_ACADEMIC_TH = objEmp.PREFIX_ACADEMIC_TH;
                                                    root.FIRSTNAME_TH = objEmp.FIRSTNAME_TH;
                                                    root.LASTNAME_TH = objEmp.LASTNAME_TH;
                                                    root.PREFIX_EN = objEmp.PREFIX_EN;
                                                    root.PREFIX_ACADEMIC_EN = objEmp.PREFIX_ACADEMIC_EN;
                                                    root.FIRSTNAME_EN = objEmp.FIRSTNAME_EN;
                                                    root.LASTNAME_EN = objEmp.LASTNAME_EN;
                                                    root.BIRTHDATE = (objEmp.BIRTHDATE ?? DateTime.Now).ToString();
                                                    root.EMAIL = objEmp.EMAIL;
                                                    root.TELEPHONE = objEmp.TELEPHONE;
                                                    root.MANAGERID = objEmp.MANAGERID;





                                                    empNameTH = string.Format("{0}{1} {2}", root.PREFIX_TH, root.FIRSTNAME_TH, root.LASTNAME_TH);
                                                    empNameEN = string.Format("{0}{1} {2}", root.PREFIX_EN, root.FIRSTNAME_EN, root.LASTNAME_EN);

                                                    lblApproverName.Text = vs_Lang == "TH" ? empNameTH : empNameEN;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        ddlApproverDepartment.DataSource = empData.RESULT.ToList();
                                        ddlApproverDepartment.DataTextField = "DEPARTMENT_NAME_TH";
                                        ddlApproverDepartment.DataValueField = "DEPARTMENT_ID";
                                        ddlApproverDepartment.DataBind();
                                        ddlApproverDepartment.SelectedValue = DeptID.ToString();

                                        ddlApproverSubDepartment.DataSource = empData.RESULT.Where(x => x.DEPARTMENT_ID == ddlApproverDepartment.SelectedValue).ToList();
                                        ddlApproverSubDepartment.DataTextField = "SUBDEPARTMENT_NAME_TH";
                                        ddlApproverSubDepartment.DataValueField = "SUBDEPARTMENT_ID";
                                        ddlApproverSubDepartment.DataBind();
                                        ddlApproverSubDepartment.SelectedValue = SubDeptID.ToString();

                                        ddlApproverPosition.DataSource = empData.RESULT.Where(x => x.DEPARTMENT_ID == DeptID.ToString() && x.SUBDEPARTMENT_ID == ddlApproverSubDepartment.SelectedValue).ToList();
                                        ddlApproverPosition.DataTextField = "POSTION_NAME_TH";
                                        ddlApproverPosition.DataValueField = "POSITION_TD";
                                        ddlApproverPosition.DataBind();
                                        ddlApproverPosition.SelectedValue = PosID.ToString();
                                    }
                                    else
                                    {
                                        Extension.MessageBox(this.Page, "PosID is Null");
                                    }
                                }
                                else
                                {
                                    Extension.MessageBox(this.Page, "SubDeptID is Null");
                                }
                            }
                            else
                            {
                                Extension.MessageBox(this.Page, "DeptID is Null");
                            }

                        }
                        else
                        {
                            Extension.MessageBox(this.Page, "EmpID is Null");
                        }
                    }
                }
            }
        }
        protected void gvApprovelList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dt_SpecificApprover = vs_ApprovalList;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dt_SpecificApprover != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    dt_SpecificApprover.Rows[iRowIndex].Delete();
                    int iSequence = 1;
                    foreach (DataRow dr in dt_SpecificApprover.Rows)
                    {
                        dr["Sequence"] = iSequence;
                        iSequence++;
                    }
                }
                else if (e.CommandName == "ClickUP")
                {
                    if (iRowIndex > 0)
                    {
                        int CurrentSeq = Convert.ToInt32(dt_SpecificApprover.Rows[iRowIndex]["Sequence"].ToString());
                        dt_SpecificApprover.Rows[iRowIndex]["Sequence"] = CurrentSeq - 1;

                        int AboveCurrentSeq = Convert.ToInt32(dt_SpecificApprover.Rows[iRowIndex - 1]["Sequence"].ToString());
                        dt_SpecificApprover.Rows[iRowIndex - 1]["Sequence"] = AboveCurrentSeq + 1;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (e.CommandName == "ClickDown")
                {
                    if (iRowIndex < dt_SpecificApprover.Rows.Count - 1)
                    {
                        int CurrentSeq = Convert.ToInt32(dt_SpecificApprover.Rows[iRowIndex]["Sequence"].ToString());
                        dt_SpecificApprover.Rows[iRowIndex]["Sequence"] = CurrentSeq + 1;

                        int BelowCurrentSeq = Convert.ToInt32(dt_SpecificApprover.Rows[iRowIndex + 1]["Sequence"].ToString());
                        dt_SpecificApprover.Rows[iRowIndex + 1]["Sequence"] = BelowCurrentSeq - 1;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            DataView dv = dt_SpecificApprover.DefaultView;
            dv.Sort = "Sequence ASC";
            DataTable sortedDT = dv.ToTable();
            vs_ApprovalList = sortedDT;
            BinddtApproverViewStateToGridview();
        }
        //        protected void gvApprovelList_RowDataBound(object sender, GridViewRowEventArgs e)
        //        {
        //            if (e.Row.RowType == DataControlRowType.DataRow)
        //            {
        //                Label lblApproverName = (Label)e.Row.FindControl("lblApproverName");
        //                Label lblDepartment = (Label)e.Row.FindControl("lblApproverDepartment");
        //                Label lblSubDepartment = (Label)e.Row.FindControl("lblApproverSubDepartment");
        //                Label lblPosition = (Label)e.Row.FindControl("lblApproverPosition");
        //                //DropDownList ddlPermissionPosition = (DropDownList)e.Row.FindControl("ddl_PermissionPosition");
        //                if (lblApproverName != null && lblDepartment != null && lblSubDepartment != null && lblPosition != null)
        //                {
        //                    var EmpID = DataBinder.Eval(e.Row.DataItem, "EmpID");
        //                    var DeptID = DataBinder.Eval(e.Row.DataItem, "DepartmentID");
        //                    var SubDeptID = DataBinder.Eval(e.Row.DataItem, "SubDepartmentID");
        //                    var PosID = DataBinder.Eval(e.Row.DataItem, "PositionID");
        //                    if (EmpID != null && EmpID != DBNull.Value)
        //                    {
        //                        if (DeptID != null && DeptID != DBNull.Value)
        //                        {
        //                            if (SubDeptID != null && SubDeptID != DBNull.Value)
        //                            {
        //                                if (PosID != null && PosID != DBNull.Value)
        //                                {
        //                                    DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
        //                                    if (dtEmp != null)
        //                                    {
        //                                        DataView dvFilter = dtEmp.DefaultView;
        //                                        dvFilter.RowFilter = string.Format(@"EmployeeID = '{0}'
        //                                                                            AND Department_ID = '{1}'
        //                                                                            AND SubDepartment_ID = '{2}'
        //                                                                            AND Position_TD = '{3}'", EmpID.ToString(), DeptID.ToString(), SubDeptID.ToString(), PosID.ToString());
        //                                        if (dvFilter.ToTable().Rows.Count > 0)
        //                                        {
        //                                            lblApproverName.Text = dvFilter.ToTable().Rows[0]["EmployeeName_TH"].ToString();
        //                                            lblDepartment.Text = dvFilter.ToTable().Rows[0]["Department_Name_TH"].ToString();
        //                                            lblSubDepartment.Text = dvFilter.ToTable().Rows[0]["SubDepartment_Name_TH"].ToString();
        //                                            lblPosition.Text = dvFilter.ToTable().Rows[0]["Postion_Name_TH"].ToString();
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        Extension.MessageBox(this.Page, "PosID is Null");
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    Extension.MessageBox(this.Page, "SubDeptID is Null");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                Extension.MessageBox(this.Page, "DeptID is Null");
        //                            }

        //                        }
        //                        else
        //                        {
        //                            Extension.MessageBox(this.Page, "EmpID is Null");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        #endregion

        #region "Popup Search Permission"
        protected void BinddtPermissionViewStateToGridview()
        {
            gv_Permission.DataSource = vs_PermissionList;
            gv_Permission.DataBind();
        }
        protected void BinddtGroupMailViewStateToGridview()
        {
            gv_GroupEmail.DataSource = vs_GroupMailList;
            gv_GroupEmail.DataBind();
        }
        //        protected void gv_Permission_RowDataBound(object sender, GridViewRowEventArgs e)
        //        {
        //            if (e.Row.RowType == DataControlRowType.DataRow)
        //            {

        //                Label lblApproverName = (Label)e.Row.FindControl("lblPermissionName");
        //                Label lblDepartment = (Label)e.Row.FindControl("lblPermissionDepartment");
        //                Label lblSubDepartment = (Label)e.Row.FindControl("lblPermissionSubDepartment");
        //                Label lblPosition = (Label)e.Row.FindControl("lblPermissionPosition");
        //                //DropDownList ddlPermissionPosition = (DropDownList)e.Row.FindControl("ddl_PermissionPosition");
        //                if (lblApproverName != null && lblDepartment != null && lblSubDepartment != null && lblPosition != null)
        //                {
        //                    var EmpID = DataBinder.Eval(e.Row.DataItem, "EmpID");
        //                    var DeptID = DataBinder.Eval(e.Row.DataItem, "DepartmentID");
        //                    var SubDeptID = DataBinder.Eval(e.Row.DataItem, "SubDepartmentID");
        //                    var PosID = DataBinder.Eval(e.Row.DataItem, "PositionID");
        //                    if (EmpID != null && EmpID != DBNull.Value)
        //                    {
        //                        if (DeptID != null && DeptID != DBNull.Value)
        //                        {
        //                            if (SubDeptID != null && SubDeptID != DBNull.Value)
        //                            {
        //                                if (PosID != null && PosID != DBNull.Value)
        //                                {
        //                                    DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
        //                                    if (dtEmp != null)
        //                                    {
        //                                        DataView dvFilter = dtEmp.DefaultView;
        //                                        dvFilter.RowFilter = string.Format(@"EmployeeID = '{0}'
        //                                                                            AND Department_ID = '{1}'
        //                                                                            AND SubDepartment_ID = '{2}'
        //                                                                            AND Position_TD = '{3}'", EmpID.ToString(), DeptID.ToString(), SubDeptID.ToString(), PosID.ToString());
        //                                        if (dvFilter.ToTable().Rows.Count > 0)
        //                                        {

        //                                            lblApproverName.Text = dvFilter.ToTable().Rows[0]["EmployeeName_TH"].ToString();
        //                                            lblDepartment.Text = dvFilter.ToTable().Rows[0]["Department_Name_TH"].ToString();
        //                                            lblSubDepartment.Text = dvFilter.ToTable().Rows[0]["SubDepartment_Name_TH"].ToString();
        //                                            lblPosition.Text = dvFilter.ToTable().Rows[0]["Postion_Name_TH"].ToString();
        //                                            //ddlPermissionPosition.Items.Insert(0, new ListItem("--Please Select--", ""));
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        Extension.MessageBox(this.Page, "PosID is Null");
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    Extension.MessageBox(this.Page, "SubDeptID is Null");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                Extension.MessageBox(this.Page, "DeptID is Null");
        //                            }

        //                        }
        //                        else
        //                        {
        //                            Extension.MessageBox(this.Page, "EmpID is Null");
        //                        }
        //                    }
        //                }
        //            }
        //        }

        protected void ddl_PermissionDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            PermissionDDLChanged(sender, "DepartmentID");
        }
        protected void ddl_PermissionSubDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            PermissionDDLChanged(sender, "SubDepartmentID");
        }

        protected void ddl_PermissionPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            PermissionDDLChanged(sender, "PositionID");
        }
        protected void PermissionDDLChanged(object sender, string target)
        {
            DropDownList ddl = (DropDownList)sender;
            if (ddl != null)
            {
                Panel pnl = (Panel)ddl.Parent;
                if (pnl != null)
                {
                    Label lbl = (Label)pnl.FindControl("lblPermissionSequence");
                    if (lbl != null)
                    {
                        string sSeq = lbl.Text;
                        DataTable dt_SpecificPermission = vs_PermissionList;
                        if (dt_SpecificPermission != null)
                        {
                            foreach (DataRow dr in dt_SpecificPermission.Rows)
                            {
                                if (dr["Sequence"].ToString() == sSeq)
                                {
                                    dr[target] = Convert.ToInt32(ddl.SelectedValue);
                                    break;
                                }
                            }
                            vs_PermissionList = dt_SpecificPermission;
                            BinddtPermissionViewStateToGridview();
                        }
                    }
                }
            }
        }

        protected void gv_Permission_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Label lblApproverName = (Label)e.Row.FindControl("lblPermissionName");

                DropDownList ddlPermissionDepartment = (DropDownList)e.Row.FindControl("ddl_PermissionDepartment");
                DropDownList ddlPermissionSubDepartment = (DropDownList)e.Row.FindControl("ddl_PermissionSubDepartment");
                DropDownList ddlPermissionPosition = (DropDownList)e.Row.FindControl("ddl_PermissionPosition");
                if (ddlPermissionDepartment != null && ddlPermissionSubDepartment != null && ddlPermissionPosition != null && lblApproverName != null)
                {
                    object EmpID = DataBinder.Eval(e.Row.DataItem, "EmpID");
                    object DeptID = DataBinder.Eval(e.Row.DataItem, "DepartmentID");
                    object SubDeptID = DataBinder.Eval(e.Row.DataItem, "SubDepartmentID");
                    object PosID = DataBinder.Eval(e.Row.DataItem, "PositionID");
                    if (EmpID != null && EmpID != DBNull.Value)
                    {
                        if (DeptID != null && DeptID != DBNull.Value)
                        {
                            if (SubDeptID != null && SubDeptID != DBNull.Value)
                            {
                                if (PosID != null && PosID != DBNull.Value)
                                {
                                    SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(this.Page, EmpID.ToString());
                                    if (empData != null)
                                    {
                                        string empNameTH = string.Format("{0}{1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH, empData.LASTNAME_TH);
                                        string empNameEN = string.Format("{0}{1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN, empData.LASTNAME_EN);

                                        lblApproverName.Text = vs_Lang == "TH" ? empNameTH : empNameEN;

                                        ddlPermissionDepartment.DataSource = empData.RESULT.ToList();
                                        ddlPermissionDepartment.DataTextField = "DEPARTMENT_NAME_TH";
                                        ddlPermissionDepartment.DataValueField = "DEPARTMENT_ID";
                                        ddlPermissionDepartment.DataBind();
                                        ddlPermissionDepartment.SelectedValue = DeptID.ToString();

                                        ddlPermissionSubDepartment.DataSource = empData.RESULT.Where(x => x.DEPARTMENT_ID == ddlPermissionDepartment.SelectedValue).ToList();
                                        ddlPermissionSubDepartment.DataTextField = "SUBDEPARTMENT_NAME_TH";
                                        ddlPermissionSubDepartment.DataValueField = "SUBDEPARTMENT_ID";
                                        ddlPermissionSubDepartment.DataBind();
                                        ddlPermissionSubDepartment.SelectedValue = SubDeptID.ToString();

                                        ddlPermissionPosition.DataSource = empData.RESULT.Where(x => x.DEPARTMENT_ID == DeptID.ToString() && x.SUBDEPARTMENT_ID == ddlPermissionSubDepartment.SelectedValue).ToList();
                                        ddlPermissionPosition.DataTextField = "POSTION_NAME_TH";
                                        ddlPermissionPosition.DataValueField = "POSITION_TD";
                                        ddlPermissionPosition.DataBind();
                                        ddlPermissionPosition.SelectedValue = PosID.ToString();

                                    }
                                    else
                                    {
                                        Extension.MessageBox(this.Page, "PosID is Null");
                                    }
                                }
                                else
                                {
                                    Extension.MessageBox(this.Page, "SubDeptID is Null");
                                }
                            }
                            else
                            {
                                Extension.MessageBox(this.Page, "DeptID is Null");
                            }

                        }
                        else
                        {
                            Extension.MessageBox(this.Page, "EmpID is Null");
                        }
                    }
                }
            }
        }
        protected void gv_Permission_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dt_SpecificPermission = vs_PermissionList;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dt_SpecificPermission != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    dt_SpecificPermission.Rows[iRowIndex].Delete();
                    int iSequence = 1;
                    foreach (DataRow dr in dt_SpecificPermission.Rows)
                    {
                        dr["Sequence"] = iSequence;
                        iSequence++;
                    }
                }
            }
            DataView dv = dt_SpecificPermission.DefaultView;
            dv.Sort = "Sequence ASC";
            DataTable sortedDT = dv.ToTable();
            vs_PermissionList = sortedDT;
            BinddtPermissionViewStateToGridview();
        }

        #endregion

        #region "Popup Department"
        protected void lkbtnSearchDepartment_Click(object sender, EventArgs e)
        {
            loadDataDept();
        }
        protected void loadDataDept()
        {
            DataTable dtDep = Extension.GetDepartmentData(this.Page).Copy();
            if (!string.IsNullOrEmpty(txtSearch_Department.Text))
            {
                DataView dvFilter = dtDep.Copy().DefaultView;
                dvFilter.RowFilter = string.Format(@"
				(DEPARTMENT_ID LIKE '%{0}%'
				OR DEPARTMENT_CODE LIKE '%{0}%' 
				OR DEPARTMENT_NAME_TH LIKE '%{0}%' 
				OR DEPARTMENT_NAME_EN LIKE '%{0}%'
				OR PARENT_ID LIKE '%{0}%')
                AND PRIMARY = {1}", txtSearch_Department.Text, ddl_seachDeptLevel.SelectedValue);
                dtDep = dvFilter.ToTable();
            }
            DataView dvFilterLV = dtDep.Copy().DefaultView;
            dvFilterLV.RowFilter = string.Format(@"PRIMARY = {0}", ddl_seachDeptLevel.SelectedValue);
            dtDep = dvFilterLV.ToTable();

            gv_SearchDepartment.DataSource = dtDep;
            gv_SearchDepartment.DataBind();
        }
        protected void btn_searchDepartment_Click(object sender, EventArgs e)
        {
            LinkButton linkBtn = new LinkButton();
            Button btn = new Button();

            if (sender is LinkButton)
            {
                linkBtn = (LinkButton)sender;
            }
            else if (sender is Button)
            {
                btn = (Button)sender;
            }
            else
            {
                return;
            }
            switch (sender is LinkButton ? linkBtn.ID : btn.ID)
            {
                case "btn_ToDeptAdd":
                    vs_DeptTarget = "ToDepartment";
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hideToDepartment", "$('#toDepartmentModal').modal('hide');", true);
                    break;
                case "btn_addGroupEmail":
                    vs_DeptTarget = "Permission";
                    break;
                default:
                    vs_DeptTarget = ""; break;
            }
            txtSearch_Department.Text = "";
            ddl_seachDeptLevel.SelectedValue = "1";
            loadDataDept();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popSearchToModal1", "$('#searchDepartmentModal').modal('show');", true);
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "callTag", "callTag();", true);
        }

        protected void gv_SearchDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                e.Row.ToolTip = "Click for selecting this row.";
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_SearchDepartment, "Select$" + e.Row.RowIndex);

                Label lbl_DepartmentLevel = (Label)e.Row.FindControl("lbl_DepartmentLevel");
                object objDocLv = DataBinder.Eval(e.Row.DataItem, "PRIMARY");
                if (lbl_DepartmentLevel != null)
                    if (objDocLv != null && objDocLv != DBNull.Value) lbl_DepartmentLevel.Text = objDocLv.ToString() == "1" ? "1" : "2";

                Label lbl_DepartmentAcronym = (Label)e.Row.FindControl("lbl_DepartmentAcronym");
                if (lbl_DepartmentAcronym != null)
                {
                    object objAcronymTH = DataBinder.Eval(e.Row.DataItem, "DEPARTMENT_ACRONYM_TH");
                    object objAcronymEN = DataBinder.Eval(e.Row.DataItem, "DEPARTMENT_ACRONYM_EN");
                    if (objAcronymEN != null && objAcronymEN != DBNull.Value && objAcronymTH != null && objAcronymTH != DBNull.Value)
                    {
                        string acroTH = objAcronymTH.ToString().ToLower().Equals("null") ? "-" : objAcronymTH.ToString();
                        string acroEN = objAcronymEN.ToString().ToLower().Equals("null") ? "-" : objAcronymEN.ToString();
                        lbl_DepartmentAcronym.Text = string.Format("{0}/{1}", acroTH, acroEN);
                    }
                }
            }

        }

        protected void gv_SearchDepartment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_SearchDepartment.PageIndex = e.NewPageIndex;
            loadDataDept();
        }

        protected void btn_resetDepartment_Click(object sender, EventArgs e)
        {
            txtSearch_Department.Text = "";
            ddl_seachDeptLevel.SelectedValue = "1";
            loadDataDept();
        }

        protected void gv_SearchDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow gv_row in gv_SearchDepartment.Rows)
            {
                if (gv_row.RowIndex == gv_SearchDepartment.SelectedIndex)
                {
                    HiddenField hdnDeptID = (HiddenField)gv_row.FindControl("hdnDepartmentID");
                    string sDepID = hdnDeptID.Value;
                    if (!string.IsNullOrWhiteSpace(sDepID))
                    {
                        DataView dvFilter = Extension.GetDepartmentData(this.Page).Copy().DefaultView;
                        dvFilter.RowFilter = string.Format("DEPARTMENT_ID = '{0}'", sDepID);
                        DataTable dtDep = dvFilter.ToTable();
                        if (dtDep.Rows.Count > 0)
                        {
                            switch (vs_DeptTarget)
                            {
                                case "ToDepartment":

                                    DataTable dt = vs_ToDepartment;
                                    if (vs_ToDepartment != null && vs_ToDepartment.Rows.Count > 0)
                                    {
                                        foreach (DataRow item in vs_ToDepartment.Rows)
                                        {
                                            if (item["DeptID"].ToString().Equals(sDepID))
                                            {
                                                Extension.MessageBox(Page, "มีหน่วยงานที่เลือกไว้แล้ว");
                                                return;
                                            }
                                        }
                                    }

                                    if (string.IsNullOrWhiteSpace(txt_ToDepartment.Text))
                                    {
                                        txt_ToDepartment.Text = dtDep.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                                        hdn_ToDepartment.Text = dtDep.Rows[0]["DEPARTMENT_ID"].ToString();
                                    }
                                    else
                                    {
                                        txt_ToDepartment.Text = string.Format("{0}, {1}", txt_ToDepartment.Text,
                                            dtDep.Rows[0]["DEPARTMENT_NAME_TH"]);
                                        hdn_ToDepartment.Text = string.Format("{0},{1}", hdn_ToDepartment.Text,
                                            dtDep.Rows[0]["DEPARTMENT_ID"].ToString());
                                    }

                                    DataRow dr = dt.NewRow();
                                    dr["Sequence"] = dt.Rows.Count + 1;
                                    dr["DeptID"] = sDepID;
                                    dr["DEPARTMENT_CODE"] = dtDep.Rows[0]["DEPARTMENT_CODE"].ToString(); ;
                                    dr["DEPARTMENT_NAME_TH"] = dtDep.Rows[0]["DEPARTMENT_NAME_TH"].ToString(); ;
                                    dr["DEPARTMENT_NAME_EN"] = dtDep.Rows[0]["DEPARTMENT_NAME_EN"].ToString(); ;
                                    dt.Rows.Add(dr);
                                    vs_ToDepartment = dt;
                                    BindingVStoToDepartment();

                                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popToDepartmentModal1", "$('#toDepartmentModal').modal('show');", true);

                                    break;
                                case "Permission":
                                    DataTable dtPermission = vs_GroupMailList;
                                    DataRow drPermission = dtPermission.NewRow();
                                    drPermission["Sequence"] = dtPermission.Rows.Count + 1;
                                    drPermission["DepartmentID"] = dtDep.Rows[0]["DEPARTMENT_ID"];
                                    drPermission["DepartmentName"] = dtDep.Rows[0]["DEPARTMENT_NAME_TH"];
                                    drPermission["DepartmentGroupMail"] = dtDep.Rows[0]["DEPARTMENT_GROUPMAIL"]; ;
                                    dtPermission.Rows.Add(drPermission);
                                    vs_GroupMailList = dtPermission;
                                    BinddtGroupMailViewStateToGridview();
                                    break;

                            }

                        }
                    }

                }
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchEmpModal", "$('#searchDepartmentModal').modal('hide');", true);
        }

        protected void btn_Close_popup_SearchDepartment_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchToModal1", "$('#searchDepartmentModal').modal('hide');", true);
        }
        protected void ddl_seachDeptLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDataDept();
        }
        #endregion

        #region "Popup Assign"

        private void BinddtAssignViewStateToGridview()
        {
            gv_Assign.DataSource = vs_Assign;
            gv_Assign.DataBind();
        }


        protected void gv_Assign_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].ColumnSpan = e.Row.Cells.Count;
                for (int i = e.Row.Cells.Count - 1; i > 0; i--)
                {
                    e.Row.Cells.RemoveAt(i);
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                #region | Employee Name |
                Label lblAssignName = (Label)e.Row.FindControl("lblEmpName");
                object EmpID = DataBinder.Eval(e.Row.DataItem, "EmpID");
                object DeptID = DataBinder.Eval(e.Row.DataItem, "DepartmentID");
                if (lblAssignName != null)
                {
                    if (EmpID != null && EmpID != DBNull.Value)
                    {
                        SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(this.Page, EmpID.ToString());
                        if (empData != null)
                        {
                            string empNameTH = string.Format("{0}{1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH, empData.LASTNAME_TH);
                            string empNameEN = string.Format("{0}{1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN, empData.LASTNAME_EN);

                            lblAssignName.Text = vs_Lang == "TH" ? empNameTH : empNameEN;
                        }
                    }
                    else
                    {
                        Extension.MessageBox(this.Page, "EmpID is Null");
                    }
                }
                #endregion

                #region | Department |

                DropDownList ddl_assignDepartment = (DropDownList)e.Row.FindControl("ddl_Department");
                HiddenField hdnDeptID = (HiddenField)e.Row.FindControl("hdnDeptID");

                if (ddl_assignDepartment != null && hdnDeptID != null)
                {
                    if (EmpID != null && EmpID != DBNull.Value)
                    {
                        SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(this.Page, EmpID.ToString());
                        if (empData != null)
                        {
                            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                            //Add Employee Extension
                            List<TRNEmployeeExtension> listEmpEx = db.TRNEmployeeExtensions.Where(x => x.EMPLOYEEID == EmpID.ToString()).ToList();
                            if (listEmpEx.Count > 0)
                            {
                                foreach (TRNEmployeeExtension item in listEmpEx)
                                {
                                    SpecificEmployeeData.RESULT result = new SpecificEmployeeData.RESULT();
                                    result.DEPARTMENT_ID = item.DEPARTMENT_ID;
                                    result.DEPARTMENT_NAME_EN = item.DEPARTMENT_NAME_EN;
                                    result.DEPARTMENT_NAME_TH = item.DEPARTMENT_NAME_TH;
                                    result.SUBDEPARTMENT_ID = item.SUBDEPARTMENT_ID;
                                    result.SUBDEPARTMENT_NAME_EN = item.SUBDEPARTMENT_NAME_EN;
                                    result.SUBDEPARTMENT_NAME_TH = item.SUBDEPARTMENT_NAME_TH;
                                    result.POSITION_TD = item.POSITION_TD;
                                    result.POSTION_NAME_EN = item.POSTION_NAME_EN;
                                    result.POSTION_NAME_TH = item.POSTION_NAME_TH;
                                    result.PRIMARY_POSITION = item.PRIMARY_POSITION;
                                    empData.RESULT.Add(result);
                                }
                            }

                            List<SpecificEmployeeData.RESULT> listDept = empData.RESULT.ToList();
                            ddl_assignDepartment.DataSource = listDept;
                            ddl_assignDepartment.DataValueField = "DEPARTMENT_ID";
                            ddl_assignDepartment.DataTextField = vs_Lang == "TH"
                                ? "DEPARTMENT_NAME_TH"
                                : "DEPARTMENT_NAME_EN";
                            ddl_assignDepartment.DataBind();

                            hdnDeptID.Value =
                            ddl_assignDepartment.SelectedValue = hdnDeptID.Value;
                        }
                    }
                }

                #endregion
            }
        }

        protected void gv_Assign_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gv_Assign_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_Assign.PageIndex = e.NewPageIndex;
            BinddtAssignViewStateToGridview();
        }

        protected void ddl_AssignDepartment_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (DataRow row in vs_Assign.Rows)
            {

            }
        }
        protected void btnCancelAssign_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "HideAssign", "$('#searchAssign').modal('hide');", true);
        }
        protected void gv_Assign_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dt_Assign = vs_Assign;
            if (dt_Assign != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
                    dt_Assign.Rows[iRowIndex].Delete();
                    int iSequence = 1;
                    foreach (DataRow dr in dt_Assign.Rows)
                    {
                        dr["Sequence"] = iSequence;
                        iSequence++;
                    }
                    vs_Assign = dt_Assign;
                    gv_Assign.DataSource = dt_Assign;
                    gv_Assign.DataBind();

                }
            }
        }
        protected void lkbtnSearchAssign_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "HideAssign", "$('#searchAssign').modal('hide');", true);
            OpenPopup(sender, e);
        }
        #endregion

        #region | Popup To Department |
        protected void open_ToDepartmentPopup(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popToDepartmentModal1", "$('#toDepartmentModal').modal('show');", true);

        }
        protected void BindingVStoToDepartment()
        {
            gv_ToDepartment.DataSource = vs_ToDepartment;
            gv_ToDepartment.DataBind();
        }
        protected void gv_ToDepartment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dt_ToDepartment = vs_ToDepartment;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dt_ToDepartment != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    dt_ToDepartment.Rows[iRowIndex].Delete();
                    txt_ToDepartment.Text = "";
                    hdn_ToDepartment.Text = "";
                    foreach (DataRow dr in dt_ToDepartment.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(txt_ToDepartment.Text))
                        {
                            txt_ToDepartment.Text = dr["DEPARTMENT_NAME_TH"].ToString();
                            hdn_ToDepartment.Text = dr["DeptID"].ToString();
                        }
                        else
                        {
                            txt_ToDepartment.Text = string.Format("{0}, {1}", txt_ToDepartment.Text,
                                dr["DEPARTMENT_NAME_TH"]);
                            hdn_ToDepartment.Text = string.Format("{0},{1}", hdn_ToDepartment.Text,
                                dr["DeptID"].ToString());
                        }
                    }
                    int iSequence = 1;
                    foreach (DataRow dr in dt_ToDepartment.Rows)
                    {
                        dr["Sequence"] = iSequence;
                        iSequence++;
                    }
                }
            }
            DataView dv = dt_ToDepartment.DefaultView;
            dv.Sort = "Sequence ASC";
            DataTable sortedDT = dv.ToTable();
            vs_ToDepartment = sortedDT;
            BindingVStoToDepartment();
        }

        protected void gv_ToDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].ColumnSpan = e.Row.Cells.Count;
                for (int i = e.Row.Cells.Count - 1; i > 0; i--)
                {
                    e.Row.Cells.RemoveAt(i);
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                #region | Department |

                Label DepartmentNameTH = (Label)e.Row.FindControl("lblDepNameTH");
                Label DepartmentNameEN = (Label)e.Row.FindControl("lblDepNameEN");
                Label DepartmentCode = (Label)e.Row.FindControl("lblDepCode");
                HiddenField hdnDeptID = (HiddenField)e.Row.FindControl("hdnDeptID");

                if (DepartmentCode != null && DepartmentNameTH != null && DepartmentNameEN != null && hdnDeptID != null)
                {
                    DataTable dept = Extension.GetSpecificDepartmentDataFromTemp(Page, hdnDeptID.Value);
                    if (!dept.DataTableIsNullOrEmpty())
                    {
                        DepartmentNameEN.Text = dept.Rows[0]["DEPARTMENT_NAME_EN"].ToString();
                        DepartmentNameTH.Text = dept.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                        DepartmentCode.Text = dept.Rows[0]["DEPARTMENT_CODE"].ToString();
                    }
                }

                #endregion
            }
        }

        protected void gv_ToDepartment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_ToDepartment.PageIndex = e.NewPageIndex;
            BindingVStoToDepartment();
        }

        protected void btn_ToDeptClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "HideToDepartment", "$('#toDepartmentModal').modal('hide');", true);
        }
        #endregion
        #endregion

        #region | Validation |
        private bool IsPassValidate(ref string sMessage)
        {
            string invalidTxt = "invalid-tb";
            #region | Type |
            if (rdb_Type.SelectedIndex == 0)
            {
                sMessage = "กรุณาเลือกประเภท";
                rdb_Type.Focus();
                if (!rdb_Type.CssClass.Contains(invalidTxt))
                {
                    rdb_Type.CssClass = string.Format("{0} {1}", rdb_Type.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                rdb_Type.CssClass = rdb_Type.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Category |
            if (rdb_Category.SelectedIndex == 0)
            {
                sMessage = "กรุณาเลือกหมวดหมู่";
                rdb_Category.Focus();
                if (!rdb_Category.CssClass.Contains(invalidTxt))
                {
                    rdb_Category.CssClass = string.Format("{0} {1}", rdb_Category.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                rdb_Category.CssClass = rdb_Category.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | DocType |
            if (ddl_DocType.SelectedIndex == 0)
            {
                sMessage = "กรุณาเลือกประเภทเอกสาร";
                ddl_DocType.Focus();
                if (!ddl_DocType.CssClass.Contains(invalidTxt))
                {
                    ddl_DocType.CssClass = string.Format("{0} {1}", ddl_DocType.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                ddl_DocType.CssClass = ddl_DocType.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Title |
            if (string.IsNullOrEmpty(txt_title.Text))
            {
                sMessage = "กรุณากรอกข้อมูล: ชื่อเรื่อง";
                txt_title.Focus();
                if (!txt_title.CssClass.Contains(invalidTxt))
                {
                    txt_title.CssClass = string.Format("{0} {1}", txt_title.CssClass, invalidTxt);
                }
                return false;
            }
            else if (txt_title.Text.Count() > 500)
            {
                sMessage = "ชื่อเรื่อง ยาวเกินที่กำหนด";
                txt_title.Focus();
                if (!txt_title.CssClass.Contains(invalidTxt))
                {
                    txt_title.CssClass = string.Format("{0} {1}", txt_title.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                txt_title.CssClass = txt_title.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Description |
            if (!string.IsNullOrWhiteSpace(txt_DocDescription.Text) && txt_DocDescription.Text.Count() > 500)
            {
                sMessage = "รายละเอียดของเอกสาร(โดยย่อ) ยาวเกินที่กำหนด";
                txt_DocDescription.Focus();
                if (!txt_DocDescription.CssClass.Contains(invalidTxt))
                {
                    txt_DocDescription.CssClass = string.Format("{0} {1}", txt_DocDescription.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                txt_DocDescription.CssClass = txt_DocDescription.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Receive Doc No |
            if (!string.IsNullOrWhiteSpace(txt_RecieveDocNo.Text))
            {
                sMessage = CheckReceiveDocumentNoUsed(txt_RecieveDocNo.Text);
                if (!string.IsNullOrWhiteSpace(sMessage))
                {
                    txt_RecieveDocNo.Text = "";
                    return false;
                }
            }
            else if (txt_RecieveDocNo.Text.Count() > 100)
            {
                sMessage = "เลขที่หนังสือรับจากภายนอก ยาวเกินที่กำหนด";
                txt_RecieveDocNo.Focus();
                if (!txt_RecieveDocNo.CssClass.Contains(invalidTxt))
                {
                    txt_RecieveDocNo.CssClass = string.Format("{0} {1}", txt_RecieveDocNo.CssClass, invalidTxt);
                }
                return false;
            }
            #endregion
            #region | Document Source |
            if (!string.IsNullOrWhiteSpace(txt_Source.Text) && txt_Source.Text.Count() > 100)
            {
                sMessage = "แหล่งที่มาของเอกสาร ยาวเกินที่กำหนด";
                txt_Source.Focus();
                if (!txt_Source.CssClass.Contains(invalidTxt))
                {
                    txt_Source.CssClass = string.Format("{0} {1}", txt_Source.CssClass, invalidTxt);
                }
                return false;
            }
            #endregion
            #region | TXT Attachment |
            if (!string.IsNullOrWhiteSpace(txt_Attachment.Text) && txt_Attachment.Text.Count() > 500)
            {
                sMessage = "สิ่งที่แนบมาด้วย ยาวเกินที่กำหนด";
                txt_Attachment.Focus();
                if (!txt_Attachment.CssClass.Contains(invalidTxt))
                {
                    txt_Attachment.CssClass = string.Format("{0} {1}", txt_Attachment.CssClass, invalidTxt);
                }
                return false;
            }
            #endregion
            #region | ToDepartment |
            if (string.IsNullOrWhiteSpace(txt_ToDepartment.Text))
            {
                if (!(ddl_DocType.SelectedValue == "Im" || ddl_DocType.SelectedValue == "Other" || ddl_DocType.SelectedValue == "Ex" || ddl_DocType.SelectedValue == "ExEN"))
                {
                    sMessage = "กรุณาเลือกหน่วยงานรับเรื่อง";
                    txt_ToDepartment.Focus();
                    if (!txt_ToDepartment.CssClass.Contains(invalidTxt))
                    {
                        txt_ToDepartment.CssClass = string.Format("{0} {1}", txt_ToDepartment.CssClass, invalidTxt);
                    }
                    return false;
                }
            }
            else
            {
                txt_ToDepartment.CssClass = txt_ToDepartment.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Approver |
            if (rdb_Type.SelectedValue.Equals("Submit") && (vs_ApprovalList == null || vs_ApprovalList.Rows.Count == 0))
            {
                sMessage = "กรุณาระบุ Approver อย่างน้อย 1 คน";
                Btn_Add_Spec_Approver.Focus();
                return false;
            }
            #endregion
            #region | DOA |
            if (rdb_Type.SelectedValue.Equals("Submit") && (vs_ApprovalList.Rows.Count > 0))
            {
                if (chk_DOA.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txt_Amount.Text))
                    {
                        sMessage = "กรุณาใส่จำนวนเงิน";
                        txt_Amount.Focus();
                        if (!txt_Amount.CssClass.Contains(invalidTxt))
                        {
                            txt_Amount.CssClass = string.Format("{0} {1}", txt_Amount.CssClass, invalidTxt);
                        }
                        return false;
                    }
                    else
                    {
                        txt_Amount.CssClass = txt_Amount.CssClass.Replace(invalidTxt, "");
                    }
                    foreach (DataRow item in vs_ApprovalList.Rows)
                    {
                        if (!Extension.isApprovalDOALvSupport(txt_Amount.Text, item["PositionID"].ToString()))
                        {
                            sMessage = string.Format("{0} ไม่มีอำนาจอนุมัติวงเงินจำนวน {1} บาท", item["EmployeeName"].ToString(), txt_Amount.Text);
                            return false;
                        }
                    }
                }
            }
            #endregion
            #region | PDF Attachment |
            if (string.IsNullOrWhiteSpace(vs_DocPDFPath))
            {
                if (ddl_DocType.SelectedValue == "Im" && rdb_Type.SelectedValue.Equals("Submit"))
                {
                    sMessage = "กรุณาแนบ Document File.";
                    doc_upload.Focus();
                    if (!doc_upload.CssClass.Contains(invalidTxt))
                    {
                        doc_upload.CssClass = string.Format("{0} {1}", doc_upload.CssClass, invalidTxt);
                    }
                    return false;
                }
            }
            else if (MimeMapping.GetMimeMapping(vs_DocPDFPath) != "application/pdf")
            {
                sMessage = "Only files of type PDF is supported. Uploaded File Type: " + MimeMapping.GetMimeMapping(vs_DocPDFPath);
                doc_upload.Focus();
                if (!doc_upload.CssClass.Contains(invalidTxt))
                {
                    doc_upload.CssClass = string.Format("{0} {1}", doc_upload.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                doc_upload.CssClass = doc_upload.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Permission |
            if (ddl_Permission.SelectedValue == "Secret")
            {
                if (vs_PermissionList.DataTableIsNullOrEmpty())
                {
                    sMessage = "กรุณาระบุ Permission อย่างน้อย 1 คน";
                    btn_addPermission.Focus();
                    return false;
                }
            }
            #endregion
            #region | Word Template |
            if (btn_DownloadTemplate.Text == "Download Template" && btn_DownloadTemplate.Visible)
            {
                sMessage = "Please Download Template.";
                btn_DownloadTemplate.Focus();
                return false;
            }
            #endregion
            return true;
        }
        private bool IsWordPassValidate(ref string sMessage)
        {
            string invalidTxt = "invalid-tb";
            #region | Type |
            if (rdb_Type.SelectedIndex == 0)
            {
                sMessage = "กรุณาเลือกประเภท";
                rdb_Type.Focus();
                if (!rdb_Type.CssClass.Contains(invalidTxt))
                {
                    rdb_Type.CssClass = string.Format("{0} {1}", rdb_Type.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                rdb_Type.CssClass = rdb_Type.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Category |
            if (rdb_Category.SelectedIndex == 0)
            {
                sMessage = "กรุณาเลือกหมวดหมู่";
                rdb_Category.Focus();
                if (!rdb_Category.CssClass.Contains(invalidTxt))
                {
                    rdb_Category.CssClass = string.Format("{0} {1}", rdb_Category.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                rdb_Category.CssClass = rdb_Category.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Title |
            if (string.IsNullOrEmpty(txt_title.Text))
            {
                sMessage = "กรุณากรอกข้อมูล: ชื่อเรื่อง";
                txt_title.Focus();
                if (!txt_title.CssClass.Contains(invalidTxt))
                {
                    txt_title.CssClass = string.Format("{0} {1}", txt_title.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                txt_title.CssClass = txt_title.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | Approver |
            if (rdb_Type.SelectedValue.Equals("Submit") && (vs_ApprovalList == null || vs_ApprovalList.Rows.Count == 0))
            {
                sMessage = "กรุณาระบุ Approver อย่างน้อย 1 คน";
                Btn_Add_Spec_Approver.Focus();
                return false;
            }
            #endregion
            #region | DocType |
            if (ddl_DocType.SelectedIndex == 0)
            {
                sMessage = "กรุณาเลือกประเภทเอกสาร";
                ddl_DocType.Focus();
                if (!ddl_DocType.CssClass.Contains(invalidTxt))
                {
                    ddl_DocType.CssClass = string.Format("{0} {1}", ddl_DocType.CssClass, invalidTxt);
                }
                return false;
            }
            else
            {
                ddl_DocType.CssClass = ddl_DocType.CssClass.Replace(invalidTxt, "");
            }
            #endregion
            #region | DOA |
            if (rdb_Type.SelectedValue.Equals("Submit") && (vs_ApprovalList.Rows.Count > 0))
            {
                if (chk_DOA.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txt_Amount.Text))
                    {
                        sMessage = "กรุณาใส่จำนวนเงิน";
                        txt_Amount.Focus();
                        if (!txt_Amount.CssClass.Contains(invalidTxt))
                        {
                            txt_Amount.CssClass = string.Format("{0} {1}", txt_Amount.CssClass, invalidTxt);
                        }
                        return false;
                    }
                    else
                    {
                        txt_Amount.CssClass = txt_Amount.CssClass.Replace(invalidTxt, "");
                    }
                    foreach (DataRow item in vs_ApprovalList.Rows)
                    {
                        if (!Extension.isApprovalDOALvSupport(txt_Amount.Text, item["PositionID"].ToString()))
                        {
                            sMessage = string.Format("{0} ไม่มีอำนาจอนุมัติวงเงินจำนวน {1} บาท", item["EmployeeName"].ToString(), txt_Amount.Text);
                            return false;
                        }
                    }
                }
            }
            #endregion
            return true;
        }
        #endregion

        #region | User Authorization |

        private void UserAuthorize()
        {
            string tempCurrentUserID = vs_CurrentUserID;
            string tempCurrentUserDeptID = vs_CurrentUserDepID;

            #region | Check Delegation |
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(vs_ConnectionString);
            TRNDocument objDocument = db.TRNDocuments.SingleOrDefault(x => x.DocID == Convert.ToInt32(vs_PK));
            List<TRNAssign> listAssigns = db.TRNAssigns.Where(x => x.DocID == Convert.ToInt32(objDocument.DocID)).ToList();
            List<v_TRNDelegateDetail> listDelegate = new List<v_TRNDelegateDetail>();
            listDelegate = db.v_TRNDelegateDetails
                .Where(x => x.DelegateToID == vs_CurrentUserID).ToList();
            listDelegate = listDelegate
                .Where(x => objDocument.WaitingFor.Replace(" ", "").Split(',').Contains(x.ApproverID)).ToList();
            listDelegate = listDelegate
                .Where(x => x.DepartmentID == objDocument.WaitingForDeptID).ToList();
            listDelegate = listDelegate
                .Where(x => Convert.ToBoolean(x.IsActive)).ToList();
            listDelegate = listDelegate.Where(x =>
                {
                    bool i = Convert.ToBoolean(x.IsByDocType);
                    return i ? x.DocType == objDocument.DocTypeCode : x.DocID == vs_PK;
                }
                ).ToList();
            listDelegate = listDelegate
                .Where(x => (DateTime.Now >= x.DateFrom && (x.DateTo == null || DateTime.Now <= x.DateTo))).ToList();

            #endregion

            vs_WaitingFor = objDocument.WaitingFor;
            vs_isCentreAdmin = false;
            vs_Role = "Viewer";
            vs_isLastApproval = false;
            vs_isAssignable = false;
            vs_isDelegate = false;
            panel_Container.Visible = true;

            if (objDocument != null)
            {
                if (listDelegate != null && listDelegate.Count > 0)
                {
                    vs_isDelegate = true;
                    tempCurrentUserID = objDocument.WaitingFor;
                    tempCurrentUserDeptID = objDocument.WaitingForDeptID;
                }

                //Check AdminCentre
                DataTable dtCentreAdmin = Extension.GetDataTable("MstAdminCentre");
                if (!dtCentreAdmin.DataTableIsNullOrEmpty())
                {
                    foreach (DataRow row in dtCentreAdmin.Rows)
                    {
                        //if (row["EmpID"].ToString() == vs_CurrentUserID)
                        if (row["UserName"].ToString().Trim().Split(',').Contains(vs_CurrentUserLoginName) || row["EmpID"].ToString().Trim().Split(',').Contains(vs_CurrentUserID))
                        {
                            if (row["DeptID"].ToString() == ddl_RequestorDepartment.SelectedValue)
                            {
                                vs_isCentreAdmin = true;
                            }
                        }
                    }
                }

                if (!vs_PermissionList.DataTableIsNullOrEmpty())
                {
                    DataTable dtPer = vs_PermissionList;
                    foreach (DataRow dr in dtPer.Rows)
                    {
                        if (dr["EmpID"].ToString() == tempCurrentUserID)
                        {
                            vs_Role = "Permission";
                            break;
                        }
                    }
                }

                if (lbl_Status.Text.Equals(Extension._Completed))
                {
                    if (listAssigns != null && listAssigns.Count > 0)
                    {
                        foreach (TRNAssign item in listAssigns)
                        {
                            if (item.AssignToID == tempCurrentUserID)
                            {
                                vs_Role = "Assign";
                                vs_isAssignable = true;
                            }
                        }
                    }

                    if (!(vs_Role == "Approvor" || vs_Role == "Assign"))
                    {
                        if (tempCurrentUserID == lbl_RequestorID.Text) vs_Role = "Requestor";
                        if (tempCurrentUserID == lbl_CreatorID.Text) vs_Role = "Creator";
                        vs_isAssignable = true;
                    }
                    //if (vs_isCentreAdmin)
                    //{
                    //    vs_Role = "AdminCentre";
                    //}

                }
                else if (lbl_Status.Text.Equals(Extension._WaitForAdminCentre) && objDocument.WaitingFor.Trim().Split(',').Contains(tempCurrentUserID))
                {
                    vs_Role = "AdminCentre";
                }
                else
                {
                    if (objDocument.WaitingFor.Trim(' ').Split(',').Contains(tempCurrentUserID)
                    && (objDocument.Status != Extension._Draft
                    && objDocument.Status != Extension._Rework
                    && objDocument.Status != Extension._WaitForRequestorReview
                    && objDocument.Status != Extension._WaitForComment))
                    {
                        vs_Role = "Approvor";
                    }
                    else if (objDocument.WaitingFor.Trim(' ').Split(',').Contains(tempCurrentUserID)
                       && (objDocument.Status == Extension._WaitForComment))
                    {
                        vs_Role = "Advisor";
                    }
                    else if (tempCurrentUserID == objDocument.CreatorID ||
                       objDocument.Status.Equals(Extension._NewRequest))
                    {
                        vs_Role = "Creator";
                        if (tempCurrentUserID == objDocument.RequestorID &&
                            (objDocument.Status != Extension._Draft && objDocument.Status != Extension._Rework))
                        {
                            vs_Role = "Requestor";
                        }
                    }
                    else if (tempCurrentUserID == objDocument.RequestorID)
                    {
                        vs_Role = "Requestor";
                    }
                    else
                    {
                        vs_Role = "Viewer";
                    }
                }

                if (!vs_ApprovalList.DataTableIsNullOrEmpty() && vs_Role != "Advisor" && vs_Status != Extension._Draft && vs_Status != Extension._WaitForRequestorReview && vs_Status != Extension._Rework && vs_Role != "AdminCentre")//Ken && vs_Role != "AdminCentre"
                {
                    DataTable dtApp = vs_ApprovalList;
                    for (int index = 0; index < dtApp.Rows.Count; index++)
                    {
                        DataRow dr = dtApp.Rows[index];
                        if (dr["EmpID"].ToString() == tempCurrentUserID)
                        {
                            if (vs_Role == "Approvor")
                            {
                                //vs_Role = "Approvor";
                                if (index == dtApp.Rows.Count - 1)
                                {
                                    vs_isLastApproval = true;
                                }
                                break;
                            }
                            else
                            {
                                vs_Role = "Assign";
                                vs_isAssignable = true;
                                break;
                            }
                        }
                    }
                }

            }

            if (vs_Role == "Viewer" && (vs_Status.Equals(Extension._Completed) || vs_Status.Equals(Extension._Cancelled)))
            {
                string spGroup = "All_ViewDocument";
                if (!string.IsNullOrEmpty(spGroup))
                {
                    List<string> userList = Rule.SharedRules.GetAllUserInGroup(spGroup);
                    DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
                    if (!dtEmp.DataTableIsNullOrEmpty())
                    {
                        foreach (string user in userList)
                        {
                            if (tempCurrentUserID == SharedRules.FindUserID(user, this.Page))
                            {
                                vs_Role = "Permission";
                                break;
                            }
                        }
                    }
                }
            }
            //check for RequestCancel
            #region | Request Cancel |

            if (vs_Role == "Approvor" && vs_Status.Equals(Extension._RequestCancel))
            {
                vs_Role = "Manager";
            }
            #endregion
            //check for ITADMIN
            #region | IT Admin |
            if (vs_Role == "Viewer" || vs_Role == "Permission")
            {
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
                #endregion
            }


            if (vs_Role == "Viewer" && ddl_Permission.SelectedValue != "Public")
            //&& rdb_Category.SelectedValue != "centre")
            {
                panel_Container.Visible = false;
                Extension.MessageBox(this.Page, "ท่านไม่มีสิทธิ์เข้าถึงข้อมูลให้ติดต่อไปยังผู้จัดทำเอกสาร",
                    "Worklist.aspx");
            }
            else if (vs_Role == "Viewer"
              && rdb_Category.SelectedValue == "internal"
              && ddl_Permission.SelectedValue == "Public"
              && vs_CurrentUser_DepID != hdn_FromDepartmentID.Text)
            {
                //สิทธิ์คือเห็นเอกสารทุกประเภทที่เป็นภายในหน่วยงานตนเองค่ะ ยกเว้นเอกสารที่เซ็ตสิทธิ์ความลับ เพิ่ม 30-04-2563 
                panel_Container.Visible = false;
                //string allertdate = string.Format("vs_CurrentUserDepID = {0} | FromDepartmentID = {1}", vs_CurrentUser_DepID, hdn_FromDepartmentID.Text);
                Extension.MessageBox(this.Page, "ท่านไม่มีสิทธิ์เข้าถึงข้อมูลให้ติดต่อไปยังผู้จัดทำเอกสาร",
                    "Worklist.aspx");
            }

            bool isSave = rdb_Type.SelectedValue.Equals("Save");
            bool isCompleted = lbl_Status.Text.ToLower().Equals("completed");
            HideAllButton();


            switch (vs_Role)
            {
                case "Creator": sUserCreator(isSave); break;
                case "Requestor": sUserRequestor(isSave); break;
                case "Approvor": sUserApprover(isSave); break;
                case "Advisor": sUserAdvisor(isSave); break;
                case "Viewer": sUserViewer(isSave); break;
                case "Assign": sUserAssign(isSave); break;
                case "Permission": sUserViewer(isSave); break;
                case "Manager": sUserManager(isSave); break;
                case "AdminCentre": sUserAdminCentre(isSave); break;
                case "ITAdmin": sUserITAdmin(isSave); break;
                default: break;
            }
            if (vs_Status.Equals("Completed") || vs_Status.Equals(Extension._WaitForAdminCentre))
            {
                PanelInfoExtend.Visible = false;
            }

        }

        private void sUserAdminCentre(bool isSave)
        {
            DisableEditing("AdminCentre");
            DivButtonForSubmit.Visible = true;
            if (vs_Status == "Completed")
            {
                btn4 = btn4.SetUpActionButton("Assign & Forward", "", true, "CssButton custom-btn-Success", " ");
                btn4.Click += new EventHandler(this.btnAssign_Click);
                btn5 = btn5.SetUpActionButton("Send Email", "", true, "CssButton custom-btn-Info", string.Format("OnPreventDoubleClick({0}, 'Sending...');", btn5.ClientID));

                //btn4.Visible = true;
                //btn4.Text = "Assign & Forward";
                //btn4.CssClass = "CssButton custom-btn-Success";
                //btn4.OnClientClick = "";
                //btn4.Click += new EventHandler(this.btnAssign_Click);

                //btn5.Text = "Send Email";
                //btn5.CssClass = "CssButton custom-btn-Info";
                //btn5.OnClientClick = string.Format("confirm('Action: {0}');", btn5.Text);
            }
            else
            {
                btn4 = btn4.SetUpActionButton("", "", false, "", string.Empty);
                btn5 = btn5.SetUpActionButton("Confirm", "", true, "CssButton custom-btn-Success", string.Format("OnPreventDoubleClick({0}, 'Updating...');", btn5.ClientID));

                //btn4.Visible = false;
                //btn5.Text = "Confirm";
                //btn5.CssClass = "CssButton custom-btn-Success";
                //btn5.OnClientClick = string.Format("confirm('Action: {0}');", btn5.Text);
            }
        }
        private void sUserITAdmin(bool isSave)
        {
            DisableEditing("AdminCentre");
            DivButtonForSubmit.Visible = true;
            if (vs_Status.Equals(Extension._Completed))// || (isSave && vs_Status.Equals(Extension._Draft)))
            {
                btn4 = btn4.SetUpActionButton("Cancel", "", true, "CssButton custom-btn-Warning", "return confirm('Are you sure to cancel this document?');" + string.Format("OnPreventDoubleClick({0}, 'Cancelling...');", btn4.ClientID));
                btn5 = btn5.SetUpActionButton("Update", "Update", true, "CssButton custom-btn-Success", string.Format("OnPreventDoubleClick({0}, 'Updating...');", btn5.ClientID));
            }
            else
            {
                btn4 = btn4.SetUpActionButton("Cancel", "", true, "CssButton custom-btn-Warning", "return confirm('Are you sure to cancel this document?');" + string.Format("OnPreventDoubleClick({0}, 'Cancelling...');", btn4.ClientID));
                btn5 = btn5.SetUpActionButton("Confirm", "", true, "CssButton custom-btn-Success", string.Format("OnPreventDoubleClick({0}, 'Updating...');", btn5.ClientID));
            }
        }
        private void sUserCreator(bool isSave)
        {
            if (isSave)
            {

                if (!lbl_Status.Text.Trim().Equals(Extension._Completed))
                {
                    rdb_Type.Enabled = false;
                    rdb_Category.Enabled = false;
                    ddl_DocType.Enabled = false;

                    if (lbl_Status.Text.Trim().Equals(Extension._Draft))
                    {
                        btnCancel1.Visible = true;
                    }
                    else if (lbl_Status.Text.Trim().Equals(Extension._WaitForRequestorReview))
                    {
                        btnSubmit1.Visible = false;
                    }
                    DivButtonForSave.Visible = true;
                }
            }
            else
            {
                DivButtonForSubmit.Visible = true;
                if (lbl_Status.Text.Equals(Extension._Completed))
                {
                    btn4 = btn4.SetUpActionButton("", "", false, "", string.Empty);
                    btn5 = btn5.SetUpActionButton("Assign & Forward", "", true, "CssButton custom-btn-Success", " ");
                    btn5.Click += new EventHandler(this.btnAssign_Click);

                    DisableEditing("Creator");
                }
                else if (!lbl_Status.Text.Trim().Equals(Extension._NewRequest) && !lbl_Status.Text.Trim().Equals(Extension._Rework) && !lbl_Status.Text.Trim().Equals(Extension._Draft))
                {
                    sUserViewer(isSave);
                }
                else
                {
                    if (lbl_Status.Text.Trim().Equals(Extension._Draft) || lbl_Status.Text.Trim().Equals(Extension._Rework))
                    {
                        rdb_Type.Enabled = false;
                        ddl_DocType.Enabled = false;
                    }

                    btn3 = btn3.SetUpActionButton("Cancel", "", true, "CssButton custom-btn-Danger", string.Format("OnPreventDoubleClick({0}, 'Cancelling...');", btn3.ClientID));
                }
            }

        }
        private void sUserRequestor(bool isSave)
        {
            if (isSave)
            {
                if (!lbl_Status.Text.Trim().Equals(Extension._Completed))
                {
                    rdb_Type.Enabled = false;
                    rdb_Category.Enabled = false;
                    ddl_DocType.Enabled = false;

                    if (lbl_Status.Text.Trim().Equals(Extension._WaitForRequestorReview))
                    {
                        btnSubmit1.Visible = true;
                        btnCancel1.Visible = true;
                        DivButtonForSave.Visible = true;
                    }
                }
            }
            else
            {
                DivButtonForSubmit.Visible = true;
                if (lbl_Status.Text.Equals(Extension._Completed))
                {
                    btn4 = btn4.SetUpActionButton("Request Cancel", "", true, "CssButton custom-btn-Danger", string.Format("OnPreventDoubleClick({0}, 'Requesting...');return confirm('Action: Request Cancel');", btn4.ClientID));
                    btn5 = btn5.SetUpActionButton("Assign & Forward", "", true, "CssButton custom-btn-Success", " ");
                    btn5.Click += new EventHandler(this.btnAssign_Click);

                    DisableEditing("Requestor");
                }
                else if (lbl_Status.Text.Trim().Equals(Extension._WaitForRequestorReview))
                {
                    rdb_Type.Enabled = false;
                    ddl_DocType.Enabled = false;
                    btnCancel1.Visible = true;

                    //DivButtonForSave.Visible = true;
                }

                else if (!lbl_Status.Text.Trim().Equals(Extension._NewRequest) && !lbl_Status.Text.Trim().Equals(Extension._Rework) && !lbl_Status.Text.Trim().Equals(Extension._Draft))
                {
                    sUserViewer(isSave);
                }

                else
                {
                    if (lbl_Status.Text.Equals(Extension._WaitForRequestorReview))
                    {
                        btn3 = btn3.SetUpActionButton("Cancel", "", true, "CssButton custom-btn-Danger", string.Format("OnPreventDoubleClick({0}, 'Cancelling...');", btn3.ClientID));
                        btn4 = btn4.SetUpActionButton(Extension._Rework, "", true, "CssButton custom-btn-Warning", string.Format("OnPreventDoubleClick({0}, 'Reworking...');", btn4.ClientID));
                        btn5 = btn5.SetUpActionButton("Submit", "", true, "CssButton custom-btn-Success", string.Format("OnPreventDoubleClick({0}, 'Submitting...');", btn5.ClientID));

                        btnSearchEmployee.Enabled = false;
                        rdb_Type.Enabled = false;
                        ddl_DocType.Enabled = false;

                        //btnSaveDraft.Click += new EventHandler(this.btnRework_Click);
                    }
                    else if (lbl_Status.Text.Equals("Completed"))
                    {
                        btn4 = btn4.SetUpActionButton("", "", false, "", string.Empty);
                        btn5 = btn5.SetUpActionButton("Assign & Forward", "", true, "CssButton custom-btn-Success", " ");
                        btn5.Click += new EventHandler(this.btnAssign_Click);
                        DisableEditing("Requestor");
                    }
                    else
                    {
                        sUserViewer(isSave);
                    }
                }
            }
        }
        private void sUserAdvisor(bool isSave)
        {
            if (isSave)
            {
                sUserViewer(isSave);
            }
            else
            {

                if (lbl_Status.Text.Trim().ToLower().Equals(Extension._WaitForComment))
                {
                    DivButtonForSubmit.Visible = true;

                    btn2 = btn2.SetUpActionButton("", "", false, "", string.Empty);
                    btn3 = btn3.SetUpActionButton("", "", false, "", string.Empty);
                    btn4 = btn4.SetUpActionButton("", "", false, "", string.Empty);
                    btn5 = btn5.SetUpActionButton("Reply", "", true, "CssButton custom-btn-Success", string.Format("OnPreventDoubleClick({0}, 'Replying...');", btn5.ClientID));


                    //btn2.Visible = false;
                    //btn3.Visible = false;
                    //btn4.Visible = false;
                    //btn5.Text = "Reply";
                    //btn5.CssClass = "CssButton custom-btn-Success";
                    DisableEditing("");
                }
                else
                {
                    sUserViewer(isSave);
                }
            }
        }
        private void sUserManager(bool isSave)
        {
            if (isSave)
            {
                sUserViewer(isSave);
            }
            else
            {
                DivButtonForSubmit.Visible = true;
                btn2 = btn2.SetUpActionButton("", "", false, "", string.Empty);
                btn3 = btn3.SetUpActionButton("", "", false, "", string.Empty);
                btn4 = btn4.SetUpActionButton("Reject", "", true, "CssButton custom-btn-Danger", "return confirm('Are you you want to Reject?');" + string.Format("OnPreventDoubleClick({0}, 'Rejecting...');", btn4.ClientID));
                btn5 = btn5.SetUpActionButton("Approve", "", true, "CssButton custom-btn-Success", string.Format("OnPreventDoubleClick({0}, 'Approving...');", btn5.ClientID));
            }
        }
        private void sUserApprover(bool isSave)
        {
            if (isSave)
            {
                sUserViewer(isSave);
            }
            else
            {
                if (lbl_Status.Text.Trim().Equals(Extension._WaitForApprove) || lbl_Status.Text.Trim().Equals(Extension._RequestCancel) || lbl_Status.Text.Trim().Equals(Extension._WaitForRequestorReview) || lbl_Status.Text.Trim().Equals(Extension._Completed))
                {
                    DivButtonForSubmit.Visible = true;
                    if (vs_Status != "Completed")
                    {

                        btn2 = btn2.SetUpActionButton("RequestComment", "", true, "CssButton custom-btn-Info", string.Empty);
                        btn2.Click += new EventHandler(this.btnRequestComment_Click);
                        btn3 = btn3.SetUpActionButton("Reject", "", true, "CssButton custom-btn-Danger", "return confirm('Are you you want to Reject?');" + string.Format("OnPreventDoubleClick({0}, 'Rejecting...');", btn3.ClientID));
                        btn4 = btn4.SetUpActionButton(Extension._Rework, "", true, "CssButton custom-btn-Danger", "return confirm('Are you you want to Rework?');" + string.Format("OnPreventDoubleClick({0}, 'Reworking...');", btn4.ClientID));
                        btn5 = btn5.SetUpActionButton("Approve", "", true, "CssButton custom-btn-Success", string.Format("OnPreventDoubleClick({0}, 'Approving...');", btn5.ClientID));

                        //btn2.Visible = true;
                        //btn2.Text = "RequestComment";
                        //btn2.Click += new EventHandler(this.btnRequestComment_Click);
                        //btn2.CssClass = "CssButton custom-btn-Info";

                        //btn3.Text = "Reject";
                        //btn3.CssClass = "CssButton custom-btn-Danger";
                        //btn3.OnClientClick = "return confirm('Are you you want to Reject?');";
                        //btn3.Visible = true;

                        //btn4.Text = Extension._Rework;
                        //btn4.CssClass = "CssButton custom-btn-Warning";
                        //btn5.Text = "Approve";
                        //btn5.CssClass = "CssButton custom-btn-Success";
                    }
                    else
                    {
                        btn4 = btn4.SetUpActionButton("", "", false, "", string.Empty);
                        btn5 = btn5.SetUpActionButton("Assign & Forward", "", true, "CssButton custom-btn-Success", " ");
                        btn5.Click += new EventHandler(this.btnAssign_Click);

                        //btn4.Visible = false;

                        //btn5.Text = "Assign & Forward";
                        //btn5.CssClass = "CssButton custom-btn-Success";
                        //btn5.OnClientClick = "";
                        //btn5.Click += new EventHandler(this.btnAssign_Click);

                    }
                    DisableEditing("Approvor");
                }
                else
                {
                    sUserViewer(isSave);
                }
            }
        }
        private void sUserAssign(bool isSave)
        {
            if (isSave)
            {
                DivButtonForSubmit.Visible = true;

                btn4 = btn4.SetUpActionButton("", "", false, "", string.Empty);
                btn5 = btn5.SetUpActionButton("Assign & Forward", "", true, "CssButton custom-btn-Success", " ");
                btn5.Click += new EventHandler(this.btnAssign_Click);

                DisableEditing("Assign");
                //sUserViewer(isSave);
            }
            else
            {

                if (lbl_Status.Text.Equals("Completed"))
                {
                    DivButtonForSubmit.Visible = true;

                    btn4 = btn4.SetUpActionButton("", "", false, "", string.Empty);
                    btn5 = btn5.SetUpActionButton("Assign & Forward", "", true, "CssButton custom-btn-Success", " ");
                    btn5.Click += new EventHandler(this.btnAssign_Click);

                    //btn4.Visible = false;

                    //btn5.Text = "Assign & Forward";
                    //btn5.CssClass = "CssButton custom-btn-Success";
                    //btn5.OnClientClick = "";
                    //btn5.Click += new EventHandler(this.btnAssign_Click);
                    DisableEditing("Assign");
                }
                else
                {
                    sUserViewer(isSave);
                }
            }
        }
        private void sUserViewer(bool isSave)
        {
            DivButtonForSubmit.Visible = false;
            DivButtonForSave.Visible = false;
            DivButtonForViewer.Visible = true;
            DisableEditing("Viewer");
        }
        private void DisableEditing(string sUser)
        {
            if (vs_isLastApproval && lbl_Status.Text == "Wait for Approve")
            {
                //For Last Approval
                if (ddl_DocType.SelectedValue == "Im")
                {
                    chk_AutoStamp.Enabled = false;
                }
                rdb_Type.Enabled = false;
                rdb_Category.Enabled = false;
                ddl_DocType.Enabled = false;

                collapseCreator.Enabled = false;
                collapseRequestor.Enabled = false;
                chk_isAttachWord.Enabled = false;

                panel_Approval.Enabled = false;
                gvApprovelList.Columns[gvApprovelList.Columns.Count - 1].Visible = false;
                gvApprovelList.Columns[1].Visible = false;
                Btn_Add_Spec_Approver.Visible = false;
                //btn_DownloadTemplate.Visible = chk_isAttachWord.Checked;
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "enableTextEditing();", true);

            }
            else
            {
                if (sUser.Equals("AdminCentre"))
                {
                    //For AdminCentre
                    rdb_Type.Enabled = false;
                    rdb_Category.Enabled = false;
                    ddl_DocType.Enabled = false;
                    txt_ToDepartment.Enabled = false;

                    if (vs_Status.Equals("Completed"))
                    {
                        //Attach File
                        gv_AttachDocumentFile.Columns[gv_AttachDocumentFile.Columns.Count - 1].Visible = false;
                        doc_upload.Visible = false;
                        if (vs_Status == "Completed" || vs_WaitingFor != vs_CurrentUserID)
                        {
                            gv_AttachFile.Columns[gv_AttachFile.Columns.Count - 1].Visible = false;
                            file_upload.Visible = false;
                        }
                    }

                    txt_DocDescription.Enabled = false;
                    txt_title.Enabled = false;
                    txt_subtitle.Enabled = false;
                    btn_searchDepartment.Enabled = false;
                    other_extend.Enabled = false;
                    info_extend.Enabled = false;
                    memo_extend.Enabled = false;
                    ddl_Priority.Enabled = false;
                    txt_deadline.Enabled = false;
                    chk_AutoStamp.Enabled = false;
                    chk_DOA.Enabled = false;
                    btn_addReferenceDocument.Enabled = false;
                    gv_ReferenceDocument.Enabled = false;
                    tblAmount.Enabled = false;
                    panel_Approval.Enabled = false;
                    btn_DownloadTemplate.Visible = false;
                    //Refer Doc
                    gv_ReferenceDocument.Columns[gv_ReferenceDocument.Columns.Count - 1].Visible = false;

                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "disableTextEditing();", true);
                    //} 
                }
                else
                {
                    btn_DownloadTemplate.Visible = false;
                    //txtContent.Enabled = false;
                    //Information
                    panelInfomation.Enabled = false;

                    //permission
                    panel_Permission.Enabled = false;
                    gv_Permission.Columns[gv_Permission.Columns.Count - 1].Visible = false;
                    gv_GroupEmail.Columns[gv_GroupEmail.Columns.Count - 1].Visible = false;
                    btn_addPermission.Visible = false;
                    btn_addGroupEmail.Visible = false;

                    //Attach File
                    gv_AttachDocumentFile.Columns[gv_AttachDocumentFile.Columns.Count - 1].Visible = false;
                    doc_upload.Visible = false;
                    if (vs_Status == "Completed" || vs_WaitingFor != vs_CurrentUserID)
                    {
                        gv_AttachFile.Columns[gv_AttachFile.Columns.Count - 1].Visible = false;
                        file_upload.Visible = false;
                    }

                    //Refer Doc
                    gv_ReferenceDocument.Columns[gv_ReferenceDocument.Columns.Count - 1].Visible = false;
                    //Info Extend
                    tblAmount.Enabled = false;

                }

                ddl_DocType.Enabled = false;
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "disableTextEditing();", true);
                //Creator & Requestor
                collapseCreator.Enabled = false;
                collapseRequestor.Enabled = false;

                chk_isAttachWord.Enabled = false;

                //Approval
                panel_Approval.Enabled = false;
                gvApprovelList.Columns[gvApprovelList.Columns.Count - 1].Visible = false;
                gvApprovelList.Columns[1].Visible = false;
                Btn_Add_Spec_Approver.Visible = false;

            }
        }

        private void HideAllButton()
        {
            DivButtonForSubmit.Visible = false;
            DivButtonForSave.Visible = false;
            DivButtonForViewer.Visible = false;
        }
        #endregion

        #region | Print Preview |
        private byte[] PrintMemoDetail(string sAction, DataClassesDataAccessDataContext db)
        {
            string sDocType = ddl_DocType.SelectedValue;
            string sDocLib = "TempDocument";
            string sDocTypeName = ddl_DocType.SelectedItem.ToString();
            string sCategory = rdb_Category.SelectedValue;
            string sType = rdb_Type.SelectedValue;
            string sOtherDocType = ddl_otherDocType.SelectedValue;
            string sTitle = txt_title.Text;
            string sTo = txt_to.Text;
            string sCC = txt_CC.Text;
            string sAttach = txt_Attachment.Text;
            string sReceiveDocNo = txt_RecieveDocNo.Text;
            string sDocDate = txt_DocumentDate.Text;
            string sReceiveDocDate = txt_DocumentRecieve.Text;
            string sSource = txt_Source.Text;
            string sFromDepartment = txt_FromDepartment.Text;
            string sGenerateStatus = Extension._WaitForAdminCentre;
            string sRequestorID = lbl_RequestorID.Text;
            string sRequestorDeptID = ddl_RequestorDepartment.SelectedValue;
            string sRequestorDeptName = "";
            string sRequestorSubDeptID = ddl_RequestorSubDepartment.SelectedValue;
            string sRequestorDeptAcronymTH = "";
            string sRequestorDeptAcronymEN = "";
            string sInstitudeName = "สถาบันการจัดการปัญญาภิวัฒน์";
            bool sIsInternalOnlyStamp = chk_InternalStamp.Checked;
            //string sCreateDate = txt_CreateDate.Text ;

            try
            {
                if (db == null)
                {
                    db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                }
                startConversion = true;
                TRNDocument objDocument;
                SpecificEmployeeData.RootObject objRequestorInfo = Extension.GetSpecificEmployeeFromTemp(Page, lbl_RequestorID.Text);
                DataTable dtRequestorDepInfo = Extension.GetSpecificDepartmentDataFromTemp(Page, sRequestorDeptID);
                if (!string.IsNullOrWhiteSpace(vs_PK))
                {
                    objDocument = db.TRNDocuments.FirstOrDefault(x => x.DocID == Convert.ToInt32(vs_PK));
                    if (objDocument != null)
                    {
                        sDocLib = objDocument.DocLib;
                        sIsInternalOnlyStamp = objDocument.InternalOnlyStamp ?? false;
                        objRequestorInfo = Extension.GetSpecificEmployeeFromTemp(Page, objDocument.RequestorID.ToString());
                        sRequestorID = objDocument.RequestorID.ToString();
                        sRequestorDeptID = objDocument.RequestorDepartmentID.ToString();
                    }
                }
                if (!dtRequestorDepInfo.DataTableIsNullOrEmpty())
                {
                    sRequestorDeptAcronymTH = dtRequestorDepInfo.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString();
                    sRequestorDeptAcronymEN = dtRequestorDepInfo.Rows[0]["DEPARTMENT_ACRONYM_EN"].ToString();
                    sRequestorDeptName = dtRequestorDepInfo.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                }

                bool isAutoStamp = chk_AutoStamp.Checked;
                string sDocumentTypeHead = sDocTypeName + sInstitudeName;
                if (rdb_Category.SelectedValue == "internal" && (sDocType == "C" || sDocType == "P"))
                {
                    sDocumentTypeHead = string.Format("{0}{1} {2}", sDocTypeName, sRequestorDeptName, sInstitudeName); ;
                }

                string sLogoPath = string.Format("{0}/{1}/{2}", SPContext.Current.Site.Url, "Img", "logo.png");
                string sDocumentNumber = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/{0}", DateTime.Now.Year.ToString().ConvertToBE());
                if (vs_Status == sGenerateStatus || vs_Status == "Completed")
                {
                    string[] a = lbl_DocumentNo.Text.Split(new char[] { ' ', '/' });
                    string docNo = "";
                    string docYear = "";
                    if (a.Length >= 2)
                    {
                        int b = Convert.ToInt32(a[a.Length - 2]);
                        docNo = b > 999 ? b.ToString() : string.Format("{0:D4}", b);
                        int year = Convert.ToInt32(a[a.Length - 1]);
                        docYear = year.ToString();
                    }
                    sDocumentNumber = string.Format("{0}/{1}", docNo, docYear.ConvertToBE());
                    if (sDocType == "ExEN")
                    {
                        sDocumentNumber = string.Format("{0}/{1}", docNo, docYear.ConvertToAD());
                    }
                }
                else if (sDocType == "Ex")
                {
                    sDocumentNumber = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/{0}", DateTime.Now.Year.ToString().ConvertToBE());
                }

                DateTime sCreateDate = DateTime.ParseExact(txt_CreateDate.Text, "dd/MM/yyyy", _ctli);
                if (!string.IsNullOrWhiteSpace(hdn_ApproveDate.Value))
                {
                    sCreateDate = DateTime.ParseExact(hdn_ApproveDate.Value, "dd/MM/yyyy", _ctli);
                }
                string sDocumentDate = string.Format("{0} {1} {2}", sCreateDate.Day,
                    sCreateDate.Month.GetTHMonth(), sCreateDate.Year.ToString().ConvertToBE());
                //string sDocumentTo = string.Format("เรียน {0}", sTo);
                //string sDocumentTitle = string.Format("เรื่อง {0}", sTitle);

                #region | HeaderTemplate |

                string sHeader = "";

                #region | Memo (บันทึกข้อความ) & Other (อื่นๆ) |

                if (sDocType == "M" || sDocType == "Other")
                {
                    if (sDocType.Equals("Other"))
                    {
                        //sDocType = sOtherDocType;
                        DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
                        if (!dtDocType.DataTableIsNullOrEmpty())
                        {
                            dtDocType = dtDocType.AsEnumerable().Where(x => x.Field<String>("Value").Equals(sOtherDocType)).ToList().CopyToDataTable();
                            if (!dtDocType.DataTableIsNullOrEmpty())
                            {
                                sDocTypeName = dtDocType.Rows[0]["DocTypeName"].ToString();
                            }
                        }
                    }
                    else
                    {
                        //sDocType = "บันทึกข้อความ";
                    }
                    string MemoDocNo = string.Format("{0} {1}", sRequestorDeptAcronymTH, sDocumentNumber);
                    if (!dtRequestorDepInfo.DataTableIsNullOrEmpty())
                    {
                        //เอกสารจากศูนย์?
                        if (dtRequestorDepInfo.Rows[0]["DEPARTMENT_ID"].ToString().Equals("6"))
                        {
                            DataTable dtRequestorSubDepInfo = Extension.GetSpecificDepartmentDataFromTemp(Page, sRequestorSubDeptID);
                            if (!dtRequestorSubDepInfo.DataTableIsNullOrEmpty())
                            {
                                MemoDocNo = string.Format("สบว.{0} {1}", dtRequestorSubDepInfo.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString(), sDocumentNumber);
                            }
                        }
                    }



                    //บันทึกข้อความ
                    sHeader = string.Format(
                        @"<div style=""width:800px; margin:50 auto 0 140;"">
						<div style=""text-align:left; color: black; font-family:TH SarabunPSK; font-size:28px;"">
							<img style=""position:absolute;"" src=""{0}"" height=""120"" width=""120""/>
							<div style=""float:right; width:800px; margin-top:50px; text-align:center; font-size:60px;"">
							  <b>{1}</b>
							</div>     
							<div style=""float:left; width:550px;"">
							  <b>{2}&nbsp;&nbsp;</b>{3}
							  <br/>
							  <b>{4}&nbsp;&nbsp;</b>{5}                                
							</div>
							<div style=""float:right; width:250px;"">
							  <b>{8}&nbsp;&nbsp;</b>{9}
							  <br/>
							  <b>{10}&nbsp;&nbsp;</b>{11}  
							</div>
							<div style=""float:right; width:800px;"">
								<b>{6}&nbsp;&nbsp;</b>{7}
							</div>
						</div>
						<div style=""margin: 0 auto; clear:both; width:800px; border-bottom :1px solid; padding-top: 5px; padding-bottom: 5px;""></div>  
					</div>"
                        , sLogoPath, sDocTypeName, "หน่วยงาน", sFromDepartment, "ที่", MemoDocNo, "เรื่อง",
                        sTitle, "โทรศัพท์", lbl_RequestorTel.Text, "วันที่", sDocumentDate);
                }
                #endregion
                #region | Proclamation (ประกาศ) & Command (คำสั่ง) |

                else if (sDocType == "C" || sDocType == "P")
                {
                    sHeader = string.Format(
                        //ประกาศ  คำสั่ง
                        @"<div style=""width:800px; margin:50 auto; "">
			<div style=""text-align:center; color: black; font-family:TH SarabunPSK; font-size:27px; line-height:1.2;"">
				<img src=""{3}""  height=""130"" />
				<br/>
				<b>{0}</b>
				<br/>
				<b>ที่ {1}</b>
				<br />
				<b>เรื่อง {2}</b>   
			</div>
			<div style=""margin: 0 auto; clear:both; width:200px; border-bottom :1px solid; padding-top: 5px; padding-bottom: 5px;""></div>  
			
</div>", sDocumentTypeHead, sDocumentNumber, sTitle, sLogoPath);

                }
                #endregion
                #region | Law (ข้อบังคับ) & Regulation (ระเบียบ) |

                else if (sDocType == "R" || sDocType == "L")
                {
                    sHeader = string.Format(
                        //ข้อบังคับ  ระเบียบ    
                        @"<div style=""width:800px; margin:50 auto 0; "">
			<div style=""text-align:center; color: black; font-family:TH SarabunPSK; font-size:28px;  line-height:1.2;"">
				<img src=""{2}""  height=""130"" />
				<br/>
				<b>{0}</b>
				<br/>
				<b>ว่าด้วย {1}</b> 
			</div>
			<div style=""margin: 0 auto; clear:both; width:200px; border-bottom :1px solid; padding-top: 5px; padding-bottom: 5px;""></div>  
			
</div>", sDocumentTypeHead, sTitle, sLogoPath);

                }
                #endregion
                #region | Export (จดหมายออก) |

                else if (sDocType == "Ex")
                {
                    #region | reference Document |

                    string refTemp = "";
                    if (!vs_DtRefDoc.DataTableIsNullOrEmpty())
                    {
                        int rRefDocID = Convert.ToInt32(vs_DtRefDoc.Rows[0]["RefDocID"].ToString());
                        TRNDocument objRefDoc = db.TRNDocuments.Where(x => x.DocID == rRefDocID).SingleOrDefault();
                        if (objRefDoc != null)
                        {
                            refTemp = Extension.GenerateDocumentNo(objRefDoc.DocNo, Convert.ToInt32(objRefDoc.FromDepartmentID), objRefDoc.Category, objRefDoc.DocTypeCode, Page);
                        }
                    }
                    //foreach (DataRow row in vs_DtRefDoc.Rows)
                    //{
                    //    refTemp += string.IsNullOrWhiteSpace(refTemp) ? "" : "</br>";
                    //    refTemp += row["RefDocNo"].ToString();
                    //}

                    string refDoc = string.IsNullOrEmpty(refTemp) ? "" : refTemp;

                    #endregion

                    #region | Attachment |

                    string attachDoc = string.IsNullOrEmpty(txt_Attachment.Text)
                        ? ""
                        : sAttach;

                    #endregion

                    sHeader = string.Format(
                        //จดหมายออก   
                        @"<div style=""width:800px; margin:150 auto 0 140; line-height: 1.2; display:-webkit-box; font-family:TH SarabunPSK; font-size:27px;"">
                            <div style=""width:200px;"">
                                <p>ที่ สจป.{0}</p>
                            </div>
                            <div style=""width:335px;""></div>
                            <div style=""width:265px;"">
                                <span>สถาบันการจัดการปัญญาภิวัฒน์ </br>
                                   85/1 หมู่ 2 ถนนแจ้งวัฒนะ </br>
                                   ตำบลบางตลาด อำเภอปากเกร็ด</br>
                                   จังหวัดนนทบุรี 11120</span>
                            </div>
                          </div>
                          <div style=""width:800px; margin-left:140; font-family:TH SarabunPSK; font-size:27px; "">
                            <div style=""width:500px; text-align:right; margin-top:10px; margin-buttom:10px;"">
                                {1}
                            </div>
                            <div style""width:100%;"">
                                <table>
                                    {2}{3}{4}{5}{6}
                                </table>
                            </div>                      
                          </div>", sCategory == "internal" ? string.Format("{0} {1}", sRequestorDeptAcronymTH, sDocumentNumber) : " " + sDocumentNumber,
                                 sDocumentDate,
                        string.IsNullOrWhiteSpace(sTitle) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:70px; vertical-align:top;"">เรื่อง</td>
                                        <td >{0}</td>
                                    </tr>", sTitle),
                        string.IsNullOrWhiteSpace(sTo) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:70px; vertical-align:top;"">เรียน</td>
                                        <td >{0}</td>
                                    </tr>", sTo),
                        string.IsNullOrWhiteSpace(sCC) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:80px; vertical-align:top;"">สำเนาถึง</td>
                                        <td >{0}</td>
                                    </tr>", sCC),
                        string.IsNullOrWhiteSpace(refDoc) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:70px; vertical-align:top;"">อ้างถึง</td>
                                        <td >{0}</td>
                                    </tr>", refDoc),
                        string.IsNullOrWhiteSpace(attachDoc) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:130px; vertical-align:top;"">สิ่งที่แนบมาด้วย</td>
                                        <td >{0}</td>
                                    </tr>", attachDoc));

                }
                #endregion
                #region | Export (จดหมายออกภาษาอังกฤษ) |

                else if (sDocType == "ExEN")
                {
                    sDocumentDate = string.Format("{0} {1} {2}", sCreateDate.Day,
                        sCreateDate.Month.GetENMonth(), sCreateDate.Year.ToString().ConvertToAD());
                    sHeader = string.Format(
                        //จดหมายออกภาษาอังกฤษ  
                        @"<div style=""width:800px; margin:150 auto 0 140; line-height: 1.2;  font-family:TH SarabunPSK; font-size:27px;"">
                            
                                <p>No. PIM.{0}{1}</p>
                            	<p>{2}</p>
                                <p>Panyapiwat Institute of Management<br/>
                                   85/1 Moo 2 Chaengwattana Rd., <br/>
                                   Pakkred, Nonthaburi 11120</p>                           		
                          </div>", sCategory == "internal" ? string.Format("{0}. {1}", sRequestorDeptAcronymEN, sDocumentNumber) : sDocumentNumber, "", sDocumentDate);
                }

                #endregion

                #endregion

                #region | BodyTemplate |

                string sContent;
                if (sDocType == "Im")
                {
                    sContent =
                        string.Format(@"<div style=""width:800px; margin:50 auto 0 140; font-family:TH SarabunPSK; font-size:26px;"">
<h3>รายละเอียดหนังสือภายนอก (Receive Document Detail)</h3>

<div style=""width: 300px; display:inline; float:left;"">
	<span>Document Number: </span></br>
	<span>เลขที่เอกสาร: </span></br>
	<span>Receive Document Number: </span> </br>
	<span>เลขที่หนังสือรับจากภายนอก: </span></br>
	<span>Document Date: </span> </br>
	<span>วันที่ในเอกสาร: </span></br>
	<span>Document Receive: </span></br>
	<span>วันที่รับเอกสาร: </span></br>
	<span>Title: </span></br>
	<span>ชื่อเรื่อง: </span>
</div>
<div style=""width:400px; display:inline; float:left"">
	<span>{0}</span> </br></br>
	<span>{1}</span> </br></br>
	<span>{2}</span> </br></br>
	<span>{3}</span> </br></br>
	<span>{4}</span>
</div>
</div>", sDocumentNumber, sReceiveDocNo, sDocDate, sReceiveDocDate, sTitle);
                }
                else
                {
                    sContent = string.Format(@" <div style=""width:800px; margin:0 auto 0 140;  "">{0}</div>",
                        "");
                    //sContent = string.Format(@" <div style=""width:800px; margin:0 auto 0 140;  "">{0}</div>",
                    //    txtContent.Text);
                }

                #endregion

                #region | Signature |

                List<SignatureImage> listSignatureImages = db.SignatureImages.ToList();
                string wording = "";
                string sLastApprovalID;
                string sLastApprovalFullName;
                string sLastApprovalPosition;
                string sLastApprovalDepartment;
                string sSignatureTag;
                if (sDocType == "L" || sDocType == "R" ||
                    sDocType == "P")
                {
                    wording = "ประกาศ";
                }
                if (sDocType == "C")
                {
                    wording = "สั่ง";
                }

                #region | Signature Head |
                string sSignatureHead =
                    string.Format(@"</br></br><div style=""width:800px; margin:0 auto 0 140;font-family:TH SarabunPSK; font-size:28px;"">
  <div style=""margin-left:140px;"">
  		<p><span>{0}&nbsp;&nbsp;&nbsp;ณ&nbsp;&nbsp;&nbsp;วันที่&nbsp;&nbsp;&nbsp;&nbsp;{1}&nbsp;&nbsp;&nbsp;&nbsp;{2}&nbsp;&nbsp;&nbsp;พ.ศ.&nbsp;&nbsp;&nbsp;{3}</span></p>
  </div>", wording, sCreateDate.Day, sCreateDate.Month.GetTHMonth(), sCreateDate.Year.ToString().ConvertToBE());

                if (sDocType == "Ex")
                {
                    sSignatureHead =
                        string.Format(
                            @"</br></br><div style=""width:800px; margin:0 auto 0 140;font-family:TH SarabunPSK; font-size:28px;"">
  <div style=""width:500px; float:right; text-align:center; "">
  <p><span>ขอแสดงความนับถือ</span></p>
  </div>
");
                }
                if (sDocType == "ExEN")
                {
                    sSignatureHead =
                        string.Format(
                            @"</br></br><div style=""width:800px; margin:0 auto 0 140;font-family:TH SarabunPSK; font-size:28px;"">
  <div style=""float:left;"">
  <p><span>Yours sincerely,</span></p>
  </div>
");
                }
                if (sDocType == "M" || sDocType == "Other")
                {
                    sSignatureHead =
                        string.Format(
                            @"</br></br><div style=""width:800px; margin:0 auto 0 140;font-family:TH SarabunPSK; font-size:28px;"">");
                }
                #endregion

                #region | get Approvor Sign Path |

                DataTable dtApproval = vs_ApprovalList;
                if (!dtApproval.DataTableIsNullOrEmpty())
                {
                    DataView dvApprover = dtApproval.DefaultView;
                    dvApprover.Sort = "Sequence DESC";
                    dtApproval = dvApprover.ToTable();

                    sLastApprovalID = dtApproval.Rows[0]["EmpID"].ToString();

                    string sSignaturePath = string.Format("{0}/Signature/{1}.gif", Extension.GetSPSite(), sLastApprovalID);
                    sSignatureTag = string.Format(@" <img height=130 src=""{0}"" />", sSignaturePath);

                    SpecificEmployeeData.RootObject lastApprovalData = Extension.GetSpecificEmployeeFromTemp(Page,
                        sLastApprovalID);
                    if (lastApprovalData != null)
                    {
                        sLastApprovalFullName = string.Format("{0}{1} {2}", lastApprovalData.PREFIX_ACADEMIC_TH,
                            lastApprovalData.FIRSTNAME_TH, lastApprovalData.LASTNAME_TH);
                        sLastApprovalPosition = dtApproval.Rows[0]["PositionName"].ToString();
                        sLastApprovalDepartment = dtApproval.Rows[0]["DepartmentName"].ToString();
                        if (ddl_DocType.SelectedValue == "ExEN")
                        {
                            sLastApprovalFullName = string.Format("{0} {1} {2}", lastApprovalData.PREFIX_ACADEMIC_EN,
                           lastApprovalData.FIRSTNAME_EN, lastApprovalData.LASTNAME_EN);
                            sLastApprovalPosition = lastApprovalData.RESULT.First(x => x.POSITION_TD == dtApproval.Rows[0]["PositionID"].ToString()).POSTION_NAME_EN;
                            sLastApprovalDepartment = lastApprovalData.RESULT.First(x => x.DEPARTMENT_ID == dtApproval.Rows[0]["DepartmentID"].ToString()).DEPARTMENT_NAME_EN;
                        }
                        if (listSignatureImages.All(x => x.EmployeeID != sLastApprovalID))
                        {
                            sSignatureTag = string.Format("<span style='font-size:40px'><em>{0} {1}</em></span>",
                                lastApprovalData.FIRSTNAME_TH, lastApprovalData.LASTNAME_TH);
                        }
                    }
                    else
                    {
                        throw new Exception("Approval Data Empty");
                    }
                }
                else
                {
                    throw new Exception("Approval Data Row Count 0 or Empty");
                }

                #endregion

                #region | Signature Body |
                string sSignatureBody;
                if (ddl_DocType.SelectedValue == "Ex")
                {
                    sLastApprovalPosition += "สถาบันการจัดการปัญญาภิวัฒน์";
                }
                if (ddl_DocType.SelectedValue == "ExEN")
                {
                    sSignatureBody = string.Format(@"  
                                                      <p>
                                                      {0}</br>
                                                      <span>({1})</br>{2}<br/>{3}</span></p>
                                                      </div>
                                                    ",
                        isAutoStamp && (vs_Status == "Completed" || vs_Status == Extension._WaitForAdminCentre)
                            ? sSignatureTag
                            : "</br></br></br></br>", sLastApprovalFullName, sLastApprovalPosition, sLastApprovalDepartment);
                }
                else
                {
                    sSignatureBody = string.Format(@"  
                                                      <div style=""width: 500px; float:right;text-align:center; "">
                                                      {0}<br/>
                                                      <span>({1})<br/>{2}<br/>{3}</span>
                                                      </div>
                                                    </div>
                                                    ",
                        isAutoStamp && (vs_Status == "Completed" || vs_Status == Extension._WaitForAdminCentre)
                            ? sSignatureTag
                            : "<br/><br/>", sLastApprovalFullName, sLastApprovalPosition, sLastApprovalDepartment);
                }
                #endregion

                if (sDocType != "Im")
                {
                    sContent += (sSignatureHead + sSignatureBody);

                }
                #endregion

                #region | History |
                string History = "";
                if (!string.IsNullOrEmpty(vs_PK))
                {
                    List<TRNHistory> ListHistory = new List<TRNHistory>();
                    ListHistory = db.TRNHistories.Where(x => (x.DocID == Convert.ToInt32(vs_PK)) && x.ActionName != "Save Draft" && x.ActionName != "Confirm").OrderByDescending(x => x.ActionDate).ToList();
                    if (ListHistory != null)
                    {
                        if (ListHistory.Count > 0)
                        {
                            string sTagApprovalHistory = vs_Lang == "EN" ? "Approval History" : "ประวัติการดำเนินการ";
                            string sActionDateTag = vs_Lang == "EN" ? "Date" : "วันที่";
                            string sActorName = vs_Lang == "EN" ? "Name" : "ชื่อ-นามสกุล";
                            string sPositionName = vs_Lang == "EN" ? "Position" : "ตำแหน่ง";
                            string sActionName = vs_Lang == "EN" ? "Action" : "ดำเนินการโดย";
                            string sSign = vs_Lang == "EN" ? "Signature" : "ลายเซ็นต์";
                            string sComment = vs_Lang == "EN" ? "Comment" : "ความคิดเห็น";
                            string sTemplateHistory = string.Format(@"<div style="" clear :both;padding-bottom:10px;""></div>
           <br/><br/> <div style=""width:900px; margin:0 auto;""><div style=""width:900px;font-family: 'TH SarabunPSK'; font-size: 20px;"">
                <b>{0}</b>
                <table style=""width: 100%; border: 1px solid black; table-layout:fixed; border-collapse: collapse; font-family: 'TH SarabunPSK'; font-size: 20px;"">                   
                    <tr>
                        <td style=""border: 1px solid black; width: 80px; text-align:center; word-wrap:break-word;""><b>{1}</b></td>
                        <td style=""border: 1px solid black; width: 140px; text-align:center; word-wrap:break-word;""><b>{2}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{3}</b></td>
                        <td style=""border: 1px solid black; width: 100px; text-align:center; word-wrap:break-word;""><b>{4}</b></td>
                        <td style=""border: 1px solid black; width: 130px; text-align:center; word-wrap:break-word;""><b>{5}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{6}</b></td>
                    </tr>                    
                ", sTagApprovalHistory, sActionDateTag, sActorName,
                   sPositionName, sActionName, sSign, sComment);
                            string sBodyTableHistory = "";

                            //                            DataTable dtSignature = ObjSPLib.GetList("MstSignatureWording", "");
                            foreach (TRNHistory ObjactHistory in ListHistory)
                            {
                                string sHistorySignaturePath = "";
                                string sSignatureWording = vs_Lang == "EN" ? "	Yours sincerely" : "ขอแสดงความนับถือ";
                                DataView dvHistory = Extension.GetEmployeeData(this.Page).DefaultView;
                                dvHistory.RowFilter = string.Format(@"EMPLOYEEID = '{0}'", ObjactHistory.EmpID);
                                DataTable dtHisEmp = dvHistory.ToTable();
                                if (dtHisEmp != null)
                                {
                                    if (dtHisEmp.Rows.Count > 0)
                                    {
                                        string sActorEmpName = vs_Lang == "EN" ? dtHisEmp.Rows[0]["EmployeeName_EN"].ToString() : dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                        if (listSignatureImages.Any(x => x.EmployeeID == ObjactHistory.EmpID.ToString()))
                                        {
                                            sHistorySignaturePath = string.Format("<img src='{0}' width='130' />", string.Format("{0}/Signature/{1}.gif", Extension.GetSPSite(), ObjactHistory.EmpID.ToString()));
                                        }
                                        if (string.IsNullOrEmpty(sHistorySignaturePath))
                                        {
                                            //string sHisUserLoginName = dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                            string sHisUserLoginName = string.Format("{0} {1}", dtHisEmp.Rows[0]["FIRSTNAME_TH"].ToString(), dtHisEmp.Rows[0]["LASTNAME_TH"].ToString());
                                            sHistorySignaturePath = string.Format("<span style='font-size:20px'><em>{0}</em></span>", sHisUserLoginName);
                                        }
                                        string sBodyRow = string.Format(@"<tr style=""border: 1px solid black;"">
                                                            <td style=""word-wrap:break-word; padding:5px; width: 80px; border: 1px solid black;"">{0}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 140px; border: 1px solid black;"">{1}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{2}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 100px; border: 1px solid black;"">{3}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 130px; border: 1px solid black;"">{4}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{5}</td>
                                                        </tr>", DateTime.Parse(ObjactHistory.ActionDate.ToString()).ToString("dd/MM/yyyy HH:mm", _ctli),
                                                  sActorEmpName,
                                                  vs_Lang == "TH" ? dtHisEmp.Rows[0]["POSTION_NAME_TH"].ToString() : dtHisEmp.Rows[0]["POSTION_NAME_EN"].ToString(),
                                                  ObjactHistory.ActionName,
                                                  sHistorySignaturePath,
                                                  ObjactHistory.Comment);
                                        sBodyTableHistory += sBodyRow;
                                    }
                                }
                            }
                            string sTemplateHistoryEnd = "</table></div></div>";
                            sTemplateHistory = sTemplateHistory + sBodyTableHistory + sTemplateHistoryEnd;
                            History = string.Format(@" <div style=""width:900px; margin:0 auto;  "">{0}</div>", sTemplateHistory);
                            if (sDocType == "Im")
                            {
                                sContent += History;
                            }
                        }
                    }
                }
                #endregion

                #region | Assign |
                string Assign = "";
                if (!string.IsNullOrEmpty(vs_PK))
                {
                    List<TRNAssign> ListAssign = new List<TRNAssign>();
                    ListAssign = db.TRNAssigns.Where(x => (x.DocID == Convert.ToInt32(vs_PK))).OrderByDescending(x => x.ActionDate).ToList();
                    if (ListAssign != null)
                    {
                        if (ListAssign.Count > 0)
                        {
                            string sTagAssign = vs_Lang == "EN" ? "Assignl History" : "ประวัติการมอบหมายงาน";
                            string sActionDateTag = vs_Lang == "EN" ? "Date" : "วันที่";
                            string sActorName = vs_Lang == "EN" ? "Name" : "ชื่อ-นามสกุล";
                            string sPositionName = vs_Lang == "EN" ? "Position" : "ตำแหน่ง";
                            string sActionName = vs_Lang == "EN" ? "Assign To" : "มอบหมายงานให้";
                            string sComment = vs_Lang == "EN" ? "Comment" : "ความคิดเห็น";
                            string sTemplateAssign = string.Format(@"<div style="" clear :both;padding-bottom:10px;""></div>
           <br/><br/> <div style=""width:900px; margin:0 auto;""><div style=""width:900px;font-family: 'TH SarabunPSK'; font-size: 20px;"">
                <b>{0}</b>
                <table style=""width: 100%; border: 1px solid black; table-layout:fixed; border-collapse: collapse; font-family: 'TH SarabunPSK'; font-size: 20px;"">                   
                    <tr>
                        <td style=""border: 1px solid black; width: 80px; text-align:center; word-wrap:break-word;""><b>{1}</b></td>
                        <td style=""border: 1px solid black; width: 140px; text-align:center; word-wrap:break-word;""><b>{2}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{3}</b></td>
                        <td style=""border: 1px solid black; width: 100px; text-align:center; word-wrap:break-word;""><b>{4}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{5}</b></td>
                    </tr>                    
                ", sTagAssign, sActionDateTag, sActorName,
                   sPositionName, sActionName, sComment);
                            string sBodyTableAssign = "";
                            //                            DataTable dtSignature = ObjSPLib.GetList("MstSignatureWording", "");
                            foreach (TRNAssign ObjactAssign in ListAssign)
                            {
                                DataView dvHistory = Extension.GetEmployeeData(this.Page).DefaultView;
                                dvHistory.RowFilter = string.Format(@"EMPLOYEEID = '{0}'", ObjactAssign.ActorID);
                                DataTable dtHisEmp = dvHistory.ToTable();
                                if (dtHisEmp != null)
                                {
                                    if (dtHisEmp.Rows.Count > 0)
                                    {
                                        string sActorEmpName = vs_Lang == "EN" ? dtHisEmp.Rows[0]["EmployeeName_EN"].ToString() : dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                        SpecificEmployeeData.RootObject objAssignTO = Extension.GetSpecificEmployeeFromTemp(Page, ObjactAssign.AssignToID.ToString());
                                        string sAssignEmpName = string.Format("{0}{1} {2}", objAssignTO.PREFIX_TH, objAssignTO.FIRSTNAME_TH, objAssignTO.LASTNAME_TH);
                                        string sBodyRow = string.Format(@"<tr style=""border: 1px solid black;"">
                                                            <td style=""word-wrap:break-word; padding:5px; width: 80px; border: 1px solid black;"">{0}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 140px; border: 1px solid black;"">{1}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{2}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 100px; border: 1px solid black;"">{3}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{4}</td>
                                                        </tr>", DateTime.Parse(ObjactAssign.ActionDate.ToString()).ToString("dd/MM/yyyy HH:mm", _ctli),
                                                  sActorEmpName,
                                                  vs_Lang == "TH" ? dtHisEmp.Rows[0]["POSTION_NAME_TH"].ToString() : dtHisEmp.Rows[0]["POSTION_NAME_EN"].ToString(),
                                                  sAssignEmpName,
                                                  ObjactAssign.Comment);
                                        sBodyTableAssign += sBodyRow;
                                    }
                                }
                            }
                            string sTemplateAssignEnd = "</table></div></div>";
                            sTemplateAssign = sTemplateAssign + sBodyTableAssign + sTemplateAssignEnd;
                            Assign = string.Format(@" <div style=""width:900px; margin:0 auto;  "">{0}</div>", sTemplateAssign);
                            if (ddl_DocType.SelectedValue == "Im")
                            {
                                sContent += Assign;
                            }
                        }
                    }
                }
                #endregion

                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), "A4", true);
                PdfPageOrientation pdfOrientation =
                    (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                    "Portrait", true);

                HtmlToPdf converter = new HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = 1024;
                converter.Options.WebPageHeight = 0;
                converter.Options.MaxPageLoadTime = 300;
                if (ddl_DocType.SelectedValue == "Im")
                {

                }
                else if (ddl_DocType.SelectedValue == "Ex")
                {
                    sContent = sHeader + sContent;
                    string sExFooter = "";
                    sExFooter = string.Format(@"
                                <div style=""width:800px; margin:0 auto 0 140; font-family:TH SarabunPSK; font-size:27px; line-height: 1.2;""> 
                                <p>{0}</br>
                                โทร. {1}</br>
                                โทรสาร 0-2832-0391</br>
                                อีเมล์ {2}</p>
                                </div>", objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_TH, objRequestorInfo.TELEPHONE, objRequestorInfo.EMAIL);

                    converter.Options.DisplayFooter = true;
                    converter.Footer.Height = 150;
                    converter.Footer.Add(new PdfHtmlSection(sExFooter, ""));
                }
                else if (ddl_DocType.SelectedValue == "ExEN")
                {
                    sContent = sHeader + sContent;
                    string sExFooter = "";
                    sExFooter = string.Format(@"
                                <div style=""width:800px; margin:20 auto 0 140; font-family:TH SarabunPSK; font-size:27px; line-height: 1.2;""> 
                                <p>{0}<br/>
                                Tel: {1}<br/>
                                Fax: 0-­2832-­0391<br/>
								E-Mail: {2}</p>
                                </div>", objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_EN, objRequestorInfo.TELEPHONE, objRequestorInfo.EMAIL);

                    converter.Options.DisplayFooter = true;
                    converter.Footer.Height = 150;
                    converter.Footer.Add(new PdfHtmlSection(sExFooter, ""));
                }

                else
                {
                    sContent = sHeader + sContent;
                }



                //add page number
                PdfTextSection pdfPaging = new PdfTextSection(0, 10,
        "Page: {page_number} of {total_pages}",
        new System.Drawing.Font("Angsana New", 12));
                pdfPaging.HorizontalAlign = PdfTextHorizontalAlign.Center;
                //converter.Footer.Add(pdfPaging);

                HtmlToPdf exHtmlPage = new HtmlToPdf();
                exHtmlPage.Options.PdfPageSize = pageSize;
                exHtmlPage.Options.PdfPageOrientation = pdfOrientation;
                exHtmlPage.Options.WebPageWidth = 1024;
                exHtmlPage.Options.WebPageHeight = 0;
                exHtmlPage.Options.MaxPageLoadTime = 300;
                // footer settings
                exHtmlPage.Options.DisplayFooter = true;
                exHtmlPage.Footer.Height = 60;


                SelectPdf.GlobalProperties.LicenseKey = "XHdtfG5pbXxtaWp8bWtybHxvbXJtbnJlZWVl";

                // create a new pdf document converting the html string of the page
                SelectPdf.GlobalProperties.HtmlEngineFullPath = Extension.getValueFromTable("SiteSetting", "HtmlToPDFPath");


                PdfDocument doc = new PdfDocument();
                PdfDocument doc1 = new PdfDocument();
                PdfDocument doc2 = null;
                PdfDocument doc3 = null;
                SPSecurity.RunWithElevatedPrivileges(delegate
            {
                doc1 = converter.ConvertHtmlString(sContent);
                if (sDocType == "Im")
                {
                    doc.Append(doc1);
                }
                if (!string.IsNullOrWhiteSpace(History) && (ddl_DocType.SelectedValue != "Im" || string.IsNullOrWhiteSpace(vs_DocPDFPath)))
                {
                    doc2 = new PdfDocument();
                    doc2 = exHtmlPage.ConvertHtmlString(History);// + Assign);
                    doc.Append(doc2);
                }
                if (!string.IsNullOrWhiteSpace(Assign) && ddl_DocType.SelectedValue != "Im" && !string.IsNullOrWhiteSpace(vs_DocPDFPath) && sAction != "Print")
                {
                    doc3 = new PdfDocument();
                    doc3 = exHtmlPage.ConvertHtmlString(Assign);
                    doc.Append(doc3);
                }

            });

                #region | Merge PDF |

                if ((!string.IsNullOrWhiteSpace(vs_DocPDFPath) && sDocType == "Im") || (!string.IsNullOrWhiteSpace(vs_DocWordPath) && sDocType != "Im"))
                {
                    #region | Approver Signature Info |
                    DataTable dtApprovalSignature = new DataTable();
                    dtApprovalSignature.Columns.Add("EmpID");
                    dtApprovalSignature.Columns.Add("EmployeeName");
                    List<string> approvalList = new List<string>();
                    if (!vs_ApprovalList.DataTableIsNullOrEmpty())
                    {
                        for (int index = 0; index < vs_ApprovalList.Rows.Count; index++)
                        {
                            if (vs_ApprovalList.Rows.Count > 3 && index == 0)
                            {
                                index = vs_ApprovalList.Rows.Count - 3;
                            }
                            DataRow row = vs_ApprovalList.Rows[index];
                            DataRow target = dtApprovalSignature.NewRow();
                            if (row["EmpID"] != null)
                            {
                                approvalList.Add(row["EmpID"].ToString());
                                //approvalList.Add(row["EmployeeName"].ToString());
                                target["EmpID"] = row["EmpID"];
                                string fullName = "";
                                for (int i = 1; i < row["EmployeeName"].ToString().Split().Count(); i++)
                                {
                                    fullName += row["EmployeeName"].ToString().Split()[i];
                                    fullName += " ";
                                }
                                target["EmployeeName"] = fullName;
                                dtApprovalSignature.Rows.Add(target);
                                if (approvalList.Count == 3 || dtApprovalSignature.Rows.Count >= 3)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    //byte[] mergedPdf = null;
                    PdfDocument mergedPdf = null;
                    List<PdfDocument> listPdf = new List<PdfDocument>();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate
                        {
                            #region | 'Im' PageInfo Counter |
                            int infoPageCounter = 0;
                            iTextSharp.text.pdf.PdfReader readerDoc = null;
                            if (doc != null && sDocType.Equals("Im"))
                            {
                                readerDoc = new iTextSharp.text.pdf.PdfReader(doc.Save());
                                infoPageCounter = readerDoc.NumberOfPages;
                            }
                            #endregion
                            #region | History Page Counter |
                            iTextSharp.text.pdf.PdfReader readerDoc2 = null;
                            int historyPageCounter = 0;
                            if (doc2 != null)
                            {
                                List<TRNHistory> sHistory = db.TRNHistories.Where(x => (x.DocID == Convert.ToInt32(vs_PK))).ToList();
                                if (sHistory != null)
                                {
                                    if (sHistory.Count() > 1)
                                    {
                                        readerDoc2 = new iTextSharp.text.pdf.PdfReader(doc2.Save());
                                        historyPageCounter = readerDoc2.NumberOfPages;
                                    }
                                }
                            }
                            #endregion
                            #region | Assign Page Counter |
                            iTextSharp.text.pdf.PdfReader readerDoc3 = null;
                            int assignPageCounter = 0;
                            if (doc3 != null)
                            {
                                IQueryable<IGrouping<DateTime, TRNAssign>> sAssign = db.TRNAssigns.Where(x => (x.DocID == Convert.ToInt32(vs_PK))).GroupBy(x => x.ActionDate.Value);
                                if (sAssign != null)
                                {
                                    if (sAssign.Count() > 1)
                                    {
                                        readerDoc3 = new iTextSharp.text.pdf.PdfReader(doc3.Save());
                                        assignPageCounter = readerDoc3.NumberOfPages;
                                    }
                                }
                            }
                            #endregion
                            if (!string.IsNullOrWhiteSpace(vs_DocPDFPath))
                            {
                                byte[] docPdfByte = SharedRules.GetSPFile(vs_DocPDFPath);

                                MemoryStream DocPDFms = new MemoryStream(docPdfByte);
                                PdfDocument pdf = new PdfDocument(DocPDFms);

                                #region | Insert Signature Image for 'หนังสือรับเข้าจากภายนอก' (Im) |

                                if (lbl_Status.Text.Equals(Extension._Completed) && chk_AutoStamp.Checked && sDocType == "Im")
                                {
                                    float width = pdf.Pages[0].PageSize.Width;
                                    float height = pdf.Pages[0].PageSize.Height;
                                    bool isPortrait = width < height;

                                    int seqCounter = 1;
                                    List<KeyValuePair<string, string>> listSignature = new List<KeyValuePair<string, string>>();
                                    for (int index = dtApprovalSignature.Rows.Count - 1; index >= 0; index--)
                                    {
                                        string EmpID = dtApprovalSignature.Rows[index]["EmpID"].ToString();
                                        SignatureImage SignaturePath = listSignatureImages.FirstOrDefault(x => x.EmployeeID.Equals(EmpID));
                                        if (SignaturePath == null)
                                        {
                                            string sUserFullnName = dtApprovalSignature.Rows[index]["EmployeeName"].ToString();
                                            listSignature.Add(new KeyValuePair<string, string>(sUserFullnName, "string"));
                                        }
                                        else
                                        {
                                            listSignature.Add(new KeyValuePair<string, string>(SignaturePath.Path, "image"));
                                        }
                                        seqCounter++;
                                    }
                                    pdf = Extension.SelectPDFAddSignature(pdf, listSignature);
                                }
                                listPdf.Add(pdf);

                                #endregion

                                //int n = cover.NumberOfPages;
                                #region | Remove redundant page (History - Assign) |
                                //if (vs_Status.Equals("Completed"))
                                //{
                                //    n = cover.NumberOfPages - assignPageCounter;
                                //    if (n < 2 && assignPageCounter > 0)
                                //    {
                                //        n = cover.NumberOfPages - (assignPageCounter - 1);
                                //    }
                                //    if (n > 1 && !string.IsNullOrWhiteSpace(vs_PK) && historyPageCounter >= 0 && historyPageCounter < n && !vs_isExternalUpload)//&& author.Equals("TCB-EDocument System"))
                                //    {
                                //        n = n - historyPageCounter;
                                //    }
                                //    if (n > 1 && !string.IsNullOrWhiteSpace(vs_PK) && infoPageCounter >= 0 && infoPageCounter < n)
                                //    {
                                //        n = n - infoPageCounter;
                                //    }
                                //}
                                #endregion
                            }
                            // Add MS Word (Type: !Im)
                            if (!string.IsNullOrWhiteSpace(vs_DocWordPath) && (sAction != "Confirm" && lbl_DocumentNo.Text == "Auto Generate"))
                            {

                                string uploadResult = UpdateMSWord(false, lbl_DocumentNo.Text);
                                if (!string.IsNullOrWhiteSpace(uploadResult))
                                {
                                    throw new Exception(uploadResult);
                                }
                                string pdfFile = Extension.ChangeTypeToPDF(vs_DocWordPath, sDocLib, lbl_docSet.Text, Page);
                                byte[] docPdfByte = SharedRules.GetSPFile(pdfFile);

                                MemoryStream DocPDFms = new MemoryStream(docPdfByte);
                                PdfDocument pdfWord = new PdfDocument(DocPDFms);
                                listPdf.Add(pdfWord);
                            }

                            if (sDocType == "Im" && doc != null)
                            {
                                listPdf.Add(doc);
                            }
                            //Add History TBL && Assign TBL 
                            if (doc2 != null)
                            {
                                listPdf.Add(doc2);
                            }
                            if (doc3 != null)
                            {
                                listPdf.Add(doc3);
                            }
                            mergedPdf = Extension.MergeSelectPDF(listPdf);

                        });

                        if (sAction == "Cancel" || vs_Status == "Cancelled")
                        {
                            mergedPdf = Extension.SelectPDFAddWaterMark(mergedPdf, "เอกสารยกเลิก", false);
                        }
                        else if (sIsInternalOnlyStamp)
                        {
                            mergedPdf = Extension.SelectPDFAddWaterMark(mergedPdf, "เอกสารใช้ภายในเท่านั้น (Internal Use Only)", false);
                        }
                        //}
                    }
                    return Extension.SelectPDFAddPageNumber(mergedPdf, true).Save(); //Extension.AddPageNumbers(mergedPdf);
                }
                #endregion

                if (vs_Status.Equals(Extension._Cancelled))
                {
                    return Extension.SelectPDFAddWaterMark(doc, "เอกสารยกเลิก", false).Save();
                }
                else if (sIsInternalOnlyStamp)
                {
                    return Extension.SelectPDFAddWaterMark(doc, "เอกสารใช้ภายในเท่านั้น (Internal Use Only)", false).Save();
                }
                startConversion = false;
                return Extension.SelectPDFAddPageNumber(doc, true).Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                startConversion = false;
                throw ex;
                //return null;
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string sValidtionMsg = string.Empty;
                if (IsPassValidate(ref sValidtionMsg))
                {
                    Extension.LogWriter.Write(new Exception("B UpdateMSWord"));
                    UpdateMSWord(false, lbl_DocumentNo.Text);
                    Extension.LogWriter.Write(new Exception("E UpdateMSWord B Extension.PrintMemoDetail"));
                    byte[] pdf = Extension.PrintMemoDetail("Print", null, TRNDocumentAdapter(), Page);//PrintMemoDetail("Print", null);
                    Extension.LogWriter.Write(new Exception("E Extension.PrintMemoDetail"));
                    MemoryStream ms = new MemoryStream(pdf);
                    Response.ContentType = "application/pdf";
                    string sFileName = "Sample";
                    string PDF = string.Format("attachment;filename={0}.pdf", sFileName);
                    Response.AddHeader("content-disposition", PDF);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
                }
                else
                {
                    throw new Exception(sValidtionMsg);
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                Extension.MessageBox(Page, ex.Message);
            }
        }
        #endregion

        #region | Upload File |
        protected void fileUploadBtn_Click(object sender, EventArgs e)
        {
            //string DocLibName = lbl_Status.Text.Equals(Extension._WaitForAdminCentre) || lbl_Status.Text.Equals("Completed") ? "Document_Library" : "TempDocument";

            string DocLibName = "TempDocument";
            string uploadResult = "";

            Button btn = (Button)sender;
            bool isPrimary = btn.CommandName == "Y" ? true : false;
            if (isPrimary && gv_AttachDocumentFile.Rows.Count > 0)
            {
                Extension.MessageBox(this.Page, "Only 1 file can attach as Document File.");
            }
            else if (isPrimary ? doc_upload.HasFile : file_upload.HasFile)
            {
                if (isPrimary && doc_upload.PostedFile.ContentType != "application/pdf")
                {
                    Extension.MessageBox(this.Page, "Only files of type PDF is supported. Uploaded File Type: " + System.IO.Path.GetExtension(doc_upload.PostedFile.FileName)); return;
                }
                try
                {
                    //SharedRules objSPLIB = new SharedRules();

                    DataTable dtAttachlList = isPrimary ? vs_attachDocTable : vs_attachFileTable;
                    int iCurrentRowCount = dtAttachlList != null ? dtAttachlList.Rows.Count : 0;


                    if (!vs_isDocSetCreated)
                    {
                        string result = SharedRules.CreateDocumentSet(DocLibName, lbl_docSet.Text, null);
                        if (!string.IsNullOrEmpty(result)) { Extension.MessageBox(this.Page, result); }
                        vs_isDocSetCreated = true;
                        vs_isAttach = true;
                    }
                    string docFileName = lbl_Status.Text.Equals("Completed") || lbl_Status.Text.Equals(Extension._WaitForAdminCentre) ? lbl_docSet.Text + ".pdf" : doc_upload.FileName;
                    DocLibName = lbl_Status.Text.Equals("Completed") || lbl_Status.Text.Equals(Extension._WaitForAdminCentre) ? "Document_Library" : DocLibName;

                    docFileName = docFileName.ReplaeInvalidFileName();
                    if (isPrimary)
                    {
                        uploadResult = SharedRules.UploadFileIntoDocumentSet(DocLibName, lbl_docSet.Text, docFileName,
                            doc_upload.PostedFile.InputStream, "", vs_CurrentUserID);
                    }
                    else
                    {
                        uploadResult = SharedRules.UploadFileIntoDocumentSet(DocLibName, lbl_docSet.Text, file_upload.FileName.ReplaeInvalidFileName(),
                            file_upload.PostedFile.InputStream, "", vs_CurrentUserID);
                    }

                    if (!string.IsNullOrWhiteSpace(uploadResult))
                    {
                        throw new Exception(uploadResult);
                    }

                    DataRow dr = isPrimary ? vs_attachDocTable.NewRow() : vs_attachFileTable.NewRow();

                    dr["Sequence"] = isPrimary ? 1 : iCurrentRowCount + 1;
                    dr["AttachFile"] = isPrimary ? docFileName : file_upload.FileName.ReplaeInvalidFileName();
                    //dr["FileName"] = isPrimary ? txt_DocumentFileName.Text : txt_fileUploadName.Text;
                    //dr["ActorName"] = isPrimary && ddl_DocType.SelectedValue != "Im" && rdb_Type.SelectedValue != "Save" ? "-" : vs_CurrentUserID;
                    dr["ActorName"] = vs_CurrentUserID;
                    dr["AttachDate"] = DateTime.Now;
                    dr["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, lbl_docSet.Text.Trim(), isPrimary ? docFileName : file_upload.FileName.ReplaeInvalidFileName());
                    if (isPrimary)
                    {
                        vs_DocPDFPath = dr["AttachFilePath"].ToString();
                    }
                    dr["DocSetName"] = lbl_docSet.Text;
                    dr["DocLibName"] = DocLibName;
                    dr["ActorID"] = vs_CurrentUserID;
                    dr["IsPrimary"] = isPrimary ? "Y" : "N";

                    if (isPrimary)
                    {
                        vs_isDocAttach = true;
                        vs_attachDocTable.Rows.Add(dr);
                        gv_AttachDocumentFile.DataSource = vs_attachDocTable;
                        gv_AttachDocumentFile.DataBind();

                        //txt_DocumentFileName.Text = "";
                    }
                    else
                    {
                        vs_attachFileTable.Rows.Add(dr);
                        gv_AttachFile.DataSource = vs_attachFileTable;
                        gv_AttachFile.DataBind();
                        //txt_fileUploadName.Text = "";
                    }

                    if (lbl_Status.Text == "Completed" || lbl_Status.Text == Extension._WaitForAdminCentre)
                    {
                        if (isPrimary)
                        {
                            vs_isExternalUpload = true;
                        }
                        Extension.MessageBox(Page, "บันทึกไฟล์เอกสารเรียบร้อยแล้ว");
                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "disableTextEditing();", true);
                    }
                    else
                    {
                        Extension.MessageBox(Page, "อัพโหลดไฟล์เอกสารเรียบร้อยแล้ว");
                    }

                }
                catch (Exception ex)
                {
                    Extension.MessageBox(this.Page, "Error: " + ex.Message.ToString());
                    Extension.LogWriter.Write(ex);
                }
            }
            else
            {
                Extension.MessageBox(this.Page, "No attach file.");
            }
        }
        protected void gv_AttachFile_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string DocLibName = "TempDocument";
            DataTable dt_attach = vs_attachFileTable;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dt_attach != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    string actorID = dt_attach.Rows[iRowIndex]["ActorName"].ToString();
                    if (actorID.Equals(vs_CurrentUserID))
                    {
                        string fileName = dt_attach.Rows[iRowIndex]["AttachFile"].ToString();
                        SharedRules sp = new SharedRules();
                        //sp.DeleteDocumentByURL(DocLibName, lbl_docSet.Text, fileName);

                        dt_attach.Rows[iRowIndex].Delete();
                        int iSequence = 1;
                        foreach (DataRow dr in dt_attach.Rows)
                        {
                            dr["Sequence"] = iSequence;
                            iSequence++;
                        }
                        vs_attachFileTable = dt_attach;
                        gv_AttachFile.DataSource = dt_attach;
                        gv_AttachFile.DataBind();

                    }
                    else
                    {
                        Extension.MessageBox(this.Page, "You do not have permission to delete this attached file");
                    }


                }
            }
        }
        protected void gv_AttachFile_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                #region | Employee Name |

                Label lblActorName = (Label)e.Row.FindControl("lblEmpName");
                object EmpID = DataBinder.Eval(e.Row.DataItem, "ActorName");
                if (lblActorName != null)
                {
                    if (EmpID != null && EmpID != DBNull.Value)
                    {
                        SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(this.Page,
                            EmpID.ToString());
                        if (empData != null)
                        {
                            string empNameTH = string.Format("{0}{1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH,
                                empData.LASTNAME_TH);
                            string empNameEN = string.Format("{0}{1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN,
                                empData.LASTNAME_EN);

                            lblActorName.Text = vs_Lang == "TH" ? empNameTH : empNameEN;
                        }
                    }
                    else
                    {
                        Extension.MessageBox(this.Page, "EmpID is Null");
                    }
                }

                #endregion
            }
        }
        protected void gv_AttachDocumentFile_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string DocLibName = "TempDocument";
            DataTable dt_attach = vs_attachDocTable;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dt_attach != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    string actorID = dt_attach.Rows[iRowIndex]["ActorName"].ToString();
                    if (actorID.Equals(vs_CurrentUserID) || (lbl_Status.Text.Equals("Completed") || lbl_Status.Text.Equals(Extension._WaitForAdminCentre)))
                    {
                        string fileName = dt_attach.Rows[iRowIndex]["AttachFile"].ToString();
                        DocLibName = dt_attach.Rows[iRowIndex]["DocLibName"].ToString();

                        SharedRules sp = new SharedRules();
                        if (!(lbl_Status.Text.Equals("Completed") || lbl_Status.Text.Equals(Extension._WaitForAdminCentre)))
                        {
                            //sp.DeleteDocumentByURL(DocLibName, lbl_docSet.Text, fileName);
                        }

                        dt_attach.Rows[iRowIndex].Delete();
                        int iSequence = 1;
                        foreach (DataRow dr in dt_attach.Rows)
                        {
                            dr["Sequence"] = iSequence;
                            iSequence++;
                        }
                        vs_DocPDFPath = "";
                        vs_attachDocTable = dt_attach;
                        gv_AttachDocumentFile.DataSource = dt_attach;
                        gv_AttachDocumentFile.DataBind();

                    }
                    else
                    {
                        Extension.MessageBox(this.Page, "You do not have permission to delete this attached file");
                    }


                }
            }
        }
        protected void gv_AttachDocumentFile_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                #region | Employee Name |

                Label lblActorName = (Label)e.Row.FindControl("lblEmpName");
                object EmpID = DataBinder.Eval(e.Row.DataItem, "ActorName");
                if (lblActorName != null)
                {
                    if (EmpID != null && EmpID != DBNull.Value)
                    {
                        SpecificEmployeeData.RootObject empData = Extension.GetSpecificEmployeeFromTemp(this.Page,
                            EmpID.ToString());
                        if (empData != null)
                        {
                            string empNameTH = string.Format("{0}{1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH,
                                empData.LASTNAME_TH);
                            string empNameEN = string.Format("{0}{1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN,
                                empData.LASTNAME_EN);

                            lblActorName.Text = vs_Lang == "TH" ? empNameTH : empNameEN;
                        }
                    }
                    else
                    {
                        Extension.MessageBox(this.Page, "EmpID is Null");
                    }
                }

                #endregion
            }
        }
        #endregion

        protected void btn_DownloadTemplate_Click(object sender, EventArgs e)
        {
            string docNo = string.Empty;
            if (lbl_DocumentNo.Text != "Auto Generate")
            {
                docNo = lbl_DocumentNo.Text;
            }
            UpdateMSWord(true, docNo);
        }
        private string UpdateMSWord(bool isRedirect, string sDocNo)
        {
            //Using OpenXML
            try
            {
                string sValidtionWord = string.Empty;
                if (IsWordPassValidate(ref sValidtionWord))
                {
                    #region | MSWord Template |
                    try
                    {
                        string sValidtionMsg = string.Empty;

                        //if (IsPassValidate(ref sValidtionMsg))
                        if (ddl_DocType.SelectedIndex > 0)
                        {
                            string oDocLib = "TempDocument";
                            if (vs_Status == "Completed" || vs_Status == Extension._WaitForAdminCentre)
                            {
                                oDocLib = "Document_Library";
                            }
                            string oDocName = string.Format("PIMEdocumentTemplate-[{0}].docx", ddl_DocType.SelectedValue);

                            if (!vs_isDocSetCreated)
                            {
                                string result = SharedRules.CreateDocumentSet(oDocLib, lbl_docSet.Text, null);
                                if (!string.IsNullOrEmpty(result)) { Extension.MessageBox(this.Page, result); }
                                vs_isDocSetCreated = true;
                            }
                            string finalPath = vs_DocWordPath;
                            //Get MS Word path if exist in Docset
                            if (string.IsNullOrWhiteSpace(finalPath))
                            {
                                finalPath = SharedRules.GetDocSetFilePathIfExist(oDocLib, lbl_docSet.Text, oDocName);
                                vs_DocWordPath = finalPath;
                            }
                            //copy Template to DocSet
                            if (string.IsNullOrWhiteSpace(finalPath))
                            {
                                //if template not Created yet
                                if (string.IsNullOrWhiteSpace(SharedRules.CopyFileFromListToDocSet("MstTemplate", oDocName, oDocLib, lbl_docSet.Text)))
                                {
                                    vs_DocWordPath = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), oDocLib, lbl_docSet.Text, oDocName);

                                    rdb_Type.Enabled = false;
                                    ddl_DocType.Enabled = false;
                                    rdb_Category.Enabled = false;

                                    btn_DownloadTemplate.Text = "Edit Document";
                                    btn_DownloadTemplate.CommandName = "Edit";
                                    //btn_DownloadTemplate.PostBackUrl = finalPath;
                                }
                            }
                            #region | For .NetFramework bug |
                            //MemoryStream m = new MemoryStream();
                            //System.IO.Packaging.Package p = System.IO.Packaging.Package.Open(m, System.IO.FileMode.Create);
                            //p.Close();
                            //m.Close();
                            #endregion

                            byte[] data = Extension.getMSWord(vs_DocWordPath);
                            Extension.LogWriter.Write(new Exception("data Count: " + data.LongCount().ToString()));
                            string[] referenceDocument = new string[vs_DtRefDoc.Rows.Count];

                            #region | Convert Form Data to Object |
                            #region | objDoc |
                            TRNDocument objDoc = TRNDocumentAdapter();//new TRNDocument();
                            #region | Backup |
                            //string docType = ddl_DocType.SelectedValue;
                            //if (docType == "Other")
                            //{
                            //    objDoc.OtherDocType = ddl_otherDocType.SelectedValue;
                            //}
                            //else if (docType == "Im")
                            //{
                            //    objDoc.To = txt_to.Text;
                            //    objDoc.CC = txt_CC.Text;
                            //    objDoc.RecieveDocumentNo = txt_RecieveDocNo.Text;
                            //    objDoc.DocumentSource = txt_Source.Text;
                            //    if (!string.IsNullOrWhiteSpace(txt_DocumentDate.Text))
                            //    {
                            //        objDoc.DocumentDate = DateTime.Parse(txt_DocumentDate.Text, new CultureInfo("en-GB"));
                            //    }
                            //    if (!string.IsNullOrWhiteSpace(txt_DocumentRecieve.Text))
                            //    {
                            //        objDoc.DocumentRecieveDate = DateTime.Parse(txt_DocumentRecieve.Text, new CultureInfo("en-GB"));
                            //    }
                            //    objDoc.AttachFilePath = vs_DocPDFPath;
                            //}
                            //else if (docType == "M")
                            //{
                            //    if (!string.IsNullOrWhiteSpace(lbl_sendToID.Text))
                            //    {
                            //        objDoc.SendToID = lbl_sendToID.Text;
                            //    }
                            //}
                            //else if (docType == "Ex")
                            //{
                            //    if (!vs_DtRefDoc.DataTableIsNullOrEmpty())
                            //    {
                            //        DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                            //        int iCounter = 0;
                            //        foreach (DataRow dr in vs_DtRefDoc.Rows)
                            //        {
                            //            string sRefDocID = dr["RefDocID"].ToString();
                            //            int iRefDocID = 0;
                            //            if (Int32.TryParse(sRefDocID, out iRefDocID) && iRefDocID > 0)
                            //            {
                            //                try
                            //                {
                            //                    TRNDocument objRefDoc = db.TRNDocuments.SingleOrDefault(x => x.DocID == iRefDocID);
                            //                    if (objRefDoc != null)
                            //                    {
                            //                        //objDoc.DocumentSource = Extension.GenerateDocumentNo(objRefDoc.DocNo, Convert.ToInt32(objRefDoc.FromDepartmentID), objRefDoc.Category, objRefDoc.DocTypeCode, Page);
                            //                        referenceDocument[iCounter] = Extension.GenerateDocumentNo(objRefDoc.DocNo, Convert.ToInt32(objRefDoc.FromDepartmentID), objRefDoc.Category, objRefDoc.DocTypeCode, Page);
                            //                        switch (objRefDoc.DocTypeCode)
                            //                        {
                            //                            case "Im":
                            //                                referenceDocument[iCounter] = string.Format("{0} {1}", objRefDoc.RecieveDocumentNo ?? "", objRefDoc.Title ?? "");
                            //                                break;
                            //                            case "L":
                            //                                referenceDocument[iCounter] = objRefDoc.Title ?? "";
                            //                                break;
                            //                            case "R":
                            //                                referenceDocument[iCounter] = objRefDoc.Title ?? "";
                            //                                break;
                            //                            case "M":
                            //                                referenceDocument[iCounter] = string.Format("{0} {1}", referenceDocument[iCounter], objRefDoc.Title ?? "");
                            //                                break;
                            //                            case "Other":
                            //                                referenceDocument[iCounter] = string.Format("{0} {1}", referenceDocument[iCounter], objRefDoc.Title ?? "");
                            //                                break;
                            //                            default:
                            //                                break;
                            //                        }



                            //                        if (string.IsNullOrWhiteSpace(referenceDocument[iCounter]))
                            //                        {
                            //                            referenceDocument[iCounter] = string.Format("{0}-{1}-{2} {3}", objRefDoc.Category, objRefDoc.DocTypeCode, objRefDoc.FromDepartmentName, objRefDoc.Title);
                            //                        }
                            //                        iCounter++;
                            //                    }
                            //                }
                            //                catch (Exception ex) { }
                            //            }
                            //        }

                            //    }
                            //}
                            //if (!string.IsNullOrWhiteSpace(sDocNo))
                            //{
                            //    objDoc.DocNo = sDocNo;
                            //}
                            //objDoc.Status = vs_Status;
                            //objDoc.Description = txt_DocDescription.Text;
                            //objDoc.DocTypeCode = ddl_DocType.SelectedValue;
                            //objDoc.CreatorID = Convert.ToInt32(lbl_CreatorID.Text.Trim());
                            //objDoc.CreatorDepartmentID = Convert.ToInt32(ddl_CreatorDepartment.SelectedValue);
                            //objDoc.CreatorSubDepartmentID = Convert.ToInt32(ddl_CreatorSubDepartment.SelectedValue);
                            //objDoc.CreatorPositionID = Convert.ToInt32(ddl_CreatorPosition.SelectedValue);
                            //objDoc.RequestorID = Convert.ToInt32(lbl_RequestorID.Text.Trim());
                            //objDoc.RequestorDepartmentID = Convert.ToInt32(ddl_RequestorDepartment.SelectedValue);
                            //objDoc.RequestorSubDepartmentID = Convert.ToInt32(ddl_RequestorSubDepartment.SelectedValue);
                            //objDoc.RequestorPositionID = Convert.ToInt32(ddl_RequestorPosition.SelectedValue);
                            //objDoc.Type = rdb_Type.SelectedValue.ToString();
                            //objDoc.Category = rdb_Category.SelectedValue.ToString();
                            //objDoc.To = txt_to.Text;
                            //objDoc.CC = txt_CC.Text;
                            //objDoc.Attachment = txt_Attachment.Text;
                            //objDoc.Title = txt_title.Text;
                            //objDoc.FromDepartmentID = Convert.ToInt32(hdn_FromDepartmentID.Text);
                            //objDoc.FromDepartmentName = txt_FromDepartment.Text;
                            //objDoc.ToDepartmentID = hdn_ToDepartment.Text;
                            //objDoc.ToDepartmentName = txt_ToDepartment.Text;
                            //objDoc.Priority = ddl_Priority.SelectedValue;
                            //objDoc.ModifiedBy = Convert.ToInt32(vs_CurrentUserID);
                            //objDoc.ModifiedDate = DateTime.Now;
                            //objDoc.IsOccurBySubDepartment = vs_isOccurBySubDepartment;
                            //if (!string.IsNullOrWhiteSpace(txt_deadline.Text))
                            //{
                            //    objDoc.Deadline = DateTime.Parse(txt_deadline.Text, new CultureInfo("en-GB"));
                            //}
                            //objDoc.DOA = chk_DOA.Checked ? "Y" : "N";
                            //objDoc.AutoStamp = chk_AutoStamp.Checked ? "Y" : "N";
                            //if (vs_isLastApproval || hdn_ApproveDate.Value != "")
                            //{
                            //    if (string.IsNullOrWhiteSpace(hdn_ApproveDate.Value))
                            //    {
                            //        hdn_ApproveDate.Value = DateTime.Now.ToString("dd/MM/yyyy", _ctli);
                            //    }
                            //    objDoc.ApproveDate = DateTime.ParseExact(hdn_ApproveDate.Value, "dd/MM/yyyy", _ctli);
                            //}


                            //if (chk_DOA.Checked)
                            //{
                            //    if (objDoc.Type.ToLower().Equals("submit"))
                            //    {
                            //        decimal dcm;
                            //        if (decimal.TryParse(txt_Amount.Text, out dcm))
                            //        {
                            //            objDoc.Amount = dcm;
                            //        }
                            //        else
                            //        {
                            //            objDoc.Amount = 0;
                            //        }
                            //        objDoc.CostCenter = lbl_CostCenter.Text;
                            //    }
                            //    else
                            //    {
                            //        objDoc.CostCenter = "";
                            //        objDoc.Amount = 0;
                            //    }
                            //}
                            //objDoc.PermissionType = ddl_Permission.SelectedValue;
                            //objDoc.Comment = txt_AdditionalComment.Text; 
                            #endregion
                            #endregion
                            #region | objApp |
                            //get last approval info
                            DataTable dtApp = vs_ApprovalList.Copy();

                            TRNApprover objApp = new TRNApprover();
                            if (!dtApp.DataTableIsNullOrEmpty())
                            {
                                DataRow dr = dtApp.Rows[dtApp.Rows.Count - 1];
                                objApp.EmpID = dr["EmpID"].ToString();

                                SpecificEmployeeData.RootObject specApp = Extension.GetSpecificEmployeeFromTemp(Page, objApp.EmpID.ToString());
                                objApp.EmployeeName = string.Format("{0}{1} {2}", specApp.PREFIX_TH, specApp.FIRSTNAME_TH, specApp.LASTNAME_TH);

                                objApp.DepartmentID = Convert.ToInt32(dr["DepartmentID"].ToString());
                                objApp.DepartmentName = dr["DepartmentName"].ToString();
                                objApp.SubDepartmentID = Convert.ToInt32(dr["SubDepartmentID"].ToString());
                                objApp.SubDepartmentName = dr["SubDepartmentName"].ToString();
                                objApp.PositionID = Convert.ToInt32(dr["PositionID"].ToString());
                                objApp.PositionName = dr["PositionName"].ToString();
                                if (ddl_DocType.SelectedValue == "Ex")
                                {
                                    objApp.PositionName += "สถาบันการจัดการปัญญาภิวัฒน์";
                                }

                                if (ddl_DocType.SelectedValue == "ExEN")
                                {
                                    DataTable specDeptEN = Extension.GetSpecificDepartmentDataFromTemp(Page, objApp.DepartmentID.ToString());
                                    DataTable specPosEN = Extension.GetSpecificPositionDataFromTemp(Page, objApp.PositionID.ToString());
                                    objDoc.FromDepartmentName = Extension.GetSpecificDepartmentDataFromTemp(Page, objDoc.FromDepartmentID.ToString()).Rows[0]["DEPARTMENT_NAME_EN"].ToString();
                                    objApp.EmployeeName = string.Format("{0}{1} {2}", specApp.PREFIX_EN, specApp.FIRSTNAME_EN, specApp.LASTNAME_EN);
                                    objApp.DepartmentName = specDeptEN.Rows[0]["DEPARTMENT_NAME_EN"].ToString();
                                    objApp.PositionName = specPosEN.Rows[0]["POSITION_NAME_EN"].ToString();
                                }
                            }
                            #endregion
                            #endregion

                            //Add data from screen to Word
                            byte[] bArr = Extension.UpdateMSWord(Page, data, objDoc, objApp, referenceDocument);

                            //Upload word back to sharepoint
                            string uploadResult = SharedRules.UploadFileIntoDocumentSet(oDocLib, lbl_docSet.Text, oDocName, new MemoryStream(bArr), "", "");
                            if (uploadResult.Contains("locked for shared"))
                            {
                                throw new Exception("This Document is in editing mode, Please close all editing documents.");
                            }

                            //finalPath = UpdateWordInfo(vs_DocWordPath);
                            if (isRedirect)
                            {
                                Extension.RedirectNewTab(Page, vs_DocWordPath);
                            }
                        }
                        else
                        {
                            Extension.MessageBox(Page, "กรุณาเลือกประเภทเอกสาร");
                        }
                    }
                    catch (Exception ex)
                    {
                        Extension.MessageBox(Page, ex.Message.ToString());
                        Extension.LogWriter.Write(ex);
                        return ex.Message.ToString();
                    }
                    #endregion
                }
                else
                {
                    Extension.MessageBox(Page, sValidtionWord);
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            return "";
        }


        private TRNDocument TRNDocumentAdapter()
        {
            string[] referenceDocument = new string[vs_DtRefDoc.Rows.Count];
            string sDocNo = lbl_DocumentNo.Text;

            TRNDocument objDoc = new TRNDocument();
            string docType = ddl_DocType.SelectedValue;

            objDoc.AttachWordPath = vs_DocWordPath;
            if (docType == "Other")
            {
                objDoc.OtherDocType = ddl_otherDocType.SelectedValue;
            }
            else if (docType == "Im")
            {
                objDoc.AttachFilePath = vs_DocPDFPath;
                objDoc.To = txt_to.Text;
                objDoc.CC = txt_CC.Text;
                objDoc.RecieveDocumentNo = txt_RecieveDocNo.Text;
                objDoc.DocumentSource = txt_Source.Text;
                if (!string.IsNullOrWhiteSpace(txt_DocumentDate.Text))
                {
                    objDoc.DocumentDate = DateTime.Parse(txt_DocumentDate.Text, new CultureInfo("en-GB"));
                }
                if (!string.IsNullOrWhiteSpace(txt_DocumentRecieve.Text))
                {
                    objDoc.DocumentRecieveDate = DateTime.Parse(txt_DocumentRecieve.Text, new CultureInfo("en-GB"));
                }
                objDoc.AttachFilePath = vs_DocPDFPath;
            }
            else if (docType == "M")
            {
                if (!string.IsNullOrWhiteSpace(lbl_sendToID.Text))
                {
                    objDoc.SendToID = lbl_sendToID.Text;
                }
            }
            else if (docType == "Ex")
            {
                if (!vs_DtRefDoc.DataTableIsNullOrEmpty())
                {
                    DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                    int iCounter = 0;
                    foreach (DataRow dr in vs_DtRefDoc.Rows)
                    {
                        string sRefDocID = dr["RefDocID"].ToString();
                        int iRefDocID = 0;
                        if (Int32.TryParse(sRefDocID, out iRefDocID) && iRefDocID > 0)
                        {
                            try
                            {
                                TRNDocument objRefDoc = db.TRNDocuments.SingleOrDefault(x => x.DocID == iRefDocID);
                                if (objRefDoc != null)
                                {
                                    //objDoc.DocumentSource = Extension.GenerateDocumentNo(objRefDoc.DocNo, Convert.ToInt32(objRefDoc.FromDepartmentID), objRefDoc.Category, objRefDoc.DocTypeCode, Page);
                                    referenceDocument[iCounter] = Extension.GenerateDocumentNo(objRefDoc.DocNo, Convert.ToInt32(objRefDoc.FromDepartmentID), objRefDoc.Category, objRefDoc.DocTypeCode, Page);
                                    switch (objRefDoc.DocTypeCode)
                                    {
                                        case "Im":
                                            referenceDocument[iCounter] = string.Format("{0} {1}", objRefDoc.RecieveDocumentNo ?? "", objRefDoc.Title ?? "");
                                            break;
                                        case "L":
                                            referenceDocument[iCounter] = objRefDoc.Title ?? "";
                                            break;
                                        case "R":
                                            referenceDocument[iCounter] = objRefDoc.Title ?? "";
                                            break;
                                        case "M":
                                            referenceDocument[iCounter] = string.Format("{0} {1}", referenceDocument[iCounter], objRefDoc.Title ?? "");
                                            break;
                                        case "Other":
                                            referenceDocument[iCounter] = string.Format("{0} {1}", referenceDocument[iCounter], objRefDoc.Title ?? "");
                                            break;
                                        default:
                                            break;
                                    }



                                    if (string.IsNullOrWhiteSpace(referenceDocument[iCounter]))
                                    {
                                        referenceDocument[iCounter] = string.Format("{0}-{1}-{2} {3}", objRefDoc.Category, objRefDoc.DocTypeCode, objRefDoc.FromDepartmentName, objRefDoc.Title);
                                    }
                                    iCounter++;
                                }
                            }
                            catch (Exception ex) { }
                        }
                    }

                }
            }
            if (!string.IsNullOrWhiteSpace(sDocNo))
            {
                objDoc.DocNo = sDocNo;
            }
            objDoc.DocSet = lbl_docSet.Text;
            objDoc.DocLib = lbl_DocumentNo.Text.Equals("Auto Generate") ? "TempDocument" : objDoc.DocSet;
            objDoc.Status = vs_Status;
            objDoc.Description = txt_DocDescription.Text;
            objDoc.DocTypeCode = ddl_DocType.SelectedValue;
            objDoc.CreatorID = lbl_CreatorID.Text.Trim();
            objDoc.CreatorDepartmentID = Convert.ToInt32(ddl_CreatorDepartment.SelectedValue);
            objDoc.CreatorSubDepartmentID = Convert.ToInt32(ddl_CreatorSubDepartment.SelectedValue);
            objDoc.CreatorPositionID = Convert.ToInt32(ddl_CreatorPosition.SelectedValue);
            objDoc.RequestorID = lbl_RequestorID.Text.Trim();
            objDoc.RequestorDepartmentID = Convert.ToInt32(ddl_RequestorDepartment.SelectedValue);
            objDoc.RequestorSubDepartmentID = Convert.ToInt32(ddl_RequestorSubDepartment.SelectedValue);
            objDoc.RequestorPositionID = Convert.ToInt32(ddl_RequestorPosition.SelectedValue);
            objDoc.Type = rdb_Type.SelectedValue.ToString();
            objDoc.Category = rdb_Category.SelectedValue.ToString();
            objDoc.To = txt_to.Text;
            objDoc.CC = txt_CC.Text;
            objDoc.Attachment = txt_Attachment.Text;
            objDoc.Title = txt_title.Text;
            objDoc.SubTitle = txt_subtitle.Text;
            objDoc.FromDepartmentID = Convert.ToInt32(hdn_FromDepartmentID.Text);
            objDoc.FromDepartmentName = txt_FromDepartment.Text;
            objDoc.ToDepartmentID = hdn_ToDepartment.Text;
            objDoc.ToDepartmentName = txt_ToDepartment.Text;
            objDoc.Priority = ddl_Priority.SelectedValue;
            objDoc.ModifiedBy = vs_CurrentUserID;
            objDoc.ModifiedDate = DateTime.Now;
            objDoc.IsOccurBySubDepartment = vs_isOccurBySubDepartment;
            if (!string.IsNullOrWhiteSpace(txt_deadline.Text))
            {
                objDoc.Deadline = DateTime.Parse(txt_deadline.Text, new CultureInfo("en-GB"));
            }
            objDoc.DOA = chk_DOA.Checked ? "Y" : "N";
            objDoc.AutoStamp = chk_AutoStamp.Checked ? "Y" : "N";
            if (vs_isLastApproval || hdn_ApproveDate.Value != "")
            {
                if (string.IsNullOrWhiteSpace(hdn_ApproveDate.Value))
                {
                    hdn_ApproveDate.Value = DateTime.Now.ToString("dd/MM/yyyy", _ctli);
                }
                objDoc.ApproveDate = DateTime.ParseExact(hdn_ApproveDate.Value, "dd/MM/yyyy", _ctli);
            }


            if (chk_DOA.Checked)
            {
                if (objDoc.Type.ToLower().Equals("submit"))
                {
                    decimal dcm;
                    if (decimal.TryParse(txt_Amount.Text, out dcm))
                    {
                        objDoc.Amount = dcm;
                    }
                    else
                    {
                        objDoc.Amount = 0;
                    }
                    objDoc.CostCenter = lbl_CostCenter.Text;
                }
                else
                {
                    objDoc.CostCenter = "";
                    objDoc.Amount = 0;
                }
            }
            objDoc.PermissionType = ddl_Permission.SelectedValue;
            objDoc.Comment = txt_AdditionalComment.Text;


            return objDoc;
        }

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            try
            {
                DataClassesDataAccessDataContext DataContext = new DataClassesDataAccessDataContext(vs_ConnectionString);
                if (DataContext.Connection.State == ConnectionState.Open)
                {
                    DataContext.Connection.Close();
                    DataContext.Connection.Open();
                }
                else
                {
                    DataContext.Connection.Open();
                    System.Data.Common.DbTransaction dbTrabs = DataContext.Connection.BeginTransaction();
                    DataContext.Transaction = dbTrabs;
                }

                TRNDocument objDocument = new TRNDocument();
                if (!string.IsNullOrEmpty(vs_PK))
                {
                    objDocument = DataContext.TRNDocuments.SingleOrDefault(x => x.DocID.ToString().Equals(vs_PK));


                    #region | Send Email | 
                    Extension.SendEmailTemplate(objDocument.Status, objDocument.WaitingFor.ToString(), objDocument.WaitingForDeptID.ToString(), "Approve test", "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID);
                    //Extension.SendEmailTemplate("ReplyComment", "9000007", objDocument.WaitingForDeptID.ToString(), "Send mail test", "", "", objDocument.DocID.ToString(), objDocument, Page, vs_CurrentUserID, true);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
    }
}
