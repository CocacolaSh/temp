﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Ocean.Framework.Mvc.Routes
{
    public interface IRoutePublisher
    {
        void RegisterRoutes(RouteCollection routeCollection);
    }
}