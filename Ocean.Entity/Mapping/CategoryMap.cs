using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            // 主键
            this.HasKey(c => c.Id);
            // 属性
            this.Property(c => c.Name)
                .HasMaxLength(200);

            // 表 & 列映射
            this.ToTable("Category");
            this.Property(c => c.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(c => c.Name).HasColumnName("Name");
            this.Property(c => c.CreateDate).HasColumnName("CreateDate");

            // 关系
        }
    }
}