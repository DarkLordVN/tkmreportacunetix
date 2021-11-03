using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System;
using TKM.Utils;
using TKM.Services;
using TKM.BLL;
//using CaptchaMvc.HtmlHelpers;
using System.Net;

namespace TKM.WebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly NguoiDungService _nguoiDungService;
        private readonly NhatKyHeThongService _nhatKyHeThongService;
        private readonly QuyenService _quyenService;

        public AuthenticationController()
        {
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
            if (_nhatKyHeThongService == null) _nhatKyHeThongService = new NhatKyHeThongService();
            if (_quyenService == null) _quyenService = new QuyenService();
        }
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel m, string returnUrl)
        {
            //if (!this.IsCaptchaValid(""))
            //{
            //    ModelState.AddModelError("ErrMess", "Nhập sai captcha. Vui lòng nhập lại!");
            //    ViewBag.focusCaptcha = 1;
            //    return View(m);
            //}
            //else
            //{
            if (ModelState.IsValid)
            {
                var strLocked = "";
                if (!string.IsNullOrEmpty(m.RedirectUrl) || (string.IsNullOrEmpty(m.RedirectUrl) && (!string.IsNullOrEmpty(m.TenDangNhap) || !string.IsNullOrEmpty(m.MatKhau))))
                {
                    if (string.IsNullOrEmpty(m.TenDangNhap) || string.IsNullOrEmpty(m.MatKhau))
                    {
                        m.MatKhau = string.Empty;
                        ModelState.AddModelError("ErrMess", "Anh/chị chưa nhập đầy đủ thông tin đăng nhập");
                    }
                    else
                    {
                        var acc = new NguoiDungViewModel();
                        //acc = _nguoiDungService.GetNguoiDungById(5);
                        //var isOk = true;

                        //var isOk = _accountService.DoLogin(m.Email, Encrypt.EncryptMD5(m.Password), ref acc);
                        var isOk = _nguoiDungService.DoLogin(m.TenDangNhap, Encrypt.EncryptMD5(m.MatKhau), ref acc, ref strLocked);
                        if (isOk)
                        {
                            var lstQuyens = _quyenService.GetDanhSachChucNangByUserId(acc.ID);
                            var lstController = ",";
                            var lstAction = ",";
                            if (lstQuyens != null && lstQuyens.Count > 0)
                            {
                                foreach (var item in lstQuyens.Where(x => x.TrangThai))
                                {
                                    lstController += item.Controller.ToUpper() + ",";
                                    lstAction += item.Action.ToUpper() + ",";
                                }
                            }
                            var menuHTML = _quyenService.GetMenuHtmlByUserId(acc.ID);
                            Session.Clear();
                            Session["menu"] = menuHTML;
                            Session["controller"] = lstController;
                            Session["action"] = lstAction;
                            var accTicket = new UserTicketAuthenViewModel()
                            {
                                ID = acc.ID,
                                TenDangNhap = acc.TenDangNhap,
                                HoVaTen = acc.HoVaTen,
                                AnhDaiDien = acc.AnhDaiDien,
                                ChucVuID = acc.ChucVuID,
                                NhomQuyenID = acc.NhomQuyenID,
                                DonViID = acc.DonViID,
                                IsCuc = acc.IsCuc,
                                IsLanhDao = acc.IsLanhDao,
                                IsPho = acc.IsPho,
                                IsTongHop = acc.IsTongHop,
                                IsTruong = acc.IsTruong,
                                IsVanThu = acc.IsVanThu,
                                TenDonVi = acc.TenDonVi
                            };
                            var json = Newtonsoft.Json.JsonConvert.SerializeObject(accTicket);
                            FormsAuthentication.SetAuthCookie(json, true);
                            //Ghi nhật ký hệ thống
                            _nhatKyHeThongService.Add(new NhatKyHeThongViewModel()
                            {
                                NguoiDungId = acc.ID,
                                MoTa = "Đăng nhập hệ thống",
                                NgayTao = DateTime.Now,
                                IpClient = Request.UserHostAddress
                                //IpClient = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString()
                            });
                            if (string.IsNullOrEmpty(m.RedirectUrl))
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                return Redirect(Server.UrlDecode(m.RedirectUrl));
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strLocked))
                            {
                                ModelState.AddModelError("ErrMess", strLocked);
                            }
                            else
                            {
                                m.MatKhau = string.Empty;
                                ModelState.AddModelError("ErrMess", "Tên đăng nhập hoặc mật khẩu không chính xác");
                            }

                        }
                    }
                }

            }
            return View("Login", m);
            //}
        }

        [HttpGet]
        public ActionResult Loginv1(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
       
        public ActionResult DoLogout()
        {
            //Ghi nhật ký hệ thống
            _nhatKyHeThongService.Add(new NhatKyHeThongViewModel()
            {
                NguoiDungId = SessionInfo.CurrentUser.ID,
                MoTa = "Đăng xuất hệ thống",
                NgayTao = DateTime.Now,
                IpClient = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString()
            });
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Authentication");
        }

        public ActionResult ForgotPassword()
        {
            ForgetPassword model = new ForgetPassword();
            return View(model);
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgetPassword model)
        {
            var captchaValid = true;
            //if (!this.IsCaptchaValid(""))
            //{
            //    ModelState.AddModelError("ErrMess", "Nhập sai captcha. Vui lòng nhập lại!");
            //    captchaValid = false;
            //}
            if (!string.IsNullOrEmpty(model.Email))
            {
                if (captchaValid)
                {
                    var user = _nguoiDungService.GetByFilter(x => x.Email.Equals(model.Email) && !x.IsDeleted);
                    if (user != null && user.ID > 0)
                    {
                        var newPassword = Utils.CommonUtils.RandomString(6, false, true);
                        user.MatKhau = Utils.Encrypt.EncryptMD5(newPassword);
                        //Update người dùng vào db
                        var check = _nguoiDungService.Update(user);
                        if (check)
                        {
                            var mailBody = Utils.Template.TemplateEmail.TemplateFogetPass(user.HoVaTen, user.TenDangNhap, newPassword);
                            SendEmail.Send("letoan9002@gmail.com", "Quên mật khẩu - Hệ thống văn bản điều hành", mailBody);
                            ViewBag.Message = "Mật khẩu đã gửi thành công đến địa chỉ hòm thư";
                        }
                        else ModelState.AddModelError("ErrMess", "Có lỗi xảy ra. Vui lòng liên hệ quản trị và thực hiện lại lúc khác!");
                    }
                    else ModelState.AddModelError("Email", "Không tìm thấy tài khoản. Vui lòng liên hệ quản trị để biết thêm thông tin!");
                }
            }
            else ModelState.AddModelError("Email", "Chưa nhập địa chỉ email!");
            return View(model);
        }

        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(LoginModel model)
        {
            try
            {
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Xảy ra lỗi khi cập nhật mật khẩu. Xin vui lòng thử lại sau!";
            }
            return RedirectToAction("Index", "Home");
        }
    }
}