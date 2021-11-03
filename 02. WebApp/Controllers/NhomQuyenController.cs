using TKM.BLL;
using PagedList;
using TKM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TKM.DAO.EntityFramework;

namespace TKM.WebApp.Controllers
{
    public class NhomQuyenController : BaseController
    {
        private NhomQuyenService _nhomQuyenService;
        private QuyenService _quyenService;
        private NguoiDungService _nguoiDungService;
        public NhomQuyenController()
        {
            if (_nhomQuyenService == null)
            {
                _nhomQuyenService = new NhomQuyenService();
            }
            if (_quyenService == null) _quyenService = new QuyenService();
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
        }
        // GET: NhomQuyen
        /// <summary>
        /// </summary>
        /// <param name="isSuccess">Check cờ: Show và set hiển thị thông báo sau khi xóa thành công</param>
        /// <param name="TenNhom">Filter: Tên nhóm quyền</param>
        /// <param name="MaNhom">Filter: MaNhom</param>
        /// <param name="pnum">Index trang hiện tại</param>
        /// <param name="psize">Số lượng bản ghi hiển thị</param>
        /// <returns></returns>
        public ActionResult Index(string TenNhom, string KyHieu, int? trangThai, int? pnum, int? psize)
        {
            //Tạo và set giá trị biến
            int total = 0;

            //SubString input text seach
            if (!String.IsNullOrEmpty(KyHieu) && KyHieu.Trim().Length > 50)
            {
                KyHieu = KyHieu.Substring(0, 50);
            }
            if (!String.IsNullOrEmpty(TenNhom) && TenNhom.Trim().Length > 250)
            {
                TenNhom = TenNhom.Substring(0, 250);
            }

            List<NhomQuyenViewModel> lstResult;

            //Tạo constructor view model
            var viewModel = new BLL.NhomQuyenListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10
            };

            //Save lại filter truyền vào
            viewModel.TenNhom = TenNhom;
            viewModel.KyHieu = KyHieu;
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
        public ActionResult IndexDetail(NhomQuyenListView viewModel)
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

            if (!string.IsNullOrEmpty(viewModel.TenNhom))
            {
                viewModel.TenNhom = viewModel.TenNhom.Replace("\t", "");
            }
            if (!string.IsNullOrEmpty(viewModel.KyHieu))
            {
                viewModel.KyHieu = viewModel.KyHieu.Replace("\t", "");
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
            var lstResult = _nhomQuyenService.GetList(viewModel.TuKhoa, viewModel.KyHieu, viewModel.TenNhom, viewModel.TrangThai, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);

            viewModel.LstNhomQuyen = lstResult;
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
            NhomQuyenViewModel viewModel = new NhomQuyenViewModel();
            this.LoadInit(viewModel);


            viewModel.ThuTu = _nhomQuyenService.GetMaxThuTu() + 1;
            viewModel.ThuTus = viewModel.ThuTu.ToString();
            viewModel.TrangThai = true;
            var lstDescription = new List<string>();
            foreach (var item in Enum.GetValues(typeof(TKM.Utils.Enums.Quyen)).Cast<TKM.Utils.Enums.Quyen>())
            {
                lstDescription.Add(TKM.Utils.ObjectUtils.GetDescription(item));
            }
            ViewBag.lstDescription = lstDescription;
            return View(viewModel);
        }
        //public ActionResult AddN()
        //{
        //    TempData["AlertData"] = null;
        //    NhomQuyenViewModel viewModel = new NhomQuyenViewModel();
        //    this.LoadInit(viewModel);


        //    viewModel.ThuTu = _NhomQuyenService.GetMaxThuTu() + 1;
        //    viewModel.ThuTus = viewModel.ThuTu.ToString();
        //    viewModel.TrangThai = true;


        //    return View("~/Views/NhomQuyen/AddN.cshtml", viewModel);
        //}

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Add(NhomQuyenViewModel viewModel)
        {
            try
            {
                //viewModel.LstTrangThai = new List<TrangThai>();
                this.LoadInit(viewModel);

                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }
                var message = validateNhomQuyen(viewModel);
                ////Check Trùng mã
                //var isTrung = checkTonTaiKyHieu(viewModel);
                //if (isTrung)
                //    message = "Ký hiệu nhóm quyền đã tồn tại </br>";
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
                    if (_nhomQuyenService.GetCheckTrungThuTu(Convert.ToInt32(viewModel.ThuTu)) && string.IsNullOrEmpty(message))
                    {
                        //message += "Thứ tự đã tồn tại. Vui lòng chọn số khác! </br>";
                        //Lấy ra danh sách để cập nhật lại thứ tự
                        _nhomQuyenService.UpdateThuTu(viewModel.ThuTu.Value);
                    }
                }
                if (string.IsNullOrEmpty(message))
                {

                    //viewModel.MaNhom = GenMaNguoiDUng();
                    //while (!flagMaNhom)
                    //{
                    //    if (checkMaNhom(viewModel.MaNhom))
                    //    {
                    //        viewModel.MaNhom = GenMaNguoiDUng();
                    //    }
                    //    else
                    //    {
                    //        flagMaNhom = true;
                    //    }
                    //}
                    viewModel.NgayTao = DateTime.Now;
                    viewModel.NguoiTaoID = SessionInfo.CurrentUser.ID;
                    viewModel.NguoiTao = SessionInfo.CurrentUser.TenDangNhap;
                    viewModel.LstQuyen = viewModel.LstQuyen.Where(x => x.Checked).ToList();
                    var check = _nhomQuyenService.Add(viewModel);
                    if (check)
                    {
                        int total = 0;
                        var objAddNew = _nhomQuyenService.GetListTopOne(1, 1, ref total, "ID", "asc");
                        var lobjNhomQuyenQuyen = new List<NhomQuyenQuyen>();
                        var objQuyen = new Quyen();
                        var objNhomQuyen = new NhomQuyen();
                        if (objAddNew != null)
                        {
                            var nhomQuyenID = objAddNew.FirstOrDefault().ID;
                            var arrQuyen = Request.Form.GetValues("Quyen");
                            if (arrQuyen != null)
                            {
                                for (int i = 0; i < arrQuyen.Length; i++)
                                {
                                    var arrCheckedQuyen = Request.Form.GetValues("CheckedQuyen_" + arrQuyen[i]);
                                    if (arrCheckedQuyen != null && arrCheckedQuyen.Length > 0)
                                    {
                                        lobjNhomQuyenQuyen.Add(new NhomQuyenQuyen()
                                        {
                                            QuyenID = Convert.ToInt32(arrQuyen[i]),
                                            NhomQuyenID = nhomQuyenID,
                                            QuyenChiTiet = "," + string.Join(",", arrCheckedQuyen) + ","
                                        });
                                    }
                                }
                                check = _nhomQuyenService.AddNhomQuyenQuyen(lobjNhomQuyenQuyen);
                            }
                        }
                        if (check)
                        {
                            TempData["AlertType"] = "alert-success";
                            TempData["AlertData"] = "Thêm mới nhóm quyền thành công";
                            var menuHTML = _quyenService.GetMenuHtmlByUserId(SessionInfo.CurrentUser.ID);
                            Session["menu"] = menuHTML;
                            return RedirectToAction("Index", "NhomQuyen", null);
                        }
                    }
                    message = "Thêm mới không thành công";
                }

                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = message;
                LoadInit(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                LoadInit(viewModel);
                return View(viewModel);
            }
        }

        public ActionResult Update(int? id)
        {
            TempData["AlertData"] = null;
            if (id.HasValue && id.Value > 0)
            {
                NhomQuyenViewModel viewModel = _nhomQuyenService.GetById(id.Value);
                viewModel.ThuTus = viewModel.ThuTu.ToString();
                ViewBag.lstNhomQuyenQuyen = _nhomQuyenService.GetListNhomQuyenQuyen(g => g.NhomQuyenID == id).ToList();
                this.LoadInit(viewModel);
                var lstDescription = new List<string>();
                foreach (var item in Enum.GetValues(typeof(TKM.Utils.Enums.Quyen)).Cast<TKM.Utils.Enums.Quyen>())
                {
                    lstDescription.Add(TKM.Utils.ObjectUtils.GetDescription(item));
                }
                ViewBag.lstDescription = lstDescription;
                return View(viewModel);
            }
            return RedirectToAction("Index", "NhomQuyen", null);
        }

        //public ActionResult UpdateN(int? id)
        //{
        //    TempData["AlertData"] = null;
        //    if (id.HasValue && id.Value > 0)
        //    {
        //        NhomQuyenViewModel viewModel = _NhomQuyenService.GetById(id.Value);
        //        viewModel.ThuTus = viewModel.ThuTu.ToString();
        //        ViewBag.lstNhomQuyenQuyen = _entities.NhomQuyenQuyens.Where(g => g.NhomQuyenID == id).ToList();
        //        this.LoadInit(viewModel);
        //        return View("~/Views/NhomQuyen/UpdateN.cshtml", viewModel);
        //    }
        //    return RedirectToAction("Index", "NhomQuyen", null);
        //}


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(NhomQuyenViewModel viewModel)
        {
            try
            {
                var message = String.Empty;
                this.LoadInit(viewModel);
                //Remove ModelState Validate
                //  ModelState["MatKhau"].Errors.Clear();
                //Check validate ở ViewModel
                if (!ModelState.IsValid)
                {
                    message = "Cập nhật không thành công";
                }
                else
                {
                    //Check validate ở Controller
                    message = validateNhomQuyen(viewModel);
                    ////Check Trùng mã
                    //var isTrung = checkTonTaiKyHieu(viewModel, viewModel.ID);
                    //if (isTrung)
                    //    message = "Ký hiệu nhóm quyền đã tồn tại";
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
                        if (_nhomQuyenService.GetCheckTrungThuTu(Convert.ToInt32(viewModel.ThuTu), viewModel.ID) && string.IsNullOrEmpty(message))
                        {
                            //message += "Thứ tự đã tồn tại. Vui lòng chọn số khác! </br>";
                            //Lấy ra danh sách để cập nhật lại thứ tự
                            _nhomQuyenService.UpdateThuTu(viewModel.ThuTu.Value);
                        }
                    }
                    if (string.IsNullOrEmpty(message))
                    {
                        //Chỉ update những trường có thể thay đổi
                        var oViewModel = _nhomQuyenService.GetById(viewModel.ID);
                        oViewModel.TenNhom = viewModel.TenNhom;
                        oViewModel.KyHieu = viewModel.KyHieu;
                        oViewModel.ThuTu = viewModel.ThuTu;
                        oViewModel.TrangThai = viewModel.TrangThai;
                        oViewModel.LstQuyen = viewModel.LstQuyen.Where(x => x.Checked).ToList();
                        oViewModel.NguoiCapNhatID = SessionInfo.CurrentUser.ID;
                        oViewModel.NguoiCapNhat = SessionInfo.CurrentUser.TenDangNhap;
                        oViewModel.NgayCapNhat = DateTime.Now;
                        //Update người dùng vào db
                        var check = _nhomQuyenService.Update(oViewModel);
                        if (check)
                        {
                            var lobjNhomQuyenQuyen = new List<NhomQuyenQuyen>();
                            if (oViewModel != null)
                            {
                                var arrQuyen = Request.Form.GetValues("Quyen");
                                if (arrQuyen != null)
                                {
                                    for (int i = 0; i < arrQuyen.Length; i++)
                                    {
                                        var arrCheckedQuyen = Request.Form.GetValues("CheckedQuyen_" + arrQuyen[i]);
                                        if (arrCheckedQuyen != null && arrCheckedQuyen.Length > 0)
                                        {
                                            lobjNhomQuyenQuyen.Add(new NhomQuyenQuyen()
                                            {
                                                QuyenID = Convert.ToInt32(arrQuyen[i]),
                                                NhomQuyenID = oViewModel.ID,
                                                QuyenChiTiet = "," + string.Join(",", arrCheckedQuyen) + ","
                                            });
                                        }
                                    }
                                    check = _nhomQuyenService.UpdateNhomQuyenQuyen(lobjNhomQuyenQuyen);
                                }
                            }
                            if (check)
                            {
                                TempData["AlertType"] = "alert-success";
                                TempData["AlertData"] = "Cập nhật nhóm quyền thành công";
                                var menuHTML = _quyenService.GetMenuHtmlByUserId(SessionInfo.CurrentUser.ID);
                                Session["menu"] = menuHTML;
                                return RedirectToAction("Index", "NhomQuyen", null);
                            }
                        }
                        message = "Cập nhật không thành công";
                    }
                }
                //Show lỗi 
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = message;
                viewModel.MaNhom = "ShowErrorPopup()";
                ViewBag.lstNhomQuyenQuyen = _nhomQuyenService.GetListNhomQuyenQuyen(g => g.NhomQuyenID == viewModel.ID).ToList();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertData"] = ex.Message;
                ViewBag.lstNhomQuyenQuyen = _nhomQuyenService.GetListNhomQuyenQuyen(g => g.NhomQuyenID == viewModel.ID).ToList();
                return View(viewModel);
            }
        }
        [HttpGet]
        public ViewResult Detail(int id = 0)
        {
            var obj = new NhomQuyenViewModel();
            obj = _nhomQuyenService.GetById(id);
            return View("~/Views/NhomQuyen/Detail.cshtml", obj);
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
            var obj = _nhomQuyenService.GetById(id ?? 0);
            if (obj == null)
            {
                message = "Bản ghi đã bị xóa";
                return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                obj.TrangThai = !obj.TrangThai;
                isSuccess = _nhomQuyenService.Update(obj);
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
                //Check xem có người dung nào sử dụng nhóm này không?
                var lstNguoiDungSDNhom = _nguoiDungService.GetList(x => x.NhomQuyenID == id).ToList();
                if (lstNguoiDungSDNhom != null && lstNguoiDungSDNhom.Count > 0)
                    isSuccess = 0;
                else
                {
                    checkDel = _nhomQuyenService.Delete(id);
                    if (checkDel)
                        isSuccess = 1;
                    else
                        isSuccess = 0;
                }
            }
            catch (Exception)
            {
                isSuccess = -1;
            }
            return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }


        private string validateNhomQuyen(NhomQuyenViewModel viewModel)
        {
            return String.Empty;
        }
        private bool checkTonTaiKyHieu(NhomQuyenViewModel viewModel, int id = 0)
        {
            int total = 0;
            if (_nhomQuyenService.GetListKyHieu(viewModel.KyHieu, 0, 0, ref total, id).Count > 0)
            {
                return true;
            }
            return false;
        }
        private void LoadInit(NhomQuyenViewModel viewModel)
        {
            if (viewModel.LstQuyen == null || viewModel.LstQuyen.Count == 0)
            {
                var lstQuyen = _quyenService.GetDdlList(0, 0, string.Empty);
                viewModel.LstQuyen = lstQuyen;
                if (viewModel.LstQuyenIdChecked != null && viewModel.LstQuyenIdChecked.Count > 0) lstQuyen.ForEach(x => { if (viewModel.LstQuyenIdChecked.Contains(x.ID)) x.Checked = true; });
            }
        }


    }
}
