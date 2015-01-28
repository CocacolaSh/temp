using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Ocean.Core.Enums
{
    /// <summary>
    /// 枚举实体
    /// </summary>
    public class EnumItem
    {
        public string EnumKey
        {
            get;
            set;
        }
        public string EnumValue
        {
            get;
            set;
        }
        public string EnumDescript
        {
            get;
            set;
        }
        public EnumItem()
        {

        }
        public EnumItem(string enumKey, string enumValue,string enumDescript)
        {
            EnumKey = enumKey;
            EnumValue = enumValue;
            EnumDescript = enumDescript;
        }
    }
}
