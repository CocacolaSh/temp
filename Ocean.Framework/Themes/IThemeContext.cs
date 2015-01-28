using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.Themes
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IThemeContext
    {
        /// <summary>
        /// 获取或设置当前桌面主题
        /// </summary>
        string WorkingDesktopTheme { get; set; }

        /// <summary>
        /// 当前移动端主题
        /// </summary>
        string WorkingMobileTheme { get; }
    }
}