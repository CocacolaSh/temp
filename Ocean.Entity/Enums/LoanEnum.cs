using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Enums;

namespace Ocean.Entity.Enums.Loan
{
    #region 已经失效
    /// <summary>
    /// 贷款种类
    /// </summary>
    public enum LoanCategory
    {
        /// <summary>
        /// 经营贷
        /// </summary>
        [EnumDescription("经营贷")]
        Operate=1,
        /// <summary>
        /// 消费贷
        /// </summary>
        [EnumDescription("消费贷")]
        Consumption
    }

    /// <summary>
    /// 贷款期限
    /// </summary>
    public enum LoanDeadline
    {
        /// <summary>
        /// 一年内（含）
        /// </summary>
        [EnumDescription("一年内（含）")]
        OneYear = 1,
        /// <summary>
        /// 一年以上
        /// </summary>
        [EnumDescription("一年以上")]
        OneYearMore,
        /// <summary>
        /// 不定期
        /// </summary>
        [EnumDescription("不定期")]
        WithoutDay
    }

    /// <summary>
    /// 还款方式
    /// </summary>
    public enum RepaymentMode
    {
        /// <summary>
        /// 按揭还款
        /// </summary>
        [EnumDescription("按揭还款")]
        Mortgage = 1,
        /// <summary>
        /// 按日/月/季结息，到期还本
        /// </summary>
        [EnumDescription("按日/月/季结息，到期还本")]
        DueDate
    }

    /// <summary>
    /// 担保方式
    /// </summary>
    public enum GuaranteeWay
    {
        /// <summary>
        /// 有抵/质押
        /// </summary>
        [EnumDescription("有抵/质押")]
        Mortgage = 1,
        /// <summary>
        /// 无抵/质押
        /// </summary>
        [EnumDescription("无抵/质押")]
        CleanLoan
    }

    /// <summary>
    /// 资产情况
    /// </summary>
    public enum AssetSituation
    {
        /// <summary>
        /// 房产
        /// </summary>
        [EnumDescription("房产")]
        Hource = 1,
        /// <summary>
        /// 车
        /// </summary>
        [EnumDescription("车")]
        Car
    }
    #endregion

    /// <summary>
    /// 申请状态
    /// </summary>
    public enum StatusEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        [EnumDescription("正常")]
        Normal = 0,
        /// <summary>
        /// 被撤销
        /// </summary>
        [EnumDescription("被撤销")]
        Revocation = 1
    }

    /// <summary>
    /// 分配状态
    /// </summary>
    public enum AssignStatusEnum
    {
        /// <summary>
        /// 无任何处理
        /// </summary>
        [EnumDescription("无任何处理")]
        NoAssign = 0,
        /// <summary>
        /// 已分配
        /// </summary>
        [EnumDescription("已分配")]
        Assigned = 1
    }

    /// <summary>
    /// 受理状态
    /// </summary>
    public enum ProcessStatusEnum
    {
        /// <summary>
        /// 未受理
        /// </summary>
        [EnumDescription("未受理")]
        NoReceive = 0,
        /// <summary>
        /// 已通过
        /// </summary>
        [EnumDescription("已通过")]
        Received = 1,
        /// <summary>
        /// 未通过
        /// </summary>
        [EnumDescription("未通过")]
        WithdrawAssign = 2,
        /// <summary>
        /// 受理中
        /// </summary>
        [EnumDescription("受理中")]
        Process = 3
    }
}