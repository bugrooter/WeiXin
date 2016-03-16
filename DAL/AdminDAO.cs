using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;

namespace DAL
{
    public class AdminDAO : BaseDAO<tb_Admin>
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public tb_Admin UserLogin(string username, string userpwd, out string error)
        {
            error = "";
            try
            {
                tb_Admin imodel = DAO.tb_Admin.Where(r => r.loginName == username || r.loginMail == username).FirstOrDefault();
                if (imodel == null)
                {
                    error = "用户不存在";
                    return null;
                }
                else
                {
                    if (imodel.loginState == 1)
                    {
                        error = "用户被锁定登录，请联系管理员";
                        return null;
                    }
                    else if (imodel.loginPwd != Comm.Common.GetMD5(userpwd, imodel.loginSalt))
                    {
                        error = "用户密码错误";
                        return null;
                    }
                    else
                    {
                        imodel.loginDate = DateTime.Now;
                        imodel.loginIP = Comm.Common.GetRemoteIP();
                        imodel.logintimes += 1;
                        DAO.SaveChanges();
                        return imodel;
                    }
                }
            }
            catch
            {
                error = "操作失败";
                return null;
            }
        }

        public tb_Admin GetModel(long id)
        {
            try
            {
                return DAO.tb_Admin.Where(r => r.loginId == id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<tb_Admin> QueryModel(int curpage, int pagesize, out int total)
        {
            IQueryable<tb_Admin> list = null;
            list = DAO.tb_Admin.OrderBy(r => r.loginId);
            string swhere = (HttpContext.Current.Request.Form["sWhere"] + "").Trim();
            if (swhere != "")
            {
                list = list.Where(r => r.loginName.Contains(swhere));
            }
            if (list == null)
            {
                total = 0;
                return null;
            }
            total = list.Count();
            if (curpage * pagesize > total)
            {
                curpage = total / pagesize;
                curpage = (total % pagesize > 0) ? curpage + 1 : curpage;
            }

            list = list.OrderBy(r => r.loginId).Skip((curpage < 1 ? 1 : curpage - 1) * pagesize).Take(pagesize);
            return list.ToList();
        }

        public bool Del(long id, out string error)
        {
            try
            {
                error = "";
                tb_Admin imodel = DAO.tb_Admin.Where(r => r.loginId == id).FirstOrDefault();
                if (imodel == null)
                {
                    error = "该记录不存在";
                    return false;
                }
                if (DAO.tb_Admin.Where(r => r.loginState == 0).Count() == 1)
                {
                    error = "用户组内唯一有效用户无法删除";
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

        public bool AddOrUpdate(tb_Admin model)
        {
            try
            {
                if (model.loginId == -1)
                {
                    DAO.AddTotb_Admin(model);
                }
                if (model.loginState == 1 && DAO.tb_Admin.Where(r => r.loginId != model.loginId && r.loginState == 0).Count() == 0)
                {
                    throw new Exception("唯一登录不能被禁用");
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