using AutoCopyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Specialized;
using System.Reflection;

internal class HttpRequestParamsExpressionProvider : TargetExpressionProviderBase
{
    public HttpRequestParamsExpressionProvider(Type targetType) : base(targetType)
    {
        if (!typeof(NameValueCollection).IsAssignableFrom(targetType))
        {
            throw new ArgumentException(string.Format("{0}不是有效的NameValueCollection派生类", targetType.Name));
        }
    }
    private MethodInfo isNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty");

    public override bool TryGetExpression(string name, ParameterExpression parameter, Type destType, out Expression exp, out ParameterExpression variable, out Expression test, out bool ifTrue)
    {
        exp = null;
        test = null;
        variable = null;
        ifTrue = false;
        try
        {
            exp = Expression.Property(parameter, "Item", Expression.Constant(name, typeof(string)));
            if(destType==typeof(string))
            {
                test = Expression.Not(Expression.Call(null, isNullOrEmpty, exp));
                ifTrue = true;
            }
            return true;
        }
        catch
        {
            return false;
        }

    }
}