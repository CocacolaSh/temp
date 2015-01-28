using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ocean.Services;
using Ocean.Core.Utility;
using Ocean.Entity;
using Ocean.Core.Logging;
using Ocean.Core;

namespace Ocean.Page
{
    public class IdenAuthBaseController : WeixinOAuthController
    {
        protected readonly IFunongbaoService _funongbaoService;

        public IdenAuthBaseController(IFunongbaoService funongbaoService)
        {
            _funongbaoService = funongbaoService;
        }

        protected Funongbao CurrentFunongbao { get; set; } 

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            try
            {
                if (!string.IsNullOrEmpty(this.OpenID))
                {
                    CurrentFunongbao = _funongbaoService.GetFunongbaoByOpenid(this.OpenID);
                    Log4NetImpl.Write("CurrentFunongbao:");
                    if (CurrentFunongbao == null || CurrentFunongbao.IsAuth == 0)
                    {
                        filterContext.Result = new RedirectResult("/IdentAuth/Index?refUrl=" + this.Request.Url);
                    }
                    else
                    {
                        Log4NetImpl.Write("ViewBag.UserName:");
                        ViewBag.UserName = CurrentFunongbao.Name;
                        if (CurrentFunongbao.PassportType==1)
                        {
                            ViewBag.Sex = StringHelper.GetSexFormIdent(CurrentFunongbao.PassportNO,true);
                        }
                        else
                        {
                            Log4NetImpl.Write("MpUser:");
                            MpUser user = _mpUserService.GetById(this.MpUserID);

                            if (user != null)
                            {
                                ViewBag.Sex = (user.Sex == 1 ? "先生" : "女士");
                            }
                            Log4NetImpl.Write("MpUser: end");
                        }
                        Log4NetImpl.Write("CurrentFunongbao:");
                    }
                }
            }
            catch (Exception ex){
                Log4NetImpl.Write(ex.ToString());
                throw new OceanException(ex.Message);
            }
        }
    }
}
