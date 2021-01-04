using System;
using System.Collections.Generic;
using System.Globalization;

public static class TimeUtils
{
    private static Dictionary<object, long> _freq_last_times = new Dictionary<object, long>();
    public const long TicksPerMs = 0x2710L;

    public static bool CheckFreq(object key, int duration)
    {
        long num = 0L;
        if (!_freq_last_times.TryGetValue(key, out num))
        {
            num = 0L;
        }
        long num2 = DateTime.Now.Ticks / 0x2710L;
        if ((num2 - num) > duration)
        {
            _freq_last_times[key] = num2;
            return true;
        }
        return false;
    }

    public static string DateTime2DateStr(DateTime dt)
    {
        return string.Format("{0:yyyy-MM-dd}", dt);
    }

    public static string DateTime2DateTimeStr(DateTime dt)
    {
        return string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
    }

    public static DateTime DateTimeStr2DateTime(string str)
    {
        return DateTime.Parse(str);
    }

    public static string FormatBy_HHMMSS(int seconds)
    {
        return FormatBy_HHMMSS(seconds, true);
    }

    public static string FormatBy_HHMMSS(int seconds, bool hasHour)
    {
        if (seconds < 0)
        {
            seconds = 0;
        }
        int num = seconds / 0xe10;
        int num2 = (seconds % 0xe10) / 60;
        seconds = (seconds % 0xe10) % 60;
        if (hasHour)
        {
            string[] textArray1 = new string[] { num.ToString("D2"), ":", num2.ToString("D2"), ":", seconds.ToString("D2") };
            return string.Concat(textArray1);
        }
        return (num2.ToString("D2") + ":" + seconds.ToString("D2"));
    }

    public static long GetClientTimeMs()
    {
        return (DateTime.Now.Ticks / 0x2710L);
    }

    public static DateTime GetDateTimeByHHMM(string str)
    {
        DateTimeFormatInfo provider = new DateTimeFormatInfo {
            ShortTimePattern = "HH:mm"
        };
        return Convert.ToDateTime(str, provider);
    }

    public static string GetRemainTime(int sec, bool allow_zero)
    {
        if (sec < 0)
        {
            if (!allow_zero)
            {
                return null;
            }
            sec = 0;
        }
        int num = sec / 0xe10;
        int num2 = (sec % 0xe10) / 60;
        sec = (sec % 0xe10) % 60;
        if (num > 0)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", num, num2, sec);
        }
        return string.Format("{0:D2}:{1:D2}", num2, sec);
    }

    public static string GetRemainTime(string time_end, bool allow_zero)
    {
        DateTime time = DateTime.Parse(time_end);
        DateTime currentServerDateTime = ServiceInfo.CurrentServerDateTime;
        TimeSpan span = (TimeSpan) (time - currentServerDateTime);
        int totalSeconds = (int) span.TotalSeconds;
        return GetRemainTime(totalSeconds, allow_zero);
    }
}

