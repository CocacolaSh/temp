using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;

namespace Ocean.Core.Logging
{
    public class Log4NetImpl
    {
        public static void SetConfig(string configFile)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(configFile)); 
        }
        public enum ErrorLevel
        {
            Debug,
            Error,
            Info,
            Warn,
            Fatal,
        }

        #region 写日志 public static void Write(string logMessage)
        /// <summary>
        /// 写日志
        /// </summary>
        public static void Write(string logMessage)
        {
            Write("OceanLog", logMessage);
        }
        #endregion

        #region 写日志 public static void Write(string name, string logMessage)
        /// <summary>
        /// 写日志
        /// </summary>
        public static void Write(string name, string logMessage)
        {
            Write(name, logMessage, ErrorLevel.Debug);
        }
        #endregion

        #region 写日记 public static void Write(string logMessage,ErrorLevel errorLevel)
        /// <summary>
        /// 写日记
        /// </summary>
        public static void Write(string logMessage, ErrorLevel errorLevel)
        {
            Write("OceanLog", logMessage, errorLevel);
        }
        #endregion

        #region 写日志 public static void Write(string name, string logMessage, ErrorLevel errorLevel)
        /// <summary>
        /// 写日志
        /// </summary>
        public static void Write(string name, string logMessage, ErrorLevel errorLevel)
        {
            ILog logger = LogManager.GetLogger(name);

            switch (errorLevel)
            {
                case ErrorLevel.Debug:
                    logger.Debug(logMessage);
                    break;
                case ErrorLevel.Error:
                    logger.Error(logMessage);
                    break;
                case ErrorLevel.Info:
                    logger.Info(logMessage);
                    break;
                case ErrorLevel.Warn:
                    logger.Warn(logMessage);
                    break;
                case ErrorLevel.Fatal:
                    logger.Fatal(logMessage);
                    break;
                default:
                    logger.Debug(logMessage);
                    break;
            }
        }
        #endregion

        #region 写日志 public static void Write(Type type, string logMessage, ErrorLevel errorLevel)
        /// <summary>
        /// 写日志
        /// </summary>
        public static void Write(Type type, string logMessage, ErrorLevel errorLevel)
        {
            ILog logger = LogManager.GetLogger(type);

            switch (errorLevel)
            {
                case ErrorLevel.Debug:
                    logger.Debug(logMessage);
                    break;
                case ErrorLevel.Error:
                    logger.Error(logMessage);
                    break;
                case ErrorLevel.Info:
                    logger.Info(logMessage);
                    break;
                case ErrorLevel.Warn:
                    logger.Warn(logMessage);
                    break;
                case ErrorLevel.Fatal:
                    logger.Fatal(logMessage);
                    break;
                default:
                    logger.Debug(logMessage);
                    break;
            }
        }
        #endregion
    }
}
