﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpCenter.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-01-28 16:24
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
	public class MpCenter : BaseEntity
    {
        /// <summary>
        /// 实体类-MpCenter
        /// </summary>
        public MpCenter()
        {
        }
		
		/// <summary>
		/// 
		/// </summary>
		public string MpName { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string MpHeadImg { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string OriginID { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string AppID { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string AppSecret { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string AccessToken { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string Token { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public int MpType { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? UpdateDate { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? GetTokenDate { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public Guid? CreateUser { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public Guid? UpdateUser { set; get; }
    }
}
