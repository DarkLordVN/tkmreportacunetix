using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TKM.BLL
{
    public class DmDonViViewModel
    {
        public int ID { get; set; }
        public int? KhoaChaID { get; set; }
        [AllowHtml]
        [StringLength(13, ErrorMessage = "Đã nhập quá 13 ký tự")]
        //[Range(12,13,ErrorMessage = "Mã định danh phải có 13 ký tự")]
        public string MaDinhDanh { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Ký hiệu không được để trống!")]
        [StringLength(50, ErrorMessage = "Đã nhập quá 50 ký tự")]
        public string KyHieu { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Tên đơn vị không được để trống!")]
        [StringLength(250, ErrorMessage = "Đã nhập quá 250 ký tự")]
        public string TenDonVi { get; set; }
        [StringLength(250, ErrorMessage = "Đã nhập quá 250 ký tự")]
        [AllowHtml]
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public int? NguoiTaoID { get; set; }
        public int? NguoiCapNhatID { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        //[Required(ErrorMessage = "Số thứ tự không được để trống")]
       
        [StringLength(9, ErrorMessage = "Thứ tự không được vượt quá 9 ký tự")]
        public string ThuTus { get; set; }
        public int? ThuTu { get; set; }
        public bool TrangThai { get; set; }
        public bool IsDonVi { get; set; }
        public string TenVaKyHieu { get; set; }
        public string ListCanBoXuLyVanBanDenID { get; set; }
        public string ListCanBoXuLyVanBanDen { get; set; }
        public string ListCanBoXuLyHoSoCongViecID { get; set; }
        public string ListCanBoXuLyHoSoCongViec { get; set; }
        public List<DmDonViViewModel> CbDonVi { get; set; }
        public int Level { set; get; }
    }

    public class DmDonViListView : PagedListBase
    {
        public int? ID { get; set; }
        [AllowHtml]
        public string TuKhoa { get; set; }
        [AllowHtml]
        public string MaDinhDanh { get; set; }
        [AllowHtml]
        public string KyHieu { get; set; }
        public string TenDonVi { get; set; }
        public string DonViCha { get; set; }

        public bool? TrangThai { get; set; }
        public int? srTrangThai { get; set; }
        public int Level { set; get; }

        public List<DmDonViDetailListView> LstDonVi { get; set; }
    }

    public class DmDonViDetailListView
    {
        public int ID { get; set; }
        public string MaDinhDanh { get; set; }
        public string KyHieu { get; set; }
        public string TenDonVi { get; set; }
        public string DonViCha { get; set; }
        public int ThuTu { get; set; }
        public string GhiChu { get; set; }
        public int Level { set; get; }

        public bool TrangThai { get; set; }
        public string TenTrangThai { get; set; }
    }

}
