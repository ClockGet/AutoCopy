using System.Linq.Expressions;

namespace AutoCopyLib
{
    public partial class AutoCopy<TSource, TDest>
    {
        internal class GetTryBodyRewriter:ExpressionVisitor
        {
            private Expression _body;
            private Expression _node;
            public GetTryBodyRewriter(Expression node)
            {
                _node = node;
                Visit(_node);
            }
            protected override Expression VisitTry(TryExpression node)
            {
                _body = node.Body;
                return base.VisitTry(node);
            }
            public Expression Body
            {
                get
                {
                    return _body;
                }
            }
        }
    }
}
