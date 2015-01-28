using System;
using System.Text;
using Ocean.Core.Common;
using Ocean.Core.ExceptionHandling;
using System.Web;
using Ocean.Core.Utility;
namespace Ocean.Core.Configuration
{

    /// <summary>
    ///  FTP配置类
    /// </summary>
    public class FTPConfigs
    {
        private static object lockhelper = new object();

        #region 声明上传信息(静态)对象

        private static FTPConfigInfoCollection ftpList;
        //当前索引
        private static FTPConfigInfo currentFtp;

        #endregion

        private static string m_configfilepath = HttpContext.Current.Server.MapPath("~/config/ftp.config");

        /// <summary>
        /// 程序刚加载时ftp.config文件修改时间
        /// </summary>
        private static DateTime m_fileoldchange;
        /// <summary>
        /// 最近ftp.config文件修改时间
        /// </summary>
        private static DateTime m_filenewchange;


        static FTPConfigs()
        {
            if (FileHelper.FileExists(m_configfilepath))
            {
                SetFtpConfigInfo();
                m_fileoldchange = System.IO.File.GetLastWriteTime(m_configfilepath);
            }
        }

        /// <summary>
        /// 设置FTP对象信息
        /// </summary>
        private static void SetFtpConfigInfo()
        {
            ftpList = (FTPConfigInfoCollection)Serializer.Load(typeof(FTPConfigInfoCollection), m_configfilepath);
            FTPConfigInfoCollection.FTPConfigInfoCollectionEnumerator ftps = ftpList.GetEnumerator();
            //遍历集合并设置相应的FTP信息(静态)对象
            while (ftps.MoveNext())
            {
                if (ftps.Current.Default==1&&ftps.Current.Enable==1)
                {
                    currentFtp = ftps.Current;
                    break;
                }
            }
        }

        /// <summary>
        /// 获取当前配置
        /// </summary>
        public static FTPConfigInfo CurrentFTP
        {
            get
            {
                FtpFileMonitor();
                return currentFtp;
            }
        }
        /// <summary>
        /// 根据FTP唯一标识获取FTP配置信息
        /// </summary>
        public static FTPConfigInfo GetFTP(int ident)
        {
            if (ident == 0)
            {
                return CurrentFTP;
            }
            else
            {
                FtpFileMonitor();
                FTPConfigInfoCollection.FTPConfigInfoCollectionEnumerator ftps = ftpList.GetEnumerator();
                while (ftps.MoveNext())
                {
                    if (ftps.Current.Enable == 1 && ftps.Current.Ident == ident)
                    {
                        return ftps.Current;
                    }
                }
            }
            throw ExceptionManager.MessageException("对不起，存储系统唯一标识【" + ident.ToString() + "】不存在或未启用，请检查！");
        }
        /// <summary>
        /// FTP配置文件监视方法
        /// </summary>
        private static void FtpFileMonitor()
        {
            if (FileHelper.FileExists(m_configfilepath))
            {
                //获取文件最近修改时间 
                m_filenewchange = System.IO.File.GetLastWriteTime(m_configfilepath);

                //当ftp.config修改时间发生变化时
                if (m_fileoldchange != m_filenewchange)
                {
                    lock (lockhelper)
                    {
                        if (m_fileoldchange != m_filenewchange)
                        {
                            //当文件发生修改(时间变化)则重新设置相关FTP信息对象
                            SetFtpConfigInfo();
                            m_fileoldchange = m_filenewchange;
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// 获取配置类实例
        ///// </summary>
        ///// <returns></returns>
        //public static FTPConfigInfo GetConfig()
        //{
        //    return FTPConfigFileManager.LoadConfig();
        //}

        /// <summary>
        /// 保存配置类实例
        /// </summary>
        /// <param name="emailconfiginfo"></param>
        /// <returns></returns>
        public static bool SaveConfig(FTPConfigInfo ftpconfiginfo)
        {
            Serializer.Save(ftpconfiginfo, m_configfilepath);
            return true;
        }
        /// <summary>
        /// 测试保存FTP配置文件
        /// </summary>
        public static void TestSaveFTPConfigInfo()
        {
            FTPConfigInfoCollection ftps = new FTPConfigInfoCollection();
            ftps.Add(new FTPConfigInfo());
            Serializer.Save(ftps, @"F:\ftp.config");
        }
    }
}
