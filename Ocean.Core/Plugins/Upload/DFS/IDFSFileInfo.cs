using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Plugins.DFS
{
    public interface IDFSFileInfo
    {
        long FileSize{get;set;}
        DateTime CreateTime{get;set;}
        long Crc32{get;set;}
    }
}
