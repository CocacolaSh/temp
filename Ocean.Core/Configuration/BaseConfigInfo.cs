using System;

namespace Ocean.Core.Configuration
{
    /// <summary>
    /// 基本设置描述类, 加[Serializable]标记为可序列化
    /// </summary>
    [Serializable]
    public class BaseConfigInfo : IConfigInfo
    {
        #region 私有字段
        private string webSiteTitle = "Ocean"; //名称
        private string webSiteUrl = "/"; //url地址
        private string userUrl = "/";
        private string webImagesUrl = "/images/";
        private string cdnImagesUrl = "/images/";
        private string domain = ".ocean.com";
        private string template = "default";
        private string skin = "Default";
        private string lang = "zh_CN";//语言
        private string icp = ""; //网站备案信息
        private int closed = 0; //网站关闭
        private string closedReason = ""; //网站关闭提示信息

        private string passwordKey = "1234567890"; //用户密码Key

        private string ipRegCtrl = ""; //特殊 IP 注册限制
        private string ipDenyAccess = ""; //IP禁止访问列表
        private string apiIpAccess = ""; //Api IP访问列表
        private string adminIpAccess = ""; //管理员后台IP访问列表
       
        private string filterKey = "";

        private string seoTitle = ""; //标题附加字
        private string seoKeyWords = ""; //Meta Keywords
        private string seoDescription = ""; //Meta Description
        private string seoHead = "<meta name=\"generator\" content=\"石狮农商银行\" />"; //其他头部信息

        private int attachSave = 0; //保存方式  0=按年/月/日存入不同目录 1=按年/月/日/存入不同目录 2=按存入不同目录 3=按文件类型存入不同目录
        private int waterMarkStatus = 3; //图片添加水印 用0=不使 1=左上 2=中上 3=右上 4=左中 ... 9=右下
        private int waterMarkType = 0; //图片添加何种水印 0=文字 1=图片
        private int waterMarkTransParency = 5; //图片水印透明度 取值范围1--10 (10为不透明)
        private string waterMarkText = "Ocean";  //图片添加文字水印的内容 {1}表示标题 {2}表示地址 {3}表示当前日期 {4}表示当前时间, 例如: {3} {4}上传于{1} {2}
        private string waterMarkPic = "watermark.gif";   //使用的水印图片的名称
        private string waterMarkFontName = "Tahoma"; //图片添加文字水印的字体
        private int waterMarkFontSize = 12; //图片添加文字水印的大小(像素)
        private int attachImgQuality = 80; //是否是高质量图片 取值范围0--100
        private int attachImgMaxHeight = 0;//限制上传图片最大宽高
        private int attachImgMaxWidth = 0;//限制上传图片最大宽度

        private string cookieDomain = "";//身份验证Cookie域

        private string verifyImageAssemly = "VerifyImage";//验证码生成所使用的程序集

        private int _iscache = 0;
        private int enableTrade = 1;

        private string publishVesion = "1.0.0.0";

        private int distributeFileSystem = 0;//分布式文件系统
        private int ftpEnable = 0;//Ftp
        private string ftpTempPath = "";//FTP临时文件
        private int ftpPathFomat = 0;//FTP临时文件保存格式
        private string bakclassname = "";//备份记录使用的外部类名
        #endregion

        #region 属性

        /// <summary>
        /// 网站名称
        /// </summary>
        public string Title
        {
            get { return webSiteTitle; }
            set { webSiteTitle = value; }
        }

        /// <summary>
        /// url地址
        /// </summary>
        public string WebUrl
        {
            get { return webSiteUrl; }
            set { webSiteUrl = value; }
        }
        /// <summary>
        /// 网站图片服务器地址
        /// </summary>
        public string WebImagesUrl
        {
            get { return webImagesUrl; }
            set { webImagesUrl = value; }
        }

        /// <summary>
        /// 网站图片cdn地址
        /// </summary>
        public string CdnImagesUrl
        {
            get { return cdnImagesUrl; }
            set { cdnImagesUrl = value; }
        }
        /// <summary>
        /// 站点域
        /// </summary>
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }
        /// <summary>
        /// 网站模板方案
        /// </summary>
        public string Template
        {
            get { return template; }
            set { template = value; }
        }

        /// <summary>
        /// 风格方案
        /// </summary>
        public string Skin
        {
            get { return skin; }
            set { skin = value; }
        }

        /// <summary>
        /// 网站语言
        /// </summary>
        public string Lang
        {
            get { return lang; }
            set { lang = value; }
        }

        /// <summary>
        /// 网站备案信息
        /// </summary>
        public string Icp
        {
            get { return icp; }
            set { icp = value; }
        }
        /// <summary>
        /// 版权文字 (只读)
        /// </summary>
        public string Copyright
        {
            get { return "&copy; 2013-" + DateTime.Now.Year.ToString() + " <a href=\"http://www.石狮农商银行.com\" target=\"_blank\">石狮农商银行 Inc</a>."; }
        }

        /// <summary>
        /// 网站关闭
        /// </summary>
        public int Closed
        {
            get { return closed; }
            set { closed = value; }
        }

        /// <summary>
        /// 关闭提示信息
        /// </summary>
        public string ClosedReason
        {
            get { return closedReason; }
            set { closedReason = value; }
        }

        /// <summary>
        /// 用户密码Key
        /// </summary>
        public string PasswordKey
        {
            get { return passwordKey; }
            set { passwordKey = value; }
        }

        /// <summary>
        /// 特殊 IP 注册限制
        /// </summary>
        public string IpRegCtrl
        {
            get { return ipRegCtrl; }
            set { ipRegCtrl = value; }
        }

        /// <summary>
        /// IP禁止访问列表
        /// </summary>
        public string IpDenyAccess
        {
            get { return ipDenyAccess; }
            set { ipDenyAccess = value; }
        }
        /// <summary>
        /// Api IP访问列表
        /// </summary>
        public string ApiIpAccess
        {
            get { return apiIpAccess; }
            set { apiIpAccess = value; }
        }

        /// <summary>
        /// 管理员后台IP访问列表
        /// </summary>
        public string AdminIpAccess
        {
            get { return adminIpAccess; }
            set { adminIpAccess = value; }
        }

        /// <summary>
        /// 敏感关键词过滤
        /// </summary>
        public string FilterKey
        {
            get { return filterKey; }
            set { filterKey = value; }
 
        }

        /// <summary>
        /// 标题后缀
        /// </summary>
        public string SeoTitle
        {
            get { return seoTitle; }
            set { seoTitle = value; }
        }
        /// <summary>
        /// Meta Keywords
        /// </summary>
        public string SeoKeyWords
        {
            get { return seoKeyWords; }
            set { seoKeyWords = value; }
        }

        /// <summary>
        /// Meta Description
        /// </summary>
        public string SeoDescription
        {
            get { return seoDescription; }
            set { seoDescription = value; }
        }
        /// <summary>
        /// 保存方式  0=按年/月/日存入不同目录 1=按年/月/日/存入不同目录 2=按存入不同目录 3=按文件类型存入不同目录
        /// </summary>
        public int AttachSave
        {
            get { return attachSave; }
            set { attachSave = value; }
        }
        /// <summary>
        /// 图片添加水印 0=不使用 1=左上 2=中上 3=右上 4=左中 ... 9=右下
        /// </summary>
        public int WaterMarkStatus
        {
            get { return waterMarkStatus; }
            set { waterMarkStatus = value; }
        }

        /// <summary>
        /// 图片添加何种水印 0=文字 1=图片
        /// </summary>
        public int WaterMarkType
        {
            get { return waterMarkType; }
            set { waterMarkType = value; }
        }

        /// <summary>
        /// 图片水印透明度 取值范围1--10 (10为不透明)
        /// </summary>
        public int WaterMarkTransparency
        {
            get { return waterMarkTransParency; }
            set { waterMarkTransParency = value; }
        }

        /// <summary>
        /// 图片添加文字水印的内容 {1}表示标题 {2}表示地址 {3}表示当前日期 {4}表示当前时间, 例如: {3} {4}上传于{1} {2}
        /// </summary>
        public string WaterMarkText
        {
            get { return waterMarkText; }
            set { waterMarkText = value; }
        }

        /// <summary>
        /// 使用的水印图片的名称
        /// </summary>
        public string WaterMarkPic
        {
            get { return waterMarkPic; }
            set { waterMarkPic = value; }
        }

        /// <summary>
        /// 图片添加文字水印的字体
        /// </summary>
        public string WaterMarkFontName
        {
            get { return waterMarkFontName; }
            set { waterMarkFontName = value; }
        }

        /// <summary>
        /// 图片添加文字水印的大小(像素)
        /// </summary>
        public int WaterMarkFontSize
        {
            get { return waterMarkFontSize; }
            set { waterMarkFontSize = value; }
        }

        /// <summary>
        /// 图片质量
        /// </summary>
        public int AttachImgQuality
        {
            get { return attachImgQuality; }
            set { attachImgQuality = value; }
        }
        /// <summary>
        /// 限制图片最大高度
        /// </summary>
        public int AttachImgMaxHeight
        {
            get { return attachImgMaxHeight; }
            set { attachImgMaxHeight = value; }
        }
        /// <summary>
        /// 限制图片最大宽度
        /// </summary>
        public int AttachImgMaxWidth
        {
            get { return attachImgMaxWidth; }
            set { attachImgMaxWidth = value; }
        }
        /// <summary>
        /// 身份验证Cookie域
        /// </summary>
        public string CookieDomain
        {
            get { return cookieDomain; }
            set { cookieDomain = value; }
        }
        
        /// <summary>
        /// 验证码生成所使用的程序集
        /// </summary>
        public string VerifyImageAssemly
        {
            get { return verifyImageAssemly; }
            set { verifyImageAssemly = value; }
        }
        #endregion

        /// <summary>
        /// 是否开通缓存
        /// </summary>
        public int IsCache
        {
            get { return _iscache; }
            set { _iscache = value; }
        }
        /// <summary>
        /// 是否开启交易
        /// </summary>
        public int EnableTrade
        {
            get { return enableTrade; }
            set { enableTrade = value; }
        }
        /// <summary>
        /// 发布版本
        /// </summary>
        public string PublishVesion
        {
            get { return publishVesion; }
            set { publishVesion = value; }
        }
        /// <summary>
        /// 是否启用分布式文件系统
        /// </summary>
        public int DistributeFileSystem
        {
            get { return distributeFileSystem; }
            set { distributeFileSystem = value; }
        }
        /// <summary>
        /// Ftp启用状态:1=启用
        /// </summary>
        public int FTPEnable
        {
            get { return ftpEnable; }
            set { ftpEnable = value; }
        }
        /// <summary>
        /// FTP临时存储目录
        /// </summary>
        public string FTPTempPath
        {
            get { return ftpTempPath; }
            set { ftpTempPath = value; }
        }
        /// <summary>
        /// FTP临时文件保存格式
        /// </summary>
        public int FTPPathFormat
        {
            get { return ftpPathFomat; }
            set { ftpPathFomat = value; }
        }
        /// <summary>
        /// 备份记录使用的外部类名
        /// </summary>
        public string BakClassName
        {
            get { return bakclassname; }
            set { bakclassname = value; }
        }
    }
}
