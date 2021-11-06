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
    public class AffectedItemService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly AffectedItemRepository _AffectedItemResp;
        private readonly v_AffectedItemRepository _vAffectedItemRepo;
        public AffectedItemService()
        {
            _AffectedItemResp = new AffectedItemRepository(new EFRepository<AffectedItem>(), oUnitOfWork);
            _vAffectedItemRepo = new v_AffectedItemRepository(new EFRepository<v_AffectedItem>(), oUnitOfWork);
        }
        public List<AffectedItemViewModel> GetList(string tuKhoa, int pageIndex, int pageSize, ref int totalItem, ref string error, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<AffectedItem, bool>> where;
                where = x =>
                   (!string.IsNullOrEmpty(tuKhoa) ? x.Detail.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true);
                //Đổ dữ liệu
                var leReturn = _AffectedItemResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? x.DateCreated.Value.ToString("yyyyMMddHHmmss") :
                     columnName.Equals("ScanPath") ? x.ScanPath.ToString() : x.DateCreated.Value.ToString("yyyyMMddHHmmss")),
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

        public List<AffectedItemViewModel> GetList(Expression<Func<AffectedItem, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var leReturn = _AffectedItemResp.GetList(where, pageIndex, pageSize, ref totalItem, x => x.DateCreated, true);
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
        public List<AffectedItemViewModel> GetListForWebsiteScan(string tuKhoa, int webSiteScannedID, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy, int tab = 0)
        {
            try
            {
                var lsReturn = new List<AffectedItemViewModel>();
                Expression<Func<v_AffectedItem, bool>> where;
                where = x => x.WebiteScanID == webSiteScannedID;
                //Đổ dữ liệu
                var leResult = _vAffectedItemRepo.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? x.ScanPath : (columnName.Equals("Severity") ? x.SeverityNum.ToString() : (columnName.Equals("AlertName") ? x.AlertName : x.ScanPath))),
                     //Order by
                     string.IsNullOrEmpty(orderBy) ? true : orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                if(leResult != null && leResult.Count > 0)
                {
                    foreach(var item in leResult)
                    {
                        var vm = new AffectedItemViewModel() { ScanPath = item.ScanPath};
                        vm.vmAlertGroup = new AlertGroupViewModel()
                        {
                            AlertName = item.AlertName,
                            Severity = item.Severity
                        };
                        lsReturn.Add(vm);
                    }
                }
                return lsReturn;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<AffectedItemViewModel> GetListFive(Expression<Func<AffectedItem, bool>> where)
        {
            try
            {
                var total = 0;
                var leReturn = _AffectedItemResp.GetList(where, 1, 5, ref total, x => x.DateCreated, true);
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

        public List<AffectedItemViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                //Expression<Func<AffectedItem, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _AffectedItemResp.GetList(null, pageIndex, pageSize, ref totalItem,
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
        public List<AffectedItemViewModel> GetListAll(int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                //Expression<Func<AffectedItem, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _AffectedItemResp.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public AffectedItemViewModel GetByFilter(Expression<Func<AffectedItem, bool>> where)
        {
            try
            {
                var leReturn = _AffectedItemResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public AffectedItemViewModel GetById(int id)
        {
            try
            {
                var eReturn = _AffectedItemResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(AffectedItemViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _AffectedItemResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(AffectedItemViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _AffectedItemResp.Add(entity);
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
                return _AffectedItemResp.DeleteList(listId);
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
                return _AffectedItemResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
