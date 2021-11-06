using System;
using System.Linq;
using System.Collections.Generic; 
using System.Data;
using TKM.Utils;
using System.Linq.Expressions;
using System.Transactions;
// This file is auto generated and will be overwritten as soon as the template is executed
// Do not modify this file...
	
namespace TKM.DAO.EntityFramework
{   
	public partial class v_AffectedItemRepository
	{
		private IRepository<v_AffectedItem> _repository {get;set;}
		public IRepository<v_AffectedItem> Repository
		{
			get { return _repository; }
			set { _repository = value; }
		}
		
		public v_AffectedItemRepository(IRepository<v_AffectedItem> repository, IUnitOfWork unitOfWork)
    	{
    		Repository = repository;
			Repository.UnitOfWork = new EFUnitOfWork(); 
    	}

		public List<v_AffectedItem> GetList(Expression<Func<v_AffectedItem, bool>> where)
        {
            try
            {
                IEnumerable<v_AffectedItem> lResult;
                if (where != null)
                {
                    lResult = Repository.GetMany(where);
                }
                else
                {
                    lResult = Repository.GetAll();
                }
                return lResult.ToList();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

		public List<v_AffectedItem> GetList(Expression<Func<v_AffectedItem, bool>> where, int pageIndex, int pageSize, ref int totalItem, Func<v_AffectedItem, object> orderCol = null, bool isDecending = true)
        {
            try
            {
                IEnumerable<v_AffectedItem> lResult;
                if (where != null)
                {
                    lResult = Repository.GetMany(where);
                }
                else
                {
                    lResult = Repository.GetAll();
                }
                if (orderCol != null)
                {
                    if (!isDecending) lResult = lResult.OrderBy(orderCol).AsEnumerable();
                    else lResult = lResult.OrderByDescending(orderCol).AsEnumerable();
                }
                if (pageIndex > 0 && pageSize > 0)
                {
                    totalItem = lResult.Count();
                    if (totalItem > ((pageIndex - 1) * pageSize))
                        lResult = lResult.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                }
                return lResult.ToList();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

		public v_AffectedItem GetById(int id)
        {
            try
            {
                var obj = Repository.GetById(id);
                return obj;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return new v_AffectedItem();
            }
        }
        public v_AffectedItem GetByFilter(Expression<Func<v_AffectedItem, bool>> where)
        {
            try
            {
                Repository.Save();
                var obj = Repository.GetByFilter(where);
                return obj;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return new v_AffectedItem();
            }
        }
		public bool CheckByFilter(Expression<Func<v_AffectedItem, bool>> where)
        {
            try
            {
                Repository.Save();
                var obj = Repository.GetByFilter(where);
				if(obj != null) return true;
                return false;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool Add(v_AffectedItem entity)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    Repository.Add(entity);
                    Repository.Save();
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
		
		public bool Add(List<v_AffectedItem> lentity)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
					if(lentity != null && lentity.Count() > 0) {
						foreach(var entity in lentity) {
							Repository.Add(entity);
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

        public bool Update(v_AffectedItem entity)
        {
            try
            {
                var objOld = GetById(entity.ID);
                if (objOld.ID > 0)
                {
                    ObjectUtils.CopyObject(entity, ref objOld, true);
                    Repository.Update(objOld);
					Repository.UnitOfWork.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

		public bool Update(List<v_AffectedItem> lentity)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
					if(lentity != null && lentity.Count() > 0) {
						foreach(var entity in lentity) {
							var objOld = GetById(entity.ID);
							if (objOld.ID > 0)
							{
								ObjectUtils.CopyObject(entity, ref objOld, true);
								Repository.Update(objOld);
								Repository.UnitOfWork.Commit();
							}
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

        public bool Delete(int id)
        {
            try
            {
                var objOld = GetById(id);
                Repository.Delete(objOld);
				Repository.UnitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool DeleteList(List<int> ListId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var objOld = Repository.GetMany(x => ListId.Contains(x.ID)).ToList();
                    if (objOld != null && objOld.Count > 0)
                    {
                        foreach (var obj in objOld)
                        {
                            Repository.Delete(obj);
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