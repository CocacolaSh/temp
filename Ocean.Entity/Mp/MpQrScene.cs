﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpQrScene.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-03-03 22:19
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
	public class MpQrScene : BaseEntity
    {
        /// <summary>
        /// 实体类-MpQrScene
        /// </summary>
        public MpQrScene()
        {
        }
		
		/// <summary>
		/// 
		/// </summary>
		public string Ticket { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string Title { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string Description { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public int SceneId { set; get; }

        public int TimeoutTicks { set; get; }
        public int ActionName { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string ImgUrl { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public Guid UpdateUser { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public Guid CreateUser { set; get; }

		private DateTime updatedate;
		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateDate
		{
			set { updatedate = value; }
			get
			{
				if (updatedate < new DateTime(1900, 1, 1))
				{
					updatedate = new DateTime(1900, 1, 1);
				}
				return updatedate;
			}
		}
    }
}
