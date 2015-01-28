using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Common
{
    public static class Hash
    {
        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">要加密的字串</param>
        /// <returns>密文</returns>
        public static string MD5Encrypt(string input)
        {
            return HashEncrypt(input, "MD5");
        }
        #endregion

        #region SHA1加密
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="input">要加密的字串</param>
        /// <returns>密文</returns>
        public static string SHA1Encrypt(string input)
        {
            return HashEncrypt(input, "SHA1");
        }
        #endregion

        #region SHA1,MD5加密
        /// <summary>
        /// SHA1,MD5加密 
        /// </summary>
        /// <param name="input">要加密的字符串</param>
        /// <param name="format">加密格式 MD5 或 SHA1</param>
        /// <returns>返回加密后的字串</returns>		
        private static string HashEncrypt(string input, string format)
        {
            if (format == "SHA1")
            {
                input = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(input + SaltValue(input), "SHA1");
            }
            else if (format == "MD5")
            {
                input = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(input + SaltValue(input), "MD5");
            }

            return input;
        }
        #endregion

        #region 获取盐值
        /// <summary>
        /// 获取盐值
        /// </summary>
        /// <param name="InputString"></param>
        private static string SaltValue(string InputString)
        {
            InputString = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(InputString, "MD5");
            return InputString.Replace("-", string.Empty).Substring(0, 16);
        }
        #endregion
    }
}