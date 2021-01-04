using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTObjectHead : XUICtrlTemplate<XObjectHead>
{
	private delegate void OnOriginalReady(XObjectHead head);
	private List<XObjectModel> m_Reqers;
	
	public XUTObjectHead()
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
		om._onObjectHeadReady(OriginalUI);
		return true;
	}
	
	public override void OnOriginal(object arg)
	{
		base.OnOriginal(arg);
		foreach(XObjectModel om in m_Reqers)
		{
			om._onObjectHeadReady(OriginalUI);
		}
		m_Reqers.Clear();
	}
}

class XUIPlayerHead : XUTObjectHead
{
}

class XUINpcHead : XUTObjectHead
{
}

class XUIMonsterHead : XUTObjectHead
{
}
