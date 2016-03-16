using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

namespace web
{
    public class BasePage : System.Web.UI.Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsLogin())
            {
                Response.Redirect("/index.aspx?action=common_login");
                return;
            }
            base.OnPreLoad(e);
        }

        /// <summary>
        /// 是否登陆
        /// </summary>
        /// <returns></returns>
        protected bool IsLogin()
        {
            tb_Admin imodel = (tb_Admin)Session["AdminInfo"];
            if (imodel != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public tb_Admin GetAdmin
        {
            get { return (tb_Admin)HttpContext.Current.Session["AdminInfo"]; }
        }
    }
}