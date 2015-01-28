//http://www.cnblogs.com/yinzixin/archive/2012/11/30/entity-framework-dynamic-query.html
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocean.Core.Data.DynamicQuery
{
    public class QueryDescriptor
    {
        public OrderByClause OrderBy { get; set; }
        public IList<QueryCondition> Conditions { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

    public class OrderByClause
    {
        public string Key { get; set; }
        public OrderSequence Order { get; set; }
    }

    public enum OrderSequence
    {
        ASC,
        DESC        
    }
}
