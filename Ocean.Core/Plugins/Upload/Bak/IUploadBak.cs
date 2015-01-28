using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Plugins.Upload;

namespace Ocean.Core.Plugins.Bak
{
    /// <summary>
    /// 文件上传与记录
    /// </summary>
    public interface IUploadBak
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="attch">附件</param>
        /// <returns></returns>
        int Create(AttachmentInfo attch);
        /// <summary>
        /// 更新缩略图数
        /// </summary>
        /// <param name="id">附件记录</param>
        /// <param name="thumbnail">缩略图【逗号隔】</param>
        /// <returns></returns>
        void Update(int id, string thumbnail);
        /// <summary>
        /// 获取上传的记录列表
        /// </summary>
        /// <param name="siteId">站点ID</param>
        /// <returns></returns>
        IList<AttachmentInfo> GetAttachmentList(int siteId);

        /// <summary>
        /// 获取上传的记录
        /// </summary>
        /// <param name="siteId">站点ID</param>
        /// <param name="url">文件URL</param>
        /// <returns></returns>
        AttachmentInfo GetAttachment(int siteId, string url);
        /// <summary>
        /// 更新引用次数
        /// </summary>
        /// <param name="siteId">站点ID</param>
        /// <param name="id">唯一标识ID</param>
        /// <param name="increaseNum">增加的次数【为负数则为减】</param>
        /// <returns></returns>
        int QuoteAttach(int siteId,int id, int increaseNum);

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="siteId">站点ID</param>
        /// <param name="id">唯一标识ID</param>
        /// <returns></returns>
        int Delete(int siteId,int id);

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="siteId">站点ID</param>
        /// <param name="remoteUrl">文件URL</param>
        /// <returns></returns>
        int Delete(int siteId, string url);
    }
}
