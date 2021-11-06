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
    public class WebsiteScanViewModel
    {
        public WebsiteScanViewModel()
        {
            vmWebsite = new WebsiteViewModel();
            if (LsAlertGroup == null)
                LsAlertGroup = new List<AlertGroupViewModel>();
            if (LsAffectedItem == null)
                LsAffectedItem = new List<AffectedItemViewModel>();
            if (LsWebsiteItem == null)
                LsWebsiteItem = new List<WebsiteItemViewModel>();
            if (LsScanItem == null)
                LsScanItem = new List<ScannedItemViewModel>();
        }

        public int ID { get; set; }
        public int ScanID { get; set; }
        public int WebsiteID { get; set; }
        public DateTime StartTime { get; set; }
        public string ScanTime { get; set; }
        public string ScanTimeTrans
        {
            get
            {
                if (!string.IsNullOrEmpty(ScanTime))
                {
                    return ScanTime.Replace("hours", "giờ").Replace("hour", "giờ").Replace("minutes","phút").Replace("minute", "phút").Replace("seconds","giây").Replace("second", "giây");
                }
                return "";
            }
        }
        public string ThreatLevel { get; set; }
        public int TotalSecond { get; set; }
        public int TotalItemScan { get; set; }
        public int TotalAlertFound { get; set; }
        public int HighAlert { get; set; }
        public int MediumAlert { get; set; }
        public int LowAlert { get; set; }
        public int InforAlert { get; set; }
        public string ScanProfile { get; set; }
        public string ScanStatus { get; set; }
        public string RootReportUrl { get; set; }
        public string NewReportUrl { get; set; }
        public string ReportHTML { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Status { get; set; }
        public WebsiteViewModel vmWebsite { get; set; }
        public List<AlertGroupViewModel> LsAlertGroup { get; set; }
        public List<AffectedItemViewModel> LsAffectedItem { get; set; }
        public List<WebsiteItemViewModel> LsWebsiteItem { get; set; }
        public List<ScannedItemViewModel> LsScanItem { get; set; }
    }
    public class WebsiteScanListView : PagedListBase
    {
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string WebsiteID { get; set; }
        public string TuKhoa { get; set; }
        public List<WebsiteScanViewModel> lstWebsiteScan { get; set; }

    }
}
