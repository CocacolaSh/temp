using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ocean.Services
{
    /// <summary>
    /// 移动设备辅助接口[临时的]
    /// </summary>
    public partial interface IMobileDeviceHelper
    {
        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// 返回一个值,该值指示请求是否由一个移动设备
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        bool IsMobileDevice(HttpContextBase httpContext);

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// 返回一个值指示是否启用移动设备支持
        /// </summary>
        bool MobileDevicesSupported();

        /// <summary>
        /// Returns a value indicating whether current customer prefer to use full desktop version (even request is made by a mobile device)
        /// 返回一个值,该值指示当前客户是否喜欢使用完整的桌面版(甚至请求是由一个移动设备)
        /// </summary>
        bool CustomerDontUseMobileVersion();
    }
}