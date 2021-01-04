using System;
using UnityEngine;
using System.Collections;

public enum EBehaviourType
{
	e_BehaviourType_MainPlayer,
	e_BehaviourType_OtherPlayer,
	e_BehaviourType_Npc,
	e_BehaviourType_Monster,
	e_BehaviourType_TransPoint,
	e_BehaviourType_GatherObject,
	e_BehaviourType_Weapon,
	e_BehaviourType_OtherWeapon,
	e_BehaviourType_Terrain,
	e_BehaviourType_Other,
}

class XBehaviour : MonoBehaviour
{
	public EBehaviourType BehaType = EBehaviourType.e_BehaviourType_Other;
	
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (m_ListenerTable != null)
        {
            foreach (IBehaviourListener listener in m_ListenerTable.Values)
            {
                listener.OnControllerColliderHit(hit);
            }
        }
    }
	
	public void WeOnMouseDown(int mouseCode, Vector3 clickPoint)
	{
		if(m_ListenerTable != null)
		{
			foreach(IBehaviourListener listener in m_ListenerTable.Values)
			{
				listener.OnMouseDown(mouseCode, clickPoint);
			}
		}
	}
	
	public void WeOnMouseUp(int mouseCode)
	{
		if(m_ListenerTable != null)
		{
			foreach(IBehaviourListener listener in m_ListenerTable.Values)
			{
				listener.OnMouseUp(mouseCode);
			}
		}
	}
	
	public void WeOnMouseUpAsButton(int mouseCode)
    {
        if (m_ListenerTable != null)
        {
            foreach (IBehaviourListener listener in m_ListenerTable.Values)
            {
                listener.OnMouseUpAsButton(mouseCode);
            }
        }
    }

    public void WeOnMouseEnter()
    {
        if (m_ListenerTable != null)
        {
            foreach (IBehaviourListener listener in m_ListenerTable.Values)
            {
                listener.OnMouseEnter();
            }
        }
    }
	
	public void WeOnMouseExit()
	{
		if(m_ListenerTable != null)
		{
           	foreach (IBehaviourListener listener in m_ListenerTable.Values)
            {
                listener.OnMouseExit();
            }
		}
	}
	
	public void OnCancelSelect()
	{
		if(m_ListenerTable != null)
		{
           	foreach (IBehaviourListener listener in m_ListenerTable.Values)
            {
                listener.OnCancelSelect();
            }
		}
	}
	
    public void AddListener(IBehaviourListener listener)
    {
        if (m_ListenerTable == null)
        {
            m_ListenerTable = new Hashtable();
        }
        if (m_ListenerTable.Contains(listener))
        {
            return;
        }
        m_ListenerTable[listener] = listener;
    }

    //--4>TODO: 考虑是否需要
    private Hashtable m_ListenerTable;
}
