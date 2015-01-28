using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ocean.Core.Infrastructure;
using Ocean.Framework.Mvc.Routes;
using Ocean.Framework.Mvc;
using Ocean.Framework.Themes;

namespace Ocean.Agent
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

            routes.MapRoute(
                name: "Default", // 路由名称
                url: "{controller}/{action}/{id}", // 带有参数的 URL
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Ocean.Admin.Controllers" } // 参数默认值
            );
        }

        protected void Application_Start()
        {
            //初始化引擎上下文环境
            EngineContext.Initialize(false);
            //设置依赖项解析器
            var dependencyResolver = new OceanDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);
            //移除所有视图引擎
            ViewEngines.Engines.Clear();
            //使用自定义ThemeableRazorViewEngine视图引擎
            ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            //注册ASP.NET MVC应用程序中的所有区域
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ////注册虚拟路径提供者为嵌入式视图
            //var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            //var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            //HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);
            //开始任务调度
            //TaskManager.Instance.Initialize();
            //TaskManager.Instance.Start();
        }
    }
}