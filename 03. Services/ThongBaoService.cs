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
    public class ThongBaoService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly ThongBaoRepository _thongBaoResp;
        private readonly NguoiDungRepository _nguoiDungResp;
        private readonly NhomNguoiDungRepository _nhomNguoiDungResp;
        public ThongBaoService()
        {
            _thongBaoResp = new ThongBaoRepository(new EFRepository<ThongBao>(), oUnitOfWork);
            _nguoiDungResp = new NguoiDungRepository(new EFRepository<NguoiDung>(), oUnitOfWork);
            _nhomNguoiDungResp = new NhomNguoiDungRepository(new EFRepository<NhomNguoiDung>(), oUnitOfWork);
        }
        public List<ThongBaoViewModel> GetList(string tuKhoa, string phamViTimKiem, string tieuDe, int? nguoiTaoID, int? chucNangID, string tuNgayTaoTB, string denNgayTaoTB, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy, int tab = 0, string action = "", string typeUser = "", int donvilogin = 0, int userloginid = 0,string tendangnhap = "")
        {
            try
            {
                Expression<Func<ThongBao, bool>> where;
                DateTime? tuNgayTaoTBDate = new DateTime();
                DateTime? denNgayTaoTBDate = new DateTime();
                int findTieuDe = 0;
                int findNoiDung = 0;
                if (!string.IsNullOrEmpty(phamViTimKiem))
                {
                    if (phamViTimKiem.Contains("tieude"))
                        findTieuDe = 1;
                    if (phamViTimKiem.Contains("noidung"))
                        findNoiDung = 1;
                }
                if (!string.IsNullOrEmpty(tuNgayTaoTB) && tuNgayTaoTB.Trim().ToLower().Equals("day/month/year")) tuNgayTaoTB = "";
                if (!string.IsNullOrEmpty(denNgayTaoTB) && denNgayTaoTB.Trim().ToLower().Equals("day/month/year")) denNgayTaoTB = "";
                if (!string.IsNullOrEmpty(tuNgayTaoTB))
                {
                    tuNgayTaoTB = tuNgayTaoTB.Replace(" ", "");
                    tuNgayTaoTBDate = DateTime.ParseExact(tuNgayTaoTB, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(denNgayTaoTB))
                {
                    denNgayTaoTB = denNgayTaoTB.Replace(" ", "");
                    denNgayTaoTBDate = DateTime.ParseExact(denNgayTaoTB, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                //tab = 0 : Thông báo đi
                //      1 : Thông báo đến
                switch (tab)
                {
                    #region Thông báo đi
                    case 0:
                        where = x => (!string.IsNullOrEmpty(tendangnhap) && tendangnhap.Equals("admin")? true : x.NguoiTaoID == userloginid)
                        && (!string.IsNullOrEmpty(tuKhoa) && ((findTieuDe == 1 && findNoiDung == 1 ) || string.IsNullOrEmpty(phamViTimKiem)) ? (x.TieuDe.ToLower().Contains(tuKhoa.Trim().ToLower()) || x.NoiDung.ToLower().Contains(tuKhoa.Trim().ToLower())) : !string.IsNullOrEmpty(tuKhoa) && findTieuDe == 1 ? x.TieuDe.ToLower().Trim().Contains(tuKhoa.Trim().ToLower()) : !string.IsNullOrEmpty(tuKhoa) && findNoiDung == 1 ? x.NoiDung.ToLower().Trim().Contains(tuKhoa.Trim().ToLower()) : true)
                && (x.NgayTao.HasValue ? (string.IsNullOrEmpty(tuNgayTaoTB)) ?
                                                    ((string.IsNullOrEmpty(denNgayTaoTB)) ? true : DateTime.Compare(denNgayTaoTBDate.Value, x.NgayTao.Value) >= 0)
                                                    : ((!string.IsNullOrEmpty(denNgayTaoTB))
                                                                                        ? ((DateTime.Compare(x.NgayTao.Value, tuNgayTaoTBDate.Value) >= 0) && (DateTime.Compare(x.NgayTao.Value, denNgayTaoTBDate.Value) <= 0)) : (DateTime.Compare(x.NgayTao.Value, tuNgayTaoTBDate.Value) >= 0)) : (!string.IsNullOrEmpty(tuNgayTaoTB) || (!string.IsNullOrEmpty(denNgayTaoTB))) ? false : true)
                && (nguoiTaoID != null && nguoiTaoID.Value != 0 ? x.NguoiTaoID == nguoiTaoID : true);
                        break;
                    #endregion
                    #region Thông báo đến
                    case 1:
                        where = x =>
                        (!string.IsNullOrEmpty(tendangnhap) && tendangnhap.Equals("admin") ? true : ((nguoiTaoID != null && nguoiTaoID != 0 ? x.NguoiTaoID == nguoiTaoID : true))) &&
                        //(userloginid != 0 ? x.LstNguoiNhanID.Contains("," + userloginid + ",") : true)))
                        // (nguoiTaoID != null && nguoiTaoID != 0 ? x.NguoiTaoID == nguoiTaoID : true)&& 
                        //(userloginid != 0 ? x.LstNguoiNhanID.Contains("," + userloginid + ",") : true)
                        //&&
                        (!string.IsNullOrEmpty(tuKhoa) && ((findTieuDe == 1 && findNoiDung == 1) || string.IsNullOrEmpty(phamViTimKiem)) ? (x.TieuDe.ToLower().Contains(tuKhoa.Trim().ToLower()) || x.NoiDung.ToLower().Contains(tuKhoa.Trim().ToLower())) : !string.IsNullOrEmpty(tuKhoa) && findTieuDe == 1 ? x.TieuDe.ToLower().Trim().Contains(tuKhoa.Trim().ToLower()) : !string.IsNullOrEmpty(tuKhoa) && findNoiDung == 1 ? x.NoiDung.ToLower().Trim().Contains(tuKhoa.Trim().ToLower()) : true)
               && (x.NgayTao.HasValue ? (string.IsNullOrEmpty(tuNgayTaoTB)) ?
                                                   ((string.IsNullOrEmpty(denNgayTaoTB)) ? true : DateTime.Compare(denNgayTaoTBDate.Value, x.NgayTao.Value) >= 0)
                                                   : ((!string.IsNullOrEmpty(denNgayTaoTB))
                                                                                       ? ((DateTime.Compare(x.NgayTao.Value, tuNgayTaoTBDate.Value) >= 0) && (DateTime.Compare(x.NgayTao.Value, denNgayTaoTBDate.Value) <= 0)) : (DateTime.Compare(x.NgayTao.Value, tuNgayTaoTBDate.Value) >= 0)) : (!string.IsNullOrEmpty(tuNgayTaoTB) || (!string.IsNullOrEmpty(denNgayTaoTB))) ? false : true)
               && (chucNangID != 0 && chucNangID != null ? x.ChucNangID == chucNangID : true);
                        break;
                    #endregion
                    default:
                        where = x => false;
                        break;
                }
                
                //Đổ dữ liệu
                var leReturn = _thongBaoResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? x.NgayTao.Value.ToString("yyyyMMddHHmmss") :
                     columnName.Equals("TieuDe") ? x.TieuDe.ToString() : x.NgayTao.Value.ToString("yyyyMMddHHmmss")),
                     //Order by
                     string.IsNullOrEmpty(orderBy) ? true: orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                var objNguoiTao = new NguoiDung();
                var lst = leReturn.ToListModel();
                foreach (var item in lst)
                {
                    objNguoiTao = _nguoiDungResp.GetById(item.NguoiTaoID ?? 0);
                    if (objNguoiTao != null)
                    {
                        //item.NguoiTao = !string.IsNullOrEmpty(objNguoiTao.HoVaTen) ? objNguoiTao.HoVaTen : "";
                        //item.ChucVuVaTen = !string.IsNullOrEmpty(objNguoiTao.ChucVuVaTen) ? objNguoiTao.ChucVuVaTen : "";
                    }
                    
                }
                return lst;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<ThongBaoViewModel> GetList(Expression<Func<ThongBao, bool>> where, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                var leReturn = _thongBaoResp.GetList(where, pageIndex, pageSize, ref totalItem, x=>x.NgayTao, true);
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
        public List<ThongBaoViewModel> GetListFive(Expression<Func<ThongBao, bool>> where)
        {
            try
            {
                var total = 0;
                var leReturn = _thongBaoResp.GetList(where,1,5,ref total,x=>x.NgayTao,true);
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

        public List<ThongBaoViewModel> GetListTopOne(int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                //Expression<Func<ThongBao, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _thongBaoResp.GetList(null, pageIndex, pageSize, ref totalItem,
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
        public List<ThongBaoViewModel> GetListAll(int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                //Expression<Func<ThongBao, bool>> where = x => x.IsXoaTam == false;
                var leReturn = _thongBaoResp.GetList(null, 0, 0, ref totalItem);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public ThongBaoViewModel GetByFilter(Expression<Func<ThongBao, bool>> where)
        {
            try
            {
                var leReturn = _thongBaoResp.GetByFilter(where);
                return leReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public ThongBaoViewModel GetById(int id)
        {
            try
            {
                var eReturn = _thongBaoResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(ThongBaoViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _thongBaoResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public string AddThongBaoHeThong(string tieude = "",string noidung = "", int chucnangid = 0, string link= "", string lstnguoinhanid = "", int nguoitaoid = 0)
        {
            try
            {
                ThongBaoViewModel model = new ThongBaoViewModel()
                {
                    TieuDe = tieude,
                    NoiDung = noidung,
                    ChucNangID = chucnangid,
                    Link = link,
                    LstNguoiNhanID = lstnguoinhanid,
                    FileDinhKem = null,
                    NguoiTaoID = nguoitaoid,
                    NgayTao = DateTime.Now,
                    IsDaGui = true,
                };
                var entity = model.ToEntity();
                return _thongBaoResp.Add(entity,"");
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return "";
            }
        }
        public bool Add(ThongBaoViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _thongBaoResp.Add(entity);
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
                return _thongBaoResp.DeleteList(listId);
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
                return _thongBaoResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
