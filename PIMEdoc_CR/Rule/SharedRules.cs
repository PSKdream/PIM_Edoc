using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Mobile.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace PIMEdoc_CR.Default.Rule
{
    public class SharedRules
    {
        private string _SiteCollectionURL = Extension.GetSPSite();
        public static DataTable GetList(string DocLibName, string CAMLQuery)
        {
            try
            {
                using (SPSite oSite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        // Get data from a list.
                        SPList oSpList = oWeb.Lists[DocLibName];
                        SPQuery oSpQuery = new SPQuery();
                        oSpQuery.Query = CAMLQuery;
                        SPListItemCollection collListItems = oSpList.GetItems(oSpQuery);
                        DataTable table = collListItems.GetDataTable();
                        if (!table.DataTableIsNullOrEmpty())
                        {
                            return table;
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string GetDocTypeName(string sDocTypeCode)
        {
            if (!string.IsNullOrWhiteSpace(sDocTypeCode))
            {
                DataTable dt = GetList("MstDocumentType", string.Format("<Where><And><Eq><FieldRef Name='Value' /><Value Type='Text'>{0}</Value></Eq><Eq><FieldRef Name='IsActive' /><Value Type='Boolean'>1</Value></Eq></And></Where>", sDocTypeCode));
                if (!dt.DataTableIsNullOrEmpty())
                {
                    return dt.Rows[0]["DocTypeName"].ToString();
                }
            }
            return "";
        }

        public static string FindUserNameByID(string id, Page page)
        {
            string Result = string.Empty;
            try
            {
                DataTable dtEmp = Extension.GetEmployeeData(page).Copy();
                if (!dtEmp.DataTableIsNullOrEmpty())
                {
                    DataView dv = dtEmp.DefaultView;
                    dv.RowFilter = string.Format("EMPLOYEEID = '{0}'", id);

                    DataTable dtResult = dv.ToTable();
                    if (!dtResult.DataTableIsNullOrEmpty())
                    {
                        Result = dtResult.Rows[0]["USERNAME"].ToString();
                    }
                }
                return Result;

            }
            catch (Exception ex)
            {
                return Result;
            }
        }
        public static string FindUserID(string logonName, Page page)
        {
            string Result = string.Empty;
            try
            {
                string[] user = logonName.Split('\\');
                if (user.Length != 2) { }
                else { logonName = user[1]; }
                DataTable dtEmp = Extension.GetEmployeeData(page).Copy();
                if (!dtEmp.DataTableIsNullOrEmpty())
                {
                    DataView dv = dtEmp.DefaultView;
                    dv.RowFilter = string.Format("USERNAME = '{0}'", logonName);

                    DataTable dtResult = dv.ToTable();
                    if (!dtResult.DataTableIsNullOrEmpty())
                    {
                        Result = dtResult.Rows[0]["EMPLOYEEID"].ToString();
                    }
                }
                return Result;

            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return Result;
            }
        }
        public static string ConvertLoginNameToUserID(string logonName, Page page)
        {
            string Result = string.Empty;
            try
            {
                DataTable dtEmp = Extension.GetEmployeeData(page).Copy();
                if (!dtEmp.DataTableIsNullOrEmpty())
                {
                    DataView dv = dtEmp.DefaultView;
                    dv.RowFilter = string.Format("USERNAME = '{0}'", logonName);

                    DataTable dtResult = dv.ToTable();
                    if (!dtResult.DataTableIsNullOrEmpty())
                    {
                        Result = dtResult.Rows[0]["EMPLOYEEID"].ToString();
                    }
                }
                return Result;

            }
            catch (Exception ex)
            {
                return Result;
            }
        }



        public static DirectoryEntry GetDirectoryEntry(string path, string username, string password)
        {
            try
            {
                DirectoryEntry direct_entry = new DirectoryEntry(path, username, password);//กำหนด LDAP Server , username,password
                direct_entry.AuthenticationType = AuthenticationTypes.Secure;
                return direct_entry;
            }
            catch (Exception ex)
            {
                //WriteLog(ex);
                return null;
            }

        }
        public static string LogonName()
        {
            if (string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) { return string.Empty; }
            string[] user = HttpContext.Current.User.Identity.Name.Split('\\');
            if (user.Length != 2) { return string.Empty; }
            return user[1];
        }
        public static byte[] GetSPFile(string path)
        {
            try
            {

                byte[] data = null;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    Uri url = new Uri(path);
                    using (SPSite site = new SPSite(Extension.GetSPSite()))
                    {
                        using (SPWeb oSPweb = site.OpenWeb())
                        {
                            oSPweb.AllowUnsafeUpdates = true;

                            SPFile file = oSPweb.GetFile(url.AbsoluteUri);
                            data = file.OpenBinary();

                            oSPweb.AllowUnsafeUpdates = false;
                        }
                    }
                });
                return data;
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                throw ex;
            }
        }

        #region "DocumentLibrary"
        public static SPFieldLookupValueCollection GetLookFieldIDS(ClientContext context, string lookupValues, string sLookupFieldName, List lookupSourceList)
        {
            SPFieldLookupValueCollection lookupIds = new SPFieldLookupValueCollection();
            string[] lookups = lookupValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] lookupsFieldName = sLookupFieldName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lookups.Length; i++)
            {
                CamlQuery query = new CamlQuery();
                query.ViewXml = string.Format("<View><Query><Where><Eq><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></Eq></Where></Query></View>", lookupsFieldName[i], lookups[i]);
                Microsoft.SharePoint.Client.ListItemCollection listItems = lookupSourceList.GetItems(query);
                context.Load(lookupSourceList);
                context.Load(listItems);
                context.ExecuteQuery();
                foreach (Microsoft.SharePoint.Client.ListItem item in listItems)
                {
                    SPFieldLookupValue value = new SPFieldLookupValue(Convert.ToInt32(item["ID"]), item[lookupsFieldName[i]].ToString());
                    lookupIds.Add(value);
                    break;
                }
            }
            return lookupIds;
        }
        public static string UploadFileIntoDocumentSet(string DocLibName, string sDocumentSetName, string sFileName, Stream documentStream, string fileDisc, string CreatedBy)
        {
            try
            {
                ////Remove Invalid FileName
                sDocumentSetName = sDocumentSetName.Replace("/", "_");

                Regex rgx = new Regex("[~#%&*{}\\:<>?/+|]");
                sFileName = rgx.Replace(sFileName, "-");
                using (SPSite oSite = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        oWeb.AllowUnsafeUpdates = true;
                        if (documentStream == null)
                            throw new FileNotFoundException("File not found.", sFileName);

                        SPFolder myLibrary = oWeb.Folders[DocLibName].SubFolders[sDocumentSetName];
                        //SPFolder myLibrary = oWeb.Folders[DocLibName];
                        // Prepare to upload
                        Boolean replaceExistingFiles = true;

                        // Upload document
                        SPFile spfile = myLibrary.Files.Add(sFileName, documentStream, replaceExistingFiles);

                        // Commit 
                        myLibrary.Update();
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteDocumentByURL(string sDocumentLib, string sDocumentSetName, string sFileURL)
        {
            try
            {
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        SPFolder folder = web.GetFolder(string.Format("{0}/{1}/{2}", site.Url, sDocumentLib, sDocumentSetName));

                        SPFile file = folder.Files[sFileURL];

                        SPListItem item = file.Item;

                        item.Delete();
                        return "";
                    }

                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return ex.Message.ToString();
            }

        }
        public static string CreateDocumentSet(string DocLibName, string sDocumentSetName, string[,] sMetaData)
        {
            try
            {
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.RootWeb)
                    {
                        SPList list = web.Lists[DocLibName];
                        if (list != null)
                        {
                            CreateDocumentSet(list, sDocumentSetName, sMetaData);
                        }
                        return "";
                    }

                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return ex.Message.ToString();
            }
        }
        public static string UploadFileIntoDocumentLibrary(string DocLibName, string sFileName, Stream documentStream)
        {
            try
            {
                using (SPSite oSite = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        if (documentStream == null)
                            throw new FileNotFoundException("File not found.", sFileName);

                        SPFolder myLibrary = oWeb.Folders[DocLibName];
                        //SPFolder myLibrary = oWeb.Folders[DocLibName];
                        // Prepare to upload
                        Boolean replaceExistingFiles = true;

                        // Upload document
                        SPFile spfile = myLibrary.Files.Add(sFileName, documentStream, replaceExistingFiles);

                        // Commit 
                        myLibrary.Update();
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return ex.ToString();
            }
        }
        public static string RenameFile(string sDocumentLibrary, string sDocumentSet, string sFileName, string sNewName)
        {
            try
            {
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFolder srcFolder = web.GetFolder(string.Format("{0}/{1}/{2}", site.Url, sDocumentLibrary, sDocumentSet));

                        for (int i = 0; i < srcFolder.Files.Count; i++)
                        {
                            if (srcFolder.Files[i].Name == sFileName)
                            {
                                srcFolder.Files[i].MoveTo(string.Format("{0}/{1}/{2}/{3}", site.Url, sDocumentLibrary, sDocumentSet, sNewName));
                            }
                        }
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return ex.Message;
            }
        }
        public static string CopyFileFromListToDocSet(string srcList, string srcFileName, string destDocLib, string destDocSet)
        {
            try
            {
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFolder srcFolder = web.GetFolder(string.Format("{0}/{1}/", site.Url, srcList, srcFileName));
                        SPFolder destFolder = web.GetFolder(string.Format("{0}/{1}/{2}", site.Url, destDocLib, destDocSet));
                        List<SPFile> spFile = new List<SPFile>();
                        foreach (SPFile file in srcFolder.Files)
                        {
                            if (file.Name.Equals(srcFileName))
                            {
                                spFile.Add(file);
                            }
                        }
                        foreach (SPFile file in spFile)
                        {
                            file.CopyTo(string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), destDocLib, destDocSet, file.Name), true);
                        }
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return ex.Message;
            }
        }
        public static string GetDocSetFilePathIfExist(string srcDocLib, string srcDocSet, string srcFileName)
        {
            try
            {
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFolder srcFolder = web.GetFolder(string.Format("{0}/{1}/{2}", site.Url, srcDocLib, srcDocSet));
                        List<SPFile> spFile = new List<SPFile>();
                        foreach (SPFile file in srcFolder.Files)
                        {
                            if (file.Name.Equals(srcFileName))
                            {
                                return string.Format("{0}/{1}/{2}/{3}", site.Url, srcDocLib, srcDocSet, srcFileName);
                            }
                        }
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                return "";
            }
        }
        public static string CopyFileToAnotherDocSet(string srcDocLib, string srcDocSet, string destDocLib, string destDocSet)
        {
            try
            {
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFolder srcFolder = web.GetFolder(string.Format("{0}/{1}/{2}", site.Url, srcDocLib, srcDocSet));
                        SPFolder destFolder = web.GetFolder(string.Format("{0}/{1}/{2}", site.Url, destDocLib, destDocSet));
                        List<SPFile> spFile = new List<SPFile>();
                        foreach (SPFile file in srcFolder.Files)
                        {
                            spFile.Add(file);
                        }
                        foreach (SPFile file in spFile)
                        {
                            //file.MoveTo(string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), destDocLib, destDocSet, file.Name), true);
                            file.CopyTo(string.Format("{0}/{1}/{2}/{3}", Extension.GetSPSite(), destDocLib, destDocSet, file.Name), true);
                        }
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                throw ex;
            }
        }
        public static string DeleteDocSet(string DocLib, string DocSet)
        {
            try
            {
                using (SPSite site = new SPSite(Extension.GetSPSite()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFolder srcFolder = web.GetFolder(string.Format("{0}/{1}/{2}",site.Url,DocLib,DocSet));
                        srcFolder.Delete();

                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                throw ex;
            }
        }
        private static DocumentSet CreateDocumentSet(SPList list, string DocumentSetName, string[,] sMetaData)
        {
            DocumentSetName = DocumentSetName.Replace("/", "_");
            SPContentType docsetCT = list.ContentTypes["Document Set"];
            Hashtable properties = new Hashtable();
            if (sMetaData != null)
            {
                for (int i = 0; i < sMetaData.GetLength(0); i++)
                {
                    properties.Add(sMetaData[i, 0], sMetaData[i, 1]);
                }
            }
            SPFolder parentFolder = list.RootFolder;
            DocumentSet docSet = DocumentSet.Create(parentFolder, DocumentSetName, docsetCT.Id, properties, true);
            return docSet;

        }
        public static byte[] GetByteDataFromSharepoint(string UrlSharepoint, string URLFile)
        {
            string sFilename = string.Empty;
            byte[] byteArray = null;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite oSPsite = new SPSite(UrlSharepoint))
                    {
                        using (SPWeb oSPweb = oSPsite.OpenWeb())
                        {
                            oSPweb.AllowUnsafeUpdates = true;

                            SPFile SPTemplatefile = oSPweb.GetFile(URLFile);
                            if (SPTemplatefile.Exists)
                            {
                                sFilename = SPTemplatefile.Name;
                                byteArray = SPTemplatefile.OpenBinary();
                            }
                            oSPweb.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
                throw ex;
            }
            return byteArray;
        }
        #endregion

        #region "SharePoint Permission"
        public static List<string> GetAllUserInGroup(string sGroupName)
        {
            List<string> LstUserInGroup = new List<string>();
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        SPGroup group = oSPWeb.Groups.GetByName(sGroupName);

                        foreach (SPUser user in group.Users)
                        {
                            LstUserInGroup.Add(user.LoginName);
                        }
                    }
                }
            });
            return LstUserInGroup;
        }
        #endregion
    }

}
