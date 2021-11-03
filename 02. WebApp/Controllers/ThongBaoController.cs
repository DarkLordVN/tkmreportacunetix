using TKM.BLL;
using TKM.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using TKM.Utils;
using TKM.DAO.EntityFramework;

namespace TKM.WebApp.Controllers
{
    public class ThongBaoController : BaseController
    {
        private string folderPath = @"\UploadedFiles\Staffs";
        private static EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private ThongBaoService _thongBaoService;
        private NguoiDungService _nguoiDungService;
        private NhomNguoiDungService _nhomNguoiDungService;
        private ThongBaoAttachService _thongBaoAttachService;
        private readonly ThongBaoAttachRepository _thongBaoAttachResp;
        private readonly NguoiDungRepository _nguoiDungResp;
        private readonly NhomNguoiDungRepository _nhomNguoiDungResp;
        #region Constructor
        public ThongBaoController()
        {
            if (_thongBaoService == null) _thongBaoService = new ThongBaoService();
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
            if (_nhomNguoiDungService == null) _nhomNguoiDungService = new NhomNguoiDungService();
            if (_thongBaoAttachService == null) _thongBaoAttachService = new ThongBaoAttachService();
            _thongBaoAttachResp = new ThongBaoAttachRepository(new EFRepository<ThongBaoAttach>(), oUnitOfWork);
            _nguoiDungResp = new NguoiDungRepository(new EFRepository<NguoiDung>(), oUnitOfWork);
            _nhomNguoiDungResp = new NhomNguoiDungRepository(new EFRepository<NhomNguoiDung>(), oUnitOfWork);


        }
        #endregion
        /// GET: DonVi
        /// <summary>
        /// </summary>
        /// <param name="isSuccess">Check cờ: Show và set hiển thị thông báo sau khi xóa thành công</param>
        /// <param name="tieude">Filter: tiêu đề</param>
        /// <param name="nguoitaoID">Filter: người tạo</param>
        /// <param name="tuNgayTaoTB">Filter: tiêu đề</param>
        /// <param name="denNgayTaoTB">Filter: người tạo</param>
        /// <param name="pnum">Index trang hiện tại</param>
        /// <param name="psize">Số lượng bản ghi hiển thị</param>
        /// <returns></returns>
        public ActionResult Index(int tab = 0)
        {
            var viewModel = new ThongBaoListView();
            viewModel.tab = tab;
            viewModel.lstNguoiDung = loadNguoiDung(0);
            ViewBag.lstChucNang = loadDSChucNang();
            return View(viewModel);
        }
        private List<CommonDDL> loadDSChucNang()
        {
            //Return chức năng enums
            return ObjectUtils.GetListEnums<TKM.Utils.Enums.ChucNang>(typeof(TKM.Utils.Enums.ChucNang));
        }
        public ActionResult IndexDetail(ThongBaoListView viewModel)
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

            if (!string.IsNullOrEmpty(viewModel.TieuDe))
            {
                viewModel.TieuDe = viewModel.TieuDe.Trim();
            }

            int total = 0;
            var typeUser = SessionInfo.CurrentUser.TypeUser;
            var lstResult = _thongBaoService.GetList(viewModel.TuKhoa, viewModel.PhamViTimKiem, viewModel.TieuDe, viewModel.NguoiTaoID, viewModel.ChucNangID, viewModel.TuNgayTaoTB, viewModel.DenNgayTaoTB, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy, viewModel.tab, "", typeUser, SessionInfo.CurrentUser.DonViID, SessionInfo.CurrentUser.ID, SessionInfo.CurrentUser.TenDangNhap.ToLower());

            viewModel.lstThongBao = lstResult;
            viewModel.TotalItem = total;
            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            ViewBag.tab = viewModel.tab;
            return PartialView(viewModel);
        }

        public ActionResult Add()
        {
            TempData["AlertData"] = null;
            ThongBaoViewModel viewModel = new ThongBaoViewModel();
            viewModel.lstNguoiDung = loadNguoiDung(SessionInfo.CurrentUser.ID);
            viewModel.lstNhomNguoiDung = loadNhomNguoiDung();
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Add(ThongBaoViewModel viewModel)
        {
            var total = 0;
            var idNew = 0;
            var message = String.Empty;
            var isSuccess = true;
            try
            {
                if (!string.IsNullOrEmpty(viewModel.TieuDe))
                    viewModel.TieuDe = viewModel.TieuDe.Trim();
                if (!string.IsNullOrEmpty(viewModel.NoiDung))
                    viewModel.NoiDung = viewModel.NoiDung.Trim();
                viewModel.LstNguoiNhanID = !string.IsNullOrEmpty(Request["LstNguoiNhanID"]) ? "," + Request["LstNguoiNhanID"] + "," : "";
                viewModel.LstNhomNguoiNhanID = !string.IsNullOrEmpty(Request["LstNhomNguoiNhanID"]) ? "," + Request["LstNhomNguoiNhanID"] + "," : "";
                message = Validate(viewModel);
                if (string.IsNullOrEmpty(message))
                {
                    
                    
                    //viewModel.MaDonVi = Utils.CommonUtils.RandomString(6);
                    viewModel.Link = TKM.Utils.Utils.Parameter.CONST_LINKTHONGBAO;
                    viewModel.ChucNangID = (int)TKM.Utils.Enums.ChucNang.ThongBao;
                    viewModel.NgayTao = DateTime.Now;
                    viewModel.NguoiTaoID = SessionInfo.CurrentUser.ID;
                    viewModel.NguoiTao = SessionInfo.CurrentUser.TenDangNhap;
                    //File
                    var arrNameFile = Request.Form.GetValues("nameFile");
                    var arrLinkFile = Request.Form.GetValues("linkFile");
                    var arrReplaceNameFile = Request.Form.GetValues("abcd"); // set ten file
                    var arrSize = Request.Form.GetValues("sizeFile");
                    var lstLink = new List<string>();
                    if (arrLinkFile != null)
                    {
                        int numfile = arrLinkFile.Count();
                        for (int i = 0; i < numfile; i++)
                        {
                            var linkfile = arrLinkFile[i] != null ? arrLinkFile[i] : "";
                            if (!string.IsNullOrEmpty(linkfile))
                            {
                                lstLink.Add(linkfile);
                            }
                        }
                    }
                    if (lstLink != null && lstLink.Count > 0)
                    {
                        viewModel.FileDinhKem = string.Join("[--]", lstLink);
                    }
                    //Insert đơn vị vào db                  
                    var check = _thongBaoService.Add(viewModel);
                    if (check)
                    {
                        //File JqueryUpload
                        var objAddNew = _thongBaoService.GetListTopOne(1, 1, ref total, "ID", "asc").FirstOrDefault();
                        idNew = objAddNew.ID;
                        if (arrLinkFile != null)
                        {
                            int numfile = arrLinkFile.Count();
                            for (int i = 0; i < numfile; i++)
                            {
                                var namefile = arrNameFile[i] != null ? arrNameFile[i] : "";
                                var linkfile = arrLinkFile[i] != null ? arrLinkFile[i] : "";
                                var replaceName = arrReplaceNameFile != null ? arrReplaceNameFile[i] : "";
                                var size = arrSize[i] != null ? arrSize[i] : "";
                                var code = "filethongbao";
                                if (!string.IsNullOrEmpty(linkfile))
                                {
                                    insert_filetoDb(namefile, linkfile, replaceName, idNew, code, size);
                                }
                            }
                        }
                    }

                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                message = "Có lỗi trong quá trình thêm mới!";
                isSuccess = false;
            }
            return Json(new { isSuccess = isSuccess, message = message, idNew = idNew }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(int? id)
        {
            TempData["AlertData"] = null;
            var viewModel = new ThongBaoViewModel();
            if (id.HasValue && id.Value > 0)
            {
                var getid = id.Value;
                viewModel = _thongBaoService.GetById(getid);
                viewModel.NgayTaoStr = !string.IsNullOrEmpty(viewModel.NgayTao.ToString()) ? Convert.ToDateTime(viewModel.NgayTao.ToString()).ToString("dd/MM/yyyy") : "";
                viewModel.lstNguoiDung = loadNguoiDung(SessionInfo.CurrentUser.ID);
                viewModel.lstNhomNguoiDung = loadNhomNguoiDung();
                //File
                var lstFile = _thongBaoAttachService.GetList(g => g.ThongBaoID == id && g.Code == "filethongbao");
                var arrLink = lstFile.Select(g => g.LinkFile);
                var arrName = lstFile.Select(g => g.NameFile);
                var arrReplaceName = lstFile.Select(g => g.ReplaceName);
                var arrSize = lstFile.Select(g => g.Size);
                TempData["group_link"] = string.Join(",", arrLink);
                TempData["group_name"] = string.Join("|", arrName);
                TempData["group_replacename"] = string.Join("|", arrReplaceName);
                TempData["group_size"] = string.Join("|", arrSize);
                return View(viewModel);
            }
            return RedirectToAction("Index", "ThongBao", null);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Update(ThongBaoViewModel viewModel)
        {
            var message = String.Empty;
            var isSuccess = true;
            try
            {
                if (viewModel != null && viewModel.ID > 0)
                {
                    var oViewModel = _thongBaoService.GetById(viewModel.ID);
                    if (oViewModel != null)
                    {
                        if (!string.IsNullOrEmpty(viewModel.TieuDe))
                            oViewModel.TieuDe = viewModel.TieuDe.Trim();
                        if (!string.IsNullOrEmpty(viewModel.NoiDung))
                            oViewModel.NoiDung = viewModel.NoiDung.Trim();
                        viewModel.LstNguoiNhanID = !string.IsNullOrEmpty(Request["LstNguoiNhanID"]) ? "," + Request["LstNguoiNhanID"] + "," : "";
                        viewModel.LstNhomNguoiNhanID = !string.IsNullOrEmpty(Request["LstNhomNguoiNhanID"]) ? "," + Request["LstNhomNguoiNhanID"] + "," : "";
                        message = Validate(viewModel);
                        if (string.IsNullOrEmpty(message))
                        {
                            oViewModel.LstNguoiNhanID = viewModel.LstNguoiNhanID;
                            oViewModel.LstNhomNguoiNhanID = viewModel.LstNhomNguoiNhanID;
                            oViewModel.Link = TKM.Utils.Utils.Parameter.CONST_LINKTHONGBAO;
                            oViewModel.ChucNangID = (int)TKM.Utils.Enums.ChucNang.ThongBao;
                            //File
                            var arrNameFile = Request.Form.GetValues("nameFile");
                            var arrLinkFile = Request.Form.GetValues("linkFile");
                            var arrReplaceNameFile = Request.Form.GetValues("abcd"); // set ten file
                            var arrSize = Request.Form.GetValues("sizeFile");
                            var lstLink = new List<string>();
                            if (arrLinkFile != null)
                            {
                                int numfile = arrLinkFile.Count();
                                for (int i = 0; i < numfile; i++)
                                {
                                    var linkfile = arrLinkFile[i] != null ? arrLinkFile[i] : "";
                                    if (!string.IsNullOrEmpty(linkfile))
                                    {
                                        lstLink.Add(linkfile);
                                    }
                                }
                            }
                            oViewModel.FileDinhKem = null;
                            if (lstLink != null && lstLink.Count > 0)
                            {
                                oViewModel.FileDinhKem = string.Join("[--]", lstLink);
                            }
                            var check = _thongBaoService.Update(oViewModel);
                            if (check)
                            {
                                //File JqueryUpload
                                //Xóa file cũ
                                delete_filetoDb(oViewModel.ID, "filethongbao");
                                if (arrLinkFile != null)
                                {
                                    int numfile = arrLinkFile.Count();
                                    for (int i = 0; i < numfile; i++)
                                    {
                                        var namefile = arrNameFile[i] != null ? arrNameFile[i] : "";
                                        var linkfile = arrLinkFile[i] != null ? arrLinkFile[i] : "";
                                        var replaceName = arrReplaceNameFile != null ? arrReplaceNameFile[i] : "";
                                        var size = arrSize[i] != null ? arrSize[i] : "";
                                        var code = "filethongbao";
                                        if (!string.IsNullOrEmpty(linkfile))
                                        {
                                            //Upload file mới
                                            insert_filetoDb(namefile, linkfile, replaceName, oViewModel.ID, code, size);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                message = "Có lỗi trong quá trình cập nhật!";
                                isSuccess = false;
                            }

                        }
                        else
                        {
                            isSuccess = false;
                        }

                    }
                    else
                    {
                        message = "Không tìm thấy bản ghi trên hệ thống. Liên hệ ban quản trị!";
                        isSuccess = false;
                    }
                }
                else
                {
                    message = "Bạn chưa chọn bản ghi cần cập nhật!";
                    isSuccess = false;
                }

            }
            catch (Exception ex)
            {
                message = "Có lỗi trong quá trình cập nhật!";
                isSuccess = false;
            }
            return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }
        public string Validate(ThongBaoViewModel oViewModel)
        {
            var message = "";
            var arrNguoiNhanTrongNhom = new string[10000];
            if (string.IsNullOrEmpty(oViewModel.TieuDe))
            {
                //|| (!string.IsNullOrEmpty(oViewModel.TieuDe) && oViewModel.TieuDe.Length < 3)
                //message += "Tiêu đề chứa tối thiểu 3 ký tự!</br>";
                message += "Tiêu đề không để trống!</br>";
            }
            if (!string.IsNullOrEmpty(oViewModel.TieuDe) && oViewModel.TieuDe.Length > 250)
            {
                message += "Tiêu đề không được nhập quá 250 ký tự!</br>";
            }
            if (string.IsNullOrEmpty(oViewModel.NoiDung))
            {
                message += "Nội dung không để trống!</br>";
                // || (!string.IsNullOrEmpty(oViewModel.NoiDung) && oViewModel.NoiDung.Length < 3)
                //message += "Nội dung chứa tối thiểu 3 ký tự!</br>";
            }
            if (!string.IsNullOrEmpty(oViewModel.NoiDung) && oViewModel.NoiDung.Length > 250)
            {
                message += "Nội dung không được nhập quá 250 ký tự!</br>";
            }
            var nguoiNhanTrongNhomStr = "";
            var objNhomNguoiNhan = new NhomNguoiDungViewModel();
            //lây ra người dùng trong nhóm rồi lọc các người dùng trùng
            if (!string.IsNullOrEmpty(oViewModel.LstNhomNguoiNhanID) && oViewModel.LstNhomNguoiNhanID.Contains(","))
            {
                var arrNhomNguoiNhan = oViewModel.LstNhomNguoiNhanID.Split(',');
                foreach (var itemNguoiNhan in arrNhomNguoiNhan)
                {
                    if (!string.IsNullOrEmpty(itemNguoiNhan))
                    {
                        objNhomNguoiNhan = _nhomNguoiDungService.GetById(Convert.ToInt32(itemNguoiNhan));
                        if (objNhomNguoiNhan != null)
                        {
                            nguoiNhanTrongNhomStr += objNhomNguoiNhan.ListNguoiDungThuocNhomID;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(nguoiNhanTrongNhomStr))
            {
                arrNguoiNhanTrongNhom = nguoiNhanTrongNhomStr.Split(',');
                if (arrNguoiNhanTrongNhom.Length > 1)
                {
                    arrNguoiNhanTrongNhom = arrNguoiNhanTrongNhom.Where(val => val != "").ToArray();
                    arrNguoiNhanTrongNhom = arrNguoiNhanTrongNhom.Distinct().ToArray();
                }
            }
            if (arrNguoiNhanTrongNhom[0] == null && (string.IsNullOrEmpty(oViewModel.LstNguoiNhanID) || oViewModel.LstNguoiNhanID.Equals(",,")))
            {
                message += "Phải có ít nhất 1 người nhận thông báo!</br>";
            }
            return message;
        }
        [HttpGet]
        public ActionResult Detail(int id = 0,string position = "")
        {
            var obj = new ThongBaoViewModel();
            try
            {
                obj = _thongBaoService.GetById(id);
                if (obj != null)
                {
                    obj.LstNguoiNhan = GetHoVaTenVaTrangThai(obj.IsDaGui, obj.LstNguoiNhanID, obj.LstNguoiNhanDaNhanMoiNhatID, obj.LstNguoiNhanDaXemID);
                    obj.LstNhomNguoiNhan = GetTenNhomNguoiDung(obj.LstNhomNguoiNhanID);
                    //File
                    var lstFile = _thongBaoAttachService.GetList(g => g.ThongBaoID == id && g.Code == "filethongbao");
                    if (lstFile != null && lstFile.Count > 0)
                    {
                        var arrLink = lstFile.Select(g => g.LinkFile);
                        var arrName = lstFile.Select(g => g.NameFile);
                        var arrSize = lstFile.Select(g => g.Size);
                        var arrReplaceName = lstFile.Select(g => g.ReplaceName);
                        TempData["group_link"] = string.Join(",", arrLink);
                        TempData["group_name"] = string.Join("|", arrName);
                        TempData["group_size"] = string.Join("|", arrSize);
                        TempData["group_replacename"] = string.Join("|", arrReplaceName);
                    }
                    if (position == "right")
                    {
                        if (!string.IsNullOrEmpty(obj.LstNguoiNhanID) && obj.LstNguoiNhanID.Contains("," + SessionInfo.CurrentUser.ID + ",") && (string.IsNullOrEmpty(obj.LstNguoiNhanDaXemID) || !string.IsNullOrEmpty(obj.LstNguoiNhanDaXemID) && !obj.LstNguoiNhanDaXemID.Contains("," + SessionInfo.CurrentUser.ID + ",")))
                        {
                            if (string.IsNullOrEmpty(obj.LstNguoiNhanDaXemID))
                                obj.LstNguoiNhanDaXemID = "," + SessionInfo.CurrentUser.ID.ToString() + ",";
                            else
                                obj.LstNguoiNhanDaXemID += SessionInfo.CurrentUser.ID.ToString() + ",";
                            _thongBaoService.Update(obj);
                        }
                        return View("~/Views/ThongBao/DetailDashBoard.cshtml", obj);
                    }
                    return View("~/Views/ThongBao/Detail.cshtml", obj);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "ThongBao");
            }
            return RedirectToAction("Index", "ThongBao");
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
            var obj = _thongBaoService.GetById(id ?? 0);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                //obj.TrangThai = !obj.TrangThai;
                isSuccess = _thongBaoService.Update(obj);
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
                var checkDel = _thongBaoService.Delete(id);
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
        public JsonResult Send(int id)
        {
            // 1: xóa thành công || -1: Có lỗi xảy ra khi xóa
            var isSuccess = -1;
            var check = true;
            var arrNguoiNhanTrongNhom = new string[10000];
            try
            {
                var objThongBao = _thongBaoService.GetById(id);
                if (objThongBao != null)
                {
                    var nguoiNhanTrongNhomStr = "";
                    var objNhomNguoiNhan = new NhomNguoiDungViewModel();
                    //lây ra người dùng trong nhóm rồi lọc các người dùng trùng
                    if (!string.IsNullOrEmpty(objThongBao.LstNhomNguoiNhanID) && objThongBao.LstNhomNguoiNhanID.Contains(","))
                    {
                        var arrNhomNguoiNhan = objThongBao.LstNhomNguoiNhanID.Split(',');
                        foreach (var itemNguoiNhan in arrNhomNguoiNhan)
                        {
                            if (!string.IsNullOrEmpty(itemNguoiNhan))
                            {
                                objNhomNguoiNhan = _nhomNguoiDungService.GetById(Convert.ToInt32(itemNguoiNhan));
                                if (objNhomNguoiNhan != null)
                                {
                                    nguoiNhanTrongNhomStr += objNhomNguoiNhan.ListNguoiDungThuocNhomID;
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(nguoiNhanTrongNhomStr))
                    {
                        nguoiNhanTrongNhomStr += objThongBao.LstNguoiNhanID;
                        arrNguoiNhanTrongNhom = nguoiNhanTrongNhomStr.Split(',');
                        if (arrNguoiNhanTrongNhom.Length > 1)
                        {
                            arrNguoiNhanTrongNhom = arrNguoiNhanTrongNhom.Where(val => val != "").ToArray();
                            arrNguoiNhanTrongNhom = arrNguoiNhanTrongNhom.Distinct().ToArray();
                            objThongBao.LstNguoiNhanID = "," + string.Join(",", arrNguoiNhanTrongNhom) + ",";
                        }
                    }
                    objThongBao.IsDaGui = true;
                    objThongBao.NgayTao = DateTime.Now;
                    check = _thongBaoService.Update(objThongBao);
                }
                if (check)
                    isSuccess = 1;
            }
            catch (Exception ex)
            {
                isSuccess = -1;
            }
            return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateFillNewestNotification(string lstid)
        {
            // Update lại thông báo mới nhất của người dùng
            try
            {
                var total = 0;
                var objThongBao = new ThongBaoViewModel();
                var lstThongBao = new List<ThongBaoViewModel>();
                //Khi đã đăng nhập
                if (!string.IsNullOrEmpty(lstid) && lstid.Contains(","))
                {
                    var arrID = lstid.Split(',');
                    arrID = arrID.Distinct().ToArray();
                    foreach (var itemID in arrID)
                    {
                        if (!string.IsNullOrEmpty(itemID))
                        {
                            objThongBao = _thongBaoService.GetById(Convert.ToInt32(itemID));
                            if (objThongBao != null)
                            {
                                if (!string.IsNullOrEmpty(objThongBao.LstNguoiNhanID) && objThongBao.LstNguoiNhanID.Contains("," + SessionInfo.CurrentUser.ID + ",") && (string.IsNullOrEmpty(objThongBao.LstNguoiNhanDaNhanMoiNhatID) || (!string.IsNullOrEmpty(objThongBao.LstNguoiNhanDaNhanMoiNhatID) && !objThongBao.LstNguoiNhanDaNhanMoiNhatID.Contains("," + SessionInfo.CurrentUser.ID + ","))))
                                {
                                    if (string.IsNullOrEmpty(objThongBao.LstNguoiNhanDaNhanMoiNhatID))
                                        objThongBao.LstNguoiNhanDaNhanMoiNhatID = "," + SessionInfo.CurrentUser.ID + ",";
                                    else
                                        objThongBao.LstNguoiNhanDaNhanMoiNhatID += SessionInfo.CurrentUser.ID + ",";
                                    _thongBaoService.Update(objThongBao);
                                }
                            }
                        }
                    }
                }
                //Khi đăng nhập vào hiển thị lại count thông báo
                if (string.IsNullOrEmpty(lstid))
                {
                    lstThongBao = _thongBaoService.GetList(x => x.IsDaGui && x.LstNguoiNhanID.Contains("," + SessionInfo.CurrentUser.ID + ",") && (x.LstNguoiNhanDaNhanMoiNhatID == null || (x.LstNguoiNhanDaNhanMoiNhatID != null && !x.LstNguoiNhanDaNhanMoiNhatID.Contains("," + SessionInfo.CurrentUser.ID + ","))) /*&& !x.LstNguoiNhanDaXemID.Contains("," + SessionInfo.CurrentUser.ID + ",")*/, 1, int.MaxValue, ref total);
                    if (lstThongBao != null && lstThongBao.Count > 0)
                    {
                        foreach (var item in lstThongBao)
                        {
                            if (string.IsNullOrEmpty(item.LstNguoiNhanDaNhanMoiNhatID)) item.LstNguoiNhanDaNhanMoiNhatID = "," + SessionInfo.CurrentUser.ID + ",";
                            else item.LstNguoiNhanDaNhanMoiNhatID += SessionInfo.CurrentUser.ID + ",";
                            _thongBaoService.Update(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DaXem(int id = 0, string link = "", string position = "")
        {
            try
            {
                if (id > 0)
                {
                    var objThongBao = _thongBaoService.GetById(id);
                    if (objThongBao != null)
                    {
                        if (!string.IsNullOrEmpty(objThongBao.LstNguoiNhanID) && objThongBao.LstNguoiNhanID.Contains("," + SessionInfo.CurrentUser.ID + ",") && (string.IsNullOrEmpty(objThongBao.LstNguoiNhanDaXemID) || !string.IsNullOrEmpty(objThongBao.LstNguoiNhanDaXemID) && !objThongBao.LstNguoiNhanDaXemID.Contains("," + SessionInfo.CurrentUser.ID + ",")))
                        {
                            if (string.IsNullOrEmpty(objThongBao.LstNguoiNhanDaXemID))
                                objThongBao.LstNguoiNhanDaXemID = "," + SessionInfo.CurrentUser.ID.ToString() + ",";
                            else
                                objThongBao.LstNguoiNhanDaXemID += SessionInfo.CurrentUser.ID.ToString() + ",";
                            _thongBaoService.Update(objThongBao);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { link = link, position = position }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DaXemTatCa(string lstid)
        {
            try
            {
                var objThongBao = new ThongBaoViewModel();
                if (!string.IsNullOrEmpty(lstid) && lstid.Contains(","))
                {
                    var arrID = lstid.Split(',');
                    foreach (var itemID in arrID)
                    {
                        if (!string.IsNullOrEmpty(itemID))
                        {
                            objThongBao = _thongBaoService.GetById(Convert.ToInt32(itemID));
                            if (objThongBao != null)
                            {
                                if (!string.IsNullOrEmpty(objThongBao.LstNguoiNhanID) && objThongBao.LstNguoiNhanID.Contains("," + SessionInfo.CurrentUser.ID + ",") && !objThongBao.LstNguoiNhanDaXemID.Contains("," + SessionInfo.CurrentUser.ID + ","))
                                {
                                    //objThongBao.IsDaXem = true;
                                    _thongBaoService.Update(objThongBao);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
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
                    _thongBaoService.Delete(Convert.ToInt32(item));
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
        #region Tạo + Xóa File
        public void insert_filetoDb(string namefile = "", string linkfile = "", string replaceName = "", int ThongBaoID = 0, string code = "", string size = "")
        {
            if (string.IsNullOrEmpty(replaceName))
                replaceName = namefile;
            var obj = new ThongBaoAttach();
            obj.NameFile = namefile;
            obj.LinkFile = linkfile;
            obj.ReplaceName = replaceName;
            obj.Size = size;
            obj.ThongBaoID = ThongBaoID;
            obj.Code = code;
            obj.TrangThai = true;
            obj.NgayTao = DateTime.Now;
            _thongBaoAttachResp.Add(obj);
        }
        public void delete_filetoDb(int thongbaoID = 0, string code = "")
        {
            if (thongbaoID > 0)
            {
                var lstFile = _thongBaoAttachResp.GetList(g => g.ThongBaoID == thongbaoID);
                if (lstFile != null && lstFile.Count() > 0)
                {
                    var itemId = 0;
                    foreach (var item in lstFile)
                    {
                        itemId = Convert.ToInt32(item.ID);
                        _thongBaoAttachResp.Delete(itemId);
                    }
                }
            }
        }
        #endregion
        public string GetHoVaTenVaTrangThai(bool isDaGui, string lstNguoiNhanID, string LstNguoiNhanDaNhanMoiNhatID, string lstNguoiNhanDaXemID)
        {
            //
            var strITrangThai = "";
            var lstStr = new List<string>();
            var lstNguoiDung = _nguoiDungResp.GetList(x => !string.IsNullOrEmpty(lstNguoiNhanID) && lstNguoiNhanID.Contains("," + x.ID.ToString() + ","));
            if (lstNguoiDung != null && lstNguoiDung.Count > 0)
            {
                if (!isDaGui)
                    return string.Join("</br>", lstNguoiDung.Select(x => x.HoVaTen).ToList());
                else
                {
                    foreach (var item in lstNguoiDung)
                    {
                        strITrangThai = "<i title=\"Đã gửi\" class=\"far fa-check-circle text-warning mr-2\"></i>";
                        if (!string.IsNullOrEmpty(LstNguoiNhanDaNhanMoiNhatID) && LstNguoiNhanDaNhanMoiNhatID.Contains("," + item.ID.ToString() + ","))
                            strITrangThai = "<i title=\"Đã nhận\" class=\"fas fa-check-circle text-primary mr-2\"></i>";
                        if (!string.IsNullOrEmpty(lstNguoiNhanDaXemID) && lstNguoiNhanDaXemID.Contains("," + item.ID.ToString() + ","))
                            strITrangThai = "<i title=\"Đã xem\" class=\"far fa-check-double text-success mr-2\"></i>";
                        lstStr.Add(strITrangThai + item.HoVaTen);
                    }
                    if (lstStr != null && lstStr.Count > 0)
                        return string.Join("</br>", lstStr);
                }
            }
            return "";
        }
        public string GetTenNhomNguoiDung(string lstID)
        {
            var lstNhomNguoiDung = _nhomNguoiDungResp.GetList(x => !string.IsNullOrEmpty(lstID) && lstID.Contains("," + x.ID.ToString() + ","));
            if (lstNhomNguoiDung != null && lstNhomNguoiDung.Count > 0)
            {
                return string.Join("</br>", lstNhomNguoiDung.Select(x => x.TenNhom).ToList());
            }
            return "";
        }
        private List<NguoiDungViewModel> loadNguoiDung(int id)
        {
            //Lấy danh sách nguoi dung
            var lvmNguoiDung = _nguoiDungService.GetListDDL(id);
            //Thêm bản ghi vào vị trí đầu tiên (0)
            if (lvmNguoiDung == null) lvmNguoiDung = new List<NguoiDungViewModel>();
            lvmNguoiDung.Insert(0, new NguoiDungViewModel { ID = 0, HoVaTen = "--- Chọn người dùng ---" });
            return lvmNguoiDung;
        }
        private List<NhomNguoiDungViewModel> loadNhomNguoiDung()
        {
            //Lấy danh sách nhóm đơn vị
            var lvmNhomNguoiDung = _nhomNguoiDungService.GetDdlList(0);
            //Thêm bản ghi vào vị trí đầu tiên (0)
            if (lvmNhomNguoiDung == null) lvmNhomNguoiDung = new List<NhomNguoiDungViewModel>();
            lvmNhomNguoiDung.Insert(0, new NhomNguoiDungViewModel { ID = 0, TenNhom = "--- Chọn nhóm người dùng ---" });
            return lvmNhomNguoiDung;
        }
    }
}