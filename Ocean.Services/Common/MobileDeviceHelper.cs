using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ocean.Services
{
    /// <summary>
    /// Mobile device helper
    /// </summary>
    public partial class MobileDeviceHelper : IMobileDeviceHelper
    {
        #region Fields

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public MobileDeviceHelper()
        {
           
        }

        #endregion

        #region Methods


        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        public virtual bool IsMobileDevice(HttpContextBase httpContext)
        {
            return httpContext.Request.Browser.IsMobileDevice;
        }

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        public virtual bool MobileDevicesSupported()
        {
            return true;
        }

        /// <summary>
        /// Returns a value indicating whether current customer prefer to use full desktop version (even request is made by a mobile device)
        /// </summary>
        public virtual bool CustomerDontUseMobileVersion()
        {
            return true;
        }

        #endregion
    }
}