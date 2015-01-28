using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ocean.Core.Common;
using Ocean.Framework.Mvc.PostTokens;

namespace Ocean.Framework.Mvc
{
    public static class HtmlHelperExt
    {
        public static HtmlString GeneratePostToken(this HtmlHelper htmlhelper)
        {
            string formValue = Hash.MD5Encrypt(HttpContext.Current.Session.SessionID + DateTime.Now.Ticks.ToString());
            HttpContext.Current.Session[PostTokenBase.MyToken] = formValue;

            string fieldName = PostTokenBase.TokenName;
            TagBuilder builder = new TagBuilder("input");
            builder.Attributes["type"] = "hidden";
            builder.Attributes["name"] = fieldName;
            builder.Attributes["value"] = formValue;
            return new HtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}
