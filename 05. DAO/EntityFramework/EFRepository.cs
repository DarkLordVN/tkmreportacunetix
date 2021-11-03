using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace TKM.DAO.EntityFramework
{
	public class EFRepository<T> : IRepository<T> where T : class
	{
		public IUnitOfWork UnitOfWork { get; set; }
		private IDbSet<T> _objectset;
		private IDbSet<T> ObjectSet
		{
			get
			{
				if (_objectset == null)
				{
					_objectset = UnitOfWork.Context.Set<T>();
				}
				return _objectset;
			}
		}

		public virtual IQueryable<T> All()
		{
			return ObjectSet.AsQueryable();
		}

		public virtual IQueryable<T> All(string includes)
		{
            DbQuery<T> Query = UnitOfWork.Context.Set<T>();
			if (!string.IsNullOrEmpty(includes))
            {
                foreach (string includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Query = Query.Include(includeProperty.Trim());
                }
            }
            return Query;
		}

		public virtual IQueryable<T> All(List<string> lstInclude)
		{
            DbQuery<T> Query = UnitOfWork.Context.Set<T>();
			foreach(string itemInclude in lstInclude)
                Query = Query.Include(itemInclude.Trim());
            return Query;
		}

		public IQueryable<T> Find(Func<T, bool> expression)
		{
			return ObjectSet.Where(expression).AsQueryable();
		}

		public T Add(T entity)
		{
			return ObjectSet.Add(entity);
		}

		public void Delete(T entity)
		{
			ObjectSet.Remove(entity);
		}

		public void Save()
		{
			UnitOfWork.Save();
		}

		//ToanLK - 2019 - Modified
		public void Update(T entity)
        {
            DbQuery<T> Query = UnitOfWork.Context.Set<T>();
            ObjectSet.Attach(entity);
            UnitOfWork.Context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

		public void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = ObjectSet.Where<T>(where).AsEnumerable();
			if(objects != null && objects.Count() > 0)
			{
				foreach (T obj in objects)
					ObjectSet.Remove(obj);
			}

        }
		
		public virtual T GetByFilter(Expression<Func<T, bool>> where)
        {
            return ObjectSet.Where(where).FirstOrDefault();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return ObjectSet.AsEnumerable();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return ObjectSet.Where(where);
        }
        public virtual T GetById(int id)
        {
            return ObjectSet.Find(id);
        }
	}
}