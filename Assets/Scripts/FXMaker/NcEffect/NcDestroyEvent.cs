using System;
using UnityEngine;

// 用于捕捉特效的销毁时机
public class NcDestroyEvent : MonoBehaviour
{
	public delegate	void OnDestroyEvt();
	
	public OnDestroyEvt onDestroyEvt = null;
	
	public void OnDestroy()
	{
		if(null != onDestroyEvt)
		{
			onDestroyEvt();
			onDestroyEvt = null;
		}
	}
}
