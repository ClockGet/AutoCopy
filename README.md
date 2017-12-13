## AutoCopy

AutoCopy is a tool that reduces development time and helps programmers get out of some heavy human coding, which is inspired by **[AutoMapper](https://github.com/AutoMapper/AutoMapper "AutoMapper")**.

## Document

[Chinese](README_CN.md)

## Dependencies

* **[Mono.Reflection.dll](https://github.com/jbevain/mono.reflection "Mono.Reflection")**
* **[DelegateDecompiler.dll](https://github.com/hazzik/DelegateDecompiler "DelegateDecompiler")**

## Attribute

1. Fast execution
2. Based on the abstract class **TargetExpressionProviderBase** can be any extension
3. Support automatic / manual type conversion
4. Support for multiple instances of AutoCopy nesting

## Benchmark

iterations:100,000

| Action | mean time(ms)
---|---
hand map | 4.267375
AutoCopy | 4.18163333333333
AutoMapper | 42.4985

iterations:1,000,000

| Action | mean time(ms)
---|---
hand map | 30.884225
AutoCopy | 38.647675
AutoMapper | 322.8877

iterations:10,000,000

| Action | mean time(ms)
---|---
hand map | 440.14825
AutoCopy | 459.17575
AutoMapper | 3895.974725

Benchmark code see [here](/Console.Test/Program.cs)

## Example

### 1 Same type of object copyed

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

### 2 Different type of object copyed

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

    var autoCopy = AutoCopy.CreateMap<Customer, CustomerInfo>();

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

### 3 Multiple AutoCopy instances nested

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

    var ac = AutoCopy.CreateMap<NameValueCollection, Ext>();

    ac.Provider= new HttpRequestParamsExpressionProvider(typeof(NameValueCollection));

    var autoCopy = AutoCopy.CreateMap<NameValueCollection, Data>();

    autoCopy.ForMember(p => p.ext, opt => opt.MapFrom(p=>ac.Map(p)));

    autoCopy.Provider = new HttpRequestParamsExpressionProvider(typeof(NameValueCollection));

    autoCopy.Register();

    Data data=autoCopy.Map(collection);

```
## Type Convert
### Automatic conversion
The TryConvert method of the internal class [TypeConverter](/AutoCopyLib/TypeConverter.cs) performs automatic conversion of the type in the following order：

1. Whether it can be converted explicitly
2. Whether it can be converted implicitly
3. Whether it is a subclass
4. Whether there is the Convert.ToXXX method
5. Whether there is the TryParse method on the target type
6. Call [Convert.ChangeType](https://msdn.microsoft.com/en-us/library/system.convert.changetype(v=vs.110).aspx "Convert.ChangeType") method
### Manual conversion

Type conversions are registered by calling the **ForTypeConvert<T1, T2>** method of the **AutoCopy<T, D>** instance.

## Explanation of Parameter in [TryGetExpression](/AutoCopyLib/TargetExpressionProviderBase.cs) method

With AutoCopy<T1, T2>, assume T1 is the source type and T2 is the destination type

| | Parameter Name | Description
---|---|---
1 | name | destination property name
2 | parameter | Expression of source parameter Expression
3 | destType | destination type
4 | exp | the final Expression
5 | variable | variables
6 | test | test Expression
7 | ifTrue | Whether need to test or not; If the value is true, then only the test Expression executed return true can exp Expression will be called

## ChangeLog
2017-12-05 Add a demo which show the DataRow class convert to entity class  
2017-12-12 Adjust the order of parameters in AutoCopy<,> and fixed the parameter type bug in Option.ResolveUsing  
## Warning

Since AutoCopy uses reflection at runtime to analysis the properties of classes by calling **Register** methods automatically, bugs may occur if the source code is obfuscated.
