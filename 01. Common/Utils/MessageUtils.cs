using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Utils
{
    public class MessageUtils
    {
        public static string Err(string message)
        {
            return string.Format("<div class=\"alert alert-danger fade in\">{0}</div>", message);
        }

        public static string Err(List<string> msgList)
        {
            if (msgList != null && msgList.Count > 0)
            {
                var str = new StringBuilder();
                str.AppendLine("<div class=\"alert alert-danger fade in\"><button class=\"close\" data-dismiss=\"alert\">×</button>");
                str.AppendLine("<ul>");

                foreach (var o in msgList)
                {
                    str.AppendFormat("<li>{0}</li>", o);
                }
                str.AppendLine("</ul>");
                str.AppendFormat("</div>");

                return str.ToString();
            }
            return string.Empty;
        }
    }
}
