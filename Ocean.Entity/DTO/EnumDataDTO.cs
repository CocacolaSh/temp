using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity.Enums;

namespace Ocean.Entity.DTO
{
    public class EnumDataDTO : BaseDTO
    {
        public EnumDataDTO(EnumData enumData)
        {
            this.Id = enumData.Id;
            this.CreateDate = enumData.CreateDate;
            this.Name = enumData.Name;
            this.Value = enumData.Value;
            this.EnumTypeName = enumData.EnumType.Name;
        }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 枚举值
        /// </summary>
        public string Value { set; get; }

        /// <summary>
        /// 所属枚举类型
        /// </summary>
        public string EnumTypeName { set; get; }
    }
}
