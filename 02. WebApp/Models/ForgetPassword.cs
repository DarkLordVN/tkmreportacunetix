using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKM.WebApp
{
    public class ForgetPassword
    {
        [AllowHtml]
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        public string TenDangNhap { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email sai định dạng!")]
        public string Email { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất từ 6 ký tự trở lên", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Xác nhận Mật khẩu không được để trống!")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu không trùng khớp!")]
        public string XacNhanMatKhau { get; set; }
        public string ErrMess { get; set; }
    }
}