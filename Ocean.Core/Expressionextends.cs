using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Ocean.Core
{
//    public class Expressionextends
//    {
//        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
//        {
//            // build parameter map (from parameters of second to parameters of first)
//            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
//            // replace parameters in the second lambda expression with parameters from the first
//            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
//            // apply composition of lambda expression bodies to parameters from the first expression 
//            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
//        }
//        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
//        {
//            return first.Compose(second, Expression.AndAlso);
//        }
//        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
//        {
//            return first.Compose(second, Expression.Or);
//        }
//        public class ParameterRebinder : ExpressionVisitor
//        {
//            private readonly Dictionary<ParameterExpression, ParameterExpression> map;
//            public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
//            {
//                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
//            }
//            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
//            {
//                return new ParameterRebinder(map).Visit(exp);
//            }
//            protected override Expression VisitParameter(ParameterExpression p)
//            {
//                ParameterExpression replacement;
//                if (map.TryGetValue(p, out replacement))
//                {
//                    p = replacement;
//                }
//                return base.VisitParameter(p);
//            }
//        }


//        //---------------------------
//        //那么我们可不可以直接写Expression，将条件转换成上述SQL语句呢
//        public static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(Expression<Func<TElement, TValue>> valueSelector,IEnumerable<TValue> values)
//        {
//            var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
//            var startWiths = values.Select(value => (Expression)Expression.Call(valueSelector.Body, startsWithMethod, Expression.Constant(value, typeof(TValue))));
//            var body = startWiths.Aggregate<Expression>(((accumulate, equal) => Expression.Or(accumulate, equal)));
//            var p = Expression.Parameter(typeof(TElement));
//            return Expression.Lambda<Func<TElement, bool>>(body, p);
//        }
//        private static void QueryProducts<T>(IQueryable<T> query) 
//{ 
//var productNames = new string[] {"P1", "P2"}; 
//var query1 = from a in query.Where(BuildContainsExpression<T, string>(d=>d.ProductName, productNames)) 
//select a; 
//var items2 = query1.ToList(); 
//} 
//private static void QueryProducts(IQueryable<T> query) 
//{ 
//var productNames = new string[] {"P1", "P2"}; 
//var query1 = from a in query.Where(BuildContainsExpression<T, string>(d=>d.ProductName, productNames)) 
//select a; 
//var items2 = query1.ToList(); 
//}
//    }
}
