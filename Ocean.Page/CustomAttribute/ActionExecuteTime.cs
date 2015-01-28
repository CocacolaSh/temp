using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Diagnostics;

namespace Ocean.Page.CustomAttribute
{
    public class ActionExecuteTimeAttribute : ActionFilterAttribute
    {
        private Stopwatch timer;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            timer = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            timer.Stop();
            filterContext.HttpContext.Response.Write(
                string.Format("页面执行时间: {0} 毫秒",
                timer.Elapsed.TotalMilliseconds));
        }
    }
}