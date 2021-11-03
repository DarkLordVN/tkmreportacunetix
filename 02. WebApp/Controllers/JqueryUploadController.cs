using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TKM.Utils;

namespace TKM.WebApp.Controllers
{
    public class JqueryUploadController : Controller
    {
        private string folderPath = @"/UploadedFiles/FileVanBan";
        // GET: JqueryUpload
        [HttpPost]
        public ActionResult UploadFiles(string intFolder = "0")
        {

            // Checking no of files injected in Request object  
            var lstFile = new List<string>();
            var serverPath = Server.MapPath("~");
            serverPath = serverPath.TrimEnd('\\');
            if (Request.Files.Count <= 0) return Json("Bạn chưa chọn file.");
            try
            {
                //  Get all files from Request object  
                var files = Request.Files;
                var name = ""; long size = 0;
                object[] response = new object[files.Count];
                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var fname = "";
                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        if (file != null)
                        {
                            var testfiles = file.FileName.Split(new[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                    }
                    else
                    {
                        if (file != null) fname = file.FileName;
                    }
                    name = fname.Split('.')[0];
                    var ext = Path.GetExtension(file.FileName);
                    var strFolder = "";
                    string strDate = DateTime.Now.ToString("dd_MM_yy_HH_mm_ss");
                    DateTime d = DateTime.Now;
                    switch (intFolder)
                    {
                        case "1":
                            //Avatar
                            folderPath = "/UploadedFiles/Staffs";
                            break;
                        case "2":
                            //File văn bản đến
                            strFolder = "/UploadedFiles/FileVanBan/VanBanDen";
                            break;
                        case "3":
                            //File văn bản đi
                            strFolder = "/UploadedFiles/FileVanBan/VanBanDi";
                            break;
                        case "4":
                            //File văn bản dự thảo đi
                            strFolder = "/UploadedFiles/FileVanBan/VanBanDuThaoDi";
                            break;
                        default:
                            folderPath = "/UploadedFiles/Upload";
                            break;
                    }
                    folderPath = strFolder + "/" + d.Year.ToString() + "/" + d.ToString("MM") + "/" + d.ToString("dd");
                    if (!Directory.Exists(serverPath + folderPath))
                    {
                        Directory.CreateDirectory(serverPath + folderPath);
                    }
                    //if ((".pdf".ToUpper()).Equals(ext.ToUpper()) || ((".doc").ToUpper()).Equals(ext.ToUpper()) || ((".docx").ToUpper()).Equals(ext.ToUpper()) || ((".xls").ToUpper()).Equals(ext.ToUpper()) || ((".xlsx").ToUpper()).Equals(ext.ToUpper()) || ext.ToLower().Equals(".pdf") || ext.ToLower().Equals(".ps") || ext.ToLower().Equals(".tif") || ext.ToLower().Equals(".gif") || ext.ToLower().Equals(".jpeg") || ext.ToLower().Equals(".html") || ext.ToLower().Equals(".rft") || ext.ToLower().Equals(".doc") || ext.ToLower().Equals(".docx") || ext.ToLower().Equals(".xls") || ext.ToLower().Equals(".xlsx") || ext.ToLower().Equals(".ppt") || ext.ToLower().Equals(".pptx"))
                    //{

                    //}
                    string _path = Path.Combine(folderPath, strDate + ext);
                    var linkfile = _path.Replace('\\', '/');
                    lstFile.Add(linkfile);
                    _path = _path.Replace('/', '\\');
                    var pathFull = serverPath + _path;
                    if (file == null) continue;
                    file.SaveAs(pathFull);
                    long sizefile = new System.IO.FileInfo(pathFull).Length;
                    size = sizefile;
                    //get ten 
                    var abcd = Request.Form.GetValues("abcd");

                    var linkabcd = "";
                    var error = "";
                    if (file.ContentLength > 15000000000)
                    {
                        error = "File quá lớn";
                    }
                    var fileb = new
                    {
                        thumbnailUrl = pathFull,
                        url = linkfile,
                        urlabcd = linkabcd,
                        name = name,
                        abcd = abcd,
                        deleteUrl = "/JqueryUpload/DeleteFile?url=" + pathFull,
                        size = size,
                        error = error,
                        deleteType = "GET"
                    };
                    response[i] = fileb;
                }
                // Returns message that successfully uploaded  
                return Json(new { files = response, JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new
                {
                    error = "Upload không thành công"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateInput(false)]
        public ActionResult DeleteFile(string url)
        {
            var arr = url.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                if (url.Contains(@"\"))
                    System.IO.File.Delete(arr[i]);
                else {
                    var link = TKM.Utils.CommonUtils.GetPath("~" + url);
                    System.IO.File.Delete(link);
                }
            }
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SaveImage(string data)
        {
            string base64 = data.Substring(data.IndexOf(',') + 1);
            base64 = base64.Trim('\0');
            byte[] chartData = Convert.FromBase64String(base64);
            string filePath = Server.MapPath("~/2021/text.png");
            System.IO.File.WriteAllBytes(filePath, chartData);

            string htmlString = "<html><header><style>table {border: 1px solid #dee2e6;width: 100%;}table thead {background-color: #5c6bc0!important;color: #fff;}.table thead th {vertical-align: middle;}.table-bordered thead td, .table-bordered thead th {border-bottom: 2px solid #dee2e6;}table th {font-weight: bold !important;}.table-bordered td, .table-bordered th {border: 1px solid #dee2e6;}table.table td, table.table th {padding-top: 8px;padding-bottom: 8px;padding-left: 8px;padding-right: 8px;}.table td {font-weight: 400;color: #4f4f4f;}</style></header><body><table class='table table-bordered'><thead><th style='width: 50px;'>STT</th><th>Text</th></thead><tbody><tr><td>1</td><td>Test</td></tr><tr><td>2</td><td><b>bold</b></td></tr></tbody></table><img src='" + data + "' /></body></html>";
            string tenFile = "tesst";
            string widthtable = "";
            string baseUrl = "";
            try
            {
                //string pdf_page_size = "A4";
                //PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                //    pdf_page_size, true);

                //string pdf_orientation = "Landscape";
                //PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdf_orientation, true);

                //int webPageWidth = 1024;
                //int webPageHeight = 0;

                //HtmlToPdf converter = new HtmlToPdf();
                //converter.Options.PdfPageSize = pageSize;
                //converter.Options.PdfPageOrientation = pdfOrientation;
                //converter.Options.WebPageWidth = webPageWidth;
                //converter.Options.WebPageHeight = webPageHeight;
                ////converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
                //converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.NoAdjustment;
                //converter.Options.DisplayHeader = false;
                //converter.Options.DisplayFooter = false;
                //converter.Options.MarginLeft = 10;
                //converter.Options.MarginRight = 10;
                //converter.Options.MarginTop = 20;
                //converter.Options.MarginBottom = 20;
                //PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
                //byte[] pdf = doc.Save();
                //doc.Close();
                //filePath = Server.MapPath("~/2021/htmlToPDF-Jupload.pdf");
                //System.IO.File.WriteAllBytes(filePath, pdf);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string getImagesPath(byte[] bytesimg, double widthExpect = 0, string extension = ".png")
        {
            var filePath = "";
            try
            {
                if (widthExpect > 0)
                {
                    using (var ms = new MemoryStream(bytesimg))
                    {
                        var image = Image.FromStream(ms);

                        //var ratioX = (double)150 / image.Width;
                        //var ratioY = (double)50 / image.Height;
                        //var ratio = Math.Min(ratioX, ratioY);
                        var ratio = widthExpect / image.Width;

                        var width = (int)(image.Width * ratio);
                        var height = (int)(image.Height * ratio);

                        var newImage = new Bitmap(width, height);
                        Graphics.FromImage(newImage).DrawImage(image, 0, 0, width, height);
                        Bitmap bmp = new Bitmap(newImage);

                        ImageConverter converter = new ImageConverter();

                        bytesimg = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                    }
                }
                var dt = DateTime.Now;
                var folder = "/TempFile/Images";
                var folderFull = CommonUtils.GetPath("~" + folder);
                if (!Directory.Exists(folderFull))
                    Directory.CreateDirectory(folderFull);
                var fileName = string.Format("/{0:yyyyMMddHHmmss}", dt) + "_w" + widthExpect + extension;
                filePath = folderFull + fileName;
                System.IO.File.WriteAllBytes(filePath, bytesimg);
                return filePath;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex.ToString());
            }
            return filePath;
        }
    }
}