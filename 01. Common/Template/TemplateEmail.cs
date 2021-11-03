using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Utils.Template
{
    public class TemplateEmail
    {
        public static string TemplateFogetPass(string name, string username, string pass)
        {
            var str = new StringBuilder();
            str.AppendFormat("<p>Kính gửi ông/ bà: <b>{0}</b>,{1}</p>", name, Environment.NewLine);
            str.AppendFormat("<p>Ông/ bà được cấp lại mật khẩu đăng nhập Hệ thống quản lý văn bản điều hành. {0}</p>", Environment.NewLine);
            str.AppendFormat("<p>Ông/ bà vui lòng đổi mật khẩu ngay lần đầu tiên để đảm bảo bảo mật tài khoản.{0}</p>", Environment.NewLine);
            str.AppendFormat("<p>Thông tin tài khoản đăng nhập phần mềm như sau.{0}</p>", Environment.NewLine);
            str.AppendFormat("<p>Tài khoản: <b>{0}</b>{1}</p>", username, Environment.NewLine);
            str.AppendFormat("<p>Mật khẩu: <b>{0}</b>{1}</p>", pass, Environment.NewLine);
            str.AppendFormat("<p>Trân trọng.{0}</p>", Environment.NewLine);
            str.AppendFormat("<p><i>Đây là Email gửi đăng ký tự động, vui lòng không reply. Xin cảm ơn!</i></p>");
            return str.ToString();
        }

        public static string TemplateAddAccount(string name, string username, string pass)
        {
            var str = new StringBuilder();
            str.AppendFormat("<p>Kính gửi ông/ bà: <b>{0}</b>,{1}</p>", name, Environment.NewLine);
            str.AppendFormat("<p>Ông/ bà được cấp tài khoản đăng nhập Hệ thống quản lý văn bản điều hành. {0}</p>", Environment.NewLine);
            str.AppendFormat("<p>Ông/ bà vui lòng đổi mật khẩu ngay lần đầu tiên để đảm bảo bảo mật tài khoản.{0}</p>", Environment.NewLine);
            str.AppendFormat("<p>Thông tin tài khoản đăng nhập phần mềm như sau.{0}</p>", Environment.NewLine);
            str.AppendFormat("<p>Tài khoản: <b>{0}</b>{1}</p>", username, Environment.NewLine);
            str.AppendFormat("<p>Mật khẩu: <b>{0}</b>{1}</p>", pass, Environment.NewLine);
            str.AppendFormat("<p>Trân trọng.{0}</p>", Environment.NewLine);
            str.AppendFormat("<p><i>Đây là Email gửi đăng ký tự động, vui lòng không reply. Xin cảm ơn!</i></p>");
            return str.ToString();
        }
    }
}
