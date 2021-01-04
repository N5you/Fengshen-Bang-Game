using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XSevenTargetUI")]
public class XSevenTargetUI : XDefaultFrame
{
	class TargetItemObject
	{
		uint uiid = 0;
		private bool mCanLink = false;
		private UISprite mbutton;
		
		public TargetItemObject(int parentid, int id, UILabel label, UISprite button)
		{
			mbutton = button;
			
			label.onMouseHover += onHover;
			UIEventListener lis = UIEventListener.Get(label.gameObject);
			lis.onClick += onClickLink;
			
			button.onMouseHover += onHover;
			lis = UIEventListener.Get(button.gameObject);
			lis.onClick += onClickLink;
			
			XCfgSevenTarget target = XCfgSevenTargetMgr.SP.GetConfig((byte)parentid);
			switch ( id )
			{
			case 0:
				uiid = target.linkUI1;
				
				break;
			case 1:
				uiid = target.linkUI2;
				
				break;
			case 2:
				uiid = target.linkUI3;
				
				break;
			default:
				break;
			}
		}
		
		public void SetCanLink(bool can)
		{
			mCanLink = can;
			mbutton.gameObject.SetActive((mCanLink && 0 < uiid));
		}
		
		private void onHover(bool over)
		{
			if ( !mCanLink || 0 == uiid )
				return;
			
			if ( over )
				CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_Link);
			else
				CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_None);
		}
		
		private void onClickLink(GameObject go)
		{
			if ( !mCanLink || 0 == uiid )
				return;
			
			if ( !FeatureDataUnLockMgr.SP.IsUnLock((int)uiid) )
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 10057);
				return;
			}
			
			XEventManager.SP.SendEvent(EEvent.UI_Show, (EUIPanel)uiid);
		}
	}
	
	class SevenTarget
	{
		public SevenTarget(int id, GameObject buttion, UILabel[] label, UISprite[] button)
		{
			UIEventListener lis = UIEventListener.Get(buttion);
			lis.onClick += onClickGetAward;
			_id = id;
			
			int startIndex = (_id - 1) * 3;
			for( int i = 0; i < 3; i ++ )
			{
				items[i] = new XSevenTargetUI.TargetItemObject(id, i, label[startIndex], button[startIndex]);
				startIndex++;
			}
		}
		
		private void onClickGetAward(GameObject go)
		{
			if ( _id == 0 )
				return;
			
			XSevenTargetManager.SP.GetAwardById(_id);
		}
		
		public void SetLinkStatus(SevenTargetItem itemStatus)
		{
			items[0].SetCanLink(itemStatus.status1 == 0x00);
			items[1].SetCanLink(itemStatus.status2 == 0x00);
			items[2].SetCanLink(itemStatus.status3 == 0x00);
		}
		
		private TargetItemObject[] items = new TargetItemObject[3];
		int _id = 0;
	}
	
	public UISprite[] spriteStar;
	public UILabel[]  labelDes;
	public UISprite[]  buttonLink;
	public XActionIcon[] actionIcon;
	public UILabel[]  labelLeftTimeTitle;
	public UILabel[]  labelLeftTime;
	public UIImageButton[] buttion;
	public UILabel[] buttionText;
	public XBaseActionIcon[] awardIcons = new XBaseActionIcon[14];
	
	private SevenTarget[] targetItems = new SevenTarget[7];
	
	public override bool Init ()
	{
		base.Init();
		
		for ( int i = 0; i < targetItems.Length; i++ )
		{
			targetItems[i] = new SevenTarget(i + 1, buttion[i].gameObject, labelDes, buttonLink);
		}
		
		for ( int i = 0; i < awardIcons.Length; i++ )
		{
			awardIcons[i] = new XBaseActionIcon();
			awardIcons[i].IsCanDBClick = false;
			awardIcons[i].IsCanDrag = false;
			awardIcons[i].IsCanDrop = false;
			awardIcons[i].IsCanPopMenu = false;
			awardIcons[i].SetUIIcon(actionIcon[i]);
		}
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eSevenTarget);
	}
	
	public override void Show ()
	{
		base.Show ();
		
		for ( int i = 0; i < targetItems.Length; i++ )
		{
			UpdateOneTargetItem(i + 1);
		}
		
		checkNeedShowEffect();
		
		XSevenTargetManager.SP.GetAllSevenTarget();
	}
	
	private void checkNeedShowEffect()
	{
		bool needShowEffect = false;
		for ( int i = 1; i <= 7; i++)
		{
			SevenTargetItem item = XSevenTargetManager.SP.GetTargetItemStatus((uint)i);
			if ( 0x01 == item.status )
			{
				needShowEffect = true;
				break;
			}
		}
		if ( needShowEffect )
			XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)EFeatureID.EFeatureID_SevenTarget, 900039u, 0);
		else
			XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STOPEFFECT, (uint)EFeatureID.EFeatureID_SevenTarget, 0u);
	}
	
	public void UpdateOneTargetItem(int id)
	{
		checkNeedShowEffect();
		
		SevenTargetItem itemStatus = XSevenTargetManager.SP.GetTargetItemStatus((uint)(id));
		if ( null == itemStatus )
			return;
		
		XCfgSevenTarget target = XCfgSevenTargetMgr.SP.GetConfig((byte)(id));
		if ( null == target )
			return;
		
		targetItems[id - 1].SetLinkStatus(itemStatus);
		
		// 灰色字体172 164 141 灰色图片 11001109
		// 金色字体255 243 55 金色图片 11001108
		
		int spriteStartIndex = 3 * (id - 1);
		int labelStartIndex = 3 * (id - 1);
		int actionIconStartIndex = 2 * (id - 1);
		int imageButtionIndex = (id - 1);
		
		UInt64 passTime = XSevenTargetManager.SP.m_currTime - XSevenTargetManager.SP.m_userFirstLoginTime;
		UInt64 passDay = passTime / ( 24 * 60 * 60);
		
		bool canAward = ((int)passDay + 1 >= id);
		// 设置领取按钮状态
		if ( itemStatus.status == 0x01 && canAward )
			buttion[imageButtionIndex].isEnabled = true;
		else
			buttion[imageButtionIndex].isEnabled = false;
		buttion[imageButtionIndex].UpdateImage();
		
		if ( itemStatus.status == 0x11 )
			buttionText[imageButtionIndex].text = "[color=ACA48D]" + XStringManager.SP.GetString(1047);
		else if ( itemStatus.status == 0x01 && canAward )
			buttionText[imageButtionIndex].text = "[color=FEBF02]" + XStringManager.SP.GetString(1046);
		else
			buttionText[imageButtionIndex].text = "[color=ACA48D]" + XStringManager.SP.GetString(1046);
		
		//time_t是世界时间， 比 本地时间 少8小时(即28800秒)
		if ( itemStatus.status != 0x11 )
		{
			double seconds = XSevenTargetManager.SP.m_currTime + 28800;
 			double secs = Convert.ToDouble(seconds);
 			DateTime crrdt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(secs);
			seconds = XSevenTargetManager.SP.m_userFirstLoginTime + 28800;
 			secs = Convert.ToDouble(seconds);
			DateTime firstdt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(secs);
			
			if ( passDay + 1 >= (UInt64)id )
			{
				labelLeftTimeTitle[imageButtionIndex].text = XStringManager.SP.GetString(1080);
				
				int leftDay = 10 - (int)passDay - 1;
				DateTime ddtime = new DateTime(crrdt.Year, crrdt.Month, 24);
				TimeSpan ts = ddtime.Subtract(crrdt).Duration();
				int leftHour = ts.Hours;
				if ( ts.Minutes > 0 )
					leftHour++;
				
				if ( leftDay > 0 )
				{
					labelLeftTime[imageButtionIndex].text = string.Format(XStringManager.SP.GetString(1081), leftDay, leftHour);
				}
				else
				{
					labelLeftTime[imageButtionIndex].text =  string.Format(XStringManager.SP.GetString(1082), leftHour);
				}
			}
			else
			{
				DateTime ddtime = new DateTime(crrdt.Year, crrdt.Month, 24);
				TimeSpan ts = ddtime.Subtract(crrdt).Duration();
				int leftDay = id - (int)passDay - 2;
				int leftHour = ts.Hours;
				if ( ts.Minutes > 0 )
					leftHour++;
				if ( leftDay > 0 )
				{
					labelLeftTime[imageButtionIndex].text = string.Format(XStringManager.SP.GetString(1081), leftDay, leftHour);
				}
				else
				{
					labelLeftTime[imageButtionIndex].text =  string.Format(XStringManager.SP.GetString(1082), leftHour);
				}
				labelLeftTimeTitle[imageButtionIndex].text = XStringManager.SP.GetString(1079);
			}
		}
		else
		{
			labelLeftTime[imageButtionIndex].gameObject.SetActive(false);
			labelLeftTimeTitle[imageButtionIndex].gameObject.SetActive(false);
		}
			
		// 设置条件星星状态
		XCfgCondition condition1 = XCfgConditionMgr.SP.GetConfig((int)target.conditionID1);
		if ( null == condition1 )
		{
			spriteStar[spriteStartIndex].gameObject.SetActive(false);
		}
		else
		{
			if ( itemStatus.status == 0x11 || itemStatus.status1 == 0x00 )
			{
				spriteStar[spriteStartIndex].spriteName = "11001109";
			}
			else
			{
				spriteStar[spriteStartIndex].spriteName = "11001108";
			}
		}
		
		condition1 = XCfgConditionMgr.SP.GetConfig((int)target.conditionID2);
		spriteStartIndex++;
		if ( null == condition1 )
		{
			spriteStar[spriteStartIndex].gameObject.SetActive(false);
		}
		else
		{
			if ( itemStatus.status == 0x11 || itemStatus.status2 == 0x00 )
			{
				spriteStar[spriteStartIndex].spriteName = "11001109";
			}
			else
			{
				spriteStar[spriteStartIndex].spriteName = "11001108";
			}
		}
		
		condition1 = XCfgConditionMgr.SP.GetConfig((int)target.conditionID3);
		spriteStartIndex++;
		if ( null == condition1 )
		{
			spriteStar[spriteStartIndex].gameObject.SetActive(false);
		}
		else
		{
			if ( itemStatus.status == 0x11 || itemStatus.status3 == 0x00 )
			{
				spriteStar[spriteStartIndex].spriteName = "11001109";
			}
			else
			{
				spriteStar[spriteStartIndex].spriteName = "11001108";
			}
		}
		
		// 设置条件描述
		XCfgCondition condition = XCfgConditionMgr.SP.GetConfig((int)target.conditionID1);
		if ( null != condition )
		{
			if (itemStatus.status == 0x11 )
			{
				labelDes[labelStartIndex].effectStyle = UILabel.Effect.Outline;
				labelDes[labelStartIndex].effectColor = new Color(0.729f, 0.729f, 0.729f, 255);
				labelDes[labelStartIndex].text = "[color=696969]" + condition.Name;
			}
			else
			{
				labelDes[labelStartIndex].effectStyle = UILabel.Effect.Outline;
				labelDes[labelStartIndex].effectColor = new Color(0.671f, 0.714f, 0.290f, 1);
				labelDes[labelStartIndex].text = "[color=FF0000]" + condition.Name;
			}
		}
		else
		{
			labelDes[labelStartIndex].gameObject.SetActive(false);
		}
		
		labelStartIndex++;
		condition = XCfgConditionMgr.SP.GetConfig((int)target.conditionID2);
		if ( null != condition )
		{
			if (itemStatus.status == 0x11 )
			{
				labelDes[labelStartIndex].effectStyle = UILabel.Effect.Outline;
				labelDes[labelStartIndex].effectColor = new Color(0.729f, 0.729f, 0.729f, 255);
				labelDes[labelStartIndex].text = "[color=696969]" + condition.Name;
			}
			else
			{
				labelDes[labelStartIndex].effectStyle = UILabel.Effect.Outline;
				labelDes[labelStartIndex].effectColor = new Color(0.671f, 0.714f, 0.290f, 1);
				labelDes[labelStartIndex].text = "[color=FF0000]" + condition.Name;
			}
		}
		else
		{
			labelDes[labelStartIndex].gameObject.SetActive(false);
		}
		
		labelStartIndex++;
		condition = XCfgConditionMgr.SP.GetConfig((int)target.conditionID3);
		if ( null != condition )
		{
			if (itemStatus.status == 0x11 )
			{
				labelDes[labelStartIndex].effectStyle = UILabel.Effect.Outline;
				labelDes[labelStartIndex].effectColor = new Color(0.729f, 0.729f, 0.729f, 255);
				labelDes[labelStartIndex].text = "[color=696969]" + condition.Name;
			}
			else
			{
				labelDes[labelStartIndex].effectStyle = UILabel.Effect.Outline;
				labelDes[labelStartIndex].effectColor = new Color(0.671f, 0.714f, 0.290f, 1);
				labelDes[labelStartIndex].text = "[color=FF0000]" + condition.Name;
			}
		}
		else
		{
			labelDes[labelStartIndex].gameObject.SetActive(false);
		}
		
		// 设置物品数据
		uint itemid1 = 0;
		uint itemid2 = 0;
		int cls = XLogicWorld.SP.MainPlayer.Class;
		switch (cls)
		{
		case 1:
			itemid1 = target.awardWarrior1;
			itemid2 = target.awardWarrior2;
		
			break;
		case  2:
			itemid1 = target.awardMage1;
			itemid2 = target.awardMage2;
		
			break;;
		case 3:
			itemid1 = target.awardArcher1;
			itemid2 = target.awardArcher2;
		
			break;
		default:
			break;
		}
		
		if ( itemStatus.status == 0x11 )
		{
			awardIcons[actionIconStartIndex++].Reset();
			awardIcons[actionIconStartIndex++].Reset();
		}
		else
		{
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemid1);
			if ( null != cfgItem )
			{
				awardIcons[actionIconStartIndex].SetSprite(cfgItem.IconAtlasID, cfgItem.IconID, 
					(EItem_Quality)(cfgItem.QualityLevel), (ushort)target.awardNum1);
				awardIcons[actionIconStartIndex].IconData = (int)itemid1;
			}
			actionIconStartIndex++;
			
			cfgItem = XCfgItemMgr.SP.GetConfig(itemid2);
			if ( null != cfgItem )
			{
				awardIcons[actionIconStartIndex].SetSprite(cfgItem.IconAtlasID, cfgItem.IconID, 
					(EItem_Quality)(cfgItem.QualityLevel), (ushort)target.awardNum2);
				awardIcons[actionIconStartIndex].IconData = (int)itemid2;
			}
		}
	}
}
