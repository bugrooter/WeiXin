using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string filepath = Request.FilePath;
            if ((System.IO.Path.GetExtension(filepath) + "").ToLower() == ".html")
            {
                string phypath = Server.MapPath(filepath);
                if (!System.IO.File.Exists(phypath))
                {
                    HttpContext context = ((HttpApplication)sender).Context;
                    string querystring = string.Empty;
                    if (context.Request.ServerVariables["QUERY_STRING"] != null && context.Request.ServerVariables["QUERY_STRING"] != "")
                        querystring += "?" + context.Request.ServerVariables["QUERY_STRING"];
                    else
                    {
                        querystring = "";
                    }
                    context.RewritePath(filepath.Replace(".html", ".aspx") + querystring);
                }
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            Response.Write((ex.Message != null ? ex.Message : "") + (ex.InnerException != null ? "," + ex.InnerException.Message : ""));
            Response.End();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}