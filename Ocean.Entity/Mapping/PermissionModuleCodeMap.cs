using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class PermissionModuleCodeMap : EntityTypeConfiguration<PermissionModuleCode>
    {
        public PermissionModuleCodeMap()
        {
            // 主键
            this.HasKey(p => p.Id);

            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(20);
            this.Property(p => p.Code)
                .HasMaxLength(30);

            // 表 & 列映射
            this.ToTable("PermissionModuleCode");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
            this.HasRequired(p => p.PermissionModule)
                .WithMany(p => p.PermissionModuleCodes)
                .HasForeignKey(p => p.PermissionModuleId);
        }
    }
}