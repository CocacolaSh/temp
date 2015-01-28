using System;
using System.Text;
using Ocean.Core.Common;
using Ocean.Core.Configuration;
using System.Collections.Generic;
using Ocean.Core.Plugins.FTP;
using System.Reflection;
using Ocean.Core.ExceptionHandling;
using System.IO;
using Ocean.Core.Plugins.Upload;
using Ocean.Core.Utility;

namespace Ocean.Core.Plugins.FTP
{
    /// <summary>
    /// FTP操作类
    /// </summary>
    public class FTPs
    {
        static Dictionary<int, IFTP> ftpDic = new Dictionary<int, IFTP>();
        public static IFTP Instrance(int ident)
        {
            IFTP ftp = null;
            FTPConfigInfo ftpConfig = FTPConfigs.GetFTP(ident);
            if (!string.IsNullOrEmpty(ftpConfig.AssemblyType))
            {
                string[] filenames = ftpConfig.AssemblyType.Split(',');
                Assembly ass = Assembly.LoadFrom(System.Web.HttpRuntime.BinDirectory + filenames[1] + ".dll");
                ftp = (IFTP)Activator.CreateInstance(ass.GetType(filenames[0], false, true), new object[] { (IConfigInfo)ftpConfig });
                if (ftp == null)
                {
                    throw ExceptionManager.MessageException("存储系统配置文件有误，请检查！");
                }
            }else
                {
                    if (ftpConfig.Timeout == 0)//不限时
                    {
                        ftp = new FTPClient(ftpConfig.Serveraddress, ftpConfig.Serverport, ftpConfig.Username, ftpConfig.Password, ftpConfig.Mode);
                    }
                    else
                    {
                        ftp = new FTPClient(ftpConfig.Serveraddress, ftpConfig.Serverport, ftpConfig.Username, ftpConfig.Password, ftpConfig.Mode, ftpConfig.Timeout);
                    }
                }
            ftp.FtpConfig = ftpConfig;
            //ftpDic.TryGetValue(ident, out ftp);
            //if (ftp == null)
            //{
            //    FTPConfigInfo ftpConfig = FTPConfigs.GetFTP(ident);
            //    if (!string.IsNullOrEmpty(ftpConfig.AssemblyType))
            //    {
            //        string[] filenames=ftpConfig.AssemblyType.Split(',');
            //        Assembly ass = Assembly.LoadFrom(System.Web.HttpRuntime.BinDirectory + filenames[1] + ".dll");
            //        ftp = (IFTP)Activator.CreateInstance(ass.GetType(filenames[0], false, true), new object[] { (IConfigInfo)ftpConfig });
            //        if (ftp == null)
            //        {
            //            throw ExceptionManager.MessageException("存储系统配置文件有误，请检查！");
            //        }
            //    }
            //    else
            //    {
            //        if (ftpConfig.Timeout == 0)//不限时
            //        {
            //            ftp = new FTPClient(ftpConfig.Serveraddress, ftpConfig.Serverport, ftpConfig.Username, ftpConfig.Password, ftpConfig.Mode);
            //        }
            //        else
            //        {
            //            ftp = new FTPClient(ftpConfig.Serveraddress, ftpConfig.Serverport, ftpConfig.Username, ftpConfig.Password, ftpConfig.Mode, ftpConfig.Timeout);
            //        }
            //    }
            //    ftp.FtpConfig = ftpConfig;
            //    ftpDic[ident] = ftp;
            //}
            return ftp;
        }
        public static IFTP CurrentFTP
        {
            get
            {
                 FTPConfigInfo ftpConfig = FTPConfigs.CurrentFTP;
                 IFTP ftp = null;
                 ftpDic.TryGetValue(ftpConfig.Ident, out ftp);
                if (ftp == null)
                {
                        if (!string.IsNullOrEmpty(ftpConfig.AssemblyType))
                        {
                            string[] filenames = ftpConfig.AssemblyType.Split(',');
                            Assembly ass = Assembly.LoadFrom(System.Web.HttpRuntime.BinDirectory + filenames[1] + ".dll");
                            ftp = (IFTP)Activator.CreateInstance(ass.GetType(filenames[0], false, true));
                            if (ftp == null)
                            {
                                throw ExceptionManager.MessageException("存储系统配置文件有误，请检查！");
                            }
                        }
                        else
                        {
                            if (ftpConfig.Timeout == 0)//不限时
                            {
                                ftp = new FTPClient(ftpConfig.Serveraddress, ftpConfig.Serverport, ftpConfig.Username, ftpConfig.Password, ftpConfig.Mode);
                            }
                            else
                            {
                                ftp = new FTPClient(ftpConfig.Serveraddress, ftpConfig.Serverport, ftpConfig.Username, ftpConfig.Password, ftpConfig.Mode, ftpConfig.Timeout);
                            }
                        }
                    ftpDic[ftpConfig.Ident] = ftp;
                }
                ftp.FtpConfig = ftpConfig;
                return ftp;
            }
        }
        
        public FTPs()
        { }



        #region 异步FTP上传文件

        private delegate bool delegateUpLoadFile(string path, string file,UpFileEntity upfileEntity, int ftpIdent);

        //异步FTP上传文件代理
        private delegateUpLoadFile upload_aysncallback;

        public void AsyncUpLoadFile(string path, string file, UpFileEntity upfileEntity, int ftpIdent)
        {
            upload_aysncallback = new delegateUpLoadFile(UpLoadFile);
            upload_aysncallback.BeginInvoke(path, file, upfileEntity, ftpIdent, null, null);
        }

        #endregion


        /// <summary>
        /// 判断FTP当前是否配置为本地
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public static bool IsLocalhost(int ident)
        {
            FTPConfigInfo ftpConfig = null;
            if(ident>0)
            {
                ftpConfig = FTPConfigs.GetFTP(ident);
            }
            else{
                ftpConfig = FTPConfigs.CurrentFTP;
            }
            return ftpConfig.Serveraddress.ToLower() == "localhost";
        }

        /// <summary>
        /// 普通FTP上传文件
        /// </summary>
        /// <param name="file">要FTP上传的文件</param>
        /// <returns>上传是否成功</returns>
        public static bool UpLoadFile(string path, string file, UpFileEntity upfileEntity, int ftpIdent = 0)
        {
            IFTP ftpupload;
            FTPConfigInfo ftpConfig = null;
            //path = ftpConfig.Uploadpath + path;
            if (upfileEntity.ftpclient == null)
            {
                if (ftpIdent == 0)
                {
                    ftpupload = CurrentFTP;
                }
                else
                {
                    ftpupload = Instrance(ftpIdent);
                }
                upfileEntity.ftpclient = ftpupload;
                //转换路径分割符为"/"
                path = path.Replace("\\", "/");
                path = path.StartsWith("/") ? path : "/" + path;

                ftpupload.Connect();
                //切换到指定路径下,如果目录不存在,将创建
                if (!ftpupload.ChangeDir(path))
                {
                    string[] pathArr = path.Split('/');
                    foreach (string pathstr in pathArr)
                    {
                        if (pathstr.Trim() != "")
                        {
                            ftpupload.MakeDir(pathstr);
                            ftpupload.ChangeDir(pathstr);
                        }
                    }
                }


            }
            else {
                ftpupload = upfileEntity.ftpclient;
            }
                if (!ftpupload.IsConnected)
                    return false;
            ftpConfig = ftpupload.FtpConfig as FTPConfigInfo;
            
            int perc = 0;
            //绑定要上传的文件
            if (!ftpupload.OpenUpload(file, System.IO.Path.GetFileName(file)))
            {
                //ftpupload.Disconnect();
                return false;
            }

            //开始进行上传
            while (ftpupload.DoUpload() > 0)
                perc = (int)(((ftpupload.BytesTotal) * 100) / ftpupload.FileSize);

            //ftpupload.Disconnect();


            //删除file参数文件
            bool delfile = true;
            delfile = (ftpConfig.Reservelocalattach == 1) ? false : true;

            //(如存在)删除指定目录下的文件
            if (delfile && FileHelper.FileExists(file))
                System.IO.File.Delete(file);

            return (perc >= 100) ? true : false;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="ftpIdent"></param>
        /// <returns></returns>
        public static bool DeleteFiles(string[] files, string filepath, int ftpIdent = 0)
        {
            IFTP ftpupload;
            if (ftpIdent == 0)
            {
                ftpupload = CurrentFTP;
            }
            else
            {
                ftpupload = Instrance(ftpIdent);
            }

            //转换路径分割符为"/"
            //string path = filepath.Replace("\\", "/");
            //path = path.StartsWith("/") ? path : "/" + path;
            FTPConfigInfo ftpConfig = ftpupload.FtpConfig as FTPConfigInfo;
            //path = ftpConfig.Uploadpath + path;
            ftpupload.Connect();
            string dir = Path.GetDirectoryName(filepath).Replace("\\", "/");
            string[] pathArr = dir.Split('/');
            foreach (string pathstr in pathArr)
            {
                if (pathstr.Trim() != "")
                {
                    ftpupload.MakeDir(pathstr);
                    ftpupload.ChangeDir(pathstr);
                }
            }
            foreach (string p in files)
            {
                ftpupload.RemoveFile(Path.GetFileName(p));
            }
            ftpupload.Disconnect();//关闭
            return true;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="ftpIdent"></param>
        /// <returns></returns>
        public static bool DeleteFile(string path, string file, int ftpIdent = 0)
        {
            IFTP ftpupload;
            if (ftpIdent == 0)
            {
                ftpupload = CurrentFTP;
            }
            else
            {
                ftpupload = Instrance(ftpIdent);
            }
            //转换路径分割符为"/"
            //path = path.Replace("\\", "/");
            //path = path.StartsWith("/") ? path : "/" + path;
            FTPConfigInfo ftpConfig = ftpupload.FtpConfig as FTPConfigInfo;
            //path = ftpConfig.Uploadpath + path;
            ftpupload.Connect();
            string dir = Path.GetDirectoryName(path).Replace("\\", "/");
            string[] pathArr = dir.Split('/');
            foreach (string pathstr in pathArr)
            {
                if (pathstr.Trim() != "")
                {
                    ftpupload.MakeDir(pathstr);
                    ftpupload.ChangeDir(pathstr);
                }
            }
            ftpupload.RemoveFile(Path.GetFileName(path));
            ftpupload.Disconnect();//关闭
            return true;
        }
        /// <summary>
        /// FTP连接测试
        /// </summary>
        /// <param name="Serveraddress">FTP服务器地址</param>
        /// <param name="Serverport">FTP端口</param>
        /// <param name="Username">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="Timeout">超时时间(秒)</param>
        /// <param name="uploadpath">附件保存路径</param>
        /// <param name="message">返回信息</param>
        /// <returns>是否可用</returns>
        public static bool TestConnect(string uploadpath, ref string message)
        {
            IFTP ftpupload = CurrentFTP;
            bool isvalid = ftpupload.Connect();
            if (!isvalid)
            {
                message = ftpupload.ErrorMessage;
                return isvalid;
            }

            //切换到指定路径下,如果目录不存在,将创建
            if (!ftpupload.ChangeDir(uploadpath))
            {
                ftpupload.MakeDir(uploadpath);
                if (!ftpupload.ChangeDir(uploadpath))
                {
                    message += ftpupload.ErrorMessage;
                    isvalid = false;
                }
            }
            return isvalid;
        }
    }
}
