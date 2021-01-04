using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XStateMachince
{
	private SortedList<int, XStateBase> m_states;
	private XStateBase m_preState;
	private XStateBase m_curState;
	
	public XStateMachince(XStateBase beginState)
	{
		m_states = new SortedList<int, XStateBase>();
		m_preState = null;
		m_curState = beginState;
		RegState(m_curState);
		m_curState.Enter();
	}
	
	public void Breathe()
	{
		m_curState.Breathe(Time.deltaTime);
	}
	
	public bool OnEvent(int evt, params object[] args)
	{
		return m_curState.OnEvent(evt, args);
	}
	
	public void TranslateToState(int id, params object[] args)
	{
		int nId = (int)id;
		if(!m_states.ContainsKey(nId))
		{
			Log.Write(LogLevel.WARN, "XStateMachine, 切换状态时没有找到目标状态 {0}", id.ToString());
			return;
		}
		m_curState.Exit();
		m_preState = m_curState;
		m_curState = m_states[nId];
		m_curState.Enter(args);
	}
	
	public void RotateTo(int id, params object[] args)
	{
		int nId = (int)id;
		if(!m_states.ContainsKey(nId))
		{
			Log.Write(LogLevel.WARN, "XStateMachine, 切换状态时没有找到目标状态 {0}", id.ToString());
			return;
		}
		XCharStateMove state = (XCharStateMove)m_states[nId];
		state.rotateTo(args);
	}
	
	public void RevertToPreState()
	{
		if(null == m_preState)
			return;
		TranslateToState(m_preState.ID);
	}
	
	public void RegState(XStateBase state)
	{
		int id = (int)(state.ID);
		if(m_states.ContainsKey(id))
		{
			Log.Write(LogLevel.WARN, "XStateMachine, 重复注册状态 {0}", id.ToString());
			return;
		}
		m_states.Add(id, state);
		state.Machine = this;
	}
	
	public XStateBase CurrentState
	{
		get { return m_curState; }
	}
}

