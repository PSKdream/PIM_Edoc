using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMEdoc_CR.Rule
{
    [Serializable()]
    class DocumentLibraryGV
    {
        [Serializable()]
        public class RootObject
        {
            public List<RESULT> RESULT { get; set; }
        }

        [Serializable()]
        public class RESULT {
            public int DocID { get; set; }
            public string DocNo { get; set; }
            public string Title { get; set; }
            public string OtherDocType { get; set; }
            public string FromDepartmentName { get; set; }
            public string CreatedDate { get; set; }
            public string DocumentFile { get; set; }
            public string Creator { get; set; }
            public string Status { get; set; }
            public string Description { get; set; }
        }
    }
}
