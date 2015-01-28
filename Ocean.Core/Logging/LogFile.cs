using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ocean.Core.Logging
{
    public class LogFileImpl
    {
        private static int _MaxFileSize = 1024 * 1024 * 2;//1M，单位：字节

        #region 写错误日志，如果没有写权限将不会产生异常
        /// <summary>
        /// 写错误日志，如果没有写权限将不会产生异常
        /// </summary>
        /// <param name="logMessage"></param>
        public static void Write(string logMessage)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "oceanlogfile.txt";
            FileInfo fileInfo = new FileInfo(fileName);

            if (fileInfo.Exists && fileInfo.Length > _MaxFileSize)
            {
                fileInfo.Delete();
            }

            try
            {
                if (logMessage.Contains("Response.End")) return;

                using (FileStream fileStream = fileInfo.OpenWrite())
                {
                    StreamWriter streamWriter = new StreamWriter(fileStream);
                    streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                    streamWriter.Write("\r\n{0}\r\n", DateTime.Now.ToString());

                    try
                    {
                        if (HttpContext.Current != null && HttpContext.Current.Request != null)
                        {
                            streamWriter.Write("URL:" + HttpContext.Current.Request.Url.PathAndQuery + "\r\n");

                            if (HttpContext.Current.Request.UrlReferrer != null)
                                streamWriter.Write("RefererURL：" + HttpContext.Current.Request.UrlReferrer.AbsolutePath + "\r\n");
                        }
                    }
                    catch
                    {
                    }

                    streamWriter.Write(logMessage + "\r\n");
                    streamWriter.Write("------------------------------------------------------------------\r\n");
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch
            {
            }
        }
        #endregion

        #region public static void Write(Exception exception)
        public static void Write(Exception exception)
        {
            Write(exception.Message + "\r\n" + exception.StackTrace);
        }
        #endregion
    }
}
