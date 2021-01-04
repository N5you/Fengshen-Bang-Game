using System;
using UnityEngine;

//战斗场景中的一个对象
public class XBattleObject : XPlayer
{
	protected XBattlePosition m_curBattlePos = null;
	
	public bool m_bIsSummon = false;
	
	public XBattlePosition curBattlePos{
		get{return m_curBattlePos; }
		set{m_curBattlePos = value; }
	}
	
	public XBattleObject()
		: base(0)
	{
		
	}
	
	protected override void OnDead()
	{
		if(true==m_bIsSummon)
			return;
		
		BattleDisplayerMgr.SP.DeadSoul(this );
	}
	
	public bool IsFullMorale {get;set;}	
}

public class XBattlePlayer : XBattleObject
{
	private uint m_uiSoulValue = 0;
	
	public uint SoulValue{
		get{return m_uiSoulValue; }
		
		set{
			m_uiSoulValue = value;
			
			m_uiSoulValue = Math.Min(m_uiSoulValue,12 );
			
		}
	}
	
	public XBattlePlayer(){
		ObjectType = EObjectType.OtherPlayer;
	}
	
}

public class XBattleMonster : XBattleObject
{
	public XBattleMonster()
	{
		ObjectType = EObjectType.Monster;
	}
}

public class XBattlePet : XBattleObject
{
	public XBattlePet()
	{
		ObjectType = EObjectType.Pet;
	}
}
