## AutoCopy

AutoCopy 一个可以缩短开发时间的工具类，帮助程序员从某些繁重的人肉编码中解脱出来。灵感来自于**[AutoMapper](https://github.com/AutoMapper/AutoMapper "AutoMapper")**，初次使用AutoMapper时被它实现的功能深深吸引，但是在逐渐学习中发现AutoMapper速度不能令人满意，并且实现有些复杂，小白不容易看懂原理，所以萌生了自己写一个更简单、更高效类库的想法。在这样一个契机之下，AutoCopy应运而生（:clap::smirk:）。AutoCopy的一些方法模仿了AutoMapper的命名和使用方式，降低使用上的难度。

## 依赖

* **[Mono.Reflection.dll](https://github.com/jbevain/mono.reflection "Mono.Reflection")**
* **[DelegateDecompiler.dll](https://github.com/hazzik/DelegateDecompiler "DelegateDecompiler")**

## 特性

1. 执行速度快
2. 基于抽象类**TargetExpressionProviderBase**可以实现任意扩展
3. 支持自动/手工的类型转换
4. 支持多AutoCopy实例嵌套

## 原理说明

在调用**Register**方法时基于[Reflection](https://msdn.microsoft.com/en-us/library/system.reflection(v=vs.110).aspx "Reflection")分析源类和目标类的所有属性，并生成[Expression](https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression(v=vs.110).aspx "Expression")列表，之后使用[Expression.Lambda](https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression.lambda(v=vs.110).aspx "Expression.Lambda")方法把Expression列表以及相应参数包装成[LambdaExpression](https://msdn.microsoft.com/en-us/library/system.linq.expressions.lambdaexpression(v=vs.110).aspx "LambdaExpression")，通过调用Compile方法编译为[Func Delegate](https://msdn.microsoft.com/en-us/library/bb549151(v=vs.110).aspx "Func Delegate")。

## 示例

### 示例1 相同类型对象拷贝

```csharp

    public class Address
    {
        public string ZipCode { get; set; }
    }

    var autoCopy = AutoCopy.CreateMap<Address, Address>();

    autoCopy.Register();

    Address a=new Address { ZipCode="1234567890"; };

    Address b=autoCopy.Map(a);

```

### 示例2 不同类型对象拷贝

```csharp

    public class Address
    {
        public string ZipCode { get; set; }
    }

    public class Telephone
    {
        public string Number { get; set; }
    }

    public class Customer
    {
        public Address Address { get; set; }
        public Telephone Phone { get; set; }
        public string Memo { get; set; }
    }

    public class CustomerInfo
    {
        public string zipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Memo { get; set; }
    }

    var autoCopy = AutoCopy.CreateMap<CustomerInfo, Customer>();

    autoCopy
        .ForMember(p => p.zipCode, opt => opt.MapFrom(p => p.Address.ZipCode))
        .ForMember(p => p.PhoneNumber, opt => opt.MapFrom(p => p.Phone.Number));

    autoCopy.Register();

    Customer customer = new Customer();
    
    customer.Address = new Address { ZipCode = "1234567890" };
    
    customer.Phone = new Telephone { Number = "17791704580" };
    
    customer.Memo = "Test";
    
    CustomerInfo info = autoCopy.Map(customer);

```

### 示例3 多AutoCopy示例嵌套

```csharp

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

    string surl = "id=10010&width=10&height=10&ua=ua&ip=127.0.0.1&imei=00000000000000&android_id=A00000000000000&make=1111111111&model=XXX&os=android&osv=4.0.1&connectionType=1&deviceType=1&mac=0.0.0.0.0.0.0&screenWidth=100&screenHeight=100&appName=test&ppi=600&dpidsha1=dpidsha1&plmn=1&orientation=1&pos=1&instl=true&ver=1.0.0&bundle=bundle";

    HttpQueryCollection collection = new HttpQueryCollection(surl, false);

    var ac = AutoCopy.CreateMap<Ext, NameValueCollection>();

    ac.Provider= new HttpRequestParamsExpressionProvider(typeof(NameValueCollection));

    var autoCopy = AutoCopy.CreateMap<Data, NameValueCollection>();

    autoCopy.ForMember(p => p.ext, opt => opt.MapFrom(p=>ac.Map(p)));

    autoCopy.Provider = new HttpRequestParamsExpressionProvider(typeof(NameValueCollection));

    autoCopy.Register();

    Data data=autoCopy.Map(collection);

```
## 类型转换
### 自动转换
内部类[TypeConverter](/AutoCopyLib/TypeConverter.cs)的TryConvert方法通过以下顺序进行类型的自动转换：

1. 是否可以显式转换
2. 是否可以隐式转换
3. 是否存在继承关系
4. 是否存在Convert.ToXXX方法
5. 是否可以调用目标类型上的TryParse方法
6. 调用[Convert.ChangeType](https://msdn.microsoft.com/en-us/library/system.convert.changetype(v=vs.110).aspx "Convert.ChangeType")方法
### 手动转换

通过调用**AutoCopy<T, D>**类实例的**ForTypeConvert<T1, T2>**方法来注册类型转换。

## 抽象类[TargetExpressionProviderBase](/AutoCopyLib/TargetExpressionProviderBase.cs)的TryGetExpression方法参数说明

假定AutoCopy<T1, T2>中T1为目标类型，T2为源类型

1. name			源属性名称
2. parameter	源属性的参数表达式
3. destType		目标类型
4. exp			通过TryGetExpression方法最后生成的表达式
5. variable		临时变量
6. test			测试表达式
7. ifTrue		是否需要测试；如果该值为true，则只有test执行返回true时才会继续执行exp

## 修改日志
2017-12-05 增加DataRow映射到实体类的示例程序

## 注意事项

由于AutoCopy在运行时通过主动调用**Register**方法使用反射分析类的属性，所以如果方法进行了混淆可能会出现Bug。