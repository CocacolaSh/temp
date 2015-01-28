using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;

namespace Ocean.Core.Data.DynamicQuery.Mvc
{
    public static class QueryConditionExtension
    {
        public static MvcHtmlString QueryTextbox(this HtmlHelper html, string properyName, string labelText, QueryOperator opt = QueryOperator.EQUAL, string valuetype=null)
        {
            var input = "<label>{0}</label><input class='input-medium search-query' type='text' name='{1}'/>";
            var res = string.Format(input, HttpUtility.HtmlEncode(labelText), "_query." + properyName);            
            if (QueryOperator.EQUAL != opt)
            {
                res += "<input type='hidden' name='_query." + properyName + ".operator' value='" + opt.ToString() + "'/>";
            }

            if (!string.IsNullOrEmpty(valuetype))
            {
                res += "<input type='hidden' name='_query." + properyName + ".valuetype' value='" +valuetype+ "'/>";
            }
            return MvcHtmlString.Create(res);
        }
    }
    
}
