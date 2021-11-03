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
    public class NhomQuyenService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly NhomQuyenRepository _nhomQuyenResp;
        private readonly QuyenRepository _quyenResp;
        public NhomQuyenService()
        {
            _nhomQuyenResp = new NhomQuyenRepository(new EFRepository<NhomQuyen>(), oUnitOfWork);
            _quyenResp = new QuyenRepository(new EFRepository<Quyen>(), oUnitOfWork);
        }

        public List<NhomQuyenViewModel> GetList(Expression<Func<NhomQuyen, bool>> where)
        {
            try
            {
                var leReturn = _nhomQuyenResp.GetList(where);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<NhomQuyenViewModel> GetDdlList(int khoaChaId = 0)
        {
            try
            {
                Expression<Func<NhomQuyen, bool>> where = x => (khoaChaId > 0) ? x.ID == khoaChaId : true && x.IsDeleted == false;
                int totalItem = 0;
                var leReturn = _nhomQuyenResp.GetList(where, 0, 0, ref totalItem);
                var lstAll = leReturn.Cast<object>().ToList();
                var lstBuild = new List<object>();
                var lstTinhThnh = new List<NhomQuyenViewModel>();
                foreach (NhomQuyen item in leReturn)
                {
                    NhomQuyenViewModel modelview = new NhomQuyenViewModel();
                    modelview.MaNhom = item.MaNhom;
                    modelview.TenNhom = item.TenNhom;
                    modelview.NgayCapNhat = item.NgayCapNhat;
                    modelview.ID = item.ID;
                    lstTinhThnh.Add(modelview);
                }
                return lstTinhThnh;
                //TKM.Utils.ObjectUtils.BuildHierachy(khoaChaId, lstAll, ref lstBuild, "PhongBanId", "KhoaChaId", "TenPhongBan", string.Empty);
                //return lstBuild.Cast<PhongBan>().ToList().ToListModel();

            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<NhomQuyenViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<NhomQuyen, bool>> where = x => x.IsDeleted == false;

                var leReturn = _nhomQuyenResp.GetList(where, pageIndex, pageSize, ref totalItem,
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

        public List<NhomQuyenViewModel> GetList(string tuKhoa, string KyHieu, string TenNhom, bool? trangThai, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<NhomQuyen, bool>> where;
                where = x => x.IsDeleted == false
                  && (!string.IsNullOrEmpty(tuKhoa) ? x.KyHieu.ToLower().Trim().Contains(tuKhoa.ToLower().Trim())
                || x.TenNhom.ToLower().Trim().Contains(tuKhoa.ToLower().Trim())
                || x.ThuTu.ToString().ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true)
                && (!string.IsNullOrEmpty(KyHieu) ? x.KyHieu.ToLower().Trim().Contains(KyHieu.ToLower().Trim()) : true)
                && (!string.IsNullOrEmpty(TenNhom) ? x.TenNhom.ToLower().Trim().Contains(TenNhom.ToLower().Trim()) : true)
                && (trangThai != null ? x.TrangThai == trangThai : true);

                var leReturn = _nhomQuyenResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : x.NgayTao.Value.ToString("yyyyMMddHHmmss")) :
                         columnName.Equals("TenNhom") ? x.TenNhom :
                         columnName.Equals("KyHieu") ? x.KyHieu : (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : x.NgayTao.Value.ToString("yyyyMMddHHmmss"))),
                     //Order by
                     orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<NhomQuyenViewModel> GetListKyHieu(string kyHieu, int pageIndex, int pageSize, ref int totalItem, int id = 0)
        {
            try
            {
                Expression<Func<NhomQuyen, bool>> where;
                if (id == 0)
                    where = x => x.IsDeleted == false && ((!string.IsNullOrEmpty(kyHieu)) ? x.KyHieu.ToLower().Trim().Contains(kyHieu.ToLower().Trim()) : true);
                else
                    where = x => x.ID != id && x.IsDeleted == false && ((!string.IsNullOrEmpty(kyHieu)) ? x.KyHieu.ToLower().Trim().Contains(kyHieu.ToLower().Trim()) : true);
                var leReturn = _nhomQuyenResp.GetList(where, pageIndex, pageSize, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<NhomQuyenViewModel> searchNhomQuyen(string TenNhom, string MaNhom, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                Expression<Func<NhomQuyen, bool>> where = x => (!String.IsNullOrEmpty(TenNhom)) ? x.TenNhom.ToLower().Contains(TenNhom.ToLower()) : true && x.IsDeleted == false && (!string.IsNullOrEmpty(MaNhom)) ? x.MaNhom.ToLower().Contains(MaNhom.ToLower()) : true && x.IsDeleted == false;
                var leReturn = _nhomQuyenResp.GetList(where, pageIndex, pageSize, ref totalItem);
                return leReturn.ToListModel();
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
                return _nhomQuyenResp.GetList(x => x.TrangThai).Select(x => x.ThuTu).Max() ?? 0;
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
                return _nhomQuyenResp.CheckByFilter(x => (id > 0 ? x.ID == id : true) && x.ThuTu == so);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public List<int> getQuyenByNhomQuyenId(int id)
        {
            try
            {
                var eReturn = _nhomQuyenResp.GetById(id);
                if(eReturn != null && eReturn.ID > 0) return eReturn.NhomQuyenQuyens.Where(x => x.Quyen != null ? x.Quyen.TrangThai : true).Select(x => x.QuyenID).ToList();
                return null;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public NhomQuyenViewModel GetById(int id)
        {
            try
            {
                var eReturn = _nhomQuyenResp.GetById(id);
                var viewModel = eReturn.ToModel();
                viewModel.LstQuyenIdChecked = eReturn.NhomQuyenQuyens.Select(x => x.QuyenID).ToList();
                return viewModel;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(NhomQuyenViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                var lstQuyenId = new List<int>();
                if (model.LstQuyen != null && model.LstQuyen.Count > 0)
                {
                    lstQuyenId = model.LstQuyen.Select(x => x.ID).ToList();
                }
                return _nhomQuyenResp.Update(entity, lstQuyenId);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool UpdateThuTu(int soThuTu)
        {
            try
            {
                var lstUpdateThuTu = _nhomQuyenResp.GetList(x => x.ThuTu != null && x.ThuTu >= soThuTu).OrderBy(x => x.ThuTu).ToList();
                if (lstUpdateThuTu != null && lstUpdateThuTu.Count() > 0)
                {
                    lstUpdateThuTu.ForEach(x => x.ThuTu = x.ThuTu + 1);
                }
                return _nhomQuyenResp.Update(lstUpdateThuTu);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(NhomQuyenViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                var lstQuyenId = new List<int>();
                if (model.LstQuyen != null && model.LstQuyen.Count > 0)
                {
                    lstQuyenId = model.LstQuyen.Select(x => x.ID).ToList();
                }
                return _nhomQuyenResp.Add(entity, lstQuyenId);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool DeleteList(List<int> listId)
        {
            try
            {
                return _nhomQuyenResp.DeleteList(listId);
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
                return _nhomQuyenResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        #region NhomQuyenQuyen
        public List<NhomQuyenQuyen> GetListNhomQuyenQuyen(Expression<Func<NhomQuyenQuyen, bool>> where)
        {
            try
            {
                return _nhomQuyenResp.GetListNhomQuyenQuyen(where);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool AddNhomQuyenQuyen(List<NhomQuyenQuyen> lstEntity)
        {
            try
            {
                return _nhomQuyenResp.AddNhomQuyenQuyen(lstEntity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool UpdateNhomQuyenQuyen(List<NhomQuyenQuyen> lstEntity)
        {
            try
            {
                return _nhomQuyenResp.UpdateNhomQuyenQuyen(lstEntity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        #endregion
    }
}
