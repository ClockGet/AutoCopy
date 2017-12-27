using System;
using System.Linq.Expressions;

namespace AutoCopyLib.Visitors
{
    public class ReturnTargetRewriter : ExpressionVisitor
    {
        private LabelTarget _returnTarget;
        private Expression _defaultValue;
        private Expression _ifTure;
        public ReturnTargetRewriter(Type returnType, Expression defaultValue)
        {
            _returnTarget = Expression.Label(returnType);
            _defaultValue = defaultValue;
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_ifTure == null && node.Type == _returnTarget.Type)
                _ifTure = node;
            return node;
        }
        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return _returnTarget;
        }
        protected override Expression VisitLabel(LabelExpression node)
        {
            return node.Update(this.VisitLabelTarget(node.Target), _defaultValue);
        }
        protected override Expression VisitGoto(GotoExpression node)
        {
            var constant = node.Value as ConstantExpression;
            Expression value = _defaultValue;
            if (constant != null && (bool)constant.Value)
                value = _ifTure;
            return node.Update(this.VisitLabelTarget(node.Target), value);
        }
        public LabelTarget LabelTarget
        {
            get
            {
                return _returnTarget;
            }
        }
        public Expression DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }
    }
}
