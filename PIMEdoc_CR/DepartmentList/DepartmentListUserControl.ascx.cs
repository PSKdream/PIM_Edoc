using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using Microsoft.SharePoint;
using System.Linq;
using System.Collections.Generic;

namespace PIMEdoc_CR.DepartmentList
{
    public partial class DepartmentListUserControl : UserControl
    {
        private static string siteUrl = SPContext.Current.Web.Url;
        private SPSite oSPsite;
        public string ErrorMessage = string.Empty;
        public string SharedPointSite = SPContext.Current.Site.Url;
        public DataTable GetDataTable(string ListName, string Condition, string Sort)
        {
            try
            {
                string x = SPContext.Current.Site.Url;
                DataTable dt = new DataTable();
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (oSPsite = new SPSite(SharedPointSite))
                    {
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {
                            oSPWeb.AllowUnsafeUpdates = true;
                            SPList oList = oSPWeb.Lists[ListName];
                            SPQuery myquery = new SPQuery();
                            SPListItemCollection ListSPListItem = oList.GetItems();
                            dt = ListSPListItem.GetDataTable();


                            if (!dt.IsNullOrEmpty())
                            {
                                if (!string.IsNullOrEmpty(Condition)) { dt = dt.SelectToTable(Condition); }
                                if (!string.IsNullOrEmpty(Sort)) { dt = dt.Sort(Sort); }
                            }

                            oSPWeb.AllowUnsafeUpdates = false;
                        }
                    }
                });
                return dt;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }
        public SPListItemCollection GetCollectionList(string ListName)
        {
            try
            {               
                SPListItemCollection ListSPListItem = null;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (oSPsite = new SPSite(SharedPointSite))
                    {
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {
                            oSPWeb.AllowUnsafeUpdates = true;
                            SPList oList = oSPWeb.Lists[ListName];
                            SPQuery myquery = new SPQuery();
                            myquery.Query = "<OrderBy><FieldRef Name='OrderNo' Ascending='TRUE' /></OrderBy>";
                            ListSPListItem = oList.GetItems(myquery);
                            oSPWeb.AllowUnsafeUpdates = false;

                        }
                    }
                });
                return ListSPListItem;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    SPListItemCollection spColDepartment = GetCollectionList("Config_DepartmentList");
                    if (spColDepartment != null)
                    {
                        foreach (SPListItem dtr in spColDepartment)
                        {
                            if (dtr["Permission"] != null)
                            {
                                if (!string.IsNullOrEmpty(dtr["Permission"].ToString()))
                                {
                                    if (!IsExistUser(dtr, SPContext.Current.Web.CurrentUser.LoginName.ToLower())) continue;
                                }
                            }

                            System.Web.UI.HtmlControls.HtmlGenericControl createDiv =
    new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
                            createDiv.ID = System.Guid.NewGuid().ToString();
                            createDiv.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                            createDiv.Style.Add(HtmlTextWriterStyle.Height, "190px");
                            createDiv.Style.Add(HtmlTextWriterStyle.Display, "block");
                            createDiv.Attributes.Add("style", "float: left;");
                            createDiv.Attributes.Add("class", "itemDepartment");

                            System.Web.UI.HtmlControls.HtmlGenericControl createInnerDiv =
   new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
                            createInnerDiv.ID = System.Guid.NewGuid().ToString();
                            createInnerDiv.Attributes.Add("style", "border-top: 2px solid #078E3C;");
                            createInnerDiv.Style.Add(HtmlTextWriterStyle.Width, "85%");
                            createInnerDiv.Style.Add(HtmlTextWriterStyle.Height, "160px");
                            createInnerDiv.Style.Add(HtmlTextWriterStyle.TextAlign, "center");

                            Image img = new Image();
                            img.ID = System.Guid.NewGuid().ToString();
                            img.Height = 140;
                            img.Width = Unit.Percentage(100);
                            img.ImageUrl = dtr["ThumbNail_Img"] != null ? dtr["ThumbNail_Img"].ToString() : string.Empty;

                            HyperLink hpl = new HyperLink();
                            hpl.Text = dtr["Title"] != null ? dtr["Title"].ToString() : string.Empty;
                            hpl.NavigateUrl = dtr["TargetURL"] != null ? dtr["TargetURL"].ToString() : "#";
                            hpl.CssClass = "DepartmentLink";

                            createInnerDiv.Controls.Add(img);
                            createInnerDiv.Controls.Add(new LiteralControl("<br />"));
                            createInnerDiv.Controls.Add(hpl);
                            createInnerDiv.Controls.Add(new LiteralControl("<br />"));

                            createDiv.Controls.Add(createInnerDiv);
                            panelRow.Controls.Add(createDiv);

                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }
            }
        }

        public bool IsExistUser(SPListItem item, string sLowerUser)
        {
            try
            {
                //retrieve user value collection from the "AssignedTo" field and iterate
                SPFieldUserValueCollection usersFields = new SPFieldUserValueCollection(SPContext.Current.Web.Site.RootWeb, item["Permission"].ToString());
                foreach (SPFieldUserValue usersField in usersFields)
                {
                    if (usersField.User == null)
                    {
                        //UserField contains a SharePoint group -> extract users from it                   
                        SPGroup group = SPContext.Current.Web.Groups.GetByID(usersField.LookupId);
                        foreach (SPUser user in group.Users)
                        {
                            if (user.LoginName.ToLower() == sLowerUser)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (usersField.User.LoginName == sLowerUser) return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }            
            return false;
        }
    }
}
