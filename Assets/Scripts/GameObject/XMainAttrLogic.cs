using System;
using UnityEngine;
using XGame.Client.Packets;

/*
 * 类名: XMainPlayer
 * 功能: 维护主角的属性
 */ 
public partial class XMainPlayer : XPlayer
{
	private XAttrMainPlayer m_AttrMainPlayer = new XAttrMainPlayer();

    public long GameMoney
    {
        get { return m_AttrMainPlayer.GameMoney; }
        set
        {
			if(m_AttrMainPlayer.GameMoney != value)
			{
//				if(value > m_AttrMainPlayer.GameMoney)
//				{
//					long delta = value - m_AttrMainPlayer.GameMoney;
//					XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,507,delta);
//				}
				
	            m_AttrMainPlayer.GameMoney = value < 0 ? 0 : value;
				XEventManager.SP.SendEvent(EEvent.Attr_GameMoney, this, m_AttrMainPlayer.GameMoney);				
			}
        }
    }

    public long RealMoney
    {
        get { return m_AttrMainPlayer.RealMoney; }
        set
        {
			if(m_AttrMainPlayer.RealMoney != value)
			{
//				if(value > m_AttrMainPlayer.RealMoney)
//				{
//					long delta = value - m_AttrMainPlayer.RealMoney;
//					XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,506,delta);
//				}
				
            	m_AttrMainPlayer.RealMoney = value < 0 ? 0 : value;	
				XEventManager.SP.SendEvent(EEvent.Attr_RealMoney, this, m_AttrMainPlayer.RealMoney);
				XEventManager.SP.SendEvent(EEvent.auction_RealMoney_Change, m_AttrMainPlayer.RealMoney);
				
			}
        }
    }

    public uint Exp
    {
        get { return m_AttrMainPlayer.Exp; }
        set
        {
			if(m_AttrMainPlayer.Exp != value)
			{
//				if(value > m_AttrMainPlayer.Exp)
//				{
//					long delta = value - m_AttrMainPlayer.Exp;
//					XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,505,delta);
//				}
            	m_AttrMainPlayer.Exp = value;
				XEventManager.SP.SendEvent(EEvent.Attr_Exp, this, Exp);	
			}
        }
    }

    public uint BagSize
    {
        get { return m_AttrMainPlayer.BagSize; }
        set
        {
			if(m_AttrMainPlayer.BagSize != value)
			{
            	m_AttrMainPlayer.BagSize = value;
            	//--4>TODO: 背包界面更新
			}
        }
    }

    public uint BankSize
    {
        get { return m_AttrMainPlayer.BankSize; }
        set
        {
			if(m_AttrMainPlayer.BankSize != value)
			{
            	m_AttrMainPlayer.BankSize = value;
            	//--4>TODO: 背包界面更新
			}
        }
    }

    public uint Power
    {
        get { return m_AttrMainPlayer.Power; }
        set
        {
			if(m_AttrMainPlayer.Power != value)
			{
            	m_AttrMainPlayer.Power = value;
				XEventManager.SP.SendEvent(EEvent.Attr_Power);
			}
        }
    }

    public uint FightRemain
    {
        get { return m_AttrMainPlayer.FightRemain; }
        set
        {
			if(m_AttrMainPlayer.FightRemain != value)
			{
            	m_AttrMainPlayer.FightRemain = value;
            	//--4>TODO: 更新界面
			}
        }
    }

    public uint BattlePos
    {
        get { return m_AttrMainPlayer.BattlePos; }
        set
        {
			if(m_AttrMainPlayer.BattlePos != value)
			{
            	m_AttrMainPlayer.BattlePos = value;
				
				//battle displayerMgr recode main player pos
				BattleDisplayerMgr.SP.MainPlayerPos = XBattlePosition.Create(EBattleGroupType.eBattleGroup_Right,value);
			}
        }
    }
}
