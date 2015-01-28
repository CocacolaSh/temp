using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.Configuration
{
    public interface IConfig<T>
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        string GetConfigFilePath { get; }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        void SaveConfig();

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        T LoadConfig();
    }
}
