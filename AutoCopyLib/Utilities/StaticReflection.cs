//   Original C# code written by
//   Unity - https://github.com/unitycontainer/unity
//	 Copyright (C) 2015-2017 Microsoft
// 
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this product except in 
//   compliance with the License. You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software distributed under the License is 
//   distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//   See the License for the specific language governing permissions and limitations under the License.
//

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoCopyLib.Utilities
{
    public static class StaticReflection
    {
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return StaticReflection.GetMethodInfo((LambdaExpression)expression);
        }

        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return StaticReflection.GetMethodInfo((LambdaExpression)expression);
        }

        private static MethodInfo GetMethodInfo(LambdaExpression lambda)
        {
            StaticReflection.GuardProperExpressionForm(lambda.Body);
            return ((MethodCallExpression)lambda.Body).Method;
        }

        public static MethodInfo GetPropertyGetMethodInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            MethodInfo getMethod = StaticReflection.GetPropertyInfo<T, TProperty>(expression).GetGetMethod(true);
            if (getMethod == (MethodInfo)null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return getMethod;
        }

        public static MethodInfo GetPropertySetMethodInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            MethodInfo setMethod = StaticReflection.GetPropertyInfo<T, TProperty>(expression).GetSetMethod(true);
            if (setMethod == (MethodInfo)null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return setMethod;
        }

        private static PropertyInfo GetPropertyInfo<T, TProperty>(LambdaExpression lambda)
        {
            MemberExpression obj = lambda.Body as MemberExpression;
            if (obj == null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            PropertyInfo obj2 = obj.Member as PropertyInfo;
            if (obj2 == (PropertyInfo)null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return obj2;
        }

        public static MemberInfo GetMemberInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            Guard.ArgumentNotNull(expression, "expression");
            MemberExpression obj = expression.Body as MemberExpression;
            if (obj == null)
            {
                throw new InvalidOperationException("invalid expression form passed");
            }
            MemberInfo member = obj.Member;
            if (member == (MemberInfo)null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return member;
        }

        public static ConstructorInfo GetConstructorInfo<T>(Expression<Func<T>> expression)
        {
            Guard.ArgumentNotNull(expression, "expression");
            NewExpression obj = expression.Body as NewExpression;
            if (obj == null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return obj.Constructor;
        }

        private static void GuardProperExpressionForm(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Call)
            {
                return;
            }
            throw new InvalidOperationException("Invalid expression form passed");
        }
    }
}
