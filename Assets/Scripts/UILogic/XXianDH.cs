using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XXianDH")]
public class XXianDH : XDefaultFrame
{
	public static uint MAX_BATTLE_RECORD_NUM = 6;
	public static uint MAX_SORT_RECORD_NUM = 10;
	
	public UILabel 	AwardGameMoney;
	public UILabel	AwardHonour;
	public UILabel	AwardItemName;
	public UILabel	AwardTitle;
	public UILabel	LeftTime;
	public UILabel	Notice;
	public UIImageButton	AddCount;
	public UIImageButton	AddSpeed;
	public UILabel	CDTime;
	
	public UILabel	Info_Rank;
	public UILabel	Info_CurContinueWin;
	public UILabel	Info_BestRank;
	public UILabel	Info_MaxContinueWin;
	public UILabel	Info_Title;
	public UILabel	Info_BattleValue;
	public UILabel	Info_Rate;
	public UILabel	LeftRealTime;
	
	public UILabel	CurRank;
	public UILabel	ContinueCount;
	public UILabel	CurChallengeCount;
	public SingleObj[]	ObjList = new SingleObj[5];
	public GameObject[] ObjGo = new GameObject[5];
	
	public UISpriteGroup	SpriteGroup;
	public GameObject	RankInfo;
	public GameObject	SortRank;
	public GameObject	ShowInfo;
	
	[System.Serializable]
	public class SingleObj
	{
		public SingleObj(uint clothesModelID,uint WeaponModelID)
		{
		
		}
		
		public void Init(uint clothesModelID,uint WeaponModelID)
		{
			mClothesModelID	= clothesModelID;
			mWeaponModelID	= WeaponModelID;
			
			InitFunc();
			UpdateModel();
		}
		
		public UILabel		ObjName;
		public UILabel		ObjLevel;
		public UILabel		ObjRank;
		public UITexture	ModelTexture;
		public GameObject	BKCollider;
		
		public GameObject	ObjRoot;
		private uint		mClothesModelID;
		private uint		mWeaponModelID;
		public uint			mCurRank;
		//private XU3dModel	m_mainModel;
		//private XU3dModel	m_weaponModel;
		//private XUIModel 	mUIModel;
		
		
		public SingleModelRTT	rtt;		
		private XObjectModel m_objectModel;
		
		public void HoverHandle(GameObject go, bool state)
		{
			if(state)
			{
				m_objectModel.HoverIn();				
				CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_Seal);
				XEventManager.SP.SendEvent(EEvent.PKData_Tip,mCurRank);
			}
			else				
			{
				m_objectModel.HoverOut();				
				CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_None);
				XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipC);
			}
		}
		
		public void ClickHandle(GameObject go)
		{
			XEventManager.SP.SendEvent(EEvent.PKData_PKPlayer,mCurRank);
			
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_XianDaoHuiStart);
		}
		
		public void InitFunc()
		{
			UIEventListener ls = UIEventListener.Get(BKCollider);
			ls.onHover	+= HoverHandle;
			ls.onClick	+= ClickHandle;
		}
		
		public void LoadModelComplete (XObjectModel model)
		{
			m_objectModel	= model;
			if(rtt != null)
				rtt.DestoryNotModel();
			rtt = XModelRTTMgr.SP.AddObjectRTT(m_objectModel.mainModel, ModelTexture, 1.5f, new Vector3(0f, -1.5f, 2f),Vector3.zero,new Vector3(1, 1, 1), false);
		}
		
		public void ClearModel()
		{
//			if(rtt != null)
//			{
//				rtt.DestoryNotModel();
//			}
		}
		
		private void UpdateModel()
		{
			if(m_objectModel == null)
				m_objectModel = new XObjectModel();
			m_objectModel.LoadModelEvent	+= LoadModelComplete;
			m_objectModel.HandleEvent(EModelEvent.evtModelId, mClothesModelID);
			m_objectModel.HandleEvent(EModelEvent.evtWeaponId,mWeaponModelID);
			
		}
	}
	
	[System.Serializable]
	public class BattleRecord
	{
		public UILabel Record;
		public UILabel LookBattle;
	}
	
	[System.Serializable]
	public class SortRecord
	{
		public void Clear()
		{
			Rank.text	= "";
			Name.text	= "";
			Level.text	= "";
			BattleValue.text	= "";
		}
		public UILabel	Rank;
		public UILabel	Name;
		public UILabel	Level;
		public UILabel	BattleValue;
		public UISprite	Sprite;
	}
	
	public BattleRecord[]	RecordList = new BattleRecord[MAX_BATTLE_RECORD_NUM];
	public SortRecord[]		SortRecordList = new SortRecord[MAX_SORT_RECORD_NUM];
	
	
	public void AddPKObject(int index,string name,uint level,uint rank,uint ClothesModelID,uint WeaponModelID)
	{
		if(index < 0 || index >= 5)
			return ;
		
		SingleObj obj = ObjList[index];
		obj.ObjName.text	= name;
		obj.ObjLevel.text	= level.ToString() + XStringManager.SP.GetString(712);
		obj.ObjRank.text	= rank.ToString();
		obj.mCurRank		= rank;
		
		obj.Init(ClothesModelID,WeaponModelID);	
	}
	
	public void ClearSortRecord()
	{
		for(int i = 0; i < MAX_SORT_RECORD_NUM; i++)
		{
			SortRecordList[i].Clear();
		}
	}
	
	public void ShowNewPlayerGuide()
	{
		int maxIndex = ObjList.Length - 1;
		for ( int i = maxIndex; i >= 0; i-- )
		{
			if ( ObjList[i].mCurRank > 0 )
			{
				GameObject obj =  ObjGo[i];
				Vector3 showPos = new Vector3(501, -65, -500);
				XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_XianDaoHuiStart, 
					2, showPos, obj);
				break;
			}
		}
	}
	
	public void AddSortRecord(int index,uint rank,string name,uint level,uint battleValue,PKSortType flag)
	{
		SortRecord record = SortRecordList[index];
		record.Rank.text	= rank.ToString();
		record.Name.text	= name;
		record.Level.text	= level.ToString();
		record.BattleValue.text	= battleValue.ToString();
		if(flag == PKSortType.PKSortType_Down)
			record.Sprite.name	= "11000124";
		else
			record.Sprite.name	= "11000123";
	}
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener ls = UIEventListener.Get(AddCount.gameObject);
		ls.onClick	= ClickAddCount;
		ls.onHover	= HoverDelegate;
		
		UIEventListener ls1 = UIEventListener.Get(AddSpeed.gameObject);
		ls1.onClick	= ClickAddSpeed;
		ls1.onHover	= AddSpeedTip;
		
		SpriteGroup.mModify	= OnSelect;
		
		UIEventListener lis = UIEventListener.Get(Notice.gameObject);
		lis.onClickHyperLink	+= ClickName;
		
		UIEventListener lBR1 = UIEventListener.Get(RecordList[0].Record.gameObject);
		lBR1.onClickHyperLink	+= ClickName;
		
		UIEventListener lBR2 = UIEventListener.Get(RecordList[1].Record.gameObject);
		lBR2.onClickHyperLink	+= ClickName;
		
		UIEventListener lBR3 = UIEventListener.Get(RecordList[2].Record.gameObject);
		lBR3.onClickHyperLink	+= ClickName;
		
		UIEventListener lBR4 = UIEventListener.Get(RecordList[3].Record.gameObject);
		lBR4.onClickHyperLink	+= ClickName;
		
		UIEventListener lBR5 = UIEventListener.Get(RecordList[4].Record.gameObject);
		lBR5.onClickHyperLink	+= ClickName;
		
		UIEventListener lBR6 = UIEventListener.Get(RecordList[5].Record.gameObject);
		lBR6.onClickHyperLink	+= ClickName;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eXianDH);
	}
	
	public override void Show()
	{
		base.Show();
	}
	
	public void HoverDelegate (GameObject go, bool state)
	{
		if(state)
		{
			//XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eToolTipB);
			//XEventManager.SP.SendEvent(EEvent.ToolTip_B,XStringManager.SP.GetString(182),"",strTip);
			XEventManager.SP.SendEvent(EEvent.UI_ShowChallengeCount);
			
		}
		else
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipC);
		}
	}
	
	private void ClickName(GameObject go,string str)
	{
		HyperLinkMgr.SP.Process(str);
	}

	public void ClickAddCount(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.PKData_UI_BuyChallenge);
		
		//CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		//XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_PK_Buy_Challenge, builder.Build());	
	}
	
	public void ClickAddSpeed(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.PKData_UI_AddSpeed);
		
		//CS_Empty.Builder builder = CS_Empty.CreateBuilder();
		//XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_PK_Add_Speed, builder.Build());	
	}
	
	public void AddSpeedTip(GameObject go, bool state)
	{
		if(state)
		{
			XEventManager.SP.SendEvent(EEvent.PKData_UI_AddSpeed_Tip);
		}
		else
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eToolTipC);
		}
		
	}
	
	public void OnSelect(int index)
	{
		if(index == 0)
		{
			RankInfo.gameObject.SetActive(true);
			SortRank.gameObject.SetActive(false);
			ShowInfo.gameObject.SetActive(false);
		}
		else
		{
			RankInfo.gameObject.SetActive(false);
			SortRank.gameObject.SetActive(true);
			ShowInfo.gameObject.SetActive(true);
		}
	}
	
	public void ClearModel()
	{
		for(int i = 0; i < ObjList.Length; i++)
		{
			ObjList[i].ClearModel();
		}
	}
}
