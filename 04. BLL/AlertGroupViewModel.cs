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
    public class AlertGroupViewModel
    {
        public int ID { get; set; }
        public string AlertName { get; set; }
        public string AlertNameTrans { get; set; }
        public string Severity { get; set; }
        public string SeverityHtml
        {
            get
            {
                var html = "";
                if (!string.IsNullOrEmpty(this.Severity))
                {
                    switch (this.Severity.ToLower())
                    {
                        case "high": html = "<a class='text-danger text-uppercase font-weight-bold'>Cao</a>"; break;
                        case "medium": html = "<a class='text-warning text-uppercase font-weight-bold'>Trung bình</a>"; break;
                        case "low": html = "<a class='text-info text-uppercase font-weight-bold'>Thấp</a>"; break;
                        case "informational": html = " <a class='text-uppercase font-weight-bold'>Thông tin</a>"; break;
                    }
                }
                return html;
            }
        }
        public string AlertDescription { get; set; }
        public string AlertDescriptionTrans { get; set; }
        public string Recommendations { get; set; }
        public string RecommendationsTrans { get; set; }
        public string AlertVariants { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Status { get; set; }

        //Them home
        public string TongWebsite { get; set; }
    }
    public class AlertGroupListView : PagedListBase
    {
        public int tab { get; set; }
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string TuKhoa { get; set; }
        public string PhamViTimKiem { get; set; }
        public List<AlertGroupViewModel> lstAlertGroup { get; set; }


    }
}
