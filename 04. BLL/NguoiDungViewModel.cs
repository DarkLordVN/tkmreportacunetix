using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TKM.BLL
{
    public class UserTicketAuthenViewModel
    {
        public int ID { get; set; }
        public string MaNhanVien { get; set; }
        public string TenDangNhap { get; set; }
        public string HoVaTen { get; set; }
        public string AnhDaiDien { get; set; }
        public int NhomQuyenID { get; set; }
        public int NguoiQuanLyId { get; set; }
        public int ChucVuID { get; set; }
        public int DonViID { get; set; }
        public string TypeUser { get; set; }
        public string CapBac { set; get; }
        public bool IsLanhDao { get; set; }
        public bool IsVanThu { get; set; }
        public bool IsTongHop { get; set; }
        public bool IsCuc { get; set; }
        public bool IsTruong { get; set; }
        public bool IsPho { get; set; }
        public bool IsDonVi { get; set; }
        public string TenDonVi { get; set; }

    }
    public class NguoiDungViewModel
    {
        public int ID { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        public string TenDangNhap { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất từ 6 ký tự trở lên", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất từ 6 ký tự trở lên", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string XacNhanMatKhau { get; set; }
        [Required(ErrorMessage = "Họ tên không được để trống!")]
        [AllowHtml]
        public string HoVaTen { get; set; }
        public bool GioiTinh { get; set; }
        public string AnhDaiDien { get; set; }
        [Phone(ErrorMessage = "Nhập đúng định dạng số điện thoại")]
        [AllowHtml]
        public string SoDienThoai { get; set; }
        [AllowHtml]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng")]
        [StringLength(100, ErrorMessage = "Đã nhập quá 100 ký tự")]
        public string Email { get; set; }
        public Nullable<System.DateTime> NgaySinh { get; set; }
        public string NgaySinhStr { get; set; }
        [AllowHtml]
        public string DiaChi { get; set; }
        [AllowHtml]
        public string Fax { get; set; }
        public bool IsLanhDaoCuc { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn đơn vị")]
        public int DonViID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhóm quyền")]
        public int NhomQuyenID{ get; set; }
        public string NhomQuyenList { get; set; }
        public string TenNhomQuyenList { get; set; }
        // public List<string> LstNhomQuyenSelected { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Vui lòng chọn chức vụ")]
        public int ChucVuID { get; set; }
        public bool IsAdmin { get; set; }
        [AllowHtml]
        public string GhiChu { get; set; }
        public bool TrangThai { get; set; }
        //[Required(ErrorMessage = "Số thứ tự không được để trống")]
        [StringLength(9, ErrorMessage = "Thứ tự không được vượt quá 9 ký tự")]
        public string ThuTus { get; set; }
        public int? ThuTu { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string StrNgayTao { get; set; }
        public string NguoiTao { get; set; }
        public int? NguoiTaoID { get; set; }
        public int? NguoiCapNhatID { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }

        public bool IsLanhDao { get; set; }
        public bool IsVanThu { get; set; }
        public bool IsTongHop { get; set; }
        public bool IsCuc { get; set; }
        public bool IsTruong { get; set; }
        public bool IsPho { get; set; }
        public string TypeUser { get; set; }
        public string CapBac { get; set; }
        public string ChucVuVaTen { get; set; }


        //Thông tin khác
        //[DataType(DataType.Password)]
        //[Display(Name = "Nhập lại mật khẩu")]
        //[System.ComponentModel.DataAnnotations.Compare("MatKhau", ErrorMessage = "Mật khẩu mới và nhập lại mật khẩu mới không trùng nhau.")]
        //public bool CheckTrangThai { get { return (TrangThai.HasValue && TrangThai == 2) ? false : true; } set { TrangThai = value ? 1 : 2; } }
        public List<DmDonViViewModel> LstDonVi { get; set; }
        public List<NhomQuyenViewModel> LstNhomQuyen { get; set; }
        public List<NguoiDungDetailListView> LstNguoiQuanLy { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public string TenChucVu { get; set; }
        public string TenDonVi { get; set; }
        public int KhoaChaIDDonVi { get; set; }
        public string TenNhomQuyen { get; set; }
        public string SoLuongThongBao { get; set; }
        public string DonViHoTen { get; set; }
    }


    public class NguoiDungTheoDonViViewModel
    {
        public int DonViID { get; set; }
        public int KhoaChaID { get; set; }
        public string TenDonVi { get; set; }
        public List<NguoiDungTheoDonViViewModel> LstDonViTrucThuoc { get; set; }
        public List<NguoiDungGeneralViewModel> LstNguoiDungPhanCap { get; set; }
    }

    public class NguoiDungGeneralViewModel
    {
        public int ID { get; set; }
        public string HoVaTen { get; set; }
        public string TenChucVu { get; set; }
        public string ChucVuVaTen { get; set; }
    }

    public class NguoiDungListView : PagedListBase
    {
        [AllowHtml]
        public string TuKhoa { get; set; }
        [AllowHtml]
        public string HoVaTen { get; set; }
        public int? DonViID { get; set; }
        public int? ChucVuID { get; set; }
        public int NhomQuyenID { get; set; }
        public bool? TrangThai { get; set; }
        public int? srTrangThai { get; set; }
        public List<NguoiDungDetailListView> LstNguoiDung { get; set; }
        public List<NguoiDungViewModel> DsNguoiDung { get; set; }

        public List<DmDonViViewModel> LstDonVi { get; set; }
    }

    public class NguoiDungDetailListView
    {
        public int ID { get; set; }
        public string TenDangNhap { get; set; }
        public string HoVaTen { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Fax { get; set; }
        public string DiDong { get; set; }
        public string Email { get; set; }
        public string TenDonVi { get; set; }
        public string TenChucVu { get; set; }
        public bool TrangThai { get; set; }
        public string TenNhomQuyen { get; set; }
        public string MaNhanVien { get; set; }
        public string AnhDaiDien { get; set; }
        public int? DonViID { get; set; }
        public int? ChucVuID { get; set; }
        public int? NhomQuyenID { get; set; }
    }

    public class ChangePassViewModel
    {
        public int ID { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string MatKhauMoi { get; set; }
        public string NhapLaiMatKhau { get; set; }
    }

    public class NguoiDungContact
    {
        public string ID { get; set; }
        public string MaNhanVien { get; set; }
        public string HoVaTen { get; set; }
        public string AnhDaiDien { get; set; }
        public string GioiTinh { get; set; }
        public string TenChucVu { get; set; }
        public string ChucVuID { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Fax { get; set; }
        public string DiDong { get; set; }
        public string Email { get; set; }
        public string NgaySinh { get; set; }
        public string DonViID { get; set; }
        public string TenDonVi { get; set; }
    }

    public class NguoiDungCoBanViewModel
    {
        public int ID { get; set; }
        public string TenChucVu { get; set; }
        public string HoVaTen { get; set; }
        public string TenDonVi { get; set; }
    }
}
