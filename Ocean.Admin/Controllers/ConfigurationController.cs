using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Core;
using Ocean.Entity;
using Ocean.Services;
using Ocean.Entity.DTO;
using Ocean.Core.Utility;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Framework.Caching.Cache;
using Ocean.Core.Logging;

namespace Ocean.Admin.Controllers
{
    public class ConfigurationController : AdminBaseController
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }

        /// <summary>
        /// 初始化SafeIPEdit页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SafeIPEdit()
        {
            if (!base.HasPermission("safeip", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            ViewBag.SafeIP = ConfigurationCache.Instance.GetValue(ConfigurationEnum.Config_SafeIP);
            return View();
        }

        /// <summary>
        /// 编辑安全IP
        /// </summary>
        [HttpPost]
        [ActionName("_SafeIPEdit")]
        public ActionResult SafeIPEditProvide()
        {
            List<string> list = new List<string>();
            list.Add("安全IP|" + ConfigurationEnum.Config_SafeIP + "|" + RQuery["SafeIP"]);

            //事务 
            try
            {
                foreach (string config in list)
                {
                    string[] arrConfig = config.Split('|');

                    if (arrConfig.Length != 3)
                    {
                        return JsonMessage(false, "格式错误");
                    }

                    _configurationService.UpdateConfiguration(arrConfig[1], arrConfig[2].Trim());
                }

                //更新配置缓存
                ConfigurationCache.Instance.RemoveCache();
                base.AddLog(string.Format("更新安全IP成功"), AdminLoggerModuleEnum.Configuration);
                return JsonMessage(true, "保存成功");
            }
            catch (Exception ex)
            {
                Log4NetImpl.Write(ex.Message, Log4NetImpl.ErrorLevel.Error);
                return JsonMessage(false, ex.Message);
            }
        }

        /// <summary>
        /// 更新配置缓存
        /// </summary>
        [HttpPost]
        [ActionName("_UpdateConfigurationCache")]
        public ActionResult UpdateConfigurationCache()
        {
            ConfigurationCache.Instance.RemoveCache();
            return JsonMessage(true, "更新配置缓存成功");
        }
    }
}