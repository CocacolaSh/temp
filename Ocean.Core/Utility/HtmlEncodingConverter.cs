using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ocean.Core
{
    public class HtmlEncodingConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var encoding = "";
            if (value is String)
            {
                encoding = HttpUtility.HtmlEncode(value.ToString());
            }
            writer.WriteValue(encoding);
        }
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
