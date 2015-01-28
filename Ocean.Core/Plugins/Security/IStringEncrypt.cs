using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Plugins.Security
{
    public interface IStringEncrypt
    {
        string EncryptKey { get; set;}
        string EncryptString(string encryptText);
        string DecryptString(string decryptText);
    }
}
