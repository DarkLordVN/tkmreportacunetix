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
    public class ThongBaoAttachService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly ThongBaoAttachRepository _thongBaoAttachResp;
        public ThongBaoAttachService()
        {
            _thongBaoAttachResp = new ThongBaoAttachRepository(new EFRepository<ThongBaoAttach>(), oUnitOfWork);
        }
        public List<ThongBaoAttachViewModel> GetList(string tuKhoa, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<ThongBaoAttach, bool>> where;
                //Nếu là tìm từ khóa
                where = x => true;

                var leReturn = _thongBaoAttachResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     null,
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

        public List<ThongBaoAttachViewModel> GetList(Expression<Func<ThongBaoAttach, bool>> where)
        {
            try
            {
                var leReturn = _thongBaoAttachResp.GetList(where);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<ThongBaoAttachViewModel> GetListAll( int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                Expression<Func<ThongBaoAttach, bool>> where = x => true;
                var leReturn = _thongBaoAttachResp.GetList(where, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        
        public ThongBaoAttachViewModel GetById(int id)
        {
            try
            {
                var eReturn = _thongBaoAttachResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public ThongBaoAttachViewModel  GetByFilter(Expression<Func<ThongBaoAttach, bool>> where)
        {
            try
            {
                var leReturn = _thongBaoAttachResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(ThongBaoAttachViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _thongBaoAttachResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(ThongBaoAttachViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _thongBaoAttachResp.Add(entity);
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
                return _thongBaoAttachResp.DeleteList(listId);
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
                return _thongBaoAttachResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
