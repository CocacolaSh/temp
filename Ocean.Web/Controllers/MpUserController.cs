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
using Senparc.Weixin.MP.AdvancedAPIs;
using Ocean.Core.Logging;
using System.Threading;

namespace Ocean.Web.Controllers
{
    public class MpUserController : WebBaseController
    {
        private readonly IMpUserService MpUserService;
        private readonly IMpUserGroupService MpUserGroupService;
        private readonly IFunongbaoApplyService _funongbaoApplyService;
        private readonly ILoanService _loanService;
        private readonly IPosApplyService _posApplyService;

        public MpUserController(IMpUserService MpUserService,IMpUserGroupService MpUserGroupService,
            IFunongbaoApplyService funongbaoApplyService,
            ILoanService loanService,
            IPosApplyService posApplyService)
        {
            this.MpUserService = MpUserService;
            this.MpUserGroupService = MpUserGroupService;
            _funongbaoApplyService = funongbaoApplyService;
            _loanService = loanService;
            _posApplyService = posApplyService;
        }
        
        public ActionResult AutoLogin()
        {
            string code =  RQuery["code"];
            string state = RQuery["state"];
            string refUrl = RQuery["refUrl"];
            string openid = RQuery["openid"];

            if (string.IsNullOrEmpty(openid))
            {   
                OAuthAccessTokenResult authResult = OAuth.GetAccessToken(MpCenterCache.AppID, MpCenterCache.AppSecret, code);
                openid = authResult.openid;
            }
            if (!string.IsNullOrEmpty(openid))
            {
                MpUser currUser = MpUserService.GetByOpenID(openid);
                if (currUser == null)
                {
                    currUser = new MpUser();
                    currUser.MpID = MpCenterCache.Id;
                    currUser.CreateDate = DateTime.Now;
                    currUser.MpGroupID = MpUserGroupService.GetSystemGroup("默认分组").Id;
                    currUser.IsSubscribe = true;
                    currUser.LastVisitDate = DateTime.Now;
                    currUser.OpenID = openid;
                    currUser.OrginID = MpCenterCache.OriginID;
                    currUser.UserState = 0;
                    MpUserService.Insert(currUser);
                }
                if (currUser != null)
                {
                    Log4NetImpl.Write("oAuth:" + currUser.OpenID);
                    WriteMpUserCookie(currUser);
                }
                if (string.IsNullOrEmpty(OpenID))
                {
                    return Content("用户未识别");
                }
                if (!string.IsNullOrEmpty(refUrl))
                {
                    Response.Redirect(refUrl);
                }
            }            
            return Content("请联系管理员");
        }
        public ActionResult AutoLogin2()
        {
            string code = RQuery["code"];
            string state = RQuery["state"];
            string refUrl = RQuery["refUrl"];
            string openid = RQuery["openid"];
            string gid = RQuery["guid"];
            Log4NetImpl.Write("refUrl:" + refUrl);
            if (string.IsNullOrEmpty(openid))
            {
                OAuthAccessTokenResult authResult = OAuth.GetAccessToken(MpCenterCache.AppID, MpCenterCache.AppSecret, code);
                openid = authResult.openid;
            }
            if (!string.IsNullOrEmpty(openid))
            {
                Log4NetImpl.Write("currUser:" + openid);
                MpUser currUser = MpUserService.GetByOpenID(openid);
                if (currUser == null)
                {
                    currUser = new MpUser();
                    currUser.MpID = MpCenterCache.Id;
                    currUser.CreateDate = DateTime.Now;
                    currUser.MpGroupID = MpUserGroupService.GetSystemGroup("默认分组").Id;
                    currUser.IsSubscribe = true;
                    currUser.LastVisitDate = DateTime.Now;
                    currUser.OpenID = openid;
                    currUser.OrginID = MpCenterCache.OriginID;
                    currUser.UserState = 0;
                    MpUserService.Insert(currUser);
                }
                if (currUser != null)
                {
                    Log4NetImpl.Write("oAuth:" + currUser.OpenID);
                    WriteMpUserCookie(currUser);
                }
                if (string.IsNullOrEmpty(OpenID))
                {
                    Log4NetImpl.Write("用户未识别:" + currUser.OpenID);
                    return Content("用户未识别");
                }
                if (!string.IsNullOrEmpty(refUrl))
                {
                    Log4NetImpl.Write("跳转回来！");
                    //Response.Redirect(refUrl);
                    return RedirectToRoute(new { controller = "PluginsScene", action = "PluginScratch", id = gid });

                }
            }
            return Content("请联系管理员");
        }
        public ActionResult LogOut()
        {
            string refUrl = RQuery["refUrl"];
            ClearCookie();
            string str=string.IsNullOrEmpty(OpenID)?"":OpenID;
            string str1 = string.IsNullOrEmpty(MpUserID.ToString()) ? "" : MpUserID.ToString();
            string str2 = Session["MpUserID"] == null ? "" : Session["MpUserID"].ToString();
            return Content(str+","+str1+","+str2);
        }
        public ActionResult Ucenter()
        {
             MpUser currUser = MpUserService.GetById(this.MpUserID);
            if(currUser!=null)
            {
                ViewBag.Point = currUser.LocationX + "," + currUser.LocationY;
            }
            return View();
        }
        public ActionResult MyBusiness()
        {
            OceanDynamicList<object> bizList = _mpUserService.GetMyBusiness(this.MpUserID);
            return View(bizList);
        }

        public ActionResult Business(int applyType,Guid id)
        {
            OceanDynamic model = null;
            if (applyType == 1)
            {
                model = _funongbaoApplyService.GetDynamicUnique("select 1 as ApplyType,* from FunongbaoApply where Id='" + id.ToString() + "'");
            }
            else if (applyType == 2)
            {

                model = _funongbaoApplyService.GetDynamicUnique("select 2 as ApplyType,* from Loan where Id='" + id.ToString() + "'");
            }
            else
            {

                model = _funongbaoApplyService.GetDynamicUnique("select 3 as ApplyType,* from PosApply where Id='" + id.ToString() + "'");
            }
            return View(model);
        }
    }
}