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
	public class Plugin : BaseEntity
    {
        /// <summary>
        /// 实体类-Plugin
        /// </summary>
        public Plugin()
        {
        }
		
		/// <summary>
		/// 关联插件ID(用户免费与收费只存在一种)
		/// </summary>
		public Guid PluginBaseId { set; get; }

		/// <summary>
		/// 自定义名称
		/// </summary>
		public string Name { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsShow { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public int IsOpen { set; get; }

		/// <summary>
		/// 插件类型
		/// </summary>
		public Guid CategoryId { set; get; }

		/// <summary>
		/// 语言版本ID
		/// </summary>
		public int LanguageId { set; get; }

		/// <summary>
		/// 风格ID
		/// </summary>
        public Guid StyleId { set; get; }

		/// <summary>
		/// 样式的文件名
		/// </summary>
		public string StyleFolder { set; get; }

		/// <summary>
		/// 配置值(xml存储)
		/// </summary>
		public string Value { set; get; }

		/// <summary>
		/// 排序
		/// </summary>
		public int Sort { set; get; }

		private DateTime startdate;
		/// <summary>
		/// 
		/// </summary>
		public DateTime StartDate
		{
			set { startdate = value; }
			get
			{
				if (startdate < new DateTime(1900, 1, 1))
				{
					startdate = new DateTime(1900, 1, 1);
				}
				return startdate;
			}
		}

		private DateTime enddate;
		/// <summary>
		/// 
		/// </summary>
		public DateTime EndDate
		{
			set { enddate = value; }
			get
			{
				if (enddate < new DateTime(1900, 1, 1))
				{
					enddate = new DateTime(1900, 1, 1);
				}
				return enddate;
			}
		}

        //关系
        /// <summary>
        /// 样式｜插件展示方式
        /// </summary>
        public virtual PluginBaseStyle RPluginBaseStyle { get; set; }
        /// <summary>
        /// 插件基础表
        /// </summary>
        public virtual PluginBase RPluginBase { get; set; }
    }
}
