﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpMaterial.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-08 17:25
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Ocean.Entity;

namespace Ocean.Entity.Mapping
{
    /// <summary>
    /// 实体类-数据表映射——
    /// </summary>    
	public partial class MpMaterialMap : EntityTypeConfiguration<MpMaterial>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——MpMaterial
        /// </summary>
        public MpMaterialMap()
        {
			// 主键
            this.HasKey(m => m.Id);
			this.Property(m => m.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(m => m.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("MpMaterial");
			//属性
			this.Property(m => m.MpID).HasColumnName("MpID");
			this.Property(m => m.IsDynamic).HasColumnName("IsDynamic");
			this.Property(m => m.DynamicType).HasColumnName("DynamicType");
            this.Property(m => m.ApiUrl).HasColumnName("ApiUrl");
            this.Property(m => m.IsMul).HasColumnName("IsMul");
			this.Property(m => m.PostData).HasColumnName("PostData");
			this.Property(m => m.MedeaID).HasColumnName("MedeaID");
            this.Property(m => m.TypeID).HasColumnName("TypeID");
            this.Property(m => m.IsChat).HasColumnName("IsChat");
			this.Property(m => m.TypeName).HasColumnName("TypeName");
			this.Property(m => m.LastMedeaDate).HasColumnName("LastMedeaDate");
			this.Property(m => m.UpateDate).HasColumnName("UpateDate");
			this.Property(m => m.CreateUser).HasColumnName("CreateUser");
			this.Property(m => m.UpdateUser).HasColumnName("UpdateUser");
        }
    }
}
