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
using System.Reflection;

namespace AutoCopyLib.Utilities
{
    internal static class Guard
    {
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue != null)
            {
                return;
            }
            throw new ArgumentNullException(argumentName);
        }

        public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
            if (argumentValue.Length != 0)
            {
                return;
            }
            throw new ArgumentException("The provided string argument must not be empty.", argumentName);
        }

        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            if (assignmentTargetType == (Type)null)
            {
                throw new ArgumentNullException("assignmentTargetType");
            }
            if (assignmentValueType == (Type)null)
            {
                throw new ArgumentNullException("assignmentValueType");
            }
            if (assignmentTargetType.GetTypeInfo().IsAssignableFrom(assignmentValueType.GetTypeInfo()))
            {
                return;
            }
            throw new ArgumentException(string.Format("The type {1} cannot be assigned to variables of type {0}.", new object[2]
            {
            assignmentTargetType,
            assignmentValueType
            }), argumentName);
        }

        public static void InstanceIsAssignable(Type assignmentTargetType, object assignmentInstance, string argumentName)
        {
            if (assignmentTargetType == (Type)null)
            {
                throw new ArgumentNullException("assignmentTargetType");
            }
            if (assignmentInstance == null)
            {
                throw new ArgumentNullException("assignmentInstance");
            }
            if (assignmentTargetType.GetTypeInfo().IsAssignableFrom(assignmentInstance.GetType().GetTypeInfo()))
            {
                return;
            }
            throw new ArgumentException(string.Format("The type {1} cannot be assigned to variables of type {0}.", new object[2]
            {
            assignmentTargetType,
            Guard.GetTypeName(assignmentInstance)
            }), argumentName);
        }

        private static string GetTypeName(object assignmentInstance)
        {
            try
            {
                return assignmentInstance.GetType().FullName;
            }
            catch (Exception)
            {
                return "<unknown>";
            }
        }
    }
}
