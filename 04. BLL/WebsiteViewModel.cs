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
    public class WebsiteViewModel
    {
        public int ID { get; set; }
        public string StartURL { get; set; }
        public string Host { get; set; }
        public string Responsive { get; set; }
        public string ServerOS { get; set; }
        public string ServerInformation { get; set; }
        public string ServerTechnologies { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Status { get; set; }

        public List<WebsiteScanViewModel> LsWebsiteScan { get; set; }

        //Them vao home
        public string TongNguyCoCao { get; set; }
        public string TongNguyCoTrungBinh { get; set; }
    }
    public class WebsiteListView : PagedListBase
    {
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string TuKhoa { get; set; }
        public string PhamViTimKiem { get; set; }
        public List<WebsiteViewModel> lstWebsite { get; set; }

    }
}
