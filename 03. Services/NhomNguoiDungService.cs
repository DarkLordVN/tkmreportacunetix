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
    public class NhomNguoiDungService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly NhomNguoiDungRepository _nhomNguoiDungResp;
        private readonly NguoiDungRepository _nguoiDungResp;
        public NhomNguoiDungService()
        {
            _nhomNguoiDungResp = new NhomNguoiDungRepository(new EFRepository<NhomNguoiDung>(), oUnitOfWork);
            _nguoiDungResp = new NguoiDungRepository(new EFRepository<NguoiDung>(), oUnitOfWork);
        }
        public List<NhomNguoiDungViewModel> GetList(string tuKhoa, string tenNhom, bool? phamViSuDung, bool? trangThai, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<NhomNguoiDung, bool>> where;

                where = x => x.IsDeleted == false
                  && (!string.IsNullOrEmpty(tuKhoa) ? x.TenNhom.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true)
                && (!string.IsNullOrEmpty(tenNhom) ? x.TenNhom.ToLower().Trim().Contains(tenNhom.ToLower().Trim()) : true)
                && (phamViSuDung != null ? x.PhamViSuDung == phamViSuDung : true)
                && (trangThai != null ? x.TrangThai == trangThai : true);

                var leReturn = _nhomNguoiDungResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : x.NgayTao.Value.ToString("yyyyMMddHHmmss")) :
                         columnName.Equals("TenNhom") ? x.TenNhom : (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : x.NgayTao.Value.ToString("yyyyMMddHHmmss"))),
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
        public List<NhomNguoiDungViewModel> GetList(Expression<Func<NhomNguoiDung, bool>> where)
        {
            try
            {
                var leReturn = _nhomNguoiDungResp.GetList(where);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<NhomNguoiDungViewModel> GetListAll( int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                Expression<Func<NhomNguoiDung, bool>> where = x =>  x.IsDeleted == false;
                var leReturn = _nhomNguoiDungResp.GetList(where, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<NhomNguoiDungViewModel> GetDdlList(int id = 0)
        {
            try
            {
                Expression<Func<NhomNguoiDung, bool>> where = x =>
                    x.IsDeleted == false && x.TrangThai && x.ID != id;
                int totalItem = 0;
                var leReturn = _nhomNguoiDungResp.GetList(where, 0, 0, ref totalItem);
                return leReturn.Cast<NhomNguoiDung>().ToList().ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public NhomNguoiDungViewModel GetById(int id)
        {
            try
            {
                var eReturn = _nhomNguoiDungResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(NhomNguoiDungViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _nhomNguoiDungResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(NhomNguoiDungViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _nhomNguoiDungResp.Add(entity);
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
                return _nhomNguoiDungResp.DeleteList(listId);
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
                return _nhomNguoiDungResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
