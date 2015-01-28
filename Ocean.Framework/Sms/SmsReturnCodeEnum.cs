using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Ocean.Framework.Sms
{
    public enum SmsReturnCode
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [XmlEnum("0")]
        操作成功 = 0,

        /// <summary>
        /// 连接数据库出错
        /// </summary>
        [XmlEnum("-1")]
        连接数据库出错 = -1,

        /// <summary>
        /// 数据库关闭失败
        /// </summary>
        [XmlEnum("-2")]
        数据库关闭失败 = -2,

        /// <summary>
        /// 数据库插入错误
        /// </summary>
        [XmlEnum("-3")]
        数据库插入错误 = -3,

        /// <summary>
        /// 数据库删除错误
        /// </summary>
        [XmlEnum("-4")]
        数据库删除错误 = -4,

        /// <summary>
        /// 数据库查询错误
        /// </summary>
        [XmlEnum("-5")]
        数据库查询错误 = -5,

        /// <summary>
        /// 参数错误
        /// </summary>
        [XmlEnum("-6")]
        参数错误 = -6,

        /// <summary>
        /// API标识非法
        /// </summary>
        [XmlEnum("-7")]
        API标识非法 = -7,

        /// <summary>
        /// 消息内容太长
        /// </summary>
        [XmlEnum("-8")]
        消息内容太长 = -8,

        /// <summary>
        /// 没有初始化或初始化失败
        /// </summary>
        [XmlEnum("-9")]
        没有初始化或初始化失败 = -9,

        /// <summary>
        /// API接口处于暂停失效状态
        /// </summary>
        [XmlEnum("-10")]
        API接口处于暂停失效状态 = -10,

        /// <summary>
        /// 短信网关未连接
        /// </summary>
        [XmlEnum("-11")]
        短信网关未连接 = -11,

        /// <summary>
        /// 未知错误
        /// </summary>
        [XmlEnum("-12")]
        未知错误 = -12
    }
}