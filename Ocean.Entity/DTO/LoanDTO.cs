using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class LoanDTO : BaseDTO
    {
        public LoanDTO() { }

        public LoanDTO(Loan loan)
        {
            this.Id = loan.Id;
            this.CreateDate = loan.CreateDate;
            this.MpUserId = loan.MpUserId;
            this.LoanName = loan.MpUser.Name;
            this.Phone = loan.MpUser.MobilePhone;
            this.Address = loan.Address;
            this.Company = loan.Company;
            this.Position = loan.Position;
            this.LoanCompanyId = loan.LoanCompanyId;
            this.LoanFormalId = loan.LoanFormalId;
            this.LoanAssetHourseId = loan.LoanAssetHourseId;
            this.ApplyMoney = loan.ApplyMoney;
            this.LoanCategoryId = loan.LoanCategoryId;
            this.DeadlineId = loan.DeadlineId;
            this.RepaymentModeId = loan.RepaymentModeId;
            this.GuaranteeWayId = loan.GuaranteeWayId;
            this.AssetSituation = loan.AssetSituation;
            this.RepealDate = loan.RepealDate;
            this.Status = loan.Status;
            this.AssignStatus = loan.AssignStatus;
            this.AssignSubbranch = loan.AssignSubbranch;
            this.AssignCustomerManager = loan.AssignCustomerManager;
            this.CustomerManagerPhone = loan.CustomerManagerPhone;
            this.ProcessStatus = loan.ProcessStatus;
            this.ProcessResult = loan.ProcessResult;
            this.ProcessDate = loan.ProcessDate;
            this.ReturnVisitDate =loan.ReturnVisitDate;
            this.ReturnVisitResult = loan.ReturnVisitResult;
            this.ReturnStatus = loan.ReturnStatus;
            if (loan.CreateDate < DateTime.Now.AddMonths(-1))
            {
                this.PastDue = "已过期";
            }
            else
            {
                this.PastDue = "未过期";
            }
        }

        /// <summary>
        /// 申请开始时间
        /// </summary>
        public DateTime? StartDate { set; get; }

        /// <summary>
        /// 申请结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid MpUserId { set; get; }

        /// <summary>
        /// 公众平台openID
        /// </summary>
        public string OpenId { set; get; }

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
        /// <summary>
        /// 企业类型
        /// </summary>
        public string LoanCompanyMode { set; get; }
        /// 编制类型
        /// </summary>
        public int? LoanFormalId { set; get; }/// <summary>
        /// <summary>
        /// 编制类型
        /// </summary>
        public string LoanFormalMode { set; get; }
        /// 自有房产
        /// </summary>
        public int? LoanAssetHourseId { set; get; }
        /// <summary>
        /// 自有房产
        /// </summary>
        public string LoanAssetHourseMode { set; get; }

        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal ApplyMoney { set; get; }

        /// <summary>
        /// 贷款种类枚举
        /// </summary>
        public int? LoanCategoryId { set; get; }
        /// <summary>
        /// 贷款种类
        /// </summary>
        public string LoanCategory { set; get; }

        /// <summary>
        /// 贷款期限枚举
        /// </summary>
        public int? DeadlineId { set; get; }
        /// <summary>
        /// 贷款期限
        /// </summary>
        public string Deadline { set; get; }

        /// <summary>
        /// 还款方式枚举
        /// </summary>
        public int? RepaymentModeId { set; get; }
        /// <summary>
        /// 还款方式
        /// </summary>
        public string RepaymentMode { set; get; }

        /// <summary>
        /// 担保方式枚举
        /// </summary>
        public int? GuaranteeWayId { set; get; }
        /// <summary>
        /// 担保方式
        /// </summary>
        public string GuaranteeWay { set; get; }

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
        public int? Status { set; get; }

        /// <summary>
        /// 分配状态［0-无任何处理，1-已分配］
        /// </summary>
        public int? AssignStatus { set; get; }

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
        /// 受理状态［0-未受理，1-已通过，2-未通过］
        /// </summary>
        public int? ProcessStatus { set; get; }

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
        /// 回访满意度
        /// </summary>
        public int? ReturnStatus { set; get; }

        /// <summary>
        /// 是否已经过期[0:未过期，1:已过期]
        /// </summary>
        public int? PastDueId { set; get; }
        public string PastDue { set; get; }
    }
}