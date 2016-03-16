using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NVelocity.App;
using NVelocity.Runtime;
using NVelocity;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Comm
{
    public class MyHepler
    {
        /// <summary>
        /// 输出模版内容
        /// </summary>
        /// <param name="templetepath">模版目录</param>
        /// <param name="templetename">模版文件名称</param>
        /// <param name="paramsName">参数名称</param>
        /// <param name="parameters">参数值</param>
        /// <returns></returns>
        public static string GetTempleteHtml(string templetepath, string templetename, object parameters, string paramsName = "Model")
        {
            VelocityEngine vltEngine = new VelocityEngine();
            vltEngine.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");
            //模板存放目录
            vltEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, System.Web.Hosting.HostingEnvironment.MapPath(templetepath));
            vltEngine.Init();
            VelocityContext vltContext = new VelocityContext();
            // 传入模板所需要的参数
            if (parameters != null)
            {
                vltContext.Put(paramsName, parameters);
            }
            //兼容JQ插件$.
            vltContext.Put("JQ", "$.");
            Template vltTemplate = vltEngine.GetTemplate(templetename, "utf-8");
            // 定义一个字符串输出流
            System.IO.StringWriter vltWriter = new System.IO.StringWriter();
            vltTemplate.Merge(vltContext, vltWriter);
            string shtml = vltWriter.GetStringBuilder().ToString();
            vltWriter.Close();

            return shtml;
        }

        public static string GetTempleteHtml(string templetepath, string templetename)
        {
            VelocityEngine vltEngine = new VelocityEngine();
            vltEngine.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");
            //模板存放目录
            vltEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, System.Web.Hosting.HostingEnvironment.MapPath(templetepath));
            vltEngine.Init();
            VelocityContext vltContext = new VelocityContext();
            // 传入模板所需要的参数
            //兼容JQ插件$.
            vltContext.Put("JQ", "$.");
            Template vltTemplate = vltEngine.GetTemplate(templetename, "utf-8");
            // 定义一个字符串输出流
            System.IO.StringWriter vltWriter = new System.IO.StringWriter();
            vltTemplate.Merge(vltContext, vltWriter);
            string shtml = vltWriter.GetStringBuilder().ToString();
            vltWriter.Close();

            return shtml;
        }

        /// <summary>
        /// 输出html到请求页面
        /// </summary>
        /// <param name="shtml"></param>
        public static void DisplayHtml(string shtml)
        {
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.ContentType = "text/html";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(shtml);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="containsPage">要输出到的page对象</param>
        /// <param name="validateNum">验证码</param>
        public static byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 36);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 30; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 13, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 8);
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        /// 图片转字节
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] BitmapToBytes(Bitmap bmp)
        {
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);
            //输出图片流
            return stream.ToArray();
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns></returns>
        public static string CreateValidateCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /// <summary>
        /// 获取最新的Token值
        /// </summary>
        /// <returns></returns>
        public static string GetNewToken()
        {
            string token = Guid.NewGuid().ToString().Replace("-", "").Trim();
            HttpContext.Current.Session["Token"] = token;
            return token;
        }

        /// <summary>
        /// 获取URL图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Bitmap GetUrlImage(string url)
        {
            try
            {
                System.Net.WebRequest wr = System.Net.WebRequest.Create(url);
                System.Net.HttpWebResponse wresp = (System.Net.HttpWebResponse)wr.GetResponse();
                Stream s = wresp.GetResponseStream();
                System.Drawing.Image pic = System.Drawing.Image.FromStream(s);
                return (Bitmap)pic;
            }
            catch
            {
                return null;
            }
        }
    }

    //辅助类
    public class NVHelper
    {
        /// <summary>
        /// 获取数组元素
        /// </summary>
        /// <param name="Arr"></param>
        /// <param name="ReadIndex"></param>
        /// <returns></returns>
        public string getIndex(string[] Arr, int ReadIndex)
        {
            if (Arr == null)
            {
                return "";
            }
            if (ReadIndex < Arr.Length)
            {
                return Arr[ReadIndex];
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取分割字符窜索引处的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getSplitIndex(string str, int index)
        {
            if (str == null || str == string.Empty || str == "")
            {
                return "";
            }
            string[] sl = str.Split(new char[] { '|' }, StringSplitOptions.None);
            if (index < sl.Length)
            {
                return sl[index];
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取Url参数的值
        /// </summary>
        /// <param name="keyname"></param>
        /// <returns></returns>
        public string getUrlParamter(string keyname, string format)
        {
            string value = HttpContext.Current.Request.QueryString[keyname] + "";
            if (value == "")
            {
                return "";
            }
            return String.Format(format, value);
        }

        /// <summary>
        /// 获取时间格式
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string getDateFormat(object dt, string format)
        {
            try
            {
                if (dt == null)
                {
                    return "";
                }
                else
                {
                    return Convert.ToDateTime(dt).ToString(format);
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取URL中GET参数集合,并添加或修改指定的键值对
        /// </summary>
        /// <param name="keyname"></param>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public string getUrlParamters(string keyname, string keyvalue)
        {
            System.Collections.Specialized.NameValueCollection keys = HttpContext.Current.Request.QueryString;
            string result = "";
            for (int i = 0; i < keys.AllKeys.Length; i++)
            {
                if (keys[i] != "")
                {
                    if (keys.GetKey(i) == keyname)
                    {
                        if (keyvalue != "")
                        {
                            result += "&" + keys.GetKey(i) + "=" + keyvalue;
                        }
                    }
                    else
                    {
                        result += "&" + keys.GetKey(i) + "=" + keys[i];
                    }
                }
            }
            if (keyname != "" && keyvalue != "" && !keys.AllKeys.Contains(keyname))
            {
                result += "&" + keyname + "=" + keyvalue;
            }
            return result.TrimStart(new char[] { '&' });
        }

        /// <summary>
        /// 获取URL中GET参数集合,并添加或修改指定的键值对
        /// </summary>
        /// <param name="keyname"></param>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public string getQueryString(string keynames, string splt)
        {
            System.Collections.Specialized.NameValueCollection keys = HttpContext.Current.Request.QueryString;
            string[] names = keynames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string result = splt + "";
            for (int i = 0; i < names.Length; i++)
            {
                if ((keys[names[i]] + "").Trim() != "")
                {
                    result += names[i] + "=" + (keys[names[i]] + "").Trim() + "&";
                }
            }

            return result.TrimEnd(new char[] { '&' });
        }

        public string UrlEncode(string src)
        {
            return System.Web.HttpUtility.UrlEncode(src).Replace("+", "%20");
        }

        public bool IsNullOrEmpty(string src)
        {
            if (src.Trim() == "" || src.Trim().Length == 0)
            {
                return true;
            }
            return false;
        }

        public string SubLen(string obj, int len)
        {
            obj = (obj + "").Trim();
            if (obj.Length <= len)
            {
                return obj;
            }
            return obj.Substring(0, len) + "...";
        }

        public string GetIP(string ip)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("/Content/ipdata/ip.dat");
                IPLocation objScan = new IPLocation();
                return objScan.GetIPLocation(path, ip);
            }
            catch (Exception)
            {
                return "转换错误";
            }
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        /// <returns></returns>
        public string GetPlug(string fun)
        {
            return Comm.Core.core.InvokeMethod("Comm", fun) + "";
        }
    }
}