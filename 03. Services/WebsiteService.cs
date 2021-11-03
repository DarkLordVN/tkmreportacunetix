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
    public class WebsiteService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly WebsiteRepository _WebsiteResp;
        public WebsiteService()
        {
            _WebsiteResp = new WebsiteRepository(new EFRepository<Website>(), oUnitOfWork);
        }
        public List<WebsiteViewModel> GetList(string tuKhoa, string phamViTimKiem, string tieuDe, int? nguoiTaoID, int? chucNangID, string tuNgayTaoTB, string denNgayTaoTB, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy, int tab = 0, string action = "", string typeUser = "", int donvilogin = 0, int userloginid = 0, string tendangnhap = "")
        {
            try
            {
                Expression<Func<Website, bool>> where;
                where = x =>
                   (!string.IsNullOrEmpty(tuKhoa) ? x.Host.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true);
                //Đổ dữ liệu
                var leReturn = _WebsiteResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? x.DateCreated.Value.ToString("yyyyMMddHHmmss") :
                     columnName.Equals("Host") ? x.Host.ToString() : x.DateCreated.Value.ToString("yyyyMMddHHmmss")),
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

        public List<WebsiteViewModel> GetList(Expression<Func<Website, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var leReturn = _WebsiteResp.GetList(where, pageIndex, pageSize, ref totalItem, x => x.DateCreated, true);
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
        public List<WebsiteViewModel> GetListFive(Expression<Func<Website, bool>> where)
        {
            try
            {
                var total = 0;
                var leReturn = _WebsiteResp.GetList(where, 1, 5, ref total, x => x.DateCreated, true);
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

        public List<WebsiteViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                //Expression<Func<Website, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _WebsiteResp.GetList(null, pageIndex, pageSize, ref totalItem,
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
        public List<WebsiteViewModel> GetListAll(int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                //Expression<Func<Website, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _WebsiteResp.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public WebsiteViewModel GetByFilter(Expression<Func<Website, bool>> where)
        {
            try
            {
                var leReturn = _WebsiteResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public WebsiteViewModel GetById(int id)
        {
            try
            {
                var eReturn = _WebsiteResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(WebsiteViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _WebsiteResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(WebsiteViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _WebsiteResp.Add(entity);
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
                return _WebsiteResp.DeleteList(listId);
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
                return _WebsiteResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
