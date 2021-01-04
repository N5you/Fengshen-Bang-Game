using System;
using UnityEngine;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using System.Collections;

/* 
 * 键盘事件功能实现
 */ 
class XKeyEventGate : XSingleton<XKeyEventGate>
{
	public bool Init()
	{
		XEventManager.SP.AddHandler(OnMainPlayerEnterGame, EEvent.MainPlayer_EnterGame);
		return true;
	}
	
	public void OnMainPlayerEnterGame(EEvent evt, params object[] args)
	{
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.Return, StartChat);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.V, ToggleSkillOperation);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.Tab, ToggleWorldMap);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.Escape, CloseTopFrame);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.Space, StartJump);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.F12, StartSit);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.B, ToggleBagWindow);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.C, ToggleRoleInformation);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.U, ToggleStrengthenUI);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.F, ToggleProductUI);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.I, ToggleAuctionUI);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.L, ToggleFriend);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.P, ToggleAuctionUI);
		
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyDown, (int)KeyCode.W, ChangeMoveDir);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyDown, (int)KeyCode.A, ChangeMoveDir);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyDown, (int)KeyCode.D, ChangeMoveDir);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyDown, (int)KeyCode.S, ChangeMoveDir);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.W, ChangeMoveDir);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.A, ChangeMoveDir);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.S, ChangeMoveDir);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_KeyUp, (int)KeyCode.D, ChangeMoveDir);
	}

	// 开始聊天or发送聊天信息
	public void StartChat()
	{	
		XEventManager.SP.SendEvent(EEvent.Chat_OrderChat);
		return;
	}
	
	// 技能界面
	public void ToggleSkillOperation()
	{
		if(FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_Skill))
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eSkillOpertation);
	}
	
	public void ToggleBagWindow()
	{
		if(FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_Bag))
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eBagWindow);
	}
	
	public void ToggleRoleInformation()
	{
		if(FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_Char))
		{
			XUIRoleInformation.CurrPlayer = XLogicWorld.SP.MainPlayer;
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eRoleInformation);
		}
	}

    public void ToggleWorldMap()
    {
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eWorldMap);
    }
	
	public void ToggleStrengthenUI()
	{
		if(FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_Strengthen))
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eStrengthenWindow);
	}
	
	public void ToggleProductUI()
	{
		if(FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_Product))
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eProduct);
	}
	
	public void ToggleAuctionUI()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eAuction);
	}
	
	public void ToggleFriend()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eFriend);
	}
	
	public void CloseTopFrame()
	{
		XDefaultFrame.CloseTopFrame();
	}
	
	public void StartJump()
	{
		XLogicWorld.SP.MainPlayer.StartJump();
	}

	public void StartSit()
	{
		XLogicWorld.SP.MainPlayer.StartSit();
	}
	
	public Vector2 getMoveDir()
	{
		Vector2 vec = Vector2.zero;
		if(Input.GetKey(KeyCode.A)) vec -= Vector2.right;
		if(Input.GetKey(KeyCode.D)) vec += Vector2.right;
		if(Input.GetKey(KeyCode.W)) vec += Vector2.up;
		if(Input.GetKey(KeyCode.S)) vec -= Vector2.up;
		return vec;
	}
	
	public void ChangeMoveDir()
	{
		XLogicWorld.SP.MainPlayer.KeyMove(getMoveDir());
	}
}
