using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class VehicleLicenseMap : EntityTypeConfiguration<VehicleLicense>
    {
        public VehicleLicenseMap()
        {
            // 主键
            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(p => p.CreateDate).HasColumnName("CreateDate");

            // 属性
            this.Property(p => p.MpUserId).HasColumnName("MpUserId");
            this.Property(p => p.PlateNo).HasColumnName("PlateNo");
            this.Property(p => p.VehicleType).HasColumnName("VehicleType");
            this.Property(p => p.Owner).HasColumnName("Owner");
            this.Property(p => p.Address).HasColumnName("Address");
            this.Property(p => p.UseCharacter).HasColumnName("UseCharacter");
            this.Property(p => p.CarModel).HasColumnName("CarModel");
            this.Property(p => p.VIN).HasColumnName("VIN");
            this.Property(p => p.EngineNo).HasColumnName("EngineNo");
            this.Property(p => p.RegisterDate).HasColumnName("RegisterDate");
            this.Property(p => p.IssueDate).HasColumnName("IssueDate");

            // 表 & 列映射
            this.ToTable("VehicleLicense");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
        }
    }
}