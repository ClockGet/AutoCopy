using AutoCopyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataRowTest
{
    internal sealed class DataRowExpressionProvider : TargetExpressionProviderBase
    {
        public DataRowExpressionProvider(Type targetType) : base(targetType)
        {
        }

        public override bool TryGetExpression(string name, ParameterExpression parameter, Type destType, out Expression exp, out ParameterExpression variable, out Expression test, out bool ifTrue)
        {
            exp = null;
            test = null;
            variable = null;
            ifTrue = true;
            try
            {
                exp = Expression.Convert(Expression.Property(parameter, "Item", Expression.Constant(name, typeof(string))),destType);
                test = Expression.Not(Expression.TypeIs(Expression.Property(parameter, "Item", Expression.Constant(name, typeof(string))), typeof(DBNull)));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
