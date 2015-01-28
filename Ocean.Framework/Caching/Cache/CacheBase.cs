using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ocean.Core.Caching;

namespace Ocean.Framework.Caching.Cache
{
    public class CacheBase
    {
        private readonly ICacheManager _cacheManager;

        public CacheBase()
        {
            _cacheManager = new MemoryCacheManager();
        }

        #region 锁对象
        /// <summary> 
        /// 锁对象
        /// </summary>
        private static object objLock = new object();
        #endregion

        #region 缓存管理实例队列
        /// <summary>
        /// 缓存管理实例队列
        /// </summary>
        private static Hashtable htInstance = Hashtable.Synchronized(new Hashtable());
        #endregion

        #region 获取实例
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static T GetInstance<T>()
        {
            Type type = typeof(T);

            if (htInstance[type] == null)
            {
                lock (objLock)
                {
                    if (htInstance[type] == null)
                    {
                        htInstance[type] = Activator.CreateInstance(type);
                    }
                }
            }

            return (T)htInstance[type];
        }
        #endregion

        #region 获取当前实例缓存
        /// <summary>
        /// 获取当前实例缓存
        /// </summary>
        /// <returns></returns>
        protected object GetCache()
        {
            return _cacheManager.Get<object>(this.GetType().FullName);
        }
        #endregion

        #region 设置当前实例缓存
        /// <summary>
        /// 设置当前实例缓存
        /// </summary>
        /// <param name="obj"></param>
        protected void SetCache(object obj)
        {
            _cacheManager.Set(this.GetType().FullName, obj, 99999);
        }
        #endregion

        #region 移除当前实例缓存
        /// <summary>
        /// 移除当前实例缓存
        /// </summary>
        public void RemoveCache()
        {
            _cacheManager.Remove(this.GetType().FullName);
        }
        #endregion
    }
}
