﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class KfBlackListMap : EntityTypeConfiguration<KfBlackList>
    {
        public KfBlackListMap()
        {
			// 主键
            this.HasKey(m => m.Id);
			this.Property(m => m.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(m => m.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("KfBlackList");
			//属性
        }
    }
}