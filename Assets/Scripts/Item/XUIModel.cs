
using UnityEngine;
using System;

public class XUIModel
{
	public XUIModel()
	{	
	}
	private XU3dModel m_ObjectModel = null;
	
	
	public bool Init(XU3dModel go)
	{		
		SetObjectModel(go);
		return true;
	}
	
	private void SetObjectModel(XU3dModel go)
	{
		if(go == null)
			return ;
		
		m_ObjectModel = go;
		m_ObjectModel.PlayAnimation(EAnimName.Idle, 1.0f, false);
		
		// 更新Layer		
		m_ObjectModel.Layer = GlobalU3dDefine.Layer_UI_2D;
		return ;
	}	
	
	public void SetPosition(GameObject parent,Vector3 localPos)
	{
		if(m_ObjectModel != null)
		{
			m_ObjectModel.Parent = parent.transform ;
			m_ObjectModel.LocalPosition = localPos;
		}		
	}
	
	public void SetScale(Vector3 scale)
	{
		if(m_ObjectModel != null)
		{
			m_ObjectModel.Scale	= new Vector3(scale.x,scale.y,scale.z);
		}		
	}
	
	public void SetRotationL()
	{
		if(m_ObjectModel != null)
			m_ObjectModel.Direction	= new Vector3(m_ObjectModel.Direction.x,m_ObjectModel.Direction.y + 5.0f,m_ObjectModel.Direction.z);		
	}
	
	public void SetRotationR()
	{
		if(m_ObjectModel != null)
			m_ObjectModel.Direction	= new Vector3(m_ObjectModel.Direction.x,m_ObjectModel.Direction.y - 5.0f,m_ObjectModel.Direction.z);
	}
}
