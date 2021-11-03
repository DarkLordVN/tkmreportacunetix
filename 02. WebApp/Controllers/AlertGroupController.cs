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
    public class AlertGroupController : BaseController
    {
        private AlertGroupService _AlertGroupService;
        private TuDienVietHoaService _TuDienVietHoaService;

        #region Constructor
        public AlertGroupController()
        {
            if (_AlertGroupService == null) _AlertGroupService = new AlertGroupService();
            if (_TuDienVietHoaService == null) _TuDienVietHoaService = new TuDienVietHoaService();
        }
        #endregion
        /// GET: AlertGroup
        /// <summary>
        /// </summary>
        /// <param name="isSuccess">Check cờ: Show và set hiển thị thông báo sau khi xóa thành công</param>
        /// <param name="tenAlertGroup">Filter: Tên nhóm nguy cơ</param>
        /// <param name="donViID">Filter: Phòng ban</param>
        /// <param name="pnum">Index trang hiện tại</param>
        /// <param name="psize">Số lượng bản ghi hiển thị</param>
        /// <returns></returns>
        public ActionResult Index(int? pnum, int? psize)
        {
            //Tạo và set giá trị biến

            //Tạo constructor view model
            var viewModel = new AlertGroupListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10
            };
            var errorMess = String.Empty;
            //Gọi đến View
            return View(viewModel);
        }
        public ActionResult IndexDetail(AlertGroupListView viewModel)
        {
            var errorMess = String.Empty;
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            if (!string.IsNullOrEmpty(viewModel.TuKhoa))
            {
                viewModel.TuKhoa = viewModel.TuKhoa.Replace("\t", "");
            }
            int total = 0;
            var fullDataND = new List<AlertGroupViewModel>();
            var lstResult = _AlertGroupService.GetList(viewModel.TuKhoa, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, ref errorMess, viewModel.ColumnName, viewModel.OrderBy);
            viewModel.lstAlertGroup = lstResult;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }

        public ActionResult Update(int? id)
        {
            TempData["AlertData"] = null;
            if (id.HasValue && id.Value > 0)
            {
                AlertGroupViewModel viewModel = _AlertGroupService.GetById(id.Value);
                if(viewModel != null && viewModel.ID > 0)
                {
                    if (string.IsNullOrEmpty(viewModel.AlertNameTrans))
                    {
                        viewModel.AlertNameTrans = _TuDienVietHoaService.GetVietHoaByFilter(x => x.CumTuKhoa.Equals(viewModel.AlertName));
                    }
                    if (string.IsNullOrEmpty(viewModel.AlertDescriptionTrans))
                    {
                        viewModel.AlertDescriptionTrans = _TuDienVietHoaService.GetVietHoaByFilter(x => x.CumTuKhoa.Equals(viewModel.AlertDescription));
                    }
                    if (string.IsNullOrEmpty(viewModel.RecommendationsTrans))
                    {
                        viewModel.RecommendationsTrans = _TuDienVietHoaService.GetVietHoaByFilter(x => x.CumTuKhoa.Equals(viewModel.Recommendations));
                    }
                }
                return View(viewModel);
            }
            return RedirectToAction("Index", "AlertGroup", null);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(AlertGroupViewModel viewModel)
        {
            var isSuccess = true;
            var message = String.Empty;
            try
            {
                string errorCode = "";
                message = validateAlertGroup(viewModel, true);
                if (string.IsNullOrEmpty(message))
                {
                    //Chỉ update những trường có thể thay đổi
                    var oViewModel = _AlertGroupService.GetById(viewModel.ID);
                    if (!string.IsNullOrEmpty(viewModel.AlertNameTrans) && !viewModel.AlertNameTrans.Equals(oViewModel.AlertNameTrans))
                    {
                        isSuccess = insertOrUpdateTuDien(viewModel.AlertName, viewModel.AlertNameTrans);
                        if (isSuccess)
                        {
                            oViewModel.AlertNameTrans = viewModel.AlertNameTrans;
                        }
                    }
                    if (!string.IsNullOrEmpty(viewModel.AlertDescriptionTrans) && !viewModel.AlertDescriptionTrans.Equals(oViewModel.AlertDescriptionTrans))
                    {
                        isSuccess = insertOrUpdateTuDien(viewModel.AlertDescription, viewModel.AlertDescriptionTrans);
                        if (isSuccess)
                        {
                            oViewModel.AlertDescriptionTrans = viewModel.AlertDescriptionTrans;
                        }
                    }
                    if (!string.IsNullOrEmpty(viewModel.RecommendationsTrans) && !viewModel.RecommendationsTrans.Equals(oViewModel.RecommendationsTrans))
                    {
                        isSuccess = insertOrUpdateTuDien(viewModel.Recommendations, viewModel.RecommendationsTrans);
                        if (isSuccess)
                        {
                            oViewModel.RecommendationsTrans = viewModel.RecommendationsTrans;
                        }
                    }
                    if (isSuccess)
                    {
                        //Update nhóm nguy cơ vào db
                        isSuccess = _AlertGroupService.Update(oViewModel);
                    }
                }
                else { isSuccess = false; }
                //Show lỗi 
                TempData["AlertType"] = isSuccess ? "alert-success" : "alert-danger";
                TempData["AlertData"] = isSuccess ? "Cập nhật thành công" : "Cập nhật không thành công";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = "Cập nhật không thành công";
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                return View(viewModel);
            }
        }

        private bool insertOrUpdateTuDien(string root, string trans)
        {
            var success = true;
            if (!string.IsNullOrEmpty(root) && !root.Equals(trans))
            {
                var vmTuDien = _TuDienVietHoaService.GetByFilter(x => x.CumTuKhoa.Equals(root));
                if (vmTuDien != null && vmTuDien.ID > 0)
                {
                    vmTuDien.CumTuThayDoi = trans;
                    success = _TuDienVietHoaService.Update(vmTuDien);
                }
                else
                {
                    vmTuDien = new TuDienVietHoaViewModel();
                    vmTuDien.CumTuKhoa = root;
                    vmTuDien.CumTuThayDoi = trans;
                    _TuDienVietHoaService.Add(vmTuDien);
                }
            }
            return success;
        }

        [HttpGet]
        public ViewResult Detail(int id = 0)
        {
            var obj = new AlertGroupViewModel();
            if (id > 0)
            {
                obj = _AlertGroupService.GetById(id);
            }
            return View("~/Views/AlertGroup/Detail.cshtml", obj);
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
            var obj = _AlertGroupService.GetById(id.Value);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.Status = !obj.Status;
                isSuccess = _AlertGroupService.Update(obj);
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

        private string validateAlertGroup(AlertGroupViewModel viewModel, bool update = false)
        {
            var errStr = string.Empty;

            return errStr;
        }

    }
}