﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpDiyMenu.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-20 09:39
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
	public partial class MpDiyMenuMap : EntityTypeConfiguration<MpDiyMenu>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——MpDiyMenu
        /// </summary>
        public MpDiyMenuMap()
        {
			// 主键
            this.HasKey(m => m.Id);
			this.Property(m => m.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(m => m.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("MpDiyMenu");
			//属性
			this.Property(m => m.MpID).HasColumnName("MpID");
			this.Property(m => m.ParentID).HasColumnName("ParentID");
			this.Property(m => m.BtnName).HasColumnName("BtnName");
			this.Property(m => m.BtnType).HasColumnName("BtnType");
			this.Property(m => m.BtnKey).HasColumnName("BtnKey");
			this.Property(m => m.IsDeleted).HasColumnName("IsDeleted");
			this.Property(m => m.MaterialID).HasColumnName("MaterialID");
			this.Property(m => m.SortIndex).HasColumnName("SortIndex");
			this.Property(m => m.CreateUser).HasColumnName("CreateUser");
			this.Property(m => m.UpdateDate).HasColumnName("UpdateDate");
			this.Property(m => m.UpdateUser).HasColumnName("UpdateUser");
        }
    }
}
