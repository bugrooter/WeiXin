using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Comm;
using DAL;
using web;
using System.Reflection;

namespace Web.ashx
{
    /// <summary>
    /// DataHandler 的摘要说明
    /// </summary>
    public class DataHandler : BaseAshx
    {
        public override void ProcessRequest(HttpContext context)
        {
            Response.ContentType = "text/plain";
            string action = (Request["action"] + "").ToLower().Trim();
            string result = string.Empty;
            switch (action)
            {
                case "getform":
                    result = GetForm();
                    break;
                case "querylist":
                    result = QueryList();
                    break;
                case "savedo":
                    result = SaveDo();
                    break;
                case "deldo":
                    result = DelDo();
                    break;
                case "ctrldo":
                    result = CtrlDo();
                    break;
                case "plugdo":
                    result = PlugDo();
                    break;
                case "logindo":
                    Login();
                    return;
                default:
                    result = "&nbsp;";
                    break;
            }
            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        public void Login()
        {
            Response.Clear();
            Response.ContentType = "text/json";
            string username = Request.Form["username"] + "";
            string password = Request.Form["password"] + "";
            string vercode = Request.Form["vercode"] + "";
            string rempwd = Request.Form["rempwd"] + "";
            string json = new AdminService().UserLogin(username, password, vercode, rempwd);
            Response.Write(json);
            Response.End();
        }

        /// <summary>
        /// 加载表单页面
        /// </summary>
        /// <returns></returns>
        public string GetForm()
        {
            if (!IsLogin())
            {
                return Comm.Common.ToJson(new { success = false, info = "登陆超时" });
            }
            string method = (Request.QueryString["method"] + "").Trim();
            string getid = (Request.QueryString["id"] + "").Trim() == "" ? "-1" : (Request.QueryString["id"] + "").Trim();

            if (method.StartsWith("form_"))
            {
                string tf = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(method.Replace("form_", ""));
                Assembly asm = Assembly.LoadFrom(Server.MapPath("~/Bin/BLL.dll"));
                Type t = asm.GetType("BLL." + tf + "Service");
                object obj = null;
                if (t != null)
                {
                    if ((obj = Activator.CreateInstance(t)) != null)
                    {
                        MethodInfo o = obj.GetType().GetMethod("GetFormHtml");
                        string html = o.Invoke(obj, new object[] { (object)getid }) + "";
                        return Comm.Common.ToJson(new { success = false, info = html });
                    }
                }
            }
            return Comm.Common.ToJson(new { success = false, info = "参数错误" });
        }

        /// <summary>
        /// 加载列表页面
        /// </summary>
        /// <returns></returns>
        public string QueryList()
        {
            if (!IsLogin())
            {
                return Comm.Common.ToJson(new { success = false, info = "登陆超时" });
            }
            string method = Request.Form["method"] + "";
            string cuepage = Request.Form["curpage"] + "";
            string pagesize = Request.Form["pagesize"] + "";
            if (method.StartsWith("list_"))
            {
                string tf = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(method.Replace("list_", ""));
                Assembly asm = Assembly.LoadFrom(Server.MapPath("~/Bin/BLL.dll"));
                Type t = asm.GetType("BLL." + tf + "Service");
                object obj = null;
                if (t != null)
                {
                    if ((obj = Activator.CreateInstance(t)) != null)
                    {
                        MethodInfo o = obj.GetType().GetMethod("GetListHtml");
                        string html = o.Invoke(obj, new object[] { (object)cuepage, (object)pagesize }) + "";
                        return Comm.Common.ToJson(new { success = false, info = html });
                    }
                }
            }
            return Comm.Common.ToJson(new { success = false, info = "参数错误" });
        }

        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <returns></returns>
        public string SaveDo()
        {
            if (!IsLogin())
            {
                return Comm.Common.ToJson(new { success = false, info = "登陆超时" });
            }
            string method = (Request.QueryString["method"] + "").Trim();
            if (method.StartsWith("sav_"))
            {
                string tf = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(method.Replace("sav_", ""));
                Assembly asm = Assembly.LoadFrom(Server.MapPath("~/Bin/BLL.dll"));
                Type t = asm.GetType("BLL." + tf + "Service");
                object obj = null;
                if (t != null)
                {
                    if ((obj = Activator.CreateInstance(t)) != null)
                    {
                        MethodInfo o = obj.GetType().GetMethod("Save");
                        string html = o.Invoke(obj, new object[] { (object)Request.Form }) + "";
                        return html;
                    }
                }
            }
            return Comm.Common.ToJson(new { success = false, info = "参数无效" });
        }

        /// <summary>
        /// 删除列表数据
        /// </summary>
        /// <returns></returns>
        public string DelDo()
        {
            if (!IsLogin())
            {
                return Comm.Common.ToJson(new { success = true, info = "登陆超时" });
            }
            string method = Request.Form["method"] + "";
            string id = Request.Form["id"] + "";
            if (method.StartsWith("del_"))
            {
                string tf = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(method.Replace("del_", ""));
                Assembly asm = Assembly.LoadFrom(Server.MapPath("~/Bin/BLL.dll"));
                Type t = asm.GetType("BLL." + tf + "Service");
                object obj = null;
                if (t != null)
                {
                    if ((obj = Activator.CreateInstance(t)) != null)
                    {
                        MethodInfo o = obj.GetType().GetMethod("Del");
                        string html = o.Invoke(obj, new object[] { (object)id }) + "";
                        return html;
                    }
                }
            }
            return Comm.Common.ToJson(new { success = false, info = "参数无效" });
        }

        public string CtrlDo()
        {
            if (!IsLogin())
            {
                return Comm.Common.ToJson(new { success = true, info = "登陆超时" });
            }
            string function = Request.Form["method"] + "";
            Assembly asm = Assembly.LoadFrom(Server.MapPath("~/Bin/BLL.dll"));
            Type t = asm.GetType("BLL.FunctionService");
            object obj = null;
            if (t != null)
            {
                if ((obj = Activator.CreateInstance(t)) != null)
                {
                    string fun = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(function);
                    MethodInfo o = obj.GetType().GetMethod(fun);
                    string html = o.Invoke(obj, null) + "";
                    return html;
                }
            }
            return Comm.Common.ToJson(new { success = false, info = "参数无效" });
            //switch (method.Trim())
            //{
            //    case "sendmsg":
            //        {
            //            return Comm.Common.ToJson(new { success = true, info = "" });
            //        }
            //    case "createmenu":
            //        {
            //            string weixinid = (Request["weixinid"] + "").Trim();
            //            if (weixinid == "")
            //            {
            //                return Comm.Common.ToJson(new { success = false, info = "公众号不正确" });
            //            }
            //            System.Text.StringBuilder menustr = new System.Text.StringBuilder();
            //            List<WeiXinMenu> list = new WeiXinMenuDAO().QueryModel();
            //            menustr.AppendLine("{");
            //            menustr.AppendLine("\"button\":");
            //            menustr.AppendLine("[");
            //            foreach (WeiXinMenu item in list.Where(r => r.parentid == -1).OrderBy(r => r.sort).ToList())
            //            {
            //                menustr.AppendLine("{");
            //                menustr.AppendLine("\"type\":\"" + item.menuType + "\",");
            //                menustr.AppendLine("\"name\":\"" + item.menuName + "\",");
            //                menustr.AppendLine("\"key\":\"" + item.menuKey + "\",");
            //                menustr.AppendLine("\"url\":\"" + item.menuUrl + "\",");
            //                menustr.AppendLine("\"media_id\":\"" + item.menuMediaid + "\",");
            //                menustr.AppendLine("\"sub_button\": [");
            //                foreach (WeiXinMenu itm in list.Where(r => r.parentid == item.Id).OrderBy(r => r.sort).ToList())
            //                {
            //                    menustr.AppendLine("{");
            //                    menustr.AppendLine("\"type\":\"" + itm.menuType + "\",");
            //                    menustr.AppendLine("\"name\":\"" + itm.menuName + "\",");
            //                    menustr.AppendLine("\"key\":\"" + itm.menuKey + "\",");
            //                    menustr.AppendLine("\"url\":\"" + itm.menuUrl + "\",");
            //                    menustr.AppendLine("\"media_id\":\"" + itm.menuMediaid + "\"");
            //                    menustr.AppendLine("},");
            //                }
            //                if (menustr[menustr.Length - 3] == ',')
            //                {
            //                    menustr.Remove(menustr.Length - 3, 1);
            //                }
            //                menustr.AppendLine("]");
            //                menustr.AppendLine("},");
            //            }
            //            if (menustr[menustr.Length - 3] == ',')
            //            {
            //                menustr.Remove(menustr.Length - 3, 1);
            //            }
            //            menustr.AppendLine("]");
            //            menustr.AppendLine("}");
            //            string error;
            //            bool ret = new WeiXinAPI.WeixinApiFun().CreateMenu(weixinid, menustr.ToString(), out error);
            //            return Comm.Common.ToJson(new { success = ret, info = error });
            //        }
            //    default:
            //        return Comm.Common.ToJson(new { success = false, info = "参数无效" });
            //}
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        /// <returns></returns>
        public string PlugDo()
        {
            string classname = Request["class"] + "";
            string function = Request["method"] + "";
            return Comm.Core.core.InvokeMethod(classname, function) + "";
        }
    }
}