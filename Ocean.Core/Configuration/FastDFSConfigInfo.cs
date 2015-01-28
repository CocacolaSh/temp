using System;
using System.Xml.Serialization;
using Ocean.Core.Common;
using Ocean.Core.ExceptionHandling;
namespace Ocean.Core.Configuration
{
	/// <summary>
	/// FastDFS配置信息类
	/// </summary>
	[Serializable]
    public class FastDFSConfigInfo : IConfigInfo
    {
        #region FastDFS私有字段

        private string groupname = ""; //组名

        private string serveraddress = ""; //服务器地址

        private int serverport = 25; //服务器端口号

        private int bakftpident = 0;//备份FTP服务器唯一标识

        private string remoteurl = ""; //远程访问 URL

        private string remoteurl2 = ""; //远程访问 URL2

        private string assemblytype = "";//FastDFS插件类名:为空时，则调用系统默认

        private string bakclassname = "";//FastDFS备份使用的外部类名

        private int enable = 0;//是否启用

        private int _default = 0;//是否默认
        #endregion

        public FastDFSConfigInfo()
		{
        }

        #region 属性

        /// <summary>
        /// 名称
		/// </summary>
        public string Groupname
		{
            get { return groupname; }
            set { groupname = value; }
		}

        /// <summary>
        /// FastDFS服务器地址
		/// </summary>
        public string Serveraddress
		{
            get { return serveraddress; }
            set { serveraddress = value; }
		}

		/// <summary>
        /// FastDFS端口号
		/// </summary>
        public int Serverport
		{
            get { return serverport; }
            set { serverport = value; }
		}

        /// <summary>
        /// 备份FTP服务器唯一标识
        /// </summary>
        public int BakFtpIdent
        {
            get { return bakftpident; }
            set { bakftpident = value; }
        }

        /// <summary>
        /// 远程访问 URL
        /// </summary>
        public string Remoteurl
        {
            get { return remoteurl; }
            set { remoteurl = value; }
        }

        /// <summary>
        /// 远程访问 URL2
        /// </summary>
        public string Remoteurl2
        {
            get { return remoteurl2; }
            set { remoteurl2 = value; }
        }

        /// <summary>
        /// FastDFS插件类名：为空时，则调用系统默认
        /// </summary>
        public string AssemblyType
        {
            get { return assemblytype; }
            set { assemblytype = value; }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public int Enable
        {
            get { return enable; }
            set { enable = value; }
        }
        /// <summary>
        /// 是否默认
        /// </summary>
        public int Default
        {
            get { return _default; }
            set { _default = value; }
        }
        #endregion

    }
}
