using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Common;
using System.Web;

namespace Ocean.Framework.Mvc.PostTokens
{
    public class SessionPostToken : PostTokenBase
    {
        #region PageTokenViewBase

        /// <summary>
        /// Generates the page token.
        /// </summary>
        /// <returns></returns>
        public override string GeneratePostToken()
        {
            if (HttpContext.Current.Session[MyToken] != null)
            {
                return HttpContext.Current.Session[MyToken].ToString();
            }
            else
            {
                var token = GenerateHashToken();
                HttpContext.Current.Session[MyToken] = token;
                return token;
            }
        }

        /// <summary>
        /// Gets the get last page token from Form
        /// </summary>
        public override string GetLastPostToken
        {
            get
            {
                return HttpContext.Current.Request.Params[TokenName];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [tokens match].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [tokens match]; otherwise, <c>false</c>.
        /// </value>
        public override bool TokensMatch
        {
            get
            {
                string formToken = GetLastPostToken;
                if (formToken != null)
                {
                    if (formToken.Equals(GeneratePostToken()))
                    {
                        //Refresh token
                        HttpContext.Current.Session[MyToken] = GenerateHashToken();
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Private Help Method
        /// <summary>
        /// Generates the hash token.
        /// </summary>
        /// <returns></returns>
        private string GenerateHashToken()
        {
            return Hash.MD5Encrypt(
                HttpContext.Current.Session.SessionID + DateTime.Now.Ticks.ToString());
        }
        #endregion
    }
}
