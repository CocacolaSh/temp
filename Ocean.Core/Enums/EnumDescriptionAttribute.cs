using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDescriptionAttribute:Attribute
    {
        public string Description
        {
            get;
            set;
        }
        public EnumDescriptionAttribute()
        {
        }
        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
