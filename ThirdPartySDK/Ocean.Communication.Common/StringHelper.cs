using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ocean.Communication.Common
{
    public static class StringHelper
    {
        #region 去除HTML标记
        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="html">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            string[] aryReg ={
          @"<script[^>]*?>.*?</script>",

          @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
          @"([\r\n])[\s]+",
          @"&(quot|#34);",
          @"&(amp|#38);",
          @"&(lt|#60);",
          @"&(gt|#62);", 
          @"&(nbsp|#160);", 
          @"&(iexcl|#161);",
          @"&(cent|#162);",
          @"&(pound|#163);",
          @"&(copy|#169);",
          @"&#(\d+);",
          @"-->",
          @"<!--.*\n"
         
         };

            string[] aryRep = {
           "",
           "",
           "",
           "\"",
           "&",
           "<",
           ">",
           " ",
           "\xa1",//chr(161),
           "\xa2",//chr(162),
           "\xa3",//chr(163),
           "\xa9",//chr(169),
           "",
           "\r\n",
           ""
          };

            string output = html;

            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                output = regex.Replace(output, aryRep[i]);
            }

            return output.Replace("<", "").Replace(">", "").Replace("\r\n", "").Replace("&nbsp;", " ");
        }

        /// <summary>
        /// 转换html标记[]
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string ToHtmlCode(string inputStr)
        {
            if (!string.IsNullOrEmpty(inputStr))
            {
                return inputStr.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\"", "&quot;");
            }
            return inputStr;
        }
        /// <summary>
        /// 含用HTML标签的返原文本
        /// </summary>
        /// <param name="outputStr"></param>
        /// <returns></returns>
        public static string ToTextCode(string outputStr)
        {
            if (!string.IsNullOrEmpty(outputStr))
            {
                return outputStr.Replace("<br />", "\r\n").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
            }
            return outputStr;
        }
        #endregion
    }
}