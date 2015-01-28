using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ocean.Core.Infrastructure;
using Ocean.Framework.Mvc;
using Ocean.Framework.Themes;
using Ocean.Framework.Mvc.Routes;
using Ocean.Core.Data;
using Ocean.Core.Logging;
using Ocean.Page;
using Ocean.Communication.Comet;

namespace Ocean.Admin
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

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
                namespaces:new string[]{"Ocean.Admin.Controllers"} // 参数默认值
            );
        }

        protected void Application_Start()
        {
            //初始化引擎上下文环境
            EngineContext.Initialize(false);
            //数据库是否已经安装
            bool databaseInstalled = true;// DataSettingsHelper.DatabaseIsInstalled();
            //设置依赖项解析器
            var dependencyResolver = new OceanDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);
            //模型绑定
            ModelBinders.Binders.Add(typeof(BaseOceanModel), new OceanModelBinder());
            //移除所有视图引擎
            if (databaseInstalled)
            {
                ViewEngines.Engines.Clear();
                //使用自定义ThemeableRazorViewEngine视图引擎
                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            }
            //添加一些功能默认ModelMetadataProvider之上
            ModelMetadataProviders.Current = new OceanMetadataProvider();
            //注册ASP.NET MVC应用程序中的所有区域
            AreaRegistration.RegisterAllAreas();
            //注册筛选器
            RegisterGlobalFilters(GlobalFilters.Filters);
            //注册路由
            RegisterRoutes(RouteTable.Routes);
            ////注册虚拟路径提供者为嵌入式视图
            //var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            //var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            //HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);
            //开始任务调度
            //TaskManager.Instance.Initialize();
            //TaskManager.Instance.Start();

            #region 初始化消息处理线程
            //初始化消息处理线程
            CometThreadPool.CreateThreads(5);
            #endregion
        }

        #region Application_Error
        public void Application_Error(object sender, EventArgs e)
        {
            //在出现未处理的错误时运行的代码
            //错误日志
            Exception exception = Server.GetLastError();
            if (exception == null) return;
            if (exception.InnerException != null) exception = exception.InnerException;
            if (exception.Message.Contains("禁止路径")) return;
            if (exception.Message.Contains("服务器") || exception.Message.Contains("超时") || exception.Message.Contains("登录失败"))
            {
                Log4NetImpl.Write(exception.Message,Log4NetImpl.ErrorLevel.Error);
            }
            else if (exception.Message.Contains("最大池大小"))
            {
                Log4NetImpl.Write("超出最大池大小：重启进程", Log4NetImpl.ErrorLevel.Error);
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                Log4NetImpl.Write(exception.Message, Log4NetImpl.ErrorLevel.Error);
            }
        }
        #endregion
    }
}