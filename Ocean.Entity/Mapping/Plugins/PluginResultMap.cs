﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="PluginResult.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-22 14:36
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Ocean.Entity;
using Ocean.Entity;

namespace Ocean.Entity
{
    /// <summary>
    /// 实体类-数据表映射——
    /// </summary>    
	public partial class PluginResultMap : EntityTypeConfiguration<PluginResult>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——PluginResult
        /// </summary>
        public PluginResultMap()
        {
			// 主键
            this.HasKey(p => p.Id);
			this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(p => p.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("PluginResult");
			//属性
			this.Property(p => p.MpUserId).HasColumnName("MpUserId");
			this.Property(p => p.PluginId).HasColumnName("PluginId");
			this.Property(p => p.UserName).HasColumnName("UserName");
			this.Property(p => p.Name).HasColumnName("Name");
			this.Property(p => p.Email).HasColumnName("Email");
			this.Property(p => p.Phone).HasColumnName("Phone");
			this.Property(p => p.MobilePhone).HasColumnName("MobilePhone");
			this.Property(p => p.Address).HasColumnName("Address");
			this.Property(p => p.SN).HasColumnName("SN");
			this.Property(p => p.IsUse).HasColumnName("IsUse");
			this.Property(p => p.Summary).HasColumnName("Summary");
			this.Property(p => p.Value).HasColumnName("Value");
        }
    }
}
