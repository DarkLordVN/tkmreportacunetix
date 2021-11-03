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
    public class TuDienVietHoaController : BaseController
    {
        private TuDienVietHoaService _TuDienVietHoaService;
        private NguoiDungService _nguoiDungService;
        #region Constructor
        public TuDienVietHoaController()
        {
            if (_TuDienVietHoaService == null) _TuDienVietHoaService = new TuDienVietHoaService();
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
        }
        #endregion
        public ActionResult Index(string tenNhom, int? phamViSuDung, int? trangThai, int? pnum, int? psize)
        {
            var viewModel = new TuDienVietHoaListView();
            //SubString input text seach
            if (!String.IsNullOrEmpty(tenNhom) && tenNhom.Trim().Length > 250)
            {
                tenNhom = tenNhom.Substring(0, 250);
            }
            viewModel.PageNumber = pnum ?? 1;
            viewModel.PageSize = psize ?? 10;

            //Gọi đến View
            return View(viewModel);
        }
        public ActionResult IndexDetail(TuDienVietHoaListView viewModel)
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

            //if (!string.IsNullOrEmpty(viewModel.TenNhom))
            //{
            //    viewModel.TenNhom = viewModel.TenNhom.Trim();
            //}
            //bool? srTrangThai = null;
            //if (viewModel.srTrangThai.HasValue)
            //{
            //    if (viewModel.srTrangThai == 1)
            //        srTrangThai = true;
            //    else
            //        srTrangThai = false;
            //}
            //bool? srPhamViSuDung = null;
            //if (viewModel.srPhamViSuDung.HasValue)
            //{
            //    if (viewModel.srPhamViSuDung == 1)
            //        srPhamViSuDung = true;
            //    else
            //        srPhamViSuDung = false;
            //}
            //viewModel.TrangThai = srTrangThai;
            //viewModel.PhamViSuDung = srPhamViSuDung;

            int total = 0;
            var lstResult = _TuDienVietHoaService.GetList(viewModel.TuKhoa, "", "" , null, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);

            viewModel.lstTuDienVietHoa = lstResult;
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
            TuDienVietHoaViewModel viewModel = new TuDienVietHoaViewModel();
            viewModel.TrangThai = true;
            ViewBag.lstNguoiDung = _nguoiDungService.GetListDDL(0);
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Add(TuDienVietHoaViewModel viewModel)
        {
            try
            {
                var message = validateTuDienVietHoa(viewModel);
                
                if (string.IsNullOrEmpty(message))
                {
                    viewModel.TrangThai = viewModel.TrangThai;
                    viewModel.NgayTao = DateTime.Now;
                    viewModel.NguoiTaoID = SessionInfo.CurrentUser.ID;
                    var check = _TuDienVietHoaService.Add(viewModel);
                    if (string.IsNullOrEmpty(check))
                    {
                        TempData["AlertType"] = "alert-success";
                        TempData["AlertData"] = "Thêm mới từ điển thành công";
                        return RedirectToAction("Index", "TuDienVietHoa", null);
                    }
                    message = check;
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

        private string GenMaTuDienVietHoa()
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
                TuDienVietHoaViewModel viewModel = _TuDienVietHoaService.GetById(id.Value);
                return View(viewModel);
            }
            return RedirectToAction("Index", "TuDienVietHoa", null);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(TuDienVietHoaViewModel viewModel)
        {
            try
            {
                var message = String.Empty;
                if (!ModelState.IsValid)
                {
                    message = "Cập nhật không thành công";
                }
                else
                {
                    message = validateTuDienVietHoa(viewModel);
                    if (string.IsNullOrEmpty(message))
                    {
                        var oViewModel = _TuDienVietHoaService.GetById(viewModel.ID);
                        oViewModel.TrangThai = viewModel.TrangThai;
                        oViewModel.NguoiCapNhatID = SessionInfo.CurrentUser.ID;
                        oViewModel.CumTuKhoa = viewModel.CumTuKhoa;
                        oViewModel.CumTuThayDoi = viewModel.CumTuThayDoi;
                        var check = _TuDienVietHoaService.Update(oViewModel);
                        if (check)
                        {
                            TempData["AlertType"] = "alert-success";
                            TempData["AlertData"] = "Cập nhật từ điển thành công";
                            return RedirectToAction("Index", "TuDienVietHoa", null);
                        }
                        message = "Cập nhật không thành công";
                    }
                }
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = message;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                return View(viewModel);
            }
        }
        [HttpGet]
        public ViewResult Detail(int id = 0)
        {
            var obj = new TuDienVietHoaViewModel();
            obj = _TuDienVietHoaService.GetById(id);
            return View("~/Views/TuDienVietHoa/Detail.cshtml", obj);
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
            var obj = _TuDienVietHoaService.GetById(id ?? 0);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.TrangThai = !obj.TrangThai;
                isSuccess = _TuDienVietHoaService.Update(obj);
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
        public JsonResult Delete(int id)
        {
            // 1: xóa thành công || -1: Có lỗi xảy ra khi xóa
            var isSuccess = -1;
            try
            {
                var checkDel = _TuDienVietHoaService.Delete(id);
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
                    _TuDienVietHoaService.Delete(Convert.ToInt32(item));
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
        private string validateTuDienVietHoa(TuDienVietHoaViewModel viewModel)
        {
            return String.Empty;
        }
        
    }
}