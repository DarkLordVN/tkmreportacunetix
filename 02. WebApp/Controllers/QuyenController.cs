using TKM.BLL;
using PagedList;
using TKM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Newtonsoft.Json;
using TKM.DAO.EntityFramework;

namespace TKM.WebApp.Controllers
{
    public class QuyenController : BaseController
    {
        private NhomQuyenService _nhomQuyenService;
        private QuyenService _quyenService;
        public List<SelectListItem> Employees { set; get; }


        public QuyenController()
        {
            if (_quyenService == null)
            {
                _quyenService = new QuyenService();
            }
            if (_nhomQuyenService == null) _nhomQuyenService = new NhomQuyenService();
        }
        // GET: Quyen
        /// <summary>
        /// </summary>
        /// <param name="isSuccess">Check cờ: Show và set hiển thị thông báo sau khi xóa thành công</param>
        /// <param name="tenQuyen">Filter: Tên quyền</param>
        /// <param name="maQuyen">Filter: maQuyen</param>
        /// <param name="pnum">Index trang hiện tại</param>
        /// <param name="psize">Số lượng bản ghi hiển thị</param>
        /// <returns></returns>
        public ActionResult Index( string tenQuyen, string maQuyen, int? trangThai, int? pnum, int? psize)
        {
            //Tạo constructor view model
            var viewModel = new BLL.QuyenListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 20
            };

            //Load phòng ban
            //  viewModel.LstQuyen = _QuyenService.GetList();

            //Save lại filter truyền vào
            viewModel.TenQuyen = tenQuyen;
            viewModel.MaQuyen = maQuyen;
            bool? srTrangThai = null;
            if (trangThai.HasValue)
            {
                if (trangThai == 1)
                    srTrangThai = true;
                else
                    srTrangThai = false;
            }
            viewModel.TrangThai = srTrangThai;
         
            //Gọi đến View
            return View(viewModel);
        }
        public ActionResult IndexDetail(QuyenListView viewModel)
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
                viewModel.TuKhoa = viewModel.TuKhoa.Replace("\t", "");
            }

            if (!string.IsNullOrEmpty(viewModel.TenQuyen))
            {
                viewModel.TenQuyen = viewModel.TenQuyen.Replace("\t", "");
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
            var lstResult = _quyenService.GetList(viewModel.TuKhoa, viewModel.TenQuyen, viewModel.MaQuyen, viewModel.TrangThai, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);

            viewModel.LstQuyen = lstResult;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }
        [HttpPost]
        public string LoadAction(string cname)
        {
            string jsonReturn = "";
            jsonReturn = JsonConvert.SerializeObject(getAction(cname));
            return jsonReturn;
        }
        public ActionResult Add()
        {
            TempData["AlertData"] = null;
            QuyenViewModel viewModel = new QuyenViewModel();
            viewModel.LstQuyenCha = loadQuyenCha(0);
            viewModel.LstController = getController();
            //viewModel.LstAction = getAction();
            viewModel.TrangThai = true;
            viewModel.ThuTu = _quyenService.GetMaxThuTu() + 1;
            viewModel.ThuTus = viewModel.ThuTu.ToString();
            return View(viewModel);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Add(QuyenViewModel viewModel)
        {
            try
            {
                viewModel.LstQuyenCha = loadQuyenCha(0);
                viewModel.LstController = getController();
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }
                viewModel.Controller = viewModel.Controller.Contains("--- ") ? string.Empty : viewModel.Controller;
                viewModel.Action = viewModel.Action.Contains("--- ") ? string.Empty : viewModel.Action;
                var message = validateQuyen(viewModel);
              
                try
                {
                    if (!string.IsNullOrEmpty(viewModel.ThuTus))
                    {
                        viewModel.ThuTu = int.Parse(viewModel.ThuTus);
                    }
                }
                catch (Exception e)
                {
                    message += "Số thứ tự phải là số </br>";
                    ModelState.AddModelError("ThuTus", "Số thứ tự phải là số");
                }
                if (viewModel.ThuTu.HasValue && viewModel.ThuTu.Value <= 0)
                {
                    message += "Số thứ tự phải lớn hơn 0 </br>";
                    ModelState.AddModelError("ThuTus", "Số thứ tự phải lớn hơn 0");
                }
                if (viewModel.ThuTu != null)
                {
                    if (_quyenService.GetCheckTrungThuTu(Convert.ToInt32(viewModel.ThuTu)) && string.IsNullOrEmpty(message))
                    {
                        //message += "Thứ tự đã tồn tại. Vui lòng chọn số khác! </br>";
                        //Lấy ra danh sách để cập nhật lại thứ tự
                        _quyenService.UpdateThuTu(viewModel.ThuTu.Value);
                    }
                }
                if (string.IsNullOrEmpty(message))
                {
                    viewModel.NgayTao = DateTime.Now;
                    viewModel.NguoiTaoID = SessionInfo.CurrentUser.ID;
                    viewModel.NguoiTao = SessionInfo.CurrentUser.TenDangNhap;
                    if(viewModel.ActionCustom != null && viewModel.Action != null &&!viewModel.Action.Equals(viewModel.ActionCustom))
                    {
                        viewModel.Action = viewModel.ActionCustom;
                    }
                    var check = _quyenService.Add(viewModel);
                    if (check)
                    {
                        TempData["AlertType"] = "alert-success";
                        TempData["AlertData"] = "Thêm mới quyền thành công";
                        return RedirectToAction("Index", "Quyen", null);
                    }
                    message = "Thêm mới không thành công";
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
        private string GenMaQuyen()
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

        private bool checkTonTaiMaQuyen(QuyenViewModel viewModel)
        {
            int total = 0;
            if (_quyenService.GetListMaQuyen(viewModel.MaQuyen, 0, 0, ref total).Count > 0)
            {
                return true;
            }
            return false;
        }
        public ActionResult Update(int? id)
        {
            TempData["AlertData"] = null;
            if (id.HasValue && id.Value > 0)
            {
                QuyenViewModel viewModel = _quyenService.GetById(id.Value);
                viewModel.ActionCustom = viewModel.Action;
                if (viewModel != null && viewModel.Action != null && viewModel.Action.Contains("?"))
                {
                    var action = viewModel.Action.Split('?');
                    if(action != null && action.Length > 0)
                    {
                        viewModel.Action = action[0] + "";
                    }
                }
                viewModel.LstQuyenCha = loadQuyenCha(viewModel.ID);
                viewModel.LstController = getController();
                viewModel.LstAction = getAction(viewModel.Controller);
                viewModel.ThuTus = viewModel.ThuTu.ToString();
                return View(viewModel);
            }
            return RedirectToAction("Index", "Quyen", null);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(QuyenViewModel viewModel)
        {
            try
            {
                viewModel.Controller = viewModel.Controller.Contains("---") ? string.Empty : viewModel.Controller;
                viewModel.Action = viewModel.Action.Contains("---") ? string.Empty : viewModel.Action;
                var message = String.Empty;
                viewModel.LstQuyenCha = loadQuyenCha(viewModel.ID);
                viewModel.LstController = getController();
                viewModel.LstAction = getAction(viewModel.Controller);
                //Check validate ở ViewModel
                if (!ModelState.IsValid)
                {
                    message = "Cập nhật không thành công";
                }
                else
                {
                    //Check validate ở Controller
                    message = validateQuyen(viewModel);
                    try
                    {
                        if (!string.IsNullOrEmpty(viewModel.ThuTus))
                        {
                            viewModel.ThuTu = int.Parse(viewModel.ThuTus);
                        }
                    }
                    catch (Exception e)
                    {
                        message += "Số thứ tự phải là số </br>";
                        ModelState.AddModelError("ThuTus", "Số thứ tự phải là số");
                    }
                    if (viewModel.ThuTu < 0)
                    {
                        message += "Số thứ tự phải lớn hơn 0 </br>";
                        ModelState.AddModelError("ThuTus", "Số thứ tự phải lớn hơn 0");
                    }
                    if (viewModel.ThuTu != null)
                    {
                        if (_quyenService.GetCheckTrungThuTu(Convert.ToInt32(viewModel.ThuTu), viewModel.ID) && string.IsNullOrEmpty(message))
                        {
                            //message += "Thứ tự đã tồn tại. Vui lòng chọn số khác! </br>";
                            //Lấy ra danh sách để cập nhật lại thứ tự
                            _quyenService.UpdateThuTu(viewModel.ThuTu.Value);
                        }
                    }
                    if (string.IsNullOrEmpty(message))
                    {
                        //Chỉ update những trường có thể thay đổi
                        var newModel = _quyenService.GetById(viewModel.ID);
                        newModel.KhoaChaId = viewModel.KhoaChaId;
                        newModel.TenQuyen = viewModel.TenQuyen;
                        newModel.Controller = viewModel.Controller;
                        newModel.Action = viewModel.Action;
                        if (viewModel.ActionCustom != null && viewModel.Action != null && !viewModel.Action.Equals(viewModel.ActionCustom))
                        {
                            newModel.Action = viewModel.ActionCustom;
                        }
                        newModel.TrangThai = viewModel.TrangThai;
                        newModel.GhiChu = viewModel.GhiChu;
                        newModel.IconPath = viewModel.IconPath;
                        newModel.IsMenu = viewModel.IsMenu;
                        newModel.ThuTu = viewModel.ThuTu;
                        newModel.NguoiCapNhatID = SessionInfo.CurrentUser.ID;
                        newModel.NguoiCapNhat = SessionInfo.CurrentUser.TenDangNhap;
                        newModel.NgayCapNhat = DateTime.Now;
                        //Update người dùng vào db
                        var check = _quyenService.Update(newModel);
                        if (check)
                        {
                            TempData["AlertType"] = "alert-success";
                            TempData["AlertData"] = "Cập nhật quyền thành công";
                            var menuHTML = _quyenService.GetMenuHtmlByUserId(SessionInfo.CurrentUser.ID);
                            Session["menu"] = menuHTML;
                            return RedirectToAction("Index", "Quyen", null);
                        }
                        message = "Cập nhật không thành công";
                    }
                }
                //Show lỗi 
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
            var obj = new QuyenViewModel();
            obj = _quyenService.GetById(id);
            var tenQuyenCha = "";
            if (obj.KhoaChaId.HasValue && obj.KhoaChaId.Value > 0)
            {
                var objCha = _quyenService.GetByFilter(x => x.ID == obj.KhoaChaId);
                if (objCha != null)
                {
                    tenQuyenCha = objCha.TenQuyen;
                }
            }
            ViewBag.tenQuyenCha = tenQuyenCha;
            return View("~/Views/Quyen/Detail.cshtml", obj);
        }
        [HttpPost]
        public JsonResult onChangeMenu(int? id)
        {
            bool isSuccess = false;
            var message = "";
            if (id == null)
            {
                message = "Id không được để trống";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            var obj = _quyenService.GetById(id ?? 0);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.IsMenu = !obj.IsMenu;
                isSuccess = _quyenService.Update(obj);
                message = isSuccess ? "Thay đổi trạng thái menu thành công" : "Đã có lỗi xảy ra";
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
        public JsonResult onChangeStatus(int? id)
        {
            bool isSuccess = false;
            var message = "";
            if (id == null)
            {
                message = "Id không được để trống";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            var obj = _quyenService.GetById(id ?? 0);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.TrangThai = !obj.TrangThai;
                isSuccess = _quyenService.Update(obj);
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
            var isSuccess = -1;
            bool checkDel = false;
            try
            {
                //Check xem có nhóm người dung nào sử dụng quyền này không?
                var lstNhomSDQuyen = _nhomQuyenService.GetListNhomQuyenQuyen(x => x.QuyenID == id);
                if (lstNhomSDQuyen != null && lstNhomSDQuyen.Count > 0)
                    isSuccess = 0;
                else
                    checkDel = _quyenService.Delete(id);
                if (checkDel)
                    isSuccess = 1;
                else
                    isSuccess = 0;
            }
            catch (Exception)
            {
                isSuccess = -1;
            }
            return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        private string validateQuyen(QuyenViewModel viewModel)
        {
            var errStr = string.Empty;
            //&& string.IsNullOrEmpty(viewModel.Action)
            //if (!string.IsNullOrEmpty(viewModel.Controller) )
            //{
            //    errStr = "Chưa chọn hành động.";
            //}
            return errStr;
        }
        private bool checkMaQuyen(string MaTT)
        {
            //TODO
            return false;
        }
        private List<QuyenDetailViewModel> loadQuyenCha(int currId, int khoaChaId = 0)
        {
            var lstQuyen = _quyenService.GetDdlList(currId, khoaChaId);
            lstQuyen.Insert(0, new QuyenDetailViewModel() { ID = 0, TenQuyen = "--- Thuộc quyền ---" });
            return lstQuyen;
        }

        private List<string> getController()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            //filter controllers
            var lstController = asm.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type) && type.FullName.Contains("TKM.WebApp.Controllers") && !type.FullName.Contains("TKM.WebApp.Controllers.Authenticate") && type.Name != "BaseController" && type.Name != "TestAPIController").OrderBy(ord => ord.Name).Select(x => x.Name.EndsWith("Controller") ? x.Name.Remove(x.Name.Count() - 10, 10)  : x.Name).ToList(); 
            lstController.Insert(0, "--- Chọn điều hướng ---");
            return lstController;
        }

        private List<string> getAction(string cname) {
            Assembly asm = Assembly.GetExecutingAssembly();
            //filters
            var lstAction = asm.GetTypes()
                .Where(type => type.Name.Equals(cname + "Controller")).SelectMany(c => c.GetMethods()).Where(act => act.ReturnType.Name.Equals("ActionResult") && act.CustomAttributes.Count() == 0).OrderBy(ord => ord.Name).Select(name => name.Name).ToList();
            lstAction.Insert(0, "--- Chọn hành động ---");
            return lstAction;
        }
    }
}
