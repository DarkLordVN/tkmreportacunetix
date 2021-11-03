using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TKM.BLL
{
    public class QuyenViewModel
    {
        public int ID { get; set; }
        public Nullable<int> KhoaChaId { get; set; }
        public string MaQuyen { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Tên quyền không được để trống")]
        [StringLength(250, ErrorMessage = "Đã nhập quá 250 ký tự")]
        public string TenQuyen { get; set; }
        public Nullable<bool> NhanQuyen { get; set; }
        public string TenHienThi { get; set; }
        public string Controller { get; set; }
        [AllowHtml]
        [StringLength(50, ErrorMessage = "Đã nhập quá 50 ký tự")]
        public string IconPath { get; set; }
        [AllowHtml]
        [StringLength(250, ErrorMessage = "Đã nhập quá 250 ký tự")]
        public string GhiChu { get; set; }
        public string Action { get; set; }
        public string ActionCustom { get; set; }
        public string FontAwesome { get; set; }
        public bool IsMenu { get; set; }
       
        //[Required(ErrorMessage = "Số thứ tự không được để trống")]
        [StringLength(9, ErrorMessage = "Thứ tự không được vượt quá 9 ký tự")]
        public string ThuTus { get; set; }
        public int? ThuTu { get; set; }
        public Nullable<byte> PhanHe { get; set; }
        public bool TrangThai { get; set; }
        //public bool CheckTrangThai { get { return (TrangThai.HasValue && TrangThai == 2) ? false : true; } set { TrangThai = value ? 1 : 2; } }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public int? NguoiTaoID { get; set; }
        public int? NguoiCapNhatID { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }

        public List<QuyenDetailViewModel> LstQuyenCha { get; set; }
        public List<string> LstController { get; set; }
        public List<string> LstAction { get; set; }
        public int Level { set; get; }
    }
    public class QuyenListView : PagedListBase
    {
        [AllowHtml]
        public string TuKhoa { get; set; }
        [AllowHtml]
        public string TenQuyen { get; set; }
        [AllowHtml]
        public string MaQuyen { get; set; }
        public bool? TrangThai { get; set; }
        public int? srTrangThai { get; set; }
        public List<QuyenDetailViewModel> LstQuyen { get; set; }
        public int Level { set; get; }
    }

    public class QuyenDetailViewModel
    {
        public int ID { get; set; }
        public Nullable<int> KhoaChaID { get; set; }
        public string MaQuyen { get; set; }
        public string TenQuyen { get; set; }
        public int DataLevel { get; set; }
        public bool TrangThai { get; set; }
        public bool HienThiMenu { get; set; }
        public int? ThuTu { get; set; }
        public string PhanCapStype { get; set; }
        public bool Checked { get; set; }
        public bool HasChild { get; set; }
        public string FontAwesome { get; set; }

        public int Level { set; get; }
    }
}
