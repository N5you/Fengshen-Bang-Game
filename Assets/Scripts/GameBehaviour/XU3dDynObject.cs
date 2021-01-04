using UnityEngine;
using resource;
using System.Collections;

public abstract class XU3dDynObject : XU3dObject
{
	public uint m_nId;
	protected XResourceBase m_DynObject;
	
	public XU3dDynObject()
	{
		m_nId = 0;
	}
	
	~XU3dDynObject()
	{

	}
	
	public abstract void RefreshDynObject(uint id);
}

