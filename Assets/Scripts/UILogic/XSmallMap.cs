using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XSmallMap")]
public class XSmallMap : XUIBaseLogic
{
	public UIImageButton	BtnMap;
	public UIImageButton	BtnSound;
	public UIImageButton	BtnMail;
	public UILabel			mailTipLabel;
	public UISprite			mailTipSprite;
	public UIImageButton	BtnNoticeTest;
	public UIImageButton	BtnAuction;
	
	public override bool Init()
	{
		base.Init();
		UIEventListener listenBtnMap = UIEventListener.Get(BtnMap.gameObject);
		listenBtnMap.onClick += ClickBtnMap;
		UIEventListener listenBtnSound = UIEventListener.Get(BtnSound.gameObject);
		listenBtnSound.onClick += ClickBtnSound;
		
		UIEventListener listenBtnMail = UIEventListener.Get(BtnMail.gameObject);
		listenBtnMail.onClick += ClickBtnMail;
		
		UIEventListener listenBtnNotice = UIEventListener.Get(BtnNoticeTest.gameObject);
		listenBtnNotice.onClick += ClickBtnNoticeTest;
		
		UIEventListener listenBtnAuction = UIEventListener.Get(BtnAuction.gameObject);
		listenBtnAuction.onClick += ClickBtnAuction;
		
		return true;
	}
	
	public override void Show ()
	{
		base.Show();
	}
	
	public void  UpdateMailCount(int count)
	{
		mailTipLabel.gameObject.SetActive(count!=0);
		mailTipSprite.gameObject.SetActive(count!=0);
		if ( 0 == count )
			return;
		if ( count >= 100 )
			count = 99;
		mailTipLabel.text = count.ToString();
	}
	
	public void ClickBtnMap(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle,EUIPanel.eWorldMap);
	}
	
	public void ClickBtnSound(GameObject _go)
	{
		if( 0.0f == AudioListener.volume)
		{
			AudioListener.volume = 1.0f;
		}else
		{
			AudioListener.volume = 0.0f;
		}
	}
	
	public void ClickBtnMail(GameObject _go)
	{
		mailTipLabel.gameObject.SetActive(false);
		mailTipSprite.gameObject.SetActive(false);
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle,EUIPanel.eMailBox);
		XEventManager.SP.SendEvent(EEvent.Mail_WriteMail);
	}
	
	public void ClickBtnNoticeTest(GameObject _go)
	{
		//XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemTop,171,XLogicWorld.SP.MainPlayer.Name);
		//XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Chat,171,XLogicWorld.SP.MainPlayer.Name);
		//XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid,171,XLogicWorld.SP.MainPlayer.Name);
		//XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,1);
		//XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eShengWang);
		/*if(XLogicWorld.SP.MainPlayer.GuildId == 0)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eGuildList);
		}
		else if(XLogicWorld.SP.MainPlayer.GuildId > 0)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eGuildMain);
		}
		else 
		{
			Log.Write(LogLevel.ERROR,"Guild ID ERROR");
		}*/
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eBattleFailGuide);
	}
	
	public void ClickBtnAuction(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eAuction);
	}
	
}