using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TKM.WebApp
{
    public class LoginModel : BaseModel
    {
        [Required(ErrorMessage = "Anh/Chị chưa nhập tài khoản")]
        public string TenDangNhap { get; set; }
        [Required(ErrorMessage = "Anh/Chị chưa nhập mật khẩu")]
        public string MatKhau { get; set; }
        //[Required(ErrorMessage = "Anh/Chị chưa nhập mã bảo mật")]
        //public string txtimgcode { get; set; }
        public string Capcha { get; set; }
        public string RedirectUrl { get; set; }
    }
}