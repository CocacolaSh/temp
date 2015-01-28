using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Ocean.Core.Logging;

namespace Senparc.Weixin.MP
{
    public class HttpHelper
    {
        #region 私有变量
        private static CookieContainer cc = new CookieContainer();
        private static string contentType = "application/x-www-form-urlencoded";
        private static string contentYixinType = "application/json";
        private static string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        private static string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.76 Safari/537.36";
        private static Encoding encoding = Encoding.GetEncoding("utf-8");
        private static int delay = 0;//延迟访问防止连续访问被发现
        private static int maxTry = 10;
        private static int currentTry = 0;
        public static string myCookies = "";
        #endregion

        #region 属性
        /// <summary></summary>
        /// Cookie容器
        /// 
        public static CookieContainer CookieContainer
        {
            get
            {
                return cc;
            }
        }

        /// <summary></summary>
        /// 获取网页源码时使用的编码
        /// 
        /// <value></value>
        public static Encoding Encoding
        {
            get
            {
                return encoding;
            }
            set
            {
                encoding = value;
            }
        }

        public static int NetworkDelay
        {
            get
            {
                Random r = new Random();
                return (r.Next(delay / 1000, delay / 1000 * 2)) * 1000;
            }
            set
            {
                delay = value;
            }
        }

        public static int MaxTry
        {
            get
            {
                return maxTry;
            }
            set
            {
                maxTry = value;
            }
        }
        #endregion

        #region 公共方法
        /// <summary></summary>
        /// 获取指定页面的HTML代码
        /// 
        /// <param name="url">指定页面的路径
        /// <param name="postData">回发的数据
        /// <param name="isPost">是否以post方式发送请求
        /// <param name="cookieCollection">Cookie集合
        /// <returns></returns>
        public static string GetHtml(string url, string postData, bool isPost, CookieContainer cookieContainer, string myCookie = "", string referUrl = "",string proIp="")
        {

            if (string.IsNullOrEmpty(postData))
            {
                return GetHtml(url, cookieContainer, "", "", referUrl);
            }

            currentTry++;

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                byte[] byteRequest = Encoding.UTF8.GetBytes(postData);
                if (cookieContainer == null)
                {
                    cookieContainer = new System.Net.CookieContainer();
                }
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.ContentType = contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                if (string.IsNullOrEmpty(referUrl))
                {
                    referUrl = url;
                }
                httpWebRequest.Referer = referUrl;
                httpWebRequest.Accept = accept;
                httpWebRequest.Timeout = 3500;
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Method = isPost ? "POST" : "GET";
                //httpWebRequest.Headers.Set("Accept-Encoding", "gzip,deflate,sdch");
                if (!string.IsNullOrEmpty(proIp))
                {
                    SetProxy(httpWebRequest, proIp);
                }

                httpWebRequest.Headers.Set("Accept-Language", "zh-CN,zh;q=0.8");
                httpWebRequest.Headers.Set("Cookie", myCookie);//===
                httpWebRequest.ContentLength = byteRequest.Length;

                Stream stream = httpWebRequest.GetRequestStream();
                stream.Write(byteRequest, 0, byteRequest.Length);
                stream.Close();

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                currentTry = 0;

                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                //Logs.Info("CoreHttpHelper-Post:" + url + ":" + e.Message);

                if (currentTry <= maxTry)
                {
                    //GetHtml(url, postData, isPost, cookieContainer, referUrl);
                }
                currentTry--;

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                } if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 使用Post方法获取字符串结果
        /// </summary>
        /// <returns></returns>
        public static string HttpPost(string url, CookieContainer cookieContainer = null, string dataString = "", Encoding encoding = null)
        {
            var formDataBytes = dataString == "" ? new byte[0] : Encoding.UTF8.GetBytes(dataString);
            MemoryStream ms = new MemoryStream();
            ms.Write(formDataBytes, 0, formDataBytes.Length);
            ms.Seek(0, SeekOrigin.Begin);//设置指针读取位置
            return HttpPost(url, cookieContainer, ms, false, encoding);
        }

        public static string HttpPost(string url, CookieContainer cookieContainer = null, Stream postStream = null, bool isFile = false, Encoding encoding = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postStream != null ? postStream.Length : 0;

            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }

            if (postStream != null)
            {
                //postStream.Position = 0;

                //上传文件流
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                postStream.Close();//关闭文件访问
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream, encoding ?? Encoding.GetEncoding("utf-8")))
                {
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
            }
        }

        public static string GetHtmls(string url, string postData, bool isPost, CookieContainer cookieContainer, string myCookie = "", string referUrl = "")
        {

            if (string.IsNullOrEmpty(postData))
            {
                return GetHtml(url, cookieContainer);
            }

            currentTry++;

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                UTF8Encoding Utfencoding = new UTF8Encoding();
                byte[] byteRequest = Utfencoding.GetBytes(postData);
                if (cookieContainer == null)
                {
                    cookieContainer = new System.Net.CookieContainer();
                }
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //httpWebRequest.CookieContainer = cookieContainer;//===
                httpWebRequest.ContentType = contentYixinType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                if (string.IsNullOrEmpty(referUrl))
                {
                    referUrl = url;
                }
                httpWebRequest.Referer = referUrl;
                httpWebRequest.Accept = "application/json";
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Method = isPost ? "PUT" : "GET";
                //httpWebRequest.Headers.Set("Accept-Encoding", "gzip,deflate,sdch");
                httpWebRequest.Headers.Set("Accept-Language", "zh-CN,zh;q=0.8");
                httpWebRequest.Headers.Set("Cookie", myCookie);//===
                httpWebRequest.ContentLength = byteRequest.Length;
                Stream stream = httpWebRequest.GetRequestStream();
                stream.Write(byteRequest, 0, byteRequest.Length);
                stream.Close();

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string html = streamReader.ReadToEnd();
                //string cookiesstr = httpWebRequest.CookieContainer.GetCookieHeader(httpWebRequest.RequestUri); //把cookies转换成字符串
                streamReader.Close();
                responseStream.Close();
                currentTry = 0;

                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                //Logs.Info("CoreHttpHelper-GetHtmls:" + e.Message);

                if (currentTry <= maxTry)
                {
                    //GetHtml(url, postData, isPost, cookieContainer, referUrl);
                }
                currentTry--;

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                } if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
                return string.Empty;
            }
        }

        public static string GetCookie(string url, string postData, bool isPost, CookieContainer cookieContainer,string proIp="")
        {
            if (string.IsNullOrEmpty(postData))
            {
                return GetHtml(url, cookieContainer);
            }

            //Thread.Sleep(NetworkDelay);//延迟访问

            currentTry++;

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                byte[] byteRequest = Encoding.Default.GetBytes(postData);
                if (cookieContainer == null)
                {
                    cookieContainer = new System.Net.CookieContainer();
                }
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                httpWebRequest.Referer = url;
                httpWebRequest.Accept = accept;
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Timeout = 3500;
                httpWebRequest.Method = isPost ? "POST" : "GET";
                if (!string.IsNullOrEmpty(proIp))
                {
                    SetProxy(httpWebRequest, proIp);
                }

                httpWebRequest.Headers.Set("Accept-Encoding", "gzip,deflate,sdch");
                httpWebRequest.Headers.Set("Accept-Language", "zh-CN,zh;q=0.8");
                //httpWebRequest.Headers.Set("Cookie", myCookie);//===
                httpWebRequest.ContentLength = byteRequest.Length;
                Stream stream = httpWebRequest.GetRequestStream();
                stream.Write(byteRequest, 0, byteRequest.Length);
                stream.Close();

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string html = streamReader.ReadToEnd();
                string cookiesstr = httpWebRequest.CookieContainer.GetCookieHeader(httpWebResponse.ResponseUri);
                streamReader.Close();
                responseStream.Close();
                currentTry = 0;

                httpWebRequest.Abort();
                httpWebResponse.Close();
                return cookiesstr + "|" + html;
            }
            catch (Exception e)
            {
                //Logs.Info("CoreHttpHelper-GetCookie:" + postData + ":" + e.Message);

                if (currentTry <= maxTry)
                {
                    //GetHtml(url, postData, isPost, cookieContainer);
                }
                currentTry--;

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                } if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
                return string.Empty;
            }
        }
        public static string GetCookie(string url, string referUrl="",string proIp="")
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();  
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                if (string.IsNullOrEmpty(referUrl))
                {
                    referUrl = url;
                }
                httpWebRequest.Referer = referUrl;
                httpWebRequest.Accept = accept;
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = 3500;
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Method = "GET";
                if (!string.IsNullOrEmpty(proIp))
                {
                    SetProxy(httpWebRequest, proIp);
                }
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
              
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string cookiesstr = httpWebRequest.CookieContainer.GetCookieHeader(httpWebResponse.ResponseUri);
                streamReader.Close();
                responseStream.Close();

                currentTry--;

                httpWebRequest.Abort();
                httpWebResponse.Close();

                return cookiesstr;
            }
            catch (Exception e)
            {
                //Logs.Info("CoreHttpGetCookieGet: " + proIp + ":" + e.Message);
                if (currentTry <= maxTry)
                {
                    //GetHtml(url, cookieContainer);
                }

                currentTry--;

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                } if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
                return string.Empty;
            }
        }

        /// <summary></summary>
        /// 获取指定页面的HTML代码
        /// 
        /// <param name="url">指定页面的路径
        /// <param name="cookieCollection">Cookie集合
        /// <returns></returns>
        public static string GetHtml(string url, CookieContainer cookieContainer, string loginUrl = "", string myCookie = "", string referUrl = "", string proIp = "")
        {
            //Thread.Sleep(NetworkDelay);

            currentTry++;
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //if (cookieContainer == null)
                //{
                //    cookieContainer = new System.Net.CookieContainer();
                //}
                //httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                if (string.IsNullOrEmpty(referUrl))
                {
                    referUrl = url;
                }
                httpWebRequest.Referer = referUrl;
                httpWebRequest.Accept = accept;
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = 3500;
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Cookie", myCookie);

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //if (httpWebResponse.ResponseUri.ToString() != url && !string.IsNullOrEmpty(loginUrl))
                //{
                //    GetHtml(loginUrl, "", true);
                //}                
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string html = streamReader.ReadToEnd();
                //string cookiesstr = httpWebRequest.CookieContainer.GetCookieHeader(httpWebResponse.ResponseUri);
                streamReader.Close();
                responseStream.Close();

                currentTry--;

                httpWebRequest.Abort();
                httpWebResponse.Close();

                return html;
            }
            catch (Exception e)
            {
                //Logs.Info("CoreHttpHelper-Get:" + url + ":" + e.Message);

                if (currentTry <= maxTry)
                {
                    //GetHtml(url, cookieContainer);
                }

                currentTry--;

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                } if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
                return string.Empty;
            }
        }

        /// <summary></summary>
        /// 获取指定页面的HTML代码
        /// 
        /// <param name="url">指定页面的路径
        /// <returns></returns>
        public static string GetHtml(string url, string myCookie = "",string proIp="")
        {
            return GetHtml(url, cc, "", myCookie,"",proIp);
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        { //直接确认，否则打不开
            return true;
        }


        /// <summary></summary>
        /// 获取指定页面的HTML代码
        /// 
        /// <param name="url">指定页面的路径
        /// <param name="postData">回发的数据
        /// <param name="isPost">是否以post方式发送请求
        /// <returns></returns>
        public static string GetHtml(string url, string postData, bool isPost, string myCookie = "", string referUrl = "", string proIp = "")
        {
            return GetHtml(url, postData, isPost, cc, myCookie, referUrl,proIp);
        }
        public static string GetHtmls(string url, string postData, bool isPost, string myCookie = "", string referUrl = "")
        {
            return GetHtmls(url, postData, isPost, cc, myCookie, referUrl);
        }

        /// <summary></summary>
        /// 获取指定页面的Stream
        /// <param name="url">指定页面的路径
        /// <param name="postData">回发的数据
        /// <param name="isPost">是否以post方式发送请求
        /// <param name="cookieCollection">Cookie集合
        /// <returns></returns>
        public static Stream GetStream(string url, string myCookie = "")
        {
            //Thread.Sleep(delay);

            currentTry++;
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;

            try
            {

                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //httpWebRequest.CookieContainer = cookieContainer;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                httpWebRequest.Referer = url;
                httpWebRequest.Accept = accept;
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Cookie", myCookie);

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                currentTry--;

                //httpWebRequest.Abort();
                //httpWebResponse.Close();

                return responseStream;
            }
            catch (Exception e)
            {
                Log4NetImpl.Write("MpQrCodeEditProvide失败:"+e.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ") + e.Message);
                Console.ForegroundColor = ConsoleColor.White;

                if (currentTry <= maxTry)
                {
                    //GetHtml(url, cookieContainer);
                }

                currentTry--;

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                } if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
                return null;
            }
        }
        /// <summary>
        /// 获取CookieContainer
        /// </summary>
        /// <param name="siteUserCookie"></param>
        /// <returns></returns>
        public static CookieContainer SetCookieContainer(string siteUserCookie, string domain)
        {
            CookieContainer myCookieContainer = new CookieContainer();
            string[] cookstr = siteUserCookie.Split(';');
            foreach (string str in cookstr)
            {
                string[] cookieNameValue = str.Split('=');
                Cookie ck = new Cookie(cookieNameValue[0].Trim().ToString(), cookieNameValue[1].Trim().ToString());
                ck.Domain = domain;
                myCookieContainer.Add(ck);
            }
            return myCookieContainer;
        }
        public static CookieContainer SetCookieContainer(List<Cookie> cookies)
        {
            CookieContainer myCookieContainer = new CookieContainer();
            foreach (Cookie item in cookies)
            {
                myCookieContainer.Add(item);
            }

            return myCookieContainer;
        }

        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies)
                    {
                        c.Expired = false;
                        c.Expires = DateTime.Now.AddDays(100);
                        lstCookies.Add(c);
                    }
            }

            return lstCookies;
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="item">参数对象</param>
        private static void SetProxy(HttpWebRequest request, string ProxyIp)
        {
            if (!string.IsNullOrWhiteSpace(ProxyIp))
            {
                //设置代理服务器
                if (ProxyIp.Contains(":"))
                {
                    string[] plist = ProxyIp.Split(':');
                    WebProxy myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
                    //建议连接
                    myProxy.Credentials = new NetworkCredential("", "");
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
                else
                {
                    WebProxy myProxy = new WebProxy(ProxyIp, false);
                    //建议连接
                    myProxy.Credentials = new NetworkCredential("", "");
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
                request.Credentials = CredentialCache.DefaultCredentials;
            }
        }

        #endregion

    }
}




