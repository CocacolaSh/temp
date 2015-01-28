using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class KfMeetingMap : EntityTypeConfiguration<KfMeeting>
    {
        public KfMeetingMap()
        {
			// 主键
            this.HasKey(m => m.Id);
			this.Property(m => m.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(m => m.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("KfMeeting");
			//属性
            //关系
            this.HasRequired(m => m.KfNumber)
                .WithMany(a => a.KfMeetings)
                //.WillCascadeOnDelete(false);
                .HasForeignKey(m => m.KfNumberId);
            this.HasRequired(m => m.MpUser)
                .WithMany(a => a.KfMeetings)
                //.WillCascadeOnDelete(false);
                .HasForeignKey(m => m.MpUserId);
        }
    }
}