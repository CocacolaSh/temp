using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class KfMeeting : BaseEntity
    {
        private ICollection<KfMeetingMessage> _kfMeetingsMessage;

        /// <summary>
        /// 客服工号实体
        /// </summary>
        public virtual KfNumber KfNumber { set; get; }

        /// <summary>
        /// 客服工号Id
        /// </summary>
        public Guid KfNumberId { set; get; }

        /// <summary>
        /// 访客实体
        /// </summary>
        public virtual MpUser MpUser { set; get; }

        /// <summary>
        /// 访客Id
        /// </summary>
        public Guid MpUserId { set; get; }

        /// <summary>
        /// 记录总数
        /// </summary>
        public int RecordCount { set; get; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { set; get; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }

        /// <summary>
        /// 咨询页面
        /// </summary>
        public string MeetingPage { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Explain { set; get; }

        /// <summary>
        /// 是否结束
        /// </summary>
        public bool IsEnd { set; get; }

        /// <summary>
        /// 会话聊天集
        /// </summary>
        public virtual ICollection<KfMeetingMessage> KfMeetingsMessage
        {
            get { return _kfMeetingsMessage ?? (_kfMeetingsMessage = new List<KfMeetingMessage>()); }
            protected set { _kfMeetingsMessage = value; }
        }
    }
}
