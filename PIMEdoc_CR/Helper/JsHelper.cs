using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PIMEdoc_CR.Helper
{

    public static class JsHelper
    {
        public static void togglePopup(Page page, string popupID, bool toggleUp)
        {
            try
            {
                if (toggleUp)
                {
                    ScriptManager.RegisterStartupScript(page, typeof(Page), popupID, "$('#{" + popupID +"}').modal('show');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(page, typeof(Page), popupID, "$('#{" + popupID +"}').modal('hide');", true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
