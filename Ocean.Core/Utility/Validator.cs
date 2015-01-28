using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace Ocean.Core.Utility
{
    public class Validator
    {
        private static Regex RegNumber = new Regex("^[0-9]+$");

        private static Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");
        private static Regex RegDecimal = new Regex("^[0-9]+\\.?[0-9]{0,3}$");
        private static Regex RegDecimal0 = new Regex(" ^(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
        private static Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //�等价于^[+-]?\d+[.]?\d+$
        private static Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(cc|com|cn|hk|net|org|edu|mil|me|tv|tw|biz|info)$");//w 英文字母或数字的字符串，?[a-zA-Z0-9] �语法一?
        private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");


        private static Regex NumRegex = new Regex("^[a-zA-Z][a-zA-Z0-9]{4,15}$"); //�字母开头，只能包含数字和字母，长度4-15
        private static Regex NameRegex = new Regex("^[\u4e00-\u9fa5]+$"); //只能包含中文
        private static Regex EmailRegex = new Regex("^[\\w-]+@[\\w-]+\\.(cc|com|cn|hk|net|org|edu|me|mil|tv|tw|biz|info)$");
        private static Regex IDCardRegex = new Regex("^\\d{15}|\\d{18}$");
        private static Regex MobileRegex = new Regex("^1[3|4|5|8][0-9]\\d{8}$");
        private static Regex PhoneRegex = new Regex("^(\\d{11})|^((\\d{7,8})|(\\d{4}|\\d{3})-(\\d{7,8})|(\\d{4}|\\d{3})-(\\d{7,8})-(\\d{4}|\\d{3}|\\d{2}|\\d{1})|(\\d{7,8})-(\\d{4}|\\d{3}|\\d{2}|\\d{1}))$");
        private static Regex IpRegex = new Regex(@"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        private static Dictionary<int,string> IDentCardArea = new Dictionary<int,string>(){{11,"北京"},{12,"天津"},{13,"河北"},{14,"山西"},{15,"内蒙古"},{21,"辽宁"},{22,"吉林"},{23,"黑龙江"},{31,"上海"},{32,"江苏"},{33,"浙江"},{34,"安徽"},{35,"福建"},{36,"江西"},{37,"山东"},{41,"河南"},{42,"湖北"},{43,"湖南"},{44,"广东"},{45,"广西"},{46,"海南"},{50,"重庆"},{51,"四川"},{52,"贵州"},{53,"云南"},{54,"西藏"},{61,"陕西"},{62,"甘肃"},{63,"青海"},{64,"宁夏"},{65,"新疆"},{71,"台湾"},{81,"香港"},{82,"澳门"},{91,"国外"}};

        private static Regex LimitProgrammeRegex = new Regex(@"(\d+)万.*?([\d\.]+)‰");

        public Validator() { }

        public static bool IsNull(string inputData)
        {
            if (inputData == null || inputData == string.Empty || inputData == "")
            {
                return true;
            }
            return false;
        }
        public static bool IsNum(string inputData)
        {
            Match m = NumRegex.Match(inputData);
            return m.Success;
        }
        public static bool IsCHZN(string inputData)
        {
            Match m = NameRegex.Match(inputData);
            return m.Success;
        }
        public static bool IsEmail(string inputData)
        {
            Match m = EmailRegex.Match(inputData);
            return m.Success;
        }
        public static bool IsIDCard(string inputData)
        {
            Match m = IDCardRegex.Match(inputData);
            return m.Success;
        }
        public static bool IsMobile(string inputData)
        {
            Match m = MobileRegex.Match(inputData);
            return m.Success;
        }
        public static bool IsPhone(string inputData)
        {
            Match m = PhoneRegex.Match(inputData);
            return m.Success;
        }
        public static bool IsLength(string inputData, int minLth, int maxLth)
        {
            if ((inputData.Length >= minLth) && (inputData.Length <= maxLth))
            {
                return true;
            }
            return false;
        }
        public static bool IsInt(string inputData)
        {
            Match m = RegNumber.Match(inputData);
            return m.Success;
        }

        #region //.......
        //public static string GetSubString(string str, int num)
        //{
        //    return (str.Length > num) ? str.Substring(0, num) + "..." : str;
        //}
        //public static string InputText(string text, int maxLength)
        //{
        //    text = text.Trim();
        //    if (string.IsNullOrEmpty(text))
        //        return string.Empty;
        //    if (text.Length > maxLength)
        //    {
        //        text = text.Substring(0, maxLength);
        //        text = Regex.Replace(text, "[\\s]{2,}", " ");    //two or more spaces
        //        text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");    //<br>
        //        text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");    //&nbsp;
        //        text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);    //any other tags
        //        text = text.Replace("'", "''");
        //        return text;
        //    }
        //} 
        #endregion

        #region 数字字符串检?

        /// <summary>
        /// �检查Request查询字符串的键值，是否是数字，最大长度限?
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="inputKey">Request�的键?/param>
        /// <param name="maxLen">�最大长?/param>
        /// <returns>�返回Request查询字符?/returns>
        public static string FetchInputDigit(HttpRequest req, string inputKey, int maxLen)
        {
            string retVal = string.Empty;
            if (inputKey != null && inputKey != string.Empty)
            {
                retVal = req.QueryString[inputKey];
                if (null == retVal)
                    retVal = req.Form[inputKey];
                if (null != retVal)
                {
                    retVal = SqlText(retVal, maxLen);
                    if (!IsNumber(retVal))
                        retVal = string.Empty;
                }
            }
            if (retVal == null)
                retVal = string.Empty;
            return retVal;
        }
        /// <summary>
        /// �是否数字字符?
        /// </summary>
        /// <param name="inputData">�输入字符?/param>
        /// <returns></returns>
        public static bool IsNumber(string inputData)
        {
            Match m = RegNumber.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// �是否数字字符?�可带正负?
        /// </summary>
        /// <param name="inputData">�输入字符?/param>
        /// <returns></returns>
        public static bool IsNumberSign(string inputData)
        {
            Match m = RegNumberSign.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// �是否是浮点数
        /// </summary>
        /// <param name="inputData">输入字符?/param>
        /// <returns></returns>
        public static bool IsDecimal(string inputData)
        {
            Match m = RegDecimal.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// �是否是浮点数 可带正负?
        /// </summary>
        /// <param name="inputData">�输入字符?/param>
        /// <returns></returns>
        public static bool IsDecimalSign(string inputData)
        {
            Match m = RegDecimalSign.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 中文检查

        /// <summary>
        /// �检测是否有中文字符
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsHasCHZN(string inputData)
        {
            Match m = RegCHZN.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 邮件地址
        /// <summary>
        /// 是否是浮点数 可带正负?
        /// </summary>
        /// <param name="inputData">�输入字符?/param>
        /// <returns></returns>
        //public static bool IsEmail(string inputData)
        //{
        //    Match m = RegEmail.Match(inputData);
        //    return m.Success;
        //}

        #endregion

        #region 其他

        /// <summary>
        /// 检查字符串最大长度，返回指定长度的串
        /// </summary>
        /// <param name="sqlInput">输入字符?/param>
        /// <param name="maxLength">�最大长?/param>
        /// <returns></returns>            
        public static string SqlText(string sqlInput, int maxLength)
        {
            if (sqlInput != null && sqlInput != string.Empty)
            {
                sqlInput = sqlInput.Trim();
                if (sqlInput.Length > maxLength)//�按最大长度截取字符串
                    sqlInput = sqlInput.Substring(0, maxLength);
            }
            return sqlInput;
        }


        /// <summary>
        /// 字符串编?
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string HtmlEncode(string inputData)
        {
            return HttpUtility.HtmlEncode(inputData);
        }
        /// <summary>
        /// �设置Label显示Encode的字符串
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="txtInput"></param>
        public static void SetLabel(Label lbl, string txtInput)
        {
            lbl.Text = HtmlEncode(txtInput);
        }
        public static void SetLabel(Label lbl, object inputObj)
        {
            SetLabel(lbl, inputObj.ToString());
        }
        /// <summary>
        /// 获取安全字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetSafeStr(string str)
        {
            str = str.Replace("'", "");
            str = str.Replace(";", "");
            str = str.Replace("--", "");
            return str;
        }


        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            if (strNumber == null)
                return false;

            if (strNumber.Length < 1)
                return false;

            foreach (string id in strNumber)
            {
                if (!IsNumeric(id))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
                return IsNumeric(expression.ToString());

            return false;

        }

        /// <summary>
        /// 判断对象是否为数字类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 判断给定的字符串数组(strInt)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsInts(string[] strInt)
        {
            if (strInt == null)
                return false;

            if (strInt.Length < 1)
                return false;

            foreach (string id in strInt)
            {
                if (!IsInts(id))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 判断对象是否为整数类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsInts(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression.ToString(), @"^\d+(\.\d+)?$");

            return false;
        }
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return IpRegex.IsMatch(ip);
        }
        #endregion

        #region 调额方案
        /// <summary>
        /// 判断是否包含格式正确的调额方案
        /// </summary>
        /// <param name="limitProgramme">调额方案</param>
        /// <returns></returns>
        public static bool HasLimitProgramme(string limitProgramme)
        {
            if(string.IsNullOrEmpty(limitProgramme))
                return false;
            return LimitProgrammeRegex.Matches(limitProgramme).Count > 0;
        }
        #endregion

        #region 身份证
        public static bool IsIDentCard(string nostr)
        {
            if (string.IsNullOrEmpty(nostr))
            {
                return false;
            }
            if (!IsIDCard(nostr))
            {
                return false;
            }
            if (IDentCardArea.Keys.Where(i => i == TypeConverter.StrToInt(nostr.Substring(0, 2))).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}