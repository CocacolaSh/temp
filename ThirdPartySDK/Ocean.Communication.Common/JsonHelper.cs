using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace Ocean.Communication.Common
{
    public static class JsonHelper
    {
        #region Json数据类型
        /// <summary>
        /// Json数据类型
        /// </summary>
        private enum JsonPropertyType
        {
            String,
            Model,
            Array,
            DataTable,
            IDataReader,
            Null
        }
        #endregion

        #region 解析json(value格式String,Model,List<T>,ArrayList,DataTalbe,IDataReader)
        /// <summary>
        /// 解析json(value格式String,Model,List<T>,ArrayList,DataTalbe,IDataReader)
        /// </summary>
        /// <typeparam name="T">对应的实体类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="code">编码</param>
        /// <param name="dataExt">扩展数据</param>
        /// <param name="key">需要的属性，没有则返回全部</param>
        /// <returns></returns>
        public static string AnalysisJson<T>(object value, object code, object dataExt, params string[] key) where T : class
        {
            List<T> listData = new List<T>();
            JsonPropertyType _type;

            if (value is String)
            {
                _type = JsonPropertyType.String;
            }
            else if (value is DataTable)
            {
                _type = JsonPropertyType.DataTable;
            }
            else if (value is IDataReader)
            {
                _type = JsonPropertyType.IDataReader;
            }
            else if (value is T)
            {
                _type = JsonPropertyType.Model;
            }
            else if (value is List<T>)
            {
                _type = JsonPropertyType.Array;
                listData = (List<T>)value;
            }
            else if (value is ArrayList)
            {
                _type = JsonPropertyType.Array;

                foreach (T model in (ArrayList)value)
                {
                    listData.Add(model);
                }
            }
            else if (value == null || value == DBNull.Value)
            {
                _type = JsonPropertyType.Null;
            }
            else
            {
                throw new ArgumentException("暂未支持解析当前value类型的对象!");
            }

            //构造json对象
            JsonObject json = new JsonObject((a) =>
            {
                a["data"] = new JsonProperty(new JsonObject((b) =>
                {
                    //处理String
                    if (_type == JsonPropertyType.String)
                    {
                        b["model"] = new JsonProperty(value.ToString());
                    }
                    //处理单个实体
                    if (_type == JsonPropertyType.Model)
                    {
                        b["model"] = new JsonProperty();

                        b["model"].Add(new JsonObject((c) =>
                        {
                            System.Reflection.PropertyInfo[] arrPropertyInfo = typeof(T).GetProperties();

                            foreach (System.Reflection.PropertyInfo propertyInfo in arrPropertyInfo)
                            {
                                if (key == null || key.Length == 0)
                                {
                                    if (propertyInfo.CanWrite && !string.Equals(propertyInfo.Name, "PrimaryKey", StringComparison.OrdinalIgnoreCase) && !string.Equals(propertyInfo.Name, "TableName", StringComparison.OrdinalIgnoreCase))
                                    {
                                        c[propertyInfo.Name] = new JsonProperty(propertyInfo.GetValue(value, null));
                                    }
                                }
                                else
                                {
                                    if (Array.IndexOf(key, propertyInfo.Name) > -1)
                                    {
                                        c[propertyInfo.Name] = new JsonProperty(propertyInfo.GetValue(value, null));
                                    }
                                }
                            }
                        }));
                    }
                    //处理List<T>和ArrayList
                    if (_type == JsonPropertyType.Array)
                    {
                        if (listData.Count == 0)
                        {
                            b["model"] = new JsonProperty(string.Empty);
                        }
                        else
                        {
                            b["model"] = new JsonProperty();

                            foreach (T model in listData)
                            {
                                b["model"].Add(new JsonObject((c) =>
                                {
                                    System.Reflection.PropertyInfo[] arrPropertyInfo = typeof(T).GetProperties();

                                    foreach (System.Reflection.PropertyInfo propertyInfo in arrPropertyInfo)
                                    {
                                        if (key == null || key.Length == 0)
                                        {
                                            if (propertyInfo.CanWrite && !string.Equals(propertyInfo.Name, "PrimaryKey", StringComparison.OrdinalIgnoreCase) && !string.Equals(propertyInfo.Name, "TableName", StringComparison.OrdinalIgnoreCase))
                                            {
                                                c[propertyInfo.Name] = new JsonProperty(propertyInfo.GetValue(model, null));
                                            }
                                        }
                                        else
                                        {
                                            if (Array.IndexOf(key, propertyInfo.Name) > -1)
                                            {
                                                c[propertyInfo.Name] = new JsonProperty(propertyInfo.GetValue(model, null));
                                            }
                                        }
                                    }
                                }));
                            }
                        }
                    }
                    //处理DataTable
                    if (_type == JsonPropertyType.DataTable)
                    {
                        DataTable dt = (DataTable)value;

                        if (dt.Rows.Count == 0)
                        {
                            b["model"] = new JsonProperty(string.Empty);
                        }
                        else
                        {
                            b["model"] = new JsonProperty();

                            foreach (DataRow dr in dt.Rows)
                            {
                                b["model"].Add(new JsonObject((c) =>
                                {
                                    DataColumnCollection columns = dt.Columns;

                                    for (int i = 0; i < columns.Count; i++)
                                    {
                                        if (key == null || key.Length == 0)
                                        {
                                            if (dr[columns[i]] != null && dr[columns[i]] != DBNull.Value)
                                                c[columns[i].ColumnName] = new JsonProperty(dr[columns[i].ColumnName]);
                                            else
                                                c[columns[i].ColumnName] = new JsonProperty(string.Empty);
                                        }
                                        else
                                        {
                                            if (Array.IndexOf(key, columns[i].ColumnName) > -1)
                                            {
                                                if (dr[columns[i]] != null && dr[columns[i]] != DBNull.Value)
                                                    c[columns[i].ColumnName] = new JsonProperty(dr[columns[i].ColumnName]);
                                                else
                                                    c[columns[i].ColumnName] = new JsonProperty(string.Empty);
                                            }
                                        }
                                    }
                                }));
                            }
                        }
                    }
                    //处理IDataReader
                    if (_type == JsonPropertyType.IDataReader)
                    {
                        b["model"] = new JsonProperty();
                        int nIndex = 0;
                        IDataReader dataReader = (IDataReader)value;

                        using (dataReader)
                        {
                            while (dataReader.Read())
                            {
                                nIndex++;

                                b["model"].Add(new JsonObject((c) =>
                                {
                                    for (int i = 0; i < dataReader.FieldCount; i++)
                                    {
                                        if (key == null || key.Length == 0)
                                        {
                                            if (dataReader[i] != null && dataReader[i] != DBNull.Value)
                                                c[dataReader.GetName(i)] = new JsonProperty(dataReader[i]);
                                            else
                                                c[dataReader.GetName(i)] = new JsonProperty(string.Empty);
                                        }
                                        else
                                        {
                                            if (Array.IndexOf(key, dataReader.GetName(i)) > -1)
                                            {
                                                if (dataReader[i] != null && dataReader[i] != DBNull.Value)
                                                    c[dataReader.GetName(i)] = new JsonProperty(dataReader[i]);
                                                else
                                                    c[dataReader.GetName(i)] = new JsonProperty(string.Empty);
                                            }
                                        }
                                    }
                                }));
                            }
                        }
                        if (nIndex == 0)
                        {
                            b["model"] = new JsonProperty(string.Empty);
                        }
                    }
                    //处理空值
                    if (_type == JsonPropertyType.Null)
                    {
                        b["model"] = new JsonProperty(string.Empty);
                    }

                    b["code"] = new JsonProperty(code);
                }));

                a["dataExt"] = new JsonProperty(dataExt);
            });

            return json.ToString();
        }
        #endregion

        #region 返回分页数据
        /// <summary>
        /// 返回分页数据
        /// </summary>
        /// <param name="size"></param>
        /// <param name="currrent"></param>
        /// <param name="count"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string PagingData(int size, int currrent, int count, params string[] key)
        {
            //构造json对象
            JsonObject json = new JsonObject((a) =>
            {
                a["size"] = new JsonProperty(size);
                a["currrent"] = new JsonProperty(currrent);
                a["count"] = new JsonProperty(count);
                for (int i = 0; i < key.Length; i++)
                {
                    a["@p" + i] = new JsonProperty(key[i]);
                }
            });
            return json.ToString();
        }
        #endregion

        #region 解析Json对象数组
        /// <summary>
        /// 解析Json对象数组
        /// </summary>
        /// <returns></returns>
        public static string AnalysisJsons(IList<JsonObject> list, bool hadTopNode = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            foreach (JsonObject item in list)
            {
                sb.Append(item.ToString());
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            if (hadTopNode)
            {
                JsonObject jsonObject = new JsonObject();
                jsonObject["id"] = new JsonProperty(Guid.Empty.ToString());
                jsonObject["text"] = new JsonProperty("--请选择--");
                
                if (list.Count > 0)
                {
                    jsonObject["children"] = new JsonProperty(sb.ToString());
                }

                StringBuilder sbTop = new StringBuilder();
                sbTop.Append("[");
                sbTop.Append(jsonObject.ToString());
                sbTop.Append("]");
                return sbTop.ToString();
            }
            else
            {
                return sb.ToString();
            }
        }
        #endregion
    }
}