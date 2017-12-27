using System.Linq.Expressions;

namespace AutoCopyLib.Visitors
{
    public class ConditionFalseRewriter : ExpressionVisitor
    {
        private Expression _ifFalse;
        public ConditionFalseRewriter(Expression ifFalse)
        {
            _ifFalse = ifFalse;
        }
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            if (node.IfFalse == null || node.IfFalse.NodeType == ExpressionType.Default)
            {
                return node.Update(this.Visit(node.Test), this.Visit(node.IfTrue), this.Visit(_ifFalse));
            }
            return base.VisitConditional(node);
        }
    }
}
