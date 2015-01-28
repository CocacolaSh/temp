using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using Ocean.Entity.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class EnumDataMap : EntityTypeConfiguration<EnumData>
    {
        public EnumDataMap()
        {
            // 主键
            this.HasKey(p => p.Id);
            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(50);
            this.Property(p => p.Value)
                .HasMaxLength(50);

            // 表 & 列映射
            this.ToTable("EnumData");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
            this.HasRequired(p => p.EnumType)
                .WithMany(p=>p.EnumDatas)
                .HasForeignKey(p => p.EnumTypeId);
        }
    }
}