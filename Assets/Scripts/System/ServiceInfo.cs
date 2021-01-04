using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
//using Unity3d;
using UnityEngine;

public class ServiceInfo
{
    private static string _bbs_url = string.Empty;
    private static string _cardToken = string.Empty;
    public static string _close_xunlei_tq = string.Empty;
    private static string _glIndulgeToken = string.Empty;
    private static bool _isShowRoleUI = true;
    private static string _netKey = string.Empty;
    private static bool _on_focus = true;
    private static TimeSpan _timeZoneOffset = new TimeSpan(8, 0, 0);
    private static string _url_param = string.Empty;
    private static string _voucherUrl = string.Empty;
    public static string account;
    public static string action_stat_url;
    public static int aid;
    public static string bugreport_url;
    public static int create_type = 0;
    public static bool debug;
    private static long delayMax = 0x2710L;
    public static DateTime entry_index_time;
    public static string indulge_url = string.Empty;
    public static string ip;
    public static bool IsFullScreen = false;
    public static bool IsNeedChangeScreen = false;
    public static bool isShowAnimGUI = true;
    public static string lang;
    public static string lianyun = string.Empty;
    public static int load_scene_cost_time = 0;
    public static bool MailOpen = false;
    public static string ParamTxtName = "param.txt";
    public static int player_age;
    public static int playerid;
    public static int port;
    public static string product_id = "0";
    public static string proxy_host;
    public static string proxy_ips;
    public static int real_rid = 0;
    public static string realname_url = string.Empty;
    public static string res_path;
    public static string rmb_name = "rmb";
    public static int ScreenHeight = 600;
    public static int ScreenWidth = 0x3e8;
    public const int SERVER_AID_169 = 0x26158;
    public const int SERVER_AID_S0 = 0x26157;
    public static string server_name = string.Empty;
    public static string ShellCommandArgs = string.Empty;
    public static string ShellCommandLine = string.Empty;
    public static string site_url;
    private static long time_c2s = 0L;
    private static long time_delay = 0L;
    private static TimeZoneInfo timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("kunlun", new TimeSpan(8, 0, 0), string.Empty, string.Empty);
    public static string wd_down_url;
    public static int world_level = 0;
    public static string xunlei_activity_url = string.Empty;
    public static string xunlei_vip_token = "test";

    public static long ClientTime2ServerTimeMs(long ctime)
    {
        return (ctime + time_c2s);
    }

    public static string GetLangConfig()
    {
        if (!string.IsNullOrEmpty(lang))
        {
            return lang;
        }
        if ((aid >= 0x81e20) && (aid <= 0x82207))
        {
            return "tw";
        }
        return "cn";
    }

    public static long GetNetworkDelay()
    {
        return time_delay;
    }

    public static long GetServerTimeMs()
    {
        return (TimeUtils.GetClientTimeMs() + time_c2s);
    }

    public static DateTime GetTimeFromStamp(ulong stamp)
    {
        DateTime time2 = new DateTime(0x7b2, 1, 1);
        return (time2.AddSeconds((double) stamp) + _timeZoneOffset);
    }

    public static bool IsTestServerAid(int aid)
    {
        return ((aid >= 0x26157) && (aid <= 0x2615f));
    }

    public static void OnGetServerTime(long ctime, long stime, string timezone)
    {
//        long clientTimeMs = TimeUtils.GetClientTimeMs();
//        long num2 = clientTimeMs - ctime;
//        int hours = 0;
//        int minutes = 0;
//        int seconds = 0;
//        char[] separator = new char[] { ':' };
//        string[] strArray = timezone.Split(separator);
//        hours = Utility.str2int(strArray[0]);
//        if (strArray.Length == 3)
//        {
//            minutes = Utility.str2int(strArray[1]);
//            seconds = Utility.str2int(strArray[2]);
//        }
//        _timeZoneOffset = new TimeSpan(hours, minutes, seconds);
//        timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("kunlun", _timeZoneOffset, string.Empty, string.Empty);
//        if ((num2 < 0x7d0L) || (time_delay == 0))
//        {
//            time_delay = num2;
//            time_c2s = (stime + (num2 / 2L)) - ctime;
//            string str = TimeUtils.DateTime2DateTimeStr(CurrentServerDateTime);
//            string str2 = TimeUtils.DateTime2DateTimeStr(DateTime.Now);
//        }
//        if (num2 > delayMax)
//        {
//            Control control = ControlManager.GetControl<Control>();
//            object[] args = new object[] { ctime, clientTimeMs, num2, control.CurrentPackId };
//            UEDebug.LogError(string.Format("time delay too large! time_send:{0}, time_recv:{1}, time_used:{2}, pack_id_recv:{3}", args));
//        }
    }

    public static void ParseStartupParams(string text, bool is_ini)
    {
//        StringBuilder builder = new StringBuilder();
//        builder.AppendFormat("ParseStartupParams:\n", new object[0]);
//        builder.AppendFormat("{0}\n--- params used ---\n", text);
//        System.Type type = typeof(ServiceInfo);
//        Dictionary<string, string> dictionary = !is_ini ? TextParser.ParseUrl(text) : TextParser.ParseIni(text);
//        foreach (KeyValuePair<string, string> pair in dictionary)
//        {
//            object obj2;
//            string key = pair.Key;
//            if ((key.IndexOf('.') > 0) && AppUtils.SetGlobalValue(key, pair.Value, out obj2))
//            {
//                builder.AppendFormat("  {0} = {1}\n", pair.Key, obj2);
//            }
//            FieldInfo field = type.GetField(pair.Key);
//            if (field != null)
//            {
//                obj2 = Convert.ChangeType(pair.Value, field.FieldType);
//                builder.AppendFormat("  {0} = {1}\n", pair.Key, obj2);
//                field.SetValue(null, obj2);
//            }
//            else
//            {
//                PropertyInfo property = type.GetProperty(pair.Key);
//                if (property != null)
//                {
//                    obj2 = Convert.ChangeType(pair.Value, property.PropertyType);
//                    builder.AppendFormat("  {0} = {1}\n", pair.Key, obj2);
//                    property.SetValue(null, obj2, null);
//                }
//            }
//        }
//        Debug.Log(builder.ToString());
    }

    public static long ServerTime2ClientTimeMs(long stime)
    {
        return (stime - time_c2s);
    }

    public static void SetAID(string str)
    {
        if (!int.TryParse(str, out aid))
        {
            aid = 0;
        }
    }

    public static void SetDefault()
    {
		string[] str = LogicApp.SP.ApacheServer.Split(new char[]{':'});
        ip = str[0];
		port = Convert.ToInt32(str[1]);
        debug = false;
        //res_path = "http://183.129.178.211/WebPlayer/";
		res_path = "http://" + ip + "/WebPlayer/";
        //indulge_url = "http://www.kunlun.com/?act=index.login&ref=http://www.kunlun.com/?act=passport.ogfs";
        //realname_url = "http://www.kunlun.com/?act=index.login&ref=http://www.kunlun.com/?act=passport.ogfs";
    }

    public static void SetTestParams()
    {
    }

    public static string bbs_url
    {
        get
        {
            if (!string.IsNullOrEmpty(_bbs_url))
            {
                return _bbs_url;
            }
            if ((Config.LANG != "cn") && (Config.LANG == "tw"))
            {
                return "http://bbs.kimi.com.tw/forum.php?gid=395";
            }
            return "http://bbs.kunlun.com/forum-546-1.html";
        }
        set
        {
            _bbs_url = value;
        }
    }

    public static bool CameraLockScroll
    {
        get
        {
            return MailOpen;
        }
    }

    public static string CardToken
    {
        get
        {
            return _cardToken;
        }
        set
        {
            _cardToken = value;
        }
    }

    public static string close_xunlei_tq
    {
        get
        {
            return _close_xunlei_tq;
        }
        set
        {
            _close_xunlei_tq = value;
            //EventManager.SendEvent(0x27db, null);
        }
    }

    public static string CurrentServerDataTimeStr
    {
        get
        {
            return TimeUtils.DateTime2DateTimeStr(CurrentServerDateTime);
        }
    }

    public static DateTime CurrentServerDateTime
    {
        get
        {
            return GetTimeFromStamp((ulong) CurrentServerTimeSec);
        }
    }

    public static uint CurrentServerTimeSec
    {
        get
        {
            return (uint) (GetServerTimeMs() / 0x3e8L);
        }
    }

    public static string GlIndulgeToken
    {
        get
        {
            return _glIndulgeToken;
        }
        set
        {
            _glIndulgeToken = value;
        }
    }

    public static bool IsDebugServer
    {
        get
        {
            return (ip == "58.83.173.11");
        }
    }

    public static bool IsShowRoleUI
    {
        get
        {
            return _isShowRoleUI;
        }
        set
        {
            _isShowRoleUI = value;
        }
    }

    public static string NetKey
    {
        get
        {
            return _netKey;
        }
        set
        {
            _netKey = value;
        }
    }

    public static bool OnFocus
    {
        get
        {
            return _on_focus;
        }
        set
        {
            if (_on_focus != value)
            {
                Debug.Log(string.Format("OnFocusChange, value:{0}", value));
                _on_focus = value;
                //EventManager.SendEvent(0xc383, null);
            }
        }
    }

    public static TimeSpan TimeZoneOffset
    {
        get
        {
            return _timeZoneOffset;
        }
    }

    public static string url_param
    {
        get
        {
            return _url_param;
        }
        set
        {
            _url_param = value;
            ParseStartupParams(_url_param, false);
        }
    }

    public static string voucher_url
    {
        get
        {
            if (!string.IsNullOrEmpty(_voucherUrl))
            {
                if (!_voucherUrl.Contains("?"))
                {
                    return _voucherUrl;
                }
                return (_voucherUrl + "&pid=" + product_id);
            }
            if ((Config.LANG != "cn") && (Config.LANG == "tw"))
            {
                return "http://www.kimi.com.tw/?act=voucher.main&pid=532";
            }
            return ("http://www.kunlun.com/?act=voucher.main&pid=" + product_id);
        }
        set
        {
            _voucherUrl = value;
        }
    }
}

