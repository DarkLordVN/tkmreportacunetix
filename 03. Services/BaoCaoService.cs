using TKM.BLL;
using TKM.DAO;
using TKM.DAO.EntityFramework;
using TKM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Services
{
    public class BaoCaoService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        public BaoCaoService() { }

        public List<CountProblemByStatus_Result> countProblemByGroup(string lstGroup, string trangThai, string groupBy, DateTime? tuNgay, DateTime? denNgay, int? pageIndex, int? pageSize, string columnName, string orderBy)
        {
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                context.Database.CommandTimeout = 60;
                var details = context.CountProblemByStatus(lstGroup, trangThai, groupBy, tuNgay, denNgay, pageIndex, pageSize, columnName, orderBy).ToList();
                return details;
            }
        }

        public void countProblemGroupLevel(string lstGroup, string trangThai, string groupBy, DateTime? tuNgay, DateTime? denNgay, int? pageIndex, int? pageSize, string columnName, string orderBy, ref int pChuaBiet, ref int pThongTin, ref int pCanhBao, ref int pTrungBinh, ref int pCao, ref int pNghiemTrong)
        {
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                context.Database.CommandTimeout = 60;
                var details = context.CountProblemBySeverity(lstGroup, trangThai, groupBy, tuNgay, denNgay, pageIndex, pageSize, columnName, orderBy).ToList();
                if (details != null && details.Count() > 0)
                {
                    var chuaBiet = details.Sum(x => x.chuabiet);
                    pChuaBiet = chuaBiet.HasValue ? chuaBiet.Value : 0;
                    var thongtin = details.Sum(x => x.thongtin);
                    pThongTin = thongtin.HasValue ? thongtin.Value : 0;
                    var canhbao = details.Sum(x => x.canhbao);
                    pCanhBao = canhbao.HasValue ? canhbao.Value : 0;
                    var trungbinh = details.Sum(x => x.trungbinh);
                    pTrungBinh = trungbinh.HasValue ? trungbinh.Value : 0;
                    var cao = details.Sum(x => x.cao);
                    pCao = cao.HasValue ? cao.Value : 0;
                    var nghiemtrong = details.Sum(x => x.nghiemtrong);
                    pNghiemTrong = nghiemtrong.HasValue ? nghiemtrong.Value : 0;
                }
            }
        }

        public List<GetProblemDetail_Result> getProblem(string lstGroup, string trangThai, string groupBy, int? capDo, DateTime? tuNgay, DateTime? denNgay, int? pageIndex, int? pageSize, string columnName, string orderBy, ref string dataChart, bool isHome = true)
        {
            var lstResult = new List<GetProblemDetail_Result>();
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                context.Database.CommandTimeout = 60;
                var details = context.GetProblemDetail(lstGroup, trangThai, groupBy, capDo, tuNgay, denNgay, pageIndex, pageSize, columnName, orderBy).ToList();
                if (details != null && details.Count > 0)
                {
                    if (isHome)
                    {
                        pageIndex = 1;
                        pageSize = 3;
                    }
                    dataChart = details.Where(x => string.IsNullOrEmpty(x.thoigiantontai)).Count() + "|" + details.Where(x => !string.IsNullOrEmpty(x.thoigiantontai)).Count();
                    if (pageSize != null && pageIndex != null && pageSize > 0 && pageIndex > 0)
                    {
                        lstResult = details.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList(); ;
                    }
                    else
                    {
                        lstResult = details;
                    }
                }
                return lstResult;
            }
        }

        public List<CountHostsByGroup_Result> countHostsByGroup(string lstGroup, string trangThai, DateTime? tuNgay, DateTime? denNgay, int? pageIndex, int? pageSize, string columnName, string orderBy)
        {
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                context.Database.CommandTimeout = 60;
                var details = context.CountHostsByGroup(lstGroup, trangThai, tuNgay, denNgay, pageIndex, pageSize, columnName, orderBy).ToList();
                return details;
            }
        }

        public List<HostsByGroup_Result> chiTietThietBiTheoNhom(string lstGroup, string hostName, int? pageIndex, int? pageSize, string columnName, string orderBy)
        {
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                context.Database.CommandTimeout = 60;
                var details = context.HostsByGroup(lstGroup, hostName, pageIndex, pageSize, columnName, orderBy).ToList();
                return details;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstGroup"></param>
        /// <param name="hostName"></param>
        /// <param name="groupBy">group/host</param>
        /// <param name="groupTime">week/month/quarter/year</param>
        /// <param name="keyType">cpu/ram/hdd/bwd</param>
        /// <param name="getDetail">1 de get itemname + ip (xuat)</param>
        /// <param name="tuNgay"></param>
        /// <param name="denNgay"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="columnName"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public List<HieuNangNhomThietBi_Result> hieuNangThietBi(string lstGroup, string hostName, string groupBy, string groupTime, string keyType, int? getDetail, DateTime? tuNgay, DateTime? denNgay, int? pageIndex, int? pageSize, string columnName, string orderBy)
        {
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                context.Database.CommandTimeout = 60;
                var lstResult = new List<HieuNangNhomThietBi_Result>();
                var details = context.HieuNangNhomThietBi(lstGroup, hostName, groupBy, groupTime, keyType, getDetail, tuNgay, denNgay, pageIndex, pageSize, columnName, orderBy).ToList();
                if (details != null && details.Count() > 0)
                {
                    Dictionary<string, decimal> dicVal = new Dictionary<string, decimal>();
                    double hieuNang = 0;
                    var index = 0;
                    foreach (var item in details.OrderByDescending(x => x.rnum))
                    {
                        decimal lastVal = 0;
                        var key = item.groupname + item.name + item.itemname;
                       
                        index++;
                        lstResult.Add(item);
                    }
                    lstResult = lstResult.OrderBy(x => x.rnum).ToList();
                }
                return lstResult;
            }
        }

        public List<DeXuatNhomThietBi_Result> deXuatThietBi(string lstGroup, string hostName, string groupBy, string groupTime, string keyType, int? getDetail, DateTime? tuNgay, DateTime? denNgay, int? pageIndex, int? pageSize, string columnName, string orderBy)
        {
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                context.Database.CommandTimeout = 60;
                var lstResult = new List<DeXuatNhomThietBi_Result>();
                var details = context.DeXuatNhomThietBi(lstGroup, hostName, groupBy, groupTime, keyType, getDetail, tuNgay, denNgay, pageIndex, pageSize, columnName, orderBy).ToList();
                if (details != null && details.Count() > 0)
                {
                    return details;
                }
                return lstResult;
            }
        }

        #region dropdown
        public List<CommonDropDownList> getDDLNhomThietBi()
        {
            var lstResult = new List<CommonDropDownList>();
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                var data = context.GetNhomThietBi().ToList();
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        lstResult.Add(new CommonDropDownList() { Text = item.name, Value = (int)item.groupid });
                    }
                }
            }
            return lstResult;
        }

        public List<CommonDropDownList> getDDLFilterTimeHieuNang(string groupType = "month")
        {
            var lstResult = new List<CommonDropDownList>();
            using (TKMReportsEntities context = new TKMReportsEntities())
            {
                var data = context.GetFilterHieuNang(groupType).ToList();
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        lstResult.Add(new CommonDropDownList() { Text = item.timelabel, ValueStr = item.timelabel });
                    }
                }
            }
            return lstResult;
        }
        #endregion
    }
}
