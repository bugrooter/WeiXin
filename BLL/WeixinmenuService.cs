using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;
using System.Collections.Specialized;
using DAL;

namespace BLL
{
    public class WeixinmenuService : BaseService
    {
        private WeiXinMenuDAO dao = new WeiXinMenuDAO();
        public WeixinmenuService()
        {
        }

        /// <summary>
        /// 获取列表
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

            List<WeiXinMenu> list = dao.QueryModel();
            List<WeiXinConfig> wlist = new WeiXinConfigDAO().QueryAll();
            List<WeiXinMenu> retlist = new List<WeiXinMenu>();
            foreach (WeiXinMenu item in list.Where(r => r.parentid == -1).OrderBy(r => r.sort).ToList())
            {
                retlist.Add(item);
                foreach (WeiXinMenu itm in list.Where(r => r.parentid == item.Id).OrderBy(r => r.sort).ToList())
                {
                    retlist.Add(itm);
                }
            }
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
                @Title = "公众号菜单列表",
                @PageCount = pagecount,
                @TotalCount = total,
                @CurPage = _curpage,
                @FormParam = new
                {
                    @WhereName = (Request["weixinid"] + "").Trim(),
                },
                @DataList = retlist,
                @WeiXinList = wlist
            };
            string shtml = MyHepler.GetTempleteHtml("/public/tpl/" + className, className + "List.html", model);
            return shtml;
        }

        public string GetFormHtml(string id)
        {
            WeiXinMenu imodel = dao.GetModel(id.Trim() == "" ? 0 : long.Parse(id.Trim()));
            if (imodel == null)
            {
                imodel = new WeiXinMenu();
                imodel.Id = -1;
                imodel.state = 0;
                imodel.sort = 1;
                imodel.weixinid = HttpContext.Current.Request.QueryString["weixinid"] + "";
            }
            List<WeiXinMenu> list = dao.QueryModel().Where(r=>r.parentid==-1&&r.Id!=imodel.Id).ToList();
            var model = new
            {
                //网页标题
                @Title = id.Trim() == "" ? "新增公众号菜单" : "编辑公众号菜单",
                @DataModel = imodel,
                @MenuTypeList = WeiXinAPI.WeiXinApiConfig.WeiXinMenuType,
                @MenuParentList=list
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
                WeiXinMenu model = null;

                if (ConvertType.GetLong((Request.Form["Id"] + "").Trim(), -1) == -1)
                {
                    model = new WeiXinMenu();
                    model.Id = -1;
                }
                else
                {
                    model = dao.GetModel(ConvertType.GetLong((Request.Form["Id"] + "").Trim(), -1));
                }
                model.parentid = ConvertType.GetLong((Request.Form["parentid"] + "").Trim(), -1);
                model.weixinid = form["weixinid"] + "";
                model.parentname = form["parentname"] + "";
                model.menuName = form["menuName"] + "";
                model.menuType = form["menuType"] + "";
                model.menuKey = form["menuKey"] + "";
                model.menuUrl = form["menuUrl"] + "";
                model.menuMediaid = form["menuMediaid"] + "";
                model.sort = ConvertType.GetInt((Request.Form["sort"] + "").Trim(), 1);
                model.state = ConvertType.GetInt((Request.Form["state"] + "").Trim(), 0);
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
                return Comm.Common.ToJson(new { success = false, info = "保存失败" });
            }
        }
    }
}