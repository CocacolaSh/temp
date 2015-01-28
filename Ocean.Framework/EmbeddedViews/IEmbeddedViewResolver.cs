using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.EmbeddedViews
{
    public interface IEmbeddedViewResolver
    {
        EmbeddedViewTable GetEmbeddedViews();
    }
}
