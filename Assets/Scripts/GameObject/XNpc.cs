using System;
using XGame.Client.Packets;
using UnityEngine;
using Google.ProtocolBuffers;
using System.Collections.Generic;
using System.Collections;

public enum ENpc_HeadEffect
{
	ENHE_Effect_CanReceive 	= 900039,
	ENHE_Effect_CanRefer	= 900040,
	ENHE_Effect_Action		= 900041,
}

public class XNpc : XCharacter
{
	// 以静态传送门ID为索引的所有npc对象列表(当前Appear)
	private static SortedList<int, XNpc> m_allNpc = new SortedList<int, XNpc>();
	
	public static XNpc GetByKindIndex(int kindIndex)
	{
		if(!m_allNpc.ContainsKey(kindIndex))
			return null;
		return m_allNpc[kindIndex];
	}
	
    public int KindIndex { get; private set; }
	private string m_strHeadIcon = "";
	private static readonly uint SelectEffect = 900022;
	
	public enum eNpcFction
	{
		eNf_warehouse=1,
		eNf_auction=2,
		eNf_hillSeaBook=4,
		eNf_shop=8,
		eNf_outLink=16,
		eNf_zhanYaoLu=32,
	}
	
	
	private static int MIN_ACTION_TIME = 10;
	private static int MAX_ACTION_TIME = 20;
	private TimeCalc mTime = new TimeCalc();
	private static int MAX_RANDOM_ANIMATION_NUM = 2;
	
	private List<uint> m_mainPlayerReceiveMissionList = new List<uint>();
		
	private List<uint> m_referMissionList = new List<uint>();
	
	private List<uint> m_baseReceiveMissionList = new List<uint>();
	
	private List<uint> m_baseReferMissionList = new List<uint>();
	
	public List<uint> receiveMissionList{
		get{ return m_mainPlayerReceiveMissionList; }
	}
	
	public List<uint> referMissionList{
		get{ return m_referMissionList; }
	}
	
    public XNpc(ulong id) : base(id)
    {
        ObjectType = EObjectType.Npc;
    }
	
	public override EHeadBoard_Type GetHeadBoardType() { return EHeadBoard_Type.EHeadBoard_Type_NPC; }
	
	public override void Breathe()
	{
		base.Breathe();
		RandomAction();
	}
	
	private void RandomAction()
	{
		if ( !mTime.IsStart() )
		{
			int time = UnityEngine.Random.Range(MIN_ACTION_TIME,MAX_ACTION_TIME);
			mTime.BeginTimeCalc(time,false);
		}

		if ( mTime.CountTime(Time.deltaTime) )
		{
			//到了的话，重新刷新间隔时间
			int time = UnityEngine.Random.Range(MIN_ACTION_TIME,MAX_ACTION_TIME);
			mTime.BeginTimeCalc(time,false);
	
			//怪物播放动作
			int actionID = UnityEngine.Random.Range(0,MAX_RANDOM_ANIMATION_NUM);
			SendModelEvent(EModelEvent.evtAnimation,EAnimName.Specialidle,1.0f,false);
			SendModelEvent(EModelEvent.evtAnimation,EAnimName.Idle,1.0f,true);
		}
	}
	
	public  override void OnModelLoaded()
	{
		base.OnModelLoaded();
		m_ObjectModel.AddBehaviourListener(EBehaviourType.e_BehaviourType_Npc,this);
	}
	
	public override void Appear ()
	{
		base.Appear ();
		if(!m_allNpc.ContainsKey(KindIndex))
		{
			m_allNpc.Add(KindIndex, this);
		}
		if ( null != m_ObjectModel )
			m_ObjectModel.index = KindIndex;
		SendModelEvent(EModelEvent.evtNpcFuncHead, m_strHeadIcon);
	}
	
    public override void SetAppearData(object data)
    {
        base.SetAppearData(data);

        SC_NpcAppearData msg = data as SC_NpcAppearData;
        if (null == msg)
        {
            return;
        }

	    Vector3 pos = Position;
        Version = msg.Version;
	
        if (msg.HasPosX) pos.x = msg.PosX;
        if (msg.HasPosZ) pos.z = msg.PosZ;
		Position = pos;
		PushOnTerrain();
        if (msg.HasDirection) Direction = (new Vector3(0, msg.Direction, 0));
        if (msg.HasKindIndex) KindIndex = msg.KindIndex;
		XCfgNpcBase cfgNpcBase = XCfgNpcBaseMgr.SP.GetConfig((uint)KindIndex);
		Name = cfgNpcBase.Name;
		SetModel(EModelCtrlType.eModelCtrl_Original, cfgNpcBase.ModelId);
		Sex = cfgNpcBase.Sex;
		Level = (int)cfgNpcBase.Level;
		Hp = MaxHp = cfgNpcBase.MaxHp;
		if(cfgNpcBase.Title == "0" || cfgNpcBase.Title == "")
		{
			NickName = "";
		}
		else
		{
			NickName = cfgNpcBase.Title;
		}
		initBaseMissionList();
		
		XEventManager.SP.AddHandler(missionDataNeedChange, EEvent.Mission_ActionState);
		XEventManager.SP.AddHandler(missionDataNeedChange, EEvent.Mission_CanReceiveListUpdate);
		
		makeNewMissionLists();
    }
	
	public override void DisAppear ()
	{
		base.DisAppear ();
		if(m_allNpc.ContainsKey(KindIndex))
		{
			m_allNpc.Remove(KindIndex);
			XNewPlayerGuideManager.SP.handleNpcDisapper(KindIndex);
		}
	}
	
	private void makeNewMissionLists()
	{
		checkReceiveMissionList();
		
		checkReferMissionList();
		
		updateNpcHeadIcon();
		
		XEventManager.SP.SendEvent(EEvent.Mission_NpcMissionUpdate,KindIndex );
	}
	
	public override float GetClickDistance ()
	{
		float d = Radius() + XLogicWorld.SP.MainPlayer.Radius() + 0.8f;	// 加上一个矫值
		if(d > Define.GOOD_NPC_CLICK_DISTANCE) d = Define.GOOD_NPC_CLICK_DISTANCE;
		return d;
	}
	
	//on mouse button down click NPC
    public override void OnMouseUpAsButton(int mouseCode)
    {
		if(0 != mouseCode) return;
		
        //--4>TODO: 判断 NPC 类型, 阵营等, 目前只是显示对话框
        Vector3 npcPos = Position;
        Vector3 myPos = XLogicWorld.SP.MainPlayer.Position;
		float distace = XUtil.CalcDistanceXZ(npcPos, myPos);
		
        if (distace > GetClickDistance())
        {
			XLogicWorld.SP.MainPlayer.AutoMoveTo(this);			
        }
        else//yes i can show
        {
			XNpcDialog.m_NpcCurrent = this;
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eNpcDialog);
			
			if ( KindIndex == 2 )
				XNewPlayerGuideManager.SP.handleHideMissionGuide();
        }
    }
	
	public bool isNpcCanShowDialog()
	{
		Vector3 npcPos = Position;
        Vector3 myPos = XLogicWorld.SP.MainPlayer.Position;
		float distace = XUtil.CalcDistanceXZ(npcPos, myPos);
		
        if (distace > GetClickDistance() )
		{
			return false;
		}else
		{
			return true;
		}
		
	}
	
    public void OnSelectEvent(uint eventId, uint eventType, uint param)
    {
        CS_SelectDialogEvent.Builder builder = CS_SelectDialogEvent.CreateBuilder();
        builder.SetNpcId(ID);
        builder.SetEventId(eventId);
        if (eventType > 0)
        {
            builder.SetEventType(eventType);
            builder.SetParam(param);
        }
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_SelectDialogEvent, builder.Build());
    }
	
	public void initBaseMissionList()
	{
		m_baseReceiveMissionList.Clear();
		m_baseReferMissionList.Clear();
		//ready all of Missions get this NPC missions ID
		foreach( KeyValuePair<uint,XCfgMission> m in XCfgMissionMgr.SP.ItemTable )
		{
			if( (uint)KindIndex == m.Value.reciveNPCID )
			{
				m_baseReceiveMissionList.Add( m.Value.ID );
			}
			
			if( (uint)KindIndex == m.Value.referNPCID )
			{
				m_baseReferMissionList.Add( m.Value.ID );
			}
		}
	}
	
	static public bool checkNPCFunction(uint NPC_ID,eNpcFction function)
	{
		uint uiFunction=(uint)function;
		
		if(0!=(uiFunction & XCfgNpcBaseMgr.SP.GetConfig(NPC_ID).Function) )
		{
			return true;
		}else
		{
			return false;
		}
	}
	
	public void checkReceiveMissionList()
	{
		m_mainPlayerReceiveMissionList = checkReceiveMissionList(m_baseReceiveMissionList,0);
	}
	
	static public List<uint> checkReceiveMissionList(List<uint> receiveList,uint levelOffset)
	{
		List<uint> checkList = new List<uint>();
		
		//XLogicWorld.SP.MainPlayer;
		
		foreach(uint missionID in receiveList) // action list check
		{
			if(true == missionReceiveCheck(missionID,levelOffset) )
				checkList.Add(missionID);	
		}
		
		return checkList;
		
	}
	
	static public bool missionReceiveCheck( uint missionID , uint levelOffset )
	{
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID);
			
		XMainPlayer player = XLogicWorld.SP.MainPlayer;
			
		if( XMissionManager.SP.isActionMission(missionID) ) //is actioning mission not need receive
		{
			return false;
		}
			
		if( XMissionManager.SP.hasReferMissionInList( missionID ) ) //already refer
		{
			return false;
		}
			
		if ( 0 != missionBase.needJob && (player.DynGet(EShareAttr.esa_Class) ) != (missionBase.needJob) )
		{
			return false;
		}
			
		if ( 0 != missionBase.needLevel && (player.Level+levelOffset) < missionBase.needLevel )
		{
			return false;
		}
			
		if ( 0 != missionBase.needMissionID && !XMissionManager.SP.hasReferMissionInList(missionBase.needMissionID) )
		{
			return false;
		}
			
		return true;
	}
	
	public void checkReferMissionList()
	{
		m_referMissionList.Clear();
		//XLogicWorld.SP.MainPlayer;
		
		foreach(uint missionID in m_baseReferMissionList) // action list check
		{
			if ( XMissionManager.SP.isActionMission(missionID) ) //need it's already in action
			{
				m_referMissionList.Add(missionID );
			}
		}
		
	}
	
	private void missionDataNeedChange(EEvent evt, params object[] args)
	{
		makeNewMissionLists();
		
		SendModelEvent(EModelEvent.evtNpcFuncHead, m_strHeadIcon);
	}
	
	public override void OnMouseDown(int mouseCode, Vector3 clickPoint)
	{		
		SendModelEvent(EModelEvent.evtSelect,SelectEffect,true);
	}
	
	private void updateNpcHeadIcon()
	{
		m_strHeadIcon = "";
		
		foreach(uint missionID in m_referMissionList)
		{
			if(XMissionManager.SP.getActionMission(missionID).m_bMissionCompleted)
			{
				m_strHeadIcon = "11000687";//yellow ? icon
				return ;
			}
		}
		
		if( 0 < m_mainPlayerReceiveMissionList.Count )
		{
			m_strHeadIcon = "11000686";    // yellow ! icon
			return;
		}
		
		
		foreach(uint missionID in m_referMissionList)
		{
			if(!XMissionManager.SP.getActionMission(missionID).m_bMissionCompleted)
			{
				m_strHeadIcon = "11000688";//white ? icon
				return;
			}
		}
		
		XCfgNpcBase npcBase = XCfgNpcBaseMgr.SP.GetConfig((uint)KindIndex);
		
		if( null == npcBase )
		{
			m_strHeadIcon = "";
			return;
		}
		
		//get base icon from npc
		m_strHeadIcon = npcBase.headIcon;
		
		if("0"==m_strHeadIcon)
		{
			m_strHeadIcon = "";  //empty icon
			return;
		}
		
		return;
	}
	
}
