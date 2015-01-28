using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data;

namespace Ocean.Core.T4
{
    /// <summary>
    /// T4实体模型信息类
    /// </summary>
    public class T4ModelInfo
    {
        /// <summary>
        /// 获取 是否使用模块文件夹
        /// </summary>
        public bool UseModuleDir { get; private set; }

        /// <summary>
        /// 获取 模型所在模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 获取 模型名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取 模型描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 主键类型
        /// </summary>
        public Type KeyType { get; set; }

        /// <summary>
        /// 主键类型名称
        /// </summary>
        public string KeyTypeName { get; set; }


        public IEnumerable<PropertyInfo> Properties { get; private set; }

        public T4ModelInfo(Type modelType,string moduleName, bool useModuleDir = false)
        {
            if (modelType != null)
            {
                var @namespace = modelType.Namespace;
                if (@namespace == null)
                {
                    return;
                }
                UseModuleDir = useModuleDir;
                if (UseModuleDir)
                {
                    var index = @namespace.LastIndexOf('.') + 1;
                    ModuleName = @namespace.Substring(index, @namespace.Length - index);
                }

                Name = modelType.Name;
                PropertyInfo keyProp = modelType.GetProperty("Id");
                KeyType = keyProp.PropertyType;
                KeyTypeName = KeyType.Name;

                var descAttributes = modelType.GetCustomAttributes(typeof(DescriptionAttribute), true);
                Description = descAttributes.Length == 1 ? ((DescriptionAttribute)descAttributes[0]).Description : Name;
                Properties = modelType.GetProperties();
            }
            else
            {
                ModuleName = moduleName;
                Name = "";
                Description = "";
                UseModuleDir = useModuleDir;
            }
        }
    }


//    #region DbTable
//    /// <summary>
//    /// 表结构
//    /// </summary>
//    public sealed class DbTable
//    {
//        /// <summary>
//        /// 表名称
//        /// </summary>
//        public string TableName { get; set; }
//        /// <summary>
//        /// 表的架构
//        /// </summary>
//        public string SchemaName { get; set; }
//        /// <summary>
//        /// 表的记录数
//        /// </summary>
//        public int Rows { get; set; }

//        /// <summary>
//        /// 是否含有主键
//        /// </summary>
//        public bool HasPrimaryKey { get; set; }
//    }
//    #endregion

//    #region DbColumn
//    /// <summary>
//    /// 表字段结构
//    /// </summary>
//    public sealed class DbColumn
//    {
//        /// <summary>
//        /// 字段ID
//        /// </summary>
//        public int ColumnID { get; set; }

//        /// <summary>
//        /// 是否主键
//        /// </summary>
//        public bool IsPrimaryKey { get; set; }

//        /// <summary>
//        /// 字段名称
//        /// </summary>
//        public string ColumnName { get; set; }

//        /// <summary>
//        /// 字段类型
//        /// </summary>
//        public string ColumnType { get; set; }

//        /// <summary>
//        /// 数据库类型对应的C#类型
//        /// </summary>
//        public string CSharpType
//        {
//            get
//            {
//                return SqlServerDbTypeMap.MapCsharpType(ColumnType);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public Type CommonType
//        {
//            get
//            {
//                return SqlServerDbTypeMap.MapCommonType(ColumnType);
//            }
//        }

//        /// <summary>
//        /// 字节长度
//        /// </summary>
//        public int ByteLength { get; set; }

//        /// <summary>
//        /// 字符长度
//        /// </summary>
//        public int CharLength { get; set; }

//        /// <summary>
//        /// 小数位
//        /// </summary>
//        public int Scale { get; set; }

//        /// <summary>
//        /// 是否自增列
//        /// </summary>
//        public bool IsIdentity { get; set; }

//        /// <summary>
//        /// 是否允许空
//        /// </summary>
//        public bool IsNullable { get; set; }

//        /// <summary>
//        /// 描述
//        /// </summary>
//        public string Remark { get; set; }
//    }
//    #endregion

//    #region SqlServerDbTypeMap

//    public class SqlServerDbTypeMap
//    {
//        public static string MapCsharpType(string dbtype)
//        {
//            if (string.IsNullOrEmpty(dbtype)) return dbtype;
//            dbtype = dbtype.ToLower();
//            string csharpType = "object";
//            switch (dbtype)
//            {
//                case "bigint": csharpType = "long"; break;
//                case "binary": csharpType = "byte[]"; break;
//                case "bit": csharpType = "bool"; break;
//                case "char": csharpType = "string"; break;
//                case "date": csharpType = "DateTime"; break;
//                case "datetime": csharpType = "DateTime"; break;
//                case "datetime2": csharpType = "DateTime"; break;
//                case "datetimeoffset": csharpType = "DateTimeOffset"; break;
//                case "decimal": csharpType = "decimal"; break;
//                case "float": csharpType = "double"; break;
//                case "image": csharpType = "byte[]"; break;
//                case "int": csharpType = "int"; break;
//                case "money": csharpType = "decimal"; break;
//                case "nchar": csharpType = "string"; break;
//                case "ntext": csharpType = "string"; break;
//                case "numeric": csharpType = "decimal"; break;
//                case "nvarchar": csharpType = "string"; break;
//                case "real": csharpType = "Single"; break;
//                case "smalldatetime": csharpType = "DateTime"; break;
//                case "smallint": csharpType = "short"; break;
//                case "smallmoney": csharpType = "decimal"; break;
//                case "sql_variant": csharpType = "object"; break;
//                case "sysname": csharpType = "object"; break;
//                case "text": csharpType = "string"; break;
//                case "time": csharpType = "TimeSpan"; break;
//                case "timestamp": csharpType = "byte[]"; break;
//                case "tinyint": csharpType = "byte"; break;
//                case "uniqueidentifier": csharpType = "Guid"; break;
//                case "varbinary": csharpType = "byte[]"; break;
//                case "varchar": csharpType = "string"; break;
//                case "xml": csharpType = "string"; break;
//                default: csharpType = "object"; break;
//            }
//            return csharpType;
//        }

//        public static Type MapCommonType(string dbtype)
//        {
//            if (string.IsNullOrEmpty(dbtype)) return Type.Missing.GetType();
//            dbtype = dbtype.ToLower();
//            Type commonType = typeof(object);
//            switch (dbtype)
//            {
//                case "bigint": commonType = typeof(long); break;
//                case "binary": commonType = typeof(byte[]); break;
//                case "bit": commonType = typeof(bool); break;
//                case "char": commonType = typeof(string); break;
//                case "date": commonType = typeof(DateTime); break;
//                case "datetime": commonType = typeof(DateTime); break;
//                case "datetime2": commonType = typeof(DateTime); break;
//                case "datetimeoffset": commonType = typeof(DateTimeOffset); break;
//                case "decimal": commonType = typeof(decimal); break;
//                case "float": commonType = typeof(double); break;
//                case "image": commonType = typeof(byte[]); break;
//                case "int": commonType = typeof(int); break;
//                case "money": commonType = typeof(decimal); break;
//                case "nchar": commonType = typeof(string); break;
//                case "ntext": commonType = typeof(string); break;
//                case "numeric": commonType = typeof(decimal); break;
//                case "nvarchar": commonType = typeof(string); break;
//                case "real": commonType = typeof(Single); break;
//                case "smalldatetime": commonType = typeof(DateTime); break;
//                case "smallint": commonType = typeof(short); break;
//                case "smallmoney": commonType = typeof(decimal); break;
//                case "sql_variant": commonType = typeof(object); break;
//                case "sysname": commonType = typeof(object); break;
//                case "text": commonType = typeof(string); break;
//                case "time": commonType = typeof(TimeSpan); break;
//                case "timestamp": commonType = typeof(byte[]); break;
//                case "tinyint": commonType = typeof(byte); break;
//                case "uniqueidentifier": commonType = typeof(Guid); break;
//                case "varbinary": commonType = typeof(byte[]); break;
//                case "varchar": commonType = typeof(string); break;
//                case "xml": commonType = typeof(string); break;
//                default: commonType = typeof(object); break;
//            }
//            return commonType;
//        }
//    }
//    #endregion
}
