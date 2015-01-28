using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ocean.Core.Configuration;
namespace Ocean.Framework.Mvc.Extensions
{
    public static class UrlHelperExtensions
    {
        static BaseConfigInfo config = BaseConfigs.GetConfig();
        public static string ImagesUrl(this UrlHelper helper, string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.WebImagesUrl + config.Lang + "/" + config.Template + "/" + config.Skin + "/" + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.WebImagesUrl + path;
            }
        }
        public static string ImagesRoot(this UrlHelper helper, string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic) { 
            return config.WebImagesUrl + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.WebImagesUrl + path;
            }
        }
        public static string ImagesRoot(string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.WebImagesUrl + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.WebImagesUrl + path;
            }
        }
        public static string ImagesTemplte(this UrlHelper helper, string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.WebImagesUrl + config.Lang + "/" + config.Template + "/" + path + "?p_v=" + config.PublishVesion; ;
            }
            else
            {
                return config.WebImagesUrl + config.Lang + "/" + config.Template + "/" + path;
            }
        }
        public static string ImagesTemplte(string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.WebImagesUrl + config.Lang + "/" + config.Template + "/" + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.WebImagesUrl + config.Lang + "/" + config.Template + "/" + path;
            }
        }

        #region cdn
        public static string ImagesCdnUrl(this UrlHelper helper, string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.CdnImagesUrl + config.Lang + "/" + config.Template + "/" + config.Skin + "/" + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.CdnImagesUrl + config.Lang + "/" + config.Template + "/" + config.Skin + "/" + path;
            }
        }
        public static string ImagesCdnRoot(this UrlHelper helper, string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.CdnImagesUrl + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.CdnImagesUrl + path;
            }
        }
        public static string ImagesCdnRoot(string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.CdnImagesUrl + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.CdnImagesUrl + path;
            }
        }
        public static string ImagesCdnTemplte(this UrlHelper helper, string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.CdnImagesUrl + config.Lang + "/" + config.Template + "/" + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.CdnImagesUrl + config.Lang + "/" + config.Template + "/" + path;
            }
        }
        public static string ImagesCdnTemplte(string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return config.CdnImagesUrl + config.Lang + "/" + config.Template + "/" + path + "?p_v=" + config.PublishVesion;
            }
            else
            {
                return config.CdnImagesUrl + config.Lang + "/" + config.Template + "/" + path;
            }
        }
        #endregion

       
        ///// <summary>
        ///// 控件应用静态资源访问地址
        ///// [调用方式:原访问(/widgets/docCategory/widget.css)=>Url.WidgetUrl("docCategory/widget.css")]
        ///// </summary>
        ///// <param name="helper"></param>
        ///// <param name="path">路径[调用方式中："docCategory/widget.css"]</param>
        ///// <returns></returns>
        //public static string WidgetUrl(this UrlHelper helper, string path, bool isStatic = true)
        //{
        //    if (!string.IsNullOrEmpty(path) && isStatic)
        //    {
        //        return config.WidgetCdn + path + "?p_v=" + config.PublishVesion;
        //    }
        //    else
        //    {
        //        return config.WidgetCdn + path;
        //    }
        //}
        ///// <summary>
        ///// 控件Style文件夹中静态资源访问地址
        ///// [调用方式:原访问(/styles/navs/nav_zd_1/dhpic.png)=>Url.StyleUrl("navs/nav_zd_1/dhpic.png")]
        ///// </summary>
        ///// <param name="helper"></param>
        ///// <param name="path">路径[调用方式中："navs/nav_zd_1/dhpic.png"]</param>
        ///// <returns></returns>
        //public static string StyleUrl(this UrlHelper helper, string path, bool isStatic = true)
        //{
        //    if (!string.IsNullOrEmpty(path) && isStatic)
        //    {
        //        return config.StyleCdn + path + "?p_v=" + config.PublishVesion;
        //    }
        //    else
        //    {
        //        return config.StyleCdn + path;
        //    }
        //}
        /// <summary>
        /// 插件应用静态资源访问地址
        /// [调用方式:原访问(/plugins/lottery/plugin.css)=>Url.PluginUrl("docCategory/widget.css")]
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path">路径[调用方式中："docCategory/widget.css"]</param>
        /// <returns></returns>
        public static string PluginUrl(this UrlHelper helper, string path, bool isStatic = true)
        {
            if (!string.IsNullOrEmpty(path) && isStatic)
            {
                return "/Themes/default/views/Plugins/" + path + "?v=" + config.PublishVesion;
            }
            else
            {
                return "/Themes/default/views/Plugins/" + path;
            }
        }
    }
}