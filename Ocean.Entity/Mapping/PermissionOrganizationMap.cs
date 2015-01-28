using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class PermissionOrganizationMap : EntityTypeConfiguration<PermissionOrganization>
    {
        public PermissionOrganizationMap()
        {
            // 主键
            this.HasKey(p => p.Id);

            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(50);
            this.Property(p => p.RootPath)
                .HasMaxLength(500);

            // 表 & 列映射
            this.ToTable("PermissionOrganization");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
        }
    }
}