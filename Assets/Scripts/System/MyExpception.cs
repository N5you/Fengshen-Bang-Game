using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public static class MyExpception
{
    private static List<string> errList = new List<string>();

    public static  event Action<string> NewErrorEvent;

    public static List<string> GetErrorList()
    {
        return errList;
    }

    public static string GetFramesDesc(int start, int count)
    {
        StackFrame[] frames = new StackTrace().GetFrames();
        StringBuilder builder = new StringBuilder();
        for (int i = start; i < (start + count); i++)
        {
            if (i >= frames.Length)
            {
                break;
            }
            builder.AppendFormat("{0}.{1} ", frames[i].GetMethod().DeclaringType.Name, frames[i].GetMethod().Name);
        }
        return builder.ToString();
    }

    public static void OnLogCallback(string condition, string stackTrace, UnityEngine.LogType type)
    {
        condition = condition.Trim();
        stackTrace = stackTrace.Trim();
		
        if ((!string.IsNullOrEmpty(condition) || !string.IsNullOrEmpty(stackTrace)) && (((type == UnityEngine.LogType.Assert) || (type == UnityEngine.LogType.Error)) || (type == UnityEngine.LogType.Exception)))
        {
            string str = string.Empty;
            List<string> tempLog = Log.GetTempLog();
            if ((tempLog != null) && (tempLog.Count > 0))
            {
				str = string.Join(";", tempLog.ToArray());
            }
            object[] args = new object[] { type, condition, str, stackTrace };
            string item = string.Format("type: {0}, condition: {1}, tmplog:{2}, stackTrace: {3}", args);
            errList.Add(item);
            if (NewErrorEvent != null)
            {
                NewErrorEvent(item);
            }
        }
    }

    public static bool HasError
    {
        get
        {
            return (errList.Count > 0);
        }
    }
}

