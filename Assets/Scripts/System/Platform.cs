using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum LogLevel
{
	ERROR,
	WARN,
	INFO,
    DEBUG,
}

public static class Log	
{
	private static List<string>	mNetLogs = new List<string>();
	private static List<string> mTempLog = new List<string>();
	private static object _trace_lock = new object();
	
	public static bool IsLogs = false;
	public static int MAX_NET_LOGS = 20480;
	public static int MAX_TEMP_LOG = 10;
	
	public static string ObjectToString(object obj)
	{
		return obj.ToString();
	}
	
	public static void AddNetLog(string str)
	{
		str = DateTime.Now.ToLongTimeString() + " " + str;
        if (IsLogs)
        {
            Debug.Log(str);
        }
        List<string> list = mNetLogs;
        lock (list)
        {
            mNetLogs.Add(str);
            if (mNetLogs.Count > MAX_NET_LOGS)
            {
                mNetLogs.RemoveAt(0);
            }
        }
	}
	
	public static void ClearTempLog()
	{
		mTempLog.Clear();
	}
	
	public static string GetNetLog()
	{
		string[] strArray = null;
        List<string> list = mNetLogs;
        lock (list)
        {
            strArray = mNetLogs.ToArray();
        }
        return string.Join("\r\n", strArray);
	}
	
	public static List<string> GetTempLog()
    {
        return mTempLog;
    }
	
	public static void LogMsg(string msg)
    {
        object obj2 = _trace_lock;
        lock (obj2)
        {
            Console.WriteLine("{0}: LOGMSG[{1}]: {2}", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, msg);
        }
    }

    public static void LogMsg(string msg, int i)
    {
        LogMsg(msg);
    }

    public static void RemoveTempLog(string log)
    {
        if (mTempLog.Contains(log))
        {
            mTempLog.Remove(log);
        }
    }
	
	public static void AddTempLog(params object[] args)
	{
		string item = string.Join(",", Array.ConvertAll<object, string>(args, new Converter<object,string>(ObjectToString)));
        mTempLog.Add(item);
        if (mTempLog.Count > MAX_TEMP_LOG)
        {
            mTempLog.RemoveAt(0);
        }
	}
	
	
//    [Conditional("ENABLE_TRACE")]
//    public static void TraceEnter(string name)
//    {
//        object obj2 = _trace_lock;
//        lock (obj2)
//        {
//            Console.WriteLine("{0} =>{1}", DateTime.Now.ToLongTimeString(), name);
//        }
//    }
//
//    [Conditional("ENABLE_TRACE")]
//    public static void TraceLeave(string name)
//    {
//        object obj2 = _trace_lock;
//        lock (obj2)
//        {
//            Console.WriteLine("{0} <={1}", DateTime.Now.ToLongTimeString(), name);
//        }
//    }
	
    public static void Write(string format, params object[] args)
    {
        DoLog(LogLevel.DEBUG, string.Format(format, args));
    }

    public static void Write(LogLevel level, string format, params object[] args)
    {
        DoLog(level, string.Format(format, args));
    }

    public static void Write(LogLevel level, object message)
    {
        DoLog(level, message);
    }

    public static void Write(object message)
    {
        DoLog(LogLevel.DEBUG, message);
    }

	private static void DoLog(LogLevel level, object message)
	{
		//if(!Application.isEditor)
		//	return;
        switch (level)
        {
		    case LogLevel.ERROR:
                //--4>TODO: 发布后 error 也会导致游戏崩溃/退出吗? 如果是, 那取消所有 ERROR
    			Debug.LogError(message);
                break;
            case LogLevel.WARN:
    			Debug.LogWarning(message);
                break;
		    case LogLevel.INFO:
    			Debug.Log(message);
                break;
            case LogLevel.DEBUG:
                //--4>TODO: 以后需要改成判断是否为调试版本
                Debug.Log(message);
                break;
            default:
                break;
        }
	}
}