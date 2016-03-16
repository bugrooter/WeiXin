using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinAPI
{
    /// <summary>
    ///WeiXinAPIConfig 的摘要说明
    /// </summary>
    public class WeiXinApiConfig
    {
        /// <summary>
        /// grant_type
        /// </summary>
        public static string grant_type = "client_credential";
        /// <summary>
        /// 应用ID
        /// </summary>
        public static string appid = "";
        /// <summary>
        /// 应用密钥
        /// </summary>
        public static string secret = "";
        /// <summary>
        /// 消息加解密密钥
        /// </summary>
        public static string EncodingAESKey = "";

        /// <summary>
        /// 微信菜单类型
        /// </summary>
        public static List<MenuType> WeiXinMenuType = new List<MenuType>() 
        {
            new MenuType(){Name="点击推事件",Value="click"},
            new MenuType(){Name="跳转URL",Value="view"},
            new MenuType(){Name="扫码推事件",Value="scancode_push"},
            new MenuType(){Name="扫码推事件且弹出“消息接收中”提示框",Value="scancode_waitmsg"},
            new MenuType(){Name="弹出系统拍照发图",Value="pic_sysphoto"},
            new MenuType(){Name="弹出拍照或者相册发图",Value="pic_photo_or_album"},
            new MenuType(){Name="弹出微信相册发图器",Value="pic_weixin"},
            new MenuType(){Name="弹出地理位置选择器",Value="location_select"},
            new MenuType(){Name="下发消息（除文本消息）",Value="media_id"},
            new MenuType(){Name="跳转图文消息URL",Value="view_limited"}
        };

        public WeiXinApiConfig()
        {

        }
    }

    public class WXComm
    {
        public static string GetHtmlResponse(string url, string method = "GET", string postdata = null, string postcode = "utf-8", string xAuth = "")
        {
            try
            {
                if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                {
                    url = "http://" + url;
                }
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                req.Timeout = 10000;
                if (xAuth.Trim() != "")
                {
                    req.Headers.Add("X-Auth", xAuth);
                }
                req.Method = method.ToUpper();
                req.KeepAlive = false;
                if (req.Method == "POST" && !string.IsNullOrEmpty(postdata))
                {
                    byte[] buffer = System.Text.Encoding.GetEncoding(postcode).GetBytes(postdata);
                    req.ContentLength = buffer.Length;
                    using (System.IO.Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(buffer, 0, buffer.Length);
                        reqStream.Close();
                    }
                }

                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();
                string code = res.CharacterSet;
                System.Text.Encoding cod = System.Text.Encoding.Default;
                if (code.ToUpper().StartsWith("UTF"))
                {
                    cod = System.Text.Encoding.UTF8;
                }
                else
                {
                    cod = System.Text.Encoding.Default;
                }
                System.IO.StreamReader sr = new System.IO.StreamReader(res.GetResponseStream(), cod);
                string result = sr.ReadToEnd();
                sr.Close();
                return result;
            }
            catch
            {
                return "{\"errcode\":-1,\"errmsg\":\"调用接口错误\"}";
            }
        }

        public static Dictionary<string, string> StringToDictionary(string jsonstr)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            jsonstr = jsonstr.Replace(",\"", "|");
            if (!string.IsNullOrEmpty(jsonstr) && jsonstr.StartsWith("{") && jsonstr.EndsWith("}"))
            {
                jsonstr = jsonstr.Trim(new char[] { '{', '}' });
                string[] tmp = jsonstr.Split('|');
                for (int i = 0; i < tmp.Length; i++)
                {
                    string[] tp = tmp[i].Split(':');
                    dic.Add(tp[0].Trim(new char[] { '\"' }), tp[1].Trim(new char[] { '\"' }));
                }
            }
            return dic;
        }

        public static string GetDictionaryValue(string key, Dictionary<string, string> dic)
        {
            if (dic.Keys.Contains(key))
            {
                return dic[key];
            }
            else
            {
                return null;
            }
        }
    }
}