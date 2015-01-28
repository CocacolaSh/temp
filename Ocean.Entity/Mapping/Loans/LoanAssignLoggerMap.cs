using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ocean.Entity.Mapping
{
    public partial class LoanAssignLoggerMap : EntityTypeConfiguration<LoanAssignLogger>
    {
        /// <summary>
        /// 实体类-数据表映射构造函数——LoanAssignLogger
        /// </summary>
        public LoanAssignLoggerMap()
        {
            // 主键
            this.HasKey(l => l.Id);
            this.Property(l => l.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(l => l.CreateDate).HasColumnName("CreateDate");
            // 表 & 列映射
            this.ToTable("LoanAssignLogger");
            //属性
            this.Property(l => l.AssignSubbranch).HasColumnName("AssignSubbranch").HasMaxLength(250);
            this.Property(l => l.AssignCustomerManager).HasColumnName("AssignCustomerManager").HasMaxLength(50);
            this.Property(l => l.CustomerManagerPhone).HasColumnName("CustomerManagerPhone").HasMaxLength(20);
            this.Property(l => l.ModifyName).HasColumnName("ModifyName").HasMaxLength(20);
            this.Property(l => l.ModifyDate).HasColumnName("ModifyDate");
            // 关系
            this.HasRequired(l => l.Loan)
                .WithMany(l => l.LoanAssignLogger)
                //.WillCascadeOnDelete(false);
                .HasForeignKey(l => l.LoanId);
        }
    }
}
