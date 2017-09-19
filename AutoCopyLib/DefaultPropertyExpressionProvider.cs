using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoCopyLib
{
    public sealed class DefaultPropertyExpressionProvider : TargetExpressionProviderBase
    {
        public override bool TryGetExpression(string name, ParameterExpression parameter, Type destType, out Expression exp, out ParameterExpression variable, out Expression test, out bool ifTrue)
        {
            exp = null;
            test = null;
            variable = null;
            ifTrue = false;
            var dstPropertyInfo = _infos.Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (dstPropertyInfo == null || !dstPropertyInfo.CanRead)
                return false;
            exp = Expression.MakeMemberAccess(parameter, dstPropertyInfo);
            return true;
        }
        private PropertyInfo[] _infos;

        public DefaultPropertyExpressionProvider(Type targetType) : base(targetType)
        {
            _infos = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}
