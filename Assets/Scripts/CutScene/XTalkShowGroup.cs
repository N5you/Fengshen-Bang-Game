using UnityEngine;
using System.Collections;

public enum CutSceneWordType
{
	LeftSay  		=0,
	RightSay		=1,
	TopSay			=2,
}

[System.Serializable]
public class talkPark{
	public CutSceneWordType m_type = CutSceneWordType.LeftSay;
	public int m_uiModelID=0;											//0 is mainPlayer
	public string m_strModelName="";
	public string m_strWord="";
}

[USequencerEvent("37Game/TalkShowGroup")]
[USequencerFriendlyName("talk show group")]
public class XTalkShowGroup : USEventBase {
	
	public talkPark[] m_talkList = new talkPark[10];
	
	public USSequencer m_nextSequence = null;
	
	private uint m_uiCurrentIndex = 0;
	
	private bool m_bFinished = false;
	
	public override void FireEvent()
	{
		m_bFinished = false;
		XEventManager.SP.AddHandler(nextEvent,EEvent.CutScene_NextTalk);
		playNext();
	}
	
	public override void ProcessEvent( float deltaTime )
	{
		int a =0;
		a = 0;
	}
	
	private void nextEvent(EEvent evt, params object[] args )
	{
		if( true == m_bFinished)
			return;
		
		playNext();
	}
	
	public void playNext()
	{
		//in editor
		if(!XCutSceneMgr.SP.isStarted)
		{
			playEnd();
			return;
		}
		
		if(true == m_bFinished )
		{
			return;
		}
		
		if( m_uiCurrentIndex >= m_talkList.Length) //then end
		{
			playEnd();
			return;
		}
		
		talkPark currentTalkPark = m_talkList[m_uiCurrentIndex];
		
		uint sayModelID = (uint)currentTalkPark.m_uiModelID;
		string strModelName = currentTalkPark.m_strModelName;
		if(0 == sayModelID )
		{
			sayModelID = XLogicWorld.SP.MainPlayer.ModelId;
			strModelName = XLogicWorld.SP.MainPlayer.Name;
		}
		
		switch(currentTalkPark.m_type )
		{
		case CutSceneWordType.LeftSay:
			XEventManager.SP.SendEvent(EEvent.CutScene_LeftSay,sayModelID,strModelName,currentTalkPark.m_strWord);
			break;
		case CutSceneWordType.RightSay:
			XEventManager.SP.SendEvent(EEvent.CutScene_RightSay,sayModelID,strModelName,currentTalkPark.m_strWord);
			break;
		case CutSceneWordType.TopSay:
			XEventManager.SP.SendEvent(EEvent.CutScene_TopSay,currentTalkPark.m_strWord);
			break;
		}
		
		m_uiCurrentIndex++;
	}
	
	private void playEnd()
	{
		m_bFinished = true;
		
		if(null == m_nextSequence)
		{
			if(XCutSceneMgr.SP.isStarted)
			{
				LogicApp.SP.UICamera.gameObject.SetActive(true);
			}	
			
			XCutSceneMgr.SP.finishCutScene();
			
		}else
		{
			m_nextSequence.RunningTime = 0.0f;
			m_nextSequence.Play();
		}
		
	}
	
}
