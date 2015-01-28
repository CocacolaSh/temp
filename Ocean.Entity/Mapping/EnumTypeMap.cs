using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using Ocean.Entity.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class EnumTypeMap : EntityTypeConfiguration<EnumType>
    {
        public EnumTypeMap()
        {
            // 主键
            this.HasKey(p => p.Id);
            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(50);
            this.Property(p => p.Identifying)
               .HasMaxLength(30);
            this.Property(p => p.Remark)
                .HasMaxLength(250);

            // 表 & 列映射
            this.ToTable("EnumType");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
        }
    }
}