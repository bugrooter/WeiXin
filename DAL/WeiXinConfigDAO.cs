using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;

namespace DAL
{
    public class WeiXinConfigDAO : BaseDAO<WeiXinConfig>
    {

        public WeiXinConfig GetModel(long id)
        {
            try
            {
                return DAO.WeiXinConfig.Where(r => r.wId == id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public WeiXinConfig GetModelByWeiXinId(string weixinid)
        {
            try
            {
                return DAO.WeiXinConfig.Where(r => r.WeiXinID == weixinid).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<WeiXinConfig> QueryAll()
        {
            return DAO.WeiXinConfig.OrderBy(r => r.wId).ToList();
        }

        public List<WeiXinConfig> QueryModel(int curpage, int pagesize, out int total)
        {
            List<WeiXinConfig> list = null;
            list = DAO.WeiXinConfig.OrderBy(r => r.wId).ToList();
            if (list == null)
            {
                total = 0;
                return null;
            }
            string where = (HttpContext.Current.Request.Form["sWhere"] + "").Trim();
            if (where != "")
            {
                list = list.Where(r => r.WeiXinName.Contains(where)).ToList();
            }
            total = list.Count();
            if (curpage * pagesize > total)
            {
                curpage = total / pagesize;
                curpage = (total % pagesize > 0) ? curpage + 1 : curpage;
            }

            list = list.OrderByDescending(r => r.wId).Skip((curpage < 1 ? 1 : curpage - 1) * pagesize).Take(pagesize).ToList();
            return list.ToList();
        }

        public bool Del(long id, out string error)
        {
            try
            {
                error = "";
                WeiXinConfig imodel = DAO.WeiXinConfig.Where(r => r.wId == id).FirstOrDefault();
                if (imodel == null)
                {
                    error = "该记录不存在";
                    return false;
                }
                DAO.DeleteObject(imodel);
                DAO.SaveChanges();
                return true;
            }
            catch
            {
                error = "删除失败";
                return false;
            }
        }

        public bool AddOrUpdate(WeiXinConfig model)
        {
            try
            {
                if (model.wId == -1)
                {
                    DAO.AddToWeiXinConfig(model);
                }
                if (model.IsApply==true)
                {
                    WeiXinConfig tmodel = DAO.WeiXinConfig.Where(r => r.IsApply == true).FirstOrDefault();
                    if (tmodel!=null)
                    {
                        tmodel.IsApply = false;
                    }
                }
                DAO.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}