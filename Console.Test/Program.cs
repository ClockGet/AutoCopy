using AutoCopyLib;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Web;

namespace GetPropertyInfoViaPath
{
    #region 测试类(弃用)
    //public class Address
    //{
    //    public string ZipCode { get; set; }
    //}
    //public class Telephone
    //{
    //    public string Number { get; set; }
    //}

    //public class Customer
    //{
    //    public Address Address { get; set; }
    //    public Telephone Phone { get; set; }
    //    public string Memo { get; set; }
    //}
    //public class CustomerInfo
    //{
    //    public string zipCode { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string Memo { get; set; }
    //}
    #endregion
    #region 自定义NameValueCollection类
    public class HttpQueryCollection : NameValueCollection
    {
        private static int MaxHttpCollectionKeys = 1000;

        public HttpQueryCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public HttpQueryCollection(int capacity) : base(capacity, StringComparer.OrdinalIgnoreCase)
        {
        }

        protected HttpQueryCollection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HttpQueryCollection(string querystr, bool urlencoded)
        {
            if (!string.IsNullOrEmpty(querystr))
            {
                this.FillFromString(querystr, urlencoded, Encoding.Default);
            }
            base.IsReadOnly = true;
        }

        public HttpQueryCollection(string str, bool readOnly, bool urlencoded, Encoding encoding) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this.FillFromString(str, urlencoded, encoding);
            }
            base.IsReadOnly = readOnly;
        }

        public static bool SplitHttpUrl(string httpurl, out string url, out string queryString)
        {
            url = "";
            queryString = "";
            if (string.IsNullOrEmpty(httpurl))
            {
                return false;
            }
            int num = httpurl.IndexOf('?');
            url = httpurl;
            if (num > 0)
            {
                url = httpurl.Substring(0, num);
                queryString = httpurl.Substring(num + 1, httpurl.Length - num - 1);
            }
            return true;
        }

        public void Add(HttpCookieCollection c)
        {
            int count = c.Count;
            for (int i = 0; i < count; i++)
            {
                this.ThrowIfMaxHttpCollectionKeysExceeded();
                HttpCookie httpCookie = c.Get(i);
                base.Add(httpCookie.Name, httpCookie.Value);
            }
        }

        public void FillFromEncodedBytes(byte[] bytes, Encoding encoding)
        {
            int num = (bytes != null) ? bytes.Length : 0;
            for (int i = 0; i < num; i++)
            {
                this.ThrowIfMaxHttpCollectionKeysExceeded();
                int num2 = i;
                int num3 = -1;
                while (i < num)
                {
                    byte b = bytes[i];
                    if (b == 61)
                    {
                        if (num3 < 0)
                        {
                            num3 = i;
                        }
                    }
                    else if (b == 38)
                    {
                        break;
                    }
                    i++;
                }
                string name;
                string value;
                if (num3 >= 0)
                {
                    name = HttpUtility.UrlDecode(bytes, num2, num3 - num2, encoding);
                    value = HttpUtility.UrlDecode(bytes, num3 + 1, i - num3 - 1, encoding);
                }
                else
                {
                    name = null;
                    value = HttpUtility.UrlDecode(bytes, num2, i - num2, encoding);
                }
                base.Add(name, value);
                if (i == num - 1 && bytes[i] == 38)
                {
                    base.Add(null, string.Empty);
                }
            }
        }

        public void FillFromString(string s)
        {
            this.FillFromString(s, false, null);
        }

        public void FillFromString(string s, bool urlencoded, Encoding encoding)
        {
            int num = (s != null) ? s.Length : 0;
            for (int i = 0; i < num; i++)
            {
                this.ThrowIfMaxHttpCollectionKeysExceeded();
                int num2 = i;
                int num3 = -1;
                while (i < num)
                {
                    char c = s[i];
                    if (c == '=')
                    {
                        if (num3 < 0)
                        {
                            num3 = i;
                        }
                    }
                    else if (c == '&')
                    {
                        break;
                    }
                    i++;
                }
                string text = null;
                string text2;
                if (num3 >= 0)
                {
                    text = s.Substring(num2, num3 - num2);
                    text2 = s.Substring(num3 + 1, i - num3 - 1);
                }
                else
                {
                    text2 = s.Substring(num2, i - num2);
                }
                if (urlencoded)
                {
                    base.Add(HttpUtility.UrlDecode(text, encoding), HttpUtility.UrlDecode(text2, encoding));
                }
                else
                {
                    base.Add(text, text2);
                }
                if (i == num - 1 && s[i] == '&')
                {
                    base.Add(null, string.Empty);
                }
            }
        }

        public void MakeReadOnly()
        {
            base.IsReadOnly = true;
        }

        public void MakeReadWrite()
        {
            base.IsReadOnly = false;
        }

        public void Reset()
        {
            base.Clear();
        }

        private void ThrowIfMaxHttpCollectionKeysExceeded()
        {
            if (this.Count >= HttpQueryCollection.MaxHttpCollectionKeys)
            {
                throw new InvalidOperationException();
            }
        }

        public override string ToString()
        {
            return this.ToString(true);
        }

        public virtual string ToString(bool urlencoded)
        {
            return this.ToString(urlencoded, null);
        }

        public virtual string ToString(bool urlencoded, IDictionary excludeKeys)
        {
            int count = this.Count;
            if (count == 0)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = excludeKeys != null && excludeKeys["__VIEWSTATE"] != null;
            for (int i = 0; i < count; i++)
            {
                string text = this.GetKey(i);
                if ((!flag || text == null || !text.StartsWith("__VIEWSTATE", StringComparison.Ordinal)) && (excludeKeys == null || text == null || excludeKeys[text] == null))
                {
                    if (urlencoded)
                    {
                        text = HttpUtility.UrlEncodeUnicode(text);
                    }
                    string value = (!string.IsNullOrEmpty(text)) ? (text + "=") : string.Empty;
                    ArrayList arrayList = (ArrayList)base.BaseGet(i);
                    int num = (arrayList != null) ? arrayList.Count : 0;
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append('&');
                    }
                    if (num == 1)
                    {
                        stringBuilder.Append(value);
                        string text2 = (string)arrayList[0];
                        if (urlencoded)
                        {
                            text2 = HttpUtility.UrlEncodeUnicode(text2);
                        }
                        stringBuilder.Append(text2);
                    }
                    else if (num == 0)
                    {
                        stringBuilder.Append(value);
                    }
                    else
                    {
                        for (int j = 0; j < num; j++)
                        {
                            if (j > 0)
                            {
                                stringBuilder.Append('&');
                            }
                            stringBuilder.Append(value);
                            string text2 = (string)arrayList[j];
                            if (urlencoded)
                            {
                                text2 = HttpUtility.UrlEncodeUnicode(text2);
                            }
                            stringBuilder.Append(text2);
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
    #endregion
    #region 自定义参数提供者
    internal class HttpRequestParamsExpressionProvider : TargetExpressionProviderBase
    {
        public HttpRequestParamsExpressionProvider(Type targetType) : base(targetType)
        {
            if (!typeof(NameValueCollection).IsAssignableFrom(targetType))
            {
                throw new ArgumentException(string.Format("{0}不是有效的NameValueCollection派生类", targetType.Name));
            }
        }
        private MethodInfo isNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty");

        public override bool TryGetExpression(string name, ParameterExpression parameter, Type destType, out Expression exp, out ParameterExpression variable, out Expression test, out bool ifTrue)
        {
            exp = null;
            test = null;
            variable = null;
            ifTrue = false;
            try
            {
                exp = Expression.Property(parameter, "Item", Expression.Constant(name, typeof(string)));
                if (destType == typeof(string))
                {
                    test = Expression.Not(Expression.Call(null, isNullOrEmpty, exp));
                    ifTrue = true;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
    #endregion
    #region 测试类
    public class Data
    {
        public int width { get; set; }
        public int height { get; set; }
        public string ua { get; set; }
        public string ip { get; set; }
        public string imei { get; set; }
        public string android_id { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string os { get; set; }
        public string osv { get; set; }
        public int connectionType { get; set; }
        public int deviceType { get; set; }
        public string mac { get; set; }
        public int screenWidth { get; set; }
        public int screenHeight { get; set; }
        public string appName { get; set; }
        public int ppi { get; set; }
        public string dpidsha1 { get; set; }
        public string plmn { get; set; }
        public string orientation { get; set; }
        public int pos { get; set; }
        public bool instl { get; set; }
        public string ver { get; set; }
        public string bundle { get; set; }
        public Ext ext { get; set; }
    }
    public class Ext
    {
        public int ID { get; set; }
    }
    #endregion
    class Program
    {
        #region 测试对照方法
        public static bool TryParse(NameValueCollection dst, out Data src)
        {
            src = new Data();
            try
            {
                int val1, val2, val3, val4, val5, val6, val7, val8;
                bool r;
                if (int.TryParse(dst["width"], out val1))
                {
                    src.width = val1;
                }
                if (int.TryParse(dst["height"], out val2))
                {
                    src.height = val2;
                }

                if (!string.IsNullOrEmpty(dst["ua"]))
                {
                    src.ua = dst["ua"];
                }

                if (!string.IsNullOrEmpty(dst["ip"]))
                {
                    src.ip = dst["ip"];
                }

                if (!string.IsNullOrEmpty(dst["imei"]))
                {
                    src.imei = dst["imei"];
                }

                if (!string.IsNullOrEmpty(dst["android_id"]))
                {
                    src.android_id = dst["android_id"];
                }

                if (!string.IsNullOrEmpty(dst["make"]))
                {
                    src.make = dst["make"];
                }

                if (!string.IsNullOrEmpty(dst["model"]))
                {
                    src.model = dst["model"];
                }

                if (!string.IsNullOrEmpty(dst["os"]))
                {
                    src.os = dst["os"];
                }

                if (!string.IsNullOrEmpty(dst["osv"]))
                {
                    src.osv = dst["osv"];
                }

                if (int.TryParse(
                        dst["connectionType"],
                        out val3))
                {
                    src.connectionType = val3;
                }

                if (int.TryParse(
                        dst["deviceType"],
                        out val4))
                {
                    src.deviceType = val4;
                }

                if (!string.IsNullOrEmpty(dst["mac"]))
                {
                    src.mac = dst["mac"];
                }

                if (int.TryParse(
                        dst["screenWidth"],
                        out val5))
                {
                    src.screenWidth = val5;
                }

                if (int.TryParse(
                        dst["screenHeight"],
                        out val6))
                {
                    src.screenHeight = val6;
                }

                if (!string.IsNullOrEmpty(dst["appName"]))
                {
                    src.appName = dst["appName"];
                }

                if (int.TryParse(
                        dst["ppi"],
                        out val7))
                {
                    src.ppi = val7;
                }

                if (!string.IsNullOrEmpty(dst["dpidsha1"]))
                {
                    src.dpidsha1 = dst["dpidsha1"];
                }

                if (!string.IsNullOrEmpty(dst["plmn"]))
                {
                    src.plmn = dst["plmn"];
                }

                if (!string.IsNullOrEmpty(dst["orientation"]))
                {
                    src.orientation = dst["orientation"];
                }

                if (int.TryParse(
                        dst["pos"],
                        out val8))
                {
                    src.pos = val8;
                }

                if (bool.TryParse(
                        dst["instl"],
                        out r))
                {
                    src.instl = r;
                }

                if (!string.IsNullOrEmpty(dst["ver"]))
                {
                    src.ver = dst["ver"];
                }

                if (!string.IsNullOrEmpty(dst["bundle"]))
                {
                    src.bundle = dst["bundle"];
                }

                return true;
            }
            catch
            {
                src = null;
                return false;
            }
        }
        #endregion
        static void Main(string[] args)
        {
            //初始化NameValueCollection
            string surl = "id=10010&width=10&height=10&ua=ua&ip=127.0.0.1&imei=00000000000000&android_id=A00000000000000&make=1111111111&model=XXX&os=android&osv=4.0.1&connectionType=1&deviceType=1&mac=0.0.0.0.0.0.0&screenWidth=100&screenHeight=100&appName=test&ppi=600&dpidsha1=dpidsha1&plmn=1&orientation=1&pos=1&instl=true&ver=1.0.0&bundle=bundle";
            HttpQueryCollection collection = new HttpQueryCollection(surl, false);

            //初始化AutoCopy
            var ac = AutoCopy.CreateMap<Ext, NameValueCollection>();
            ac.Provider= new HttpRequestParamsExpressionProvider(typeof(NameValueCollection));
            var autoCopy = AutoCopy.CreateMap<Data, NameValueCollection>();
            autoCopy.ForMember(p => p.ext, opt => opt.MapFrom(p=>ac.Map(p)));
            autoCopy.Provider = new HttpRequestParamsExpressionProvider(typeof(NameValueCollection));
            autoCopy.Register();

            //测试开始
            Stopwatch sw = new Stopwatch();
#if !DEBUG
            int loop = 1000000;
#else
            int loop = 10;
#endif
            sw.Start();
            for (int i = 0; i < loop; i++)
            {
                Data data;
                TryParse(collection, out data);
            }
            sw.Stop();
            Console.WriteLine("手写解析方法循环" + loop + "次耗时" + sw.ElapsedMilliseconds + "毫秒");
            sw.Restart();
            for (int i = 0; i < loop; i++)
            {
                Data data;
                data = autoCopy.Map(collection);
            }
            sw.Stop();
            Console.WriteLine("自动解析方法循环" + loop + "次耗时" + sw.ElapsedMilliseconds + "毫秒 fastmode:"+autoCopy.IsFastMode);

            //            var autoCopy = AutoCopy.CreateMap<CustomerInfo, Customer>();
            //            autoCopy
            //                .ForMember(p => p.zipCode, opt => opt.MapFrom(p => p.Address.ZipCode))
            //                .ForMember(p => p.PhoneNumber, opt => opt.MapFrom(p => p.Phone.Number));
            //            autoCopy.Register();
            //#if !DEBUG
            //                        int loop = 10000000;
            //#else
            //            int loop = 10;
            //#endif
            //            Stopwatch sw = new Stopwatch();

            //            sw.Restart();
            //            for (int i = 0; i < loop; i++)
            //            {
            //                Customer customer = new Customer();
            //                CustomerInfo customerInfo = new CustomerInfo();
            //                customer.Address = new Address { ZipCode = "1234567890" };
            //                customer.Phone = new Telephone { Number = "17791704580" };
            //                customer.Memo = "测试默认拷贝";
            //                ShallowCopy(customerInfo, customer);
            //            }
            //            sw.Stop();
            //            Console.WriteLine(sw.ElapsedMilliseconds);

            //            sw.Restart();
            //            for (int i = 0; i < loop; i++)
            //            {
            //                Customer customer = new Customer();
            //                CustomerInfo customerInfo = new CustomerInfo();
            //                customer.Address = new Address { ZipCode = "1234567890" };
            //                customer.Phone = new Telephone { Number = "17791704580" };
            //                customer.Memo = "测试默认拷贝";
            //                autoCopy.ShallowCopy(customerInfo, customer);
            //            }
            //            sw.Stop();
            //            Console.WriteLine(sw.ElapsedMilliseconds);
            //            Customer c = new Customer();
            //            c.Address = new Address { ZipCode = "1234567890" };
            //            c.Phone = new Telephone { Number = "17791704580" };
            //            c.Memo = "测试默认拷贝";
            //            var ci = autoCopy.Map(c);
            //            var propertyInfo = PropertyHelper<Customer>.GetProperty(p => p.Address.ZipCode);
            //            var parameter = Expression.Parameter(typeof(Customer), "c");
            //            var body = Expression.MakeMemberAccess(parameter, propertyInfo);
            //            var func = Expression.Lambda(body, parameter).Compile();
        }
        #region 测试方法(弃用)
        //static bool ShallowCopy(CustomerInfo customerInfo, Customer customer)
        //{
        //    try
        //    {
        //        customerInfo = customerInfo ?? new CustomerInfo();
        //        customerInfo.PhoneNumber = customer.Phone.Number;
        //        customerInfo.zipCode = customer.Address.ZipCode;
        //        customerInfo.Memo = customer.Memo;
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //public static class PropertyHelper<T>
        //{
        //    public static PropertyInfo GetProperty<TValue>(
        //        Expression<Func<T, TValue>> selector)
        //    {
        //        Expression body = selector;
        //        if (body is LambdaExpression)
        //        {
        //            body = ((LambdaExpression)body).Body;
        //        }
        //        switch (body.NodeType)
        //        {
        //            case ExpressionType.MemberAccess:
        //                return (PropertyInfo)((MemberExpression)body).Member;
        //            default:
        //                throw new InvalidOperationException();
        //        }
        //    }
        //}
        #endregion
    }
}
