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
    public class ScanViewModel
    {
        public int ID { get; set; }
        public string RootReportUrl { get; set; }
        public string NewReportUrl { get; set; }
        public string RootDetailHtml { get; set; }
        public string NewDetailHtml { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Status { get; set; }
    }
    public class ScanListView : PagedListBase
    {
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string TuKhoa { get; set; }
        public string PhamViTimKiem { get; set; }
        public List<ScanViewModel> lstScan { get; set; }

    }
}
