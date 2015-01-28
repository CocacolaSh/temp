using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Configuration;
using Ocean.Core.Plugins.FTP;

namespace Ocean.Core.Plugins.Upload
{
    public class UpFileEntity
    {
        public UpFileEntity()
        {
            BaseConfigInfo config = BaseConfigs.GetConfig();
            this.ftpEnable = config.FTPEnable;
            this.distributeFileSystem = config.DistributeFileSystem;
            IsClose = true;
        }
        private string dir = "";
        private int size = 500;
        private string[] thumbnailSizes;
        private string allowType = ".jpg,.jpeg,.gif,.bmp,.png";
        private int maxAllowFileCount = 5;
        private int width = 0;
        private int maxWidth = 0;
        private int height = 0;
        private int maxHeight = 0;
        private bool isWarterMark = false;
        private int warterMarkStatus = 0;
        private string key = "FXUpload";

        private string saveFullPath;

        #region 自定义路径
        public string PathFormat { get; set; }
        #endregion

        #region FTP
        public IFTP ftpclient { get; set; }
        public bool IsClose { get; set; }
        private int ftpEnable = 1;//FTP启用状态-当不使用配置文件时，可以手动设置
        private int ident = 0;
        /// <summary>
        /// 目标文件-暂停不用
        /// </summary>
        public string Target_File_Name { get; set; }
        /// <summary>
        /// FTP启用状态-当不使用配置文件时，可以手动设置
        /// </summary>
        public int FTPEnable
        {
            get { return ftpEnable; }
            set { ftpEnable = value; }
        }
        #endregion

        #region FastDFS
        private int distributeFileSystem = 1;//分布式文件系统启用状态-当不使用配置文件时，可以手动设置
        private string groupName = "group1";
        /// <summary>
        /// 主文件名
        /// </summary>
        public string Mast_File_Name { get; set; }
        /// <summary>
        /// 从文件名
        /// </summary>
        public string Prefix_Name { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string File_Ext { get; set; }
        /// <summary>
        /// 分布式文件系统启用状态-当不使用配置文件时，可以手动设置
        /// </summary>
        public int DistributeFileSystem
        {
            get { return distributeFileSystem; }
            set { distributeFileSystem = value; }
        }
        #endregion

        #region bak
        public int AttachID { get; set; }
        /// <summary>
        /// 站点ID
        /// </summary>
        public virtual int Site_ID
        {
            get;
            set;
        }
        /// <summary>
        /// 用户卡号-冗余
        /// </summary>
        public virtual string User_No
        {
            get;
            set;
        }
        /// <summary>
        /// 用户ID-冗余
        /// </summary>
        public virtual int User_ID
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 保存的中间路径
        /// </summary>
        public string Dir
        {
            get { return dir; }
            set { dir = value; }
        }
        /// <summary>
        /// 上传文件大小
        /// </summary>
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        /// <summary>
        /// 缩略图new string[]{"100_100","80_80"} N张
        /// </summary>
        public string[] ThumbnailSizes
        {
            get { return thumbnailSizes; }
            set { thumbnailSizes = value; }
        }
        /// <summary>
        /// 上传文件类型
        /// </summary>
        public string AllowType
        {
            get { return allowType; }
            set { allowType = value; }
        }
        /// <summary>
        /// 固定宽度
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        /// <summary>
        /// 不超过宽度
        /// </summary>
        public int MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }
        /// <summary>
        /// 固定高度
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        /// <summary>
        /// 不超过高度
        /// </summary>
        public int MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
        }
        /// <summary>
        /// 是否水印
        /// </summary>
        public bool IsWarterMark
        {
            get { return isWarterMark; }
            set { isWarterMark = value; }
        }
        /// <summary>
        /// 水印位置
        /// </summary>
        public int WarterMarkStatus
        {
            get { return warterMarkStatus; }
            set { warterMarkStatus = value; }
        }
        /// <summary>
        /// 最多上传文件数
        /// </summary>
        public int MaxAllowFileCount
        {
            get { return maxAllowFileCount; }
            set { maxAllowFileCount = value; }
        }
        /// <summary>
        /// File控件的Key(即Name属性)
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; }
        }
        /// <summary>
        /// 保存详细地址
        /// </summary>
        public string SaveFullPath
        {
            get { return saveFullPath; }
            set { saveFullPath = value; }
        }

        #region 存储路径设置

        #endregion

        #region FTP
        /// <summary>
        /// FTP服务器的唯一标识
        /// </summary>
        public int FTPIdent
        {
            get { return ident; }
            set { ident = value; }
        }
        #endregion

        #region FastDFS
        /// <summary>
        /// FastDFS组名
        /// </summary>
        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }
        #endregion
    }
}
