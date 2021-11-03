using TKM.BLL;
using TKM.DAO.EntityFramework;
using TKM.Services;
using TKM.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Xceed.Words.NET;

namespace TKM.WebApp.Controllers
{
    public class NguoiDungController : BaseController
    {
        private NguoiDungService _nguoiDungService;
        private NhomQuyenService _NhomQuyenService;

        private string folderPath = @"\UploadedFiles\Staffs";
        #region Constructor
        public NguoiDungController()
        {
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
            if (_NhomQuyenService == null) _NhomQuyenService = new NhomQuyenService();
        }
        #endregion
        /// GET: NguoiDung
        /// <summary>
        /// </summary>
        /// <param name="isSuccess">Check cờ: Show và set hiển thị thông báo sau khi xóa thành công</param>
        /// <param name="tenNguoiDung">Filter: Tên người dùng</param>
        /// <param name="donViID">Filter: Phòng ban</param>
        /// <param name="pnum">Index trang hiện tại</param>
        /// <param name="psize">Số lượng bản ghi hiển thị</param>
        /// <returns></returns>
        public ActionResult Index(string hoVaTen, int? chucVuID, int? donViID, int? trangThai, int? pnum, int? psize)
        {
            //Tạo và set giá trị biến
            int total = 0;

            //Tạo constructor view model
            var viewModel = new NguoiDungListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10
            };
            var errorMess = String.Empty;
            //viewModel.LstDmChucVu = loadChucVu();
            bool? srTrangThai = null;
            if (trangThai.HasValue)
            {
                if (trangThai == 1)
                    srTrangThai = true;
                else
                    srTrangThai = false;
            }
            //Save lại filter truyền vào
            viewModel.HoVaTen = hoVaTen;
            viewModel.ChucVuID = chucVuID;
            viewModel.DonViID = donViID;
            viewModel.TrangThai = srTrangThai;
            //Gọi đến View
            return View(viewModel);
        }
        public ActionResult IndexDetail(NguoiDungListView viewModel)
        {
            var errorMess = String.Empty;
            //if (TempData.ContainsKey("AlertType") && TempData.ContainsKey("AlertData"))
            //{
            //    TempData["AlertType"] = TempData["AlertType"].ToString();
            //    TempData["AlertData"] = TempData["AlertData"].ToString();
            //}
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            if (!string.IsNullOrEmpty(viewModel.TuKhoa))
            {
                viewModel.TuKhoa = viewModel.TuKhoa.Replace("\t", "");
            }
            if (!string.IsNullOrEmpty(viewModel.HoVaTen))
            {
                viewModel.HoVaTen = viewModel.HoVaTen.Replace("\t", "");
            }
            bool? srTrangThai = null;
            if (viewModel.srTrangThai.HasValue)
            {
                if (viewModel.srTrangThai == 1)
                    srTrangThai = true;
                else
                    srTrangThai = false;
            }
            viewModel.TrangThai = srTrangThai;

            int total = 0;
            var fullDataND = new List<NguoiDungViewModel>();
            var lstResult = _nguoiDungService.GetList(ref fullDataND,viewModel.TuKhoa, viewModel.HoVaTen, viewModel.DonViID, viewModel.ChucVuID, viewModel.TrangThai, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, ref errorMess, viewModel.ColumnName, viewModel.OrderBy);
            Session["NguoiDung"] = fullDataND;
            viewModel.LstNguoiDung = lstResult;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }
        public ActionResult Add()
        {
            TempData["AlertData"] = null;
            NguoiDungViewModel viewModel = new NguoiDungViewModel();
            viewModel.GioiTinh = true;

            if (TempData["NguoiDung"] != null)
            {
                viewModel = TempData["NguoiDung"] as NguoiDungViewModel;
            }
            viewModel.LstNhomQuyen = loadNhomQuyen();
            //viewModel.LstNguoiQuanLy = loadNguoiQuanLy();
            viewModel.ThuTu = _nguoiDungService.GetMaxThuTu() + 1;
            viewModel.ThuTus = viewModel.ThuTu.ToString();
            viewModel.TrangThai = true;
            //viewModel.CheckTrangThai = true;
            return View(viewModel);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Add(NguoiDungViewModel viewModel)
        {
            var isSuccess = true;
            var total = 0;
            var typeSubmit = Request["typeSubmit"];
            viewModel.LstNhomQuyen = loadNhomQuyen();
            viewModel.NhomQuyenList = "," + Request["NhomQuyenList"] + ",";
            var message = validateNguoiDung(viewModel);
            if (string.IsNullOrEmpty(message))
            {
                try
                {
                    //if (string.IsNullOrEmpty(message))
                    //{
                    //    var arrLinkFile = Request.Form.GetValues("linkFile");
                    //    if(arrLinkFile != null && arrLinkFile.Count() > 0)
                    //    {
                    //        viewModel.AnhDaiDien = arrLinkFile[0];
                    //    }
                    //}
                    ////Check validate ở Controller
                    //Check trùng tên đăng nhập
                    if (_nguoiDungService.CheckTrungTenDangNhap(viewModel.TenDangNhap))
                    {
                        message += "Tên đăng nhập đã có người sử dụng. Vui lòng nhập tên khác! </br>";
                    }
                    if (string.IsNullOrEmpty(message))
                    {
                        if (!string.IsNullOrEmpty(viewModel.NgaySinhStr))
                        {
                            viewModel.NgaySinh = Utils.ConvertDateTime.ConvertToDate(viewModel.NgaySinhStr);
                        }
                        viewModel.GioiTinh = Request["GioiTinh"] != null && Request["GioiTinh"].Equals("true") ? true : false;
                        viewModel.IsVanThu = Request["IsVanThu"] != null && Request["IsVanThu"].Equals("true") ? true : false;
                        viewModel.IsTongHop = Request["IsTongHop"] != null && Request["IsTongHop"].Equals("true") ? true : false;
                        viewModel.IsLanhDao = Request["IsLanhDao"] != null && Request["IsLanhDao"].Equals("true") ? true : false;
                        if (viewModel.IsLanhDao == true)
                        {
                            viewModel.IsTruong = Request["IsTruong"] != null && Request["IsTruong"].Equals("true") ? true : false;
                            viewModel.IsPho = Request["IsTruong"] != null && Request["IsTruong"].Equals("false") ? true : false;
                        }
                        if (!string.IsNullOrEmpty(viewModel.NhomQuyenList.Trim(',')))
                        {
                            var nhomQuyenId = 0;
                            int.TryParse(viewModel.NhomQuyenList.Trim(',').Split(',')[0], out nhomQuyenId);
                            viewModel.NhomQuyenID = nhomQuyenId;
                        }
                        //Insert người dùng vào db
                        viewModel.NgayTao = DateTime.Now;
                        viewModel.NguoiTao = SessionInfo.CurrentUser.TenDangNhap;
                        viewModel.NguoiTaoID = SessionInfo.CurrentUser.ID;
                        viewModel.TrangThai = viewModel.TrangThai;
                        viewModel.MatKhau = Utils.Encrypt.EncryptMD5("123123");
                        var objCheck = _nguoiDungService.AddGetObject(viewModel);
                        if (objCheck == null)
                        {
                            isSuccess = false;
                            message = "Thêm mới không thành công";
                        }
                        else if (typeSubmit.Equals("luuvanhaptiep") && isSuccess == true)
                        {
                            TempData["NguoiDung"] = null;
                            if (objCheck != null)
                            {
                                TempData["NguoiDung"] = objCheck;
                            }
                        }
                    }
                    //Show lỗi + Load lại data
                    //TempData["AlertType"] = "alert-danger";
                    //TempData["AlertData"] = message;
                    //ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                    //return View(viewModel);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    message = "Thêm mới không thành công";
                    //TempData["AlertType"] = "alert-danger";
                    //TempData["AlertData"] = ex.Message;
                    //ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                    //return View(viewModel);
                }
            }
            else { isSuccess = false; }
            return Json(new { isSuccess = isSuccess, message = message, typeSubmit= typeSubmit }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(int? id)
        {
            TempData["AlertData"] = null;
            if (id.HasValue && id.Value > 0)
            {
                NguoiDungViewModel viewModel = _nguoiDungService.GetById(id.Value);
                viewModel.LstNhomQuyen = loadNhomQuyen();
                //viewModel.LstNguoiQuanLy = loadNguoiQuanLy();
                viewModel.XacNhanMatKhau = viewModel.MatKhau;
                viewModel.NgaySinhStr = Utils.ConvertDateTime.ConvertDateToString(viewModel.NgaySinh, "dd/MM/yyyy");
                viewModel.ThuTus = viewModel.ThuTu.ToString();
                return View(viewModel);
            }
            return RedirectToAction("Index", "NguoiDung", null);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(NguoiDungViewModel viewModel)
        {
            var isSuccess = true;
            var message = String.Empty;
            try
            {
                //viewModel.LstDonVi = loadDonVi();
                ////viewModel.LstDmChucVu = loadChucVu();
                //viewModel.LstNhomQuyen = loadNhomQuyen();
                ////viewModel.LstNguoiQuanLy = loadNguoiQuanLy();
                ////Remove ModelState Validate
                //ModelState["MatKhau"].Errors.Clear();
                //Check validate ở ViewModel

                string errorCode = "";
                //upload file tài liệu
                //if (viewModel.Files != null && viewModel.Files.Count > 0 && viewModel.Files[0] != null)
                //{
                //    viewModel.AnhDaiDien = UploadFile.uploadFile(viewModel.Files[0], Server.MapPath("~"), folderPath, ref errorCode, 1);
                //    if (!string.IsNullOrEmpty(errorCode))
                //    {
                //        TempData["AlertType"] = "alert-danger";
                //        TempData["AlertData"] = errorCode;
                //        viewModel.Files = null;
                //        ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                //        return View(viewModel);
                //    }
                //}
                //Check validate ở Controller
                viewModel.NhomQuyenList = "," + Request["NhomQuyenList"] + ",";
                message = validateNguoiDung(viewModel, true);
                if (string.IsNullOrEmpty(message))
                {
                    //Chỉ update những trường có thể thay đổi
                    var oViewModel = _nguoiDungService.GetById(viewModel.ID);
                    if (!string.IsNullOrEmpty(viewModel.NgaySinhStr))
                    {
                        viewModel.NgaySinhStr = viewModel.NgaySinhStr.Replace(" ", "");
                        oViewModel.NgaySinh = DateTime.ParseExact(viewModel.NgaySinhStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    var arrLinkFile = Request.Form.GetValues("linkFile");
                    if (arrLinkFile != null)
                    {
                        int numfile = arrLinkFile.Count();
                        for (int i = 0; i < numfile; i++)
                        {
                            oViewModel.AnhDaiDien = arrLinkFile[i] != null ? arrLinkFile[i] : "";
                        }
                    }
                    if (!string.IsNullOrEmpty(viewModel.NhomQuyenList.Trim(',')))
                    {
                        var nhomQuyenId = 0;
                        int.TryParse(viewModel.NhomQuyenList.Trim(',').Split(',')[0], out nhomQuyenId);
                        oViewModel.NhomQuyenID = nhomQuyenId;
                    }
                    oViewModel.NhomQuyenList = viewModel.NhomQuyenList;
                    oViewModel.HoVaTen = viewModel.HoVaTen;
                    //oViewModel.MaNhanVien = viewModel.MaNhanVien;
                    oViewModel.SoDienThoai = viewModel.SoDienThoai;
                    oViewModel.GioiTinh = Request["GioiTinh"] != null && Request["GioiTinh"].Equals("true") ? true : false;
                    oViewModel.IsVanThu = Request["IsVanThu"] != null && Request["IsVanThu"].Equals("true") ? true : false;
                    oViewModel.IsTongHop = Request["IsTongHop"] != null && Request["IsTongHop"].Equals("true") ? true : false;
                    oViewModel.IsLanhDao = Request["IsLanhDao"] != null && Request["IsLanhDao"].Equals("true") ? true : false;
                    if (oViewModel.IsLanhDao == true)
                    {
                        oViewModel.IsTruong = Request["IsTruong"] != null && Request["IsTruong"].Equals("true") ? true : false;
                        oViewModel.IsPho = Request["IsTruong"] != null && Request["IsTruong"].Equals("false") ? true : false;
                    }
                    oViewModel.Email = viewModel.Email;
                    oViewModel.ThuTu = viewModel.ThuTu;
                    oViewModel.SoDienThoai = viewModel.SoDienThoai;
                    oViewModel.Fax = viewModel.Fax;
                    oViewModel.DiaChi = viewModel.DiaChi;
                    oViewModel.TrangThai = viewModel.TrangThai;
                    oViewModel.ChucVuID = viewModel.ChucVuID;
                    oViewModel.DonViID = viewModel.DonViID;
                    oViewModel.NguoiCapNhatID = SessionInfo.CurrentUser.ID;
                    oViewModel.NguoiCapNhat = SessionInfo.CurrentUser.TenDangNhap;
                    oViewModel.NgayCapNhat = DateTime.Now;
                    //Update người dùng vào db
                    var check = _nguoiDungService.Update(oViewModel);
                    if (false)
                    {
                        isSuccess = false;
                        message = "Cập nhật không thành công";
                    }
                }
                else { isSuccess = false; }

                //Show lỗi 
                //TempData["AlertType"] = "alert-danger";
                //TempData["AlertData"] = message;
                //ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                //return View(viewModel);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = "Cập nhật không thành công";
                //TempData["AlertType"] = "alert-danger";
                //TempData["AlertData"] = ex.Message;
                //ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                //return View(viewModel);
            }
            return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateInfor()
        {
            TempData["AlertData"] = null;
            NguoiDungViewModel viewModel = _nguoiDungService.GetById(SessionInfo.CurrentUser.ID);
            if (viewModel != null && viewModel.ID > 0)
            {
                viewModel.NgaySinhStr = Utils.ConvertDateTime.ConvertDateTimeToString(viewModel.NgaySinh, "dd/MM/yyyy");
                return View(viewModel);
            }
            return RedirectToAction("Index", "NguoiDung", null);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateInfor(NguoiDungViewModel viewModel)
        {
            var isSuccess = true;
            var message = "Cập nhật thành công"; ;
            try
            {
                viewModel.NhomQuyenList = ",1,";
                //Check validate ở Controller
                message = validateNguoiDung(viewModel, true);
                if (string.IsNullOrEmpty(message))
                {
                    //Chỉ update những trường có thể thay đổi
                    var oViewModel = _nguoiDungService.GetById(viewModel.ID);
                    if (!string.IsNullOrEmpty(viewModel.NgaySinhStr))
                    {
                        viewModel.NgaySinhStr = viewModel.NgaySinhStr.Replace(" ", "");
                        oViewModel.NgaySinh = DateTime.ParseExact(viewModel.NgaySinhStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    var arrLinkFile = Request.Form.GetValues("linkFile");
                    if (arrLinkFile != null)
                    {
                        int numfile = arrLinkFile.Count();
                        for (int i = 0; i < numfile; i++)
                        {
                            oViewModel.AnhDaiDien = arrLinkFile[i] != null ? arrLinkFile[i] : "";
                        }
                    }
                    oViewModel.SoDienThoai = viewModel.SoDienThoai;
                    oViewModel.GioiTinh = Request["GioiTinh"] != null && Request["GioiTinh"].Equals("true") ? true : false;
                    oViewModel.DiaChi = viewModel.DiaChi;
                    //Update người dùng vào db
                    var check = _nguoiDungService.Update(oViewModel);
                    if (false)
                    {
                        isSuccess = false;
                        message = "Cập nhật không thành công";
                    }
                }
                else { isSuccess = false; }

                //Show lỗi 
                TempData["AlertType"] = isSuccess ? "alert-success" : "alert-danger";
                TempData["AlertData"] = "Cập nhật " + (isSuccess ? "thành công!" : "không thành công!");
                //ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = "Cập nhật không thành công";
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                //ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                return View(viewModel);
            }
            //return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            return View(new ChangePassViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(ChangePassViewModel viewModel)
        {
            var isSuccess = false;
            var message = "";
            try
            {
                //Check validate ở Controller
                message = validateChangePass(viewModel);
                if (string.IsNullOrEmpty(message))
                {
                    //Chỉ update những trường có thể thay đổi
                    var oViewModel = _nguoiDungService.GetById(SessionInfo.CurrentUser.ID);
                    //var oViewModel = _nguoiDungService.GetById(5);
                    //Change password
                    oViewModel.MatKhau = Utils.Encrypt.EncryptMD5(viewModel.MatKhauMoi);
                    //Update người dùng vào db
                    var check = _nguoiDungService.Update(oViewModel);
                    isSuccess = check;
                    message = "Thay đổi mật khẩu" + (isSuccess ? " thành công!" : " không thành công!");
                }

                //Show lỗi 
                TempData["AlertType"] = isSuccess ? "alert-success" : "alert-danger";
                TempData["AlertData"] = message;
                //ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                //ViewBag.lstChucVu = _chucVuService.GetList(g => g.TrangThai && g.IsDeleted == false).ToList();
                return View(viewModel);
            }
            //return Json(new { isSuccess = isSuccess, message = !isSuccess ? (string.IsNullOrEmpty(message) ? "Cập nhật không thành công" : message) : "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResult Detail(int id = 0)
        {
            var obj = new NguoiDungViewModel();
            if (id > 0)
            {
                obj = _nguoiDungService.GetById(id);
            }
            return View("~/Views/NguoiDung/Detail.cshtml", obj);
        }
        [HttpPost]
        public JsonResult onChangeStatus(int? id)
        {
            bool isSuccess = false;
            var message = "";
            if (id == null)
            {
                message = "Id không được để trống";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            var obj = _nguoiDungService.GetById(id.Value);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.TrangThai = !obj.TrangThai;
                isSuccess = _nguoiDungService.Update(obj);
                isSuccess = true;
                message = "Thay đổi trạng thái thành công";
            }
            catch (Exception)
            {
                isSuccess = false;
                message = "Đã có lỗi xảy ra";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SignOutUser(int? id)
        {
            // 1: Đăng xuất thành công || -1: Có lỗi xảy ra khi xóa
            var isSuccess = 1;
            var message = "";
            try
            {
                if (id != null)
                {
                    var acc = _nguoiDungService.GetById(id.Value);
                    if (acc != null)
                    {
                        //MembershipUser user = Membership.GetUser(acc.TenDangNhap,false);
                        //user.IsApproved = false;
                        //Membership.UpdateUser(user);
                        message = "Đăng xuất thành công!";
                    }
                    else
                        message = "Bạn chưa chọn tài khoản cần đăng xuất!";
                }
                else
                    message = "Tài khoản không tồn tại trên hệ thống!";

            }
            catch (Exception ex)
            {
                isSuccess = -1;
                message = "Có lỗi khi đăng xuất tài khoản!";
            }
            return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ResetPasswordUser(int? id)
        {
            // 1: Đăng xuất thành công || -1: Có lỗi xảy ra khi xóa
            var isSuccess = 1;
            var message = "";
            var check = false;
            try
            {
                if (id != null)
                {
                    var acc = _nguoiDungService.GetById(id.Value);
                    if (acc != null)
                    {
                        acc.MatKhau = Encrypt.EncryptMD5(Utils.Utils.Parameter.CONST_MATKHAUMACDINH);
                        check = _nguoiDungService.Update(acc);
                        if (check)
                            message = "Reset mật khẩu thành công!";
                        else
                        {
                            isSuccess = 0;
                            message = "Có lỗi khi reset mật khẩu tài khoản!";
                        }
                    }
                    else
                        message = "Bạn chưa chọn tài khoản cần reset!";
                }
                else
                    message = "Tài khoản không tồn tại trên hệ thống!";

            }
            catch (Exception ex)
            {
                isSuccess = -1;
                message = "Có lỗi khi reset mật khẩu tài khoản!";
            }
            return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            // 1: xóa thành công || -1: Có lỗi xảy ra khi xóa
            var isSuccess = -1;
            try
            {
                var checkDel = _nguoiDungService.Delete(id);
                if (checkDel)
                    isSuccess = 1;
            }
            catch (Exception)
            {
                isSuccess = -1;
            }
            return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteMul(string lstid)
        {
            bool isSuccess = false;
            var arrid = lstid.Split(',');
            try
            {
                foreach (var item in arrid)
                {
                    _nguoiDungService.Delete(Convert.ToInt32(item));
                }
                isSuccess = true;
                return Json(new
                {
                    isSuccess = isSuccess,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                isSuccess = false;
                return Json(new
                {
                    isSuccess = isSuccess,
                }, JsonRequestBehavior.AllowGet);
            }

        }
        private string validateNguoiDung(NguoiDungViewModel viewModel, bool update = false)
        {
            var errStr = string.Empty;
            //if (update && !string.IsNullOrEmpty(viewModel.MatKhau) && !viewModel.MatKhau.Equals(viewModel.XacNhanMatKhau))
            //{
            //    ModelState.AddModelError("MatKhau", "Mật khẩu không khớp, vui lòng kiểm nhập lại!");
            //    ModelState.AddModelError("XacNhanMatKhau", "Mật khẩu không khớp, vui lòng kiểm nhập lại!");
            //    errStr += "Thêm mới không thành công <br/>";
            //}

            if (checkTenDangNhap(viewModel.TenDangNhap) && !update)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại!");
                errStr += "Thêm mới không thành công <br/>";
            }

            if (string.IsNullOrEmpty(viewModel.NhomQuyenList.Trim(',')))
            {
                errStr += "Phải chọn ít nhất 1 nhóm quyền! <br/>";
            }

            try
            {
                if (!string.IsNullOrEmpty(viewModel.ThuTus))
                {
                    viewModel.ThuTu = int.Parse(viewModel.ThuTus);
                }
            }
            catch (Exception e)
            {
                errStr += "Số thứ tự phải là số <br/>";
                ModelState.AddModelError("ThuTus", "Số thứ tự phải là số");
            }
            if (viewModel.ThuTu.HasValue && viewModel.ThuTu.Value <= 0)
            {
                errStr += "Số thứ tự phải lớn hơn 0 <br/>";
                ModelState.AddModelError("ThuTus", "Số thứ tự phải lớn hơn 0");
            }

            return errStr;
        }

        private string validateChangePass(ChangePassViewModel viewModel)
        {
            var errStr = string.Empty;
            if (string.IsNullOrEmpty(viewModel.MatKhau))
            {
                errStr += "Mật khẩu hiện tại không được để trống <br />";
            }
            else
            {
                if (string.IsNullOrEmpty(viewModel.MatKhauMoi))
                {
                    errStr += "Mật khẩu mới không được để trống <br />";
                }
                else
                {
                    if (viewModel.MatKhauMoi.Count() > 20 || viewModel.MatKhauMoi.Count() < 6)
                    {
                        errStr += "Mật khẩu mới phải từ 6 đến 20 ký tự <br />";
                    }
                    if (!string.IsNullOrEmpty(viewModel.MatKhauMoi) && !viewModel.MatKhauMoi.Equals(viewModel.NhapLaiMatKhau))
                    {
                        errStr += "Mật khẩu nhập lại không khớp, vui lòng kiểm nhập lại! <br/>";
                    }
                    else
                    {
                        var matKhauEncrypt = Utils.Encrypt.EncryptMD5(viewModel.MatKhau);
                        var currUser = _nguoiDungService.GetByFilter(x => x.ID == SessionInfo.CurrentUser.ID && x.MatKhau.Equals(matKhauEncrypt));
                        if (currUser == null || (currUser != null && currUser.ID <= 0))
                        {
                            errStr += "Sai mật khẩu hiện tại <br/>";
                        }
                    }
                }
            }
            return errStr;
        }

        private bool checkTenDangNhap(string tendangnhap)
        {
            var objND = _nguoiDungService.GetListMaNhom(tendangnhap);
            if (objND != null && objND.Count > 0)
            {
                return true;
            }
            return false;
        }
        private List<NhomQuyenViewModel> loadNhomQuyen()
        {
            //Lấy danh sách chức vụ
            var lvmNhom = _NhomQuyenService.GetDdlList(0);
            //Thêm bản ghi vào vị trí đầu tiên (0)
            //lvmNhom.Insert(0, new NhomQuyenViewModel { ID = 0, TenNhom = "--- Chọn nhóm quyền ---" });
            return lvmNhom;
        }
        //private List<NguoiDungDetailListView> loadNguoiQuanLy()
        //{
        //    //Lấy danh sách chức vụ
        //    var lvmNhom = _nguoiDungService.GetListDDL(SessionInfo.CurrentUser.ID);
        //    //Thêm bản ghi vào vị trí đầu tiên (0)
        //    lvmNhom.Insert(0, new NguoiDungDetailListView { ID = 0, HoVaTen = "--- Chọn người quản lý ---" });
        //    return lvmNhom;
        //}
        [HttpGet]
        public JsonResult CheckTrungTenDangNhap(string id = "0", string name = "")
        {
            var trungTen = false;
            var obj = new List<NguoiDungViewModel>();
            if (id == "0")
                obj = _nguoiDungService.GetList(g => g.TenDangNhap.ToLower().Trim() == name.ToLower().Trim());
            else
                obj = _nguoiDungService.GetList(g => g.TenDangNhap.ToLower().Trim() == name.ToLower().Trim() && g.ID != Convert.ToInt32(id) && g.DonViID == SessionInfo.CurrentUser.DonViID);
            if (obj != null && obj.Count > 0)
            {
                trungTen = true;
            }
            return Json(new { trungTen = trungTen }, JsonRequestBehavior.AllowGet);
        }
        #region Xuất File: Excel,word,pdf,browser
        public ActionResult ExportWord()
        {
            var typeUser = SessionInfo.CurrentUser.TypeUser;
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            var TempPath = Server.MapPath("~/Content/Temp/");
            var gDoc = DocX.Load(TempPath + @"\Temp_NguoiDung.docx");
            #region Thay text
            gDoc.ReplaceText("%TieuDe%", "Danh sách người dùng");
            #endregion
            var lstNguoiDung = (List<NguoiDungViewModel>)Session["NguoiDung"];
            gDoc.PageLayout.Orientation = Orientation.Landscape;
            if (lstNguoiDung != null && lstNguoiDung.Count > 0)
            {
                var column = 7;
                var index = 0;
                var table = gDoc.AddTable(lstNguoiDung.Count + 1, column);
                table.Design = TableDesign.TableGrid;
                table.Alignment = Alignment.center;
                table.SetTableCellMargin(TableCellMarginType.top, 1.0);
                table.SetTableCellMargin(TableCellMarginType.bottom, 1.0);
                table.Rows[0].InsertParagraph().Font(new Xceed.Words.NET.Font("Times New Roman")).FontSize(13).Bold();
                table.Rows[0].Cells[0].Paragraphs.First().Append("Tên đăng nhập").Bold().Alignment = Alignment.center;
                table.Rows[0].Cells[1].Paragraphs.First().Append("Họ và tên").Bold().Alignment = Alignment.center;
                table.Rows[0].Cells[2].Paragraphs.First().Append("Email").Bold().Alignment = Alignment.center;
                table.Rows[0].Cells[3].Paragraphs.First().Append("Nhóm quyền").Bold().Alignment = Alignment.center;
                table.Rows[0].Cells[4].Paragraphs.First().Append("Chức vụ").Bold().Alignment = Alignment.center;
                table.Rows[0].Cells[5].Paragraphs.First().Append("Đơn vị").Bold().Alignment = Alignment.center;
                table.Rows[0].Cells[6].Paragraphs.First().Append("Trạng thái").Bold().Alignment = Alignment.center;
                table.Rows[0].Cells[0].VerticalAlignment = VerticalAlignment.Center;
                table.Rows[0].Cells[1].VerticalAlignment = VerticalAlignment.Center;
                table.Rows[0].Cells[2].VerticalAlignment = VerticalAlignment.Center;
                table.Rows[0].Cells[3].VerticalAlignment = VerticalAlignment.Center;
                table.Rows[0].Cells[4].VerticalAlignment = VerticalAlignment.Center;
                table.Rows[0].Cells[5].VerticalAlignment = VerticalAlignment.Center;
                table.Rows[0].Cells[6].VerticalAlignment = VerticalAlignment.Center;
                foreach (var item in lstNguoiDung)
                {
                    ++index;
                    table.Rows[index].Cells[0].Paragraphs.First().Append(item.TenDangNhap).Alignment = Alignment.left;
                    table.Rows[index].Cells[1].Paragraphs.First().Append(item.HoVaTen).Alignment = Alignment.left;
                    table.Rows[index].Cells[2].Paragraphs.First().Append(item.Email).Alignment = Alignment.left;
                    table.Rows[index].Cells[3].Paragraphs.First().Append(item.TenNhomQuyen.Replace("<br/>",Environment.NewLine)).Alignment = Alignment.left;
                    table.Rows[index].Cells[4].Paragraphs.First().Append(item.TenChucVu).Alignment = Alignment.center;
                    table.Rows[index].Cells[5].Paragraphs.First().Append(item.TenDonVi);
                    table.Rows[index].Cells[6].Paragraphs.First().Append(item.TrangThai ? "Sử dụng":"Không sử dụng");
                }
                for (int d = 0; d <= lstNguoiDung.Count(); d++)
                {
                    for (int c = 0; c < 7; c++)
                    {
                        table.Rows[d].Cells[c].VerticalAlignment = VerticalAlignment.Center;
                        table.Rows[d].Cells[c].MarginTop = 10;
                        table.Rows[d].Cells[c].MarginBottom = 10;
                        table.Rows[d].Cells[c].Paragraphs.First().Font(new Xceed.Words.NET.Font("Times New Roman")).FontSize(13);
                    }
                }
                table.AutoFit = AutoFit.Contents;
                gDoc.InsertTable(table);
            }

            //var fName = DateTime.Now.ToString("InSoVanBanDen_ddMMyyyyhhmmss");
            //gDoc.SaveAs(TempPath + @"\"+ fName + ".docx");
            var path = "~/Content/Export";
            var fileName = "Word-NguoiDung-" + timeStr + ".docx";
            var pathSrv = Server.MapPath(path);
            if (!Directory.Exists(pathSrv))
            {
                Directory.CreateDirectory(pathSrv);
            }
            var outfile = TKM.Utils.CommonUtils.GetPath(path, fileName);
            gDoc.SaveAs(outfile);
            gDoc.Dispose();
            return File(outfile, "application/docx", fileName);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportPDF(FormCollection collection)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            string htmlString = collection["GridHtml"];
            string tenFile = collection["tenFilePDF"];
            string widthtable = collection["widthTable"];
            string baseUrl = "";

            string pdf_page_size = "A4";
            //PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
            //    pdf_page_size, true);

            //string pdf_orientation = "Landscape";
            //PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdf_orientation, true);

            //int webPageWidth = 1024;
            //int webPageHeight = 0;

            //HtmlToPdf converter = new HtmlToPdf();
            //converter.Options.PdfPageSize = pageSize;
            //converter.Options.PdfPageOrientation = pdfOrientation;
            //converter.Options.WebPageWidth = webPageWidth;
            //converter.Options.WebPageHeight = webPageHeight;
            ////converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            //converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.NoAdjustment;
            //converter.Options.DisplayHeader = false;
            //converter.Options.DisplayFooter = false;
            //converter.Options.MarginLeft = 10;
            //converter.Options.MarginRight = 10;
            //converter.Options.MarginTop = 20;
            //converter.Options.MarginBottom = 20;
            //PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            //byte[] pdf = doc.Save();
            //doc.Close();
            byte[] pdf = null;
            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "PDF-" + tenFile + "-" + timeStr + ".pdf";
            return fileResult;
        }
        public void ExportExcel()
        {
            var typeUser = SessionInfo.CurrentUser.TypeUser;
            using (var package = new ExcelPackage())
            {
                var Title = "DANH SÁCH NGƯỜI DÙNG";
                var nameFile = "DanhSachNguoiDung";
                var lstVanBanDen = (List<NguoiDungViewModel>)Session["NguoiDung"];
                var ws = package.Workbook.Worksheets.Add("Sheet1");
                ws.PrinterSettings.Orientation = eOrientation.Landscape;

                // Tạo header
                ws.Cells["A1:E1"].Merge = true;
                ws.Cells["F1:G1"].Merge = true;
                ws.Cells["A2:E2"].Merge = true;
                ws.Cells["F2:G2"].Merge = true;
                ws.Cells["F3:G3"].Merge = true;
                ws.Cells["F4:G4"].Merge = true;
                ws.Cells["F4:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["F4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["A5:G5"].Merge = true;

                ws.Cells["A1"].Value = "CỤC ĐĂNG KIỂM VIỆT NAM";
                //ws.Cells["A2"].Value = "CỤC ĐĂNG KIỂM VIỆT NAM";
                ws.Cells["F1"].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
                ws.Cells["F2"].Value = "Độc lập - Tự do - Hạnh phúc";
                ws.Cells["F4"].Value = "Hà Nội, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                ws.Cells["A5"].Value = Title;

                //ws.Cells["A6"].Value = "Từ ngày: ";
                //ws.Cells["A7"].Value = "Đến ngày: ";
                //ws.Cells["A8"].Value = "Đơn vị: ";

                //Header data
                //ws.Cells["A7"].Value = "STT";
                ws.Cells["A7"].Value = "Tên đăng nhập";
                ws.Cells["B7"].Value = "Họ tên";
                ws.Cells["C7"].Value = "Email";
                ws.Cells["D7"].Value = "Nhóm quyền";
                ws.Cells["E7"].Value = "Chức vụ";
                ws.Cells["F7"].Value = "Đơn vị";
                ws.Cells["G7"].Value = "Trạng thái";


                ws.Row(7).Height = 45;
                ws.Column(1).Width = 20;
                ws.Column(2).Width = 30;
                ws.Column(3).Width = 32;
                ws.Column(4).Width = 35;
                ws.Column(5).Width = 25;
                ws.Column(6).Width = 25;
                ws.Column(7).Width = 15;
                int row = 7;
                int i = 0;
                var countData = lstVanBanDen.Count();
                foreach (var item in lstVanBanDen)
                {
                    row++;
                    i++;
                    ws.Cells["A" + row].Value = item.TenDangNhap;
                    ws.Cells["B" + row].Value = item.HoVaTen;
                    ws.Cells["C" + row].Value = item.Email;
                    ws.Cells["D" + row].Value = item.TenNhomQuyen.Replace("<br/>", Environment.NewLine);
                    ws.Cells["E" + row].Value = item.TenChucVu;
                    ws.Cells["F" + row].Value = item.TenDonVi;
                    ws.Cells["G" + row].Value = item.TrangThai ? "Sử dụng" : "Không sử dụng";
                    //Style
                    ws.Cells["A" + row + ":G" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":G" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":G" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":G" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":G" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A" + row + ":A" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["C" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["E" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["F" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["G" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + row + ":G" + row].Style.WrapText = true;
                }
                var allCells = ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column];
                var cellFont = allCells.Style.Font;
                cellFont.SetFromFont(new System.Drawing.Font("Times New Roman", 14));
                ws.Cells["A5"].Style.Font.Bold = true;
                ws.Cells["A7:G7"].Style.Font.Bold = true;
                ws.Cells["A1:G1"].Style.Font.Bold = true;
                ws.Cells["A1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2:G2"].Style.Font.Bold = true;
                ws.Cells["A2:G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2:G2"].Style.WrapText = true;
                ws.Cells["A:XGD"].Style.Font.Name = "Times New Roman";
                //ws.Cells["A9:J9"].Style.Font.Bold = true;
                ws.Cells["A7:G7"].Style.WrapText = true;
                ws.Cells["A7:G7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:G7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:G7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:G7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells["A1:G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A1:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["C3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A7:G7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A7:G7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //Phần header
                string timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + string.Format("{0}-{1}{2}{3}_{4}.xlsx", nameFile, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeStr));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.BinaryWrite(package.GetAsByteArray());
                System.Web.HttpContext.Current.Response.End();
            }
        }
        #endregion
    }
}