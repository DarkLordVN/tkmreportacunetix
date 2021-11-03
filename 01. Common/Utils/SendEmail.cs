using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Web.Configuration;

namespace TKM.Utils
{
    public class SendEmail
    {
        public static bool Send(string toEmail, string subject, string body, List<string> lstCC = null,string mailusername= "",string mailpassword = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(mailusername))
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(toEmail);
                    mail.From = new MailAddress(mailusername);
                    mail.Body = body;
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    //smtp.Host = "mail.vr.org.vn";
                    //smtp.Port = 25;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(mailusername, mailpassword); // Enter seders User name and password  
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }

        }
        public static bool SendVanBan(string toEmail, string subject, string sokyhieu,string trichyeu,string noidung,string url, List<string> lstCC = null, string mailusername = "", string mailpassword = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(mailusername) && !string.IsNullOrEmpty(mailpassword))
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(toEmail);
                    mail.From = new MailAddress(mailusername);
                    mail.Body = (!string.IsNullOrEmpty(sokyhieu) ? "Số ký hiệu: " +sokyhieu + "</br>" : "")
                        + "Trích yếu: " + trichyeu +"</br>" 
                        +"Nội dung: " + noidung + "</br>"
                        + "Bấm vào <a href=\"http://222.254.35.35" + url + "\">đây</a> để xử lý";
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    //smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    //smtp.Host = "smtp.gmail.com";
                    //smtp.Port = 587;
                    smtp.Host = "mail.vr.org.vn";
                    smtp.Port = 25;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(mailusername, mailpassword); // Enter seders User name and password  
                    smtp.EnableSsl = true;
                    smtp.ServicePoint.MaxIdleTime = 1;
                    smtp.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }

        }
    }
}
