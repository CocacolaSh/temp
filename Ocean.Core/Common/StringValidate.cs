using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ocean.Core.Common
{
    public static class StringValidate
    {
        private static readonly Regex RegNumber = new Regex("^[0-9]+$");
        private static readonly Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");
        private static readonly Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$");
        private static readonly Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$");
        private static readonly Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(cn|com|net|org|edu|mil|tv|biz|info)+$");
        private static readonly Regex RegChzn = new Regex("[\u4e00-\u9fa5]");

        #region 数字字符串检测
        /// <summary>
        /// 是否数字字符串
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumber(string input)
        {
            Match m = RegNumber.Match(input);
            return m.Success;
        }

        /// <summary>
        /// 是否数字字符串 可带正负号
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumberSign(string input)
        {
            Match m = RegNumberSign.Match(input);
            return m.Success;
        }

        /// <summary>
        /// 是否浮点数
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(string input)
        {
            Match m = RegDecimal.Match(input);
            return m.Success;
        }

        /// <summary>
        /// 是否浮点数 可带正负号
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimalSign(string input)
        {
            Match m = RegDecimalSign.Match(input);
            return m.Success;
        }
        #endregion

        #region 中文检测
        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsHasChzn(string input)
        {
            Match m = RegChzn.Match(input);
            return m.Success;
        }
        #endregion

        #region 邮件地址检测
        /// <summary>
        /// 邮件地址检测
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            Match m = RegEmail.Match(input);
            return m.Success;
        }
        #endregion

        #region 手机号码检测
        /// <summary>
        /// 手机号码检测
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobile(string input)
        {
            Regex regMobile = new Regex("^(13|15|18)\\d{9}$");
            Match m = regMobile.Match(input);
            return m.Success;
        }
        #endregion

        #region 电话号码检测
        /// <summary>
        /// 电话号码检测
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsTelephone(string input)
        {
            Regex regTelephone = new Regex("^(\\(\\d{3,4}\\)|\\d{3,4}-|\\s)?\\d{7,8}");
            Match m = regTelephone.Match(input);
            return m.Success;
        }
        #endregion

        #region 由字母、数字组成
        /// <summary>
        /// 由字母、数字组成
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumCha(string input)
        {
            Regex regNumCHA = new Regex("^[a-zA-Z0-9]*$");
            Match m = regNumCHA.Match(input);
            return m.Success;
        }
        #endregion

        #region 由6-16字母、数字组成
        /// <summary>
        /// 由6-16字母、数字组成
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumCHA16(string input)
        {
            Regex regNumCHA = new Regex("^[a-zA-Z0-9]{6,16}$");
            Match m = regNumCHA.Match(input);
            return m.Success;
        }
        #endregion

        #region 由6-20字母、数字组成
        /// <summary>
        /// 由6-20字母、数字组成
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumCHA20(string input)
        {
            Regex regNumCHA = new Regex("^[a-zA-Z0-9]{6,20}$");
            Match m = regNumCHA.Match(input);
            return m.Success;
        }
        #endregion

        #region 由字母组成
        /// <summary>
        /// 由字母组成
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsCHA(string input)
        {
            Regex regCHA = new Regex("^[a-zA-Z]+$");
            Match m = regCHA.Match(input);
            return m.Success;
        }
        #endregion

        #region 身份证检测
        /// <summary>
        /// 身份证检测
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIdentifyCard(string input)
        {
            Regex idc = new Regex("d{15}|d{18}");
            Match m = idc.Match(input);
            return m.Success;
        }
        #endregion

        #region 是否时间日期
        /// <summary>
        /// 是否时间日期
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDateTime(string input)
        {
            DateTime dt;
            return DateTime.TryParse(input, out dt);
        }
        #endregion

        #region 是否网址
        /// <summary>
        /// 是否网址
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static bool IsUrl(string input)
        {
            Regex reg = new Regex(@"^http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            Match m = reg.Match(input);
            return m.Success;
        }
        #endregion

        #region 是否Guid格式
        /// <summary>
        /// 是否Guid格式
        /// </summary>
        public static bool IsGuid(string input)
        {
            bool isGuid = false;
            const string regexPatten = @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\"
                                       + @"-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";

            if (input != null && !input.Equals(""))
            {
                isGuid = Regex.IsMatch(input, regexPatten);
            }

            return isGuid;
        }
        #endregion

        #region 判断是否为ip
        /// <summary>
        /// 判断是否为ip
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIp(string input)
        {
            return Regex.IsMatch(input, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion
    }
}