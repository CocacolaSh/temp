using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using Ocean.Core.Common;
using Ocean.Core.Utility;

namespace Ocean.Core.Configuration
{
    /// <summary>
    /// 基本设置类
    /// </summary>
    public class BaseConfigs
    {
        private static object lockHelper = new object();

        private static System.Timers.Timer generalConfigTimer = new System.Timers.Timer(15000);

        private static BaseConfigInfo m_configinfo;

        /// <summary>
        /// 静态构造函数初始化相应实例和定时器
        /// </summary>
        static BaseConfigs()
        {

            m_configinfo = BaseConfigFileManager.LoadConfig();

            generalConfigTimer.AutoReset = true;
            generalConfigTimer.Enabled = true;
            generalConfigTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            generalConfigTimer.Start();
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ResetConfig();
        }


        /// <summary>
        /// 重设配置类实例
        /// </summary>
        public static void ResetConfig()
        {
            m_configinfo = BaseConfigFileManager.LoadConfig();
        }

        public static BaseConfigInfo GetConfig()
        {
            return m_configinfo;
        }

        /// <summary>
        /// 获取Lang
        /// </summary>
        /// <returns></returns>
        public static string GetLang()
        {
            return GetConfig().Lang;
        }

        /// <summary>
        /// 获得设禁用IP信息
        /// </summary>
        /// <returns>设置项</returns>
        public static bool SetIpDenyAccess(string denyipaccess)
        {
            bool result;

            lock (lockHelper)
            {
                try
                {
                    BaseConfigInfo configInfo = BaseConfigs.GetConfig();
                    configInfo.IpDenyAccess = configInfo.IpDenyAccess + "\n" + denyipaccess;
                    BaseConfigs.Serialiaze(configInfo, FileHelper.GetMapPath("config/general.config"));
                    result = true;
                }
                catch
                {
                    return false;
                }

            }
            return result;

        }


        /// <summary>
        /// 保存配置类实例
        /// </summary>
        /// <param name="generalconfiginfo"></param>
        /// <returns></returns>
        public static bool SaveConfig(BaseConfigInfo generalconfiginfo)
        {
            BaseConfigFileManager gcf = new BaseConfigFileManager();
            BaseConfigFileManager.ConfigInfo = generalconfiginfo;
            return gcf.SaveConfig();
        }



        #region Helper

        /// <summary>
        /// 序列化配置信息为XML
        /// </summary>
        /// <param name="configinfo">配置信息</param>
        /// <param name="configFilePath">配置文件完整路径</param>
        public static BaseConfigInfo Serialiaze(BaseConfigInfo configinfo, string configFilePath)
        {
            lock (lockHelper)
            {
                Serializer.Save(configinfo, configFilePath);
            }
            return configinfo;
        }


        public static BaseConfigInfo Deserialize(string configFilePath)
        {
            return (BaseConfigInfo)Serializer.Load(typeof(BaseConfigInfo), configFilePath);
        }

        #endregion



    }
}
