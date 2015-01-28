using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace Ocean.Entity.Mapping
{
    public partial class AreaMap : EntityTypeConfiguration<Area>
    {
        public AreaMap()
        {
            // 主键
            this.HasKey(p => p.Id);

            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(20);

            // 表 & 列映射
            this.ToTable("Area");

            // 关系
        }
    }
}