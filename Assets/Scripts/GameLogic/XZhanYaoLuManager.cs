using UnityEngine;
using System;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;

public class XZhanYaoLuManager : XSingleton<XZhanYaoLuManager>
{
	private int m_GuanKaID;
	private int m_ComboCnt;
	private int m_LeftFightCnt;
	private float m_LeftCDTime;
	private int m_MaxComboCnt;
	private bool m_IsAllreadyKilled = false;
	
	public XZhanYaoLuManager()
	{
	}
	
	public void Breathe()
	{
		if(LeftCDTime <= 0)
			return;
		int oldTime = (int)LeftCDTime ;
		LeftCDTime -= Time.deltaTime;
	}
	
	public int GuanKaID
	{
        get
        {
            return m_GuanKaID;
        }
        set 
		{ 
			m_GuanKaID = value;
		}
	}
	
	public int ComboCnt
	{
        get
        {
            return m_ComboCnt;
        }
        set 
		{ 
			m_ComboCnt = value;
			if(m_ComboCnt > MaxComboCnt)
			{
				MaxComboCnt = m_ComboCnt;
			}
		}
	}
	
	public int MaxComboCnt
	{
        get
        {
            return m_MaxComboCnt;
        }
        set 
		{ 
			m_MaxComboCnt = value;
		}
	}
	public int LeftFightCnt
	{
        get
        {
            return m_LeftFightCnt;
        }
        set 
		{ 
			m_LeftFightCnt = value;
			
			XDailyPlaySignMgr.SP.DoHandleShowPlaySign(DailyPlayKey.DailyPlay_ZhanYaoLu, m_LeftFightCnt, 0);
		}
	}

	public float LeftCDTime
	{
        get
        {
            return m_LeftCDTime;
        }
        set 
		{ 
			m_LeftCDTime = value;
			XEventManager.SP.SendEvent(EEvent.ZYL_UpdateTime);
			
			XDailyPlaySignMgr.SP.UpdateZhanYaoLeftTime(XUtil.GetTimeStrByInt((int)m_LeftCDTime, 2), m_LeftCDTime > 0, LeftFightCnt);
		}
	}
	
	public bool IsAllreadyKilled
	{
        get
        {
            return m_IsAllreadyKilled;
        }
        set 
		{ 
			m_IsAllreadyKilled = value;
		}
	}
	
	public int HasBuyCnt{ get;set; }
	
	public void ApplyMonsterInfo()
	{
		CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ZYL_GetMonster, builder.Build());
	}
	
	public void ApplyKillMonster()
	{
		//剩余挑战次数
		if(LeftFightCnt <= 0)
		{
			AskBuyFightCnt();
			return;
		}
		//CD时间
		if(LeftCDTime > 0)
		{
			AskClearCD();
			return;
		}
		CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ZYL_StartBattle, builder.Build());
	}
	public void AskBuyFightCnt()
	{
		if(HasBuyCnt >= XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyZhylCount))
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,16);
			return;
		}
		int costYB =  (HasBuyCnt + 1) * 10;
		string str = string.Format(XStringManager.SP.GetString(520),costYB);
		UIEventListener.VoidDelegate funcBuyCnt = new UIEventListener.VoidDelegate (SubmitBuyCnt);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcBuyCnt, null, str);
	}
	
	public void AskClearCD()
	{
		int costYB = Mathf.CeilToInt(LeftCDTime / 60); ;
		string str = string.Format(XStringManager.SP.GetString(521),costYB);
		UIEventListener.VoidDelegate funcClearCD = new UIEventListener.VoidDelegate (SubmitClearCD);
		XEventManager.SP.SendEvent (EEvent.MessageBox, funcClearCD, null, str);
	}
	
	public void SubmitBuyCnt(GameObject go)
	{
		//判断条件
		if(HasBuyCnt >= 10)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,16);
			return;
		}
		int costYB =  (HasBuyCnt + 1) * 10;
		if(XLogicWorld.SP.MainPlayer.RealMoney < costYB)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,15);
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
			return;
		}
		CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ZYL_BuyCount, builder.Build());
		//购买次数加1
		HasBuyCnt += 1;
		LeftFightCnt += 1;
		XEventManager.SP.SendEvent(EEvent.ZYL_UpdateInfo);
	}
		
	public void SubmitClearCD(GameObject go)
	{
		int costYB =  Mathf.CeilToInt(LeftCDTime / 60); 
		if(XLogicWorld.SP.MainPlayer.RealMoney < costYB)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,15);
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
			return;
		}
		CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ZYL_ClearCD, builder.Build());
	}
	public void ON_SC_MonsterInfo(int guankaID)
	{
		GuanKaID = guankaID;
		LeftCDTime = 0;
		IsAllreadyKilled = false;
		XEventManager.SP.SendEvent(EEvent.ZYL_UpdateInfo);
	}
	
	public void ON_SC_BaseData(SC_ZhanYaoLu_Data msg)
	{
		if(msg == null)
			return;
		m_GuanKaID = (int)msg.CurGuanKaID;
		LeftFightCnt = (int)msg.LeftFightCnt;
		m_LeftCDTime =(float)msg.LeftRefreshTime;
		m_ComboCnt = (int)msg.ComboCnt;
		m_MaxComboCnt = (int)msg.MaxComboCnt;
		m_IsAllreadyKilled = msg.IsKilled;
		HasBuyCnt = (int)msg.LeftBuyCnt;
		XEventManager.SP.SendEvent(EEvent.ZYL_UpdateInfo);
	}
	
	public void ON_SC_Notice_UltraKil(SC_String msg)
	{
		if(msg == null)
			return;
		if(msg.Uid == 8)
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid,535,msg.Data,msg.Uid);
		else if(msg.Uid == 12)
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid,536,msg.Data,msg.Uid);
		else
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid,537,msg.Data,msg.Uid);
	}
}