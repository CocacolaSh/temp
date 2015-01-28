using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Configuration;
using Ocean.Core.Plugins.FTP;
using Ocean.Core.Plugins.DFS;

namespace Ocean.Core.Plugins.Upload
{
    public class OperateParam
    {
        public BaseConfigInfo config { get { return BaseConfigs.GetConfig(); } }

        public OperateParam()
        {
            GroupName = "none";
            FtpIdent = -1;
            IsChangeDir = true;
        }
        public IFTP FTP { get; set; }
        public IDFS DFS { get; set; }
        /// <summary>
        /// 源文件或目录地址
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// FTP唯一标识
        /// </summary>
        public int Source_FtpIdent { get; set; }
        /// <summary>
        /// 分布式文件系统-组名
        /// </summary>
        public string Source_GroupName { get; set; }
        /// <summary>
        /// 目标文件或地址
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// FTP唯一标识
        /// </summary>
        public int FtpIdent { get; set; }
        /// <summary>
        /// 是否切换目录
        /// </summary>
        public bool IsChangeDir { get; set; }
        /// <summary>
        /// 分布式文件系统-组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 主文件路径
        /// </summary>
        public string MastFilePath { get; set; }
        /// <summary>
        /// 主文件名称-当传目录时，要分出哪个为主文件
        /// </summary>
        public string MasterName { get; set; }
        /// <summary>
        /// 根据主文件名称-MastFilePath，得到结构路径
        /// </summary>
        public string ResultPath { get; set; }
        /// <summary>
        /// 从文件名|
        /// </summary>
        public string[] SlaveNames { get; set; }

        /// <summary>
        /// 替换文件目录
        /// </summary>
        public string ReplaceDir { get; set; }

        #region 路径
        /// <summary>
        /// 文件长度
        /// </summary>
        public int ContentLength { get; set; }
        /// <summary>
        /// 设置当前目录
        /// </summary>
        public string Dir { get; set; }
        /// <summary>
        /// 路径格式
        /// </summary>
        public string PathFormat { get; set; }

        #endregion
        /// <summary>
        /// 是否保存目标文件名
        /// </summary>
        public bool IsSaveTarget { get; set; }
        /// <summary>
        /// 需要更新扩展名的文件名称，多个用“，”隔开
        /// </summary>
        public string NeedChangeExtensions { get; set; }

    }
}
