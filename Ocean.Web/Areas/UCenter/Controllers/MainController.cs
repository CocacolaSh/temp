using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Services;
using Ocean.Entity;
using Autofac;
using Ocean.Core.Infrastructure.DependencyManagement;

namespace Ocean.Web.Areas.UCenter.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}