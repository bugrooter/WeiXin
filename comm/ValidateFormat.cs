using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Comm
{
    /// <summary>
    /// 基础数据验证
    /// </summary>
    public class ValidateFormat
    {

        /// <summary>
        /// 是否整数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsInt(string obj)
        {
            int d;
            return int.TryParse(obj, out d);
        }

        public static bool IsLong(string obj)
        {
            long d;
            return long.TryParse(obj, out d);
        }

        public static bool IsFloat(string obj)
        {
            float d;
            return float.TryParse(obj, out d);
        }

        public static bool IsDateTime(string obj)
        {
            DateTime d;
            return DateTime.TryParse(obj, out d);
        }

        public static bool IsNullOrEmpty(object obj)
        {
            if (obj == null || obj == string.Empty || obj.ToString().Trim() == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}