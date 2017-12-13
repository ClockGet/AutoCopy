using System.Collections.Specialized;
using System.Web;
using AgileObjects.ReadableExpressions.Extensions;
using AgileObjects.ReadableExpressions;
/// <summary>
/// AutoCopyUtils 的摘要说明
/// </summary>
public class AutoCopyUtils
{
    private static AutoCopyLib.AutoCopy<NameValueCollection,Data> autoCopy = null;
    public static void Initialize()
    {
        autoCopy = AutoCopyLib.AutoCopy.CreateMap<NameValueCollection,Data>();
        autoCopy.Provider = new HttpRequestParamsExpressionProvider(typeof(NameValueCollection));
        autoCopy.Register(true);
    }
    public static AutoCopyLib.AutoCopy<NameValueCollection,Data> Instance
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