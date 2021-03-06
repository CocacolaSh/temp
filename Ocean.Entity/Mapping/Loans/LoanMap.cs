﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="Loan.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-01-26 13:51
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
    public partial class LoanMap : EntityTypeConfiguration<Loan>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——Loan
        /// </summary>
        public LoanMap()
        {
            // 主键
            this.HasKey(l => l.Id);
            this.Property(l => l.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(l => l.CreateDate).HasColumnName("CreateDate");
            // 表 & 列映射
            this.ToTable("Loan");
            //属性
            this.Property(l => l.MpUserId).HasColumnName("MpUserId");
            this.Property(l => l.LoanName).HasColumnName("LoanName").HasMaxLength(30);
            this.Property(l => l.Phone).HasColumnName("Phone").HasMaxLength(20);
            this.Property(l => l.Address).HasColumnName("Address").HasMaxLength(250);
            this.Property(l => l.Company).HasColumnName("Company").HasMaxLength(250);
            this.Property(l => l.Position).HasColumnName("Position").HasMaxLength(250);
            this.Property(l => l.LoanCompanyId).HasColumnName("LoanCompanyId");
            this.Property(l => l.LoanFormalId).HasColumnName("LoanFormalId");
            this.Property(l => l.LoanAssetHourseId).HasColumnName("LoanAssetHourseId");
            this.Property(l => l.ApplyMoney).HasColumnName("ApplyMoney");
            this.Property(l => l.LoanCategoryId).HasColumnName("LoanCategoryId");
            this.Property(l => l.DeadlineId).HasColumnName("DeadlineId");
            this.Property(l => l.RepaymentModeId).HasColumnName("RepaymentModeId");
            this.Property(l => l.GuaranteeWayId).HasColumnName("GuaranteeWayId");
            this.Property(l => l.AssetSituation).HasColumnName("AssetSituation").HasMaxLength(100);
            this.Property(l => l.RepealDate).HasColumnName("RepealDate");
            this.Property(l => l.Status).HasColumnName("Status");
            this.Property(l => l.AssignStatus).HasColumnName("AssignStatus");
            this.Property(l => l.AssignSubbranch).HasColumnName("AssignSubbranch").HasMaxLength(250);
            this.Property(l => l.AssignCustomerManager).HasColumnName("AssignCustomerManager").HasMaxLength(50);
            this.Property(l => l.CustomerManagerPhone).HasColumnName("CustomerManagerPhone").HasMaxLength(20);
            this.Property(l => l.ProcessStatus).HasColumnName("ProcessStatus");
            this.Property(l => l.ProcessResult).HasColumnName("ProcessResult").HasMaxLength(500);
            this.Property(l => l.ProcessDate).HasColumnName("ProcessDate");
            this.Property(l => l.ReturnVisitDate).HasColumnName("ReturnVisitDate");
            this.Property(l => l.ReturnVisitResult).HasColumnName("ReturnVisitResult").HasMaxLength(500);
            this.Property(l => l.ReturnStatus).HasColumnName("ReturnStatus");
            //关系
            this.HasRequired(l => l.MpUser).WithMany(m => m.Loans)
                //.WillCascadeOnDelete(false);
                .HasForeignKey(l => l.MpUserId);
        }
    }
}