using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Common;

namespace Ocean.Framework.Configuration.global.config
{
    public class GlobalConfig
    {
        private static ConfigFileManager<SerializableStringDictionary> _configFileManager = null;

        /// <summary>
        /// 初始化
        /// </summary>
        static GlobalConfig()
        {
            GetConfig();
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <returns></returns>
        public static SerializableStringDictionary GetConfig()
        {
            if (_configFileManager == null)
            {
                _configFileManager = new ConfigFileManager<SerializableStringDictionary>("/config/global.config");
            }

            return _configFileManager.LoadConfig();
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static void SaveConfig(SerializableStringDictionary info)
        {
            _configFileManager.ConfigInfo = info;
            _configFileManager.SaveConfig();
        }
    }
}
