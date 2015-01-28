using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ocean.Framework.Mvc.Routes;
using System.Web.Routing;

namespace Ocean.Admin.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {

        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}