using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.BLL
{
    public class NhatKyHeThongViewModel
    {
        public int ID { get; set; }
        public int NguoiDungId { get; set; }
        public string TenNguoiDung { get; set; }
        public string IpClient { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayTao { get; set; }
    }
    public class NhatKyHeThongListView : PagedListBase
    {
        public string TuKhoa { get; set; }
        public int? NguoiDungID { get; set; }
        public string IpClient { get; set; }
        public string MoTa { get; set; }
        public string TuNgayNT { get; set; }
        public string DenNgayNT { get; set; }
        public string TenNhatKyHeThong { get; set; }
        public List<NguoiDungViewModel> lstNguoiDung { get; set; }
        public List<NhatKyHeThongViewModel> LstNhatKyHeThong { get; set; }
    }
}
