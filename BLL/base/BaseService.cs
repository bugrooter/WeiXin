using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BLL
{
    public class BaseService
    {
        protected string className = string.Empty;
        public BaseService()
        {
            className = this.GetType().Name.Replace("Service", "").ToLower();
        }

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