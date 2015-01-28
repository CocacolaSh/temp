﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MobileCode.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-07 12:31
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
	public class MobileCode : BaseEntity
    {
        /// <summary>
        /// 实体类-MobileCode
        /// </summary>
        public MobileCode()
        {
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid MpUserId { set; get; }
		
		/// <summary>
		/// 手机
		/// </summary>
		public string Mobile { set; get; }

		/// <summary>
		/// 验证码
		/// </summary>
		public string Code { set; get; }

        /// <summary>
        /// 次数
        /// </summary>
        public int Time { set; get; }

        /// <summary>
        /// 操作类型[1:福农宝，2:贷款]
        /// </summary>
        public int OperationType { set; get; }

        /// <summary>
        /// 使用状态
        /// </summary>
        public int Status { set; get; }
    }
}