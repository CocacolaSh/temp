using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Ocean.Core.Plugins.Upload
{
    /// <summary>
    /// 文件信息描述类
    /// </summary>
    [DataContract]
    public class AttachmentInfo
    {
        private int attachmenterID;	//对应的位置
        private DateTime attachmentDate;	//发布时间
        private string fileName;	//存储文件名
        private string description;	//描述
        private string fileType;	//文件类型
        private long fileSize;	//文件尺寸
        private int width;	//图片附件宽度
        private int height;    //图片附件高度
        private string error = "";

        private string remoteUrl;//远程地址
        private string remoteUrl2;//远程地址2-备用

        #region FTP
        private int ftpEnable = 0;//FTP启用状态
        private int ftpIdent = 0;//普通，>0为相应的FTP配置
        #endregion

        #region FastDFS
        private int distributeFileSystem = 0;//分布式文件系统启用状态
        private string groupName = "group1";//分布式文件系统-文件组
        #endregion

        #region bak
        private int siteId;//站点ID
        private string userNo;//用户卡号-冗余
        private int userId;//用户ID-冗余
        private int isBak;//是否需要备份
        #endregion

        #region 文件库管理
        private int categoryId;//分类ID
        #endregion

        ///<summary>
        ///附件ID
        ///</summary>
        [DataMember]
        public virtual int ID
        {
            get;
            set;
        }

        ///<summary>
        ///对应的位置
        ///</summary>
        [DataMember]
        public virtual int AttachmenterID
        {
            get { return attachmenterID; }
            set { attachmenterID = value; }
        }

        ///<summary>
        ///发布时间
        ///</summary>
        [DataMember]
        public virtual DateTime Date
        {
            get { return attachmentDate; }
            set { attachmentDate = value; }
        }
        ///<summary>
        ///存储文件名
        ///</summary>
        [DataMember]
        public virtual string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        ///<summary>
        ///描述
        ///</summary>
        [DataMember]
        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        ///<summary>
        ///文件类型
        ///</summary>
        [DataMember]
        public virtual string FileType
        {
            get {
                if (string.IsNullOrEmpty(fileType))
                {
                    return fileType;
                }
                return fileType.Trim(); 
            }
            set { fileType = value; }
        }

        ///<summary>
        ///文件尺寸
        ///</summary>
        [DataMember]
        public virtual long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        /// <summary>
        /// 图片附件宽度
        /// </summary>
        [DataMember]
        public virtual int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// 图片附件高度
        /// </summary>
        [DataMember]
        public virtual int Height
        {
            get { return height; }
            set { height = value; }
        }
        /// <summary>
        /// 上传附件信息
        /// </summary>
        [DataMember]
        public virtual string Error
        {
            get { return error; }
            set { error = value; }
        }
        /// <summary>
        /// //远程地址
        /// </summary>
        [DataMember]
        public virtual string RemoteUrl
        {
            get { return remoteUrl; }
            set { remoteUrl = value; }
        }
        /// <summary>
        /// 远程地址2-备用
        /// </summary>
        [DataMember]
        public virtual string RemoteUrl2
        {
            get { return remoteUrl2; }
            set { remoteUrl2 = value; }
        }

        #region FTP
        /// <summary>
        /// FTP启用状态
        /// </summary>
        [DataMember]
        public virtual int FTPEnable
        {
            get { return ftpEnable; }
            set { ftpEnable = value; }
        }
        /// <summary>
        /// 普通，>0为相应的FTP配置
        /// </summary>
        [DataMember]
        public virtual int FtpIdent
        {
            get { return ftpIdent; }
            set { ftpIdent = value; }
        }
        #endregion

        #region FastDFS
        /// <summary>
        /// 分布式文件系统启用状态
        /// </summary>
        [DataMember]
        public virtual int DistributeFileSystem
        {
            get { return distributeFileSystem; }
            set { distributeFileSystem = value; }
        }
        /// <summary>
        /// 分布式文件系统-文件组
        /// </summary>
        [DataMember]
        public virtual string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }
        #endregion

        #region bak
        /// <summary>
        /// 站点ID
        /// </summary>
        [DataMember]
        public virtual int Site_ID
        {
            get { return siteId; }
            set { siteId = value; }
        }
        /// <summary>
        /// 用户卡号-冗余
        /// </summary>
        [DataMember]
        public virtual string User_No
        {
            get { return userNo; }
            set { userNo = value; }
        }
        /// <summary>
        /// 用户ID-冗余
        /// </summary>
        [DataMember]
        public virtual int User_ID
        {
            get { return userId; }
            set { userId = value; }
        }
        /// <summary>
        /// 是否需要备份
        /// </summary>
       [DataMember]
        public virtual int Is_Bak
        {
            get { return isBak; }
            set { isBak = value; }
        }
       [DataMember]
       public virtual int Bak_Status
       {
           get;
           set;
       }
        #endregion

        #region 文件库管理
        /// <summary>
       /// 模型ID[如：文章、产品……]
        /// </summary>
       [DataMember]
        public virtual int Model_ID
       {
           get;
           set;
       }
        /// <summary>
        /// 分类ID
        /// </summary>
        [DataMember]
       public virtual int Category_ID
        {
            get { return categoryId; }
            set { categoryId = value; }
        }
        #endregion

        #region 数据表字段
        /// <summary>
        /// 分类（music -0，flash -1，在线视频 -2，4··）
        /// </summary>
        [DataMember]
        public virtual int Attach_Type
        {
            get;
            set;
        }
        /// <summary>
        /// 文件上传来源（０－手动，１－编辑器）
        /// </summary>
        [DataMember]
        public virtual int Source
        { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public virtual string Name
        { get; set; }
        /// <summary>
        /// 引用次数
        /// </summary>
        [DataMember]
        public virtual int Quote_Times
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public virtual int Status
        { get; set; }
        /// <summary>
        /// 缩略图-用","隔开
        /// </summary>
        [DataMember]
        public virtual string Thumbnail_Sizes
        {
            get;
            set;
        }
        #endregion

        [DataMember]
        public virtual int Row_Version
        {
            get;
            set;
        }
    }
}
