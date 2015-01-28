using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Entity.Enums;
using Ocean.Page;
using Ocean.Framework.Caching.Cache;
using Ocean.Services;
using Ocean.Entity;
using Ocean.Entity.DTO;
using Ocean.Core;
using Ocean.Core.Utility;
using Ocean.Core.Logging;

namespace Ocean.Web.Controllers
{
    public class GongzhiBaoController : WeixinOAuthController
    {
        private readonly IFunongbaoApplyService _funongbaoApplyService;
        private readonly IFunongbaoService _funongbaoService;
        public GongzhiBaoController(IFunongbaoService funongbaoService, IFunongbaoApplyService funongbaoApplyService)
        {
            _funongbaoService = funongbaoService;
            _funongbaoApplyService = funongbaoApplyService;
        }
        public ActionResult Index()
        {
            //IList<Funongbao> funongbaoGroup = _funongbaoService.GetGroupByNo(CurrentFunongbao.GroupNO);
            //IEnumerable<Funongbao> funs = funongbaoGroup.Where(f => f.IsAuth == 0).ToList();
            return View();
        }
    }
}
