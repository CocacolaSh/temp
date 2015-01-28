using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.Mvc;

namespace Ocean.Framework.Mvc.Extensions
{
    /// <summary>
    /// 辅助获取ViewData内的数据
    /// </summary>
    public static class ViewBagExtension
    {
        /// <summary>
        /// 取得viewdata里的某个值,并且转换成指定的对象类型,如果不是该类型或如果是一个数组类型而元素为0个或没有此key都将返回空,
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewData"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this IDictionary<string, object> viewData, string key)
        {
            if (typeof(T) == typeof(string))
            {
                TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                return Get<T>(viewData, key, (T)conv.ConvertFrom(string.Empty));
            }
            else
                return Get<T>(viewData, key, default(T));
        }

        /// <summary>
        /// 取得viewdata里的某个值,并且转换成指定的对象类型,如果不是该类型或如果是一个数组类型而元素为0个或没有此key都将返回空,
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static T Get<T>(this IDictionary<string, object> viewData, string key, T defaultValue)
        {
            if (viewData.ContainsKey(key))
            {
                object value;
                viewData.TryGetValue(key, out value);

                if (value != null)
                {
                    Type tType = typeof(T);
                    if (tType.IsInterface || tType.IsClass)
                    {
                        if (value is T)
                            return (T)value;
                    }
                    else if (tType.IsGenericType && tType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(tType));
                    }
                    else if (tType.IsEnum)
                    {
                        return (T)Enum.Parse(tType, value.ToString());
                    }
                    else
                    {
                        return (T)Convert.ChangeType(value, tType);
                    }
                }
            }

            return defaultValue;
        }


        /// <summary>
        /// GetString
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static string GetString(this IDictionary<string, object> viewData, string key, string defaultValue)
        {
            string returnValue = defaultValue;

            if (viewData.ContainsKey(key))
            {
                object value = null;
                viewData.TryGetValue(key, out value);
                if (value != null)
                    returnValue = value.ToString();
            }

            return returnValue;
        }

        /// <summary>
        /// GetString 默认值为string.Empty
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        public static string GetString(this IDictionary<string, object> viewData, string key)
        {
            return GetString(viewData, key, string.Empty);
        }

        /// <summary>
        /// GetDateTime
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static DateTime GetDateTime(this IDictionary<string, object> viewData, string key, DateTime defaultValue)
        {
            DateTime returnValue = defaultValue;

            if (viewData.ContainsKey(key))
            {
                object value = null;
                viewData.TryGetValue(key, out value);
                if (value != null)
                {
                    DateTime.TryParse(value.ToString(), out returnValue);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// GetInt
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static int GetInt(this IDictionary<string, object> viewData, string key, int defaultValue)
        {
            int returnValue = defaultValue;

            if (viewData.ContainsKey(key))
            {
                object value = null;
                viewData.TryGetValue(key, out value);
                if (value != null)
                {
                    int.TryParse(value.ToString(), out returnValue);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// GetInt 默认值为0
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        public static int GetInt(this IDictionary<string, object> viewData, string key)
        {
            return GetInt(viewData, key, 0);
        }

        /// <summary>
        /// GetLong
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static long GetLong(this IDictionary<string, object> viewData, string key, long defaultValue)
        {
            long returnValue = defaultValue;

            if (viewData.ContainsKey(key))
            {
                object value = null;
                viewData.TryGetValue(key, out value);

                if (value != null)
                {
                    long.TryParse(value.ToString(), out returnValue);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// GetDouble
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static double GetDouble(this IDictionary<string, object> viewData, string key, double defaultValue)
        {
            double returnValue = defaultValue;

            if (viewData.ContainsKey(key))
            {
                object value = null;
                viewData.TryGetValue(key, out value);

                if (value != null)
                {
                    double.TryParse(value.ToString(), out returnValue);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// GetFloat
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static float GetFloat(this IDictionary<string, object> viewData, string key, float defaultValue)
        {
            float returnValue = defaultValue;

            if (viewData.ContainsKey(key))
            {
                object value = null;
                viewData.TryGetValue(key, out value);

                if (value != null)
                {
                    float.TryParse(value.ToString(), out returnValue);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// GetBool
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static bool GetBool(this IDictionary<string, object> viewData, string key, bool defaultValue)
        {
            bool returnValue = defaultValue;

            if (viewData.ContainsKey(key))
            {
                object value = null;
                viewData.TryGetValue(key, out value);

                if (value != null)
                {
                    bool.TryParse(value.ToString(), out returnValue);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// GetDecimal
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">如果未找到则返回该默认值</param>
        public static string DecimalToString(this IDictionary<string, object> viewData, string key)
        {
            if (viewData.ContainsKey(key))
            {
                object value = null;
                viewData.TryGetValue(key, out value);

                if (value != null)
                {
                    string[] decimalStr = value.ToString().Split('.');
                    if (!string.IsNullOrEmpty(decimalStr[1].TrimEnd('0')))
                    {
                        return decimalStr[0] + "." + decimalStr[1].TrimEnd('0');
                    }
                    else
                    {
                        return decimalStr[0];
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// GetBool 默认值为false
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <param name="key">key</param>
        public static bool GetBool(this IDictionary<string, object> viewData, string key)
        {
            return GetBool(viewData, key, false);
        }
    }
}
