using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace PIMEdoc_CR.Default.ClearCacheData
{
    public partial class ClearCacheDataUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                System.Web.UI.Page page = System.Web.HttpContext.Current.Handler as System.Web.UI.Page;

                Button1.Enabled = page.Cache["TCB_PIM_EMP"] != null;

                if (Page.Request.QueryString["Hide"] != null)
                {
                    GridView1.Visible = false;
                }
                else
                {
                    GridView1.DataSource = (DataTable)page.Cache["TCB_PIM_EMP"];
                    GridView1.DataBind();
                    GridView2.DataSource = (DataTable)page.Cache["TCB_PIM_DEPT"];
                    GridView2.DataBind();
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Cache.Remove("TCB_PIM_EMP");
            Cache.Remove("TCB_PIM_DEPT");
            Button1.Enabled = false;
        }
    }
}
