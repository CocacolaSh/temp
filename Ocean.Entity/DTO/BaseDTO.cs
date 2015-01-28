using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class BaseDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id
        {
            set;
            get;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            set;
            get;
        }
    }
}
