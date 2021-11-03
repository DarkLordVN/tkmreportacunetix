using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TKM.Utils;

namespace TKM.DAO.EntityFramework
{
    public partial class WebsiteScanRepository
    {
        // Add your own data access methods.
        // This file should not get overridden
        public bool insertData(List<WebsiteScan> lsEntity)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(1, 0, 0)))
            {
                var repoWebsite = new EFRepository<Website>(); repoWebsite.UnitOfWork = Repository.UnitOfWork;
                var repoWebsiteItem = new EFRepository<WebsiteItem>(); repoWebsiteItem.UnitOfWork = Repository.UnitOfWork;
                var repoScannedItem = new EFRepository<ScannedItem>(); repoScannedItem.UnitOfWork = Repository.UnitOfWork;
                var repoAlertGroup = new EFRepository<AlertGroup>(); repoAlertGroup.UnitOfWork = Repository.UnitOfWork;
                var repoAffectedItem = new EFRepository<AffectedItem>(); repoAffectedItem.UnitOfWork = Repository.UnitOfWork;
                try
                {
                    var lsAlertGroup = new List<AlertGroup>();
                    var lsCheck = repoAlertGroup.GetAll();
                    if (lsCheck != null && lsCheck.Count() > 0)
                    {
                        lsAlertGroup = lsCheck.ToList();
                    }
                    foreach (var eWebsiteScan in lsEntity)
                    {
                        if (eWebsiteScan != null && eWebsiteScan.eWebsite != null)
                        {
                            var websiteID = 0;
                            var websiteScanID = 0;
                            //check website exist
                            var eWebsiteTemp = repoWebsite.GetByFilter(x => x.Host.Equals(eWebsiteScan.eWebsite.Host) && x.StartURL.Equals(eWebsiteScan.eWebsite.StartURL) && x.ServerOS.Equals(eWebsiteScan.eWebsite.ServerOS));
                            if (eWebsiteTemp != null && eWebsiteTemp.ID > 0)
                            {
                                eWebsiteScan.eWebsite.ID = eWebsiteTemp.ID;
                            }
                            else
                            {
                                repoWebsite.Add(eWebsiteScan.eWebsite);
                                repoWebsite.UnitOfWork.Commit();
                            }
                            websiteID = eWebsiteScan.WebsiteID = eWebsiteScan.eWebsite.ID;
                            //check websitescan exist => insert website scan
                            var eWebsiteScanTemp = Repository.GetByFilter(x => x.WebsiteID == websiteID && x.StartTime.Equals(eWebsiteScan.StartTime));
                            if (eWebsiteScanTemp == null)
                            {
                                //them moi website scan
                                Repository.Add(eWebsiteScan);
                                Repository.UnitOfWork.Commit();
                                if (eWebsiteScan != null && eWebsiteScan.ID > 0)
                                {
                                    websiteScanID = eWebsiteScan.ID;
                                    if (eWebsiteScan.LsWebsiteItem != null && eWebsiteScan.LsWebsiteItem.Count > 0)
                                    {
                                        //OutputLog.WriteOutputLog("Start website item - " + DateTime.Now, "checkTime");
                                        //them moi website item
                                        foreach (var eWebsiteItem in eWebsiteScan.LsWebsiteItem)
                                        {
                                            eWebsiteItem.WebsiteID = websiteID;
                                            eWebsiteItem.LastScanID = websiteScanID;
                                            var eWebsiteItemTemp = repoWebsiteItem.GetByFilter(x => x.WebsiteID == websiteID && x.FullPath.Equals(eWebsiteItem.FullPath));
                                            if (eWebsiteItemTemp != null && eWebsiteItemTemp.ID > 0)
                                            {
                                                eWebsiteItem.ID = eWebsiteItemTemp.ID;
                                            }
                                            else
                                            {
                                                repoWebsiteItem.Add(eWebsiteItem);
                                            }
                                        }
                                        repoWebsiteItem.UnitOfWork.Commit();
                                        //OutputLog.WriteOutputLog("End website item - " + DateTime.Now, "checkTime");
                                        if (eWebsiteScan.LsScannedItem != null && eWebsiteScan.LsScannedItem.Count > 0)
                                        {
                                            //OutputLog.WriteOutputLog("Start scanned item - " + DateTime.Now, "checkTime");
                                            foreach (var eScannedItem in eWebsiteScan.LsScannedItem)
                                            {
                                                var eItemCheck = eWebsiteScan.LsWebsiteItem.Where(x => x.FullPath.Equals(eScannedItem.FullPath));
                                                if (eItemCheck != null && eItemCheck.FirstOrDefault().ID > 0)
                                                {
                                                    eScannedItem.WebsiteID = websiteID;
                                                    eScannedItem.WebiteScanID = websiteScanID;
                                                    eScannedItem.WebsiteItemID = eItemCheck.FirstOrDefault().ID;
                                                    repoScannedItem.Add(eScannedItem);
                                                    ////update website item
                                                    //var eWebsiteItemTemp = repoWebsiteItem.GetById(eScannedItem.WebsiteItemID);
                                                    //eWebsiteItemTemp.Status = eScannedItem.Status;
                                                    //if (eScannedItem.Status.HasValue && !eScannedItem.Status.Value)
                                                    //{
                                                    //    eWebsiteItemTemp.ErrorCount++;
                                                    //}
                                                    //repoWebsiteItem.Update(eWebsiteItemTemp);
                                                    //repoWebsiteItem.UnitOfWork.Commit();
                                                }
                                            }
                                            repoScannedItem.UnitOfWork.Commit();
                                            //OutputLog.WriteOutputLog("End scanned item - " + DateTime.Now, "checkTime");
                                        }
                                        if (eWebsiteScan.LsAlertGroup != null && eWebsiteScan.LsAlertGroup.Count > 0)
                                        {
                                            //OutputLog.WriteOutputLog("Start alertgroup item - " + DateTime.Now, "checkTime");
                                            foreach (var eAlertGroup in eWebsiteScan.LsAlertGroup)
                                            {
                                                //var alertGroupCheck = repoAlertGroup.GetByFilter(x => x.AlertName.Equals(eAlertGroup.AlertName) && x.Severity.Equals(eAlertGroup.Severity));
                                                var alertGroupCheck = lsAlertGroup.Where(x => x.AlertName.Equals(eAlertGroup.AlertName) && x.Severity.Equals(eAlertGroup.Severity));
                                                if (alertGroupCheck != null && alertGroupCheck.Count() > 0 && alertGroupCheck.FirstOrDefault().ID > 0)
                                                {
                                                    eAlertGroup.ID = alertGroupCheck.FirstOrDefault().ID;
                                                }
                                                else
                                                {
                                                    repoAlertGroup.Add(eAlertGroup);
                                                    repoAlertGroup.UnitOfWork.Commit();
                                                    lsAlertGroup.Add(eAlertGroup);
                                                }
                                            }
                                            //OutputLog.WriteOutputLog("End alertgroup item - " + DateTime.Now, "checkTime");
                                            if (eWebsiteScan.LsAffectedItem != null && eWebsiteScan.LsAffectedItem.Count > 0)
                                            {
                                                //OutputLog.WriteOutputLog("Start affected item - " + DateTime.Now, "checkTime");
                                                foreach (var eAffectedItem in eWebsiteScan.LsAffectedItem)
                                                {
                                                    eAffectedItem.WebiteScanID = websiteScanID;
                                                    eAffectedItem.WebsiteID = websiteID;
                                                    if (eAffectedItem.eAlertGroup != null)
                                                    {
                                                        var alertGroupCheck = eWebsiteScan.LsAlertGroup.Where(x => x.AlertName.Equals(eAffectedItem.eAlertGroup.AlertName) && x.Severity.Equals(eAffectedItem.eAlertGroup.Severity));
                                                        if (alertGroupCheck != null && alertGroupCheck.FirstOrDefault().ID > 0)
                                                        {
                                                            eAffectedItem.AlertGroupID = alertGroupCheck.FirstOrDefault().ID;
                                                            repoAffectedItem.Add(eAffectedItem);
                                                        }
                                                    }
                                                }
                                                //OutputLog.WriteOutputLog("Emd affected item - " + DateTime.Now, "checkTime");
                                                repoAffectedItem.UnitOfWork.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Repository.UnitOfWork.Commit();
                        OutputLog.WriteOutputLog("End website - " + DateTime.Now, "checkTime");
                    }
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
        }
        //public bool insertData(List<WebsiteScan> lsEntity)
        //{
        //    createRepository();
        //    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(1, 0, 0)))
        //    {
        //        try
        //        {
        //            foreach (var eWebsiteScan in lsEntity)
        //            {
        //                var websiteID = 0;
        //                var websiteScanID = 0;
        //                eWebsiteScan.eWebsite = RepoWebsite.Add(eWebsiteScan.eWebsite);
        //                RepoWebsite.UnitOfWork.Commit();
        //                if (eWebsiteScan != null && eWebsiteScan.ID > 0)
        //                {
        //                    //them moi website
        //                    websiteID = eWebsiteScan.ID;
        //                    Repository.Add(eWebsiteScan);
        //                    Repository.UnitOfWork.Commit();
        //                    if (eWebsiteScan != null && eWebsiteScan.ID > 0)
        //                    {
        //                        websiteScanID = eWebsiteScan.ID;
        //                        if (eWebsiteScan.LsWebsiteItem != null && eWebsiteScan.LsWebsiteItem.Count > 0)
        //                        {
        //                            //them moi website item
        //                            foreach (var eWebsiteItem in eWebsiteScan.LsWebsiteItem)
        //                            {
        //                                eWebsiteItem.WebsiteID = websiteID;
        //                                eWebsiteItem.LastScanID = websiteScanID;
        //                                RepoWebsiteItem.Add(eWebsiteItem);
        //                                RepoWebsiteItem.UnitOfWork.Commit();
        //                            }
        //                        }
        //                    }
        //                }
        //            }






        //            //if (lentity != null && lentity.Count() > 0)
        //            //{
        //            //    foreach (var entity in lentity)
        //            //    {
        //            //        Repository.Add(entity);
        //            //        Repository.UnitOfWork.Commit();
        //            //    }
        //            //}
        //            scope.Complete();
        //            scope.Dispose();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            scope.Dispose();
        //            OutputLog.WriteOutputLog(ex);
        //            return false;
        //        }
        //    }
        //}
    }
}