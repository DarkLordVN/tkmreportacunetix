using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Utils.Enums
{
    public enum StatusRole
    {
        [Description("Đang hoạt động")]
        DangHoatDong = 1,
        [Description("Ngừng hoạt động")]
        NgungHoatDong = 0,
    }
    public enum StatusUser
    {
        [Description("Còn hiệu lực")]
        ConHieuLuc = 1,
        [Description("Không sử dụng")]
        HetHieuLuc = 2,
    }
    public enum EDmChucVu
    {
        [Description("Chủ tịch HĐQT")]
        CTHDQT = 1,
        [Description("Tổng giám đốc")]
        TGĐ = 2,
        [Description("Phó tổng giám đốc")]
        PTGĐ = 3,
        [Description("Giám đốc")]
        GĐ = 4,
        [Description("Phó giám đốc")]
        PGĐ = 5,
        [Description("Trưởng phòng")]
        TP = 6,
        [Description("Phó phòng")]
        PP = 7,
        [Description("Nhân viên")]
        NV = 10,
    }
    public enum EVungMien
    {
        [Description("Miền Bắc")]
        MIEN_BAC = 1,
        [Description("Miền Trung")]
        MIEN_TRUNG = 2,
        [Description("Miền Nam")]
        MIEN_NAM = 3
    }
    public enum HOSTNAME
    {
        [Description("http://portalTKM.tinhvan.com/")]
        HostURl = 1
    }

    public enum ThuVienPhapLuatEnum
    {
        [Description("Các văn bản pháp luật")]
        VBPL = 1,
        [Description("Quy chế, tiêu chuẩn")]
        QCTC = 2,
        [Description("Biểu thuế")]
        BT = 3,
        [Description("Các hiệp định thương mại")]
        HDTM = 4,
        [Description("Thông tư, nghị định")]
        TTND = 5,
        [Description("Chính sách hỗ trợ")]
        CSHT = 6
    }
    public enum TypeAlbum
    {
        [Description("Hình ảnh")]
        HinhAnh = 1,
        [Description("Video")]
        Video = 2,
        [Description("Tài liệu tiếng việt")]
        DocTiengViet = 3,
        [Description("Tài liệu tiếng anh")]
        DocTiengAnh = 4
    }
    public enum Quyen
    {
        [Description("Thêm mới")]
        Add = 1,
        [Description("Chỉnh sửa")]
        Update = 2,
        [Description("Xem chi tiết")]
        View = 3,
        [Description("Tìm kiếm")]
        Search = 4,
        [Description("Xóa")]
        Delete = 5,
        [Description("Xóa nhiều")]
        MulDelete = 6,
    }
    public enum LoaiLienKetVanBan
    {
        [Description("Bị thay thế")]
        BiThayThe = 1,
        [Description("Bị thu hồi")]
        BiThuHoi = 2,
        [Description("Được trả lời")]
        DuocTraLoi = 3,
        [Description("Là văn bản trả lời")]
        LaVanBanTraLoi = 4,
        [Description("Cùng nội dung")]
        CungNoiDung = 5
    }
    public enum TrangThaiVanBan
    {
        [Description("Chờ xử lý")]
        ChoXuLy = 1,
        [Description("Đã phân công")]
        PhanCong = 2,
        [Description("Đang xử lý")]
        DangXuLy = 3,
        [Description("Hoàn thành")]
        HoanThanh = 4
    }
    public enum TrangThaiNguoiDung
    {
        [Description("Chưa xử lý")]
        ChuaXuLy = 1,
        [Description("Đang xử lý")]
        DangXuLy = 2,
        [Description("Hoàn thành")]
        HoanThanh = 3
    }
    public enum TrangThaiKieuXuLy
    {
        [Description("Xử lý chính")]
        XuLyChinh = 1,
        [Description("Theo dõi")]
        TheoDoi = 2,
        [Description("Phối hợp")]
        PhoiHop = 3,
        [Description("Nhận để biết")]
        NhanDeBiet = 4
    }
    public enum TrangThaiQuaTrinhXuLy
    {
        [Description("Đã gửi")]
        DaGui = 1,
        [Description("Đã nhận")]
        DaNhan = 2,
        [Description("Trả lại")]
        TraLai = 3
    }
    public enum TrangThaiVanBanLT
    {
        [Description("Đã đến")]
        DaDen = 1,
        [Description("Từ chối tiếp nhận")]
        TuChoi = 2,
        [Description("Đã tiếp nhận")]
        TiepNhan = 3,
        [Description("Phân công")]
        PhanCong = 4,
        [Description("Đang xử lý")]
        DangXuLy = 5,
        [Description("Hoàn thành")]
        HoanThanh = 6,
        [Description("Bị lấy lại")]
        BiLayLai = 7,
        [Description("Bị thu hồi")]
        BiThuHoi = 8,
        [Description("Bị thay thế")]
        BiThayThe = 9,
        [Description("Bị cập nhật")]
        BiCapNhat = 10,
        [Description("Chờ xử lý")]
        ChoXuLy = 11
    }
    public enum TrangThaiYeuCauLT
    {
        [Description("Chưa xử lý")]
        DaDen = 1,
        [Description("Từ chối xử lý")]
        TuChoi = 2,
        [Description("Đã xử lý")]
        DaXuLy = 3
    }
    public enum LoaiNghiepVuVanBanLT
    {
        [Description("Thêm mới")]
        ThemMoi = 0,
        [Description("Thu hồi")]
        ThuHoi = 1,
        [Description("Cập nhật")]
        CapNhat = 2,
        [Description("Thay thế")]
        ThayThe = 3,
        [Description("Lấy lại")]
        LayLai = 4
    }
    public enum DoKhan
    {
        [Description("Thường")]
        Thuong = 0,
        [Description("Khẩn")]
        Khan = 1,
        [Description("Thượng khẩn")]
        ThuongKhan = 2,
        [Description("Hỏa tốc")]
        HoaToc = 3,
        [Description("Hỏa tốc hẹn giờ")]
        HoaTocHenGio = 4
    }
    public enum NgonNgu
    {
        [Description("Tiếng việt")]
        TiengViet = 0,
        [Description("Tiếng anh")]
        TiengAnh = 1
    }
    public enum ThoiGianLichLanhDao
    {
        [Description("Sáng")]
        Sang = 1,
        [Description("Chiều")]
        Chieu = 2,
        [Description("Cả ngày")]
        CaNgay = 3,
    }
    public enum ChucNang
    {
        [Description("Văn bản đến")]
        VanBanDen = 1,
        [Description("Văn bản dự thảo")]
        VanBanDuThao = 2,
        [Description("Văn bản đi")]
        VanBanDi = 3,
        [Description("Công việc")]
        CongViec = 4,
        [Description("Thông báo")]
        ThongBao = 5
    }
}
