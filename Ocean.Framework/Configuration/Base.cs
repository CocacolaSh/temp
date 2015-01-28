using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Common;

namespace Ocean.Framework.Configuration
{
    public class Base<T>
    {
        private static readonly object MLockHelper = new object();

        /// <summary>
        /// 反序列化文件
        /// </summary>
        /// <param name="fileOldChange"></param>
        /// <param name="configFilePath"></param>
        /// <param name="configInfo"></param>
        /// <returns></returns>
        protected T LoadConfig(ref DateTime fileOldChange, string configFilePath, T configInfo)
        {
            return LoadConfig(ref fileOldChange, configFilePath, configInfo, true);
        }

        /// <summary>
        /// 反序列化文件
        /// </summary>
        /// <param name="fileOldChange"></param>
        /// <param name="configFilePath"></param>
        /// <param name="configInfo"></param>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        protected T LoadConfig(ref DateTime fileOldChange, string configFilePath, T configInfo, bool checkTime)
        {
            if (checkTime)
            {
                DateTime fileNewChange = System.IO.File.GetLastWriteTime(configFilePath);

                if (fileOldChange != fileNewChange)
                {
                    fileOldChange = fileNewChange;

                    lock (MLockHelper)
                    {
                        configInfo = Serializer.Load<T>(configFilePath);
                    }
                }
            }
            else
            {
                lock (MLockHelper)
                {
                    configInfo = Serializer.Load<T>(configFilePath);
                }
            }

            return configInfo;
        }

        /// <summary>
        /// 序列化文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="configFilePath"></param>
        protected void Save(object obj, string configFilePath)
        {
            Serializer.Save(obj, configFilePath);
        }
    }
}