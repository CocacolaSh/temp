using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class KfMessage : BaseEntity
    {
        /// <summary>
        /// 访客Id
        /// </summary>
        public Guid MpUserId { set; get; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { set; get; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }

        /// <summary>
        /// 留言内容
        /// </summary>
        public string MessageContent { set; get; }

        /// <summary>
        /// 是否处理
        /// </summary>
        public bool IsDeal { set; get; }
    }
}