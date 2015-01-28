/*
 ASP.NET MvcPager control
 Copyright:2009-2010 Webdiyer (http://www.webdiyer.com)
 Source code released under Ms-PL license
 */
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using Ocean.Core;

namespace Ocean.Framework.Mvc.Pagination
{
    public static class PagerHelper
    {
        #region Html Pager
        public static MvcHtmlString Pager(this HtmlHelper helper, int totalPageCount, int pageIndex, string actionName, string controllerName,
            PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        {
            var builder = new PagerBuilder
                (
                    helper,
                    actionName,
                    controllerName,
                    totalPageCount,
                    pageIndex,
                    pagerOptions,
                    routeName,
                    new RouteValueDictionary(routeValues),
                    new RouteValueDictionary(htmlAttributes)
                );
            return builder.RenderPager();
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, int totalPageCount, int pageIndex, string actionName, string controllerName,
            PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            var builder = new PagerBuilder
                (
                    helper,
                    actionName,
                    controllerName,
                    totalPageCount,
                    pageIndex,
                    pagerOptions,
                    routeName,
                    routeValues,
                    htmlAttributes
                );
            return builder.RenderPager();
        }

        private static MvcHtmlString Pager(HtmlHelper helper, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(helper, null, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList)
        {
            if (pagedList == null)
                return Pager(helper, pagedList, null);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, null, null, null);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, null);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, htmlAttributes);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, string routeName, object routeValues)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, null);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, null);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, string routeName,
            RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, htmlAttributes);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, string routeName, object routeValues, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, null, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, string routeName, RouteValueDictionary routeValues,
            IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, null, htmlAttributes);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        }

        #region SiteDynamicList-vebin.h-2011.11.12
        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, pagedList, null);
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, null, null, null);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, PagerOptions pagerOptions)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, pagerOptions, null);
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, null);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, object htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, pagerOptions, new RouteValueDictionary(htmlAttributes));
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, pagerOptions, htmlAttributes);
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, string routeName, object routeValues)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, pagerOptions, null);
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, null);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, pagerOptions, null);
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, null);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, pagerOptions, new RouteValueDictionary(htmlAttributes));
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, string routeName,
        //    RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, pagerOptions, htmlAttributes);
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, string routeName, object routeValues, object htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, null, new RouteValueDictionary(htmlAttributes));
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        //}

        //public static MvcHtmlString Pager(this HtmlHelper helper, SiteDynamicList<object> pagedList, string routeName, RouteValueDictionary routeValues,
        //    IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(helper, null, htmlAttributes);
        //    return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        //}
        #endregion
        #endregion

        #region jQuery Ajax Pager

        private static MvcHtmlString AjaxPager(HtmlHelper html, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(html, null, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, int totalPageCount, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            pagerOptions.UseJqueryAjax = true;

            var builder = new PagerBuilder(html, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, new RouteValueDictionary(routeValues), ajaxOptions, new RouteValueDictionary(htmlAttributes));
            return builder.RenderPager();
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, int totalPageCount, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            pagerOptions.UseJqueryAjax = true;

            var builder = new PagerBuilder(html, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, routeValues, ajaxOptions, htmlAttributes);
            return builder.RenderPager();
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, null, null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, null, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string routeName, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, null, null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, null, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null,
                             ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions,
            IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, htmlAttributes);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null,
                             ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string routeName, object routeValues, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string routeName, object routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions,
                             routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string routeName, RouteValueDictionary routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, htmlAttributes);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions,
                             routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string actionName, string controllerName,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName, null, pagerOptions, null, ajaxOptions,
                             null);
        }

        #region SiteDynamicList-vebin.h-2011.11.12
        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, AjaxOptions ajaxOptions)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, null, null);
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, null, null, ajaxOptions,
        //                     null);
        //}

        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, string routeName, AjaxOptions ajaxOptions)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, null, null);
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, null, null, ajaxOptions,
        //                     null);
        //}

        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, pagerOptions, null);
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions,
        //                     null);
        //}

        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, pagerOptions, new RouteValueDictionary(htmlAttributes));
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null,
        //                     ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions,
        //    IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, pagerOptions, htmlAttributes);
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null,
        //                     ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, string routeName, object routeValues, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, pagerOptions, null);
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions,
        //                     null);
        //}

        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, string routeName, object routeValues,
        //    PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, pagerOptions, new RouteValueDictionary(htmlAttributes));
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions,
        //                     routeValues, ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, string routeName, RouteValueDictionary routeValues,
        //    PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, pagerOptions, htmlAttributes);
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions,
        //                     routeValues, ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, SiteDynamicList<object> pagedList, string actionName, string controllerName,
        //    PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        //{
        //    if (pagedList == null)
        //        return AjaxPager(html, pagerOptions, null);
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName, null, pagerOptions, null, ajaxOptions,
        //                     null);
        //}
        #endregion
        #endregion

        #region Microsoft Ajax Pager

        public static MvcHtmlString Pager(this AjaxHelper ajax, int totalPageCount, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var builder = new PagerBuilder(ajax, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, new RouteValueDictionary(routeValues), ajaxOptions, new RouteValueDictionary(htmlAttributes));
            return builder.RenderPager();
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, int totalPageCount, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            var builder = new PagerBuilder(ajax, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, routeValues, ajaxOptions, htmlAttributes);
            return builder.RenderPager();
        }

        private static MvcHtmlString Pager(AjaxHelper ajax, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(null, ajax, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, AjaxOptions ajaxOptions)
        {
            return pagedList == null ? Pager(ajax, null, null) : Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, null, null, ajaxOptions, null);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            return pagedList == null ? Pager(ajax, pagerOptions, null) : Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex,
                null, null, null, pagerOptions, null, ajaxOptions, null);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, htmlAttributes);
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, string routeName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, null, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, string routeName, RouteValueDictionary routeValues,
            AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, null, htmlAttributes);
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, string routeName, object routeValues, PagerOptions pagerOptions,
            AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, string routeName, RouteValueDictionary routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, htmlAttributes);
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        }

        #region SiteDynamicList-vebin.h-2011.11.12
        //public static MvcHtmlString Pager<T>(this AjaxHelper ajax, SiteDynamicList<object> pagedList, AjaxOptions ajaxOptions)
        //{
        //    return pagedList == null ? Pager(ajax, null, null) : Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, null, null, ajaxOptions, null);
        //}

        //public static MvcHtmlString Pager<T>(this AjaxHelper ajax, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        //{
        //    return pagedList == null ? Pager(ajax, pagerOptions, null) : Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex,
        //        null, null, null, pagerOptions, null, ajaxOptions, null);
        //}

        //public static MvcHtmlString Pager<T>(this AjaxHelper ajax, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(ajax, pagerOptions, new RouteValueDictionary(htmlAttributes));
        //    return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString Pager<T>(this AjaxHelper ajax, SiteDynamicList<object> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(ajax, pagerOptions, htmlAttributes);
        //    return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString Pager<T>(this AjaxHelper ajax, SiteDynamicList<object> pagedList, string routeName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(ajax, null, new RouteValueDictionary(htmlAttributes));
        //    return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString Pager<T>(this AjaxHelper ajax, SiteDynamicList<object> pagedList, string routeName, RouteValueDictionary routeValues,
        //    AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(ajax, null, htmlAttributes);
        //    return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString Pager<T>(this AjaxHelper ajax, SiteDynamicList<object> pagedList, string routeName, object routeValues, PagerOptions pagerOptions,
        //    AjaxOptions ajaxOptions, object htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(ajax, pagerOptions, new RouteValueDictionary(htmlAttributes));
        //    return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        //}

        //public static MvcHtmlString Pager<T>(this AjaxHelper ajax, SiteDynamicList<object> pagedList, string routeName, RouteValueDictionary routeValues,
        //    PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(ajax, pagerOptions, htmlAttributes);
        //    return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        //}
        #endregion
        #endregion

        #region using mode moredu 2011.12.14

        public static MvcHtmlString PagerForMode(this HtmlHelper html, int totalPageCount, int pageIndex, string actionName, string controllerName,
            string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, PaginationMode mode = PaginationMode.NumericNextPrevious, ShowInfoGoMode gom = ShowInfoGoMode.Default, PageIndexBoxType pageIndexBoxType = PageIndexBoxType.TextBox, string pageNumberFormatString = "")
        {
            PagerOptions pageOpt = new PagerOptions()
            {
                //PageIndexParameterName = "pageIndex",
                AutoHide = true,
                /*跳转区*/
                ShowPageIndexBox = true,/*是否显示跳转*/
                PageIndexBoxType = pageIndexBoxType,/*跳转输入方式*/
                ShowGoButton = true,
                ShowInfo = true,
                PageIndexBoxWrapperFormatString = "到第{0}页",
                CssClass = "pager",
                /*FirstPageText = "第一页",
                PrevPageText = "前一页",
                NextPageText = "下一页",
                LastPageText = "最后页",
                PagerItemWrapperCss = "page-default",*/
                AlwaysShowFirstLastPageNumber = true,
                PrevItemWrapperCss = "page-pre",
                NextItemWrapperCss = "page-next",
                LastItemWrapperCss = "page-last",
                FirstItemWrapperCss = "page-fir",
                /*数字部分
                NumericPagerItemWrapperCss = "page-break",*/
                ShowNumericPagerItems = true,
                PageNumberFormatString = pageNumberFormatString,
                PageNumberCss = "page-num",
                SeparatorHtml = " ",
                CurrentPagerItemWrapperCss = "page-cur",
                DisabledCss = "page-disabled",
                MorePagerItemWrapperCss = "<span>{0}</span>",
                GoToPageSectionWrapperFormatString = ""
            };
            if (mode == PaginationMode.NextPrevious)
            {
                pageOpt.AlwaysShowFirstLastPageNumber = false;
                pageOpt.ShowPageIndexBox = false;
                pageOpt.ShowNumericPagerItems = false;
                pageOpt.ShowFirstLast = false;
                pageOpt.ShowMorePagerItems = false;
                pageOpt.TotalInfoFormatString = "共%_TotalPage_%页 当前第%_PageIndex_%页";
            }
            if (mode == PaginationMode.NextPreviousFirstLast)
            {
                pageOpt.ShowPageIndexBox = false;
                pageOpt.ShowNumericPagerItems = false;
            }
            if (mode == PaginationMode.Numerics)
            {
                pageOpt.AlwaysShowFirstLastPageNumber = false;
                pageOpt.ShowFirstLast = false;
            }
            if (gom == ShowInfoGoMode.Simple)
            {
                pageOpt.PageIndexBoxWrapperFormatString = "";
                pageOpt.TotalInfoFormatString = "%_PageIndex_%/%_TotalPage_%";
            }
            if (gom == ShowInfoGoMode.OnlyShowInfo)
            {
                pageOpt.ShowPageIndexBox = false;
            }
            if (gom == ShowInfoGoMode.OnlyInfoSimple)
            {
                pageOpt.ShowPageIndexBox = false;
                pageOpt.PageIndexBoxWrapperFormatString = "";
                pageOpt.TotalInfoFormatString = "%_PageIndex_%/%_TotalPage_%";
            }

            if (gom == ShowInfoGoMode.None)
            {
                pageOpt.ShowPageIndexBox = false;
                pageOpt.ShowInfo = false;
            }
            return Pager(html, totalPageCount, pageIndex, actionName, controllerName, pageOpt, routeName, routeValues, htmlAttributes);
        }

        //public static MvcHtmlString PagerForMode(this HtmlHelper html, SiteDynamicList<object> pagedList, string actionName, string controllerName,
        //    string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, PaginationMode mode = PaginationMode.NumericNextPrevious, ShowInfoGoMode gom = ShowInfoGoMode.Default, PageIndexBoxType pageIndexBoxType = PageIndexBoxType.TextBox, string pageNumberFormatString = "")
        //{
        //    if (pagedList == null)
        //        return Pager(html, null, new RouteValueDictionary(htmlAttributes));
        //    return PagerForMode(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName,
        //        routeName, routeValues, htmlAttributes, mode, gom, pageIndexBoxType, pageNumberFormatString);
        //}

        public static MvcHtmlString PagerForMode<T>(this HtmlHelper html, PagedList<T> pagedList, string actionName, string controllerName,
            string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, PaginationMode mode = PaginationMode.NumericNextPrevious, ShowInfoGoMode gom = ShowInfoGoMode.Default, PageIndexBoxType pageIndexBoxType = PageIndexBoxType.TextBox, string pageNumberFormatString = "")
        {
            if (pagedList == null)
                return Pager(html, null, new RouteValueDictionary(htmlAttributes));
            return PagerForMode(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName,
                routeName, routeValues, htmlAttributes, mode, gom, pageIndexBoxType, pageNumberFormatString);
        }

        public static MvcHtmlString AjaxPagerForMode(this HtmlHelper html, int totalPageCount, int pageIndex, string actionName, string controllerName,
            string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes, PaginationMode mode = PaginationMode.NextPreviousFirstLast, ShowInfoGoMode gom = ShowInfoGoMode.Default, PageIndexBoxType pageIndexBoxType = PageIndexBoxType.TextBox, string pageNumberFormatString = "")
        {
            PagerOptions pageOpt = new PagerOptions()
            {
                //PageIndexParameterName = "pageIndex",
                AutoHide = true,
                /*跳转区*/
                ShowPageIndexBox = true,/*是否显示跳转*/
                PageIndexBoxType = pageIndexBoxType,/*跳转输入方式*/
                ShowGoButton = true,
                ShowInfo = true,
                PageIndexBoxWrapperFormatString = "到第{0}页",
                CssClass = "pager",
                /*FirstPageText = "第一页",
                PrevPageText = "前一页",
                NextPageText = "下一页",
                LastPageText = "最后页",
                PagerItemWrapperCss = "page-default",*/
                AlwaysShowFirstLastPageNumber = true,
                PrevItemWrapperCss = "page-pre",
                NextItemWrapperCss = "page-next",
                LastItemWrapperCss = "page-last",
                FirstItemWrapperCss = "page-fir",
                /*数字部分
                NumericPagerItemWrapperCss = "page-break",*/
                ShowNumericPagerItems = true,
                PageNumberFormatString = pageNumberFormatString,
                PageNumberCss = "page-num",
                SeparatorHtml = " ",
                CurrentPagerItemWrapperCss = "page-cur",
                DisabledCss = "page-disabled",
                MorePagerItemWrapperCss = "<span>{0}</span>",
                GoToPageSectionWrapperFormatString = ""
            };
            if (mode == PaginationMode.NextPrevious)
            {
                pageOpt.AlwaysShowFirstLastPageNumber = false;
                pageOpt.ShowPageIndexBox = false;
                pageOpt.ShowNumericPagerItems = false;
                pageOpt.ShowFirstLast = false;
                pageOpt.ShowMorePagerItems = false;
                pageOpt.TotalInfoFormatString = "共%_TotalPage_%页 当前第%_PageIndex_%页";
            }
            if (mode == PaginationMode.NextPreviousFirstLast)
            {
                pageOpt.ShowPageIndexBox = false;
                pageOpt.ShowNumericPagerItems = false;
            }
            if (mode == PaginationMode.Numerics)
            {
                pageOpt.AlwaysShowFirstLastPageNumber = false;
                pageOpt.ShowFirstLast = false;
            }
            if (gom == ShowInfoGoMode.Simple)
            {
                pageOpt.PageIndexBoxWrapperFormatString = "";
                pageOpt.TotalInfoFormatString = "%_PageIndex_%/%_TotalPage_%";
            }
            if (gom == ShowInfoGoMode.OnlyShowInfo)
            {
                pageOpt.ShowPageIndexBox = false;
            }
            if (gom == ShowInfoGoMode.OnlyInfoSimple)
            {
                pageOpt.ShowPageIndexBox = false;
                pageOpt.TotalInfoFormatString = "%_PageIndex_%/%_TotalPage_%";
            }

            if (gom == ShowInfoGoMode.None)
            {
                pageOpt.ShowPageIndexBox = false;
                pageOpt.ShowInfo = false;
            }
            return AjaxPager(html, totalPageCount, pageIndex, actionName, controllerName, routeName, pageOpt, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPagerForMode<T>(this HtmlHelper html, PagedList<T> pagedList, string actionName, string controllerName,
            string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes, PaginationMode mode = PaginationMode.NextPreviousFirstLast, ShowInfoGoMode gom = ShowInfoGoMode.Default, PageIndexBoxType pageIndexBoxType = PageIndexBoxType.TextBox, string pageNumberFormatString = "")
        {
            if (pagedList == null)
                return Pager(html, null, htmlAttributes);
            return AjaxPagerForMode(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName, routeName, routeValues, ajaxOptions, htmlAttributes, mode, gom, pageIndexBoxType, pageNumberFormatString);
        }
        //public static MvcHtmlString AjaxPagerForMode(this HtmlHelper html, SiteDynamicList<object> pagedList, string actionName, string controllerName,
        //    string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes, PaginationMode mode = PaginationMode.NextPreviousFirstLast, ShowInfoGoMode gom = ShowInfoGoMode.Default, PageIndexBoxType pageIndexBoxType = PageIndexBoxType.TextBox, string pageNumberFormatString = "")
        //{
        //    if (pagedList == null)
        //        return Pager(html, null, htmlAttributes);
        //    return AjaxPagerForMode(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName, routeName, routeValues, ajaxOptions, htmlAttributes, mode, gom, pageIndexBoxType, pageNumberFormatString);
        //}
        //public static MvcHtmlString AjaxPagerForMode(this HtmlHelper html, SiteDynamicList<object> pagedList, string actionName, string controllerName,
        //    string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, PagerOptions pageOpt, IDictionary<string, object> htmlAttributes)
        //{
        //    if (pagedList == null)
        //        return Pager(html, null, htmlAttributes);
        //    return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName, routeName, pageOpt, routeValues, ajaxOptions, htmlAttributes);
        //}
        
        #endregion
    }
}