using System;
using System.Reflection;
using System.Text;
using UnityEngine;

public class MachineInfo
{
    public static void AutoChooseQualityLevel()
    {
        int graphicsMemorySize = SystemInfo.graphicsMemorySize;
        int graphicsShaderLevel = SystemInfo.graphicsShaderLevel;
        int num3 = 0;
        if (graphicsShaderLevel <= 10)
        {
            num3 = 0;
        }
        else if (graphicsShaderLevel <= 20)
        {
            if (graphicsMemorySize < 0x200)
            {
                num3 = 1;
            }
            else
            {
                num3 = 2;
            }
        }
        else if (graphicsShaderLevel >= 30)
        {
            if (graphicsMemorySize < 100)
            {
                num3 = 2;
            }
            else
            {
                num3 = 3;
            }
        }
        else if (graphicsShaderLevel <= 40)
        {
            num3 = 4;
        }
        else
        {
            num3 = 5;
        }
        string str = DumpSystemInfo();
        Debug.Log(string.Format("AutoChooseQualityLevel, level:{0}, SystemInfo:\r\n{1}", num3, str));
        QualitySettings.SetQualityLevel(num3);
    }

    public static string DumpSystemInfo()
    {
        StringBuilder builder = new StringBuilder();
        System.Type type = typeof(SystemInfo);
        foreach (PropertyInfo info in type.GetProperties())
        {
            if (info.Name != "deviceUniqueIdentifier")
            {
                object obj2 = null;
                try
                {
                    obj2 = info.GetValue(null, null);
                }
                catch (Exception exception)
                {
                    obj2 = exception.ToString();
                }
                builder.AppendFormat("{0}: {1}\n", info.Name, obj2);
            }
        }
        return builder.ToString();
    }
}

