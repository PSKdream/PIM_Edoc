using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using PIMEdoc_CR.Default.Rule;
namespace PIMEdoc_CR.ReportTimeManagement
{
    public partial class ReportTimeManagementUserControl : UserControl
    {
        #region | Viewstate & Global Variable |

        private TimeSpan vs_TotalTime
        {
            get
            {
                if (ViewState["vs_TotalTime"] == null)
                {
                    return new TimeSpan(0, 0, 0);
                }
                return (TimeSpan)ViewState["vs_TotalTime"];
            }
            set
            {
                ViewState["vs_TotalTime"] = value;
            }
        }
        private int vs_CounterDocument
        {
            get
            {
                if (ViewState["vs_MeanTime"] == null)
                {
                    return 0;
                }
                return (int)ViewState["vs_MeanTime"];
            }
            set
            {
                ViewState["vs_MeanTime"] = value;
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
        System.Globalization.CultureInfo _ctliGB = new System.Globalization.CultureInfo("en-GB");
        System.Globalization.CultureInfo _ctliTH = new System.Globalization.CultureInfo("th-TH");
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    InitialPage();
                }
                catch (Exception ex)
                {
                    Extension.LogWriter.Write(ex);
                }
            }
        }
        #region | Initial & Information |
        protected void InitialPage()
        {
            DataTable dtCategory = Extension.GetDataTable("MstCategory");
            if (!dtCategory.DataTableIsNullOrEmpty())
            {
                ddl_Category.DataSource = dtCategory;
                ddl_Category.DataTextField = "CategoryName";
                ddl_Category.DataValueField = "Value";
                ddl_Category.DataBind();
            }

            DataTable dtDocumentType = SharedRules.GetList("MstDocumentType", "<Where><And><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq><Eq><FieldRef Name='Level' /><Value Type='Number'>0</Value></Eq></And></Where>");
            if (!dtDocumentType.DataTableIsNullOrEmpty())
            {
                ddl_DocType.DataSource = dtDocumentType;
                ddl_DocType.DataTextField = "DocTypeName";
                ddl_DocType.DataValueField = "Value";
                ddl_DocType.DataBind();
                ddl_DocType.Items.Insert(0, new ListItem("-- All Document Type --", ""));
            }

            DataTable dtOtherDocumentType = SharedRules.GetList("MstDocumentType", "<Where><And><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq><Eq><FieldRef Name='Level' /><Value Type='Number'>1</Value></Eq></And></Where>");
            if (!dtOtherDocumentType.DataTableIsNullOrEmpty())
            {
                ddl_OtherDocType.DataSource = dtOtherDocumentType;
                ddl_OtherDocType.DataTextField = "DocTypeName";
                ddl_OtherDocType.DataValueField = "Value";
                ddl_OtherDocType.DataBind();
                ddl_OtherDocType.Items.Insert(0, new ListItem("-- All Other Type --", ""));
            }

            DataTable dtDepartment = Extension.GetDepartmentData(Page);
            if (!dtDepartment.DataTableIsNullOrEmpty())
            {
                DataView dv = dtDepartment.DefaultView;
                dv.RowFilter = "PRIMARY = 1";
                dv.Sort = "DEPARTMENT_NAME_TH asc";
                dtDepartment = dv.ToTable();

                ddl_FromDepartment.DataSource = dtDepartment;
                ddl_FromDepartment.DataTextField = "DEPARTMENT_NAME_TH";
                ddl_FromDepartment.DataValueField = "DEPARTMENT_ID";
                ddl_FromDepartment.DataBind();
                ddl_FromDepartment.Items.Insert(0, new ListItem("-- All Department Type --", ""));

                ddl_ToDepartment.DataSource = dtDepartment;
                ddl_ToDepartment.DataTextField = "DEPARTMENT_NAME_TH";
                ddl_ToDepartment.DataValueField = "DEPARTMENT_ID";
                ddl_ToDepartment.DataBind();
                ddl_ToDepartment.Items.Insert(0, new ListItem("-- All Department Type --", ""));
            }

            rdb_DataType.SelectedIndex = 0;
            txt_search.Text = "";
            ddl_Priority.SelectedIndex = 0;
            txt_Deadline.Text = "";
            ddl_Permission.SelectedIndex = 0;
            //chk_DOA.Checked = false;
            ddl_DOA.SelectedIndex = 0;
            txt_CreateDateFrom.Text = txt_CreateDateTo.Text = "";
            //ddl_Status.SelectedIndex = 0;
            vs_CounterDocument = 0;
            vs_TotalTime = new TimeSpan();


            FilterData();

        }

        protected void FilterData()
        {
            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
            List<v_TRNApproveDetail> listDocument = db.v_TRNApproveDetails
                    .Where(x => x.Status == "Completed"
                             && x.Category == ddl_Category.SelectedValue
                           )
                    .OrderByDescending(x => x.CreatedDate ?? DateTime.Now)
                    .ToList();

            if (ddl_DOA.SelectedIndex > 0)
            {
                listDocument = listDocument.Where(x => x.DOA == ddl_DOA.SelectedValue).ToList();
            }
            if (ddl_DocType.SelectedIndex > 0)
            {
                listDocument = listDocument.Where(x => x.DocTypeCode == ddl_DocType.SelectedValue).ToList();
            }
            if (ddl_OtherDocType.SelectedIndex > 0)
            {
                listDocument = listDocument.Where(x => x.OtherDocType == ddl_OtherDocType.SelectedValue).ToList();
            }
            if (ddl_FromDepartment.SelectedIndex > 0)
            {
                listDocument = listDocument.Where(x => x.FromDepartmentID == Convert.ToInt32(ddl_FromDepartment.SelectedValue)).ToList();
            }
            if (ddl_ToDepartment.SelectedIndex > 0)
            {
                listDocument = listDocument.Where(x => x.ToDepartmentID == ddl_ToDepartment.SelectedValue).ToList();
            }
            if (ddl_Priority.SelectedIndex > 0)
            {
                listDocument = listDocument.Where(x => x.Priority == ddl_Priority.SelectedValue).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txt_Deadline.Text))
            {
                listDocument = listDocument.Where(x => x.Deadline != null && ((DateTime)(x.Deadline ?? DateTime.Today)).Date == DateTime.ParseExact(txt_Deadline.Text, "dd/MM/yyyy", _ctliGB).Date).ToList();
            }
            if (ddl_Permission.SelectedIndex > 0)
            {
                listDocument = listDocument.Where(x => x.PermissionType == ddl_Permission.SelectedValue).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txt_CreateDateFrom.Text))
            {
                listDocument = listDocument.Where(x => ((DateTime)(x.CreatedDate ?? DateTime.Today)).Date >= DateTime.ParseExact(txt_CreateDateFrom.Text, "dd/MM/yyyy", _ctliGB).Date).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txt_CreateDateTo.Text))
            {
                listDocument = listDocument.Where(x => ((DateTime)(x.CreatedDate ?? DateTime.Today)).Date <= DateTime.ParseExact(txt_CreateDateTo.Text, "dd/MM/yyyy", _ctliGB).Date).ToList();
            }
            //if (ddl_Status.SelectedIndex > 0)
            //{
            //    listDocument = listDocument.Where(x => x.Status == ddl_Status.SelectedValue).ToList();
            //}


            #region | Filter Gridview by Mapping GVData to MstData |
            string textSearch = txt_search.Text;
            if (!string.IsNullOrWhiteSpace(textSearch))
            {
                //Mapping GvData with MSTData
                DataTable dtDept = Extension.GetDepartmentData(Page);
                DataTable dtEmp = Extension.GetEmployeeData(Page);
                DataTable dtDocument = Extension.ListToDataTable<v_TRNApproveDetail>(listDocument);

                //add new Mapping Column
                dtDocument.Columns.Add("CreatorNameTH");
                dtDocument.Columns.Add("CreatorNameEN");
                dtDocument.Columns.Add("RequesterNameTH");
                dtDocument.Columns.Add("RequesterNameEN");

                foreach (DataRow row in dtDocument.Rows)
                {
                    var objRequestor = Extension.GetSpecificEmployeeFromTemp(Page, row["EmpID"].ToString());// dtEmp.Copy().Select(string.Format("EmpID = '{0}'", row["Requester"].ToString()));
                    var objCreator = Extension.GetSpecificEmployeeFromTemp(Page, row["CreatorID"].ToString());// dtEmp.Copy().Select(string.Format("EmpID = '{0}'", row["Requester"].ToString()));

                    if (objCreator != null)
                    {
                        row["CreatorNameTH"] = string.Format("{0}{1} {2}", objCreator.PREFIX_TH, objCreator.FIRSTNAME_TH, objCreator.LASTNAME_TH);
                        row["CreatorNameEN"] = string.Format("{0}{1} {2}", objCreator.PREFIX_EN, objCreator.FIRSTNAME_EN, objCreator.LASTNAME_EN);
                    }
                    if (objRequestor != null)
                    {
                        row["RequesterNameTH"] = string.Format("{0}{1} {2}", objRequestor.PREFIX_TH, objRequestor.FIRSTNAME_TH, objRequestor.LASTNAME_TH);
                        row["RequesterNameEN"] = string.Format("{0}{1} {2}", objRequestor.PREFIX_EN, objRequestor.FIRSTNAME_EN, objRequestor.LASTNAME_EN);
                    }
                }

                var filtered = dtDocument.AsEnumerable()
                    .Where(r => r.Field<string>("RequesterNameTH").Contains(textSearch)
                             || r.Field<string>("RequesterNameEN").Contains(textSearch)
                             || r.Field<string>("CreatorNameTH").Contains(textSearch)
                             || r.Field<string>("CreatorNameEN").Contains(textSearch)
                             || r.Field<string>("Title").Contains(textSearch)
                             || r.Field<string>("DocNo").Contains(textSearch));

                if (filtered != null && filtered.Count() > 0)
                {
                    dtDocument = filtered.CopyToDataTable();
                }
                else
                {
                    dtDocument.Rows.Clear();
                }

                listDocument = Extension.DataTableToList<v_TRNApproveDetail>(dtDocument);

            }
            #endregion

            List<List<v_TRNApproveDetail>> filterDoc = listDocument
                .GroupBy(x => x.DocID)
                .Select(grp => grp.OrderBy(g => g.HistoryID).ToList())
                .ToList();

            vs_TotalTime = new TimeSpan();
            vs_CounterDocument = 0;
            List<v_TRNApproveDetail> tempList = new List<v_TRNApproveDetail>();
            if (rdb_DataType.SelectedValue == "finish")
            {
                foreach (List<v_TRNApproveDetail> lists in filterDoc)
                {
                    var TotalTime = "";
                    DateTime tempDate = DateTime.Now;
                    var index = lists.Count - 1;
                    var toDate = (DateTime)lists[index].ActionDate;
                    var timeSpan = toDate.Subtract((DateTime)lists[index].CreatedDate);
                    TotalTime = string.Format("{0} วัน {1} ชม. {2} นาที {3} วินาที", Math.Abs(timeSpan.Days), Math.Abs(timeSpan.Hours), Math.Abs(timeSpan.Minutes),Math.Abs(timeSpan.Seconds));
                    lists[index].TotalTime = TotalTime;
                    tempList.Add(lists[index]);
                    vs_TotalTime = vs_TotalTime.Add(timeSpan);
                    vs_CounterDocument++;
                }
            }
            else
            {
                foreach (List<v_TRNApproveDetail> lists in filterDoc)
                {
                    var TotalTime = "";
                    DateTime tempDate = DateTime.Now;
                    for (int i = 0; i < lists.Count; i++)
                    {
                        if (lists[i].ActionName != "Save Draft")
                        {
                            if (i > 0)
                            {
                                var fromDate = (DateTime)lists[i - 1].ActionDate;
                                var toDate = (DateTime)lists[i].ActionDate;
                                var timeSpan = toDate.Subtract(fromDate);
                                TotalTime = string.Format("{0} วัน {1} ชม. {2} นาที {3} วินาที", Math.Abs(timeSpan.Days), Math.Abs(timeSpan.Hours), Math.Abs(timeSpan.Minutes), Math.Abs(timeSpan.Seconds));
                                lists[i].TotalTime = TotalTime;
                                vs_TotalTime = vs_TotalTime.Add(timeSpan);
                            }
                            else
                            {
                                var fromDate = (DateTime)lists[i].CreatedDate;
                                var toDate = (DateTime)lists[i].ActionDate;
                                var timeSpan = toDate.Subtract(fromDate);
                                TotalTime = string.Format("{0} วัน {1} ชม. {2} นาที {3} วินาที", Math.Abs(timeSpan.Days), Math.Abs(timeSpan.Hours), Math.Abs(timeSpan.Minutes), Math.Abs(timeSpan.Seconds));
                                lists[i].TotalTime = TotalTime;
                                vs_TotalTime = vs_TotalTime.Add(timeSpan);
                            }
                            tempList.Add(lists[i]);
                        }
                    }
                    vs_CounterDocument++;

                }
            }
            if (vs_CounterDocument == 0)
            {
                vs_CounterDocument = 1;
            }
            lbl_TotalTime.Text = string.Format("{0} วัน {1} ชม. {2} นาที {3} วินาที", Math.Abs(vs_TotalTime.Days), Math.Abs(vs_TotalTime.Hours), Math.Abs(vs_TotalTime.Minutes), Math.Abs(vs_TotalTime.Seconds));
            lbl_MeanTime.Text = Math.Abs((vs_TotalTime.Hours / vs_CounterDocument)) + " ชม. " + Math.Abs((vs_TotalTime.Minutes / vs_CounterDocument)) + " นาที " + Math.Abs((vs_TotalTime.Seconds / vs_CounterDocument)) + " วินาที ";
            vs_DocumentList = Extension.ListToDataTable<v_TRNApproveDetail>(tempList);
            BindingViewstateToGV();
        }
        #endregion
        #region | Button Action |
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            FilterData();
        }
        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            InitialPage();
        }
        #endregion
        #region | Gridview |

        protected void gv_Report_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_Report.PageIndex = e.NewPageIndex;
            BindingViewstateToGV();
        }

        protected void BindingViewstateToGV()
        {
            gv_Report.DataSource = vs_DocumentList;
            gv_Report.DataBind();
        }

        protected void gv_Report_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_Report, "Select$" + e.Row.RowIndex);
                
                #region | Document Type |
                Label lblDocType = (Label)e.Row.FindControl("lblDocType");
                var objDocType = DataBinder.Eval(e.Row.DataItem, "DocTypeCode");
                if (lblDocType != null && objDocType != null && objDocType != DBNull.Value)
                {
                    lblDocType.Text = SharedRules.GetDocTypeName(objDocType.ToString());
                }
                #endregion

                #region | Other Document Type |
                Label lblOtherType = (Label)e.Row.FindControl("lblOtherDocType");
                var objOtherType = DataBinder.Eval(e.Row.DataItem, "OtherDocType");
                if (lblOtherType != null && objOtherType != null && objOtherType != DBNull.Value)
                {
                    lblOtherType.Text = SharedRules.GetDocTypeName(objOtherType.ToString());
                }
                #endregion

                #region | From Department |
                Label lbl_FromDepartment = (Label)e.Row.FindControl("lblFromDepartment");
                if (lbl_FromDepartment != null)
                {
                    var objFromDeptName = DataBinder.Eval(e.Row.DataItem, "FromDepartmentName");
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
                Label lbl_ToDepartment = (Label)e.Row.FindControl("lblToDepartment");
                if (lbl_ToDepartment != null)
                {
                    var objToDeptName = DataBinder.Eval(e.Row.DataItem, "ToDepartmentName");
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

                #region | Priority |
                Label lblPriority = (Label)e.Row.FindControl("lblPriority");
                if (lblPriority != null)
                {
                    var objPriority = DataBinder.Eval(e.Row.DataItem, "Priority");
                    if (objPriority != null && objPriority != DBNull.Value)
                    {
                        switch (objPriority.ToString())
                        {
                            case "Normal":
                                lblPriority.Text = "ทั่วไป";
                                break;
                            case "Fast":
                                lblPriority.Text = "ด่วน";
                                break;
                            case "Faster":
                                lblPriority.Text = "ด่วนมาก";
                                break;
                            case "Fastest":
                                lblPriority.Text = "ด่วนที่สุด";
                                break;
                            default: break;
                        }
                    }
                }
                #endregion

                #region | Deadline |
                Label lblDeadLine = (Label)e.Row.FindControl("lblDeadLine");
                if (lblDeadLine != null)
                {
                    var objDeadline = DataBinder.Eval(e.Row.DataItem, "Deadline");
                    if (objDeadline != null && objDeadline != DBNull.Value)
                    {
                        lblDeadLine.Text = DateTime.Parse(objDeadline.ToString()).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        lblDeadLine.Text = "-";
                    }
                }
                #endregion

                #region | DOA |
                Label lblDOA = (Label)e.Row.FindControl("lblDOA");
                if (lblDOA != null)
                {
                    var objDOA = DataBinder.Eval(e.Row.DataItem, "DOA");
                    if (objDOA != null && objDOA != DBNull.Value)
                    {
                        lblDOA.Text = objDOA.ToString() == "Y" ? "Yes" : "No";
                    }
                }
                #endregion

                #region | Permission |
                Label lblPermission = (Label)e.Row.FindControl("lblPermissionDoc");
                if (lblPermission != null)
                {
                    var objPermission = DataBinder.Eval(e.Row.DataItem, "PermissionType");
                    if (objPermission != null && objPermission != DBNull.Value)
                    {
                        lblPermission.Text = objPermission.ToString() == "Public" ? "เปิดเผย" : "ความลับ";
                    }
                }
                #endregion

                #region | Created Date |
                Label lblCreateDate = (Label)e.Row.FindControl("lblCreateDate");
                if (lblCreateDate != null)
                {
                    var objCreatedDate = DataBinder.Eval(e.Row.DataItem, "CreatedDate");
                    if (objCreatedDate != null && objCreatedDate != DBNull.Value)
                    {
                        lblCreateDate.Text = DateTime.Parse(objCreatedDate.ToString()).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        lblCreateDate.Text = "-";
                    }
                }
                #endregion

                #region | Created By |
                Label lblCreator = (Label)e.Row.FindControl("lblCreator");
                if (lblCreator != null)
                {
                    var objCreator = DataBinder.Eval(e.Row.DataItem, "CreatorID");
                    if (objCreator != null && objCreator != DBNull.Value)
                    {
                        var creator = Extension.GetSpecificEmployeeFromTemp(Page, objCreator.ToString());
                        lblCreator.Text = string.Format("{0} {1}", creator.FIRSTNAME_TH, creator.LASTNAME_TH);
                    }
                }
                #endregion

                #region | Employee Name |
                Label lblEmpName = (Label)e.Row.FindControl("lblEmpName");
                if (lblEmpName != null)
                {
                    var objEmployee = DataBinder.Eval(e.Row.DataItem, "EmpID");
                    if (objEmployee != null && objEmployee != DBNull.Value)
                    {
                        var Emp = Extension.GetSpecificEmployeeFromTemp(Page, objEmployee.ToString());
                        lblEmpName.Text = string.Format("{0} {1}", Emp.FIRSTNAME_TH, Emp.LASTNAME_TH);
                    }
                }
                #endregion

                #region | Action Date |
                Label lblActionDate = (Label)e.Row.FindControl("lblActionDate");
                if (lblActionDate != null)
                {
                    var objActionDate = DataBinder.Eval(e.Row.DataItem, "ActionDate");
                    if (objActionDate != null && objActionDate != DBNull.Value)
                    {
                        lblActionDate.Text = DateTime.Parse(objActionDate.ToString()).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        lblActionDate.Text = "-";
                    }
                }
                #endregion

                #region | Timing |
                Label lblTiming = (Label)e.Row.FindControl("lblTiming");
                if (lblTiming != null)
                {
                    var objTotalTime = DataBinder.Eval(e.Row.DataItem, "TotalTime");
                    if (objTotalTime != null && objTotalTime != DBNull.Value)
                    {
                        lblTiming.Text = objTotalTime.ToString();
                    }
                    else
                    {
                        lblTiming.Text = "-";
                    }
                }
                #endregion

            }
        }

        protected void gv_Report_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow gv_row in gv_Report.Rows)
            {
                if (gv_row.RowIndex == gv_Report.SelectedIndex)
                {
                    HiddenField hdnDocID = (HiddenField)gv_row.Cells[0].FindControl("hdnDocID");
                    if (hdnDocID != null)
                    {
                        DataClassesDataAccessDataContext dataContext = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                        var listHistory = dataContext.TRNHistories.Where(x => x.DocID == Convert.ToInt32(hdnDocID.Value)).ToList();
                        if (listHistory.Count > 0)
                        {
                            gv_History.DataSource = listHistory;
                            gv_History.DataBind();
                        }
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popHistory", "$('#showHistory').modal('show');", true);
                    }
                }
            }
        }
        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            foreach (GridViewRow r in gv_Report.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation(gv_Report.UniqueID, "Select$" + r.RowIndex);
                }
            }
            base.Render(writer);
        }

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
                    var objActionBy = DataBinder.Eval(e.Row.DataItem, "EmpID");
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
                                lblActionBy.Text = name_TH;
                            }
                        }
                    }
                    Label lblActionDate = (Label)e.Row.FindControl("lbl_ActionDate");
                    var objActionDate = DataBinder.Eval(e.Row.DataItem, "ActionDate");
                    if (lblActionDate != null)
                        if (objActionDate != null && objActionDate != DBNull.Value) lblActionDate.Text = ((DateTime)objActionDate).ToString("dd/MM/yyyy HH:mm:ss");


                    Label lblPosition = (Label)e.Row.FindControl("lbl_Position");
                    var objPosition = DataBinder.Eval(e.Row.DataItem, "PositionID");
                    if (lblPosition == null) return;
                    if (objPosition == null || objPosition == DBNull.Value || objEmp == null) return;
                    var Dept = objEmp.RESULT.First(x => x.POSITION_TD == objPosition.ToString());
                    lblPosition.Text = Dept.POSTION_NAME_TH;
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


    }
}
