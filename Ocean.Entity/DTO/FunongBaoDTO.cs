using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class FunongBaoDTO : BaseDTO
    {
        public FunongBaoDTO() { }

        public FunongBaoDTO(Funongbao funongbao)
        {
            this.Id = funongbao.Id;
            this.CreateDate = funongbao.CreateDate;
            //this.MpUserId = funongbao.MpUserId;
            //this.OpenId = funongbao.OpenId;
            //this.PassportNO = funongbao.PassportNO;
            //this.Name = funongbao.Name;
            //this.MobilePhone = funongbao.MobilePhone;
            //this.FunongbaoNO = funongbao.FunongbaoNO;
            //this.BankNO = funongbao.BankNO;
            //this.CurrentLimit = funongbao.CurrentLimit;
            //this.CurrentRates = funongbao.CurrentRates;
            //this.ApplyLimit = funongbao.ApplyLimit;
            //this.ApplyRates = funongbao.ApplyRates;
            //this.ChangedLimit = funongbao.ChangedLimit;
            //this.ChangedRates = funongbao.ChangedRates;
            //this.GroupNO = funongbao.GroupNO;
            //this.IsAuth = funongbao.IsAuth;
            //this.IsSignAgreement = funongbao.IsSignAgreement;
            //this.ProcessStatus = funongbao.ProcessStatus;
            //this.ProcessResult = funongbao.ProcessResult;
            //this.FinishDate = funongbao.FinishDate;
            //this.Subbranch = funongbao.Subbranch;
            //this.Marketer = funongbao.Marketer;
            //this.ChangeType = funongbao.ChangeType;
            //this.SuggestionLimit = funongbao.SuggestionLimit;
        }

        /// <summary>
        /// 申请开始时间
        /// </summary>
        public DateTime? StartDate { set; get; }

        /// <summary>
        /// 申请结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }

        ///// <summary>
        ///// 用户ID
        ///// </summary>
        //public Guid MpUserId { set; get; }

        ///// <summary>
        ///// 公众账号openid
        ///// </summary>
        //public string OpenId { set; get; }

        ///// <summary>
        ///// 证件号码
        ///// </summary>
        //public string PassportNO { set; get; }

        ///// <summary>
        ///// 姓名
        ///// </summary>
        //public string Name { set; get; }

        ///// <summary>
        ///// 手机号码
        ///// </summary>
        //public string MobilePhone { set; get; }

        ///// <summary>
        ///// 福农宝号
        ///// </summary>
        //public string FunongbaoNO { set; get; }

        ///// <summary>
        ///// 绑定的银行卡号
        ///// </summary>
        //public string BankNO { set; get; }

        ///// <summary>
        ///// 当前额度
        ///// </summary>
        //public decimal CurrentLimit { set; get; }

        ///// <summary>
        ///// 当前费率
        ///// </summary>
        //public decimal CurrentRates { set; get; }

        ///// <summary>
        ///// 申请额度
        ///// </summary>
        //public decimal ApplyLimit { set; get; }

        ///// <summary>
        ///// 申请费率
        ///// </summary>
        //public decimal ApplyRates { set; get; }

        ///// <summary>
        ///// 调整后额度
        ///// </summary>
        //public decimal ChangedLimit { set; get; }

        ///// <summary>
        ///// 调整后费率
        ///// </summary>
        //public decimal ChangedRates { set; get; }

        ///// <summary>
        ///// 组编号
        ///// </summary>
        //public string GroupNO { set; get; }

        ///// <summary>
        ///// 是否身份认证
        ///// </summary>
        //public int IsAuth { set; get; }

        ///// <summary>
        ///// 是否签定协议
        ///// </summary>
        //public int IsSignAgreement { set; get; }

        ///// <summary>
        ///// 受理状态
        ///// </summary>
        //public int ProcessStatus { set; get; }

        ///// <summary>
        ///// 受理结果
        ///// </summary>
        //public string ProcessResult { set; get; }

        //private DateTime finishdate;
        ///// <summary>
        ///// 完成日期
        ///// </summary>
        //public DateTime FinishDate
        //{
        //    set { finishdate = value; }
        //    get
        //    {
        //        if (finishdate < new DateTime(1900, 1, 1))
        //        {
        //            finishdate = new DateTime(1900, 1, 1);
        //        }
        //        return finishdate;
        //    }
        //}

        ///// <summary>
        ///// 支行
        ///// </summary>
        //public string Subbranch { set; get; }

        ///// <summary>
        ///// 营销人
        ///// </summary>
        //public string Marketer { set; get; }

        ///// <summary>
        ///// 调整类型
        ///// </summary>
        //public string ChangeType { set; get; }

        ///// <summary>
        ///// 建议额度
        ///// </summary>
        //public string SuggestionLimit { set; get; }
    }
}