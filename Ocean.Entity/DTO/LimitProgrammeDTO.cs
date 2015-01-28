using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class LimitProgrammeDTO : BaseDTO
    {
        /// <summary>
        /// 申请的额度
        /// </summary>
        public decimal ApplyLimit { get; set; }
        /// <summary>
        /// 申请的费率
        /// </summary>
        public decimal ApplyRates { get; set; }

        /// <summary>
        /// 申请的类型［升、降额］
        /// </summary>
        public int ApplyType { get; set; }

    }
}
