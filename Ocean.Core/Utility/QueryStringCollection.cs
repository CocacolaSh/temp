using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ComponentModel;
using Ocean.Core.Common;

namespace Ocean.Core.Utility
{
    public class QueryStringCollection : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _queryStringDictionary;

        #region public QueryStringCollection(HttpRequestBase httpRequestBase)
        public QueryStringCollection(HttpRequestBase httpRequestBase)
        {
            if (httpRequestBase == null)
            {
                throw new ArgumentNullException("HttpRequestBase");
            }

            _queryStringDictionary = new Dictionary<string, string>();

            //首先处理get方式的参数
            var queryGet = httpRequestBase.QueryString;

            foreach (string key in queryGet)
            {
                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(queryGet[key]))
                {
                    _queryStringDictionary[key] = queryGet[key];
                }
            }

            //随后处理post方式的参数
            var queryPost = httpRequestBase.Form;

            foreach (string key in queryPost)
            {
                if (!string.IsNullOrWhiteSpace(key))// && !string.IsNullOrWhiteSpace(queryGet[key])
                {
                    //if (!_queryStringDictionary.ContainsKey("key"))
                    //{
                        _queryStringDictionary[key] = queryPost[key];
                    //}
                }
            }
        }
        #endregion

        #region 把参数值转换成指定类型
        /// <summary>
        /// 把参数值转换成指定类型
        /// </summary>
        public bool TryGetAndConvert<T>(string key, out T value)
        {
            value = default(T);
            string valueStr;
            var type = typeof(T);

            if (_queryStringDictionary.TryGetValue(key, out valueStr))
            {
                if (String.IsNullOrEmpty(valueStr))
                {
                    if (type.IsValueType && !IsNullableType(type))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                var converter = TypeDescriptor.GetConverter(typeof(T));

                if (converter == null || !converter.IsValid(valueStr))
                {
                    return false;
                }

                value = (T)converter.ConvertFromString(valueStr);
            }

            return true;
        }


        private static bool IsNullableType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
        #endregion

        #region 获取参数总数
        /// <summary>
        /// 获取参数总数
        /// </summary>
        public int Count { get { return _queryStringDictionary.Count; } }
        #endregion

        #region 获取指定键值（string）
        /// <summary>
        /// 获取指定键值（string）
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                string value = null;

                if (_queryStringDictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                return string.Empty;
            }
        }
        /// <summary>
        /// 获取指定键值（string）
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string this[string key,string val]
        {
            get
            {
                string value = null;

                if (_queryStringDictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                return val;
            }
        }
        #endregion

        #region 获取指定键值（int）
        /// <summary>
        /// 获取指定键值（int）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public int this[string key, int defaultValue]
        {
            get
            {
                string value = null;

                if (_queryStringDictionary.TryGetValue(key, out value))
                {
                    if (StringValidate.IsNumber(value))
                    {
                        return Int32.Parse(value);
                    }
                }

                return defaultValue;
            }
        }
        #endregion

        #region 获取指定键值（bool）
        /// <summary>
        /// 获取指定键值（bool）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public bool this[string key, bool defaultValue]
        {
            get
            {
                string value = null;

                if (_queryStringDictionary.TryGetValue(key, out value))
                {
                    bool value2;

                    if (bool.TryParse(value, out value2))
                    {
                        return value2;
                    }
                }

                return defaultValue;
            }
        }
        #endregion

        #region 获取指定键值（Datetime）
        /// <summary>
        /// 获取指定键值（int）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public DateTime this[string key, DateTime defaultValue]
        {
            get
            {
                string value = null;

                if (_queryStringDictionary.TryGetValue(key, out value))
                {
                    return TypeConverter.StrToDateTime(value, defaultValue);
                }

                return defaultValue;
            }
        }
        #endregion

        #region public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _queryStringDictionary.GetEnumerator();
        }
        #endregion

        #region System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _queryStringDictionary.GetEnumerator();
        }
        #endregion
    }
}
