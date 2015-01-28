using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class KfBlackList : BaseEntity
    {
        /// <summary>
        /// 被阻止访客Id
        /// </summary>
        public Guid MpUserId { set; get; }

        /// <summary>
        /// 操作工号Id
        /// </summary>
        public Guid KfNumberId { set; get; }

        /// <summary>
        /// 阻止理由
        /// </summary>
        public string Reason { set; get; }

        /// <summary>
        /// 失效时间(不填写表示永久禁言)
        /// </summary>
        public DateTime? EndDate { set; get; }
    }
}