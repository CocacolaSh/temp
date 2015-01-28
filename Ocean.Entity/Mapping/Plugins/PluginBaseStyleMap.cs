﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="PluginBaseStyle.cs">
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
	public partial class PluginBaseStyleMap : EntityTypeConfiguration<PluginBaseStyle>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——PluginBaseStyle
        /// </summary>
        public PluginBaseStyleMap()
        {
			// 主键
            this.HasKey(p => p.Id);
			this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(p => p.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("PluginBaseStyle");
			//属性
			this.Property(p => p.PluginBaseId).HasColumnName("PluginBaseId");
			this.Property(p => p.Name).HasColumnName("Name");
			this.Property(p => p.Folder).HasColumnName("Folder");
			this.Property(p => p.IdentCode).HasColumnName("IdentCode");
			this.Property(p => p.SourcePic).HasColumnName("SourcePic");
			this.Property(p => p.PreviewPic).HasColumnName("PreviewPic");
			this.Property(p => p.Pic).HasColumnName("Pic");
			this.Property(p => p.IsPass).HasColumnName("IsPass");
			this.Property(p => p.IsDefault).HasColumnName("IsDefault");
			this.Property(p => p.IsMulti).HasColumnName("IsMulti");
        }
    }
}