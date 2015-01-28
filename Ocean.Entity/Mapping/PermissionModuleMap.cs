using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class PermissionModuleMap : EntityTypeConfiguration<PermissionModule>
    {
        public PermissionModuleMap()
        {
            // 主键
            this.HasKey(p => p.Id);

            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(20);
            this.Property(p => p.Url)
                .HasMaxLength(100);
            this.Property(p => p.Identifying)
                .HasMaxLength(30);
            this.Property(p => p.RootPath)
                .HasMaxLength(200);

            // 表 & 列映射
            this.ToTable("PermissionModule");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
        }
    }
}