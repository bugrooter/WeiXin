using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinAPI
{
    public class XiaoIApi
    {
        public string GetReplay(string ques)
        {
            string app_key = "QxGrzaKxCuIm";
            string app_secret = "BAZMYgz0TEakGXS3GjyP";
            String realm = "xiaoi.com";
            String method = "POST";
            String uri = "/ask.do";
            byte[] b = new byte[20];
            new Random().NextBytes(b);
            String nonce = Hex(b);
            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] HA1 = sha.ComputeHash(Encoding.UTF8.GetBytes(app_key + ":" + realm + ":" + app_secret));
            byte[] HA2 = sha.ComputeHash(Encoding.UTF8.GetBytes(method + ":" + uri));
            String sign = Hex(sha.ComputeHash(Encoding.UTF8.GetBytes(Hex(HA1) + ":" + nonce + ":" + Hex(HA2))));
            string postdata = string.Format("question={0}&userId={1}&platform=custom&type=1",ques,"001");
            return WeiXinAPI.WXComm.GetHtmlResponse("http://www.ibotcloud.com/api", "POST", ques, "utf-8", sign);
        }

        public string GetSaiKeApi(string quest)
        {
            return WeiXinAPI.WXComm.GetHtmlResponse("http://dev.skjqr.com/api/weixin.php?email=myfree007@163.com&appkey=714a0be36241099529e70a722ba52201&msg="+quest);
        }

        public static string GetTuLingApi(string quest)
        {
            string url = "http://www.tuling123.com/openapi/api";
            string postdata = string.Format("key={0}&info={1}&userid={2}", "4e44a4e8d0bf73bdb0c122b0edc50b78",quest,"007");
            return WeiXinAPI.WXComm.GetHtmlResponse(url+"?"+postdata);
        }

        public static String Hex(byte[] data)
        {
            String r = "";
            for (int i = 0; i < data.Length; i++)
                r += data[i].ToString("X2");
            return r.ToLower();
        }
    }
}
