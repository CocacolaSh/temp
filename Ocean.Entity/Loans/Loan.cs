﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="Loan.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-01-26 09:55
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;
using Newtonsoft.Json;

namespace Ocean.Entity
{
    /// <summary>
    /// 实体类—
    /// </summary>    
	public class Loan : BaseEntity
    {
        /// <summary>
        /// 实体类-Loan
        /// </summary>
        public Loan()
        {
        }

        private ICollection<LoanAssignLogger> _loanAssignLogger;

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid MpUserId { set; get; }

        /// <summary>
        /// 微信用户
        /// </summary>
        [JsonIgnore]
        public virtual MpUser MpUser { set; get; } 

		/// <summary>
		/// 贷款人姓名
		/// </summary>
		public string LoanName { set; get; }

		/// <summary>
		/// 手机号码
		/// </summary>
		public string Phone { set; get; }

        /// <summary>
        /// 贷款人地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 贷款人工作单位
        /// </summary>
        public string Company { set; get; }
        /// <summary>
        /// 贷款人工作岗位
        /// </summary>
        public string Position { set; get; }
        /// <summary>
        /// 企业类型
        /// </summary>
        public int? LoanCompanyId { set; get; }/// <summary>
        /// 编制类型
        /// </summary>
        public int? LoanFormalId { set; get; }/// <summary>
        /// 自有房产
        /// </summary>
        public int? LoanAssetHourseId { set; get; }
		/// <summary>
		/// 申请金额
		/// </summary>
		public decimal ApplyMoney { set; get; }

		/// <summary>
		/// 贷款种类
		/// </summary>
		public int? LoanCategoryId { set; get; }

		/// <summary>
        /// 贷款期限
		/// </summary>
		public int? DeadlineId { set; get; }

		/// <summary>
		/// 还款方式
		/// </summary>
		public int? RepaymentModeId { set; get; }

		/// <summary>
		/// 担保方式
		/// </summary>
		public int? GuaranteeWayId { set; get; }

		/// <summary>
		/// 资产情况
		/// </summary>
		public string AssetSituation { set; get; }

		/// <summary>
		/// 撤消日期
		/// </summary>
		public DateTime? RepealDate { set; get; }

		/// <summary>
		/// 申请状态［0-正常，1-被撤销］
		/// </summary>
		public int Status { set; get; }

        /// <summary>
        /// 分配状态［0-无任何处理，1-已分配］
        /// </summary>
        public int AssignStatus { set; get; }

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
        /// 受理状态［0-未受理，1-已通过，2-未通过，3-受理中］（管理员拒绝用户请求也是改变此状态，把受理状态置成未通过即标识拒绝）
		/// </summary>
		public int ProcessStatus { set; get; }

		/// <summary>
		/// 受理结果
		/// </summary>
		public string ProcessResult { set; get; }

        /// <summary>
        /// 受理日期
        /// </summary>
        public DateTime? ProcessDate { set; get; }

		/// <summary>
		/// 回访日期
		/// </summary>
		public DateTime? ReturnVisitDate { set; get; }

		/// <summary>
		/// 回访结果
		/// </summary>
		public string ReturnVisitResult { set; get; }

        /// <summary>
        /// 回访满意度[1:满意，2:一般，3:不满意]
        /// </summary>
        public int? ReturnStatus { set; get; }

        /// <summary>
        /// 分配记录
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<LoanAssignLogger> LoanAssignLogger
        {
            get { return _loanAssignLogger ?? (_loanAssignLogger = new List<LoanAssignLogger>()); }
            protected set { _loanAssignLogger = value; }
        }
    }
}