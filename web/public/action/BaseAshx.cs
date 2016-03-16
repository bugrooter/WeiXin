using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;
using System.Web.SessionState;
using DAL;

namespace web
{
    public abstract class BaseAshx : IHttpHandler, IRequiresSessionState
    {
        public abstract void ProcessRequest(HttpContext context);

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 是否登陆
        /// </summary>
        /// <returns></returns>
        protected bool IsLogin()
        {
            tb_Admin imodel = (tb_Admin)HttpContext.Current.Session["AdminInfo"];
            if (imodel != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected HttpRequest Request
        {
            get { return HttpContext.Current.Request; }
        }

        protected HttpResponse Response
        {
            get { return HttpContext.Current.Response; }
        }

        protected System.Web.Caching.Cache Cache
        {
            get { return HttpContext.Current.Cache; }
        }

        protected System.Web.SessionState.HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        protected System.Web.HttpServerUtility Server
        {
            get { return HttpContext.Current.Server; }
        }

        public tb_Admin GetAdmin
        {
            get { return (tb_Admin)HttpContext.Current.Session["AdminInfo"]; }
        }
    }
}