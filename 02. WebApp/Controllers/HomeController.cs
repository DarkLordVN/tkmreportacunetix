using TKM.BLL;
using TKM.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Threading;

namespace TKM.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        private NguoiDungService _nguoiDungService;
        private ThongBaoService _thongBaoService;
        private HeThongThamSoService _heThongThamSoService;
        private QuyenService _quyenService;
        private BaoCaoService _baoCaoService;
        private WebsiteService _websiteService;
        private WebsiteScanService _websiteScanService;
        private AlertGroupService _alertGroupService;
        // GET: Home
        public HomeController()
        {
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
            if (_thongBaoService == null) _thongBaoService = new ThongBaoService();
            if (_heThongThamSoService == null) _heThongThamSoService = new HeThongThamSoService();
            if (_quyenService == null) _quyenService = new QuyenService();
            if (_baoCaoService == null) _baoCaoService = new BaoCaoService();
            if (_websiteService == null) _websiteService = new WebsiteService();
            if (_websiteScanService == null) _websiteScanService = new WebsiteScanService();
            if (_alertGroupService == null) _alertGroupService = new AlertGroupService();
        }
        public ActionResult Index()
        {
            //TongHopDuLieu cTongHop = new TongHopDuLieu();
            //cTongHop.createThreadTongHop();
            TongHopDuLieu.createThreadTongHop();
            if (Session["menu"] == null)
            {
                var menuHTML = _quyenService.GetMenuHtmlByUserId(SessionInfo.CurrentUser.ID);
                Session["menu"] = menuHTML;
            }
            return View();
        }

        public ActionResult ThongTinChung()
        {
            var viewModel = new HomeViewModel();
            viewModel.TongNguyCo = _alertGroupService.GetCountByFilter(x=>x.Severity.ToLower().Equals("high")) + "," + _alertGroupService.GetCountByFilter(x => x.Severity.ToLower().Equals("medium")) + "," + _alertGroupService.GetCountByFilter(x => x.Severity.ToLower().Equals("low"));
            viewModel.TongLuotRaQuet = _websiteScanService.GetCountByFilter(null) + "";
            viewModel.TongSoWebsite = _websiteService.GetCountByFilter(null) + "";
            return View(viewModel);
        }

        public ActionResult NguyCoWebsite()
        {
            var viewModel = new HomeViewModel();
            viewModel.TitleNguyCoWebsite = "Danh sách website gặp nguy cơ nhiều nhất";
            var total = 0;
            var lsWebsite = _websiteService.GetListTopNguyCo(null, 1, 10, ref total);
            if(lsWebsite != null && lsWebsite.Count > 0)
            {
                foreach(var item in lsWebsite)
                {
                    viewModel.TenWebsite += item.Host + ",";
                    viewModel.SoLuotScanWebsite += item.LuotQuet + ",";
                    viewModel.NguyCoCao += item.TongNguyCoCao + ",";
                    viewModel.NguyCoTrungBinh += item.TongNguyCoTrungBinh + ",";
                    viewModel.NguyCoThap += item.TongNguyCoThap + ",";
                }
                viewModel.TenWebsite = viewModel.TenWebsite.Trim(',');
                viewModel.SoLuotScanWebsite = viewModel.SoLuotScanWebsite.Trim(',');
                viewModel.NguyCoCao = viewModel.NguyCoCao.Trim(',');
                viewModel.NguyCoTrungBinh = viewModel.NguyCoTrungBinh.Trim(',');
                viewModel.NguyCoThap = viewModel.NguyCoThap.Trim(',');
            }
            return View(viewModel);
        }

        public ActionResult DanhSachTop()
        {
            var viewModel = new HomeViewModel();
            var total = 0;
            viewModel.LsWebsite = _websiteService.GetListTopNguyCo(null, 1, 5, ref total);
            viewModel.LsAlertGroup = _alertGroupService.GetListTopNguyCo(null, 1, 5, ref total);
            return View(viewModel);
        }

        public void LoadMenuSession()
        {
            if (SessionInfo.CurrentUser != null)
            {
                ViewBag.typeUser = SessionInfo.CurrentUser.TypeUser;
                ViewBag.capBac = SessionInfo.CurrentUser.CapBac;
                ViewBag.userloginid = SessionInfo.CurrentUser.ID;
                ViewBag.donviloginid = SessionInfo.CurrentUser.DonViID;
            }
            if (Session["menu"] == null)
            {
                var menuHTML = _quyenService.GetMenuHtmlByUserId(SessionInfo.CurrentUser.ID);
                Session["menu"] = menuHTML;
                RedirectToAction("Index", "Home", null);
            }
        }

        public ActionResult PhanLoaiVanDe()
        {
            var dicCountTab = new Dictionary<string, int>();
            var lstData = new List<string>();
            int pChuaBiet = 0, pThongTin = 0, pCanhBao = 0, pTrungBinh = 0, pCao = 0, pNghiemTrong = 0;
            _baoCaoService.countProblemGroupLevel("", "", "group", null, null, 0, 0, "", "", ref pChuaBiet, ref pThongTin, ref pCanhBao, ref pTrungBinh, ref pCao, ref pNghiemTrong);
            lstData.Add("fa-question-circle secondary-color[--]" + pChuaBiet + "[--]Chưa biết[--]/BaoCao/ChiTietVanDe");
            lstData.Add("fa-shield-check default-color[--]" + pThongTin + "[--]Thông tin[--]/BaoCao/ChiTietVanDe");
            lstData.Add("fa-info-circle info-color[--]" + pCanhBao + "[--]Cảnh báo[--]/BaoCao/ChiTietVanDe");
            lstData.Add("fa-exclamation-triangle success-color[--]" + pTrungBinh + "[--]Trung bình[--]/BaoCao/ChiTietVanDe");
            lstData.Add("fa-do-not-enter warning-color[--]" + pCao + "[--]Cao[--]/BaoCao/ChiTietVanDe");
            lstData.Add("fa-radiation-alt danger-color[--]" + pNghiemTrong + "[--]Nghiêm trọng[--]/BaoCao/ChiTietVanDe");
            ViewBag.lstData = lstData;
            return View();
        }
        public ActionResult VanBanDiChart(string typeUser = "", string capBac = "", int userloginid = 0, int donviloginid = 0)
        {
            var total = 0;
            var tab_chovaoso = 0;
            var tab_daphathanh = 0;
            var lstDataChart = new List<double>();
            //DataTable tableCV = _vanBanDiService.GetVanBanDiDashBoard(ref tab_chovaoso, ref tab_daphathanh, SessionInfo.CurrentUser.ID, SessionInfo.CurrentUser.DonViID, typeUser, SessionInfo.CurrentUser.CapBac);
            total = tab_chovaoso + tab_daphathanh;
            lstDataChart.Add(Double.IsNaN(Math.Round(((double)tab_chovaoso / (double)total) * 100, 1)) ? 0 : Math.Round(((double)tab_chovaoso / (double)total) * 100, 1));
            lstDataChart.Add(Double.IsNaN(Math.Round(((double)tab_daphathanh / (double)total) * 100, 1)) ? 0 : Math.Round(((double)tab_daphathanh / (double)total) * 100, 1));
            var dataChart = string.Join("|", lstDataChart);
            ViewBag.dataChart = dataChart;
            //ViewBag.tableCV = tableCV;
            return View();
        }
        public ActionResult VanBanDuThaoDiChart(string typeUser = "", string capBac = "", int userloginid = 0, int donviloginid = 0)
        {
            var total = 0;
            var tab_chuaphathanh = 0;
            var tab_daphathanh = 0;
            var lstDataChart = new List<double>();
            total = tab_chuaphathanh + tab_daphathanh;
            lstDataChart.Add(Double.IsNaN(Math.Round(((double)tab_chuaphathanh / (double)total) * 100, 1)) ? 0 : Math.Round(((double)tab_chuaphathanh / (double)total) * 100, 1));
            lstDataChart.Add(Double.IsNaN(Math.Round(((double)tab_daphathanh / (double)total) * 100, 1)) ? 0 : Math.Round(((double)tab_daphathanh / (double)total) * 100, 1));

            var dataChart = string.Join("|", lstDataChart);
            ViewBag.dataChart = dataChart;
            //ViewBag.tableCV = tableCV;
            return View();
        }
        public ActionResult SoLuongThietBi()
        {
            //var arr_dithuong = "4,1,1,1,5,3,19,2,23,4,10,48,198,31,5";
            //var arr_didientu = "";
            //_vanBanDiService.GetSoLuongVanBanDiDashBoard(ref arr_dithuong, ref arr_didientu, SessionInfo.CurrentUser.ID, SessionInfo.CurrentUser.DonViID, typeUser, SessionInfo.CurrentUser.CapBac);
            var viewModel = new BaoCaoTongHopListView();
            string vTotalLoi = "", vTongThietBi = "", vGroupName = "";
            var lstData = _baoCaoService.countHostsByGroup("", "", null, null, 0, 0, "", "");
            if (lstData != null && lstData.Count > 0)
            {
                vTotalLoi = lstData.Select(x => x.totalLoi).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                vTongThietBi = lstData.Select(x => x.totalThietBi).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                vGroupName = lstData.Select(x => x.name).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
            }
            viewModel.TongThietBiLoi = vTotalLoi;
            viewModel.TongThietBi = vTongThietBi;
            viewModel.LstGroup = vGroupName;
            return View(viewModel);
        }
        public ActionResult TongHopVanDe()
        {
            var dataChart = "";
            var viewModel = new BaoCaoTongHopListView();
            var lstData = _baoCaoService.getProblem("", "", "group", null, null, null, 0, 0, "", "", ref dataChart);
            viewModel.LstResultDataProblemDetail = lstData;
            ViewBag.dataChart = dataChart;
            return View(viewModel);
        }
        public ActionResult ThongBaoChuong(int userloginid = 0)
        {
            var lstThongBao = new List<ThongBaoViewModel>();
            var total = 0;
            lstThongBao = _thongBaoService.GetList(x => x.IsDaGui && x.LstNguoiNhanID.Contains("," + userloginid + ","), 1, 10, ref total);
            _thongBaoService.GetList(x => x.IsDaGui && x.LstNguoiNhanID != null && x.LstNguoiNhanID.Contains("," + userloginid + ",") && (x.LstNguoiNhanDaNhanMoiNhatID != null && !x.LstNguoiNhanDaNhanMoiNhatID.Contains("," + userloginid + ",") || x.LstNguoiNhanDaNhanMoiNhatID == null), 1, 10, ref total);
            ViewBag.countNoti = total;
            return View();
        }
        public JsonResult LoadNotificationBell(int? pageIndex, int? pageSize, string act = "")
        {
            var html = "";
            var tieude = "";
            var itemLi = new StringBuilder();
            if (pageIndex == null) pageIndex = 1;
            if (pageSize == null) pageSize = 10;
            var total = 0;
            var lstThongBaoChuong = _thongBaoService.GetList(x => x.IsDaGui && x.LstNguoiNhanID.Contains("," + SessionInfo.CurrentUser.ID + ",") || x.NguoiTaoID == SessionInfo.CurrentUser.ID, pageIndex.Value, pageSize.Value, ref total);
            if (lstThongBaoChuong != null && lstThongBaoChuong.Count > 0)
            {
                foreach (var item in lstThongBaoChuong)
                {
                    tieude = item.ChucNang + ": " + item.TieuDe;
                    itemLi.Append("<div role=\'alert\' aria-live=\'assertive\' aria-atomic=\'true\' class=\'toast\' data-autohide=\'false\'><div class=\'toast-header\'><div class=\'image-box ratio-1-1 view overlay rounded-circle mr-2\' style=\'width: 36px; height: 36px\'><div class=\'image\' style=\'background-image:url(\"" + (!string.IsNullOrEmpty(item.Avatar) ? item.Avatar : "/Assets/images/logo/signin.png") + "\");\' ></div></div><strong class=\'mr-auto\'>" + item.ChucVuNguoiGui + " - " + item.TenNguoiGui + "</strong><small>" + TKM.Utils.ConvertDateTime.GetTimeSpanBefore(item.NgayTao.Value) + "</small> </div><div class=\'toast-body\'><p class=\"mb-0\" title=\"" + tieude + "\">" + tieude + "</p><a class=\'p-0 d-block\' href=\'javascript://\' onclick=\'onDaXem(" + item.ID + ",\"" + item.Link + "\",\"bell\")\'>" + (!string.IsNullOrEmpty(item.LstNguoiNhanDaXemID) && item.LstNguoiNhanDaXemID.Contains("," + SessionInfo.CurrentUser.ID + ",") ? "<i title=\'Đã xem\' class=\'far fa-check-double mr-2 text-success\'></i>" : "<i title=\'Đánh dấu là đã xem\' class=\'far fa-check mr-2 text-primary\'></i>") + item.NoiDung + "</a></div></div>");
                }
                html += itemLi;
            }
            return Json(new { html = html, act = act, pageIndex = pageIndex + 1, total = total }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult LoadNotificationRight()
        //{
        //    var html = "";
        //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //if (!string.IsNullOrEmpty(controllerName) && controllerName.ToUpper().Equals("HOME"))
        //{
        //    var itemFile = "";
        //    var itemLi = new StringBuilder();
        //    var total = 0;
        //    //var lstDonViCuc = _dmDonViService.GetList(x => x.IsDonVi == false || x.KhoaChaID == 0).Select(x => x.ID).ToList();
        //    var lstVanThuCuc = _nguoiDungService.GetList(x => /*x.IsVanThu == true &&*/ (x.DmDonVi != null ? (x.DmDonVi.IsDonVi == false || x.DmDonVi.KhoaChaID == 0) : true) /*lstDonViCuc.Contains(x.DonViID.Value)*/).Select(x => x.ID).ToList();
        //    var lstThongBao = _thongBaoService.GetList(x => x.IsDaGui && (x.LstNguoiNhanID.Contains("," + SessionInfo.CurrentUser.ID + ",") || x.NguoiTaoID == SessionInfo.CurrentUser.ID) && x.Link.Contains("/ThongBao?") /*&& lstVanThuCuc.Contains(x.NguoiTaoID.Value)*/, 1, 5, ref total);
        //    if (lstThongBao != null && lstThongBao.Count > 0)
        //    {
        //        foreach (var item in lstThongBao)
        //        {
        //            itemFile = "";
        //            if (!string.IsNullOrEmpty(item.FileDinhKem))
        //            {
        //                itemFile = "<a href=\"javascript://\" title=\"File đính kèm\" class=\"viewFile text-primary ml-2\" data-link=\"" + item.FileDinhKem + "\"><span><i class=\"fal fa-file-pdf fs20\" data-toggle=\"tooltip\" title=\"File đính kèm\" data-original-title=\"\"></i></span></a>";
        //            }
        //            //<p class=\'mt-0 mb-0\'><a class=\'text-truncate d-block\' href=\'javascript://\'>" + item.TenNguoiGui + "</a></p><small class=\'font-italic text-truncate d-block w-100\'><i class=\'fal fa-user-tag mr-1\'></i>" + item.ChucVuNguoiGui + "</small>
        //            itemLi.Append("<li class=\'media py-3 border-bottom rm\'><div class=\'image-box ratio-1-1 view overlay rounded-circle mr-2\' style=\'width: 50px\'><div class=\'image\' style=\'background-image:url(\"" + (!string.IsNullOrEmpty(item.Avatar) ? item.Avatar : "/Assets/images/logo/signin.png") + "\");\' ></div><a><div class=\'mask rgba-white-slight\'></div></a></div><div class=\'media-body\'><p class=\'mb-1 font-weight-bolder\'><a href=\'javascript://\' title=\'" + item.TieuDe + "\' onclick=\'loadfrmViewThongBaoDashBoard(" + item.ID + ")\'>" + item.TieuDe + "</a>" + itemFile + "</p><p class=\'mb-1\'><small><i class=\'fal fa-clock mr-2\'></i>" + TKM.Utils.ConvertDateTime.GetTimeSpanBefore(item.NgayTao.Value) + "</small></p></div></li>");
        //        }
        //        html += itemLi;
        //    }
        //}
        //    return Json(new { html = html }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult LichLamViecLanhDao()
        //{
        //    var html = ""; string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //    if (!string.IsNullOrEmpty(controllerName) && controllerName.ToUpper().Equals("HOME"))
        //    {
        //        var itemLi = new StringBuilder();
        //        var total = 0;
        //        for (int i = 2; i > -2; i--)
        //        {
        //            html += "<li title=\"Lịch lãnh đạo tuần từ " + Utils.ConvertDateTime.StartOfWeek(DayOfWeek.Monday, 7 * i) + " đến " + Utils.ConvertDateTime.StartOfWeek(DayOfWeek.Sunday, 7 + (7 * i)) + "\" class=\"list-group-item px-0\"><a href=\"/LichLanhDao/IndexView\">Lịch lãnh đạo tuần từ " + Utils.ConvertDateTime.StartOfWeek(DayOfWeek.Monday, 7 * i) + " đến " + Utils.ConvertDateTime.StartOfWeek(DayOfWeek.Sunday, 7 + (7 * i)) + "</a></li>";
        //        }
        //        //var lstLichLamViec = _lichLanhDaoService.GetList(x=>x.IsDeleted == false,1,5,ref total);
        //        //if (lstLichLamViec != null && lstLichLamViec.Count > 0)
        //        //{
        //        //    foreach (var item in lstLichLamViec)
        //        //    {
        //        //        itemLi.Append("<li title=\'" + item.NoiDung + " từ " + TKM.Utils.ConvertDateTime.ConvertDateTimeToString(item.ThoiGianBatDau) + " đến " + TKM.Utils.ConvertDateTime.ConvertDateTimeToString(item.ThoiGianKetThuc) + "\' class=\'list-group-item px-0\'><a href=\'/LichLanhDao/IndexView\'>" + item.NoiDung+" từ "+ TKM.Utils.ConvertDateTime.ConvertDateTimeToString(item.ThoiGianBatDau) + " đến " + TKM.Utils.ConvertDateTime.ConvertDateTimeToString(item.ThoiGianKetThuc) + "</a></li>");
        //        //    }
        //        //    html += itemLi;
        //        //}
        //    }
        //    return Json(new { html = html }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Error()
        {
            TempData["AlertType"] = "alert-danger";
            return View();
        }
        public PartialViewResult ViewFilePopup(string link = "")
        {
            ViewData["link"] = link;
            return PartialView();
        }
        public ActionResult ViewFile(string linkdown, int cks = 0)
        {
            TempData["linkdown"] = linkdown;
            var httpText = "";
            var objHeThongThamSo = _heThongThamSoService.GetByFilter(x => x.MaThamSo.Trim() == "URL_HOST");
            if (objHeThongThamSo != null)
                httpText = objHeThongThamSo.TenThamSo.Trim();
            ViewBag.httpText = httpText;
            ViewBag.cks = cks;
            return View();
        }

    }
}