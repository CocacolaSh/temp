using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class ComplainDTO : BaseDTO
    {
        public ComplainDTO() { }

        public ComplainDTO(Complain complain)
        {
            this.Id = complain.Id;
            this.MpUserId = complain.MpUserId;
            this.CreateDate = complain.CreateDate;
            this.Name = complain.Name;
            this.Phone = complain.Phone;
            this.ContactName = complain.ContactName;
            this.ContactPhone = complain.ContactPhone;
        }
        public Guid MpUserId { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 号码
        /// </summary>
        public string Phone { set; get; }

        /// <summary>
        /// 联系姓名
        /// </summary>
        public string ContactName { set; get; }
        /// <summary>
        /// 联系号码
        /// </summary>
        public string ContactPhone { set; get; }
        
        /// <summary>
        /// 投诉日期开始时间
        /// </summary>
        public DateTime? StartDate { set; get; }

        /// <summary>
        /// 投诉日期结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }

        
    }
}