using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ocean.Core.Utility
{
    public class TypeConverter
    {
        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            if (expression != null)
                return StrToBool(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            if (expression != null)
            {
                if (string.Compare(expression, "true", true) == 0)
                    return true;
                else if (string.Compare(expression, "false", true) == 0)
                    return false;
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object expression)
        {
            return ObjectToInt(expression, 0);
        }
        public static int ObjectToInt32(object expression)
        {
            return ObjectToInt(expression);
        }

        /// <summary>
        /// 将对象转换为Int64类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的Int64类型结果</returns>
        public static Int64 ObjectToInt64(object expression)
        {
            return ObjectToInt64(expression, 0);
        }
        /// <summary>
        /// 将对象转换为Int64类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的Int64类型结果</returns>
        public static Int64 ObjectToInt64(object expression, int defValue)
        {
            if (expression != null)
                return StrToInt64(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int64类型,转换失败返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的Int64类型结果</returns>
        public static Int64 StrToInt64(string str)
        {
            return StrToInt64(str, 0);
        }
        /// <summary>
        /// 将对象转换为Int64类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的Int64类型结果</returns>
        public static Int64 StrToInt64(string str, int defValue)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 11 || !Regex.IsMatch(str.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return Convert.ToInt64(defValue);

            Int64 rv;
            if (Int64.TryParse(str, out rv))
                return rv;

            return Convert.ToInt64(StrToFloat(str, defValue));
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object expression, int defValue)
        {
            if (expression != null)
                return StrToInt(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为Guid型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的Guid类型结果</returns>
        public static Guid StrToGuid(string strValue, Guid defValue)
        {
            if (string.IsNullOrEmpty(strValue))
                return defValue;
            Guid guidValue;
            if (Guid.TryParse(strValue, out guidValue))
            {
                return guidValue;
            }
            else
            {
                return defValue;
            }
        }
        /// <summary>
        /// 将对象转换为Int32类型,转换失败返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str)
        {
            return StrToInt(str, 0);
        }
        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str, int defValue)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 11 || !Regex.IsMatch(str.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(str.Trim(), out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(str, defValue));
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            if ((strValue == null))
                return defValue;

            return StrToFloat(strValue.ToString(), defValue);
        }

        /// <summary>
        /// string[]型转换为guid[]型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static Guid[] StrsToGuids(string[] strArr)
        {
            if (strArr != null && strArr.Length > 0)
            { 
                Guid[] gArr=new Guid[strArr.Length];
                for (int i = 0; i < strArr.Length; i++)
                {
                    Guid g = Guid.Parse(strArr[i]);
                    gArr[i] = g;
                }
                return gArr;
            }

            return null;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjectToFloat(object strValue, float defValue)
        {
            if ((strValue == null))
                return defValue;

            return StrToFloat(strValue.ToString(), defValue);
        }

        /// <summary>
        /// string型转换为Decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的Decimal类型结果</returns>
        public static decimal ObjectToDecimal(object strValue, decimal defValue)
        {
            if ((strValue == null))
                return defValue;

            return StrToDecimal(strValue.ToString(), defValue);
        }
        /// <summary>
        /// string型转换为Decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <returns>转换后的Decimal类型结果</returns>
        public static decimal ObjectToDecimal(object strValue)
        {
            return ObjectToDecimal(strValue, 0.00M);
        }
        /// <summary>
        /// string型转换为Decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的Decimal类型结果</returns>
        public static decimal StrToDecimal(string strValue)
        {
            return StrToDecimal(strValue, 0.00M);
        }
        /// <summary>
        /// string型转换为Decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static decimal StrToDecimal(string strValue, decimal defValue)
        {
            if ((strValue == null) || (strValue.Length > 18))
                return defValue;

            decimal intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    decimal.TryParse(strValue, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjectToFloat(object strValue)
        {
            return ObjectToFloat(strValue.ToString(), 0);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <returns>转换后的float类型结果</returns>
        public static float StrToFloat(object strValue)
        {
            if ((strValue == null))
                return 0;

            return StrToFloat(strValue.ToString(), 0);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的float类型结果</returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            if ((strValue == null) || (strValue.Length > 10))
                return defValue;

            float intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(strValue, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// string型转换为double型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <returns>转换后的double类型结果</returns>
        public static double StrToDouble(object strValue)
        {
            if ((strValue == null))
                return 0;

            return StrToDouble(strValue.ToString(), 0);
        }

        /// <summary>
        /// string型转换为double型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的double类型结果</returns>
        public static double StrToDouble(string strValue, double defValue)
        {
            if (strValue == null)
                return defValue;

            double intValue = defValue;

            if (strValue != null)
            {
                if (double.TryParse(strValue, out intValue))
                {
                    return intValue;
                }  
            }

            return intValue;
        }

        /// <summary>
        /// 型转换为string型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的string类型结果</returns>
        public static string ObjectToString(object strValue)
        {
            if (strValue == null)
            {
                return "";
            }
            return Convert.ToString(strValue);
        }
        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str, DateTime defValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                {
                    if (dateTime.Year < 1900 || dateTime.Year > 9999)
                    {
                        return defValue;
                    }
                    return dateTime;

                }
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str)
        {
            return StrToDateTime(str, DateTime.Parse("1900-1-1"));
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj)
        {
            return StrToDateTime(obj.ToString());
        }
        /// <summary>
        /// 字符串转化为base64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StrToBase64(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// base64转化为字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StrFromBase64(string base64Str)
        {
            if (!string.IsNullOrEmpty(base64Str))
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64Str));
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj, DateTime defValue)
        {
            return StrToDateTime(obj.ToString(), defValue);
        }
        /// <summary>
        /// 数据库中与C#中的数据类型对照
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ChangeToCSharpType(string type)
        {
            string reval = string.Empty;
            switch (type.ToLower())
            {
                case "int":
                    reval = "Int32";
                    break;
                case "text":
                    reval = "string";
                    break;
                case "bigint":
                    reval = "Int64";
                    break;
                case "binary":
                    reval = "byte[]";
                    break;
                case "bit":
                    reval = "bool";
                    break;
                case "char":
                    reval = "string";
                    break;
                case "datetime":
                    reval = "DateTime";
                    break;
                case "decimal":
                    reval = "decimal";
                    break;
                case "float":
                    reval = "double";
                    break;
                case "image":
                    reval = "byte[]";
                    break;
                case "money":
                    reval = "decimal";
                    break;
                case "nchar":
                    reval = "string";
                    break;
                case "ntext":
                    reval = "string";
                    break;
                case "numeric":
                    reval = "decimal";
                    break;
                case "nvarchar":
                    reval = "string";
                    break;
                case "real":
                    reval = "single";
                    break;
                case "smalldatetime":
                    reval = "DateTime";
                    break;
                case "smallint":
                    reval = "Int16";
                    break;
                case "smallmoney":
                    reval = "decimal";
                    break;
                case "timestamp":
                    reval = "DateTime";
                    break;
                case "tinyint":
                    reval = "byte";
                    break;
                case "uniqueidentifier":
                    reval = "Guid";
                    break;
                case "varbinary":
                    reval = "byte[]";
                    break;
                case "varchar":
                    reval = "string";
                    break;
                case "Variant":
                    reval = "Object";
                    break;
                default:
                    reval = "string";
                    break;
            }
            return reval;
        }

        /// <summary>
        /// 数据库中与C#中的数据类型对照
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ChangeToSqlDbType(string type)
        {
            string reval = string.Empty;
            switch (type.ToLower())
            {
                case "int":
                    reval = "Int";
                    break;
                case "text":
                    reval = "Text";
                    break;
                case "bigint":
                    reval = "BigInt";
                    break;
                case "binary":
                    reval = "Binary";
                    break;
                case "bit":
                    reval = "Bit";
                    break;
                case "char":
                    reval = "Char";
                    break;
                case "datetime":
                    reval = "DateTime";
                    break;
                case "decimal":
                    reval = "Decimal";
                    break;
                case "float":
                    reval = "Float";
                    break;
                case "image":
                    reval = "Image";
                    break;
                case "money":
                    reval = "Money";
                    break;
                case "nchar":
                    reval = "NChar";
                    break;
                case "ntext":
                    reval = "NText";
                    break;
                case "numeric":
                    reval = "Numeric";
                    break;
                case "nvarchar":
                    reval = "NVarChar";
                    break;
                case "real":
                    reval = "Real";
                    break;
                case "smalldatetime":
                    reval = "SmallDateTime";
                    break;
                case "smallint":
                    reval = "SmallInt";
                    break;
                case "smallmoney":
                    reval = "SmallMoney";
                    break;
                case "timestamp":
                    reval = "Timestamp";
                    break;
                case "tinyint":
                    reval = "TinyInt";
                    break;
                case "uniqueidentifier":
                    reval = "Uniqueidentifier";
                    break;
                case "varbinary":
                    reval = "VarBinary";
                    break;
                case "varchar":
                    reval = "VarChar";
                    break;
                case "Variant":
                    reval = "Variant";
                    break;
                default:
                    reval = "VarChar";
                    break;
            }
            return reval;
        }

        /// <summary>
        /// 数据库中字段与C#中的数据类型转化
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ChangeToDataType(string type)
        {
            string reval = string.Empty;
            switch (type.ToLower())
            {
                case "int":
                    reval = "TypeConverter.ObjectToInt";
                    break;
                case "text":
                    reval = "TypeConverter.ObjectToString";
                    break;
                case "bigint":
                    reval = "TypeConverter.ObjectToInt64";
                    break;
                case "binary":
                    reval = "";
                    break;
                case "bit":
                    reval = "TypeConverter.ObjectToBool";
                    break;
                case "char":
                    reval = "TypeConverter.ObjectToString";
                    break;
                case "datetime":
                    reval = "TypeConverter.ObjectToDateTime";
                    break;
                case "decimal":
                    reval = "TypeConverter.ObjectToDecimal";
                    break;
                case "float":
                    reval = "TypeConverter.ObjectToFloat";
                    break;
                case "image":
                    reval = "";
                    break;
                case "money":
                    reval = "";
                    break;
                case "nchar":
                    reval = "TypeConverter.ObjectToString";
                    break;
                case "ntext":
                    reval = "TypeConverter.ObjectToString";
                    break;
                case "numeric":
                    reval = "";
                    break;
                case "nvarchar":
                    reval = "TypeConverter.ObjectToString";
                    break;
                case "real":
                    reval = "";
                    break;
                case "smalldatetime":
                    reval = "TypeConverter.ObjectToDateTime";
                    break;
                case "smallint":
                    reval = "TypeConverter.ObjectToInt";
                    break;
                case "smallmoney":
                    reval = "";
                    break;
                case "timestamp":
                    reval = "";
                    break;
                case "tinyint":
                    reval = "";
                    break;
                case "uniqueidentifier":
                    reval = "";
                    break;
                case "varbinary":
                    reval = "";
                    break;
                case "varchar":
                    reval = "TypeConverter.ObjectToString";
                    break;
                case "Variant":
                    reval = "TypeConverter.ObjectToString";
                    break;
                default:
                    reval = "";
                    break;
            }
            return reval;
        }
    }
}
