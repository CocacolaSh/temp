using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Ocean.Core.Data.CreateQuery
{
    public class CreateLambda
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="sort">排序字段</param>
        /// <param name="dir">ASC/DESC</param>
        /// <param name="count">总纪录数</param>
        /// <returns></returns>
        public List<T> PageList<T>(IQueryable<T> query, int start, int limit, string sort, string dir, out int count)
        {
            if (string.IsNullOrEmpty(sort))
                throw new Exception("使用此方法,必须指定排序字段!");

            ParameterExpression param = Expression.Parameter(typeof(T), "it");
            Expression body = param;

            if (Nullable.GetUnderlyingType(body.Type) != null)
                body = Expression.Property(body, "Value");

            PropertyInfo sortProperty = typeof(T).GetProperty(sort, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (sortProperty == null)
                throw new Exception("对像上不存在" + sortProperty + "的字段");

            body = Expression.MakeMemberAccess(body, sortProperty);
            LambdaExpression keySelectorLambda = Expression.Lambda(body, param);
            string queryMethod = dir == "DESC" ? "OrderByDescending" : "OrderBy";
            query = query.Provider.CreateQuery<T>(Expression.Call(typeof(Queryable), queryMethod,
                                                               new Type[] { typeof(T), body.Type },
                                                               query.Expression,
                                                               Expression.Quote(keySelectorLambda)));

            count = query.Count();
            if (start >= 0 && limit > 0) query = query.Skip(start).Take(limit);
            return query.ToList();
        }

        public static IQueryable<T> MakeQuery<T>(IQueryable<T> source, string colName, string colValue)
        {
            Type theType = typeof(T);
            //创建一个参数c
            ParameterExpression param = Expression.Parameter(typeof(T), "c");
            //c.City=="London"
            Expression left = Expression.Property(param, typeof(T).GetProperty(colName));
            Expression right = Expression.Constant(colValue);
            Expression filter = Expression.Equal(left, right);
            Expression pred = Expression.Lambda(filter, param);
            //Where(c=>c.City=="London")
            Expression expr = Expression.Call(typeof(Queryable), "Where",
                new Type[] { typeof(T) },
                Expression.Constant(source), pred);
            //生成动态查询
            IQueryable<T> query = source.AsQueryable()
                .Provider.CreateQuery<T>(expr);
            return query;
        }
    }
}
