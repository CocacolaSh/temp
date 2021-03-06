﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="Plugin.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-22 14:36
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    /// <summary>
    /// 实体类—
    /// </summary>    
    public class VoteInfo : BaseEntity
    {
        /// <summary>
        /// 实体类-Plugin
        /// </summary>
        public VoteInfo()
        {
        }

        /// <summary>
        /// 自定义名称
        /// </summary>
        public Guid VoteItemID { set; get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid MpUserId { set; get; }
        public string Remark { set; get; }

        private DateTime votedate;
        /// <summary>
        /// 最近使用日期
        /// </summary>
        public DateTime VoteDate
        {
            set { votedate = value; }
            get
            {
                if (votedate < new DateTime(1900, 1, 1))
                {
                    votedate = new DateTime(1900, 1, 1);
                }
                return votedate;
            }
        }
    }
}
