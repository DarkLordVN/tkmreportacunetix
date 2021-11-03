using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TKM.BLL
{
    public class NhomQuyenViewModel
    {
        public int ID { get; set; }
        [AllowHtml]
        [StringLength(50, ErrorMessage = "Đã nhập quá 50 ký tự")]
        public string KyHieu { get; set; }
        public string MaNhom { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Tên nhóm quyền không được để trống")]
        [StringLength(250, ErrorMessage = "Đã nhập quá 250 ký tự")]
        public string TenNhom { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public int? NguoiTaoID { get; set; }
        public int? NguoiCapNhatID { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
       
        [Required(ErrorMessage = "Số thứ tự không được để trống")]
        [StringLength(9, ErrorMessage = "Thứ tự không được vượt quá 9 ký tự")]
        public string ThuTus { get; set; }
        public int? ThuTu { get; set; }
        public bool TrangThai { get; set; }
        //public bool BoolTrangThai { get { return (TrangThai.HasValue && TrangThai == 2) ? false : true; } set { TrangThai = value ? 1 : 2; } }
        public List<QuyenDetailViewModel> LstQuyen { get; set; }
        public List<int> LstQuyenIdChecked { get; set; }
    }
    public class NhomQuyenListView : PagedListBase
    {
        [AllowHtml]
        public string TuKhoa { get; set; }
        [AllowHtml]
        public string KyHieu { get; set; }
        [AllowHtml]
        public string TenNhom { get; set; }
        public bool? TrangThai { get; set; }
        public int? srTrangThai { get; set; }
        public List<NhomQuyenViewModel> LstNhomQuyen { get; set; }
    }
}

