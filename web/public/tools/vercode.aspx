<%@ Page Language="C#" AutoEventWireup="true" %>
<% 
    string code = Comm.MyHepler.CreateValidateCode(4);
    Session["ValidateCode"] = code;
    byte[] bytes = Comm.MyHepler.CreateValidateGraphic(code);
    Response.ClearContent();
    //输出流的HTTP MIME类型设置为"image/Png"
    Response.ContentType = "image/Jpeg";
    //输出图片的二进制流
    Response.BinaryWrite(bytes);
 %>
