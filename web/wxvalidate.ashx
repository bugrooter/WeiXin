<%@ WebHandler Language="C#" Class="wxvalidate" %>

using System;
using System.Web;

public class wxvalidate : IHttpHandler
{
    private string token = "jmsoeasy";
    private WeiXinAPI.WeixinApiFun wx = new WeiXinAPI.WeixinApiFun();
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        if (context.Request.HttpMethod.ToUpper() == "POST")
        {
            try
            {
                wx.RequestMsg();
            }
            catch (Exception ex)
            {
                wx.WriteTxt("报错了---"+ex.Message+"--"+ex.Source+"--"+ex.StackTrace);
            }
        }
        else
        {
            string echoStr = context.Request.QueryString["echoStr"];
            if (wx.CheckSignature(token))
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    context.Response.Write(echoStr);
                    context.Response.End();
                }
            }
        }
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}