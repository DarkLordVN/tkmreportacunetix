

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace TKM.DAO.EntityFramework
{ 
	public interface IRepository<T> 
    {
		IUnitOfWork UnitOfWork { get; set; }
		IQueryable<T> All();
        IQueryable<T> All(string includes);
		IQueryable<T> All(List<string> lstInclude);
		IQueryable<T> Find(Func<T, bool> expression);
		void Save();
		//ToanLK - 2019
		T Add(T entity);
		void Delete(T entity);
        void Update(T entity);
        void Delete(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
		IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
		T GetByFilter(Expression<Func<T, bool>> where);
        T GetById(int Id);
    }
}

