using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ocean.Core.Configuration;
using Ocean.Core;
namespace Ocean.Framework.Mvc.Handlers
{
    public class OceanHandleErrorAttribute : HandleErrorAttribute
    {
        private BaseConfigInfo _config;
        private BaseConfigInfo config
        {
            get
            {
                if (_config == null)
                {
                    _config = BaseConfigs.GetConfig();
                }
                return _config;
            }
        }
        public string ErrorType { get; set; }
        public override void OnException(ExceptionContext filterContext)
        {
            #region AJAX的处理方式
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult()
                {
                    ContentEncoding = null,
                    ContentType = null,
                    Data = new
                    {
                        success = false,
                        timeout = false,
                        message = filterContext.Exception.Message,
                        syserror = !(filterContext.Exception is OceanException)
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                //设置500的HTTP错误码，方便jQuery的全局函数可以在error中抓到错误
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                return;
            }
            #endregion

                Exception exception = filterContext.Exception;
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.IsChildAction)
            {
                return;
            }
            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (filterContext.ExceptionHandled || filterContext.HttpContext.IsCustomErrorEnabled)
            {
                //
            }
            else
            {
                switch (ErrorType)
                {
                    case "Admin":
                        {
                            this.View = "~/Themes/Default/Views/Shared/Error.cshtml";
                            break;
                        }
                    default:
                        {
                            this.View = "~/Themes/Default/Views/Shared/Error.cshtml";
                            break;
                        }
                }
                // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
                // ignore it.
                if (new HttpException(null, exception).GetHttpCode() != 500)
                {
                    return;
                }

                if (!ExceptionType.IsInstanceOfType(exception))
                {
                    return;
                }
                string controllerName = (string)filterContext.RouteData.Values["controller"];
                string actionName = (string)filterContext.RouteData.Values["action"];
                HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
                filterContext.HttpContext.Response.AddHeader("HttpStatus", "500"); 
                filterContext.ExceptionHandled = true;
            }



//#if DEBUG
//            throw exception;
//#endif
            base.OnException(filterContext);
        }
    }
}