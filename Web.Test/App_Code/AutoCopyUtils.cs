using System.Collections.Specialized;
using System.Web;
using AgileObjects.ReadableExpressions.Extensions;
using AgileObjects.ReadableExpressions;
/// <summary>
/// AutoCopyUtils 的摘要说明
/// </summary>
public class AutoCopyUtils
{
    private static AutoCopyLib.AutoCopy<Data, NameValueCollection> autoCopy = null;
    public static void Initialize()
    {
        autoCopy = AutoCopyLib.AutoCopy.CreateMap<Data, NameValueCollection>();
        autoCopy.Provider = new HttpRequestParamsExpressionProvider(typeof(NameValueCollection));
        autoCopy.Register(true);
    }
    public static AutoCopyLib.AutoCopy<Data,NameValueCollection> Instance
    {
        get
        {
            return autoCopy;
        }
    }
    public static string LambdaExpressionString
    {
        get
        {
            return autoCopy.Lambda.ToReadableString();
        }
    }
}