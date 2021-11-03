using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKM.WebApp.Controllers
{
    public class BackupDatabaseController : BaseController
    {
        //
        // GET: /Admin/BackupDatabase/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Backup(string Ngay = "",string Gio = "",string Phut = "")
        {
            var filePathBackupSql = ConfigurationManager.AppSettings["FilePathBackupSQL"];
            if (!Directory.Exists(filePathBackupSql))
            {
                Directory.CreateDirectory(filePathBackupSql);
            }
            var splNgay = new string[3];
            if (!string.IsNullOrEmpty(Ngay) && Ngay.Contains("/"))
            {
                splNgay = Ngay.Split('/');
                if (splNgay.Length > 0)
                    Ngay = splNgay[2] + splNgay[1] + splNgay[0];
            }
            if (!string.IsNullOrEmpty(Ngay) && Ngay.Contains("-"))
            {
                splNgay = Ngay.Split('-');
                if (splNgay.Length > 0)
                    Ngay = splNgay[2] + splNgay[1] + splNgay[0];
            }
            var sqlConnectionString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
            var conn = new SqlConnection(sqlConnectionString);
            try
            {
                conn.Open();
                var dbName = conn.Database;
                var strBackup = dbName + "_" + Ngay + "_" + Gio + Phut;
                var command = new SqlCommand(@"backup database " + string.Format("[{0}]", dbName) + " to disk ='" + filePathBackupSql + string.Format("{0}.bak", strBackup) + "' with init,stats=10", conn)
                {
                    CommandTimeout = 300
                };
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Backup thất bại",
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Backup thành công",
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Restore(string Ngay = "", string Gio = "", string Phut = "")
        {
            var filePathBackupSql = ConfigurationManager.AppSettings["FilePathBackupSQL"];
            var sqlConnectionString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
            var conn = new SqlConnection(sqlConnectionString);
            try
            {
                conn.Open();
                var dbName = conn.Database;
                var splNgay = new string[3];
                if (!string.IsNullOrEmpty(Ngay) && Ngay.Contains("/"))
                {
                    splNgay = Ngay.Split('/');
                    if (splNgay.Length > 0)
                        Ngay = splNgay[2] + splNgay[1] + splNgay[0];
                }
                if (!string.IsNullOrEmpty(Ngay) && Ngay.Contains("-"))
                {
                    splNgay = Ngay.Split('-');
                    if (splNgay.Length > 0)
                        Ngay = splNgay[2] + splNgay[1] + splNgay[0];
                }
                filePathBackupSql = filePathBackupSql + dbName +"_"+ Ngay + "_" + Gio + Phut + ".bak";
                if (!System.IO.File.Exists(filePathBackupSql))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Chưa có file backup",
                    }, JsonRequestBehavior.AllowGet);
                }
                var command = new SqlCommand(@"USE MASTER; ALTER DATABASE " + dbName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
"RESTORE DATABASE " + dbName + " FROM DISK = '" + filePathBackupSql + "' WITH REPLACE ALTER DATABASE " + dbName + " SET MULTI_USER", conn);
                command.ExecuteReader();
                conn.Close();
            }
            catch (Exception)
            {
                conn.Close();
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Restore thất bại",
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Restore thành công",
            }, JsonRequestBehavior.AllowGet);
        }
    }
}