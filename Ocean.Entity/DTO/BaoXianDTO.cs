using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class BaoXianDTO : BaseDTO
    {
        public BaoXianDTO() { }

        public BaoXianDTO(BaoXian baoDan)
        {
            this.Id = baoDan.Id;
            this.CreateDate = baoDan.CreateDate;
            this.MpUserId = baoDan.MpUserId;
            this.TouBaoGongSi = baoDan.TouBaoGongSi;
            this.TouBaoRen = baoDan.TouBaoRen;
            this.ChePai = baoDan.ChePai;
            this.BaoXianQiXian = baoDan.BaoXianQiXian;
            this.HouSiWei = baoDan.HouSiWei;
            this.BaoXianFei = baoDan.BaoXianFei;
            this.CheChuanSui = baoDan.CheChuanSui;
            this.LaiYuan = baoDan.LaiYuan;
            this.XianZhong = baoDan.XianZhong;
        }
        public Guid ?MpUserId { set; get; }
        /// <summary>
        /// 起保开始时间
        /// </summary>
        public DateTime? StartDate { set; get; }

        /// <summary>
        /// 起保结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }


        /// <summary>
        /// 投保公司
        /// </summary>
        public string TouBaoGongSi { set; get; }

        /// <summary>
        /// 投保人
        /// </summary>
        public string TouBaoRen { set; get; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string ChePai { set; get; }

        /// <summary>
        /// 保险期限
        /// </summary>
        public int ?BaoXianQiXian { set; get; }

        /// <summary>
        /// 后四位
        /// </summary>
        public string HouSiWei { set; get; }

        /// <summary>
        /// 保险费
        /// </summary>
        public decimal BaoXianFei { set; get; }
        /// <summary>
        /// 车船税
        /// </summary>
        public decimal CheChuanSui { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string LaiYuan { set; get; }

        /// <summary>
        /// 险种
        /// </summary>
        public int? XianZhong { set; get; }
        
    }
}