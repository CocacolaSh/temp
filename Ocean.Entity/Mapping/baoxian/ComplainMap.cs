using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class ComplainMap : EntityTypeConfiguration<Complain>
    {
        public ComplainMap()
        {
            // 主键
            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(p => p.CreateDate).HasColumnName("CreateDate");

            // 属性
            this.Property(p => p.MpUserId).HasColumnName("MpUserId");
            this.Property(p => p.Name).HasColumnName("Name");
            this.Property(p => p.Phone).HasColumnName("Phone");
            this.Property(p => p.ContactName).HasColumnName("ContactName");
            this.Property(p => p.ContactPhone).HasColumnName("ContactPhone");
            this.Property(p => p.ComplainContent).HasColumnName("ComplainContent");
            this.Property(p => p.ProcessStatus).HasColumnName("ProcessStatus");
            this.Property(p => p.ProcessResult).HasColumnName("ProcessResult");
            this.Property(p => p.ProcessDate).HasColumnName("ProcessDate");

            // 表 & 列映射
            this.ToTable("Complain");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
        }
    }
}