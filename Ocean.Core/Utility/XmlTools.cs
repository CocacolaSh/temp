using System;
using System.Collections;
using System.Xml;
using System.Data;
using System.IO;

namespace Ocean.Core.Utility
{
    public class XmlTools
    {
        #region 根据节点名称查找节点
        /// <summary>
        /// 根据节点名称查找节点
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <returns></returns>
        public static XmlNode NodeByName(XmlNode ParentNode, string NodeName)
        {
            if (ParentNode == null) return null;
            XmlNode node = ParentNode.SelectSingleNode(NodeName);
            return node;
        }
        #endregion

        #region 根据节点名称查找节点，如节点不存在自动创建
        /// <summary>
        /// 根据节点名称查找节点，如节点不存在自动创建
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <returns></returns>
        public static XmlNode NodeFindOrCreate(XmlNode ParentNode, string NodeName)
        {
            if (ParentNode == null) return null;
            XmlNode node = ParentNode.SelectSingleNode(NodeName);
            if (node == null)
            {
                node = ParentNode.OwnerDocument.CreateElement(NodeName);
                ParentNode.AppendChild(node);
            }
            return node;
        }
        #endregion

        #region 根据属性值查找节点
        /// <summary>
        /// 根据属性值查找节点
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        /// <returns></returns>
        public static XmlNode NodeByAttributeValue(XmlNode ParentNode, string AttributeName, string AttributeValue)
        {
            foreach (XmlNode node in ParentNode.ChildNodes)
            {
                if (node.Attributes[AttributeName] != null && node.Attributes[AttributeName].Value == AttributeValue)
                {
                    return node;
                }
            }
            return null;
        }
        #endregion

        #region 查找属性值
        /// <summary>
        /// 查找属性值
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="AttributeName"></param>
        /// <returns></returns>
        public static string NodeByAttributeValue(XmlNode ParentNode, string AttributeName)
        {
            foreach (XmlNode node in ParentNode.ChildNodes)
            {
                if (node.Attributes[AttributeName] != null)
                {
                    return node.Attributes[AttributeName].Value;
                }
            }
            return "";
        }
        #endregion

        #region 根据属性值查找节点列表
        /// <summary>
        /// 根据属性值查找节点列表
        /// </summary>
        public static ArrayList NodeListByAttributeValue(XmlNode ParentNode, string AttributeName, string AttributeValue)
        {
            ArrayList list = new ArrayList();
            foreach (XmlNode node in ParentNode.ChildNodes)
            {
                if (node.Attributes[AttributeName] != null && node.Attributes[AttributeName].Value.ToLower() == AttributeValue.ToLower())
                {
                    list.Add(node);
                }
            }
            return list;
        }
        #endregion

        #region 返回字符串值
        /// <summary>
        /// 返回字符串值
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <returns></returns>
        public static string NodeValue(XmlNode ParentNode, string NodeName)
        {
            XmlNode node = NodeByName(ParentNode, NodeName);
            if (node == null)
                return "";
            else
                return node.InnerText;
        }

        public static string NodeValue(XmlNode ParentNode, string NodeName, string Default)
        {
            XmlNode node = NodeByName(ParentNode, NodeName);
            if (node == null)
                return Default;
            else
                return node.InnerText;
        }
        #endregion

        #region 返回整型值
        /// <summary>
        /// 返回整型值
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static int NodeValue(XmlNode ParentNode, string NodeName, int Default)
        {
            XmlNode node = NodeByName(ParentNode, NodeName);
            if (node == null)
                return Default;
            else
            {
                int nResult = Default;
                int.TryParse(node.InnerText, out nResult);
                return nResult;
            }
        }
        #endregion

        #region 返回布尔值
        /// <summary>
        /// 返回布尔值
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static bool NodeValue(XmlNode ParentNode, string NodeName, bool Default)
        {
            XmlNode node = NodeByName(ParentNode, NodeName);
            if (node == null)
                return Default;
            else
            {
                bool bResult = Default;
                bool.TryParse(node.InnerText, out bResult);
                return bResult;
            }
        }
        #endregion

        #region 返回时间
        /// <summary>
        /// 返回时间
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static DateTime NodeValue(XmlNode ParentNode, string NodeName, DateTime Default)
        {
            XmlNode node = NodeByName(ParentNode, NodeName);
            if (node == null)
                return Default;
            else
            {
                DateTime dtResult = Default;
                DateTime.TryParse(node.InnerText, out dtResult);
                return dtResult;
            }
        }
        #endregion

        #region 返回Guid
        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static Guid NodeValue(XmlNode ParentNode, string NodeName, Guid Default)
        {
            XmlNode node = NodeByName(ParentNode, NodeName);
            if (node == null)
                return Default;
            else
            {
                try
                {
                    return new Guid(node.InnerText);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }
        #endregion

        #region 设置节点值(字符串)
        /// <summary>
        /// 设置节点值(字符串)
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <param name="Value"></param>
        public static void SetNodeValue(XmlNode ParentNode, string NodeName, string Value)
        {
            XmlNode node = NodeFindOrCreate(ParentNode, NodeName);
            node.InnerText = Value;
        }
        #endregion

        #region 设置节点值（整形）
        /// <summary>
        /// 设置节点值（整形）
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <param name="Value"></param>
        public static void SetNodeValue(XmlNode ParentNode, string NodeName, int Value)
        {
            XmlNode node = NodeFindOrCreate(ParentNode, NodeName);
            node.InnerText = Value.ToString();
        }
        #endregion

        #region 设置节点值（布尔值）
        /// <summary>
        /// 设置节点值（布尔值）
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="NodeName"></param>
        /// <param name="Value"></param>
        public static void SetNodeValue(XmlNode ParentNode, string NodeName, bool Value)
        {
            XmlNode node = NodeFindOrCreate(ParentNode, NodeName);
            node.InnerText = Value.ToString();
        }
        #endregion

        #region 将Xml内容字符串转换成DataSet对象
        /// <summary>
        /// 将Xml内容字符串转换成DataSet对象
        /// </summary>
        /// <param name="xmlStr">Xml内容字符串</param>
        /// <returns>DataSet对象</returns>
        public static DataSet XmlToDataSet(string xmlStr)
        {
            if (!string.IsNullOrEmpty(xmlStr))
            {
                StringReader StrStream = null;
                XmlTextReader Xmlrdr = null;
                try
                {
                    DataSet ds = new DataSet();
                    StrStream = new StringReader(xmlStr);
                    Xmlrdr = new XmlTextReader(StrStream);
                    ds.ReadXml(Xmlrdr);
                    return ds;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    //释放资源
                    if (Xmlrdr != null)
                    {
                        Xmlrdr.Close();
                        StrStream.Close();
                        StrStream.Dispose();
                    }
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region XML转换成DataSet
        /// <summary>
        /// XML转换成DataSet
        /// </summary>
        /// <param name="xmlPath">XML文档路径</param>
        /// <returns></returns>
        public static DataSet XmlDocumentationDataSet(string xmlPath)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(System.Web.HttpContext.Current.Server.MapPath(xmlPath));
            return ds;
        }
        #endregion

        #region 获取属性值（字符串）
        /// <summary>
        /// 获取属性值（字符串）
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        public static string GetAttribute(XmlNode Node, string AttributeName)
        {
            if (Node.Attributes[AttributeName] == null)
            {
                return string.Empty;
            }
            else
            {
                return Node.Attributes[AttributeName].Value;
            }
        }
        #endregion

        #region 获取属性值（整形）
        /// <summary>
        /// 获取属性值（整形）
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        public static int GetAttribute(XmlNode Node, string AttributeName, int Default)
        {
            if (Node.Attributes[AttributeName] == null)
            {
                return Default;
            }
            else
            {
                int nResult = Default;
                int.TryParse(Node.Attributes[AttributeName].Value, out nResult);
                return nResult;
            }
        }
        #endregion

        #region 获取属性值（布尔值）
        /// <summary>
        /// 获取属性值（布尔值）
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        public static bool GetAttribute(XmlNode Node, string AttributeName, bool Default)
        {
            if (Node.Attributes[AttributeName] == null)
            {
                return Default;
            }
            else
            {
                bool nResult = Default;
                bool.TryParse(Node.Attributes[AttributeName].Value, out nResult);
                return nResult;
            }
        }
        #endregion

        #region 获取属性值（时间日期）
        /// <summary>
        /// 获取属性值（时间日期）
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        public static DateTime GetAttribute(XmlNode Node, string AttributeName, DateTime Default)
        {
            if (Node.Attributes[AttributeName] == null)
            {
                return Default;
            }
            else
            {
                DateTime nResult = Default;
                DateTime.TryParse(Node.Attributes[AttributeName].Value, out nResult);
                return nResult;
            }
        }
        #endregion

        #region 获取属性值（Guid）
        /// <summary>
        /// 获取属性值(Guid)
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        public static Guid GetAttribute(XmlNode Node, string AttributeName, Guid Default)
        {
            if (Node.Attributes[AttributeName] == null)
            {
                return Default;
            }
            else
            {
                try
                {
                    return new Guid(Node.Attributes[AttributeName].Value);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }
        #endregion

        #region 设置属性值(Void)
        /// <summary>
        /// 设置属性值(Void)
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        public static void SetAttributeValue(XmlNode Node, string AttributeName, string AttributeValue)
        {
            if (Node.Attributes[AttributeName] == null)
            {
                XmlAttribute attr = Node.OwnerDocument.CreateAttribute(AttributeName);
                Node.Attributes.Append(attr);
                attr.Value = AttributeValue;
            }
            else
            {
                Node.Attributes[AttributeName].Value = AttributeValue;
            }
        }
        #endregion

        #region 安全加载XML文件
        /// <summary>
        /// 安全加载XML文件
        /// </summary>
        public static void Load(XmlDocument XmlDoc, string FileName)
        {
            if (File.Exists(FileName))
                XmlDoc.Load(FileName);
            else
                XmlDoc.LoadXml("<?xml version=\"1.0\" encoding=\"gb2312\"?><List />");
        }
        #endregion
    }
}
