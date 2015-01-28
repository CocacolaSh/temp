﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="PosApply.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-05-10 17:57
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
	public partial class PosApplyMap : EntityTypeConfiguration<PosApply>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——PosApply
        /// </summary>
        public PosApplyMap()
        {
			// 主键
            this.HasKey(p => p.Id);
			this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(p => p.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("PosApply");
			//属性
			this.Property(p => p.MpUserId).HasColumnName("MpUserId");
			this.Property(p => p.PosAuthId).HasColumnName("PosAuthId");
			this.Property(p => p.VendorAddress).HasColumnName("VendorAddress");
			this.Property(p => p.VendorPic).HasColumnName("VendorPic");
			this.Property(p => p.VendorName).HasColumnName("VendorName");
			this.Property(p => p.InstallAddress).HasColumnName("InstallAddress");
			this.Property(p => p.DeductRate).HasColumnName("DeductRate");
			this.Property(p => p.RepealDate).HasColumnName("RepealDate");
			this.Property(p => p.Status).HasColumnName("Status");
			this.Property(p => p.AssignStatus).HasColumnName("AssignStatus");
			this.Property(p => p.AssignSubbranch).HasColumnName("AssignSubbranch");
			this.Property(p => p.AssignCustomerManager).HasColumnName("AssignCustomerManager");
			this.Property(p => p.CustomerManagerPhone).HasColumnName("CustomerManagerPhone");
			this.Property(p => p.ProcessStatus).HasColumnName("ProcessStatus");
			this.Property(p => p.ProcessResult).HasColumnName("ProcessResult");
			this.Property(p => p.ProcessDate).HasColumnName("ProcessDate");
			this.Property(p => p.ReturnStatus).HasColumnName("ReturnStatus");
			this.Property(p => p.ReturnVisitDate).HasColumnName("ReturnVisitDate");
			this.Property(p => p.ReturnVisitResult).HasColumnName("ReturnVisitResult");
        }
    }
}
