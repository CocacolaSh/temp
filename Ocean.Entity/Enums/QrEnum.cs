using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Enums;

namespace Ocean.Entity.Enums.Branch
{
    public enum QrEnum
    {
        /// <summary>
        /// 营业网点
        /// </summary>
        [EnumDescription("临时")]
        QR_SCEN = 0,
        /// <summary>
        /// 自助银行
        /// </summary>
        [EnumDescription("永久")]
        QR_LIMIT_SCENE = 1
    }
}