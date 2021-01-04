using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XTabFrame : MonoBehaviour
{
	public XDefaultFrame TgtFrame;
}

[AddComponentMenu("UILogic/XDefaultFrame")]
public class XDefaultFrame : XUIBaseLogic
{
	private static readonly float s_fFrontZ = -200f;
	private static readonly float s_fDeltaZ = -150f;
	private static List<XDefaultFrame> s_frameList = new List<XDefaultFrame>();

	public GameObject ButtonExit;
	public Collider[] TabCollider;
	
	public delegate void AfterReposition(Vector3 v);
	public AfterReposition afterReposition;
	
	public override bool Init()
	{
		base.Init();
		if(null != ButtonExit)
		{
			UIEventListener listenExit = UIEventListener.Get(ButtonExit);
			listenExit.onClick += Exit;
		}
		 for(int i=0; i<TabCollider.Length; i++)
		 {
		 	XTabFrame tabFrame = TabCollider[i].gameObject.AddComponent<XTabFrame>();
			tabFrame.TgtFrame = this;
		 }
		return true;
	}
	
	public virtual void Close()
	{
	}
	
	public override void Show()
	{
		base.Show();
		Top();
		
		if ( afterReposition != null )
			afterReposition(transform.localPosition);
	}
	
	public override void Hide()
	{
		base.Hide();
		EscFrameList();
	}
	
	public void Exit(GameObject go)
	{
		Close();
	}
	
	public static void CloseTopFrame()
	{
		if(s_frameList.Count > 0)
		{
			s_frameList[s_frameList.Count - 1].Hide();
		}
	}
	
	// 退出Frame管理
	private void EscFrameList()
	{
		for(int i=0; i<s_frameList.Count; i++)
		{
			if(s_frameList[i] == this)
			{
				for(int j=i+1; j<s_frameList.Count; j++)
				{
					if ( null != s_frameList[j] )
						s_frameList[j].Front();
				}
				s_frameList.RemoveAt(i);
				break;
			}
		}		
	}
	
	// 置顶
	public void Top()
	{
		EscFrameList();
		Vector3 vec = transform.localPosition;
		vec.z = s_fFrontZ + s_fDeltaZ * s_frameList.Count;
		transform.localPosition = vec;
		s_frameList.Add(this);
	}
	
	// z轴前进
	private void Front()
	{
		Vector3 vec = transform.localPosition;
		vec.z -= s_fDeltaZ;
		transform.localPosition = vec;
	}
	
	// z轴后退
	private void Back()
	{
		Vector3 vec = transform.localPosition;
		vec.z += s_fDeltaZ;
		transform.localPosition = vec;
	}
}

