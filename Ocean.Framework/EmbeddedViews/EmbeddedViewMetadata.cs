using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.EmbeddedViews
{
    [Serializable]
    public class EmbeddedViewMetadata
    {
        public string Name { get; set; }
        public string AssemblyFullName { get; set; }
    }
}
