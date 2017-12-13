﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AutoCopyLib
{
    public partial class AutoCopy<TSource, TDest>
    {
        #region copied from NeinLinq (MIT License): https://github.com/axelheer/nein-linq/blob/master/src/NeinLinq/NullsafeQueryRewriter.cs
        internal class NullsafeQueryRewriter : ExpressionVisitor
        {
            private static readonly LockingConcurrentDictionary<Type, Expression> cache = new LockingConcurrentDictionary<Type, Expression>(new Func<Type, Expression>(NullsafeQueryRewriter.Fallback));
            protected override Expression VisitMember(MemberExpression node)
            {
                if (node == null)
                    throw new ArgumentException(nameof(node));
                var target = Visit(node.Expression);
                if(!IsSafe(target))
                {
                    return BeSafe(target, node, node.Update);
                }
                return node.Update(target);
            }
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                var target = Visit(node.Object);

                if (!IsSafe(target))
                {
                    // insert null-check before invoking instance method
                    return BeSafe(target, node, fallback => node.Update(fallback, node.Arguments));
                }

                var arguments = Visit(node.Arguments);

                if (IsExtensionMethod(node.Method) && !IsSafe(arguments[0]))
                {
                    // insert null-check before invoking extension method
                    return BeSafe(arguments[0], node.Update(null, arguments), fallback =>
                    {
                        var args = new Expression[arguments.Count];
                        arguments.CopyTo(args, 0);
                        args[0] = fallback;

                        return node.Update(null, args);
                    });
                }

                return node.Update(target, arguments);
            }
            public Expression visit(Expression node)
            {
                return this.Visit(node);
            }
            static bool IsSafe(Expression expression)
            {
                // in method call results and constant values we trust to avoid too much conditions...
                return expression == null
                    || expression.NodeType == ExpressionType.Call
                    || expression.NodeType == ExpressionType.Constant
                    || !IsNullableOrReferenceType(expression.Type);
            }
            static bool IsNullableOrReferenceType(Type type)
            {
                return !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
            }
            static Expression BeSafe(Expression target, Expression expression, Func<Expression, Expression> update)
            {
                var fallback = cache.GetOrAdd(target.Type);

                if (fallback != null)
                {
                    // coalesce instead, a bit intrusive but fast...
                    return update(Expression.Coalesce(target, fallback));
                }

                // target can be null, which is why we are actually here...
                var targetFallback = Expression.Constant(null, target.Type);

                // expression can be default or null, which is basically the same...
                var expressionFallback = !IsNullableOrReferenceType(expression.Type)
                    ? (Expression)Expression.Default(expression.Type) : Expression.Constant(null, expression.Type);

                return Expression.Condition(Expression.Equal(target, targetFallback), expressionFallback, expression);
            }
            static Expression Fallback(Type type)
            {
                // default values for generic collections
                if (type.IsConstructedGenericType && type.GenericTypeArguments.Length == 1)
                {
                    return CollectionFallback(typeof(List<>), type)
                        ?? CollectionFallback(typeof(HashSet<>), type);
                }

                // default value for arrays
                if (type.IsArray)
                {
                    return Expression.NewArrayInit(type.GetElementType());
                }

                return null;
            }

            static Expression CollectionFallback(Type definition, Type type)
            {
                var collection = definition.MakeGenericType(type.GenericTypeArguments);

                // try if an instance of this collection would suffice
                if (type.GetTypeInfo().IsAssignableFrom(collection.GetTypeInfo()))
                {
                    return Expression.Convert(Expression.New(collection), type);
                }

                return null;
            }

            static bool IsExtensionMethod(MethodInfo element)
            {
                return element.IsDefined(typeof(ExtensionAttribute), false);
            }
        }
        #endregion
    }
}
