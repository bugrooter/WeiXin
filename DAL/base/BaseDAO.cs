using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL
{
    public abstract class BaseDAO<T> where T:class,new()
    {
        private dbjmtEntities dal = new dbjmtEntities();

        protected dbjmtEntities DAO { get { return dal; } }

        public List<T> QueryBySql(string strsql,params System.Data.Objects.ObjectParameter[] paramters)
        {
            var list = dal.CreateQuery<T>(strsql, paramters);
            return list == null ? null : list.ToList();
        }
    }
}