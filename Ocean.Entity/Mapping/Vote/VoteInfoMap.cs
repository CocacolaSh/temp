﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="PluginAllowUser.cs">
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
    public partial class VoteInfoMap : EntityTypeConfiguration<VoteInfo>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——VoteInfoMap
        /// </summary>
        public VoteInfoMap()
        {
			// 主键
            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(p => p.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("VoteInfo");
			//属性
            this.Property(p => p.VoteItemID).HasColumnName("VoteItemID");
            this.Property(p => p.MpUserId).HasColumnName("MpUserId");
            this.Property(p => p.Remark).HasColumnName("Remark");
            this.Property(p => p.VoteDate).HasColumnName("VoteDate");
        }
    }
}
