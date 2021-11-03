using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TKM.Utils
{
    public static class CommonUtils
    {
        //public static string Cnstr = @"data source=.\SQLEXPRESS;initial catalog=btmg0865_WHM001;integrated security=True;MultipleActiveResultSets=True;";
        //public static string Cnstr = @"data source=192.168.168.140;initial catalog=QLVB_CDK;User Id=qlvbdh;Password=abcd@1234;";

        public static double RoundUp(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }

        public static string GetPath(string path = "~/", string filename = "")
        {
            var p = System.Web.HttpContext.Current.Server.MapPath(path);
            return Path.Combine(p, filename);
        }
        //Get description attribute enum c#
        public static string GetDescription(System.Enum input)
        {
            Type type = input.GetType();
            MemberInfo[] memInfo = type.GetMember(input.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = (object[])memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return input.ToString();
        }
        public static string GenMaBaoCaoNoiBo()
        {
            return "HTC-BCNB-BM03-" + DateTime.Now.Ticks.ToString();
        }

        public static string RandomString(int length, bool allUppercase = true, bool includeNumber = false, bool onlyNumber = false)
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (!allUppercase) chars += "abcdefghijklmnopqrstuvwxyz";
            if (includeNumber) chars += "0123456789";
            if (onlyNumber) chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RemoveSpecialChar(string str)
        {
            //str = Regex.Replace(str, "[^0-9A-Za-z]+", "");
            str = str.Replace("|", "");
            return str;
        }
        
        public static object GetValueByName<T>(T obj, string fieldName)
        {
            var t = obj.GetType();

            var prop = t.GetProperty(fieldName);
            if (prop == null) return null;
            return prop.GetValue(obj);
        }
        public static object GetValueByName(object obj, string fieldName)
        {
            var t = obj.GetType();

            var prop = t.GetProperty(fieldName);

            return prop.GetValue(obj);
        }
        //Dùng cho module có phân cấp cha con
        /// <summary>
        /// Set Level cho danh mục
        /// </summary>
        private static readonly Dictionary<string, int> DicLevel = new Dictionary<string, int>();
        private static int _level;

        public static List<T> CreateLevel<T>(List<T> listAllCategory, int KhoaChaID = 0)
        {
            var lstParent = (from g in listAllCategory
                             where ((int)GetValueByName(g, "KhoaChaID")) == KhoaChaID
                             // orderby (int)GetValueByName(g, "Ordering")
                             select g).ToList<T>();
            var lstOrder = new List<T>();
            FindChild(listAllCategory, lstParent, ref lstOrder);
            return lstOrder;
        }
        public static void FindChild<T>(List<T> listAllCategory, List<T> lstParent, ref List<T> lstOrder)
        {
            //using (var enumerator = lstParent.OrderBy(g => (int)GetValueByName(g, "Ordering")).GetEnumerator())
            using (var enumerator = lstParent.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    Func<KeyValuePair<string, int>, bool> predicate = g => g.Key == GetValueByName(item, "KhoaChaID").ToString();
                    var pair = DicLevel.FirstOrDefault(predicate);
                    if (((int)GetValueByName(item, "KhoaChaID")) == 0)
                    {
                        _level = 0;
                    }
                    if (string.IsNullOrEmpty(pair.Key))
                    {
                        DicLevel.Add(GetValueByName(item, "KhoaChaID").ToString(), _level);
                    }
                    else
                    {
                        _level = pair.Value;
                    }
                    if (item != null)
                    {
                        var property = item.GetType().GetProperty("Level");
                        try
                        {
                            property.SetValue(item, _level, null);
                        }
                        catch (Exception)
                        {
                            //WriteLog.Write("Không có Field Level \n");
                            break;
                        }
                    }
                    Func<T, bool> func2 = g => GetValueByName(g, "ID").ToString() == GetValueByName(item, "ID").ToString();
                    if (!lstOrder.Where(func2).Any())
                    {
                        lstOrder.Add(item);
                    }
                    Func<T, bool> func3 = g => GetValueByName(g, "KhoaChaID").ToString() == GetValueByName(item, "ID").ToString();
                    var list = listAllCategory.Where<T>(func3).ToList<T>();
                    if (list.Count <= 0) continue;
                    foreach (var info2 in list.Select(local => item != null ? item.GetType().GetProperty("Level") : null))
                    {
                        try
                        {
                            info2.SetValue(item, _level, null);
                        }
                        catch (Exception)
                        {
                            //WriteLog.Write("Không có Field Level \n");
                            break;
                        }
                    }
                    ++_level;
                    FindChild(listAllCategory, list, ref lstOrder);
                }
            }
        }

        private static List<int> lstChildID = new List<int>();

        public static List<int> GetChild<T>(List<T> lstCategory, int unitId, bool first = false, string KhoaChaID = "KhoaChaID", string vID = "ID", string Ordering = "Ordering")
        {
            if (!first)
                lstChildID = new List<int>();
            lstChildID.Add(unitId);
            var tmp = (from g in lstCategory
                       where ((int)GetValueByName(g, KhoaChaID)) == unitId
                       //orderby (int)GetValueByName(g, Ordering)
                       select g).ToList<T>();
            var id = 0;
            for (var i = 0; i < tmp.Count(); i++)
            {
                id = (int)GetValueByName(tmp[i], vID);
                lstChildID.Add(id);
                var tmpCount = (from g in lstCategory
                                where ((int)GetValueByName(g, KhoaChaID)) == id
                                //      orderby (int)GetValueByName(g, Ordering)
                                select g).ToList<T>().Count();
                if (tmpCount > 0)
                    GetChild(lstCategory, id, true, KhoaChaID, vID, Ordering);
            }
            return lstChildID;
        }
        private static List<int> lstKhoaChaID = new List<int>();
        public static List<int> GetAllParent<T>(List<T> lstCategory, int unitId, bool first = false, string KhoaChaID = "KhoaChaID", string vID = "ID")
        {
            if (!first)
                lstKhoaChaID = new List<int>();
            lstKhoaChaID.Add(unitId);
            var tmp = (from g in lstCategory
                       where ((int)GetValueByName(g, vID)) == unitId
                       select g).FirstOrDefault();
            int pid = (int)GetValueByName(tmp, KhoaChaID);
            if (pid > 0)
            {
                GetAllParent(lstCategory, pid, true, KhoaChaID, vID);
            }
            return lstKhoaChaID;
        }
        
        public static int TryParseInt(string str)
        {
            try {
                var intVal = 0;
                int.TryParse(str, out intVal);
                return intVal;
            } catch
            {
                return 0;
            }
        }
    }
}
