using comm;
using DAL;
using System.Data.Entity;

namespace plugin
{
    public class Comm : comm.Core.Plugin
    {
        public string print()
        {
            string aa = "";
            System.Collections.Generic.List<DAL.WeiXinConfig> a = new DAL.WeiXinConfigDAO().QueryAll();
            for (int i = 0; i < a.Count; i++)
            {
                aa += "<li>" + a[i].WeiXinName +a[i].WeiXinID+ "</li>";
            }
            return aa;
        }
    }
}