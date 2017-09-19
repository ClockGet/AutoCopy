<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        var data=AutoCopyUtils.Instance.Map(context.Request.Params);
        context.Response.Write("lambda表达式为：" + AutoCopyUtils.LambdaExpressionString);
        if(data==null)
        {
            context.Response.Write("没有接收到数据");
        }
        else
        {
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}