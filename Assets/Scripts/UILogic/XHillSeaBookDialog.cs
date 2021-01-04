using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Network;

[System.Serializable]
public class HillSeaBookUnitStruct{
		public UITexture m_BossView = null;
		
		public GameObject m_BossBtn = null;
		
		public GameObject m_KillImg = null;
		
		public GameObject m_root = null;
		
		public void setKilledShow()
		{
			m_KillImg.SetActive(true );
		}
		
		public void setCanNotKillShow()
		{
			m_KillImg.SetActive(false );
			m_BossBtn.SetActive(false );
		}
		
		public void hide()
		{
			m_root.SetActive(false );
		}
		
		public void show()
		{
			m_root.SetActive(true);
		}
		
}

[AddComponentMenu("UILogic/XHillSeaBookDialog")]
public class XHillSeaBookDialog : XDefaultFrame {
	
	public HillSeaBookUnitStruct[] m_BossViewGroup;
	
	public GameObject guideObj = null;
	
	public GameObject leftBtn = null;
	
	public GameObject rightBtn = null;
	
	private int m_iCurrentPage = 0;
	
	public override bool Init()
	{
		bool b = base.Init();
		
		XEventManager.SP.AddHandler(listenHillSeaBookUpdate,EEvent.HillSeaBook_Message );
		
		for(uint i=0;i<m_BossViewGroup.Length;i++ )
		{
			bossBtnCall btnCall = new bossBtnCall();
			btnCall.owner = this;
			btnCall.uiPageIndex = i;
			UIEventListener.Get(m_BossViewGroup[i].m_BossBtn ).onClick += btnCall.bossBtnCallBack;
		}
		
		UIEventListener.Get(leftBtn ).onClick += leftPage;
		UIEventListener.Get(rightBtn ).onClick += rightPage;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return b;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eHillSeaBook);
	}
	
	private void listenHillSeaBookUpdate(EEvent evt, params object[] args)
	{
		showCurrentPage();
	}
	
	public override void Show()
	{
		base.Show();
		showCurrentPage();
		
		Vector3 showPos = new Vector3(70, -230, Mathf.Round(guideObj.transform.localPosition.z));
		XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_StartSealHill, 2, showPos, guideObj);
	}
	
	private uint uiBossCountOnePage{
		get{
			return (uint) m_BossViewGroup.Length;
		}
	}
	
	private uint maxPage{
		get{
			uint uiCurrentMaxBossCount = (uint)Mathf.Min((int)XHillSeaBookManager.SP.bossTotalCount,(int)XHillSeaBookManager.m_uiMaxBossCount  );;
			
			int iMaxPage =(int) Mathf.Ceil((float)uiCurrentMaxBossCount/(float)uiBossCountOnePage);
			
			return (uint)(Mathf.Max( 1,iMaxPage ) - 1);
		}
	}
	
	private uint minPage{
		get{ return 0; }
	}
	
	 //start the boss id in the page
	private uint startCurrentBossID{
		get{
			return (uint)(m_iCurrentPage*uiBossCountOnePage );
		}
	}
	
	private uint endCurrentBossID{
		get{
			return startCurrentBossID+uiBossCountOnePage;
		}
	}
	
	private void rightPage(GameObject _go){
		m_iCurrentPage++;
		m_iCurrentPage = (int) Mathf.Max(minPage,Mathf.Min(m_iCurrentPage,maxPage) );
		showCurrentPage();
	}
	
	private void leftPage(GameObject _go){
		m_iCurrentPage--;
		m_iCurrentPage = (int) Mathf.Max(minPage,Mathf.Min(m_iCurrentPage,maxPage) );
		showCurrentPage();
	}
	
	private void showCurrentPage()
	{
		clearState();
		
		uint uiEndBossId = endCurrentBossID;   //end the boss id in the page but not contain it self
		
		uint uiStartBossId = startCurrentBossID;
		for(uint i = uiStartBossId;i < uiEndBossId;i++ )
		{
			uint uiCurrentID = i - uiStartBossId;
			XCfgHillSeaBook hillSeaBookBase = XCfgHillSeaBookMgr.SP.GetConfig(i);
			
			if(null == hillSeaBookBase )//no need show
			{
				m_BossViewGroup[uiCurrentID].hide();
				continue;
			}
			
			m_BossViewGroup[uiCurrentID].show();
			
			if( isCanBeKill((int)uiCurrentID) )
			{
				m_BossViewGroup[uiCurrentID].setKilledShow();
			}else
			{
				m_BossViewGroup[uiCurrentID].setCanNotKillShow();
			}
			
			XModelRTTMgr.SP.AddSingleModelWhole(hillSeaBookBase.ModelID,m_BossViewGroup[uiCurrentID].m_BossView );
		}
	}
	
	private uint getCurrentBossID(uint index )
	{
		if(minPage>index || maxPage < index )
			return 0;
		
		return startCurrentBossID+index;
	}
	
	private bool isCanBeKill(int index )
	{
		return XHillSeaBookManager.SP.bossIndex <= (uint)index;
	}
	
	private void clearState()
	{
		foreach( HillSeaBookUnitStruct ut in m_BossViewGroup )
		{
			ut.hide();
		}
	}
	
	class bossBtnCall{
		
		private uint m_uiPageIndex = 0;
		
		private XHillSeaBookDialog m_owner = null;
		
		public XHillSeaBookDialog owner{
			set{
				m_owner = value;
			}
		}
		
		public uint uiPageIndex{
			set{ m_uiPageIndex = value; }
		}
		
		public void bossBtnCallBack(GameObject _go)
		{
			uint bossID = m_owner.getCurrentBossID(m_uiPageIndex);
			uint uiKilledCount =  XHillSeaBookManager.SP.getBossKillCount(bossID);
			
			uint freeCount = XHillSeaBookManager.SP.freeKillCount;
			
			if( freeCount <= uiKilledCount )//need pay for buy
			{
				if(  XVipManager.SP.GetVipAttri(EVipConst.eVip_BuyShhjKillCount) >= (XHillSeaBookManager.SP.getBossKillCount(bossID) - freeCount )  )//not enought vip kill count
				{
					XEventManager.SP.SendEvent(EEvent.MessageBox,null,null,XStringManager.SP.GetString(723) );
				}else
				{
					UIEventListener.VoidDelegate funcOK = new UIEventListener.VoidDelegate (sendRequestKillBoss);
					XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,XStringManager.SP.GetString(703) );
				}
				
				
			}else//is free
			{
				sendRequestKillBoss(null);
			}
		}
		
		private void sendRequestKillBoss(GameObject go)
		{
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_StartSealHill);
				
			SC_Int.Builder builder = SC_Int.CreateBuilder();
			builder.Data = (int)m_owner.getCurrentBossID(m_uiPageIndex);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_HillSeaBook_StartBattle,builder.Build() );
			
			XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eHillSeaBook );
		}
		
	}
}
