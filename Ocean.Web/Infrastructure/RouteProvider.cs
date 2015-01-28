using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ocean.Framework.Mvc.Routes;
using System.Web.Routing;

namespace Ocean.Web.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            ////home page
            //routes.MapLocalizedRoute("HomePage",
            //                "",
            //                new { controller = "Home", action = "Index" },
            //                new[] { "Ocean.Web.Controllers" });
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