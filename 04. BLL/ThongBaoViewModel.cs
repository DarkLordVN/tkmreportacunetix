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
    public class ThongBaoViewModel
    {
        public int ID { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public int? ChucNangID { get; set; }
        public string ChucNang { get; set; }
        public string LstNguoiNhanID { get; set; }
        public string LstNguoiNhanDaXemID { get; set; }
        public string LstNguoiNhanDaNhanMoiNhatID { get; set; }
        public string LstNguoiNhan { get; set; }
        public string LstNhomNguoiNhanID { get; set; }
        public string LstNhomNguoiNhan { get; set; }
        public int? NguoiTaoID { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NgayTaoStr { get; set; }
        public string FileDinhKem { set; get; }
        public string ChucVuVaTen { get; set; }
        public string Avatar { get; set; }
        public string TenNguoiGui { get; set; }
        public string ChucVuNguoiGui { get; set; }
        public string Link { get; set; }
        public bool IsDaGui { get; set; }
        public List<NguoiDungViewModel> lstNguoiDung { get; set; }
        public List<NhomNguoiDungViewModel> lstNhomNguoiDung { get; set; }
    }
    public class ThongBaoListView : PagedListBase
    {
        public int tab { get; set; }
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string TieuDe { get; set; }
        public int? NguoiTaoID { get; set; }
        public string NguoiTao { get; set; }
        public string TuNgayTaoTB { get; set; }
        public string DenNgayTaoTB { get; set; }
        [AllowHtml]
        [StringLength(20, ErrorMessage = "Từ khóa không được quá 20 ký tự")]
        public string TuKhoa { get; set; }
        public int? ChucNangID { get; set; }
        public string ChucNang { get; set; }
        public string PhamViTimKiem { get; set; }
        public List<ThongBaoViewModel> lstThongBao { get; set; }
        public List<NguoiDungViewModel> lstNguoiDung { get; set; }


    }
}
