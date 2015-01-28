using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Drawing;

namespace Ocean.Core.Plugins.Upload
{
    public interface IUpload
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="uploadFile">上传文件类</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <returns></returns>
        AttachmentInfo UploadFile(HttpPostedFileBase uploadFile,UpFileEntity upfileEntity);
        AttachmentInfo UploadFile(HttpPostedFile uploadFile, UpFileEntity upfileEntity);
        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <param name="uploadFiles">上传文件类集合</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <returns></returns>
        AttachmentInfo[] UploadFiles(IList<HttpPostedFileBase> uploadFiles,UpFileEntity upfileEntity);
        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="contentLength">大小</param>
        /// <param name="clientFileName">客户端文件名</param>
        /// <param name="img">图片类</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <returns></returns>
        AttachmentInfo UploadImage(int contentLength,string clientFileName,Image img, UpFileEntity upfileEntity);
        
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string saveDir, string fileName,int ident,string groupName);
        /// <summary>
        /// 删除本地文件
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        int DeleteImage(string saveDir, string fileName);
        /// <summary>
        /// 删除FTP文件
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        int DeleteImage(string saveDir, string fileName,int ident);
        /// <summary>
        /// 删除分布式存储文件
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string saveDir, string fileName,string groupName);

        /// <summary>
        /// 删除本地文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string fileName,int ident,string groupName);
        /// <summary>
        /// 删除本地文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        int DeleteImage(string fileName);
        /// <summary>
        /// 删除FTP文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        int DeleteImage(string fileName,int ident);
        /// <summary>
        /// 删除分布式存储文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="groupName">分布式组名</param>
        /// <param name="isGroup">无实际意思，为了方法签名不同</param>
        /// <returns></returns>
        int DeleteImage(string fileName,string groupName,bool isGroup);

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="isAddImagesFilename">是否存储为带[images/]的文件夹内</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string fileName, bool isAddImagesFilename, int ident, string groupName);
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="isAddImagesFilename">是否存储为带[images/]的文件夹内</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        int DeleteImage(string fileName, bool isAddImagesFilename, int ident);
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="isAddImagesFilename">是否存储为带[images/]的文件夹内</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string fileName, bool isAddImagesFilename, string groupName);
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="isAddImagesFilename">是否存储为带[images/]的文件夹内</param>
        /// <returns></returns>
        int DeleteImage(string fileName, bool isAddImagesFilename);


        /// <summary>
        /// 删除本地缩略图片
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string saveDir, string fileName, string[] thumbnailSizes,int ident,string groupName);
        /// <summary>
        /// 删除本地缩略图片
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <returns></returns>
        int DeleteImage(string saveDir, string fileName, string[] thumbnailSizes);
        /// <summary>
        /// 删除FTP缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        int DeleteImage(string saveDir, string fileName, string[] thumbnailSizes,int ident);
        /// <summary>
        /// 删除分布式缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string saveDir, string fileName, string[] thumbnailSizes,string groupName);


        /// <summary>
        /// 删除本地缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="isAddImgFilename">是否在images目录下</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string fileName, string[] thumbnailSizes, bool isAddImgFilename,int ident,string groupName);
        /// <summary>
        /// 删除本地缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="isAddImgFilename">是否在images目录下</param>
        /// <returns></returns>
        int DeleteImage(string fileName, string[] thumbnailSizes, bool isAddImgFilename);
        /// <summary>
        /// 删除FTP缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="isAddImgFilename">是否在images目录下</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        int DeleteImage(string fileName, string[] thumbnailSizes, bool isAddImgFilename,int ident);
        /// <summary>
        /// 删除FTP缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="isAddImgFilename">是否在images目录下【暂无用处】</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string fileName, string[] thumbnailSizes, bool isAddImgFilename,string groupName);



        /// <summary>
        /// 删除本地缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string fileName, string[] thumbnailSizes,int ident,string groupName);
        /// <summary>
        /// 删除本地缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <returns></returns>
        int DeleteImage(string fileName, string[] thumbnailSizes);
        /// <summary>
        /// 删除FTP缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        int DeleteImage(string fileName, string[] thumbnailSizes,int ident);
        /// <summary>
        /// 删除分布式缩略图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailSizes">缩略图数组</param>
        /// <param name="groupName">分布式组名</param>
        /// <returns></returns>
        int DeleteImage(string fileName, string[] thumbnailSizes,string groupName);



        /// <summary>
        /// 通过原始图地址获取缩略图地址（不带域，主机头-如：2011/1/11/***.jpg)
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <param name="sizeStr">100_100</param>
        /// <returns></returns>
        string GetOriThumbPath(string saveDir, string fileName, string sizeStr, string thumbPrefix = "_");
        string GetOriThumbPath(string filePath, string sizeStr, string thumbPrefix = "_");
        string GetNoJudgeThumbPath(string filePath, string sizeStr);
        string GetNoJudgeThumbPath(string saveDir, string fileName, string sizeStr, string thumbPrefix = "_");
        /// <summary>
        /// 获取缩略图地址(带域、主机头-如：http://images.juxiangke.com/)
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <param name="sizeStr"></param>
        /// <returns></returns>
        string GetThumbPath(string saveDir, string fileName, string sizeStr, string thumbPrefix = "_");
        /// <summary>
        /// 从缩略图路径获取原图
        /// </summary>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <param name="sizeStr">缩略图大小</param>
        /// <param name="saveDir">中间路径</param>
        /// <param name="isUp">是否加入主机头完整地址</param>
        /// <returns></returns>
        string GetOriPath(string saveDir, string thumbnailPath, string sizeStr, bool isUp, string thumbPrefix="_");
        /// <summary>
        /// 从缩略图路径获取原图
        /// </summary>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <param name="sizeStr">缩略图大小</param>
        /// <param name="saveDir">中间路径</param>
        /// <param name="isUp">是否加入主机头完整地址</param>
        /// <returns></returns>
        string GetOriPath(string thumbnailPath, string sizeStr, bool isUp, string thumbPrefix = "_");
        /// <summary>
        /// 从缩略图路径获取缩略图地址
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <param name="sizeStr">缩略图大小</param>
        /// <param name="otherSizeStr">其他缩略图大小</param>
        /// <param name="isUp">是否加入主机头完整地址</param>
        /// <returns></returns>
        string GetOriPath(string saveDir, string thumbnailPath, string sizeStr, string otherSizeStr, bool isUp, string thumbPrefix = "_");


        #region 获取判断是否存在缩略图
        /// <summary>
        /// 通过原文件判断是否存在缩略图，不存在返回原图
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="filePath">原文件路径</param>
        /// <param name="bakFilePath">分布式文件系统时-备份的路径</param>
        /// <param name="thumbnailSize">缩略图大小格式</param>
        /// <param name="minThumbnailSize">缩略图压缩最小范围格式</param>
        /// <param name="isUp">是否返回完整路径[本地文件系统]</param>
        /// <returns></returns>
        //string GetJudgeThumbnailPath(string saveDir, string filePath, string bakFilePath, string thumbnailSize, string minThumbnailSize, bool isUp);
        //string GetJudgeThumbnailPath(string saveDir, string filePath, string bakFilePath, string thumbnailSize, bool isUp);
        //string GetJudgeThumbnailPath(string filePath, string bakFilePath, string thumbnailSize, bool isUp);
        //string GetJudgeThumbnailPath(string filePath, string bakFilePath, string thumbnailSize);
        //string GetJudgeThumbnailPath(string filePath, string bakFilePath, string thumbnailSize, string minThumbnailSize);
        #endregion



        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileName">原图文件地址</param>
        /// <param name="thumbnailSizes">缩略图字符串</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <returns></returns>
        string MakeThumbnailImage(string fileName, string thumbnailSizes, UpFileEntity upfileEntity, bool isDispose = true);
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileName">原图文件地址</param>
        /// <param name="savePath">保存地址</param>
        /// <param name="thumbnailSizes">缩略图字符串</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <returns></returns>
        string MakeThumbnailImage(string fileName, string savePath, string thumbnailSizes, UpFileEntity upfileEntity, bool isDispose = true);
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="img">原图文件</param>
        /// <param name="savePath">保存地址</param>
        /// <param name="thumbnailSizes">缩略图字符串</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <returns></returns>
        string MakeThumbnailImage(Image img, string savePath, string thumbnailSizes, UpFileEntity upfileEntity, bool isDispose = true);
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="img">原图文件</param>
        /// <param name="thumbnailSizes">缩略图字符串</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <returns></returns>
        string MakeThumbnailImage(Image img, string thumbnailSizes, UpFileEntity upfileEntity, bool isDispose = true);

        #region 获取文件
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="path">远程地址</param>
        /// <param name="ftpIdent">FTP唯一标识</param>
        /// <param name="groupName">组名</param>
        /// <returns>文件字节</returns>
        byte[] GetFile(string path,int ftpIdent, string groupName);
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="param">操作参数类</param>
        /// <returns></returns>
        string DownLoadFile(OperateParam param);
        #endregion

        #region 拷贝文件
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="param">操作参数类</param>
        /// <returns></returns>
        string UploadFile(OperateParam param);
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="param">操作参数类</param>
        string CopyFile(OperateParam param);
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourcePaths">源文件列表</param>
        /// <param name="targetPaths">目标文件列表</param>
        /// <param name="param">操作参数类</param>
        string[] CopyFiles(string[] sourcePaths, string[] targetPaths, OperateParam param);
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="param">操作参数类</param>
        /// <returns></returns>
        string CopyFiles(OperateParam param);
        #endregion

        #region 切图
        /// <summary>
        /// 切图 并上传
        /// </summary>
        /// <param name="param">切图参数类</param>
        /// <returns></returns>
        string Segmentation(SegmentationParam param);
        #endregion

        string PathFormat { get; set; }
    }
}
