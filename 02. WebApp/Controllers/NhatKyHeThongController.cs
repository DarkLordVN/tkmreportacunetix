using TKM.BLL;
using TKM.Services;
using TKM.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKM.WebApp.Controllers
{
    public class NhatKyHeThongController : BaseController
    {
        private NhatKyHeThongService _nhatKyHeThongService;
        private NguoiDungService _nguoiDungService;
        public NhatKyHeThongController()
        {
            if (_nhatKyHeThongService == null) _nhatKyHeThongService = new NhatKyHeThongService();
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
        }

        // GET: NhatKyHeThong
        public ActionResult Index(int? nguoiDungID, string ipClient, string moTa, int? pnum, int? psize)
        {
            var viewModel = new NhatKyHeThongListView();
            //SubString input text seach
            if (!String.IsNullOrEmpty(ipClient) && ipClient.Trim().Length > 50)
            {
                ipClient = ipClient.Substring(0, 50);
            }
            if (!String.IsNullOrEmpty(moTa) && moTa.Trim().Length > 250)
            {
                moTa = moTa.Substring(0, 250);
            }
            viewModel.PageNumber = pnum ?? 1;
            viewModel.PageSize = psize ?? 20;
            viewModel.NguoiDungID = nguoiDungID;
            viewModel.IpClient = ipClient;
            viewModel.MoTa = moTa;
            viewModel.TuNgayNT = DateTime.Now.ToString("dd/MM/yyyy");
            viewModel.DenNgayNT = DateTime.Now.ToString("dd/MM/yyyy");
            viewModel.lstNguoiDung = _nguoiDungService.GetList(null);
            //Gọi đến View
            return View(viewModel);
        }
        public ActionResult IndexDetail(NhatKyHeThongListView viewModel)
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
            if (!string.IsNullOrEmpty(viewModel.IpClient))
            {
                viewModel.IpClient = viewModel.IpClient.Trim();
            }
            if (!string.IsNullOrEmpty(viewModel.MoTa))
            {
                viewModel.MoTa = viewModel.MoTa.Trim();
            }

            int total = 0;
            var lstResult = _nhatKyHeThongService.GetList(viewModel.TuKhoa, viewModel.NguoiDungID, viewModel.IpClient, viewModel.MoTa, viewModel.TuNgayNT, viewModel.DenNgayNT, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);

            viewModel.LstNhatKyHeThong = lstResult;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }
    }
}