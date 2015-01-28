using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Globalization;

namespace Ocean.Core.Utility
{
    public class ExtractUrls
    {
        static Regex urlPattern = new Regex(@"(?:(?:https?|ftp)://|www\.)[^\s]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region 分析内容里的Url链接
        /// <summary>
        /// 分析内容里的Url链接
        /// </summary>
        public static string TransformAndExtractUrls(string message, out HashSet<string> extractedUrls)
        {
            var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            message = urlPattern.Replace(message, m =>
            {
                string url = HttpUtility.HtmlDecode(m.Value);
                if (!url.Contains("://"))
                {
                    url = "http://" + url;
                }

                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    return m.Value;
                }

                urls.Add(url);

                return String.Format(CultureInfo.InvariantCulture,
                                     "<a rel='nofollow external' target='_blank' href='{0}' title='{1}'>{1}</a>",
                                     HttpUtility.UrlEncode(url),
                                     m.Value);
            });

            extractedUrls = urls;
            return message;
        }
        #endregion
    }
}
