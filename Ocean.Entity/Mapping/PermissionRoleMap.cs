using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace Ocean.Entity.Mapping
{
    public partial class PermissionRoleMap : EntityTypeConfiguration<PermissionRole>
    {
        public PermissionRoleMap()
        {
            // 主键
            this.HasKey(p => p.Id);

            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(50);
            this.Property(p => p.Permissions);

            // 表 & 列映射
            this.ToTable("PermissionRole");

            // 关系
        }
    }
}