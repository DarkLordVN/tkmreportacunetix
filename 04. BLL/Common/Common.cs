using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.BLL
{
    public class PagedListBase
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int TotalItem { get; set; }
        
        public string ColumnName { get; set; }
        public string OrderBy { get; set; }

        public bool? HasChart { get; set; }
        public bool? HasTableData { get; set; }
    }

    public class CommonDropDownList
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public string ValueStr { get; set; }
    }

    public class ButtonViewModel
    {
        public bool IsAdd { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsGopY { get; set; }
        public bool IsDuyet { get; set; }
        public bool IsKySo { get; set; }
    }
    
}
