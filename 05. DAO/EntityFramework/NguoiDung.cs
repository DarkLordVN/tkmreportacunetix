//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TKM.DAO.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class NguoiDung
    {
        public int ID { get; set; }
        public string HoVaTen { get; set; }
        public Nullable<System.DateTime> NgaySinh { get; set; }
        public Nullable<bool> GioiTinh { get; set; }
        public Nullable<int> NhomQuyenID { get; set; }
        public string NhomQuyenList { get; set; }
        public Nullable<int> ChucVuID { get; set; }
        public Nullable<int> DonViID { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string AnhDaiDien { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Fax { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> NguoiTaoID { get; set; }
        public Nullable<int> NguoiCapNhatID { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public Nullable<int> ThuTu { get; set; }
        public bool TrangThai { get; set; }
        public bool IsDeleted { get; set; }
    }
}
