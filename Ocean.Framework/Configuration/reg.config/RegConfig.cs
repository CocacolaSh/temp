using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.Configuration.reg.config
{
    public class RegConfig
    {
        private static ConfigFileManager<RegConfigInfo> _configFileManager = null;

        /// <summary>
        /// 初始化
        /// </summary>
        static RegConfig()
        {
            GetConfig();
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <returns></returns>
        public static RegConfigInfo GetConfig()
        {
            if (_configFileManager == null)
            {
                _configFileManager = new ConfigFileManager<RegConfigInfo>("/config/reg.config");
            }

            return _configFileManager.LoadConfig();
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static void SaveConfig(RegConfigInfo info)
        {
            _configFileManager.ConfigInfo = info;
            _configFileManager.SaveConfig();
        }
    }
}
