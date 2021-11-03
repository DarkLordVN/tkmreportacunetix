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
    public class WebsiteItemService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly WebsiteItemRepository _WebsiteItemResp;
        public WebsiteItemService()
        {
            _WebsiteItemResp = new WebsiteItemRepository(new EFRepository<WebsiteItem>(), oUnitOfWork);
        }
        public List<WebsiteItemViewModel> GetList(string tuKhoa, string phamViTimKiem, string tieuDe, int? nguoiTaoID, int? chucNangID, string tuNgayTaoTB, string denNgayTaoTB, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy, int tab = 0, string action = "", string typeUser = "", int donvilogin = 0, int userloginid = 0, string tendangnhap = "")
        {
            try
            {
                Expression<Func<WebsiteItem, bool>> where;
                where = x =>
                   (!string.IsNullOrEmpty(tuKhoa) ? x.FullPath.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true);
                //Đổ dữ liệu
                var leReturn = _WebsiteItemResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? x.DateCreated.Value.ToString("yyyyMMddHHmmss") :
                     columnName.Equals("ScanPath") ? x.FullPath.ToString() : x.DateCreated.Value.ToString("yyyyMMddHHmmss")),
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

        public List<WebsiteItemViewModel> GetList(Expression<Func<WebsiteItem, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var leReturn = _WebsiteItemResp.GetList(where, pageIndex, pageSize, ref totalItem, x => x.DateCreated, true);
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
        public List<WebsiteItemViewModel> GetListFive(Expression<Func<WebsiteItem, bool>> where)
        {
            try
            {
                var total = 0;
                var leReturn = _WebsiteItemResp.GetList(where, 1, 5, ref total, x => x.DateCreated, true);
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

        public List<WebsiteItemViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                //Expression<Func<WebsiteItem, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _WebsiteItemResp.GetList(null, pageIndex, pageSize, ref totalItem,
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
        public List<WebsiteItemViewModel> GetListAll(int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                //Expression<Func<WebsiteItem, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _WebsiteItemResp.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public WebsiteItemViewModel GetByFilter(Expression<Func<WebsiteItem, bool>> where)
        {
            try
            {
                var leReturn = _WebsiteItemResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public WebsiteItemViewModel GetById(int id)
        {
            try
            {
                var eReturn = _WebsiteItemResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(WebsiteItemViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _WebsiteItemResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(WebsiteItemViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _WebsiteItemResp.Add(entity);
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
                return _WebsiteItemResp.DeleteList(listId);
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
                return _WebsiteItemResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
