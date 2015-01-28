﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpUserGroup.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-10 13:44
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Ocean.Entity;

namespace Ocean.Entity.Mapping
{
    /// <summary>
    /// 实体类-数据表映射——
    /// </summary>    
	public partial class MpUserGroupMap : EntityTypeConfiguration<MpUserGroup>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——MpUserGroup
        /// </summary>
        public MpUserGroupMap()
        {
			// 主键
            this.HasKey(m => m.Id);
			this.Property(m => m.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(m => m.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("MpUserGroup");
			//属性
			this.Property(m => m.Name).HasColumnName("Name");
			this.Property(m => m.Description).HasColumnName("Description");
			this.Property(m => m.GID).HasColumnName("GID");
			this.Property(m => m.OrderIndex).HasColumnName("OrderIndex");
			this.Property(m => m.IsSystem).HasColumnName("IsSystem");
			this.Property(m => m.UpdateDate).HasColumnName("UpdateDate");
			this.Property(m => m.CreateUser).HasColumnName("CreateUser");
			this.Property(m => m.UpdateUser).HasColumnName("UpdateUser");
        }
    }
}