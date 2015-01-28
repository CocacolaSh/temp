using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Entity.Enums;
using Ocean.Page;
using Ocean.Framework.Caching.Cache;
using Ocean.Core;
using Ocean.Entity;
using Ocean.Services;
using Ocean.Core.Logging;

namespace Ocean.Web.Controllers
{
    public class IdentAuthController : WeixinOAuthController
    {
        private readonly IFunongbaoService _funongbaoService;
        private readonly IMobileCodeService _mobileCodeService;
        private readonly IMpUserService _mpUserservice;
        public IdentAuthController(IFunongbaoService funongbaoService,IMobileCodeService mobileCodeService
            , IMpUserService mpUserservice)
        {
            _funongbaoService = funongbaoService;
            _mobileCodeService = mobileCodeService;
            _mpUserservice = mpUserservice;
        }
        public ActionResult AgreementDoc()
        {
            return View();
        }
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(this.OpenID))
            {
                Funongbao fnb = _funongbaoService.GetFunongbaoByOpenid(this.OpenID);
                if (fnb != null)
                {
                    return new RedirectResult("/funongbao/apply");
                }
            }
           
            Funongbao funongbao = new Funongbao();
            if (WebHelper.IsPost())
            {
                ViewBag.AuthStatus = 0;
                TryUpdateModel<Funongbao>(funongbao);
                int useCount = _mobileCodeService.GetMobileCodeCount(this.MpUserID, 1);//当天使用次数
                if (useCount <= 5)
                {
                    MobileCode lastCode = _mobileCodeService.GetMobileCode(this.MpUserID,1);//是否存在未使用的证码
                    if (lastCode != null)
                    {
                        MobileCode checkCode = _mobileCodeService.GetMobileCode(funongbao.MobilePhone, WebHelper.GetString("Code"), this.MpUserID, 1);//检查验证码
                        if (checkCode != null)
                        {
                            TimeSpan ts = DateTime.Now.Subtract(checkCode.CreateDate).Duration();
                            if (ts.Minutes > 10)
                            {
                                checkCode.Time += 1;
                                checkCode.Status = 1;
                                _mobileCodeService.Update(lastCode);
                                ViewBag.CodeError = "验证码已经失效，请重新获取！";
                            }
                            else
                            {
                                Funongbao authFnb = null;
                                int authStatus = _funongbaoService.IdentAuth2(funongbao, out authFnb);
                                if (authStatus == -1)
                                {
                                    ViewBag.CodeError = "对不起，该客户已经被绑定了，您无法再次绑定！";
                                }
                                else
                                {
                                    if (authStatus == 1)
                                    {
                                        //更新用户认证并更新cookie
                                        MpUser user = _mpUserservice.GetById(this.MpUserID);
                                        user.Name = funongbao.Name;
                                        user.PassportNO = funongbao.PassportNO;
                                        user.MobilePhone = funongbao.MobilePhone;
                                        user.IsAuth = 1;
                                        //更新当前验证码失败
                                        checkCode.Status = 1;//已用
                                        //更新福农保信息
                                        authFnb.IsAuth = 1;
                                        authFnb.IsSignAgreement = WebHelper.GetInt("IsSignAgreement", 1);
                                        authFnb.MpUserId = this.MpUserID;
                                        authFnb.OpenId = this.OpenID;
                                        try
                                        {
                                            Log4NetImpl.Write("测试事务写入");
                                            _funongbaoService.BeginTransaction();
                                            _mpUserservice.Update(user);
                                            _mobileCodeService.Update(checkCode);
                                            _funongbaoService.Update(authFnb);
                                            _funongbaoService.Commit();
                                            WriteMpUserCookie(user);
                                            Log4NetImpl.Write("测试获取cookie");
                                            var a = MpUserArr;
                                            if (!string.IsNullOrEmpty(WebHelper.GetString("refUrl")))
                                            {
                                                return Redirect(WebHelper.GetString("refUrl"));
                                            }
                                            else
                                            {
                                                return RedirectToAction("index", "funongbao");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _funongbaoService.Rollback();
                                            Log4NetImpl.Write(ex.ToString());
                                            throw new OceanException("对不起，系统异常，身份认证失败！", ex);
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.AuthStatus = authStatus;
                                    }
                                }
                            }

                        }
                        else
                        {
                            lastCode.Time += 1;
                            _mobileCodeService.Update(lastCode);
                            ViewBag.CodeError = "对不起，验证码有误，请检查！";
                        }
                    }
                    else
                    {
                        ViewBag.CodeError = "对不起，您还未点击发送验证码！";
                    }
                }
                else
                {
                    ViewBag.CodeError = "对不起，您今天最多只能发起5次验证码";
                }
            }
            return View(funongbao);
        }
    }
}
