using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class Area : BaseEntity
    {
        /// <summary>
        /// 地区编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 地区名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 地区全名
        /// </summary>
        public string FullName { set; get; }

        /// <summary>
        /// 地区邮编
        /// </summary>
        public string PostCode { set; get; }

        /// <summary>
        /// 区号
        /// </summary>
        public string AreaCode { set; get; }

        /// <summary>
        /// 深度
        /// </summary>
        public int Depth { set; get; }
    }
}