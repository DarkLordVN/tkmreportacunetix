using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.BLL
{
    public class HeThongThamSoViewModel
    {
        public int ID { get; set; }
        public string MaThamSo { get; set; }
        public string TenThamSo { get; set; }
        public string MoTa { get; set; }
        public int? NguoiCapNhatID { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }
    }

    public class HeThongThamSoListView : PagedListBase
    {
        public string TuKhoa { get; set; }
        public int ID { get; set; }
        public string MaThamSo { get; set; }
        public string TenThamSo { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public List<HeThongThamSoViewModel> LstHeThongThamSo { get; set; }
    }
}
