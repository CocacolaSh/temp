using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.Model
{
    [Serializable]
    public class MessageModel
    {
        #region 发送者Id
        /// <summary>
        /// 发送者Id
        /// </summary>
        public Guid SendUserId { set; get; }
        #endregion

        #region 接收者Id
        /// <summary>
        /// 接收者Id
        /// </summary>
        public Guid ReceiveUserId { set; get; }
        #endregion

        #region 消息
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { set; get; }
        #endregion

        #region 发送时间(服务端)
        /// <summary>
        /// 发送时间(服务端)
        /// </summary>
        public DateTime SendTimeOfService { set; get; }
        #endregion

        #region 接收时间(服务端)
        /// <summary>
        /// 接收时间(服务端)
        /// </summary>
        public DateTime ReceiveTimeOfService { set; get; }
        #endregion

        #region 是否已被客户端接收
        /// <summary>
        /// 是否已被客户端接收
        /// </summary>
        public bool IsReceive { set; get; }
        #endregion
    }
}