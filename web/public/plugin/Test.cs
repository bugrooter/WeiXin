using comm;
using System;
using System.Web;

namespace plugin
{
    public class Test : comm.Core.Plugin
	{
        public string print()
        {
            return "aaaaaaaa" + Request["aaa"]; 
        }

        public string printa()
        {
            return "31321321";
        }
	}
}