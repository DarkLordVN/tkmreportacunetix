using System.Data.Entity;

namespace TKM.DAO.EntityFramework
{
	public interface IUnitOfWork
	{
		DbContext Context { get; set; }
		void Save();
		bool LazyLoadingEnabled { get; set; }
		bool ProxyCreationEnabled { get; set; }
		string ConnectionString { get; set; }
		void Commit();
	}
}