﻿using System;

namespace plugin
{
	public class TestA
	{
        public string printb()
        {
            return "aaa" + System.DateTime.Now.ToShortDateString();
        }

        public string printc()
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
	}
}