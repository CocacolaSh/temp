using System;
using System.Text;
using Ocean.Core.Common;
using Ocean.Core.ExceptionHandling;
using System.Web;
using Ocean.Core.Utility;
namespace Ocean.Core.Configuration
{

    /// <summary>
    ///  FastDFS配置类
    /// </summary>
    public class FastDFSConfigs
    {
        private static object lockhelper = new object();

        #region 声明上传信息(静态)对象

        private static FastDFSConfigInfoCollection fastdfsList;
        //当前索引
        private static FastDFSConfigInfo currentFtp;

        #endregion

        private static string m_configfilepath = HttpContext.Current.Server.MapPath("~/config/fastdfs.config");//"F:\\Project\\CMS\\Ocean\\trunk\\04.系统编码\\Ocean.Web\\Config\\fastdfs.config";

        /// <summary>
        /// 程序刚加载时fastdfs.config文件修改时间
        /// </summary>
        private static DateTime m_fileoldchange;
        /// <summary>
        /// 最近fastdfs.config文件修改时间
        /// </summary>
        private static DateTime m_filenewchange;


        static FastDFSConfigs()
        {
            if (FileHelper.FileExists(m_configfilepath))
            {
                SetFtpConfigInfo();
                m_fileoldchange = System.IO.File.GetLastWriteTime(m_configfilepath);
            }
        }

        /// <summary>
        /// 设置FastDFS对象信息
        /// </summary>
        private static void SetFtpConfigInfo()
        {
            fastdfsList = (FastDFSConfigInfoCollection)Serializer.Load(typeof(FastDFSConfigInfoCollection), m_configfilepath);
            FastDFSConfigInfoCollection.FastDFSConfigInfoCollectionEnumerator fastdfss = fastdfsList.GetEnumerator();
            //遍历集合并设置相应的FastDFS信息(静态)对象
            while (fastdfss.MoveNext())
            {
                if (fastdfss.Current.Default==1&&fastdfss.Current.Enable==1)
                {
                    currentFtp = fastdfss.Current;
                    break;
                }
            }
        }

        /// <summary>
        /// 获取当前配置
        /// </summary>
        public static FastDFSConfigInfo CurrentFastDFS
        {
            get
            {
                FtpFileMonitor();
                return currentFtp;
            }
        }
        /// <summary>
        /// 根据FastDFS groupName获取FastDFS配置信息
        /// </summary>
        public static FastDFSConfigInfo GetFastDFS(string groupName)
        {
            FtpFileMonitor();
            if (string.IsNullOrEmpty(groupName))
            {
                return CurrentFastDFS;
            }
            FastDFSConfigInfoCollection.FastDFSConfigInfoCollectionEnumerator fastdfss = fastdfsList.GetEnumerator();
            while (fastdfss.MoveNext())
            {
                if (fastdfss.Current.Enable == 1 && fastdfss.Current.Groupname == groupName)
                {
                    return fastdfss.Current;
                }
            }
            throw ExceptionManager.MessageException("对不起，存储系统组名【" + groupName + "】不存在或未启用，请检查！");
        }

        /// <summary>
        /// FastDFS配置文件监视方法
        /// </summary>
        private static void FtpFileMonitor()
        {
            if (FileHelper.FileExists(m_configfilepath))
            {
                //获取文件最近修改时间 
                m_filenewchange = System.IO.File.GetLastWriteTime(m_configfilepath);

                //当fastdfs.config修改时间发生变化时
                if (m_fileoldchange != m_filenewchange)
                {
                    lock (lockhelper)
                    {
                        if (m_fileoldchange != m_filenewchange)
                        {
                            //当文件发生修改(时间变化)则重新设置相关FastDFS信息对象
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
        //public static FastDFSConfigInfo GetConfig()
        //{
        //    return FastDFSConfigFileManager.LoadConfig();
        //}

        /// <summary>
        /// 保存配置类实例
        /// </summary>
        /// <param name="emailconfiginfo"></param>
        /// <returns></returns>
        public static bool SaveConfig(FastDFSConfigInfo fastdfsconfiginfo)
        {
            Serializer.Save(fastdfsconfiginfo, m_configfilepath);
            return true;
        }
        /// <summary>
        /// 测试保存FastDFS配置文件
        /// </summary>
        public static void TestSaveFastDFSConfigInfo()
        {
            FastDFSConfigInfoCollection collections = new FastDFSConfigInfoCollection();
            collections.Add(new FastDFSConfigInfo());
            Serializer.Save(collections, @"F:\fastdfs.config");
        }
    }
}
