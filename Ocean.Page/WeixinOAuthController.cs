using Ocean.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ocean.Page
{
    public class WeixinOAuthController : WebBaseController
    {

        public override int UserLoginValidate(ActionExecutingContext filterContext)
        {
            return 1;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (MpUserID == Guid.Empty)
            {
                string rawUrl = WebHelper.GetRawUrl();

                if (string.IsNullOrEmpty(RQuery["openid"]))
                {
                    Response.Redirect(string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl));
                    return;
                }
                else
                {
                    Response.Redirect("http://localhost:2014/mpuser/autologin?openid=" + RQuery["openid"]);
                }
            }
            else
            {
            }
        }
    }
}