using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Ocean.Core.Enums;

namespace Ocean.Core
{
    public static class EnumExtensions
    {
        #region 枚举描述
        private static Dictionary<Enum, string> dictDiscs = new Dictionary<Enum, string>();
        /// <summary>
        /// 获取当前枚举值的描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string Discription(this Enum enumType)
        {
            string strDisc = string.Empty;
            if (dictDiscs.ContainsKey(enumType))
            {
                strDisc = dictDiscs[enumType];
            }
            else
            {
                strDisc = GetDiscription(enumType);
                dictDiscs.Add(enumType, strDisc);
            }
            return strDisc;
        }
        private static string GetDiscription(Enum enumType)
        {
            FieldInfo fieldInfo = enumType.GetType().GetField(enumType.ToString());
            object[] attrs = fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                EnumDescriptionAttribute desc = attrs[0] as EnumDescriptionAttribute;
                if (desc != null)
                {
                    return desc.Description;
                }
            }
            return enumType.ToString();
        }
        #endregion

        #region 枚举列表
        ///// <summary>
        ///// 将当前枚举类型转换为IList&lt;EnumItem>
        ///// </summary>
        ///// <param name="_enum">枚举类型</param>
        ///// <returns></returns>
        //public static IList<EnumItem> ToListEnumItem(this Enum _enum)
        //{
        //    return ToListEnumItem(_enum, null);
        //}
        ///// <summary>
        ///// 将当前枚举类型转换为IList&lt;EnumItem>
        ///// </summary>
        ///// <param name="_enum">枚举类型</param>
        ///// <param name="enumItem">默认第一项的设置[如：请选择xxx]</param>
        ///// <returns></returns>
        //public static IList<EnumItem> ToListEnumItem(this Enum _enum, EnumItem enumItem)
        //{
        //    return PrivateToListEnumItem(_enum, enumItem);
        //}
        //public static List<EnumItem> PrivateToListEnumItem(Enum _enu,EnumItem enumItem)
        //{
        //    List<EnumItem> enumList = new List<EnumItem>();
        //    Type enumType = _enu.GetType();

        //    FieldInfo[] fieldInfos = enumType.GetFields();
        //    foreach (FieldInfo fieldInfo in fieldInfos)
        //    {
        //        EnumItem item = new EnumItem();
        //        // 过滤掉一个不是枚举值的，记录的是枚举的源类型
        //        if (fieldInfo.FieldType.IsEnum == false)
        //            continue;
        //        // 通过字段的名字得到枚举的值
        //        item.EnumValue = ((int)enumType.InvokeMember(fieldInfo.Name, BindingFlags.GetField, null, null, null)).ToString();
        //        item.EnumKey = fieldInfo.Name;
        //        object[] attrs = fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), true);
        //        if (attrs != null && attrs.Length > 0)
        //        {
        //            EnumDescriptionAttribute desc = attrs[0] as EnumDescriptionAttribute;
        //            if (desc != null)
        //            {
        //                item.EnumDescript = desc.Description;
        //            }
        //        }
        //        enumList.Add(item);
        //    }
        //    if (enumItem != null)
        //    {
        //        enumList.Insert(0, enumItem);
        //    }
        //    return enumList;
        //}
        /// <summary>
        /// 将当前枚举类型转换为IList&lt;EnumItem>-静态的方法调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItem"></param>
        /// <returns></returns>
        public static List<EnumItem> ToListEnumItem<T>(EnumItem enumItem)
        {
            List<EnumItem> enumList = new List<EnumItem>();
            Type enumType = typeof(T);

            FieldInfo[] fieldInfos = enumType.GetFields();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                EnumItem item = new EnumItem();
                // 过滤掉一个不是枚举值的，记录的是枚举的源类型
                if (fieldInfo.FieldType.IsEnum == false)
                    continue;
                // 通过字段的名字得到枚举的值
                item.EnumValue = ((int)enumType.InvokeMember(fieldInfo.Name, BindingFlags.GetField, null, null, null)).ToString();
                item.EnumKey = fieldInfo.Name;
                object[] attrs = fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    EnumDescriptionAttribute desc = attrs[0] as EnumDescriptionAttribute;
                    if (desc != null)
                    {
                        item.EnumDescript = desc.Description;
                    }
                }
                enumList.Add(item);
            }
            if (enumItem != null)
            {
                enumList.Insert(0, enumItem);
            }
            return enumList;
        }
        /// <summary>
        /// 将当前枚举类型转换为IList&lt;EnumItem>-静态的方法调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItem"></param>
        /// <returns></returns>
        public static List<EnumItem> ToListEnumItem<T>()
        {
            return ToListEnumItem<T>(null);
        }
        /// <summary>
        /// 将当前枚举类型转换为IEnumerable&lt;EnumItem>-静态的方法调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItem"></param>
        /// <returns></returns>
        public static IEnumerable<EnumItem> AsEnumerable<T>(EnumItem enumItem)
        {
            bool flag = true;
            Type enumType = typeof(T);
            FieldInfo[] fieldInfos = enumType.GetFields();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (enumItem != null && flag)
                {
                    flag = false;
                    yield return enumItem;
                }
                EnumItem item = new EnumItem();
                // 过滤掉一个不是枚举值的，记录的是枚举的源类型
                if (fieldInfo.FieldType.IsEnum == false)
                    continue;
                // 通过字段的名字得到枚举的值
                item.EnumValue = ((int)enumType.InvokeMember(fieldInfo.Name, BindingFlags.GetField, null, null, null)).ToString();
                item.EnumKey = fieldInfo.Name;
                object[] attrs = fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    EnumDescriptionAttribute desc = attrs[0] as EnumDescriptionAttribute;
                    if (desc != null)
                    {
                        item.EnumDescript = desc.Description;
                    }
                }
                yield return item;
            }
        }
        #endregion

        #region 相应的字符串获取枚举
        /// <summary>
        /// 根据Enum 的value值返回Enum 的key
        /// </summary>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetItemKeyByValue(this List<EnumItem> items, object value)
        {
            string strKey = String.Empty;
            foreach (EnumItem item in items)
            {
                if (item.EnumKey.Equals(value.ToString()))
                {
                    strKey = item.EnumValue;
                    break;
                }
            }
            return strKey;
        }

        /// <summary>
        /// 根据Enum 的value值返回Enum 的Descript
        /// </summary>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetItemDesByValue(this List<EnumItem> items, object value)
        {
            string strDes = String.Empty;
            foreach (EnumItem item in items)
            {
                if (item.EnumValue.Equals(value.ToString()))
                {
                    strDes = item.EnumDescript;
                    break;
                }
            }
            return strDes;
        }

        /// <summary>
        /// 根据Enum 的key值返回Enum 的Descript
        /// </summary>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetItemDesByKey(this List<EnumItem> items, string key)
        {
            string strDes = String.Empty;
            foreach (EnumItem item in items)
            {
                if (item.EnumKey.Equals(key))
                {
                    strDes = item.EnumDescript;
                    break;
                }
            }
            return strDes;
        }
        #endregion
    }
}
