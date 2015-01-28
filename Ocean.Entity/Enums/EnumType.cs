using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class EnumType : BaseEntity
    {
        private ICollection<EnumData> _enumDatas;

        /// <summary>
        /// 枚举类型名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 枚举类型标识
        /// </summary>
        public string Identifying { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }

        /// <summary>
        /// 枚举集
        /// </summary>
        public virtual ICollection<EnumData> EnumDatas
        {
            get { return _enumDatas ?? (_enumDatas = new List<EnumData>()); }
            protected set { _enumDatas = value; }
        }
    }
}