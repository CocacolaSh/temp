using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Plugins.Security
{
    /// <summary>
    /// 密码编码接口，目前只做标记使用
    /// </summary>
    public interface IPasswordEncrypt
    {
        string Encrypt(string password);
    }
}
