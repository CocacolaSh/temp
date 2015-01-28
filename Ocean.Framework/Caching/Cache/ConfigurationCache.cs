using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Services;
using Ocean.Core.Infrastructure;
using Ocean.Core.Logging;
using Ocean.Entity;

namespace Ocean.Framework.Caching.Cache
{
    public class ConfigurationCache : CacheBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationCache()
        {
            _configurationService = EngineContext.Current.Resolve<IConfigurationService>();
        }

        /// <summary>
        /// 锁对象
        /// </summary>
        private static readonly object objLock = new object();

        #region public static Instance 实例
        /// <summary>
        /// 实例
        /// </summary>
        public static ConfigurationCache Instance
        {
            get
            {
                return GetInstance<ConfigurationCache>();
            }
        }
        #endregion

        #region 配置列表 private List<Ocean.Entity.Configuration> CacheList
        /// <summary>
        /// 配置列表
        /// </summary>
        private List<Ocean.Entity.Configuration> CacheList
        {
            get
            {
                if (GetCache() == null)
                {
                    lock (objLock)
                    {
                        if (GetCache() == null)
                        {
                            List<Ocean.Entity.Configuration> list = _configurationService.GetALL().ToList();
                            SetCache(list);
                        }
                    }
                }

                if (GetCache() == null)
                {
                    Log4NetImpl.Write("获取配置缓存发生异常-----------", Log4NetImpl.ErrorLevel.Info);
                    return _configurationService.GetALL().ToList();
                }
                else
                {
                    return (List<Ocean.Entity.Configuration>)GetCache();
                }
            }
        }
        #endregion

        #region 取得键值
        /// <summary>
        /// 取得键值
        /// </summary>
        public string GetValue(ConfigurationEnum key)
        {
            return GetValue(key.ToString());
        }

        /// <summary>
        /// 取得键值
        /// </summary>
        public string GetValue(string key)
        {
            foreach (Ocean.Entity.Configuration model in CacheList)
            {
                if (model.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return model.Value;
                }
            }

            return string.Empty;
        }
        #endregion
    }
}
