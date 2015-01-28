using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Services;
using Ocean.Entity;

namespace Ocean.Web.Controllers
{
    public class MpMaterialController : Controller //WeixinOAuthController
    {
        private readonly IMpMaterialService MpMaterialService;
        private readonly IMpMaterialItemService MpMaterialItemService;

        public MpMaterialController(IMpMaterialService MpMaterialService, IMpMaterialItemService MpMaterialItemService)
        {
            this.MpMaterialService = MpMaterialService;
            this.MpMaterialItemService = MpMaterialItemService;
        }
        /// <summary>
        /// 微信详细页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MaterialDetail(Guid id)
        {
            MpMaterialItem item = MpMaterialItemService.GetById(id) ?? new MpMaterialItem();
            return View(item);
        }
        /// <summary>
        /// 素材内容页
        /// </summary>
        [HttpGet]
        public ActionResult Content(Guid id)
        {
            return View();
        }

        /// <summary>
        /// 手机银行下载页面
        /// </summary>
        public ActionResult App()
        {
            return View();
        }
        /// <summary>
        /// 手机银行下载页面
        /// </summary>
        public ActionResult AppInfo()
        {
            return View();
        }
        /// <summary>
        /// 手机银行下载页面
        /// </summary>
        public ActionResult MobileBank()
        {
            return View();
        }
    }
}