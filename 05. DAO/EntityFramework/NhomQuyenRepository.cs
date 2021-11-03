using System;
using System.Linq;
using System.Collections.Generic;
using System.Transactions;
using TKM.Utils;
using System.Linq.Expressions;

namespace TKM.DAO.EntityFramework
{
    public partial class NhomQuyenRepository
    {
        // Add your own data access methods.
        // This file should not get overridden

        public bool Add(NhomQuyen entity, List<int> lstQuyenId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var quyenResp = new EFRepository<Quyen>(); quyenResp.UnitOfWork = Repository.UnitOfWork;
                var nhomQuyenQuyenResp = new EFRepository<NhomQuyenQuyen>(); nhomQuyenQuyenResp.UnitOfWork = Repository.UnitOfWork;
                try
                {
                    Repository.Add(entity);
                    if (lstQuyenId != null && lstQuyenId.Count > 0)
                    {
                        var lstQuyen = quyenResp.GetByFilter(x => lstQuyenId.Contains(x.ID));
                        foreach (var eItem in lstQuyenId)
                        {
                            nhomQuyenQuyenResp.Add(new NhomQuyenQuyen()
                            {
                                QuyenChiTiet = "",
                                NhomQuyenID = entity.ID,
                                QuyenID = eItem
                            });
                            Repository.UnitOfWork.Commit();
                        }
                    }
                    Repository.UnitOfWork.Commit();
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

        public bool Update(NhomQuyen entity, List<int> lstQuyenId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var quyenResp = new EFRepository<Quyen>(); quyenResp.UnitOfWork = Repository.UnitOfWork;
                var nhomQuyenQuyenResp = new EFRepository<NhomQuyenQuyen>(); nhomQuyenQuyenResp.UnitOfWork = Repository.UnitOfWork;
                try
                {
                    var objOld = GetById(entity.ID);
                    if (objOld.ID > 0)
                    {
                        Repository.UnitOfWork.Commit();
                        ObjectUtils.CopyObject(entity, ref objOld, true);
                        if (lstQuyenId != null && lstQuyenId.Count > 0)
                        {
                            var lstQuyen = quyenResp.GetByFilter(x => lstQuyenId.Contains(x.ID));
                            foreach (var eItem in lstQuyenId)
                            {
                                nhomQuyenQuyenResp.Add(new NhomQuyenQuyen()
                                {
                                    QuyenChiTiet = "",
                                    NhomQuyenID = entity.ID,
                                    QuyenID = eItem
                                });
                                Repository.UnitOfWork.Commit();
                            }
                        }
                        Repository.Update(objOld);
                        Repository.UnitOfWork.Commit();
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
        public List<NhomQuyenQuyen> GetListNhomQuyenQuyen(Expression<Func<NhomQuyenQuyen, bool>> where)
        {
            var nhomQuyenQuyenResp = new EFRepository<NhomQuyenQuyen>(); nhomQuyenQuyenResp.UnitOfWork = Repository.UnitOfWork;
            return nhomQuyenQuyenResp.GetMany(where).ToList();
        }
        public NhomQuyenQuyen GetByFilterNhomQuyenQuyen(Expression<Func<NhomQuyenQuyen, bool>> where)
        {
            var nhomQuyenQuyenResp = new EFRepository<NhomQuyenQuyen>(); nhomQuyenQuyenResp.UnitOfWork = Repository.UnitOfWork;
            return nhomQuyenQuyenResp.GetByFilter(where);
        }
        public bool AddNhomQuyenQuyen(List<NhomQuyenQuyen> lentity)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var nhomQuyenQuyenResp = new EFRepository<NhomQuyenQuyen>(); nhomQuyenQuyenResp.UnitOfWork = Repository.UnitOfWork;
                try
                {
                    if (lentity != null && lentity.Count() > 0)
                    {
                        foreach (var eItem in lentity)
                        {
                            nhomQuyenQuyenResp.Add(eItem);
                            Repository.UnitOfWork.Commit();
                        }
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
        public bool UpdateNhomQuyenQuyen(List<NhomQuyenQuyen> lentity)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var nhomQuyenQuyenResp = new EFRepository<NhomQuyenQuyen>(); nhomQuyenQuyenResp.UnitOfWork = Repository.UnitOfWork;
                try
                {
                    if (lentity != null && lentity.Count() > 0)
                    {
                        var nhomQuyenId = lentity.FirstOrDefault().NhomQuyenID;
                        var objOld = nhomQuyenQuyenResp.GetMany(x => x.NhomQuyenID == nhomQuyenId).ToList();
                        if (objOld != null && objOld.Count > 0)
                        {
                            foreach (var obj in objOld)
                            {
                                nhomQuyenQuyenResp.Delete(obj);
                                Repository.UnitOfWork.Commit();
                            }
                        }
                        foreach (var eItem in lentity)
                        {
                            nhomQuyenQuyenResp.Add(eItem);
                            Repository.UnitOfWork.Commit();
                        }
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
    }
}