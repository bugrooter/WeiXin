using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;

namespace DAL
{
    public class WeiXinMenuDAO : BaseDAO<WeiXinMenu>
    {
        public WeiXinMenu GetModel(long id)
        {
            try
            {
                return DAO.WeiXinMenu.Where(r => r.Id == id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<WeiXinMenu> QueryModel()
        {
            List<WeiXinMenu> list = null;
            list = DAO.WeiXinMenu.OrderByDescending(r => r.sort).ToList();
            if (list == null)
            {
                return null;
            }
            string weixinid = (HttpContext.Current.Request["weixinid"] + "").Trim();
            if (weixinid != "")
            {
                list = list.Where(r => r.weixinid == weixinid).ToList();
            }

            list = list.OrderByDescending(r => r.sort).ToList();
            return list.ToList();
        }

        public bool Del(long id, out string error)
        {
            try
            {
                error = "";
                WeiXinMenu imodel = DAO.WeiXinMenu.Where(r => r.Id == id).FirstOrDefault();
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

        public bool AddOrUpdate(WeiXinMenu model)
        {
            try
            {
                if (model.Id == -1)
                {
                    DAO.AddToWeiXinMenu(model);
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