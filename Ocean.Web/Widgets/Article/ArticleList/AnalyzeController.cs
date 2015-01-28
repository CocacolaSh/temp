using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Entity.Widget;

namespace Ocean.Web.Widgets.Article.ArticleList
{
    public class AnalyzeController : BaseWidgetController
    {
        /// <summary>
        /// 挂件名称
        /// </summary>
        public override string WidgetName
        {
            get
            {
                return "文章列表挂件";
            }
        }

        /// <summary>
        /// 挂件标识
        /// </summary>
        public override string WidgetIdentifying
        {
            get
            {
                return WidgetEnum.Article_ArticleList.ToString();
            }
        }

        /// <summary>
        /// 渲染挂件
        /// </summary>
        public override ActionResult RenderWidget(string layout = "Layout_1_1", bool mobile = false)
        {
            //加载数据源
            //ToDo
            layout = string.Format("{0}{1}", layout, (mobile ? ".mobile" : string.Empty));
            return PartialView(string.Format("~/Widgets/{0}/{1}.cshtml", WidgetIdentifying.Replace(".", "/").Replace("_", "/"), layout));
        }
    }
}