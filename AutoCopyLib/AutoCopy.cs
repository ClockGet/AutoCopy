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
    public partial class AutoCopy<T, D>
    {
        delegate bool RefFunc(ref T src, D des, AutoCopy<T, D> proxy);
        private Exception exception = null;
        internal AutoCopy()
        {
            p1 = Expression.Parameter(typeof(T).MakeByRefType(), "src");
            p2 = Expression.Parameter(typeof(D), "dst");
            p3 = Expression.Parameter(this.GetType(), "proxy");
            opt = new Option<D>(p2);
        }
        private RefFunc m_func;
        private Option<D> opt = null;
        ParameterExpression p1 = null;
        ParameterExpression p2 = null;
        ParameterExpression p3 = null;
        private Dictionary<PropertyInfo, Expression> _map = new Dictionary<PropertyInfo, Expression>();
        private NullsafeQueryRewriter nullsafeQueryRewriter = new NullsafeQueryRewriter();
        private Func<Expression, Expression> nullsafeQueryFunc = null;
        private Dictionary<TypeTuple, MethodInfo> _typeConvertMap = new Dictionary<TypeTuple, MethodInfo>();

        public AutoCopy<T, D> ForMember<TValue>(Expression<Func<T, TValue>> src, Func<Option<D>, Expression> dst)
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
            var p = FindProperty(src) as PropertyInfo;
            var expression = dst.Invoke(opt);
            _map[p] = expression;
            return this;
        }
        public AutoCopy<T, D> ForMember<TValue>(Expression<Func<T, TValue>> src, Func<Option<D>, Func<D, TValue>> dst)
        {
            var p = FindProperty(src) as PropertyInfo;
            var func = dst(opt);
            Expression expression = null;
            try
            {
                var exp = func.Decompile();
                expression = ParameterReplacer.Replace(exp.Body, p2, p2);
            }
            catch
            {
                expression = Expression.Call(Expression.Constant(func.Target), func.Method, p2);
            }

            _map[p] = expression;
            return this;
        }
        public AutoCopy<T, D> ForTypeConvert<T1, T2>(Expression<Func<T2, T1>> converter)
        {
            Type src = typeof(T1);
            Type dst = typeof(T2);
            if (src == dst)
                throw new Exception($"{src.Name}与{dst.Name}类型相同");
            TypeTuple tuple = new TypeTuple(src, dst);
            if (_typeConvertMap.ContainsKey(tuple))
                throw new Exception($"已经存在{dst.Name}到{src.Name}的转换");
            Expression body = converter;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
                if (body == null)
                    throw new Exception("lambda表达式不能为空");
                var method = body as MethodCallExpression;
                if (method == null)
                    throw new Exception("获取不到lambda表达式中调用的方法");
                _typeConvertMap.Add(tuple, method.Method);
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
                var srcPropertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.GetCustomAttributes(typeof(CopyIgnoreAttribute), true).Length == 0);
                //var dstPropertyInfos = typeof(D).GetProperties(BindingFlags.Instance | BindingFlags.Public);

                List<Expression> list = new List<Expression>();
                List<ParameterExpression> variables = new List<ParameterExpression>();
                list.Add(Expression.Assign(p1, Expression.Coalesce(p1, Expression.New(typeof(T)))));
                foreach (var srcPropertyInfo in srcPropertyInfos)
                {
                    if (!srcPropertyInfo.CanWrite)
                    {
                        continue;
                    }
                    Expression exp, test = null;
                    ParameterExpression @var;
                    bool ifTrue = false;
                    if (!_map.TryGetValue(srcPropertyInfo, out exp))
                    {
                        if (!Provider.TryGetExpression(srcPropertyInfo.Name, p2, srcPropertyInfo.PropertyType, out exp, out @var, out test, out ifTrue))
                        {
                            continue;
                        }
                        if (@var != null)
                        {
                            variables.Add(@var);
                        }
                    }
                    if (srcPropertyInfo.PropertyType != exp.Type)
                    {
                        TypeTuple typeTuple = new TypeTuple(srcPropertyInfo.PropertyType, exp.Type);
                        MethodInfo m;
                        if (_typeConvertMap.TryGetValue(typeTuple, out m))
                        {
                            exp = Expression.Call(exp, m);
                            list.Add(Expression.Assign(Expression.MakeMemberAccess(p1, srcPropertyInfo), nullsafeQueryFunc(exp)));
                        }
                        else
                        {
                            Action<List<Expression>, ParameterExpression, PropertyInfo, Func<Expression, Expression>> act;
                            ParameterExpression variable;
                            //ConvertType
                            if (!TypeConverter.TryConvert(exp, exp.Type, srcPropertyInfo.PropertyType, out act, out variable))
                            {
                                continue;
                            }
                            if (act == null)
                                continue;
                            act(list, p1, srcPropertyInfo, nullsafeQueryFunc);
                            if (variable != null)
                            {
                                variables.Add(variable);
                            }
                        }
                    }
                    else
                    {
                        list.Add(Expression.Assign(Expression.MakeMemberAccess(p1, srcPropertyInfo), nullsafeQueryFunc(exp)));
                    }
                    //表示表达式需要进行ifTrue的判断
                    if (ifTrue && test != null)
                    {
                        //对当前表达式进行包装
                        var tempExp = list[list.Count - 1];
                        tempExp = Expression.IfThen(test, tempExp);
                        list[list.Count - 1] = tempExp;
                    }
                }
                list.Add(Expression.Constant(true, typeof(bool)));
                BlockExpression body = null;
                if (variables.Count == 0)
                    body = Expression.Block(list);
                else
                    body = Expression.Block(variables, list);
                this.Body = body;
                var parExcep = Expression.Parameter(typeof(Exception), "ex");
                var catchBlock = Expression.Block(
                    Expression.Call(p3, this.GetType().GetMethod(nameof(this.SetException), BindingFlags.Instance | BindingFlags.Public), parExcep),
                    Expression.Constant(false, typeof(bool))
                    );
                var body2 = Expression.TryCatch(body, Expression.Catch(parExcep, catchBlock));

                var lambdaExpression = Expression.Lambda<RefFunc>(body2, p1, p2, p3);

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
        public bool ShallowCopy(T src, D dst)
        {
            return m_func(ref src, dst, this);
        }
        public T Map(D dst)
        {
            T t = default(T);
            bool r = m_func(ref t, dst, this);
            return t;
        }
        public TargetExpressionProviderBase Provider { get; set; } = new DefaultPropertyExpressionProvider(typeof(D));
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
        public void SetException(Exception ex)
        {
            exception = ex;
        }
        public Exception GetLastError()
        {
            return exception;
        }
        public bool IsFastMode
        {
            get;
            private set;
        }
    }
}
