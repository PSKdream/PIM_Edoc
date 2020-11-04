
using Microsoft.SharePoint;
using Newtonsoft.Json;
using PIMEdoc_CR.Rule;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PIMEdoc_CR.Default.Rule
{

    public static class Extension
    {
        #region | Status |
        public const string _NewRequest = "New Request";
        public const string _Draft = "Draft";
        public const string _Rework = "Rework";
        public const string _WaitForRequestorReview = "Wait for Requestor Review";
        public const string _WaitForApprove = "Wait for Approve";
        public const string _WaitForComment = "Wait for Comment";
        public const string _WaitForAdminCentre = "Wait for Admin Centre";
        public const string _Completed = "Completed";
        public const string _Cancelled = "Cancelled";
        public const string _Rejected = "Rejected";
        public const string _RequestCancel = "Request Cancel";
        #endregion

        public static string _ConnectionString;
        public const string _PIMWebUser = "edocument";
        public const string _PIMWebPass = "techconbiz";

        public static bool _IsTestEmail = false;
        public static readonly System.Globalization.CultureInfo _ctli = new System.Globalization.CultureInfo("th-TH");
        public static readonly System.Globalization.CultureInfo _ctliTH = new System.Globalization.CultureInfo("th-TH");
        public static readonly System.Globalization.CultureInfo _ctliEN = new System.Globalization.CultureInfo("en-GB");
        private static Microsoft.Office.Interop.Word.Application wordApp;

        public static string GetSPSite()
        {
            return SPContext.Current.Site.Url;
        }

        public static bool DataTableIsNullOrEmpty(this DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                return false;
            }
            return true;
        }

        public static string GenerateDocumentNo(string currentNo, int sDepID, string Category, string DocType, Page page)
        {
            try
            {
                string runningNo = "";
                //generate RunningNo
                string projectYear = (DateTime.Now.Year).ToString();
                projectYear = DocType == "ExEN"
                    ? projectYear.ConvertToAD()
                    : projectYear.ConvertToBE();

                string sCategory = "", sDocType = "", sDepCode = "";
                string sDepName = "";
                DataTable dtDept = GetDepartmentData(page).Copy();
                if (dtDept.Rows.Count > 0 && sDepID > 0)
                {
                    DataTable sDeptResult = dtDept.AsEnumerable().Where(r => r.Field<String>("DEPARTMENT_ID").Equals(sDepID.ToString())).ToList().CopyToDataTable();
                    if (!sDeptResult.DataTableIsNullOrEmpty())
                    {
                        sDepName = dtDept.Rows[0]["Department_Name_TH"].ToString();
                        sDepCode = dtDept.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString();
                        if (DocType == "ExEN")
                        {
                            sDepName = dtDept.Rows[0]["Department_Name_EN"].ToString();
                            sDepCode = dtDept.Rows[0]["DEPARTMENT_ACRONYM_EN"].ToString();
                        }
                    }
                }
                DataTable dtCategory = SharedRules.GetList("MstCategory",
                    string.Format(@"<Where><Eq>
				<FieldRef Name='Value' /><Value Type='Text'>{0}</Value></Eq></Where>", Category));
                if (dtCategory.Rows.Count > 0)
                {
                    sCategory = dtCategory.Rows[0]["CategoryName"].ToString();
                }
                DataTable dtDocumentType = SharedRules.GetList("MstDocumentType",
                    string.Format(@"<Where><Eq>
				<FieldRef Name='Value' /><Value Type='Text'>{0}</Value></Eq></Where>", DocType));
                if (dtDocumentType.Rows.Count > 0)
                {
                    sDocType = dtDocumentType.Rows[0]["DocTypeName"].ToString();
                }
                string institudeName = "สถาบันการจัดการปัญญาภิวัฒน์";

                string newRunningNo = "0001";
                if (!string.IsNullOrWhiteSpace(currentNo))
                {
                    newRunningNo = currentNo.Split('/')[0];
                }

                #region | Running No By Doctype |
                runningNo = GetDocumentNo(Category, DocType, sDocType, sDepName, sDepCode, institudeName, newRunningNo, projectYear);

                #endregion

                return runningNo;

            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            return "";
        }
        public static string GetDocumentNo(string Category, string DocType, string sDocHead, string sDepName, string sDepCode, string institudeName, string newRunningNo, string projectYear)
        {
            #region "Running No By Doctype"
            string runningNo = "";
            if (DocType == "P" || DocType == "C")
            {
                //ประกาศ คำสั่ง
                runningNo = string.Format("{0}{1}{2} {3} {4}/{5}",
                    sDocHead
                    , Category == "centre" ? "" : sDepName + " "
                    , institudeName, "ที่", newRunningNo, projectYear);
            }
            else if (DocType == "Ex")
            {
                //จดหมายออก ไทย
                runningNo = string.Format("{0} {1} {2}{3}/{4}", "ที่", "สจป."
                    , Category == "centre" ? "" : sDepCode
                    , newRunningNo, projectYear);
            }
            else if (DocType == "ExEN")
            {
                //จดหมายออก Eng
                runningNo = string.Format("{0} {1} {2}{3}/{4}", "NO.", "PIM."
                    , Category == "centre" ? "" : sDepCode + " "
                    , newRunningNo, projectYear);
            }
            else if (DocType == "R" || DocType == "L")
            {
                //ระเบียบ ข้อบังคับ 
                runningNo = "";
            }
            else if (DocType == "M")
            {
                runningNo = string.Format("{0}/{1}", newRunningNo, projectYear);
            }
            else if (DocType == "M")
            {
                //บันทึกข้อความ
                if (Category == "centre")
                {
                    runningNo = "";
                }
                else
                {
                    runningNo = string.Format("{0}{1} {2}/{3}"
                        , "สบว."
                        , sDepCode + "."
                        , newRunningNo, projectYear);
                }
            }
            else if (DocType == "Other")
            {
                //อื่นๆ
                if (Category == "centre")
                {
                    runningNo = "";
                }
                else
                {
                    runningNo = string.Format("{0} {2}/{3}"
                        , sDepCode
                        , sDepCode
                        , newRunningNo, projectYear);
                }
            }
            else
            {

            }
            return runningNo;
            #endregion
        }

        public static DataTable GetDataTable(string ListName)
        {
            try
            {
                string spUrl = SPContext.Current.Site.Url;
                DataTable dt = new DataTable();
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite oSPsite = new SPSite(spUrl))
                    {
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {
                            oSPWeb.AllowUnsafeUpdates = true;
                            SPList oList = oSPWeb.Lists[ListName];
                            SPQuery myquery = new SPQuery();
                            SPListItemCollection ListSPListItem = oList.GetItems();
                            dt = ListSPListItem.GetDataTable();
                            oSPWeb.AllowUnsafeUpdates = false;
                        }
                    }
                });
                return dt;
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public static string getValueFromTable(string table, string key)
        {
            string result = "";
            try
            {

                DataTable dtSetting = GetDataTable(table);

                if (!dtSetting.DataTableIsNullOrEmpty())
                {
                    DataTable oResult = dtSetting.AsEnumerable()
                        .Where(r => r.Field<String>("Key").Equals(key)).ToList().CopyToDataTable();
                    result = oResult.Rows[0]["Value"].ToString();
                }

            }
            catch (Exception ex)
            {
                LogWriter.Write(ex);
            }
            return result;
        }
        public static string GetDBConnectionString()
        {
            DataTable dtDb = GetDataTable("SiteSetting");
            {
                if (!dtDb.DataTableIsNullOrEmpty())
                {
                    return dtDb.Rows[0]["Value"].ToString();
                }
            }
            return "";
        }

        #region | Get Data From WebService |
        public static string GetJsonString(string path)
        {
            string value = "";
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                Uri url = new Uri(path);
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFile file = web.GetFile(url.AbsoluteUri);
                        StreamReader reader = new StreamReader(file.OpenBinaryStream());
                        value = reader.ReadToEnd();
                        reader.Close();
                    }
                }
            });
            return value;
        }
        public static DataTable GetEmployeeData(Page objPage)
        {
            DataTable dtResult;
            if (objPage.Cache["TCB_PIM_EMP"] != null)
            {
                dtResult = (DataTable)objPage.Cache["TCB_PIM_EMP"];
            }
            else
            {
                //Connect PIM Webservices
                string json = new PIMWS.GetPosition().GetAllStaffProfile(_PIMWebUser, _PIMWebPass, "");

                //string json = "";

                //dtResult = JsonStringToDataTable(a);
                dtResult = DeserializeEmployee(json, "RESULT_POSITION");
                dtResult.Columns.Add("Sequence");
                dtResult.Columns.Add("EmployeeName_TH");
                dtResult.Columns.Add("EmployeeName_EN");
                int counter = 1;
                foreach (DataRow row in dtResult.Rows)
                {
                    //row["Sequence"] = counter++;
                    row["EmployeeName_TH"] = string.Format("{0}{1} {2}", row["PREFIX_TH"], row["FIRSTNAME_TH"], row["LASTNAME_TH"]);
                    row["EmployeeName_EN"] = string.Format("{0}{1} {2}", row["PREFIX_EN"], row["FIRSTNAME_EN"], row["LASTNAME_EN"]);
                }
                objPage.Cache.Insert("TCB_PIM_EMP", dtResult.Copy(), null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return dtResult.Copy();
        }
        public static DataTable GetDepartmentData(Page objPage)
        {
            DataTable dtResult;
            if (objPage.Cache["TCB_PIM_DEPT"] != null)
            {
                dtResult = (DataTable)objPage.Cache["TCB_PIM_DEPT"];
            }
            else
            {
                //Connect PIM Webservices
                string a = new PIMWS.GetPosition().GetDepartment(_PIMWebUser, _PIMWebPass, "");
                //a = GetJsonString(_DepartmentPath);
                //string a = "";
                //string a = File.ReadAllText(_DepartmentPath);
                dtResult = JsonStringToDataTable(a);
                objPage.Cache.Insert("TCB_PIM_DEPT", dtResult.Copy(), null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return dtResult.Copy();
        }
        public static DataTable GetPositionData(Page objPage)
        {
            DataTable dtResult;
            if (objPage.Cache["TCB_PIM_POST"] != null)
            {
                dtResult = (DataTable)objPage.Cache["TCB_PIM_POST"];
            }
            else
            {
                //Connect PIM Webservices
                string a = new PIMWS.GetPosition().CallGetPosition(_PIMWebUser, _PIMWebPass, "");
                //a = GetJsonString(_PositionPath);
                //string a = "";
                //string a = File.ReadAllText(_DepartmentPath);
                dtResult = JsonStringToDataTable(a);
                objPage.Cache.Insert("TCB_PIM_POST", dtResult.Copy(), null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return dtResult.Copy();
        }

        public static DataTable GetSpecificDepartmentData(Page objPage, string deptID)
        {
            DataTable dtResult;
            {
                //Connect PIM Webservices
                string a = new PIMWS.GetPosition().GetDepartment(_PIMWebUser, _PIMWebPass, deptID);
                dtResult = JsonStringToDataTable(a);
            }
            return dtResult.Copy();
        }
        public static DataTable GetSpecificDepartmentDataFromTemp(Page objPage, string deptID)
        {
            DataTable dtDept = GetDepartmentData(objPage).Copy();
            try
            {
                if (!dtDept.DataTableIsNullOrEmpty())
                {
                    dtDept = dtDept.AsEnumerable().Where(r => r.Field<String>("DEPARTMENT_ID").Equals(deptID)).CopyToDataTable();
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write(ex);
            }
            return dtDept.Copy();
        }
        public static SpecificEmployeeData.RootObject GetSpecificEmployeeFromTemp(Page objPage, string empID)
        {
            DataTable dtEmp = GetEmployeeData(objPage).Copy();
            SpecificEmployeeData.RootObject empData = new SpecificEmployeeData.RootObject();
            empData.RESULT = new List<SpecificEmployeeData.RESULT>();
            if (dtEmp != null)
            {
                DataView dvFilter = dtEmp.DefaultView;
                if (dvFilter != null)
                {
                    dvFilter.RowFilter = string.Format(@"EmployeeID = '{0}'", empID.ToString());
                    if (dvFilter.ToTable().Rows.Count > 0)
                    {
                        foreach (DataRow dr in dvFilter.ToTable().Rows)
                        {
                            empData.EMPLOYEEID = dr["EMPLOYEEID"].ToString();
                            empData.USERNAME = dr["USERNAME"].ToString();
                            empData.PREFIX_TH = dr["PREFIX_TH"].ToString();
                            empData.PREFIX_ACADEMIC_TH = dr["PREFIX_ACADEMIC_TH"].ToString();
                            empData.FIRSTNAME_TH = dr["FIRSTNAME_TH"].ToString();
                            empData.LASTNAME_TH = dr["LASTNAME_TH"].ToString();
                            empData.PREFIX_EN = dr["PREFIX_EN"].ToString();
                            empData.PREFIX_ACADEMIC_EN = dr["PREFIX_ACADEMIC_EN"].ToString();
                            empData.FIRSTNAME_EN = dr["FIRSTNAME_EN"].ToString();
                            empData.LASTNAME_EN = dr["LASTNAME_EN"].ToString();
                            empData.BIRTHDATE = dr["BIRTHDATE"].ToString();
                            empData.MANAGERID = dr["MANAGERID"].ToString();
                            empData.MANAGERUSERNAME = dr["MANAGERUSERNAME"].ToString();
                            empData.EMAIL = dr["EMAIL"].ToString();
                            empData.TELEPHONE = dr["TELEPHONE"].ToString();
                            empData.MOBILE = dr["MOBILE"].ToString();
                            empData.MODIFIED_BY = dr["MODIFIED_BY"].ToString();
                            empData.MODIFIED_DATETIME = dr["MODIFIED_DATETIME"].ToString();

                            SpecificEmployeeData.RESULT listDept = new SpecificEmployeeData.RESULT();
                            listDept.DEPARTMENT_ID = dr["DEPARTMENT_ID"].ToString();
                            listDept.DEPARTMENT_NAME_TH = dr["DEPARTMENT_NAME_TH"].ToString();
                            listDept.DEPARTMENT_NAME_EN = dr["DEPARTMENT_NAME_EN"].ToString();
                            listDept.SUBDEPARTMENT_ID = dr["SUBDEPARTMENT_ID"].ToString();
                            listDept.SUBDEPARTMENT_NAME_TH = dr["SUBDEPARTMENT_NAME_TH"].ToString();
                            listDept.SUBDEPARTMENT_NAME_EN = dr["SUBDEPARTMENT_NAME_EN"].ToString();
                            listDept.POSITION_TD = dr["POSITION_TD"].ToString();
                            listDept.POSTION_NAME_TH = dr["POSTION_NAME_TH"].ToString();
                            listDept.POSTION_NAME_EN = dr["POSTION_NAME_EN"].ToString();
                            listDept.PRIMARY_POSITION = dr["PRIMARY_POSITION"].ToString();

                            empData.RESULT.Add(listDept);
                        }
                    }
                }

            }
            return empData;
        }
        public static SpecificEmployeeData.RootObject GetSpecificEmployeeData(Page objPage, string empID)
        {
            string json = new PIMWS.GetPosition().GetStaffProfile(_PIMWebUser, _PIMWebPass, empID);
            //string json = "";
            //string json = File.ReadAllText(_StaffPath);
            SpecificEmployeeData.RootObject empData = JsonConvert.DeserializeObject<SpecificEmployeeData.RootObject>(json);

            return empData;
        }
        public static DataTable GetSpecificPositionDataFromTemp(Page page, string posID)
        {
            DataTable dtDept = GetPositionData(page).Copy();
            try
            {
                if (!dtDept.DataTableIsNullOrEmpty())
                {
                    dtDept = dtDept.AsEnumerable().Where(r => r.Field<String>("POSITION_ID").Equals(posID)).CopyToDataTable();
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write(ex);
            }
            return dtDept.Copy();
        }

        private static DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            if (ColumnsNameString.Equals("RESULT") || ColumnsNameString.Equals("RESULT_POSITION"))
                            {
                                ColumnsNameString = ColumnsNameData.Substring(idx + 1).Replace("\"", "");
                                ColumnsNameString = ColumnsNameString.Substring(0, ColumnsNameString.IndexOf(":")).Replace("\"", "");
                            }
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        if (RowColumns.Equals("RESULT") || RowColumns.Equals("RESULT_POSITION"))
                        {
                            RowColumns = rowData.Substring(idx + 1).Replace("\"", "");
                            RowColumns = RowColumns.Substring(0, RowColumns.IndexOf(":")).Replace("\"", "");

                            RowDataString = RowDataString.Substring(RowDataString.IndexOf(":") + 1).Replace("\"", "");
                        }
                        RowDataString = System.Text.RegularExpressions.Regex.Unescape(RowDataString);

                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }
        private static DataTable DeserializeEmployee(string json, string innerProp)
        {
            EmployeeData.RootObject empData = JsonConvert.DeserializeObject<EmployeeData.RootObject>(json);

            return CreateNestedDataTable<EmployeeData.RESULT, EmployeeData.RESULT_POSITION>(empData.RESULT.Where(x => !string.IsNullOrWhiteSpace(x.EMPLOYEEID)).GroupBy(x => new { x.EMPLOYEEID }).Select(x => x.First()).OrderBy(x => x.EMPLOYEEID), innerProp);

        }
        #endregion

        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (DataRow row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            if (propertyInfo != null)
                            {
                                Type t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                                object safeValue = (row[prop.Name] == null) ? null : Convert.ChangeType(row[prop.Name], t);

                                propertyInfo.SetValue(obj, safeValue, null);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }
        public static DataTable ListToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
        public static DataTable CreateNestedDataTable<TOuter, TInner>(IEnumerable<TOuter> list, string innerListPropertyName)
        {
            PropertyInfo[] outerProperties = typeof(TOuter).GetProperties().Where(pi => pi.Name != innerListPropertyName).ToArray();
            PropertyInfo[] innerProperties = typeof(TInner).GetProperties();
            MethodInfo innerListGetter = typeof(TOuter).GetProperty(innerListPropertyName).GetGetMethod(true);

            // set up columns
            DataTable table = new DataTable();
            foreach (PropertyInfo pi in outerProperties)
                table.Columns.Add(pi.Name, Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType);
            foreach (PropertyInfo pi in innerProperties)
                table.Columns.Add(pi.Name, Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType);

            // iterate through outer items
            foreach (TOuter outerItem in list)
            {
                IEnumerable<TInner> innerList = innerListGetter.Invoke(outerItem, null) as IEnumerable<TInner>;
                if (innerList == null || innerList.Count() == 0)
                {
                    // outer item has no inner items
                    DataRow row = table.NewRow();
                    foreach (PropertyInfo pi in outerProperties)
                        row[pi.Name] = pi.GetValue(outerItem, null) ?? DBNull.Value;
                    table.Rows.Add(row);
                }
                else
                {
                    // iterate through inner items
                    foreach (object innerItem in innerList)
                    {
                        DataRow row = table.NewRow();
                        foreach (PropertyInfo pi in outerProperties)
                            row[pi.Name] = pi.GetValue(outerItem, null) ?? DBNull.Value;
                        foreach (PropertyInfo pi in innerProperties)
                            row[pi.Name] = pi.GetValue(innerItem, null) ?? DBNull.Value;
                        table.Rows.Add(row);
                    }
                }
            }

            return table;
        }


        public static bool IsContain(string word, string key, char splitBy)
        {
            return word.Split(splitBy).Any(item => item.ToLower().Trim().Equals(key.ToLower().Trim()));
        }

        public static DataTable GetDocTypeByCategory(string type, bool isAdminCentre = false)
        {
            DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
            if (!dtDocType.DataTableIsNullOrEmpty())
            {
                dtDocType = dtDocType.AsEnumerable()
                    .Where(r => r.Field<String>(type).Equals("1")
                        && r.Field<Double>("Level") == 0
                        && r.Field<String>("IsActive").Equals("1")
                        && (isAdminCentre ? true : r.Field<String>("isAdminCentre").Equals("0"))).ToList().CopyToDataTable();
            }

            return dtDocType;
        }

        public static string SetDecimalFormat(string dectext)
        {
            if (dectext == "" || dectext == "0" || dectext == "0.00" || dectext == ".00")
            {
                return "0.00";
            }
            else
            {
                Decimal dcm;
                if (Decimal.TryParse(dectext, out dcm))
                {
                    return dcm.ToString("#,###.00");
                }
                return "0.00";
            }
        }
        /// <summary>
        /// Validate Approval DOA
        /// </summary>
        /// <param name="sAmount"></param>
        /// <param name="sPosID"></param>
        /// <returns></returns>
        public static bool isApprovalDOALvSupport(string sAmount, string sPosID)
        {
            if (sAmount == "" || sAmount == "0" || sAmount == "0.00" || sAmount == ".00")
            {
                //Amount = 0
                return true;
            }
            else
            {
                Decimal dcm;
                if (Decimal.TryParse(sAmount, out dcm))
                {
                    //Get DOAList
                    DataTable dtAmount = Extension.GetDataTable("MstMatrix");
                    List<int> listPositionGroupID = new List<int>();
                    #region | Filter Position Group By Amount |
                    foreach (DataRow row in dtAmount.Rows)
                    {
                        Decimal tblDcm;
                        if (Decimal.TryParse(row["Amount"].ToString(), out tblDcm))
                        {
                            if (tblDcm > dcm)
                            {
                                int posGroupID = Convert.ToInt32(row["PositionGroup_x003a_PositionGrou"].ToString());
                                listPositionGroupID.Add(posGroupID);
                            }
                        }
                    }
                    #endregion

                    DataTable dtPosition = Extension.GetDataTable("MstPosition");
                    List<int> listPosition = new List<int>();
                    #region | Filter Position By PositionGroup |
                    foreach (DataRow row in dtPosition.Rows)
                    {
                        int posID;
                        int posGroupID;
                        if (Int32.TryParse(row["PositionID"].ToString(), out posID) && Int32.TryParse(row["PositionGroupName_x003a_Position"].ToString(), out posGroupID))
                        {
                            if (listPositionGroupID.Contains(posGroupID))
                            {
                                listPosition.Add(posID);
                            }
                        }
                    }
                    #endregion

                    int empPosID;
                    if (Int32.TryParse(sPosID, out empPosID))
                    {
                        if (listPosition.Contains(empPosID))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Search Text in gridview after Binding Data.
        /// *** Not Support BoundField ***
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="searchText"></param>
        public static void SearchGridview(GridView gv, string searchText, Page page)
        {
            try
            {
                foreach (GridViewRow row in gv.Rows)
                {
                    bool isContain = false;
                    ControlCollection controls = row.Controls;
                    foreach (Control control in controls)
                    {
                        string text = "";
                        if (control.Controls.Count > 2)
                        {
                            if (control.Controls[1] is Label)
                            {
                                text = ((Label)row.FindControl(control.Controls[1].ID)).Text;

                            }
                            else if (control.Controls[1] is HyperLink)
                            {
                                text = ((HyperLink)row.FindControl(control.Controls[1].ID)).Text;
                            }

                            if (text.Contains(searchText))
                            {
                                isContain = true;
                            }
                        }
                    }
                    if (!isContain)
                    {
                        row.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.MessageBox(page, ex.Message.Replace('\'', ' '));
            }
        }
        public static string GetTextInGridViewRow(GridViewRow row, ControlCollection controls, int controlIdx)
        {
            string text = "";
            try
            {
                if (controlIdx <= controls.Count)
                {
                    if (controls[controlIdx].Controls.Count > 2)
                    {
                        if (controls[controlIdx].Controls[1] is Label)
                        {
                            text = ((Label)row.FindControl(controls[controlIdx].Controls[1].ID)).Text;

                        }
                        else if (controls[controlIdx].Controls[1] is HyperLink)
                        {
                            text = ((HyperLink)row.FindControl(controls[controlIdx].Controls[1].ID)).Text;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write(ex);
            }
            return text;
        }

        //Button Setup
        public static Button SetUpActionButton(this Button btn, string text, string commandName, bool isVisible, string cssClass, string onClientConfirm)
        {
            btn.Text = text;
            btn.CommandName = commandName;
            btn.Visible = isVisible;
            btn.CssClass = cssClass;
            btn.OnClientClick = string.Format("return confirm('Action: {0}');", btn.Text);
            if (!string.IsNullOrEmpty(onClientConfirm))
            {
                btn.OnClientClick = onClientConfirm;
            }
            return btn;
        }

        public static string ReplaeInvalidFileName(this string input)
        {
            //Remove Invalid FileName
            List<string> str = new List<string>(new string[] { "[", "~", "#", "%", "&", "*", "{", "}", "\\", ":", "<", ">", "?", "/", "+", "|", "]", " " });

            input = input.Replace("/", "_");
            foreach (string item in str)
            {
                input = input.Replace(item, "-");
            }
            return input;
        }
        public static string SetDateTimeFormat(this DateTime datetime)
        {
            return string.Format("{0}/{1}/{2}", datetime.Day.ToString("D2"), datetime.Month.ToString("D2"), datetime.Year < 2500 ? datetime.Year + 543 : datetime.Year);
        }
        //MessageBox
        public static void MessageBox(Page page, string Message)
        {
            MessageBox(page, Message, string.Empty);
        }
        public static void MessageBox(Page page, string Message, string Url)
        {
            if (Message.Contains("locked for shared"))
            {
                string user = Message.Split(' ').Last();
                if (!string.IsNullOrWhiteSpace(user))
                {
                    if (user.Contains("\\"))
                    {
                        user = ToUpperFirstLetter(user.Split('\\')[1]);
                    }
                    Message = string.Format("The document is editing by {0}, Please close all editing documents. (กรุณาปิดไฟล์ Word)", user.Trim().Replace(".", ""));
                }
                else
                {
                    Message = "This Document is in editing mode, Please close all editing documents. (กรุณาปิดไฟล์ Word)";
                }
            }
            string sScript = string.Format("alert('{0}');", Message.Replace('\'', ' ').Replace("\r", "").Replace("\n", "")); //"alert('" + Message.Replace('\'', ' ') + "');";
            if (!string.IsNullOrEmpty(Url))
            {
                sScript = string.Format("{0} window.parent.location = '{1}';", sScript, Url);// "window.parent.location = '" + Url + "'; ";
            }
            ScriptManager.RegisterStartupScript(page, typeof(Page), "AlertMessage", sScript, true);
        }
        public static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }

        //Email
        public static void SendEmailTemplate(string sNextStatus, string sToEmpID, string sToDepID, string sAction, string sCCEmail, string sGroupMail, string sMemoID, TRNDocument objDocument, Page page, string currentUserID, bool testMail = false)
        {
            try
            {
                DataTable dtEmailTemplate = Extension.GetDataTable("MstEmail");
                if (dtEmailTemplate != null)
                {
                    if (dtEmailTemplate.Rows.Count > 0)
                    {
                        dtEmailTemplate = (from DataRow dr in dtEmailTemplate.Rows
                                           where (string)dr["Situation"] == sNextStatus
                                           select dr).ToList().CopyToDataTable();
                    }
                }

                if (dtEmailTemplate != null)
                {
                    if (dtEmailTemplate.Rows.Count > 0)
                    {
                        DataClassesDataAccessDataContext dataContext = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

                        //Variable 
                        bool __IsTestEmail = (getValueFromTable("SiteSetting", "TestEmail").ToLower().Equals("true") || testMail);

                        string sPDFLanguage = "TH";
                        string sSubject = dtEmailTemplate.Rows[0][string.Format("Subject{0}", sPDFLanguage)].ToString();
                        string sBody = dtEmailTemplate.Rows[0][string.Format("Detail{0}", sPDFLanguage)].ToString(); string sActionDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm", new System.Globalization.CultureInfo("th-TH"));
                        string sActorName = "";
                        string sAttachFilePath = "";
                        string sDestinationUserName = "";
                        string sDestinationEmail = "";
                        string sRequestNo = "";
                        string sRequestDate = "";
                        string sSubjectMemo = "";
                        SpecificEmployeeData.RootObject requestor = null;
                        string sReqester = "";
                        string sDocumentType = "";
                        string sDocumentNo = "";
                        string sCategory = "";
                        string sStatus = "";
                        string sComment = "";

                        if (objDocument != null)
                        {
                            sRequestNo = objDocument.DocNo;
                            sRequestDate = ((DateTime)objDocument.CreatedDate).ToString("dd/MM/yyyy HH:mm", new System.Globalization.CultureInfo("th-TH"));
                            sSubjectMemo = objDocument.Title;
                            requestor = GetSpecificEmployeeFromTemp(page, objDocument.RequestorID.ToString());
                            sReqester = string.Format("{0}{1} {2}", requestor.PREFIX_TH, requestor.FIRSTNAME_TH, requestor.LASTNAME_TH);
                            sDocumentType = objDocument.DocTypeCode;
                            sDocumentNo = objDocument.DocNo;
                            sCategory = objDocument.Category;
                            sStatus = objDocument.Status;
                            sComment = objDocument.Comment;
                            if (!string.IsNullOrWhiteSpace(objDocument.AttachFilePath))
                            {
                                sAttachFilePath = objDocument.AttachFilePath;
                            }
                            else
                            {
                                sAttachFilePath = string.Format("{0}/{1}/{2}/{3}", GetSPSite(), objDocument.DocLib, objDocument.DocSet, "fullPreviewPDF.pdf");
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(sDocumentType))
                        {
                            DataTable dtDocType = GetDataTable("MSTDocumentType");
                            foreach (DataRow dr in dtDocType.Rows)
                            {
                                if (dr["Value"].ToString().Equals(sDocumentType))
                                {
                                    sDocumentType = dr["DocTypeName"].ToString();
                                    break;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(sCategory))
                        {
                            DataTable dtCategory = GetDataTable("MSTCategory");
                            foreach (DataRow dr in dtCategory.Rows)
                            {
                                if (dr["Value"].ToString().Equals(sCategory))
                                {
                                    sCategory = dr["CategoryName"].ToString();
                                    break;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(sGroupMail))
                        {
                            SpecificEmployeeData.RootObject emp = Extension.GetSpecificEmployeeFromTemp(page, sToEmpID);
                            SpecificEmployeeData.RootObject current_user = Extension.GetSpecificEmployeeFromTemp(page, currentUserID);
                            sDestinationUserName = string.Format("{0} {1}", emp.FIRSTNAME_EN, emp.LASTNAME_EN);
                            sDestinationEmail = emp.EMAIL;
                            sActorName = string.Format("{0}{1} {2}", current_user.PREFIX_TH, current_user.FIRSTNAME_TH, current_user.LASTNAME_TH);

                            List<Secretary> listSecretary = dataContext.Secretaries.Where(x => x.EmpID == sToEmpID).ToList();
                            if (!string.IsNullOrWhiteSpace(sToDepID))
                            {
                                listSecretary = listSecretary.Where(x => x.DepID == sToDepID).ToList();
                            }
                            foreach (Secretary item in listSecretary)
                            {
                                if (!__IsTestEmail)
                                {
                                    string secretaryID = item.SecretaryID;
                                    sCCEmail += Extension.GetSpecificEmployeeFromTemp(page, secretaryID).EMAIL.ToString() + ";";
                                }
                                else
                                {
                                    sCCEmail += string.Format("{0}@mail.com;", item.SecretaryID);
                                }
                            }
                        }
                        else
                        {
                            sDestinationEmail = sGroupMail;
                        }
                        if (__IsTestEmail)
                        {
                            sDestinationEmail = "atcharee@techconsbiz.com";
                            sCCEmail += "atcharee.atch@gmail.com";
                        }

                        sSubject = sSubject.Replace("[Request No.]", sRequestNo).Replace("[Subject Memo]", sSubjectMemo);
                        sBody = sBody.Replace("[Request No.]", sRequestNo)
                            .Replace("[Destination User]", sDestinationUserName)
                            .Replace("[Category]", sCategory)
                            .Replace("[Document No.]", sDocumentNo)
                            .Replace("[Document Type]", sDocumentType)
                            .Replace("[Subject Memo]", sSubjectMemo)
                            .Replace("[Request Name]", sReqester)
                            .Replace("[Request Date]", sRequestDate)
                            .Replace("[ActorName]", sActorName)
                            .Replace("[Action]", sAction)
                            .Replace("[ActionDate]", sActionDate)
                            .Replace("[Status]", sStatus)
                            .Replace("[Comment]", sComment)
                            .Replace("[Link PDF]", string.Format("<a href='{0}'>Click</a>", sAttachFilePath))
                            .Replace("[Link URL]", string.Format("<a href='{0}/SitePages/e-Document.aspx?pk={1}'>Click</a>", Extension.GetSPSite(), sMemoID));
                        SendEmail(sDestinationEmail, sCCEmail, sSubject, sBody);
                    }
                    else
                    {
                        throw new Exception("Not Found Template ID");
                    }

                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }

        }
        #region |back up|
        //public static void SendEmailTemplate(string sNextStatus, string sToEmpID, string sToDepID, string sAction, string sCCEmail, string sGroupMail, string sMemoID, TRNDocument objDocument, Page page, string currentUserID)
        //{
        //    try
        //    {
        //        //DataTable dtEmailTemplate = SharedRules.GetList("MstEmail", string.Format("<View><Query><Where><Eq><FieldRef Name='Situation'/><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>", sNextStatus));
        //        DataTable dtEmailTemplate = Extension.GetDataTable("MstEmail");
        //        if (!dtEmailTemplate.DataTableIsNullOrEmpty())
        //        {
        //            dtEmailTemplate = dtEmailTemplate.AsEnumerable().Where(r => r.Field<String>("Situation").Equals(sNextStatus)).ToList().CopyToDataTable();

        //        }

        //        if (dtEmailTemplate != null)
        //        {
        //            if (dtEmailTemplate.Rows.Count > 0)
        //            {
        //                DataClassesDataAccessDataContext dataContext = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());

        //                //Variable 
        //                bool __IsTestEmail = getValueFromTable("SiteSetting", "TestEmail").ToLower().Equals("true");

        //                string sPDFLanguage = "TH";
        //                string sSubject = dtEmailTemplate.Rows[0][string.Format("Subject{0}", sPDFLanguage)].ToString();
        //                string sBody = dtEmailTemplate.Rows[0][string.Format("Detail{0}", sPDFLanguage)].ToString(); string sActionDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm", new System.Globalization.CultureInfo("th-TH"));
        //                string sActorName = "";
        //                string sAttachFilePath = "";
        //                string sDestinationUserName = "";
        //                string sDestinationEmail = "";
        //                string sRequestNo = "";
        //                string sRequestDate = "";
        //                string sSubjectMemo = "";
        //                SpecificEmployeeData.RootObject requestor = null;
        //                string sReqester = "";
        //                string sDocumentType = "";
        //                string sDocumentNo = "";
        //                string sCategory = "";
        //                string sStatus = "";
        //                string sComment = "";

        //                if (objDocument != null)
        //                {
        //                    sRequestNo = objDocument.DocNo;
        //                    sRequestDate = ((DateTime)objDocument.CreatedDate).ToString("dd/MM/yyyy HH:mm", new System.Globalization.CultureInfo("th-TH"));
        //                    sSubjectMemo = objDocument.Title;
        //                    requestor = GetSpecificEmployeeFromTemp(page, objDocument.RequestorID.ToString());
        //                    sReqester = string.Format("{0}{1} {2}", requestor.PREFIX_TH, requestor.FIRSTNAME_TH, requestor.LASTNAME_TH);
        //                    sDocumentType = objDocument.DocTypeCode;
        //                    sDocumentNo = objDocument.DocNo;
        //                    sCategory = objDocument.Category;
        //                    sStatus = objDocument.Status;
        //                    sComment = objDocument.Comment;
        //                    if (!string.IsNullOrWhiteSpace(objDocument.AttachFilePath))
        //                    {
        //                        sAttachFilePath = objDocument.AttachFilePath;
        //                    }
        //                    else
        //                    {
        //                        sAttachFilePath = string.Format("{0}/{1}/{2}/{3}", GetSPSite(), objDocument.DocLib, objDocument.DocSet, "fullPreviewPDF.pdf");
        //                    }
        //                }

        //                if (!string.IsNullOrWhiteSpace(sDocumentType))
        //                {
        //                    DataTable dtDocType = GetDataTable("MSTDocumentType");
        //                    foreach (DataRow dr in dtDocType.Rows)
        //                    {
        //                        if (dr["Value"].ToString().Equals(sDocumentType))
        //                        {
        //                            sDocumentType = dr["DocTypeName"].ToString();
        //                            break;
        //                        }
        //                    }
        //                }
        //                if (!string.IsNullOrWhiteSpace(sCategory))
        //                {
        //                    DataTable dtCategory = GetDataTable("MSTCategory");
        //                    foreach (DataRow dr in dtCategory.Rows)
        //                    {
        //                        if (dr["Value"].ToString().Equals(sCategory))
        //                        {
        //                            sCategory = dr["CategoryName"].ToString();
        //                            break;
        //                        }
        //                    }
        //                }

        //                if (string.IsNullOrWhiteSpace(sGroupMail))
        //                {
        //                    SpecificEmployeeData.RootObject emp = Extension.GetSpecificEmployeeFromTemp(page, sToEmpID);
        //                    SpecificEmployeeData.RootObject current_user = Extension.GetSpecificEmployeeFromTemp(page, currentUserID);
        //                    sDestinationUserName = string.Format("{0} {1}", emp.FIRSTNAME_EN, emp.LASTNAME_EN);
        //                    sDestinationEmail = emp.EMAIL;
        //                    sActorName = string.Format("{0}{1} {2}", current_user.PREFIX_TH, current_user.FIRSTNAME_TH, current_user.LASTNAME_TH);

        //                    List<Secretary> listSecretary = dataContext.Secretaries.Where(x => x.EmpID == sToEmpID).ToList();
        //                    if (!string.IsNullOrWhiteSpace(sToDepID))
        //                    {
        //                        listSecretary = listSecretary.Where(x => x.DepID == sToDepID).ToList();
        //                    }
        //                    foreach (Secretary item in listSecretary)
        //                    {
        //                        if (!__IsTestEmail)
        //                        {
        //                            string secretaryID = item.SecretaryID;
        //                            sCCEmail += Extension.GetSpecificEmployeeFromTemp(page, secretaryID).EMAIL.ToString() + ";";
        //                        }
        //                        else
        //                        {
        //                            sCCEmail += string.Format("{0}@mail.com;", item.SecretaryID);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    sDestinationEmail = sGroupMail;
        //                }
        //                if (__IsTestEmail)
        //                {
        //                    sDestinationEmail = "tanawad@techconsbiz.com";
        //                    sCCEmail += "atcharee@techconsbiz.com";
        //                }

        //                sSubject = sSubject.Replace("[Request No.]", sRequestNo).Replace("[Subject Memo]", sSubjectMemo);
        //                sBody = sBody.Replace("[Request No.]", sRequestNo)
        //                    .Replace("[Destination User]", sDestinationUserName)
        //                    .Replace("[Category]", sCategory)
        //                    .Replace("[Document No.]", sDocumentNo)
        //                    .Replace("[Document Type]", sDocumentType)
        //                    .Replace("[Subject Memo]", sSubjectMemo)
        //                    .Replace("[Request Name]", sReqester)
        //                    .Replace("[Request Date]", sRequestDate)
        //                    .Replace("[ActorName]", sActorName)
        //                    .Replace("[Action]", sAction)
        //                    .Replace("[ActionDate]", sActionDate)
        //                    .Replace("[Status]", sStatus)
        //                    .Replace("[Comment]", sComment)
        //                    .Replace("[Link PDF]", string.Format("<a href='{0}'>Click</a>", sAttachFilePath))
        //                    .Replace("[Link URL]", string.Format("<a href='{0}/SitePages/e-Document.aspx?pk={1}'>Click</a>", Extension.GetSPSite(), sMemoID));
        //                SendEmail(sDestinationEmail, sCCEmail, sSubject, sBody);

        //                #region | Backup |
        //                //DataTable dtEmp = Extension.GetEmployeeData(page);
        //                //if (dtEmp != null)
        //                //{
        //                //    if (dtEmp.Rows.Count > 0)
        //                //    {
        //                //        DataView dv = dtEmp.Copy().DefaultView;
        //                //        dv.RowFilter = string.Format("EMPLOYEEID = '{0}'", sToEmpID);
        //                //        DataTable dt = dv.ToTable();
        //                //        if (dt != null)
        //                //        {
        //                //            if (dt.Rows.Count > 0)
        //                //            {
        //                //                //Variable
        //                //                string sDestinationUserName = string.Format("{0} {1}", dt.Rows[0][string.Format("FIRSTNAME_{0}", sPDFLanguage)].ToString(), dt.Rows[0][string.Format("LASTNAME_{0}", sPDFLanguage)].ToString());
        //                //                //string sDestinationEmail = dt.Rows[0]["Email"].ToString();
        //                //                string sDestinationEmail = "sarinrat@techconsbiz.com";
        //                //                sCCEmail = "peerapon@techconsbiz.com";
        //                //                //string sDestinationEmail = "chootimaphe@pim.ac.th";
        //                //                DataView dvLastActionBy = dtEmp.Copy().DefaultView;
        //                //                dvLastActionBy.RowFilter = string.Format("EMPLOYEEID = '{0}'", currentUserID);
        //                //                DataTable dtLastActionBy = dvLastActionBy.ToTable();
        //                //                if (dtLastActionBy != null)
        //                //                {
        //                //                    if (dtLastActionBy.Rows.Count > 0)
        //                //                    {
        //                //                        //Variable
        //                //                        string sActorName = string.Format("{0} {1}", dtLastActionBy.Rows[0][string.Format("FIRSTNAME_{0}", sPDFLanguage)].ToString(), dtLastActionBy.Rows[0][string.Format("LASTNAME_{0}", sPDFLanguage)].ToString());

        //                //                        sSubject = sSubject.Replace("[Request No.]", sRequestNo).Replace("[Subject Memo]", sSubjectMemo);
        //                //                        sBody = sBody.Replace("[Request No.]", sRequestNo)
        //                //                            .Replace("[Destination User]", sDestinationUserName)
        //                //                            .Replace("[Subject Memo]", sSubjectMemo)
        //                //                            .Replace("[Request Name]", sReqester)
        //                //                            .Replace("[Request Date]", sRequestDate)
        //                //                            .Replace("[ActorName]", sActorName)
        //                //                            .Replace("[Action]", sAction)
        //                //                            .Replace("[ActionDate]", sActionDate)
        //                //                            .Replace("[Status]", objDocument.Status)
        //                //                            .Replace("[Link URL]", string.Format("<a href='{0}/SitePages/e-Document.aspx?pk={1}'>Click</a>", Extension.GetSPSite(), sMemoID));
        //                //                        SendEmail(sDestinationEmail, sCCEmail, sSubject, sBody);

        //                //                    }
        //                //                    else
        //                //                    {
        //                //                        throw new Exception("Not Found Last Action By");
        //                //                    }
        //                //                }
        //                //                else
        //                //                {
        //                //                    throw new Exception("Null Last Action By");
        //                //                }
        //                //            }
        //                //        }
        //                //        else
        //                //        {
        //                //            throw new Exception("EmpDATA Filter EmpID Null");
        //                //        }
        //                //    }
        //                //    else
        //                //    {
        //                //        throw new Exception("EmpDATA Row Count 0");
        //                //    }
        //                //}
        //                //else
        //                //{
        //                //    throw new Exception("Retrieve EmpDATA Null");
        //                //}
        //                #endregion
        //            }
        //            else
        //            {
        //                throw new Exception("Not Found Template ID");
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Extension.LogWriter.Write(ex);
        //    }

        //}
        #endregion
        public static string SendEmail(string To, string CC, string Subject, string html, bool Sync = false)
        {
            string _SMTPServer = getValueFromTable("SiteSetting", "SMTPServer");
            string _SMTPPort = getValueFromTable("SiteSetting", "SMTPPort");
            string _SMTPEnableSSL = getValueFromTable("SiteSetting", "SMTPEnableSSL");
            string _SMTPUser = getValueFromTable("SiteSetting", "SMTPUser");
            string _SMTPPassword = getValueFromTable("SiteSetting", "SMTPPassword");

            SmtpClient _smtp = new SmtpClient();
            _smtp.Host = _SMTPServer;
            _smtp.Port = Convert.ToInt32(_SMTPPort);
            _smtp.EnableSsl = _SMTPEnableSSL == "true" ? true : false;
            _smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtp.UseDefaultCredentials = false;
            try
            {
                _smtp.Timeout = 600000; // 4 May 2016 : 10 Minutes Timeout
                if (!String.IsNullOrEmpty(_SMTPUser) && !String.IsNullOrEmpty(_SMTPPassword))
                {
                    _smtp.Credentials = new System.Net.NetworkCredential(_SMTPUser, _SMTPPassword);
                }
                else
                {
                    _smtp.UseDefaultCredentials = true;
                }
                MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(_SMTPUser);

                if (To.IndexOf(';') > -1)
                {
                    String[] obj = To.Split(';').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim().ToLower()).Distinct().ToArray();
                    for (int i = 0; i < obj.Length; i++)
                    {
                        mailMessage.To.Add(obj[i].Trim());
                    }
                }
                else
                    mailMessage.To.Add(To.Trim());

                if (!string.IsNullOrEmpty(CC))
                    if (CC.IndexOf(';') > -1)
                    {
                        String[] objcc = CC.Split(';').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim().ToLower()).Distinct().ToArray();
                        for (int i = 0; i < objcc.Length; i++)
                        {
                            mailMessage.CC.Add(objcc[i].Trim());
                        }
                    }
                    else
                        mailMessage.CC.Add(CC.Trim());


                // mailMessage.CC.Add("Sarinrat@techconsbiz.com");

                mailMessage.Subject = Subject.Replace('\r', ' ').Replace('\n', ' '); ;
                System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                mailMessage.AlternateViews.Add(htmlView);
                mailMessage.IsBodyHtml = true;

                if (mailMessage.To.Count > 0)
                {
                    _smtp.Send(mailMessage);

                }
                return "";
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return ex.Message.ToString();
            }
        }

        //Redirect
        public static void Redirect(Page page, string Url)
        {
            string sScript = "window.parent.location = '" + Url + "'; ";
            ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", sScript, true);
        }
        public static void RedirectNewTab(Page page, string Url)
        {
            string sScript = "window.open('" + Url + "', '_blank'); ";
            ScriptManager.RegisterStartupScript(page, typeof(Page), "RedirectNewTab", sScript, true);
        }
        //Log
        public static class LogWriter
        {
            public static string ErrorMessage = string.Empty;
            public static void Write(Exception err)
            {
                DataTable dtDb = GetDataTable("SiteSetting");
                {
                    if (!dtDb.DataTableIsNullOrEmpty())
                    {
                        _ConnectionString = dtDb.Rows[0]["Value"].ToString();
                    }
                }
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(_ConnectionString);
                try
                {
                    if (db.Connection.State == ConnectionState.Open) { db.Connection.Close(); db.Connection.Open(); } else { db.Connection.Open(); }
                    Log objLog = new Log();
                    objLog.Message = err.Message;
                    objLog.StackTrace = err.StackTrace;
                    objLog.PostedDate = DateTime.Now;
                    objLog.PostedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    db.Logs.InsertOnSubmit(objLog);
                    db.SubmitChanges();
                    if (db.Connection.State == ConnectionState.Open) { db.Connection.Close(); }
                }
                catch (Exception ex)
                {
                    if (db.Connection.State == ConnectionState.Open) { db.Connection.Close(); }
                }
            }
            public static void Write(string msg)
            {
                DataTable dtDb = GetDataTable("SiteSetting");
                {
                    if (!dtDb.DataTableIsNullOrEmpty())
                    {
                        _ConnectionString = dtDb.Rows[0]["Value"].ToString();
                    }
                }
                DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(_ConnectionString);
                try
                {
                    if (db.Connection.State == ConnectionState.Open) { db.Connection.Close(); db.Connection.Open(); } else { db.Connection.Open(); }
                    Log objLog = new Log();
                    objLog.Message = msg;
                    objLog.StackTrace = "";
                    objLog.PostedDate = DateTime.Now;
                    objLog.PostedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    db.Logs.InsertOnSubmit(objLog);
                    db.SubmitChanges();
                    if (db.Connection.State == ConnectionState.Open) { db.Connection.Close(); }
                }
                catch (Exception ex)
                {
                    if (db.Connection.State == ConnectionState.Open) { db.Connection.Close(); }
                }
            }
        }

        //PDF
        public static byte[] PrintMemoDetail(string sAction, DataClassesDataAccessDataContext db, int docID, Page page)
        {
            try
            {
                if (db == null)
                {
                    db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                }
                TRNDocument objDocument = db.TRNDocuments.First(x => x.DocID == docID);
                string sDocType = objDocument.DocTypeCode;
                string sCategory = objDocument.Category;
                string sType = objDocument.Type;
                string sOtherDocType = objDocument.OtherDocType;
                string sTitle = objDocument.Title;
                string sTo = objDocument.To;
                string sCC = objDocument.CC;
                string sAttach = objDocument.Attachment;
                string sReceiveDocNo = objDocument.RecieveDocumentNo;
                string sDocDate = objDocument.DocumentDate == null ? "" : ((DateTime)objDocument.DocumentDate).ToString("dd/MM/yyyy");
                string sReceiveDocDate = objDocument.DocumentRecieveDate == null ? "" : ((DateTime)objDocument.DocumentRecieveDate).ToString("dd/MM/yyyy");
                string sSource = objDocument.DocumentSource;
                string sGenerateStatus = "Wait for Admin Centre";
                string sRequestorID = objDocument.RequestorID.ToString();
                string sRequestorDeptID = objDocument.RequestorDepartmentID.ToString();
                string sRequestorDeptName = "";
                string sRequestorSubDeptID = objDocument.RequestorSubDepartmentID.ToString();
                string sRequestorDeptAcronymTH = "";
                string sRequestorDeptAcronymEN = "";
                string sLang = "TH";
                bool sIsInternalOnlyStamp = objDocument.InternalOnlyStamp ?? false;
                string sDocTypeName = GetDataTable("MstDocumentType").AsEnumerable().Where(r => r.Field<String>("Value").Equals(objDocument.DocTypeCode)).ToList().CopyToDataTable().Rows[0]["DocTypeName"].ToString();
                string sFromDepartment = "";
                string DocPDFPath = "";
                string sInstitudeName = "สถาบันการจัดการปัญญาภิวัฒน์";


                List<TRNAttachFileDoc> listAttachDoc = db.TRNAttachFileDocs.Where(x => x.DocID == docID && x.IsPrimary == "Y").ToList();
                if (listAttachDoc.Count > 0)
                {
                    DocPDFPath = listAttachDoc[0].AttachFIlePath;
                }

                DataTable dtRequestorDepInfo = Extension.GetSpecificDepartmentData(page, sRequestorDeptID);
                SpecificEmployeeData.RootObject objRequestorInfo = Extension.GetSpecificEmployeeFromTemp(page, objDocument.RequestorID.ToString());
                if (!string.IsNullOrWhiteSpace(docID.ToString()))
                {
                    objDocument = db.TRNDocuments.First(x => x.DocID == Convert.ToInt32(docID));
                    if (objDocument != null)
                    {
                        objRequestorInfo = Extension.GetSpecificEmployeeFromTemp(page, objDocument.RequestorID.ToString());
                        sRequestorID = objDocument.RequestorID.ToString();
                        sRequestorDeptID = objDocument.RequestorDepartmentID.ToString();
                        sFromDepartment = objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_TH;
                        if (sLang == "EN")
                        {
                            sFromDepartment = objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_EN;
                        }
                    }
                }
                if (!dtRequestorDepInfo.DataTableIsNullOrEmpty())
                {
                    sRequestorDeptAcronymTH = dtRequestorDepInfo.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString();
                    sRequestorDeptAcronymEN = dtRequestorDepInfo.Rows[0]["DEPARTMENT_ACRONYM_EN"].ToString();
                }
                bool isAutoStamp = objDocument.AutoStamp == "Y" ? true : false;
                string sDocumentTypeHead = sDocTypeName + sInstitudeName;
                if (sCategory == "internal" && (sDocType == "C" || sDocType == "P"))
                {
                    sDocumentTypeHead = string.Format("{0}{1} {2}", sDocTypeName, sRequestorDeptName, sInstitudeName); ;
                }

                string sLogoPath = string.Format("{0}/{1}/{2}", SPContext.Current.Site.Url, "Img", "logo.png");
                string sDocumentNumber = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/{0}", DateTime.Now.Year.ToString().ConvertToBE());
                if (objDocument.Status == sGenerateStatus || objDocument.Status == "Completed")
                {
                    string[] a = objDocument.DocNo.Split(new char[] { ' ', '/' });
                    string docNo = "";
                    string docYear = "";
                    if (a.Length >= 2)
                    {
                        int b = Convert.ToInt32(a[a.Length - 2]);
                        docNo = b > 999 ? b.ToString() : string.Format("{0:D4}", b);
                        int year = Convert.ToInt32(a[a.Length - 1]);
                        docYear = year.ToString();
                    }
                    sDocumentNumber = string.Format("{0}/{1}", docNo, docYear.ConvertToBE());
                    if (sDocType == "ExEN")
                    {
                        sDocumentNumber = string.Format("{0}/{1}", docNo, docYear.ConvertToAD());
                    }
                }
                else if (sDocType == "Ex")
                {
                    sDocumentNumber = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/{0}", DateTime.Now.Year.ToString().ConvertToBE());
                }

                DateTime sCreateDate = (DateTime)objDocument.CreatedDate;
                if (!string.IsNullOrWhiteSpace(objDocument.ApproveDate.ToString()))
                {
                    sCreateDate = (DateTime)objDocument.ApproveDate;
                }
                string sDocumentDate = string.Format("{0} {1} {2}", sCreateDate.Day,
                    sCreateDate.Month.GetTHMonth(), sCreateDate.Year.ToString().ConvertToBE());
                //string sDocumentTo = string.Format("เรียน {0}", sTo);
                //string sDocumentTitle = string.Format("เรื่อง {0}", sTitle);

                #region | HeaderTemplate |

                string sHeader = "";

                #region | Memo (บันทึกข้อความ) & Other (อื่นๆ) |

                if (sDocType == "M" || sDocType == "Other")
                {
                    if (sDocType.Equals("Other"))
                    {
                        //sDocType = sOtherDocType;
                        DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
                        if (!dtDocType.DataTableIsNullOrEmpty())
                        {
                            dtDocType = dtDocType.AsEnumerable().Where(x => x.Field<String>("Value").Equals(sOtherDocType)).ToList().CopyToDataTable();
                            if (!dtDocType.DataTableIsNullOrEmpty())
                            {
                                sDocTypeName = dtDocType.Rows[0]["DocTypeName"].ToString();
                            }
                        }
                    }
                    else
                    {
                        //sDocType = "บันทึกข้อความ";
                    }
                    string MemoDocNo = string.Format("{0} {1}", sRequestorDeptAcronymTH, sDocumentNumber);
                    if (!dtRequestorDepInfo.DataTableIsNullOrEmpty())
                    {
                        //เอกสารจากศูนย์?
                        if (dtRequestorDepInfo.Rows[0]["DEPARTMENT_ID"].ToString().Equals("6"))
                        {
                            DataTable dtRequestorSubDepInfo = Extension.GetSpecificDepartmentData(page, sRequestorSubDeptID);
                            if (!dtRequestorSubDepInfo.DataTableIsNullOrEmpty())
                            {
                                MemoDocNo = string.Format("สบว.{0} {1}", dtRequestorSubDepInfo.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString(), sDocumentNumber);
                            }
                        }
                    }


                    //บันทึกข้อความ
                    sHeader = string.Format(
                        @"<div style=""width:800px; margin:50 auto 0 140;"">
						<div style=""text-align:left; color: black; font-family:TH SarabunPSK; font-size:28px;"">
							<img style=""position:absolute;"" src=""{0}"" height=""120"" width=""120""/>
							<div style=""float:right; width:800px; margin-top:50px; text-align:center; font-size:60px;"">
							  <b>{1}</b>
							</div>     
							<div style=""float:left; width:550px;"">
							  <b>{2}&nbsp;&nbsp;</b>{3}
							  <br/>
							  <b>{4}&nbsp;&nbsp;</b>{5}                                
							</div>
							<div style=""float:right; width:250px;"">
							  <b>{8}&nbsp;&nbsp;</b>{9}
							  <br/>
							  <b>{10}&nbsp;&nbsp;</b>{11}  
							</div>
							<div style=""float:right; width:800px;"">
								<b>{6}&nbsp;&nbsp;</b>{7}
							</div>
						</div>
						<div style=""margin: 0 auto; clear:both; width:800px; border-bottom :1px solid; padding-top: 5px; padding-bottom: 5px;""></div>  
					</div>"
                        , sLogoPath, sDocTypeName, "หน่วยงาน", sFromDepartment, "ที่", MemoDocNo, "เรื่อง",
                        sTitle, "โทรศัพท์", objRequestorInfo.TELEPHONE, "วันที่", sDocumentDate);
                }
                #endregion
                #region | Proclamation (ประกาศ) & Command (คำสั่ง) |

                else if (sDocType == "C" || sDocType == "P")
                {
                    sHeader = string.Format(
                        //ประกาศ  คำสั่ง
                        @"<div style=""width:800px; margin:50 auto; "">
			<div style=""text-align:center; color: black; font-family:TH SarabunPSK; font-size:27px; line-height:1.2;"">
				<img src=""{3}""  height=""130"" />
				<br/>
				<b>{0}</b>
				<br/>
				<b>ที่ {1}</b>
				<br />
				<b>เรื่อง {2}</b>   
			</div>
			<div style=""margin: 0 auto; clear:both; width:200px; border-bottom :1px solid; padding-top: 5px; padding-bottom: 5px;""></div>  
			
</div>", sDocumentTypeHead, sDocumentNumber, sTitle, sLogoPath);

                }
                #endregion
                #region | Law (ข้อบังคับ) & Regulation (ระเบียบ) |

                else if (sDocType == "R" || sDocType == "L")
                {
                    sHeader = string.Format(
                        //ข้อบังคับ  ระเบียบ    
                        @"<div style=""width:800px; margin:50 auto 0; "">
			<div style=""text-align:center; color: black; font-family:TH SarabunPSK; font-size:28px;  line-height:1.2;"">
				<img src=""{2}""  height=""130"" />
				<br/>
				<b>{0}</b>
				<br/>
				<b>ว่าด้วย {1}</b> 
			</div>
			<div style=""margin: 0 auto; clear:both; width:200px; border-bottom :1px solid; padding-top: 5px; padding-bottom: 5px;""></div>  
			
</div>", sDocumentTypeHead, sTitle, sLogoPath);

                }
                #endregion
                #region | Export (จดหมายออก) |

                else if (sDocType == "Ex")
                {
                    #region | reference Document |

                    string refTemp = "";
                    DataTable vs_DtRefDoc = Extension.ListToDataTable<TRNReferenceDoc>(db.TRNReferenceDocs.Where(x => x.DocID == docID).ToList());
                    if (!vs_DtRefDoc.DataTableIsNullOrEmpty())
                    {
                        int rRefDocID = Convert.ToInt32(vs_DtRefDoc.Rows[0]["RefDocID"].ToString());
                        TRNDocument objRefDoc = db.TRNDocuments.Where(x => x.DocID == rRefDocID).SingleOrDefault();
                        if (objRefDoc != null)
                        {
                            refTemp = GenerateDocumentNo(objRefDoc.DocNo, Convert.ToInt32(objRefDoc.FromDepartmentID), objRefDoc.Category, objRefDoc.DocTypeCode, page);
                        }
                    }

                    //foreach (DataRow row in vs_DtRefDoc.Rows)
                    //{
                    //    refTemp += string.IsNullOrWhiteSpace(refTemp) ? "" : "</br>";
                    //    refTemp += row["DocNo"].ToString();
                    //}

                    string refDoc = string.IsNullOrEmpty(refTemp) ? "" : refTemp;

                    #endregion

                    #region | Attachment |

                    string attachDoc = string.IsNullOrEmpty(objDocument.Attachment)
                        ? ""
                        : sAttach;

                    #endregion

                    sHeader = string.Format(
                        //จดหมายออก   
                        @"<div style=""width:800px; margin:150 auto 0 140; line-height: 1.2; display:-webkit-box; font-family:TH SarabunPSK; font-size:27px;"">
                            <div style=""width:200px;"">
                                <p>ที่ สจป.{0}</p>
                            </div>
                            <div style=""width:335px;""></div>
                            <div style=""width:265px;"">
                                <span>สถาบันการจัดการปัญญาภิวัฒน์ </br>
                                   85/1 หมู่ 2 ถนนแจ้งวัฒนะ </br>
                                   ตำบลบางตลาด อำเภอปากเกร็ด</br>
                                   จังหวัดนนทบุรี 11120</span>
                            </div>
                          </div>
                          <div style=""width:800px; margin-left:140; font-family:TH SarabunPSK; font-size:27px;"">
                            <div style=""width:500px; text-align:right; margin-top:10px; margin-buttom:10px;"">
                                {1}
                            </div>
                            <div style""width:100%;"">
                                <table>
                                    {2}{3}{4}{5}{6}
                                </table>
                            </div>                      
                          </div>", sCategory == "internal" ? string.Format("{0} {1}", sRequestorDeptAcronymTH, sDocumentNumber) : " " + sDocumentNumber,
                                 sDocumentDate,
                        string.IsNullOrWhiteSpace(sTitle) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:70px; vertical-align:top;"">เรื่อง</td>
                                        <td >{0}</td>
                                    </tr>", sTitle),
                        string.IsNullOrWhiteSpace(sTo) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:70px; vertical-align:top;"">เรียน</td>
                                        <td >{0}</td>
                                    </tr>", sTo),
                        string.IsNullOrWhiteSpace(sCC) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:80px; vertical-align:top;"">สำเนาถึง</td>
                                        <td >{0}</td>
                                    </tr>", sCC),
                        string.IsNullOrWhiteSpace(refDoc) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:70px; vertical-align:top;"">อ้างถึง</td>
                                        <td >{0}</td>
                                    </tr>", refDoc),
                        string.IsNullOrWhiteSpace(attachDoc) ? "" : string.Format(@"<tr style=""font-family:TH SarabunPSK; font-size:27px;"">
                                        <td style=""width:130px; vertical-align:top;"">สิ่งที่แนบมาด้วย</td>
                                        <td >{0}</td>
                                    </tr>", attachDoc));

                }
                #endregion
                #region | Export (จดหมายออกภาษาอังกฤษ) |

                else if (sDocType == "ExEN")
                {
                    sDocumentDate = string.Format("{0} {1} {2}", sCreateDate.Day,
                        sCreateDate.Month.GetENMonth(), sCreateDate.Year.ToString().ConvertToAD());
                    sHeader = string.Format(
                        //จดหมายออกภาษาอังกฤษ  
                        @"<div style=""width:800px; margin:150 auto 0 140; line-height: 1.2;  font-family:TH SarabunPSK; font-size:27px;"">
                            
                                <p>No. PIM.{0}{1}</p>
                            	<p>{2}</p>
                                <p>Panyapiwat Institute of Management<br/>
                                   85/1 Moo 2 Chaengwattana Rd., <br/>
                                   Pakkred, Nonthaburi 11120</p>                           		
                          </div>", sCategory == "internal" ? string.Format("{0}. {1}", sRequestorDeptAcronymEN, sDocumentNumber) : sDocumentNumber, "", sDocumentDate);

                }

                #endregion

                #endregion

                #region | BodyTemplate |

                string sContent;
                if (sDocType == "Im")
                {
                    sContent =
                        string.Format(@"<div style=""width:800px; margin:50 auto 0 140; font-family:TH SarabunPSK; font-size:26px;"">
<h3>รายละเอียดหนังสือภายนอก (Receive Document Detail)</h3>

<div style=""width: 300px; display:inline; float:left;"">
	<span>Document Number: </span></br>
	<span>เลขที่เอกสาร: </span></br>
	<span>Receive Document Number: </span> </br>
	<span>เลขที่หนังสือรับจากภายนอก: </span></br>
	<span>Document Date: </span> </br>
	<span>วันที่ในเอกสาร: </span></br>
	<span>Document Receive: </span></br>
	<span>วันที่รับเอกสาร: </span></br>
	<span>Title: </span></br>
	<span>ชื่อเรื่อง: </span>
</div>
<div style=""width:400px; display:inline; float:left"">
	<span>{0}</span> </br></br>
	<span>{1}</span> </br></br>
	<span>{2}</span> </br></br>
	<span>{3}</span> </br></br>
	<span>{4}</span>
</div>
</div>", sDocumentNumber, sReceiveDocNo, sDocDate, sReceiveDocDate, sTitle);
                }
                else
                {
                    sContent = string.Format(@" <div style=""width:800px; margin:0 auto 0 140;  "">{0}</div>",
                        objDocument.Content);
                }

                #endregion

                #region | Signature |

                List<SignatureImage> listSignatureImages = db.SignatureImages.ToList();
                string wording = "";
                string sLastApprovalID;
                string sLastApprovalFullName;
                string sLastApprovalPosition;
                string sSignatureTag;
                if (sDocType == "L" || sDocType == "R" ||
                    sDocType == "P")
                {
                    wording = "ประกาศ";
                }
                if (sDocType == "C")
                {
                    wording = "สั่ง";
                }

                #region | Signature Head |
                string sSignatureHead =
                    string.Format(@"</br></br><div style=""width:800px; margin:0 auto 0 140;font-family:TH SarabunPSK; font-size:28px;"">
  <div style=""margin-left:140px;"">
  		<p><span>{0}&nbsp;&nbsp;&nbsp;ณ&nbsp;&nbsp;&nbsp;วันที่&nbsp;&nbsp;&nbsp;&nbsp;{1}&nbsp;&nbsp;&nbsp;&nbsp;{2}&nbsp;&nbsp;&nbsp;พ.ศ.&nbsp;&nbsp;&nbsp;{3}</span></p>
  </div>", wording, sCreateDate.Day, sCreateDate.Month.GetTHMonth(), sCreateDate.Year.ToString().ConvertToBE());

                if (sDocType == "Ex")
                {
                    sSignatureHead =
                        string.Format(
                            @"</br></br><div style=""width:800px; margin:0 auto 0 140;font-family:TH SarabunPSK; font-size:28px;"">
  <div style=""width:500px; float:right; text-align:center; "">
  <p><span>ขอแสดงความนับถือ</span></p>
  </div>
");
                }
                if (sDocType == "ExEN")
                {
                    sSignatureHead =
                        string.Format(
                            @"</br></br><div style=""width:800px; margin:0 auto 0 140;font-family:TH SarabunPSK; font-size:28px;"">
  <div style=""float:left;"">
  <p><span>Yours sincerely,</span></p>
  </div>
");
                }
                if (sDocType == "M" || sDocType == "Other")
                {
                    sSignatureHead =
                        string.Format(
                            @"</br></br><div style=""width:800px; margin:0 auto 0 140;font-family:TH SarabunPSK; font-size:28px;"">");
                }
                #endregion

                #region | get Approvor Sign Path |

                DataTable dtApproval = Extension.ListToDataTable<TRNApprover>(db.TRNApprovers.Where(x => x.DocID == docID).ToList());
                if (!dtApproval.DataTableIsNullOrEmpty())
                {
                    DataView dvApprover = dtApproval.DefaultView;
                    dvApprover.Sort = "Sequence DESC";
                    dtApproval = dvApprover.ToTable();

                    sLastApprovalID = dtApproval.Rows[0]["EmpID"].ToString();

                    string sSignaturePath = string.Format("{0}/Signature/{1}.gif", Extension.GetSPSite(), sLastApprovalID);
                    sSignatureTag = string.Format(@" <img height=130 src=""{0}"" />", sSignaturePath);

                    SpecificEmployeeData.RootObject lastApprovalData = Extension.GetSpecificEmployeeFromTemp(page,
                        sLastApprovalID);
                    if (lastApprovalData != null)
                    {
                        sLastApprovalFullName = string.Format("{0}{1} {2}", lastApprovalData.PREFIX_ACADEMIC_TH,
                            lastApprovalData.FIRSTNAME_TH, lastApprovalData.LASTNAME_TH);
                        sLastApprovalPosition = dtApproval.Rows[0]["PositionName"].ToString();
                        if (objDocument.DocTypeCode == "ExEN")
                        {
                            sLastApprovalFullName = string.Format("{0} {1} {2}", lastApprovalData.PREFIX_ACADEMIC_EN,
                           lastApprovalData.FIRSTNAME_EN, lastApprovalData.LASTNAME_EN);
                            sLastApprovalPosition = lastApprovalData.RESULT.First(x => x.POSITION_TD == dtApproval.Rows[0]["PositionID"].ToString()).POSTION_NAME_EN;
                        }
                        if (listSignatureImages.All(x => x.EmployeeID != sLastApprovalID))
                        {
                            sSignatureTag = string.Format("<span style='font-size:40px'><em>{0} {1}</em></span>",
                                lastApprovalData.FIRSTNAME_TH, lastApprovalData.LASTNAME_TH);
                        }
                    }
                    else
                    {
                        throw new Exception("Approval Data Empty");
                    }
                }
                else
                {
                    throw new Exception("Approval Data Row Count 0 or Empty");
                }

                #endregion

                #region | Signature Body |
                string sSignatureBody;
                if (objDocument.DocTypeCode == "ExEN")
                {
                    sSignatureBody = string.Format(@"  
  <p>
  {0}</br>
  <span>({1})</br>{2}</span></p>
  </div>
",
                        isAutoStamp && (objDocument.Status == "Completed" || objDocument.Status == "Wait for Admin Centre")
                            ? sSignatureTag
                            : "</br></br></br></br>", sLastApprovalFullName, sLastApprovalPosition);
                }
                else
                {
                    sSignatureBody = string.Format(@"  
  <div style=""width: 500px; float:right;text-align:center; "">
  {0}<br/>
  <span>({1})<br/>{2}</span>
  </div>
</div>
",
                        isAutoStamp && (objDocument.Status == "Completed" || objDocument.Status == "Wait for Admin Centre")
                            ? sSignatureTag
                            : "<br/><br/>", sLastApprovalFullName, sLastApprovalPosition);
                }
                #endregion

                if (objDocument.DocTypeCode != "Im")
                {
                    sContent += (sSignatureHead + sSignatureBody);
                }
                #endregion

                #region | History |
                string History = "";
                if (!string.IsNullOrEmpty(docID.ToString()))
                {
                    List<TRNHistory> ListHistory = new List<TRNHistory>();
                    ListHistory = db.TRNHistories.Where(x => (x.DocID == docID) && x.ActionName != "Save Draft" && x.ActionName != "Send Email").OrderByDescending(x => x.ActionDate).ToList();
                    if (ListHistory != null)
                    {
                        if (ListHistory.Count > 0)
                        {
                            string sTagApprovalHistory = sLang == "EN" ? "Approval History" : "ประวัติการดำเนินการ";
                            string sActionDateTag = sLang == "EN" ? "Date" : "วันที่";
                            string sActorName = sLang == "EN" ? "Name" : "ชื่อ-นามสกุล";
                            string sPositionName = sLang == "EN" ? "Position" : "ตำแหน่ง";
                            string sActionName = sLang == "EN" ? "Action" : "ดำเนินการโดย";
                            string sSign = sLang == "EN" ? "Signature" : "ลายเซ็นต์";
                            string sComment = sLang == "EN" ? "Comment" : "ความคิดเห็น";
                            string sTemplateHistory = string.Format(@"<div style="" clear :both;padding-bottom:10px;""></div>
           <br/><br/> <div style=""width:900px; margin:0 auto;""><div style=""width:900px;font-family: 'TH SarabunPSK'; font-size: 20px;"">
                <b>{0}</b>
                <table style=""width: 100%; border: 1px solid black; table-layout:fixed; border-collapse: collapse; font-family: 'TH SarabunPSK'; font-size: 20px;"">                   
                    <tr>
                        <td style=""border: 1px solid black; width: 80px; text-align:center; word-wrap:break-word;""><b>{1}</b></td>
                        <td style=""border: 1px solid black; width: 140px; text-align:center; word-wrap:break-word;""><b>{2}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{3}</b></td>
                        <td style=""border: 1px solid black; width: 100px; text-align:center; word-wrap:break-word;""><b>{4}</b></td>
                        <td style=""border: 1px solid black; width: 130px; text-align:center; word-wrap:break-word;""><b>{5}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{6}</b></td>
                    </tr>                    
                ", sTagApprovalHistory, sActionDateTag, sActorName,
                   sPositionName, sActionName, sSign, sComment);
                            string sBodyTableHistory = "";

                            //                            DataTable dtSignature = ObjSPLib.GetList("MstSignatureWording", "");
                            foreach (TRNHistory ObjactHistory in ListHistory)
                            {
                                string sHistorySignaturePath = "";
                                string sSignatureWording = sLang == "EN" ? "	Yours sincerely" : "ขอแสดงความนับถือ";
                                DataView dvHistory = Extension.GetEmployeeData(page).DefaultView;
                                dvHistory.RowFilter = string.Format(@"EMPLOYEEID = '{0}'", ObjactHistory.EmpID);
                                DataTable dtHisEmp = dvHistory.ToTable();
                                if (dtHisEmp != null)
                                {
                                    if (dtHisEmp.Rows.Count > 0)
                                    {
                                        string sActorEmpName = sLang == "EN" ? dtHisEmp.Rows[0]["EmployeeName_EN"].ToString() : dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                        if (listSignatureImages.Any(x => x.EmployeeID == ObjactHistory.EmpID.ToString()))
                                        {
                                            sHistorySignaturePath = string.Format("<img src='{0}' width='130' />", string.Format("{0}/Signature/{1}.gif", Extension.GetSPSite(), ObjactHistory.EmpID.ToString()));
                                        }
                                        if (string.IsNullOrEmpty(sHistorySignaturePath))
                                        {
                                            //string sHisUserLoginName = dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                            string sHisUserLoginName = string.Format("{0} {1}", dtHisEmp.Rows[0]["FIRSTNAME_TH"].ToString(), dtHisEmp.Rows[0]["LASTNAME_TH"].ToString());
                                            sHistorySignaturePath = string.Format("<span style='font-size:20px'><em>{0}</em></span>", sHisUserLoginName);
                                        }
                                        string sBodyRow = string.Format(@"<tr style=""border: 1px solid black;"">
                                                            <td style=""word-wrap:break-word; padding:5px; width: 80px; border: 1px solid black;"">{0}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 140px; border: 1px solid black;"">{1}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{2}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 100px; border: 1px solid black;"">{3}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 130px; border: 1px solid black;"">{4}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{5}</td>
                                                        </tr>", DateTime.Parse(ObjactHistory.ActionDate.ToString()).ToString("dd/MM/yyyy HH:mm", _ctli),
                                                  sActorEmpName,
                                                  sLang == "TH" ? dtHisEmp.Rows[0]["POSTION_NAME_TH"].ToString() : dtHisEmp.Rows[0]["POSTION_NAME_EN"].ToString(),
                                                  ObjactHistory.ActionName,
                                                  sHistorySignaturePath,
                                                  ObjactHistory.Comment);
                                        sBodyTableHistory += sBodyRow;
                                    }
                                }
                            }
                            string sTemplateHistoryEnd = "</table></div></div>";
                            sTemplateHistory = sTemplateHistory + sBodyTableHistory + sTemplateHistoryEnd;
                            History = string.Format(@" <div style=""width:900px; margin:0 auto;  "">{0}</div>", sTemplateHistory);
                            if (sDocType == "Im")
                            {
                                sContent += History;
                            }
                        }
                    }
                }
                #endregion

                #region | Assign |
                string Assign = "";
                if (!string.IsNullOrEmpty(docID.ToString()))
                {
                    List<TRNAssign> ListAssign = new List<TRNAssign>();
                    ListAssign = db.TRNAssigns.Where(x => (x.DocID == docID)).OrderByDescending(x => x.ActionDate).ToList();
                    if (ListAssign != null)
                    {
                        if (ListAssign.Count > 0)
                        {
                            string sTagAssign = sLang == "EN" ? "Assignl History" : "ประวัติการมอบหมายงาน";
                            string sActionDateTag = sLang == "EN" ? "Date" : "วันที่";
                            string sActorName = sLang == "EN" ? "Name" : "ชื่อ-นามสกุล";
                            string sPositionName = sLang == "EN" ? "Position" : "ตำแหน่ง";
                            string sActionName = sLang == "EN" ? "Assign To" : "มอบหมายงานให้";
                            string sComment = sLang == "EN" ? "Comment" : "ความคิดเห็น";
                            string sTemplateAssign = string.Format(@"<div style="" clear :both;padding-bottom:10px;""></div>
           <br/><br/> <div style=""width:900px; margin:0 auto;""><div style=""width:900px;font-family: 'TH SarabunPSK'; font-size: 20px;"">
                <b>{0}</b>
                <table style=""width: 100%; border: 1px solid black; table-layout:fixed; border-collapse: collapse; font-family: 'TH SarabunPSK'; font-size: 20px;"">                   
                    <tr>
                        <td style=""border: 1px solid black; width: 80px; text-align:center; word-wrap:break-word;""><b>{1}</b></td>
                        <td style=""border: 1px solid black; width: 140px; text-align:center; word-wrap:break-word;""><b>{2}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{3}</b></td>
                        <td style=""border: 1px solid black; width: 100px; text-align:center; word-wrap:break-word;""><b>{4}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{5}</b></td>
                    </tr>                    
                ", sTagAssign, sActionDateTag, sActorName,
                   sPositionName, sActionName, sComment);
                            string sBodyTableAssign = "";
                            //                            DataTable dtSignature = ObjSPLib.GetList("MstSignatureWording", "");
                            foreach (TRNAssign ObjactAssign in ListAssign)
                            {
                                DataView dvHistory = Extension.GetEmployeeData(page).DefaultView;
                                dvHistory.RowFilter = string.Format(@"EMPLOYEEID = '{0}'", ObjactAssign.ActorID);
                                DataTable dtHisEmp = dvHistory.ToTable();
                                if (dtHisEmp != null)
                                {
                                    if (dtHisEmp.Rows.Count > 0)
                                    {
                                        string sActorEmpName = sLang == "EN" ? dtHisEmp.Rows[0]["EmployeeName_EN"].ToString() : dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                        SpecificEmployeeData.RootObject objAssignTO = GetSpecificEmployeeFromTemp(page, ObjactAssign.AssignToID.ToString());
                                        string sAssignEmpName = string.Format("{0}{1} {2}", objAssignTO.PREFIX_TH, objAssignTO.FIRSTNAME_TH, objAssignTO.LASTNAME_TH);
                                        string sBodyRow = string.Format(@"<tr style=""border: 1px solid black;"">
                                                            <td style=""word-wrap:break-word; padding:5px; width: 80px; border: 1px solid black;"">{0}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 140px; border: 1px solid black;"">{1}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{2}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 100px; border: 1px solid black;"">{3}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{4}</td>
                                                        </tr>", DateTime.Parse(ObjactAssign.ActionDate.ToString()).ToString("dd/MM/yyyy HH:mm", _ctli),
                                                  sActorEmpName,
                                                  sLang == "TH" ? dtHisEmp.Rows[0]["POSTION_NAME_TH"].ToString() : dtHisEmp.Rows[0]["POSTION_NAME_EN"].ToString(),
                                                  sAssignEmpName,
                                                  ObjactAssign.Comment);
                                        sBodyTableAssign += sBodyRow;
                                    }
                                }
                            }
                            string sTemplateAssignEnd = "</table></div></div>";
                            sTemplateAssign = sTemplateAssign + sBodyTableAssign + sTemplateAssignEnd;
                            Assign = string.Format(@" <div style=""width:900px; margin:0 auto;  "">{0}</div>", sTemplateAssign);
                            if (objDocument.DocTypeCode == "Im")
                            {
                                sContent += Assign;
                            }
                        }
                    }
                }
                #endregion

                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), "A4", true);
                PdfPageOrientation pdfOrientation =
                    (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                    "Portrait", true);

                HtmlToPdf converter = new HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = 1024;
                converter.Options.WebPageHeight = 0;
                converter.Options.MaxPageLoadTime = 300;
                if (objDocument.DocTypeCode == "Im")
                {

                }
                else if (objDocument.DocTypeCode == "Ex")
                {
                    sContent = sHeader + sContent;
                    string sExFooter = "";
                    sExFooter = string.Format(@"
                                <div style=""width:800px; margin:0 auto 0 140; font-family:TH SarabunPSK; font-size:27px; line-height: 1.0;""> 
                                <p>{0}</br>
                                โทร. {1}</br>
                                โทรสาร 0-2832-0391</br>
                                อีเมล์ {2}</p>
                                </div>", objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_TH, objRequestorInfo.TELEPHONE, objRequestorInfo.EMAIL);

                    converter.Options.DisplayFooter = true;
                    converter.Footer.Height = 150;
                    converter.Footer.Add(new PdfHtmlSection(sExFooter, ""));
                }
                else if (objDocument.DocTypeCode == "ExEN")
                {
                    sContent = sHeader + sContent;
                    string sExFooter = "";
                    sExFooter = string.Format(@"
                                <div style=""width:800px; margin:20 auto 0 140; font-family:TH SarabunPSK; font-size:27px; line-height: 1.0;""> 
                                <p>{0}<br/>
                                Tel: {1}<br/>
                                Fax: 0-­2832-­0391<br/>
								E-Mail: {2}</p>
                                </div>", objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_EN, objRequestorInfo.TELEPHONE, objRequestorInfo.EMAIL);

                    converter.Options.DisplayFooter = true;
                    converter.Footer.Height = 150;
                    converter.Footer.Add(new PdfHtmlSection(sExFooter, ""));
                }

                else
                {
                    sContent = sHeader + sContent;
                }



                //add page number
                PdfTextSection pdfPaging = new PdfTextSection(0, 10,
        "Page: {page_number} of {total_pages}",
        new System.Drawing.Font("Angsana New", 12));
                pdfPaging.HorizontalAlign = PdfTextHorizontalAlign.Center;
                //converter.Footer.Add(pdfPaging);

                HtmlToPdf exHtmlPage = new HtmlToPdf();
                exHtmlPage.Options.PdfPageSize = pageSize;
                exHtmlPage.Options.PdfPageOrientation = pdfOrientation;
                exHtmlPage.Options.WebPageWidth = 1024;
                exHtmlPage.Options.WebPageHeight = 0;
                exHtmlPage.Options.MaxPageLoadTime = 300;
                // footer settings
                exHtmlPage.Options.DisplayFooter = true;
                exHtmlPage.Footer.Height = 60;


                // create a new pdf document converting the html string of the page
                SelectPdf.GlobalProperties.HtmlEngineFullPath = Extension.getValueFromTable("SiteSetting", "HtmlToPDFPath");


                PdfDocument doc = new PdfDocument();
                PdfDocument doc1 = new PdfDocument();
                PdfDocument doc2 = new PdfDocument();
                PdfDocument doc3 = new PdfDocument();
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    doc1 = converter.ConvertHtmlString(sContent);
                    doc.Append(doc1);
                    if (!string.IsNullOrEmpty(History) && objDocument.DocTypeCode != "Im")
                    {
                        doc2 = exHtmlPage.ConvertHtmlString(History + Assign);
                        doc.Append(doc2);
                    }
                    if (!string.IsNullOrEmpty(Assign) && objDocument.DocTypeCode != "Im")
                    {
                        ///doc3 = exHtmlPage.ConvertHtmlString(Assign);
                        //doc.Append(doc3);
                    }
                });




                #region | Merge PDF |

                if (!string.IsNullOrEmpty(DocPDFPath) && sDocType == "Im")
                {
                    DataTable dtApprovalSignature = new DataTable();
                    dtApprovalSignature.Columns.Add("EmpID");
                    dtApprovalSignature.Columns.Add("EmployeeName");
                    List<string> approvalList = new List<string>();
                    if (!dtApproval.DataTableIsNullOrEmpty())
                    {
                        //for (var index = dtApproval.Rows.Count; index > 0; index--)
                        for (int index = 0; index < dtApproval.Rows.Count; index++)
                        {
                            if (dtApproval.Rows.Count > 3 && index == 0)
                            {
                                index = dtApproval.Rows.Count - 3;
                            }
                            DataRow row = dtApproval.Rows[index];
                            DataRow target = dtApprovalSignature.NewRow();
                            if (row["EmpID"] != null)
                            {
                                approvalList.Add(row["EmpID"].ToString());
                                //approvalList.Add(row["EmployeeName"].ToString());
                                target["EmpID"] = row["EmpID"];
                                string fullName = "";
                                for (int i = 1; i < row["EmployeeName"].ToString().Split().Count(); i++)
                                {
                                    fullName += row["EmployeeName"].ToString().Split()[i];
                                    fullName += " ";
                                }
                                target["EmployeeName"] = fullName;
                                //target["EmployeeName"] = row["EmployeeName"].ToString().Replace("นางสาว", "").Replace("นาย", "").Replace("นาง", "").Replace("ดอกเตอร์", "");
                                dtApprovalSignature.Rows.Add(target);
                                if (approvalList.Count == 3 || dtApprovalSignature.Rows.Count >= 3)
                                {
                                    break;
                                }
                            }
                        }
                    }



                    byte[] mergedPdf = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (MemoryStream newMemoryStream = new MemoryStream())
                        {
                            using (MemoryStream AddPageMS = new MemoryStream())
                            {
                                using (iTextSharp.text.Document document = new iTextSharp.text.Document())
                                {
                                    using (iTextSharp.text.pdf.PdfCopy copy = new iTextSharp.text.pdf.PdfCopy(document, ms))
                                    {
                                        document.Open();
                                        SPSecurity.RunWithElevatedPrivileges(delegate
                                        {
                                            iTextSharp.text.pdf.PdfReader cover =
                                                new iTextSharp.text.pdf.PdfReader(DocPDFPath);

                                            #region | Insert Signature Image |

                                            iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(cover, newMemoryStream, '\0', true);
                                            if (objDocument.Status == "Completed" && objDocument.AutoStamp == "Y")
                                            {

                                                //for (var index = 0; index < dtApprovalSignature.Rows.Count; index++)
                                                for (int index = dtApprovalSignature.Rows.Count - 1; index >= 0; index--)
                                                {
                                                    stamper.FormFlattening = false;
                                                    iTextSharp.text.pdf.PdfContentByte pdfData = stamper.GetOverContent(1);
                                                    iTextSharp.text.pdf.PdfGState graphicState = new iTextSharp.text.pdf.PdfGState();
                                                    graphicState.FillOpacity = 1.0F;
                                                    pdfData.SetGState(graphicState);
                                                    pdfData.BeginText();
                                                    //string EmpID = approvalList[index];
                                                    string EmpID = dtApprovalSignature.Rows[index]["EmpID"].ToString();
                                                    float width = cover.GetPageSizeWithRotation(1).Width;
                                                    float height = cover.GetPageSizeWithRotation(1).Height;
                                                    bool isPortrait = width < height;

                                                    if (!listSignatureImages.Any(x => x.EmployeeID == dtApprovalSignature.Rows[index]["EmpID"].ToString()))
                                                    {
                                                        string sUserFullnName = dtApprovalSignature.Rows[index]["EmployeeName"].ToString();
                                                        #region | Add Font |
                                                        if (!iTextSharp.text.FontFactory.IsRegistered("TH SarabunPSK"))
                                                        {
                                                            string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                                                            iTextSharp.text.FontFactory.Register(fontPath);
                                                        }
                                                        iTextSharp.text.Font THSarabun = iTextSharp.text.FontFactory.GetFont("TH SarabunPSK", iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
                                                        //var baseFont = iTextSharp.text.pdf.BaseFont.CreateFont("TH SarabunPSK", iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                                                        #endregion
                                                        pdfData.SetFontAndSize(THSarabun.BaseFont, 20);
                                                        pdfData.ShowTextAligned(iTextSharp.text.pdf.PdfContentByte.ALIGN_CENTER, sUserFullnName, 100 + ((isPortrait ? width - 40 : height - 40) / 3) * index, 100, 30);
                                                        //pdfData.ShowTextAligned(iTextSharp.text.pdf.PdfContentByte.ALIGN_CENTER, sUserFullnName, 100 + ((index - 2) * -1 * 193), 100, 30);
                                                    }
                                                    else
                                                    {
                                                        iTextSharp.text.Image img1 =
                                                            iTextSharp.text.Image.GetInstance(string.Format(
                                                                "{0}/Signature/{1}.gif", Extension.GetSPSite(), EmpID));
                                                        img1.SetAbsolutePosition(((isPortrait ? width - 40 : height - 40) / 3) * index, 20);
                                                        img1.ScaleAbsolute(180f, (img1.Height * 180f) / img1.Width);
                                                        //img1.SetAbsolutePosition(20 + (index * 193), 20);
                                                        //img1.ScaleAbsolute(180f, 150f);
                                                        pdfData.AddImage(img1);
                                                    }
                                                    pdfData.EndText();
                                                    //stamper.GetOverContent(1).AddImage(img1, true);
                                                }
                                            }

                                            stamper.Close();
                                            #endregion
                                            cover = new iTextSharp.text.pdf.PdfReader(newMemoryStream.ToArray());

                                            int n = cover.NumberOfPages;
                                            for (int i = 0; i < n;)
                                            {
                                                ++i;
                                                iTextSharp.text.pdf.PdfImportedPage importedPage =
                                                    copy.GetImportedPage(cover, i);
                                                copy.AddPage(importedPage);
                                            }

                                            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(doc.Save());
                                            int m = reader.NumberOfPages;

                                            for (int i = 0; i < m;)
                                            {
                                                ++i;
                                                copy.AddPage(copy.GetImportedPage(reader, i));
                                            }


                                        });
                                    }
                                }
                            }

                            mergedPdf = ms.ToArray();
                            if (sAction == "Cancel" || objDocument.Status == "Cancelled")
                            {
                                //mergedPdf = Extension.AddCancelWatermark(mergedPdf);
                                mergedPdf = AddWaterMark(mergedPdf, "เอกสารยกเลิก");
                            }
                            else if (sIsInternalOnlyStamp)
                            {
                                mergedPdf = AddWaterMark(mergedPdf, "เอกสารใช้ภายในเท่านั้น (Internal Use Only)");
                            }
                            mergedPdf = AddPageNumbers(mergedPdf);
                        }
                    }
                    return mergedPdf;
                }
                #endregion
                #region | Merge PDF Backup |

                //if (!string.IsNullOrEmpty(DocPDFPath) && sDocType == "Im")
                //{
                //    DataTable dtApprovalSignature = new DataTable();
                //    dtApprovalSignature.Columns.Add("EmpID");
                //    dtApprovalSignature.Columns.Add("EmployeeName");
                //    List<string> approvalList = new List<string>();
                //    if (!dtApproval.DataTableIsNullOrEmpty())
                //    {
                //        for (var index = dtApproval.Rows.Count-1; index >= 0 ; index--)
                //        {
                //            DataRow row = dtApproval.Rows[index];
                //            DataRow target = dtApprovalSignature.NewRow();
                //            if (row["EmpID"] != null)
                //            {
                //                target["EmpID"] = row["EmpID"];
                //                string fullName = "";
                //                for (int i = 1; i < row["EmployeeName"].ToString().Split().Count(); i++)
                //                {
                //                    fullName += row["EmployeeName"].ToString().Split()[i];
                //                    fullName += " ";
                //                }
                //                target["EmployeeName"] = fullName;
                //                //target["EmployeeName"] = row["EmployeeName"].ToString().Replace("นางสาว", "").Replace("นาย", "").Replace("นาง", "").Replace("ดอกเตอร์", "");
                //                dtApprovalSignature.Rows.Add(target);
                //                if (dtApprovalSignature.Rows.Count >= 3)
                //                {
                //                    break;
                //                }
                //            }
                //        }
                //    }



                //    byte[] mergedPdf = null;
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        using (MemoryStream newMemoryStream = new MemoryStream())
                //        {
                //            using (iTextSharp.text.Document document = new iTextSharp.text.Document())
                //            {
                //                using (iTextSharp.text.pdf.PdfCopy copy = new iTextSharp.text.pdf.PdfCopy(document, ms))
                //                {
                //                    document.Open();
                //                    SPSecurity.RunWithElevatedPrivileges(delegate
                //                    {
                //                        iTextSharp.text.pdf.PdfReader cover =
                //                            new iTextSharp.text.pdf.PdfReader(DocPDFPath);

                //                        #region | Insert Signature Image |

                //                        iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(cover, newMemoryStream, '\0', true);
                //                        if (objDocument.Status == "Completed")
                //                        {
                //                            for (var index = approvalList.Count-1; index >= 0 ; index--)
                //                            {
                //                                stamper.FormFlattening = false;
                //                                iTextSharp.text.pdf.PdfContentByte pdfData = stamper.GetOverContent(1);
                //                                iTextSharp.text.pdf.PdfGState graphicState = new iTextSharp.text.pdf.PdfGState();
                //                                graphicState.FillOpacity = 1.0F;
                //                                pdfData.SetGState(graphicState);
                //                                pdfData.BeginText();

                //                                //string EmpID = approvalList[index];
                //                                string EmpID = dtApprovalSignature.Rows[index]["EmpID"].ToString();

                //                                if (!listSignatureImages.Any(x => x.EmployeeID == dtApprovalSignature.Rows[index]["EmpID"].ToString()))
                //                                {
                //                                    string sUserFullnName = dtApprovalSignature.Rows[index]["EmployeeName"].ToString();
                //                                    #region | Add Font |
                //                                    if (!iTextSharp.text.FontFactory.IsRegistered("TH SarabunPSK"))
                //                                    {
                //                                        var fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                //                                        iTextSharp.text.FontFactory.Register(fontPath);
                //                                    }
                //                                    iTextSharp.text.Font THSarabun = iTextSharp.text.FontFactory.GetFont("TH SarabunPSK", iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
                //                                    //var baseFont = iTextSharp.text.pdf.BaseFont.CreateFont("TH SarabunPSK", iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                //                                    #endregion
                //                                    pdfData.SetFontAndSize(THSarabun.BaseFont, 20);
                //                                    pdfData.ShowTextAligned(iTextSharp.text.pdf.PdfContentByte.ALIGN_CENTER, sUserFullnName, 100 + ((index - 2) * -1 * 193), 100, 30);
                //                                }
                //                                else
                //                                {
                //                                    iTextSharp.text.Image img1 =
                //                                        iTextSharp.text.Image.GetInstance(string.Format(
                //                                            "{0}/Signature/{1}.gif", Extension.GetSPSite(), EmpID));
                //                                    img1.SetAbsolutePosition(20 + (index * 193), 20);
                //                                    img1.ScaleAbsolute(180f, 150f);
                //                                    pdfData.AddImage(img1);
                //                                }
                //                                pdfData.EndText();

                //                                //string item = approvalList[index];
                //                                //iTextSharp.text.Image img1 =
                //                                //    iTextSharp.text.Image.GetInstance(string.Format(
                //                                //        "{0}/Signature/{1}.jpg", Extension.GetSPSite(), item));
                //                                //img1.SetAbsolutePosition(20 + (index * 193), 20);
                //                                //img1.ScaleAbsolute(180f, 150f);
                //                                //stamper.GetOverContent(1).AddImage(img1, true);
                //                            }
                //                        }

                //                        stamper.Close();
                //                        #endregion

                //                        cover = new iTextSharp.text.pdf.PdfReader(newMemoryStream.ToArray());

                //                        int n = cover.NumberOfPages;
                //                        for (int pages = 0; pages < n; )
                //                        {
                //                            iTextSharp.text.pdf.PdfImportedPage importedPage =
                //                                copy.GetImportedPage(cover, ++pages);
                //                            copy.AddPage(importedPage);
                //                        }

                //                        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(doc.Save());
                //                        int m = reader.NumberOfPages;
                //                        for (int pages = 0; pages < m; )
                //                        {
                //                            copy.AddPage(copy.GetImportedPage(reader, ++pages));
                //                        }

                //                    });
                //                }
                //            }
                //            mergedPdf = ms.ToArray();
                //        }
                //    }
                //    return mergedPdf;
                //}
                #endregion

                return AddPageNumbers(doc.Save());
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return null;
            }
        }

        public static byte[] PrintMemoDetail(string sAction, DataClassesDataAccessDataContext db, TRNDocument objDocument, Page page)
        {
            int sDocID = objDocument.DocID;
            string sDocType = objDocument.DocTypeCode;
            string sDocLib = objDocument == null ? "TempDocument" : objDocument.DocLib;
            string sDocSet = objDocument.DocSet;
            //string sDocTypeName = ddl_DocType.SelectedItem.ToString();
            string sCategory = objDocument.Category;
            string sType = objDocument.Type;
            string sOtherDocType = objDocument.OtherDocType;
            string sTitle = objDocument.Title;
            string sTo = objDocument.To;
            string sCC = objDocument.CC;
            string sAttach = objDocument.Attachment;
            string sReceiveDocNo = objDocument.RecieveDocumentNo;
            string sDocDate = objDocument.DocumentDate == null ? "" : objDocument.DocumentDate.Value.ToString("dd/MM/yyyy");
            string sReceiveDocDate = objDocument.RecieveDocumentNo;
            string sSource = objDocument.DocumentSource;
            string sFromDepartment = objDocument.FromDepartmentName;
            string sGenerateStatus = Extension._WaitForAdminCentre;
            string sRequestorID = objDocument.RequestorID ?? "";
            string sRequestorDeptID = objDocument.RequestorDepartmentID.ToString();
            string sRequestorDeptName = "";
            string sRequestorSubDeptID = objDocument.RequestorSubDepartmentID.ToString();
            string sRequestorDeptAcronymTH = "";
            string sRequestorDeptAcronymEN = "";
            string sLang = "TH";
            bool sIsInternalOnlyStamp = objDocument.InternalOnlyStamp ?? false;
            string sDocTypeName = GetDataTable("MstDocumentType").AsEnumerable().Where(r => r.Field<String>("Value").Equals(objDocument.DocTypeCode)).ToList().CopyToDataTable().Rows[0]["DocTypeName"].ToString();
            string sDocPDFPath = objDocument.AttachFilePath;
            string sDocWordPath = objDocument.AttachWordPath;
            string sInstitudeName = "สถาบันการจัดการปัญญาภิวัฒน์";

            try
            {
                if (db == null)
                {
                    db = new DataClassesDataAccessDataContext(Extension.GetDBConnectionString());
                }
                //startConversion = true;
                SpecificEmployeeData.RootObject objRequestorInfo = GetSpecificEmployeeFromTemp(page, sRequestorID);
                if (objRequestorInfo != null)
                {
                    //if (sLang == "EN")
                    //{
                    //    sFromDepartment = objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_EN;
                    //}
                }
                DataTable dtRequestorDepInfo = Extension.GetSpecificDepartmentDataFromTemp(page, sRequestorDeptID);
                if (!dtRequestorDepInfo.DataTableIsNullOrEmpty())
                {
                    sRequestorDeptAcronymTH = dtRequestorDepInfo.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString();
                    sRequestorDeptAcronymEN = dtRequestorDepInfo.Rows[0]["DEPARTMENT_ACRONYM_EN"].ToString();
                    sRequestorDeptName = dtRequestorDepInfo.Rows[0][string.Format("DEPARTMENT_NAME_{0}", sLang)].ToString();
                }

                bool isAutoStamp = objDocument.AutoStamp.Equals("Y");
                string sDocumentTypeHead = sDocTypeName + sInstitudeName;
                if (sCategory.Equals("internal") && (sDocType == "C" || sDocType == "P"))
                {
                    sDocumentTypeHead = string.Format("{0}{1} {2}", sDocTypeName, sRequestorDeptName, sInstitudeName); ;
                }

                string sLogoPath = string.Format("{0}/{1}/{2}", SPContext.Current.Site.Url, "Img", "logo.png");
                string sDocumentNumber = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/{0}", DateTime.Now.Year.ToString().ConvertToBE());
                if (objDocument.Status.Equals(sGenerateStatus) || objDocument.Status.Equals(_Completed))
                {
                    string[] a = objDocument.DocNo.Split(new char[] { ' ', '/' });
                    string docNo = "";
                    string docYear = "";
                    if (a.Length >= 2)
                    {
                        int b = Convert.ToInt32(a[a.Length - 2]);
                        docNo = b > 999 ? b.ToString() : string.Format("{0:D4}", b);
                        int year = Convert.ToInt32(a[a.Length - 1]);
                        docYear = year.ToString();
                    }
                    sDocumentNumber = string.Format("{0}/{1}", docNo, docYear.ConvertToBE());
                    if (sDocType == "ExEN")
                    {
                        sDocumentNumber = string.Format("{0}/{1}", docNo, docYear.ConvertToAD());
                    }
                }
                else if (sDocType == "Ex")
                {
                    sDocumentNumber = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/{0}", DateTime.Now.Year.ToString().ConvertToBE());
                }

                DateTime sCreateDate = objDocument.CreatedDate ?? DateTime.MinValue;
                if (objDocument.ApproveDate != null)
                {
                    sCreateDate = objDocument.ApproveDate.Value;
                }
                string sDocumentDate = string.Format("{0} {1} {2}", sCreateDate.Day,
                    sCreateDate.Month.GetTHMonth(), sCreateDate.Year.ToString().ConvertToBE());

                #region | HeaderTemplate |

                string sHeader = "";

                #endregion

                #region | BodyTemplate |

                string sContent;
                if (sDocType == "Im")
                {
                    sContent =
                        string.Format(@"<div style=""width:800px; margin:50 auto 0 140; font-family:TH SarabunPSK; font-size:26px;"">
<h3>รายละเอียดหนังสือภายนอก (Receive Document Detail)</h3>

<div style=""width: 300px; display:inline; float:left;"">
	<span>Document Number: </span></br>
	<span>เลขที่เอกสาร: </span></br>
	<span>Receive Document Number: </span> </br>
	<span>เลขที่หนังสือรับจากภายนอก: </span></br>
	<span>Document Date: </span> </br>
	<span>วันที่ในเอกสาร: </span></br>
	<span>Document Receive: </span></br>
	<span>วันที่รับเอกสาร: </span></br>
	<span>Title: </span></br>
	<span>ชื่อเรื่อง: </span>
</div>
<div style=""width:400px; display:inline; float:left"">
	<span>{0}</span> </br></br>
	<span>{1}</span> </br></br>
	<span>{2}</span> </br></br>
	<span>{3}</span> </br></br>
	<span>{4}</span>
</div>
</div>", sDocumentNumber, sReceiveDocNo, sDocDate, sReceiveDocDate, sTitle);
                }
                else
                {
                    sContent = string.Format(@" <div style=""width:800px; margin:0 auto 0 140;  "">{0}</div>",
                        "");
                }

                #endregion

                #region | Signature |

                List<SignatureImage> listSignatureImages = db.SignatureImages.ToList();

                #endregion

                #region | History |
                string History = "";
                if (objDocument.DocID > 0)
                {
                    List<TRNHistory> ListHistory = db.TRNHistories.Where(x => (x.DocID == sDocID) && x.ActionName != "Save Draft" && x.ActionName != "Confirm").OrderByDescending(x => x.ActionDate).ToList();
                    if (ListHistory != null && ListHistory.Count > 0)
                    {
                        string sTagApprovalHistory = sLang == "EN" ? "Approval History" : "ประวัติการดำเนินการ";
                        string sActionDateTag = sLang == "EN" ? "Date" : "วันที่";
                        string sActorName = sLang == "EN" ? "Name" : "ชื่อ-นามสกุล";
                        string sPositionName = sLang == "EN" ? "Position" : "ตำแหน่ง";
                        string sActionName = sLang == "EN" ? "Action" : "ดำเนินการโดย";
                        string sSign = sLang == "EN" ? "Signature" : "ลายเซ็นต์";
                        string sComment = sLang == "EN" ? "Comment" : "ความคิดเห็น";
                        string sTemplateHistory = string.Format(@"<div style="" clear :both;padding-bottom:10px;""></div>
           <br/><br/> <div style=""width:900px; margin:0 auto;""><div style=""width:900px;font-family: 'TH SarabunPSK'; font-size: 20px;"">
                <b>{0}</b>
                <table style=""width: 100%; border: 1px solid black; table-layout:fixed; border-collapse: collapse; font-family: 'TH SarabunPSK'; font-size: 20px;"">                   
                    <tr>
                        <td style=""border: 1px solid black; width: 80px; text-align:center; word-wrap:break-word;""><b>{1}</b></td>
                        <td style=""border: 1px solid black; width: 140px; text-align:center; word-wrap:break-word;""><b>{2}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{3}</b></td>
                        <td style=""border: 1px solid black; width: 100px; text-align:center; word-wrap:break-word;""><b>{4}</b></td>
                        <td style=""border: 1px solid black; width: 130px; text-align:center; word-wrap:break-word;""><b>{5}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{6}</b></td>
                    </tr>                    
                ", sTagApprovalHistory, sActionDateTag, sActorName,
               sPositionName, sActionName, sSign, sComment);
                        string sBodyTableHistory = "";

                        //                            DataTable dtSignature = ObjSPLib.GetList("MstSignatureWording", "");
                        foreach (TRNHistory ObjactHistory in ListHistory)
                        {
                            string sHistorySignaturePath = "";
                            string sSignatureWording = sLang == "EN" ? "	Yours sincerely" : "ขอแสดงความนับถือ";
                            DataView dvHistory = Extension.GetEmployeeData(page).DefaultView;
                            dvHistory.RowFilter = string.Format(@"EMPLOYEEID = '{0}'", ObjactHistory.EmpID);
                            DataTable dtHisEmp = dvHistory.ToTable();
                            if (dtHisEmp != null)
                            {
                                if (dtHisEmp.Rows.Count > 0)
                                {
                                    string sActorEmpName = sLang == "EN" ? dtHisEmp.Rows[0]["EmployeeName_EN"].ToString() : dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                    if (listSignatureImages.Any(x => x.EmployeeID == ObjactHistory.EmpID.ToString()))
                                    {
                                        sHistorySignaturePath = string.Format("<img src='{0}' width='130' />", string.Format("{0}/Signature/{1}.gif", Extension.GetSPSite(), ObjactHistory.EmpID.ToString()));
                                    }
                                    if (string.IsNullOrEmpty(sHistorySignaturePath))
                                    {
                                        //string sHisUserLoginName = dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                        string sHisUserLoginName = string.Format("{0} {1}", dtHisEmp.Rows[0]["FIRSTNAME_TH"].ToString(), dtHisEmp.Rows[0]["LASTNAME_TH"].ToString());
                                        sHistorySignaturePath = string.Format("<span style='font-size:20px'><em>{0}</em></span>", sHisUserLoginName);
                                    }
                                    string sBodyRow = string.Format(@"<tr style=""border: 1px solid black;"">
                                                            <td style=""word-wrap:break-word; padding:5px; width: 80px; border: 1px solid black;"">{0}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 140px; border: 1px solid black;"">{1}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{2}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 100px; border: 1px solid black;"">{3}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 130px; border: 1px solid black;"">{4}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{5}</td>
                                                        </tr>", DateTime.Parse(ObjactHistory.ActionDate.ToString()).ToString("dd/MM/yyyy HH:mm", _ctli),
                                              sActorEmpName,
                                              sLang == "TH" ? dtHisEmp.Rows[0]["POSTION_NAME_TH"].ToString() : dtHisEmp.Rows[0]["POSTION_NAME_EN"].ToString(),
                                              ObjactHistory.ActionName,
                                              sHistorySignaturePath,
                                              ObjactHistory.Comment);
                                    sBodyTableHistory += sBodyRow;
                                }
                            }
                        }
                        string sTemplateHistoryEnd = "</table></div></div>";
                        sTemplateHistory = sTemplateHistory + sBodyTableHistory + sTemplateHistoryEnd;
                        History = string.Format(@" <div style=""width:900px; margin:0 auto;  "">{0}</div>", sTemplateHistory);
                        if (sDocType == "Im")
                        {
                            sContent += History;
                        }

                    }
                }
                #endregion

                #region | Assign |
                string Assign = "";
                if (objDocument.DocID > 0)
                {
                    List<TRNAssign> ListAssign = db.TRNAssigns.Where(x => (x.DocID == sDocID)).OrderByDescending(x => x.ActionDate).ToList();
                    if (ListAssign != null && ListAssign.Count > 0)
                    {

                        string sTagAssign = sLang == "EN" ? "Assignl History" : "ประวัติการมอบหมายงาน";
                        string sActionDateTag = sLang == "EN" ? "Date" : "วันที่";
                        string sActorName = sLang == "EN" ? "Name" : "ชื่อ-นามสกุล";
                        string sPositionName = sLang == "EN" ? "Position" : "ตำแหน่ง";
                        string sActionName = sLang == "EN" ? "Assign To" : "มอบหมายงานให้";
                        string sComment = sLang == "EN" ? "Comment" : "ความคิดเห็น";
                        string sTemplateAssign = string.Format(@"<div style="" clear :both;padding-bottom:10px;""></div>
           <br/><br/> <div style=""width:900px; margin:0 auto;""><div style=""width:900px;font-family: 'TH SarabunPSK'; font-size: 20px;"">
                <b>{0}</b>
                <table style=""width: 100%; border: 1px solid black; table-layout:fixed; border-collapse: collapse; font-family: 'TH SarabunPSK'; font-size: 20px;"">                   
                    <tr>
                        <td style=""border: 1px solid black; width: 80px; text-align:center; word-wrap:break-word;""><b>{1}</b></td>
                        <td style=""border: 1px solid black; width: 140px; text-align:center; word-wrap:break-word;""><b>{2}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{3}</b></td>
                        <td style=""border: 1px solid black; width: 100px; text-align:center; word-wrap:break-word;""><b>{4}</b></td>
                        <td style=""border: 1px solid black; width: 150px; text-align:center; word-wrap:break-word;""><b>{5}</b></td>
                    </tr>                    
                ", sTagAssign, sActionDateTag, sActorName,
               sPositionName, sActionName, sComment);
                        string sBodyTableAssign = "";
                        //                            DataTable dtSignature = ObjSPLib.GetList("MstSignatureWording", "");
                        foreach (TRNAssign ObjactAssign in ListAssign)
                        {
                            DataView dvHistory = Extension.GetEmployeeData(page).DefaultView;
                            dvHistory.RowFilter = string.Format(@"EMPLOYEEID = '{0}'", ObjactAssign.ActorID);
                            DataTable dtHisEmp = dvHistory.ToTable();
                            if (dtHisEmp != null)
                            {
                                if (dtHisEmp.Rows.Count > 0)
                                {
                                    string sActorEmpName = sLang == "EN" ? dtHisEmp.Rows[0]["EmployeeName_EN"].ToString() : dtHisEmp.Rows[0]["EmployeeName_TH"].ToString();
                                    SpecificEmployeeData.RootObject objAssignTO = Extension.GetSpecificEmployeeFromTemp(page, ObjactAssign.AssignToID.ToString());
                                    string sAssignEmpName = string.Format("{0}{1} {2}", objAssignTO.PREFIX_TH, objAssignTO.FIRSTNAME_TH, objAssignTO.LASTNAME_TH);
                                    string sBodyRow = string.Format(@"<tr style=""border: 1px solid black;"">
                                                            <td style=""word-wrap:break-word; padding:5px; width: 80px; border: 1px solid black;"">{0}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 140px; border: 1px solid black;"">{1}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{2}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 100px; border: 1px solid black;"">{3}</td>
                                                            <td style=""word-wrap:break-word; padding:5px; width: 150px; border: 1px solid black;"">{4}</td>
                                                        </tr>", DateTime.Parse(ObjactAssign.ActionDate.ToString()).ToString("dd/MM/yyyy HH:mm", _ctli),
                                              sActorEmpName,
                                              sLang == "TH" ? dtHisEmp.Rows[0]["POSTION_NAME_TH"].ToString() : dtHisEmp.Rows[0]["POSTION_NAME_EN"].ToString(),
                                              sAssignEmpName,
                                              ObjactAssign.Comment);
                                    sBodyTableAssign += sBodyRow;
                                }
                            }
                        }
                        string sTemplateAssignEnd = "</table></div></div>";
                        sTemplateAssign = sTemplateAssign + sBodyTableAssign + sTemplateAssignEnd;
                        Assign = string.Format(@" <div style=""width:900px; margin:0 auto;  "">{0}</div>", sTemplateAssign);
                        if (sDocType.Equals("Im"))
                        {
                            sContent += Assign;
                        }

                    }
                }
                #endregion

                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), "A4", true);
                PdfPageOrientation pdfOrientation =
                    (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                    "Portrait", true);
                Extension.LogWriter.Write(new Exception("B  HtmlToPdf converter = new HtmlToPdf();"));
                HtmlToPdf converter = new HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = 1024;
                converter.Options.WebPageHeight = 0;
                converter.Options.MaxPageLoadTime = 300;
                if (sDocType.Equals("Im"))
                {

                }
                else if (sDocType.Equals("Ex"))
                {
                    sContent = sHeader + sContent;
                    string sExFooter = "";
                    sExFooter = string.Format(@"
                                <div style=""width:800px; margin:0 auto 0 140; font-family:TH SarabunPSK; font-size:27px; line-height: 1.2;""> 
                                <p>{0}</br>
                                โทร. {1}</br>
                                โทรสาร 0-2832-0391</br>
                                อีเมล์ {2}</p>
                                </div>", objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_TH, objRequestorInfo.TELEPHONE, objRequestorInfo.EMAIL);

                    converter.Options.DisplayFooter = true;
                    converter.Footer.Height = 150;
                    converter.Footer.Add(new PdfHtmlSection(sExFooter, ""));
                }
                else if (sDocType.Equals("ExEN"))
                {
                    sContent = sHeader + sContent;
                    string sExFooter = "";
                    sExFooter = string.Format(@"
                                <div style=""width:800px; margin:20 auto 0 140; font-family:TH SarabunPSK; font-size:27px; line-height: 1.2;""> 
                                <p>{0}<br/>
                                Tel: {1}<br/>
                                Fax: 0-­2832-­0391<br/>
								E-Mail: {2}</p>
                                </div>", objRequestorInfo.RESULT.First(x => x.DEPARTMENT_ID == sRequestorDeptID).DEPARTMENT_NAME_EN, objRequestorInfo.TELEPHONE, objRequestorInfo.EMAIL);

                    converter.Options.DisplayFooter = true;
                    converter.Footer.Height = 150;
                    converter.Footer.Add(new PdfHtmlSection(sExFooter, ""));
                }

                else
                {
                    sContent = sHeader + sContent;
                }


                Extension.LogWriter.Write(new Exception("B  PdfTextSection pdfPaging = new PdfTextSection"));
                //add page number
                PdfTextSection pdfPaging = new PdfTextSection(0, 10,
        "Page: {page_number} of {total_pages}",
        new System.Drawing.Font("Angsana New", 12));
                pdfPaging.HorizontalAlign = PdfTextHorizontalAlign.Center;
                //converter.Footer.Add(pdfPaging);

                HtmlToPdf exHtmlPage = new HtmlToPdf();
                exHtmlPage.Options.PdfPageSize = pageSize;
                exHtmlPage.Options.PdfPageOrientation = pdfOrientation;
                exHtmlPage.Options.WebPageWidth = 1024;
                exHtmlPage.Options.WebPageHeight = 0;
                exHtmlPage.Options.MaxPageLoadTime = 300;
                // footer settings
                exHtmlPage.Options.DisplayFooter = true;
                exHtmlPage.Footer.Height = 60;


                SelectPdf.GlobalProperties.LicenseKey = "XHdtfG5pbXxtaWp8bWtybHxvbXJtbnJlZWVl";

                // create a new pdf document converting the html string of the page
                SelectPdf.GlobalProperties.HtmlEngineFullPath = Extension.getValueFromTable("SiteSetting", "HtmlToPDFPath");


                PdfDocument doc = new PdfDocument();
                PdfDocument doc1 = new PdfDocument();
                PdfDocument doc2 = null;
                PdfDocument doc3 = null;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    doc1 = converter.ConvertHtmlString(sContent);
                    if (sDocType == "Im")
                    {
                        doc.Append(doc1);
                    }
                    if (!string.IsNullOrWhiteSpace(History) && (!sDocType.Equals("Im") || string.IsNullOrWhiteSpace(sDocPDFPath)))
                    {
                        doc2 = new PdfDocument();
                        doc2 = exHtmlPage.ConvertHtmlString(History);// + Assign);
                        doc.Append(doc2);
                    }
                    if (!string.IsNullOrWhiteSpace(Assign) && !sDocType.Equals("Im") && !string.IsNullOrWhiteSpace(sDocPDFPath) && sAction != "Print")
                    {
                        doc3 = new PdfDocument();
                        doc3 = exHtmlPage.ConvertHtmlString(Assign);
                        doc.Append(doc3);
                    }

                });

                Extension.LogWriter.Write(new Exception("B Merge PDF"));
                #region | Merge PDF |

                if ((!string.IsNullOrWhiteSpace(sDocPDFPath) && sDocType == "Im") || (!string.IsNullOrWhiteSpace(sDocWordPath) && sDocType != "Im"))
                {
                    PdfDocument mergedPdf = null;
                    List<PdfDocument> listPdf = new List<PdfDocument>();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate
                        {
                            #region | 'Im' PageInfo Counter |
                            int infoPageCounter = 0;
                            iTextSharp.text.pdf.PdfReader readerDoc = null;
                            if (doc != null && sDocType.Equals("Im"))
                            {
                                readerDoc = new iTextSharp.text.pdf.PdfReader(doc.Save());
                                infoPageCounter = readerDoc.NumberOfPages;
                            }
                            #endregion
                            #region | History Page Counter |
                            iTextSharp.text.pdf.PdfReader readerDoc2 = null;
                            int historyPageCounter = 0;
                            if (doc2 != null)
                            {
                                List<TRNHistory> sHistory = db.TRNHistories.Where(x => (x.DocID == sDocID)).ToList();
                                if (sHistory != null)
                                {
                                    if (sHistory.Count() > 1)
                                    {
                                        readerDoc2 = new iTextSharp.text.pdf.PdfReader(doc2.Save());
                                        historyPageCounter = readerDoc2.NumberOfPages;
                                    }
                                }
                            }
                            #endregion
                            #region | Assign Page Counter |
                            iTextSharp.text.pdf.PdfReader readerDoc3 = null;
                            int assignPageCounter = 0;
                            if (doc3 != null)
                            {
                                IQueryable<IGrouping<DateTime, TRNAssign>> sAssign = db.TRNAssigns.Where(x => (x.DocID == sDocID)).GroupBy(x => x.ActionDate.Value);
                                if (sAssign != null)
                                {
                                    if (sAssign.Count() > 1)
                                    {
                                        readerDoc3 = new iTextSharp.text.pdf.PdfReader(doc3.Save());
                                        assignPageCounter = readerDoc3.NumberOfPages;
                                    }
                                }
                            }
                            #endregion
                            if (!string.IsNullOrWhiteSpace(sDocPDFPath))
                            {
                                byte[] docPdfByte = SharedRules.GetSPFile(sDocPDFPath);

                                MemoryStream DocPDFms = new MemoryStream(docPdfByte);
                                PdfDocument pdf = new PdfDocument(DocPDFms);

                                #region | Insert Signature Image for 'หนังสือรับเข้าจากภายนอก' (Im) |

                                #region | Approver Signature Info |
                                DataTable dtApprovalSignature = new DataTable();
                                dtApprovalSignature.Columns.Add("EmpID");
                                dtApprovalSignature.Columns.Add("EmployeeName");
                                List<string> approvalList = new List<string>();

                                DataTable dtApproval = Extension.ListToDataTable<TRNApprover>(db.TRNApprovers.Where(x => x.DocID == sDocID).ToList());
                                if (!dtApproval.DataTableIsNullOrEmpty())
                                {
                                    for (int index = 0; index < dtApproval.Rows.Count; index++)
                                    {
                                        if (dtApproval.Rows.Count > 3 && index == 0)
                                        {
                                            index = dtApproval.Rows.Count - 3;
                                        }
                                        DataRow row = dtApproval.Rows[index];
                                        DataRow target = dtApprovalSignature.NewRow();
                                        if (row["EmpID"] != null)
                                        {
                                            approvalList.Add(row["EmpID"].ToString());
                                            //approvalList.Add(row["EmployeeName"].ToString());
                                            target["EmpID"] = row["EmpID"];
                                            string fullName = "";
                                            for (int i = 1; i < row["EmployeeName"].ToString().Split().Count(); i++)
                                            {
                                                fullName += row["EmployeeName"].ToString().Split()[i];
                                                fullName += " ";
                                            }
                                            target["EmployeeName"] = fullName;
                                            dtApprovalSignature.Rows.Add(target);
                                            if (approvalList.Count == 3 || dtApprovalSignature.Rows.Count >= 3)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion
                                if (objDocument.Status.Equals(Extension._Completed) && isAutoStamp && sDocType.Equals("Im"))
                                {
                                    float width = pdf.Pages[0].PageSize.Width;
                                    float height = pdf.Pages[0].PageSize.Height;
                                    bool isPortrait = width < height;

                                    int seqCounter = 1;
                                    List<KeyValuePair<string, string>> listSignature = new List<KeyValuePair<string, string>>();
                                    for (int index = dtApprovalSignature.Rows.Count - 1; index >= 0; index--)
                                    {
                                        string EmpID = dtApprovalSignature.Rows[index]["EmpID"].ToString();
                                        SignatureImage SignaturePath = listSignatureImages.FirstOrDefault(x => x.EmployeeID.Equals(EmpID));
                                        if (SignaturePath == null)
                                        {
                                            string sUserFullnName = dtApprovalSignature.Rows[index]["EmployeeName"].ToString();
                                            listSignature.Add(new KeyValuePair<string, string>(sUserFullnName, "string"));
                                        }
                                        else
                                        {
                                            listSignature.Add(new KeyValuePair<string, string>(SignaturePath.Path, "image"));
                                        }
                                        seqCounter++;
                                    }
                                    pdf = Extension.SelectPDFAddSignature(pdf, listSignature);
                                }
                                listPdf.Add(pdf);

                                #endregion

                                //int n = cover.NumberOfPages;
                                #region | Remove redundant page (History - Assign) |
                                //if (vs_Status.Equals("Completed"))
                                //{
                                //    n = cover.NumberOfPages - assignPageCounter;
                                //    if (n < 2 && assignPageCounter > 0)
                                //    {
                                //        n = cover.NumberOfPages - (assignPageCounter - 1);
                                //    }
                                //    if (n > 1 && !string.IsNullOrWhiteSpace(vs_PK) && historyPageCounter >= 0 && historyPageCounter < n && !vs_isExternalUpload)//&& author.Equals("TCB-EDocument System"))
                                //    {
                                //        n = n - historyPageCounter;
                                //    }
                                //    if (n > 1 && !string.IsNullOrWhiteSpace(vs_PK) && infoPageCounter >= 0 && infoPageCounter < n)
                                //    {
                                //        n = n - infoPageCounter;
                                //    }
                                //}
                                #endregion
                            }
                            // Add MS Word (Type: !Im)
                            if (!string.IsNullOrWhiteSpace(sDocWordPath) && (sAction != "Confirm" && objDocument.DocNo.Equals("Auto Generate")))
                            {

                                //string uploadResult = UpdateMSWord(false, sDocumentNumber);
                                //if (!string.IsNullOrWhiteSpace(uploadResult))
                                //{
                                //    throw new Exception(uploadResult);
                                //}
                                string pdfFile = Extension.ChangeTypeToPDF(sDocWordPath, sDocLib, sDocSet, page);
                                byte[] docPdfByte = SharedRules.GetSPFile(pdfFile);

                                MemoryStream DocPDFms = new MemoryStream(docPdfByte);
                                PdfDocument pdfWord = new PdfDocument(DocPDFms);
                                listPdf.Add(pdfWord);
                            }

                            if (sDocType == "Im" && doc != null)
                            {
                                listPdf.Add(doc);
                            }
                            //Add History TBL && Assign TBL 
                            if (doc2 != null)
                            {
                                listPdf.Add(doc2);
                            }
                            if (doc3 != null)
                            {
                                listPdf.Add(doc3);
                            }
                            mergedPdf = Extension.MergeSelectPDF(listPdf);

                        });

                        if (sAction == "Cancel" || objDocument.Status.Equals("Cancelled"))
                        {
                            mergedPdf = Extension.SelectPDFAddWaterMark(mergedPdf, "เอกสารยกเลิก", false);
                        }
                        else if (sIsInternalOnlyStamp)
                        {
                            mergedPdf = Extension.SelectPDFAddWaterMark(mergedPdf, "เอกสารใช้ภายในเท่านั้น (Internal Use Only)", false);
                        }
                        //}
                    }
                    return Extension.SelectPDFAddPageNumber(mergedPdf, true).Save(); //Extension.AddPageNumbers(mergedPdf);
                }
                #endregion
                Extension.LogWriter.Write(new Exception("E Merge PDF"));
                if (objDocument.Status.Equals(Extension._Cancelled))
                {
                    return Extension.SelectPDFAddWaterMark(doc, "เอกสารยกเลิก", false).Save();
                }
                else if (sIsInternalOnlyStamp)
                {
                    return Extension.SelectPDFAddWaterMark(doc, "เอกสารใช้ภายในเท่านั้น (Internal Use Only)", false).Save();
                }
                //startConversion = false;
                Extension.LogWriter.Write(new Exception("B return Extension.SelectPDFAddPageNumber(doc, true).Save();"));
                return Extension.SelectPDFAddPageNumber(doc, true).Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                //startConversion = false;
                throw ex;
                //return null;
            }
        }



        public static PdfDocument MergeSelectPDF(List<PdfDocument> PDFs)
        {
            try
            {
                PdfDocument mergeDoc = new PdfDocument();
                foreach (PdfDocument pdf in PDFs)
                {
                    mergeDoc.Append(pdf);
                }
                return mergeDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFAddPageNumber(PdfDocument PDFs, bool isAddToTop)
        {
            try
            {
                // create a new pdf document
                PdfDocument doc = PDFs;
                doc.Margins = new PdfMargins(10, 10, 0, 0);

                // create a new pdf font
                string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                PdfFont font = doc.AddFont(fontPath);
                //PdfFont font = doc.AddFont(PdfStandardFont.TimesRoman);

                font.Size = 14;
                for (int i = 1; i < PDFs.Pages.Count; i++)
                {
                    PdfPage page = doc.Pages[i];
                    page.DisplayHeader = false;
                    #region | Back up |
                    //PdfPage page = doc.Pages[i];
                    //page.DisplayHeader = true;

                    //float width = page.PageSize.Width;
                    //float height = page.PageSize.Height;
                    //bool isPortrait = width > height;

                    //float yPosition = isAddToTop ? 15 : height - 15;

                    //PdfTemplate customHeader = doc.AddTemplate(page.ClientRectangle);
                    //PdfTextElement text1 = new PdfTextElement(0, yPosition,
                    //    "{page_number}", font);
                    //text1.HorizontalAlign = PdfTextHorizontalAlign.Center;
                    //text1.ForeColor = System.Drawing.Color.Black;
                    //customHeader.DisplayOnFirstPage = false;
                    //customHeader.Add(text1);
                    #endregion
                }

                return doc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFAddWaterMark(PdfDocument PDFs, string WaterText, bool isAdditional)
        {
            try
            {
                PDFs.Margins = new PdfMargins(10, 10, 0, 0);

                // create a new pdf font
                string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                PdfFont font = PDFs.AddFont(fontPath);
                font.Size = 14;
                for (int i = 0; i < PDFs.Pages.Count; i++)
                {
                    PdfPage page = PDFs.Pages[i];

                    float width = page.PageSize.Width;
                    float height = page.PageSize.Height;
                    float additionalHeight = isAdditional ? 45 : 0;
                    bool isPortrait = width < height;

                    PdfTemplate customTemplate = PDFs.AddTemplate(width, height);//;
                    PdfTextElement textElement = new PdfTextElement(-60, (height / 2) + additionalHeight, width, height, WaterText, font)
                    {
                        ForeColor = System.Drawing.Color.FromArgb(50, 0, 0, 0),
                        Direction = 45

                    };
                    textElement.Font.Size = 40;
                    textElement.Transparency = 50;
                    textElement.HorizontalAlign = PdfTextHorizontalAlign.Center;
                    textElement.VerticalAlign = PdfTextVerticalAlign.Middle;

                    customTemplate.Add(textElement);
                }

                return PDFs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFAddSignature(PdfDocument PDFs, List<KeyValuePair<string, string>> ListApproval)
        {
            try
            {
                PdfDocument firstPage = SelectPDFGetFirstPage(PDFs);

                // create a new pdf font
                string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun Italic.ttf";
                PdfFont font = firstPage.AddFont(fontPath);
                font.Size = 14;

                PdfPage page = firstPage.Pages[0];

                float pageWidth = page.PageSize.Width;
                float pageHeight = page.PageSize.Height;
                bool isPortrait = pageWidth < pageHeight;

                PdfTemplate customTemplate = firstPage.AddTemplate(page.ClientRectangle);
                customTemplate.DisplayOnFirstPage = true;
                customTemplate.Background = false;

                for (int i = 0; i < ListApproval.Count; i++)
                {
                    KeyValuePair<string, string> approval = ListApproval[i];
                    bool isImage = approval.Value.Equals("image");
                    float xPosition = (((isPortrait ? pageWidth - 40 : pageHeight - 40) / 3) * (i)) + 10;
                    float yPosition = pageHeight - 150;

                    if (isImage)
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(SharedRules.GetSPFile(approval.Key)));

                        float scaledHeight = 180f;
                        float scaledWidth = (image.Height * scaledHeight) / image.Width;

                        PdfImageElement imageElm = new PdfImageElement(xPosition, yPosition, 180, image);
                        imageElm.TransparentRendering = true;

                        customTemplate.Add(imageElm);
                    }
                    else
                    {
                        PdfTextElement textElement = new PdfTextElement(xPosition, yPosition + 100, 180, approval.Key, font)
                        {
                            HorizontalAlign = PdfTextHorizontalAlign.Center,
                            ForeColor = System.Drawing.Color.Black,
                            Direction = 45

                        };
                        textElement.Font.Size = 30;
                        customTemplate.Add(textElement);
                    }
                }

                return SelectPDFReplaceFirstPage(PDFs, firstPage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFGetFirstPage(PdfDocument PDFs)
        {
            try
            {
                // load the pdf document
                PdfDocument doc1 = PDFs;
                // create a new pdf document
                PdfDocument doc = new PdfDocument();

                doc.AddPage(doc1.Pages[0]);
                return doc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFReplaceFirstPage(PdfDocument MainPDFs, PdfDocument NewFirstPage)
        {
            try
            {
                // load the pdf document
                PdfDocument doc1 = NewFirstPage;
                PdfDocument doc2 = MainPDFs;
                // create a new pdf document
                PdfDocument doc = new PdfDocument();

                doc.AddPage(doc1.Pages[0]);
                for (int i = 1; i < doc2.Pages.Count; i++)
                {
                    doc.AddPage(doc2.Pages[i]);
                }
                return doc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //ITextSharp
        public static byte[] MergePDFs(List<iTextSharp.text.pdf.PdfReader> listPdf)
        {
            MemoryStream ms = new MemoryStream();

            // step 1: creation of a document-object
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            // step 2: we create a writer that listens to the document
            iTextSharp.text.pdf.PdfCopy writer = new iTextSharp.text.pdf.PdfSmartCopy(document, ms);
            // step 3: we open the document
            document.Open();
            // step 4: Get ContentByte
            iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;

            foreach (iTextSharp.text.pdf.PdfReader pdf in listPdf)
            {
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(pdf);
                reader.ConsolidateNamedDestinations();

                for (int i = 1; i < pdf.NumberOfPages; i++)
                {
                    // get page dimensions
                    iTextSharp.text.Rectangle pageSize = reader.GetPageSize(i);
                    // check if rotation is required
                    bool isRotationRequired = (pageSize.Width > pageSize.Height);

                    // step 5: get the page imported
                    iTextSharp.text.pdf.PdfImportedPage importedPage = writer.GetImportedPage(reader, i);
                    if (isRotationRequired)
                    {
                        int rotation = reader.GetPageRotation(i) - 90;
                        cb.AddTemplate(importedPage, 0, -1f, 1f, 0, reader.GetPageSizeWithRotation(i).Width, reader.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        cb.AddTemplate(importedPage, 1f, 0, 0, 1f, 0, 0);
                    }
                }

                // step 6: we add content
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    iTextSharp.text.pdf.PdfImportedPage page = writer.GetImportedPage(reader, i);
                    writer.AddPage(page);
                }
                // close the reader
                reader.Close();
                //writer.CopyDocumentFields(reader);
            }
            // step 5: we close the document
            writer.Close();
            document.Close();
            return ms.ToArray();
        }
        public static byte[] AddSignature(iTextSharp.text.pdf.PdfReader pdf, string Emp, bool isImage, int Sequence)
        {
            string text = isImage ? string.Format("{0}/Signature/{1}.gif", GetSPSite(), Emp) : Emp;


            MemoryStream ms = new MemoryStream();
            // we create a reader for a certain document
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(pdf);
            // we retrieve the total number of pages
            int n = reader.NumberOfPages;
            // we retrieve the size of the first page
            iTextSharp.text.Rectangle psize = reader.GetPageSizeWithRotation(1);

            // step 1: creation of a document-object
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            // step 2: we create a writer that listens to the document
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
            // step 3: we open the document
            document.Open();
            // step 4: we add content
            iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;

            int page = 1;

            document.SetPageSize(reader.GetPageSizeWithRotation(page));
            document.NewPage();
            float width = reader.GetPageSizeWithRotation(page).Width;
            float height = reader.GetPageSizeWithRotation(page).Height;
            bool isPortrait = height < width;
            if (width > height)
            {
                //iTextSharp.text.pdf.PdfDictionary pageDict = reader.GetPageNRelease(page);
                //iTextSharp.text.pdf.PdfNumber rotate = pageDict.GetAsNumber(iTextSharp.text.pdf.PdfName.ROTATE);
                //int rotation = rotate == null ? 90 : (rotate.IntValue + 90) % 360;
                //pageDict.Put(iTextSharp.text.pdf.PdfName.ROTATE, new iTextSharp.text.pdf.PdfNumber(rotation));
            }

            cb.AddTemplate(writer.GetImportedPage(reader, page), 0, 0);
            #region | Add Font |
            if (!iTextSharp.text.FontFactory.IsRegistered("TH SarabunPSK"))
            {
                string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                iTextSharp.text.FontFactory.Register(fontPath);
            }
            iTextSharp.text.Font THSarabun = iTextSharp.text.FontFactory.GetFont("TH SarabunPSK", iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
            #endregion
            cb.BeginText();
            if (isImage)
            {
                iTextSharp.text.Image img1 = iTextSharp.text.Image.GetInstance(text);
                img1.SetAbsolutePosition(((isPortrait ? width - 40 : height - 40) / 3) * Sequence, 20);
                img1.ScaleAbsolute(180f, (img1.Height * 180f) / img1.Width);
                cb.AddImage(img1);
            }
            else
            {
                cb.SetFontAndSize(THSarabun.BaseFont, 30);
                cb.ShowTextAligned(iTextSharp.text.pdf.PdfContentByte.ALIGN_CENTER, text, (width / 2), (height / 2), 30);
            }
            cb.EndText();

            // step 5: we close the document
            document.Close();
            return ms.ToArray();
        }
        public static byte[] AddPageNumbers(byte[] pdf)
        {
            MemoryStream ms = new MemoryStream();
            // we create a reader for a certain document
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(pdf);
            // we retrieve the total number of pages
            int n = reader.NumberOfPages;
            // we retrieve the size of the first page
            iTextSharp.text.Rectangle psize = reader.GetPageSizeWithRotation(1);

            // step 1: creation of a document-object
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            //document.SetPageSize(iTextSharp.text.PageSize.A4);
            //document.SetPageSize(psize);
            // step 2: we create a writer that listens to the document
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
            //iTextSharp.text.pdf.PdfCopy copy = new iTextSharp.text.pdf.PdfCopy(document, ms);
            // step 3: we open the document
            document.Open();
            // step 4: we add content
            iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
            //iTextSharp.text.pdf.PdfContentByte cb = copy.DirectContent;


            int p = 0;
            Console.WriteLine("There are " + n + " pages in the document.");
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                document.SetPageSize(reader.GetPageSizeWithRotation(page));
                document.NewPage();
                p++;
                float width = reader.GetPageSizeWithRotation(page).Width;
                float height = reader.GetPageSizeWithRotation(page).Height;
                if (width > height)
                {
                    //iTextSharp.text.pdf.PdfDictionary pageDict = reader.GetPageNRelease(page);
                    //iTextSharp.text.pdf.PdfNumber rotate = pageDict.GetAsNumber(iTextSharp.text.pdf.PdfName.ROTATE);
                    //int rotation = rotate == null ? 90 : (rotate.IntValue + 90) % 360;
                    //pageDict.Put(iTextSharp.text.pdf.PdfName.ROTATE, new iTextSharp.text.pdf.PdfNumber(rotation));
                }

                cb.AddTemplate(writer.GetImportedPage(reader, page), 0, 0);
                #region | Add Font |
                if (!iTextSharp.text.FontFactory.IsRegistered("TH SarabunPSK"))
                {
                    string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                    iTextSharp.text.FontFactory.Register(fontPath);
                }
                iTextSharp.text.Font THSarabun = iTextSharp.text.FontFactory.GetFont("TH SarabunPSK", iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
                #endregion
                cb.BeginText();
                cb.SetFontAndSize(THSarabun.BaseFont, 14);
                if (p > 1)
                {
                    cb.ShowTextAligned(iTextSharp.text.pdf.PdfContentByte.ALIGN_CENTER, p.ToString(), width / 2, height - 20, 0);
                }
                cb.EndText();
            }
            // step 5: we close the document
            document.Close();
            return ms.ToArray();
        }
        public static byte[] AddCancelWatermark(byte[] pdf)
        {
            MemoryStream ms = new MemoryStream();
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(pdf);
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
            document.Open();
            iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                document.SetPageSize(reader.GetPageSizeWithRotation(page));
                document.NewPage();
                float width = reader.GetPageSizeWithRotation(page).Width;
                float height = reader.GetPageSizeWithRotation(page).Height;

                cb.AddTemplate(writer.GetImportedPage(reader, page), 0, 0);
                #region | Add Font |
                if (!iTextSharp.text.FontFactory.IsRegistered("TH SarabunPSK"))
                {
                    string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                    iTextSharp.text.FontFactory.Register(fontPath);
                }
                iTextSharp.text.Font THSarabun = iTextSharp.text.FontFactory.GetFont("TH SarabunPSK", iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED, 50, 0, iTextSharp.text.BaseColor.RED);
                #endregion
                cb.BeginText();
                cb.SetFontAndSize(THSarabun.BaseFont, 50);
                cb.ShowTextAligned(iTextSharp.text.pdf.PdfContentByte.ALIGN_CENTER, "เอกสารยกเลิก", width / 2, height / 2, 45);
                cb.EndText();
            }
            document.Close();
            return ms.ToArray();
        }
        public static byte[] AddWaterMark(byte[] pdf, string text, bool additionalWatermark = false)
        {
            MemoryStream ms = new MemoryStream();
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(pdf);
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
            document.Open();
            iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;

            iTextSharp.text.pdf.PdfGState gstate = new iTextSharp.text.pdf.PdfGState();
            gstate.FillOpacity = 0.5f;
            gstate.StrokeOpacity = 0.5f;

            int additaional = additionalWatermark ? 50 : 0;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                document.SetPageSize(reader.GetPageSizeWithRotation(page));
                document.NewPage();
                float width = reader.GetPageSizeWithRotation(page).Width;
                float height = reader.GetPageSizeWithRotation(page).Height;

                cb.AddTemplate(writer.GetImportedPage(reader, page), 0, 0);
                #region | Add Font |
                if (!iTextSharp.text.FontFactory.IsRegistered("TH SarabunPSK"))
                {
                    string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                    iTextSharp.text.FontFactory.Register(fontPath);
                }
                iTextSharp.text.Font THSarabun = iTextSharp.text.FontFactory.GetFont("TH SarabunPSK", iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED, 50, 0, iTextSharp.text.BaseColor.RED);
                #endregion

                cb.SaveState();
                cb.SetGState(gstate);

                cb.BeginText();
                cb.SetFontAndSize(THSarabun.BaseFont, 50);
                cb.SetColorFill(new iTextSharp.text.BaseColor(0f, 0f, 0f, 0.5f));
                cb.ShowTextAligned(iTextSharp.text.pdf.PdfContentByte.ALIGN_CENTER, text, (width / 2), (height / 2) + additaional, 45);
                cb.EndText();
                cb.RestoreState();
            }
            document.Close();
            return ms.ToArray();
        }

        //WordInterop
        public static string GetTemporaryDirectory(string fileName, string fileType)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), string.Format("{0}.{1}", fileName, fileType));
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
        public static byte[] GetFileAsByteArray(string sURI)
        {
            try
            {
                byte[] result = null;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    result = new System.Net.WebClient().DownloadData(sURI);
                });
                if (result == null)
                {
                    throw new Exception(string.Format("File has no content: {0}", sURI));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string SaveFileToLocal(string sFilePath, string FileName, byte[] Data)
        {
            string Name = sFilePath + FileName;
            try
            {
                string username = Extension.getValueFromTable("SiteSetting", "comUsername");
                string domainName = Extension.getValueFromTable("SiteSetting", "comDomainName");
                string password = Extension.getValueFromTable("SiteSetting", "comPassword");
                using (new Impersonator(username, domainName, password))
                {
                    using (FileStream fs = new FileStream(Name, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        fs.Write(Data, 0, Data.Length);
                        fs.Close();
                    }
                }
                return Name;
            }
            catch (Exception ex)
            {
                LogWriter.Write(ex);
                throw ex;
            }
        }
        public static string ChangeTypeToPDF(string sPath, string sDocLib, string sDocSet, Page page)
        {
            byte[] bArr = getMSWord(sPath);
            if (bArr != null)
            {
                //Use for the parameter whose type are not known or say Missing
                object Unknown = System.Reflection.Missing.Value;
                //LogWriter.Write(new Exception(sPath));
                string finalPdfPath = string.Empty;
                using (MemoryStream ms = new MemoryStream())
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate
                    {
                        // Open WordApp > Download file to Local > Edit Docuemnt > Save file > Upload to DocSet > Quit WordApp

                        //if (wordApp == null) { wordApp = new Microsoft.Office.Interop.Word.Application(); }
                        wordApp = new Microsoft.Office.Interop.Word.Application();

                        object missing = System.Reflection.Missing.Value;
                        string savePath = SaveFileToLocal(Path.GetTempPath(), Guid.NewGuid().ToString() + ".docx", bArr);
                        object path = savePath;//sPath; // input
                        object newFileName = Path.GetTempPath() + sDocSet + ".pdf";// "document.pdf"; // output
                        object readOnly = false;
                        object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;
                        object doNotSaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                        try
                        {
                            Microsoft.Office.Interop.Word.Document docs = new Microsoft.Office.Interop.Word.Document();

                            docs = wordApp.Documents.Open(ref path, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

                            if (docs != null)
                            {
                                try
                                {
                                    docs.Activate();
                                    wordApp.Visible = false;
                                    wordApp.ActiveWindow.View.ReadingLayout = false;

                                    docs.SaveAs2(ref newFileName, ref format, ref missing, ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                    //LogWriter.Write(ex);
                                }
                                finally
                                {
                                    docs.Close(doNotSaveChanges, ref Unknown, ref Unknown);
                                }

                                //upload to SP
                                FileStream fs = new FileStream(newFileName.ToString(), FileMode.Open);
                                SharedRules.UploadFileIntoDocumentSet(sDocLib, sDocSet, "Template.pdf", fs, "", "");
                                finalPdfPath = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), sDocLib, sDocSet, "Template.pdf");
                                fs.Close();
                                fs.Dispose();

                            }
                            else
                            {
                                MessageBox(page, "Document is null");
                                throw new Exception("Document is null");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogWriter.Write(ex);
                            throw ex;
                        }
                        finally
                        {
                            // for closing the application
                            wordApp.Quit(ref Unknown, ref Unknown, ref Unknown);

                            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wordApp);
                            wordApp = null;
                        }

                    });
                    return finalPdfPath.ToString();
                }
            }
            return "";
        }
        public static void UploadFileToSP(string sDocLib, string sDocSet, string sFileName, byte[] pdf)
        {
            //upload to SP
            MemoryStream ms = new MemoryStream(pdf);
            SharedRules.UploadFileIntoDocumentSet(sDocLib, sDocSet, sFileName, ms, "", ""); //Filename include .pdf
            //finalPdfPath = string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), sDocLib, sDocSet, sFileName); 
        }

        //OpenXML
        public static byte[] UpdateMSWord(Page page, byte[] byteArray, TRNDocument objDocument, TRNApprover objApproval, string[] referenceDocument)
        {
            if (byteArray != null)
            {
                //Extension.LogWriter.Write(new Exception("Create MS"));
                using (MemoryStream ms = new MemoryStream())
                {
                    //Extension.LogWriter.Write(new Exception("Write MS"));
                    ms.Write(byteArray, 0, byteArray.Length);
                    //Extension.LogWriter.Write(new Exception("Opening OPENXML"));
                    using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument wDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(ms, true))
                    {
                        //Extension.LogWriter.Write(new Exception("Opened OPENXML"));
                        DocumentFormat.OpenXml.Wordprocessing.Table headerTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Header");

                        if (headerTbl != null)
                        {
                            DataClassesDataAccessDataContext db = new DataClassesDataAccessDataContext(GetDBConnectionString());
                            List<SignatureImage> listSignature = db.SignatureImages.ToList();
                            #region | Add value to Document |
                            DataTable deptInfo = GetSpecificDepartmentDataFromTemp(page, (objDocument.IsOccurBySubDepartment ?? false) ? objDocument.RequestorSubDepartmentID.ToString() : objDocument.FromDepartmentID.ToString());
                            SpecificEmployeeData.RootObject reqEmp = GetSpecificEmployeeFromTemp(page, objDocument.RequestorID.ToString());
                            #region | Document No |
                            string oDocNo = string.Format("    /{0}", DateTime.Now.ToString("yyyy", new System.Globalization.CultureInfo("th-TH")));
                            if (objDocument.DocTypeCode == "ExEN")
                            {
                                oDocNo = string.Format("    /{0}", DateTime.Now.ToString("yyyy", new System.Globalization.CultureInfo("en-GB")));
                            }
                            if (!string.IsNullOrWhiteSpace(objDocument.DocNo) && objDocument.DocNo != "Auto Generate")
                            {
                                if (objDocument.DocTypeCode == "Ex")
                                {
                                    oDocNo = string.Format("สจป.{0}", objDocument.Category == "internal" ? string.Format("{0} {1}", deptInfo.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString(), objDocument.DocNo) : " " + objDocument.DocNo);
                                }
                                else if (objDocument.DocTypeCode == "ExEN")
                                {
                                    oDocNo = string.Format("PIM.{0}", objDocument.Category == "internal" ? string.Format("{0} {1}", deptInfo.Rows[0]["DEPARTMENT_ACRONYM_EN"].ToString(), objDocument.DocNo) : " " + objDocument.DocNo);
                                }
                                else if (objDocument.DocTypeCode == "M" || objDocument.DocTypeCode == "Other")
                                {
                                    oDocNo = string.Format("{0}", objDocument.Category == "internal" ? string.Format("{0} {1}", deptInfo.Rows[0]["DEPARTMENT_ACRONYM_TH"].ToString(), objDocument.DocNo) : " " + objDocument.DocNo);
                                }
                                else
                                {
                                    oDocNo = objDocument.DocNo;
                                }

                            }
                            #endregion
                            #region | Approve Date |
                            bool isApprove = false;
                            DateTime ApprovedDate = DateTime.MinValue;
                            if (objDocument.ApproveDate > DateTime.MinValue)
                            {
                                isApprove = true;
                                ApprovedDate = (DateTime)objDocument.ApproveDate;
                            }
                            else
                            {
                                ApprovedDate = DateTime.Now;
                            }
                            #endregion
                            //Add Value to MSWord by Template
                            switch (objDocument.DocTypeCode)
                            {
                                case "P":
                                    #region | TBL_Header |
                                    //Remove Table to be same as Template
                                    XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, headerTbl, 4);
                                    //Head
                                    string pDept = objDocument.Category.Equals("internal") ? string.Format("{0} ", deptInfo.Rows[0]["DEPARTMENT_NAME_TH"].ToString()) : "";
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 1, 1, string.Format("ประกาศ{0}{1}", pDept, "สถาบันการจัดการปัญญาภิวัฒน์"));
                                    //Docno
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 2, 1, string.Format("ที่  {0}", oDocNo));
                                    //Title
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 3, 1, string.Format("เรื่อง  {0}", objDocument.Title));
                                    #endregion
                                    #region | TBL_Signature |
                                    DocumentFormat.OpenXml.Wordprocessing.Table pSignatureTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Signature");
                                    if (pSignatureTbl != null)
                                    {
                                        //Remove Table to be same as Template
                                        //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, pSignatureTbl, 7);
                                        if (isApprove)
                                        {
                                            XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, pSignatureTbl, 1, 1, string.Format("ประกาศ     ณ     วันที่   {0}   {1}   พ.ศ.   {2}", ApprovedDate.Day.ToString(), ApprovedDate.ToString("MMMM", _ctliTH), ApprovedDate.ToString("yyyy", _ctliTH)));
                                        }
                                        else
                                        {
                                            XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, pSignatureTbl, 1, 1, "ประกาศ     ณ     วันที่      พ.ศ.   ");
                                        }
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, pSignatureTbl, 5, 1, string.Format("({0})", objApproval.EmployeeName), "centre");
                                        //XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, pSignatureTbl, 6, 1, objApproval.PositionName, "centre");
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, pSignatureTbl, 7, 1, objApproval.PositionName, "centre");
                                    }
                                    else
                                    {
                                        MessageBox(page, "TBL_Signature was missing");
                                    }
                                    #endregion
                                    break;
                                case "C":
                                    #region | TBL_Header |
                                    //Remove Table to be same as Template
                                    XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, headerTbl, 4);
                                    //Head
                                    string cDept = objDocument.Category.Equals("internal") ? string.Format("{0} ", deptInfo.Rows[0]["DEPARTMENT_NAME_TH"].ToString()) : "";
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 1, 1, string.Format("คำสั่ง{0}{1}", cDept, "สถาบันการจัดการปัญญาภิวัฒน์"));
                                    //Docno
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 2, 1, string.Format("ที่  {0}", oDocNo));
                                    //Title
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 3, 1, string.Format("เรื่อง  {0}", objDocument.Title));
                                    #endregion
                                    #region | TBL_Signature |
                                    DocumentFormat.OpenXml.Wordprocessing.Table cSignatureTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Signature");
                                    if (cSignatureTbl != null)
                                    {
                                        //Remove Table to be same as Template
                                        //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, cSignatureTbl, 7);
                                        if (isApprove)
                                        {
                                            XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, cSignatureTbl, 1, 1, string.Format("สั่ง     ณ     วันที่   {0}   {1}   พ.ศ.   {2}", ApprovedDate.Day.ToString(), ApprovedDate.ToString("MMMM", _ctliTH), ApprovedDate.ToString("yyyy", _ctliTH)));
                                        }
                                        else
                                        {
                                            XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, cSignatureTbl, 1, 1, "สั่ง     ณ     วันที่      พ.ศ.   ");
                                        }
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, cSignatureTbl, 5, 1, string.Format("({0})", objApproval.EmployeeName), "centre");
                                        //XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, cSignatureTbl, 6, 1, objApproval.PositionName, "centre");
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, cSignatureTbl, 7, 1, objApproval.PositionName, "centre");
                                    }
                                    else
                                    {
                                        MessageBox(page, "TBL_Signature was missing");
                                    }
                                    #endregion
                                    break;
                                case "L":
                                    #region | TBL_Header |
                                    //Remove Table to be same as Template
                                    //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, headerTbl, 3);
                                    //Head
                                    string lDept = objDocument.Category.Equals("internal") ? string.Format("{0} ", deptInfo.Rows[0]["DEPARTMENT_NAME_TH"].ToString()) : "";
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 1, 1, string.Format("ข้อบังคับ{0}{1}", lDept, "สถาบันการจัดการปัญญาภิวัฒน์"));
                                    //Title
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 2, 1, string.Format("ว่าด้วย  {0}", objDocument.Title));
                                    #endregion
                                    #region | TBL_Signature |
                                    DocumentFormat.OpenXml.Wordprocessing.Table lSignatureTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Signature");
                                    if (lSignatureTbl != null)
                                    {
                                        //Remove Table to be same as Template
                                        //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, lSignatureTbl, 7);
                                        if (isApprove)
                                        {
                                            XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, lSignatureTbl, 1, 1, string.Format("ประกาศ     ณ     วันที่   {0}   {1}   พ.ศ.   {2}", ApprovedDate.Day.ToString(), ApprovedDate.ToString("MMMM", _ctliTH), ApprovedDate.ToString("yyyy", _ctliTH)));
                                        }
                                        else
                                        {
                                            XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, lSignatureTbl, 1, 1, "ประกาศ     ณ     วันที่      พ.ศ.   ");
                                        }
                                        //XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, lSignatureTbl, 1, 1, string.Format("ประกาศ     ณ     วันที่   {0}   {1}   พ.ศ.   {2}", ApprovedDate.ToString("dd", _ctliTH), ApprovedDate.ToString("MMMM", _ctliTH), ApprovedDate.ToString("yyyy", _ctliTH)));
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, lSignatureTbl, 5, 1, string.Format("({0})", objApproval.EmployeeName), "centre");
                                        //XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, lSignatureTbl, 6, 1, objApproval.PositionName, "centre");
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, lSignatureTbl, 7, 1, objApproval.PositionName, "centre");
                                    }
                                    else
                                    {
                                        MessageBox(page, "TBL_Signature was missing");
                                    }
                                    #endregion
                                    break;
                                case "R":
                                    #region | TBL_Header |
                                    //Remove Table to be same as Template
                                    //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, headerTbl, 4);
                                    //Head
                                    string rDept = objDocument.Category.Equals("internal") ? string.Format("{0} ", deptInfo.Rows[0]["DEPARTMENT_NAME_TH"].ToString()) : "";
                                    XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, headerTbl, 1, 1, string.Format("ระเบียบ{0}{1}", rDept, "สถาบันการจัดการปัญญาภิวัฒน์"), "centre");
                                    //Title
                                    XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, headerTbl, 2, 1, string.Format("ว่าด้วย  {0}", objDocument.Title), "centre");
                                    #endregion
                                    #region | TBL_Signature |
                                    DocumentFormat.OpenXml.Wordprocessing.Table rSignatureTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Signature");
                                    if (rSignatureTbl != null)
                                    {
                                        //Remove Table to be same as Template
                                        //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, rSignatureTbl, 7);
                                        if (isApprove)
                                        {
                                            XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, rSignatureTbl, 1, 1, string.Format("ประกาศ     ณ     วันที่   {0}   {1}   พ.ศ.   {2}", ApprovedDate.Day.ToString(), ApprovedDate.ToString("MMMM", _ctliTH), ApprovedDate.ToString("yyyy", _ctliTH)));
                                        }
                                        else
                                        {
                                            XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, rSignatureTbl, 1, 1, "ประกาศ     ณ     วันที่      พ.ศ.   ");
                                        }
                                        //XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, rSignatureTbl, 1, 1, string.Format("ประกาศ     ณ     วันที่   {0}   {1}   พ.ศ.   {2}", ApprovedDate.ToString("dd", _ctliTH), ApprovedDate.ToString("MMMM", _ctliTH), ApprovedDate.ToString("yyyy", _ctliTH)));
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, rSignatureTbl, 5, 1, string.Format("({0})", objApproval.EmployeeName), "centre");
                                        //XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, rSignatureTbl, 6, 1, objApproval.PositionName, "centre");
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, rSignatureTbl, 7, 1, objApproval.PositionName, "centre");
                                    }
                                    else
                                    {
                                        MessageBox(page, "TBL_Signature was missing");
                                    }
                                    #endregion
                                    break;
                                case "Ex":
                                    #region | TBL_Header |
                                    //Remove Table to be same as Template
                                    //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, headerTbl, 3);

                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 1, 1, string.Format("ที่  {0}", oDocNo));
                                    //created date
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 2, 2, isApprove ? ApprovedDate.ToString("d MMMM yyyy", _ctliTH) : DateTime.Now.ToString("MMMM yyyy", _ctliTH));

                                    int exRow = 3;
                                    #region | Title |
                                    //Title
                                    //XMLFORWORD.XMLFORWORD.setNewCellWidth(wDoc, headerTbl, exRow, 1, exCellWidth);
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 3, 2, objDocument.Title);
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 4, 2, objDocument.To);
                                    #endregion
                                    //if (false)
                                    //{
                                    //    #region | To |
                                    //    if (!string.IsNullOrWhiteSpace(objDocument.To))
                                    //    {
                                    //        //To
                                    //        exRow++;
                                    //        //XMLFORWORD.XMLFORWORD.insertRow(wDoc, headerTbl, 1, 1);
                                    //        XMLFORWORD.XMLFORWORD.DuplicateRow(wDoc, headerTbl, 3);
                                    //        //XMLFORWORD.XMLFORWORD.setNewCellWidth(wDoc, headerTbl, exRow, 1, exCellWidth);
                                    //        XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, exRow, 1, string.Format("เรียน  {0}", objDocument.To));
                                    //    }
                                    //    #endregion
                                    //    #region | CC |
                                    //    if (!string.IsNullOrWhiteSpace(objDocument.CC))
                                    //    {
                                    //        //CC
                                    //        exRow++;
                                    //        //XMLFORWORD.XMLFORWORD.insertRow(wDoc, headerTbl, 1, 1);
                                    //        XMLFORWORD.XMLFORWORD.DuplicateRow(wDoc, headerTbl, 3);
                                    //        //XMLFORWORD.XMLFORWORD.setNewCellWidth(wDoc, headerTbl, exRow, 1, exCellWidth);
                                    //        XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, exRow, 1, string.Format("สำเนาถึง  {0}", objDocument.CC));
                                    //    }
                                    //    #endregion
                                    //    #region | Reference Document |
                                    //    if (referenceDocument.Length > 0)
                                    //    {
                                    //        for (int i = 0; i < referenceDocument.Length; i++)
                                    //        {
                                    //            exRow++;
                                    //            XMLFORWORD.XMLFORWORD.DuplicateRow(wDoc, headerTbl, 3);
                                    //            if (i == 0)
                                    //            {
                                    //                XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, exRow, 1, string.Format("อ้างถึง  {0}", referenceDocument[i]));
                                    //            }
                                    //            else
                                    //            {
                                    //                XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, exRow, 1, string.Format("          {0}", referenceDocument[i]));
                                    //            }
                                    //        }
                                    //    }
                                    //    //if (!string.IsNullOrWhiteSpace(objDocument.DocumentSource))
                                    //    //{
                                    //    //    //DocumentSource
                                    //    //    exRow++;
                                    //    //    //XMLFORWORD.XMLFORWORD.insertRow(wDoc, headerTbl, 1, 1);
                                    //    //    XMLFORWORD.XMLFORWORD.DuplicateRow(wDoc, headerTbl, 3);
                                    //    //    //XMLFORWORD.XMLFORWORD.setNewCellWidth(wDoc, headerTbl, exRow, 1, exCellWidth);
                                    //    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, exRow, 1, string.Format("อ้างถึง  {0}", objDocument.DocumentSource));
                                    //    //}
                                    //    #endregion
                                    //    #region | Attachment |
                                    //    if (!string.IsNullOrWhiteSpace(objDocument.Attachment))
                                    //    {
                                    //        //Attachment
                                    //        exRow++;
                                    //        //XMLFORWORD.XMLFORWORD.setNewCellWidth(wDoc, headerTbl, exRow, 1, exCellWidth);
                                    //        //XMLFORWORD.XMLFORWORD.insertRow(wDoc, headerTbl, 1, 1);
                                    //        XMLFORWORD.XMLFORWORD.DuplicateRow(wDoc, headerTbl, 3);
                                    //        XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, exRow, 1, string.Format("สิ่งที่แนบมาด้วย  {0}", objDocument.Attachment));
                                    //    }
                                    //    #endregion
                                    //}
                                    #endregion
                                    #region | TBL_Signature |
                                    DocumentFormat.OpenXml.Wordprocessing.Table exSignatureTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Signature");
                                    if (exSignatureTbl != null)
                                    {
                                        //Remove Table to be same as Template
                                        //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, exSignatureTbl, 5);
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, exSignatureTbl, 1, 1, "ขอแสดงความนับถือ", "centre");
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, exSignatureTbl, 3, 1, string.Format("({0})", objApproval.EmployeeName), "centre");
                                        //XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, exSignatureTbl, 4, 1, objApproval.PositionName, "centre");
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, exSignatureTbl, 5, 1, objApproval.PositionName, "centre");
                                    }
                                    else
                                    {
                                        MessageBox(page, "TBL_Signature was missing");
                                    }
                                    #endregion
                                    //#region | TBL_Footer |
                                    //var exFooterTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Footer");
                                    //if (exFooterTbl != null)
                                    //{
                                    //    //Remove Table to be same as Template
                                    //    XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, exFooterTbl, 4);
                                    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exFooterTbl, 1, 1, deptInfo.Rows[0]["DEPARTMENT_NAME_TH"].ToString());//objDocument.FromDepartmentName.ToString());
                                    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exFooterTbl, 2, 1, string.Format("โทรศัพท์  {0}", reqEmp.TELEPHONE.ToString().Replace('-', ' ')));
                                    //    // XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exFooterTbl, 2, 2, reqEmp.TELEPHONE.ToString());
                                    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exFooterTbl, 3, 1, "โทรสาร  0 2855 0391");
                                    //    //XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exFooterTbl, 3, 2, "0 2855 1044");
                                    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exFooterTbl, 4, 1, string.Format("Email  {0}", reqEmp.EMAIL.ToString()));
                                    //    //XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exFooterTbl, 4, 2, reqEmp.EMAIL.ToString());
                                    //}
                                    //else
                                    //{
                                    //    MessageBox(page, "TBL_Footer was missing");
                                    //}
                                    //#endregion
                                    break;
                                case "ExEN":
                                    #region | TBL_Header |
                                    //Remove Table to be same as Template
                                    XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, headerTbl, 5);

                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 1, 1, string.Format("No. {0}", oDocNo));
                                    //created date
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 3, 1, isApprove ? ApprovedDate.ToString("d MMMM yyyy", _ctliEN) : DateTime.Now.ToString("MMMM yyyy", _ctliEN));
                                    #endregion
                                    #region | TBL_Signature |
                                    DocumentFormat.OpenXml.Wordprocessing.Table exenSignatureTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Signature");
                                    if (exenSignatureTbl != null)
                                    {
                                        //Remove Table to be same as Template
                                        //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, exenSignatureTbl, 4);
                                        XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exenSignatureTbl, 1, 1, "Yours sincerely,");
                                        XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exenSignatureTbl, 3, 1, string.Format("({0})", objApproval.EmployeeName));
                                        //XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exenSignatureTbl, 4, 1, objApproval.PositionName);
                                    }
                                    else
                                    {
                                        MessageBox(page, "TBL_Signature was missing");
                                    }
                                    #endregion
                                    //#region | TBL_Footer |
                                    //var exenFooterTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Footer");
                                    //if (exenFooterTbl != null)
                                    //{
                                    //    if (string.IsNullOrWhiteSpace(reqEmp.TELEPHONE.ToString())) { reqEmp.TELEPHONE = " "; }

                                    //    //Remove Table to be same as Template
                                    //    XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, exenFooterTbl, 4);
                                    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exenFooterTbl, 1, 1, objDocument.FromDepartmentName.ToString());
                                    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exenFooterTbl, 2, 1, string.Format("Tel : +66{0}", reqEmp.TELEPHONE.ToString().Replace('-', ' ').Remove(0, 1)));
                                    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exenFooterTbl, 3, 1, string.Format("Fax : {0}", "+66 2855 0391"));
                                    //    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, exenFooterTbl, 4, 1, string.Format("Email : {0}", reqEmp.EMAIL.ToString()));
                                    //}
                                    //else
                                    //{
                                    //    MessageBox(page, "TBL_Footer was missing");
                                    //}
                                    //#endregion
                                    break;
                                case "M":
                                    #region | TBL_Header |
                                    //Remove Table to be same as Template
                                    XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, headerTbl, 4);
                                    //Head
                                    string mDept = objDocument.Category.Equals("internal") ? string.Format("{0} ", deptInfo.Rows[0]["DEPARTMENT_NAME_TH"].ToString()) : "";
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 1, 1, string.Format("บันทึกข้อความ"));
                                    //Department
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 2, 1, string.Format("หน่วยงาน  {0}", mDept));
                                    //Tel
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 2, 2, string.Format("โทรศัพท์  {0}", reqEmp.TELEPHONE));
                                    //Docno
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 3, 1, string.Format("ที่  {0}", oDocNo));
                                    //Date
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 3, 2, isApprove ? string.Format("วันที่  {0}", ApprovedDate.ToString("d MMMM yyyy", _ctliTH)) : string.Format("วันที่  {0}", ApprovedDate.ToString(" MMMM yyyy", _ctliTH)));
                                    //Title
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 4, 1, string.Format("เรื่อง  {0}", objDocument.Title));
                                    #endregion
                                    #region | TBL_Signature |
                                    DocumentFormat.OpenXml.Wordprocessing.Table mSignatureTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Signature");
                                    if (mSignatureTbl != null)
                                    {
                                        //Remove Table to be same as Template
                                        //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, mSignatureTbl, 4);
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, mSignatureTbl, 2, 1, string.Format("({0})", objApproval.EmployeeName), "centre");
                                        //XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, mSignatureTbl, 3, 1, objApproval.PositionName, "centre");
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, mSignatureTbl, 4, 1, objApproval.PositionName, "centre");
                                        if (objDocument.AutoStamp == "Y" && (objDocument.Status.Equals(_Completed) || objDocument.Status.Equals(_WaitForAdminCentre) || objDocument.Status == "LastApp"))
                                        {
                                            if (listSignature.Any(x => x.EmployeeID == objApproval.EmpID.ToString()))
                                            {
                                                string signaturePath = string.Format("{0}/Signature/{1}.gif", GetSPSite(), objApproval.EmpID);
                                                XMLFORWORD.XMLFORWORD.AddImageToTable(wDoc, mSignatureTbl, 1, 1, signaturePath);
                                            }
                                            else
                                            {
                                                XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, mSignatureTbl, 1, 1, string.Format("{0}", objApproval.EmployeeName), "centre");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox(page, "TBL_Signature was missing");
                                    }
                                    #endregion
                                    break;
                                case "Other":
                                    #region | TBL_Header |
                                    //Remove Table to be same as Template
                                    XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, headerTbl, 4);
                                    //Head
                                    string oDept = objDocument.Category.Equals("internal") ? string.Format("{0} ", deptInfo.Rows[0]["DEPARTMENT_NAME_TH"].ToString()) : "";
                                    string sDocTypeName = "";
                                    DataTable dtDocType = Extension.GetDataTable("MstDocumentType");
                                    if (!dtDocType.DataTableIsNullOrEmpty())
                                    {
                                        dtDocType = dtDocType.AsEnumerable().Where(x => x.Field<String>("Value").Equals(objDocument.OtherDocType)).ToList().CopyToDataTable();
                                        if (!dtDocType.DataTableIsNullOrEmpty())
                                        {
                                            sDocTypeName = dtDocType.Rows[0]["DocTypeName"].ToString();
                                        }
                                    }
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 1, 1, sDocTypeName);
                                    //Department
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 2, 1, string.Format("หน่วยงาน  {0}", oDept));
                                    //Tel
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 2, 2, string.Format("โทรศัพท์  {0}", reqEmp.TELEPHONE));
                                    //Docno
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 3, 1, string.Format("ที่  {0}", oDocNo));
                                    //Date
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 3, 2, isApprove ? string.Format("วันที่  {0}", ApprovedDate.ToString("d MMMM yyyy", _ctliTH)) : string.Format("วันที่  {0}", ApprovedDate.ToString(" MMMM yyyy", _ctliTH)));
                                    //Title
                                    XMLFORWORD.XMLFORWORD.bodyFillTable(wDoc, headerTbl, 4, 1, string.Format("เรื่อง  {0}", objDocument.Title));
                                    #endregion
                                    #region | TBL_Signature |
                                    DocumentFormat.OpenXml.Wordprocessing.Table oSignatureTbl = XMLFORWORD.XMLFORWORD.bodyFindTableByBookMark(wDoc, "TBL_Signature");
                                    if (oSignatureTbl != null)
                                    {
                                        //Remove Table to be same as Template
                                        //XMLFORWORD.XMLFORWORD.RemoveRowUntil(wDoc, oSignatureTbl, 4);
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, oSignatureTbl, 2, 1, string.Format("({0})", objApproval.EmployeeName), "centre");
                                        //XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, oSignatureTbl, 3, 1, objApproval.PositionName, "centre");
                                        XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, oSignatureTbl, 4, 1, objApproval.PositionName, "centre");
                                        if (objDocument.AutoStamp == "Y" && (objDocument.Status.Equals(_Completed) || objDocument.Status == "LastApp"))
                                        {
                                            if (listSignature.Any(x => x.EmployeeID == objApproval.EmpID.ToString()))
                                            {
                                                string signaturePath = string.Format("{0}/Signature/{1}.gif", GetSPSite(), objApproval.EmpID);
                                                XMLFORWORD.XMLFORWORD.AddImageToTable(wDoc, oSignatureTbl, 1, 1, signaturePath);
                                            }
                                            else
                                            {
                                                XMLFORWORD.XMLFORWORD.bodyFillTableWithTextAlign(wDoc, oSignatureTbl, 1, 1, string.Format("{0}", objApproval.EmployeeName), "centre");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox(page, "TBL_Signature was missing");
                                    }
                                    #endregion
                                    break;
                                default: break;
                            }
                            #endregion
                        }
                        else
                        {
                            MessageBox(page, "TBL_Header was missing.");
                        }


                        //Extension.LogWriter.Write(new Exception("OPENXML settingPropertyFile"));
                        XMLFORWORD.XMLFORWORD.settingPropertyFile(wDoc, "PIMEdocumentTemplate", "PIMEdocumentTemplate", "PIMUser");

                        //Extension.LogWriter.Write(new Exception("OPENXML Save"));
                        wDoc.MainDocumentPart.Document.Save();
                    }
                    return ms.ToArray();
                }
            }

            return null;
        }

        internal static DateTime? ConvertTextToDate(string txt)
        {
            DateTime? DateResult = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(txt))
                {
                    string[] spTxt = txt.Split('/');
                    int Year = int.Parse(spTxt[2]) - 543;
                    int Month = int.Parse(spTxt[1]);
                    int Day = int.Parse(spTxt[0]);
                    DateResult = new DateTime(Year, Month, Day);
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }

            return DateResult;

        }
        internal static string ConvertTextToDateFormat(string txt)
        {
            string DateResult = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(txt))
                {
                    string[] spTxt = txt.Split('/');
                    int Year = int.Parse(spTxt[2]) - 543;
                    string Month = spTxt[1];
                    string Day = spTxt[0];
                    DateResult = string.Format("{0}{1}{2}", Year.ToString(), Month, Day);
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }

            return DateResult;

        }

        public static byte[] getMSWord(string sUrl)
        {
            byte[] data = null;
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                Uri url = new Uri(sUrl);
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFile file = web.GetFile(url.AbsoluteUri);
                        data = file.OpenBinary();
                    }
                }
            });
            return data;
        }


        #region | DateTime |
        public static string ConvertToBE(this string year)
        {
            try
            {
                int sYear = Convert.ToInt32(year);
                if (sYear < 2500) { return (sYear + 543).ToString(); }
                else { return year; }
            }
            catch (Exception ex)
            {
                LogWriter.Write(ex);
            }
            return year;
        }
        public static string ConvertToAD(this string year)
        {
            try
            {
                int sYear = Convert.ToInt32(year);
                if (sYear > 2500) { return (sYear - 543).ToString(); }
                else { return year; }
            }
            catch (Exception ex)
            {
                LogWriter.Write(ex);
            }
            return year;
        }

        public static string GetTHMonth(this int month)
        {
            switch (month)
            {
                case 1:
                    return "มกราคม";
                case 2:
                    return "กุมภาพันธ์";
                case 3:
                    return "มีนาคม";
                case 4:
                    return "เมษายน";
                case 5:
                    return "พฤษภาคม";
                case 6:
                    return "มิถุนายน";
                case 7:
                    return "กรกฎาคม";
                case 8:
                    return "สิงหาคม";
                case 9:
                    return "กันยายน";
                case 10:
                    return "ตุลาคม";
                case 11:
                    return "พฤศจิกายน";
                case 12:
                    return "ธันวาคม";
                default: return "มกราคม"; break;
            }
        }
        public static string GetENMonth(this int month)
        {
            switch (month)
            {
                case 1:
                    return "January";
                case 2:
                    return "Febuary";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default: return "January"; break;
            }
        }
        #endregion
    }
}
