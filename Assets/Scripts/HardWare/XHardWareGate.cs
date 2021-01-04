using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame.Client.Base.Pattern;

public enum EHWEventType
{
	e_HW_KeyDown = 0,
	e_HW_KeyUp,
	e_HW_MouseDown,
	e_HW_MouseUp,
	e_HW_MouseMove,
	e_HW_MouseScroll,
	e_Count,
}

/* 发射硬件消息
 * 键盘: 每个键的按下和松开
 * 鼠标: 鼠标三个键的按下/松开, 滚轮, 移动
 * 
 * 键盘状态获取:
 * 键盘: 获取键盘按键的按下状态
 */
public class XHardWareGate : XSingleton<XHardWareGate>
{
	public delegate void EventHandle();
	private SortedList<int, List<EventHandle>>[] m_Events;
	
	private int m_nLockKeyBoard = 0;
	private int m_nLockMouse = 0;
	
	public bool LockKeyBoard 
	{ 
		get { return m_nLockKeyBoard > 0; } 
		set 
		{ 
			if(value) m_nLockKeyBoard++; 
			else if(m_nLockKeyBoard > 0) m_nLockKeyBoard--;
		}
	}
	public bool LockMouse
	{
		get { return m_nLockMouse > 0; }
		set
		{
			if(value) m_nLockMouse++;
			else if(m_nLockMouse > 0) m_nLockMouse--;
		}
	}
	public bool	Lock { set { LockKeyBoard = LockMouse = value; } } 

	private float m_MouseMoveX = 0f;
	private float m_MouseMoveY = 0f;
	private float m_MouseScroll = 0f;
	
	public float MouseMoveX { get {	return LockMouse ? 0f : m_MouseMoveX; }	}
	public float MouseMoveY { get { return LockMouse ? 0f : m_MouseMoveY; } }
	public float MouseScroll { get { return LockMouse ? 0f : m_MouseScroll; } }
	
	public bool GetKey(KeyCode code) {	return LockKeyBoard ? false : Input.GetKey(code); }
	
	public XHardWareGate()
	{
		m_Events = new SortedList<int, List<EventHandle>>[(int)EHWEventType.e_Count];
		for(int i=0; i<m_Events.Length; i++)
		{
			m_Events[i] = new SortedList<int, List<EventHandle>>();
		}
	}
		
	public bool Breathe()
	{
		if(!LockKeyBoard)
		{
			// 按下Key
			SortedList<int, List<EventHandle>> list = m_Events[(int)EHWEventType.e_HW_KeyDown];
			foreach(KeyValuePair<int, List<EventHandle>> kvp in list)
			{
				if(Input.GetKeyDown((KeyCode)kvp.Key))
				{
					fireEvent(kvp.Value);
				}
			}
			
			// 松开Key
			list = m_Events[(int)EHWEventType.e_HW_KeyUp];
			foreach(KeyValuePair<int, List<EventHandle>> kvp in list)
			{
				if(Input.GetKeyUp((KeyCode)kvp.Key))
				{
					fireEvent(kvp.Value);
				}
			}
		}
		
		if(!LockMouse)
		{
			// 按下鼠标的某个键
			SortedList<int, List<EventHandle>> list = m_Events[(int)EHWEventType.e_HW_MouseDown];
			foreach(KeyValuePair<int, List<EventHandle>> kvp in list)
			{
				if(Input.GetMouseButtonDown(kvp.Key))
				{
					fireEvent(kvp.Value);
				}
			}
			
			// 松开鼠标的某个键
			list = m_Events[(int)EHWEventType.e_HW_MouseUp];
			foreach(KeyValuePair<int, List<EventHandle>> kvp in list)
			{
				if(Input.GetMouseButtonUp(kvp.Key))
				{
					fireEvent(kvp.Value);
				}
			}
			
			// 鼠标移动
			list = m_Events[(int)EHWEventType.e_HW_MouseMove];
			m_MouseMoveX = Input.GetAxis("Mouse X");
			m_MouseMoveY = Input.GetAxis("Mouse Y");
			if(0.0f != m_MouseMoveX || 0.0f != m_MouseMoveY)
			{
				foreach(KeyValuePair<int, List<EventHandle>> kvp in list)
				{
					fireEvent(kvp.Value);
				}
			}
			
			// 鼠标滚轮
			list = m_Events[(int)EHWEventType.e_HW_MouseScroll];
			m_MouseScroll = Input.GetAxis("Mouse ScrollWheel");
			if(0.0f != m_MouseScroll)
			{
				foreach(KeyValuePair<int, List<EventHandle>> kvp in list)
				{
					fireEvent(kvp.Value);
				}
			}
		}
		return true;
	}
	
	public void InitHardWareState()
	{
		// 重置鼠标右键
		SortedList<int, List<EventHandle>> list = m_Events[(int)EHWEventType.e_HW_MouseUp];
		foreach(KeyValuePair<int, List<EventHandle>> kvp in list)
		{
			if ( 1 != kvp.Key )
				continue;
			
			fireEvent(kvp.Value);
		}
	}
	
	// 注册事件, evtType->事件类型, code->事件代码, handle->事件捕捉器
	// 鼠标事件: code: 0->左键, 1->中键, 2->右键
	public void RegEvent(EHWEventType evtType, int code, EventHandle handle)
	{
		SortedList<int, List<EventHandle>> list = m_Events[(int)evtType];
		if(!list.ContainsKey(code)) list.Add(code, new List<EventHandle>());
		List<EventHandle> handles = list[code];
		handles.Add(handle);
	}
	
	// 注销事件
	public void UnRegEvent(EHWEventType evtType, int code, EventHandle handle)
	{
		SortedList<int, List<EventHandle>> list = m_Events[(int)evtType];
		if(!list.ContainsKey(code)) return;
		List<EventHandle> handles = list[code];
		handles.Remove(handle);
		if(handles.Count == 0) list.Remove(code);
	}
	
	private void fireEvent(List<EventHandle> evts)
	{
		for(int i=evts.Count-1; i>=0; i--)
		{
			if(null == evts[i])
			{
				Log.Write(LogLevel.WARN, "[WARN] XHardWareGate, 注册的事件变空了");
				evts.RemoveAt(i);
				continue;
			}
			evts[i]();
		}
	}
}

