using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMEdoc_CR
{
    public static class ExtensionMethod
    {
        public static bool IsNullOrEmpty(this DataRow[] drs)
        {
            if (drs == null) { return true; }
            if (drs.Length == 0) { return true; }
            return false;
        }
        public static bool IsNullOrEmpty(this DataTable dt)
        {
            if (dt == null) { return true; }
            if (dt.Rows.Count == 0) { return true; }
            return false;
        }
        public static DataTable ToTable(this DataRow[] drs)
        {
            if (drs == null) { return null; }
            if (drs.Length == 0) { return null; }

            DataTable dt = drs[0].Table.Copy();
            dt.Clear();
            foreach (DataRow dr in drs)
            {
                dt.ImportRow(dr);
            }
            return dt;
        }
        public static DataTable SelectToTable(this DataTable dt, string filter)
        {
            if (dt == null) { return null; }
            if (dt.Rows.Count == 0) { return null; }
            DataRow[] drs = dt.Select(filter);
            return drs.ToTable();
        }
        public static DataTable Sort(this DataTable dt, string Sort = "")
        {
            if (!dt.IsNullOrEmpty())
            {
                DataView dv = dt.Copy().DefaultView;
                dv.Sort = Sort;
                dt = dv.ToTable().Copy();
            }
            return dt;
        }
    }
}
