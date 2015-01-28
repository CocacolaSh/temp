using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.Model
{
    [Serializable]
    public class NoticeModel
    {
        #region 唯一标识
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Hash { set; get; }
        #endregion

        #region 发送者Id
        /// <summary>
        /// 发送者Id
        /// </summary>
        public Guid SendUserId { set; get; }
        #endregion

        #region 发送者昵称
        /// <summary>
        /// 发送者昵称
        /// </summary>
        public string SendNickName { set; get; }
        #endregion

        #region 接收者Id
        /// <summary>
        /// 接收者Id
        /// </summary>
        public Guid ReceiveUserId { set; get; }
        #endregion

        #region 通知类型
        /// <summary>
        /// 通知类型[1:向所有在线客服发送接入请求,2:向其他客服发送当前客户已经接管的通知,3:访客结束会话,4:向其他客服发送当前客户已经拒绝的通知]
        /// </summary>
        public int NoticeType { set; get; }
        #endregion

        #region 当前在线状态
        /// <summary>
        /// 当前在线状态
        /// </summary>
        public int Status { set; get; }
        #endregion

        #region 最后活动时间
        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActiveDate { set; get; }
        #endregion

        #region 附带参数
        /// <summary>
        /// 附带参数
        /// </summary>
        public string Args { set; get; }
        #endregion
    }
}