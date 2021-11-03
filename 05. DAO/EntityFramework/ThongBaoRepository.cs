using System;
using System.Linq;
using System.Collections.Generic;
using System.Transactions;
using TKM.Utils;

namespace TKM.DAO.EntityFramework
{   
	public partial class ThongBaoRepository
	{
        // Add your own data access methods.
        // This file should not get overridden
        public string Add(ThongBao entity,string ext = "")
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    Repository.Add(entity);
                    Repository.UnitOfWork.Commit();
                    scope.Complete();
                    scope.Dispose();
                    return entity.ID.ToString();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    OutputLog.WriteOutputLog(ex);
                    return "";
                }
            }
        }
    }
}