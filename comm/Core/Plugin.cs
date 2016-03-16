using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace comm.Core
{
    public abstract class Plugin
    {
        protected HttpRequest Request
        {
            get { return HttpContext.Current.Request; }
        }

        protected HttpResponse Response
        {
            get { return HttpContext.Current.Response; }
        }

        protected System.Web.SessionState.HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        protected System.Web.HttpServerUtility Server
        {
            get { return HttpContext.Current.Server; }
        }
    }
}
