using TKM.BLL;
using TKM.DAO.EntityFramework;
using TKM.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Services
{
    public class HeThongThamSoService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly HeThongThamSoRepository _heThongThamSoResp;


        public HeThongThamSoService()
        {
            _heThongThamSoResp = new HeThongThamSoRepository(new EFRepository<HeThongThamSo>(), oUnitOfWork);
        }

        public List<HeThongThamSoViewModel> GetList(string tuKhoa, string maThamSo, string tenThamSo, string moTa, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<HeThongThamSo, bool>> where;
               
                where = x =>  ((!string.IsNullOrEmpty(tuKhoa) ? x.MaThamSo.ToLower().Trim().Contains(tuKhoa.ToLower().Trim())
                || x.TenThamSo.ToLower().Trim().Contains(tuKhoa.ToLower().Trim())
                || x.MoTa.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true)
                && (!string.IsNullOrEmpty(maThamSo) ? x.MaThamSo.ToLower().Trim().Contains(maThamSo.ToLower().Trim()) : true)
                && (!string.IsNullOrEmpty(tenThamSo) ? x.TenThamSo.ToLower().Trim().Contains(tenThamSo.ToLower().Trim()) : true)
                && (!string.IsNullOrEmpty(moTa) ? x.MoTa.ToLower().Trim().Contains(moTa.ToLower().Trim()) : true)
                
               );

                var leReturn = _heThongThamSoResp.GetList(where, pageIndex, pageSize, ref totalItem,
                    x => (string.IsNullOrEmpty(columnName) ? (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : null) :
                     columnName.Equals("MaThamSo") ? x.MaThamSo :
                     columnName.Equals("TenThamSo") ? x.TenThamSo :
                     columnName.Equals("MoTa") ? x.MoTa : (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss"))),
                     //Order by
                     orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                var lst = leReturn.ToListModel();                

                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<HeThongThamSoViewModel> GetList(Expression<Func<HeThongThamSo, bool>> where)
        {
            try
            {
                var leReturn = _heThongThamSoResp.GetList(where);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public HeThongThamSoViewModel GetById(int id)
        {
            try
            {
                var eReturn = _heThongThamSoResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public HeThongThamSoViewModel GetByFilter(Expression<Func<HeThongThamSo, bool>> where)
        {
            try
            {
                var leReturn = _heThongThamSoResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public bool Add(HeThongThamSoViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _heThongThamSoResp.Add(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Update(HeThongThamSoViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _heThongThamSoResp.Update(entity);
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
                return _heThongThamSoResp.DeleteList(listId);
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
                return _heThongThamSoResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }    
}
