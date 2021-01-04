using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/UIPractiseWindow")]
public class UIPractiseWindow : XDefaultFrame
{
	class GrowthTarget
	{
		public static UITree rootTree;
		private GameObject itemObject;
		private int _targetId;
		private UIPractiseWindow _ui;
		private static int number_child = 10;
		private static int number_parent = 10;
		
		public GrowthTarget(string name, int targetId, GameObject parent, UIPractiseWindow ui)
		{
			if ( null == parent )
			{
				itemObject = rootTree.insertNode(name);
				UIEventListener lis = UIEventListener.Get(itemObject.GetComponent<UITreeParentNode>().m_controlCheckBox);
				lis.onClick += onClick;
				number_parent++;
				itemObject.GetComponent<UITreeParentNode>().name = number_parent.ToString() + name;
			}
			else
			{
				itemObject = rootTree.insertItem("[color=767875]" + name, parent);
				NGUITools.AddWidgetCollider(itemObject, true);
				UIEventListener lis = UIEventListener.Get(itemObject);
				lis.onClick += onClick;
				number_child++;
				itemObject.name = number_child.ToString() + name;
			}

			_targetId = targetId;
			_ui = ui;
		}
		
		public GameObject getNode()
		{
			return itemObject;
		}
		
		public void UpdateStatus(XCfgGrowthTarget target, int status)
		{
			for( int i=0; i<itemObject.transform.childCount; i++ )
			{
				UILabel label = itemObject.transform.GetChild(i).GetComponent<UILabel>();
				if ( label && status > 0)
				{
					label.text = "[color=fff337]" + target.Name;
					break;
				}
			}
			
			Transform spriteObj = itemObject.transform.Find("tagfinish");
			if ( spriteObj && 0x11 == status )
			{
				spriteObj.gameObject.SetActive(true);
			}
		}
		
		private void onClick(GameObject go)
		{
			_ui.onClickTarget(_targetId);
		}
	}
	
	// 奖励物品显示UI
	private class XAwardItem : XBaseActionIcon
	{
		public XAwardItem(XActionIcon actionIcon)
		{
			IsCanDrag = false;
			IsCanDrop = false;
			IsCanPopMenu = false;
			SetUIIcon(actionIcon);
		}
		
		// 设置奖励物品显示信息
		public void setAwardItemInfo(uint itemId, XCfgItem cfgItem, uint count)
		{
			SetSprite(cfgItem.IconAtlasID, cfgItem.IconID, (EItem_Quality)(cfgItem.QualityLevel), (ushort)count);
			IconData = (int)itemId;
		}

		public void Show(bool active)
		{
			mUIIcon.gameObject.SetActive(active);
		}
	}
	
	private class GrowthTargetItem
	{
		public GrowthTargetItem(int s, GrowthTarget tar)
		{
			status = s;
			target = tar;
		}
		public int status;
		public GrowthTarget target;
	}
	
	// 成长目标树形控件
	public UITree Tree_GrowthTarget;
	public UIScrollBar Tree_Scrollbar;
	
	// 一级目录成长目标详细信息界面
	//public GameObject GameObject_TargetFirst;
	public GameObject GameObject_Targe1;
	public UILabel[] Label_SubTarget_Name;
	public UISprite[] Sprite_SubTarget_Show;
	
	// 二级目录成长目标详细信息界面
	public GameObject GameObject_TargetSecond;
	public UILabel Label_ItemTarget_Name;
	public UILabel Label_ItemTarget_Des;
	
	// 奖励物品信息
	public XActionIcon[] Award_Action;
	private XAwardItem[] Award_items;
	public UILabel Label_Money;
	public UILabel Label_InGot;
	public UILabel Label_Exp;
	public UIImageButton Button_GET;
	public GameObject Object_Disable;
	public UILabel Button_Text;
	
	private int currentTargetId = -1;
	
	SortedList<int, GrowthTarget> m_allTargetStatus = new SortedList<int, GrowthTarget>();
	
	public override bool Init()
	{
		base.Init ();
		
		Tree_GrowthTarget.SetScrollBar(Tree_Scrollbar);
		
		UIEventListener lis = UIEventListener.Get(Button_GET.gameObject);
		lis.onClick += onClickGetAward;
		
		GrowthTarget.rootTree = Tree_GrowthTarget;
		
		int maxAwardCount = Award_Action.Length;
		Award_items = new XAwardItem[maxAwardCount];
		for ( int i = 0; i < maxAwardCount; i++ )
		{
			Award_items[i] = new XAwardItem(Award_Action[i]);
		}
		
		showAllTargets();
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.ePractice);
	}
	
	private void onClickGetAward(GameObject go)
	{
		if ( currentTargetId <= 0 )
			return;
		
		// 背包空格判断
		int emptyNum = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag);
		XCfgGrowthTarget target = XCfgGrowthTargetMgr.SP.GetConfig(currentTargetId);
		if ( null == target )
			return;
		int awardCount = 0;
		if ( target.AwardItemCount1 > 0 )
			awardCount++;
		if ( target.AwardItemCount2 > 0 )
			awardCount++;
		if ( target.AwardItemCount3 > 0 )
			awardCount++;
		if ( target.AwardItemCount4 > 0 )
			awardCount++;
		if ( emptyNum < awardCount )
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, string.Format(XStringManager.SP.GetString(1048), 4));
            return ;
		}
		
		if ( XGrowthTargetManger.SP.GetTargetStatus(currentTargetId) == 0 )
		{
			XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(90));
			return;
		}
		
		XEventManager.SP.SendEvent(EEvent.GROWTH_GETAWARDITEMS, currentTargetId);
	}
	
	public void onClickTarget(int targetid)
	{
		XCfgGrowthTarget target = XCfgGrowthTargetMgr.SP.GetConfig(targetid);
		if ( null == target )
			return;
		
		currentTargetId = targetid;
		
		if ( 0 == target.Type )
		{
			GameObject_Targe1.SetActive(true);
			GameObject_TargetSecond.SetActive(false);
			showTargetType1Info(target);
		}
		else
		{
			GameObject_Targe1.SetActive(false);
			GameObject_TargetSecond.SetActive(true);
			showTargetType2Info(target);
		}
		
		showAwardItems(target);
		
		UpdateButtonInfo(target.ID);
	}
	
	public void showAllTargets()
	{
		int totalCount = XCfgGrowthTargetMgr.SP.ItemTable.Count;
		for( int i = 0; i < totalCount; i++ )
		{
			XCfgGrowthTarget target = XCfgGrowthTargetMgr.SP.GetConfig(i);
			if ( null == target )
				continue;
			if ( 1 == target.Type )
				continue;
			
			if ( m_allTargetStatus.ContainsKey(target.ID) )
				continue;
			
			if ( target.Level - XLogicWorld.SP.MainPlayer.Level >= 10 )
				continue;
			
			GrowthTarget nodeTarget = new GrowthTarget(target.Name, target.ID, null, this);
			m_allTargetStatus.Add(target.ID, nodeTarget);
			UpdateGrowthTargetInfo(target.ID, XGrowthTargetManger.SP.GetTargetStatus(target.ID));
			string nodeString = target.Range;
			string[] items = nodeString.Split(',');
			foreach( string item in items )
			{
				int itemId = int.Parse(item);
				XCfgGrowthTarget itemTarget = XCfgGrowthTargetMgr.SP.GetConfig(itemId);
				if ( null == itemTarget )
					continue;
				GrowthTarget tmp = new GrowthTarget(itemTarget.Name, itemTarget.ID, nodeTarget.getNode(), this);
				m_allTargetStatus.Add(itemTarget.ID, tmp);
				UpdateGrowthTargetInfo(itemTarget.ID, XGrowthTargetManger.SP.GetTargetStatus(itemTarget.ID));
			}
		}
	}
	
	private void showTargetType1Info(XCfgGrowthTarget target)
	{
		if ( null == target )
			return;
		
		string nodeString = target.Range;
		string[] items = nodeString.Split(',');
		if ( items.Length > 10 )
			return;
		
		int pos = 0;
		foreach( string item in items )
		{
			int itemId = int.Parse(item);
			XCfgGrowthTarget itemTarget = XCfgGrowthTargetMgr.SP.GetConfig(itemId);
			if ( null == itemTarget )
				continue;

			Label_SubTarget_Name[pos].gameObject.SetActive(true);
			Sprite_SubTarget_Show[pos].gameObject.SetActive(true);
			if ( 0x00 == XGrowthTargetManger.SP.GetTargetStatus(itemId) )
			{
				Label_SubTarget_Name[pos].text = "[color=767875]" + itemTarget.Name;
				Sprite_SubTarget_Show[pos].spriteName = "11000028";
			}
			else
			{
				Label_SubTarget_Name[pos].text = "[color=00FF00]" + itemTarget.Name;
				Sprite_SubTarget_Show[pos].spriteName = "11000030";
			}
			pos++;
		}
		int labelCount = Label_SubTarget_Name.Length;
		for( ; pos <labelCount; pos++ )
		{
			Label_SubTarget_Name[pos].gameObject.SetActive(false);
			Sprite_SubTarget_Show[pos].gameObject.SetActive(false);
		}
	}
	
	private void showTargetType2Info(XCfgGrowthTarget target)
	{
		if ( null == target )
			return;
		
		Label_ItemTarget_Name.text = target.Name;
		Label_ItemTarget_Des.text = target.Description;
	}
	
	private void showAwardItems(XCfgGrowthTarget target)
	{
		if ( null == target )
			return;
		int count = Award_items.Length;
		if ( count < 4 )
			return;
		
		int pos = 0;
		do
		{
			XCfgItem item = XCfgItemMgr.SP.GetConfig(target.AwardItemID1);
			if ( null != item )
			{
				Award_items[pos].setAwardItemInfo(target.AwardItemID1, item, target.AwardItemCount1);
				Award_items[pos].Show(true);
				pos++;
			}
			
			item = XCfgItemMgr.SP.GetConfig(target.AwardItemID2);
			if ( null != item )
			{
				Award_items[pos].setAwardItemInfo(target.AwardItemID2, item, target.AwardItemCount2);
				Award_items[pos].Show(true);
				pos++;
			}
			
			item = XCfgItemMgr.SP.GetConfig(target.AwardItemID3);
			if ( null != item )
			{
				Award_items[pos].setAwardItemInfo(target.AwardItemID3, item, target.AwardItemCount3);
				Award_items[pos].Show(true);
				pos++;
			}

			item = XCfgItemMgr.SP.GetConfig(target.AwardItemID4);
			if ( null != item )
			{
				Award_items[pos].setAwardItemInfo(target.AwardItemID4, item, target.AwardItemCount4);
				Award_items[pos].Show(true);
				pos++;
			}

			for( int i = pos; i < count; i++ )
				Award_items[i].Show(false);
			
		}while(false);
		
		Label_Money.text = target.AwardMonty.ToString();
		Label_InGot.text = target.AwardIngot.ToString();
		Label_Exp.text = target.AwardExp.ToString();
	}
	
	private void UpdateButtonInfo(int targetid)
	{
		if ( !m_allTargetStatus.ContainsKey(targetid) )
		{
			Button_GET.enabled = false;
			Object_Disable.SetActive(true);
			Button_GET.gameObject.SetActive(false);
		}
		else if ( 0x00 == XGrowthTargetManger.SP.GetTargetStatus(targetid) )
		{
			Button_GET.enabled = true;
			Button_Text.text = XStringManager.SP.GetString(1046);
			Object_Disable.SetActive(false);
			Button_GET.gameObject.SetActive(true);
		}
		else if ( 0x01 == XGrowthTargetManger.SP.GetTargetStatus(targetid) )
		{
			Button_GET.enabled = true;
			Button_Text.text = XStringManager.SP.GetString(1046);
			Object_Disable.SetActive(false);
			Button_GET.gameObject.SetActive(true);
		}
		else if ( 0x11 == XGrowthTargetManger.SP.GetTargetStatus(targetid) )
		{
			Button_GET.enabled = false;
			Button_Text.text = XStringManager.SP.GetString(1047);
			Object_Disable.SetActive(true);
			Button_GET.gameObject.SetActive(false);
		}
	}
	
	public void UpdateTargetStatus(int targetid, int status)
	{
		if ( !m_allTargetStatus.ContainsKey(targetid) )
			return;
		
		UpdateGrowthTargetInfo(targetid, status);
		
		checkAllFinish();
	}
	
	public void SetTargetStatus(int targetid, int status)
	{
		if ( !m_allTargetStatus.ContainsKey(targetid) )
			return;
		
		UpdateGrowthTargetInfo(targetid, status);
	}
	
	private void UpdateGrowthTargetInfo(int targetid, int status)
	{
		if ( currentTargetId == targetid )
		{
			UpdateButtonInfo(targetid);	
		}
		
		XCfgGrowthTarget target = XCfgGrowthTargetMgr.SP.GetConfig(targetid);
		if ( 1 == target.Type )
			m_allTargetStatus[targetid].UpdateStatus(target, status);
		
		XCfgGrowthTarget currTarget = XCfgGrowthTargetMgr.SP.GetConfig(currentTargetId);
		if ( null == currTarget )
			return;
		
		if ( 1 == target.Type && 0 == currTarget.Type )
		{
			string[] splitStrs = currTarget.Range.Split(',');
			int pos = -1;
			for ( int i = 0; i < splitStrs.Length; i++ )
			{
				if ( int.Parse(splitStrs[i]) == targetid )
				{
					pos = i;
					break;
				}
			}
			
			if ( -1 != pos )
			{
				Label_SubTarget_Name[pos].text = "[color=00ff00]" + target.Name;
				Sprite_SubTarget_Show[pos].spriteName = "11000030";
			}
		}
	}
	
	private void checkAllFinish()
	{
		foreach ( KeyValuePair<int, GrowthTarget> item in m_allTargetStatus )
		{
			if ( XGrowthTargetManger.SP.GetTargetStatus(item.Key) == 1 )
			{
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)EFeatureID.EFeatureID_Cultivate, 900039u, 0);
				return;
			}
		}
		XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STOPEFFECT, (uint)EFeatureID.EFeatureID_Cultivate, 0u);
	}
}

