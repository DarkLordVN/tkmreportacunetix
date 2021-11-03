using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TKM.DAO.EntityFramework;

namespace TKM.BLL
{
    public class HomeViewModel
    {
        #region ThongTinChung
        public string TongSoWebsite { get; set; }
        public string TongLuotRaQuet { get; set; }
        public string TongNguyCo { get; set; }

        #endregion

        #region NguyCoWebsite
        public string TitleNguyCoWebsite { get; set; }
        public string TenWebsite { get; set; }
        public string SoLuotScanWebsite { get; set; }
        public string NguyCoCao { get; set; }
        public string NguyCoTrungBinh { get; set; }
        public string NguyCoThap { get; set; }
        #endregion

        #region
        public List<WebsiteViewModel> LsWebsite { get; set; }
        public List<AlertGroupViewModel> LsAlertGroup { get; set; }
        #endregion

    }

    public class BaoCaoTongHopListView : PagedListBase
    {
        public string BaoCao { get; set; }
        public string NhomThietBi { get; set; }
        public string TenThietBi { get; set; }
        public string GroupBy { get; set; }
        public string GroupTime { get; set; }
        public string KeyType { get; set; }
        public int? GetDetail { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public bool? TrangThai { get; set; }
        public int? CapDo { get; set; }
        public int? srTrangThai { get; set; }
        public string LstGroup { get; set; }
        public List<CommonDropDownList> LstGroupData { get; set; }
        public List<CommonDropDownList> LstFilterWeek { get; set; }
        public List<CommonDropDownList> LstFilterMonth { get; set; }
        public List<CommonDropDownList> LstFilterQuarter { get; set; }
        public List<CommonDropDownList> LstFilterYear { get; set; }
        public string ChartImage { get; set; }



        public string ChartTongThietBiNhom { get; set; }
        public string ChartTongSLVanDe { get; set; }

        public List<GetProblemDetail_Result> LstResultDataProblemDetail { get; set; }
        public List<HieuNangNhomThietBi_Result> LstResultDataHieuNangNhomThietBi { get; set; }
        public List<HostsByGroup_Result> LstResultDataHostsByGroup { get; set; }
        public List<CountHostsByGroup_Result> LstResultDataCountHostsByGroup { get; set; }
        public List<DeXuatNhomThietBi_Result> LstResultDataDeXuatNhomThietBi { get; set; }


        public string TongThietBiLoi { get; set; }
        public string TongThietBi { get; set; }

        public List<CountProblemByStatus_Result> LstResultDataCountProblemByStatus { get; set; }
        public string TongChuaXuLy { get; set; }
        public string TongDaXuLy { get; set; }
    }

}
