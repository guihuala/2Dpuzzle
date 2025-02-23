using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // 引入 TimeSpan

public static class TIMEMGR
{
    public static float oriT; // 初始标准时间
    public static float curT; // 当前时间    

    // 设置初始时间
    public static void SetOriTime()
    {
        float tempT = Time.realtimeSinceStartup;
        oriT = SaveManager.Instance.gameTime - tempT;
        SetCurTime();
    }

    // 更新当前时间
    public static void SetCurTime()
    {
        // 当前时间不能小于 0
        curT = Mathf.Max(TIMEMGR.oriT + Time.realtimeSinceStartup, 0);
        SaveManager.Instance.gameTime = curT;
    }

    // 将秒数转换为格式化的时间字符串 00:00:00
    public static string GetFormatTime(int seconds)
    {
        // 将秒数转为时间段
        TimeSpan ts = new TimeSpan(0, 0, seconds);
        return $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
    }

    // 将 8 位数字日期转换为 YYYY/MM/DD 格式
    public static void SetDate(ref string date)
    {
        date = date.Insert(4, "/");
        date = date.Insert(7, "/");
    }

    // 将 6 位数字时间转换为 HH:MM:SS 格式
    public static void SetTime(ref string time)
    {
        time = time.Insert(2, ":");
        time = time.Insert(5, ":");
    }
}