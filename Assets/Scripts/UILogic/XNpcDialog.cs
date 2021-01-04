using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Network;
using XGame.Client.Base.Pattern;

[AddComponentMenu("UILogic/XNpcDialog")]
public class XNpcDialog : XDefaultFrame						
{
		public UITexture m_NPCViewTexture;
		
		private uint m_NpcID = 0;
		
		public uint NpcID{
			private set{}
			get{ return m_NpcID;}
		}
		
		public GameObject m_ReceivePanel = null;
		
		public GameObject m_ReferPanel = null;
		
		public GameObject m_DoingPanel = null;
		
		public GameObject m_OpenPanel=null;
		
		public GameObject m_ExitBtn = null;
		
		public UILabel m_NpcName = null;
		
		public XActionIcon m_awardItemJobIcon = null;
		
		private XBaseActionIcon m_jobAwardItemLogic = new XBaseActionIcon();
		
		public XBaseActionIcon awardJobItem
		{
			get{return m_jobAwardItemLogic;}
		}
		
		public XActionIcon m_awardItemIcon1 = null;
		
		public XActionIcon m_awardItemIcon2 = null;
		
		private XBaseActionIcon m_awardItemLogic1 = new XBaseActionIcon();
		
		private XBaseActionIcon m_awardItemLogic2 = new XBaseActionIcon();
		
		public XBaseActionIcon awardItem1
		{
			get{return m_awardItemLogic1;}
		}
		
		public XBaseActionIcon awardItem2
		{
			get{return m_awardItemLogic2;}
		}
		
		public GameObject m_receiveButton = null;
		
		public GameObject m_rejectButton = null;
		
		public GameObject m_referButton = null;
		
		public GameObject m_iKnowButton = null;
		
		//public GameObject m_missionShowNode = null;
		
		public GameObject m_missionButton = null;
		
		public GameObject m_textNpc = null;
		
		public GameObject m_awardMoney = null;
		
		public GameObject m_awardExp = null;
		
		public GameObject m_awardIngot = null;
		
		public List<uint> receiveMissionList{
			get{ return m_NpcCurrent.receiveMissionList; }
		}
		
		public List<uint> referMissionList{
			get{ return m_NpcCurrent.referMissionList; }
		}
		
		static public XNpc m_NpcCurrent = null;
		
		private XStateMachince m_NPCDialogSM;
		
		private List<GameObject> m_openMissionButtonList = new List<GameObject>();
		
		public List<GameObject> openButtonList{
			get{ return m_openMissionButtonList; }
		}
		private uint m_missionID = 0;
		public uint missionID{
			get{return m_missionID;}
			set{m_missionID=value; }
		}
	
		public void openByNpcID(XNpc npc){
			m_NpcCurrent = npc;
		}
		
		public override bool Init()
		{
			base.Init();
			
			m_jobAwardItemLogic.SetUIIcon( m_awardItemJobIcon );
			
			m_awardItemLogic1.SetUIIcon( m_awardItemIcon1 );
			m_awardItemLogic2.SetUIIcon( m_awardItemIcon2 );	
			
			initStateMachince();
			XEventManager.SP.AddHandler(referMission,EEvent.Mission_ReferMission );
			XEventManager.SP.AddHandler(eventHideSelf,EEvent.NpcDialog_FarDistance );
			XEventManager.SP.AddHandler(MissionNpcUpdate,EEvent.Mission_NpcMissionUpdate );
			
			UIEventListener Listener = UIEventListener.Get(m_iKnowButton);
			Listener.onClick += hideSelf;
			
			Listener = UIEventListener.Get(m_rejectButton);
			Listener.onClick += hideSelf;
			
			Listener = UIEventListener.Get(m_receiveButton);
			Listener.onClick += hideSelf;
			Listener.onClick += requestReceiveMission;
			
			Listener = UIEventListener.Get(m_referButton);
			Listener.onClick += hideSelf;
			Listener.onClick += requestReferMission;
			
			Listener = UIEventListener.Get(m_ExitBtn);
			Listener.onClick += hideSelf;
		
			UIEventListener listenExit = UIEventListener.Get(ButtonExit);
			listenExit.onClick += Exit;
			
			return true;
		}
	
		public void Exit(GameObject go)
		{		
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eNpcDialog);
		}
		
		private void updateNpcMission(EEvent evt, params object[] args )
		{
			
		}
		
		public override void Show(){
			base.Show();
			
			if( null == m_NpcCurrent )
				return;
			
			m_NpcID = (uint)( m_NpcCurrent ).KindIndex;
			
			XModelRTTMgr.SP.AddNpcRTT(m_NpcID ,m_NPCViewTexture  );
			
			XCfgNpcBase npcBase = XCfgNpcBaseMgr.SP.GetConfig(m_NpcID);
			
			m_NpcName.text = npcBase.Name;
			
			showSelf();
		}
		
		public override void Hide()
		{
			base.Hide();
			m_NPCDialogSM.OnEvent((int)ENpcDialogEvent.eeReset);
		}
		
		private void initStateMachince()
		{
			m_NPCDialogSM = new XStateMachince(new XNpcDialogDufaultState(this) );
			m_NPCDialogSM.RegState( new XNpcDialogReceiveState(this) );
			m_NPCDialogSM.RegState( new XNpcDialogReferState(this) );
			m_NPCDialogSM.RegState( new XNpcDialogDoingState(this) );
			m_NPCDialogSM.RegState( new XNpcDialogOpenState(this) );
		}
		
		public void FixedUpdate()
		{
			XEventManager.SP.SendEvent(EEvent.NpcDialog_CheckSignal,null);
		}
		
		public void makeMissionAward(uint uiMissionID)
		{
			XCfgMission cfgMission = XCfgMissionMgr.SP.GetConfig(uiMissionID);
			
			XCfgMissionsDynamic missionDyn = XCfgMissionsDynamicMgr.SP.GetConfig((uint)( XLogicWorld.SP.MainPlayer.Level) );
			uint moneyParam = 1;
			uint expParam = 1;
			uint gangExpParam = 1;
			if(1==cfgMission.isDynamic)
			{
				moneyParam = missionDyn.moneyParam;
				expParam = missionDyn.expParam;
				gangExpParam = missionDyn.gangExpParam;
			}
			
			string chrTemp = XStringManager.SP.GetString(35);
			chrTemp+= (cfgMission.awardMoney*moneyParam);
			m_awardMoney.GetComponent<UILabel>().text = chrTemp;
			
			chrTemp = XStringManager.SP.GetString(36);
			chrTemp+= (cfgMission.awardExp*expParam);
			m_awardExp.GetComponent<UILabel>().text = chrTemp;
			
			chrTemp = XStringManager.SP.GetString(37);
			chrTemp+= cfgMission.awardIngot;
			m_awardIngot.GetComponent<UILabel>().text = chrTemp;
			
			if(0==cfgMission.awardItemID1 )
			{
				m_awardItemIcon1.gameObject.SetActive(false);
			}else
			{
				m_awardItemIcon1.gameObject.SetActive(true);
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(cfgMission.awardItemID1 );
				if(cfgItem == null)
					return;
				 
				m_awardItemLogic1.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,(ushort)cfgMission.awardItemCount1);
				m_awardItemLogic1.SetLogicData(ActionIcon_Type.ActionIcon_Show,(int)cfgMission.awardItemID1);
			}
			
			if(0==cfgMission.awardItemID2 )
			{
				m_awardItemIcon2.gameObject.SetActive(false);
			}else
			{
				m_awardItemIcon2.gameObject.SetActive(true);
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(cfgMission.awardItemID2 );
				if(cfgItem == null)
					return;
				 
				m_awardItemLogic2.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,(ushort)cfgMission.awardItemCount2);
				m_awardItemLogic2.SetLogicData(ActionIcon_Type.ActionIcon_Show,(int)cfgMission.awardItemID2);
			}
			
			if(0==cfgMission.awardChooseItemJob1 && 
			   0==cfgMission.awardChooseItemJob2 &&
			   0==cfgMission.awardChooseItemJob3
				)
			{
				m_awardItemJobIcon.gameObject.SetActive(false);
			}else
			{
				m_awardItemJobIcon.gameObject.SetActive(false);
			
				int playerClass = XLogicWorld.SP.MainPlayer.DynGet(XGame.Client.Packets.EShareAttr.esa_Class);
				uint itemID=0;
				switch(playerClass)
				{
				case 1:
					itemID = cfgMission.awardChooseItemJob1;
					break;
				case 2:
					itemID = cfgMission.awardChooseItemJob2;
					break;
				case 3:
					itemID = cfgMission.awardChooseItemJob3;
					break;
				}
				
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemID );
				if(cfgItem == null)
					return ;
				
				//job item award count only one i guess.....
				m_jobAwardItemLogic.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,1);
				m_jobAwardItemLogic.SetLogicData(ActionIcon_Type.ActionIcon_Show,(int)itemID);
				
				m_awardItemJobIcon.gameObject.SetActive(true);
			}
		}
		
		private void eventHideSelf(EEvent evt, params object[] args)
		{
			hideSelf(null);
		}
		
		private void hideSelf(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eNpcDialog); //hide the dialog by state maching 
			m_NPCDialogSM.OnEvent((int)ENpcDialogEvent.eeReset);
		}
		
		private void showSelf()
		{
			m_NPCDialogSM.OnEvent((int)ENpcDialogEvent.eeInitOpen);
		}
		
		private void showReceiveMission( uint missionID )
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eNpcDialog); //hide the dialog by state maching 
			m_NPCDialogSM.OnEvent((int)ENpcDialogEvent.eeReceiveMission,missionID);
		}
		
		private void updateMission()
		{	
			m_NPCDialogSM.OnEvent((int)ENpcDialogEvent.eeUpdateOpen );
		}
		
		private void requestReceiveMission(GameObject _go)
		{
			XMissionManager.SP.requestReceiveMission(m_missionID);
		}
		
		private void requestReferMission(GameObject _go)
		{
			XMissionManager.SP.requestReferMission( m_missionID );
		}
		
		private uint checkNextMission(uint referMissionID )
		{
			XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig( referMissionID );
			if( null == missionBase )
				return 0;
			
			if( 0 >= missionBase.nextID )
				return 0;
			
			XCfgMission nextMissionBase = XCfgMissionMgr.SP.GetConfig( missionBase.nextID );
			if(null == nextMissionBase)
				return 0;
			
			if(m_NpcID != nextMissionBase.reciveNPCID)
				return 0;
			
			if(null == m_NpcCurrent)
				return 0;
			
			if(false == m_NpcCurrent.isNpcCanShowDialog() )
				return 0;
			
			if(false == XNpc.missionReceiveCheck(nextMissionBase.ID,0 ) )
				return 0;
			
			return nextMissionBase.ID;
		}
		
		private void MissionNpcUpdate(EEvent evt, params object[] args )
		{
			int npcID = (int)args[0];
			
			if( (int)npcID != NpcID )
				return;
			
			m_NPCDialogSM.OnEvent((int)ENpcDialogEvent.eeInitOpen);
		}
		
		private void referMission(EEvent evt, params object[] args )
		{
			uint referMissionID = (uint)(args[0] );	
			
			uint nextMissionID = checkNextMission(referMissionID);
			
			if( 0!=nextMissionID )
			{
				showReceiveMission(nextMissionID );
			}
			
		}
		
}

public enum EStateNpcDialog
{
	esOpen,
	esReceive,
	esRefer,
	esDoing,
	esDefault,
}

public enum ENpcDialogEvent
{
	eeReset,
	eeInitOpen,
	eeReceiveMission,
	eeUpdateOpen,
}

//state machine
public abstract class XNpcDialogStateBase : XState<XNpcDialog>
{
	public XNpcDialogStateBase(EStateNpcDialog id, XNpcDialog owner)
		: base((int)id, owner)
	{
		
	}
}

public class XNpcDialogDufaultState:XNpcDialogStateBase
{
	public XNpcDialogDufaultState(XNpcDialog owner)
		: base(EStateNpcDialog.esDefault,owner )
	{
		
	}
	
	public override void Enter(params object[] args)
	{
		
	}
	
	public override bool OnEvent(int evt, params object[] args)
	{
		ENpcDialogEvent ENDE = (ENpcDialogEvent)evt;
		switch(ENDE)
		{
			case ENpcDialogEvent.eeInitOpen:
				Machine.TranslateToState((int)EStateNpcDialog.esOpen );
				break;
			case ENpcDialogEvent.eeReceiveMission:
				Machine.TranslateToState((int)EStateNpcDialog.esOpen,args );
				break;
		}
		
		return false;
	}
	
	public override void Exit(){
		
	}
}

public class XNpcDialogOpenState:XNpcDialogStateBase
{
	public class missionButtonCall
	{
		public uint missionID = 0;
		public XNpcDialog outPut = null;
		public XStateMachince stateSM = null;
		
		public void clickCallBack(GameObject _go)
		{
			outPut.missionID = missionID;
			
			if(XMissionManager.SP.isActionMission(missionID) )//already action
			{
				MissionMessage mm = XMissionManager.SP.getActionMission(missionID);
				if( mm.m_bMissionCompleted ) //completed can to refer
				{
					stateSM.TranslateToState((int)EStateNpcDialog.esRefer,missionID );
				}else 						 //can not refer is doing
				{
					stateSM.TranslateToState((int)EStateNpcDialog.esDoing,missionID );
				}
			}else //no action I can receive
			{
				stateSM.TranslateToState((int )EStateNpcDialog.esReceive,missionID );
			}
		}
		
	}
	
	//shop button call back class
	public class shopButtonCall
	{
		public uint m_npcID = 0;
		
		public void clickCallBack(GameObject _go)
		{
			//show the shop dialog 
			XShopItemMgr.SP.npcID = m_npcID;
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eShopDialog );  //show shop
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eNpcDialog );  //hide npc dialog
		}
	}
	
	//HillSeaBook call back class
	public class HillSeaBookCall
	{
		public void clickCallBack(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eHillSeaBook );  //show shop
		}
	}
	
	// call back class
	public class ZhanYaoLuCall
	{
		public void clickCallBack(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eZhanYaoLu ); 
		}
	}
	
	private List<missionButtonCall> buttonCallList = new List<missionButtonCall>();
	
	public XNpcDialogOpenState(XNpcDialog owner)
		: base(EStateNpcDialog.esOpen,owner )
	{
		m_shopCall = new shopButtonCall();
		
		m_hillSeaBookCall = new HillSeaBookCall();
		
		m_ZhanYaoLuCall = new ZhanYaoLuCall();  
	}
	
	GameObject makeNewButton(string strShow)
	{
		GameObject obj = GameObject.Instantiate( m_Owner.m_missionButton ) as GameObject;			
		Vector3 pos = m_Owner.m_missionButton.transform.localPosition;
		pos.y = pos.y-m_Owner.openButtonList.Count*m_Owner.m_missionButton.transform.Find("Background").gameObject.transform.localScale.y;
		obj.SetActive(true);
		UILabel label = obj.GetComponentInChildren<UILabel>();
		
		obj.transform.parent = m_Owner.m_OpenPanel.transform;
		obj.transform.localPosition = pos;
		obj.transform.localScale 	= m_Owner.m_missionButton.transform.localScale;
		m_Owner.openButtonList.Add(obj);
		label.text  = strShow;
		
		return obj;
	}
	
	private void clearOpenButton()
	{
		foreach( GameObject b in m_Owner.openButtonList )
		{
			GameObject.Destroy(b);
		}
		
		m_Owner.openButtonList.Clear();
		buttonCallList.Clear();
		
		
	}
	
	private void createOpenButton()
	{
		//refer mission list need show
		foreach( uint missionID in m_Owner.referMissionList )
		{
			GameObject obj = createMissionButton(missionID );
			
			//new guy
			if ( null!=obj && 999u == missionID )
			{
				UILabel label = obj.GetComponentInChildren<UILabel>();
				if ( null != label )
				{
					Vector3 showPos = new Vector3(200, 10, Mathf.Round(label.transform.localPosition.z));
					XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_Mission_Finish, 
						1, showPos, obj);
				}
			}
		}
		
		//(m_Owner.m_missionButton)  receive mission list show
		foreach( uint missionID in m_Owner.receiveMissionList )
		{
			createMissionButton(missionID );
		}
		
		//this npc has shop item mean need shop button
		if( XNpc.checkNPCFunction(m_Owner.NpcID,XNpc.eNpcFction.eNf_shop) )
		{
			GameObject obj = makeNewButton(XStringManager.SP.GetString(44) );
			UIEventListener Listener = UIEventListener.Get(obj);
			m_shopCall.m_npcID = m_Owner.NpcID;
			Listener.onClick += m_shopCall.clickCallBack;
		}else if(XNpc.checkNPCFunction(m_Owner.NpcID,XNpc.eNpcFction.eNf_hillSeaBook) ) //hill sea book btn
		{
			GameObject obj = makeNewButton(XStringManager.SP.GetString(510) );
			UIEventListener Listener = UIEventListener.Get(obj);
			Listener.onClick += m_hillSeaBookCall.clickCallBack;
		}
		if(XNpc.checkNPCFunction(m_Owner.NpcID,XNpc.eNpcFction.eNf_zhanYaoLu) ) 
		{
			GameObject obj = makeNewButton(XStringManager.SP.GetString(511) );
			UIEventListener Listener = UIEventListener.Get(obj);
			Listener.onClick += m_ZhanYaoLuCall.clickCallBack;
		}
	}
	
	private GameObject createMissionButton(uint missionID)
	{
		//must not be less 5 buttons
		if(5<=m_Owner.openButtonList.Count )
		{
			return null;
		}
		
		GameObject obj = makeNewButton(XMissionManager.SP.makeMissionSuffitx(missionID));
			
		//call back class set
		missionButtonCall bc = new missionButtonCall();
		bc.outPut = m_Owner;
		bc.stateSM = Machine;
		bc.missionID = missionID;
			
		UIEventListener Listener = UIEventListener.Get(obj);
		Listener.onClick += bc.clickCallBack;
			
		buttonCallList.Add(bc);	
		
		return obj;
	}
	
	public override void Enter(params object[] args){
		
		m_Owner.m_OpenPanel.SetActive( true );
		createOpenButton();
		
		//random npc open world 
		string[] talkGroup = XCfgNpcBaseMgr.SP.GetConfig(m_Owner.NpcID).Talk;
		string talk = talkGroup[ Random.Range(0,talkGroup.Length)];
		m_Owner.m_textNpc.GetComponent<UILabel>().text = talk;
		
		m_shopCall.m_npcID = m_Owner.NpcID;
		
		if(0<args.Length)
		{
			//direct go to receive panel
			Machine.TranslateToState((int )EStateNpcDialog.esReceive,(uint)args[0]  );
		}
		
		
		
	}
	
	public override void Breathe(float deltaTime){
		
	}
	public override bool OnEvent(int evt, params object[] args){
		
		ENpcDialogEvent ENDE = (ENpcDialogEvent)evt;
		switch(ENDE)
		{
			case ENpcDialogEvent.eeReset:
				Machine.TranslateToState((int)EStateNpcDialog.esDefault );
				break;
			case ENpcDialogEvent.eeReceiveMission:
				Machine.TranslateToState((int )EStateNpcDialog.esReceive,(uint)args[0]  );
				break;
			case ENpcDialogEvent.eeInitOpen:		//need update again open button
				clearOpenButton();//first clear
				createOpenButton();//and create
				break;
		}
		
		return false;
	}
	
	public override void Exit(){
		clearOpenButton();
		m_Owner.m_OpenPanel.SetActive( false );
	}
	
	//item
	private shopButtonCall m_shopCall;
	
	private HillSeaBookCall m_hillSeaBookCall;
	
	private ZhanYaoLuCall m_ZhanYaoLuCall;
}

public class XNpcDialogReceiveState:XNpcDialogStateBase
{
	
	public XNpcDialogReceiveState(XNpcDialog owner)
		: base(EStateNpcDialog.esReceive,owner )
	{
		
	}
	
	public override void Enter(params object[] args){
		uint missionID = (uint)args[0];
		m_Owner.missionID = missionID;
		m_Owner.m_ReceivePanel.SetActive( true );
		//m_Owner.m_missionShowNode.SetActive(true);
		XCfgMission cfgMission = XCfgMissionMgr.SP.GetConfig(missionID);
		if(null==cfgMission)
		{
			return;
		}
		
		m_Owner.m_textNpc.GetComponent<UILabel>().text = cfgMission.missionMessage;
		
		m_Owner.m_receiveButton.SetActive(true );
		UILabel[] labels = m_Owner.m_receiveButton.GetComponentsInChildren<UILabel>(true);
		if(0<labels.Length)
		{
			labels[0].text = cfgMission.NPCRecive;
		}
		
		labels = m_Owner.m_rejectButton.GetComponentsInChildren<UILabel>(true);
		m_Owner.m_rejectButton.SetActive(true );
		if(0<labels.Length)
		{
			labels[0].text = cfgMission.NPCDisclaim;
		}
		
	}
	
	public override void Breathe(float deltaTime){}
	public override bool OnEvent(int evt, params object[] args){ 
		ENpcDialogEvent ENDE = (ENpcDialogEvent)evt;
		switch(ENDE)
		{
		case ENpcDialogEvent.eeReset:
			Machine.TranslateToState((int)EStateNpcDialog.esDefault);
			break;
		}
		return false;
	}
	
	public override void Exit(){
		m_Owner.m_ReceivePanel.SetActive( false );
		//m_Owner.m_missionShowNode.SetActive(false);
	}
	
}

public class XNpcDialogReferState:XNpcDialogStateBase
{
	public XNpcDialogReferState(XNpcDialog owner)
		: base(EStateNpcDialog.esRefer,owner )
	{
		
	}
	
	public override void Enter(params object[] args){
		uint missionID = (uint)args[0];
		m_Owner.missionID = missionID;
		m_Owner.m_ReferPanel.SetActive( true );
		//m_Owner.m_missionShowNode.SetActive(true);
		
		XCfgMission cfgMission = XCfgMissionMgr.SP.GetConfig(missionID);
		if(null==cfgMission)
		{
			return;
		}
		
		m_Owner.m_textNpc.GetComponent<UILabel>().text = cfgMission.NPCCompleted;
		
		UILabel[] labels = m_Owner.m_referButton.GetComponentsInChildren<UILabel>(true);
		
		if(0<labels.Length)
		{
			labels[0].text = cfgMission.NPCCompletedAsk;
		}
	}
	
	public override void Breathe(float deltaTime){}
	public override bool OnEvent(int evt, params object[] args){ 
		ENpcDialogEvent ENDE = (ENpcDialogEvent)evt;
		switch(ENDE)
		{
		case ENpcDialogEvent.eeReset:
			Machine.TranslateToState((int)EStateNpcDialog.esDefault);
			break;
		}
		return false;
	}
	
	public override void Exit(){
		m_Owner.m_ReferPanel.SetActive( false );
		//m_Owner.m_missionShowNode.SetActive(false);
	}
	
}

public class XNpcDialogDoingState:XNpcDialogStateBase
{
	public XNpcDialogDoingState(XNpcDialog owner)
		: base(EStateNpcDialog.esDoing,owner )
	{
		
	}
	
	public override void Enter(params object[] args){
		uint missionID = (uint)args[0];
		m_Owner.m_DoingPanel.SetActive( true );
		
		XCfgMission cfgMission = XCfgMissionMgr.SP.GetConfig(missionID);
		if(null==cfgMission)
		{
			return;
		}
		
		m_Owner.m_textNpc.GetComponent<UILabel>().text = cfgMission.NPCUncompleted;
		
		UILabel[] labels = m_Owner.m_iKnowButton.GetComponentsInChildren<UILabel>(true);
		if(0<labels.Length)
		{
			labels[0].text = cfgMission.NPCUncompletedAsk;
		}
	}
	
	public override void Breathe(float deltaTime){}
	public override bool OnEvent(int evt, params object[] args){ 
		ENpcDialogEvent ENDE = (ENpcDialogEvent)evt;
		switch(ENDE)
		{
		case ENpcDialogEvent.eeReset:
			Machine.TranslateToState((int)EStateNpcDialog.esDefault);
			break;
		}
		return false;
	}
	
	public override void Exit(){
		m_Owner.m_DoingPanel.SetActive( false );
	}
	
}