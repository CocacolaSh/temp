using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace Ocean.Core.Data.DynamicQuery
{
    public static class DynamicQueryExtension
    {
        public static IQueryable<T> Query<T>(this IQueryable<T> data, QueryDescriptor descriptor)
        {
            int totalPages = 0;
            return Query<T>(data, descriptor, out totalPages);
        }

        public static IQueryable<T> Query<T>(this IQueryable<T> data, QueryDescriptor descriptor,out int totalPages)
        {
            totalPages = 0;
            var parser = new QueryExpressionParser<T>();
           
            //filter
            var filter = parser.Parse(descriptor);
            var result = data.Where(filter);
            
            //order
            if (descriptor.OrderBy != null)
            {
                Type type = typeof(T);
                var parameter=Expression.Parameter(type);

                var propertyInfo = type.GetProperty(descriptor.OrderBy.Key);
                Expression propertySelector = Expression.Property(parameter, propertyInfo);

                //To deal with different type of the property selector, very hacky
                //looking for a better solution
                if (propertyInfo.PropertyType == typeof(string))
                {
                    var orderby = Expression.Lambda<Func<T, string>>(propertySelector, parameter);
                    if (descriptor.OrderBy.Order == OrderSequence.DESC)
                        result = result.OrderByDescending(orderby);
                    else
                        result = result.OrderBy(orderby);
                }
                else if (propertyInfo.PropertyType == typeof(decimal))
                {
                    var orderby = Expression.Lambda<Func<T, decimal>>(propertySelector, parameter);
                    if (descriptor.OrderBy.Order == OrderSequence.DESC)
                        result = result.OrderByDescending(orderby);
                    else
                        result = result.OrderBy(orderby);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    var orderby = Expression.Lambda<Func<T, int>>(propertySelector, parameter);
                    if (descriptor.OrderBy.Order == OrderSequence.DESC)
                        result = result.OrderByDescending(orderby);
                    else
                        result = result.OrderBy(orderby);
                }
                else if (propertyInfo.PropertyType == typeof(double))
                {
                    var orderby = Expression.Lambda<Func<T, double>>(propertySelector, parameter);
                    if (descriptor.OrderBy.Order == OrderSequence.DESC)
                        result = result.OrderByDescending(orderby);
                    else
                        result = result.OrderBy(orderby);
                }
                else if (propertyInfo.PropertyType == typeof(float))
                {
                    var orderby = Expression.Lambda<Func<T, float>>(propertySelector, parameter);
                    if (descriptor.OrderBy.Order == OrderSequence.DESC)
                        result = result.OrderByDescending(orderby);
                    else
                        result = result.OrderBy(orderby);
                }
            }

            //paging
            if (descriptor.PageSize > 0)
            {
                int totalCount = result.Count();
                totalPages = totalCount % descriptor.PageSize == 0 ? totalCount / descriptor.PageSize : totalCount / descriptor.PageSize + 1;
                return result.Skip((descriptor.PageIndex - 1) * descriptor.PageSize).Take(descriptor.PageSize);
            }
            return result;
        }
    }
}
