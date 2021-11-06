using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TKM.DAO.EntityFramework;

namespace TKM.BLL
{
    public class ScannedItemViewModel
    {
        public int ID { get; set; }
        public int WebsiteID { get; set; }
        public int WebsiteScanID { get; set; }
        public int WebsiteItemID { get; set; }
        public string FullPath { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Status { get; set; }
    }
    public class ScannedItemListView : PagedListBase
    {
        public int tab { get; set; }
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string TuKhoa { get; set; }
        public string PhamViTimKiem { get; set; }
        public int WebsiteScanID { get; set; }
        public List<ScannedItemViewModel> lstScannedItem { get; set; }
        public List<NguoiDungViewModel> lstNguoiDung { get; set; }


    }
}
