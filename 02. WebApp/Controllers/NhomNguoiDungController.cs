using TKM.BLL;
using TKM.DAO.EntityFramework;
using TKM.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKM.WebApp.Controllers
{
    public class NhomNguoiDungController : BaseController
    {
        private NhomNguoiDungService _nhomNguoiDungService;
        private NguoiDungService _nguoiDungService;
        #region Constructor
        public NhomNguoiDungController()
        {
            if (_nhomNguoiDungService == null) _nhomNguoiDungService = new NhomNguoiDungService();
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
        }
        #endregion
        public ActionResult Index(string tenNhom, int? phamViSuDung, int? trangThai, int? pnum, int? psize)
        {
            var viewModel = new NhomNguoiDungListView();
            //SubString input text seach
            if (!String.IsNullOrEmpty(tenNhom) && tenNhom.Trim().Length > 250)
            {
                tenNhom = tenNhom.Substring(0, 250);
            }
            viewModel.PageNumber = pnum ?? 1;
            viewModel.PageSize = psize ?? 10;
            viewModel.TenNhom = tenNhom;

            //Gọi đến View
            return View(viewModel);
        }
        public ActionResult IndexDetail(NhomNguoiDungListView viewModel)
        {
            //if (TempData.ContainsKey("AlertType") && TempData.ContainsKey("AlertData"))
            //{
            //    TempData["AlertType"] = TempData["AlertType"].ToString();
            //    TempData["AlertData"] = TempData["AlertData"].ToString();
            //}
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            if (!string.IsNullOrEmpty(viewModel.TuKhoa))
            {
                viewModel.TuKhoa = viewModel.TuKhoa.Trim();
            }

            if (!string.IsNullOrEmpty(viewModel.TenNhom))
            {
                viewModel.TenNhom = viewModel.TenNhom.Trim();
            }
            bool? srTrangThai = null;
            if (viewModel.srTrangThai.HasValue)
            {
                if (viewModel.srTrangThai == 1)
                    srTrangThai = true;
                else
                    srTrangThai = false;
            }
            bool? srPhamViSuDung = null;
            if (viewModel.srPhamViSuDung.HasValue)
            {
                if (viewModel.srPhamViSuDung == 1)
                    srPhamViSuDung = true;
                else
                    srPhamViSuDung = false;
            }
            viewModel.TrangThai = srTrangThai;
            viewModel.PhamViSuDung = srPhamViSuDung;

            int total = 0;
            var lstResult = _nhomNguoiDungService.GetList(viewModel.TuKhoa, viewModel.TenNhom,viewModel.PhamViSuDung, viewModel.TrangThai, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);
            var ds = "";
            var ten = "";
            foreach (var item in lstResult)
            {

                var arrNguoiDungID = item.ListNguoiDungThuocNhomID.Split(',');
                for (int i = 0; i < arrNguoiDungID.Length; i++)
                {
                    if (!string.IsNullOrEmpty(arrNguoiDungID[i]))
                    {
                        ten = _nguoiDungService.GetById(Convert.ToInt32(arrNguoiDungID[i]))?.HoVaTen;
                        ds += !string.IsNullOrEmpty(ten)? ten : "" + "</br>";
                    }
                }
                item.NguoiDungTrongNhom = ds;
                ds = "";
            }
            viewModel.LstNhomNguoiDung = lstResult;
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
            NhomNguoiDungViewModel viewModel = new NhomNguoiDungViewModel();
            viewModel.TrangThai = true;
            ViewBag.lstNguoiDung = _nguoiDungService.GetListDDL(0);
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Add(NhomNguoiDungViewModel viewModel)
        {
            try
            {
                //Chọn người dùng nhóm 
                var strNguoiDungID = Request["ListNguoiDungThuocNhomID"];
                if (!ModelState.IsValid)
                {
                    viewModel.ListNguoiDungThuocNhomID = "," + strNguoiDungID + ",";
                    ViewBag.lstNguoiDung = _nguoiDungService.GetListDDL(0);
                    return View(viewModel);
                }
                var message = validateNhomNguoiDung(viewModel);
                
                if (string.IsNullOrEmpty(strNguoiDungID))
                {
                    message += "Bạn chưa chọn người dùng </br>";
                }
                if (string.IsNullOrEmpty(message))
                {
                    if (!string.IsNullOrEmpty(viewModel.TenNhom))
                        viewModel.TenNhom = viewModel.TenNhom.Trim();

                    viewModel.TrangThai = viewModel.TrangThai;
                    viewModel.ListNguoiDungThuocNhomID = ","+ strNguoiDungID + ",";
                    viewModel.NgayTao = DateTime.Now;
                    viewModel.NguoiTaoID = SessionInfo.CurrentUser.ID;
                    viewModel.NguoiTao = SessionInfo.CurrentUser.TenDangNhap;
                    var check = _nhomNguoiDungService.Add(viewModel);
                    if (check)
                    {
                        TempData["AlertType"] = "alert-success";
                        TempData["AlertData"] = "Thêm mới nhóm người dùng thành công";
                        return RedirectToAction("Index", "NhomNguoiDung", null);
                    }
                    message = "Thêm mới không thành công";
                }
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = message;
                ViewBag.lstNguoiDung = _nguoiDungService.GetListDDL(0);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                ViewBag.lstNguoiDung = _nguoiDungService.GetListDDL(0);
                return View(viewModel);
            }
        }

        private string GenMaNhomNguoiDung()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

      
        public ActionResult Update(int? id)
        {
            TempData["AlertData"] = null;
            ViewBag.lstNguoiDung = _nguoiDungService.GetListDDL(0);
            if (id.HasValue && id.Value > 0)
            {
                NhomNguoiDungViewModel viewModel = _nhomNguoiDungService.GetById(id.Value);
                return View(viewModel);
            }
            return RedirectToAction("Index", "NhomNguoiDung", null);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(NhomNguoiDungViewModel viewModel)
        {
            try
            {
                var strNguoiDungID = Request["ListNguoiDungThuocNhomID"];
                var message = String.Empty;
                if (!ModelState.IsValid)
                {
                    viewModel.ListNguoiDungThuocNhomID = "," + strNguoiDungID + ",";
                    message = "Cập nhật không thành công";
                }
                else
                {
                    message = validateNhomNguoiDung(viewModel);
                    //Chọn người dùng nhóm 
                    if (string.IsNullOrEmpty(strNguoiDungID))
                    {
                        message += "Bạn chưa chọn người dùng </br>";
                    }
                    if (string.IsNullOrEmpty(message))
                    {
                        var oViewModel = _nhomNguoiDungService.GetById(viewModel.ID);
                        if (!string.IsNullOrEmpty(viewModel.TenNhom))
                            oViewModel.TenNhom = viewModel.TenNhom.Trim();
                        oViewModel.PhamViSuDung = viewModel.PhamViSuDung;
                        oViewModel.ListNguoiDungThuocNhomID = "," + strNguoiDungID + ",";
                        oViewModel.TrangThai = viewModel.TrangThai;
                        oViewModel.NguoiCapNhatID = SessionInfo.CurrentUser.ID;
                        oViewModel.NguoiCapNhat = SessionInfo.CurrentUser.TenDangNhap;
                        oViewModel.NgayCapNhat = DateTime.Now;
                        var check = _nhomNguoiDungService.Update(oViewModel);
                        if (check)
                        {
                            TempData["AlertType"] = "alert-success";
                            TempData["AlertData"] = "Cập nhật nhóm người dùng thành công";
                            return RedirectToAction("Index", "NhomNguoiDung", null);
                        }
                        message = "Cập nhật không thành công";
                    }
                }
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = message;
                ViewBag.lstNguoiDung = _nguoiDungService.GetListDDL(0);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                ViewBag.lstNguoiDung = _nguoiDungService.GetListDDL(0);
                return View(viewModel);
            }
        }
        [HttpGet]
        public ViewResult Detail(int id = 0)
        {
            var obj = new NhomNguoiDungViewModel();
            obj = _nhomNguoiDungService.GetById(id);
            var ds = "";
            var ten = "";
            var arrNguoiDungID = obj.ListNguoiDungThuocNhomID.Split(',');
            for (int i = 0; i < arrNguoiDungID.Length; i++)
            {
                if (!string.IsNullOrEmpty(arrNguoiDungID[i]))
                {
                    var intID = Convert.ToInt32(arrNguoiDungID[i]);
                    ten = _nguoiDungService.GetById(intID)?.HoVaTen;
                    ds += "<span><i class='fa fa-circle fs8 pr4'></i>" + (!string.IsNullOrEmpty(ten) ? ten : "") + "</span></br>";
                }
            }
            obj.NguoiDungTrongNhom = ds;
            return View("~/Views/NhomNguoiDung/Detail.cshtml", obj);
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
            var obj = _nhomNguoiDungService.GetById(id ?? 0);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.TrangThai = !obj.TrangThai;
                isSuccess = _nhomNguoiDungService.Update(obj);
                message = isSuccess ? "Thay đổi trạng thái thành công" : "Đã có lỗi xảy ra";
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
        public JsonResult onChangePVSD(int? id)
        {
            bool isSuccess = false;
            var message = "";
            if (id == null)
            {
                message = "Id không được để trống";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            var obj = _nhomNguoiDungService.GetById(id ?? 0);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.PhamViSuDung = !obj.PhamViSuDung;
                isSuccess = _nhomNguoiDungService.Update(obj);
                message = isSuccess ? "Thay đổi phạm vi sử dụng thành công" : "Đã có lỗi xảy ra";
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
        public JsonResult Delete(int id)
        {
            // 1: xóa thành công || -1: Có lỗi xảy ra khi xóa
            var isSuccess = -1;
            try
            {
                var checkDel = _nhomNguoiDungService.Delete(id);
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
                    _nhomNguoiDungService.Delete(Convert.ToInt32(item));
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
        private string validateNhomNguoiDung(NhomNguoiDungViewModel viewModel)
        {
            return String.Empty;
        }
        
    }
}