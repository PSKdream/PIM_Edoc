using Microsoft.SharePoint;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using PIMEdoc_CR;


namespace PIMEdoc_CR.ControlTemplates.PIMEdoc_CR
{
    public partial class Footer : UserControl
    {
        //

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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {

                    
                    DataTable dtDataRelateApp = GetDataTable("Related_App", "IsActive = 1", "Created desc");
                    if (!dtDataRelateApp.IsNullOrEmpty())
                    {
                        dtDataRelateApp.Columns.Add("RelateName");
                        dtDataRelateApp.Columns.Add("LinkUrl");
                        foreach (DataRow item in dtDataRelateApp.Rows)
                        {
                            SPFieldUrlValue spUrl = new SPFieldUrlValue(item["Name"].ToString());
                            item["RelateName"] = spUrl.Description;
                            item["LinkUrl"] = spUrl.Url;
                        }
                        DataRow[] dr1 = dtDataRelateApp.Select("Group_No = '1'", "Sequence ASC");
                        DataRow[] dr2 = dtDataRelateApp.Select("Group_No = '2'", "Sequence ASC");
                        DataRow[] dr3 = dtDataRelateApp.Select("Group_No = '3'", "Sequence ASC");
                        DataRow[] dr4 = dtDataRelateApp.Select("Group_No = '4'", "Sequence ASC");

                        grvMenu1.DataSource = dr1.ToTable();
                        grvMenu1.DataBind();
                        grvMenu2.DataSource = dr2.ToTable();
                        grvMenu2.DataBind();
                        grvMenu3.DataSource = dr3.ToTable();
                        grvMenu3.DataBind();
                        grvMenu4.DataSource = dr4.ToTable();
                        grvMenu4.DataBind();

                    }



                    //SPListItemCollection spCol = sp.getListLibrary("", "AboutUs", "<Where><Eq><FieldRef Name='IsActive' /><Value Type='1'></Value></Eq></Where><OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy>");
                    DataTable dtData = GetDataTable("Contact_Us_Info", "", "Created desc"); //IsActive = 1

                    if (!dtData.IsNullOrEmpty())
                    {
                        lblAddress.Text = dtData.Rows[0]["FullAddress"] != null ? dtData.Rows[0]["FullAddress"].ToString() : "";
                    }

                    //FullAddress
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
