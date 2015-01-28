using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity.Enums;

namespace Ocean.Entity.DTO
{
    public class EnumTypeDTO : BaseDTO
    {
        public EnumTypeDTO(EnumType enumType)
        {
            this.Id = enumType.Id;
            this.CreateDate = enumType.CreateDate;
            this.Name = enumType.Name;
            this.Identifying = enumType.Identifying;
            this.Remark = enumType.Remark;
            this.Sort = enumType.Sort;
        }

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
    }
}
