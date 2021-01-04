using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using XGame.Client.Network;

public class MissionMessage
{
	public uint m_uiMissionID = 0;
		
	public uint m_uiMissionKillCount1 = 0;
		
	public uint m_uiMissionKillCount2 = 0;
		
	public uint m_uiMissionKillCount3 = 0;
		
	public uint m_uiMissionItemCount1 = 0;
		
	public uint m_uiMissionItemCount2 = 0;
		
	public uint m_uiMissionItemCount3 = 0;
		
	public bool m_bMissionCompleted = false;
}

public enum eMissionState
	{
		eMs_DoingState=1,
		eMs_CompletedState,
		eMs_CanReceive,
		eMs_CanNotReceive,
	}

public enum eMissionRequirement
{
	eMR_Kill1 = 1,
	eMR_Kill2,
	eMR_Kill3,
	eMR_Item1,
	eMR_Item2,
	eMR_Item3,
}

public class XMissionManager:XSingleton<XMissionManager>
{
	private HashSet<uint> m_referMissionList = new HashSet<uint>();  //completed refered mission list 
	
	private SortedList<uint,MissionMessage> m_actionMissionList = new SortedList<uint, MissionMessage>(); // current action mission list and mission message 
	
	private List<uint> m_canReceiveMissionList = new List<uint>();//will can be recevie mission list
	
	//missing receive list
	private List<uint> m_MissionReceiveList = new List<uint>();
	
	//mission refer list
	private List<uint> m_MissionReferList = new List<uint>();
	
	public List<uint> MissionReceiveList
	{
		get{
			return m_MissionReceiveList;
		}
	}
	
	public List<uint> MissionReferList
	{
		get{
			return m_MissionReferList;
		}
	}
	
	public void init()
	{
		XEventManager.SP.AddHandler(listenLevelUpdate, EEvent.Update_Level);
	}
	
	private void listenLevelUpdate(EEvent evt, params object[] args)
	{
		makeCanbeReceiveMissionList();
	}
	
	public SortedList<uint,MissionMessage> getActionMissionList()
	{
		return m_actionMissionList;
	}
	
	public List<uint> getCanReceiveMissionList()
	{
		return m_canReceiveMissionList;
	}
	
	public bool hasReferMissionInList(uint missionID)
	{
		return m_referMissionList.Contains(missionID);
	}
	
	public void addReferMission(uint missionID)
	{
		m_referMissionList.Add(missionID);
		
		makeCanbeReceiveMissionList();
		
		XNewPlayerGuideManager.SP.handleMissionFinish(missionID);
	}
	
	public void actionMission( MissionMessage mission )
	{
		uint missionID = mission.m_uiMissionID;
		
		if(0!=missionID) //not empty action mission
		{
			if(m_actionMissionList.ContainsKey(missionID) )
			{
				m_actionMissionList[missionID] = mission;
			}else
			{
				m_actionMissionList.Add(missionID,mission);
				makeCanbeReceiveMissionList();
			}
		}else			//empty action mission
		{
			makeCanbeReceiveMissionList();
		}
		
		XEventManager.SP.SendEvent(EEvent.Mission_ActionState,m_actionMissionList );
	}
	
	public bool isActionMission(uint missionID)
	{
		return m_actionMissionList.ContainsKey(missionID);
	}
	
	public void toReferMission( uint missionID )
	{
		if(!m_actionMissionList.ContainsKey(missionID) )
			return;
		
		m_actionMissionList.Remove(missionID);
		
		m_referMissionList.Add(missionID);
		
		makeCanbeReceiveMissionList();
		
		XEventManager.SP.SendEvent(EEvent.Mission_ActionState,m_actionMissionList );
		
		XEventManager.SP.SendEvent(EEvent.Mission_ReferMission,missionID);
		
		//make effect
		
		Transform effectTrans = XLogicWorld.SP.MainPlayer.GetSkeleton(ESkeleton.eHeadCenter );
		
		XU3dEffect effectObj =new XU3dEffect(900036);
		
		Vector3 newPos = effectTrans.position;
		
		newPos.y += 1.2f;
		
		effectObj.Position = newPos;
	}
	
	public MissionMessage getActionMission(uint missionID)
	{
		if(!m_actionMissionList.ContainsKey(missionID) )
			return null;
		
		return m_actionMissionList[missionID];
	}
	
	public void requestReceiveMission(uint missionID)
	{
		CS_RequestReceiveMission.Builder builder = CS_RequestReceiveMission.CreateBuilder();
		builder.MissionID = missionID;
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_RequestReceiveMission,builder.Build() );
	}
	
	public void requestGiveUpMission(uint missionID)
	{
		CS_UInt.Builder builder = CS_UInt.CreateBuilder();
		builder.Data = missionID;
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_RequestGiveUpMission,builder.Build() );
	}
	
	public void requestReferMission(uint missionID)
	{
		CS_RequestReferMission.Builder builder = CS_RequestReferMission.CreateBuilder();
		builder.MissionID = missionID;
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_RequestReferMission,builder.Build() );
	}
	
	public void missionReceiveResult(uint missionID,EMissionMessage result)
	{
		if(EMissionMessage.emmReceiveSccess == result)
		{
			XEventManager.SP.SendEvent(EEvent.Mission_ReceiveMission,missionID );
			
			NavigateToCompleteMission(missionID);
			//make effect
		
			Transform effectTrans = XLogicWorld.SP.MainPlayer.GetSkeleton(ESkeleton.eHeadCenter );
			
			XU3dEffect effectObj =new XU3dEffect(900037);
			
			Vector3 newPos = effectTrans.position;
			
			newPos.y += 1.2f;
			
			effectObj.Position = newPos;
		}
	}
	public void NavigateToCompleteMission(uint missionID)
	{
		if ( 999u == missionID )
			return;
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID);
		if(missionBase == null)
			return;
		int id = 0;
		EObjectType objectType  = EObjectType.Begin;
		Vector3 TargetPos = Vector3.zero;
		
		bool bFind = false;
		uint sceneID = 0;
		uint duplicateID = 0;
		MissionMessage msg = XMissionManager.SP.getActionMission(missionID);
		if( true == msg.m_bMissionCompleted)
		{
			id = (int)missionBase.referNPCID;
			objectType = EObjectType.Npc;
			
			foreach( KeyValuePair<uint,List<XCfgSceneNpcInfo> > infoPair in XCfgSceneNpcInfoMgr.SP.ItemTable )
			{
				foreach(XCfgSceneNpcInfo info in infoPair.Value)
				{
					if(id  == info.NpcID )
					{
						sceneID = infoPair.Key;
						TargetPos = info.BornPos;
						bFind = true;
						break;
					}
				}
			}
		}
		else
		{
			duplicateID = missionBase.missionDuplicateID;
			id = (int)missionBase.missionConveyDoorID;
			objectType = EObjectType.TransPoint;
			
			XCfgLevelEntry entryBase = XCfgLevelEntryMgr.SP.GetConfig((uint)id);
			if(null == entryBase)
			{
				return;
			}
			TargetPos = entryBase.Position;	
			sceneID = entryBase.SceneID;
			bFind = true;
		}
			
		if(bFind)
		{
			XLogicWorld.SP.MainPlayer.NavigateTo(new XMainPlayerStateNavigate.NavigateInfo(sceneID, objectType, id, TargetPos,duplicateID));
		}
	}
	
	public void NavigateToReceiveMission(uint missionID)
	{
		if ( 999u == missionID )
			return;
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID);
		if(missionBase == null)
			return;
		
		uint nextMissionID = GetNextMissionID(missionID);
		XCfgMission nextMissionBase = XCfgMissionMgr.SP.GetConfig( nextMissionID );
		if(null == nextMissionBase)
			return;
		
		//判断是否主线
		if(nextMissionBase.property != 1)
			return;
		
		int TargetNpcID = (int)nextMissionBase.reciveNPCID;
		EObjectType objectType  = EObjectType.Npc;
		
		Vector3 TargetPos = Vector3.zero;	
		bool bFind = false;
		uint TargetSceneID = nextMissionBase.receiveNPCSceneID;
		
		List<XCfgSceneNpcInfo> infoList = XCfgSceneNpcInfoMgr.SP.GetGroup(nextMissionBase.receiveNPCSceneID);
		if(infoList.Count == 0)
			return;	
		foreach(XCfgSceneNpcInfo info in infoList)
		{
			if(TargetNpcID  == info.NpcID )
			{
				TargetPos = info.BornPos;
				bFind = true;
				break;
			}
		}
		if(bFind)
		{
			XLogicWorld.SP.MainPlayer.NavigateTo(new XMainPlayerStateNavigate.NavigateInfo(TargetSceneID, EObjectType.Npc, TargetNpcID, TargetPos));
		}
	}
	
	private uint GetNextMissionID(uint referMissionID )
	{
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig( referMissionID );
		if( null == missionBase )
			return 0;
		
		if( 0 >= missionBase.nextID )
			return 0;
		
		XCfgMission nextMissionBase = XCfgMissionMgr.SP.GetConfig( missionBase.nextID );
		if(null == nextMissionBase)
			return 0;
		
		if(false == XNpc.missionReceiveCheck(nextMissionBase.ID,0 ) )
			return 0;
		
		return nextMissionBase.ID;
	}
	
	public void CheckNavigateToRefer()
	{
		if(ESceneType.ClientScene != XLogicWorld.SP.SceneManager.PreSceneType)
			return;
		if(ESceneType.NormalScene != XLogicWorld.SP.SceneManager.LoadedSceneType)
			return;
		uint ReferMissionID = 0;
		foreach (KeyValuePair<uint,MissionMessage> s in m_actionMissionList)
		{
			MissionMessage msg =  s.Value;
			uint missionID = msg.m_uiMissionID;
			if (!msg.m_bMissionCompleted)
				return;
			XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig( missionID );
			if( null == missionBase )
				return;
			//判断是否主线
			if(missionBase.property != 1)
				return;
			XCfgClientScene cfgClient = XCfgClientSceneMgr.SP.GetConfig(missionBase.missionDuplicateID);
			if(cfgClient == null)
				return;
			if(cfgClient.SceneID == XLogicWorld.SP.SceneManager.PreSceneId)
			{
				ReferMissionID = missionID;
				break;
			}
		}
		if(ReferMissionID != 0)
			NavigateToCompleteMission(ReferMissionID);
	}
		
	public void missionReferResult(uint missionID,EMissionMessage result)
	{
		switch(result)
		{
		case EMissionMessage.emmReferSccess:
			toReferMission(missionID);
			XEventManager.SP.SendEvent(EEvent.Mission_ActionState,m_actionMissionList );
			NavigateToReceiveMission(missionID);
			break;
		case EMissionMessage.emmReferFailNotEmpty:
			XEventManager.SP.SendEvent(EEvent.MessageBox,null,null,XStringManager.SP.GetString(153) );
			break;
		}
		XNewPlayerGuideManager.SP.handleMissionFinish(missionID);
	}
	
	public void missionGiveUpResult(uint missionID)
	{
		if(!m_actionMissionList.ContainsKey(missionID) )
			return;
		m_actionMissionList.Remove(missionID);
		makeCanbeReceiveMissionList();
		
		XEventManager.SP.SendEvent(EEvent.Mission_ActionState,m_actionMissionList );
		
		XEventManager.SP.SendEvent(EEvent.Mission_GiveUpMission,missionID );
	}
	
	public string makeColorStringByRequirement(uint currentCount,uint totalCount,string strBase)
	{
		string strColor="[linkcolor=000000]";
		
		if( currentCount>=totalCount )
		{
			strColor = "[linkcolor=42db49]";
		}else
		{
			strColor = "[linkcolor=eaca33]";
		}
		
		return strColor+strBase;
	}
	
	public string makeFunctionMissionName(uint missionID)
	{
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID);
		string strBase = missionBase.title;
		//is main mission or not add word
		if( 0 == missionBase.property )
		{
			strBase = XStringManager.SP.GetString(41)+strBase;
		}else if( 1 == missionBase.property )
		{
			strBase = XStringManager.SP.GetString(40)+strBase;
		}
		
		string strColor="";
		eMissionState state = eMissionState.eMs_CanNotReceive;
		
		if(isActionMission(missionID) )
		{
			if(m_actionMissionList[missionID].m_bMissionCompleted )
			{
				state = eMissionState.eMs_CompletedState;
			}else
			{
				state = eMissionState.eMs_DoingState;
			}
		}else
		{
			if( XLogicWorld.SP.MainPlayer.Level < missionBase.needLevel )
			{
				state = eMissionState.eMs_CanNotReceive;
			}else
			{
				state = eMissionState.eMs_CanReceive;
			}
			
		}
		
		switch( state )
		{
		case eMissionState.eMs_DoingState:
			strColor = "[linkcolor=eaca33]";
			break;
		case eMissionState.eMs_CanReceive:
			strColor = "[linkcolor=ffffff]";
			break;
		case eMissionState.eMs_CompletedState:
			strColor = "[linkcolor=42db49]";
			break;
		case eMissionState.eMs_CanNotReceive:
			strColor = "[linkcolor=ff0000]";
			break;
		}
		
		return strColor+strBase;
	}
	
	private void makeCanbeReceiveMissionList()
	{
		m_canReceiveMissionList.Clear();
		
		List<uint> missionListComplete = new List<uint>();
		
		foreach( KeyValuePair<uint,XCfgMission> m in XCfgMissionMgr.SP.ItemTable )
		{
			//if not action missionID
			if( !isActionMission( m.Key ) && !hasReferMissionInList(m.Key) )
			{
				missionListComplete.Add(m.Key);
			}
			
		}
		
		m_canReceiveMissionList = XNpc.checkReceiveMissionList(missionListComplete,1);
		
		XEventManager.SP.SendEvent(EEvent.Mission_CanReceiveListUpdate,m_canReceiveMissionList );
	}
	
	public string makeShowRequirementString(string strBase,uint currentCount,uint totalCount)
	{
		string strShowTxt = makeShowNumberString( strBase,currentCount,totalCount );
		
		return XMissionManager.SP.makeColorStringByRequirement(currentCount,totalCount,strShowTxt);
	}
	
	public string makeShowRequirementString(uint missionID,eMissionRequirement eMr)
	{
		string strShowTxt=null;
		uint totalCount=0;
		uint currentCount=0;
		MissionMessage mm = null;
		bool bIsAction = isActionMission(missionID);
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID );
		
		if(bIsAction )
		{
			mm = getActionMission(missionID);
		}
		
		
		switch(eMr)
		{
		case eMissionRequirement.eMR_Kill1:
			strShowTxt = missionBase.killTargetTitle1;
			totalCount = missionBase.referKillCount1;
			if(bIsAction)
			{
				currentCount = mm.m_uiMissionKillCount1;
			}
			break;
			
		case eMissionRequirement.eMR_Kill2:
			strShowTxt = missionBase.killTargetTitle2;
			totalCount = missionBase.referKillCount2;
			if(bIsAction)
			{
				currentCount = mm.m_uiMissionKillCount2;
			}
			break;
			
		case eMissionRequirement.eMR_Kill3:
			strShowTxt = missionBase.killTargetTitle3;
			totalCount = missionBase.referKillCount3;
			if(bIsAction)
			{
				currentCount = mm.m_uiMissionKillCount3;
			}
			break;
			
		case eMissionRequirement.eMR_Item1:
			strShowTxt = missionBase.itemTargetTitle1;
			totalCount = missionBase.referItemCount1;
			if(bIsAction)
			{
				currentCount = mm.m_uiMissionItemCount1;
			}
			break;
			
		case eMissionRequirement.eMR_Item2:
			strShowTxt = missionBase.itemTargetTitle2;
			totalCount = missionBase.referItemCount2;
			if(bIsAction)
			{
				currentCount = mm.m_uiMissionItemCount2;
			}
						
			break;
			
		case eMissionRequirement.eMR_Item3:
			strShowTxt = missionBase.itemTargetTitle3;
			totalCount = missionBase.referItemCount3;
			if(bIsAction)
			{
				currentCount = mm.m_uiMissionItemCount3;
			}
			
			break;
			
		}
		
		if(bIsAction )
		{
			return makeShowRequirementString(strShowTxt,currentCount,totalCount);
		}else
		{
			return strShowTxt;
		}
	}
	
	public string makeShowNumberString( string strBase,uint currentCount,uint totalCount )
	{
		string strCurrent = currentCount.ToString();
		string strTotal = totalCount.ToString();
		string strSign = "/";
		return strBase+" "+strCurrent+strSign+strTotal;
	}
	
	private string makeCanReceiveString(uint missionID)
	{
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID);
		string strNameResult = missionBase.title;
		
		if(!isActionMission(missionID) )
		{
			string preStr = "";
			string suffix = "";
			//level offset
			if((uint)XLogicWorld.SP.MainPlayer.Level < missionBase.needLevel )
			{
				preStr="[color=ff0000]";
				suffix="\n";
				suffix+= XStringManager.SP.GetString(42);
				uint levelOffset = missionBase.needLevel-(uint)XLogicWorld.SP.MainPlayer.Level;
				suffix+= levelOffset.ToString();
				
				return preStr+strNameResult+suffix;
			}
			
		}
		
		return strNameResult;
		
	}
	
	public string makeMissionSuffitx(uint missionID)
	{
		//single
		XCfgMission mission = XCfgMissionMgr.SP.GetConfig(missionID);
		if(null == mission )
			return "";
		
		if(XMissionManager.SP.isActionMission( missionID ) ) //is action canRefer or doing
		{
			MissionMessage mm = XMissionManager.SP.getActionMission(missionID);
			if(mm.m_bMissionCompleted)
			{
				string preColorStr = "[color=42db49]";
				return preColorStr + mission.title + XStringManager.SP.GetString( 33 );
			}else
			{
				return mission.title + XStringManager.SP.GetString( 32 );
			}
				
		}else //not action can receive
		{
			return mission.title + XStringManager.SP.GetString( 34 );
		}
		
	}
	
	public void makeNPCLableLink(UILabel label,uint npcID)
	{
		string strLink = "";
		
		uint sceneID = 0;
		Vector3 bronPos;
		bool bFind = false;
		//get scene ID where npc in
		foreach( KeyValuePair<uint,List<XCfgSceneNpcInfo> > infoPair in XCfgSceneNpcInfoMgr.SP.ItemTable )
		{
			foreach(XCfgSceneNpcInfo info in infoPair.Value)
			{
				if(npcID  == info.NpcID )
				{
					sceneID = infoPair.Key;
					bronPos = info.BornPos;
					bFind = true;
					break;
				}
			}
		}
		
		//not find
		if(false==bFind)
		{
			return;
		}
		
		//导航对象，格式: 开启[link=1][linkdata=T(3)D(sceneID,ObjectType,id,posX,posY,posZ)]...[link=0]
		strLink += "[link=1][linkdata=T(3)D(";
		strLink += sceneID.ToString();
		strLink += ",";
		strLink += ((uint)0 ).ToString();
		strLink += ",";
		strLink += ((int)(EObjectType.Npc) ).ToString();
		strLink += ",";
		strLink += npcID.ToString();
		strLink += ",";
		//strLink += bronPos.x.ToString();
		strLink += ",";
	//	strLink += bronPos.y.ToString();
		strLink += ",";
		//strLink += bronPos.z.ToString();
		strLink += ")]";
		strLink += label.text;
		strLink += "[link=0]";
		
		label.text = strLink;
	}
	
	public void makeTransPointLableLink(UILabel label, uint missionID )
	{
		string strLink = "";
		
		uint sceneID = 0;
		Vector3 bronPos;
		
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID);
		if(null==missionBase)
		{
			return;
		}
		
		XCfgLevelEntry entryBase = XCfgLevelEntryMgr.SP.GetConfig(missionBase.missionConveyDoorID);
		if(null == entryBase)
		{
			return;
		}
		
		
		//导航对象，格式: 开启[link=1][linkdata=T(3)D(sceneID,ObjectType,id,posX,posY,posZ)]...[link=0]
		strLink += "[link=1][linkdata=T(3)D(";
		strLink += missionBase.missionSceneID.ToString();
		strLink += ",";
		strLink += missionBase.missionDuplicateID.ToString();
		strLink += ",";
		strLink += ((int)(EObjectType.TransPoint) ).ToString();
		strLink += ",";
		strLink += missionBase.missionConveyDoorID;
		strLink += ",";
		strLink += entryBase.Position.x.ToString();
		strLink += ",";
		strLink += entryBase.Position.y.ToString();
		strLink += ",";
		strLink += entryBase.Position.z.ToString();
		strLink += ")]";
		strLink += label.text;
		strLink += "[link=0]";
		
		label.text = strLink;
	}
	
	public string makeNpcCmdString(uint npcID){
		string strLink = "";
		
		uint sceneID = 0;
		Vector3 bronPos;
		bool bFind = false;
		//get scene ID where npc in
		foreach( KeyValuePair<uint,List<XCfgSceneNpcInfo> > infoPair in XCfgSceneNpcInfoMgr.SP.ItemTable )
		{
			foreach(XCfgSceneNpcInfo info in infoPair.Value)
			{
				if(npcID  == info.NpcID )
				{
					sceneID = infoPair.Key;
					bronPos = info.BornPos;
					bFind = true;
					break;
				}
			}
		}
		
		//not find
		if(false==bFind)
		{
			return "";
		}
		
		//导航对象，格式: 开启[link=1][linkdata=T(3)D(sceneID,ObjectType,id,posX,posY,posZ)]...[link=0]
		strLink += "T(3)D(";
		strLink += sceneID.ToString();
		strLink += ",";
		strLink += ((uint)0 ).ToString();
		strLink += ",";
		strLink += ((int)(EObjectType.Npc) ).ToString();
		strLink += ",";
		strLink += npcID.ToString();
		strLink += ",";
		//strLink += bronPos.x.ToString();
		strLink += ",";
		//strLink += bronPos.y.ToString();
		strLink += ",";
		//strLink += bronPos.z.ToString();
		strLink += ")";
		
		return strLink;
	}
	
	
}

