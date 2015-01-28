using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Ocean.Core.Utility
{
    public class HttpRequests : IDisposable
    {
        public Hashtable CookieList = new Hashtable();
        public HttpStatusCode StatusCode = new HttpStatusCode();
        public string Referer = "";
        public string ContentType = "";
        public string Location = "";
        public bool AllowAutoRedirect = false;
        private HttpWebRequest request = null;
        private bool stop = false;

        /// <summary>
        /// 用于提交PostData的编码
        /// </summary>
        private Encoding PostEncoding = Encoding.GetEncoding("gb2312");

        #region 事件
        public delegate void OnDownProgressEventHandle(long FileLength, long DownedLength);
        public event OnDownProgressEventHandle OnDonwProgress;

        #endregion

        /// <summary>
        /// 超时
        /// </summary>
        public int Timeout = 15;

        public HttpRequests()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
        }

        public HttpRequests(Encoding encoding)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            PostEncoding = encoding;
        }

        public void Dispose()
        {
            CookieList.Clear();
        }

        #region 取得网页源代码
        /// <summary>
        /// 取得网页源代码
        /// </summary>
        /// <param name="strUrl">要取的URL</param>
        /// <returns>网页源代码</returns>
        public string DownloadHtml(string strUrl, string strPostData)
        {
            stop = false;
            WebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            string buffer = "";
            try
            {
                // Prepare web request 
                request = (HttpWebRequest)WebRequest.Create(strUrl);
                //request.CookieContainer = Cookie;
                request.Referer = Referer;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/msword, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/x-shockwave-flash, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, */*";
                request.Timeout = this.Timeout * 1000;
                request.Headers.Set("Pragma", "no-cache");
                request.Headers.Set("Accept-Encoding", "gzip, deflate");
                request.Headers.Set("Accept-Language", "zh-cn");
                request.AllowAutoRedirect = this.AllowAutoRedirect;
                request.KeepAlive = true;
                request.ServicePoint.Expect100Continue = false;
                string strCookie = GetCookieString();
                if (strCookie != "")
                    request.Headers["Cookie"] = strCookie;

                if (!String.IsNullOrEmpty(strPostData))
                {
                    //byte[] data = (new ASCIIEncoding()).GetBytes(strPostData);
                    byte[] data;

                    request.Method = "POST";
                    int nIndex = strPostData.IndexOf("-----------------------");
                    if (nIndex == 0)
                    {
                        nIndex = strPostData.IndexOf('\r');
                        request.ContentType = "multipart/form-data; boundary=" + strPostData.Substring(2, nIndex);
                        data = PostEncoding.GetBytes(strPostData);
                    }
                    else if (strPostData.Contains("<?xml "))
                    {
                        request.ContentType = "text/xml;charset=GBK";
                        data = Encoding.ASCII.GetBytes(strPostData);
                    }
                    else
                    {
                        request.ContentType = "application/x-www-form-urlencoded";
                        data = Encoding.ASCII.GetBytes(strPostData);
                    }
                    request.Headers["Accept-Language"] = "zh-cn";
                    request.ContentLength = data.Length;

                    Stream requestStream = request.GetRequestStream();

                    // Send the data.
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                    requestStream = null;
                }
                else
                {
                    request.Method = "GET";
                }

                // Get response 
                response = request.GetResponse();
                if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].Contains("gzip"))
                {
                    stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    stream = response.GetResponseStream();
                }
                StatusCode = ((HttpWebResponse)response).StatusCode;
                ExtractCookies(response.Headers["Set-Cookie"]);

                if (StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (StatusCode == HttpStatusCode.Found || StatusCode == HttpStatusCode.MovedPermanently)
                {
                    Location = response.Headers["Location"];
                    if (Location.IndexOf("http://") != 0)
                    {
                        Uri uri = new Uri(new Uri(strUrl), Location);
                        Location = uri.AbsoluteUri;
                    }
                }

                string contentType = response.ContentType.ToLower();
                if (!String.IsNullOrEmpty(contentType) && !contentType.StartsWith("text/"))
                {
                    //这里可能需要异常处理
                    //return null;
                }

                Encoding encoding = GetEncodingFromHeader(contentType);

                reader = new StreamReader(stream, encoding);
                buffer = reader.ReadToEnd();

                Encoding encodingBody = GetEncodingFromBody(buffer);
                if (reader.CurrentEncoding != encodingBody && reader.CurrentEncoding == Encoding.GetEncoding("ISO-8859-1"))
                {
                    //转换为目标编码格式 
                    byte[] bytes = new byte[] { };
                    bytes = encoding.GetBytes(buffer);
                    buffer = encodingBody.GetString(bytes);
                }
                return buffer;
            }
            catch (WebException ex)
            {
                string strEx = ex.Message;
                if (ex.Response != null)
                    StatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                if (strEx.Contains("超时"))
                {
                    StatusCode = HttpStatusCode.GatewayTimeout;
                }
                return buffer;
            }
            catch (Exception ex)
            {
                //LogFileImpl.Write(ex.Message);
                return buffer;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (response != null) response.Close();
                request = null;
            }
        }

        /// <summary>
        /// 获取Cookie字符串
        /// </summary>
        /// <returns></returns>
        public string GetCookieString()
        {
            string strCookies = "";
            foreach (DictionaryEntry entry in CookieList)
            {
                strCookies = strCookies + ";" + entry.Key.ToString() + "=" + entry.Value.ToString();
            }
            if (strCookies != "")
                return strCookies.Substring(1);
            else
                return "";
        }

        /// <summary>
        /// 解析Cookie
        /// </summary>
        /// <param name="HeaderCookie"></param>
        /// <returns></returns>
        public void ExtractCookies(string HeaderCookie)
        {
            if (String.IsNullOrEmpty(HeaderCookie)) return;

            string[] CookieArray = HeaderCookie.Split(',', ';');

            foreach (string s in CookieArray)
            {
                if (String.IsNullOrEmpty(s) || !s.Contains("=")) continue;
                if (s.Trim().IndexOf("domain=") == 0 || s.Trim().IndexOf("expires=") == 0 || s.Trim().IndexOf("path=") == 0) continue;
                int nIndex = s.IndexOf("=");
                string key = s.Substring(0, nIndex);
                string value = s.Substring(nIndex + 1, s.Length - nIndex - 1);
                if (String.IsNullOrEmpty(value) && CookieList[key] != null) continue;
                CookieList[key] = value;
            }
        }
        #endregion

        #region 获取文件大小
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="strUrl">要取的URL</param>
        /// <returns>文件大小</returns>
        public long GetFileSize(string strUrl)
        {
            stop = false;
            WebResponse response = null;
            try
            {
                // Prepare web request 
                request = (HttpWebRequest)WebRequest.Create(strUrl);
                //request.CookieContainer = Cookie;
                request.Referer = Referer;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                request.Accept = "*/*";
                request.Timeout = this.Timeout * 1000;
                request.Headers.Set("Pragma", "no-cache");
                request.AllowAutoRedirect = this.AllowAutoRedirect;
                request.KeepAlive = true;
                string strCookie = GetCookieString();
                if (strCookie != "")
                    request.Headers["Cookie"] = strCookie;

                request.Method = "HEAD";

                // Get response
                response = request.GetResponse();
                StatusCode = ((HttpWebResponse)response).StatusCode;

                if (StatusCode == HttpStatusCode.OK)
                {
                    return response.ContentLength;
                }
                else
                {
                    return 0;
                }
            }
            catch (WebException ex)
            {
                string strEx = ex.Message;
                if (ex.Response != null)
                    StatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                if (strEx.Contains("超时"))
                {
                    StatusCode = HttpStatusCode.GatewayTimeout;
                }
                return 0;
            }
            catch (Exception ex)
            {
                string strEx = ex.Message;
                return 0;
            }
            finally
            {
                if (response != null) response.Close();
                request = null;
            }
        }
        #endregion

        #region 下载文件
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="strUrl">文件URL</param>
        /// <param name="strFile">本地文件名</param>
        /// <param name="Override">是否覆盖</param>
        public void Download(string strUrl, string strFile, bool Override)
        {
            stop = false;
            FileStream fileStream;
            if (Override)
                fileStream = new FileStream(strFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            else
                fileStream = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            try
            {
                request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.AllowAutoRedirect = true;
                //request.CookieContainer = Cookie;
                request.Timeout = this.Timeout * 1000;
                string strCookie = GetCookieString();
                if (strCookie != "")
                    request.Headers["Cookie"] = strCookie;

                //断点续传
                if (!Override && fileStream.Length > 0)
                {
                    request.AddRange(Convert.ToInt32(fileStream.Length));
                    fileStream.Seek(fileStream.Length, SeekOrigin.Current);
                }

                long nCount = 0;
                byte[] buffer = new byte[4096];	//4KB
                int nRecv = 0;	//接收到的字节数

                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (Exception ex)
                {
                    string strEx = ex.Message;
                    return;
                }
                ExtractCookies(response.Headers["Set-Cookie"]);
                ContentType = response.ContentType;
                Stream recvStream = response.GetResponseStream();
                long nMaxLength = (int)response.ContentLength;
                while (true)
                {
                    if (stop) break;
                    nRecv = recvStream.Read(buffer, 0, buffer.Length);
                    if (nRecv == 0) break;

                    fileStream.Write(buffer, 0, nRecv);
                    nCount += nRecv;
                    //引发下载块完成事件 
                    if (OnDonwProgress != null)
                        OnDonwProgress(nMaxLength, nCount);
                }
                recvStream.Close();
                recvStream.Dispose();
                recvStream = null;
            }
            catch
            {
            }
            finally
            {
                fileStream.Close();
                fileStream.Dispose();
                request = null;
            }
        }
        #endregion

        #region 获取HTML文件编码
        /// <summary>
        /// 获取HTML文件编码
        /// </summary>
        /// <param name="inputString">HTML文件</param>
        /// <returns></returns>
        private static Encoding GetEncodingFromBody(string inputString)
        {
            Regex r = new Regex("charset\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
            Match m = r.Match(inputString);
            string strEncoding = m.Groups[1].Value.Replace("\"", "").Replace(">", "").Replace("'", "").ToUpper();

            Encoding encoding;
            if (strEncoding == "UTF-8")
                encoding = Encoding.UTF8;
            else if (strEncoding == "UTF-7")
                encoding = Encoding.UTF7;
            else if (strEncoding == "UNICODE")
                encoding = Encoding.Unicode;
            else if (strEncoding == "GB2312")
                encoding = Encoding.GetEncoding("GB2312");
            else if (strEncoding == "BIG5")
                encoding = Encoding.GetEncoding("BIG5");
            else
                encoding = Encoding.GetEncoding("GB2312");
            return encoding;
        }

        /// <summary>
        /// 获取HTML文件编码
        /// </summary>
        /// <param name="inputString">HTML文件</param>
        /// <returns></returns>
        private static Encoding GetEncodingFromHeader(string contentType)
        {
            Encoding encoding;
            if (contentType.Contains("utf-8"))
                encoding = Encoding.UTF8;
            else if (contentType.Contains("utf-7"))
                encoding = Encoding.UTF7;
            else if (contentType.Contains("gb2312"))
                encoding = Encoding.GetEncoding("GB2312");
            else if (contentType.Contains("big5"))
                encoding = Encoding.GetEncoding("BIG5");
            else if (contentType.Contains("gbk"))
                encoding = Encoding.GetEncoding("gbk");
            else
                encoding = Encoding.GetEncoding("ISO-8859-1");
            return encoding;
        }
        #endregion

        #region 停止下载
        /// <summary>
        /// 停止下载
        /// </summary>
        public void Stop()
        {
            stop = true;
            if (request != null)
            {
                try
                {
                    request.Abort();
                }
                catch
                {
                }
            }
        }
        #endregion

        #region 取得网页源代码(用于采集文件)
        /// <summary>
        /// 取得网页源代码(用于采集文件)
        /// </summary>
        /// <param name="strUrl">要取的URL</param>
        /// <returns>网页源代码</returns>
        public string GetWebCode(string strUrl)
        {
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Timeout = 30 * 1000;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";

                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                //if (!response.ContentType.ToLower().StartsWith("text/"))
                //{
                //    //这里可能需要异常处理
                //    return null;
                //}

                stream = response.GetResponseStream();
                Encoding encoding = GetEncodingFromHeader(response.ContentType.ToLower());
                reader = new StreamReader(stream, encoding);
                string buffer = "";
                buffer = reader.ReadToEnd();
                Encoding encodingBody = GetEncodingFromBody(buffer);
                if (encodingBody != reader.CurrentEncoding && encoding == Encoding.GetEncoding("ISO-8859-1"))
                {
                    byte[] bytes = new byte[] { };
                    bytes = encoding.GetBytes(buffer);
                    buffer = encodingBody.GetString(bytes);
                }

                reader.Close();
                stream.Close();
                response.Close();
                return buffer;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (stream != null)
                    stream.Close();

                if (response != null)
                    response.Close();
            }
        }

        public static string GetEncoding(string inputString)
        {
            Regex r = new Regex("charset\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
            Match m = r.Match(inputString);
            return m.Groups[1].Value.Replace("\"", "").Replace(">", "").ToUpper();
        }
        #endregion

        #region 解析Post参数
        /// <summary>
        /// 解析Post参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string Analysis(string[] parameters, string[] values)
        {
            StringBuilder postData = new StringBuilder("");

            if (parameters != null && values != null && parameters.Length == values.Length)
            {
                //  ok, everything is ok, so craete the post data

                for (int i = 0; i < parameters.Length; i++)
                {
                    //  append for values
                    postData.AppendFormat("&{0}={1}", parameters[i], HttpUtility.UrlEncode(values[i]));
                }
            }

            return postData.ToString();
        }
        #endregion

        //网络工具类EXT------------------------------------------------------------

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest req = GetWebRequest(url, "POST");
            req.ContentType = "application/x-www-form-urlencoded;charset=gb2312";

            byte[] postData = Encoding.GetEncoding("GB2312").GetBytes(BuildQuery(parameters));
            System.IO.Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = ASCIIEncoding.UTF8;
            if (!string.IsNullOrEmpty(rsp.CharacterSet))
            {
                encoding = Encoding.GetEncoding(rsp.CharacterSet);
            }
            else
            {
                if (req.ContentType.ToLower().Contains("gb2312"))
                {
                    encoding = ASCIIEncoding.GetEncoding("gb2312");
                }
            }
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoGet(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildQuery(parameters);
                }
                else
                {
                    url = url + "?" + BuildQuery(parameters);
                }
            }

            HttpWebRequest req = GetWebRequest(url, "GET");
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = ASCIIEncoding.UTF8;
            if (!string.IsNullOrEmpty(rsp.CharacterSet))
            {
                encoding = Encoding.GetEncoding(rsp.CharacterSet);
            }
            else
            {
                if (req.ContentType.ToLower().Contains("gb2312"))
                {
                    encoding = ASCIIEncoding.GetEncoding("gb2312");
                }
            }
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行带文件上传的HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="textParams">请求文本参数</param>
        /// <param name="fileParams">请求文件参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> textParams, IDictionary<string, FileItem> fileParams)
        {
            // 如果没有文件参数，则走普通POST请求
            if (fileParams == null || fileParams.Count == 0)
            {
                return DoPost(url, textParams);
            }

            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线

            HttpWebRequest req = GetWebRequest(url, "POST");
            req.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;

            System.IO.Stream reqStream = req.GetRequestStream();
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            // 组装文本请求参数
            string textTemplate = "Content-Disposition:form-data;name=\"{0}\"\r\nContent-Type:text/plain\r\n\r\n{1}";
            IEnumerator<KeyValuePair<string, string>> textEnum = textParams.GetEnumerator();
            while (textEnum.MoveNext())
            {
                string textEntry = string.Format(textTemplate, textEnum.Current.Key, textEnum.Current.Value);
                byte[] itemBytes = Encoding.UTF8.GetBytes(textEntry);
                reqStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                reqStream.Write(itemBytes, 0, itemBytes.Length);
            }

            // 组装文件请求参数
            string fileTemplate = "Content-Disposition:form-data;name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            IEnumerator<KeyValuePair<string, FileItem>> fileEnum = fileParams.GetEnumerator();
            while (fileEnum.MoveNext())
            {
                string key = fileEnum.Current.Key;
                FileItem fileItem = fileEnum.Current.Value;
                string fileEntry = string.Format(fileTemplate, key, fileItem.GetFileName(), fileItem.GetMimeType());
                byte[] itemBytes = Encoding.UTF8.GetBytes(fileEntry);
                reqStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                reqStream.Write(itemBytes, 0, itemBytes.Length);

                byte[] fileBytes = fileItem.GetContent();
                reqStream.Write(fileBytes, 0, fileBytes.Length);
            }

            reqStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
            return GetResponseAsString(rsp, encoding);
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { 
            //直接确认，否则打不开
            return true;
        }

        public HttpWebRequest GetWebRequest(string url, string method)
        {
            HttpWebRequest req = null;
            if (url.Contains("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(url);
            }

            req.ServicePoint.Expect100Continue = false;
            req.Method = method;
            req.KeepAlive = true;
            req.UserAgent = "OceanSms.NET";
            req.Timeout = this.Timeout;

            return req;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        /// <summary>
        /// 组装GET请求URL。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>带参数的GET请求URL</returns>
        public string BuildGetUrl(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildQuery(parameters);
                }
                else
                {
                    url = url + "?" + BuildQuery(parameters);
                }
            }
            return url;
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildQuery(IDictionary<string, string> parameters)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }
    }
}