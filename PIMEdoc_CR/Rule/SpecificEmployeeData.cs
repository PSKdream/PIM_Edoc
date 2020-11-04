using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMEdoc_CR.Default.Rule
{
    public class SpecificEmployeeData
    {
        public class RootObject
        {
            public string EMPLOYEEID { get; set; }
            public string USERNAME { get; set; }
            public string PREFIX_TH { get; set; }
            public string PREFIX_ACADEMIC_TH { get; set; }
            public string FIRSTNAME_TH { get; set; }
            public string LASTNAME_TH { get; set; }
            public string PREFIX_EN { get; set; }
            public string PREFIX_ACADEMIC_EN { get; set; }
            public string FIRSTNAME_EN { get; set; }
            public string LASTNAME_EN { get; set; }
            public string BIRTHDATE { get; set; }
            public string EMAIL { get; set; }
            public string TELEPHONE { get; set; }
            public string MOBILE { get; set; }
            public string MANAGERID { get; set; }
            public string MANAGERUSERNAME { get; set; }
            public string MODIFIED_BY { get; set; }
            public string MODIFIED_DATETIME { get; set; }
            public List<RESULT> RESULT { get; set; }
        }
        public class RESULT
        {
            public string DEPARTMENT_ID { get; set; }
            public string DEPARTMENT_NAME_TH { get; set; }
            public string DEPARTMENT_NAME_EN { get; set; }
            public string SUBDEPARTMENT_ID { get; set; }
            public string SUBDEPARTMENT_NAME_TH { get; set; }
            public string SUBDEPARTMENT_NAME_EN { get; set; }
            public string POSITION_TD { get; set; }
            public string POSTION_NAME_TH { get; set; }
            public string POSTION_NAME_EN { get; set; }
            public string PRIMARY_POSITION { get; set; }
        }
    }
}
