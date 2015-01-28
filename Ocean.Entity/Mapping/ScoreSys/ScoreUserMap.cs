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
    public partial class ScoreUserMap : EntityTypeConfiguration<ScoreUser>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——ScoreUser
        /// </summary>
        public ScoreUserMap()
        {
			// 主键
            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(p => p.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("ScoreUser");
			//属性
            this.Property(p => p.MpUserId).HasColumnName("MpUserId");
            this.Property(p => p.ClientPhone).HasColumnName("ClientPhone");
            this.Property(p => p.ClientName).HasColumnName("ClientName");
            this.Property(p => p.RecommendPhone).HasColumnName("RecommendPhone");
            this.Property(p => p.RecommendName).HasColumnName("RecommendName");
            this.Property(p => p.CurrentScore).HasColumnName("CurrentScore");
            this.Property(p => p.RecommendScore).HasColumnName("RecommendScore");
            this.Property(p => p.DealScore).HasColumnName("DealScore");
            this.Property(p => p.LastUpdateDateTime).HasColumnName("LastUpdateDateTime");
            this.Property(p => p.IsEmployee).HasColumnName("IsEmployee");
            this.Property(p => p.RecvName).HasColumnName("RecvName");
            this.Property(p => p.RecvPhone).HasColumnName("RecvPhone");
            this.Property(p => p.RecvAddress).HasColumnName("RecvAddress");
        }
    }
}