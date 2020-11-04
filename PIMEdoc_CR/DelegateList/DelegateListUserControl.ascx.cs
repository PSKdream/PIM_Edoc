using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace PIMEdoc_CR.DelegateList
{
    public partial class DelegateListUserControl : UserControl
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
        private int vs_SelectedDocNo
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
        private int vs_SelectedDocType
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

            // Initial Login user as an Approver
            hdn_ApproverID.Value = vs_CurrentUserID;
            if (!string.IsNullOrWhiteSpace(hdn_ApproverID.Value))
            {
                var emp = Extension.GetSpecificEmployeeFromTemp(Page, vs_CurrentUserID);
                var dep = emp.RESULT.First();
                lbl_Approver.Text = string.Format("{0}{1} {2}", emp.PREFIX_TH, emp.FIRSTNAME_TH, emp.LASTNAME_TH);
            }
            //if not Admin_ITEDoc
            if (!vs_isITAdmin)
            {
                lkb_SearchApprover.Enabled = false;
            }

            vs_DocTypeTable = SharedRules.GetList("MstDocumentType", @"<View><Query><Where><And><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq><Eq><FieldRef Name='Level' /><Value Type='Number'>0</Value></Eq></And></Where></Query></View>");

            gv_DelegateByDocType.DataSource = new List<string>();
            gv_DelegateByDocType.DataBind();
            BindingDelegateGV();
        }
        #endregion

        #region | Infomation Panel |
        protected void BindingDelegateGV()
        {
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
            List<v_TRNDelegateDetail> listDelegateView = db.v_TRNDelegateDetails.ToList();
            if (!string.IsNullOrEmpty(hdn_DelegateToID.Value))
            {
                listDelegateView = listDelegateView.Where(x => x.ApproverID == hdn_ApproverID.Value).ToList();
            }
            if (!string.IsNullOrEmpty(hdn_DelegateToID.Value))
            {
                listDelegateView = listDelegateView.Where(x => x.DelegateToID == hdn_DelegateToID.Value).ToList();
            }
            if (!string.IsNullOrEmpty(txt_DateFrom.Text) && string.IsNullOrEmpty(txt_DateTo.Text))
            {
                DateTime dFilteredDateFrom = DateTime.ParseExact(string.Format("{0} 00:00:01", txt_DateFrom.Text), "dd/MM/yyyy HH:mm:ss", ctli);
                listDelegateView = listDelegateView.Where(x => (x.DateFrom >= dFilteredDateFrom) || (x.DateTo >= dFilteredDateFrom)).ToList();
            }
            else if (string.IsNullOrEmpty(txt_DateFrom.Text) && !string.IsNullOrEmpty(txt_DateTo.Text))
            {
                DateTime dFilteredDateTo = DateTime.ParseExact(string.Format("{0} 23:59:59", txt_DateTo.Text), "dd/MM/yyyy HH:mm:ss", ctli);
                listDelegateView = listDelegateView.Where(x => (x.DateTo <= dFilteredDateTo) || (x.DateFrom <= dFilteredDateTo)).ToList();
            }
            else if (!string.IsNullOrEmpty(txt_DateFrom.Text) && !string.IsNullOrEmpty(txt_DateTo.Text))
            {
                DateTime dFilteredDateFrom = DateTime.ParseExact(string.Format("{0} 00:00:01", txt_DateFrom.Text), "dd/MM/yyyy HH:mm:ss", ctli);
                DateTime dFilteredDateTo = DateTime.ParseExact(string.Format("{0} 23:59:59", txt_DateTo.Text), "dd/MM/yyyy HH:mm:ss", ctli);
                listDelegateView = listDelegateView.Where(x => (x.DateTo >= dFilteredDateFrom) || (x.DateFrom <= dFilteredDateTo)).ToList();
            }
            listDelegateView = listDelegateView.OrderByDescending(x => x.ModifiedDate).ToList();
            vs_dt_PendingDocType = Extension.ListToDataTable<v_TRNDelegateDetail>(listDelegateView.Where(x => x.IsByDocType == true).ToList());

            gv_DelegateByDocType.DataSource = vs_dt_PendingDocType;
            gv_DelegateByDocType.DataBind();

            vs_dt_PendingDocNo = Extension.ListToDataTable<v_TRNDelegateDetail>(listDelegateView.Where(x => x.IsByDocType == false).ToList());
            gv_DelegateByDocNo.DataSource = vs_dt_PendingDocNo;
            gv_DelegateByDocNo.DataBind();

            //Filter by txt_search
            if (!string.IsNullOrWhiteSpace(txt_search.Text))
            {
                Extension.SearchGridview(gv_DelegateByDocNo, txt_search.Text, this.Page);
                Extension.SearchGridview(gv_DelegateByDocType, txt_search.Text, this.Page);
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
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                var sDocID = DataBinder.Eval(e.Row.DataItem, "DocID");
                TRNDocument objDocument = null;
                if (sDocID != DBNull.Value && sDocID != null)
                {
                    objDocument = db.TRNDocuments.Where(x => x.DocID == Convert.ToInt32(sDocID.ToString())).First();
                    #region | DocNo |
                    Label lblDocNo = (Label)e.Row.FindControl("lbl_DocNo");
                    if (lblDocNo != null)
                    {
                        lblDocNo.Text = objDocument.DocNo;
                    }
                    #endregion
                    #region | Title |
                    Label lblTitle = (Label)e.Row.FindControl("lbl_Title");
                    if (lblTitle != null)
                    {
                        lblTitle.Text = objDocument.Title;
                    }
                    #endregion
                    #region | Category |
                    Label lblCategory = (Label)e.Row.FindControl("lbl_Category");
                    if (lblCategory != null)
                    {
                        var objCategory = objDocument.Category;
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
                    var objLDocType = DataBinder.Eval(e.Row.DataItem, "DocType");
                    if (lblDocType != null)
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
                                }
                            }
                        }
                    #endregion
                    #region | Create Date |
                    Label lblCreateDate = (Label)e.Row.FindControl("lbl_CreateDate");
                    if (lblCreateDate != null)
                    {
                        DateTime createdDate = DateTime.Parse(objDocument.CreatedDate.ToString());
                        lblCreateDate.Text = createdDate.ToString("dd/MM/yyyy");
                    }

                    #endregion
                    #region | Requestor |
                    Label lblRequestor = (Label)e.Row.FindControl("lbl_Requestor");
                    if (lblRequestor != null)
                    {
                        var emp = Extension.GetSpecificEmployeeFromTemp(this.Page, objDocument.RequestorID.ToString());
                        if (emp != null)
                        {
                            string name = string.Format("{0}{1} {2}", emp.PREFIX_TH, emp.FIRSTNAME_TH, emp.LASTNAME_TH);
                            lblRequestor.Text = name;
                        }
                    }
                    #endregion
                    #region | Amount |
                    Label lblAmount = (Label)e.Row.FindControl("lbl_Amount");
                    if (lblAmount != null)
                    {
                        lblAmount.Text = Extension.SetDecimalFormat(objDocument.Amount.ToString());
                    }
                    #endregion
                }
                #region | Delegate TO |
                var lbl_DelegateTo = (Label)e.Row.FindControl("lbl_DelegateTo");
                var objDelegateTo = DataBinder.Eval(e.Row.DataItem, "DelegateToID");
                if (lbl_DelegateTo != null && objDelegateTo != null && objDelegateTo != DBNull.Value)
                {
                    var delTo = Extension.GetSpecificEmployeeFromTemp(Page, objDelegateTo.ToString());
                    lbl_DelegateTo.Text = string.Format("{0} {1}", delTo.FIRSTNAME_TH, delTo.LASTNAME_TH);
                }
                #endregion

                #region | DateFrom |
                Label lbl_DateFrom = (Label)e.Row.FindControl("lbl_DateFrom");
                var objDateFrom = DataBinder.Eval(e.Row.DataItem, "DateFrom");
                if (lbl_DateFrom != null && objDateFrom != DBNull.Value && objDateFrom != null)
                {
                    lbl_DateFrom.Text = DateTime.Parse(objDateFrom.ToString()).ToString("dd/MM/yyyy");
                }
                #endregion

                #region | DateTo |
                Label lbl_DateTo = (Label)e.Row.FindControl("lbl_DateTo");
                var objDateTo = DataBinder.Eval(e.Row.DataItem, "DateTo");
                if (lbl_DateTo != null && objDateTo != DBNull.Value && objDateTo != null)
                {
                    lbl_DateTo.Text = DateTime.Parse(objDateTo.ToString()).ToString("dd/MM/yyyy");
                }
                #endregion

                #region | Attachment |

                var objDelegateID = DataBinder.Eval(e.Row.DataItem, "DelegateID");
                List<TRNAttachFileDelegate> listAttach = db.TRNAttachFileDelegates.Where(x => x.DelegateID == Convert.ToInt32(objDelegateID.ToString())).ToList();
                if (listAttach.Count > 0)
                {
                    HyperLink hpl_AttachDoc = (HyperLink)e.Row.FindControl("hpl_AttachDoc");
                    var objAttachDoc = listAttach[0].AttachFile;
                    var objAttachPath = listAttach[0].AttachFilePath;
                    if (hpl_AttachDoc != null)
                    {
                        if (!string.IsNullOrEmpty(objAttachDoc) && !string.IsNullOrEmpty(objAttachPath))
                        {
                            hpl_AttachDoc.Text = objAttachDoc.ToString();
                            hpl_AttachDoc.NavigateUrl = objAttachPath.ToString();
                        }
                    }
                }
                #endregion

                #region | ModiFied Date |
                Label lbl_ModifiedDate = (Label)e.Row.FindControl("lbl_ModifiedDate");
                var objModifiedDate = DataBinder.Eval(e.Row.DataItem, "ModifiedDate");
                if (lbl_ModifiedDate != null && objModifiedDate != DBNull.Value && objModifiedDate != null)
                {
                    lbl_ModifiedDate.Text = DateTime.Parse(objModifiedDate.ToString()).ToString("dd/MM/yyyy");
                }
                #endregion
                #region | ModiFied By |
                Label lbl_ModifiedBy = (Label)e.Row.FindControl("lbl_ModifiedBy");
                var objModifiedBy = DataBinder.Eval(e.Row.DataItem, "ModifiedBy");
                if (lbl_ModifiedBy != null && objDateTo != DBNull.Value && objDateTo != null)
                {
                    var empMod = Extension.GetSpecificEmployeeFromTemp(Page, objModifiedBy.ToString());
                    lbl_ModifiedBy.Text = string.Format("{0} {1}", empMod.FIRSTNAME_TH, empMod.LASTNAME_TH);
                }
                #endregion
                #region | Status |
                Label lblStatus = (Label)e.Row.FindControl("lbl_Status");
                var objStatus = DataBinder.Eval(e.Row.DataItem, "IsActive");
                if (lblStatus != null)
                {
                    if (objStatus != DBNull.Value && objStatus != null)
                    {
                        if (objDateTo != null && objDateFrom != DBNull.Value && objDateFrom != null)
                        {
                            DateTime dateFrom = DateTime.Parse(objDateFrom.ToString());
                            DateTime dateTo = objDateTo == DBNull.Value ? DateTime.MaxValue : DateTime.Parse(objDateTo.ToString());
                            if (DateTime.Now >= dateFrom && DateTime.Now <= dateTo)
                            {
                                lblStatus.Text = "Active";
                            }
                            else
                            {
                                lblStatus.Text = "Inactive";
                            }
                        }
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
                vs_SelectedDocNo--;
                if (vs_SelectedDocNo == 0)
                {
                    //btn_Submit.OnClientClick = "return confirm('ไม่มีเอกสารที่ถูกเลือก');";
                }
            }
            else
            {
                vs_SelectedDocNo++;
                //btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ {0} เอกสารใช่หรือไม่?');", vs_SelectedItems);
                if (vs_SelectedDocNo == gv_DelegateByDocNo.Rows.Count)
                {
                    checkHead.Checked = true;
                }
            }
        }

        protected void chk_HeadByDocNo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox headBox = (CheckBox)sender;
            vs_SelectedDocNo = 0;
            //btn_Submit.OnClientClick = "return confirm('ไม่มีเอกสารที่ถูกเลือก');";

            foreach (GridViewRow row in gv_DelegateByDocNo.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chk_ByDocNO");
                if (chk != null)
                {
                    if (headBox.Checked)
                    {
                        vs_SelectedDocNo++;
                    }
                    chk.Checked = headBox.Checked;
                }
            }
            if (headBox.Checked)
            {
                //btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ {0} เอกสารใช่หรือไม่?');", vs_SelectedItems);
            }
            else
            {
                //btn_Submit.OnClientClick = "return confirm('ไม่มีเอกสารที่ถูกเลือก');";
            }
        }
        #endregion

        #region | GV ByDocType |

        //protected void chk_ByDocType_CheckedChanged(object sender, EventArgs e)
        //{
        //    CheckBox checkBox = (CheckBox)sender;
        //    CheckBox checkHead = (CheckBox)gv_DelegateByDocType.HeaderRow.FindControl("chk_HeadByDocType");
        //    if (!checkBox.Checked)
        //    {
        //        checkHead.Checked = false;
        //        vs_SelectedItems--;
        //        btn_Submit.OnClientClick = "return confirm('ไม่มีประเภทเอกสารที่ถูกเลือก');";
        //    }
        //    else
        //    {
        //        vs_SelectedItems++;
        //        btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ {0} ประเภทเอกสารใช่หรือไม่?');", vs_SelectedItems);
        //        if (vs_SelectedItems == gv_DelegateByDocType.Rows.Count)
        //        {
        //            checkHead.Checked = true;
        //            btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ทุกประเภทเอกสารใช่หรือไม่?');", vs_SelectedItems);
        //        }
        //    }
        //}

        //protected void chk_HeadByDocType_CheckedChanged(object sender, EventArgs e)
        //{
        //    CheckBox headBox = (CheckBox)sender;
        //    vs_SelectedItems = 0;

        //    foreach (GridViewRow row in gv_DelegateByDocType.Rows)
        //    {
        //        CheckBox chk = (CheckBox)row.FindControl("chk_ByDocType");
        //        if (chk != null)
        //        {
        //            if (headBox.Checked)
        //            {
        //                vs_SelectedItems++;
        //            }
        //            chk.Checked = headBox.Checked;
        //        }
        //    }

        //    if (headBox.Checked)
        //    {
        //        btn_Submit.OnClientClick = string.Format("return confirm('ต้องการให้สิทธิ์ทุกประเภทเอกสารใช่หรือไม่?');", vs_SelectedItems);
        //    }
        //    else
        //    {
        //        btn_Submit.OnClientClick = "return confirm('ไม่มีประเภทเอกสารที่ถูกเลือก');";
        //    }
        //}

        protected void gv_DelegateByDocType_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";

                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                #region | Document Type |
                Label lblDocType = (Label)e.Row.FindControl("lbl_DocType");
                var objLDocType = DataBinder.Eval(e.Row.DataItem, "DocType");
                if (lblDocType != null)
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
                            }
                        }
                    }
                #endregion

                #region | Delegate TO |
                var lbl_DelegateTo = (Label)e.Row.FindControl("lbl_DelegateTo");
                var objDelegateTo = DataBinder.Eval(e.Row.DataItem, "DelegateToID");
                if (lbl_DelegateTo != null && objDelegateTo != null && objDelegateTo != DBNull.Value)
                {
                    var delTo = Extension.GetSpecificEmployeeFromTemp(Page, objDelegateTo.ToString());
                    lbl_DelegateTo.Text = string.Format("{0} {1}", delTo.FIRSTNAME_TH, delTo.LASTNAME_TH);
                }
                #endregion


                #region | Date From |
                Label lblDateFrom = (Label)e.Row.FindControl("lbl_FromDate");
                var objDateFrom = DataBinder.Eval(e.Row.DataItem, "DateFrom");
                if (lblDateFrom != null && objDateFrom != null && objDateFrom != DBNull.Value)
                {
                    string date = ((DateTime)objDateFrom).ToString("dd/MM/yyyy");
                    lblDateFrom.Text = date;
                }
                #endregion
                #region | Date To |
                Label lblDateTo = (Label)e.Row.FindControl("lbl_ToDate");
                var objDateTo = DataBinder.Eval(e.Row.DataItem, "DateTo");
                if (lblDateTo != null && objDateTo != null && objDateTo != DBNull.Value)
                {
                    string date = ((DateTime)objDateTo).ToString("dd/MM/yyyy");
                    lblDateTo.Text = date;
                }
                #endregion

                #region | Attachment |

                var objDelegateID = DataBinder.Eval(e.Row.DataItem, "DelegateID");
                List<TRNAttachFileDelegate> listAttach = db.TRNAttachFileDelegates.Where(x => x.DelegateID == Convert.ToInt32(objDelegateID.ToString())).ToList();
                if (listAttach.Count > 0)
                {
                    HyperLink hpl_AttachDoc = (HyperLink)e.Row.FindControl("hpl_AttachDoc");
                    var objAttachDoc = listAttach[0].AttachFile;
                    var objAttachPath = listAttach[0].AttachFilePath;
                    if (hpl_AttachDoc != null)
                    {
                        if (!string.IsNullOrEmpty(objAttachDoc) && !string.IsNullOrEmpty(objAttachPath))
                        {
                            hpl_AttachDoc.Text = objAttachDoc.ToString();
                            hpl_AttachDoc.NavigateUrl = objAttachPath.ToString();
                        }
                    }
                }
                #endregion

                #region | ModiFied Date |
                Label lbl_ModifiedDate = (Label)e.Row.FindControl("lbl_ModifiedDate");
                var objModifiedDate = DataBinder.Eval(e.Row.DataItem, "ModifiedDate");
                if (lbl_ModifiedDate != null && objModifiedDate != DBNull.Value && objModifiedDate != null)
                {
                    lbl_ModifiedDate.Text = DateTime.Parse(objModifiedDate.ToString()).ToString("dd/MM/yyyy");
                }
                #endregion
                #region | ModiFied By |
                Label lbl_ModifiedBy = (Label)e.Row.FindControl("lbl_ModifiedBy");
                var objModifiedBy = DataBinder.Eval(e.Row.DataItem, "ModifiedBy");
                if (lbl_ModifiedBy != null)
                {
                    var empMod = Extension.GetSpecificEmployeeFromTemp(Page, objModifiedBy.ToString());
                    lbl_ModifiedBy.Text = string.Format("{0} {1}", empMod.FIRSTNAME_TH, empMod.LASTNAME_TH);
                }
                #endregion

                #region | Status |
                Label lblStatus = (Label)e.Row.FindControl("lbl_Status");
                var objStatus = DataBinder.Eval(e.Row.DataItem, "IsActive");
                if (lblStatus != null)
                {
                    if (objStatus != DBNull.Value && objStatus != null)
                    {
                        if (objDateTo != null && objDateFrom != DBNull.Value && objDateFrom != null)
                        {
                            DateTime dateFrom = DateTime.Parse(objDateFrom.ToString());
                            DateTime dateTo = objDateTo == DBNull.Value ? DateTime.MaxValue : DateTime.Parse(objDateTo.ToString());
                            if (DateTime.Now >= dateFrom && DateTime.Now <= dateTo)
                            {
                                lblStatus.Text = "Active";
                            }
                            else
                            {
                                lblStatus.Text = "Inactive";
                            }
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
            //if (string.IsNullOrEmpty(lbl_DelegateTo.Text))
            //{
            //    sMessage = "กรุณาเลือกผู้อนุมัติงานแทน";
            //    lbl_DelegateTo.Focus();
            //    return false;
            //}         
            return true;
        }
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                 string sValidtionMsg = string.Empty;
                 if (IsPassValidate(ref sValidtionMsg))
                 {
                     BindingDelegateGV();
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
        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            txt_search.Text = "";
            lbl_DelegateTo.Text = "";
            hdn_DelegateToID.Value = "";
            txt_DateFrom.Text = "";
            txt_DateTo.Text = "";
            try
            {
                string sValidtionMsg = string.Empty;
                if (IsPassValidate(ref sValidtionMsg))
                {
                    BindingDelegateGV();
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

        


    }
}
