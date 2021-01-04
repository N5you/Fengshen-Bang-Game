using UnityEngine;
using System.Collections;
using System.IO;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using XGame.Client.Network;
using XGame.Client.Base.Map;
using System.Collections.Generic;
using resource;


public class XCutSceneMgr : XSingleton<XCutSceneMgr>
{
	public delegate void finishCutSceneCallType();
	
	public bool IsPlayEnd = false;
	
	//when the cut scene over will call
	private finishCutSceneCallType m_finishCutSceneCall = null;
	
	public bool isStarted = false;
	
	private GameObject m_mainPlayer = null;
	
	private XResourceScene m_battleResourceScene = null;
	
	private GameObject m_battleSceneObj = null;
	
	public BattleAction m_curBattleAction = null;
	
	private XBattleEnd m_battleEnd = null;
	
	public List<XBattlePosition>	m_BatTargetPosList = new List<XBattlePosition>();
	
	public void init()
	{
		XEventManager.SP.AddHandler(battleAnimationEndListen,EEvent.Show_Fight_Result_Direct );
		XEventManager.SP.AddHandler(battleAnimationEndListen,EEvent.Show_Fight_Replay );
	}
	
	private void battleAnimationEndListen(EEvent evt, params object[] args)
	{
		if( null == m_battleEnd)
			return;
		
		m_battleEnd.actionBattleAnimationEnd();
	}
	
	public void resumeCamera()
	{
		for( int i=0;i<Camera.allCameras.Length;i++ )
		{
			if("MainCamera" == Camera.allCameras[i].gameObject.tag)
			{
				if(Camera.allCameras[i].gameObject.activeSelf)
				{
					Camera.allCameras[i].gameObject.SetActive(false);
				}
			}
		}
		LogicApp.SP.MainCamera.gameObject.SetActive(true);
	}
	
	public XBattleEnd BattleEnd{
		set{ m_battleEnd = value; }
	}
	
	public GameObject battleScene{
		set{
			if(null == value)
			{
				if(null!=m_battleSceneObj )
				{
					GameObject.Destroy(m_battleSceneObj);
				}
					//nothing
			}else
			{
				m_battleSceneObj = value;
			}
		}
		get{
			return m_battleSceneObj;
		}
	}
	
	public GameObject mainPlayer{
		get{ return m_mainPlayer; }
		set{m_mainPlayer = value;}
	}
	
	public finishCutSceneCallType finishCutSceneCall{
		set{ m_finishCutSceneCall = value; }
	}
	
	public void start()
	{
		isStarted  = true;
	}
	
	public void playCutScene(uint cutSceneID,finishCutSceneCallType callBackFinish)
	{
		IsPlayEnd	= false;
		m_finishCutSceneCall = callBackFinish;
		XLogicWorld.SP.LoadScene((int)cutSceneID,ESceneType.CutScene );
	}
	
	public void playInitCutScene(uint cutSceneID,finishCutSceneCallType callBackFinish )
	{
		IsPlayEnd	= false;
		m_finishCutSceneCall = callBackFinish;
		XLogicWorld.SP.LoadScene((int)cutSceneID,ESceneType.NormalCutScene );
	}
	
	public void finishCutScene()
	{
		IsPlayEnd	= true;
		XLogicWorld.SP.MainPlayer.Visible = true;
		mainPlayer = null;
		resumeCamera();
		
		if(null!=m_finishCutSceneCall)
			m_finishCutSceneCall();
	}
	
	public void addBattleCutScene(int nSceneId )
	{
		isStarted = true;
		
		m_battleResourceScene = XResourceManager.GetResource(XResourceScene.ResTypeName,(uint)nSceneId) as XResourceScene;
		
		if(m_battleResourceScene.IsLoadDone() )
		{
			onBattleCutSceneLoaded(null);
		}
		else
		{
			if(m_battleResourceScene != null)
				m_battleResourceScene.ResLoadEvent	-= onBattleCutSceneLoaded;
			XResourceManager.StartLoadResource(XResourceScene.ResTypeName,(uint)nSceneId);
			m_battleResourceScene.ResLoadEvent += new XResourceBase.LoadCompletedDelegate(onBattleCutSceneLoaded );
		}
		
	}
	
	private void onBattleCutSceneLoaded(DownloadItem item )
	{
		#if RES_DEBUG
			string sceneName = Path.GetFileNameWithoutExtension(m_battleResourceScene.SceneName);
			
			Application.LoadLevelAdditiveAsync(sceneName );
		#else
			string sceneName = Path.GetFileNameWithoutExtension(m_battleResourceScene.SceneName);
			Application.LoadLevelAdditiveAsync(sceneName );
		#endif
		
		XEventManager.SP.SendEvent(EEvent.Fight_Anim_Start );
	}
	
	public void endBattleCutScene()
	{
		XCutSceneMgr.SP.m_curBattleAction.ActionEnd();
		XCutSceneMgr.SP.m_curBattleAction = null;
		XEventManager.SP.SendEvent(EEvent.Fight_Anim_End );
		
	}
}

