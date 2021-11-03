using TKM.BLL;
using TKM.DAO;
using TKM.DAO.EntityFramework;
using TKM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Services
{
    public class NguoiDungService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly NguoiDungRepository _nguoiDungResp;
        private readonly NhomQuyenRepository _nhomQuyenResp;
        public NguoiDungService()
        {
            _nguoiDungResp = new NguoiDungRepository(new EFRepository<NguoiDung>(), oUnitOfWork);
            _nhomQuyenResp = new NhomQuyenRepository(new EFRepository<NhomQuyen>(), oUnitOfWork);
        }

        public bool DoLogin(string username, string password, ref NguoiDungViewModel acc, ref string strLocked)
        {
            try
            {
                var eReturn = new NguoiDung();
                var success = _nguoiDungResp.DoLogin(username, password, ref eReturn, ref strLocked);
                acc = eReturn.ToModel();
                return success;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool CheckTrungTenDangNhap(string tendangnhap)
        {
            return _nguoiDungResp.CheckByFilter(x => x.TenDangNhap.ToLower().Trim().Equals(tendangnhap.ToLower().Trim()));
        }
        public List<NguoiDungDetailListView> GetList(ref List<NguoiDungViewModel> fullDataVM, string tuKhoa, string hoTen, int? donViID, int? chucVuID, bool? trangThai, int pageIndex, int pageSize, ref int totalItem, ref string message, string columnName, string orderBy)
        {
            try
            {
                var lvmResult = new List<NguoiDungDetailListView>();

                Expression<Func<NguoiDung, bool>> where;
                where = x => x.IsDeleted == false
                  && (!string.IsNullOrEmpty(tuKhoa) ? x.HoVaTen.ToLower().Contains(tuKhoa.ToLower()) || x.Email.ToLower().Contains(tuKhoa.ToLower()) || x.TenDangNhap.ToLower().Contains(tuKhoa.ToLower()) : true)
                   && (!string.IsNullOrEmpty(hoTen) ? hoTen.Trim().ToLower().Contains(x.HoVaTen.ToLower().Trim()) : true)
                   && ((donViID.HasValue && donViID.Value > 0) ? x.DonViID == donViID.Value : true)
                   && ((chucVuID.HasValue && chucVuID.Value > 0) ? x.ChucVuID == chucVuID.Value : true)
                   && (trangThai != null ? x.TrangThai == trangThai : true);

                var leReturn = _nguoiDungResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") 
                            : (x.NgayTao.HasValue ? x.NgayTao.Value.ToString("yyyyMMddHHmmss") : x.ID.ToString())) :
                         columnName.Equals("TenDangNhap") ? x.TenDangNhap :
                         columnName.Equals("HoVaTen") ? x.HoVaTen :
                         columnName.Equals("NhomQuyenID") ? x.NhomQuyenID.ToString() :
                         columnName.Equals("ChucVuID") ? x.ChucVuID.ToString() :
                         columnName.Equals("DonViID") ? x.DonViID.ToString() : (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : x.NgayTao.Value.ToString("yyyyMMddHHmmss"))),
                    //Order by
                    (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc") || string.IsNullOrEmpty(columnName)) ? true : false);
                var fullData = _nguoiDungResp.GetList(where, 0, 0, ref totalItem,
                     //Filter theo column
                     x => (null),
                     //Order by
                     string.IsNullOrEmpty(orderBy) ? true : orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                fullDataVM = fullData.ToListModel();
                if (fullDataVM != null && fullDataVM.Count > 0)
                {
                    foreach (var item in fullDataVM)
                    {
                        item.TenNhomQuyen = getTenNhomQuyen(item.NhomQuyenList);
                    }
                }
                if (leReturn != null && leReturn.Count > 0)
                {
                    foreach (var entity in leReturn)
                    {
                        var vm = new NguoiDungDetailListView();
                        vm.ID = entity.ID;
                        vm.HoVaTen = entity.HoVaTen;
                        vm.TenDangNhap = entity.TenDangNhap;
                        vm.AnhDaiDien = entity.AnhDaiDien;
                        vm.Email = entity.Email;
                        vm.ChucVuID = entity.ChucVuID;
                        vm.DonViID = entity.DonViID;
                        vm.NhomQuyenID = entity.NhomQuyenID;
                        vm.TenNhomQuyen = getTenNhomQuyen(entity.NhomQuyenList);
                        vm.TrangThai = entity.TrangThai;
                        lvmResult.Add(vm);
                    }
                }
                return lvmResult;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<NguoiDungViewModel> GetList(Expression<Func<NguoiDung, bool>> where)
        {
            try
            {
                var leReturn = _nguoiDungResp.GetList(where);
                var lst = leReturn.ToListModel();
                return lst;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public string GetNguoiDungHtml(Expression<Func<NguoiDung, bool>> where, int vanBanId, int khoaChaID = 0)
        {
            try
            {
                var lstChecked = new List<int>();
                var leResult = _nguoiDungResp.GetList(where);
                StringBuilder strPhanCap = new StringBuilder();
                strPhanCap.Append("<ul class=\"mb-1 pl-3 pb-2\">");
                if (vanBanId > 0)
                {
                    //lstChecked = _vanBanDuThaoDiGopYResp.GetList(x => x.VanBanId == vanBanId && x.NguoiGopYID.HasValue).Select(x => x.NguoiGopYID.Value).ToList();
                }
                BuildHtml(khoaChaID, leResult.ToListModel(), lstChecked, strPhanCap, 0);
                strPhanCap.Append("</ul>");
                return strPhanCap.ToString();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<NguoiDung> GetListMaNhom(string TenDangNhap)
        {
            try
            {
                int totalItem = 0;
                var lvmResult = new List<NguoiDungDetailListView>();
                Expression<Func<NguoiDung, bool>> where = x => (!string.IsNullOrEmpty(TenDangNhap) ? TenDangNhap.ToLower().Trim().Equals(x.TenDangNhap.ToLower().Trim()) : false) && (x.IsDeleted == false);
                var leReturn = _nguoiDungResp.GetList(where, 0, 0, ref totalItem);

                return leReturn;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<NguoiDung> GetListMaNhom(int? NhomQuyenId, int pageIndex, int pageSize, ref int totalItem, ref string message)
        {
            try
            {
                var lvmResult = new List<NguoiDungDetailListView>();
                Expression<Func<NguoiDung, bool>> where = x => (x.IsDeleted == false);
                var leReturn = _nguoiDungResp.GetList(where, pageIndex, pageSize, ref totalItem);

                return leReturn;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }


        public List<NguoiDungViewModel> GetListDDL(int userId = 0)
        {
            try
            {
                var message = string.Empty;
                var totalItem = 0;
                var lvmResult = new List<NguoiDungViewModel>();
                Expression<Func<NguoiDung, bool>> where = x => (userId > 0 ? x.ID != userId : true && x.IsDeleted == false && x.TrangThai);
                var leReturn = _nguoiDungResp.GetList(where, 0, 0, ref totalItem);
                if (leReturn != null && leReturn.Count > 0)
                {
                    foreach (var entity in leReturn)
                    {
                        var vm = new NguoiDungViewModel();
                        vm.ID = entity.ID;
                        vm.HoVaTen = entity.HoVaTen;
                        vm.TenDangNhap = entity.TenDangNhap;
                        vm.TrangThai = entity.TrangThai;
                        vm.DonViID = entity.DonViID ?? 0;
                        lvmResult.Add(vm);
                    }
                }
                return lvmResult;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<NguoiDungViewModel> GetListDDLCVTen()
        {
            try
            {
                var message = string.Empty;
                var totalItem = 0;
                var lvmResult = new List<NguoiDungViewModel>();
                Expression<Func<NguoiDung, bool>> where = x => (x.IsDeleted == false);
                var leReturn = _nguoiDungResp.GetList(where, 0, 0, ref totalItem);
                if (leReturn != null && leReturn.Count > 0)
                {
                    foreach (var entity in leReturn)
                    {
                        var vm = new NguoiDungViewModel();
                        vm.ID = entity.ID;
                        vm.HoVaTen = entity.HoVaTen;
                        vm.TenDangNhap = entity.TenDangNhap;
                        vm.ChucVuID = entity.ChucVuID.Value;
                        vm.DonViID = entity.DonViID.Value;
                        vm.TrangThai = entity.TrangThai;
                        lvmResult.Add(vm);
                    }
                }
                return lvmResult;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public int GetMaxThuTu()
        {
            try
            {
                return _nguoiDungResp.GetList(x => x.TrangThai).Select(x => x.ThuTu).Max() ?? 0;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return 0;
            }
        }
        public bool GetCheckTrungThuTu(int so, int id = 0)
        {
            try
            {
                return _nguoiDungResp.CheckByFilter(x => (id > 0 ? x.ID == id : true) && x.ThuTu == so);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public NguoiDungViewModel GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    var eReturn = _nguoiDungResp.GetById(id);
                    var vm = eReturn.ToModel();
                    vm.TenNhomQuyenList = getTenNhomQuyen(vm.NhomQuyenList);
                    return vm;
                }
                return null;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<NguoiDungViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<NguoiDung, bool>> where = x => x.IsDeleted == false;

                var leReturn = _nguoiDungResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => x.ID);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }

        }
        public NguoiDungViewModel GetByFilter(Expression<Func<NguoiDung, bool>> where)
        {
            try
            {
                var eReturn = _nguoiDungResp.GetByFilter(where);
                var vm = eReturn.ToModel();
                vm.TenNhomQuyenList = getTenNhomQuyen(vm.NhomQuyenList);
                return vm;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(NguoiDungViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _nguoiDungResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(NguoiDungViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                entity.NgayTao = DateTime.Now;
                return _nguoiDungResp.Add(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public NguoiDungViewModel AddGetObject(NguoiDungViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                entity.NgayTao = DateTime.Now;
                return _nguoiDungResp.AddGetObject(entity).ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public bool DeleteList(List<int> listId)
        {
            try
            {
                return _nguoiDungResp.DeleteList(listId);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                return _nguoiDungResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool CheckPermision(int action, int role, int nguoiDungId)
        {
            var eCheck = _nguoiDungResp.GetByFilter(x => x.TenDangNhap.Equals(""));
            if (eCheck.ID > 0) return false;
            return true;
        }

        public NguoiDungViewModel GetNguoiDungById(int id)
        {
            var eResult = _nguoiDungResp.GetById(id);
            NguoiDungViewModel re = new NguoiDungViewModel();
            re.ID = eResult.ID;
            re.HoVaTen = eResult.HoVaTen;
            return re;
        }

        #region private Methods
        private void BuildHtml(int khoaChaId, List<NguoiDungViewModel> lstAll, List<int> lstChecked, StringBuilder strBuilder, int dataLevel, int index = 0)
        {
            var results = lstAll.GroupBy(x => x.DonViID).Where(x => x.FirstOrDefault().KhoaChaIDDonVi == khoaChaId);
            if (results.Any())
            {
                foreach (var itemGroup in results)
                {
                    strBuilder.Append("<li>");
                    strBuilder.Append("<i class=\"fas fa-angle-right rotate" + (dataLevel == 0 ? " down" : "") + "\"></i> " + itemGroup.FirstOrDefault().TenDonVi);
                    strBuilder.Append("<ul class=\"nested" + (dataLevel == 0 ? " active" : "") + "\">");
                    foreach (var item in itemGroup)
                    {
                        strBuilder.Append("<li>");
                        strBuilder.Append("<input class=\"form-check-input\" type=\"checkbox\" name=\"LstNguoiDung[" + index + "].TrangThai\" id=\"" + index + "\" value=\"true\"" + (lstChecked.Contains(item.ID) ? "checked disabled" : "") + ">");
                        strBuilder.Append("<label class=\"form-check-label\" for=\"" + index + "\">" + item.ChucVuVaTen + "</label>");
                        strBuilder.Append("<input name=\"LstNguoiDung[" + index + "].TrangThai\" type=\"hidden\">");
                        strBuilder.Append("<input name=\"LstNguoiDung[" + index + "].ID\" type=\"hidden\" value=\"" + item.ID + "\">");
                        strBuilder.Append("</li>");
                        index++;
                    }
                    BuildHtml(itemGroup.Key, lstAll, lstChecked, strBuilder, dataLevel + 1, index);
                    strBuilder.Append("</ul>");
                    if (dataLevel == 0) strBuilder.Append("</li>");
                }
            }
        }

        private string getTenNhomQuyen(string nhomQuyenStr)
        {
            var tenNhomQuyen = string.Empty;
            if (!string.IsNullOrEmpty(nhomQuyenStr))
            {
                var nhomQuyenList = nhomQuyenStr.Split(',');
                var str = "";
                foreach (var lstNhomQuyen in nhomQuyenList)
                {
                    if (!string.IsNullOrEmpty(lstNhomQuyen))
                    {
                        var nhomQuyen = _nhomQuyenResp.GetById(int.Parse(lstNhomQuyen));
                        if (nhomQuyen != null && nhomQuyen.ID > 0)
                        {
                            str += nhomQuyen.TenNhom + "<br/>";
                        }
                    }
                }
                tenNhomQuyen += str;
            }
            return tenNhomQuyen;
        }
        #endregion
    }
}
