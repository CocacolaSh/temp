using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Configuration;

namespace Ocean.Core.Plugins.DFS
{
    public interface IDFS
    {
        /// <summary>
        /// 附加文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="contentByte">文件内容</param>
        void AppendFile(string fileName, byte[] contentByte);
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件内容</returns>
        byte[] DownloadFile(string fileName);
        /// <summary>
        /// 增量下载文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="offset">从文件起始点的偏移量</param>
        /// <param name="length">要读取的字节数</param>
        /// <returns>文件内容</returns>
        byte[] DownloadFile(string fileName, long offset, long length);
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        IDFSFileInfo GetFileInfo(string fileName);
        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        string UploadAppenderFile(byte[] contentByte, string fileExt);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        string UploadFile(byte[] contentByte, string fileExt);
        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="contentByte">文件内容</param>
        /// <param name="master_filename">主文件名</param>
        /// <param name="prefix_name">从文件后缀</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        string UploadSlaveFile(byte[] contentByte, string master_filename, string prefix_name, string fileExt);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        bool DeleteFile(string fileName);

        /// <summary>
        /// 配置文件
        /// </summary>
        IConfigInfo DfsConfig { get; set; }
    }
}
