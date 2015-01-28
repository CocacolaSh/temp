using System.Collections.Generic;
using System.Linq;
using Ocean.Core.Infrastructure;
using Ocean.Core.Logging;
using Ocean.Entity;
using Ocean.Services;

namespace Ocean.Framework.Caching.Cache
{
    public class AdminPermissionsCache : CacheBase
    {
        private readonly IPermissionModuleCodeService _permissionModuleCodeService;

        public AdminPermissionsCache()
        {
            _permissionModuleCodeService = EngineContext.Current.Resolve<IPermissionModuleCodeService>();
        }

        /// <summary>
        /// 锁对象
        /// </summary>
        private static readonly object objLock = new object();

        private static AdminPermissionsCache _instance;

        #region 实例
        /// <summary>
        /// 实例
        /// </summary>
        public static AdminPermissionsCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (objLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AdminPermissionsCache();
                        }
                    }
                }

                return _instance;
            }
        }
        #endregion

        #region 管理权限数据
        /// <summary>
        /// 管理权限数据
        /// </summary>
        public List<PermissionModuleCode> CacheData
        {
            get
            {
                if (GetCache() == null)
                {
                    lock (objLock)
                    {
                        if (GetCache() == null)
                        {
                            List<PermissionModuleCode> list = _permissionModuleCodeService.GetALL("CreateDate", true).ToList();
                            SetCache(list);
                        }
                    }
                }

                if (GetCache() == null)
                {
                    Log4NetImpl.Write("获取权限缓存发生异常-----------", Log4NetImpl.ErrorLevel.Info);
                    return _permissionModuleCodeService.GetALL().ToList();
                }
                else
                {
                    return (List<PermissionModuleCode>)GetCache();
                }
            }
        }
        #endregion
    }
}