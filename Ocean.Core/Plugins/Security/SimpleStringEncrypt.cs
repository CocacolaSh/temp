using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Plugins.Security
{
    public class SimpleStringEncrypt : IStringEncrypt
    {
        public string EncryptKey { get; set; }
        private const char XOR_STRING = 'g';

        public string EncryptString(string encryptText)
        {
            char[] sourceChars = encryptText.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < sourceChars.Length; i++)
            {
                sourceChars[i] = (char)(sourceChars[i] ^ XOR_STRING);
                sb.Append(sourceChars[i].ToString());
            }
            return sb.ToString();
        }

        public string DecryptString(string decryptText)
        {
            char[] encodeChars = decryptText.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encodeChars.Length; i++)
            {
                encodeChars[i] = (char)(encodeChars[i] ^ XOR_STRING);
                sb.Append(encodeChars[i].ToString());
            }
            return sb.ToString();
        }
    }
}
