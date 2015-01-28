using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class AdminLoggerMap : EntityTypeConfiguration<AdminLogger>
    {
        public AdminLoggerMap()
        {
            // 主键
            this.HasKey(p => p.Id);

            // 属性
            this.Property(p => p.AdminName)
                .HasMaxLength(20);
            this.Property(p => p.FromIP)
                .HasMaxLength(20);
            this.Property(p => p.Module);

            // 表 & 列映射
            this.ToTable("AdminLogger");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
        }
    }
}