using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using TKM.Utils;

namespace TKM.DAO.EntityFramework
{
    public partial class v_WebsiteRepository
    {
        // Add your own data access methods.
        // This file should not get overridden
        public List<v_Website> GetList(Expression<Func<v_Website, bool>> where, int pageIndex, int pageSize, ref int totalItem, Func<v_Website, object> orderCol = null, bool isDecending = true, Func<v_Website, object> orderCol2 = null, bool isDecending2 = true)
        {
            try
            {
                IEnumerable<v_Website> lResult;
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
                    if (orderCol2 != null)
                    {
                        if (!isDecending2) lResult = lResult.OrderBy(orderCol).ThenBy(orderCol2).AsEnumerable();
                        else lResult = lResult.OrderByDescending(orderCol).ThenByDescending(orderCol2).AsEnumerable();
                    }
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
    }
}