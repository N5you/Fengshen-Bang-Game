using System;
using UnityEngine;
using XGame.Client.Packets;

class XUTMainPlayerInfo : XUICtrlTemplate<XMainPlayerInfo>
{		
	private XBuff displayBuff = null;
	private float m_rockon = 0f;
	private bool IsInit = false;
	private float MaxPowerValue = 200.0f;
	
    public XUTMainPlayerInfo()
    {
		XEventManager.SP.AddHandler(OnNameUpdate, EEvent.Attr_Name);
		XEventManager.SP.AddHandler(OnGameMoneyUpdate, EEvent.Attr_GameMoney);
		XEventManager.SP.AddHandler(OnRealMoneyUpdate, EEvent.Attr_RealMoney);
		XEventManager.SP.AddHandler(OnHpUpdate, EEvent.Attr_Hp);
		//XEventManager.SP.AddHandler(OnMainPlayerEnterGame, EEvent.MainPlayer_EnterGame);
		XEventManager.SP.AddHandler(OnDynamicAttrUpdate, EEvent.Attr_Dynamic);
		RegEventAgent_CheckCreated(EEvent.buff_OnAddBuff, OnAddBuff);
		RegEventAgent_CheckCreated(EEvent.buff_OnRemoveBuff, OnRemoveBuff);
		RegEventAgent_CheckCreated(EEvent.buff_OnSetLayer, OnSetBuffLayer);
		XEventManager.SP.AddHandler(RemoveBuff, EEvent.buff_RemoveBuff);
		XEventManager.SP.AddHandler(OnDisplayBuff, EEvent.buff_DisplayBuff);
		XEventManager.SP.AddHandler(OnStopDisplayBuff, EEvent.buff_StopDisplayBuff);
		RegEventAgent_CheckCreated(EEvent.product_Strength, OnProductStrength);
		RegEventAgent_CheckCreated(EEvent.Attr_Power, OnSetPower);
		RegEventAgent_CheckCreated(EEvent.UI_UpdateVIP, OnUpdateVIP);
		RegEventAgent_CheckCreated(EEvent.UI_UpdateBuyHealthCount, OnUpdateBuyHealthCount);
		RegEventAgent_CheckCreated(EEvent.UI_UpdateBattleValue, OnUpdateBattleValue);
		RegEventAgent_CheckCreated(EEvent.UI_EatFoodCount, OnUpdateEatFoodCount);
		
    }
	
	public override void Breathe ()
	{
		if(null == displayBuff) 
			return;
		float now = Time.time;
		if(now - m_rockon < 1f)
			return;
		m_rockon = now;
		XEventManager.SP.SendEvent(EEvent.ToolTip_B, displayBuff.GetBuffDiscription(),"","");
	}
	
	public override void OnShow()
	{
		base.OnShow();
		XLogicWorld.SP.MainPlayer.Hp	= XLogicWorld.SP.MainPlayer.MaxHp;
		ShowHead();	
	}
	
	public void ShowHead()
	{
		if(!IsInit)
		{
			IsInit	= true;
			XModelRTTMgr.SP.AddModelRTT(XLogicWorld.SP.MainPlayer.ModelId,LogicUI.RoleHeadTex,0f,-1.8f,1f);
		}
	}
	
	public override void OnCreated(object arg)
	{
		base.OnCreated(arg);
		XMainPlayer player = XLogicWorld.SP.MainPlayer;
		LogicUI.SetName(player.Name);
		LogicUI.SetLevel(player.Level);
		LogicUI.SetGold(player.RealMoney);
		LogicUI.SetSilver(player.GameMoney);
		LogicUI.SetHealth((float)player.Power / MaxPowerValue);
	}
	
	private bool ShouldProcess(object arg)
	{
        return arg == XLogicWorld.SP.MainPlayer && LogicUI;
	}

	private void OnNameUpdate(EEvent evt, params object[] args)
	{
		if(!ShouldProcess(args[0])) return;
        if (LogicUI)
        {
            LogicUI.SetName((string)args[1]);
        }
	}
	
	private void OnGameMoneyUpdate(EEvent evt, params object[] args)
	{
		if (ShouldProcess(args[0]))
        {
            LogicUI.SetSilver((long)args[1]);
			
        }
	}
	
	private void OnRealMoneyUpdate(EEvent evt, params object[] args)
	{
		if (ShouldProcess(args[0]))
        {
            LogicUI.SetGold((long)args[1]);
        }
	}
	
	private void OnHpUpdate(EEvent evt, params object[] args)
	{
		if (ShouldProcess(args[0]))
        {
            XMainPlayer player = XLogicWorld.SP.MainPlayer;
            //LogicUI.SetHealth(((float)player.Hp) / player.MaxHp);
        }
	}
	
	private void OnDynamicAttrUpdate(EEvent evt, params object[] args)
	{
        if (!ShouldProcess(args[0])) return;

        EShareAttr att = (EShareAttr)args[1];
        if (!Enum.IsDefined(typeof(EShareAttr), att))
        {
            return;
        }
		XMainPlayer player = XLogicWorld.SP.MainPlayer;
        switch (att)
        {
			case EShareAttr.esa_Level:
				LogicUI.SetLevel(player.Level);
                break;
            case EShareAttr.esa_MaxHp:
				//LogicUI.SetHealth(((float)player.Hp) / (float)player.MaxHp);
                break;
            default:
                break;
        }
		
		XEventManager.SP.SendEvent(EEvent.UI_UpdateBattleValue);
	}
		
	private void OnAddBuff(EEvent evt, params object[] args)
	{
		if(!ShouldProcess(args[0])) return;
		XBuff buff = (XBuff)(args[1]);
		if(buff.CfgBuffBase.ShowIcon != 0)
			LogicUI.AddBuffIcon((EBuffType)(buff.CfgBuffBase.BuffType), buff.BuffId, buff.BuffLayer, buff.CfgBuffBase.AtlasId, buff.CfgBuffBase.SpriteName);
	}
	
	private void OnRemoveBuff(EEvent evt, params object[] args)
	{
		if(!ShouldProcess(args[0])) return;
		XBuff buff = (XBuff)(args[1]);
		if(buff == displayBuff)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipB);
			displayBuff = null;
		}
		if(buff.CfgBuffBase.ShowIcon !=  0)
			LogicUI.RemoveBuffIcon((EBuffType)(buff.CfgBuffBase.BuffType), buff.BuffId);
	}
	
	private void OnSetBuffLayer(EEvent evt, params object[] args)
	{
		if(!ShouldProcess(args[0])) return;
		XBuff buff = (XBuff)(args[1]);
		LogicUI.SetBuffLayer((EBuffType)(buff.CfgBuffBase.BuffType), buff.BuffId, buff.BuffLayer);
	}
	
	private void OnSetPower(EEvent evt, params object[] args)
	{		
		LogicUI.SetHealth((float)XLogicWorld.SP.MainPlayer.Power/ MaxPowerValue);
	}
	
	private void OnUpdateVIP(EEvent evt, params object[] args)
	{		
		LogicUI.SetVIPLevel(XLogicWorld.SP.MainPlayer.VIPLevel);
	}
	
	private void OnUpdateBuyHealthCount(EEvent evt, params object[] args)
	{		
		LogicUI.SetVIPLevel(XLogicWorld.SP.MainPlayer.VIPLevel);
	}
	
	private void OnUpdateBattleValue(EEvent evt, params object[] args)
	{		
		LogicUI.BattleValue.text	= XLogicWorld.SP.MainPlayer.BattleValue.ToString();
	}
	
	private void OnUpdateEatFoodCount(EEvent evt, params object[] args)
	{		
		if(XLogicWorld.SP.MainPlayer.EatFoodCount >= 3)
		{
			LogicUI.SpriteFood.gameObject.SetActive(true);
		}
		else
		{
			LogicUI.SpriteFood.gameObject.SetActive(false);
		}
	}
	
	private void RemoveBuff(EEvent evt, params object[] args)
	{
		CS_UInt.Builder builder = CS_UInt.CreateBuilder();
		builder.SetData((uint)(args[0]));
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ApplyRemoveBuff, builder.Build());
	}
	
	private void OnDisplayBuff(EEvent evt, params object[] args)
	{
		uint nBuffId = (uint)(args[0]);
		XBuff buff = XLogicWorld.SP.MainPlayer.BuffOper.GetBuff(nBuffId);
		if(null == buff) return;
		displayBuff = buff;
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eToolTipB);
		XEventManager.SP.SendEvent(EEvent.ToolTip_B, displayBuff.GetBuffDiscription(),"","");	
		m_rockon = Time.time;
	}
	
	private void OnStopDisplayBuff(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipB);
	}
		
	private void OnProductStrength(EEvent evt, params object[] args)
	{
//		uint nStrength = (uint)(args[0]);
//		int nMaxStrength = (int)(args[1]);
//		float p = ((float)nStrength) / ((float)nMaxStrength);
//		LogicUI.SetStrenth(p);
	}
	
	public void SendSystemTopNotice(string content)
	{
		if(LogicUI == null)
			return ;
		
		LogicUI.NoticeAnim(content);
	}
	
	public void SendSystemMidNotice(string content,bool isNeedCut)
	{
		if(LogicUI == null)
			return ;
		
		LogicUI.MidNoticeAnim(content,isNeedCut);
	}
	
	public void ReduceSystemMidTime()
	{
		if(LogicUI == null)
			return ;
		
		LogicUI.ReduceSystemMidTime();
	}
}
