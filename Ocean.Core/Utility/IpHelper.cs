using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ocean.Core.Common;
using System.Xml;

namespace Ocean.Core.Utility
{
    public static class IpHelper
    {
        #region 用户真实IP
        /// <summary>
        /// 用户真实IP
        /// </summary>
        public static string UserHostAddress
        {
            get
            {
                string result = String.Empty;

                result = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];

                if (null == result || result == String.Empty)
                {
                    result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }

                if (null == result || result == String.Empty)
                {
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (null == result || result == String.Empty)
                {
                    result = HttpContext.Current.Request.UserHostAddress;
                }

                if (null == result || result == String.Empty || !StringValidate.IsIp(result))
                {
                    result = "127.0.0.1";
                }

                return result;
            }
        }
        #endregion

        #region 读取http://www.yodao.com接口IP地址
        /// <summary>
        /// 读取http://www.yodao.com接口IP地址
        /// </summary>
        public static string GetstringIpAddress(string ip)
        {
            string url = "http://www.youdao.com/smartresult-xml/search.s?type=ip&q=" + ip + "";//youdao的URL
            string ipAddress = "";

            using (XmlReader read = XmlReader.Create(url))//获取youdao返回的xml格式文件内容
            {
                while (read.Read())
                {
                    switch (read.NodeType)
                    {
                        case XmlNodeType.Text://取xml格式文件当中的文本内容
                            if (string.Format("{0}", read.Value).Trim() != ip)//youdao返回的xml格式文件内容一个是IP，另一个是IP地址，如果不是IP那么就是IP地址
                            {
                                ipAddress = string.Format("{0}", read.Value).Trim();//赋值
                            }
                            break;
                        //other
                    }
                }
            }
            return ipAddress;
        }
        #endregion
    }
}
