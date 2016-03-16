using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comm;
using System.Collections.Specialized;
using DAL;

namespace BLL
{
    /// <summary>
    /// 功能函数
    /// </summary>
    public class FunctionService : BaseService
    {
        public string Sendmsg()
        {

            return Comm.Common.ToJson(new { success = true, info = "" });
        }

        /// <summary>
        /// 创建微信菜单
        /// </summary>
        /// <returns></returns>
        public string Createmenu()
        {
            string weixinid = (Request["weixinid"] + "").Trim();
            if (weixinid == "")
            {
                return Comm.Common.ToJson(new { success = false, info = "公众号不正确" });
            }
            System.Text.StringBuilder menustr = new System.Text.StringBuilder();
            List<WeiXinMenu> list = new WeiXinMenuDAO().QueryModel();
            menustr.AppendLine("{");
            menustr.AppendLine("\"button\":");
            menustr.AppendLine("[");
            foreach (WeiXinMenu item in list.Where(r => r.parentid == -1).OrderBy(r => r.sort).ToList())
            {
                menustr.AppendLine("{");
                menustr.AppendLine("\"type\":\"" + item.menuType + "\",");
                menustr.AppendLine("\"name\":\"" + item.menuName + "\",");
                menustr.AppendLine("\"key\":\"" + item.menuKey + "\",");
                menustr.AppendLine("\"url\":\"" + item.menuUrl + "\",");
                menustr.AppendLine("\"media_id\":\"" + item.menuMediaid + "\",");
                menustr.AppendLine("\"sub_button\": [");
                foreach (WeiXinMenu itm in list.Where(r => r.parentid == item.Id).OrderBy(r => r.sort).ToList())
                {
                    menustr.AppendLine("{");
                    menustr.AppendLine("\"type\":\"" + itm.menuType + "\",");
                    menustr.AppendLine("\"name\":\"" + itm.menuName + "\",");
                    menustr.AppendLine("\"key\":\"" + itm.menuKey + "\",");
                    menustr.AppendLine("\"url\":\"" + itm.menuUrl + "\",");
                    menustr.AppendLine("\"media_id\":\"" + itm.menuMediaid + "\"");
                    menustr.AppendLine("},");
                }
                if (menustr[menustr.Length - 3] == ',')
                {
                    menustr.Remove(menustr.Length - 3, 1);
                }
                menustr.AppendLine("]");
                menustr.AppendLine("},");
            }
            if (menustr[menustr.Length - 3] == ',')
            {
                menustr.Remove(menustr.Length - 3, 1);
            }
            menustr.AppendLine("]");
            menustr.AppendLine("}");
            string error;
            bool ret = new WeiXinAPI.WeixinApiFun().CreateMenu(weixinid, menustr.ToString(), out error);
            return Comm.Common.ToJson(new { success = ret, info = error });
        }
    }
}