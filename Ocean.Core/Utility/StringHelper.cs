using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ocean.Core.Utility
{
    public static class StringHelper
    {
        #region 截取字符串（中英文字符串都适合）
        /// <summary>
        /// 截取字符串（中英文字符串都适合）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public static string GetStr(string input, int byteCount)
        {
            if (String.IsNullOrEmpty(input))
            {
                return "";
            }

            Regex regex = new Regex("[\u4e00-\u9fa5]+", RegexOptions.Compiled);
            char[] stringChar = input.ToCharArray();
            StringBuilder sb = new StringBuilder();
            int length = 0;

            for (int i = 0; i < stringChar.Length; i++)
            {
                if (regex.IsMatch((stringChar[i]).ToString()))
                {
                    sb.Append(stringChar[i]);
                    length += 2;
                }
                else
                {
                    sb.Append(stringChar[i]);
                    length = length + 1;
                }

                if (length > byteCount)
                    break;
            }

            return sb.ToString();
        }
        #endregion

        #region 返回字符串真实长度单位字节
        /// <summary>
        /// 返回字符串真实长度单位字节
        /// </summary>
        /// <returns></returns>
        public static int GetStringLength(string input)
        {
            return Encoding.Default.GetBytes(input).Length;
        }
        #endregion

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

        #region 过滤特殊字符
        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FilterStr(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string filter = " |.|%|&|:|、|@|*|#|!|$|^|?|(|)|{|}|\\|[|]";
                string[] strFilterinGroup = filter.Split(new char[] { '|' }, 20);//20个可能影响到正则表达式的特殊字符,有待补充

                for (int i = 0; i < strFilterinGroup.Length; i++)
                {
                    input = input.Replace(strFilterinGroup[i], "-");
                }
            }

            return input;
        }
        #endregion

        #region 随机生成字符串
        /// <summary>
        /// 随机生成字符串
        /// </summary>
        public static string GetRandomString(int nLong)
        {
            //生成随机验证码的字符的源数据
            const string letters = "abcdefghjkmnpqrstuvwxyz23456789";
            string allCode = "";

            Random random = new Random();

            for (int i = 0; i < nLong; i++)
            {
                string theChar = letters.Substring(random.Next(0, letters.Length - 1), 1);
                allCode += theChar;
            }

            return allCode;
        }
        #endregion

        #region 随机生成字符串
        /// <summary>
        /// 随机生成字符串
        /// </summary>
        public static string GetRandomInt(int nLong)
        {
            //生成随机验证码的字符的源数据
            const string letters = "123456789";
            string allCode = "";

            Random random = new Random();

            for (int i = 0; i < nLong; i++)
            {
                string theChar = letters.Substring(random.Next(0, letters.Length - 1), 1);
                allCode += theChar;
            }

            return allCode;
        }
        #endregion

        #region 获取配置信息值
        /// <summary>
        /// 获取配置信息值
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValue(string content, string key)
        {
            if (String.IsNullOrEmpty(content))
            {
                return "";
            }

            string[] arrConfig = content.Split('&');

            foreach (string config in arrConfig)
            {
                if (config.IndexOf(key + "=", System.StringComparison.Ordinal) == -1) continue;

                return config.Replace(key + "=", "");
            }

            return "";
        }
        #endregion

        #region 格式化
        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string Format(string idList)
        {
            string tempIdList = string.Empty;
            string tempSplit = string.Empty;
            string[] splits = idList.Split(',');

            foreach (string split in splits)
            {

                if (split.Length >= 32)
                {
                    tempSplit = "'" + split + "'";
                }

                tempIdList += tempSplit + ",";
            }

            if (tempIdList.Length > 0)
            {
                tempIdList = tempIdList.Substring(0, tempIdList.Length - 1);
            }

            return tempIdList;
        }
        #endregion

        #region 移除Html标记
        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return string.Empty;
            const string regexstr = @"<[^>]*>";
            string s = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            return s.Replace("&nbsp;", string.Empty);
        }

        #endregion

        #region 过滤HTML中的不安全标签
        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }
        #endregion

        #region 删除最后一个字符
        /// <summary>
        /// 删除最后一个字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ClearLastChar(string input)
        {
            if (input == "")
            {
                return "";
            }

            return input.Substring(0, input.Length - 1);
        }

        #endregion

        #region 分割字符串
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string content, string split)
        {
            if (content.IndexOf(split, System.StringComparison.Ordinal) < 0)
            {
                string[] arrContent = { content };
                return arrContent;
            }

            return Regex.Split(content, Regex.Escape(split), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string content, string split, int length)
        {
            string[] result = new string[length];

            string[] splited = SplitString(content, split);

            for (int i = 0; i < length; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }
        #endregion

        #region 从字符串的指定位置截取指定长度的子字符串
        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="input">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string input, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;

                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = startIndex - length;
                    }
                }

                if (startIndex > input.Length)
                {
                    return "";
                }
            }
            else
            {
                if (length < 0)
                {
                    return "";
                }
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            if (input.Length - startIndex < length)
            {
                length = input.Length - startIndex;
            }

            return input.Substring(startIndex, length);
        }
        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }
        #endregion

        #region 取指定长度的字符串
        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="srcString">要检查的字符串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">指定长度</param>
        /// <param name="tailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string srcString, int startIndex, int length, string tailString)
        {
            if (string.IsNullOrEmpty(srcString))
            {
                return string.Empty;
            }

            string myResult = srcString;

            //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
            if (Regex.IsMatch(srcString, "[\u0800-\u4e00]+") ||
                Regex.IsMatch(srcString, "[\xAC00-\xD7A3]+"))
            {
                //当截取的起始位置超出字段串长度时
                if (startIndex >= srcString.Length)
                {
                    return "";
                }
                else
                {
                    return srcString.Substring(startIndex,
                                                   ((length + startIndex) > srcString.Length) ? (srcString.Length - startIndex) : length);
                }
            }


            if (length >= 0)
            {
                byte[] arrSrcString = Encoding.Default.GetBytes(srcString);

                //当字符串长度大于起始位置
                if (arrSrcString.Length > startIndex)
                {
                    int endIndex = arrSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (arrSrcString.Length > (startIndex + length))
                    {
                        endIndex = length + startIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        length = arrSrcString.Length - startIndex;
                        tailString = "";
                    }

                    int realLength = length;
                    int[] arrResultFlag = new int[length];
                    byte[] arrResult = null;
                    int flag = 0;

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (arrSrcString[i] > 127)
                        {
                            flag++;

                            if (flag == 3)
                            {
                                flag = 1;
                            }
                        }
                        else
                        {
                            flag = 0;
                        }

                        arrResultFlag[i] = flag;
                    }

                    if ((arrSrcString[endIndex - 1] > 127) && (arrResultFlag[length - 1] == 1))
                    {
                        realLength = length + 1;
                    }

                    arrResult = new byte[realLength];
                    Array.Copy(arrSrcString, startIndex, arrResult, 0, realLength);
                    myResult = Encoding.Default.GetString(arrResult);
                    myResult = myResult + tailString;
                }
            }

            return myResult;
        }
        #endregion

        #region 根据前后标志获取字符

        /// <summary>
        /// 根据前后标志获取字符
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetContentByTag(string begin, string end, string content)
        {
            int startIndex = content.IndexOf(begin, System.StringComparison.Ordinal);
            string result = "";

            while (startIndex >= 0)
            {
                startIndex = startIndex + begin.Length;
                int nEndIndex = content.IndexOf(end, startIndex, System.StringComparison.Ordinal);
                if (nEndIndex == -1) break;
                result = content.Substring(startIndex, nEndIndex - startIndex);
                break;
            }
            return result;
        }
        #endregion

        #region unicode转汉字
        /// <summary>
        /// unicode转汉字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UnicodeToChinese(string input)
        {
            return ConvertTo(input, "unicode");
        }

        public static string ConvertTo(string input, string encode)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\' && input[i + 1] == 'u')
                {
                    string s1 = input.Substring(i + 2, 2);
                    string s2 = input.Substring(i + 4, 2);
                    int t1 = Convert.ToInt32(s1, 16);
                    int t2 = Convert.ToInt32(s2, 16);
                    byte[] array = new byte[2];
                    array[0] = (byte)t2;
                    array[1] = (byte)t1;
                    string s = System.Text.Encoding.GetEncoding(encode).GetString(array);
                    sb.Append(s);
                    i = i + 5;
                }
                else { sb.Append(input[i]); }
            }
            return sb.ToString();
        }
        #endregion

        #region 返回字符
        /// <summary>
        /// 返回字符
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetValue(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return string.Empty;
            }

            return obj.ToString();
        }

        #endregion

        #region 替换字符标签
        /// <summary>
        /// 替换字符标签
        /// </summary>
        /// <param name="template"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string ReplaceStringTag(string template, string tag, string value)
        {
            int nCount = 0;

            if (String.IsNullOrEmpty(template))
            {
                return "";
            }

            if (value == null)
            {
                value = "";
            }

            tag = "{$" + tag + "}";
            int nStartIndex = template.IndexOf(tag, System.StringComparison.Ordinal);

            while (nStartIndex >= 0)
            {
                nCount++;
                if (nCount > 100) break;
                template = template.Remove(nStartIndex, tag.Length);
                template = template.Insert(nStartIndex, value);
                if (template == "" || nStartIndex >= template.Length - 1) break;
                nStartIndex = template.IndexOf(tag, nStartIndex + 1, System.StringComparison.Ordinal);
            }
            return template;
        }

        /// <summary>
        /// 正则替换
        /// </summary>
        /// <param name="regStr">正则表达式</param>
        /// <param name="oldStr">原字符串</param>
        /// <param name="newStr">新字符串</param>
        /// <returns></returns>
        public static string RegReplace(string regStr, string oldStr, string newStr)
        {
            Regex reg = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (string.IsNullOrEmpty(oldStr))
            {
                return "";
            }
            return reg.Replace(oldStr, newStr);
        }
        #endregion

        #region 获取内容
        /// <summary>
        /// 获取内容指定
        /// <param name="content">原字符串</param>
        /// <param name="s">开始字符串</param>
        /// <param name="e">结束字符串</param>
        /// </summary>
        public static string GetContent(string content, string s, string e)
        {
            int nStartIndex = content.IndexOf(s) + s.Length;
            int nEndIndex = content.IndexOf(e, nStartIndex) - e.Length;
            content = content.Substring(nStartIndex, nEndIndex - nStartIndex + 1);
            return content;
        }
        /// <summary>
        /// 正则获取匹配项
        /// </summary>
        /// <param name="regStr"></param>
        /// <param name="str"></param>
        /// <param name="groupIndex"></param>
        /// <returns></returns>
        public static string GetRegResult(string regStr, string str, int groupIndex)
        {
            Regex reg = new Regex(regStr, RegexOptions.IgnoreCase);
            Match match = reg.Match(str);
            if (match.Success)
            {
                return match.Groups[groupIndex].Value;
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 正则获取多匹配项
        /// </summary>
        /// <param name="regStr"></param>
        /// <param name="str"></param>
        /// <param name="groupIndex"></param>
        /// <returns></returns>
        public static string[] GetRegResults(string regStr, string str, int groupIndex)
        {
            Regex reg = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            MatchCollection matchs = reg.Matches(str);
            if (matchs.Count > 0)
            {
                string[] results = new string[matchs.Count];
                int _index = 0;
                foreach (Match mat in matchs)
                {
                    results[_index] = mat.Groups[groupIndex].Value;
                    _index++;
                }
                return results;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 获取,数量
        /// <summary>
        /// 获取,数量
        /// </summary>
        public static int GetDotCount(string value)
        {
            int count = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                return count;
            }

            foreach (Char c in value)
            {
                if (c == ',') count++;
            }

            return count;
        }
        #endregion

        #region 字符串判断
        /// <summary>
        /// 字段串是否为Null或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StrIsNullOrEmpty(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return true;

            return false;
        }
        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            return Validator.IsNumericArray(strNumber);
        }
        /// <summary>
        /// 判断给定的字符串数组(strInt)中的数据是不是都为整数型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsInts(string[] strInt)
        {
            return Validator.IsInts(strInt);
        }
        #endregion

        #region 是否为数值串列表，各数值间用","间隔
        /// <summary>
        /// 是否为数值串列表，各数值间用","间隔
        /// </summary>
        /// <param name="numList"></param>
        /// <returns></returns>
        public static bool IsNumericList(string numList)
        {
            if (StrIsNullOrEmpty(numList))
                return false;

            return IsNumericArray(numList.Split(','));
        }
        #endregion

        #region InArray
        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                        return i;
                }
                else if (strSearch == stringArray[i])
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }
        #endregion

        #region sql

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }

        /// <summary>
        /// SQL SERVER SQL语句转义
        /// </summary>
        /// <param name="str">需要转义的关键字符串</param>
        /// <param name="pattern">需要转义的字符数组</param>
        /// <returns>转义后的字符串</returns>
        public static string SqlEscape(string str, bool isSearch)
        {
            Regex reg = new Regex(@"declare|insert|into|select|delete|alter|drop|truncate|@|\*|update|exec|--", RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            StringBuilder strBuilder = new StringBuilder(reg.Replace(str, ""));
            if (isSearch)
            {
                string[] pattern = { @"%", @"_", @"'", "?" };
                foreach (string s in pattern)
                {
                    if (s == "'")
                    {
                        strBuilder.Replace(s, "['']");
                    }
                    else
                    {
                        strBuilder.Replace(s, "[" + s + "]");
                    }
                }
            }
            //strBuilder.Replace("{", "'+cast(0x7B AS varchar(1))+'").Replace("}", "'+cast(0x7D AS varchar(1))+'");
            return strBuilder.ToString();
        }
        /// <summary>
        /// SQL SERVER SQL语句转义
        /// </summary>
        /// <param name="str">需要转义的关键字符串</param>
        /// <param name="pattern">需要转义的字符数组</param>
        /// <returns>转义后的字符串</returns>
        public static string SqlEscape(string str)
        {
            return SqlEscape(str, false);
        }
        #endregion

        #region 获取调额方案
        /// <summary>
        /// 获取调额方案列表
        /// </summary>
        /// <param name="limitProgramme">调额方案</param>
        /// <returns></returns>
        public static IList<string> GetLimitProgrammes(string limitProgramme)
        {
            if (string.IsNullOrEmpty(limitProgramme))
                return null;
            Regex limitRegex=new Regex(@"(\d+)万.*?([\d\.]+)‰");
            MatchCollection limits = limitRegex.Matches(limitProgramme);
            IList<string> limitList = new List<string>();
            if (limits.Count > 0)
            {
                foreach (Match limit in limits)
                {
                    if (!string.IsNullOrEmpty(limit.Groups[1].Value) && Validator.IsDecimal(limit.Groups[1].Value) && !string.IsNullOrEmpty(limit.Groups[2].Value) && Validator.IsDecimal(limit.Groups[2].Value))
                    {
                        limitList.Add(limit.Groups[1].Value + "," + limit.Groups[2].Value);
                    }
                }
                return limitList;
            }
            else
            {
                return null;
            }
        }
        public static string GetLimitProgrammeStr(string limitProgramme)
        {
            if (string.IsNullOrEmpty(limitProgramme))
                return null;
            Regex limitRegex = new Regex(@"额度(\d+)万.*?([\d\.]+)‰");
            MatchCollection limits = limitRegex.Matches(limitProgramme);
            if (limits.Count > 0)
            {
                int i = 1;
                StringBuilder limitBuilder = new StringBuilder();
                foreach (Match limit in limits)
                {
                    limitBuilder.Append(limit.Value);
                    if (i < limits.Count)
                    {
                        limitBuilder.Append(";");
                    }
                    i++;
                }
                return limitBuilder.ToString();
            }
            return null;
        }
        public static string GetLimitProgramme(string limitProgramme)
        {
            if (string.IsNullOrEmpty(limitProgramme))
                return null;
            Regex limitRegex = new Regex(@"(\d+)万.*?([\d\.]+)‰");
            Match limit = limitRegex.Match(limitProgramme);
            if (limit.Success)
            {
                if (!string.IsNullOrEmpty(limit.Groups[1].Value) && Validator.IsDecimal(limit.Groups[1].Value) && !string.IsNullOrEmpty(limit.Groups[2].Value) && Validator.IsDecimal(limit.Groups[2].Value))
                {
                    return limit.Groups[1].Value + "," + limit.Groups[2].Value;
                }
            }
            return null;
        }
        #endregion

        #region Guid
        public static bool IsZeroGuid(Guid id)
        {
            if(id==new Guid("00000000-0000-0000-0000-000000000000"))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 获取季度中文

        /// <summary>
        /// 转数字季度为中文
        /// </summary>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public static string GetQuarter(int quarter)
        {
            switch (quarter)
            {
                case 1:
                    {
                        return "一";
                    }
                case 2:
                    {
                        return "二";
                    }
                case 3:
                    {
                        return "三";
                    }
                case 4:
                    {
                        return "四";
                    }
            }
            return "";
        }

        /// <summary>
        /// 根据某年季度得到季度月份组合
        /// </summary>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public static string GetQuarter(int year, int quarter)
        {
            switch (quarter)
            {
                case 1:
                    {
                        return year.ToString() + "-01," + year.ToString() + "-02," + year.ToString() + "-03";
                    }
                case 2:
                    {
                        return year.ToString() + "-04," + year.ToString() + "-05," + year.ToString() + "-06";
                    }
                case 3:
                    {
                        return year.ToString() + "-07," + year.ToString() + "-08," + year.ToString() + "-09";
                    }
                case 4:
                    {
                        return year.ToString() + "-10," + year.ToString() + "-11," + year.ToString() + "-12";
                    }
            }
            return "";
        }
        #endregion

        #region 从身份证中获取性别
        public static string GetSexFormIdent(string IdentNO,bool isIdent)
        {
            //获取得到输入的身份证号码
            string identityCard = IdentNO;

            if (string.IsNullOrEmpty(identityCard))
            {
                return "";
            }
            else
            {
                //身份证号码只能为15位或18位其它不合法
                if (!isIdent)
                {

                    return "";
                }
            }
            // string birthday = "";
            string sex = "";
            //处理18位的身份证号码从号码中得到生日和性别代码
            //if (identityCard.Length == 18)
            //{
            //    //birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
            //    sex = identityCard.Substring(14, 3);
            //}
            ////处理15位的身份证号码从号码中得到生日和性别代码
            //if (identityCard.Length == 15)
            //{
            //    //birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
            //    sex = identityCard.Substring(12, 3);
            //}
            sex = identityCard.Substring(4, 3);
            //性别代码为偶数是女性奇数为男性
            if (int.Parse(sex) % 2 == 0)
            {
                return "女士";
            }
            else
            {
                return "先生";
            }
        }
        #endregion
    }
}
