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
                if (p.Type == typeof(P))
                    return _p;
                return base.VisitParameter(p);
            }
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                //判断调用方法的实例是否为AutoCopy<>的派生类
                if(node.Object!=null && IsAssignableToGenericType(node.Object.Type,typeof(AutoCopy<,>)))
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
                    object lambda= autoCopyType.InvokeMember("RegisterCore", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, calledClass, new object[] { false }); ;
                    LambdaExpression lambdaExpression = (LambdaExpression)lambda;
                    //获取表达式的形参并根据类型定义新的形参
                    var parameter1 = lambdaExpression.Parameters[0];
                    var newparameter1 = Expression.Variable(parameter1.Type, "p1");
                    var parameter2 = lambdaExpression.Parameters[1];
                    var newparameter2 = Expression.Parameter(parameter2.Type, "p2");
                    //去掉lambda表达式最外层的try catch
                    var body=(BlockExpression)((TryExpression)lambdaExpression.Body).Body;
                    //遍历表达式的body，替换原来的两个参数为新定义的参数
                    body = ParameterReplacer.Replace(body, parameter1, newparameter1) as BlockExpression;
                    body = ParameterReplacer.Replace(body, parameter2, newparameter2) as BlockExpression;
                    //初始化临时变量并构造新的lambda表达式
                    //去掉原来lambda表达式第一行的Expression.Coalesce运算和最后的return true
                    var initExpression = Expression.Assign(newparameter1, Expression.New(newparameter1.Type));
                    var returnExpression = newparameter1;
                    var newBlockExpression = Expression.Block(new[] { newparameter1 }.Concat(body.Variables),new[] { initExpression }.Concat(body.Expressions.Skip(1).Take(body.Expressions.Count-2)).Concat(new[] { returnExpression }));
                    var newLambda = Expression.Lambda(newBlockExpression, newparameter2);
                    return Expression.Invoke(newLambda, argument);
                }
                return base.VisitMethodCall(node);
            }
            private ParameterExpression _p;
            public Option(ParameterExpression p)
            {
                _p = p;
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
            public Func<P,TValue> ResolveUsing<TValue>(Func<P,TValue> resolver)
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
