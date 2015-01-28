using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class DrivingLicenseMap : EntityTypeConfiguration<DrivingLicense>
    {
        public DrivingLicenseMap()
        {
            // 主键
            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(p => p.CreateDate).HasColumnName("CreateDate");

            // 属性
            this.Property(p => p.MpUserId).HasColumnName("MpUserId");
            this.Property(p => p.CertNo).HasColumnName("CertNo");
            this.Property(p => p.Name).HasColumnName("Name");
            this.Property(p => p.Sex).HasColumnName("Sex");
            this.Property(p => p.Nationality).HasColumnName("Nationality");
            this.Property(p => p.Address).HasColumnName("Address");
            this.Property(p => p.Birthday).HasColumnName("Birthday");
            this.Property(p => p.IssueDate).HasColumnName("IssueDate");
            this.Property(p => p.Class).HasColumnName("Class");
            this.Property(p => p.ValidFrom).HasColumnName("ValidFrom");
            this.Property(p => p.ValidFor).HasColumnName("ValidFor");

            // 表 & 列映射
            this.ToTable("DrivingLicense");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
        }
    }
}