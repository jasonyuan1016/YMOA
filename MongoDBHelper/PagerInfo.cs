using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBHelper
{
    public class PagerInfo
    {
        public int CurrenetPageIndex { get; set; }

        public int PageSize { get; set; }

        public int RecordCount { get; set; }


    }
}
