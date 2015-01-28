using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    /// <summary>
    /// 贷款分配记录
    /// </summary>
    public class LoanAssignLogger : BaseEntity
    {
        public LoanAssignLogger()
        {
        }

        /// <summary>
        /// 分配支行
        /// </summary>
        public string AssignSubbranch { set; get; }

        /// <summary>
        /// 分配客户经理
        /// </summary>
        public string AssignCustomerManager { set; get; }

        /// <summary>
        /// 客户经理手机
        /// </summary>
        public string CustomerManagerPhone { set; get; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifyName { set; get; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { set; get; }

        /// <summary>
        /// 贷款Id
        /// </summary>
        public Guid? LoanId { set; get; }

        /// <summary>
        /// 贷款
        /// </summary>
        public virtual Loan Loan { set; get; }
    }
}