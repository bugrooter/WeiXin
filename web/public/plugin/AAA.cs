using comm;
using System;

namespace plugin
{
    public class AAA : comm.Core.Plugin
	{
        public string print()
        {
            return "aaaaaaaa"+Request["aaa"];
        }

        public string printa()
        {
            return "3213213131000000000";
        }
	}
}