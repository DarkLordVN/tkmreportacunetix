using TKM.BLL;
using TKM.DAO;
using TKM.DAO.EntityFramework;
using TKM.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace TKM.Services
{
    public class NhatKyHeThongService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly NguoiDungRepository _nguoiDungResp;
        private readonly NhatKyHeThongRepository _nhatKyHeThongResp;

        public NhatKyHeThongService()
        {
            _nguoiDungResp = new NguoiDungRepository(new EFRepository<NguoiDung>(), oUnitOfWork);
            _nhatKyHeThongResp = new NhatKyHeThongRepository(new EFRepository<NhatKyHeThong>(), oUnitOfWork);
        }

        public List<NhatKyHeThongViewModel> GetList(string tuKhoa, int? nguoiDungID, string mota, string ipClient, string tuNgayNT, string denNgayNT, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                Expression<Func<NhatKyHeThong, bool>> where;
                DateTime? tuNgayNTDate = null;
                DateTime? denNgayNTDate = null;
                if (!string.IsNullOrEmpty(tuNgayNT))
                {
                    tuNgayNT = tuNgayNT.Replace(" ", "");
                    tuNgayNTDate = DateTime.ParseExact(tuNgayNT, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(denNgayNT))
                {
                    denNgayNT = denNgayNT.Replace(" ", "");
                    denNgayNTDate = DateTime.ParseExact(denNgayNT, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    denNgayNTDate = denNgayNTDate.Value.AddDays(1).AddMilliseconds(-1);
                }
                where = x =>
                    (!string.IsNullOrEmpty(tuKhoa) ? x.MoTa.ToLower().Trim().Contains(tuKhoa.ToLower().Trim())
                  || x.IpClient.ToLower().Trim().Contains(tuKhoa.ToLower().Trim()) : true)
                && (!string.IsNullOrEmpty(mota) ? x.MoTa.ToLower().Trim().Contains(mota.ToLower().Trim()) : true)
               && (!string.IsNullOrEmpty(ipClient) ? x.IpClient.ToLower().Trim().Contains(ipClient.ToLower().Trim()) : true)
                && (x.NgayTao.HasValue ? (string.IsNullOrEmpty(tuNgayNT)) ?
                                                               ((string.IsNullOrEmpty(denNgayNT)) ? true : DateTime.Compare(denNgayNTDate.Value, x.NgayTao.Value) >= 0)
                                                              : ((!string.IsNullOrEmpty(denNgayNT))
                                                                                                   ? ((DateTime.Compare(x.NgayTao.Value, tuNgayNTDate.Value) >= 0) && (DateTime.Compare(x.NgayTao.Value, denNgayNTDate.Value) <= 0)) : (DateTime.Compare(x.NgayTao.Value, tuNgayNTDate.Value) >= 0)) : (!string.IsNullOrEmpty(tuNgayNT) || (!string.IsNullOrEmpty(denNgayNT))) ? false : true)
               && (nguoiDungID != null && nguoiDungID != 0 ? x.NguoiDungId == nguoiDungID : true);
                var leReturn = _nhatKyHeThongResp.GetList(where, pageIndex, pageSize, ref totalItem,
                     //Filter theo column
                     x => (string.IsNullOrEmpty(columnName) ? x.NgayTao.Value.ToString("yyyyMMddHHmmss") :
                         columnName.Equals("MoTa") ? x.MoTa :
                         columnName.Equals("IpClient") ? x.IpClient : null),
                    //Order by
                    orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                if (leReturn != null && leReturn.Count > 0)
                {
                    var leRes = new List<NhatKyHeThongViewModel>();
                    foreach (var item in leReturn)
                    {
                        var obj = item.ToModel();
                        if (item.NguoiDungId.HasValue)
                        {
                            var objNguoiDung = _nguoiDungResp.GetById(item.NguoiDungId.Value);
                            obj.TenNguoiDung = objNguoiDung.HoVaTen;
                        }
                        leRes.Add(obj);
                    }
                    return leRes;
                }
                return null;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<NhatKyHeThongViewModel> GetList(Expression<Func<NhatKyHeThong, bool>> where)
        {
            try
            {
                var leReturn = _nhatKyHeThongResp.GetList(where);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Add(NhatKyHeThongViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _nhatKyHeThongResp.Add(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
    }
}
