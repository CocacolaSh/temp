using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Services.Cms;
using Ocean.Web.Models.Cms;
using System.Web.Routing;
using Ocean.Core.Infrastructure.DependencyManagement;
using Autofac;
using Ocean.Web.Widgets;
using Ocean.Core.Infrastructure;
using Ocean.Services;

namespace Ocean.Web.Controllers
{
    public partial class WidgetController : Controller
    {
        #region Fields

        private readonly IWidgetService _widgetService;

        #endregion

        #region Constructors

        public WidgetController(IWidgetService widgetService)
        {
            this._widgetService = widgetService;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone)
        {
            //model
            var model = new List<RenderWidgetModel>();

            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
            foreach (var widget in widgets)
            {
                var widgetModel = new RenderWidgetModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                widget.GetDisplayWidgetRoute(widgetZone, out actionName, out controllerName, out routeValues);
                widgetModel.ActionName = actionName;
                widgetModel.ControllerName = controllerName;
                widgetModel.RouteValues = routeValues;

                model.Add(widgetModel);
            }

            return PartialView(model);
        }

        /// <summary>
        /// 渲染部件
        /// </summary>
        /// <param name="widgetIdentifying">部件标识</param>
        /// <param name="layout">布局</param>
        /// <returns></returns>
        public ActionResult RenderWidget(string widgetIdentifying, string layout)
        {
            //判断是否移动设备
            var mobileDeviceHelper = EngineContext.Current.Resolve<IMobileDeviceHelper>();
            bool useMobileDevice = mobileDeviceHelper.IsMobileDevice(this.ControllerContext.HttpContext)
                && mobileDeviceHelper.MobileDevicesSupported()
                && !mobileDeviceHelper.CustomerDontUseMobileVersion();
            var typeFinder = EngineContext.Current.ContainerManager.Resolve<ITypeFinder>();
            var drTypes = typeFinder.FindClassesOfType<BaseWidgetController>();
            //遍历所有部件（需要利用缓存来优化）
            foreach (var drType in drTypes)
            {
                var widget = (BaseWidgetController)Activator.CreateInstance(drType);

                if (string.Equals(widget.WidgetIdentifying, widgetIdentifying, StringComparison.InvariantCultureIgnoreCase))
                {
                    return widget.RenderWidget(layout, useMobileDevice);
                }
            }
            //挂件加载错误
            string widgetErrorPath = string.Format("~/Widgets/Error.cshtml");
            return PartialView(widgetErrorPath);
        }

        #endregion
    }
}