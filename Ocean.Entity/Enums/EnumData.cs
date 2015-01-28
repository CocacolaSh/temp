using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class EnumData : BaseEntity
    {
        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 枚举值
        /// </summary>
        public string Value { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }


        /// <summary>
        /// 枚举类型外键Id
        /// </summary>
        public Guid EnumTypeId { set; get; }

        /// <summary>
        /// 模块实体
        /// </summary>
        public virtual EnumType EnumType { set; get; }
    }
}