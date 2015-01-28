using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class BaoDanMap : EntityTypeConfiguration<BaoXian>
    {
        public BaoDanMap()
        {
            // 主键
            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(p => p.CreateDate).HasColumnName("CreateDate");

            // 属性
            this.Property(p => p.TouBaoGongSi).HasColumnName("TouBaoGongSi");
            this.Property(p => p.TouBaoRen).HasColumnName("TouBaoRen");
            this.Property(p => p.ChePai).HasColumnName("ChePai");
            this.Property(p => p.QiBaoDate).HasColumnName("QiBaoDate");
            this.Property(p => p.BaoXianQiXian).HasColumnName("BaoXianQiXian");
            this.Property(p => p.HouSiWei).HasColumnName("HouSiWei");
            this.Property(p => p.DengJiDate).HasColumnName("DengJiDate");
            this.Property(p => p.BaoXianFei).HasColumnName("BaoXianFei");
            this.Property(p => p.CheChuanSui).HasColumnName("CheChuanSui");
            this.Property(p => p.LaiYuan).HasColumnName("LaiYuan");
            this.Property(p => p.XianZhong).HasColumnName("XianZhong");
            this.Property(p => p.VendorPic1).HasColumnName("VendorPic1");
            this.Property(p => p.VendorPic2).HasColumnName("VendorPic2");

            // 表 & 列映射
            this.ToTable("BaoDan");
            this.Property(p => p.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // 关系
        }
    }
}