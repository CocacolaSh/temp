using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class KfMeetingMessageMap : EntityTypeConfiguration<KfMeetingMessage>
    {
        public KfMeetingMessageMap()
        {
			// 主键
            this.HasKey(m => m.Id);
			this.Property(m => m.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(m => m.CreateDate).HasColumnName("CreateDate");
			// 表 & 列映射
            this.ToTable("KfMeetingMessage");
			//属性
            //关系
            this.HasRequired(m => m.KfMeeting)
                .WithMany(a => a.KfMeetingsMessage)
                //.WillCascadeOnDelete(false);
                .HasForeignKey(m => m.KfMeetingId);
        }
    }
}