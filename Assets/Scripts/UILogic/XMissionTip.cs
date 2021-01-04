using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XMissionTip")]
public class XMissionTip : XUIBaseLogic						
{
	public GameObject m_missioniItem = null;
	public GameObject m_missionMessage = null;
	public GameObject m_missionName = null;
	public GameObject m_dragPanel = null;
	
	public GameObject m_MissionDialogBtn = null;
	
	public UISpriteGroup m_pageGroup = null;
	
	private bool m_bActionState = true;
	
	private SortedList<uint,GameObject> m_ItemList = new SortedList<uint,GameObject>();
	
	private float m_fStartY = 0.0f;
	
	public static Transform PlayerGuideMoveRefernce;
	
	public static float RightXpos = 0f;
	
	public void ClickMissionLabel(GameObject go,string str)
	{
		XNewPlayerGuideManager.SP.handleHideMissionGuide();
		
		HyperLinkMgr.SP.Process(str);
	}
	
	public  void OnSelectModify(int index)
	{
		if(index == 0)
		{
			showActionMissionListCallBack(null);
		}
		else
		{
			showCanReceiveMissionListCallBack(null);
		}
	}
	
	public override bool Init()
	{
		base.Init();
		
		XEventManager.SP.AddHandler(updateCurrent,EEvent.Mission_ActionState );
		XEventManager.SP.AddHandler(updateCurrent,EEvent.Mission_CanReceiveListUpdate );
		
		XEventManager.SP.AddHandler(missionReceiveListen,EEvent.Mission_ReceiveMission );
		XEventManager.SP.AddHandler(missionGiveUpListen,EEvent.Mission_GiveUpMission );
		XEventManager.SP.AddHandler(missionReferListen,EEvent.Mission_ReferMission );
		
		//attach call back to show
		m_pageGroup.mModify	= OnSelectModify;
		
		UIEventListener ListenerMissionDialogBtn = UIEventListener.Get( m_MissionDialogBtn );
		ListenerMissionDialogBtn.onClick += showMissionDialog;
		//init
		
		autoCheckShow();
		
		PlayerGuideMoveRefernce = transform;
		RightXpos = transform.position.x;
		
		return false;
	}
	
	//create all of missionManager actionMission list show in the MissionTip
	private void actionMissionListUpdate()
	{
		clearItemList();
		SortedList<uint, MissionMessage> missionList = XMissionManager.SP.getActionMissionList();
		
		m_fStartY = 0.0f;
		
		foreach(KeyValuePair<uint,MissionMessage>  mm in missionList)
		{
			addItemList(mm.Key);
		}
		m_dragPanel.GetComponent<UITable>().repositionNow = true;
	}
	
	private void missionListener(EEvent evt, params object[] args)
	{
		autoCheckShow();
	}
	
	private void missionReceiveListen(EEvent evt, params object[] args )
	{
		manualActionMissionShow();
	}
	
	private void missionGiveUpListen(EEvent evt, params object[] args )
	{
		autoCehckCurrentStateShow();
	}
	
	private void missionReferListen(EEvent evt, params object[] args )
	{
		autoCehckCurrentStateShow();
	}
	
	private void updateCurrent(EEvent evt, params object[] args )
	{
		updateCurrentState();
	}
	
	private void canReceiveMissionListUpdate()
	{
		clearItemList();
		List<uint> missionList = XMissionManager.SP.getCanReceiveMissionList();
		m_fStartY = 0.0f;
		
		foreach( uint missionID in missionList )
		{
			addItemList(missionID);
			if ( 999u == missionID )
				XMissionManager.SP.requestReceiveMission(missionID);
		}
		m_dragPanel.GetComponent<UITable>().repositionNow = true;
	}
	
	private void clearItemList()
	{
		foreach(KeyValuePair<uint,GameObject> obj in m_ItemList)
		{
			GameObject.Destroy(obj.Value);
		}
		m_ItemList.Clear();
	}
	
	private float makeLable(GameObject cloneObj,string strText,GameObject parentObj,float startY,uint missionID)
	{
		string strOutLine = "";
		GameObject newLable = GameObject.Instantiate(cloneObj,cloneObj.transform.position,cloneObj.transform.rotation) as GameObject;
		
		UIEventListener.Get(newLable).onClickHyperLink = ClickMissionLabel;
		
		newLable.GetComponent<UILabel>().text =strOutLine + strText; // show mission Name
		
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig( missionID );
		
		if(XMissionManager.SP.isActionMission(missionID) ) //is action 
		{
			MissionMessage msg = XMissionManager.SP.getActionMission(missionID);
			
			if( true == msg.m_bMissionCompleted) //completed to find refer NPC
			{
				XMissionManager.SP.makeNPCLableLink( newLable.GetComponent<UILabel>(),missionBase.referNPCID );  // to find receive npc
			}else //not completed need to fight
			{
				XMissionManager.SP.makeTransPointLableLink(newLable.GetComponent<UILabel>(),missionID );
			}
			
		}else // not action need to receive
		{
			XMissionManager.SP.makeNPCLableLink( newLable.GetComponent<UILabel>(),missionBase.reciveNPCID );  // to find receive npc
		}
		
		newLable.transform.parent = parentObj.transform; //attach to parentObj
		
		newLable.transform.localScale = cloneObj.transform.localScale;
		
		Vector3 newPos = newLable.transform.localPosition;
		
		newPos.y = startY;
		newLable.transform.localPosition = newPos;       //new pos begin from startY
		newLable.SetActive(true);						 //show new item
		startY -= newLable.transform.localScale.y;
		startY -= 3.0f;
		return startY;									 //return new y
	}
	
	private float makeMissionNameLable(GameObject cloneObj,string strText,GameObject parentObj,float startY,uint missionID)
	{
		string strOutLine = "";
		GameObject newLable = GameObject.Instantiate(cloneObj,cloneObj.transform.position,cloneObj.transform.rotation) as GameObject;
		
		UIEventListener.Get(newLable).onClickHyperLink = ClickMissionLabel;
		
		newLable.GetComponent<UILabel>().text =strOutLine + strText; // show mission Name
		
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig( missionID );
		
		if(XMissionManager.SP.isActionMission(missionID) ) //is action 
		{
			MissionMessage msg = XMissionManager.SP.getActionMission(missionID);
			
			if( true == msg.m_bMissionCompleted) //completed to find refer NPC
			{
				XMissionManager.SP.makeNPCLableLink( newLable.GetComponent<UILabel>(),missionBase.referNPCID );  // to find receive npc
			}else //not completed need to fight
			{
				//XMissionManager.SP.makeTransPointLableLink(newLable.GetComponent<UILabel>(),missionID );
			}
			
		}else // not action need to receive
		{
			XMissionManager.SP.makeNPCLableLink( newLable.GetComponent<UILabel>(),missionBase.reciveNPCID );  // to find receive npc
		}
		
		newLable.transform.parent = parentObj.transform; //attach to parentObj
		
		newLable.transform.localScale = cloneObj.transform.localScale;
		
		Vector3 newPos = newLable.transform.localPosition;
		
		newPos.y = startY;
		newLable.transform.localPosition = newPos;       //new pos begin from startY
		newLable.SetActive(true);						 //show new item
		startY -= newLable.transform.localScale.y;
		
		// 新手引导
		if ( 1001u == missionID || 1002u == missionID || 1003u == missionID )
		{
			Vector3 showPos = new Vector3(Mathf.Round(parentObj.transform.parent.transform.localPosition.x - 120), 
				Mathf.Round(parentObj.transform.parent.transform.localPosition.y), 
				Mathf.Round(parentObj.transform.parent.transform.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_Navi_Pos,
				1, showPos, parentObj.transform.parent.parent.gameObject);
		}
		
		return startY;									 //return new y
	}
	
	private void addItemList(uint missionID)
	{
		//dragItem
		XCfgMission missionBase = XCfgMissionMgr.SP.GetConfig( missionID );
		bool isActionMission = XMissionManager.SP.isActionMission(missionID );
		GameObject dragItem = GameObject.Instantiate(m_missioniItem,m_missioniItem.transform.position,m_missioniItem.transform.rotation) as GameObject;
		dragItem.SetActive(true);
		m_ItemList.Add(missionID,dragItem); //add to list manage
		dragItem.transform.parent = m_dragPanel.transform;//attach to the dragPanel
		dragItem.transform.localScale = m_missioniItem.transform.localScale;
		
		float posY = m_missioniItem.transform.localPosition.y;
		dragItem.transform.localPosition = new Vector3(dragItem.transform.localPosition.x,posY,dragItem.transform.localPosition.z);
		///////////////////////////////////////////////////////////////////////////
		
		//mission name
		bool bNeedRequirement = true;
		if(isActionMission)
		{
			MissionMessage mm = XMissionManager.SP.getActionMission(missionID);
			if( true == mm.m_bMissionCompleted )
			{
				bNeedRequirement = false;
			}
			if(mm.m_bMissionCompleted)bNeedRequirement = false;
		}else //not action can be receive
		{
			bNeedRequirement = false;
		}
		
		m_fStartY = makeMissionNameLable( m_missionName,
			XMissionManager.SP.makeFunctionMissionName(missionID) //colour and function string change
			,dragItem
			,0
			,missionID);   //make missionName start 0 
		
		
		if(false == bNeedRequirement) //completed only show the title of mission
		{
			return;
		}
		
		m_fStartY -= 2.0f;
		///////////////////////////////////////////////////////////////////////////
		if(0!=missionBase.referKillID1 )
		{
			string strShowTxt =XMissionManager.SP.makeShowRequirementString( missionID,eMissionRequirement.eMR_Kill1 );
				
			m_fStartY = makeLable(m_missionMessage,
				strShowTxt,
				dragItem,
				m_fStartY,
				missionID );
		}
			
		if(0!=missionBase.referKillID2 )
		{
			string strShowTxt = XMissionManager.SP.makeShowRequirementString( missionID,eMissionRequirement.eMR_Kill2);
				
			m_fStartY = makeLable(m_missionMessage,
			    strShowTxt,
				dragItem,
				m_fStartY,
				missionID);
		}
			
		if(0!=missionBase.referKillID3 )
		{
			string strShowTxt = XMissionManager.SP.makeShowRequirementString( missionID,eMissionRequirement.eMR_Kill3);
			m_fStartY = makeLable(m_missionMessage,
				strShowTxt,
				dragItem,
				m_fStartY,
				missionID);
		}
			
		if(0!=missionBase.referItemID1 )
		{
			string strShowTxt = XMissionManager.SP.makeShowRequirementString(missionID,eMissionRequirement.eMR_Item1);
			m_fStartY = makeLable(m_missionMessage,
				strShowTxt,
				dragItem,
				m_fStartY,
				missionID);
		}
			
		if(0!=missionBase.referItemID2 )
		{
			string strShowTxt = XMissionManager.SP.makeShowRequirementString(missionID,eMissionRequirement.eMR_Item2);
			m_fStartY = makeLable(m_missionMessage,
				strShowTxt,
				dragItem,
				m_fStartY,
				missionID);
		}
			
		if(0!=missionBase.referItemID3 )
		{
			string strShowTxt = XMissionManager.SP.makeShowRequirementString(missionID,eMissionRequirement.eMR_Item3);
			m_fStartY = makeLable(m_missionMessage,
				strShowTxt,
				dragItem,
				m_fStartY,
				missionID);
		}
		
		dragItem.GetComponent<UITable>().repositionNow = true;
	}
	
	private void updateCurrentState()
	{
		if(m_bActionState)
		{
			actionMissionListUpdate();
		}else
		{
			canReceiveMissionListUpdate();
		}
		
	}
	
	private void showActionMissionListCallBack(GameObject _go)
	{
		m_bActionState = true;
		updateCurrentState();
	}
	
	private void showCanReceiveMissionListCallBack(GameObject _go)
	{
		m_bActionState = false;
		updateCurrentState();
	}
	
	private void showMissionDialog(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle,EUIPanel.eMissionDialog);
	}
	
	private void autoCheckShow()
	{
		if(0 < XMissionManager.SP.getActionMissionList().Count )
		{
			manualActionMissionShow();
		}else
		{
			manualCanReceiveMissionShow();
		}
		
	}
	
	private void autoCehckCurrentStateShow()
	{
		//this two case need turn state
		if(m_bActionState && 0>= XMissionManager.SP.getActionMissionList().Count )
		{
			m_bActionState = false;
		}else if(!m_bActionState && 0>= XMissionManager.SP.getCanReceiveMissionList().Count )
		{
			m_bActionState = true;
		}
		
		updateCurrentState();
	}
	
	private void manualActionMissionShow()
	{
		m_bActionState = true;
		
		m_pageGroup.Select(0);
		
		updateCurrentState();
	}
	
	private void manualCanReceiveMissionShow()
	{
		m_bActionState = false;
		
		m_pageGroup.Select(1);
		
		updateCurrentState();
	}
	
}
