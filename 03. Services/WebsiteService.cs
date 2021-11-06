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
        private readonly WebsiteRepository _WebsiteRepo;
        private readonly v_WebsiteRepository _vWebsiteRepo;
        public WebsiteService()
        {
            _WebsiteRepo = new WebsiteRepository(new EFRepository<Website>(), oUnitOfWork);
            _vWebsiteRepo = new v_WebsiteRepository(new EFRepository<v_Website>(), oUnitOfWork);
        }
        public List<WebsiteViewModel> GetList(string tuKhoa, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                var lsReturn = new List<WebsiteViewModel>();
                Func<v_Website, object> colOrder = x => x.Host;
                if (!string.IsNullOrEmpty(columnName))
                {
                    switch (columnName)
                    {
                        case "cLuotQuet": colOrder = x => x.cLuotQuet; break;
                        case "cTongNguyCo": colOrder = x => x.cTongNguyCo; break;
                        case "cTongNguyCoCao": colOrder = x => x.cTongNguyCoCao; break;
                        case "cTongItem": colOrder = x => x.cTongItem; break;
                    }
                }
                Expression<Func<v_Website, bool>> where;
                where = x =>
                   (!string.IsNullOrEmpty(tuKhoa) ? x.Host.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true);
                //Đổ dữ liệu
                var lsResult = _vWebsiteRepo.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     colOrder,
                     //Order by
                     string.IsNullOrEmpty(orderBy) ? true : orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                if (lsResult != null && lsResult.Count > 0)
                {
                    foreach (var item in lsResult)
                    {
                        var vmReturn = new WebsiteViewModel()
                        {
                            ID = item.ID,
                            Host = item.Host,
                            ServerInformation = item.ServerInformation,
                            ServerOS = item.ServerOS,
                            ServerTechnologies = item.ServerTechnologies,
                            LuotQuet = item.cLuotQuet.HasValue ? item.cLuotQuet.Value : 0,
                            TongNguyCo = item.cTongNguyCo.HasValue ? item.cTongNguyCo.Value : 0,
                            TongNguyCoCao = item.cTongNguyCoCao.HasValue ? item.cTongNguyCoCao.Value : 0,
                            TongItem = item.cTongItem.HasValue ? item.cTongItem.Value : 0,
                            LastScan = item.LastScan.HasValue ? item.LastScan.Value : DateTime.Now,
                        };
                        lsReturn.Add(vmReturn);
                    }
                }
                return lsReturn;
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
                var leReturn = _WebsiteRepo.GetList(where, pageIndex, pageSize, ref totalItem, x => x.DateCreated, true);
                if (leReturn != null && leReturn.Count > 0)
                {
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
                var leReturn = _WebsiteRepo.GetList(where, 1, 5, ref total, x => x.DateCreated, true);
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
                var leReturn = _WebsiteRepo.GetList(null, pageIndex, pageSize, ref totalItem,
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
                var leReturn = _WebsiteRepo.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<WebsiteViewModel> GetListTopNguyCo(Expression<Func<v_Website, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var lsReturn = new List<WebsiteViewModel>();
                var leResult = _vWebsiteRepo.GetList(where, pageIndex, pageSize, ref totalItem, x => x.cTongNguyCoCao, true, x => x.cTongNguyCoTB, true);
                if (leResult != null && leResult.Count > 0)
                {
                    foreach (var item in leResult)
                    {
                        var eReturn = new WebsiteViewModel();
                        eReturn.ID = item.ID;
                        eReturn.Host = item.Host;
                        eReturn.StartURL = item.StartURL;
                        eReturn.TongNguyCoCao = item.cTongNguyCoCao.HasValue ? item.cTongNguyCoCao.Value : 0;
                        eReturn.TongNguyCoTrungBinh = item.cTongNguyCoTB.HasValue ? item.cTongNguyCoTB.Value : 0;
                        eReturn.TongNguyCoThap = item.cTongNguyCoThap.HasValue ? item.cTongNguyCoThap.Value : 0;
                        eReturn.LuotQuet = item.cLuotQuet.HasValue ? item.cLuotQuet.Value : 0;
                        lsReturn.Add(eReturn);
                    }
                    return lsReturn;
                }
                return null;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public int GetCountByFilter(Expression<Func<Website, bool>> where)
        {
            try
            {
                var leReturn = _WebsiteRepo.GetList(where);
                if (leReturn != null && leReturn.Count > 0)
                    return leReturn.Count;
                return 0;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return 0;
            }
        }
        public WebsiteViewModel GetByFilter(Expression<Func<Website, bool>> where)
        {
            try
            {
                var leReturn = _WebsiteRepo.GetByFilter(where);
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
                var eReturn = _WebsiteRepo.GetById(id);
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
                return _WebsiteRepo.Update(entity);
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
                return _WebsiteRepo.Add(entity);
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
                return _WebsiteRepo.DeleteList(listId);
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
                return _WebsiteRepo.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
