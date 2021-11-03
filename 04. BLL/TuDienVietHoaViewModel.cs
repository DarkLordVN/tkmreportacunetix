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
    public class TuDienVietHoaViewModel
    {
        public int ID { get; set; }
        public string NgonNgu { get; set; }
        public string CumTuKhoa { get; set; }
        public string CumTuThayDoi { get; set; }
        public int NguoiTaoID { get; set; }
        public int NguoiCapNhatID { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool TrangThai { get; set; }
    }
    public class TuDienVietHoaListView : PagedListBase
    {
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string TuKhoa { get; set; }
        public string PhamViTimKiem { get; set; }
        public List<TuDienVietHoaViewModel> lstTuDienVietHoa { get; set; }

    }
}
