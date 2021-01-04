using UnityEngine;
using System.Collections;

public abstract class XStateBase
{
	public int ID { get; private set; }
	public XStateMachince Machine;
	
	public XStateBase(int id)
	{
		ID = id;
	}
	
	public virtual void Enter(params object[] args){}
	public virtual void Breathe(float deltaTime){}
	public virtual bool OnEvent(int evt, params object[] args){ return false; }
	public virtual void Exit(){}
}

public abstract class XState<T> : XStateBase
{
	protected T m_Owner;
	
	public XState(int id, T owner)
		: base(id)
	{
		m_Owner = owner;
	}
}