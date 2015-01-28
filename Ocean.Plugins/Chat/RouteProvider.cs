using System.Web.Mvc;
using System.Web.Routing;
using Ocean.Framework.Mvc.Routes;

namespace Plugin.Plugin1
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Ocean.Plugin.Chat",
                 "Plugins/Chat/Index",
                 new { controller = "Chat", action = "Index" },
                 new[] { "Ocean.Plugin.Chat.Controllers" }
            );
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