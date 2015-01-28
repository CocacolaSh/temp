using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ocean.Framework.Mvc.PostTokens;

namespace Ocean.Framework.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePostTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public IPostToken PostToken{ get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateReHttpPostTokenAttribute"/> class.
        /// </summary>
        public ValidatePostTokenAttribute()
        {
            //It would be better use DI inject it.
            PostToken = new SessionPostToken();
        }

        /// <summary>
        /// Called when authorization is required.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (!PostToken.TokensMatch)
            {
                //log...
                throw new Exception("Invaild Http Post!");
            }

        }
    }
}
