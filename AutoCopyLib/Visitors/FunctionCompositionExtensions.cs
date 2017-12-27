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
        private ParameterExpression[] _parameters;
        private Expression[] _replacements;
        private int _length;

        private ParameterReplacer(ParameterExpression[] parameters, Expression[] replacements)
        {
            _parameters = parameters;
            _replacements = replacements;
            _length = _parameters.Length;
        }

        public static Expression Replace(Expression expression, ParameterExpression[] parameters, Expression[] replacements)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ArgumentException($"the parameter {nameof(parameters)} cannot be null or empty");
            if (replacements == null || replacements.Length == 0)
                throw new ArgumentException($"the parameter {nameof(replacements)} cannot be null or empty");
            if (parameters.Length != replacements.Length)
                throw new ArgumentException($"the length of {nameof(parameters)} must be equal to the length of {nameof(replacements)}");
            return new ParameterReplacer(parameters, replacements).Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            for (int i = 0; i < _length;i++)
            {
                if (parameter.Type == _parameters[i].Type)
                {
                    return _replacements[i];
                }
            }
            return base.VisitParameter(parameter);
        }
    }
}
