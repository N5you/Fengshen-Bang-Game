using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

//组件组
public enum FeatureType
{
	EFeatureType_None,
	EFeatureType_Func,			//功能区
	EFeatureType_Activity,		//活动区
	EFeatureType_Num,
}

//放到了Packet里面
//public enum EFeatureID
//{
//	//功能区
//	EFeatureID_Char			=1,
//	EFeatureID_Bag			=2,
//	EFeatureID_Skill		=3,
//	EFeatureID_Product		=4,
//	EFeatureID_FaBao		=5,
//	EFeatureID_Quest		=6,
//	EFeatureID_Strengthen	=7,
//	EFeatureID_Friend		=8,
//	EFeatureID_Garden		=9,
//	EFeatureID_Formation	=10,
//	EFeatureID_Family		=11,
//	
//	//活动区
//	EFeatureID_BaFangPai	=50,
//	EFeatureID_HuoYueDu		=51,
//	EFeatureID_ShanHeTu		=52,
//	EFeatureID_ShouChong	=53,
//	EFeatureID_MeiRiQianDao	=54,
//	EFeatureID_JingJiChang	=55,
//	EFeatureID_XianDaoHui	=56,
//	EFeatureID_PaiHang		=57,
//	EFeatureID_FuLinMen		=58,
//	EFeatureID_Cultivate	=59,
//	EFeatureID_WuZiTS		=60,
//	EFeatureID_BossBat		=61,
//	EFeatureID_TeamDup		=62,
//	EFeatureID_MoneyTree	=63,
//	EFeatureID_SevenTarget	=64,
//	
//	// 混合功能
//	EFeatureID_xiangqian    =20,
//	EFeatureID_hualing	    =21,
//	EFeatureID_fuyin    	=22,
//	EFeatureID_lingdan    	=24,
//	
//	EFeatureID_AddAward		=999,
//	
//	EFeatureID_Num,
//}



public class FeatureDataUnLockMgr
{
	public static uint AddAward_SortID = 999;
	
	private static FeatureDataUnLockMgr	mSingle = new FeatureDataUnLockMgr();
	public  static FeatureDataUnLockMgr SP	{ get{ return mSingle; }}
	
	private FeatureDataUnLockMgr()	{}
	//已开启功能列表
	public List<int>							mOpenFeatureIDList	= new List<int>();
	private SortedList<int,bool>				mIsHintEffectList	= new SortedList<int, bool>();
	public SortedList<uint,AwardData>			mAwardList = new SortedList<uint, AwardData>();
	//全局唯一性标示功能点
	private uint mIndex	= 0;
	public bool	IsCanContinue = false;
	private bool mIsTopUIReady = false;
	private bool mIsBottomUIReady = false;
	private bool mIsDataLoadEnd = false;
	
	public class AwardData
	{
		public uint Index;
		public EAwardType	Type;
		public uint Rank;
		public uint GameMoney;
		public uint Honour;
		public uint ItemID;
		public uint ItemCount;
	}
	
	public bool IsUnLock(int index)
	{
		if(mOpenFeatureIDList.Contains(index))
			return true;
		
		return false;
	}
	
	public void Init()
	{
		XEventManager.SP.AddHandler(OnBottomUIReady,EEvent.BottomFuncBtn_Ready);
		XEventManager.SP.AddHandler(OnTopUIReady,EEvent.TopFuncBtn_Ready);
		XEventManager.SP.AddHandler(OnLevelChanged,EEvent.Attr_Dynamic);
		XEventManager.SP.AddHandler(AddAward,EEvent.Obj_AddAward);
		XEventManager.SP.AddHandler(OnMissionFinished,EEvent.Mission_ReferMission);
		
	}
	
	public Vector3 GetAwardPos()
	{
		XUIFunctionBottomTR UITR = XUIManager.SP.GetUIControl(EUIPanel.eFunctionBottomTR) as XUIFunctionBottomTR;
		if(UITR != null && UITR.IsResourceLoaded())
		{
			return UITR.GetAwardPos();
		}
		return Vector3.zero;
	}
	
	public void ShowAward(uint index)
	{
		if(!mAwardList.ContainsKey(index))
			return ;
		XEventManager.SP.SendEvent(EEvent.UI_ShowAwardInfo,index);	
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eAwardInfo);
	}
	
	public void DataLoadEnd()
	{
		mIsDataLoadEnd	= true;
		if(mIsDataLoadEnd && mIsBottomUIReady && mIsBottomUIReady)
		{
			Log.Write(LogLevel.INFO,"DataLoadEnd");
			CoroutineManager.StartCoroutine(UpdateFuncIcon(false));
		}
		
		XUTMainPlayerInfo info =  XUIManager.SP.GetUIControl(EUIPanel.eMainPlayerInfo) as XUTMainPlayerInfo;
		if(info != null && info.IsResourceLoaded())
			info.ShowHead();
	}	
	
	private void OnLevelChanged(EEvent evt, params object[] args)
	{
		XCharacter ch = (XCharacter)(args[0]);
		EShareAttr attr = (EShareAttr)(args[1]);
		if(ch != XLogicWorld.SP.MainPlayer) return;
		if(EShareAttr.esa_Level != attr) return;
		
		if(XLogicWorld.SP.MainPlayer.IsEnter)
			CoroutineManager.StartCoroutine(UpdateFuncIcon(true));
		
	}
	
	private void OnMissionFinished(EEvent evt, params object[] args)
	{
		if(XLogicWorld.SP.MainPlayer.IsEnter)
			CoroutineManager.StartCoroutine(UpdateFuncIcon(true));
		
	}
	
	private void OnBottomUIReady(EEvent evt, params object[] args)
	{
		mIsBottomUIReady	= true;
		
		if(mIsDataLoadEnd && mIsBottomUIReady && mIsBottomUIReady)
		{
			Log.Write(LogLevel.INFO,"OnBottomUIReady");
			CoroutineManager.StartCoroutine(UpdateFuncIcon(false));
		}
	}
	
	private void OnTopUIReady(EEvent evt, params object[] args)
	{
		mIsTopUIReady	= true;
		
		if(mIsDataLoadEnd && mIsBottomUIReady && mIsBottomUIReady)
		{
			Log.Write(LogLevel.INFO,"OnTopUIReady");
			CoroutineManager.StartCoroutine(UpdateFuncIcon(false));
		}
	}
	
	public void AddUnLock(int index)
	{
		if(mOpenFeatureIDList.Contains(index))
			return ;
		
		mOpenFeatureIDList.Add(index);
		
		XDailyPlaySignMgr.SP.NewFunctionOpen();
	}
	
	public void DelUnLock(int index)
	{
		if(!mOpenFeatureIDList.Contains(index))
			return ;
		
		mOpenFeatureIDList.Remove(index);
	}
	
	//当前是否能解锁
	public void	IsCanUnLock(List<uint> canUnLockList)
	{
		SortedList<uint, FeatureUnLock> list = FeatureUnLockMgr.SP.ItemTable;
		foreach(FeatureUnLock cfg in list.Values)
		{
			if(FeatureDataUnLockMgr.SP.IsUnLock((int)cfg.Index))
				continue;			
			
			bool questIsOK = XMissionManager.SP.hasReferMissionInList(cfg.RequireQuest);
			if(cfg.RequireQuest != 0 && !questIsOK)
				continue;
			
			uint level = (uint)XLogicWorld.SP.MainPlayer.Level;
			if(cfg.RequireLevel != 0 && level < cfg.RequireLevel)
				continue;
			
			bool DupIsOK = XLogicWorld.SP.SubSceneManager.IsUnLock(cfg.RequireDupID);
			if(cfg.RequireDupID != 0 && !DupIsOK)
				continue;
			
			if ( cfg.Index == (uint)EFeatureID.EFeatureID_SevenTarget &&
				XSevenTargetManager.SP.Finish() )
				continue;
			
			canUnLockList.Add(cfg.Index);
		}
	}
	
	public void AddAward(EEvent evt, params object[] args)
	{
		EAwardType	type = (EAwardType)args[0];
		switch(type)
		{
		case EAwardType.EAwardType_XianDH:
			{
				AwardData data = new AwardData();
				data.Index		= mIndex;
				data.Type		= type;
				data.Rank		= (uint)args[1];
				data.GameMoney	= (uint)args[2];
				data.Honour		= (uint)args[3];
				data.ItemID		= (uint)args[4];
				data.ItemCount	= (uint)args[5];
				mAwardList.Add(mIndex,data);
				mIndex++;
			}
			break;
			case EAwardType.EAwardType_ZhanYaoLu:
			{
				AwardData data = new AwardData();
				data.Index		= mIndex;
				data.Type		= type;
				data.Honour		= (uint)args[1];
				mAwardList.Add(mIndex,data);
				mIndex++;
			}
			break;
		default:
			break;
		}
		
		XEventManager.SP.SendEvent(EEvent.UI_AddAward);
	}
	
	public void DelAward(uint Index)
	{
		mAwardList.Remove(Index);
		if(mAwardList.Count == 0)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eGetAward);
			DelFeatureID((uint)EFeatureID.EFeatureID_AddAward);
		}
	}
	
	public void DelSevenTarget()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eSevenTarget);
		DelFeatureID((uint)EFeatureID.EFeatureID_SevenTarget);
	}
	
	public void DelFeatureID(uint FeatureID)
	{
		FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig(FeatureID);
		if(unLock == null)
			return ;
		
		DelUnLock((int)FeatureID);
		
		if(unLock.Group == (uint)FeatureType.EFeatureType_Activity)
		{
			XUIFunctionBottomTR ctrlTFB = XUIManager.SP.GetUIControl(EUIPanel.eFunctionBottomTR) as XUIFunctionBottomTR;
			if(ctrlTFB != null)
			{
				ctrlTFB.DelTRButtion(FeatureID);
			}
		}
	}
	
	public IEnumerator UpdateFuncIcon(bool isNeedAnim)
	{
		List<uint> canUnLockList = new List<uint>();
		IsCanUnLock(canUnLockList);
		
		if(canUnLockList.Count == 0)
			yield break;
		
		if(!isNeedAnim)
		{
			XUTFunctionButton ctrlFB = XUIManager.SP.GetUIControl(EUIPanel.eFunctionButton) as XUTFunctionButton;
			if(ctrlFB != null)
				ctrlFB.AddBtnDirect(canUnLockList);
			
			XUIFunctionBottomTR ctrlTFB = XUIManager.SP.GetUIControl(EUIPanel.eFunctionBottomTR) as XUIFunctionBottomTR;
			if(ctrlTFB != null)
				ctrlTFB.AddBtnDirect(canUnLockList);
		}
		else
		{
			XUTFunctionButton ctrlFB = XUIManager.SP.GetUIControl(EUIPanel.eFunctionButton) as XUTFunctionButton;
			XUIFunctionBottomTR ctrlTFB = XUIManager.SP.GetUIControl(EUIPanel.eFunctionBottomTR) as XUIFunctionBottomTR;
			
			if(ctrlFB == null || ctrlTFB == null)
				yield break;
			
			foreach(uint funcID in canUnLockList)
			{
				FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig(funcID);
				if(unLock == null)
					continue;
				
				XUTFuncUnLock.FuncID	= funcID;
				XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eFuncUnLock);
				
				while(!IsCanContinue)
				{
					yield return 1;
				}
				
				IsCanContinue	= false;
			
				if(unLock.Group == (uint)FeatureType.EFeatureType_Func)
				{
					Vector3 UIPos = ctrlFB.FindFinalPos(funcID);
					XEventManager.SP.SendEvent(EEvent.FuncUnLock_Data,funcID,UIPos);
					ctrlFB.AddBtn(funcID,isNeedAnim);
					yield return new WaitForSeconds(3f); 
				}
				else
				{
					ctrlTFB.AddBtn(funcID,isNeedAnim);
				}
			}
				
		}		
	}
	
	
}