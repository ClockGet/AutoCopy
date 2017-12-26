using System;
using System.Linq.Expressions;

namespace AutoCopyLib.Utilities
{
    public static class VariableGenerator
    {
        private static int index = 0;
        public static ParameterExpression Generate(Type type)
        {
            return Expression.Variable(type, $"var{++index}");
        }
    }
}
