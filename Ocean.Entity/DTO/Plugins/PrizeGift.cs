using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Ocean.Entity.DTO.Plugins
{
    public class PrizeGift
    {
        public int ID { get; set; }
        /// <summary>
        /// 机率
        /// </summary>
        public double Odds { get; set; }
        /// <summary>
        /// 赠品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 奖项别名
        /// </summary>
        public string Alias_Name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 还剩
        /// </summary>
        public int Leavings_Quantity { get; set; }
        /// <summary>
        /// 角度
        /// </summary>
        public int Angle { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Pic { get; set; }
        /// <summary>
        /// 奖项等级名称
        /// </summary>
        public string Prize_Name { get; set; }

        /// <summary>
        /// 是否有奖
        /// </summary>
        public int Has_Gift { get; set; }

        /// <summary>
        /// 抽奖
        /// </summary>
        public int Rate { get; set; }

        public Guid ResultID{get;set ;}

        /// <summary>
        /// 多结果抽奖
        /// </summary>
        public string Rate_Array { get; set; }

        /// <summary>
        /// 设定中奖者用户ID
        /// </summary>
        [JsonIgnore]
        public string Users { get; set; }
        /// <summary>
        /// 设定中奖者用户名称
        /// </summary>
        [JsonIgnore]
        public string UsersName { get; set; }
    }
}
