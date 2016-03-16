using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.IO;

namespace WeiXinAPI
{
    /// <summary>
    ///WeixinAPI 的摘要说明
    /// </summary>
    public class WeixinApiFun
    {
        public WeixinApiFun()
        {

        }

        #region 静态全局fun
        #region 初始化微信公众号配置
        /// <summary>
        /// 初始化微信公众号配置
        /// </summary>
        /// <param name="weixinid"></param>
        public static void InitWeiXinConfig(string weixinid)
        {
            DAL.WeiXinConfig model = new DAL.WeiXinConfigDAO().GetModelByWeiXinId(weixinid);
            if (model != null)
            {
                if (model.IsApply == true)
                {
                    throw new Exception("微信公众号已被禁用");
                }
                WeiXinAPI.WeiXinApiConfig.appid = model.AppID;
                WeiXinAPI.WeiXinApiConfig.secret = model.AppSecret;
                WeiXinAPI.WeiXinApiConfig.EncodingAESKey = model.EncodingAESKey;
            }
            else
            {
                throw new Exception("未获取到微信公众号");
            }
        }
        #endregion

        #region 回复信息
        /// <summary>
        /// 普通回复
        /// </summary>
        /// <param name="FromUserName"></param>
        /// <param name="ToUserName"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static string SendMessage(string FromUserName, string ToUserName, string cont)
        {
            string resXml = "";
            string reply_type = "text";
            string reply_text = cont;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            int ts = (int)(DateTime.Now - startTime).TotalSeconds;
            if (reply_type == "text")
            {
                resXml = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName><CreateTime>" + ts + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + reply_text + "]]></Content><FuncFlag>0</FuncFlag></xml>";
            }
            return resXml;
        }
        #endregion 
        #endregion

        #region 获取access_token
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <param name="weixinid">公众号id</param>
        /// <returns></returns>
        public access_token_struct GetToken(string weixinid)
        {
            access_token_struct ats = (access_token_struct)System.Web.HttpRuntime.Cache["access_token_struct_" + weixinid];
            if (ats == null)
            {
                ats = new access_token_struct();
            }
            else
            {
                TimeSpan ts = DateTime.Now - (DateTime)ats.last_get;
                if (ts.Seconds < ats.expires_in)
                {
                    return ats;
                }
            }
            InitWeiXinConfig(weixinid);
            string parstr = string.Format("grant_type={0}&appid={1}&secret={2}", "client_credential", WeiXinApiConfig.appid, WeiXinApiConfig.secret);
            string data = WXComm.GetHtmlResponse("https://api.weixin.qq.com/cgi-bin/token?" + parstr);
            Dictionary<string, string> dic = WXComm.StringToDictionary(data);
            if (WXComm.GetDictionaryValue("errcode", dic) == null)
            {
                ats.access_token = WXComm.GetDictionaryValue("access_token", dic);
                ats.expires_in = int.Parse(WXComm.GetDictionaryValue("expires_in", dic));
                ats.last_get = DateTime.Now;
                System.Web.HttpRuntime.Cache.Insert("access_token_struct_" + weixinid, ats, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(ats.expires_in / 60 - 20));
                return ats;
            }
            else
            {
                throw new Exception(WXComm.GetDictionaryValue("errcode", dic));
            }
        }
        #endregion

        #region 获取用户信息
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="weixinid"></param>
        /// <param name="openid"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public user_info_struct GetUserInfo(string weixinid, string openid, string lang)
        {
            access_token_struct atc = GetToken(weixinid);
            user_info_struct umodel = new user_info_struct();
            try
            {
                string parstr = string.Format("access_token={0}&openid={1}&lang={2}", atc.access_token, openid, lang);
                string data = WXComm.GetHtmlResponse("https://api.weixin.qq.com/cgi-bin/user/info?" + parstr);
                Dictionary<string, string> dic = WXComm.StringToDictionary(data);
                if (WXComm.GetDictionaryValue("errcode", dic) == null)
                {
                    umodel.issuccess = true;
                    umodel.subscribe = WXComm.GetDictionaryValue("subscribe", dic);
                    umodel.openid = WXComm.GetDictionaryValue("openid", dic);
                    umodel.nickname = WXComm.GetDictionaryValue("nickname", dic);
                    umodel.sex = WXComm.GetDictionaryValue("sex", dic);
                    umodel.city = WXComm.GetDictionaryValue("city", dic);
                    umodel.country = WXComm.GetDictionaryValue("country", dic);
                    umodel.province = WXComm.GetDictionaryValue("province", dic);
                    umodel.language = WXComm.GetDictionaryValue("language", dic);
                    umodel.headimgurl = WXComm.GetDictionaryValue("headimgurl", dic);
                    umodel.subscribe_time = WXComm.GetDictionaryValue("subscribe_time", dic);
                    if (WXComm.GetDictionaryValue("unionid", dic) != null)
                    {
                        umodel.unionid = WXComm.GetDictionaryValue("unionid", dic);
                    }
                    umodel.remark = WXComm.GetDictionaryValue("remark", dic);
                    umodel.groupid = WXComm.GetDictionaryValue("groupid", dic);
                }
                else
                {
                    umodel.issuccess = false;
                    umodel.remark = WXComm.GetDictionaryValue("errmsg", dic);
                }
            }
            catch (Exception ex)
            {
                umodel.issuccess = false;
                umodel.remark = ex.Message;
            }
            return umodel;
        }
        #endregion

        #region 更新菜单
        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="weixinid"></param>
        /// <param name="openid"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public bool CreateMenu(string weixinid,string menustr,out string error)
        {
            error = "";
            access_token_struct atc = GetToken(weixinid);
            try
            {
                string parstr = string.Format("access_token={0}", atc.access_token);
                string data = WXComm.GetHtmlResponse("https://api.weixin.qq.com/cgi-bin/menu/create?" + parstr,"POST",menustr);
                Dictionary<string, string> dic = WXComm.StringToDictionary(data);
                if (WXComm.GetDictionaryValue("errcode", dic) == "0")
                {
                    return true;
                }
                else
                {
                    error = WXComm.GetDictionaryValue("errmsg", dic);
                    return false;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion

        #region 更新菜单
        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="weixinid"></param>
        /// <param name="openid"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public bool DeleteMenu(string weixinid)
        {
            access_token_struct atc = GetToken(weixinid);
            try
            {
                string parstr = string.Format("access_token={0}", atc.access_token);
                string data = WXComm.GetHtmlResponse("https://api.weixin.qq.com/cgi-bin/menu/delete?" + parstr);
                Dictionary<string, string> dic = WXComm.StringToDictionary(data);
                if (WXComm.GetDictionaryValue("errcode", dic) == "0")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 验证微信签名
        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// <returns></returns>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        public bool CheckSignature(string token)
        {
            //从微信服务器接收传递过来的数据
            string signature = HttpContext.Current.Request.QueryString["signature"]; //微信加密签名
            string timestamp = HttpContext.Current.Request.QueryString["timestamp"];//时间戳
            string nonce = HttpContext.Current.Request.QueryString["nonce"];//随机数
            string[] ArrTmp = { token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序
            string tmpStr = string.Join("", ArrTmp);//将三个字符串组成一个字符串
            tmpStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");//进行sha1加密
            tmpStr = tmpStr.ToLower();
            //加过密的字符串与微信发送的signature进行比较，一样则通过微信验证，否则失败。
            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 接收微信信息
        /// <summary>
        /// 接收微信信息
        /// </summary>
        public void RequestMsg()
        {
            //接收信息流
            System.IO.Stream requestStream = System.Web.HttpContext.Current.Request.InputStream;
            byte[] requestByte = new byte[requestStream.Length];
            requestStream.Read(requestByte, 0, (int)requestStream.Length);
            //转换成字符串
            string requestStr = System.Text.Encoding.UTF8.GetString(requestByte);

            if (!string.IsNullOrEmpty(requestStr))
            {
                //封装请求类到xml文件中
                System.Xml.XmlDocument requestDocXml = new System.Xml.XmlDocument();
                requestDocXml.LoadXml(requestStr);
                System.Xml.XmlElement rootElement = requestDocXml.DocumentElement;
                System.Xml.XmlNode MsgType = rootElement.SelectSingleNode("MsgType");

                //将XML文件封装到实体类requestXml中
                RequestXml requestXml = new RequestXml();
                requestXml.ToUserName = rootElement.SelectSingleNode("ToUserName").InnerText;//开发者微信号
                requestXml.FromUserName = rootElement.SelectSingleNode("FromUserName").InnerText;//发送方微信号
                requestXml.CreateTime = rootElement.SelectSingleNode("CreateTime").InnerText;//消息发送信息
                requestXml.MsgType = MsgType.InnerText;
                if (rootElement.SelectSingleNode("MsgId") != null)
                {
                    requestXml.MsgId = rootElement.SelectSingleNode("MsgId").InnerText;
                }
                else
                {
                    WriteTxt("没有msgid了");
                }
                //获取接收信息的类型
                switch (requestXml.MsgType)
                {
                    //接收普通信息
                    case WeiXinAPI.MsgType.text://文本信息
                        requestXml.Content = rootElement.SelectSingleNode("Content").InnerText;
                        break;
                    case WeiXinAPI.MsgType.image://图片信息
                        requestXml.PicUrl = rootElement.SelectSingleNode("PicUrl").InnerText;
                        requestXml.MediaId = rootElement.SelectSingleNode("MediaId").InnerText;
                        break;
                    case WeiXinAPI.MsgType.voice://语音消息
                        requestXml.Format = rootElement.SelectSingleNode("Format").InnerText;
                        requestXml.MediaId = rootElement.SelectSingleNode("MediaId").InnerText;
                        break;
                    case WeiXinAPI.MsgType.video://视频消息
                        requestXml.ThumbMediaId = rootElement.SelectSingleNode("ThumbMediaId").InnerText;
                        requestXml.MediaId = rootElement.SelectSingleNode("MediaId").InnerText;
                        break;
                    case WeiXinAPI.MsgType.shortvideo://小视频消息
                        requestXml.ThumbMediaId = rootElement.SelectSingleNode("ThumbMediaId").InnerText;
                        requestXml.MediaId = rootElement.SelectSingleNode("MediaId").InnerText;
                        break;
                    case WeiXinAPI.MsgType.location://地理位置信息
                        requestXml.Location_X = rootElement.SelectSingleNode("Location_X").InnerText;
                        requestXml.Location_Y = rootElement.SelectSingleNode("Location_Y").InnerText;
                        requestXml.Scale = rootElement.SelectSingleNode("Scale").InnerText;
                        requestXml.Label = rootElement.SelectSingleNode("Label").InnerText;
                        break;
                    case WeiXinAPI.MsgType.link://链接消息
                        requestXml.Title = rootElement.SelectSingleNode("Title").InnerText;
                        requestXml.Description = rootElement.SelectSingleNode("Description").InnerText;
                        requestXml.Url = rootElement.SelectSingleNode("Url").InnerText;
                        break;

                    //接收事件推送
                    //大概包括有：关注/取消关注事件、扫描带参数二维码事件、上报地理位置事件、自定义菜单事件、点击菜单拉取消息时的事件推送、点击菜单跳转链接时的事件推送
                    case WeiXinAPI.MsgType.Event:
                        requestXml.Event = rootElement.SelectSingleNode("Event").InnerText;
                        switch (requestXml.Event)
                        {
                            case WeiXinAPI.EventType.subscribe:
                            case WeiXinAPI.EventType.unsubscribe:
                                break;
                            case WeiXinAPI.EventType.click:
                            case WeiXinAPI.EventType.view:
                                requestXml.RequestEventXml.EventKey = rootElement.SelectSingleNode("EventKey").InnerText; 	//事件KEY值，qrscene_为前缀，后面为二维码的参数值
                                break;
                            case WeiXinAPI.EventType.scan:
                                requestXml.RequestEventXml.EventKey = rootElement.SelectSingleNode("EventKey").InnerText; 	//事件KEY值，qrscene_为前缀，后面为二维码的参数值
                                requestXml.RequestEventXml.Ticket = rootElement.SelectSingleNode("Ticket").InnerText; 	//二维码的ticket，可用来换取二维码图片 
                                break;
                            case WeiXinAPI.EventType.location:
                                requestXml.RequestEventXml.Latitude = rootElement.SelectSingleNode("Latitude").InnerText; 	// 	地理位置纬度
                                requestXml.RequestEventXml.Longitude = rootElement.SelectSingleNode("Longitude").InnerText; 	// 	地理位置经度
                                requestXml.RequestEventXml.Precision = rootElement.SelectSingleNode("Precision").InnerText; 	// 	地理位置精度 
                                break;
                        }
                        break;
                }
                //回复消息
                ResponseMsg(requestXml);
            }
        }
        #endregion

        #region 回复消息
        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="requestXml"></param>
        private void ResponseMsg(RequestXml requestXml)
        {
            if (requestXml.MsgType == WeiXinAPI.MsgType.Event && requestXml.Event == WeiXinAPI.EventType.subscribe)
            {
                //关注
                DAL.WeiXinUser model = new DAL.WeiXinUser();
                user_info_struct um = GetUserInfo(requestXml.ToUserName, requestXml.FromUserName, "zh_CN");
                model.uId = -1;
                model.weixinid = requestXml.ToUserName;
                model.openid = requestXml.FromUserName;
                model.adddate = DateTime.Now;
                if (um.issuccess)
                {
                    model.subscribe = um.subscribe;
                    model.openid = um.openid;
                    model.nickname = um.nickname;
                    model.sex = um.sex;
                    model.city = um.city;
                    model.country = um.country;
                    model.province = um.province;
                    model.language = um.language;
                    model.headimgurl = um.headimgurl;
                    model.subscribe_time = UnixTimeToTime(um.subscribe_time);
                    model.unionid = um.unionid;
                    model.remark = um.remark;
                    model.groupid = um.groupid;
                }
                else
                {
                    model.remark = um.remark;
                }
                new DAL.WeiXinUserDAO().AddOrUpdate(model);
                HttpContext.Current.Response.Write(GetSubscribe(requestXml.FromUserName, requestXml.ToUserName));
            }
            else if (requestXml.MsgType == WeiXinAPI.MsgType.Event && requestXml.Event == WeiXinAPI.EventType.unsubscribe)
            {
                //取消关注
                new DAL.WeiXinUserDAO().DelByOpenID(requestXml.FromUserName);
            }
            else if (requestXml.MsgType == WeiXinAPI.MsgType.text)
            {
                if (requestXml.Content.Contains("菜单"))
                {
                    List<string> list = new List<string>();
                    string cont = "本店特惠\r\n";
                    for (int i = 0; i < list.Count; i++)
                    {
                        //cont += "("+(i+1)+".) 【"+list[i].dName+"】\r\n价格:"+list[i].dPrice+"元/份\r\n介绍:"+list[i].dDesc+"\r\n\r\n";
                    }
                    cont += "联系我们:<a class=\"c\" href=\"tel:10086\">呼叫呼叫</a>";
                    HttpContext.Current.Response.Write(GetCustomReply(requestXml.FromUserName, requestXml.ToUserName, cont));
                }
                else if (requestXml.Content.Contains("猜图"))
                {
                    string cont = "点击进入:<a href=\"http://www.litgame.com/pic2word/\">疯狂猜图</a>";
                    HttpContext.Current.Response.Write(GetCustomReply(requestXml.FromUserName, requestXml.ToUserName, cont));
                }
                else
                {
                    string result = XiaoIApi.GetTuLingApi(requestXml.Content);
                    result = result.Substring(result.IndexOf("\"text\":")+7).Replace("}","").Replace("<br>","").Replace("\r","").Replace("\"","");
                    HttpContext.Current.Response.Write(GetCustomReply(requestXml.FromUserName, requestXml.ToUserName, result));
                }
            }
            else if (requestXml.MsgType == WeiXinAPI.MsgType.voice)
            {
                HttpContext.Current.Response.Write(GetCustomReply(requestXml.FromUserName, requestXml.ToUserName, "测试啊<a href=\"http://www.baidu.com\">语音啊</a>"));
            }
            else if (requestXml.MsgType == WeiXinAPI.MsgType.location)
            {
                HttpContext.Current.Response.Write(GetCustomReply(requestXml.FromUserName, requestXml.ToUserName, "测试啊<a href=\"http://www.baidu.com\">位置" + requestXml.Label + "</a>"));
            }
            else if (requestXml.MsgType == WeiXinAPI.MsgType.link)
            {
                HttpContext.Current.Response.Write(GetCustomReply(requestXml.FromUserName, requestXml.ToUserName, "测试啊<a href=\"http://www.baidu.com\">链接" + requestXml.Url + "</a>"));
            }
            else if (requestXml.MsgType == WeiXinAPI.MsgType.image)
            {
                HttpContext.Current.Response.Write(GetCustomReply(requestXml.FromUserName, requestXml.ToUserName, "测试啊<a href=\"http://www.baidu.com\">图片啊</a>"));
            }
            else
            {
                HttpContext.Current.Response.Write(GetCustomReply(requestXml.FromUserName, requestXml.ToUserName, "测试啊<a href=\"http://www.baidu.com\">事件---" + requestXml.Event + "--" + requestXml.EventKey + "</a>"));
            }
        }

        #endregion

        #region 关注回复
        /// <summary>
        /// 关注的时候回复
        /// </summary>
        /// <param name="FromUserName"></param>
        /// <param name="ToUserName"></param>
        /// <returns></returns>
        public string GetSubscribe(string FromUserName, string ToUserName)
        {
            string cont = "测试啊<a href=\"http://www.baidu.com\">你关注我啊</a>";
            string resXml = "";
            string reply_type = "text";
            string reply_text = cont;
            if (reply_type == "text")
            {
                resXml = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + reply_text + "]]></Content><FuncFlag>0</FuncFlag></xml>";
            }
            else
            {
                //resXml = GetArticle(FromUserName, ToUserName, article_id, User_ID);
            }
            return resXml;
        }
        #endregion 关注回复

        #region 常规回复
        /// <summary>
        /// 普通回复
        /// </summary>
        /// <param name="FromUserName"></param>
        /// <param name="ToUserName"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public string GetCustomReply(string FromUserName, string ToUserName, string cont)
        {
            string resXml = "";
            //string article_id = dtSubscribe.Rows[0]["article_id"].ToString();
            string reply_type = "text";
            string reply_text = cont;

            if (reply_type == "text")
            {
                resXml = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + reply_text + "]]></Content><FuncFlag>0</FuncFlag></xml>";
            }
            else
            {
                //resXml = GetArticle(FromUserName, ToUserName, article_id, User_ID);
            }
            return resXml;
        }
        #endregion

        #region 自动回复
        ///// <summary>
        ///// 自动默认回复
        ///// </summary>
        ///// <param name="FromUserName"></param>
        ///// <param name="ToUserName"></param>
        ///// <param name="WeChat_ID"></param>
        ///// <param name="User_ID"></param>
        ///// <returns></returns>
        //public string GetDefault(string FromUserName, string ToUserName, string WeChat_ID, string User_ID)
        //{
        //    string resXml = "";
        //    string sqlWhere = !string.IsNullOrEmpty(WeChat_ID) ? "WeChat_ID=" + WeChat_ID + " and reply_fangshi=1" : "";
        //    //获取保存的默认回复设置信息
        //    DataTable dtDefault = replydal.GetRandomList(sqlWhere, "1").Tables[0];

        //    if (dtDefault.Rows.Count > 0)
        //    {
        //        string article_id = dtDefault.Rows[0]["article_id"].ToString();
        //        string reply_type = dtDefault.Rows[0]["reply_type"].ToString();
        //        string reply_text = dtDefault.Rows[0]["reply_text"].ToString();
        //        //如果选择的是文本
        //        if (reply_type == "text")
        //        {
        //            resXml = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + reply_text + "]]></Content><FuncFlag>0</FuncFlag></xml>";
        //        }
        //        else
        //        {
        //            //返回素材（图文列表）
        //            resXml = GetArticle(FromUserName, ToUserName, article_id, User_ID);
        //        }
        //    }

        //    return resXml;
        //}
        #endregion 默认回复

        #region 关键字回复
        ///// <summary>
        ///// 关键字回复
        ///// </summary>
        ///// <param name="FromUserName"></param>
        ///// <param name="ToUserName"></param>
        ///// <param name="Content"></param>
        ///// <returns></returns>
        //public string GetKeyword(string FromUserName, string ToUserName, string Content)
        //{
        //    string resXml = "";
        //    string sqlWhere = "wechat_id=" + WeChat_ID + " and keyword_name='" + Content + "'";

        //    DataTable dtKeyword = keyworddal.GetList(sqlWhere).Tables[0];

        //    if (dtKeyword.Rows.Count > 0)
        //    {
        //        dtKeyword = keyworddal.GetRandomList(sqlWhere, "1").Tables[0];

        //        if (dtKeyword.Rows.Count > 0)
        //        {
        //            string article_id = dtKeyword.Rows[0]["article_id"].ToString();
        //            string keyword_type = dtKeyword.Rows[0]["keyword_type"].ToString();
        //            string keyword_text = dtKeyword.Rows[0]["keyword_text"].ToString();

        //            switch (keyword_type)
        //            {
        //                case "text":
        //                    resXml = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + keyword_text + "]]></Content><FuncFlag>0</FuncFlag></xml>";
        //                    break;
        //                case "news":
        //                    resXml = GetArticle(FromUserName, ToUserName, article_id, User_ID);
        //                    break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        resXml = GetDefault(FromUserName, ToUserName, WeChat_ID, User_ID);
        //    }

        //    return resXml;
        //}
        #endregion 关键字回复

        #region 菜单单击
        ///// <summary>
        ///// 菜单点击事件回复信息
        ///// </summary>
        ///// <param name="FromUserName"></param>
        ///// <param name="ToUserName"></param>
        ///// <param name="EventKey"></param>
        ///// <returns></returns>
        //public string GetMenuClick(string FromUserName, string ToUserName, string EventKey)
        //{
        //    string resXml = "";
        //    string sqlWhere = "wechat_id=" + WeChat_ID + " and caidan_key='" + EventKey + "'";

        //    WriteTxt(sqlWhere);
        //    try
        //    {

        //        DataTable dtMenu = caidandal.GetList(sqlWhere).Tables[0];

        //        if (dtMenu.Rows.Count > 0)
        //        {
        //            string article_id = dtMenu.Rows[0]["article_id"].ToString();
        //            string caidan_retype = dtMenu.Rows[0]["caidan_retype"].ToString();
        //            string caidan_retext = dtMenu.Rows[0]["caidan_retext"].ToString();


        //            switch (caidan_retype)
        //            {
        //                case "text":
        //                    resXml = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + caidan_retext + "]]></Content><FuncFlag>0</FuncFlag></xml>";
        //                    break;
        //                case "news":
        //                    resXml = GetArticle(FromUserName, ToUserName, article_id, User_ID);
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteTxt("异常：" + ex.Message + "Struck:" + ex.StackTrace.ToString());
        //    }

        //    return resXml;
        //}
        #endregion 菜单单击

        #region 获取素材
        ///// <summary>
        ///// 获取素材
        ///// </summary>
        ///// <param name="FromUserName"></param>
        ///// <param name="ToUserName"></param>
        ///// <param name="Article_ID"></param>
        ///// <param name="User_ID"></param>
        ///// <returns></returns>
        //public string GetArticle(string FromUserName, string ToUserName, string Article_ID, string User_ID)
        //{
        //    string resXml = "";

        //    DataTable dtArticle = articledal.GetList("article_id=" + Article_ID + " OR article_layid=" + Article_ID).Tables[0];

        //    if (dtArticle.Rows.Count > 0)
        //    {
        //        resXml = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[news]]></MsgType><Content><![CDATA[]]></Content><ArticleCount>" + dtArticle.Rows.Count + "</ArticleCount><Articles>";

        //        foreach (DataRow Row in dtArticle.Rows)
        //        {
        //            string article_title = Row["article_title"].ToString();
        //            string article_description = Row["article_description"].ToString();
        //            string article_picurl = Row["article_picurl"].ToString();
        //            string article_url = Row["article_url"].ToString();
        //            string article_type = Row["article_type"].ToString();

        //            switch (article_type)
        //            {
        //                case "Content":
        //                    article_url = hostUrl + "/web/wechat/api/article.aspx?aid=" + Row["Article_ID"].ToString();
        //                    break;
        //                case "Href":
        //                    article_url = Row["article_url"].ToString();
        //                    break;
        //            }

        //            if (string.IsNullOrEmpty(article_url))
        //            {
        //                article_url = hostUrl + "/web/wechat/api/article.aspx?aid=" + Row["Article_ID"].ToString();
        //            }

        //            article_url += (article_url.IndexOf("uid=") > -1 ? "" : (article_url.IndexOf("?") > -1 ? "&" : "?") + "uid=" + User_ID);
        //            article_url += (article_url.IndexOf("wxid=") > -1 ? "" : (article_url.IndexOf("?") > -1 ? "&" : "?") + "wxid=" + FromUserName);
        //            article_url += (article_url.IndexOf("wxref=") > -1 ? "" : (article_url.IndexOf("?") > -1 ? "&" : "?") + "wxref=mp.weixin.qq.com");

        //            resXml += "<item><Title><![CDATA[" + article_title + "]]></Title><Description><![CDATA[" + article_description + "]]></Description><PicUrl><![CDATA[" + article_picurl + "]]></PicUrl><Url><![CDATA[" + article_url + "]]></Url></item>";
        //        }

        //        resXml += "</Articles><FuncFlag>1</FuncFlag></xml>";
        //    }

        //    return resXml;
        //}
        #endregion 获取图文列表

        #region 通用方法
        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 记录bug，以便调试
        /// </summary>
        /// <returns></returns>
        public bool WriteTxt(string str)
        {
            try
            {
                FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("wxbugLog.txt"), FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                //开始写入
                sw.WriteLine(str);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                fs.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion 通用方法
    }
}