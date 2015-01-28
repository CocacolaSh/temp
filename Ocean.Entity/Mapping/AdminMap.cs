using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class AdminMap : EntityTypeConfiguration<Admin>
    {
        public AdminMap()
        {
            // 主键
            this.HasKey(p => p.Id);

            // 属性
            this.Property(p => p.Name)
                .HasMaxLength(20);
            this.Property(p => p.Password)
                .HasMaxLength(50);
            this.Property(p => p.PasswordKey)
                .HasMaxLength(10);
            this.Property(p => p.LastLoginIP)
                .HasMaxLength(20);

            // 表 & 列映射
            this.ToTable("Admin");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
            this.HasRequired(p => p.PermissionOrganization)
                .WithMany(p => p.Admins)    
                //.WillCascadeOnDelete(false);
                .HasForeignKey(p => p.PermissionOrganizationId);

            this.HasRequired(p => p.PermissionRole)
                .WithMany(p => p.Admins)
                //.WillCascadeOnDelete(false);
                .HasForeignKey(p => p.PermissionRoleId);



            this.HasRequired(p => p.MpUser)
                .WithMany(p => p.Admins)
                //.WillCascadeOnDelete(false);
                .HasForeignKey(p => p.MpUserId);
        }
    }
}