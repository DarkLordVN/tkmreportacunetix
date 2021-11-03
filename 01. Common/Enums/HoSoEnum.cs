using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Utils.Enums
{
    public enum BieuMauEmailEnum
    {
        [Description("Đang hoạt động")]
        DangHoatDong = 1,
        [Description("Ngừng hoạt động")]
        NgungHoatDong = 0,
    }

    public enum QuyTrinhEnum
    {
        [Description("Đăng ký email")]
        DangKyEmail = 1,
        [Description("Đăng ký vé gửi xe")]
        DangKyVeGuiXe = 2,
        [Description("Đăng ký tài sản")]
        DangKyTaiSan = 3,
        [Description("Đăng ký văn phòng phẩm")]
        VanPhongPham = 4,
        [Description("Chuyển phát nhanh")]
        ChuyenPhatNhanh = 5,
        [Description("Cắt cơm trưa")]
        CatComTrua = 6,
        [Description("Đón tiếp khách")]
        DonTiepKhach = 7,
        [Description("Xe đưa đón")]
        XeDuaDon = 8,
        [Description("Đặt khách sạn")]
        DatKhachSan = 9,
        [Description("Vé máy bay")]
        VeMayBay = 10,
        [Description("Kế hoạch công tác")]
        KeHoachCongTac = 11,
        [Description("Điều động công tác")]
        DieuDongCongTac = 12,
        [Description("Tạm ứng công tác phí")]
        TamUngCongTacPhi = 13,
        [Description("Tờ trình")]
        ToTrinhDuyetNganSach = 14,
        [Description("Yêu cầu mua hàng")]
        YeuCauMuaHang = 15,
        [Description("Lựa chọn nhà cung cấp")]
        LuaChonNhaCungCap = 16,
        [Description("Đề nghị ký hợp đồng")]
        DeNghiKyHD = 17,
        [Description("Công văn đến HTC")]
        CVDenHTC = 18,
        [Description("Công văn đi HTC")]
        CVDiHTC = 19,
        [Description("Công văn đến đại lý")]
        CVDenDaiLy = 20,
        [Description("Công văn đi đại lý")]
        CVDiDaiLy = 21,
        [Description("Kế hoạch công tác năm")]
        KeHoachCongTacNam = 22,
    }
    public enum TrangThaiHoanThanhEnum
    {
        [Description("Đang chờ")]
        DangCho = 1,
        [Description("Đã duyệt")]
        DaDuyet = 0,
    }

    public enum NhomKhachEnum
    {
        [Description("Khách quan trọng, HMC (chủ tịch, phó chủ tịch)")]
        Nhom1 = 1,
        [Description("Khách HMC cấp trưởng phòng trở lên")]
        Nhom2 = 2,
        [Description("Khách đối tác chiến lược")]
        Nhom3 = 3,
        [Description("Khác")]
        Nhom4 = 4,
    }

    public enum XacNhanChuyenPhat
    {
        [Description("Đã nhận")]
        idXacNhan1 = 1,
        [Description("Thư chậm")]
        idXacNhan2 = 2,
        [Description("Mất/Thất lạc")]
        idXacNhan3 = 3,
        [Description("Từ chối")]
        idXacNhan4 = 4,
    }
}
