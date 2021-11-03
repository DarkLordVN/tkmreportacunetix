using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Utils.Enums
{
    public enum TrangThaiDuThaoVanBanDi
    {
        [Description("Bản nháp")]
        BanNhap = 1,
        [Description("Chờ lãnh đạo đơn vị duyệt")]
        ChoLanhDaoDVDuyet = 2,
        [Description("Chờ lãnh đạo Cục duyệt")]
        ChoLanhDaoCucDuyet = 12,
        [Description("Đã duyệt")]
        DaDuyet = 3,
        [Description("Đã ký duyệt")]
        DaKyDuyet = 4,
        [Description("Từ chối duyệt")]
        TuChoiDuyet = 5,
        [Description("Từ chối ký Lãnh đạo Cục đã duyệt")]
        TuChoiKyLanhDaoCucDaDuyet = 6,
        [Description("Lãnh đạo đơn vị từ chối duyệt")]
        LanhDaoDonViTuChoiDuyet = 7,
        [Description("Lãnh đạo Cục từ chối duyệt")]
        LanhDaoCucTuChoiDuyet = 8,
        [Description("Chờ phát hành")]
        ChoPhatHanh = 9,
        [Description("Đã phát hành")]
        DaPhatHanh = 10,
        [Description("Chờ góp ý")]
        ChoGopY = 11
    }

    public enum TrangThaiDuThaoVBNguoiDung
    {
        [Description("Đã gửi")]
        DaGui = 1,
        [Description("Đã nhận")]
        DaNhan = 2,
        [Description("Trả lại")]
        TraLai = 3
    }

    public enum TrangThaiXuLyDuThao
    {
        [Description("Duyệt")]
        Duyet = 1,
        [Description("Từ chối")]
        TuChoi = 2,
        [Description("Trình lãnh đạo")]
        TrinhLanhDao = 3,
    }
}
