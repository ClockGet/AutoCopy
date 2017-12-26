//using AutoMapper;
//using AutoMapper.QueryableExtensions;
using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

namespace AutoCopyLib
{
    [Obsolete]
    public static class FunctionCompositionExtensions
    {
        //private static ConcurrentDictionary<Tuple<Type, Type>, Tuple<MethodInfo, Expression>> dictionary = new ConcurrentDictionary<Tuple<Type, Type>, Tuple<MethodInfo, Expression>>();
        //private static MethodInfo method = typeof(FunctionCompositionExtensions).GetMethod("Compose", BindingFlags.NonPublic | BindingFlags.Static);

        //public static Expression<Func<D, bool>> MapExpression<S, D>(this Expression<Func<S, bool>> selector)
        //{
        //    var bulidMethod = dictionary.GetOrAdd(new Tuple<Type, Type>(typeof(S), typeof(D)), _ =>
        //    {
        //        var expression = Mapper.Engine.CreateMapExpression<D, S>();
        //        return new Tuple<MethodInfo, Expression>(method.MakeGenericMethod(typeof(D), typeof(bool), typeof(S)), expression);
        //    });
        //    return bulidMethod.Item1.Invoke(null, new[] { selector, bulidMethod.Item2 }) as Expression<Func<D, bool>>;

        //}

        //static Expression<Func<X, Y>> Compose<X, Y, Z>(this Expression<Func<Z, Y>> outer, Expression<Func<X, Z>> inner)
        //{
        //    return Expression.Lambda<Func<X, Y>>(
        //        ParameterReplacer.Replace(outer.Body, outer.Parameters[0], inner.Body),
        //        inner.Parameters[0]);
        //}

        //static Expression<Predicate<X>> ComposePredicate<X, Z>(this Expression<Predicate<Z>> outer, Expression<Func<X, Z>> inner)
        //{
        //    return Expression.Lambda<Predicate<X>>(
        //        ParameterReplacer.Replace(outer.Body, outer.Parameters[0], inner.Body),
        //        inner.Parameters[0]);
        //}
    }
    internal class ParameterReplacer : ExpressionVisitor
    {
        private ParameterExpression _parameter;
        private Expression _replacement;

        private ParameterReplacer(ParameterExpression parameter, Expression replacement)
        {
            _parameter = parameter;
            _replacement = replacement;
        }

        public static Expression Replace(Expression expression, ParameterExpression parameter, Expression replacement)
        {
            return new ParameterReplacer(parameter, replacement).Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            if (parameter.Type == _parameter.Type)
            {
                return _replacement;
            }
            return base.VisitParameter(parameter);
        }
    }
}
