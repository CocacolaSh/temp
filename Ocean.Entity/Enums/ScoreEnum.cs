using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Enums;

namespace Ocean.Entity.Enums.ScoreSys
{

    /// <summary>
    /// 积分明细种类
    /// </summary>
    public enum ScoreConsume
    {
        /// <summary>
        ///  1:兑换 2:转让 3:抽奖 4:交易收入[积分>0 表示收入 <0表示消耗] 5:绑定 6:推荐
        /// </summary>
        [EnumDescription("兑换 ")]
        Exchange = 1,
        /// <summary>
        /// 转让
        /// </summary>
        [EnumDescription("转让")]
        Trans = 2,
        /// <summary>
        /// 抽奖
        /// </summary>
        [EnumDescription("抽奖")]
        Plugins = 3,
        /// <summary>
        /// 交易收入
        /// </summary>
        [EnumDescription("交易收入")]
        Trade = 4,
        /// <summary>
        /// 绑定
        /// </summary>
        [EnumDescription("绑定")]
        Bind,
        /// <summary>
        /// 推荐
        /// </summary>
        [EnumDescription("推荐")]
        Recommend,
    }

}