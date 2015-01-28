using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Security.Cryptography;

namespace Ocean.Core.Plugins.Security
{
    /// <summary>
    /// MD5加密算法
    /// </summary>
    public class MD5Encrypt : IPasswordEncrypt
    {
        public MD5Encrypt()
        { }

        public string Encrypt(string password)
        {
            return MD5(password);
        }
        public byte[] ComputeHash(byte[] bytes)
        {
            return MD5ComputeHash(bytes);
        }
        public static string MD5(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
        }
        public static byte[] MD5ComputeHash(byte[] bytes)
        {
            return new MD5CryptoServiceProvider().ComputeHash(bytes);
        }
    }
}
