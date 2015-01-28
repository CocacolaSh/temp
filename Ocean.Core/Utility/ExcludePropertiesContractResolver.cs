using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ocean.Core
{

    public class ExcludePropertiesContractResolver : DefaultContractResolver
    {
        IEnumerable<string> lbzExclude;
        public ExcludePropertiesContractResolver(IEnumerable<string> excludedProperties)
        {
            lbzExclude = excludedProperties;
        }
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).ToList().FindAll(p => !lbzExclude.Contains(p.PropertyName));
        }
    }
}
