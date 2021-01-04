using System;
using UnityEngine;

class XUtil
{
    public static readonly char[] CONFIG_VECTOR3_SEPARATOR = new char[]{';'};
    public static Vector3 String2Vector3(string strPoint)
    {
        Vector3 pt = new Vector3();
        if (string.IsNullOrEmpty(strPoint) == false)
        {
            string[] sepStr = strPoint.Split(CONFIG_VECTOR3_SEPARATOR);
            try
            {
                pt.x = sepStr.Length > 0 ? float.Parse(sepStr[0]) : 0f;
                pt.y = sepStr.Length > 1 ? float.Parse(sepStr[1]) : 0f;
                pt.z = sepStr.Length > 2 ? float.Parse(sepStr[2]) : 0f;
            }
            catch (System.Exception ex)
            {
                Log.Write(LogLevel.WARN, "String2Vector3 for {0}: {1}", strPoint, ex.ToString());
            }
        }
        return pt;
    }

    public static float CalcDistanceXZ(Vector3 pos1, Vector3 pos2)
    {
        return new Vector3(pos1.x - pos2.x, 0, pos1.z - pos2.z).magnitude;
    }

    public static bool IsInRange<T>(T value, T vBegin, T vEnd) where T : IComparable
    {
        return value.CompareTo(vBegin) >= 0 && value.CompareTo(vEnd) < 0;
    }

    public static bool NotInRange<T>(T value, T vBegin, T vEnd) where T : IComparable
    {
        return !IsInRange<T>(value, vBegin, vEnd);
    }

    public static readonly float DEFAULT_TRIGGER_DISTANCE = 3.0f;
    public static bool IsTrigger(Vector3 pos1, Vector3 pos2)
    {
        return IsTrigger(pos1, pos2, DEFAULT_TRIGGER_DISTANCE);
    }

    public static bool IsTrigger(Vector3 posTarget)
    {
        return IsTrigger(posTarget, DEFAULT_TRIGGER_DISTANCE);
    }

    public static bool IsTrigger(Vector3 posTarget, float dist)
    {
        if (XLogicWorld.SP.MainPlayer == null)
        {
            return false;
        }
        return IsTrigger(posTarget, XLogicWorld.SP.MainPlayer.Position, dist);
    }

    public static bool IsTrigger(Vector3 pos1, Vector3 pos2, float dist)
    {
        return CalcDistanceXZ(pos1, pos2) <= dist;
    }
	
	public static T Instantiate<T>(T original) where T : MonoBehaviour
	{
		if(null == original) 
			return null;
		
		GameObject go = Instantiate(original.gameObject);
		if(null == go)
			return null;
		
		return go.GetComponent<T>();
	}
	
	public static GameObject Instantiate(GameObject original, Transform parent, Vector3 localPosition, Vector3 localRocation)
	{
		if(null == original) return null;
		GameObject go = GameObject.Instantiate(original) as GameObject;
		go.name = original.name;
		go.transform.parent = parent;
		go.transform.localPosition = localPosition;
		go.transform.localRotation = Quaternion.Euler(localRocation);
		return go;
	}
	
	public static GameObject Instantiate(GameObject original, Transform parent)
	{
		return Instantiate(original, parent, Vector3.zero, Vector3.zero);
	}
	
	public static GameObject Instantiate(GameObject original)
	{
		GameObject go = Instantiate(original, original.transform.parent, original.transform.localPosition, original.transform.localRotation.eulerAngles);
		go.transform.localScale = original.transform.localScale;
		return go;
	}
	
	public static void SetLayer(GameObject go, int layer)
	{
		Transform[] trans = go.GetComponentsInChildren<Transform>(true);
		for(int i=0; i<trans.Length; i++) trans[i].gameObject.layer = layer;
	}
	
	// 给总秒数, 返回时间描述字符串, n为单位个数(用来忽略尾数)
	public static string GetTimeStrByInt(int val, int n)
	{
		int s = val % 60; val /= 60;
		int m = val % 60; val /= 60;
		int h = val % 24; val /= 24;
		
		int tn = 0;
		string str = "";
		bool bd = false;
		if(val > 0)
		{
			str += val + XStringManager.SP.GetString(1003);
			bd = true; tn++;
		}
		if(0 < n && tn >= n) return str;
		
		bool bh = false;
		if(bd || h > 0)
		{
			str += h + XStringManager.SP.GetString(1004);
			bh = true; tn++;
		}
		
		if(0 < n && tn >= n) return str;
		if(bh || m > 0)
		{
			str += m + XStringManager.SP.GetString(1005);
			tn++;
		}
		if(0 < n && tn >= n) return str;
		
		str += s + XStringManager.SP.GetString(1006);
		return str;
	}
	
}
