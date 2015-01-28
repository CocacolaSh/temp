using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class KfMeetingMessage : BaseEntity
    {
        /// <summary>
        /// 会话Id
        /// </summary>
        public Guid KfMeetingId { set; get; }

        /// <summary>
        /// 会话实体
        /// </summary>
        public virtual KfMeeting KfMeeting { set; get; }

        /// <summary>
        /// 发送方[1:客服,2:访客]
        /// </summary>
        public int WhoIsSend { set; get; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageContent { set; get; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { set; get; }
    }
}