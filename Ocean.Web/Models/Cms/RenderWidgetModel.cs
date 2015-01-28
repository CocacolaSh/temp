using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ocean.Framework.Mvc;
using System.Web.Routing;

namespace Ocean.Web.Models.Cms
{
    public partial class RenderWidgetModel : BaseOceanModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}