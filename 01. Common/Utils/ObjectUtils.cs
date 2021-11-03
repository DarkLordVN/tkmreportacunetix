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

namespace TKM.Utils
{
    public static class ObjectUtils
    {
        public static void CopyObject<T>(object sourceObject, ref T destObject, bool bolNoList)
        {
            //	If either the source, or destination is null, return
            if (sourceObject == null || destObject == null)
                return;

            //	Get the type of each object
            Type sourceType = sourceObject.GetType();
            Type targetType = destObject.GetType();

            //	Loop through the source properties
            foreach (PropertyInfo p in sourceType.GetProperties())
            {
                //	Get the matching property in the destination object
                PropertyInfo targetObj = targetType.GetProperty(p.Name);
                //	If there is none, skip
                if (targetObj == null || p.PropertyType == null || p.PropertyType.Namespace.Contains("EntityFramework") || (bolNoList && p.PropertyType.Namespace.Contains("System.Collections.Generic")))
                    continue;
                try
                {
                    if (p.CanWrite)
                        targetObj.SetValue(destObject, p.GetValue(sourceObject, null), null);
                }
                catch (Exception)
                {
                }

            }
        }

        public static string ObjectToString(object obj, HashSet<string> exceptProperties)
        {
            if (obj == null) return string.Empty;
            Type t = obj.GetType();
            var props = t.GetProperties().OrderByDescending(q => q.Name).ToList();

            var strResult = string.Empty;
            foreach (PropertyInfo prp in props)
            {
                if (!exceptProperties.Contains(prp.Name)
                    && !prp.PropertyType.Namespace.Contains("System.Collections.Generic")
                    && !prp.PropertyType.Namespace.Contains("eSport5.ActionService"))
                {
                    object value = prp.GetValue(obj, new object[] { });
                    if (value == null) value = string.Empty;

                    strResult = string.Format("{0}-{1}:{2}", strResult, prp.Name, value);
                }
            }
            return strResult;
        }

        public static List<string> InvalidJsonElements;
        public static IList<T> DeserializeToList<T>(string jsonString)
        {
            InvalidJsonElements = null;
            var array = JArray.Parse(jsonString);
            IList<T> objectsList = new List<T>();
            foreach (var item in array)
            {
                try
                {
                    objectsList.Add(item.ToObject<T>());
                }
                catch (Exception ex)
                {
                    InvalidJsonElements = InvalidJsonElements ?? new List<string>();
                    InvalidJsonElements.Add(item.ToString());
                }
            }
            return objectsList;
        }

        public static DataTable ToDataTable<T>(this List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable ConvertToDatatable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.FullName.Contains("Nullable"))
                {
                    table.Columns.Add(prop.Name);
                }
                else
                {
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        #region Build tree
        public static void BuildHierachy(int khoaChaId, List<object> lstAll, ref List<object> lstReturn, string colId, string colKhoaChaId, string colThemIcon, string colSort, bool descending = false, string phanCapStyle = null, string iconPhanCapDDL = "--", int deepLv = 0, bool showChild = true, int currentId = 0)
        {
            //Check nếu list trả về null thì khởi tạo
            if (lstReturn == null) lstReturn = new List<object>();
            //Get 
            IEnumerable<object> results = null;
            results = from myRow in lstAll.AsEnumerable()
                      where (int)myRow.GetType().GetProperty(colKhoaChaId).GetValue(myRow, null) == khoaChaId
                      select myRow;
            //Check nếu cần sort
            if (!string.IsNullOrEmpty(colSort) && descending)
            {
                results = from myRow in results
                          orderby myRow.GetType().GetProperty(colSort).GetValue(myRow, null) descending
                          select myRow;
            }
            if (results.Any())
            {
                //Get icon phân cấp nếu không phải root
                var icon = string.Empty;
                if (deepLv > 0)
                    icon = getIconPhanCapLevel(deepLv, phanCapStyle, iconPhanCapDDL);
                foreach (var dtRow in results)
                {
                    //Get khóa cha id của row
                    var getKhoaChaId = dtRow.GetType().GetProperty(colKhoaChaId);
                    //Get dữ liệu trường thêm Icon
                    var getTruongThemIcon = dtRow.GetType().GetProperty(colThemIcon);
                    //Check icon và trường cần thêm icon
                    if (!string.IsNullOrEmpty(icon) && getTruongThemIcon != null)
                    {
                        //Set icon vào trường cần thêm icon
                        getTruongThemIcon.SetValue(dtRow, icon + getTruongThemIcon.GetValue(dtRow, null));
                    }
                    //Get ID
                    var dataId = (int)dtRow.GetType().GetProperty(colId).GetValue(dtRow, null);
                    if (dataId != currentId) lstReturn.Add(dtRow);
                    if (!showChild && currentId == dataId) { continue; }
                    BuildHierachy(dataId, lstAll, ref lstReturn, colId, colKhoaChaId, colThemIcon, colSort, descending, phanCapStyle, iconPhanCapDDL, (deepLv + 1), showChild, currentId);
                }
            }
        }

        private static string getIconPhanCapLevel(int deepLv, string phanCapStyle, string iconPhanCapDLL)
        {
            if (!string.IsNullOrEmpty(phanCapStyle))
            {
                string newLevelIcon = phanCapStyle.Replace(": 10px", string.Format(": {0}px", deepLv * 10));
                return newLevelIcon;
            }
            else
            {
                string newIcon = string.Empty;
                for (int i = 0; i < deepLv; i++)
                {
                    newIcon += iconPhanCapDLL;
                }
                return newIcon;
            }
        }
        #endregion

        #region XML 
        public static object StringToObject(string strNoiDung)
        {
            return "";
        }

        public static T XmlToModel<T>(this T destObject, XElement node)
        {
            //	Get the type of each object
            Type targetType = destObject.GetType();
            //	Loop through the source properties
            foreach (PropertyInfo p in targetType.GetProperties())
            {
                //	Get the matching property in the destination object
                PropertyInfo targetObj = targetType.GetProperty(p.Name);
                //	If there is none, skip
                if (targetObj == null || p.PropertyType == null)
                    continue;
                try
                {
                    if (p.CanWrite)
                    {
                        targetObj.SetValue(destObject, node.Element(targetObj.Name).Value, null);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return destObject;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">ViewModel</typeparam>
        /// <param name="nodeCurr">Current Node</param>
        /// <param name="destObject">Destination ViewModel</param>
        /// <param name="rootName">Create root node in first time</param>
        /// <param name="rootNode">Pass current root node to add node</param>
        /// <returns></returns>
        public static XElement ModelToXml<T>(this XElement nodeCurr, T destObject, string rootName = "", XElement rootNode = null)
        {
            //	Get the type of each object
            Type targetType = destObject.GetType();
            //	Loop through the source properties
            foreach (PropertyInfo p in targetType.GetProperties())
            {
                //	Get the matching property in the target object
                PropertyInfo targetObj = targetType.GetProperty(p.Name);
                //	If there is none, skip
                if (targetObj == null || p.PropertyType == null)
                    continue;
                try
                {
                    // Node add property
                    nodeCurr.Add(new XElement(targetObj.Name, p.GetValue(destObject, null)));
                }
                catch (Exception ex)
                {
                }
            }
            if (!string.IsNullOrEmpty(rootName) && rootNode == null)
            {
                rootNode = new XElement(rootName);
            }
            if (rootNode != null)
            {
                rootNode.Add(nodeCurr);
            }
            return rootNode;
        }
        #endregion

        #region Enums
        public static List<CommonDDL> GetListEnums<T>(Type enumType)
        {
            var lstReturn = new List<CommonDDL>();
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be an enum type.");
            }
            enumType = typeof(T);
            foreach (var value in Enum.GetValues(enumType))
            {
                FieldInfo fi = enumType.GetField(value.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    DescriptionAttribute description = (DescriptionAttribute)attributes[0];
                    lstReturn.Add(new CommonDDL() { Value = (int)value, Text = description.Description });
                }
            }
            lstReturn.Insert(0, new CommonDDL() { Value = 0, Text = "--- Tất cả ---" });
            return lstReturn;
        }
        public static string GetDescription<T>(this T e, int valParam = 0) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                if (valParam <= 0)
                {
                    Array values = System.Enum.GetValues(type);
                    foreach (int val in values)
                    {
                        if (val == e.ToInt32(CultureInfo.InvariantCulture))
                        {
                            var memInfo = type.GetMember(type.GetEnumName(val));
                            var descriptionAttribute = memInfo[0]
                                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                                .FirstOrDefault() as DescriptionAttribute;

                            if (descriptionAttribute != null)
                            {
                                return descriptionAttribute.Description;
                            }
                        }
                    }
                }
                else
                {
                    var memInfo = type.GetMember(type.GetEnumName(valParam));
                    var descriptionAttribute = memInfo[0]
                        .GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .FirstOrDefault() as DescriptionAttribute;

                    if (descriptionAttribute != null)
                    {
                        return descriptionAttribute.Description;
                    }
                }
            }

            return null; // could also return string.Empty
        }

        public static string GenerateCodeProfile(string Perfix, int quytrinhid = 0)
        {
            string date = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            return Perfix + "-" + date;
        }
        #endregion

        public static bool UnorderedEqual<T>(ICollection<T> a, ICollection<T> b)
        {
            // 1
            // Require that the counts are equal
            if (a.Count != b.Count)
            {
                return false;
            }
            // 2
            // Initialize new Dictionary of the type
            Dictionary<T, int> d = new Dictionary<T, int>();
            // 3
            // Add each key's frequency from collection A to the Dictionary
            foreach (T item in a)
            {
                int c;
                if (d.TryGetValue(item, out c))
                {
                    d[item] = c + 1;
                }
                else
                {
                    d.Add(item, 1);
                }
            }
            // 4
            // Add each key's frequency from collection B to the Dictionary
            // Return early if we detect a mismatch
            foreach (T item in b)
            {
                int c;
                if (d.TryGetValue(item, out c))
                {
                    if (c == 0)
                    {
                        return false;
                    }
                    else
                    {
                        d[item] = c - 1;
                    }
                }
                else
                {
                    // Not in dictionary
                    return false;
                }
            }
            // 5
            // Verify that all frequencies are zero
            foreach (int v in d.Values)
            {
                if (v != 0)
                {
                    return false;
                }
            }
            // 6
            // We know the collections are equal
            return true;
        }
    }
    public class CommonDDL
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
