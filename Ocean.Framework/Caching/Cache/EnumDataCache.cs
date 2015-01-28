using System;
using System.Collections.Generic;
using Ocean.Core.Logging;
using Ocean.Core.Infrastructure;
using Ocean.Entity;
using Ocean.Entity.Enums.TypeIdentifying;
using System.Linq;
using Ocean.Services;

namespace Ocean.Framework.Caching.Cache
{
    public class EnumDataCache : CacheBase
    {
        private readonly IEnumTypeService _enumTypeService;
        private readonly IEnumDataService _enumDataService;

        public EnumDataCache()
        {
            this._enumTypeService = EngineContext.Current.Resolve<IEnumTypeService>();
            this._enumDataService = EngineContext.Current.Resolve<IEnumDataService>();
        }

        /// <summary>
        /// 锁对象
        /// </summary>
        private static readonly object objLock = new object();

        #region public static CertificateCache Instance 实例
        /// <summary>
        /// 实例
        /// </summary>
        public static EnumDataCache Instance
        {
            get
            {
                return GetInstance<EnumDataCache>();
            }
        }
        #endregion

        #region 枚举数据
        /// <summary>
        /// 枚举数据
        /// </summary>
        private List<EnumData> CacheList
        {
            get
            {
                if (GetCache() == null)
                {
                    lock (objLock)
                    {
                        if (GetCache() == null)
                        {
                            IList<EnumData> list = _enumDataService.GetALL("Sort", true).ToList();
                            SetCache(list);
                        }
                    }
                }

                if (GetCache() == null)
                {
                    Log4NetImpl.Write("获取枚举数据缓存发生异常-----------", Log4NetImpl.ErrorLevel.Info);
                    return _enumDataService.GetALL().ToList();
                }
                else
                {
                    return (List<EnumData>)GetCache();
                }
            }
        }
        #endregion

        #region 根据枚举类型标识读取枚举数据
        /// <summary>
        /// 根据枚举类型标识读取枚举数据
        /// </summary>
        /// <param name="EnumTypeIdentifying">枚举类型标识</param>
        /// <returns>枚举数据列表</returns>
        public List<EnumData> GetList(TypeIdentifyingEnum enumTypeIdentifying)
        {
            string identifying = enumTypeIdentifying.ToString();
            List<EnumData> listAll = CacheList;
            List<EnumData> list = new List<EnumData>();

            if (listAll == null || listAll.Count == 0)
            {
                return list;
            }

            foreach (EnumData model in listAll)
            {
                if (string.Equals(model.EnumType.Identifying, identifying, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(model);
                }
            }

            return list;
        }
        #endregion

        #region 根据枚举类型标识和枚举值获取枚举数据名称
        /// <summary>
        /// 根据枚举类型标识和枚举值获取枚举数据名称
        /// </summary>
        /// <param name="enumTypeIdentifying">枚举类型标识</param>
        /// <param name="enumDataValue">枚举值</param>
        /// <returns>枚举数据名称</returns>
        public string GetEnumDataName(TypeIdentifyingEnum enumTypeIdentifying, string enumDataValue)
        {
            string identifying = enumTypeIdentifying.ToString();

            if (String.IsNullOrEmpty(enumDataValue))
            {
                return "";
            }

            List<EnumData> listAllEnumData = CacheList;

            foreach (EnumData model in listAllEnumData)
            {
                if (String.IsNullOrEmpty(model.Value))
                {
                    continue;
                }

                if (string.Equals(model.EnumType.Identifying, identifying, StringComparison.OrdinalIgnoreCase) && 
                    string.Equals(model.Value.Trim(),enumDataValue.Trim(),StringComparison.OrdinalIgnoreCase))
                {
                    return model.Name;
                }
            }

            return "";
        }
        #endregion
    }
}