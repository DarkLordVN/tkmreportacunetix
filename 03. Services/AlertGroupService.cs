using TKM.BLL;
using TKM.DAO;
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
    public class AlertGroupService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly AlertGroupRepository _AlertGroupResp;
        private readonly TuDienVietHoaRepository _tuDienVietHoaRepo;
        private readonly NhomNguoiDungRepository _nhomNguoiDungResp;
        public AlertGroupService()
        {
            _AlertGroupResp = new AlertGroupRepository(new EFRepository<AlertGroup>(), oUnitOfWork);
            _tuDienVietHoaRepo = new TuDienVietHoaRepository(new EFRepository<TuDienVietHoa>(), oUnitOfWork);
        }
        public List<AlertGroupViewModel> GetList(string tuKhoa, int pageIndex, int pageSize, ref int totalItem, ref string error, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<AlertGroup, bool>> where;
                where = x =>
                   (!string.IsNullOrEmpty(tuKhoa) ? x.AlertName.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) || x.Severity.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) || x.AlertDescription.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) || x.AlertVariants.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true);
                //Đổ dữ liệu
                var leReturn = _AlertGroupResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? x.AlertName:
                     columnName.Equals("ScanPath") ? x.AlertName.ToString() : x.DateCreated.Value.ToString("yyyyMMddHHmmss")),
                     //Order by
                     string.IsNullOrEmpty(orderBy) ? true : orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                var lst = leReturn.ToListModel();
                return lst;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<AlertGroupViewModel> GetList(Expression<Func<AlertGroup, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var leReturn = _AlertGroupResp.GetList(where, pageIndex, pageSize, ref totalItem, x => x.DateCreated, true);
                if (leReturn != null && leReturn.Count > 0)
                {
                    foreach (var item in leReturn)
                    {

                    }
                    return leReturn.ToListModel();
                }
                return null;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<AlertGroupViewModel> GetListFive(Expression<Func<AlertGroup, bool>> where)
        {
            try
            {
                var total = 0;
                var leReturn = _AlertGroupResp.GetList(where, 1, 5, ref total, x => x.DateCreated, true);
                foreach (var item in leReturn)
                {

                }
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<AlertGroupViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                //Expression<Func<AlertGroup, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _AlertGroupResp.GetList(null, pageIndex, pageSize, ref totalItem,
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
        public List<AlertGroupViewModel> GetListAll(int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                //Expression<Func<AlertGroup, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _AlertGroupResp.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public AlertGroupViewModel GetByFilter(Expression<Func<AlertGroup, bool>> where)
        {
            try
            {
                var leReturn = _AlertGroupResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public AlertGroupViewModel GetById(int id)
        {
            try
            {
                var eReturn = _AlertGroupResp.GetById(id);
                var vmReturn = eReturn.ToModel();
                return vmReturn;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(AlertGroupViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _AlertGroupResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(AlertGroupViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _AlertGroupResp.Add(entity);
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
                return _AlertGroupResp.DeleteList(listId);
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
                return _AlertGroupResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
