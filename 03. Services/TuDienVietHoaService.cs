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
    public class TuDienVietHoaService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly TuDienVietHoaRepository _TuDienVietHoaResp;
        public TuDienVietHoaService()
        {
            _TuDienVietHoaResp = new TuDienVietHoaRepository(new EFRepository<TuDienVietHoa>(), oUnitOfWork);
        }
        public List<TuDienVietHoaViewModel> GetList(string tuKhoa, string phamViTimKiem, string tieuDe, int? nguoiTaoID, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy, int tab = 0, string action = "", string typeUser = "", int donvilogin = 0, int userloginid = 0, string tendangnhap = "")
        {
            try
            {
                Expression<Func<TuDienVietHoa, bool>> where;
                where = x =>
                   (!string.IsNullOrEmpty(tuKhoa) ? x.CumTuKhoa.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true);
                //Đổ dữ liệu
                var leReturn = _TuDienVietHoaResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => x.CumTuKhoa,
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

        public List<TuDienVietHoaViewModel> GetList(Expression<Func<TuDienVietHoa, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var leReturn = _TuDienVietHoaResp.GetList(where, pageIndex, pageSize, ref totalItem, x => x.NgayTao, true);
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
        public List<TuDienVietHoaViewModel> GetListFive(Expression<Func<TuDienVietHoa, bool>> where)
        {
            try
            {
                var total = 0;
                var leReturn = _TuDienVietHoaResp.GetList(where, 1, 5, ref total, x => x.NgayTao, true);
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

        public List<TuDienVietHoaViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                //Expression<Func<TuDienVietHoa, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _TuDienVietHoaResp.GetList(null, pageIndex, pageSize, ref totalItem,
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
        public List<TuDienVietHoaViewModel> GetListAll(int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                //Expression<Func<TuDienVietHoa, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _TuDienVietHoaResp.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public string GetVietHoaByFilter(Expression<Func<TuDienVietHoa, bool>> where)
        {
            try
            {
                var eReturn = _TuDienVietHoaResp.GetByFilter(where);
                if(eReturn != null && eReturn.ID > 0)
                {
                    return eReturn.CumTuThayDoi;
                }
                return "";
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return "";
            }
        }
        public TuDienVietHoaViewModel GetByFilter(Expression<Func<TuDienVietHoa, bool>> where)
        {
            try
            {
                var leReturn = _TuDienVietHoaResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public TuDienVietHoaViewModel GetById(int id)
        {
            try
            {
                var eReturn = _TuDienVietHoaResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(TuDienVietHoaViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _TuDienVietHoaResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public string Add(TuDienVietHoaViewModel model)
        {
            try
            {
                var error = "";
                var check = _TuDienVietHoaResp.CheckByFilter(x => x.CumTuKhoa.ToLower().Equals(model.CumTuKhoa.ToLower()));
                if(!check)
                {
                    var entity = model.ToEntity();
                    check = _TuDienVietHoaResp.Add(entity);
                    if (!check)
                    {
                        error = "Cập nhật không thành công";
                    }
                }
                return error;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return "Thêm mới không thành công";
            }
        }

        public bool DeleteList(List<int> listId)
        {
            try
            {
                return _TuDienVietHoaResp.DeleteList(listId);
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
                return _TuDienVietHoaResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
