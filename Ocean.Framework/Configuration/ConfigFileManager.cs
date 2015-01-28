using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Utility;

namespace Ocean.Framework.Configuration
{
    /// <summary>
    /// 通用配置文件管理
    /// </summary>
    public class ConfigFileManager<T> : Base<T>, IConfig<T> where T : new()
    {
        private static DateTime _configFileLastWriteTime;

        /// <summary>
        /// 文件路径
        /// </summary>
        private static string _configFilePath;

        /// <summary>
        /// 通用配置文件管理
        /// </summary>
        /// <param name="configFilePath">配置文件</param>
        public ConfigFileManager(string configFilePath)
        {
            if (_configFilePath == null)
            {
                if (string.IsNullOrEmpty(configFilePath))
                {
                    throw new Exception("");
                }

                this.Initialize(configFilePath);
                this.LoadNewConfig();
                _configFileLastWriteTime = System.IO.File.GetLastWriteTime(_configFilePath);
            }
        }

        /// <summary>
        /// 配置文件信息
        /// </summary>
        public T ConfigInfo { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="configFilePath"></param>
        private void Initialize(string configFilePath)
        {
            if (_configFilePath == null)
            {
                _configFilePath = FileHelper.GetMapPath(configFilePath);

                if (!System.IO.File.Exists(_configFilePath))
                {
                    if (System.Web.HttpContext.Current == null)
                    {
                        _configFilePath = "";
                    }
                    else
                    {
                        throw new Exception(string.Format("请确定网站下是否存在[/{0}]文件.", configFilePath));
                    }
                }
            }
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        public string GetConfigFilePath
        {
            get
            {
                return _configFilePath;
            }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        public T LoadConfig()
        {
            if (!string.IsNullOrEmpty(this.GetConfigFilePath))
            {
                this.ConfigInfo = base.LoadConfig(ref _configFileLastWriteTime, this.GetConfigFilePath, this.ConfigInfo);
            }

            return this.ConfigInfo;
        }

        /// <summary>
        /// 获取最新的配置文件
        /// </summary>
        /// <returns></returns>
        public T LoadNewConfig()
        {
            if (!string.IsNullOrEmpty(this.GetConfigFilePath))
            {
                this.ConfigInfo = base.LoadConfig(ref _configFileLastWriteTime, this.GetConfigFilePath, this.ConfigInfo, false);
            }

            return this.ConfigInfo;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        public void SaveConfig()
        {
            base.Save(this.ConfigInfo, _configFilePath);
        }
    }
}
