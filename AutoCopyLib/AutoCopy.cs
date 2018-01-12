using AutoCopyLib.Attributes;
using AutoCopyLib.Visitors;
using DelegateDecompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace AutoCopyLib
{
    public class AutoCopy
    {
        public static AutoCopy<TSource, TDest> CreateMap<TSource, TDest>()
        {
            return new AutoCopy<TSource, TDest>();
        }
    }
    public partial class AutoCopy<TSource, TDest>
    {
        private static ConstantExpression ReturnTrue = Expression.Constant(true, typeof(bool));
        private static ConstantExpression ReturnFalse = Expression.Constant(false, typeof(bool));
        delegate bool RefFunc(ref TDest dest, TSource src, out string errMsg);
        internal AutoCopy()
        {
            var p1 = Expression.Parameter(typeof(TDest).MakeByRefType(), "dest");
            var p2 = Expression.Parameter(typeof(TSource), "src");
            var p3 = Expression.Parameter(typeof(string).MakeByRefType(), "errMsg");
            parameterTuple = new ParameterTuple(p1, p2, p3);
            opt = new Option<TSource>(parameterTuple);
        }
        private RefFunc m_func;
        private Option<TSource> opt = null;
        private ParameterTuple parameterTuple = null;
        private Dictionary<PropertyInfo, Expression> _map = new Dictionary<PropertyInfo, Expression>();
        private NullsafeQueryRewriter nullsafeQueryRewriter = new NullsafeQueryRewriter();
        private Func<Expression, Expression> nullsafeQueryFunc = null;
        private Dictionary<TypeTuple, MethodInfo> _typeConvertMap = new Dictionary<TypeTuple, MethodInfo>();

        public AutoCopy<TSource, TDest> ForMember<TValue>(Expression<Func<TDest, TValue>> dest, Func<Option<TSource>, Expression> src)
        {
            //Expression body = src;
            //if(body is LambdaExpression)
            //{
            //    body = ((LambdaExpression)body).Body;
            //}
            //if(body.NodeType!= ExpressionType.MemberAccess)
            //{
            //    throw new InvalidOperationException();
            //}
            //var p = (PropertyInfo)((MemberExpression)body).Member;
            var p = FindProperty(dest) as PropertyInfo;
            var expression = src(opt);
            _map[p] = expression;
            return this;
        }
        public AutoCopy<TSource, TDest> ForMember<TValue>(Expression<Func<TDest, TValue>> dest, Func<Option<TSource>, Func<TSource, TValue>> src)
        {
            var p = FindProperty(dest) as PropertyInfo;
            var func = src(opt);
            Expression expression = null;
            try
            {
                var exp = func.Decompile();
                expression = ParameterReplacer.Replace(exp.Body, new ParameterExpression[] { parameterTuple.Source }, new ParameterExpression[] { parameterTuple.Source });
            }
            catch
            {
                expression = Expression.Call(Expression.Constant(func.Target), func.Method, parameterTuple.Source);
            }

            _map[p] = expression;
            return this;
        }
        public AutoCopy<TSource, TDest> ForTypeConvert<T1, T2>(Expression<Func<T1, T2>> converter)
        {
            Type src = typeof(T1);
            Type dst = typeof(T2);
            if (src == dst)
                throw new Exception($"{src.Name}与{dst.Name}类型相同");
            TypeTuple tuple = new TypeTuple(src, dst);
            if (_typeConvertMap.ContainsKey(tuple))
                throw new Exception($"已经存在{src.Name}到{dst.Name}的转换");
            Expression body = converter;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
                if (body == null)
                    throw new Exception("lambda表达式不能为空");
                var method = body as MethodCallExpression;
                MethodInfo methodInfo = null;
                if (method == null)
                {
                    methodInfo = converter.Compile().Method;
                }
                else
                {
                    methodInfo = method.Method;
                }
                if (methodInfo == null)
                    throw new Exception("获取不到lambda表达式中调用的方法");
                _typeConvertMap.Add(tuple, methodInfo);
            }
            else
            {
                throw new Exception("不是合法的lambda表达式");
            }
            return this;
        }
        private LambdaExpression RegisterCore(bool enableNullSafe)
        {
            if (this.Lambda == null)
            {
                nullsafeQueryFunc = (exp) => enableNullSafe ? nullsafeQueryRewriter.visit(exp) : exp;
                var propertyInfos = typeof(TDest).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var destPropertyInfos = propertyInfos.Where(p => p.GetCustomAttribute<CopyIgnoreAttribute>(true) == null);
                hasReturnLabel = propertyInfos.Where(p => p.GetCustomAttribute<CopyRequiredAttribute>(true) != null).FirstOrDefault() != null;
                LabelTarget returnTarget = Expression.Label(typeof(bool));
                //var dstPropertyInfos = typeof(D).GetProperties(BindingFlags.Instance | BindingFlags.Public);

                List<Expression> list = new List<Expression>();
                List<Expression> initExprs = new List<Expression>();
                List<ParameterExpression> variables = new List<ParameterExpression>();
                initExprs.Add(Expression.Assign(parameterTuple.ErrorMsg, Expression.Constant(string.Empty)));
                initExprs.Add(Expression.Assign(parameterTuple.Destination, Expression.Coalesce(parameterTuple.Destination, Expression.New(typeof(TDest)))));
                foreach (var destPropertyInfo in destPropertyInfos)
                {
                    if (!destPropertyInfo.CanWrite)
                    {
                        continue;
                    }
                    Expression exp, test = null;
                    ParameterExpression @var;
                    bool ifTrue = false;
                    string sourceName = destPropertyInfo.GetCustomAttribute<CopyMapAttribute>()?.MapName ?? destPropertyInfo.Name;
                    if (!_map.TryGetValue(destPropertyInfo, out exp))
                    {
                        if (!Provider.TryGetExpression(sourceName, parameterTuple.Source, destPropertyInfo.PropertyType, out exp, out @var, out test, out ifTrue))
                        {
                            continue;
                        }
                        if (@var != null)
                        {
                            variables.Add(@var);
                        }
                    }
                    if (destPropertyInfo.PropertyType != exp.Type)
                    {
                        TypeTuple typeTuple = new TypeTuple(exp.Type, destPropertyInfo.PropertyType);
                        MethodInfo m;
                        if (_typeConvertMap.TryGetValue(typeTuple, out m))
                        {
                            exp = Expression.Call(m, exp);
                            list.Add(Expression.Assign(Expression.MakeMemberAccess(parameterTuple.Destination, destPropertyInfo), nullsafeQueryFunc(exp)));
                        }
                        else
                        {
                            Action<List<Expression>, ParameterExpression, PropertyInfo, Func<Expression, Expression>> act;
                            ParameterExpression variable;
                            //ConvertType
                            if (!TypeConverter.TryConvert(exp, exp.Type, destPropertyInfo.PropertyType, out act, out variable))
                            {
                                continue;
                            }
                            if (act == null)
                                continue;
                            act(list, parameterTuple.Destination, destPropertyInfo, nullsafeQueryFunc);
                            if (variable != null)
                            {
                                variables.Add(variable);
                            }
                        }
                    }
                    else
                    {
                        exp=ParameterReplacer.Replace(exp, new[] { Expression.Parameter(typeof(string), "name") }, new Expression[] {Expression.Constant(sourceName) });
                        list.Add(Expression.Assign(Expression.MakeMemberAccess(parameterTuple.Destination, destPropertyInfo), nullsafeQueryFunc(exp)));
                    }
                    //表示表达式需要进行ifTrue的判断
                    if (ifTrue && test != null)
                    {
                        //对当前表达式进行包装
                        var tempExp = list[list.Count - 1];
                        tempExp = Expression.IfThen(test, tempExp);
                        list[list.Count - 1] = tempExp;
                    }
                    //check if the property has the CopyRequiredAttribute
                    var copyRequired = destPropertyInfo.GetCustomAttribute<CopyRequiredAttribute>();
                    if (copyRequired != null)
                    {
                        var tempExp = list[list.Count - 1];
                        Expression falseBody = Expression.Block(
                            Expression.Assign(parameterTuple.ErrorMsg, Expression.Constant(string.Format(copyRequired.MsgFormat, sourceName))),
                            Expression.Return(returnTarget, ReturnFalse)
                        );
                        tempExp = new ConditionFalseRewriter(falseBody).Visit(tempExp);
                        list[list.Count - 1] = tempExp;
                    }
                }
                if (!hasReturnLabel)
                    list.Add(ReturnTrue);
                else
                    list.Add(Expression.Return(returnTarget, ReturnTrue));
                BlockExpression body = null;
                if (variables.Count == 0)
                    body = Expression.Block(list);
                else
                    body = Expression.Block(variables, list);
                this.Body = body;
                var parExcep = Expression.Parameter(typeof(Exception), "ex");
                var catchBlock = Expression.Block(
                    Expression.Assign(parameterTuple.ErrorMsg, Expression.MakeMemberAccess(parExcep, Utilities.StaticReflection.GetMemberInfo((Exception ex) => ex.Message))),
                    !hasReturnLabel ? (Expression)ReturnFalse : (Expression)Expression.Return(returnTarget, ReturnFalse)
                    );
                var body2 = Expression.TryCatch(body, Expression.Catch(parExcep, catchBlock));

                var lambdaExpression = Expression.Lambda<RefFunc>(Expression.Block(initExprs.Concat(!hasReturnLabel ? new Expression[] { body2 } : new Expression[] { body2, Expression.Label(returnTarget, ReturnFalse) })), parameterTuple.Collect());

                this.Lambda = lambdaExpression;
            }
            return this.Lambda;
        }
        public void Register(bool enableNullSafe = false)
        {
            var lambdaExpression = RegisterCore(enableNullSafe);
            try
            {
                IsFastMode = true;
                var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
              new AssemblyName("AutoCopyAssembly_" + Guid.NewGuid().ToString("N")),
              AssemblyBuilderAccess.Run);

                var moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");

                var typeBuilder = moduleBuilder.DefineType("AutoCopy_" + Guid.NewGuid().ToString("N"),
                  TypeAttributes.Public);

                var methodBuilder = typeBuilder.DefineMethod("ShallowCopy",
                  MethodAttributes.Public | MethodAttributes.Static);
                lambdaExpression.CompileToMethod(methodBuilder);

                var resultingType = typeBuilder.CreateType();

                var function = Delegate.CreateDelegate(lambdaExpression.Type,
                  resultingType.GetMethod("ShallowCopy"));
                m_func = function as RefFunc;
            }
            catch
            {
                //当使用ResolveUsing方法时，如果lambda表达式含有语句体，则try中编译失败；
                //还有一种情况是使用MapFrom方法时，里面调用了外部变量的方法，则try中依然会编译失败
                IsFastMode = false;
                m_func = lambdaExpression.Compile() as RefFunc;
            }


            //m_func = Expression.Lambda<RefFunc>(body2, p1, p2, p3).Compile();
            //m_func = function as RefFunc;
        }
        public bool ShallowCopy(TSource src, TDest dst)
        {
            string errMsg;
            return m_func(ref dst, src, out errMsg);
        }
        public TDest Map(TSource source)
        {
            TDest t = default(TDest);
            string errMsg;
            if (!m_func(ref t, source, out errMsg))
            {
                throw new Exception(errMsg);
            }
            return t;
        }
        public bool TryMap(TSource source, out TDest dest, out string errMsg)
        {
            errMsg = string.Empty;
            dest = default(TDest);
            return m_func(ref dest, source, out errMsg);
        }
        public TargetExpressionProviderBase Provider { get; set; } = new DefaultPropertyExpressionProvider(typeof(TSource));
        public LambdaExpression Lambda
        {
            get;
            private set;
        }
        public BlockExpression Body
        {
            get;
            private set;
        }
        public bool IsFastMode
        {
            get;
            private set;
        }
        internal bool hasReturnLabel
        {
            get;
            private set;
        }
    }
}
