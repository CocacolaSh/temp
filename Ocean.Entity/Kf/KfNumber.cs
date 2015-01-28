using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class KfNumber : BaseEntity
    {
        private ICollection<KfMeeting> _kfMeetings;

        /// <summary>
        /// 工号
        /// </summary>
        public string Number { set; get; }

        /// <summary>
        /// 客服昵称
        /// </summary>
        public string NickName { set; get; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline { set; get; }

        /// <summary>
        /// 在线状态[0:正常,1:离开]
        /// </summary>
        public int OnlineStatus { set; get; }

        /// <summary>
        /// 管理员外键Id
        /// </summary>
        public Guid AdminId { set; get; }

        /// <summary>
        /// 管理员实体
        /// </summary>
        public virtual Admin Admin { set; get; }

        /// <summary>
        /// 客服会话集
        /// </summary>
        public virtual ICollection<KfMeeting> KfMeetings
        {
            get { return _kfMeetings ?? (_kfMeetings = new List<KfMeeting>()); }
            protected set { _kfMeetings = value; }
        }
    }
}