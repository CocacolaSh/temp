using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class AdminLoggerDTO : BaseDTO
    {
        public AdminLoggerDTO()
        {
        }

        public AdminLoggerDTO(AdminLogger adminLogger)
        {
            this.AdminName = adminLogger.AdminName;
            this.FromIP = adminLogger.FromIP;
            this.Description = adminLogger.Description;
        }

        /// <summary>
        /// 申请开始时间
        /// </summary>
        public DateTime? StartDate { set; get; }
        
        /// <summary>
        /// 申请结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }

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
        public int? Module { set; get; }
    }
}
