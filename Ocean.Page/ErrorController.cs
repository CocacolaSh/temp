using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ocean.Page
{
    public class ErrorController : System.Web.Mvc.Controller
    {
        public ActionResult Http404(string message)
        {
            ViewBag.Title = "HTTP 404- 无法找到文件";
            ViewBag.Description = message;
            HttpContext.Response.StatusCode = 404;
            return View();
        }
        public ActionResult ErrorMessage(string message)
        {
            ViewBag.Title = "Operate Error!";
            ViewBag.Description = message;
            return View();
        }
    }
}
