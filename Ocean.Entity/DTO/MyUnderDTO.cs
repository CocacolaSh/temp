﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class MyUnderDTO : BaseDTO
    {
        public MyUnderDTO() { }

        public MyUnderDTO(MpUser user)
        {
            this.Id = user.Id;
            this.CreateDate = user.CreateDate;
            this.Name = user.Name;
            this.Phone = user.MobilePhone;
            this.IsAuth = user.IsAuth;
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 号码
        /// </summary>
        public string Phone { set; get; }

        /// <summary>
        /// 是否身份认证
        /// </summary>
        public int ?IsAuth { get; set; }
        /// <summary>
        /// 上传的MP
        /// </summary>
        public Guid ?MpUserId { set; get; }
        
    }
}