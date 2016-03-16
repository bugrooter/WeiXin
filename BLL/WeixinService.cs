using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;
using System.Collections.Specialized;
using DAL;

namespace BLL
{
    public class WeixinService : BaseService
    {
        private WeiXinConfigDAO dao = new WeiXinConfigDAO();
        public WeixinService()
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

            List<WeiXinConfig> list = dao.QueryModel(_curpage, _pagesize, out total);

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
                @Title = "公众号列表",
                @PageCount = pagecount,
                @TotalCount = total,
                @CurPage = _curpage,
                @FormParam = new
                {
                    @WhereName = (Request.Form["sWhere"] + "").Trim(),
                },
                @DataList = list
            };
            string shtml = MyHepler.GetTempleteHtml("/public/tpl/" + className, className + "List.html", model);
            return shtml;
        }

        public string GetFormHtml(string id)
        {
            WeiXinConfig imodel = dao.GetModel(id.Trim() == "" ? 0 : long.Parse(id.Trim()));
            if (imodel == null)
            {
                imodel = new WeiXinConfig();
                imodel.wId = -1;
                imodel.IsApply = false;
            }
            var model = new
            {
                //网页标题
                @Title = id.Trim() == "" ? "新增公众号" : "编辑公众号",
                @DataModel = imodel
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
                WeiXinConfig model = null;

                if (ConvertType.GetLong((Request.Form["wId"] + "").Trim(), -1) == -1)
                {
                    model = new WeiXinConfig();
                    model.wId = -1;
                    model.IsApply = false;
                }
                else
                {
                    model = dao.GetModel(ConvertType.GetLong((Request.Form["wId"] + "").Trim(), -1));
                }
                model.WeiXinName = (Request.Form["WeiXinName"] + "").Trim();
                model.WeiXinID = (Request.Form["WeiXinID"] + "").Trim();
                model.AppID = (Request.Form["AppID"] + "").Trim();
                model.AppSecret = (Request.Form["AppSecret"] + "").Trim();
                model.URL = (Request.Form["URL"] + "").Trim();
                model.Token = (Request.Form["Token"] + "").Trim();
                model.EncodingAESKey = (Request.Form["EncodingAESKey"] + "").Trim();
                model.EnMode = (int?)ConvertType.GetLong((Request.Form["EnMode"] + "").Trim(), 0);
                model.IsApply = ConvertType.GetBoolean((Request.Form["IsApply"] + "").Trim(), false);
                model.loginId = ((tb_Admin)Session["AdminInfo"]).loginId;
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