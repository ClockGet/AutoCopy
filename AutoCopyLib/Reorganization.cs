using AutoCopyLib.Visitors;
using System.Linq;
using System.Linq.Expressions;

namespace AutoCopyLib
{
    public partial class AutoCopy<TSource, TDest>
    {
        public LambdaExpression Decompiler(bool enableNullSafe)
        {
            LambdaExpression lambdaExpression = this.RegisterCore(enableNullSafe);
            var newparameter1 = Expression.Variable(parameterTuple.Destination.Type, "p1");
            var newparameter2 = Expression.Parameter(parameterTuple.Source.Type, "p2");
            var newparameter3 = Expression.Parameter(parameterTuple.ErrorMsg.Type.MakeByRefType(), "p3");
            ParameterTuple newParameterTuple = new ParameterTuple(newparameter1, newparameter2, newparameter3);
            var body = this.Body;
            var newBody= ParameterReplacer.Replace(body, parameterTuple.Collect().ToArray(), newParameterTuple.Collect().ToArray()) as BlockExpression;
            ReturnTargetRewriter returnTargetRewriter = null;
            if (hasReturnLabel)
            {
                returnTargetRewriter = new ReturnTargetRewriter(parameterTuple.Destination.Type, Expression.Default(parameterTuple.Destination.Type));
                newBody = returnTargetRewriter.Visit(newBody) as BlockExpression;
            }
            //初始化临时变量并构造新的lambda表达式
            //去掉原来lambda表达式最后的return true
            var initExpression = Expression.Assign(newparameter1, Expression.New(newparameter1.Type));
            Expression returnExpression = null;
            int exprCount = newBody.Expressions.Count - 1;
            if (hasReturnLabel)
            {
                returnExpression = Expression.Label(returnTargetRewriter.LabelTarget, returnTargetRewriter.DefaultValue);
                ++exprCount;
            }
            else
                returnExpression = newparameter1;
            var newBlockExpression = Expression.Block(new[] { newparameter1 }.Concat(newBody.Variables), new[] { initExpression }.Concat(newBody.Expressions.Take(exprCount)).Concat(new[] { returnExpression }));
            return Expression.Lambda(newBlockExpression, newParameterTuple.Source, newParameterTuple.ErrorMsg);
        }
    }
}
