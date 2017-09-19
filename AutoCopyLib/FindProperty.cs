using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoCopyLib
{
    public partial class AutoCopy<T, D>
    {
        #region FindProperty
        internal static MemberInfo FindProperty(LambdaExpression lambdaExpression)
        {
            Expression expression = lambdaExpression;
            bool flag = false;
            while (!flag)
            {
                ExpressionType nodeType = expression.NodeType;
                if (nodeType != ExpressionType.Convert)
                {
                    if (nodeType != ExpressionType.Lambda)
                    {
                        if (nodeType != ExpressionType.MemberAccess)
                        {
                            flag = true;
                        }
                        else
                        {
                            MemberExpression memberExpression = (MemberExpression)expression;
                            if (memberExpression.Expression.NodeType != ExpressionType.Parameter && memberExpression.Expression.NodeType != ExpressionType.Convert)
                            {
                                throw new ArgumentException(string.Format("Expression '{0}' must resolve to top-level member and not any child object's properties.", lambdaExpression), "lambdaExpression");
                            }
                            return memberExpression.Member;
                        }
                    }
                    else
                    {
                        expression = ((LambdaExpression)expression).Body;
                    }
                }
                else
                {
                    expression = ((UnaryExpression)expression).Operand;
                }
            }
            throw new Exception("Custom configuration for members is only supported for top-level individual members on a type.");
        }
        #endregion
    }
}
