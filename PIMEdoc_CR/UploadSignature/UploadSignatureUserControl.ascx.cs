using Microsoft.SharePoint;
using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace PIMEdoc_CR.Default.UploadSignature
{
    public partial class UploadSignatureUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        #region "Popup Requestor"
        public void OpenPopup(object sender, EventArgs e)
        {
            txt_searchBox.Text = "";

            gv_searchEmpReqTable.DataSource = new List<string>();
            gv_searchEmpReqTable.DataBind();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popSearchToModal1", "$('#searchEmpReqModal').modal('show');", true);

        }
        protected void searchBtn_Click(object sender, EventArgs e)
        {
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

                        lbl_EmployeeName.Text = empNameTH;
                        hdn_EmplyeeID.Value = empData.EMPLOYEEID;

                        //show signature if exist
                        DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                        List<SignatureImage> listSignatureImages = db.SignatureImages.Where(x => x.EmployeeID == hdn_EmplyeeID.Value).ToList();
                        if (listSignatureImages.Count > 0)
                        {
                            foreach (var item in listSignatureImages)
                            {
                                imgViewFile.ImageUrl = item.Path;
                                imgViewFile.Width = 400;
                                panel_FileName.Visible = true;
                                lbl_UplaodFileName.Text = item.FileName;
                                btn_Upload.OnClientClick =
                                    "return confirm('Are you sure to replace the original file?');";
                            }
                        }
                        else
                        {
                            btn_Upload.OnClientClick = "";
                        }

                    }


                }
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "downSearchEmpModal", "$('#searchEmpReqModal').modal('hide');", true);
        }
        #endregion



        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lbl_EmployeeName.Text))
            {
                string DocLibName = "Signature";
                if (fileUpload.HasFile)
                {
                    if (new FileInfo(fileUpload.FileName).Extension.ToLower() == ".gif")
                    {
                        //string NewName = hdn_EmplyeeID.Value + (new FileInfo(fileUpload.FileName)).Extension;
                        string NewName = hdn_EmplyeeID.Value + ".gif";
                        //SharedRules objSPLIB = new SharedRules();
                        SharedRules.UploadFileIntoDocumentLibrary(DocLibName, NewName, fileUpload.PostedFile.InputStream);

                        imgViewFile.ImageUrl = string.Format("{0}/{1}/{2}", SPContext.Current.Site.Url, DocLibName, NewName);
                        imgViewFile.Width = 400;
                        panel_FileName.Visible = true;
                        lbl_UplaodFileName.Text = NewName;

                        DataClassesDataAccessDataContext db =
                            new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                        List<SignatureImage> listSignatureImages =
                            db.SignatureImages.Where(x => x.EmployeeID == hdn_EmplyeeID.Value).ToList();
                        if (listSignatureImages.Count == 0)
                        {
                            SignatureImage objSignatureImage = new SignatureImage
                            {
                                EmployeeID = hdn_EmplyeeID.Value,
                                Path = imgViewFile.ImageUrl,
                                FileName = NewName
                            };

                            listSignatureImages.Add(objSignatureImage);
                            db.SignatureImages.InsertAllOnSubmit(listSignatureImages);
                            db.SubmitChanges();

                            btn_Upload.OnClientClick =
                                "return confirm('Are you sure to replace the original file?');";
                        }
                    }
                    else
                    {
                        Extension.MessageBox(this.Page, "กรุณาอัพโหลดเอกสารสกุล gif");
                    }
                }
                else
                {
                    Extension.MessageBox(this.Page, "No selected image.");
                }
            }
            else
            {
                Extension.MessageBox(this.Page, "Please select employee.");
            }

        }

        protected void btnDeleteFile_OnClick(object sender, EventArgs e)
        {
            string DocLibName = "Signature";

            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
            SignatureImage objSignatureImage = db.SignatureImages.First(x => x.EmployeeID == hdn_EmplyeeID.Value);
            if (objSignatureImage != null)
            {
                SharedRules sp = new SharedRules();
                sp.DeleteDocumentByURL(DocLibName, "", objSignatureImage.Path);

                db.SignatureImages.DeleteOnSubmit(objSignatureImage);
                db.SubmitChanges();

                panel_FileName.Visible = false;
                btn_Upload.OnClientClick = "";
            }



        }
    }
}
