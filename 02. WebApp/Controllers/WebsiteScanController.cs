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
    public class WebsiteScanController : BaseController
    {
        private WebsiteService _WebsiteService;
        private WebsiteScanService _WebsiteScanService;
        private TuDienVietHoaService _TuDienVietHoaService;
        private ScannedItemService _ScannedItemService;
        private AffectedItemService _AffectedItemService;

        #region Constructor
        public WebsiteScanController()
        {
            if (_WebsiteService == null) _WebsiteService = new WebsiteService();
            if (_WebsiteScanService == null) _WebsiteScanService = new WebsiteScanService();
            if (_TuDienVietHoaService == null) _TuDienVietHoaService = new TuDienVietHoaService();
            if (_ScannedItemService == null) _ScannedItemService = new ScannedItemService();
            if (_AffectedItemService == null) _AffectedItemService = new AffectedItemService();
        }
        #endregion
        /// GET: WebsiteScan
        /// <summary>
        /// </summary>
        /// <param name="isSuccess">Check cờ: Show và set hiển thị thông báo sau khi xóa thành công</param>
        /// <param name="tenWebsiteScan">Filter: Tên lượt quét</param>
        /// <param name="donViID">Filter: Phòng ban</param>
        /// <param name="pnum">Index trang hiện tại</param>
        /// <param name="psize">Số lượng bản ghi hiển thị</param>
        /// <returns></returns>
        public ActionResult Index(int? pnum, int? psize)
        {
            //Tạo và set giá trị biến
            //Tạo constructor view model
            var viewModel = new WebsiteScanListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10
            };
            var errorMess = String.Empty;
            //Gọi đến View
            return View(viewModel);
        }
        public ActionResult IndexDetail(WebsiteScanListView viewModel)
        {
            var errorMess = String.Empty;
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            if (!string.IsNullOrEmpty(viewModel.TuKhoa))
            {
                viewModel.TuKhoa = viewModel.TuKhoa.Replace("\t", "");
            }
            var websiteID = 0;
            if(viewModel.WebsiteID != null) { int.TryParse(viewModel.WebsiteID, out websiteID); }
            int total = 0;
            var fullDataND = new List<WebsiteScanViewModel>();
            var lstResult = _WebsiteScanService.GetList(viewModel.TuKhoa, websiteID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, ref errorMess, viewModel.ColumnName, viewModel.OrderBy);
            viewModel.lstWebsiteScan = lstResult;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }

        public ActionResult DetailScanItem(ScannedItemListView viewModel)
        {
            var errorMess = String.Empty;
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 5;
            if (!string.IsNullOrEmpty(viewModel.TuKhoa))
            {
                viewModel.TuKhoa = viewModel.TuKhoa.Replace("\t", "");
            }
            int total = 0;
            var fullDataND = new List<ScannedItemViewModel>();
            var lstResult = _ScannedItemService.GetList(viewModel.TuKhoa, viewModel.WebsiteScanID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);
            viewModel.lstScannedItem = lstResult;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }

        public ActionResult DetailAffectItem(AffectedItemListView viewModel)
        {
            var errorMess = String.Empty;
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 5;
            if (!string.IsNullOrEmpty(viewModel.TuKhoa))
            {
                viewModel.TuKhoa = viewModel.TuKhoa.Replace("\t", "");
            }
            int total = 0;
            var fullDataND = new List<AffectedItemViewModel>();
            var lstResult = _AffectedItemService.GetListForWebsiteScan(viewModel.TuKhoa, viewModel.WebsiteScanID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);
            viewModel.lstAffectedItem = lstResult;
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
            var obj = new WebsiteScanViewModel();
            if (id > 0)
            {
                obj = _WebsiteScanService.GetById(id);
                if (obj != null && obj.WebsiteID > 0)
                {
                    obj.vmWebsite = _WebsiteService.GetById(obj.WebsiteID);
                }
            }
            return View("~/Views/WebsiteScan/Detail.cshtml", obj);
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
            var obj = _WebsiteScanService.GetById(id.Value);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.Status = !obj.Status;
                isSuccess = _WebsiteScanService.Update(obj);
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

        private string validateWebsiteScan(WebsiteScanViewModel viewModel, bool update = false)
        {
            var errStr = string.Empty;

            return errStr;
        }

    }
}