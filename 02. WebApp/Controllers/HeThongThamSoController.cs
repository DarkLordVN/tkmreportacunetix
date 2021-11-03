using TKM.BLL;
using TKM.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKM.WebApp.Controllers
{
    public class HeThongThamSoController : BaseController
    {

        private HeThongThamSoService _heThongThamSoService;

        #region Contructor
        public HeThongThamSoController()
        {
            if (_heThongThamSoService == null) _heThongThamSoService = new HeThongThamSoService();
        }
        #endregion
        // GET: HeThongThamSo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexDetail(HeThongThamSoListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            if (!string.IsNullOrEmpty(viewModel.TuKhoa))
            {
                viewModel.TuKhoa = viewModel.TuKhoa.Trim();
            }

            if (!string.IsNullOrEmpty(viewModel.MaThamSo))
            {
                viewModel.MaThamSo = viewModel.MaThamSo.Trim();
            }

            if (!string.IsNullOrEmpty(viewModel.TenThamSo))
            {
                viewModel.TenThamSo = viewModel.TenThamSo.Trim();
            }

            if (!string.IsNullOrEmpty(viewModel.MoTa))
            {
                viewModel.MoTa = viewModel.MoTa.Trim();
            }

            int total = 0;            

            var lstResult = _heThongThamSoService.GetList(viewModel.TuKhoa, viewModel.MaThamSo, viewModel.TenThamSo, viewModel.MoTa, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);
            viewModel.LstHeThongThamSo = lstResult;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }

        [HttpGet]
        public ViewResult Detail(int id = 0)
        {
            var obj = new HeThongThamSoViewModel();
            obj = _heThongThamSoService.GetById(id);            

            return View("~/Views/HeThongThamSo/Detail.cshtml", obj);
        }

        public ActionResult Update(int? id)
        {
            TempData["AlertData"] = null;
            if (id.HasValue && id.Value > 0)
            {
                HeThongThamSoViewModel viewModel = _heThongThamSoService.GetById(id.Value);
                return View(viewModel);
            }
            return RedirectToAction("Index", "HeThongThamSo", null);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(HeThongThamSoViewModel viewModel)
        {
            try
            {
                var message = String.Empty;
                var soNgay = 0;
                //Check validate ở ViewModel
                if (!ModelState.IsValid)
                {
                    message = "Cập nhật không thành công!";
                    
                }
                else
                {
                    if (string.IsNullOrEmpty(viewModel.TenThamSo))
                    {
                        message += "Chưa nhập tên tham số!</br>";
                    }
                    if (viewModel.MaThamSo == Utils.Utils.Parameter.CONST_SONGAYLOADVANBAN)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(viewModel.TenThamSo))
                            {
                                soNgay = int.Parse(viewModel.TenThamSo);
                            }
                        }
                        catch (Exception e)
                        {
                            message += "Số ngày phải là số </br>";
                            ModelState.AddModelError("TenThamSo", "Số ngày phải là số");
                        }
                        if (soNgay < 0)
                        {
                            message += "Số ngày không được nhỏ hơn 0 </br>";
                            ModelState.AddModelError("TenThamSo", "Số ngày không được nhỏ hơn 0 ");
                        }
                    }
                    if (string.IsNullOrEmpty(message))
                    {
                        var oViewModel = _heThongThamSoService.GetById(viewModel.ID);
                        if (oViewModel != null)
                        {
                            oViewModel.TenThamSo = viewModel.TenThamSo.Trim();
                            if (viewModel.MaThamSo == Utils.Utils.Parameter.CONST_SONGAYLOADVANBAN)
                                oViewModel.TenThamSo = soNgay.ToString();
                            if (!string.IsNullOrEmpty(viewModel.MoTa))
                                oViewModel.MoTa = viewModel.MoTa.Trim();
                            oViewModel.NguoiCapNhatID = SessionInfo.CurrentUser.ID;
                            oViewModel.NgayCapNhat = DateTime.Now;
                            oViewModel.TrangThai = viewModel.TrangThai;
                            //Insert văn bản đến phân công vào db                  
                            var check = _heThongThamSoService.Update(oViewModel);
                            if (check)
                            {
                                TempData["AlertType"] = "alert-success";
                                TempData["AlertData"] = "Cập nhật hệ thống tham số thành công";
                                return RedirectToAction("Index", "HeThongThamSo", null);
                            }
                            else
                                message = "Cập nhật không thành công!";

                        }
                        else
                            message = "Không tồn tại bản ghi trên hệ thống!";
                    }
                    else
                    {
                        TempData["AlertData"] = "Cập nhật không thành công! <br/>" + message;
                    }
                }
                //Show lỗi + Load lại data
                TempData["AlertType"] = "alert-danger";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                return View(viewModel);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            // 1: xóa thành công || -1: Có lỗi xảy ra khi xóa
            var isSuccess = -1;
            try
            {
                var checkDel = _heThongThamSoService.Delete(id);
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
                    _heThongThamSoService.Delete(Convert.ToInt32(item));
                }
                isSuccess = true;
                return Json(new
                {
                    isSuccess = isSuccess,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                return Json(new
                {
                    isSuccess = isSuccess,
                }, JsonRequestBehavior.AllowGet);
            }

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
            var obj = _heThongThamSoService.GetById(id.Value);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.TrangThai = !obj.TrangThai;
                if (!string.IsNullOrEmpty(obj.MaThamSo) && obj.MaThamSo.ToUpper().Trim() == Utils.Utils.Parameter.CONST_KYSO)
                {
                    obj.MoTa = "Tắt";
                    if (obj.TrangThai)
                        obj.MoTa = "Bật";
                }
                isSuccess = _heThongThamSoService.Update(obj);
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
    }
}