using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Comm
{
    /// <summary>
    /// 常用函数
    /// </summary>
    public class Common
    {
        public static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="saltcode"></param>
        /// <returns></returns>
        public static string GetMD5(string source, string saltcode)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(source + saltcode);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }
            return byte2String;
        }

        /// <summary>
        /// 生成数据库配置文件
        /// </summary>
        /// <returns></returns>
        public static bool InstallWebConfig(params object[] vals)
        {
            try
            {
                //metadata=res://*/Models.DBGiftSys.csdl|res://*/Models.DBGiftSys.ssdl|res://*/Models.DBGiftSys.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=WIN-20140715HDZ;Initial Catalog=dbgiftsys;Persist Security Info=True;User ID=sa;Password=sql123456;MultipleActiveResultSets=True&quot;
                string connstr = "metadata=res://*/Models.DBGiftSys.csdl|res://*/Models.DBGiftSys.ssdl|res://*/Models.DBGiftSys.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};MultipleActiveResultSets=True\"";
                string rootpath = HttpContext.Current.Request.PhysicalApplicationPath.TrimEnd(new char[] { '\\' });
                if (!System.IO.File.Exists(rootpath + "\\Web.config"))
                {
                    return false;
                }
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.Load(rootpath + "\\Web.config");
                System.Xml.XmlNode node = xml.SelectSingleNode("configuration/connectionStrings");
                if (node != null)
                {
                    System.Xml.XmlNode cnode = node.FirstChild;
                    if (cnode != null && cnode.Attributes["name"].Value == "dbgiftsysEntities")
                    {
                        cnode.Attributes["connectionString"].Value = string.Format(connstr, vals);
                        //xml.Save(rootpath + "\\Web.config");
                        //str = xml.InnerXml;
                    }
                }
                node = xml.SelectSingleNode("configuration/appSettings");
                if (node != null)
                {
                    System.Xml.XmlNodeList nodelist = node.ChildNodes;
                    foreach (System.Xml.XmlNode item in nodelist)
                    {
                        if (item != null && item.Attributes["key"].Value == "IsInstall")
                        {
                            item.Attributes["value"].Value = "true";
                        }
                    }
                }
                xml.Save(rootpath + "\\Web.config");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 生成数据库配置文件
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> InstallDBSql(params object[] vals)
        {
            try
            {
                //metadata=res://*/Models.DBGiftSys.csdl|res://*/Models.DBGiftSys.ssdl|res://*/Models.DBGiftSys.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=WIN-20140715HDZ;Initial Catalog=dbgiftsys;Persist Security Info=True;User ID=sa;Password=sql123456;MultipleActiveResultSets=True&quot;
                string rootpath = HttpContext.Current.Request.PhysicalApplicationPath.TrimEnd(new char[] { '\\' });
                string datapath = rootpath + "\\Content\\SQLScript";
                string str = "";
                Dictionary<string, string> list = new Dictionary<string, string>();
                byte[] buffer = null;
                int total = 0;
                System.Text.Encoding charset = null;
                if (System.IO.Directory.Exists(datapath))
                {
                    string[] files = System.IO.Directory.GetFiles(datapath);
                    for (int i = 0; i < files.Length; i++)
                    {
                        System.IO.FileInfo fo = new System.IO.FileInfo(files[i]);
                        if (fo.Name.Contains('.') && fo.Name.Substring(fo.Name.LastIndexOf('.') + 1).ToLower() == "sql")
                        {
                            total = 0;
                            buffer = new byte[(int)fo.Length];
                            System.IO.StreamReader fs = fo.OpenText();
                            string sql = fs.ReadToEnd();
                            //charset = GetCodeSet(new byte[] { buffer[0], buffer[1] });
                            //string sql = charset.GetString(buffer);
                            //list.Add(fo.Name, files[i]);
                            fs.Close();
                            fs.Dispose();
                            list.Add(fo.Name, sql);
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取文件编码
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static System.Text.Encoding GetCodeSet(byte[] buffer)
        {
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return System.Text.Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                return System.Text.Encoding.Default;
            }
        }

        public static string GetDictionaryValueByKey(Dictionary<string, string> dic, string key)
        {
            if (dic.Keys.Contains(key))
            {
                return dic[key];
            }
            return "";
        }

        public static string GetDictionaryKeyByValue(Dictionary<string, string> dic, string val)
        {
            KeyValuePair<string, string> key = dic.Where(r => r.Value == val).FirstOrDefault();
            return key.Key + "";
        }

        public static string GetRemoteIP()
        {
            string remoteip = "";
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                remoteip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0];
            }
            if (string.IsNullOrEmpty(remoteip))
            {
                remoteip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(remoteip))
            {
                remoteip = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            return remoteip;
        }
    }
}