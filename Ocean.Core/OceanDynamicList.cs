using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace Ocean.Core
{
    public class OceanDynamicList<T>:IEnumerable
    {
        private IList<T> dynamicList;
        public IList<T> DynamicList
        {
            get
            {
                return dynamicList;
            }
        }
        private string[] columns;
        /// <summary>
        /// 过滤不返回Json的列
        /// </summary>
        public string[] FilterJsonColumns{get;set;}
        public OceanDynamicList(IList<T> ilist, string[] _columns)
        {
            dynamicList = ilist;
            columns = _columns;
            PageSize = 10;
            TotalItemCount = ilist.Count;
            TotalPageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
            CurrentPageIndex = 1;
            StartRecordIndex = (CurrentPageIndex - 1) * PageSize + 1;
            EndRecordIndex = TotalItemCount > CurrentPageIndex * PageSize ? CurrentPageIndex * PageSize : TotalItemCount;
        }
        public OceanDynamicList()
        {
            dynamicList = new List<T>();
            columns = new string[] { };
            PageSize = 10;
            TotalItemCount =0;
            TotalPageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
        }
        public OceanDynamicList(IList<T> ilist, string[] _columns, int pageIndex, int pageSize, int totalItemCount)
        {
            dynamicList = ilist;
            columns = _columns;
            TotalItemCount = totalItemCount;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
            StartRecordIndex = (pageIndex - 1) * pageSize + 1;
            EndRecordIndex = TotalItemCount > pageIndex * pageSize ? pageIndex * pageSize : totalItemCount;
        }
        public IEnumerator GetEnumerator()
        {
            return new OceanDynamicEnumerator<T>(dynamicList, columns);
        }
        public int Count
        {
            get
            {
                return dynamicList.Count;
            }
        }
        public dynamic this[int index]
        {
            get
            {
                if (index < 0 || index > dynamicList.Count)
                {
                    return null;
                }
                else
                {
                    return new OceanDynamic(dynamicList[index], columns);
                }
            }
            set
            {
                if (index < 0 || index > dynamicList.Count)
                {

                }
                else
                {
                    dynamicList[index] = value.DynamicColumns;
                }
            }
        }

        public string ToJson()
        {
            JArray jsonArray = new JArray();
            foreach (OceanDynamic d in this)
            {
                var jsonObject = new JObject();
                int i = 0;
                foreach (string propertyN in d.Columns)
                {
                    object[] dynamicColumns = d.DynamicColumns as object[];
                    jsonObject.Add(propertyN, JToken.FromObject(dynamicColumns[i]));
                    i++;
                }
                jsonArray.Add(jsonObject);
            }
            //JavaScriptDateTimeConverter timeConverter = new JavaScriptDateTimeConverter();
            //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式  
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            return "{\"rows\":" + jsonArray.ToString(Formatting.None, timeConverter) + ",\"total\":" + this.TotalItemCount.ToString() + "}";
        }
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get; private set; }
        public int StartRecordIndex { get; private set; }
        public int EndRecordIndex { get; private set; }

        class OceanDynamicEnumerator<T> : IEnumerator
        {
            private IList<T> list;
            private string[] columns;
            private int cur = -1;
            //private string[] FilterJsonColumns;
            public OceanDynamicEnumerator(IList<T> _list, string[] _columns)
            {
                this.list = _list;
                columns = _columns;
            }
            public object Current
            {
                get
                {
                    if (cur < 0 || cur >= list.Count)
                        return null;
                    else
                    {
                        return new OceanDynamic(list[cur], columns);
                    }
                }
            }
            public bool MoveNext()
            {
                cur++;
                return cur < list.Count;
            }

            public void Reset()
            {
                cur = -1;
            }
        }
    }
}
