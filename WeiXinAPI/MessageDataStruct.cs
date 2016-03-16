using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinAPI
{

    /// <summary>
    /// access_token结构
    /// </summary>
    public class access_token_struct
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }

        public DateTime? last_get { get; set; }
    }

    /// <summary>
    /// 微信关注用户信息
    /// </summary>
    public class user_info_struct
    {
        public bool issuccess { get; set; }
        public string subscribe { get; set; }// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
        public string openid { get; set; }// 用户的标识，对当前公众号唯一
        public string nickname { get; set; }// 	用户的昵称
        public string sex { get; set; }// 	用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        public string city { get; set; }// 	用户所在城市
        public string country { get; set; }// 	用户所在国家
        public string province { get; set; }// 	用户所在省份
        public string language { get; set; }// 	用户的语言，简体中文为zh_CN
        public string headimgurl { get; set; }// 	用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        public string subscribe_time { get; set; }// 	用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
        public string unionid { get; set; }// 	只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。详见：获取用户个人信息（UnionID机制）
        public string remark { get; set; }// 	公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
        public string groupid { get; set; }// 	用户所在的分组ID
    }

    /// <summary>
    /// 请求的xml数据
    /// </summary>
    public class RequestXml
    {
        public string MsgId { get; set; }//消息id
        public string ToUserName { get; set; }//开发者微信号
        public string FromUserName { get; set; }//发送方微信号
        public string CreateTime { get; set; }//消息发送信息
        public string MsgType { get; set; }//消息类型
        //文本消息
        public string Content { get; set; }//消息内容

        //图片消息
        public string PicUrl { get; set; }//图片链接 
        public string MediaId { get; set; }//图片消息媒体id，可以调用多媒体文件下载接口拉取数据。 

        //语音消息
        public string Format { get; set; } //语音格式，如amr，speex等 

        //视频消息,小视频消息
        public string ThumbMediaId { get; set; }//视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。 

        //地理位置消息
        public string Location_X { get; set; }//地理位置维度
        public string Location_Y { get; set; }//地理位置经度
        public string Scale { get; set; } //地图缩放大小
        public string Label { get; set; } //地理位置信息 

        //链接消息
        public string Title { get; set; } 	//消息标题
        public string Description { get; set; }//消息描述
        public string Url { get; set; } //消息链接 

        //事件消息
        public string Event { get; set; }//事件类型，subscribe(订阅)、unsubscribe(取消订阅) 

        public string EventKey { get; set; } //事件KEY值，qrscene_为前缀，后面为二维码的参数值
        public string Ticket { get; set; } //二维码的ticket，可用来换取二维码图片 

        //上报地理位置事件
        public string Latitude { get; set; } 	//地理位置纬度
        public string Longitude { get; set; } //地理位置经度
        public string Precision { get; set; } //地理位置精度 

        public RequestEventXml RequestEventXml = new RequestEventXml();
    }

    /// <summary>
    /// 请求的事件
    /// </summary>
    public class RequestEventXml
    {
        public string EventKey { get; set; } //事件KEY值，qrscene_为前缀，后面为二维码的参数值
        public string Ticket { get; set; } 	//二维码的ticket，可用来换取二维码图片 
        public string Latitude { get; set; }	//地理位置纬度
        public string Longitude { get; set; } 	//地理位置经度
        public string Precision { get; set; } 	//地理位置精度 
    }

    /// <summary>
    /// 消息分类
    /// </summary>
    public class MsgType
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        public const string text = "text";

        /// <summary>
        /// 图片消息
        /// </summary>
        public const string image = "image";

        /// <summary>
        /// 语音消息
        /// </summary>
        public const string voice = "voice";

        /// <summary>
        /// 视频消息
        /// </summary>
        public const string video = "video";

        /// <summary>
        /// 小视频消息
        /// </summary>
        public const string shortvideo = "shortvideo";

        /// <summary>
        /// 地理位置消息
        /// </summary>
        public const string location = "location";

        /// <summary>
        /// 链接消息
        /// </summary>
        public const string link = "link";

        /// <summary>
        /// 事件
        /// </summary>
        public const string Event = "event";
    }

    /// <summary>
    /// 事件分类
    /// </summary>
    public class EventType
    {
        /// <summary>
        /// 订阅
        /// </summary>
        public const string subscribe = "subscribe";

        /// <summary>
        /// 取消订阅
        /// </summary>
        public const string unsubscribe = "unsubscribe";

        /// <summary>
        /// 扫描二维码
        /// </summary>
        public const string scan = "SCAN";

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        public const string location = "LOCATION";

        /// <summary>
        /// 点击菜单拉取消息时的事件推送
        /// </summary>
        public const string click = "CLICK";

        /// <summary>
        /// 点击菜单跳转链接时的事件推送
        /// </summary>
        public const string view = "VIEW";
    }

    public class MenuType
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}