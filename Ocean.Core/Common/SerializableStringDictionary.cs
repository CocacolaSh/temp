using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace Ocean.Core.Common
{
    /// <summary> 
    /// 支持XML序列化的泛型StringDictionary类 
    /// </summary> 
    [XmlRoot("Dictionary")]
    public class SerializableStringDictionary : System.Collections.Specialized.StringDictionary, IXmlSerializable
    {
        public SerializableStringDictionary()
            : base()
        {
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        #region 从对象的XML表示形式生成该对象
        /// <summary> 
        /// 从对象的XML表示形式生成该对象 
        /// </summary> 
        /// <param name="reader"></param> 
        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(String));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Key");
                string key = (string)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("Value");
                string value = (string)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }
        #endregion

        #region 将对象转换为其XML表示形式
        /// <summary> 
        /// 将对象转换为其XML表示形式 
        /// </summary> 
        /// <param name="writer"></param> 
        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(String));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(String));

            foreach (string key in this.Keys)
            {
                writer.WriteStartElement("Key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("Value");
                string value = this[key];
                valueSerializer.Serialize(writer, value); writer.WriteEndElement();
            }
        }
        #endregion
    }
}
