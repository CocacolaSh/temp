using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Enums;

namespace Ocean.Entity.Enums.Branch
{
    public enum BranchEnum
    {
        /// <summary>
        /// 营业网点
        /// </summary>
        [EnumDescription("营业网点")]
        Branch = 0,
        /// <summary>
        /// 自助银行
        /// </summary>
        [EnumDescription("自助银行")]
        SelfHelp = 1,
        /// <summary>
        /// ATM
        /// </summary>
        [EnumDescription("ATM")]
        ATM = 2
    }
}