using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace PIMEdoc_CR.MappingSecretary
{
    public partial class MappingSecretaryUserControl : UserControl
    {
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
        private string vs_empID
        {
            get
            {
                return (string)ViewState["vs_empID"];
            }
            set
            {
                ViewState["vs_empID"] = value;
            }
        }
        private DataTable vs_Secretary
        {
            get
            {
                if (ViewState["vs_Secretary"] == null)
                {
                    DataTable dt_Secretary = new DataTable();
                    dt_Secretary.Columns.Add("Row", typeof(Int32));
                    dt_Secretary.Columns.Add("Sequence", typeof(Int32));
                    dt_Secretary.Columns.Add("EmpID", typeof(String));
                    dt_Secretary.Columns.Add("SecretaryID", typeof(String));
                    dt_Secretary.Columns.Add("DepID", typeof(String));
                    dt_Secretary.Columns.Add("Especially", typeof(Boolean));
                    return dt_Secretary;
                }
                return (DataTable)ViewState["vs_Secretary"];
            }
            set
            {
                ViewState["vs_Secretary"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    InitialData();
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                Extension.MessageBox(this.Page, ex.Message);
            }
        }

        #region | Information |
        private void InitialData()
        {
            gv_Secretary.DataSource = new List<string>();
            gv_Secretary.DataBind();
        }

        protected void DepartmentBinding(List<string> listDept)
        {
            DataTable dtDepartment = Extension.GetDepartmentData(Page).Copy();
            DataView dv = dtDepartment.AsDataView();
            //dv.RowFilter = string.Format("PRIMARY = '{0}'", 1);
            dtDepartment = dv.ToTable();
            if (!dtDepartment.DataTableIsNullOrEmpty() && !chk_especially.Checked)
            {
                dtDepartment = dtDepartment.AsEnumerable().Where(r => listDept.Contains(r.Field<String>("DEPARTMENT_ID"))).CopyToDataTable();
            }
            if (!dtDepartment.DataTableIsNullOrEmpty())
            {
                ddl_Department.DataSource = dtDepartment;
                ddl_Department.DataTextField = "DEPARTMENT_NAME_TH";
                ddl_Department.DataValueField = "DEPARTMENT_ID";
                ddl_Department.DataBind();
                ddl_Department.Items.Insert(0, new ListItem("-- Please Select --", ""));
            }
        }
        protected void chk_especially_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(hdn_EmplyeeID.Value))
            {
                var empData = Extension.GetSpecificEmployeeFromTemp(Page, hdn_EmplyeeID.Value);
                List<string> listDept = new List<string>();
                foreach (var item in empData.RESULT)
                {
                    listDept.Add(item.DEPARTMENT_ID);
                }
                DepartmentBinding(listDept);
            }
            if (chk_especially.Checked)
            {
                panel_especially.Visible = true;
            }
            else
            {
                panel_especially.Visible = false;
                ddl_SubDepartment.DataSource = new List<string>();
                ddl_SubDepartment.DataBind();
                ddl_Position.DataSource = new List<string>();
                ddl_Position.DataBind();
            }
        }
        protected void ddl_Department_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                DataTable dtDepartment = Extension.GetDepartmentData(Page).Copy();
                if (!dtDepartment.DataTableIsNullOrEmpty() && chk_especially.Checked)
                {
                    dtDepartment = dtDepartment.AsEnumerable().Where(r => r.Field<string>("PARENT_ID") == ddl_Department.SelectedValue).CopyToDataTable();
                    if (!dtDepartment.DataTableIsNullOrEmpty())
                    {
                        ddl_SubDepartment.DataSource = dtDepartment;
                        ddl_SubDepartment.DataTextField = "DEPARTMENT_NAME_TH";
                        ddl_SubDepartment.DataValueField = "DEPARTMENT_ID";
                        ddl_SubDepartment.DataBind();
                        ddl_SubDepartment.Items.Insert(0, new ListItem("-- Please Select --", ""));
                    }
                }

            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                Extension.MessageBox(Page, "ไม่มีพบหน่วยงานย่อย กรุณาติดต่อเจ้าหน้าที่");
            }
        }
        protected void ddl_SubDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dtPosition = Extension.GetPositionData(Page);
            if (!dtPosition.DataTableIsNullOrEmpty() && chk_especially.Checked)
            {
                ddl_Position.DataSource = dtPosition;
                ddl_Position.DataTextField = "POSITION_NAME_TH";
                ddl_Position.DataValueField = "POSITION_ID";
                ddl_Position.DataBind();
                ddl_Position.Items.Insert(0, new ListItem("-- Please Select --", ""));
            }
        }
        protected void ddl_Position_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region | GV Secretary |
        protected void BindingGvScretary()
        {
            if (!string.IsNullOrWhiteSpace(hdn_EmplyeeID.Value))
            {
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                List<Secretary> listSecretary = db.Secretaries.Where(x => x.EmpID == hdn_EmplyeeID.Value && x.IsActive).ToList();
                if (listSecretary.Count > 0)
                {

                    DataTable dt = Extension.ListToDataTable<Secretary>(listSecretary);
                    dt.Columns.Add("Row");
                    int i = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        row["Row"] = i;
                        i++;
                    }
                    vs_Secretary = dt;
                    gv_Secretary.DataSource = vs_Secretary;
                    gv_Secretary.DataBind();
                    panel_gvSecretary.Visible = true;
                }
                else
                {
                    panel_gvSecretary.Visible = false;
                }
            }
        }
        protected void gv_Secretary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv_WorkList, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#bdcde4';this.style.cursor='pointer';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                #region | Employee |
                Label lblEmployee = (Label)e.Row.FindControl("lbl_EmployeeName");
                if (lblEmployee != null)
                {
                    var objEmp = DataBinder.Eval(e.Row.DataItem, "EmpID");
                    if (objEmp != null && objEmp != DBNull.Value)
                    {
                        SpecificEmployeeData.RootObject emp = Extension.GetSpecificEmployeeFromTemp(this.Page, objEmp.ToString());
                        if (emp != null)
                        {
                            string nameTH = string.Format("{0}{1} {2}", emp.PREFIX_TH, emp.FIRSTNAME_TH, emp.LASTNAME_TH);
                            lblEmployee.Text = nameTH;
                        }
                    }
                }
                #endregion

                #region | Secretary |
                Label lblSecretary = (Label)e.Row.FindControl("lbl_Secretary");
                if (lblSecretary != null)
                {
                    var objSecretary = DataBinder.Eval(e.Row.DataItem, "SecretaryID");
                    if (objSecretary != null && objSecretary != DBNull.Value)
                    {
                        SpecificEmployeeData.RootObject emp = Extension.GetSpecificEmployeeFromTemp(this.Page, objSecretary.ToString());
                        if (emp != null)
                        {
                            string nameTH = string.Format("{0}{1} {2}", emp.PREFIX_TH, emp.FIRSTNAME_TH, emp.LASTNAME_TH);
                            lblSecretary.Text = nameTH;
                        }
                    }
                }
                #endregion

                #region | Department |
                Label lbl_Department = (Label)e.Row.FindControl("lbl_Department");
                if (lbl_Department != null)
                {
                    var objDeptName = DataBinder.Eval(e.Row.DataItem, "DepID");
                    if (objDeptName != null && objDeptName != DBNull.Value)
                    {
                        DataTable dtDept = Extension.GetSpecificDepartmentDataFromTemp(Page, objDeptName.ToString());
                        if (!dtDept.DataTableIsNullOrEmpty())
                        {
                            lbl_Department.Text = dtDept.Rows[0]["DEPARTMENT_NAME_TH"].ToString(); ;
                        }
                    }
                }
                #endregion

                #region | Especially |
                Label lbl_Especially = (Label)e.Row.FindControl("lbl_Especially");
                if (lbl_Especially != null)
                {
                    var objSecretary = DataBinder.Eval(e.Row.DataItem, "Especially");
                    if (objSecretary != null && objSecretary != DBNull.Value)
                    {

                        lbl_Especially.Text = Convert.ToBoolean(objSecretary.ToString()) ? "Y" : "N";

                    }
                }
                #endregion
                #region | Modified Date |
                Label lbl_ModifiedDate = (Label)e.Row.FindControl("lbl_ModifiedDate");
                if (lbl_ModifiedDate != null)
                {
                    var objModifiedDate = DataBinder.Eval(e.Row.DataItem, "ModifiedDate");
                    if (objModifiedDate != null && objModifiedDate != DBNull.Value)
                    {
                        DateTime modDate;
                        if (DateTime.TryParse(objModifiedDate.ToString(), out modDate))
                        {
                            lbl_ModifiedDate.Text = modDate.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                        else
                        {
                            lbl_ModifiedDate.Text = "-";
                        }

                    }
                }
                #endregion
            }
        }
        protected void gv_Secretary_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dt_Secretary = vs_Secretary;
            int iRowIndex = Convert.ToInt32(e.CommandArgument) - 1;
            if (dt_Secretary != null)
            {
                if (e.CommandName == "DeleteItem")
                {
                    int sequence = Convert.ToInt32(dt_Secretary.Rows[iRowIndex]["Sequence"].ToString());
                    DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                    Secretary objSecretary = db.Secretaries.SingleOrDefault(x => x.Sequence == sequence);
                    if (objSecretary != null)
                    {

                        dt_Secretary.Rows[iRowIndex].Delete();
                        objSecretary.IsActive = false;
                        db.SubmitChanges();
                        if (Convert.ToBoolean(objSecretary.Especially))
                        {
                            bool isDistinct = true;
                            List<Secretary> listSecretary = db.Secretaries
                                .Where(x => (x.Especially ?? false) 
                                    && x.DepID == objSecretary.DepID 
                                    && x.SubDepID == objSecretary.SubDepID 
                                    && x.PosID == objSecretary.PosID 
                                    && x.IsActive)
                                .ToList();
                            foreach (var item in listSecretary)
                            {
                                isDistinct = false;
                            }
                            if (isDistinct)
                            {
                                TRNEmployeeExtension objEmpEx = db.TRNEmployeeExtensions.First(x =>
                                   x.EMPLOYEEID == objSecretary.EmpID &&
                                   x.DEPARTMENT_ID == objSecretary.DepID &&
                                   x.SUBDEPARTMENT_ID == objSecretary.SubDepID &&
                                   x.POSITION_TD == objSecretary.PosID);
                                if (objEmpEx != null)
                                {
                                    db.TRNEmployeeExtensions.DeleteOnSubmit(objEmpEx);
                                    db.SubmitChanges();
                                }
                            }

                        }
                        db.SubmitChanges();
                        int iSequence = 1;
                        foreach (DataRow dr in dt_Secretary.Rows)
                        {
                            dr["Row"] = iSequence;
                            iSequence++;
                        }

                        vs_Secretary = dt_Secretary;
                        gv_Secretary.DataSource = vs_Secretary;
                        gv_Secretary.DataBind();
                    }
                }
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
                case "btnSearchEmployee":
                    vs_popUpTarget = "Requestor";
                    break;
                case "btn_searchSecretaryEmp":
                    vs_popUpTarget = "Secretary";
                    break;
                default:
                    vs_popUpTarget = ""; break;
            }
            LoadDataEmpToCache("");
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popSearchToModal1", "$('#searchEmpReqModal').modal('show');", true);

        }
        protected void searchBtn_Click(object sender, EventArgs e)
        {
            gv_searchEmpReqTable.PageIndex = 0;
            LoadDataEmpToCache(txt_searchBox.Text);
        }
        private void LoadDataEmpToCache(string sFilterKeyword)
        {
            DataTable dtEmp = Extension.GetEmployeeData(this.Page).Copy();
            if (!string.IsNullOrEmpty(sFilterKeyword))
            {
                DataView dvFilter = dtEmp.Copy().DefaultView;
                dvFilter.RowFilter = string.Format(@"
				DEPARTMENT_NAME_TH LIKE '%{0}%'
				OR POSTION_NAME_TH LIKE '%{0}%'
				OR EmployeeID LIKE '%{0}%' 
				OR FIRSTNAME_TH LIKE '%{0}%' 
				OR LASTNAME_TH LIKE '%{0}%' 
				OR PREFIX_TH LIKE '%{0}%' 
				OR FIRSTNAME_EN LIKE '%{0}%' 
				OR LASTNAME_EN LIKE '%{0}%' 
				OR PREFIX_EN LIKE '%{0}%' 
				OR Email LIKE '%{0}%'", sFilterKeyword);
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
        protected void btnClosePopup_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchCCModal1", "$('#searchEmpReqModal').modal('hide');", true);
        }

        protected void gv_searchEmpReqTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_searchEmpReqTable.PageIndex = e.NewPageIndex;
            LoadDataEmpToCache(txt_searchBox.Text);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            foreach (GridViewRow r in gv_searchEmpReqTable.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation(gv_searchEmpReqTable.UniqueID, "Select$" + r.RowIndex);
                }
            }
            base.Render(writer);
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
                        string empNameTH = string.Format("{0}{1} {2}", empData.PREFIX_TH, empData.FIRSTNAME_TH, empData.LASTNAME_TH);
                        string empNameEN = string.Format("{0}{1} {2}", empData.PREFIX_EN, empData.FIRSTNAME_EN, empData.LASTNAME_EN);

                        switch (vs_popUpTarget)
                        {
                            case "Requestor":
                                lbl_EmployeeName.Text = empNameTH;
                                hdn_EmplyeeID.Value = empData.EMPLOYEEID;
                                List<string> listDept = new List<string>();
                                foreach (var item in empData.RESULT)
                                {
                                    listDept.Add(item.DEPARTMENT_ID);
                                }
                                DepartmentBinding(listDept);
                                BindingGvScretary();
                                break;
                            case "Secretary":
                                lbl_Secretary.Text = empNameTH;
                                hdn_SecretaryID.Value = empData.EMPLOYEEID;
                                break;
                        }

                    }


                }
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchEmpModal", "$('#searchEmpReqModal').modal('hide');", true);
        }
        #endregion

        private bool IsPassValidate(ref string sMessage)
        {
            if (string.IsNullOrEmpty(hdn_EmplyeeID.Value))
            {
                sMessage = "กรุณาระบุ Employee";
                lbl_EmployeeName.Focus();
                return false;
            }
            if (ddl_Department.SelectedIndex == 0)
            {
                sMessage = "กรุณาเลือกหน่วยงานหลัก";
                ddl_Department.Focus();
                return false;
            }
            if (chk_especially.Checked)
            {
                if (ddl_SubDepartment.SelectedIndex == 0)
                {
                    sMessage = "กรุณาเลือกหน่วยงานย่อย";
                    ddl_Department.Focus();
                    return false;
                }
                if (ddl_Position.SelectedIndex == 0)
                {
                    sMessage = "กรุณาเลือกตำแหน่ง";
                    ddl_Department.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(hdn_SecretaryID.Value))
            {
                sMessage = "กรุณาระบุ Secretary";
                lbl_Secretary.Focus();
                return false;
            }
            return true;
        }

        protected string SubmitData()
        {
            try
            {
                string sValidtionMsg = string.Empty;
                if (IsPassValidate(ref sValidtionMsg))
                {
                    DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                    #region | Add Secretary |
                    List<Secretary> listSecretary = db.Secretaries.Where(x => x.EmpID == hdn_EmplyeeID.Value && x.DepID == ddl_Department.SelectedValue).ToList();
                    Secretary objSecretary = new Secretary();
                    bool isUpdate = false;
                    #region | Update if exist |
                    if (listSecretary != null)
                    {
                        foreach (var item in listSecretary)
                        {
                            if (item.SecretaryID == hdn_SecretaryID.Value)
                            {
                                objSecretary = item;
                                objSecretary.ModifiedDate = DateTime.Now;
                                objSecretary.IsActive = true;
                                isUpdate = true;
                                break;
                            }
                        }
                    }
                    #endregion
                    if (!isUpdate)
                    {
                        objSecretary.EmpID = hdn_EmplyeeID.Value;
                        objSecretary.DepID = ddl_Department.SelectedValue;
                        if (chk_especially.Checked)
                        {
                            objSecretary.SubDepID = ddl_SubDepartment.SelectedValue;
                            objSecretary.PosID = ddl_Position.SelectedValue;
                        }
                        objSecretary.SecretaryID = hdn_SecretaryID.Value;
                        objSecretary.Especially = chk_especially.Checked;
                        objSecretary.CreateDate = DateTime.Now;
                        objSecretary.ModifiedDate = DateTime.Now;
                        objSecretary.IsActive = true;

                        db.Secretaries.InsertOnSubmit(objSecretary);
                    }
                    db.SubmitChanges();
                    #endregion

                    #region | Add Special Employee's department detail |
                    if (chk_especially.Checked)
                    {
                        bool isExUpdate = false;
                        TRNEmployeeExtension objEmpEx = new TRNEmployeeExtension();
                        List<TRNEmployeeExtension> listEmpEx = db.TRNEmployeeExtensions
                            .Where(x => x.EMPLOYEEID == hdn_EmplyeeID.Value
                                && x.DEPARTMENT_ID == ddl_Department.SelectedValue
                                && x.SUBDEPARTMENT_ID == ddl_SubDepartment.SelectedValue
                                && x.POSITION_TD == ddl_Position.SelectedValue)
                            .ToList();
                        #region | Delete if exist |
                        if (listEmpEx != null)
                        {
                            foreach (TRNEmployeeExtension item in listEmpEx)
                            {
                                isExUpdate = true;
                                break;
                            }
                            //var queryEE = (from TRNEmployeeExtension emp in listEmpEx
                            //               where emp.EMPLOYEEID == hdn_EmplyeeID.Value && emp.DEPARTMENT_ID == ddl_Department.SelectedValue
                            //               select emp);
                            //db.TRNEmployeeExtensions.DeleteAllOnSubmit(queryEE);
                            //db.SubmitChanges();
                        }
                        #endregion
                        if (!isExUpdate)
                        {
                            var objEmp = Extension.GetSpecificEmployeeFromTemp(Page, hdn_EmplyeeID.Value);
                            objEmpEx.EMPLOYEEID = objEmp.EMPLOYEEID;
                            objEmpEx.USERNAME = objEmp.USERNAME;
                            objEmpEx.PREFIX_TH = objEmp.PREFIX_TH;
                            objEmpEx.FIRSTNAME_TH = objEmp.FIRSTNAME_TH;
                            objEmpEx.LASTNAME_TH = objEmp.LASTNAME_TH;
                            objEmpEx.PREFIX_EN = objEmp.PREFIX_EN;
                            objEmpEx.FIRSTNAME_EN = objEmp.FIRSTNAME_EN;
                            objEmpEx.LASTNAME_EN = objEmp.LASTNAME_EN;
                            objEmpEx.EMAIL = objEmp.EMAIL;
                            objEmpEx.BIRTHDATE = DateTime.Parse(objEmp.BIRTHDATE);
                            objEmpEx.TELEPHONE = objEmp.TELEPHONE;
                            objEmpEx.MOBILE = objEmp.MOBILE;
                            objEmpEx.MANAGERID = objEmp.MANAGERID;
                            objEmpEx.MANAGERUSERNAME = objEmp.MANAGERUSERNAME;
                            objEmpEx.MODIFIED_BY = objEmp.MODIFIED_BY;
                            objEmpEx.MODIFIED_DATETIME = DateTime.Parse(objEmp.MODIFIED_DATETIME);
                            objEmpEx.EmployeeName_EN = string.Format("{0}{1} {2}", objEmp.PREFIX_EN, objEmp.FIRSTNAME_EN, objEmp.LASTNAME_EN);
                            objEmpEx.EmployeeName_TH = string.Format("{0}{1} {2}", objEmp.PREFIX_TH, objEmp.FIRSTNAME_TH, objEmp.LASTNAME_TH);

                            DataTable dtDept = Extension.GetDepartmentData(Page).Copy();
                            dtDept = dtDept.AsEnumerable().Where(r => r.Field<string>("DEPARTMENT_ID") == ddl_Department.SelectedValue).CopyToDataTable();
                            if (!dtDept.DataTableIsNullOrEmpty())
                            {
                                objEmpEx.DEPARTMENT_ID = ddl_Department.SelectedValue;
                                objEmpEx.DEPARTMENT_NAME_TH = dtDept.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                                objEmpEx.DEPARTMENT_NAME_EN = dtDept.Rows[0]["DEPARTMENT_NAME_EN"].ToString();
                            }
                            dtDept = Extension.GetDepartmentData(Page).Copy();
                            dtDept = dtDept.AsEnumerable().Where(r => r.Field<string>("DEPARTMENT_ID") == ddl_SubDepartment.SelectedValue).CopyToDataTable();
                            if (!dtDept.DataTableIsNullOrEmpty())
                            {
                                objEmpEx.SUBDEPARTMENT_ID = dtDept.Rows[0]["DEPARTMENT_ID"].ToString();
                                objEmpEx.SUBDEPARTMENT_NAME_TH = dtDept.Rows[0]["DEPARTMENT_NAME_TH"].ToString();
                                objEmpEx.SUBDEPARTMENT_NAME_EN = dtDept.Rows[0]["DEPARTMENT_NAME_EN"].ToString();
                            }
                            dtDept = Extension.GetPositionData(Page).Copy();
                            dtDept = dtDept.AsEnumerable().Where(r => r.Field<string>("POSITION_ID") == ddl_Position.SelectedValue).CopyToDataTable();
                            if (!dtDept.DataTableIsNullOrEmpty())
                            {
                                objEmpEx.POSITION_TD = dtDept.Rows[0]["POSITION_ID"].ToString();
                                objEmpEx.POSTION_NAME_TH = dtDept.Rows[0]["POSITION_NAME_TH"].ToString();
                                objEmpEx.POSTION_NAME_EN = dtDept.Rows[0]["POSITION_NAME_EN"].ToString();
                            }

                            db.TRNEmployeeExtensions.InsertOnSubmit(objEmpEx);
                        }
                        db.SubmitChanges();
                    }
                    #endregion

                    return "";
                }
                return sValidtionMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            string sResult = SubmitData();
            if (string.IsNullOrWhiteSpace(sResult))
            {
                Extension.MessageBox(Page, "Mapping Secretary Completed");
                ddl_Department.SelectedIndex = 0;
                lbl_Secretary.Text = "";
                hdn_SecretaryID.Value = "";
                BindingGvScretary();
            }
            else
            {
                Extension.MessageBox(Page, sResult);
            }
        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            ddl_Department.SelectedIndex = 0;
            lbl_EmployeeName.Text = "";
            hdn_EmplyeeID.Value = "";
            lbl_Secretary.Text = "";
            hdn_SecretaryID.Value = "";

            gv_Secretary.DataSource = new List<string>();
            gv_Secretary.DataBind();

        }
    }
}
