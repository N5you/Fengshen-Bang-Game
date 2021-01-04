using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
//using Unity3d;
using UnityEngine;

public static class Statistics
{
    private static HashSet<string> _bugs = new HashSet<string>();
    private static bool _init;
    //private static RoleControlManager _roleControlManager;
    private static string _url;
    private static int ReportBugCount = 1;

    public static void Init(string url, int player_id)
    {
        if (!string.IsNullOrEmpty(url) && (player_id > 0))
        {
            _url = url;
            _init = true;
        }
    }

    public static void ReportBug(string content)
    {
        long playerid = ServiceInfo.playerid;
        long cid = 0L;
        string nick = string.Empty;
//        if (_roleControlManager == null)
//        {
//            _roleControlManager = ControlManager.GetControl<RoleControlManager>();
//        }
//        if (_roleControlManager != null)
//        {
//            ActorData mainRoleModel = _roleControlManager.GetMainRoleModel();
//            if (mainRoleModel != null)
//            {
//                cid = mainRoleModel.id;
//                nick = mainRoleModel.nick;
//            }
//        }
        ReportBug(playerid, cid, nick, content);
    }

    public static void ReportBug(long pid, long cid, string nick, string content)
    {
//        if (!Application.isEditor && (ReportBugCount < 15))
//        {
//            string str = Application.platform.ToString();
//            string str2 = StringUtils.GetMD5String(content);
//            WWWForm form = new WWWForm();
//            form.AddField("event", "game_exception_log");
//            form.AddField("server_id", ServiceInfo.aid);
//            form.AddField("pid", pid.ToString());
//            form.AddField("cid", cid.ToString());
//            form.AddField("nick", nick);
//            form.AddField("content", content + "\nclient:" + str);
//            form.AddField("md5", str2);
//            if (string.IsNullOrEmpty(ServiceInfo.bugreport_url))
//            {
//                ServiceInfo.bugreport_url = "http://" + Utility.GetWebStateFromUrl(ServiceInfo.action_stat_url);
//            }
//            CoroutineManager.StartCoroutine(SendRequest(ServiceInfo.bugreport_url + "/game_exception_log.php", form));
//            ReportBugCount++;
//        }
    }

    public static void ReportEnterApp(float time)
    {
        if (_init)
        {
            Hashtable data = new Hashtable();
            data.Add("cost_time", Mathf.Ceil(time).ToString());
            SendRequest("enterApp", data);
        }
    }

    public static void ReportEnterCreateRole(float time)
    {
        if (_init)
        {
            Hashtable data = new Hashtable();
            data.Add("cost_time", Mathf.Ceil(time).ToString());
            SendRequest("enterCreateRole", data);
        }
    }

    public static void ReportEnterScene(float time, int scene_id)
    {
        if (_init)
        {
            Hashtable data = new Hashtable();
            data.Add("cost_time", Mathf.Ceil(time).ToString());
            data.Add("scene_id", scene_id);
            SendRequest("enterScene", data);
        }
    }

    public static void ReportFirstEnterScene(float time, int scene_id)
    {
        if (_init)
        {
            Hashtable data = new Hashtable();
            data.Add("cost_time", Mathf.Ceil(time).ToString());
            data.Add("scene_id", scene_id);
            SendRequest("firstEnterScene", data);
        }
    }

    public static void ReportFps(int fps, int scene_id)
    {
        if (_init)
        {
            Hashtable data = new Hashtable();
            data.Add("pid", ServiceInfo.playerid);
            data.Add("fps", fps.ToString());
            data.Add("scene_id", scene_id);
            SendRequest("fps", data);
        }
    }

    public static void ReportPing(int pin)
    {
        if (_init)
        {
            string url = "http://s911.jd.kunlun.com/stat/stat_interface/index.php";
            Hashtable data = new Hashtable();
            data.Add("act", "DataStatService.statPinInfo");
            data.Add("pin", pin);
            data.Add("server_domain", ServiceInfo.proxy_host);
            SendRequest("statPinInfo", data, url);
        }
    }

    private static void SendRequest(string evt, Hashtable data)
    {
        SendRequest(evt, data, _url);
    }

//    [DebuggerHidden]
//    public static IEnumerator SendRequest(string url, WWWForm form)
//    {
//        return new <SendRequest>c__Iterator49 { url = url, form = form, <$>url = url, <$>form = form };
//    }

    private static void SendRequest(string evt, Hashtable data, string url)
    {
//        if (!string.IsNullOrEmpty(url))
//        {
//            data.Add("player_id", ServiceInfo.playerid);
//            data.Add("account", ServiceInfo.account);
//            data.Add("server_id", ServiceInfo.aid);
//            data.Add("is_micro_client", (Application.platform != RuntimePlatform.WindowsPlayer) ? 0 : 1);
//            data.Add("event", evt);
//            List<string> list = new List<string>();
//            IDictionaryEnumerator enumerator = data.GetEnumerator();
//            try
//            {
//                while (enumerator.MoveNext())
//                {
//                    DictionaryEntry current = (DictionaryEntry) enumerator.Current;
//                    list.Add(string.Format("{0}={1}", current.Key, current.Value));
//                }
//            }
//            finally
//            {
//                IDisposable disposable = enumerator as IDisposable;
//                if (disposable == null)
//                {
//                }
//                disposable.Dispose();
//            }
//            string str = string.Join("&", list.ToArray());
//            WWWForm form = new WWWForm();
//            IDictionaryEnumerator enumerator2 = data.GetEnumerator();
//            try
//            {
//                while (enumerator2.MoveNext())
//                {
//                    DictionaryEntry entry2 = (DictionaryEntry) enumerator2.Current;
//                    string fieldName = entry2.Key.ToString();
//                    object obj2 = entry2.Value;
//                    if (obj2 != null)
//                    {
//                        if (obj2 is string)
//                        {
//                            form.AddField(fieldName, (string) obj2);
//                        }
//                        else
//                        {
//                            form.AddField(fieldName, (int) obj2);
//                        }
//                    }
//                }
//            }
//            finally
//            {
//                IDisposable disposable2 = enumerator2 as IDisposable;
//                if (disposable2 == null)
//                {
//                }
//                disposable2.Dispose();
//            }
//            UnityEngine.Debug.Log(string.Format("SendRequest, url:{0}, param: {1}", url, str));
//            CoroutineManager.StartCoroutine(SendRequest(url, form));
//        }
    }

//    [CompilerGenerated]
//    private sealed class <SendRequest>c__Iterator49 : IEnumerator, IDisposable, IEnumerator<object>
//    {
//        internal object $current;
//        internal int $PC;
//        internal WWWForm <$>form;
//        internal string <$>url;
//        internal WWW <www>__0;
//        internal WWWForm form;
//        internal string url;
//
//        [DebuggerHidden]
//        public void Dispose()
//        {
//            this.$PC = -1;
//        }
//
//        public bool MoveNext()
//        {
//            uint num = (uint) this.$PC;
//            this.$PC = -1;
//            switch (num)
//            {
//                case 0:
//                    if (!string.IsNullOrEmpty(this.url))
//                    {
//                        this.<www>__0 = new WWW(this.url, this.form);
//                        this.$current = this.<www>__0;
//                        this.$PC = 1;
//                        return true;
//                    }
//                    break;
//
//                case 1:
//                    this.$PC = -1;
//                    break;
//            }
//            return false;
//        }
//
//        [DebuggerHidden]
//        public void Reset()
//        {
//            throw new NotSupportedException();
//        }
//
//        object IEnumerator<object>.Current
//        {
//            [DebuggerHidden]
//            get
//            {
//                return this.$current;
//            }
//        }
//
//        object IEnumerator.Current
//        {
//            [DebuggerHidden]
//            get
//            {
//                return this.$current;
//            }
//        }
//    }
}

