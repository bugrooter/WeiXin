using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;
using System.Collections.Specialized;
using DAL;

namespace BLL
{
    public class AdminService : BaseService
    {
        private AdminDAO dao = new AdminDAO();
        public AdminService()
        {
            
        }

        /// <summary>
        /// 登陆处理
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="upwd"></param>
        /// <param name="vercode"></param>
        /// <param name="repwd"></param>
        /// <returns></returns>
        public string UserLogin(string uname, string upwd, string vercode, string repwd)
        {
            string oldcode = Session["ValidateCode"] + "";
            var cookies = Response.Cookies;
            if (cookies.AllKeys.Contains("repwd"))
            {
                HttpCookie ck = cookies.Get("repwd");
                ck.Value = repwd.Trim() == "on" ? "on" : "off";
                cookies.Set(ck);
            }
            else
            {
                HttpCookie ck = new HttpCookie("repwd", repwd.Trim() == "on" ? "on" : "off");
                ck.Expires = DateTime.Now.AddDays(1);
                ck.Path = "/";
                cookies.Add(ck);
            }
            //相同浏览器之间
            if (Session["AdminInfo"] != null)
            {
                return Common.ToJson(new { success = true, info = "" });
            }

            string error = "";
            tb_Admin madmin = null;
            if (uname.Trim() == "" || upwd.Trim() == "" || vercode.Trim() == "")
            {
                Session["AdminInfo"] = null;
                return Common.ToJson(new { success = true, info = "用户名、密码或验证码不能为空" }); ;
            }
            else if (vercode != oldcode)
            {
                Session["AdminInfo"] = null;
                return Common.ToJson(new { success = false, info = "验证码输入不正确" }); ;
            }
            madmin = dao.UserLogin(uname.Trim(), upwd.Trim(), out error);

            if (madmin != null)
            {
                //写入cookies
                if (repwd.Trim() == "on")
                {
                    if (cookies.AllKeys.Contains("username"))
                    {
                        HttpCookie ck = cookies.Get("username");
                        ck.Value = madmin.loginName;
                        ck.Expires = DateTime.Now.AddDays(1);
                        cookies.Set(ck);
                    }
                    else
                    {
                        HttpCookie ck = new HttpCookie("username", madmin.loginName);
                        ck.Expires = DateTime.Now.AddDays(1);
                        ck.Path = "/";
                        cookies.Add(ck);
                    }
                    if (cookies.AllKeys.Contains("userpwd"))
                    {
                        HttpCookie ck = cookies.Get("userpwd");
                        ck.Value = upwd;
                        ck.Expires = DateTime.Now.AddDays(1);
                        cookies.Set(ck);
                    }
                    else
                    {
                        HttpCookie ck = new HttpCookie("userpwd", upwd);
                        ck.Expires = DateTime.Now.AddMonths(1);
                        ck.Path = "/";
                        cookies.Add(ck);
                    }
                }
                else
                {
                    if (cookies.AllKeys.Contains("username"))
                    {
                        HttpCookie ck = cookies.Get("username");
                        ck.Expires = DateTime.Now.AddDays(-1);
                        cookies.Set(ck);
                    }
                    if (cookies.AllKeys.Contains("userpwd"))
                    {
                        HttpCookie ck = cookies.Get("userpwd");
                        ck.Expires = DateTime.Now.AddDays(-1);
                        cookies.Set(ck);
                    }
                }

                //保存用户信息
                Session["AdminInfo"] = madmin;
                return Common.ToJson(new { success = true, info = "" });
            }
            else
            {
                Session["AdminInfo"] = null;
                return Common.ToJson(new { success = false, info = error });
            }
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public string GetListHtml(string curpage, string pagesize)
        {
            int _curpage = 0;
            int _pagesize = 10;
            int.TryParse(curpage, out _curpage);
            int.TryParse(pagesize, out _pagesize);
            _curpage = _curpage < 1 ? 1 : _curpage;
            _pagesize = _pagesize <= 0 ? 10 : _pagesize;
            int total = 0;

            List<tb_Admin> list = dao.QueryModel(_curpage, _pagesize, out total);

            int pagecount = 0;
            pagecount = total / _pagesize;
            if (total % _pagesize > 0)
            {
                pagecount = pagecount + 1;
            }
            _curpage = _curpage > pagecount ? pagecount : _curpage;
            var model = new
            {
                //网页标题
                @Title = "系统用户列表",
                @PageCount = pagecount,
                @TotalCount = total,
                @CurPage = _curpage,
                @FormParam = new
                {
                    @AdminName = (Request.Form["sWhere"] + "").Trim(),
                },
                @AdminsList = list
            };
            string shtml = MyHepler.GetTempleteHtml("/public/tpl/" + className, className + "List.html", model);
            return shtml;
        }

        public string GetFormHtml(string id)
        {
            tb_Admin imodel = dao.GetModel(id.Trim() == "" ? 0 : long.Parse(id.Trim()));
            var model = new
            {
                //网页标题
                @Title = id.Trim() == "" ? "新增系统用户" : "编辑系统用户",
                @AdminModel = imodel
            };
            string shtml = MyHepler.GetTempleteHtml("/public/tpl/" + className, className + "Form.html", model);
            return shtml;
        }

        public string Del(string id)
        {
            try
            {
                string error = "";
                if (dao.Del(long.Parse(id), out error))
                {
                    return Comm.Common.ToJson(new { success = true, info = "删除成功" });
                }
                else
                {
                    return Comm.Common.ToJson(new { success = false, info = error });
                }
            }
            catch
            {
                return Comm.Common.ToJson(new { success = false, info = "删除发生错误" });
            }
        }

        public string Save(NameValueCollection form)
        {
            try
            {
                tb_Admin model = null;
                if (ConvertType.GetLong((Request.Form["loginId"]+"").Trim(),-1)==-1)
                {
                    model = new tb_Admin();
                    model.loginId = -1;
                    model.loginName = (form["loginName"] + "").Trim();
                    model.loginSalt = new Random(Guid.NewGuid().GetHashCode()).Next(1000, 9999) + "";
                    model.loginPwd = Comm.Common.GetMD5((form["loginPwd"] + "").Trim(), model.loginSalt);
                    model.loginState = int.Parse((form["loginState"] + "").Trim());
                    model.logintimes = 0;
                    model.loginRegDate = DateTime.Now;
                    model.loginDate = DateTime.Now;
                }
                else
                {
                    model = dao.GetModel(ConvertType.GetLong((Request.Form["loginId"] + "").Trim(), -1));
                    model.loginName = (form["loginName"] + "").Trim();
                    model.loginState = int.Parse((form["loginState"] + "").Trim());
                    if ((form["loginPwd"] + "").Trim()!=string.Empty)
                    {
                        model.loginPwd = Comm.Common.GetMD5((form["loginPwd"] + "").Trim(), model.loginSalt);
                    }
                }
                if (dao.AddOrUpdate(model))
                {
                    return Comm.Common.ToJson(new { success = true, info = "保存成功" });
                }
                else
                {
                    return Comm.Common.ToJson(new { success = false, info = "保存失败" });
                }
            }
            catch
            {
                return Comm.Common.ToJson(new { success = false, info = "保存失败"});
            }
        }
    }
}