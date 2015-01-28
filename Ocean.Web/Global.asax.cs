using System.Web.Mvc;
using System.Web.Routing;
using Ocean.Core.Infrastructure;
using Ocean.Framework.Mvc.Routes;
using Ocean.Framework.Mvc;
using Ocean.Framework.Themes;
using Ocean.Services.Tasks;
using System.Web;
using System;
using Ocean.Core.Logging;
using Ocean.Framework.Sms;
using Ocean.Page;

namespace Ocean.Web
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
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        #region The application_ end.
        /// <summary>
        /// The application_start.
        /// </summary>
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
            ////安装插件
            //PluginManager.MarkAllPluginsAsUninstalled();
            //var pluginFinder = Ocean.Core.Infrastructure.EngineContext.Current.Resolve<IPluginFinder>();
            //var plugins = pluginFinder.GetPlugins<IPlugin>(false)
            //    .ToList()
            //    .OrderBy(x => x.PluginDescriptor.Group)
            //    .ThenBy(x => x.PluginDescriptor.DisplayOrder)
            //    .ToList();
            //foreach (var plugin in plugins)
            //{
            //    plugin.Install();
            //}

            //注册ASP.NET MVC应用程序中的所有区域
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ////注册虚拟路径提供者为嵌入式视图
            //var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            //var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            //HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);
            //开始任务调度
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
        }
        #endregion

        #region The application_ end.
        /// <summary>
        /// The application_end.
        /// </summary>
        protected void Application_End()
        {
            if (SmsClient.APIClientOcean != null)
            {
                SmsClient.ReleaseAPIClient();
            }
        }
        #endregion

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
                Log4NetImpl.Write(exception.Message, Log4NetImpl.ErrorLevel.Error);
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
            if (exception is HttpException)
            {
                try
                {
                    int errorCode = (exception as HttpException).GetHttpCode();
                    if (errorCode == 404)
                    {
                        Server.ClearError();
                        //avoid IIS7 getting in the middle
                        Response.TrySkipIisCustomErrors = true;
                        IController messageController = new ErrorController();
                        var http404Route = new RouteData();
                        http404Route.Values.Add("controller", "Error");
                        http404Route.Values.Add("action", "Http404");
                        http404Route.Values.Add("message", "错误提示：");
                        http404Route.Values.Add("url", this.Request.Url.OriginalString);
                        messageController.Execute(new RequestContext(new HttpContextWrapper(this.Context), http404Route));
                    }
                    else
                    {
                        string message = exception.Message;
                        Server.ClearError();
                        Response.TrySkipIisCustomErrors = true;
                        IController messageController = new ErrorController();
                        var httpErrRoute = new RouteData();
                        httpErrRoute.Values.Add("controller", "Message");
                        httpErrRoute.Values.Add("action", "ErrorMessage");
                        httpErrRoute.Values.Add("message", "对不起，系统异常，请联系客服！" + message);
                        httpErrRoute.Values.Add("url", this.Request.Url.OriginalString);
                        messageController.Execute(new RequestContext(new HttpContextWrapper(this.Context), httpErrRoute));
                    }
                }
                catch (Exception ex) { throw ex; }
            }
        }
        #endregion
    }
}