using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class AdminLogger : BaseEntity
    {
        /// <summary>
        /// 管理员用户名
        /// </summary>
        public string AdminName { set; get; }

        /// <summary>
        /// 来源IP
        /// </summary>
        public string FromIP { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 操作模块
        /// </summary>
        public int Module { set; get; }
    }
}