using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Ocean.Core.Plugins.Security
{
    public class TripleDESStringEncrypt : IStringEncrypt
    {

        //密钥
        public string EncryptKey { get; set; }
        //private const string sKey = "qJzGEh6hESZDVJeCnFPGuxzaiB7NLQM3";
        //矢量，矢量可以为空 
        private const string sIV = "qcDY6X+aPLw=";
        //构造一个对称算法 
        private SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();

        /// <summary> 
        /// 加密字符串 
        /// </summary> 
        /// <param name="Value">输入的字符串</param> 
        /// <returns>加密后的字符串</returns> 
        public string EncryptString(string encryptText)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(EncryptKey);
            mCSP.IV = Convert.FromBase64String(sIV);
            //指定加密的运算模式 
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            //获取或设置加密算法的填充模式 
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;//兼容JAVA.ISO10126

            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);
            byt = Encoding.UTF8.GetBytes(encryptText);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary> 
        /// 解密字符串 
        /// </summary> 
        /// <param name="Value">加过密的字符串</param> 
        /// <returns>解密后的字符串</returns> 
        public string DecryptString(string decryptText)
        {
            if (string.IsNullOrWhiteSpace(decryptText))
            {
                return string.Empty;
            }

            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(EncryptKey);
            mCSP.IV = Convert.FromBase64String(sIV);
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);
            byt = Convert.FromBase64String(decryptText);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
