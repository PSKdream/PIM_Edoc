using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


namespace PIMEdoc_CR.ReportLogIn
{
    public partial class ReportLogInUserControl : UserControl
    {

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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitialData();
            }
        }

        #region | Information |
        protected void InitialData()
        {
            txt_DailyFrom.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txt_DailyTo.Text = DateTime.Now.ToString("dd/MM/yyyy");

            List<int> listYear = new List<int>();
            for (int i = 2017; i <= DateTime.Now.Year; i++)
            {
                listYear.Add(i);
            }
            ddl_FromYear.DataSource = listYear;
            ddl_FromYear.DataBind();

            ddl_ToYear.DataSource = listYear;
            ddl_ToYear.DataBind();

            ddl_OnMonthYear.DataSource = listYear;
            ddl_OnMonthYear.DataBind();

            ddl_MonthlyYear.DataSource = listYear;
            ddl_MonthlyYear.DataBind();            
        }

        protected void rdb_FilterByEmpDept_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdb_FilterByEmpDept.SelectedValue == "emp")
            {
                panel_searchEmp.Visible = true;
                panel_searchDept.Visible = false;
                hdn_DepartmentID.Value = "";
                //hdn_SubDepartmentID.Value = "";
            }
            else
            {
                panel_searchEmp.Visible = false;
                panel_searchDept.Visible = true;
                hdn_EmployeeID.Value = "";
            }
        }
        protected void ddl_FilterByTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel_Daily.Visible = false;
            panel_Weekly.Visible = false;
            panel_OnMonth.Visible = false;
            panel_Monthly.Visible = false;
            panel_Year.Visible = false;

            switch (ddl_FilterByTimePeriod.SelectedValue)
            {
                case "Daily":
                    panel_Daily.Visible = true;
                    txt_DailyFrom.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    txt_DailyTo.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    break;
                case "Weekly":
                    panel_Weekly.Visible = true;
                    txt_WeeklyFrom.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    txt_WeeklyTo.Text = (DateTime.Now.AddDays(6)).ToString("dd/MM/yyyy");
                    break;
                case "Month":
                    panel_OnMonth.Visible = true;
                    ddl_OnMonth.SelectedValue = DateTime.Now.Month.ToString();
                    ddl_OnMonthYear.SelectedValue = DateTime.Now.Year.ToString();
                    break;
                case "Monthly":
                    panel_Monthly.Visible = true;
                    ddl_MonthlyYear.SelectedValue = DateTime.Now.Year.ToString();
                    ddl_FromMonth.SelectedValue = DateTime.Now.Month.ToString();
                    ddl_ToMonth.SelectedValue = DateTime.Now.Month.ToString();
                    break;
                case "Year":
                    panel_Year.Visible = true;
                    ddl_FromYear.SelectedValue = DateTime.Now.Year.ToString();
                    ddl_ToYear.SelectedValue = DateTime.Now.Year.ToString();
                    break;
                default: break;
            }
        }
        #region | Date Changed |

        protected void txt_DailyFrom_TextChanged(object sender, EventArgs e)
        {
            DateTime dailyFrom = DateTime.ParseExact(txt_DailyFrom.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
            DateTime dailyTo = DateTime.ParseExact(txt_DailyTo.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
            if (dailyFrom > dailyTo)
            {
                txt_DailyTo.Text = txt_DailyFrom.Text;
            }
        }
        protected void txt_DailyTo_TextChanged(object sender, EventArgs e)
        {
            DateTime dailyFrom = DateTime.ParseExact(txt_DailyFrom.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
            DateTime dailyTo = DateTime.ParseExact(txt_DailyTo.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
            if (dailyFrom > dailyTo)
            {
                txt_DailyFrom.Text = txt_DailyTo.Text;
            }
        }

        protected void txt_WeeklyFrom_TextChanged(object sender, EventArgs e)
        {
            DateTime weeklyFrom = DateTime.ParseExact(txt_WeeklyFrom.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
            txt_WeeklyTo.Text = weeklyFrom.AddDays(6).ToString("dd/MM/yyyy");
        }
        protected void txt_WeeklyTo_TextChanged(object sender, EventArgs e)
        {
            DateTime weeklyTo = DateTime.ParseExact(txt_WeeklyTo.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
            txt_WeeklyFrom.Text = weeklyTo.AddDays(-6).ToString("dd/MM/yyyy");
        }

        protected void ddl_FromMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(ddl_FromMonth.SelectedValue) > Convert.ToInt32(ddl_ToMonth.SelectedValue))
            {
                ddl_ToMonth.SelectedValue = ddl_FromMonth.SelectedValue;
            }
        }
        protected void ddl_ToMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(ddl_FromMonth.SelectedValue) > Convert.ToInt32(ddl_ToMonth.SelectedValue))
            {
                ddl_FromMonth.SelectedValue = ddl_ToMonth.SelectedValue;
            }
        }


        protected void ddl_FromYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(ddl_FromYear.SelectedValue) > Convert.ToInt32(ddl_ToYear.SelectedValue))
            {
                ddl_ToYear.SelectedValue = ddl_FromYear.SelectedValue;
            }
        }
        protected void ddl_ToYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(ddl_FromYear.SelectedValue) > Convert.ToInt32(ddl_ToYear.SelectedValue))
            {
                ddl_FromYear.SelectedValue = ddl_ToYear.SelectedValue;
            }
        }
        #endregion
        #endregion

        #region | Popup Search |
        #region | Employee |
        public void OpenPopup(object sender, EventArgs e)
        {
            txt_searchBox.Text = "";

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
            foreach (GridViewRow r in gv_SearchDepartment.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation(gv_SearchDepartment.UniqueID, "Select$" + r.RowIndex);
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

                        lbl_Employee.Text = empNameTH;
                        hdn_EmployeeID.Value = sEmpID;

                    }


                }
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchEmpModal", "$('#searchEmpReqModal').modal('hide');", true);
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
            DataTable dt = new DataTable();
            dt.Columns.Add("key");
            dt.Columns.Add("value");
            DataRow dr = dt.NewRow();
            switch (sender is LinkButton ? linkBtn.ID : btn.ID)
            {
                case "btnSearchDepartment":
                    vs_DeptTarget = "Department";
                    dr["key"] = "Level 1: คณะ/สำนัก/ศูนย์";
                    dr["value"] = "1";
                    dt.Rows.Add(dr);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hideToDepartment", "$('#toDepartmentModal').modal('hide');", true);
                    break;
                case "btnSearchSubDepartment":
                    vs_DeptTarget = "SubDepartment";
                    dr["key"] = "Level 2: งานต่างๆที่อยู่ภายใต้ คณะ/สำนัก";
                    dr["value"] = "0";
                    dt.Rows.Add(dr);
                    break;
                default:
                    vs_DeptTarget = ""; break;
            }

            ddl_seachDeptLevel.DataSource = dt;
            ddl_seachDeptLevel.DataTextField = "key";
            ddl_seachDeptLevel.DataValueField = "value";
            ddl_seachDeptLevel.DataBind();

            txtSearch_Department.Text = "";
            ddl_seachDeptLevel.SelectedValue = "1";
            loadDataDept();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popSearchToModal1", "$('#searchDepartmentModal').modal('show');", true);
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
                var objDocLv = DataBinder.Eval(e.Row.DataItem, "PRIMARY");
                if (lbl_DepartmentLevel != null)
                    if (objDocLv != null && objDocLv != DBNull.Value) lbl_DepartmentLevel.Text = objDocLv.ToString() == "1" ? "1" : "2";
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
                                case "Department":
                                    lbl_Department.Text = dtDep.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                                    hdn_DepartmentID.Value = dtDep.Rows[0]["DEPARTMENT_ID"].ToString();

                                    DataView dvFilter1 = Extension.GetDepartmentData(this.Page).Copy().DefaultView;
                                    dvFilter1.RowFilter = string.Format(@"PRIMARY = '{0}'
                                                                         AND PARENT_ID = '{1}'
                                                                         ", 0, hdn_DepartmentID.Value);
                                    DataTable dtDept = dvFilter1.ToTable();

                                    ddl_SubDepartment.DataSource = dtDept;
                                    ddl_SubDepartment.DataTextField = "DEPARTMENT_NAME_TH";
                                    ddl_SubDepartment.DataValueField = "DEPARTMENT_ID";
                                    ddl_SubDepartment.DataBind();
                                    ddl_SubDepartment.Items.Insert(0, new ListItem("-- All Sub Department --", ""));
                                    break;
                                case "SubDepartment":
                                    //lbl_SubDepartment.Text = dtDep.Rows[0]["SUBDEPARTMENT_NAME_TH"].ToString();
                                    //hdn_SubDepartmentID.Value = dtDep.Rows[0]["SUBDEPARTMENT_ID"].ToString();
                                    break;
                                default:
                                    lbl_Department.Text = dtDep.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                                    hdn_DepartmentID.Value = dtDep.Rows[0]["DEPARTMENT_ID"].ToString();
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

        #endregion

        #region | Graph |
        protected void GenerateData(string sType)
        {
            try
            {
                #region | Data Series |
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                List<TRNDocument> listDocument = db.TRNDocuments.Where(x => x.Status == "Completed").ToList();
                //List<TRNDocument> listDocument = db.TRNDocuments.Where(x => x.Status == "Completed" && x.Category == ddl_Category.SelectedValue).ToList();
                string oDocType = "";
                #region | Filter TRNDocuemtnList |
                if (!string.IsNullOrWhiteSpace(hdn_EmployeeID.Value))
                {
                    listDocument = listDocument.Where(x => x.RequestorID == hdn_EmployeeID.Value).ToList();
                }
                if (!string.IsNullOrWhiteSpace(hdn_DepartmentID.Value))
                {
                    listDocument = listDocument.Where(x => x.RequestorDepartmentID == Convert.ToInt32(hdn_DepartmentID.Value)).ToList();
                }
                if (ddl_SubDepartment.SelectedIndex > 0)
                {
                    listDocument = listDocument.Where(x => x.RequestorSubDepartmentID == Convert.ToInt32(ddl_SubDepartment.SelectedValue)).ToList();
                }
                
                #endregion
                string oData = "";
                switch (ddl_FilterByTimePeriod.SelectedValue)
                {
                    #region | Daily |
                    case "Daily":
                        DateTime dailyStartDate = DateTime.ParseExact(txt_DailyFrom.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
                        DateTime dailyEndDate = DateTime.ParseExact(txt_DailyTo.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
                        listDocument = listDocument
                            .Where(x => ((DateTime)x.ApproveDate).Date >= dailyStartDate.Date
                                     && ((DateTime)x.ApproveDate).Date <= dailyEndDate.Date)
                                     .ToList();
                        int dailyDifferent = Convert.ToInt32(dailyEndDate.Subtract(dailyStartDate).TotalDays);
                        oData = ConvertListToSeries(listDocument, 0, dailyDifferent, oDocType, false, false, false, dailyStartDate);
                        break;
                        #region | Backup Pie Serie Generator |

                        var DocTypeGroup = listDocument.GroupBy(x => x.DocTypeCode).Select(grp => new { docType = SharedRules.GetDocTypeName(grp.Key), total = grp.Count() }).ToArray();

                        //selected all DocType return "";
                        string DocTypeName = SharedRules.GetDocTypeName(oDocType);

                        //Select Specific DocType
                        if (!string.IsNullOrWhiteSpace(DocTypeName))
                        {
                            //Add Data If Data Exist
                            //string temp = string.Format("['{0}', {1}]", DocTypeName, 0);
                            if (DocTypeGroup.Any(x => x.docType == oData))
                            {
                                var item = DocTypeGroup.First(x => x.docType == oData);
                                oData = string.Format("['{0}', {1}]", DocTypeName, item.total);
                            }
                            //oData = temp;
                        }
                        //Select All DocType
                        else
                        {
                            //Query All DocType
                            DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
                            if (!dtDocType.DataTableIsNullOrEmpty())
                            {
                                foreach (DataRow dr in dtDocType.Rows)
                                {
                                    string temp = "";
                                    //Add Data If Data Exist
                                    if (DocTypeGroup.Any(x => x.docType == dr["DocTypeName"].ToString()))
                                    {
                                        var item = DocTypeGroup.First(x => x.docType == dr["DocTypeName"].ToString());
                                        temp = string.Format("['{0}', {1}]", dr["DocTypeName"], item.total);
                                    }
                                    if (string.IsNullOrWhiteSpace(oData))
                                    {
                                        oData = temp;
                                    }
                                    else
                                    {
                                        oData += string.Format(",{0}", temp);
                                    }
                                }

                            }
                        }
                        #endregion
                    #endregion
                    #region | Weekly |
                    case "Weekly":
                        DateTime weeklyStartDate = DateTime.ParseExact(txt_WeeklyFrom.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
                        DateTime weeklyEndDate = DateTime.ParseExact(txt_WeeklyTo.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
                        listDocument = listDocument
                            .Where(x => ((DateTime)x.ApproveDate).Date >= weeklyStartDate.Date
                                     && ((DateTime)x.ApproveDate).Date <= weeklyEndDate.Date)
                            .ToList();
                        int weeklyDifferent = Convert.ToInt32(weeklyEndDate.Subtract(weeklyStartDate).TotalDays);
                        oData = ConvertListToSeries(listDocument, 0, weeklyDifferent, oDocType, false, false, false, weeklyStartDate);
                        break;
                    #endregion
                    #region | On Month |
                    case "Month":
                        listDocument = listDocument
                            .Where(x => ((DateTime)x.ApproveDate).Month == Convert.ToInt32(ddl_OnMonth.SelectedValue)
                                    && ((DateTime)x.ApproveDate).Year == Convert.ToInt32(ddl_OnMonthYear.SelectedValue))
                            .ToList();
                        int daysInMonth = DateTime.DaysInMonth(Convert.ToInt32(ddl_OnMonthYear.SelectedValue), Convert.ToInt32(ddl_OnMonth.SelectedValue));
                        oData = ConvertListToSeries(listDocument, 1, daysInMonth, oDocType, false, false, true, null);
                        break;
                    #endregion
                    #region | Monthly |
                    case "Monthly":
                        listDocument = listDocument
                            .Where(x => ((DateTime)x.ApproveDate).Year == Convert.ToInt32(ddl_MonthlyYear.SelectedValue))
                            .ToList();
                        oData = ConvertListToSeries(listDocument, Convert.ToInt32(ddl_FromMonth.SelectedValue), Convert.ToInt32(ddl_ToMonth.SelectedValue), oDocType, false, true, false, null);
                        break;
                    #region | Backup Generator |
                    //var monthlyListDoc = listDocument
                    //    .GroupBy(x => x.DocTypeCode)
                    //    .Select(grp => new
                    //    {
                    //        docType = SharedRules.GetDocTypeName(grp.Key),
                    //        arr = listDocument
                    //                .Where(s => s.DocTypeCode == grp.Key)
                    //                .GroupBy(x => ((DateTime)x.ApproveDate).Month)
                    //                .Select(newGrp => new
                    //                {
                    //                    month = newGrp.Key,
                    //                    total = newGrp.Count()
                    //                })
                    //                .ToArray()
                    //    }).ToList();
                    //foreach (var month in monthlyListDoc)
                    //{
                    //    List<int> listTotal = new List<int>();
                    //    for (int i = Convert.ToInt32(ddl_FromMonth.SelectedValue); i <= Convert.ToInt32(ddl_ToMonth.SelectedValue); i++)
                    //    {
                    //        bool isAdd = false;
                    //        foreach (var arr in month.arr)
                    //        {
                    //            if (arr.month == i)
                    //            {
                    //                listTotal.Add(arr.total);
                    //                isAdd = true;
                    //                break;
                    //            }
                    //        }
                    //        if (!isAdd)
                    //        {
                    //            listTotal.Add(0);
                    //        }
                    //    }
                    //    string valueArr = "";
                    //    foreach (var item in listTotal)
                    //    {
                    //        if (string.IsNullOrWhiteSpace(valueArr))
                    //        {
                    //            valueArr = item.ToString();
                    //        }
                    //        else
                    //        {
                    //            valueArr += string.Format(",{0}", item);
                    //        }
                    //    }
                    //    valueArr = string.Format("[{0}]", valueArr);

                    //    string temp = string.Format("{0}name:'{2}', data:{3} {1}", "{", "}", month.docType, valueArr);
                    //    if (string.IsNullOrWhiteSpace(oData))
                    //    {
                    //        oData = temp;
                    //    }
                    //    else
                    //    {
                    //        oData += string.Format(",{0}", temp);
                    //    }
                    //}
                    #endregion
                    #endregion
                    #region | Year |
                    case "Year":
                        listDocument = listDocument
                            .Where(x => ((DateTime)x.ApproveDate).Year == Convert.ToInt32(ddl_FromYear.SelectedValue)
                                    && ((DateTime)x.ApproveDate).Year == Convert.ToInt32(ddl_ToYear.SelectedValue))
                            .ToList();
                        oData = ConvertListToSeries(listDocument, Convert.ToInt32(ddl_FromYear.SelectedValue), Convert.ToInt32(ddl_ToYear.SelectedValue), oDocType, true, false, false, null);
                        break;
                    #endregion
                    default: break;
                }
                #endregion

                #region | Category xAxis |
                string oCategory = "";
                int start = 0;
                int end = 0;
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                string periodFilter = ddl_FilterByTimePeriod.SelectedValue;
                switch (periodFilter)
                {
                    case "Daily":
                        startDate = DateTime.ParseExact(txt_DailyFrom.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
                        endDate = DateTime.ParseExact(txt_DailyTo.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
                        break;
                    case "Weekly":
                        startDate = DateTime.ParseExact(txt_WeeklyFrom.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"));
                        endDate = DateTime.ParseExact(txt_WeeklyTo.Text, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB")).AddDays(1);
                        break;
                    case "Month":
                        start = 1;
                        end = DateTime.DaysInMonth(Convert.ToInt32(ddl_OnMonthYear.SelectedValue), Convert.ToInt32(ddl_OnMonth.SelectedValue));
                        break;
                    case "Monthly":
                        start = Convert.ToInt32(ddl_FromMonth.SelectedValue);
                        end = Convert.ToInt32(ddl_ToMonth.SelectedValue);
                        break;
                    case "Year":
                        start = Convert.ToInt32(ddl_FromYear.SelectedValue);
                        end = Convert.ToInt32(ddl_ToYear.SelectedValue);
                        break;
                    default: break;
                }

                if (periodFilter == "Weekly" || periodFilter == "Daily")
                {
                    int different = Convert.ToInt32(endDate.Subtract(startDate).TotalDays);
                    for (int i = 0; i <= different; i++)
                    {
                        if (string.IsNullOrWhiteSpace(oCategory))
                        {
                            oCategory = string.Format("'{0}'", startDate.ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            oCategory += string.Format(",'{0}'", startDate.ToString("dd/MM/yyyy"));
                        }
                        startDate = startDate.AddDays(1);
                    }
                }
                else
                {
                    for (int i = start; i <= end; i++)
                    {
                        string index = i.ToString();
                        if (periodFilter == "Monthly")
                        {
                            index = Extension.GetENMonth(i);
                        }
                        if (string.IsNullOrWhiteSpace(oCategory))
                        {
                            oCategory = string.Format("'{0}'", index);
                        }
                        else
                        {
                            oCategory += string.Format(",'{0}'", index);
                        }
                    }
                }

                #endregion

                #region | yAxis |
                string oyAxis = "'Document (Completed)'";
                #endregion

                GenerateColumn(ddl_FilterByTimePeriod.SelectedValue, ShowGraph.ClientID, sType, oData, oCategory, oyAxis);

                //if (sType == "pie")
                //{
                //    GeneratePie(ddl_FilterByTimePeriod.SelectedValue, ShowGraph.ClientID, sType, oData);
                //}
                //else
                //{
                //    GenerateColumn(ddl_FilterByTimePeriod.SelectedValue, ShowGraph.ClientID, sType, oData, oCategory, oyAxis);
                //}

            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }

        protected string ConvertListToSeries(List<PIMEdoc_CR.TRNDocument> sListDocument, int sStart, int sEnd, string sDocTypeCode, bool isYear, bool isMonth, bool isInMonth, DateTime? sStartDate)
        {
            string oData = "";
            DataTable dtDocType;
            List<string[,]> oSeries = new List<string[,]>();
            if (string.IsNullOrWhiteSpace(sDocTypeCode))
            {
                dtDocType = SharedRules.GetList("MstDocumentType", "<Where><Eq><FieldRef Name='Level' /><Value Type='Number'>0</Value></Eq></Where>");
            }
            else
            {
                dtDocType = SharedRules.GetList("MstDocumentType", string.Format("<Where><And><Eq><FieldRef Name='Value' /><Value Type='Text'>{0}</Value></Eq><Eq><FieldRef Name='Level' /><Value Type='Number'>0</Value></Eq></And></Where>", sDocTypeCode));
            }
            //insert Name: & Data:
            foreach (DataRow row in dtDocType.Rows)
            {
                string[,] item = new string[2, 2];
                item[0, 0] = "name:";
                item[0, 1] = string.Format("'{0}'", row["DocTypeName"]);
                item[1, 0] = "data:";
                item[1, 1] = "";
                oSeries.Add(item);
            }
            for (int i = sStart; i <= sEnd; i++)
            {
                foreach (var serie in oSeries)
                {
                    List<TRNDocument> tempList = sListDocument;
                    if (isYear)
                    {
                        tempList = tempList.Where(x => ((DateTime)x.ApproveDate).Year == i).ToList();
                    }
                    else if (isMonth)
                    {
                        tempList = tempList.Where(x => ((DateTime)x.ApproveDate).Month == i).ToList();
                    }
                    else if (isInMonth)
                    {
                        tempList = tempList.Where(x => ((DateTime)x.ApproveDate).Day == i).ToList();
                    }
                    else
                    {
                        if (sStartDate != null)
                        {
                            tempList = tempList.Where(x => ((DateTime)x.ApproveDate).Date == ((DateTime)sStartDate).AddDays(i)).ToList();
                        }
                    }

                    var counter = tempList
                        .Where(x => SharedRules.GetDocTypeName(x.DocTypeCode) == serie[0, 1].Replace("'", ""))
                        .ToList()
                        .Count;
                    if (string.IsNullOrWhiteSpace(serie[1, 1]))
                    {
                        serie[1, 1] = counter.ToString();
                    }
                    else
                    {
                        serie[1, 1] += string.Format(",{0}", counter);
                    }
                }
            }
            //insert [] to data
            foreach (var serie in oSeries)
            {
                serie[1, 1] = string.Format("[{0}]", serie[1, 1]);
                if (string.IsNullOrWhiteSpace(oData))
                {
                    oData = string.Format("{0} {2} {3}, {4} {5} {1}", "{", "}", serie[0, 0], serie[0, 1], serie[1, 0], serie[1, 1]);
                }
                else
                {
                    oData += string.Format(", {0} {2} {3}, {4} {5} {1}", "{", "}", serie[0, 0], serie[0, 1], serie[1, 0], serie[1, 1]);

                }
            }
            return oData;
        }

        protected void GeneratePie(string sTitle, string sID, string sType, string sData)
        {
            //            string oData = string.Format(@"oData = [{0}
            //                type: '{2}',
            //                name: 'Report',
            //                innerSize: '0%',
            //                data: [
            //                {3}
            //            ]
            //            {1}];", "{", "}", sType, sData);
            //            string oTitle = string.Format("oTitle = '{0}';", sTitle);

            //            if (string.IsNullOrWhiteSpace(oData))
            //            {
            //                oTitle = "There are no Document in this period.";
            //            }
            //            string oID = string.Format("oID = '#{0}';", sID);
            //            string sGraph = String.Format(@"{0} {1} {2} pieByStage();", oData, oID, oTitle);
            //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "GeneratePie", sGraph, true);

        }

        protected void GenerateColumn(string sTitle, string sID, string sType, string sData, string sCategory, string syAxis)
        {
            string oData = string.Format(@"oData = [{0}];", sData);
            string oTitle = string.Format("oTitle = '{0}';", sTitle);
            string oCategory = string.Format("oCategory = [{0}];", sCategory);
            string oyAxis = string.Format("oyAxis = {0};", syAxis);
            string oID = string.Format("oID = '#{0}';", sID);
            string sGraph = String.Format(@"{0} {1} {2} {3} {4} columnByState();", oData, oID, oTitle, oCategory, oyAxis);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "GenerateColumn", sGraph, true);

        }
        #endregion

        #region | Button Action |
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            string selectedType = ddl_FilterByTimePeriod.SelectedValue;
            string oType = "column";
            if (selectedType == "Daily" || selectedType == "Weekly")
            {
                oType = "pie";
            }
            GenerateData(oType);
        }
        #endregion

    }
}
