using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TKM.Utils
{
    public class OutputLog
    {
        public static void WriteOutputLog(Exception ex)
        {
            //WriteOutputLog(HttpContext.Current, ex, string.Empty);
            WriteOutputLog(ex.ToString());
        }

        public static void WriteOutputLog(Exception ex, string text)
        {
            //WriteOutputLog(HttpContext.Current, ex, text);
            WriteOutputLog(ex.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="folder">Default: /Log</param>
        public static void WriteOutputLog(string text, string folder = "")
        {
            try
            {
                if(string.IsNullOrEmpty(folder)) folder = "/Log";
                var context = HttpContext.Current;
                var dt = DateTime.Now;
                string lDir;
                string lFile;
                var sbLog = new StringBuilder();
                sbLog.AppendFormat("--------- {0:HH:mm:ss} ---------", dt).AppendLine();
                if (context != null)
                {
                    lDir = context.Request.MapPath(string.Format(folder));
                    lFile = context.Request.MapPath(string.Format(folder + "/Log_{0:yyyyMMdd}.txt", dt));

                    sbLog.AppendFormat("Ref URL: {0}",
                                       (context.Request.UrlReferrer == null)
                                           ? ""
                                           : context.Request.UrlReferrer.ToString()).AppendLine();
                    sbLog.AppendFormat("Host: {0}", context.Request.Url.Authority).AppendLine();
                    sbLog.AppendFormat("URL: {0}", context.Request.RawUrl).AppendLine();
                    sbLog.AppendFormat("IP: {0}", context.Request.UserHostAddress).AppendLine();
                }
                else
                {
                    lDir = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(folder));
                    lFile = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(folder + "/Log_{0:yyyyMMdd}.txt", dt));
                }
                if (lDir == null || lFile == null) return;
                if (!Directory.Exists(lDir))
                    Directory.CreateDirectory(lDir);
                if (!string.IsNullOrEmpty(text))
                    sbLog.AppendFormat("Text: {0}", text).AppendLine();
                File.AppendAllText(lFile, sbLog.ToString());
            }
            catch(Exception ex) {
                var x = ex.ToString();
            }
        }
    }
}
