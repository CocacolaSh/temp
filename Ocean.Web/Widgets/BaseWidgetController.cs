using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ocean.Web.Widgets
{
    public abstract class BaseWidgetController : Controller
    {
        #region 挂件名称 public virtual string WidgetName
        /// <summary>
        /// 挂件名称
        /// </summary>
        public virtual string WidgetName
        {
            get
            {
                return "WidgetName";
            }
        }
        #endregion

        #region 挂件标识 public virtual string WidgetIdentifying
        /// <summary>
        /// 挂件标识
        /// </summary>
        public virtual string WidgetIdentifying
        {
            get
            {
                return "WidgetIdentifying";
            }
        }
        #endregion

        #region 渲染挂件 public abstract void RenderWidget();
        /// <summary>
        /// 渲染挂件
        /// </summary>
        public abstract ActionResult RenderWidget(string layout = "Layout_1_1", bool mobile = false);
        #endregion
    }
}