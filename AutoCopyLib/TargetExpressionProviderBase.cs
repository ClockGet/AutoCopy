using System;
using System.Linq.Expressions;

namespace AutoCopyLib
{
    public abstract class TargetExpressionProviderBase
    {
        public TargetExpressionProviderBase(Type targetType)
        {
            this._target = targetType;
        }
        private Type _target;
        protected Type Target
        {
            get
            {
                return _target;
            }
        }
        public abstract bool TryGetExpression(string name, ParameterExpression parameter, Type destType, out Expression exp, out ParameterExpression variable, out Expression test, out bool ifTrue);
    }
}
