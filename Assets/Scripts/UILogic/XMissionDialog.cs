using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XMissionDialog")]
public class XMissionDialog : XDefaultFrame						
{	
	public GameObject m_ActionMissionShow = null;
	
	public GameObject m_CanReceiveMissionShow = null;
	
	public GameObject m_MissionName = null;
	
	public GameObject m_MissionNpcReceive = null;
	
	public GameObject m_MissionNpcRefer = null;
	
	public GameObject m_MissionMessage = null;
	
	public GameObject m_MissionTargetKill1 = null;
	
	public GameObject m_MissionTargetKill2 = null;
	
	public GameObject m_MissionTargetKill3 = null;
	
	public GameObject m_MissionTargetItem1 = null;
	
	public GameObject m_MissionTargetItem2 = null;
	
	public GameObject m_MissionTargetItem3 = null;
	
	public GameObject m_MissionAwardAGName = null;
	public GameObject m_MissionAwardAGNameStatic = null;
	
	public GameObject m_MissionAwardExp = null;
	
	public GameObject m_MissionAwardExpStatic = null;
	
	public GameObject m_MissionAwardMoney = null;
	
	public GameObject m_MissionAwardMoneyStatic = null;
	
	public GameObject m_MissionAwardRealMoney = null;
	
	public GameObject m_MissionAwardRealMoneyStatic = null;
	
	public GameObject m_MissionAwardPrestige = null;
	
	public GameObject m_MissionAwardPrestigeStatic = null;
	
	public XActionIcon m_MissionAwardItem1 = null;
	
	public XActionIcon m_MissionAwardItem2 = null;
	
	public XActionIcon m_MissionAwardItemJob = null;
	
	public GameObject m_MissionMessagePanael = null;
	public GameObject m_MissionAwardPanel = null;
	
	public GameObject m_MissionMessageBtn = null;
	
	public GameObject m_treeObjReceive = null;
	
	public GameObject m_treeObjCanReceive = null;
	
	private XBaseActionIcon m_MissionAwardItemLogic1 = new XBaseActionIcon();
	
	private XBaseActionIcon m_MissionAwardItemLogic2 = new XBaseActionIcon();
	
	private XBaseActionIcon m_MissionAwardItemJobLogic = new XBaseActionIcon();
	
	private uint m_uiCurrentFocusMission = 0;
	
	class MissionGroupNode
	{
		private UITree rootTree = null;
		
		public GameObject m_TreeParentTypeNode = null;
		
		public SortedList<uint ,GameObject> m_missionIDList = new SortedList<uint, GameObject>();
		
		public MissionGroupNode(UITree treeParent)
		{
			rootTree = treeParent;
		}
		
		public bool hasMissionID(uint missionID )
		{
			return m_missionIDList.ContainsKey(missionID );
		}
		
		//return need remove self
		public bool removeMission(uint missionID )
		{
			GameObject removeMissionObj = m_missionIDList[missionID ];
			
			m_TreeParentTypeNode.GetComponent<UITreeParentNode>().removeChildByObj(removeMissionObj );
			
			m_missionIDList.Remove(missionID );
			
			if(0 == m_missionIDList.Count )
			{
				GameObject.Destroy(m_TreeParentTypeNode );
				rootTree.GetComponent<UITree>().repositionNow();
				return true;
			}
			
			return false;
		}
		
		
	}
	
	private SortedList<uint,MissionGroupNode > m_MissionTypeListAction = new SortedList<uint,MissionGroupNode >();
	
	private SortedList<uint,MissionGroupNode > m_MissionTypeListCanReveive = new SortedList<uint,MissionGroupNode >();
	
	private List<MissionCallBack> m_missionCallBackList = new List<MissionCallBack>();
	
	private bool m_bShowActionMission = true;
	
	private string m_strNpcLink="";
	
	private uint m_missionID = 0;
	
	public override bool Init()
	{
		bool b = base.Init();
		
		m_tree.SetScrollBar(m_scrollbar);
		
		XEventManager.SP.AddHandler(referMissionEventListener,EEvent.Mission_ReferMission );
		
		XEventManager.SP.AddHandler(giveUpMissionEventListener,EEvent.Mission_GiveUpMission );
		
		XEventManager.SP.AddHandler(receiveMissionEventListener,EEvent.Mission_ReceiveMission );
		
		XEventManager.SP.AddHandler(PlayerLevelUpdate,EEvent.Update_Level );
		
		m_MissionAwardItemLogic1.SetUIIcon( m_MissionAwardItem1 );
		m_MissionAwardItemLogic2.SetUIIcon( m_MissionAwardItem2 );
		
		m_MissionAwardItemLogic1.IsCanPopMenu = false;
		m_MissionAwardItemLogic2.IsCanPopMenu = false;
		
		m_MissionAwardItemJobLogic.SetUIIcon( m_MissionAwardItemJob );
		m_MissionAwardItemJobLogic.IsCanPopMenu = false;
		
		//attach call back to show
		UIEventListener ListenerAction = UIEventListener.Get( m_ActionMissionShow );
		ListenerAction.onClick += actionMissionTreeShowCallBack;
		
		UIEventListener ListenerReceive = UIEventListener.Get( m_CanReceiveMissionShow );
		ListenerReceive.onClick += canReceiveMissionTreeShowCallBack;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return b;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eMissionDialog);
	}
	
	public override void Show ()
	{
		base.Show ();
		
		autoCheckShow();
	}
	
	public void ClickMissionLabelLink(GameObject go,string str)
	{
		HyperLinkMgr.SP.Process(str);
	}
	
	public void ClickMMBtnGiveUpMessageBox(GameObject go)
	{
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(m_missionID);
		
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(ClickMMBtnGiveUp);
		string content = XStringManager.SP.GetString(67) + missionBase.title + "?";
		
		XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,content);
	}
	
	public void ClickMMBtnGiveUp(GameObject go)
	{
		XMissionManager.SP.requestGiveUpMission( m_missionID );
	}
	
	public void ClickMMBtnNpcLink(GameObject go)
	{
		HyperLinkMgr.SP.Process(m_strNpcLink);
	}
	
	public class MissionCallBack
	{
		public uint m_missionID=0;
		public XMissionDialog missionDialog = null;
		public void actionClick( GameObject _go )
		{
			missionDialog.showMissionMessage(m_missionID);
		}
		
	}
	
	public UITree 	m_tree = null;
	public UIScrollBar m_scrollbar = null;
	
	private void actionMissionListUpdate(SortedList<uint, MissionMessage> missionList)
	{
		clearAll(m_treeObjReceive );
		m_MissionTypeListAction.Clear();
		
		foreach(KeyValuePair<uint, MissionMessage> mm in missionList )
		{
			XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(mm.Key);
			
			addMissionToTreeList(missionBase,m_treeObjReceive,m_MissionTypeListAction);
		}
		
	}
	
	private void canReceiveMissionUpdate(List<uint> missionList)
	{
		clearAll(m_treeObjCanReceive);
		m_MissionTypeListCanReveive.Clear();
		
		foreach(uint missionID in missionList )
		{
			XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID );
			
			addMissionToTreeList(missionBase,m_treeObjCanReceive,m_MissionTypeListCanReveive );
		}
	}
	
	private void PlayerLevelUpdate(EEvent evt, params object[] args )
	{
		//update now message may be need update some message about level
		showMissionMessage(m_uiCurrentFocusMission );
	}
	
	private void actionMissionEventListener(EEvent evt, params object[] args )
	{
		autoCheckShow();
	}
	
	private void receiveMissionEventListener(EEvent evt, params object[] args )
	{
		uint missionID = (uint)(args[0] );
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID );
		
		addMissionToTreeList( missionBase,m_treeObjReceive,m_MissionTypeListAction );
		
		removeMissionFromTreeList( missionBase,m_treeObjCanReceive,m_MissionTypeListCanReveive );
		
		showCurrentStateTree();
	}
	
	private void referMissionEventListener(EEvent evt, params object[] args )
	{
		uint missionID = (uint)(args[0] );
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID );
		
		removeMissionFromTreeList(missionBase,m_treeObjReceive,m_MissionTypeListAction );
		
		showCurrentStateTree();
	}
	
	private void giveUpMissionEventListener(EEvent evt, params object[] args )
	{
		uint missionID = (uint)(args[0] );
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID );
		
		removeMissionFromTreeList(missionBase,m_treeObjReceive,m_MissionTypeListAction );
		
		addMissionToTreeList( missionBase,m_treeObjCanReceive,m_MissionTypeListCanReveive );
		
		showCurrentStateTree();
	}
	
	private void showCurrentStateTree()
	{
		changeShowMissionState(m_bShowActionMission );
	}
	
	private void addMissionToTreeList(XCfgMission missionBase,GameObject rootTree,SortedList<uint,MissionGroupNode > MissionLogicList )
	{
		uint typeID = missionBase.typeID;
		XCfgMissionsType typeBase = XCfgMissionsTypeMgr.SP.GetConfig(typeID);
		string strTypeName = typeBase.typeName;
		
		if(!MissionLogicList.ContainsKey(typeID) )
		{
			UITree parentTree = rootTree.GetComponent<UITree>();
			
			GameObject parentObj = parentTree.insertNode(strTypeName ); //tree insert parent Node
			
			MissionLogicList.Add(typeID,new MissionGroupNode(parentTree ) ); //recode parentObj
			
			MissionLogicList[typeID].m_TreeParentTypeNode = parentObj;
		}
		
		GameObject child = rootTree.GetComponent<UITree>().insertItem( 
				missionBase.title,
				MissionLogicList[typeID].m_TreeParentTypeNode
				); // finaly insert title
		
		MissionLogicList[typeID].m_missionIDList.Add(missionBase.ID,child);
		
		//call back set m_missionCallBackList
		MissionCallBack mCall = new MissionCallBack();
		mCall.m_missionID = missionBase.ID;
		mCall.missionDialog = this;
		UIEventListener Listener = UIEventListener.Get( child );
		Listener.onClick += mCall.actionClick;
		
		m_missionCallBackList.Add(mCall );
		
	}
	
	private void removeMissionFromTreeList(XCfgMission missionBase,GameObject rootTree,SortedList<uint,MissionGroupNode > MissionLogicList )
	{
		uint typeID = missionBase.typeID;
		
		if(!MissionLogicList.ContainsKey(typeID) )
			return;
		
		if(!MissionLogicList[typeID].hasMissionID( missionBase.ID ) )
			return;
		
		//if need remove type node self
		if( MissionLogicList[typeID].removeMission(missionBase.ID ) ) //need remove parent node
		{
			MissionLogicList.Remove(typeID );
		}
		
	}
	
	private void autoCheckShow()
	{
		if(0 < XMissionManager.SP.getActionMissionList().Count )
		{
			manualShowActionMission();
		}else
		{
			manualShowCanReceiveMission();
		}
		
	}
	
	private void manualShowActionMission()
	{
		m_ActionMissionShow.GetComponent<UICheckbox>().isChecked = true;
		
		m_bShowActionMission = true;
		updateCurrentStateTree();
	}
	
	private void manualShowCanReceiveMission()
	{
		m_CanReceiveMissionShow.GetComponent<UICheckbox>().isChecked = true;
		
		m_bShowActionMission = false;
		updateCurrentStateTree();
	}
	
	private void clearAll(GameObject treeObj){
		treeObj.GetComponent<UITree>().clearTree();
		m_MissionTypeListAction.Clear();
		m_missionCallBackList.Clear();
	}
	
	private void actionMissionTreeShowCallBack(GameObject _go)
	{
		changeShowMissionState(true );
	}
	
	private void canReceiveMissionTreeShowCallBack(GameObject _go)
	{
		changeShowMissionState(false );
	}
	
	private void changeShowMissionState(bool isActionList )
	{
		m_bShowActionMission = isActionList;
		
		m_treeObjReceive.SetActive(false );
		m_treeObjCanReceive.SetActive(false );
		
	/*	if(!m_bShowActionMission )
		{
			if( 0 == m_MissionTypeListCanReveive.Count )
			{
				m_bShowActionMission = true;
				
				//button out side
				m_ActionMissionShow.GetComponent<UICheckbox>().isChecked = true;
				m_CanReceiveMissionShow.GetComponent<UICheckbox>().isChecked = false;
			}
		}else
		{
			if(0 == m_MissionTypeListAction.Count )
			{
				m_bShowActionMission = false;
				
				//button out side
				m_ActionMissionShow.GetComponent<UICheckbox>().isChecked = false;
				m_CanReceiveMissionShow.GetComponent<UICheckbox>().isChecked = true;
			}
		}
	*/	
		if(m_bShowActionMission )
		{
			m_treeObjReceive.SetActive(true );
			focusMissionMessage(m_MissionTypeListAction );
		}else
		{
			m_treeObjCanReceive.SetActive(true );
			focusMissionMessage(m_MissionTypeListCanReveive );
		}
		
	}
	
	private void updateCurrentStateTree()
	{
		if( m_bShowActionMission )
		{
			actionMissionListUpdate( XMissionManager.SP.getActionMissionList() );
		}else
		{
			canReceiveMissionUpdate( XMissionManager.SP.getCanReceiveMissionList() );
		}
		
		showCurrentStateTree();
		
		//may be some list change so I need update the current show
		initMissionShowMessage();
	}
	
	private void focusMissionMessage(SortedList<uint,MissionGroupNode > missionTypeList )
	{
		uint uiNewMissionID = 0;//0 is mean null
		
		if( 0 == missionTypeList.Count )
		{
			showMissionMessage(uiNewMissionID );
			return;
		}
		
		bool bNeedNewFocusMission = true;
		
		
		foreach(KeyValuePair<uint,MissionGroupNode> keyValue in missionTypeList )
		{
			foreach(KeyValuePair<uint,GameObject> keyValue2 in keyValue.Value.m_missionIDList )
			{
				if(0 == uiNewMissionID )//init new mission id just use the first
				{
					uiNewMissionID = keyValue2.Key;
				}
				
				if( m_uiCurrentFocusMission == keyValue2.Key)
				{
					bNeedNewFocusMission = false;
				}
			}
		}
		
		//not need
		if(bNeedNewFocusMission )
		{
			m_uiCurrentFocusMission = uiNewMissionID;
		}
		
		showMissionMessage(uiNewMissionID );
	}
	
	private void showMissionMessage(uint missionID)
	{	
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig(missionID);
		bool bIsActionMission = XMissionManager.SP.isActionMission(missionID);
		if(null == missionBase)
		{
			m_MissionMessagePanael.SetActive(false );
			m_uiCurrentFocusMission = 0;
			return;
		}else
		{
			m_MissionMessagePanael.SetActive(true );
			m_uiCurrentFocusMission = missionID;
		}
		
		if( XLogicWorld.SP.MainPlayer.Level < missionBase.needLevel)
		{
			m_MissionName.GetComponent<UILabel>().text = missionBase.title+"  [color=ff0000]"+XStringManager.SP.GetString(292 )+missionBase.needLevel.ToString();
		}else
		{
			m_MissionName.GetComponent<UILabel>().text = missionBase.title;
		}
		
		m_MissionNpcReceive.GetComponent<UILabel>().text = XCfgNpcBaseMgr.SP.GetConfig(missionBase.reciveNPCID ).Name;
		UIEventListener.Get(m_MissionNpcReceive).onClickHyperLink = ClickMissionLabelLink;
		XMissionManager.SP.makeNPCLableLink( m_MissionNpcReceive.GetComponent<UILabel>(),missionBase.reciveNPCID );
		
		UILabel referLabel = m_MissionNpcRefer.GetComponent<UILabel>();
		
		XCfgNpcBase npcBase = XCfgNpcBaseMgr.SP.GetConfig(missionBase.referNPCID );
		
		m_MissionNpcRefer.GetComponent<UILabel>().text = XCfgNpcBaseMgr.SP.GetConfig(missionBase.referNPCID ).Name;
		UIEventListener.Get(m_MissionNpcRefer).onClickHyperLink = ClickMissionLabelLink;
		XMissionManager.SP.makeNPCLableLink( m_MissionNpcRefer.GetComponent<UILabel>(),missionBase.referNPCID );
		
		m_MissionMessage.GetComponent<UILabel>().text = missionBase.missionMessage;
		
		showMissionAward(missionBase);
		showMissionTarget(missionBase);
		showMissionBtn(missionBase);
		//update position
		m_MissionMessagePanael.GetComponent<UITable>().repositionNow = true;
	}
	
	private void showMissionAward(XCfgMission missionBase)
	{
		float moneyParam = 1;
		float expParam = 1;
		float gangExpParam = 1;
		
		if(1==missionBase.isDynamic)
		{
			XCfgMissionsDynamic missionDyn = XCfgMissionsDynamicMgr.SP.GetConfig((uint)( XLogicWorld.SP.MainPlayer.Level) );
			XCfgMissionsDynamic missionDynBase = XCfgMissionsDynamicMgr.SP.GetConfig((uint)( missionBase.needLevel ) );
			
			moneyParam = missionDyn.moneyParam/missionDynBase.moneyParam;
			expParam = missionDyn.expParam/missionDynBase.expParam;
			gangExpParam = missionDyn.gangExpParam/missionDynBase.gangExpParam;
			
		}
		
		// 称号奖励 
		if ( 0== missionBase.awardTitle )
		{
			m_MissionAwardAGName.gameObject.SetActive(false);
			m_MissionAwardAGNameStatic.gameObject.SetActive(false);
		}
		else
		{
			m_MissionAwardAGName.gameObject.SetActive(true);
			m_MissionAwardAGNameStatic.gameObject.SetActive(true);
			XCfgNickName nickNameConfig = XCfgNickNameMgr.SP.GetConfig(missionBase.awardTitle);
			if ( null == nickNameConfig )
			{
				m_MissionAwardAGName.gameObject.SetActive(false);
				m_MissionAwardAGNameStatic.gameObject.SetActive(false);
			}
			m_MissionAwardAGName.GetComponent<UILabel>().text = nickNameConfig.NickName;
		}
		
		//money
		if(0==missionBase.awardMoney)
		{
			m_MissionAwardMoney.SetActive(false);
			m_MissionAwardMoneyStatic.SetActive(false);
		}else
		{
			m_MissionAwardMoney.SetActive(true);
			m_MissionAwardMoneyStatic.SetActive(true);
			
			string chrTemp = Mathf.CeilToInt(missionBase.awardMoney*moneyParam).ToString();
			m_MissionAwardMoney.GetComponent<UILabel>().text = chrTemp;
		}
		
		//exp
		if(0==missionBase.awardExp)
		{
			m_MissionAwardExp.SetActive(false);
			m_MissionAwardExpStatic.SetActive(false);
		}else
		{
			m_MissionAwardExp.SetActive(true);
			m_MissionAwardExpStatic.SetActive(true);
			
			string chrTemp = Mathf.CeilToInt(missionBase.awardExp*expParam).ToString();
			m_MissionAwardExp.GetComponent<UILabel>().text = chrTemp;
		}
		
		//ingot
		if(0==missionBase.awardIngot)
		{
			m_MissionAwardRealMoney.SetActive(false);
			m_MissionAwardRealMoneyStatic.SetActive(false);
		}else{
			m_MissionAwardRealMoney.SetActive(true);
			m_MissionAwardRealMoneyStatic.SetActive(true);
			
			string chrTemp = missionBase.awardIngot.ToString();
			m_MissionAwardRealMoney.GetComponent<UILabel>().text = chrTemp;
		}
		
		
		if(0==missionBase.awardItemID1 )
		{
			m_MissionAwardItem1.gameObject.SetActive(false);
		}else
		{
			m_MissionAwardItem1.gameObject.SetActive(true);
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(missionBase.awardItemID1 );
			if(cfgItem == null)
				return;
			
			m_MissionAwardItemLogic1.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,(ushort)missionBase.awardItemCount1);
			m_MissionAwardItemLogic1.SetLogicData(ActionIcon_Type.ActionIcon_Show,(int)missionBase.awardItemID1);
		}
		
		if(0==missionBase.awardItemID2 )
		{
			m_MissionAwardItem2.gameObject.SetActive(false);
		}else
		{
			m_MissionAwardItem2.gameObject.SetActive(true);
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(missionBase.awardItemID2 );
			if(cfgItem == null)
				return;
			
			m_MissionAwardItemLogic2.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,(ushort)missionBase.awardItemCount2);
			m_MissionAwardItemLogic2.SetLogicData(ActionIcon_Type.ActionIcon_Show,(int)missionBase.awardItemID2);
		}
		
		
		if(0==missionBase.awardChooseItemJob1 && 
		   0==missionBase.awardChooseItemJob2 &&
		   0==missionBase.awardChooseItemJob3
			)
		{
			m_MissionAwardItemJob.gameObject.SetActive(false);
		}else
		{
			m_MissionAwardItemJob.gameObject.SetActive(true);
			int playerClass = XLogicWorld.SP.MainPlayer.DynGet(XGame.Client.Packets.EShareAttr.esa_Class);
			
			uint itemID=0;
			switch(playerClass)
			{
			case 1:
				itemID = missionBase.awardChooseItemJob1;
				break;
			case 2:
				itemID = missionBase.awardChooseItemJob2;
				break;
			case 3:
				itemID = missionBase.awardChooseItemJob3;
				break;
			}
			
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemID );
			if(cfgItem == null)
				return;
			
			//job item award count only one i guess.....
			m_MissionAwardItemJobLogic.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,1);
			m_MissionAwardItemJobLogic.SetLogicData(ActionIcon_Type.ActionIcon_Show,(int)itemID);
			
		}
		
		m_MissionAwardItem1.transform.parent.GetComponent<UITable>().repositionNow = true;
		
		m_MissionAwardPanel.GetComponent<UITable>().repositionNow = true;
	}
	
	private void showMissionTarget(XCfgMission missionBase)
	{
		m_MissionTargetKill1.SetActive(false);
		m_MissionTargetKill2.SetActive(false);
		m_MissionTargetKill3.SetActive(false);
		m_MissionTargetItem1.SetActive(false);
		m_MissionTargetItem2.SetActive(false);
		m_MissionTargetItem3.SetActive(false);
		
		///////////////////////////////////////////////////////////////////////////
		if(0!=missionBase.referKillID1 )
		{
			m_MissionTargetKill1.SetActive(true);
			m_MissionTargetKill1.GetComponent<UILabel>().text = XMissionManager.SP.makeShowRequirementString(missionBase.ID,eMissionRequirement.eMR_Kill1);
			
			UIEventListener.Get(m_MissionTargetKill1).onClickHyperLink = ClickMissionLabelLink;
			XMissionManager.SP.makeTransPointLableLink(m_MissionTargetKill1.GetComponent<UILabel>(),missionBase.ID );
		}
		
		if(0!=missionBase.referKillID2 )
		{
			m_MissionTargetKill2.SetActive(true);
			m_MissionTargetKill2.GetComponent<UILabel>().text = XMissionManager.SP.makeShowRequirementString(missionBase.ID,eMissionRequirement.eMR_Kill2);
			
			UIEventListener.Get(m_MissionTargetKill2).onClickHyperLink = ClickMissionLabelLink;
			XMissionManager.SP.makeTransPointLableLink(m_MissionTargetKill2.GetComponent<UILabel>(),missionBase.ID );
		}
		
		if(0!=missionBase.referKillID3 )
		{	
			m_MissionTargetKill3.SetActive(true);
			m_MissionTargetKill3.GetComponent<UILabel>().text = XMissionManager.SP.makeShowRequirementString(missionBase.ID,eMissionRequirement.eMR_Kill3);
			
			UIEventListener.Get(m_MissionTargetKill3).onClickHyperLink = ClickMissionLabelLink;
			XMissionManager.SP.makeTransPointLableLink(m_MissionTargetKill3.GetComponent<UILabel>(),missionBase.ID );
		}
		
		if(0!=missionBase.referItemID1 )
		{
			m_MissionTargetItem1.SetActive(true);
			m_MissionTargetItem1.GetComponent<UILabel>().text = XMissionManager.SP.makeShowRequirementString(missionBase.ID,eMissionRequirement.eMR_Item1);
			
			UIEventListener.Get(m_MissionTargetItem1).onClickHyperLink = ClickMissionLabelLink;
			XMissionManager.SP.makeTransPointLableLink(m_MissionTargetItem1.GetComponent<UILabel>(),missionBase.ID );
		}
		
		if(0!=missionBase.referItemID2 )
		{
			m_MissionTargetItem2.SetActive(true);
			m_MissionTargetItem2.GetComponent<UILabel>().text = XMissionManager.SP.makeShowRequirementString(missionBase.ID,eMissionRequirement.eMR_Item2);
			
			UIEventListener.Get(m_MissionTargetItem2).onClickHyperLink = ClickMissionLabelLink;
			XMissionManager.SP.makeTransPointLableLink(m_MissionTargetItem2.GetComponent<UILabel>(),missionBase.ID );
		}
		
		if(0!=missionBase.referItemID3 )
		{
			m_MissionTargetItem3.SetActive(true);
			m_MissionTargetItem3.GetComponent<UILabel>().text = XMissionManager.SP.makeShowRequirementString(missionBase.ID,eMissionRequirement.eMR_Item3);
			
			UIEventListener.Get(m_MissionTargetItem3).onClickHyperLink = ClickMissionLabelLink;
			XMissionManager.SP.makeTransPointLableLink(m_MissionTargetItem3.GetComponent<UILabel>(),missionBase.ID );
		}
		
	}
	
	private void showMissionBtn(XCfgMission missionBase){
		
		m_missionID = missionBase.ID;
		
		UILabel[] mBtns = m_MissionMessageBtn.GetComponentsInChildren<UILabel>(true);
		
		if( XMissionManager.SP.isActionMission(missionBase.ID) )
		{
			MissionMessage mm = XMissionManager.SP.getActionMission(missionBase.ID);
			
			mBtns[0].text = XStringManager.SP.GetString( 67 );
			UIEventListener.Get(m_MissionMessageBtn).onClick = ClickMMBtnGiveUpMessageBox;  //give up
		}else
		{
			mBtns[0].text = XStringManager.SP.GetString( 65 );
			m_strNpcLink = XMissionManager.SP.makeNpcCmdString( missionBase.reciveNPCID );
			UIEventListener.Get(m_MissionMessageBtn).onClick = ClickMMBtnNpcLink;
		}
		
	}
	
	private void initMissionShowMessage()
	{
		//not null
		if( 0!=m_missionCallBackList.Count )
		{
			m_missionCallBackList[0].actionClick( null ); //action show message first
			m_MissionMessagePanael.SetActive(true);
		}else
		{
			m_MissionMessagePanael.SetActive(false);
		}
	}
}
