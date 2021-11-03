using TKM.BLL;
using TKM.Services;
using TKM.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xceed.Words.NET;

namespace TKM.WebApp.Controllers
{
    public class BaoCaoThongKeController : BaseController
    {
        private VanBanDenService _vanBanDenService;
        private VanBanDiService _vanBanDiService;
        private DoiTuongGuiVanBanDenService _doiTuongGuiVanBanDenService;
        private DoiTuongGuiVanBanDiService _doiTuongGuiVanBanDiService;
        private v_BaoCaoCTTNService _baoCaoCTTNService;
        private v_NoiDungCongViecService _noiDungCongViecService;
        private HoSoCongViecService _hoSoCongViecService;
        private DmLoaiVanBanService _loaiVanBanDenService;
        private VanBanDenTrangThaiService _vanBanDenTrangThaiService;
        private NguoiDungService _nguoiDungService;
        private DmDonViService _dmDonViService;
        private v_BCKQThucHienChiDaoService _BCKQThucHienChiDaoService;
        private v_ThongKeVBDenService _ThongKeVBDenService;
        private v_ThongKeVBDiService _ThongKeVBDiService;

        private string kieuBaoCao = "";
        string TuNgay = "";
        string DenNgay = "";

        //string DsVanBanDi = "";

        public BaoCaoThongKeController()
        {
            if (_vanBanDenService == null) _vanBanDenService = new VanBanDenService();
            if (_doiTuongGuiVanBanDenService == null) _doiTuongGuiVanBanDenService = new DoiTuongGuiVanBanDenService();
            if (_doiTuongGuiVanBanDiService == null) _doiTuongGuiVanBanDiService = new DoiTuongGuiVanBanDiService();
            if (_baoCaoCTTNService == null) _baoCaoCTTNService = new v_BaoCaoCTTNService();
            if (_vanBanDiService == null) _vanBanDiService = new VanBanDiService();
            if (_noiDungCongViecService == null) _noiDungCongViecService = new v_NoiDungCongViecService();
            if (_hoSoCongViecService == null) _hoSoCongViecService = new HoSoCongViecService();
            if (_loaiVanBanDenService == null) _loaiVanBanDenService = new DmLoaiVanBanService();
            if (_vanBanDenTrangThaiService == null) _vanBanDenTrangThaiService = new VanBanDenTrangThaiService();
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
            if (_dmDonViService == null) _dmDonViService = new DmDonViService();
            if (_BCKQThucHienChiDaoService == null) _BCKQThucHienChiDaoService = new v_BCKQThucHienChiDaoService();
            if (_ThongKeVBDenService == null) _ThongKeVBDenService = new v_ThongKeVBDenService();
            if (_ThongKeVBDiService == null) _ThongKeVBDiService = new v_ThongKeVBDiService();

        }


        // GET: BaoCaoThongKe
        public ActionResult Index(int LoaiBaoCao = 6)
        {
            switch (LoaiBaoCao)
            {
                case 1:
                    return Redirect("/BaoCaoThongKe/BaoCaoKetQuaThucHienChiDao?LoaiBaoCao=" + LoaiBaoCao);
                case 2:
                    return Redirect("/BaoCaoThongKe/ThongKeVanBan?LoaiBaoCao=" + LoaiBaoCao);
                case 3:
                    return Redirect("/BaoCaoThongKe/BaoCaoCongTacTiepNhanVaXuLy?LoaiBaoCao=" + LoaiBaoCao);
                case 4:
                    return Redirect("/BaoCaoThongKe/BaoCaoCongTacTiepNhanVaXuLy?LoaiBaoCao=" + LoaiBaoCao);
                case 5:
                    return Redirect("/BaoCaoThongKe/NoiDungCongViecThucHienTrongTuan?LoaiBaoCao=" + LoaiBaoCao);
                case 6:
                    return Redirect("/BaoCaoThongKe/BCKQThucHienNhiemVu?LoaiBaoCao=" + LoaiBaoCao);
                default:
                    return Redirect("/BaoCaoThongKe/BaoCaoKetQuaThucHienChiDao?LoaiBaoCao=" + LoaiBaoCao);
            }
        }

        public ViewResult SelectLoaiBaoCao(int LoaiBaoCao = 6)
        {
            ViewBag.LoaiBaoCao = LoaiBaoCao;
            return View();
        }

        #region 6. Bao cao ket qua thuc hien nhiem vu do Chinh phu, Thu tuong chinh phu giao
        public ActionResult BCKQThucHienNhiemVu()
        {
            v_BCKQThucHienChiDaoListView viewModel = new v_BCKQThucHienChiDaoListView();
            return View(viewModel);
        }

        public ActionResult BCKQThucHienNhiemVuData(v_BCKQThucHienChiDaoListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            int total = 0;

            List<v_BCKQThucHienChiDaoViewModel> lstResult = new List<v_BCKQThucHienChiDaoViewModel>();
            List<v_BCKQThucHienChiDaoViewModel> lstNoPaging = new List<v_BCKQThucHienChiDaoViewModel>();

            string dt1 = "Chính phủ";
            string dt2 = "Quốc hội";

            try
            {
                string tenDN = SessionInfo.CurrentUser.TenDangNhap;
                string admin = "admin";
                if (tenDN.ToLower().Equals(admin.ToLower()))
                {
                    viewModel.dvID = 0;
                }
                else
                {
                    viewModel.dvID = SessionInfo.CurrentUser.DonViID;
                }
            }
            catch { }

            lstResult = _BCKQThucHienChiDaoService.GetList(dt1, dt2, viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);

            lstNoPaging = _BCKQThucHienChiDaoService.GetList(dt1, dt2, viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, 0, 0, ref total, viewModel.ColumnName, viewModel.OrderBy);

            if (!string.IsNullOrEmpty(viewModel.DenNgay))
            {
                viewModel.TuNgay = viewModel.TuNgay.Replace(" ", "");
                viewModel.DenNgay = viewModel.DenNgay.Substring(3, 7);
            }
            Session["Thang"] = viewModel.DenNgay;
            Session["BCKQThucHienNhiemVu"] = lstNoPaging;
            viewModel.lstBCKQThucHienChiDao = lstResult;
            viewModel.lstNoPaging = lstNoPaging;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }

        public void ExportExcelBaoCaoKetQua()
        {
            var typeUser = SessionInfo.CurrentUser.TypeUser;
            using (var package = new ExcelPackage())
            {
                var Title = "TỔNG HỢP KẾT QUẢ THỰC HIỆN NHIỆM VỤ DO CHÍNH PHỦ, THỦ TƯỚNG CHÍNH PHỦ GIAO";
                var nameFile = "BaoCaoKetQuaThucHienNhiemVu";
                var lstVanBanDen = (List<v_BCKQThucHienChiDaoViewModel>)Session["BCKQThucHienNhiemVu"];
                var ws = package.Workbook.Worksheets.Add("Sheet1");
                // Tạo header
                ws.Cells["A1:F1"].Merge = true;
                ws.Cells["A2:F2"].Merge = true;
                ws.Cells["A3:F3"].Merge = true;
                ws.Cells["C4:D4"].Merge = true;

                ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                ws.Cells["A1"].Value = "PHỤ LỤC";
                ws.Cells["A2"].Value = Title;
                ws.Cells["A3"].Value = "(Kèm theo Công văn số 1399/ĐKVN-VP ngày 17 tháng 4 năm 2019)";
                ws.Cells["B4"].Value = "Đơn vị thực hiện: ";
                ws.Cells["C4"].Value = "Cục Đăng kiểm Việt Nam";
                ws.Cells["B5"].Value = "Tháng";
                ws.Cells["C5"].Value = Session["Thang"];

                ws.Cells["A1:F1"].Style.Font.Bold = true;
                ws.Cells["A2:F2"].Style.Font.Bold = true;
                ws.Cells["A3:F3"].Style.Font.Bold = true;
                ws.Cells["A3:F3"].Style.Font.Italic = true;
                ws.Cells["B4"].Style.Font.Bold = true;
                ws.Cells["C4"].Style.Font.Bold = true;
                ws.Cells["B5"].Style.Font.Bold = true;
                ws.Cells["C5"].Style.Font.Bold = true;

                //Header data
                ws.Cells["A7"].Value = "STT";
                ws.Cells["B7"].Value = "Số TBKL";
                ws.Cells["C7"].Value = "Trích yếu";
                ws.Cells["D7"].Value = "Nội dung nhiệm vụ giao của Chính phủ";
                ws.Cells["E7"].Value = "Kết quả thực hiển của đơn vị";
                ws.Cells["F7"].Value = "Trạng thái thực hiện";
                ws.Cells["A7:F7"].Style.Font.Bold = true;
                ws.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A:XJD"].Style.Font.Name = "Times New Roman";
                ws.Cells["A7:F7"].Style.WrapText = true;
                ws.Cells["A7:F7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:F7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:F7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:F7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Row(7).Height = 45;
                ws.Column(1).Width = 10;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 20;
                int row = 7;
                int i = 0;
                var countData = lstVanBanDen.Count();
                string trangThaiVanBan = "";
                int? trangThai = 0;
                foreach (var item in lstVanBanDen)
                {
                    row++;
                    i++;
                    ws.Cells["A" + row].Value = i.ToString();
                    ws.Cells["B" + row].Value = "Số: " + item.SoKyHieu + Environment.NewLine + "Ngày: " + item.NgayVanBan.Value.ToString("dd/MM/yyyy") + Environment.NewLine + "Lãnh đạo: " + item.NguoiKy;
                    ws.Cells["C" + row].Value = item.TrichYeu;
                    ws.Cells["D" + row].Value = item.NoiDungChiDao;
                    ws.Cells["E" + row].Value = item.KetQuaXuLy;
                    trangThai = item.TrangThaiVanBan;
                    switch (trangThai)
                    {
                        case 0:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                        case 1:
                            trangThaiVanBan = "Đã tiếp nhận";
                            break;
                        case 2:
                            trangThaiVanBan = "Phân công";
                            break;
                        case 3:
                            trangThaiVanBan = "Đang xử lý";
                            break;
                        case 4:
                            trangThaiVanBan = "Hoàn thành";
                            break;
                        default:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                    }
                    ws.Cells["F" + row].Value = trangThaiVanBan;

                    //Style
                    ws.Cells["A" + row + ":F" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":F" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":F" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":F" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    ws.Cells["A" + row + ":F" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + row + ":F" + row].Style.WrapText = true;
                }
                //Phần header
                string timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + string.Format("{0}-{1}{2}{3}_{4}.xlsx", nameFile, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeStr));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.BinaryWrite(package.GetAsByteArray());
                System.Web.HttpContext.Current.Response.End();
            }
        }

        public ActionResult ExportWordBaoCaoKetQua()
        {
            //var typeUser = SessionInfo.CurrentUser.TypeUser;
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            var TempPath = Server.MapPath("~/Content/Temp/");
            var gDoc = DocX.Load(TempPath + @"\Temp_BCKQThucHienNhiemVu.docx");
            #region Thay text
            gDoc.ReplaceText("%TieuDe%", "TỔNG HỢP KẾT QUẢ THỰC HIỆN NHIỆM VỤ DO CHÍNH PHỦ, THỦ TƯỚNG CHÍNH PHỦ GIAO");
            #endregion
            var lstVanBanDen = (List<v_BCKQThucHienChiDaoViewModel>)Session["BCKQThucHienNhiemVu"];
            //Table 0 : là tiêu đề
            //Table 1 : là bảng
            var tbl = gDoc.Tables[0];
            var index = 0;
            if (lstVanBanDen != null && lstVanBanDen.Count > 0)
            {
                foreach (var item in lstVanBanDen)
                {
                    var r = tbl.InsertRow();
                    ++index;

                    r.Cells[0].Paragraphs.First().Append(index.ToString()).Alignment = Alignment.center;
                    r.Cells[1].Paragraphs.First().Append("Số: " + item.SoKyHieu + Environment.NewLine + "Ngày: " + item.NgayVanBan.Value.ToString("dd/MM/yyyy") + Environment.NewLine + "Lãnh đạo: " + item.NguoiKy).Alignment = Alignment.left;
                    r.Cells[2].Paragraphs.First().Append(item.TrichYeu);
                    r.Cells[3].Paragraphs.First().Append(item.NoiDungChiDao).Alignment = Alignment.left;
                    r.Cells[4].Paragraphs.First().Append(item.KetQuaXuLy).Alignment = Alignment.left;

                    string trangThaiVanBan = "";
                    switch (item.TrangThaiVanBan)
                    {
                        case 0:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                        case 1:
                            trangThaiVanBan = "Đã tiếp nhận";
                            break;
                        case 2:
                            trangThaiVanBan = "Phân công";
                            break;
                        case 3:
                            trangThaiVanBan = "Đang xử lý";
                            break;
                        case 4:
                            trangThaiVanBan = "Hoàn thành";
                            break;
                        default:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                    }

                    r.Cells[5].Paragraphs.First().Append(trangThaiVanBan).Alignment = Alignment.left;
                    for (int i = 0; i < 6; i++)
                    {
                        r.Cells[i].InsertParagraph().Font(new Xceed.Words.NET.Font("Times New Roman")).FontSize(14);
                    }
                }
            }
            //var fName = DateTime.Now.ToString("InSoVanBanDen_ddMMyyyyhhmmss");
            //gDoc.SaveAs(TempPath + @"\"+ fName + ".docx");
            var path = "~/Content/Export";
            var fileName = "Word-BCKQThucHienNhiemVu-" + timeStr + ".docx";
            var pathSrv = Server.MapPath(path);
            if (!Directory.Exists(pathSrv))
            {
                Directory.CreateDirectory(pathSrv);
            }
            var outfile = TKM.Utils.CommonUtils.GetPath(path, fileName);
            gDoc.SaveAs(outfile);
            gDoc.Dispose();
            return File(outfile, "application/docx", fileName);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportPDFBaoCaoKetQua(FormCollection collection)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            string htmlString = collection["GridHtml"];
            string tenFile = collection["tenFilePDF"];
            string widthtable = collection["widthTable"];
            string baseUrl = "";

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Landscape";
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdf_orientation, true);

            int webPageWidth = Convert.ToInt32(widthtable);
            int webPageHeight = 0;

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.DisplayHeader = false;
            converter.Options.DisplayFooter = false;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            byte[] pdf = doc.Save();
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "PDF-" + tenFile + "-" + timeStr + ".pdf";
            return fileResult;
        }
        #endregion

        #region 5. Bang tong hop cac noi dung cong viec can thuc hien trong (Tuan/Thang/Quy/Nam)
        public ActionResult NoiDungCongViecThucHien(string loaiBaoCao, string tuNgay, string denNgay)
        {
            if (!string.IsNullOrEmpty(loaiBaoCao))
                kieuBaoCao = loaiBaoCao;
            else
                kieuBaoCao = "TUẦN";

            var viewModel = new v_NoiDungCongViecListView();

            return View(viewModel);
        }

        public ActionResult NoiDungCongViecThucHienData(v_NoiDungCongViecListView viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.kieuBaoCao))
                kieuBaoCao = "TUẦN";

            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;

            int total = 0;

            try
            {
                string tenDN = SessionInfo.CurrentUser.TenDangNhap;
                string admin = "admin";
                if (tenDN.ToLower().Equals(admin.ToLower()))
                {
                    viewModel.dvID = 0;
                }
                else
                {
                    viewModel.dvID = SessionInfo.CurrentUser.DonViID;
                }
            }
            catch { }

            var lst = _baoCaoCTTNService.GetList(viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, 0, 0, ref total,
                viewModel.ColumnName, viewModel.OrderBy);

            var lstResult = _noiDungCongViecService.GetList(viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total,
                viewModel.ColumnName, viewModel.OrderBy);

            var lstNoPaging = _noiDungCongViecService.GetList(viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, 0, 0, ref total,
                viewModel.ColumnName, viewModel.OrderBy);

            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lstResult)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (item.ID == lst[i].ID)
                        {
                            item.NgayBanHanh = lst[i].NgayBanHanh;
                            item.SoKyHieuVBDi = lst[i].SoKyHieuDi;
                            break;
                        }
                        else
                        {
                            item.NgayBanHanh = null;
                            item.SoKyHieuVBDi = "";
                        }
                    }
                }

                foreach (var item in lstNoPaging)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (item.ID == lst[i].ID)
                        {
                            item.NgayBanHanh = lst[i].NgayBanHanh;
                            item.SoKyHieuVBDi = lst[i].SoKyHieuDi;
                            break;
                        }
                        else
                        {
                            item.NgayBanHanh = null;
                            item.SoKyHieuVBDi = "";
                        }
                    }
                }
            }
            else
            {
                foreach (var item in lstResult)
                {
                    item.NgayBanHanh = null;
                    item.SoKyHieuVBDi = null;
                }

                foreach (var item in lstNoPaging)
                {
                    item.NgayBanHanh = null;
                    item.SoKyHieuVBDi = null;
                }
            }

            if (!string.IsNullOrEmpty(viewModel.DenNgay))
            {
                viewModel.DenNgay = viewModel.DenNgay.Replace(" ", "");
                viewModel.DenNgay = viewModel.DenNgay.Substring(3, 7);
            }

            Session["NoiDungCongViec"] = lstNoPaging;
            kieuBaoCao = viewModel.kieuBaoCao;
            TuNgay = viewModel.TuNgay;
            DenNgay = viewModel.DenNgay;
            viewModel.lstNoiDungCongViec = lstResult;
            viewModel.lstNoPaging = lstNoPaging;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }

        public void ExportExcelNoiDungThucHien()
        {
            using (var package = new ExcelPackage())
            {
                var Title = "BẢNG TỔNG HỢP NỘI DUNG CẦN THỰC HIỆN TRONG " + (!string.IsNullOrEmpty(kieuBaoCao) ? kieuBaoCao : "TUẦN");
                var nameFile = "NoiDungCanThucHien";
                var lstNoiDungCongViec = (List<v_NoiDungCongViecViewModel>)Session["NoiDungCongViec"];
                var ws = package.Workbook.Worksheets.Add("Sheet1");


                // Tạo header
                ws.Cells["A1:I1"].Merge = true;
                ws.Cells["A2:I2"].Merge = true;

                ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                ws.Cells["A1"].Value = Title;
                ws.Cells["A2"].Value = "(Từ ngày " + ((!string.IsNullOrEmpty(TuNgay)) ? TuNgay : "... / ... / ... ") + "đến " + ((!string.IsNullOrEmpty(DenNgay)) ? DenNgay : DateTime.Now.ToString("dd/MM/yyyy")) + ")";

                ws.Cells["A1:I1"].Style.Font.Bold = true;
                ws.Cells["A2:I2"].Style.Font.Italic = true;
                ws.Cells["A4:I4"].Style.Font.Bold = true;

                //Header data
                ws.Cells["A4"].Value = "STT";
                ws.Cells["B4"].Value = "Ký hiệu VB";
                ws.Cells["C4"].Value = "Ngày ký";
                ws.Cells["D4"].Value = "Nơi gửi";
                ws.Cells["E4"].Value = "Nội dung/Trích yếu";
                ws.Cells["F4"].Value = "Hạn xử lý/Báo cáo";
                ws.Cells["G4"].Value = "Lãnh đạo/Đơn vị thực hiện";
                ws.Cells["H4"].Value = "KQ thực hiện/Ngày báo cáo";
                ws.Cells["I4"].Value = "Ghi chú";


                ws.Cells["A4:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A:XJD"].Style.Font.Name = "Times New Roman";
                ws.Cells["A4:I4"].Style.WrapText = true;
                ws.Cells["A4:I4"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A4:I4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A4:I4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["A4:I4"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Row(4).Height = 45;
                ws.Column(1).Width = 10;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 20;
                ws.Column(7).Width = 20;
                ws.Column(8).Width = 20;
                ws.Column(9).Width = 20;

                int row = 4;
                int i = 0;
                var countData = lstNoiDungCongViec.Count();
                string trangThaiVanBan = "";
                int? trangThai = 0;
                foreach (var item in lstNoiDungCongViec)
                {
                    row++;
                    i++;
                    ws.Cells["A" + row].Value = i.ToString();
                    ws.Cells["B" + row].Value = item.SoKyHieuVBDen;
                    ws.Cells["C" + row].Value = item.NgayKy;
                    ws.Cells["D" + row].Value = item.NoiGui;
                    ws.Cells["E" + row].Value = item.NoiDung;
                    ws.Cells["F" + row].Value = item.HanXuLy;
                    ws.Cells["G" + row].Value = item.LanhDao;
                    ws.Cells["H" + row].Value = (!string.IsNullOrEmpty(item.SoKyHieuVBDi) ? ("Số: " + item.SoKyHieuVBDi) : "") + (item.NgayBanHanh != null ? (Environment.NewLine + "Ngày: " + item.NgayBanHanh.Value.ToString("dd/MM/yyyy")) : "") + item.KetQuaThucHien;
                    trangThai = item.GhiChu;
                    switch (trangThai)
                    {
                        case 0:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                        case 1:
                            trangThaiVanBan = "Đã tiếp nhận";
                            break;
                        case 2:
                            trangThaiVanBan = "Phân công";
                            break;
                        case 3:
                            trangThaiVanBan = "Đang xử lý";
                            break;
                        case 4:
                            trangThaiVanBan = "Hoàn thành";
                            break;
                        default:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                    }
                    ws.Cells["I" + row].Value = trangThaiVanBan;

                    //Style
                    ws.Cells["A" + row + ":I" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":I" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":I" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":I" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    ws.Cells["A" + row + ":I" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["C" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["G" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["I" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + row + ":I" + row].Style.WrapText = true;
                }
                //Phần header
                string timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + string.Format("{0}-{1}{2}{3}_{4}.xlsx", nameFile, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeStr));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.BinaryWrite(package.GetAsByteArray());
                System.Web.HttpContext.Current.Response.End();
            }
        }

        public ActionResult ExportWordNoiDungThucHien()
        {
            //var typeUser = SessionInfo.CurrentUser.TypeUser;
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            var TempPath = Server.MapPath("~/Content/Temp/");
            var gDoc = DocX.Load(TempPath + @"\Temp_NoiDungCongViec.docx");
            #region Thay text
            gDoc.ReplaceText("%TieuDe%", "BẢNG TỔNG HỢP NỘI DUNG CÔNG VIỆC CẦN THỰC HIỆN TRONG " + (!string.IsNullOrEmpty(kieuBaoCao) ? kieuBaoCao : "TUẦN"));
            gDoc.ReplaceText("%ThoiGian%", "(Từ ngày " + ((!string.IsNullOrEmpty(TuNgay)) ? TuNgay : "... / ... / ... ") + "đến " + ((!string.IsNullOrEmpty(DenNgay)) ? DenNgay : DateTime.Now.ToString("dd/MM/yyyy")) + ")");

            #endregion
            var lstNoiDungCongViec = (List<v_NoiDungCongViecViewModel>)Session["NoiDungCongViec"];
            //Table 0 : là tiêu đề
            //Table 1 : là bảng
            var tbl = gDoc.Tables[0];
            var index = 0;
            if (lstNoiDungCongViec != null && lstNoiDungCongViec.Count > 0)
            {
                foreach (var item in lstNoiDungCongViec)
                {
                    var r = tbl.InsertRow();
                    ++index;

                    r.Cells[0].Paragraphs.First().Append(index.ToString()).Alignment = Alignment.center;
                    r.Cells[1].Paragraphs.First().Append(item.SoKyHieuVBDen).Alignment = Alignment.left;
                    r.Cells[2].Paragraphs.First().Append(item.NgayKy != null ? item.NgayKy.Value.ToString("dd/MM/yyyy") : "").Alignment = Alignment.center;
                    r.Cells[3].Paragraphs.First().Append(item.NoiGui).Alignment = Alignment.left;
                    r.Cells[4].Paragraphs.First().Append(item.NoiDung).Alignment = Alignment.left;
                    r.Cells[5].Paragraphs.First().Append(item.HanXuLy != null ? item.HanXuLy.Value.ToString("dd/MM/yyyy") : "").Alignment = Alignment.center;
                    r.Cells[6].Paragraphs.First().Append(item.LanhDao).Alignment = Alignment.left;
                    r.Cells[7].Paragraphs.First().Append((!string.IsNullOrEmpty(item.SoKyHieuVBDi) ? ("Số: " + item.SoKyHieuVBDi) : "") + Environment.NewLine + (item.NgayBanHanh != null ? ("Ngày: " + item.NgayBanHanh.Value.ToString("dd/MM/yyyy")) : null) + item.KetQuaThucHien).Alignment = Alignment.left;

                    string trangThaiVanBan = "";
                    switch (item.GhiChu)
                    {
                        case 0:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                        case 1:
                            trangThaiVanBan = "Đã tiếp nhận";
                            break;
                        case 2:
                            trangThaiVanBan = "Phân công";
                            break;
                        case 3:
                            trangThaiVanBan = "Đang xử lý";
                            break;
                        case 4:
                            trangThaiVanBan = "Hoàn thành";
                            break;
                        default:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                    }

                    r.Cells[8].Paragraphs.First().Append(trangThaiVanBan).Alignment = Alignment.left;
                    for (int i = 0; i < 9; i++)
                    {
                        r.Cells[i].InsertParagraph().Font(new Xceed.Words.NET.Font("Times New Roman")).FontSize(14);
                    }
                }
            }
            //var fName = DateTime.Now.ToString("InSoVanBanDen_ddMMyyyyhhmmss");
            //gDoc.SaveAs(TempPath + @"\"+ fName + ".docx");
            var path = "~/Content/Export";
            var fileName = "Word-NoiDungCongViec-" + timeStr + ".docx";
            var pathSrv = Server.MapPath(path);
            if (!Directory.Exists(pathSrv))
            {
                Directory.CreateDirectory(pathSrv);
            }
            var outfile = TKM.Utils.CommonUtils.GetPath(path, fileName);
            gDoc.SaveAs(outfile);
            gDoc.Dispose();
            return File(outfile, "application/docx", fileName);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportPDFNoiDungThucHien(FormCollection collection)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            string htmlString = collection["GridHtml"];
            string tenFile = collection["tenFilePDF"];
            string widthtable = collection["widthTable"];
            string baseUrl = "";

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Landscape";
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdf_orientation, true);

            int webPageWidth = Convert.ToInt32(widthtable);
            int webPageHeight = 0;

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.DisplayHeader = false;
            converter.Options.DisplayFooter = false;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            byte[] pdf = doc.Save();
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "PDF-" + tenFile + "-" + timeStr + ".pdf";
            return fileResult;
        }
        #endregion

        #region 4. Bao cao cong tac tiep nhan va ket qua xu ly van ban den trong thang
        public ActionResult BaoCaoCongTacTiepNhanVaKetQuaXuLy(string TuNgay, string DenNgay)
        {
            var viewModel = new v_BaoCaoCTTNListView();
            return View(viewModel);
        }
        public ActionResult BaoCaoCongTacTiepNhanVaKetQuaXuLyData(v_BaoCaoCTTNListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;

            int total = 0;

            try
            {
                string tenDN = SessionInfo.CurrentUser.TenDangNhap;
                string admin = "admin";
                if (tenDN.ToLower().Equals(admin.ToLower()))
                {
                    viewModel.dvID = 0;
                }
                else
                {
                    viewModel.dvID = SessionInfo.CurrentUser.DonViID;
                }
            }
            catch { }

            var lstResult = _baoCaoCTTNService.GetList(viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total,
                viewModel.ColumnName, viewModel.OrderBy);

            var lstNoPaging = _baoCaoCTTNService.GetList(viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, 0, 0, ref total,
                viewModel.ColumnName, viewModel.OrderBy);

            if (!string.IsNullOrEmpty(viewModel.DenNgay))
            {
                viewModel.DenNgay = viewModel.DenNgay.Replace(" ", "");
                viewModel.DenNgay = viewModel.DenNgay.Substring(3, 7);
            }
            Session["Thang"] = viewModel.DenNgay;
            //Session["BCKQThucHienNhiemVu"] = lst;
            Session["BaoCaoCTTN"] = lstNoPaging;
            viewModel.lstBaoCaoCTTN = lstResult;
            viewModel.lstNoPaging = lstNoPaging;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }

        public void ExportExcelBaoCaoCongTacTiepNhanVaKetQuaXuLy()
        {
            var typeUser = SessionInfo.CurrentUser.TypeUser;
            using (var package = new ExcelPackage())
            {
                string thang = "";
                if (Session["Thang"] != null)
                    thang = Session["Thang"].ToString();
                else
                    thang = DateTime.Now.Date.Month + "/" + DateTime.Now.Year;
                var Title = "BÁO CÁO TÌNH HÌNH XỬ LÝ VĂN BẢN ĐẾN THÁNG " + thang;
                var nameFile = "BaoCaoKetQuaThucHienNhiemVu";
                var lstBaoCaoCTTN = (List<v_BaoCaoCTTNViewModel>)Session["BaoCaoCTTN"];
                var ws = package.Workbook.Worksheets.Add("Sheet1");
                // Tạo header
                ws.Cells["A1:G1"].Merge = true;
                ws.Cells["A2:G2"].Merge = true;
                ws.Cells["A3:G3"].Merge = true;
                ws.Cells["A5:A6"].Merge = true;
                ws.Cells["C5:E5"].Merge = true;
                ws.Cells["F5:F6"].Merge = true;
                ws.Cells["G5:G6"].Merge = true;

                ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                ws.Cells["A1"].Value = "PHỤ LỤC";
                ws.Cells["A2"].Value = Title;
                ws.Cells["A3"].Value = "(Kèm theo Văn bản số 6586/ĐKVN-VP ngày 22/10/2018)";

                ws.Cells["A1:G1"].Style.Font.Bold = true;
                ws.Cells["A2:G2"].Style.Font.Bold = true;
                ws.Cells["A3:G3"].Style.Font.Bold = true;
                ws.Cells["A3:G3"].Style.Font.Italic = true;

                //Header data
                ws.Cells["A5"].Value = "STT";
                ws.Cells["B5"].Value = "Công văn đến";
                ws.Cells["B6"].Value = "Nơi gửi, Số CV/ngày";
                ws.Cells["C5"].Value = "Công văn đi";
                ws.Cells["C6"].Value = "Ngày tháng";
                ws.Cells["D6"].Value = "Ký hiệu";
                ws.Cells["E6"].Value = "Trích yếu nội dung";
                ws.Cells["F5"].Value = "Đang giải quyết";
                ws.Cells["G5"].Value = "Chưa giải quyết";
                ws.Cells["A5:G5"].Style.Font.Bold = true;
                ws.Cells["A6:G6"].Style.Font.Bold = true;
                ws.Cells["A5:G5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A5:G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A6:G6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A6:G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A:XJD"].Style.Font.Name = "Times New Roman";
                ws.Cells["A5:G5"].Style.WrapText = true;
                ws.Cells["A5:G5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:G5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:G5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:G5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:G6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:G6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:G6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:G6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Row(7).Height = 45;
                ws.Column(1).Width = 10;
                ws.Column(2).Width = 30;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 40;
                ws.Column(6).Width = 20;
                ws.Column(7).Width = 20;

                int row = 6;
                int i = 0;
                var countData = lstBaoCaoCTTN.Count();

                foreach (var item in lstBaoCaoCTTN)
                {
                    row++;
                    i++;
                    ws.Cells["A" + row].Value = i.ToString();
                    ws.Cells["B" + row].Value = item.NoiGui + ", số: " + @item.SoKyHieuDen + Environment.NewLine + "Ngày: " + @item.NgayVanBan.Value.ToString("dd/MM/yyyy");
                    ws.Cells["C" + row].Value = item.NgayBanHanh.Value.ToString("dd/MM/yyyy");
                    ws.Cells["D" + row].Value = item.SoKyHieuDi;
                    ws.Cells["E" + row].Value = item.TrichYeu;

                    //Style
                    ws.Cells["A" + row + ":G" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":G" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":G" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":G" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    ws.Cells["A" + row + ":G" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["G" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + row + ":G" + row].Style.WrapText = true;
                }
                //Phần header
                string timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + string.Format("{0}-{1}{2}{3}_{4}.xlsx", nameFile, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeStr));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.BinaryWrite(package.GetAsByteArray());
                System.Web.HttpContext.Current.Response.End();
            }
        }

        public ActionResult ExportWordBaoCaoCongTacTiepNhanVaKetQuaXuLy()
        {
            //var typeUser = SessionInfo.CurrentUser.TypeUser;
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            var TempPath = Server.MapPath("~/Content/Temp/");
            var gDoc = DocX.Load(TempPath + @"\Temp_BaoCaoCTTN.docx");
            #region Thay text
            string thang = "";
            if (Session["Thang"] != null)
                thang = Session["Thang"].ToString();
            else
                thang = DateTime.Now.Date.Month + "/" + DateTime.Now.Year;
            gDoc.ReplaceText("%TieuDe%", "BÁO CÁO TÌNH HÌNH XỬ LÝ VĂN BẢN ĐẾN THÁNG " + thang);
            gDoc.ReplaceText("%TITLE%", "(Kèm theo Văn bản số 6586/ĐKVN-VP ngày 22/10/2018)");
            #endregion
            var lstBaoCaoCTTN = (List<v_BaoCaoCTTNViewModel>)Session["BaoCaoCTTN"];
            //Table 0 : là tiêu đề
            //Table 1 : là bảng
            var tbl = gDoc.Tables[0];
            var index = 0;
            if (lstBaoCaoCTTN != null && lstBaoCaoCTTN.Count > 0)
            {
                foreach (var item in lstBaoCaoCTTN)
                {
                    var r = tbl.InsertRow();
                    ++index;

                    r.Cells[0].Paragraphs.First().Append(index.ToString()).Alignment = Alignment.center;
                    r.Cells[1].Paragraphs.First().Append(item.NoiGui + ", số: " + @item.SoKyHieuDen + Environment.NewLine + "Ngày: " + @item.NgayVanBan.Value.ToString("dd/MM/yyyy")).Alignment = Alignment.left;
                    r.Cells[2].Paragraphs.First().Append(item.NgayBanHanh.Value.ToString("dd/MM/yyyy")).Alignment = Alignment.center;
                    r.Cells[3].Paragraphs.First().Append(item.SoKyHieuDi).Alignment = Alignment.left;
                    r.Cells[4].Paragraphs.First().Append(item.TrichYeu).Alignment = Alignment.left;
                    r.Cells[5].Paragraphs.First().Append("").Alignment = Alignment.center;
                    r.Cells[6].Paragraphs.First().Append("").Alignment = Alignment.center;

                    for (int i = 0; i < 6; i++)
                    {
                        r.Cells[i].InsertParagraph().Font(new Xceed.Words.NET.Font("Times New Roman")).FontSize(14);
                    }
                }
            }
            //var fName = DateTime.Now.ToString("InSoVanBanDen_ddMMyyyyhhmmss");
            //gDoc.SaveAs(TempPath + @"\"+ fName + ".docx");
            var path = "~/Content/Export";
            var fileName = "Word-BaoCaoCTTN-" + timeStr + ".docx";
            var pathSrv = Server.MapPath(path);
            if (!Directory.Exists(pathSrv))
            {
                Directory.CreateDirectory(pathSrv);
            }
            var outfile = TKM.Utils.CommonUtils.GetPath(path, fileName);
            gDoc.SaveAs(outfile);
            gDoc.Dispose();
            return File(outfile, "application/docx", fileName);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportPDFBaoCaoCongTacTiepNhanVaKetQuaXuLy(FormCollection collection)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            string htmlString = collection["GridHtml"];
            string tenFile = collection["tenFilePDF"];
            string widthtable = collection["widthTable"];
            string baseUrl = "";

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Landscape";
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdf_orientation, true);

            int webPageWidth = Convert.ToInt32(widthtable);
            int webPageHeight = 0;

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.DisplayHeader = false;
            converter.Options.DisplayFooter = false;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            byte[] pdf = doc.Save();
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "PDF-" + tenFile + "-" + timeStr + ".pdf";
            return fileResult;
        }
        #endregion

        #region 3. Bao cao ket qua thuc hien chi dao cua Lanh dao tai cac TBKL cua (Chinh phu/Bo/Cuc)
        public ActionResult BaoCaoKetQuaThuchienChiDao()
        {
            v_BCKQThucHienChiDaoListView viewModel = new v_BCKQThucHienChiDaoListView();
            return View(viewModel);
        }

        public ActionResult BaoCaoKetQuaThuchienChiDaoData(v_BCKQThucHienChiDaoListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;

            int total = 0;

            List<v_BCKQThucHienChiDaoViewModel> lstResult = new List<v_BCKQThucHienChiDaoViewModel>();
            List<v_BCKQThucHienChiDaoViewModel> lstNoPaging = new List<v_BCKQThucHienChiDaoViewModel>();

            try
            {
                string tenDN = SessionInfo.CurrentUser.TenDangNhap;
                string admin = "admin";
                if (tenDN.ToLower().Equals(admin.ToLower()))
                {
                    viewModel.dvID = 0;
                }
                else
                {
                    viewModel.dvID = SessionInfo.CurrentUser.DonViID;
                }
            }
            catch { }

            if (viewModel.DoiTuongGuiVanBanDenID == 0)
            {
                lstResult = _BCKQThucHienChiDaoService.GetList(null, null, viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);

                lstNoPaging = _BCKQThucHienChiDaoService.GetList(null, null, viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, 0, 0, ref total, viewModel.ColumnName, viewModel.OrderBy);
            }
            else if (viewModel.DoiTuongGuiVanBanDenID == 1)
            {
                string dt1 = "Bộ GTVT";
                string dt2 = "Bộ giao thông vận tải";
                lstResult = _BCKQThucHienChiDaoService.GetList(dt1, dt2, viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);

                lstNoPaging = _BCKQThucHienChiDaoService.GetList(dt1, dt2, viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, 0, 0, ref total, viewModel.ColumnName, viewModel.OrderBy);
            }
            else if (viewModel.DoiTuongGuiVanBanDenID == 2)
            {
                string dt1 = "Chính phủ";
                string dt2 = "Quốc hội";
                lstResult = _BCKQThucHienChiDaoService.GetList(dt1, dt2, viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, viewModel.PageNumber.Value, viewModel.PageSize.Value, ref total, viewModel.ColumnName, viewModel.OrderBy);

                lstNoPaging = _BCKQThucHienChiDaoService.GetList(dt1, dt2, viewModel.TuNgay, viewModel.DenNgay, viewModel.dvID, 0, 0, ref total, viewModel.ColumnName, viewModel.OrderBy);
            }

            if (!string.IsNullOrEmpty(viewModel.DenNgay))
            {
                viewModel.TuNgay = viewModel.TuNgay.Replace(" ", "");
                viewModel.DenNgay = viewModel.DenNgay.Substring(3, 7);
            }
            Session["Thang"] = viewModel.DenNgay;
            Session["BCKQThucHienNhiemVu"] = lstNoPaging;
            viewModel.lstBCKQThucHienChiDao = lstResult;

            viewModel.lstNoPaging = lstNoPaging;
            viewModel.TotalItem = total;

            if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
            {
                int[] totals = new int[total];
                ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
            }
            return PartialView(viewModel);
        }

        public void ExportExcelBaoCaoKetQuaThuchienChiDao()
        {
            var typeUser = SessionInfo.CurrentUser.TypeUser;
            using (var package = new ExcelPackage())
            {
                var Title = "TỔNG HỢP KẾT QUẢ THỰC HIỆN CHỈ ĐẠO CỦA LÃNH ĐẠO BỘ GTVT TẠI CÁC TBKL";
                var nameFile = "BaoCaoKetQuaThucHienNhiemVu";
                var lstVanBanDen = (List<v_BCKQThucHienChiDaoViewModel>)Session["BCKQThucHienNhiemVu"];
                var ws = package.Workbook.Worksheets.Add("Sheet1");
                // Tạo header
                ws.Cells["A1:F1"].Merge = true;
                ws.Cells["A2:F2"].Merge = true;
                ws.Cells["A3:F3"].Merge = true;
                ws.Cells["C4:D4"].Merge = true;

                ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                ws.Cells["A1"].Value = "PHỤ LỤC";
                ws.Cells["A2"].Value = Title;
                ws.Cells["A3"].Value = "(Kèm theo Công văn số 1399/ĐKVN-VP ngày 17 tháng 4 năm 2019)";
                ws.Cells["B4"].Value = "Đơn vị thực hiện: ";
                ws.Cells["C4"].Value = "Cục Đăng kiểm Việt Nam";
                ws.Cells["B5"].Value = "Tháng";
                ws.Cells["C5"].Value = Session["Thang"];

                ws.Cells["A1:F1"].Style.Font.Bold = true;
                ws.Cells["A2:F2"].Style.Font.Bold = true;
                ws.Cells["A3:F3"].Style.Font.Bold = true;
                ws.Cells["A3:F3"].Style.Font.Italic = true;
                ws.Cells["B4"].Style.Font.Bold = true;
                ws.Cells["C4"].Style.Font.Bold = true;
                ws.Cells["B5"].Style.Font.Bold = true;
                ws.Cells["C5"].Style.Font.Bold = true;

                //Header data
                ws.Cells["A7"].Value = "STT";
                ws.Cells["B7"].Value = "Số TBKL";
                ws.Cells["C7"].Value = "Trích yếu";
                ws.Cells["D7"].Value = "Nội dung nhiệm vụ giao của Chính phủ";
                ws.Cells["E7"].Value = "Kết quả thực hiển của đơn vị";
                ws.Cells["F7"].Value = "Trạng thái thực hiện";
                ws.Cells["A7:F7"].Style.Font.Bold = true;
                ws.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A:XJD"].Style.Font.Name = "Times New Roman";
                ws.Cells["A7:F7"].Style.WrapText = true;
                ws.Cells["A7:F7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:F7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:F7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:F7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Row(7).Height = 45;
                ws.Column(1).Width = 10;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 20;
                int row = 7;
                int i = 0;
                var countData = lstVanBanDen.Count();
                string trangThaiVanBan = "";
                int? trangThai = 0;
                foreach (var item in lstVanBanDen)
                {
                    row++;
                    i++;
                    ws.Cells["A" + row].Value = i.ToString();
                    ws.Cells["B" + row].Value = "Số: " + item.SoKyHieu + Environment.NewLine + "Ngày: " + item.NgayVanBan.Value.ToString("dd/MM/yyyy") + Environment.NewLine + "Lãnh đạo: " + item.NguoiKy;
                    ws.Cells["C" + row].Value = item.TrichYeu;
                    ws.Cells["D" + row].Value = item.NoiDungChiDao;
                    ws.Cells["E" + row].Value = item.KetQuaXuLy;
                    trangThai = item.TrangThaiVanBan;
                    switch (trangThai)
                    {
                        case 0:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                        case 1:
                            trangThaiVanBan = "Đã tiếp nhận";
                            break;
                        case 2:
                            trangThaiVanBan = "Phân công";
                            break;
                        case 3:
                            trangThaiVanBan = "Đang xử lý";
                            break;
                        case 4:
                            trangThaiVanBan = "Hoàn thành";
                            break;
                        default:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                    }
                    ws.Cells["F" + row].Value = trangThaiVanBan;

                    //Style
                    ws.Cells["A" + row + ":F" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":F" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":F" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + row + ":F" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    ws.Cells["A" + row + ":F" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + row + ":F" + row].Style.WrapText = true;
                }
                //Phần header
                string timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + string.Format("{0}-{1}{2}{3}_{4}.xlsx", nameFile, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeStr));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.BinaryWrite(package.GetAsByteArray());
                System.Web.HttpContext.Current.Response.End();
            }
        }

        public ActionResult ExportWordBaoCaoKetQuaThuchienChiDao()
        {
            //var typeUser = SessionInfo.CurrentUser.TypeUser;
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            var TempPath = Server.MapPath("~/Content/Temp/");
            var gDoc = DocX.Load(TempPath + @"\Temp_BCKQThucHienNhiemVu.docx");
            #region Thay text
            gDoc.ReplaceText("%TieuDe%", "TỔNG HỢP KẾT QUẢ THỰC HIỆN NHIỆM VỤ DO CHÍNH PHỦ, THỦ TƯỚNG CHÍNH PHỦ GIAO");
            #endregion
            var lstVanBanDen = (List<v_BCKQThucHienChiDaoViewModel>)Session["BCKQThucHienNhiemVu"];
            //Table 0 : là tiêu đề
            //Table 1 : là bảng
            var tbl = gDoc.Tables[0];
            var index = 0;
            if (lstVanBanDen != null && lstVanBanDen.Count > 0)
            {
                foreach (var item in lstVanBanDen)
                {
                    var r = tbl.InsertRow();
                    ++index;

                    r.Cells[0].Paragraphs.First().Append(index.ToString()).Alignment = Alignment.center;
                    r.Cells[1].Paragraphs.First().Append("Số: " + item.SoKyHieu + Environment.NewLine + "Ngày: " + item.NgayVanBan.Value.ToString("dd/MM/yyyy") + Environment.NewLine + "Lãnh đạo: " + item.NguoiKy).Alignment = Alignment.left;
                    r.Cells[2].Paragraphs.First().Append(item.TrichYeu);
                    r.Cells[3].Paragraphs.First().Append(item.NoiDungChiDao).Alignment = Alignment.left;
                    r.Cells[4].Paragraphs.First().Append(item.KetQuaXuLy).Alignment = Alignment.left;

                    string trangThaiVanBan = "";
                    switch (item.TrangThaiVanBan)
                    {
                        case 0:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                        case 1:
                            trangThaiVanBan = "Đã tiếp nhận";
                            break;
                        case 2:
                            trangThaiVanBan = "Phân công";
                            break;
                        case 3:
                            trangThaiVanBan = "Đang xử lý";
                            break;
                        case 4:
                            trangThaiVanBan = "Hoàn thành";
                            break;
                        default:
                            trangThaiVanBan = "Chưa tiếp nhận";
                            break;
                    }

                    r.Cells[5].Paragraphs.First().Append(trangThaiVanBan).Alignment = Alignment.left;
                    for (int i = 0; i < 6; i++)
                    {
                        r.Cells[i].InsertParagraph().Font(new Xceed.Words.NET.Font("Times New Roman")).FontSize(14);
                    }
                }
            }
            //var fName = DateTime.Now.ToString("InSoVanBanDen_ddMMyyyyhhmmss");
            //gDoc.SaveAs(TempPath + @"\"+ fName + ".docx");
            var path = "~/Content/Export";
            var fileName = "Word-BCKQThucHienNhiemVu-" + timeStr + ".docx";
            var pathSrv = Server.MapPath(path);
            if (!Directory.Exists(pathSrv))
            {
                Directory.CreateDirectory(pathSrv);
            }
            var outfile = TKM.Utils.CommonUtils.GetPath(path, fileName);
            gDoc.SaveAs(outfile);
            gDoc.Dispose();
            return File(outfile, "application/docx", fileName);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportPDFBaoCaoKetQuaThuchienChiDao(FormCollection collection)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            string htmlString = collection["GridHtml"];
            string tenFile = collection["tenFilePDF"];
            string widthtable = collection["widthTable"];
            string baseUrl = "";

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Landscape";
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdf_orientation, true);

            int webPageWidth = Convert.ToInt32(widthtable);
            int webPageHeight = 0;

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.DisplayHeader = false;
            converter.Options.DisplayFooter = false;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            byte[] pdf = doc.Save();
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "PDF-" + tenFile + "-" + timeStr + ".pdf";
            return fileResult;
        }
        #endregion

        #region 2. Bao cao cong tac tiep nhan va xu ly van ban
        public ActionResult BaoCaoCongTacTiepNhanVaXuLyVanBan(string TuNgayNVB, string DenNgayNVB)
        {
            ViewBag.TuNgayNVB = TuNgayNVB;
            ViewBag.DenNgayNVB = DenNgayNVB;
            v_BaoCaoCTTNListView viewModel = new v_BaoCaoCTTNListView();

            return View(viewModel);
        }

        public ActionResult BaoCaoCongTacTiepNhanVaXuLyVanBanData(v_BaoCaoCTTNListView viewModel)
        {
            var dicTong = new Dictionary<string, int>();
            //VBDi
            var dicDoiTuongGuiVBDi = new Dictionary<string, int>();
            var lstDoiTuongGuiVBDi = new List<DoiTuongGuiVanBanDiViewModel>();
            //VBDen DoiTuongGuiVBDen
            var dicDoiTuongGuiVBDen = new Dictionary<string, int>();
            var lstDoiTuongGuiVBDen = new List<DoiTuongGuiVanBanDenViewModel>();
            //LoaiVBDen
            var dicLoaiVBDen = new Dictionary<string, int>();
            var lstLoaiVBDen = new List<DmLoaiVanBanViewModel>();

            DateTime? tuNgay = null, denNgay = null;
            if (!string.IsNullOrEmpty(viewModel.DenNgay))
            {
                viewModel.DenNgay = viewModel.DenNgay.Replace(" ", "");
                denNgay = DateTime.ParseExact(viewModel.DenNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
                denNgay = DateTime.Now;

            if (!string.IsNullOrEmpty(viewModel.TuNgay))
            {
                viewModel.TuNgay = viewModel.TuNgay.Replace(" ", "");
                tuNgay = DateTime.ParseExact(viewModel.TuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                if (denNgay != null)
                    tuNgay = new DateTime(DateTime.Now.Year, denNgay.Value.Month, 1);
                else
                    tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            int donViLoginId = 0;
            try
            {
                string tenDN = SessionInfo.CurrentUser.TenDangNhap;
                string admin = "admin";
                if (tenDN.ToLower().Equals(admin.ToLower()))
                {
                    donViLoginId = 0;
                }
                else
                {
                    donViLoginId = SessionInfo.CurrentUser.DonViID;
                }

                var isDonVi = false;
                var objDonVi = _dmDonViService.GetById(SessionInfo.CurrentUser.DonViID);
                if (objDonVi != null && objDonVi.KhoaChaID != 0 && objDonVi.IsDonVi)
                    isDonVi = true;

                ViewBag.TuNgayNVB = viewModel.TuNgay;
                ViewBag.DenNgayNVB = viewModel.DenNgay;
                Session["TuNgay"] = viewModel.TuNgay;
                Session["DenNgay"] = viewModel.DenNgay;

                _vanBanDenService.BaoCaoCongTacTiepNhanVaXuLyVanBanData(ref dicDoiTuongGuiVBDi, ref lstDoiTuongGuiVBDi, ref dicDoiTuongGuiVBDen, ref lstDoiTuongGuiVBDen, ref dicLoaiVBDen, ref lstLoaiVBDen, ref dicTong, tuNgay.Value.ToString("dd/MM/yyyy"), tuNgay.Value, denNgay.Value.ToString("dd/MM/yyyy"), denNgay.Value, SessionInfo.CurrentUser.TypeUser, SessionInfo.CurrentUser.CapBac, isDonVi, tenDN, SessionInfo.CurrentUser.ID, donViLoginId);


                viewModel.lstDoiTuongGuiVBDi = lstDoiTuongGuiVBDi;
                viewModel.dicDoiTuongGuiVBDi = dicDoiTuongGuiVBDi;
                Session["lstDoiTuongGuiVBDi"] = lstDoiTuongGuiVBDi;
                Session["dicDoiTuongGuiVBDi"] = dicDoiTuongGuiVBDi;
                viewModel.lstDoiTuongGuiVBDen = lstDoiTuongGuiVBDen;
                viewModel.dicDoiTuongGuiVBDen = dicDoiTuongGuiVBDen;
                Session["lstDoiTuongGuiVBDen"] = lstDoiTuongGuiVBDen;
                Session["dicDoiTuongGuiVBDen"] = dicDoiTuongGuiVBDen;
                viewModel.lstLoaiVBDen = lstLoaiVBDen;
                viewModel.dicLoaiVBDen = dicLoaiVBDen;
                Session["lstLoaiVBDen"] = lstLoaiVBDen;
                Session["dicLoaiVBDen"] = dicLoaiVBDen;
                viewModel.dicTong = dicTong;
                Session["dicTong"] = dicTong;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            //DateTime? tuNgay = null, denNgay = null;

            //int dvID = 0;

            //if (!string.IsNullOrEmpty(viewModel.DenNgay))
            //{
            //    viewModel.DenNgay = viewModel.DenNgay.Replace(" ", "");
            //    denNgay = DateTime.ParseExact(viewModel.DenNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //}
            //else
            //    denNgay = DateTime.Now;

            //if (!string.IsNullOrEmpty(viewModel.TuNgay))
            //{
            //    viewModel.TuNgay = viewModel.TuNgay.Replace(" ", "");
            //    tuNgay = DateTime.ParseExact(viewModel.TuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //}
            //else
            //{
            //    if (denNgay != null)
            //        tuNgay = new DateTime(DateTime.Now.Year, denNgay.Value.Month, 1);
            //    else
            //        tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //}

            //try
            //{
            //    string tenDN = SessionInfo.CurrentUser.TenDangNhap;
            //    string admin = "admin";
            //    if (tenDN.ToLower().Equals(admin.ToLower()))
            //    {
            //        dvID = 0;
            //    }
            //    else
            //    {
            //        dvID = SessionInfo.CurrentUser.DonViID;
            //    }
            //}
            //catch { }

            //ViewBag.TuNgayNVB = viewModel.TuNgay;
            //ViewBag.DenNgayNVB = viewModel.DenNgay;
            //Session["TuNgay"] = viewModel.TuNgay;
            //Session["DenNgay"] = viewModel.DenNgay;

            //var dicTong = new Dictionary<string, int>();

            ////VBDi
            //var dicDoiTuongGuiVBDi = new Dictionary<string, int>();
            //var lstDoiTuongGuiVBDi = new List<DoiTuongGuiVanBanDiViewModel>();
            //lstDoiTuongGuiVBDi = _doiTuongGuiVanBanDiService.GetList(x => x.IsDeleted == false && x.TrangThai == true).OrderByDescending(x => x.LoaiGuiVanBan).ToList();

            //if (lstDoiTuongGuiVBDi != null && lstDoiTuongGuiVBDi.Count > 0)
            //{
            //    int getValue = 0;
            //    int doiTuongID = 0;
            //    int tong = 0;
            //    for (int i = 0; i < lstDoiTuongGuiVBDi.Count; i++)
            //    {
            //        doiTuongID = lstDoiTuongGuiVBDi[i].ID;
            //        if (dvID != 0)
            //            getValue = _ThongKeVBDiService.GetList(x => x.DoiTuongGuiVanBanDiID == doiTuongID && x.dvID == dvID && x.NgayBanHanh >= tuNgay && x.NgayBanHanh <= denNgay).Count;
            //        else
            //            getValue = _ThongKeVBDiService.GetList(x => x.DoiTuongGuiVanBanDiID == doiTuongID && x.NgayBanHanh >= tuNgay && x.NgayBanHanh <= denNgay).Count;
            //        dicDoiTuongGuiVBDi.Add("itemDTGuiVBDi_" + i, getValue);
            //        tong += getValue;
            //    }
            //    dicTong.Add("DoiTuongGuiVBDi", tong);
            //}
            //viewModel.lstDoiTuongGuiVBDi = lstDoiTuongGuiVBDi;
            //viewModel.dicDoiTuongGuiVBDi = dicDoiTuongGuiVBDi;
            //Session["lstDoiTuongGuiVBDi"] = lstDoiTuongGuiVBDi;
            //Session["dicDoiTuongGuiVBDi"] = dicDoiTuongGuiVBDi;
            //var donVi = "";
            //var objDonVi = _dmDonViService.GetByFilter(x=>x.ID == dvID);
            //if (objDonVi != null)
            //    donVi = objDonVi.TenDonVi;
            //ViewBag.donVi = donVi;
            ////VBDen
            ////DoiTuongGuiVBDen
            //var dicDoiTuongGuiVBDen = new Dictionary<string, int>();
            //var lstDoiTuongGuiVBDen = new List<DoiTuongGuiVanBanDenViewModel>();
            //lstDoiTuongGuiVBDen = _doiTuongGuiVanBanDenService.GetList(x => x.IsDeleted == false && x.TrangThai == true);

            //if (lstDoiTuongGuiVBDen != null && lstDoiTuongGuiVBDen.Count > 0)
            //{
            //    var _lstLoaiVBDen = new List<DmLoaiVanBanViewModel>();
            //    _lstLoaiVBDen = _loaiVanBanDenService.GetList(x => x.IsDeleted == false && x.TrangThai == true);
            //    string _gm = "Giấy mời";
            //    int? idGM = null;
            //    for (int i = 0; i < _lstLoaiVBDen.Count; i++)
            //    {
            //        if (_lstLoaiVBDen[i].TenLoaiVanBan.ToLower().Contains(_gm.ToLower()))
            //        {
            //            idGM = _lstLoaiVBDen[i].ID;
            //        }
            //    }

            //    int getValue = 0;
            //    int doiTuongID = 0;
            //    int tong = 0;
            //    for (int i = 0; i < lstDoiTuongGuiVBDen.Count; i++)
            //    {
            //        doiTuongID = lstDoiTuongGuiVBDen[i].ID;
            //        if (dvID != 0)
            //            getValue = _ThongKeVBDenService.GetList(x => x.DoiTuongGuiVanBanDenID == doiTuongID && x.dvID == dvID && x.NgayVanBan >= tuNgay && x.NgayVanBan <= denNgay && x.LoaiVanBanID != idGM).Count;
            //        else
            //            getValue = _ThongKeVBDenService.GetList(x => x.DoiTuongGuiVanBanDenID == doiTuongID && x.NgayVanBan >= tuNgay && x.NgayVanBan <= denNgay && x.LoaiVanBanID != idGM).Count;
            //        dicDoiTuongGuiVBDen.Add("itemDTGuiVBDen_" + i, getValue);
            //        tong += getValue;
            //    }
            //    dicTong.Add("DoiTuongGuiVBDen", tong);
            //}
            //viewModel.lstDoiTuongGuiVBDen = lstDoiTuongGuiVBDen;
            //viewModel.dicDoiTuongGuiVBDen = dicDoiTuongGuiVBDen;
            //Session["lstDoiTuongGuiVBDen"] = lstDoiTuongGuiVBDen;
            //Session["dicDoiTuongGuiVBDen"] = dicDoiTuongGuiVBDen;

            ////LoaiVBDen
            //var dicLoaiVBDen = new Dictionary<string, int>();
            //var lstLoaiVBDen = new List<DmLoaiVanBanViewModel>();
            //lstLoaiVBDen = _loaiVanBanDenService.GetList(x => x.IsDeleted == false && x.TrangThai == true);

            //string gm = "Giấy mời";
            //for (int i = 0; i < lstLoaiVBDen.Count; i++)
            //{
            //    if (lstLoaiVBDen[i].TenLoaiVanBan.ToLower().Contains(gm.ToLower()))
            //    {
            //        lstLoaiVBDen.Remove(lstLoaiVBDen[i]);
            //    }
            //}

            //if (lstLoaiVBDen != null && lstLoaiVBDen.Count > 0)
            //{
            //    int getValue = 0;
            //    int loaiVBID = 0;
            //    int tong = 0;
            //    for (int i = 0; i < lstLoaiVBDen.Count; i++)
            //    {
            //        loaiVBID = lstLoaiVBDen[i].ID;
            //        if (dvID != 0)
            //            getValue = _ThongKeVBDenService.GetList(x => x.LoaiVanBanID == loaiVBID && x.dvID == dvID && x.NgayVanBan >= tuNgay && x.NgayVanBan <= denNgay).Count;
            //        else
            //            getValue = _ThongKeVBDenService.GetList(x => x.LoaiVanBanID == loaiVBID && x.NgayVanBan >= tuNgay && x.NgayVanBan <= denNgay).Count;
            //        dicLoaiVBDen.Add("itemLoaiVBDen_" + i, getValue);
            //        tong += getValue;
            //    }
            //    dicTong.Add("LoaiVBDen", tong);
            //}
            //viewModel.lstLoaiVBDen = lstLoaiVBDen;
            //viewModel.dicLoaiVBDen = dicLoaiVBDen;
            //Session["lstLoaiVBDen"] = lstLoaiVBDen;
            //Session["dicLoaiVBDen"] = dicLoaiVBDen;

            //viewModel.dicTong = dicTong;
            //Session["dicTong"] = dicTong;

            return PartialView(viewModel);
        }

        public void ExportExcelBaoCaoCongTacTiepNhanVaXuLyVanBan()
        {
            var typeUser = SessionInfo.CurrentUser.TypeUser;
            using (var package = new ExcelPackage())
            {
                string tuNgay = "";
                string denNgay = "";
                if (Session["DenNgay"] != null)
                    denNgay = Session["DenNgay"].ToString();
                if (Session["TuNgay"] != null)
                    tuNgay = Session["TuNgay"].ToString();
                string thang = "";
                if (!string.IsNullOrEmpty(denNgay))
                {
                    thang = denNgay.Replace(" ", "");
                    thang = denNgay.Substring(3, 7);
                }
                else
                    thang = DateTime.Now.Month + "/" + DateTime.Now.Year;

                var Title = "BÁO CÁO CÔNG TÁC TIẾP NHẬN VÀ XỬ LÝ CÔNG VĂN THÁNG " + thang;
                var nameFile = "BaoCaoCongTacTiepNhanVaXuLyCongVan";

                var lstDoiTuongGuiVBDi = new List<DoiTuongGuiVanBanDiViewModel>();
                var lstDoiTuongGuiVBDen = new List<DoiTuongGuiVanBanDenViewModel>();
                var lstLoaiVBDen = new List<DmLoaiVanBanViewModel>();

                var dicDoiTuongGuiVBDi = new Dictionary<string, int>();
                var dicDoiTuongGuiVBDen = new Dictionary<string, int>();
                var dicLoaiVBDen = new Dictionary<string, int>();
                var dicTong = new Dictionary<string, int>();

                lstDoiTuongGuiVBDi = (List<DoiTuongGuiVanBanDiViewModel>)Session["lstDoiTuongGuiVBDi"];
                lstDoiTuongGuiVBDen = (List<DoiTuongGuiVanBanDenViewModel>)Session["lstDoiTuongGuiVBDen"];
                lstLoaiVBDen = (List<DmLoaiVanBanViewModel>)Session["lstLoaiVBDen"];

                dicDoiTuongGuiVBDi = (Dictionary<string, int>)Session["dicDoiTuongGuiVBDi"];
                dicDoiTuongGuiVBDen = (Dictionary<string, int>)Session["dicDoiTuongGuiVBDen"];
                dicLoaiVBDen = (Dictionary<string, int>)Session["dicLoaiVBDen"];
                dicTong = (Dictionary<string, int>)Session["dicTong"];

                var ws = package.Workbook.Worksheets.Add("Sheet1");
                ws.Cells["A1:E1"].Merge = true;
                ws.Cells["A2:E2"].Merge = true;
                ws.Cells["A3:B3"].Merge = true;
                ws.Cells["B4:E4"].Merge = true;
                ws.Cells["B5:C5"].Merge = true;

                ws.Cells["A1:E1"].Style.Font.Bold = true;
                ws.Cells["A3:E3"].Style.Font.Bold = true;
                ws.Cells["B5:C5"].Style.Font.Bold = true;

                ws.Cells["A2"].Style.Font.Italic = true;
                ws.Cells["B5"].Style.Font.Italic = true;

                ws.Cells["A1"].Value = Title;
                ws.Cells["A2"].Value = "(Từ " + (!string.IsNullOrEmpty(tuNgay) ? tuNgay : "... / ... / ...") + " đến " + (!string.IsNullOrEmpty(denNgay) ? denNgay : DateTime.Now.ToString("dd/MM/yyyy") + ")");
                ws.Cells["A3"].Value = "1. Văn bản đi";

                //DoiTuongGuiVBDi
                if (lstDoiTuongGuiVBDi != null && lstDoiTuongGuiVBDi.Count > 0)
                {
                    ws.Cells["B4"].Value = "Từ " + (!string.IsNullOrEmpty(tuNgay) ? tuNgay : "... / ... / ...") + " đến " + (!string.IsNullOrEmpty(denNgay) ? denNgay : DateTime.Now.ToString("dd/MM/yyyy") + ", Cục ban hành " + dicTong["DoiTuongGuiVBDi"] + " văn bản, cụ thể: ");
                    ws.Cells["B5"].Value = "a) Văn bản";

                    int quyetDinh = 0;
                    string qd = "Quyết định";
                    string quyetDinhStr = "";
                    int row = 7;
                    for (int i = 0; i < lstDoiTuongGuiVBDi.Count; i++)
                    {
                        quyetDinhStr = lstDoiTuongGuiVBDi[i].TenDoiTuong.ToLower();
                        if (quyetDinhStr.Contains(qd.ToLower()))
                        {
                            quyetDinh += dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + i];
                        }
                        else
                        {
                            ws.Cells["C" + row].Value = "+/ " + lstDoiTuongGuiVBDi[i].TenDoiTuong;
                            ws.Cells["D" + row].Value = dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + i];
                            ws.Cells["D" + row].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            ws.Cells["E" + row].Value = "văn bản;";

                            ws.Cells["C" + row + ":E" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C" + row + ":E" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C" + row + ":E" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C" + row + ":E" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                            ws.Cells["C" + row + ":E" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["E" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["C" + row + ":E" + row].Style.WrapText = true;
                            row++;
                        }
                    }

                    ws.Cells["C" + row].Value = "Tổng cộng: ";
                    ws.Cells["D" + row].Value = dicTong["DoiTuongGuiVBDi"] - quyetDinh;
                    ws.Cells["D" + row].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    ws.Cells["C" + row + ":E" + row].Style.Font.Bold = true;
                    ws.Cells["E" + row].Value = "văn bản;";

                    ws.Cells["C" + row + ":E" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    ws.Cells["C" + row + ":E" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + row + ":E" + row].Style.WrapText = true;

                    row += 2;
                    ws.Cells["B" + row].Value = "b) Quyết định: ";
                    ws.Cells["B" + row].Style.Font.Bold = true;
                    ws.Cells["B" + row].Style.Font.Italic = true;
                    ws.Cells["C" + row].Value = quyetDinh + " quyết định";
                }

                //DoiTuongGuiVBDen
                int rowDoiTuongGuiVBDen = lstDoiTuongGuiVBDi.Count + 9;
                ws.Cells["A" + rowDoiTuongGuiVBDen].Value = "2. Văn bản đến";
                ws.Cells["A" + rowDoiTuongGuiVBDen].Style.Font.Bold = true;

                ws.Cells["C" + rowDoiTuongGuiVBDen].Value = "Cục tiếp nhận, xử lý " + dicTong["DoiTuongGuiVBDen"] + " văn bản.";

                if (lstDoiTuongGuiVBDen != null && lstDoiTuongGuiVBDen.Count > 0)
                {
                    ws.Cells["B" + (rowDoiTuongGuiVBDen + 1)].Value = "Trong đó:";
                    ws.Cells["B" + (rowDoiTuongGuiVBDen + 2)].Value = "a) Phân loại văn bản đến theo đơn vị:";
                    ws.Cells["B" + (rowDoiTuongGuiVBDen + 2)].Style.Font.Italic = true;

                    int row = lstDoiTuongGuiVBDi.Count + 13;
                    for (int i = 0; i < lstDoiTuongGuiVBDen.Count; i++)
                    {
                        ws.Cells["C" + row].Value = "+/ " + lstDoiTuongGuiVBDen[i].TenDoiTuong;
                        ws.Cells["D" + row].Value = dicDoiTuongGuiVBDen["itemDTGuiVBDen_" + i];
                        ws.Cells["D" + row].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        ws.Cells["E" + row].Value = "văn bản;";

                        ws.Cells["C" + row + ":E" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells["C" + row + ":E" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells["C" + row + ":E" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Cells["C" + row + ":E" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                        ws.Cells["C" + row + ":E" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells["D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells["E" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells["C" + row + ":E" + row].Style.WrapText = true;
                        row++;
                    }

                    ws.Cells["C" + row].Value = "Tổng cộng: ";
                    ws.Cells["D" + row].Value = dicTong["DoiTuongGuiVBDen"];
                    ws.Cells["D" + row].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    ws.Cells["C" + row + ":E" + row].Style.Font.Bold = true;
                    ws.Cells["E" + row].Value = "văn bản;";

                    ws.Cells["C" + row + ":E" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    ws.Cells["C" + row + ":E" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + row + ":E" + row].Style.WrapText = true;

                    row += 2;
                    ws.Cells["B" + row].Value = "b) Phân loại văn bản đến theo loại văn bản:";
                    ws.Cells["B" + row].Style.Font.Italic = true;
                }

                //LoaiVanBanDen
                if (lstLoaiVBDen != null && lstLoaiVBDen.Count > 0)
                {
                    int row = lstDoiTuongGuiVBDi.Count + lstDoiTuongGuiVBDen.Count + 17;
                    for (int i = 0; i < lstLoaiVBDen.Count; i++)
                    {
                        ws.Cells["C" + row].Value = "+/ " + lstLoaiVBDen[i].TenLoaiVanBan;
                        ws.Cells["D" + row].Value = dicLoaiVBDen["itemLoaiVBDen_" + i];
                        ws.Cells["D" + row].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        ws.Cells["E" + row].Value = "văn bản;";

                        ws.Cells["C" + row + ":E" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells["C" + row + ":E" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells["C" + row + ":E" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Cells["C" + row + ":E" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                        ws.Cells["C" + row + ":E" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells["D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells["E" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells["C" + row + ":E" + row].Style.WrapText = true;
                        row++;
                    }

                    ws.Cells["C" + row].Value = "Tổng cộng: ";
                    ws.Cells["D" + row].Value = dicTong["LoaiVBDen"];
                    ws.Cells["D" + row].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    ws.Cells["C" + row + ":E" + row].Style.Font.Bold = true;
                    ws.Cells["E" + row].Value = "văn bản;";

                    ws.Cells["C" + row + ":E" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + row + ":E" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    ws.Cells["C" + row + ":E" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + row + ":E" + row].Style.WrapText = true;
                }

                ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A:XJD"].Style.Font.Name = "Times New Roman";

                ws.Column(1).Width = 10;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 50;
                ws.Column(4).Width = 10;
                ws.Column(5).Width = 10;

                ////Phần header
                string timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + string.Format("{0}-{1}{2}{3}_{4}.xlsx", nameFile, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeStr));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.BinaryWrite(package.GetAsByteArray());
                System.Web.HttpContext.Current.Response.End();
            }
        }

        public ActionResult ExportWordBaoCaoCongTacTiepNhanVaXuLyVanBan()
        {
            //var typeUser = SessionInfo.CurrentUser.TypeUser;
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            var TempPath = Server.MapPath("~/Content/Temp/");
            var gDoc = DocX.Load(TempPath + @"\Temp_BaoCaoCongTacTiepNhanVaXuLyVanBan.docx");

            string tuNgay = "";
            string denNgay = "";
            if (Session["DenNgay"] != null)
                denNgay = Session["DenNgay"].ToString();
            else
                denNgay = DateTime.Now.ToString("dd/MM/yyyy");
            if (Session["TuNgay"] != null)
                tuNgay = Session["TuNgay"].ToString();
            else
                tuNgay = "... / ... / ...";

            string thang = "";
            if (!string.IsNullOrEmpty(denNgay))
            {
                thang = denNgay.Replace(" ", "");
                thang = denNgay.Substring(3, 7);
            }
            else
                thang = DateTime.Now.Month + "/" + DateTime.Now.Year;

            var lstDoiTuongGuiVBDi = new List<DoiTuongGuiVanBanDiViewModel>();
            var lstDoiTuongGuiVBDen = new List<DoiTuongGuiVanBanDenViewModel>();
            var lstLoaiVBDen = new List<DmLoaiVanBanViewModel>();

            var dicDoiTuongGuiVBDi = new Dictionary<string, int>();
            var dicDoiTuongGuiVBDen = new Dictionary<string, int>();
            var dicLoaiVBDen = new Dictionary<string, int>();
            var dicTong = new Dictionary<string, int>();

            lstDoiTuongGuiVBDi = (List<DoiTuongGuiVanBanDiViewModel>)Session["lstDoiTuongGuiVBDi"];
            lstDoiTuongGuiVBDen = (List<DoiTuongGuiVanBanDenViewModel>)Session["lstDoiTuongGuiVBDen"];
            lstLoaiVBDen = (List<DmLoaiVanBanViewModel>)Session["lstLoaiVBDen"];

            dicDoiTuongGuiVBDi = (Dictionary<string, int>)Session["dicDoiTuongGuiVBDi"];
            dicDoiTuongGuiVBDen = (Dictionary<string, int>)Session["dicDoiTuongGuiVBDen"];
            dicLoaiVBDen = (Dictionary<string, int>)Session["dicLoaiVBDen"];
            dicTong = (Dictionary<string, int>)Session["dicTong"];

            #region Thay text
            gDoc.ReplaceText("%TieuDe%", "TỔNG HỢP KẾT QUẢ THỰC HIỆN NHIỆM VỤ DO CHÍNH PHỦ, THỦ TƯỚNG CHÍNH PHỦ GIAO");
            gDoc.ReplaceText("%ThangTieuDe%", thang);
            gDoc.ReplaceText("%TuNgay%", tuNgay);
            gDoc.ReplaceText("%DenNgay%", denNgay);
            gDoc.ReplaceText("%TongVanBanDi%", dicTong["DoiTuongGuiVBDi"].ToString());
            gDoc.ReplaceText("%TongVBDen%", dicTong["DoiTuongGuiVBDen"].ToString());
            #endregion

            var tbl = gDoc.Tables[0];
            if (lstDoiTuongGuiVBDi != null && lstDoiTuongGuiVBDi.Count > 0)
            {
                int quyetDinh = 0;
                string quyetDinhStr = "";
                string qd = "Quyết định";
                tbl.Rows[0].Cells[0].Paragraphs.First().Append("+/ " + lstDoiTuongGuiVBDi[0].TenDoiTuong).Alignment = Alignment.left;
                tbl.Rows[0].Cells[1].Paragraphs.First().Append(dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + 0].ToString()).Alignment = Alignment.center;
                tbl.Rows[0].Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;

                for (int i = 0; i < lstDoiTuongGuiVBDi.Count; i++)
                {
                    quyetDinhStr = lstDoiTuongGuiVBDi[i].TenDoiTuong.ToLower();
                    if (quyetDinhStr.Contains(qd.ToLower()))
                    {
                        quyetDinh += dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + i];
                    }
                    else
                    {
                        if (i > 0)
                        {
                            var r = tbl.InsertRow();
                            r.Cells[0].Paragraphs.First().Append("+/ " + lstDoiTuongGuiVBDi[i].TenDoiTuong).Alignment = Alignment.left;
                            r.Cells[1].Paragraphs.First().Append(dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + i].ToString()).Alignment = Alignment.center;
                            r.Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;
                        }
                    }
                }
                var row = tbl.InsertRow();
                row.Cells[0].Paragraphs.First().Append("Tổng cộng: ").Alignment = Alignment.left;
                row.Cells[1].Paragraphs.First().Append((dicTong["DoiTuongGuiVBDi"] - quyetDinh).ToString()).Alignment = Alignment.center;
                row.Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;
                gDoc.ReplaceText("%QuyetDinh%", quyetDinh.ToString());
            }

            var tb2 = gDoc.Tables[1];
            if (lstDoiTuongGuiVBDen != null && lstDoiTuongGuiVBDen.Count > 0)
            {
                tb2.Rows[0].Cells[0].Paragraphs.First().Append("+/ " + lstDoiTuongGuiVBDen[0].TenDoiTuong).Alignment = Alignment.left;
                tb2.Rows[0].Cells[1].Paragraphs.First().Append(dicDoiTuongGuiVBDen["itemDTGuiVBDen_" + 0].ToString()).Alignment = Alignment.center;
                tb2.Rows[0].Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;

                for (int i = 0; i < lstDoiTuongGuiVBDen.Count; i++)
                {
                    if (i > 0)
                    {
                        var r = tb2.InsertRow();
                        r.Cells[0].Paragraphs.First().Append("+/ " + lstDoiTuongGuiVBDen[i].TenDoiTuong).Alignment = Alignment.left;
                        r.Cells[1].Paragraphs.First().Append(dicDoiTuongGuiVBDen["itemDTGuiVBDen_" + i].ToString()).Alignment = Alignment.center;
                        r.Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;
                    }
                }
                var row = tb2.InsertRow();
                row.Cells[0].Paragraphs.First().Append("Tổng cộng: ").Alignment = Alignment.left;
                row.Cells[1].Paragraphs.First().Append(dicTong["DoiTuongGuiVBDen"].ToString()).Alignment = Alignment.center;
                row.Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;
            }

            var tb3 = gDoc.Tables[2];
            if (lstLoaiVBDen != null && lstLoaiVBDen.Count > 0)
            {
                tb3.Rows[0].Cells[0].Paragraphs.First().Append("+/ " + lstLoaiVBDen[0].TenLoaiVanBan).Alignment = Alignment.left;
                tb3.Rows[0].Cells[1].Paragraphs.First().Append(dicLoaiVBDen["itemLoaiVBDen_" + 0].ToString()).Alignment = Alignment.center;
                tb3.Rows[0].Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;

                for (int i = 0; i < lstLoaiVBDen.Count; i++)
                {
                    if (i > 0)
                    {
                        var r = tb3.InsertRow();
                        r.Cells[0].Paragraphs.First().Append("+/ " + lstLoaiVBDen[i].TenLoaiVanBan).Alignment = Alignment.left;
                        r.Cells[1].Paragraphs.First().Append(dicLoaiVBDen["itemLoaiVBDen_" + i].ToString()).Alignment = Alignment.center;
                        r.Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;
                    }
                }
                var row = tb3.InsertRow();
                row.Cells[0].Paragraphs.First().Append("Tổng cộng: ").Alignment = Alignment.left;
                row.Cells[1].Paragraphs.First().Append(dicTong["LoaiVBDen"].ToString()).Alignment = Alignment.center;
                row.Cells[2].Paragraphs.First().Append("văn bản;").Alignment = Alignment.center;
            }

            var path = "~/Content/Export";
            var fileName = "Word-BaoCaoCongTacTiepNhanVaXuLyVanBan-" + timeStr + ".docx";
            var pathSrv = Server.MapPath(path);
            if (!Directory.Exists(pathSrv))
            {
                Directory.CreateDirectory(pathSrv);
            }
            var outfile = TKM.Utils.CommonUtils.GetPath(path, fileName);
            gDoc.SaveAs(outfile);
            gDoc.Dispose();
            return File(outfile, "application/docx", fileName);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportPDFBaoCaoCongTacTiepNhanVaXuLyVanBan(FormCollection collection)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            string htmlString = collection["GridHtml"];
            string tenFile = collection["tenFilePDF"];
            string widthtable = collection["widthTable"];
            string baseUrl = "";

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Landscape";
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdf_orientation, true);

            int webPageWidth = Convert.ToInt32(widthtable);
            int webPageHeight = 0;

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.DisplayHeader = false;
            converter.Options.DisplayFooter = false;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            byte[] pdf = doc.Save();
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "PDF-" + tenFile + "-" + timeStr + ".pdf";
            return fileResult;
        }
        #endregion

        #region 1. Thong ke van ban di/den theo nam
        public ActionResult ThongKeVanBanDiDen(int Nam = 0, int VBDi = 0)
        {
            if (Nam == 0) Nam = DateTime.Now.Year;
            ViewBag.Nam = Nam;
            ViewBag.VBDi = VBDi;
            return View();
        }
        public ActionResult ReloadThongKeVanBanDiDen(int Nam = 0, int? VBDi = null)
        {
            int dvID = 0;
            if (Nam == 0) Nam = DateTime.Now.Year;
            Session["Nam"] = Nam;

            ViewBag.Nam = Nam;
            var getValueMonth = 0;

            try
            {
                string tenDN = SessionInfo.CurrentUser.TenDangNhap;
                string admin = "admin";
                if (tenDN.ToLower().Equals(admin.ToLower()))
                {
                    dvID = 0;
                }
                else
                {
                    dvID = SessionInfo.CurrentUser.DonViID;
                }
            }
            catch { }

            //văn bản đi
            var dicDoiTuongGuiVBDi = new Dictionary<string, List<int>>();
            var lstDoiTuongGuiVBDi = new List<DoiTuongGuiVanBanDiViewModel>();

            if (VBDi != null && VBDi != 0)
            {
                lstDoiTuongGuiVBDi = _doiTuongGuiVanBanDiService.GetList(x => x.IsDeleted == false && x.TrangThai == true && x.LoaiGuiVanBan == VBDi);
            }
            else
            {
                lstDoiTuongGuiVBDi = _doiTuongGuiVanBanDiService.GetList(x => x.IsDeleted == false && x.TrangThai == true).OrderByDescending(x => x.LoaiGuiVanBan).ToList();
            }

            ViewBag.lstDoiTuongGuiVBDi = lstDoiTuongGuiVBDi;
            ViewBag.countDTGVBDi = lstDoiTuongGuiVBDi.Count();
            Session["lstDoiTuongGuiVBDi"] = lstDoiTuongGuiVBDi;

            var dicTong = new Dictionary<string, List<int>>();
            if (lstDoiTuongGuiVBDi != null && lstDoiTuongGuiVBDi.Count > 0)
            {
                var lstValue = new List<int>();
                var lstTongDoiTuongGuiVBDi = new List<int>();

                getValueMonth = 0;
                for (int i = 0; i < lstDoiTuongGuiVBDi.Count; i++)
                {
                    int id = lstDoiTuongGuiVBDi[i].ID;
                    lstValue = new List<int>();
                    for (int j = 1; j <= 12; j++)
                    {
                        getValueMonth = 0;
                        //ngày bắt đầu của tháng
                        var firstDayOfMonth = new DateTime(Nam, j, 1);
                        //ngày bắt đầu của tháng
                        var lastDayOfMonth = new DateTime(Nam, j, DateTime.DaysInMonth(Nam, j));

                        if (dvID != 0)
                            getValueMonth = _ThongKeVBDiService.GetList(x => x.DoiTuongGuiVanBanDiID == id && x.dvID == dvID && x.NgayBanHanh.Value.Month == j && x.NgayBanHanh.Value.Year == Nam).Count();
                        else
                            getValueMonth = _ThongKeVBDiService.GetList(x => x.DoiTuongGuiVanBanDiID == id && x.NgayBanHanh.Value.Month == j && x.NgayBanHanh.Value.Year == Nam).Count();
                        lstValue.Add(getValueMonth);
                    }

                    for (int j = 1; j <= 12; j += 1)//h
                    {
                        getValueMonth = 0;
                        //ngày bắt đầu của tháng
                        var firstDayOfMonth = new DateTime(Nam, j, 1);
                        //ngày bắt đầu của tháng
                        var lastDayOfMonth = new DateTime(Nam, j + 2, DateTime.DaysInMonth(Nam, j + 2));
                        if (dvID != 0)
                            getValueMonth = _ThongKeVBDiService.GetList(x => x.DoiTuongGuiVanBanDiID == id && x.dvID == dvID && (DateTime.Compare(x.NgayBanHanh.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayBanHanh.Value, lastDayOfMonth) <= 0)).Count;
                        else
                            getValueMonth = _ThongKeVBDiService.GetList(x => x.DoiTuongGuiVanBanDiID == id && (DateTime.Compare(x.NgayBanHanh.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayBanHanh.Value, lastDayOfMonth) <= 0)).Count;
                        lstValue.Add(getValueMonth);
                        j += 2;
                    }
                    dicDoiTuongGuiVBDi.Add("itemDTGuiVBDi_" + i, lstValue);
                }
                int tong = 0;
                var lstCount = new List<int>();
                var lst = new List<int>();
                lstCount = dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + 1];
                for (int m = 0; m < lstCount.Count; m++)
                {
                    tong = 0;
                    for (int k = 0; k < dicDoiTuongGuiVBDi.Count; k++)
                    {
                        lst = dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + k];
                        tong += lst[m];
                    }
                    lstTongDoiTuongGuiVBDi.Add(tong);
                }
                dicTong.Add("TongDoiTuongGuiVBDi", lstTongDoiTuongGuiVBDi);
                //for (int j = 0; j < lstValue.Count; j++)
                //{
                //    tongDoiTuongGuiVBDi = 0;
                //    for (int m = 0; m < lstDoiTuongGuiVBDi.Count; m++)
                //    {
                //        tongDoiTuongGuiVBDi += lstValue[m];
                //    }
                //    lstTongDoiTuongGuiVBDi.Add(tongDoiTuongGuiVBDi);
                //}
                //dicTong.Add("TongDoiTuongGuiVBDi", lstTongDoiTuongGuiVBDi);
            }
            ViewBag.dicDoiTuongGuiVBDi = dicDoiTuongGuiVBDi;
            Session["dicDoiTuongGuiVBDi"] = dicDoiTuongGuiVBDi;

            //Văn bản đến và đối tượng gửi văn bản đến
            var dicDoiTuongGuiVBDen = new Dictionary<string, List<int>>();
            var lstDoiTuongGuiVBDen = _doiTuongGuiVanBanDenService.GetList(x => x.IsDeleted == false && x.TrangThai == true);
            ViewBag.lstDoiTuongGuiVBDen = lstDoiTuongGuiVBDen;
            ViewBag.countDTGVBDen = lstDoiTuongGuiVBDen.Count();
            Session["lstDoiTuongGuiVBDen"] = lstDoiTuongGuiVBDen;


            if (lstDoiTuongGuiVBDen != null && lstDoiTuongGuiVBDen.Count > 0)
            {
                var lstValue = new List<int>();
                var lstTongDoiTuongGuiVBDen = new List<int>();
                getValueMonth = 0;

                var _lstLoaiVBDen = _loaiVanBanDenService.GetList(x => x.IsDeleted == false && x.TrangThai == true);
                string _gm = "Giấy mời";
                int? idGM = null;
                for (int i = 0; i < _lstLoaiVBDen.Count; i++)
                {
                    if (_lstLoaiVBDen[i].TenLoaiVanBan.ToLower().Contains(_gm.ToLower()))
                    {
                        idGM = _lstLoaiVBDen[i].ID;
                    }
                }

                for (int i = 0; i < lstDoiTuongGuiVBDen.Count; i++)
                {
                    int id = lstDoiTuongGuiVBDen[i].ID;
                    lstValue = new List<int>();
                    for (int j = 1; j <= 12; j++)
                    {
                        getValueMonth = 0;
                        //ngày bắt đầu của tháng
                        var firstDayOfMonth = new DateTime(Nam, j, 1);
                        //ngày bắt đầu của tháng
                        var lastDayOfMonth = new DateTime(Nam, j, DateTime.DaysInMonth(Nam, j));
                        if (dvID != 0)
                            getValueMonth = _ThongKeVBDenService.GetList(x => x.DoiTuongGuiVanBanDenID == id && x.dvID == dvID && x.LoaiVanBanID != idGM && (DateTime.Compare(x.NgayVanBan.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayVanBan.Value, lastDayOfMonth) <= 0)).Count;
                        else
                            getValueMonth = _ThongKeVBDenService.GetList(x => x.DoiTuongGuiVanBanDenID == id && x.LoaiVanBanID != idGM && (DateTime.Compare(x.NgayVanBan.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayVanBan.Value, lastDayOfMonth) <= 0)).Count;
                        lstValue.Add(getValueMonth);
                    }

                    for (int j = 1; j <= 12; j += 1)//h
                    {
                        getValueMonth = 0;
                        //ngày bắt đầu của tháng
                        var firstDayOfMonth = new DateTime(Nam, j, 1);
                        //ngày bắt đầu của tháng
                        var lastDayOfMonth = new DateTime(Nam, j + 2, DateTime.DaysInMonth(Nam, j + 2));
                        getValueMonth = _ThongKeVBDenService.GetList(x => x.DoiTuongGuiVanBanDenID == id && x.dvID == dvID && x.LoaiVanBanID != idGM && (DateTime.Compare(x.NgayVanBan.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayVanBan.Value, lastDayOfMonth) <= 0)).Count;
                        lstValue.Add(getValueMonth);
                        j += 2;
                    }
                    dicDoiTuongGuiVBDen.Add("itemDTGuiVBDen_" + i, lstValue);
                }

                int tong = 0;
                var lstCount = new List<int>();
                var lst = new List<int>();
                lstCount = dicDoiTuongGuiVBDen["itemDTGuiVBDen_" + 1];
                for (int m = 0; m < lstCount.Count; m++)
                {
                    tong = 0;
                    for (int k = 0; k < dicDoiTuongGuiVBDen.Count; k++)
                    {
                        lst = dicDoiTuongGuiVBDen["itemDTGuiVBDen_" + k];
                        tong += lst[m];
                    }
                    lstTongDoiTuongGuiVBDen.Add(tong);
                }
                dicTong.Add("TongDoiTuongGuiVBDen", lstTongDoiTuongGuiVBDen);
            }
            ViewBag.dicDoiTuongGuiVBDen = dicDoiTuongGuiVBDen;
            Session["dicDoiTuongGuiVBDen"] = dicDoiTuongGuiVBDen;
            //ViewBag.dicTong = dicTong;

            //Văn bản đến và loại văn ban
            var lstLoaiVBDen = _loaiVanBanDenService.GetList(x => x.IsDeleted == false && x.TrangThai == true);
            string gm = "Giấy mời";
            for (int i = 0; i < lstLoaiVBDen.Count; i++)
            {
                if (lstLoaiVBDen[i].TenLoaiVanBan.ToLower().Contains(gm.ToLower()))
                {
                    lstLoaiVBDen.Remove(lstLoaiVBDen[i]);
                }
            }
            ViewBag.lstLoaiVBDen = lstLoaiVBDen;
            ViewBag.countLoaiVBDen = lstLoaiVBDen.Count();
            Session["lstLoaiVBDen"] = lstLoaiVBDen;

            var dicLoaiVBDen = new Dictionary<string, List<int>>();
            if (lstLoaiVBDen != null && lstLoaiVBDen.Count > 0)
            {
                var lstValue = new List<int>();
                var lstTongLoaiVBDen = new List<int>();
                getValueMonth = 0;
                for (int i = 0; i < lstLoaiVBDen.Count; i++)
                {
                    int id = lstLoaiVBDen[i].ID;
                    lstValue = new List<int>();
                    for (int j = 1; j <= 12; j++)
                    {
                        getValueMonth = 0;
                        //ngày bắt đầu của tháng
                        var firstDayOfMonth = new DateTime(Nam, j, 1);
                        //ngày bắt đầu của tháng
                        var lastDayOfMonth = new DateTime(Nam, j, DateTime.DaysInMonth(Nam, j));
                        if (dvID != 0)
                            getValueMonth = _ThongKeVBDenService.GetList(x => x.LoaiVanBanID == id && x.dvID == dvID && (DateTime.Compare(x.NgayVanBan.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayVanBan.Value, lastDayOfMonth) <= 0)).Count;
                        else
                            getValueMonth = _ThongKeVBDenService.GetList(x => x.LoaiVanBanID == id && (DateTime.Compare(x.NgayVanBan.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayVanBan.Value, lastDayOfMonth) <= 0)).Count;
                        lstValue.Add(getValueMonth);
                    }

                    for (int j = 1; j <= 12; j += 1)//h
                    {
                        getValueMonth = 0;
                        //ngày bắt đầu của tháng
                        var firstDayOfMonth = new DateTime(Nam, j, 1);
                        //ngày bắt đầu của tháng
                        var lastDayOfMonth = new DateTime(Nam, j + 2, DateTime.DaysInMonth(Nam, j + 2));
                        if (dvID != 0)
                            getValueMonth = _ThongKeVBDenService.GetList(x => x.LoaiVanBanID == id && x.dvID == dvID && (DateTime.Compare(x.NgayVanBan.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayVanBan.Value, lastDayOfMonth) <= 0)).Count;
                        else
                            getValueMonth = _ThongKeVBDenService.GetList(x => x.LoaiVanBanID == id && (DateTime.Compare(x.NgayVanBan.Value, firstDayOfMonth) >= 0) && (DateTime.Compare(x.NgayVanBan.Value, lastDayOfMonth) <= 0)).Count;
                        lstValue.Add(getValueMonth);
                        j += 2;
                    }
                    dicLoaiVBDen.Add("itemLoaiVBDen_" + i, lstValue);
                }
                int tong = 0;
                var lstCount = new List<int>();
                var lst = new List<int>();
                lstCount = dicLoaiVBDen["itemLoaiVBDen_" + 1];
                for (int m = 0; m < lstCount.Count; m++)
                {
                    tong = 0;
                    for (int k = 0; k < dicLoaiVBDen.Count; k++)
                    {
                        lst = dicLoaiVBDen["itemLoaiVBDen_" + k];
                        tong += lst[m];
                    }
                    lstTongLoaiVBDen.Add(tong);
                }
                dicTong.Add("LoaiVBDen", lstTongLoaiVBDen);
            }

            ViewBag.dicLoaiVBDen = dicLoaiVBDen;
            Session["dicLoaiVBDen"] = dicLoaiVBDen;

            ViewBag.dicTong = dicTong;
            Session["dicTong"] = dicTong;

            var viewModel = new VanBanDiListView();
            return View(viewModel);
        }
        public void ExportExcelThongKeVanBanDiDen()
        {
            var typeUser = SessionInfo.CurrentUser.TypeUser;
            using (var package = new ExcelPackage())
            {
                var Title = "THỐNG KÊ VĂN BẢN ĐẾN/ĐI THEO NĂM " + Session["Nam"] != null ? Session["Nam"] : DateTime.Now.Year;
                var nameFile = "ThongKeVanBanDenDi";
                //Van ban di
                var lstDoiTuongGuiVBDi = (List<DoiTuongGuiVanBanDiViewModel>)Session["lstDoiTuongGuiVBDi"];
                var dicDoiTuongGuiVBDi = (Dictionary<string, List<int>>)Session["dicDoiTuongGuiVBDi"];
                var dicTong = (Dictionary<string, List<int>>)Session["dicTong"];

                //Doi tuong gui van ban den
                var lstDoiTuongGuiVBDen = (List<DoiTuongGuiVanBanDenViewModel>)Session["lstDoiTuongGuiVBDen"];
                var dicDoiTuongGuiVBDen = (Dictionary<string, List<int>>)Session["dicDoiTuongGuiVBDen"];

                //Loai van ban den
                var lstLoaiVBDen = (List<DmLoaiVanBanViewModel>)Session["lstLoaiVBDen"];
                var dicLoaiVBDen = (Dictionary<string, List<int>>)Session["dicLoaiVBDen"];

                var ws = package.Workbook.Worksheets.Add("Sheet1");
                // Tạo header
                ws.Cells["A1:S1"].Merge = true;
                ws.Cells["A" + (lstDoiTuongGuiVBDi.Count + 4) + ":A" + (lstDoiTuongGuiVBDi.Count + 4 + lstDoiTuongGuiVBDen.Count)].Merge = true;
                ws.Cells["A" + (lstDoiTuongGuiVBDi.Count + 5 + lstDoiTuongGuiVBDen.Count) + ":A" + (lstDoiTuongGuiVBDi.Count + 5 + lstDoiTuongGuiVBDen.Count + lstLoaiVBDen.Count)].Merge = true;
                ws.Cells["A3:A" + (lstDoiTuongGuiVBDi.Count + 3)].Merge = true;
                ws.Cells["A1"].Value = Title;
                ws.Cells["A2"].Value = "Văn bản";
                ws.Cells["B2"].Value = "Phân loại văn bản";
                ws.Cells["C2"].Value = "T1";
                ws.Cells["D2"].Value = "T2";
                ws.Cells["E2"].Value = "T3";
                ws.Cells["F2"].Value = "T4";
                ws.Cells["G2"].Value = "T5";
                ws.Cells["H2"].Value = "T6";
                ws.Cells["I2"].Value = "T7";
                ws.Cells["J2"].Value = "T8";
                ws.Cells["K2"].Value = "T9";
                ws.Cells["L2"].Value = "T10";
                ws.Cells["M2"].Value = "T11";
                ws.Cells["N2"].Value = "T12";
                ws.Cells["O2"].Value = "Q1";
                ws.Cells["P2"].Value = "Q2";
                ws.Cells["Q2"].Value = "Q3";
                ws.Cells["R2"].Value = "Q4";
                ws.Cells["S2"].Value = "TC";
                ws.Cells["A3"].Value = "Văn bản đi";
                ws.Cells["A" + (lstDoiTuongGuiVBDi.Count + 4)].Value = "Văn bản đến";
                ws.Cells["A" + (lstDoiTuongGuiVBDi.Count + 5 + lstDoiTuongGuiVBDen.Count)].Value = "Văn bản đến";
                ws.Cells["B" + (lstDoiTuongGuiVBDi.Count + 3)].Value = "Tổng cộng";
                ws.Cells["B" + (lstDoiTuongGuiVBDi.Count + 4 + lstDoiTuongGuiVBDen.Count)].Value = "Tổng cộng";
                ws.Cells["B" + (lstDoiTuongGuiVBDi.Count + 5 + lstDoiTuongGuiVBDen.Count + lstLoaiVBDen.Count)].Value = "Tổng cộng";

                ws.Cells["A2:S2"].Style.Font.Bold = true;
                ws.Cells["A2:S2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A2:S2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A2:S2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A2:S2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A2:S2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["A2:S2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                int tongDoiTuongGuiVBDi = lstDoiTuongGuiVBDi.Count + 3;
                ws.Cells["B" + (tongDoiTuongGuiVBDi) + ":S" + (tongDoiTuongGuiVBDi)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongDoiTuongGuiVBDi) + ":S" + (tongDoiTuongGuiVBDi)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongDoiTuongGuiVBDi) + ":S" + (tongDoiTuongGuiVBDi)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongDoiTuongGuiVBDi) + ":S" + (tongDoiTuongGuiVBDi)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                int tongDoiTuongGuiVBDen = lstDoiTuongGuiVBDi.Count + 4 + lstDoiTuongGuiVBDen.Count;
                ws.Cells["B" + (tongDoiTuongGuiVBDen) + ":S" + (tongDoiTuongGuiVBDen)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongDoiTuongGuiVBDen) + ":S" + (tongDoiTuongGuiVBDen)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongDoiTuongGuiVBDen) + ":S" + (tongDoiTuongGuiVBDen)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongDoiTuongGuiVBDen) + ":S" + (tongDoiTuongGuiVBDen)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                int tongLoaiVBDen = lstDoiTuongGuiVBDi.Count + 5 + lstDoiTuongGuiVBDen.Count + lstLoaiVBDen.Count;
                ws.Cells["B" + (tongLoaiVBDen) + ":S" + (tongLoaiVBDen)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongLoaiVBDen) + ":S" + (tongLoaiVBDen)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongLoaiVBDen) + ":S" + (tongLoaiVBDen)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["B" + (tongLoaiVBDen) + ":S" + (tongLoaiVBDen)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                string[] a = { "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S" };

                //Doi tuong gui van ban di
                if (lstDoiTuongGuiVBDi != null)
                {
                    for (int i = 0; i < lstDoiTuongGuiVBDi.Count; i++)
                    {
                        var lstMonth = new List<int>();
                        lstMonth = dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + i];
                        ws.Cells["B" + (i + 3)].Value = lstDoiTuongGuiVBDi[i].TenDoiTuong;
                        for (int j = 0; j < lstMonth.Count; j++)
                        {
                            ws.Cells[a[j] + (i + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[a[j] + (i + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[a[j] + (i + 3)].Value = lstMonth[j];
                        }
                        ws.Cells["A" + (i + 3) + ":S" + (i + 3)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        ws.Cells["A" + (i + 3) + ":S" + (i + 3)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + (i + 3) + ":S" + (i + 3)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + (i + 3) + ":S" + (i + 3)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + (i + 3) + ":S" + (i + 3)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }
                    var lstTong = new List<int>();
                    lstTong = dicTong["TongDoiTuongGuiVBDi"];
                    //Tong
                    int tong = 0;
                    for (int i = 0; i < a.Length - 1; i++)
                    {
                        ws.Cells[a[i] + (lstDoiTuongGuiVBDi.Count + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[a[i] + (lstDoiTuongGuiVBDi.Count + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[a[i] + (lstDoiTuongGuiVBDi.Count + 3)].Value = lstTong[i];
                        ws.Cells[a[i] + (lstDoiTuongGuiVBDi.Count + 3)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        tong += lstTong[i];
                    }
                    ws.Cells[a[a.Length - 1] + (lstDoiTuongGuiVBDi.Count + 3)].Value = tong;
                }

                //Doi tuong gui van ban den
                if (lstDoiTuongGuiVBDen != null)
                {
                    int row = 0;
                    int rowTong = 0;
                    for (int i = 0; i < lstDoiTuongGuiVBDen.Count; i++)
                    {
                        var lstMonth = new List<int>();
                        lstMonth = dicDoiTuongGuiVBDen["itemDTGuiVBDen_" + i];
                        ws.Cells["B" + (i + 4 + lstDoiTuongGuiVBDi.Count)].Value = lstDoiTuongGuiVBDen[i].TenDoiTuong;

                        for (int j = 0; j < lstMonth.Count; j++)
                        {
                            row = i + 4 + lstDoiTuongGuiVBDi.Count;
                            ws.Cells[a[j] + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[a[j] + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[a[j] + row].Value = lstMonth[j];
                        }
                        ws.Cells["A" + row + ":S" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells["A" + row + ":S" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + row + ":S" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + row + ":S" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + row + ":S" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }

                    var lstTong = new List<int>();
                    lstTong = dicTong["TongDoiTuongGuiVBDen"];
                    //Tong
                    int tong = 0;
                    for (int i = 0; i < a.Length - 1; i++)
                    {
                        rowTong = lstDoiTuongGuiVBDi.Count + 4 + lstDoiTuongGuiVBDen.Count;
                        ws.Cells[a[i] + rowTong].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[a[i] + rowTong].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[a[i] + rowTong].Value = lstTong[i];
                        ws.Cells[a[i] + rowTong].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        tong += lstTong[i];
                    }
                    ws.Cells[a[a.Length - 1] + rowTong].Value = tong;
                }

                //Loai van ban den
                if (lstLoaiVBDen != null)
                {
                    int row = 0;
                    int rowTong = 0;
                    for (int i = 0; i < lstLoaiVBDen.Count; i++)
                    {
                        var lstMonth = new List<int>();
                        lstMonth = dicLoaiVBDen["itemLoaiVBDen_" + i];
                        ws.Cells["B" + (i + 5 + lstDoiTuongGuiVBDi.Count + lstDoiTuongGuiVBDen.Count)].Value = lstLoaiVBDen[i].TenLoaiVanBan;

                        for (int j = 0; j < lstMonth.Count; j++)
                        {
                            row = i + 5 + lstDoiTuongGuiVBDi.Count + lstDoiTuongGuiVBDen.Count;
                            ws.Cells[a[j] + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[a[j] + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[a[j] + row].Value = lstMonth[j];
                        }
                        ws.Cells["A" + row + ":S" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        ws.Cells["A" + row + ":S" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + row + ":S" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + row + ":S" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + row + ":S" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }
                    var lstTong = new List<int>();
                    lstTong = dicTong["LoaiVBDen"];
                    //Tong
                    int tong = 0;
                    for (int i = 0; i < a.Length - 1; i++)
                    {
                        rowTong = lstDoiTuongGuiVBDi.Count + 5 + lstDoiTuongGuiVBDen.Count + lstLoaiVBDen.Count;
                        ws.Cells[a[i] + rowTong].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[a[i] + rowTong].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[a[i] + rowTong].Value = lstTong[i];
                        ws.Cells[a[i] + rowTong].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        tong += lstTong[i];
                    }
                    ws.Cells[a[a.Length - 1] + rowTong].Value = tong;
                }

                ws.Cells["A:XJD"].Style.Font.Name = "Times New Roman";
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 50;

                //Phần header
                string timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + string.Format("{0}-{1}{2}{3}_{4}.xlsx", nameFile, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeStr));
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.BinaryWrite(package.GetAsByteArray());
                System.Web.HttpContext.Current.Response.End();
            }
        }

        public ActionResult ExportWordThongKeVanBanDiDen()
        {
            //var typeUser = SessionInfo.CurrentUser.TypeUser;
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            var TempPath = Server.MapPath("~/Content/Temp/");
            var gDoc = DocX.Load(TempPath + @"\Temp_ThongKeVanBanDenDi.docx");
            #region Thay text
            string Nam = Session["Nam"].ToString();
            gDoc.ReplaceText("%TieuDe%", "BÁO CÁO THỐNG KÊ VĂN BẢN ĐẾN/ĐI NĂM " + (Nam != null ? Nam : DateTime.Now.Year.ToString()));
            #endregion
            var lstDoiTuongGuiVBDi = (List<DoiTuongGuiVanBanDiViewModel>)Session["lstDoiTuongGuiVBDi"];
            var dicDoiTuongGuiVBDi = (Dictionary<string, List<int>>)Session["dicDoiTuongGuiVBDi"];
            var lstDoiTuongGuiVBDen = (List<DoiTuongGuiVanBanDenViewModel>)Session["lstDoiTuongGuiVBDen"];
            var dicDoiTuongGuiVBDen = (Dictionary<string, List<int>>)Session["dicDoiTuongGuiVBDen"];
            var lstLoaiVBDen = (List<DmLoaiVanBanViewModel>)Session["lstLoaiVBDen"];
            var dicLoaiVBDen = (Dictionary<string, List<int>>)Session["dicLoaiVBDen"];

            var dicTong = (Dictionary<string, List<int>>)Session["dicTong"];

            //Table 0 : là tiêu đề
            //Table 1 : là bảng
            var tbl = gDoc.Tables[0];
            //var index = 0;

            //Doi tuong gui van ban di
            if (lstDoiTuongGuiVBDi != null && lstDoiTuongGuiVBDi.Count > 0)
            {
                for (int i = 0; i < lstDoiTuongGuiVBDi.Count; i++)
                {
                    var r = tbl.InsertRow();
                    var lstMonth = new List<int>();
                    lstMonth = dicDoiTuongGuiVBDi["itemDTGuiVBDi_" + i];
                    r.Cells[1].Paragraphs.First().Append(lstDoiTuongGuiVBDi[i].TenDoiTuong);

                    for (int j = 0; j < lstMonth.Count; j++)
                    {
                        r.Cells[j + 2].Paragraphs.First().Append(lstMonth[j].ToString()).Alignment = Alignment.center;
                    }
                }
                tbl.InsertRow();
                tbl.MergeCellsInColumn(0, 1, lstDoiTuongGuiVBDi.Count + 1);
                tbl.Rows[1].Cells[0].Paragraphs.First().Append("Văn bản đi").Alignment = Alignment.center;
                tbl.Rows[lstDoiTuongGuiVBDi.Count + 1].Cells[1].Paragraphs.First().Append("Tổng cộng");
                var lstTong = new List<int>();
                lstTong = dicTong["TongDoiTuongGuiVBDi"];
                int tong = 0;
                for (int i = 0; i < lstTong.Count; i++)
                {
                    tbl.Rows[lstDoiTuongGuiVBDi.Count + 1].Cells[i + 2].Paragraphs.First().Append(lstTong[i].ToString()).Alignment = Alignment.center;
                }
                for (int i = 0; i < 12; i++)
                {
                    tong += lstTong[i];
                }
                tbl.Rows[lstDoiTuongGuiVBDi.Count + 1].Cells[lstTong.Count + 2].Paragraphs.First().Append(tong.ToString()).Alignment = Alignment.center;
            }

            //Doi tuong gui van ban den
            if (lstDoiTuongGuiVBDen != null && lstDoiTuongGuiVBDen.Count > 0)
            {
                for (int i = 0; i < lstDoiTuongGuiVBDen.Count; i++)
                {
                    var r = tbl.InsertRow();
                    var lstMonth = new List<int>();
                    lstMonth = dicDoiTuongGuiVBDen["itemDTGuiVBDen_" + i];
                    r.Cells[1].Paragraphs.First().Append(lstDoiTuongGuiVBDen[i].TenDoiTuong);

                    for (int j = 0; j < lstMonth.Count; j++)
                    {
                        r.Cells[j + 2].Paragraphs.First().Append(lstMonth[j].ToString()).Alignment = Alignment.center;
                    }
                }
                tbl.InsertRow();
                tbl.MergeCellsInColumn(0, lstDoiTuongGuiVBDi.Count + 2, lstDoiTuongGuiVBDi.Count + 2 + lstDoiTuongGuiVBDen.Count);
                tbl.Rows[lstDoiTuongGuiVBDi.Count + 2].Cells[0].Paragraphs.First().Append("Văn bản đến").Alignment = Alignment.center;
                tbl.Rows[lstDoiTuongGuiVBDi.Count + 2 + lstDoiTuongGuiVBDen.Count].Cells[1].Paragraphs.First().Append("Tổng cộng");
                var lstTong = new List<int>();
                lstTong = dicTong["TongDoiTuongGuiVBDen"];
                int tong = 0;
                for (int i = 0; i < lstTong.Count; i++)
                {
                    tbl.Rows[lstDoiTuongGuiVBDi.Count + 2 + lstDoiTuongGuiVBDen.Count].Cells[i + 2].Paragraphs.First().Append(lstTong[i].ToString()).Alignment = Alignment.center;
                }
                for (int i = 0; i < 12; i++)
                {
                    tong += lstTong[i];
                }
                tbl.Rows[lstDoiTuongGuiVBDi.Count + 2 + lstDoiTuongGuiVBDen.Count].Cells[lstTong.Count + 2].Paragraphs.First().Append(tong.ToString()).Alignment = Alignment.center;
            }

            //Loai van ban den
            if (lstLoaiVBDen != null && lstLoaiVBDen.Count > 0)
            {
                for (int i = 0; i < lstLoaiVBDen.Count; i++)
                {
                    var r = tbl.InsertRow();
                    var lstMonth = new List<int>();
                    lstMonth = dicLoaiVBDen["itemLoaiVBDen_" + i];
                    r.Cells[1].Paragraphs.First().Append(lstLoaiVBDen[i].TenLoaiVanBan);

                    for (int j = 0; j < lstMonth.Count; j++)
                    {
                        r.Cells[j + 2].Paragraphs.First().Append(lstMonth[j].ToString()).Alignment = Alignment.center;
                    }
                }
                tbl.InsertRow();
                tbl.MergeCellsInColumn(0, lstDoiTuongGuiVBDi.Count + 3 + lstDoiTuongGuiVBDen.Count, lstDoiTuongGuiVBDi.Count + 3 + lstDoiTuongGuiVBDen.Count + lstLoaiVBDen.Count);
                tbl.Rows[lstDoiTuongGuiVBDi.Count + 3 + lstDoiTuongGuiVBDen.Count].Cells[0].Paragraphs.First().Append("Văn bản đến").Alignment = Alignment.center;
                tbl.Rows[lstDoiTuongGuiVBDi.Count + 3 + lstDoiTuongGuiVBDen.Count + lstLoaiVBDen.Count].Cells[1].Paragraphs.First().Append("Tổng cộng");
                var lstTong = new List<int>();
                lstTong = dicTong["LoaiVBDen"];
                int tong = 0;
                for (int i = 0; i < lstTong.Count; i++)
                {
                    tbl.Rows[lstDoiTuongGuiVBDi.Count + 3 + lstDoiTuongGuiVBDen.Count + lstLoaiVBDen.Count].Cells[i + 2].Paragraphs.First().Append(lstTong[i].ToString()).Alignment = Alignment.center;
                }
                for (int i = 0; i < 12; i++)
                {
                    tong += lstTong[i];
                }
                tbl.Rows[lstDoiTuongGuiVBDi.Count + 3 + lstDoiTuongGuiVBDen.Count + lstLoaiVBDen.Count].Cells[lstTong.Count + 2].Paragraphs.First().Append(tong.ToString()).Alignment = Alignment.center;
            }

            //var fName = DateTime.Now.ToString("InSoVanBanDen_ddMMyyyyhhmmss");
            //gDoc.SaveAs(TempPath + @"\"+ fName + ".docx");
            var path = "~/Content/Export";
            var fileName = "Word-BaoCaoThongKeVanBanDenDi-" + timeStr + ".docx";
            var pathSrv = Server.MapPath(path);
            if (!Directory.Exists(pathSrv))
            {
                Directory.CreateDirectory(pathSrv);
            }
            var outfile = TKM.Utils.CommonUtils.GetPath(path, fileName);
            gDoc.SaveAs(outfile);
            gDoc.Dispose();
            return File(outfile, "application/docx", fileName);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportPDFThongKeVanBanDiDen(FormCollection collection)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            string htmlString = collection["GridHtml"];
            string tenFile = collection["tenFilePDF"];
            string widthtable = collection["widthTable"];
            string baseUrl = "";

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Landscape";
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdf_orientation, true);

            int webPageWidth = Convert.ToInt32(widthtable);
            int webPageHeight = 0;

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.DisplayHeader = false;
            converter.Options.DisplayFooter = false;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            byte[] pdf = doc.Save();
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "PDF-" + tenFile + "-" + timeStr + ".pdf";
            return fileResult;
        }
        #endregion
    }
}