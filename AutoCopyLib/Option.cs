using AutoCopyLib.Visitors;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoCopyLib
{
    public partial class AutoCopy<TSource, TDest>
    {
        public class Option<P> : ExpressionVisitor
        {
            protected override Expression VisitParameter(ParameterExpression p)//替换掉对象
            {
                if (p.Name == "name" && p.Type == typeof(string))
                    return p;
                if (p.Type == typeof(P))
                    return _parameterTuple.Source;
                return p;
            }
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                //判断调用方法的实例是否为AutoCopy<>的派生类
                if (node.Object != null && IsAssignableToGenericType(node.Object.Type, typeof(AutoCopy<,>)))
                {
                    //获取调用方法的实例
                    var member = (MemberExpression)node.Object;
                    var argument = Visit(node.Arguments[0]);
                    var constant = (ConstantExpression)member.Expression;
                    var anonymousClassInstance = constant.Value;
                    var calledClassField = (FieldInfo)member.Member;
                    var calledClass = calledClassField.GetValue(anonymousClassInstance);
                    //调用实例的RegisterCore方法，获取Lambda表达式
                    var autoCopyType = calledClass.GetType();
                    object lambda = autoCopyType.InvokeMember("Decompiler", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, calledClass, new object[] { false });
                    LambdaExpression lambdaExpression = (LambdaExpression)lambda;
                    return Expression.Invoke(lambdaExpression, argument, _parameterTuple.ErrorMsg);
                }
                return base.VisitMethodCall(node);
            }
            private ParameterTuple _parameterTuple;
            public Option(ParameterTuple parameterTuple)
            {
                _parameterTuple = parameterTuple;
            }
            public Expression MapFrom<TValue>(
                Expression<Func<P, TValue>> selector)
            {
                Expression body = selector;
                if (body is LambdaExpression)
                {
                    body = ((LambdaExpression)body).Body;
                }
                var body2 = Visit(body);
                return body2;
            }
            public Expression MapFrom<TValue>(Expression<Func<P, string, TValue>> selector)
            {
                Expression body = selector;
                if (body is LambdaExpression)
                {
                    body = ((LambdaExpression)body).Body;
                }
                var body2 = Visit(body);
                return body2;
            }
            public Func<P, TValue> ResolveUsing<TValue>(Func<P, TValue> resolver)
            {
                return resolver;
            }
            bool IsAssignableToGenericType(Type givenType, Type genericType)
            {
                var interfaceTypes = givenType.GetInterfaces();

                foreach (var it in interfaceTypes)
                {
                    if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                        return true;
                }

                if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                    return true;

                Type baseType = givenType.BaseType;
                if (baseType == null) return false;

                return IsAssignableToGenericType(baseType, genericType);
            }
        }
    }
}
