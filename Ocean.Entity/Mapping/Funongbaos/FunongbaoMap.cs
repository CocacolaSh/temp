﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="Funongbao.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-07 14:21
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
	public partial class FunongbaoMap : EntityTypeConfiguration<Funongbao>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——Funongbao
        /// </summary>
        public FunongbaoMap()
        {
			// 主键
            this.HasKey(f => f.Id);
			this.Property(f => f.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(f => f.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("Funongbao");
			//属性
			this.Property(f => f.MpUserId).HasColumnName("MpUserId");
			this.Property(f => f.OpenId).HasColumnName("OpenId");
            this.Property(f => f.PassportType).HasColumnName("PassportType");
			this.Property(f => f.PassportNO).HasColumnName("PassportNO");
			this.Property(f => f.Name).HasColumnName("Name");
			this.Property(f => f.MobilePhone).HasColumnName("MobilePhone");
			this.Property(f => f.FunongbaoNO).HasColumnName("FunongbaoNO");
			this.Property(f => f.BankNO).HasColumnName("BankNO");
            this.Property(f => f.GroupLimit).HasColumnName("GroupLimit");
			this.Property(f => f.CurrentLimit).HasColumnName("CurrentLimit");
			this.Property(f => f.CurrentRates).HasColumnName("CurrentRates");
			this.Property(f => f.GroupNO).HasColumnName("GroupNO");
			this.Property(f => f.IsAuth).HasColumnName("IsAuth");
			this.Property(f => f.IsSignAgreement).HasColumnName("IsSignAgreement");
			this.Property(f => f.ProcessStatus).HasColumnName("ProcessStatus");
			this.Property(f => f.ProcessResult).HasColumnName("ProcessResult");
            this.Property(f => f.FinishDate).HasColumnName("FinishDate");
            this.Property(f => f.PreFinishDate).HasColumnName("PreFinishDate");
			this.Property(f => f.Subbranch).HasColumnName("Subbranch");
			this.Property(f => f.Marketer).HasColumnName("Marketer");
        }
    }
}