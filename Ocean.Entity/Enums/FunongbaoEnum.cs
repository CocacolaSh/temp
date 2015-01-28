using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Enums;

namespace Ocean.Entity.Enums.Funongbao
{
    /// 受理情况
    /// </summary>
    public enum FunongbaoProcessStatus
    {
        /// <summary>
        /// 未受理
        /// </summary>
        [EnumDescription("未受理")]
        NoProcess = 0,
        /// <summary>
        /// 通过
        /// </summary>
        [EnumDescription("通过")]
        Pass,
        /// <summary>
        /// 未通过
        /// </summary>
        [EnumDescription("未通过")]
        NoPass,
        /// <summary>
        /// 调整到建议额度
        /// </summary>
        [EnumDescription("调整到建议额度")]
        Suggestion
    }
}
