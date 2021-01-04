using UnityEngine;
using System.Collections.Generic;

class XUTObjectHalf : XUICtrlTemplate<XObjectHalf>
{
	private delegate void OnOriginalReady(XObjectHalf head);
	private List<XObjectModel> m_Reqers;
	
	public XUTObjectHalf()
	{
		m_Reqers = new List<XObjectModel>();
	}
	
	public override bool ReqOriginal(object arg)
	{
		XObjectModel om = (XObjectModel)arg;
		if(null == OriginalUI)
		{
			m_Reqers.Add(om);
			return base.ReqOriginal(arg);
		}
		om._onObjectHalfReady(OriginalUI);
		return true;
	}
	
	public override void OnOriginal(object arg)
	{
		base.OnOriginal(arg);
		foreach(XObjectModel om in m_Reqers)
		{
			om._onObjectHalfReady(OriginalUI);
		}
		m_Reqers.Clear();
	}
}
