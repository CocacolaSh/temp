using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Ocean.Framework.Mvc.Routes;

namespace Ocean.Web.Infrastructure
{
    //Routes used for backward compatibility with 1.x versions of oceanCommerce
    public partial class UpgradeRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
        
        }

        public int Priority
        {
            get
            {
                //register it after all other IRouteProvider are processed
                return -1;
            }
        }
    } 
}