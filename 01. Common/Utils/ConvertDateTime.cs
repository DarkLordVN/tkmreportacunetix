using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Utils
{
    public static class ConvertDateTime
    {
        public static string GetTimeSpanBefore(DateTime datetime)
        {
            var returnStr = string.Empty;
            var timeSpan = DateTime.Now - datetime;
            if (timeSpan.Days > 0)
            {
                returnStr = timeSpan.Days + " ngày trước";
            }
            else
            {
                returnStr = timeSpan.Hours > 0 ? timeSpan.Hours + " giờ " + timeSpan.Minutes + " phút trước" : timeSpan.Minutes + " phút trước";
            }
            return returnStr;
        }
        public static string ConvertDateTimeToString(DateTime? date, string format = "dd/MM/yyyy")
        {
            if (date == null)
                return "";
            return Convert.ToDateTime(date).ToString(format);
        }
        public static string ConvertDateTimeToTimeString(DateTime? date, string format = "dd/MM/yyyy hh:mm tt")
        {
            if (date == null)
                return "";
            return Convert.ToDateTime(date).ToString(format);
        }
        public static string ConvertDateToString(DateTime? date, string format = "dd/MM/yyyy")
        {
            if (date == null)
                return "";
            return Convert.ToDateTime(date).ToString(format);
        }
        public static DateTime ConvertToDate(string dateStr)
        {
            DateTime date;
            if (!string.IsNullOrEmpty(dateStr))
            {
                DateTime.TryParseExact(dateStr.Replace(" ", ""), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                if(date == DateTime.MinValue)
                {
                    DateTime.TryParseExact(dateStr.Replace(" ", ""), "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                }
            }
            else date = DateTime.Now;
            return date;
        }
        public static DateTime ConvertToDateTime(string dateStr)
        {
            DateTime date;
            if (!string.IsNullOrEmpty(dateStr))
            {
                DateTime.TryParseExact(dateStr.Replace(" ", ""), "dd/MM/yyyyHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            }
            else date = DateTime.Now;
            return date;
        }

        public static string StartOfWeek(DayOfWeek startOfWeek, int addDays = 0)
        {
            var dt = DateTime.Now;
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).AddDays(addDays).Date.ToString("dd/MM/yyyy");
        }
    }
}
