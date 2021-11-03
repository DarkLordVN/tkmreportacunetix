using TKM.Utils.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SelectPdf;
using TuesPechkin;

namespace TKM.Utils
{
    public static class ExportPDF
    {
        public static void setConfigCommon(ref HtmlToPdf converter)
        {
            int webPageWidth = 1024;
            int webPageHeight = 0;
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                       "A4", true);
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), "Landscape", true);
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.NoAdjustment;
            converter.Options.DisplayHeader = false;
            converter.Options.DisplayFooter = false;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
        }
        public static void setHeader(ref HtmlToPdf converter, string headerUrl)
        {
            //string headerUrl = CommonUtils.GetPath("~/2021/test.html");
            converter.Options.DisplayHeader = true;
            converter.Header.DisplayOnFirstPage = true;
            converter.Header.DisplayOnOddPages = true;
            converter.Header.DisplayOnEvenPages = true;
            converter.Header.Height = 30;

            PdfHtmlSection headerHtml = new PdfHtmlSection(headerUrl);
            headerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            converter.Header.Add(headerHtml);
        }
        public static void setFooter(ref HtmlToPdf converter, string headerUrl)
        {
            PdfTextSection textRight = new PdfTextSection(0, 0,
               "Trang: {page_number} / {total_pages}  ", new System.Drawing.Font("Times New Roman", 10));
            // footer settings
            converter.Options.DisplayFooter = true;
            converter.Footer.DisplayOnFirstPage = true;
            converter.Footer.DisplayOnOddPages = true;
            converter.Footer.DisplayOnEvenPages = true;
            converter.Footer.Height = 30;
            textRight.HorizontalAlign = PdfTextHorizontalAlign.Right;
            converter.Footer.Add(textRight);
            //var textLeft = new PdfTextSection(0, 0,
            //   "Công ty TKM – Lưu hành /n a <br/> v",
            //   new System.Drawing.Font("Times New Roman", 10));
            PdfHtmlSection textLeft = new PdfHtmlSection(headerUrl);
            textLeft.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            converter.Footer.Add(textLeft);
        }
        public static StringBuilder KhoiTao()
        {
            //var str = new StringBuilder("<html><head><style>body{margin:0;font-family:'Times New Roman',Roboto,Arial,'Noto Sans',sans-serif;font-size:1rem;font-weight:400;line-height:1.5;color:#212529;text-align:left;background-color:#fff}table{border:1px solid #dee2e6;width:100%}table thead{background-color:#5c6bc0!important;color:#fff}.table thead th{vertical-align:middle}.table-bordered thead td, .table-bordered thead th{border-bottom:2px solid #dee2e6}table th{font-weight:bold !important}.table-bordered td, .table-bordered th{border:1px solid #dee2e6}table.table td, table.table th{padding-top:8px;padding-bottom:8px;padding-left:8px;padding-right:8px}.table td{font-weight:400;color:#4f4f4f}.imgChart{display:block;page-break-before:always;page-break-inside:avoid}h5{margin:0;width:100%}h1{text-transform:uppercase;color:darkblue}h5{color:darkgoldenrod}.form-group{width:100%;margin-bottom:1rem}hr{border-top:1px solid rgba(0,0,0,.1);width:90%}.text-center{text-align:center}.container-fluid{width:100%;padding-right:15px;padding-left:15px;margin-right:auto;margin-left:auto}p{margin:2px 0}</style></head><body>");
            //var str = new StringBuilder("<html xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:word\" xmlns=\"http://www.w3.org/TR/REC-html40\"><head></head><body>");
            var str = new StringBuilder("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'><head><meta charset='utf-8'></head><body style='margin: 0; font-family: 'Times New Roman',Roboto,Arial,'Noto Sans',sans-serif; font-size: 1rem; font-weight: 400; line-height: 1.5; color: #212529; text-align: left; background-color: #fff;'>");
            return str;
        }
        public static StringBuilder KetThuc(bool hasTrangKy = true, string fullName = "Quản trị báo cáo")
        {
            var str = new StringBuilder("");
            //if (hasTrangKy)
            //{
            //    //Kieu 1 
            //    str.Append(getPageBreak());
            //    str.Append("<div style='page-break-before: always;'><div style='width: 100%; text-align: center;'><h2><b>TRANG KÝ NHẬN</b></h2></div> <table style='border: 1px solid #dee2e6; width: 100%;'> <tbody> <tr> <td style='padding: 8px;border: 1px solid #dee2e6;width: 25%;vertical-align: top;'><a>Người xuất báo cáo:</a><br/><a><b>" + fullName + "</b></a></td> <td style='padding: 8px;border: 1px solid #dee2e6;'>" + getBR(5) + "</td> <td style='padding: 8px;border: 1px solid #dee2e6;vertical-align: top;width: 25%;'>Ngày      /      /2021</td> </tr> <tr> <td style='padding: 8px;border: 1px solid #dee2e6;width: 25%;vertical-align: top;'><a>Người duyệt:</a><br/><a><b></b></a></td> <td style='padding: 8px;border: 1px solid #dee2e6;'>" + getBR(5) + "</td> <td style='padding: 8px;border: 1px solid #dee2e6;vertical-align: top;width: 25%;'>Ngày      /      /2021</td> </tr> </tbody> </table></div>");
            //    ////Kieu 2
            //    //str.Append(getPageBreak());
            //    //str.Append("<div style='page-break-before: always;'> <table style='width: 100%;text-align:center;'> <tbody> <tr> <td style='width: 50%;'><b style='font-size: 24px'>Người xuất báo cáo</b></td> <td><b style='font-size: 24px'>Người duyệt</b></td> </tr> </tbody> </table> </div>");

            //}
            str.Append("</body></html>");
            return str;
        }

        public static StringBuilder pdfTrangBia(string imgPath, string rightDiv, string mainTitleReport, string subTitleReport, string footerReport, bool isPDF = false, string fullName = "Quản trị hệ thống", string pathRoot = "")
        {
            if (string.IsNullOrEmpty(rightDiv))
            {
                rightDiv = "<h5><b>CÔNG TY CỔ PHẦN CÔNG NGHỆ TKM VIỆT NAM</b></h5><h6 style='font-size: 9px;'><b>Trụ sở chính:</b> số 28 , lô 14, Trung Yên 11, Trung Hòa, Cầu Giấy, Hà Nội</h6><h6 style='font-size: 9px;'><b>Điện thoại:</b> (04) 558 9970 - <b>Fax:</b> (04) 558 9971</h6><h6 style='font-size: 9px;'><b>Email:</b> info@tkmgroup.com.vn - <b>Website:</b> https://tkmgroup.com.vn/</h6></div></div>";
            }
            //var str = new StringBuilder("<div class='container-fluid' style='page-break-after: always;'><div class='form-group'><div class='text-center' style='float:left;width: 35%; margin-bottom: 30px;'><img style='width: 60%;' src='" + getImagesBase64(imgPath) + "' /></div><div style='width: 60%; float:right; text-align: right; padding-right: 5%;'><h5><b>CÔNG TY CỔ PHẦN CÔNG NGHỆ TKM VIỆT NAM</b></h5><p><b>Trụ sở chính:</b> số 28 , lô 14, Trung Yên 11, Trung Hòa, Cầu Giấy, Hà Nội</p><p><b>Điện thoại:</b> (04) 558 9970 - <b>Fax:</b> (04) 558 9971</p><p><b>Email:</b> info@tkmgroup.com.vn - <b>Website:</b> https://tkmgroup.com.vn</p></div></div><hr /><div class='form-group text-center'><h1 style='margin-top: 30%;'>Báo cáo tổng hợp</h1><h2 style='margin-bottom: 90%;'>Hệ thống quản lý và giám sát SYSMAN</h2> <b>2021</b></div></div>");
            //var strTest = new StringBuilder("<div class='container-fluid' style='page-break-after: always;'><div class='form-group'><div class='text-center' style='float:left;width: 35%; margin-bottom: 30px;'><img style='width: 60%;' src='" + imgPath + "' /></div><div style='width: 60%; float:right; text-align: right; padding-right: 5%;'>" + rightDiv + "</div></div><hr /><div class='form-group text-center'><h1 style='margin-top: 50%;'>" + mainTitleReport + "</h1><h2 style='margin-bottom: 70%;'>" + subTitleReport + "</h2> <b>" + footerReport + "</b></div></div>");
            var str = new StringBuilder("<div style='page-break-after: always; background: url(" + pathRoot + "/Assets/images/bg2.png) no-repeat;background-size: contain;'><table><tbody><tr><td style='width: 35%;text-align: center;'><img src='" + imgPath + "' /></td><td style='text-align: right; font-size: 10px;'>" + rightDiv + "</td></tr></tbody></table><hr style='border-top: 1px solid rgba(0,0,0,.1);width: 100%;'/><div style='width: 100%; margin-bottom: 1rem; text-align: center;'>" + (isPDF ? getBR(22) : getBR(14)) + "<div style='background-image: url(" + pathRoot + "/Assets/images/bg1.png); background-size: cover; color: #fff; padding: 5px;'><h1>" + mainTitleReport + "</h1><h2>" + subTitleReport + "</h2></div>" + (isPDF ? getBR(26) : getBR(16)) + "<b>" + footerReport + "</b></div></div>");

            //str.Append(getPageBreak());
            str.Append("<div style='page-break-before: always;page-break-after: always;'><div style='width: 100%; text-align: center;'><h2><b>TRANG KÝ NHẬN</b></h2></div> <table style='border: 1px solid #dee2e6; width: 100%;'> <tbody> <tr> <td style='padding: 8px;border: 1px solid #dee2e6;width: 25%;vertical-align: top;'><a>Người xuất báo cáo:</a><br/><a><b>" + fullName + "</b></a></td> <td style='padding: 8px;border: 1px solid #dee2e6;'>" + getBR(5) + "</td> <td style='padding: 8px;border: 1px solid #dee2e6;vertical-align: top;width: 25%;'>Ngày      /      /2021</td> </tr> <tr> <td style='padding: 8px;border: 1px solid #dee2e6;width: 25%;vertical-align: top;'><a>Người duyệt:</a><br/><a><b></b></a></td> <td style='padding: 8px;border: 1px solid #dee2e6;'>" + getBR(5) + "</td> <td style='padding: 8px;border: 1px solid #dee2e6;vertical-align: top;width: 25%;'>Ngày      /      /2021</td> </tr> </tbody> </table></div>");

            return str;
        }

        private static string getBR(int num)
        {
            var str = "";
            for (int i = 0; i < num; i++)
            {
                str += "<br/>";
            }
            return str;
        }

        public static string getPageBreak()
        {
            return "<pre><br clear=all style='mso-special-character:line-break;page-break-before:always;page-break-after:always;'></pre>";
        }
        public static StringBuilder pdfAnh(string imgPath, string imgStyle)
        {
            var str = new StringBuilder();
            try
            {
                str.Append("<div style='display:block;page-break-inside: avoid;width: 100%'><img style='page-break-inside: avoid;" + imgStyle + "' src='" + imgPath + "' /></div>");
                //str.Append("<table><body><tr><td><img style='page-break-inside: avoid;" + imgStyle + "' src='" + imgPath + "' /></td></tr></body></table>");
                //str.Append("<table style='white-space: nowrap;'><tbody><tr><td style='width: 35%;text-align: center;'><img src='" + imgPath + "' /><img src='" + imgPath + "' /></td><td style='text-align: right; font-size: 10px;'></td></tr></tbody></table>");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return str;
        }
        public static StringBuilder pdfBang(List<string> lstCol, DataTable tblData)
        {
            var str = new StringBuilder();
            if (lstCol != null && lstCol.Count > 0 && tblData != null)
            {
                try
                {
                    str.Append("<table style='border: 1px solid #dee2e6; width: 100%;'><tbody><tr style='background-color: #5c6bc0; color: #fff;font-weight: bold;'>");
                    //STT
                    str.Append("<th style='width: 50px; padding: 8px;border-bottom: 2px solid #dee2e6;border: 1px solid #dee2e6;'>STT</th>");
                    //generate thead
                    foreach (string col in lstCol)
                    {
                        if (col.Contains('|'))
                        {
                            var colSplit = col.Split('|');
                            str.Append("<th style='padding: 8px;border-bottom: 2px solid #dee2e6;border: 1px solid #dee2e6;" + colSplit[0] + "'>" + colSplit[1] + "</th>");
                        }
                        else
                        {
                            str.Append("<th style='padding: 8px;border-bottom: 2px solid #dee2e6;border: 1px solid #dee2e6;'>" + col + "</th>");
                        }
                    }
                    str.Append("</tr>");
                    //generate tbody
                    var index = 1;
                    foreach (DataRow row in tblData.Rows)
                    {
                        str.Append("<tr>");
                        //STT
                        str.Append("<td style='padding: 8px;border: 1px solid #dee2e6; text-align: center;'>" + index + "</td>");
                        for (int i = 0; i < tblData.Columns.Count; i++)
                        {
                            str.Append("<td style='padding: 8px;border: 1px solid #dee2e6;'>" + row[i] + "</td>");
                        }
                        str.Append("</tr>");
                        index++;
                    }
                    str.Append("</tbody></table>");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return str;
        }
    }
}
