using AutoCopyLib.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoCopyLib
{
    internal class TypeConverter
    {
        private static MethodInfo ChangeTypeMethod = null;
        private static ConcurrentDictionary<Type, MethodInfo> _tryParseDic = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> _toStringDic = new ConcurrentDictionary<Type, MethodInfo>();
        static TypeConverter()
        {
            ChangeTypeMethod = typeof(Convert).GetMethod("ChangeType", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(object), typeof(TypeCode) }, new ParameterModifier[0]);
        }
        public static bool TryConvert(Expression parameter, Type sourceType, Type destType, out Action<List<Expression>, ParameterExpression, PropertyInfo, Func<Expression, Expression>> act, out ParameterExpression variable)
        {
            Expression expression = null;
            variable = null;
            act = null;
            var typeCode = Type.GetTypeCode(sourceType);
            if (typeCode == TypeCode.Empty || typeCode == TypeCode.DBNull)
                return false;
            MethodInfo method = null;
            //判断是否可以显式转换
            if (destType.GetMethod("op_Explicit",new Type[] {sourceType })!=null)
            {
                expression = Expression.Convert(parameter, destType);
                
            }
            //判断是否可以隐式转换
            else if(destType.GetMethod("op_Implicit", new Type[] { sourceType }) != null)
            {
                expression = parameter;
            }
            //判断是否存在继承关系
            //Base base=Derive
            else if(destType.IsAssignableFrom(sourceType))
            {
                expression = parameter;
            }
            //判断是否存在Convert.ToXX方法
            else if (typeCode != TypeCode.String && typeCode != TypeCode.Object && Type.GetTypeCode(destType)!=TypeCode.Object)
            {
                method = typeof(Convert).GetMethod("To" + destType.Name, BindingFlags.Static | BindingFlags.Public, null, new Type[] { sourceType }, new ParameterModifier[0]);
                if (method != null)
                {
                    expression = Expression.Call(null, method, parameter);
                }
            }
            if (expression!=null)
            {
                act = (list, p1, srcPropertyInfo, func) =>
                {
                    list.Add(Expression.Assign(Expression.MakeMemberAccess(p1, srcPropertyInfo), func == null ? expression : func(expression)));
                };
                return true;
            }
            bool skipChangeType = false;
            //                      目标类型是否存在TryParse方法————————否——————————ChangeType
            //                          |                                                                 |
            //                          |是                                                               |
            //                          |                       否                                  否    |
            //                      源类型是否为string———————————是否存在ToString方法—————
            //                          |                                           |
            //                          |是                                         |是
            //                          |——————————————————————
            //                          |
            //                          |
            //                      调用目标类型上的TryParse方法
            if ((method = _tryParseDic.GetOrAdd(destType, t => t.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), t.MakeByRefType() }, new ParameterModifier[0]))) != null)
            {
                skipChangeType = true;
                MethodInfo toStringMethod = null;
                //首先判断sourceType是不是string
                var r = typeCode != TypeCode.String;
                if (r)
                {
                    //如果类型不是string，就需要看该类是否定义了自己的ToString方法
                    //BindingFlags.DeclaredOnly 标识只获取该类定义的方法，屏蔽继承的方法
                    if ((toStringMethod = _toStringDic.GetOrAdd(sourceType, t => t.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, new ParameterModifier[0]))) == null)
                    {
                        skipChangeType = false;
                    }
                }
                if (skipChangeType)
                {
                    Expression temp = parameter;
                    //如果sourceType不是string
                    if (r)
                    {
                        temp = Expression.Call(parameter, toStringMethod);
                    }
                    ParameterExpression p = VariableGenerator.Generate(destType);
                    expression = Expression.Call(null, method, temp, p);
                    act = (list, p1, srcPropertyInfo, func) =>
                    {
                        expression = Expression.IfThen(expression, Expression.Assign(Expression.MakeMemberAccess(p1, srcPropertyInfo), p));
                        list.Add(expression);
                    };
                    variable = p;
                }
            }
            if (!skipChangeType)
            {
                expression = Expression.Convert(Expression.Call(null, ChangeTypeMethod, Expression.Convert(parameter, typeof(object)), Expression.Constant(Type.GetTypeCode(destType), typeof(TypeCode))), destType);
                act = (list, p1, srcPropertyInfo, func) =>
                {
                    list.Add(Expression.Assign(Expression.MakeMemberAccess(p1, srcPropertyInfo), func == null ? expression : func(expression)));
                };
            }
            return true;
        }
    }
}
