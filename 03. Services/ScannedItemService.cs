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
    public class ScannedItemService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly ScannedItemRepository _ScannedItemResp;
        public ScannedItemService()
        {
            _ScannedItemResp = new ScannedItemRepository(new EFRepository<ScannedItem>(), oUnitOfWork);
        }
        public List<ScannedItemViewModel> GetList(string tuKhoa, int webSiteScannedID, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy, int tab = 0)
        {
            try
            {
                Expression<Func<ScannedItem, bool>> where;
                where = x => x.WebiteScanID == webSiteScannedID;
                //Đổ dữ liệu
                var leReturn = _ScannedItemResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (!string.IsNullOrEmpty(columnName) && columnName.Equals("Status") ? x.Status.ToString() : x.FullPath),
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

        public List<ScannedItemViewModel> GetList(Expression<Func<ScannedItem, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var leReturn = _ScannedItemResp.GetList(where, pageIndex, pageSize, ref totalItem, x => x.DateCreated, true);
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
        public List<ScannedItemViewModel> GetListFive(Expression<Func<ScannedItem, bool>> where)
        {
            try
            {
                var total = 0;
                var leReturn = _ScannedItemResp.GetList(where, 1, 5, ref total, x => x.DateCreated, true);
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

        public List<ScannedItemViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                //Expression<Func<ScannedItem, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _ScannedItemResp.GetList(null, pageIndex, pageSize, ref totalItem,
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
        public List<ScannedItemViewModel> GetListAll(int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                //Expression<Func<ScannedItem, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _ScannedItemResp.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public ScannedItemViewModel GetByFilter(Expression<Func<ScannedItem, bool>> where)
        {
            try
            {
                var leReturn = _ScannedItemResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public ScannedItemViewModel GetById(int id)
        {
            try
            {
                var eReturn = _ScannedItemResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(ScannedItemViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _ScannedItemResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(ScannedItemViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _ScannedItemResp.Add(entity);
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
                return _ScannedItemResp.DeleteList(listId);
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
                return _ScannedItemResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
