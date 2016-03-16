using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;
using DAL;

namespace web
{
    /// <summary>
    /// 网站入口
    /// </summary>
    public class WebIndex : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"] ?? string.Empty;
            if (string.IsNullOrEmpty(action))
            {
                string html = MyHepler.GetTempleteHtml("/public/tpl/common", "index.html");
                Response.Clear();
                Response.Write(html);
            }
            else
            {
                string[] cmd = action.ToString().Split('_');
                string path = Server.MapPath("~/public/tpl/" + cmd[0] + "/");
                if (!System.IO.File.Exists(path + cmd[1] + ".html") && cmd[1] != "logout")
                {
                    Response.Redirect("/index.aspx?action=common_404");
                }
                tb_Admin m = (tb_Admin)HttpContext.Current.Session["AdminInfo"];
                if (cmd[0] == "common" && (cmd[1] == "index" || cmd[1] == "logout"))
                {
                    if (m == null || cmd[1] == "logout")
                    {
                        Session["AdminInfo"] = null;
                        Response.Redirect("/index.aspx?action=common_login");
                    }
                    var model = new
                    {
                        name = m.loginName
                    };
                    string html = Comm.MyHepler.GetTempleteHtml("/public/tpl/" + cmd[0], cmd[1] + ".html", model, "Model");
                    Response.Clear();
                    Response.Write(html);
                }
                else
                {
                    string html = Comm.MyHepler.GetTempleteHtml("/public/tpl/" + cmd[0], cmd[1] + ".html");
                    Response.Clear();
                    Response.Write(html);
                }
            }
        }
    }
}