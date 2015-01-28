using System.Linq;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // 主键
            this.HasKey(p => p.Id);
            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(200);

            // 表 & 列映射
            this.ToTable("Product");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(p => p.Name).HasColumnName("Name");
            this.Property(p => p.CategoryId).HasColumnName("CategoryId");
            this.Property(p => p.CreateDate).HasColumnName("CreateDate");
            // 关系
            this.HasRequired(p => p.Category).WithMany(c=>c.Products).WillCascadeOnDelete(false);
        }
    }
}