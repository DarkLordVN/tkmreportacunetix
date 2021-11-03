using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Utils
{
    public class SendSms
    {
        public static bool Send(string strPhone, string strContent)
        {
            if (!string.IsNullOrEmpty(strPhone))
            {
                if (strPhone.Length > 9 && strPhone.Length < 12)
                {
                    string url = "http://api.esms.vn/MainService.svc/xml/SendMultipleMessage_V4/";
                    UTF8Encoding encoding = new UTF8Encoding();
                    string strResult = string.Empty;
                    string customers = "";
                    string[] lstPhone = strPhone.Split(',');

                    for (int i = 0; i < lstPhone.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(lstPhone[i]))
                        {
                            customers = customers + @"<CUSTOMER>"
                                        + "<PHONE>" + lstPhone[i] + "</PHONE>"
                                        + "</CUSTOMER>";
                        }
                        
                    }
                    string SampleXml = @"<RQST>"
                                       + "<APIKEY>"+Utils.Parameter.CONST_APIKEY + "</APIKEY>"
                                       + "<SECRETKEY>"+ Utils.Parameter.CONST_SECRETKEY + "</SECRETKEY>"
                                        + "<ISFLASH>0</ISFLASH>"
                               + "<BRANDNAME>QCAO_ONLINE</BRANDNAME>"
                                       + "<SMSTYPE>2</SMSTYPE>"
                                       + "<CONTENT>" + strContent + "</CONTENT>"
                                       + "<CONTACTS>"+ customers + "</CONTACTS>"
                                       + "</RQST>";
                    string postData = SampleXml.Trim();
                    byte[] data = encoding.GetBytes(postData);
                    HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                    webrequest.Method = "POST";
                    webrequest.Timeout = 500000;
                    webrequest.ContentType = "application/x-www-form-urlencoded";
                    webrequest.ContentLength = data.Length;
                    Stream newStream = webrequest.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                    HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
                    Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader loResponseStream =
                        new StreamReader(webresponse.GetResponseStream(), enc);
                    strResult = loResponseStream.ReadToEnd();
                    loResponseStream.Close();
                    webresponse.Close();
                    return true;
                }
            }
            return false;
        }
    }
}
