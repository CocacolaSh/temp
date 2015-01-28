using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ocean.Core.ExceptionHandling;

namespace Ocean.Core
{
    public class OceanDynamic : System.Dynamic.DynamicObject
    {
        #region 另一种解决办法
        //dynamic person = new ExpandoObject();
        //var dict = (IDictionary<String, Object>)person;
        //dict.Add("abcd", "test");
        //Console.WriteLine(person.abcd);
        #endregion

        #region IList
        /// <summary>
        /// object[]|object对象
        /// </summary>
        public object DynamicColumns
        {
            get;
            set;
        }
        /// <summary>
        /// 字段数组
        /// </summary>
        public string[] Columns
        {
            get;
            set;
        }
        public bool IsColumns
        {
            get;
            set;
        }
        /// <summary>
        /// 过滤不返回Json的列
        /// </summary>
        public string[] FilterJsonColumns { get; set; }
        public OceanDynamic(object dynamicList, string[] columns)
        {
            IsColumns = true;
            DynamicColumns = dynamicList;
            Columns = columns;
        }
        #endregion

        private Dictionary<string, object> dic = new Dictionary<string, object>();
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            if (IsColumns)
            {
                object[] dynamicColumns = DynamicColumns as object[];
                if (dynamicColumns == null)
                {
                    throw ExceptionManager.MessageException("value为空或不为object[]对象!");
                }
                int index = Array.IndexOf(Columns, binder.Name);
                if (index >= 0)
                {
                    dynamicColumns[index] = value;
                }
                else
                {
                    throw ExceptionManager.MessageException("不存在此列!");
                }
            }
            else
            {
                if (dic.ContainsKey(binder.Name))
                    dic[binder.Name] = value;
                else
                    dic.Add(binder.Name, value);
                base.TrySetMember(binder, value);
            }
            return true;
        }
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (IsColumns)
            {
                object[] dynamicColumns = DynamicColumns as object[];
                if (dynamicColumns != null)
                {
                    int index = Array.IndexOf(Columns, binder.Name);
                    if (index >= 0)
                    {
                        result = dynamicColumns[index];
                    }
                    else
                    {

                        result = null;
                    }
                }
                else
                {
                    throw ExceptionManager.MessageException("DynamicColumns为空或类型不正确!");
                }
                return true;
            }
            return dic.TryGetValue(binder.Name, out result) || base.TryGetMember(binder, out result);
        }
        public void SetMember(string key, object value)
        {
            if (IsColumns)
            {
                if (Columns == null || DynamicColumns == null)
                {
                    throw ExceptionManager.MessageException("Columns和DynamicColumns不能为空！");
                }
                object[] columns = DynamicColumns as object[];
                if (columns == null)
                {
                    throw ExceptionManager.MessageException("DynamicColumns不能为空！");
                }
                int index = Array.IndexOf(Columns, key);
                if (index >= 0)
                {
                    columns[index] = value;
                }
            }
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }
        //public JsonValue ToJson()
        //{
        //    return JsonUtils.ToJson(DynamicColumns, Columns,FilterJsonColumns);
        //}
    }
}
