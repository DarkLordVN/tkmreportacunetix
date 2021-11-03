using System;
using System.Web;
using System.IO;
namespace TKM.Utils
{
    public class UploadFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="serverPath"></param>
        /// <param name="pathFile"></param>
        /// <param name="error"></param>
        /// <param name="kieuUpload">0 : File tài liệu(doc,...) | 1: File ảnh(jpg,...)</param>
        /// <returns></returns>
        public static string uploadFile(HttpPostedFileBase file, string serverPath, string pathFile, ref string error, int kieuUpload = 0)
        {
            serverPath = serverPath.TrimEnd('\\');
            if (file == null || file.ContentLength <= 0 || file.ContentLength > 20971520)
            {
                error += "Không tìm thấy file. <br/>";
                return "";
            }
            string strDirectoryPath = Path.Combine(serverPath, pathFile);
            if (!Directory.Exists(serverPath + pathFile))
            {
                Directory.CreateDirectory(serverPath + pathFile);
            }
            string strDate = DateTime.Now.ToString("dd_MM_yy_HH_mm_ss");
            string _path = Path.Combine(pathFile, strDate + Path.GetExtension(file.FileName));
            string ext = Path.GetExtension(_path);
            _path = _path.Replace('/', '\\');
            var pathFull = serverPath + _path;
            switch (kieuUpload)
            {
                case 0:
                    if ((".pdf".ToUpper()).Equals(ext.ToUpper()) || ((".doc").ToUpper()).Equals(ext.ToUpper()) || ((".docx").ToUpper()).Equals(ext.ToUpper()) || ((".xls").ToUpper()).Equals(ext.ToUpper()) || ((".xlsx").ToUpper()).Equals(ext.ToUpper()))
                    {
                        file.SaveAs(pathFull);
                        return _path;
                    }
                    else
                    {
                        error += "File tài liệu phải có định dạng(pdf, doc, docx, xls, xlsx).<br/>";
                        return "";
                    }
                case 1:
                    if (ext.ToLower().Equals(".gif") || ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".jpeg") || ext.ToLower().Equals(".png"))
                    {
                        file.SaveAs(pathFull);
                        return _path;
                    }
                    else
                    {
                        error += "File tài liệu phải có định dạng(pdf, doc, docx, xls, xlsx).<br/>";
                        return "";
                    }
                default:
                    if (ext.ToLower().Equals(".pdf") || ext.ToLower().Equals(".ps") || ext.ToLower().Equals(".tif") || ext.ToLower().Equals(".gif") || ext.ToLower().Equals(".jpeg") || ext.ToLower().Equals(".html") || ext.ToLower().Equals(".rft") || ext.ToLower().Equals(".doc") || ext.ToLower().Equals(".docx") || ext.ToLower().Equals(".xls") || ext.ToLower().Equals(".xlsx") || ext.ToLower().Equals(".ppt") || ext.ToLower().Equals(".pptx"))
                    {
                        file.SaveAs(pathFull);
                        return _path;
                    }
                    else
                    {
                        error += "File tài liệu phải có định dạng(pdf, doc, docx, xls, xlsx).<br/>";
                        return "";
                    }
            }
            return "Lỗi exception.";
        }
    }
}
