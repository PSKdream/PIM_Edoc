using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace PIMEdoc_CR.Delegate
{
    public partial class DelegateUserControl : UserControl
    {
        #region | View State |
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
        private DataTable vs_DocTypeTable
        {
            get
            {
                return (DataTable)ViewState["vs_DocTypeTable"];
            }
            set
            {
                ViewState["vs_DocTypeTable"] = value;
            }
        }
        private DataTable vs_dt_PendingDocType
        {
            get
            {
                if (ViewState["vs_dt_PendingDocType"] == null)
                {
                    DataTable dt_PendingTask = new DataTable();
                    dt_PendingTask.Columns.Add("Selected", typeof(Boolean));
                    dt_PendingTask.Columns.Add("DocTypeCode", typeof(String));
                    dt_PendingTask.Columns.Add("DocTypeName", typeof(String));
                    dt_PendingTask.Columns.Add("Count", typeof(Int32));
                    return dt_PendingTask;
                }

                return (DataTable)ViewState["vs_dt_PendingDocType"];
            }
            set
            {
                ViewState["vs_dt_PendingDocType"] = value;
            }
        }
        private DataTable vs_dt_PendingDocNo
        {
            get
            {
                if (ViewState["vs_dt_PendingDocNo"] == null)
                {
                    DataTable dt_PendingTask = new DataTable();
                    dt_PendingTask.Columns.Add("Selected", typeof(Boolean));
                    dt_PendingTask.Columns.Add("DocID", typeof(String));
                    dt_PendingTask.Columns.Add("DocNO", typeof(String));
                    dt_PendingTask.Columns.Add("Category", typeof(String));
                    dt_PendingTask.Columns.Add("DocTypeCode", typeof(String));
                    dt_PendingTask.Columns.Add("Title", typeof(String));
                    dt_PendingTask.Columns.Add("CreateDate", typeof(DateTime));
                    dt_PendingTask.Columns.Add("Requestor", typeof(String));
                    dt_PendingTask.Columns.Add("Status", typeof(String));
                    dt_PendingTask.Columns.Add("Amount", typeof(String));
                    return dt_PendingTask;
                }

                return (DataTable)ViewState["vs_dt_PendingDocNo"];
            }
            set
            {
                ViewState["vs_dt_PendingDocNo"] = value;
            }
        }

        System.Globalization.CultureInfo ctli = new System.Globalization.CultureInfo("en-GB");
        System.Globalization.CultureInfo ctliTH = new System.Globalization.CultureInfo("th-TH");
        #endregion

        #region | Pageload & Initial |
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    string logonName = SharedRules.LogonName();
                    vs_CurrentUserLoginName = logonName;
                    vs_CurrentUserID = SharedRules.FindUserID(logonName, this.Page);
                    if (string.IsNullOrEmpty(vs_CurrentUserID))
                    {
                        vs_CurrentUserID = Request.QueryString["USERID"] != null
                            ? Request.QueryString["USERID"].ToString()
                            : "5050108";
                    }
                    InitialData();
                    Extension.GetDepartmentData(Page);
                    Extension.GetEmployeeData(Page);
                    Extension.GetPositionData(Page);
                }
                catch (Exception ex)
                {

                }
            }
        }
        protected void InitialData()
        {
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
            vs_isDocSetCreated = false;
            vs_isAttach = false;
            vs_isITAdmin = false;
            lbl_Docset.Text = Guid.NewGuid().ToString().Replace("-", "");

            string spGroup = "Admin_ITEdoc";
            if (!string.IsNullOrEmpty(spGroup))
            {
                List<string> userList = SharedRules.GetAllUserInGroup(spGroup);
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


            btn_UploadAttachFile.Attributes.Add("Style", "Display:none");
            up_Attachment.Attributes["onchange"] = "UploadFile(this)";

            vs_DocTypeTable = SharedRules.GetList("MstDocumentType", @"<View><Query><Where><And><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq><Eq><FieldRef Name='Level' /><Value Type='Number'>0</Value></Eq></And></Where></Query></View>");
            //vs_DocTypeTable = vs_DocTypeTable.AsEnumerable().Where(r => r.Field<double>("IsActive") == 1 && r.Field<double>("Level") == 0).CopyToDataTable();

            // Initial Login user as an Approver
            hdn_ApproverID.Value = vs_CurrentUserID;
            if (!string.IsNullOrWhiteSpace(hdn_ApproverID.Value))
            {
                var emp = Extension.GetSpecificEmployeeFromTemp(Page, vs_CurrentUserID);
                var dep = emp.RESULT.First();
                lbl_Approver.Text = string.Format("{0}{1} {2}", emp.PREFIX_TH, emp.FIRSTNAME_TH, emp.LASTNAME_TH);
                BindingDepartmentDDL(dep.DEPARTMENT_ID, dep.SUBDEPARTMENT_ID, dep.POSITION_TD);
                ddl_Department.SelectedValue = dep.DEPARTMENT_ID;
                ddl_SubDepartment.SelectedValue = dep.SUBDEPARTMENT_ID;
                ddl_Position.SelectedValue = dep.POSITION_TD;
                //BindingDelegateGV();
            }
            //if not Admin_ITEDoc
            if (!vs_isITAdmin)
            {
                lkb_SearchApprover.Enabled = false;
            }

            #region | Initial DataTable |
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
            vs_attachFileTable = dtAttachFile;
            vs_isAttach = true;

            #endregion
        }
        #endregion

        #region | Infomation Panel |
        #region | Dept & SubDept & Pos DDL |
        protected void BindingDepartmentDDL(string sDepID, string sSubDepID, string sPositionID)
        {
            if (!string.IsNullOrWhiteSpace(hdn_ApproverID.Value))
            {
                //Merge Table
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                List<TRNEmployeeExtension> listEmpEx = db.TRNEmployeeExtensions.Where(x => x.EMPLOYEEID == hdn_ApproverID.Value).ToList();
                var emp = Extension.GetSpecificEmployeeFromTemp(Page, hdn_ApproverID.Value);
                DataTable dtDepartment = Extension.ListToDataTable(emp.RESULT.ToList());
                if (emp != null)
                {
                    foreach (var item in listEmpEx)
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
                        emp.RESULT.Add(result);
                    }
                    DepartmentSelectedChanged(emp.RESULT.ToList());
                    if (!string.IsNullOrWhiteSpace(sDepID))
                    {
                        ddl_Department.SelectedValue = sDepID.ToString();
                        BindingDelegateGV();
                    }

                    SubDepartmentSelectedChanged(emp.RESULT.Where(x => x.DEPARTMENT_ID == ddl_Department.SelectedValue).ToList());
                    if (!string.IsNullOrWhiteSpace(sSubDepID))
                    {
                        ddl_SubDepartment.SelectedValue = sSubDepID.ToString();
                    }

                    PositionSelectedChanged(emp.RESULT.Where(x => x.DEPARTMENT_ID == ddl_Department.SelectedValue && x.SUBDEPARTMENT_ID == ddl_SubDepartment.SelectedValue).ToList());
                    if (!string.IsNullOrWhiteSpace(sPositionID))
                    {
                        ddl_Position.SelectedValue = sPositionID.ToString();
                    }
                }
            }
        }
        protected void ddl_Department_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindingDepartmentDDL(ddl_Department.SelectedValue, "", "");
        }

        protected void ddl_SubDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindingDepartmentDDL(ddl_Department.SelectedValue, ddl_SubDepartment.SelectedValue, "");
        }

        protected void ddl_Position_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindingDepartmentDDL(ddl_Department.SelectedValue, ddl_SubDepartment.SelectedValue, ddl_Position.SelectedValue);
        }
        private void DepartmentSelectedChanged(List<SpecificEmployeeData.RESULT> empResult)
        {
            ddl_Department.DataSource = empResult;
            ddl_Department.DataTextField = "DEPARTMENT_NAME_TH";
            ddl_Department.DataValueField = "DEPARTMENT_ID";
            ddl_Department.DataBind();
        }
        private void SubDepartmentSelectedChanged(List<SpecificEmployeeData.RESULT> empResult)
        {
            ddl_SubDepartment.DataSource = empResult;
            ddl_SubDepartment.DataTextField = "SUBDEPARTMENT_NAME_TH";
            ddl_SubDepartment.DataValueField = "SUBDEPARTMENT_ID";
            ddl_SubDepartment.DataBind();
        }
        private void PositionSelectedChanged(List<SpecificEmployeeData.RESULT> empResult)
        {
            ddl_Position.DataSource = empResult;
            ddl_Position.DataTextField = "POSTION_NAME_TH";
            ddl_Position.DataValueField = "POSITION_TD";
            ddl_Position.DataBind();
        }
        #endregion

        protected void rdl_DelegateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            vs_SelectedItems = 0;
            if (rdl_DelegateType.SelectedValue == "DocNo")
            {
                panelDelegateByDocNo.Visible = true;
                panelDelegateByDocType.Visible = false;
            }
            else
            {
                panelDelegateByDocNo.Visible = false;
                panelDelegateByDocType.Visible = true;
            }
            BindingDelegateGV();
        }
        protected void chk_GrantPermanent_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_GrantPermanent.Checked)
            {
                txt_DateTo.Text = "";
                txt_DateTo.Enabled = false;
            }
            else
            {
                txt_DateTo.Enabled = true;
            }
        }

        protected void BindingDelegateGV()
        {
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
            List<TRNDocument> listDocument = db.TRNDocuments.ToList();
            listDocument = listDocument.Where(x => x.WaitingFor.Replace(" ", "").Split(',').Contains(hdn_ApproverID.Value)
                && (x.WaitingForDeptID == ddl_Department.SelectedValue)
                && (x.Status.ToLower().Equals("wait for approve"))).ToList();

            if (rdl_DelegateType.SelectedValue == "DocNo")
            {
                DataTable dt = vs_dt_PendingDocNo.Clone();
                foreach (var item in listDocument)
                {
                    DataRow dr = dt.NewRow();
                    dr["Selected"] = true;
                    dr["DocID"] = item.DocID;
                    dr["DocNO"] = item.DocNo;
                    dr["Category"] = item.Category;
                    dr["DocTypeCode"] = item.DocTypeCode;
                    dr["Title"] = item.Title;
                    dr["CreateDate"] = item.CreatedDate;
                    dr["Requestor"] = item.RequestorID;
                    dr["Status"] = item.Status;
                    dr["Amount"] = item.Amount;
                    dt.Rows.Add(dr);
                }
                vs_dt_PendingDocNo = dt;
                gv_DelegateByDocNo.DataSource = dt;
                gv_DelegateByDocNo.DataBind();
            }
            else
            {
                var DocTypeGroup = listDocument.GroupBy(x => x.DocTypeCode);
                DataTable dt = vs_dt_PendingDocType.Clone();
                DataTable dtDocType = vs_DocTypeTable;

                foreach (DataRow row in dtDocType.Rows)
                {
                    if (row["IsActive"].ToString().Equals("1") && row["Level"].ToString().Equals("0"))
                    {
                        DataRow dr = dt.NewRow();
                        dr["Selected"] = false;
                        dr["DocTypeCode"] = row["Value"].ToString();
                        dr["DocTypeName"] = row["DocTypeName"].ToString();
                        dr["Count"] = listDocument.Where(x => x.DocTypeCode == row["Value"].ToString()).ToList().Count;
                        dt.Rows.Add(dr);
                    }
                }
                vs_dt_PendingDocType = dt;
                gv_DelegateByDocType.DataSource = dt;
                gv_DelegateByDocType.DataBind();
            }
        }
        #endregion

        #region | Upload File |
        protected void fileUploadBtn_Click(object sender, EventArgs e)
        {
            //string DocLibName = lbl_Status.Text.Equals("Wait for Admin Centre") || lbl_Status.Text.Equals("Completed") ? "Document_Library" : "TempDocument";

            string DocLibName = "TempDocument";

            Button btn = (Button)sender;
            if (up_Attachment.HasFile)
            {
                try
                {
                    DataTable dtAttachlList = vs_attachFileTable;
                    int iCurrentRowCount = dtAttachlList != null ? dtAttachlList.Rows.Count : 0;
                    if (!vs_isDocSetCreated)
                    {
                        string result = SharedRules.CreateDocumentSet(DocLibName, lbl_Docset.Text, null);
                        if (!string.IsNullOrEmpty(result)) { Extension.MessageBox(this.Page, result); }
                        vs_isDocSetCreated = true;
                        vs_isAttach = true;
                    }
                    SharedRules.UploadFileIntoDocumentSet(DocLibName, lbl_Docset.Text, up_Attachment.FileName,
                        up_Attachment.PostedFile.InputStream, "", vs_CurrentUserID);

                    DataRow dr = vs_attachFileTable.NewRow();

                    dr["Sequence"] = iCurrentRowCount + 1;
                    dr["AttachFile"] = up_Attachment.FileName;
                    //dr["FileName"] = txt_FileName.Text;
                    //dr["ActorName"] = isPrimary && ddl_DocType.SelectedValue != "Im" && rdb_Type.SelectedValue != "Save" ? "-" : vs_CurrentUserID;

                    dr["AttachDate"] = DateTime.Now;
                    dr["AttachFilePath"] = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), DocLibName, lbl_Docset.Text.Trim(), up_Attachment.FileName);
                    dr["DocSetName"] = lbl_Docset.Text;
                    dr["DocLibName"] = DocLibName;
                    dr["ActorID"] = vs_CurrentUserID;

                    vs_attachFileTable.Rows.Add(dr);
                    gv_AttachFile.DataSource = vs_attachFileTable;
                    gv_AttachFile.DataBind();
                }
                catch (Exception ex)
                {
                    Extension.MessageBox(this.Page, "Error: " + ex.Message.ToString());
                }
            }
            else
            {
                Extension.MessageBox(this.Page, "No attach file.");
            }
        }
        protected void gv_AttachFile_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                #region | Employee Name |

                Label lblActorName = (Label)e.Row.FindControl("lblEmpName");
                var EmpID = DataBinder.Eval(e.Row.DataItem, "ActorID");
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

                            lblActorName.Text = empNameTH;
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
        protected void gv_AttachFile_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dt_attach = vs_attachFileTable;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dt_attach != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    string actorID = dt_attach.Rows[iRowIndex]["ActorID"].ToString();
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
        #endregion

        #region | GV ByDocNo |
        protected void gv_DelegateByDocNo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";

                #region | Doc ID | 
                HiddenField hdnDocID = (HiddenField)e.Row.FindControl("hdn_DocID");
                var objDocID = DataBinder.Eval(e.Row.DataItem, "DocID");
                if (hdnDocID != null && objDocID != DBNull.Value && objDocID != null)
                {
                    hdnDocID.Value = objDocID.ToString();
                }
                #endregion

                #region | Category |
                Label lblCategory = (Label)e.Row.FindControl("lbl_Category");
                var objCategory = DataBinder.Eval(e.Row.DataItem, "Category");
                if (objCategory != null && objCategory != DBNull.Value)
                {
                    DataTable dt = Extension.GetDataTable("MstCategory");
                    if (!dt.DataTableIsNullOrEmpty())
                    {
                        DataView dv = dt.AsDataView();
                        dv.RowFilter = string.Format("Value = '{0}'", objCategory.ToString());
                        dt = dv.ToTable();
                        if (!dt.DataTableIsNullOrEmpty())
                        {
                            lblCategory.Text = dt.Rows[0]["CategoryName"].ToString();
                        }
                    }
                    else
                    {
                        throw new Exception("MSTCategory has no DataRow");
                    }
                }
                #endregion

                #region | Document Type |
                Label lblDocType = (Label)e.Row.FindControl("lbl_DocType");
                HiddenField hdnDocType = (HiddenField)e.Row.FindControl("hdn_DocType");
                var objLDocType = DataBinder.Eval(e.Row.DataItem, "DocTypeCode");
                if (lblDocType != null && hdnDocType != null)
                    if (objLDocType != null && objLDocType != DBNull.Value)
                    {
                        DataTable dtDocumentType = Extension.GetDataTable("MstDocumentType");
                        if (!dtDocumentType.DataTableIsNullOrEmpty())
                        {
                            DataView dv = dtDocumentType.AsDataView();
                            dv.RowFilter = string.Format("Value = '{0}'", objLDocType.ToString()); ;
                            DataTable oResult = dv.ToTable();

                            if (!oResult.DataTableIsNullOrEmpty())
                            {
                                lblDocType.Text = oResult.Rows[0]["DocTypeName"].ToString();
                                hdnDocType.Value = objLDocType.ToString();
                            }
                        }
                    }
                #endregion

                #region | Create Date |
                Label lblCreateDate = (Label)e.Row.FindControl("lbl_CreateDate");
                var objCreateDate = DataBinder.Eval(e.Row.DataItem, "CreateDate");
                if (lblCreateDate != null)
                    if (objCreateDate != null && objCreateDate != DBNull.Value)
                    {
                        DateTime createdDate = DateTime.Parse(objCreateDate.ToString());
                        lblCreateDate.Text = createdDate.ToString("dd/MM/yyyy");

                    }
                #endregion

                #region | Requestor |
                Label lblRequestor = (Label)e.Row.FindControl("lbl_Requestor");
                var objRequestor = DataBinder.Eval(e.Row.DataItem, "Requestor");
                if (lblRequestor != null)
                    if (objRequestor != null && objRequestor != DBNull.Value)
                    {
                        var emp = Extension.GetSpecificEmployeeFromTemp(this.Page, objRequestor.ToString());
                        if (emp != null)
                        {
                            string name = string.Format("{0}{1} {2}", emp.PREFIX_TH, emp.FIRSTNAME_TH, emp.LASTNAME_TH);
                            lblRequestor.Text = name;
                        }
                    }
                #endregion

                #region | Amount |
                Label lblAmount = (Label)e.Row.FindControl("lbl_Amount");
                var objAmount = DataBinder.Eval(e.Row.DataItem, "Amount");
                if (lbl_Approver != null && objAmount != null && objAmount != DBNull.Value)
                {
                    if (!string.IsNullOrWhiteSpace(objAmount.ToString()))
                    {
                        lblAmount.Text = Extension.SetDecimalFormat(objAmount.ToString());
                    }
                    else
                    {
                        lblAmount.Text = "-";
                    }
                }
                #endregion
            }
        }
        protected void chk_ByDocNo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            CheckBox checkHead = (CheckBox)gv_DelegateByDocNo.HeaderRow.FindControl("chk_HeadByDocNO");
            if (!checkBox.Checked)
            {
                checkHead.Checked = false;
                vs_SelectedItems--;
                if (vs_SelectedItems == 0)
                {
                    btn_Submit.OnClientClick = "return confirm('ไม่มีเอกสารที่ถูกเลือก');";
                }
            }
            else
            {
                vs_SelectedItems++;
                btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ {0} เอกสารใช่หรือไม่?');", vs_SelectedItems);
                if (vs_SelectedItems == gv_DelegateByDocNo.Rows.Count)
                {
                    checkHead.Checked = true;
                }
            }
        }

        protected void chk_HeadByDocNo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox headBox = (CheckBox)sender;
            vs_SelectedItems = 0;
            btn_Submit.OnClientClick = "return confirm('ไม่มีเอกสารที่ถูกเลือก');";

            foreach (GridViewRow row in gv_DelegateByDocNo.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chk_ByDocNO");
                if (chk != null)
                {
                    if (headBox.Checked)
                    {
                        vs_SelectedItems++;
                    }
                    chk.Checked = headBox.Checked;
                }
            }
            if (headBox.Checked)
            {
                btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ {0} เอกสารใช่หรือไม่?');", vs_SelectedItems);
            }
            else
            {
                btn_Submit.OnClientClick = "return confirm('ไม่มีเอกสารที่ถูกเลือก');";
            }
        }
        #endregion

        #region | GV ByDocType |

        protected void chk_ByDocType_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            CheckBox checkHead = (CheckBox)gv_DelegateByDocType.HeaderRow.FindControl("chk_HeadByDocType");
            if (!checkBox.Checked)
            {
                checkHead.Checked = false;
                vs_SelectedItems--;
                btn_Submit.OnClientClick = "return confirm('ไม่มีประเภทเอกสารที่ถูกเลือก');";
            }
            else
            {
                vs_SelectedItems++;
                btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ {0} ประเภทเอกสารใช่หรือไม่?');", vs_SelectedItems);
                if (vs_SelectedItems == gv_DelegateByDocType.Rows.Count)
                {
                    checkHead.Checked = true;
                    btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ทุกประเภทเอกสารใช่หรือไม่?');", vs_SelectedItems);
                }
            }
        }

        protected void chk_HeadByDocType_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox headBox = (CheckBox)sender;
            vs_SelectedItems = 0;

            foreach (GridViewRow row in gv_DelegateByDocType.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chk_ByDocType");
                if (chk != null)
                {
                    if (headBox.Checked)
                    {
                        vs_SelectedItems++;
                    }
                    chk.Checked = headBox.Checked;
                }
            }

            if (headBox.Checked)
            {
                btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ทุกประเภทเอกสารใช่หรือไม่?');", vs_SelectedItems);
            }
            else
            {
                btn_Submit.OnClientClick = "return confirm('ไม่มีประเภทเอกสารที่ถูกเลือก');";
            }
        }

        protected void gv_DelegateByDocType_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";

                #region | Document Type |
                Label lblDocType = (Label)e.Row.FindControl("lbl_DocType");
                HiddenField hdnDocType = (HiddenField)e.Row.FindControl("hdn_DocType");
                var objLDocType = DataBinder.Eval(e.Row.DataItem, "DocTypeCode");
                if (lblDocType != null && hdnDocType != null)
                    if (objLDocType != null && objLDocType != DBNull.Value)
                    {
                        //DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", "<View><Query><Where><And><Eq><FieldRef Name='DocTypeCode' /><Value Type='text'>" + objLDocType.ToString() + "</Value></Eq><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq></And></Where><RowLimit>100000</RowLimit></Query></View>");
                        DataTable dtDocumentType = Extension.GetDataTable("MstDocumentType");
                        if (!dtDocumentType.DataTableIsNullOrEmpty())
                        {
                            DataView dv = dtDocumentType.AsDataView();
                            dv.RowFilter = string.Format("Value = '{0}'", objLDocType.ToString()); ;
                            DataTable oResult = dv.ToTable();

                            //var oResult = dtDocumentType.AsEnumerable().Where(r => r.Field<string>("DocTypeCode").ToString() == objLDocType.ToString()).Select().ToList();
                            //dtDocumentType = Extension.ListToDataTable(oResult);
                            if (!oResult.DataTableIsNullOrEmpty())
                            {
                                lblDocType.Text = oResult.Rows[0]["DocTypeName"].ToString();
                                hdnDocType.Value = objLDocType.ToString();
                            }
                        }
                    }
                #endregion
            }
        }
        #endregion

        #region | Button Action |
        private bool IsPassValidate(ref string sMessage)
        {
            if (string.IsNullOrEmpty(lbl_Approver.Text))
            {
                sMessage = "กรุณาเลือกผู้อนุมัติงาน";
                lbl_Approver.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(lbl_DelegateTo.Text))
            {
                sMessage = "กรุณาเลือกผู้อนุมัติงานแทน";
                lbl_DelegateTo.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_DateFrom.Text))
            {
                sMessage = "กรุณาเลือกวันที่เริ่ม";
                txt_DateFrom.Focus();
                return false;
            }
            if (!chk_GrantPermanent.Checked)
            {

                if (string.IsNullOrEmpty(txt_DateTo.Text))
                {
                    sMessage = "กรุณาเลือกวันที่สิ้นสุด";
                    txt_DateTo.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txt_Remark.Text))
            {
                sMessage = "กรุณากรอกหมายเหตุ";
                txt_Remark.Focus();
                return false;
            }
            if (rdl_DelegateType.SelectedValue == "DocNo")
            {
                if (vs_SelectedItems == 0)
                {
                    sMessage = "กรุณาเลือกเอกสารที่จะทำการมอบอำนาจ";
                    return false;
                }
            }


            return true;
        }
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                string sValidtionMsg = string.Empty;
                if (IsPassValidate(ref sValidtionMsg))
                {
                    DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                    #region | Insert TRNDelegate |
                    TRNDelegate objDelegate = new TRNDelegate();
                    objDelegate.ApproverID = hdn_ApproverID.Value;
                    objDelegate.ApproverName = lbl_Approver.Text;
                    objDelegate.DepartmentID = ddl_Department.SelectedValue;
                    objDelegate.SubDepartmentID = ddl_SubDepartment.SelectedValue;
                    objDelegate.PositionID = ddl_Position.SelectedValue;
                    objDelegate.DelegateToID = hdn_DelegateToID.Value;
                    objDelegate.DelegateToName = lbl_DelegateTo.Text;
                    objDelegate.DateFrom = DateTime.ParseExact(string.Format("{0} 00:00:01", txt_DateFrom.Text), "dd/MM/yyyy HH:mm:ss", ctli);
                    if (!chk_GrantPermanent.Checked)
                    {
                        objDelegate.DateTo = DateTime.ParseExact(string.Format("{0} 23:59:59", txt_DateTo.Text), "dd/MM/yyyy HH:mm:ss", ctli);
                    }
                    objDelegate.Remark = txt_Remark.Text;
                    objDelegate.IsActive = true;
                    objDelegate.ModifiedBy = vs_CurrentUserID;
                    objDelegate.ModifiedDate = DateTime.Now;
                    db.TRNDelegates.InsertOnSubmit(objDelegate);
                    db.SubmitChanges();
                    #endregion

                    #region | Insert AttachDelegateFile |
                    DataTable dtAttach = vs_attachFileTable;
                    if (!dtAttach.DataTableIsNullOrEmpty())
                    {
                        //Move File From TempDocument To DocumentLibraly
                        string sDocLibName = "Document_Library";
                        string sDocSet = string.Format("[Delegate]_{0}", objDelegate.DelegateID);
                        SharedRules.CreateDocumentSet(sDocLibName, sDocSet, null);
                        SharedRules.CopyFileToAnotherDocSet("TempDocument", lbl_Docset.Text, sDocLibName, sDocSet);
                        lbl_Docset.Text = sDocSet;

                        List<TRNAttachFileDelegate> listAttach = new List<TRNAttachFileDelegate>();
                        foreach (DataRow row in dtAttach.Rows)
                        {
                            TRNAttachFileDelegate objAttachFile = new TRNAttachFileDelegate();
                            objAttachFile.DelegateID = objDelegate.DelegateID;
                            objAttachFile.ActorID = Convert.ToInt32(vs_CurrentUserID);
                            objAttachFile.AttachDate = DateTime.Parse(row["AttachDate"].ToString());
                            objAttachFile.AttachFile = row["AttachFile"].ToString();

                            string path = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), sDocLibName, sDocSet, row["AttachFile"].ToString());
                            objAttachFile.AttachFilePath = path;
                            objAttachFile.DocLibName = row["DocLibName"].ToString();
                            objAttachFile.DocSetName = row["DocSetName"].ToString();
                            listAttach.Add(objAttachFile);
                        }
                        db.TRNAttachFileDelegates.InsertAllOnSubmit(listAttach);
                        db.SubmitChanges();
                    }
                    #endregion

                    #region | Delegate Detail |
                    List<TRNDelegateDetail> listDelegateDetail = new List<TRNDelegateDetail>();
                    //GridView dt = rdl_DelegateType.SelectedValue == "DocType" ? gv_DelegateByDocType : gv_DelegateByDocType;
                    if (rdl_DelegateType.SelectedValue == "DocType")
                    {
                        foreach (GridViewRow row in gv_DelegateByDocType.Rows)
                        {
                            TRNDelegateDetail objDelegateDetail = new TRNDelegateDetail();
                            objDelegateDetail.DelegateID = objDelegate.DelegateID;
                            objDelegateDetail.IsActive = true;
                            CheckBox chk = (CheckBox)row.FindControl("chk_ByDocType");
                            if (chk != null)
                            {
                                if (chk.Checked)
                                {
                                    HiddenField lbl = (HiddenField)row.FindControl("hdn_DocType");
                                    if (lbl != null)
                                    {
                                        objDelegateDetail.DocType = lbl.Value;
                                        objDelegateDetail.IsByDocType = true;
                                        listDelegateDetail.Add(objDelegateDetail);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (GridViewRow row in gv_DelegateByDocNo.Rows)
                        {
                            CheckBox chk = (CheckBox)row.FindControl("chk_ByDocNO");
                            if (chk != null)
                            {
                                if (chk.Checked)
                                {
                                    TRNDelegateDetail objDelegateDetail = new TRNDelegateDetail();
                                    objDelegateDetail.DelegateID = objDelegate.DelegateID;
                                    objDelegateDetail.IsActive = true;
                                    HiddenField hdnDocType = (HiddenField)row.FindControl("hdn_DocType");
                                    HiddenField hdnDocID = (HiddenField)row.FindControl("hdn_DocID");
                                    if (hdnDocType != null && hdnDocID != null)
                                    {
                                        objDelegateDetail.DocID = hdnDocID.Value;
                                        objDelegateDetail.DocType = hdnDocType.Value;
                                        objDelegateDetail.IsByDocType = false;
                                        listDelegateDetail.Add(objDelegateDetail);
                                    }
                                }
                            }
                        }
                    }
                    #region Backup
                    //foreach (GridViewRow row in dt.Rows)
                    //{
                    //    TRNDelegateDetail objDelegateDetail = new TRNDelegateDetail();
                    //    objDelegateDetail.DelegateID = objDelegate.DelegateID;
                    //    objDelegateDetail.IsActive = true;
                    //    if (rdl_DelegateType.SelectedValue == "DocType")
                    //    {
                    //        CheckBox chk = (CheckBox)row.FindControl("chk_ByDocType");
                    //        if (chk != null)
                    //        {
                    //            if (chk.Checked)
                    //            {
                    //                HiddenField lbl = (HiddenField)row.FindControl("hdn_DocType");
                    //                if (lbl != null)
                    //                {
                    //                    objDelegateDetail.DocType = lbl.Value;
                    //                    objDelegateDetail.IsByDocType = true;
                    //                    listDelegateDetail.Add(objDelegateDetail);
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        CheckBox chk = (CheckBox)row.FindControl("chk_ByDocNO");
                    //        if (chk != null)
                    //        {
                    //            if (chk.Checked)
                    //            {
                    //                HiddenField hdnDocType = (HiddenField)row.FindControl("hdn_DocType");
                    //                HiddenField hdnDocID = (HiddenField)row.FindControl("hdn_DocID");
                    //                if (hdnDocType != null && hdnDocID != null)
                    //                {
                    //                    objDelegateDetail.DocID = hdnDocID.Value;
                    //                    objDelegateDetail.DocType = hdnDocType.Value;
                    //                    objDelegateDetail.IsByDocType = false;
                    //                    listDelegateDetail.Add(objDelegateDetail);
                    //                }
                    //            }
                    //        }
                    //    }
                    //    listDelegateDetail.Add(objDelegateDetail);
                    //}
                    #endregion
                    db.TRNDelegateDetails.InsertAllOnSubmit(listDelegateDetail);
                    db.SubmitChanges();
                    #endregion

                    #region | Send Email |
                    SendEmailTemplate("Delegate", objDelegate.DelegateToID, "Delegate", vs_CurrentUserID);
                    #endregion
                    Extension.MessageBox(Page, "Delegate Completed.", "DelegateList.aspx");
                }
                else
                {
                    Extension.MessageBox(Page, sValidtionMsg);
                }
            }
            catch (Exception ex)
            {

            }

        }

        protected void btn_Close_Click(object sender, EventArgs e)
        {
            Extension.MessageBox(Page, "Are you sure to exit this page? Any changes will not saved.", "DelegateList.aspx");
        }
        #endregion

        #region | Popup Requestor |
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
                case "lkb_SearchApprover":
                    vs_popUpTarget = "Approver";
                    break;
                case "lkb_searchDelegateTo":
                    vs_popUpTarget = "DelegateTo";
                    break;
                default:
                    vs_popUpTarget = ""; break;
            }
            //gv_searchEmpReqTable.DataSource = new List<string>();
            //gv_searchEmpReqTable.DataBind();
            LoadDataEmpToCache(txt_searchBox.Text);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popSearchToModal1", "$('#searchEmpReqModal').modal('show');", true);

        }
        protected void btnClosePopup_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchCCModal1", "$('#searchEmpReqModal').modal('hide');", true);
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
            if (vs_popUpTarget == "Approver")
            {
                //Merge Table
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                List<TRNEmployeeExtension> listEmpEx = db.TRNEmployeeExtensions.ToList();
                if (listEmpEx.Count > 0)
                {
                    var listEmp = Extension.DataTableToList<TRNEmployeeExtension>(dtEmp);
                    if (listEmp.Count > 0)
                    {
                        foreach (var item in listEmpEx)
                        {
                            listEmp.Add(item);
                        }
                        listEmp = listEmp.OrderBy(x => x.EMPLOYEEID).ToList();
                        dtEmp = Extension.ListToDataTable<TRNEmployeeExtension>(listEmp);
                    }
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
                var objPrefixData = DataBinder.Eval(e.Row.DataItem, "PREFIX_TH");
                var objNameData = DataBinder.Eval(e.Row.DataItem, "FIRSTNAME_TH");
                var objLNameData = DataBinder.Eval(e.Row.DataItem, "LASTNAME_TH");
                if (lblEmpName != null)
                    if (objNameData != null && objNameData != DBNull.Value) lblEmpName.Text = string.Format("{0} {1} {2}", objPrefixData, objNameData, objLNameData);

                Label lblPosition = (Label)e.Row.FindControl("lblPosition");
                var objPositionData = DataBinder.Eval(e.Row.DataItem, "POSTION_NAME_TH");
                if (lblPosition != null)
                    if (objPositionData != null && objPositionData != DBNull.Value) lblPosition.Text = (string)objPositionData;

                Label lblDepartment = (Label)e.Row.FindControl("lblDepartment");
                var objDepartmentData = DataBinder.Eval(e.Row.DataItem, "DEPARTMENT_NAME_TH");
                if (lblDepartment != null)
                    if (objDepartmentData != null && objDepartmentData != DBNull.Value) lblDepartment.Text = objDepartmentData.ToString();

                Label lblSubDepartment = (Label)e.Row.FindControl("lblSubDepartment");
                var objSubDepartmentData = DataBinder.Eval(e.Row.DataItem, "SUBDEPARTMENT_NAME_TH");
                if (lblSubDepartment != null)
                    if (objSubDepartmentData != null && objSubDepartmentData != DBNull.Value) lblSubDepartment.Text = objSubDepartmentData.ToString();

            }
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
                            case "Approver":
                                hdn_ApproverID.Value = sEmpID;
                                lbl_Approver.Text = empNameTH;
                                BindingDepartmentDDL(sDeptID, sSubDeptID, sPosID);
                                ddl_Department.SelectedValue = sDeptID;
                                ddl_SubDepartment.SelectedValue = sSubDeptID;
                                ddl_Position.SelectedValue = sPosID;
                                BindingDelegateGV();
                                break;
                            case "DelegateTo":
                                hdn_DelegateToID.Value = sEmpID;
                                lbl_DelegateTo.Text = empNameTH;
                                break;
                            default:
                                break;
                        }

                    }


                }
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchEmpModal", "$('#searchEmpReqModal').modal('hide');", true);
        }
        #endregion


        public void SendEmailTemplate(string sNextStatus, string sToEmpID, string sAction, string currentUserID)
        {
            try
            {
                DataTable dtEmailTemplate = Extension.GetDataTable("MstEmail");
                if (!dtEmailTemplate.DataTableIsNullOrEmpty())
                {
                    dtEmailTemplate = dtEmailTemplate.AsEnumerable()
                        .Where(r => r.Field<String>("Situation").Equals(sNextStatus)).ToList().CopyToDataTable();

                }

                if (dtEmailTemplate != null)
                {
                    if (dtEmailTemplate.Rows.Count > 0)
                    {
                        DataClassesDataAccessDataContext dataContext = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                        //Variable 
                        string sPDFLanguage = "TH";
                        string sSubject = dtEmailTemplate.Rows[0][string.Format("Subject{0}", sPDFLanguage)].ToString();
                        string sBody = dtEmailTemplate.Rows[0][string.Format("Detail{0}", sPDFLanguage)].ToString(); string sActionDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm", new System.Globalization.CultureInfo("th-TH"));
                        string sActorName = lbl_Approver.Text;
                        var emp = Extension.GetSpecificEmployeeFromTemp(Page, hdn_DelegateToID.Value);

                        string sDestinationUserName = string.Format("{0} {1} {2}", emp.PREFIX_EN, emp.FIRSTNAME_EN, emp.LASTNAME_EN);
                        string sDestinationEmail = "";
                        string sSubjectMemo = "";
                        string sDelegateName = lbl_DelegateTo.Text;
                        string sDelegateFrom = txt_DateFrom.Text;
                        string sDelegateTo = txt_DateTo.Text;
                        string sRemark = txt_Remark.Text;
                        string sAttachFileName = vs_attachFileTable.Rows.Count == 0 ? "" : vs_attachFileTable.Rows[0]["AttachFile"].ToString();
                        if (Extension._IsTestEmail)
                        {
                            sDestinationEmail = "peerapon@techconsbiz.com";
                        }
                        sSubject = sSubject.Replace("[Subject Memo]", sSubjectMemo);
                        sBody = sBody
                            .Replace("[Destination User]", sDestinationUserName)
                            .Replace("[Approver]", sActorName)
                            .Replace("[Delegate Name]", sDelegateName)
                            .Replace("[Delegate From]", sDelegateFrom)
                            .Replace("[Delegate To]", sDelegateTo)
                            .Replace("[Remark]", sRemark)
                            .Replace("[Link URL]", string.IsNullOrWhiteSpace(sAttachFileName) ? "" : string.Format("<a href='{0}/Document_Library/{1}/{2}'>Click</a>", Extension.GetSPSite(), lbl_Docset.Text, sAttachFileName));
                        Extension.SendEmail(sDestinationEmail, string.Empty, sSubject, sBody);

                    }
                    else
                    {
                        throw new Exception("Not Found Template ID");
                    }

                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }

        }


    }
}
