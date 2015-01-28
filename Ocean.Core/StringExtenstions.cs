using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Utility;
using Ocean.Core.Enums;

namespace Ocean.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// 字符串转换为枚举类型
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumStr">枚举字符串</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string enumStr)
        {
            T enumT = default(T);
            if (Enum.IsDefined(typeof(T), enumStr))
            {
                enumT = (T)Enum.Parse(typeof(T), enumStr);
            }
            return enumT;
        }
        public static T DescriptionToEnum<T>(this string desStr)
        {
             T enumT = default(T);
            IList<EnumItem> items = EnumExtensions.ToListEnumItem<T>();
            EnumItem item = items.Where(e => e.EnumDescript == desStr).FirstOrDefault();
            if (item != null && !string.IsNullOrEmpty(item.EnumDescript))
            {
                if (Enum.IsDefined(typeof(T), item.EnumKey))
                {
                    enumT = (T)Enum.Parse(typeof(T), item.EnumKey);
                }
            }
            return enumT;
            
        }
        /// <summary>
        /// 数组转化为用“,”拼接的字符串
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ArrayToString<T>(this T[] source)
        {
            return string.Join<T>(",", source);
        }
        /// <summary>
        /// 用“,”字符串转化为整型数组
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int[] ToIntArray(this string source)
        {
            if (!string.IsNullOrEmpty(source) && StringHelper.IsNumericList(source))
            {
                string[] newvalue = source.Split(',');
                int[] t = new int[newvalue.Length];
                for (int index = 0; index < newvalue.Length; index++)
                {
                    t[index] = TypeConverter.StrToInt(newvalue[index]);
                }
                return t;
            }
            return new int[0];
        }

        public static string ToDecimalString(this string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                string[] decimalStr = source.ToString().Split('.');
                if (!string.IsNullOrEmpty(decimalStr[1].TrimEnd('0')))
                {
                    return decimalStr[0] + "." + decimalStr[1].TrimEnd('0');
                }
                else
                {
                    return decimalStr[0];
                }
            }
            return "";
        }
    }
}
