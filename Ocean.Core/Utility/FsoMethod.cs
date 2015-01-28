using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Enums;

namespace Ocean.Core.Utility
{
    /// <summary>
    /// 文件系统操作对象枚举类型 
    /// </summary>
    public enum FsoMethod
    {
        [EnumDescription("文件夹")]
        Folder,
        [EnumDescription("文件")]
        File,
        [EnumDescription("文件夹及文件")]
        All
    }
}
