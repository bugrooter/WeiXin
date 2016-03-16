using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/// <summary>
/// 基础数据类型转换
/// </summary>
public class ConvertType
{
    public static int GetInt(string obj, int defdata)
    {
        int d;
        if (!int.TryParse(obj, out d))
        {
            d = defdata;
        }
        return d;
    }

    public static decimal GetDecimal(string obj, int defdata)
    {
        decimal d;
        if (!decimal.TryParse(obj, out d))
        {
            d = defdata;
        }
        return d;
    }

    public static long GetLong(string obj, long defdata)
    {
        long d;
        if (!long.TryParse(obj, out d))
        {
            d = defdata;
        }
        return d;
    }

    public static bool GetBoolean(string obj, bool defdata)
    {
        bool d;
        if (!bool.TryParse(obj, out d))
        {
            d = defdata;
        }
        return d;
    }

    public static DateTime GetDateTime(string obj, DateTime defdata)
    {
        DateTime d;
        if (!DateTime.TryParse(obj, out d))
        {
            d = defdata;
        }
        return d;
    }
}