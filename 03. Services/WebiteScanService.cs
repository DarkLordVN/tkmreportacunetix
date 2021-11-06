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
using System.Transactions;

namespace TKM.Services
{
    public class WebsiteScanService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly WebsiteRepository _WebsiteRepo;
        private readonly WebsiteScanRepository _WebsiteScanRepo;
        private readonly WebsiteItemRepository _WebsiteItemRepo;
        private readonly AlertGroupRepository _AlertGroupRepo;
        private readonly AffectedItemRepository _AffectedItemRepo;
        private readonly ScannedItemRepository _ScannedItemRepo;
        public WebsiteScanService()
        {
            _WebsiteRepo = new WebsiteRepository(new EFRepository<Website>(), oUnitOfWork);
            _WebsiteScanRepo = new WebsiteScanRepository(new EFRepository<WebsiteScan>(), oUnitOfWork);
            _WebsiteItemRepo = new WebsiteItemRepository(new EFRepository<WebsiteItem>(), oUnitOfWork);
            _AlertGroupRepo = new AlertGroupRepository(new EFRepository<AlertGroup>(), oUnitOfWork);
            _AffectedItemRepo = new AffectedItemRepository(new EFRepository<AffectedItem>(), oUnitOfWork);
            _ScannedItemRepo = new ScannedItemRepository(new EFRepository<ScannedItem>(), oUnitOfWork);
        }
        public List<WebsiteScanViewModel> GetList(string tuKhoa, int websiteID, int pageIndex, int pageSize, ref int totalItem, ref string error, string columnName, string orderBy, int tab = 0)
        {
            try
            {
                Expression<Func<WebsiteScan, bool>> where;
                where = x =>(websiteID > 0 ? x.WebsiteID == websiteID : true) &&
                   (!string.IsNullOrEmpty(tuKhoa) ? x.ScanProfile.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true);
                //Đổ dữ liệu
                var leReturn = _WebsiteScanRepo.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => true,
                     //Order by
                     string.IsNullOrEmpty(orderBy) ? true : orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                var lst = leReturn.ToListModel();
                if(lst != null && lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        item.vmWebsite = _WebsiteRepo.GetById(item.WebsiteID).ToModel();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<WebsiteScanViewModel> GetList(Expression<Func<WebsiteScan, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var leReturn = _WebsiteScanRepo.GetList(where, pageIndex, pageSize, ref totalItem, x => x.DateCreated, true);
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
        public List<WebsiteScanViewModel> GetListFive(Expression<Func<WebsiteScan, bool>> where)
        {
            try
            {
                var total = 0;
                var leReturn = _WebsiteScanRepo.GetList(where, 1, 5, ref total, x => x.DateCreated, true);
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

        public List<WebsiteScanViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                //Expression<Func<WebsiteScan, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _WebsiteScanRepo.GetList(null, pageIndex, pageSize, ref totalItem,
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
        public List<WebsiteScanViewModel> GetListAll(int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                //Expression<Func<WebsiteScan, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _WebsiteScanRepo.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public int GetCountByFilter(Expression<Func<WebsiteScan, bool>> where)
        {
            try
            {
                var leReturn = _WebsiteScanRepo.GetList(where);
                if (leReturn != null && leReturn.Count > 0)
                    return leReturn.Count;
                return 0;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return 0;
            }
        }
        public WebsiteScanViewModel GetByFilter(Expression<Func<WebsiteScan, bool>> where)
        {
            try
            {
                var leReturn = _WebsiteScanRepo.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public WebsiteScanViewModel GetById(int id)
        {
            try
            {
                var eReturn = _WebsiteScanRepo.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(WebsiteScanViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _WebsiteScanRepo.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool Add(List<WebsiteScanViewModel> lsModel)
        {
            if (lsModel != null && lsModel.Count > 0)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(1, 0, 0)))
                {
                    try
                    {
                        var lsEntity = new List<WebsiteScan>();
                        foreach (var vmWebsiteScan in lsModel)
                        {
                            var entity = new WebsiteScan();
                            var eWebsiteScan = vmWebsiteScan.ToEntity();
                            var eWebsite = vmWebsiteScan.vmWebsite.ToEntity();
                            eWebsite.Status = true;
                            entity = vmWebsiteScan.ToEntity();
                            entity.eWebsite = eWebsite;
                            var lsWebsiteItem = vmWebsiteScan.LsWebsiteItem.ToListEntity();
                            entity.LsWebsiteItem = lsWebsiteItem;
                            var lsScanItem = vmWebsiteScan.LsScanItem.ToListEntity();
                            entity.LsScannedItem = lsScanItem;
                            var lsAffectedItem = vmWebsiteScan.LsAffectedItem.ToListEntity();
                            entity.LsAffectedItem = lsAffectedItem;
                            if (vmWebsiteScan.LsAffectedItem != null && vmWebsiteScan.LsAffectedItem.Count > 0)
                            {
                                var lsAlertGroup = new List<AlertGroup>();
                                foreach (var affectedItem in vmWebsiteScan.LsAffectedItem)
                                {
                                    if (affectedItem.vmAlertGroup != null)
                                    {
                                        var eAlertGroup = new AlertGroup();
                                        eAlertGroup.AlertName = affectedItem.vmAlertGroup.AlertName;
                                        eAlertGroup.AlertVariants = affectedItem.vmAlertGroup.AlertVariants;
                                        eAlertGroup.AlertDescription = affectedItem.vmAlertGroup.AlertDescription;
                                        eAlertGroup.Recommendations = affectedItem.vmAlertGroup.Recommendations;
                                        eAlertGroup.Severity = affectedItem.vmAlertGroup.Severity;
                                        eAlertGroup.Status = true;
                                        if (!lsAlertGroup.Contains(eAlertGroup))
                                        {
                                            lsAlertGroup.Add(eAlertGroup);
                                        }
                                    }
                                }
                                entity.LsAlertGroup = lsAlertGroup;
                            }
                            lsEntity.Add(entity);
                        }
                        OutputLog.WriteOutputLog("Start insertData - " + DateTime.Now, "checkTime");
                        _WebsiteScanRepo.insertData(lsEntity);
                        OutputLog.WriteOutputLog("End insertData - " + DateTime.Now, "checkTime");
                        scope.Complete();
                        scope.Dispose();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        OutputLog.WriteOutputLog(ex);
                        return false;
                    }
                }
                //    var entity = model.ToEntity();
                //return _WebsiteScanResp.Add(entity);
            }
            return false;
        }

        public bool Add(WebsiteScanViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _WebsiteScanRepo.Add(entity);
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
                return _WebsiteScanRepo.DeleteList(listId);
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
                return _WebsiteScanRepo.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
