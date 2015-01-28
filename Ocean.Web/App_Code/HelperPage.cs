using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace Ocean.Web.App_Code
{
    public class HelperPage : System.Web.WebPages.HelperPage
    {
        public static new HtmlHelper Html
        {
            get { return ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).Html; }
        }

        public static new WebViewPage CurrentPage
        {
            get
            {
                return (WebViewPage)WebPageContext.Current.Page;
            }
        }
    }
}