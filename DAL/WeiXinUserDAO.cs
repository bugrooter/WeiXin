using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;

namespace DAL
{
    public class WeiXinUserDAO : BaseDAO<WeiXinUser>
    {

        public WeiXinUser GetModel(long id)
        {
            try
            {
                return DAO.WeiXinUser.Where(r => r.uId == id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public WeiXinUser GetModelByOpenId(string openid)
        {
            try
            {
                return DAO.WeiXinUser.Where(r => r.openid == openid).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<WeiXinUser> QueryModel(int curpage, int pagesize, out int total)
        {
            List<WeiXinUser> list = null;
            list = DAO.WeiXinUser.OrderByDescending(r => r.adddate).ToList();
            if (list == null)
            {
                total = 0;
                return null;
            }
            string where = (HttpContext.Current.Request.Form["sWhere"] + "").Trim();
            if (where != "")
            {
                list = list.Where(r => r.nickname != null && r.nickname.Contains(where)).ToList();
            }
            total = list.Count();
            if (curpage * pagesize > total)
            {
                curpage = total / pagesize;
                curpage = (total % pagesize > 0) ? curpage + 1 : curpage;
            }

            list = list.OrderByDescending(r => r.adddate).Skip((curpage < 1 ? 1 : curpage - 1) * pagesize).Take(pagesize).ToList();
            return list.ToList();
        }

        public bool Del(long id, out string error)
        {
            try
            {
                error = "";
                WeiXinUser imodel = DAO.WeiXinUser.Where(r => r.uId == id).FirstOrDefault();
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

        public bool DelByOpenID(string openid)
        {
            try
            {
                WeiXinUser imodel = DAO.WeiXinUser.Where(r => r.openid == openid).FirstOrDefault();
                if (imodel == null)
                {
                    return false;
                }
                DAO.DeleteObject(imodel);
                DAO.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddOrUpdate(WeiXinUser model)
        {
            try
            {
                if (model.uId == -1)
                {
                    DAO.AddToWeiXinUser(model);
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