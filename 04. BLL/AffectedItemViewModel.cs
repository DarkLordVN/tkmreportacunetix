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
    public class AffectedItemViewModel
    {
        public int ID { get; set; }
        public int WebsiteID { get; set; }
        public int WebsiteScanID { get; set; }
        public int AlertGroupID { get; set; }
        public string ScanPath { get; set; }
        public string Detail { get; set; }
        public string SourceCode { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Status { get; set; }
        public AlertGroupViewModel vmAlertGroup { get; set; }
        public WebsiteViewModel vmWebsite { get; set; }
        public WebsiteScanViewModel vmWebsiteScan { get; set; } 
        public AffectedItemViewModel()
        {
            vmAlertGroup = new AlertGroupViewModel();
            vmWebsite = new WebsiteViewModel();
            vmWebsiteScan = new WebsiteScanViewModel();
        }
    }
    public class AffectedItemListView : PagedListBase
    {
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string TuKhoa { get; set; }
        public string PhamViTimKiem { get; set; }
        public List<AffectedItemViewModel> lstAffectedItem { get; set; }

    }
}
