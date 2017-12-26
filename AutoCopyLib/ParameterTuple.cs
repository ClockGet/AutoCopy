using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoCopyLib
{
    public partial class AutoCopy<TSource, TDest>
    {
        public class ParameterTuple
        {
            public ParameterTuple(ParameterExpression p1, ParameterExpression p2, ParameterExpression p3)
            {
                Destination = p1;
                Source = p2;
                ErrorMsg = p3;
            }
            public ParameterExpression Destination { get; }
            public ParameterExpression Source { get; }
            public ParameterExpression ErrorMsg { get; }
            public IEnumerable<ParameterExpression> Collect()
            {
                yield return Destination;
                yield return Source;
                yield return ErrorMsg;
            }
        }
    }
}
