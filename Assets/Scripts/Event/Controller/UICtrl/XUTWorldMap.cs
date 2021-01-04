using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

class XUTWorldMap : XUICtrlTemplate<XWorldMap>
{
	private static uint EffectID = 900024;
	private bool m_bMainPlayerEntered = false;
	private XU3dEffect Effect;
	private Vector3 CurPos;
	private float speed = 100.0f;
	
	public XUTWorldMap()
	{
		XEventManager.SP.AddHandler(OnMainPlayerEnterGame, EEvent.MainPlayer_EnterGame);
		XEventManager.SP.AddHandler(RequireEnterScene, EEvent.WorldMap_RequireLoadScene);
		XEventManager.SP.AddHandler(ReflashMapIcon, EEvent.Mission_ReferMission);		
	}
	
	private void ReflashMapIcon(EEvent evt, params object[] args)
	{
		if(LogicUI != null)
			LogicUI.ReflashIcon();
	}
	
	public override bool Show()
	{
		if(!m_bMainPlayerEntered 
			|| ESceneType.NormalScene != XLogicWorld.SP.SceneManager.LoadedSceneType)
		{
			return true;
		}
		return base.Show();
	}
	
	public override void OnCreated(object arg)
	{
		base.OnCreated(arg);
		SortedList<uint, XCfgWorldMap> list = XCfgWorldMapMgr.SP.ItemTable;
		foreach(XCfgWorldMap cfg in list.Values)
		{
			LogicUI.OnAddMapIcon(cfg.PosId, cfg.SceneId, cfg.CloseSprite,cfg.OpenSpriteNormal,cfg.OpenSpriteHover);
		}
	}
	
	public override void OnShow()
	{
		base.OnShow();
		
		XCfgWorldMap cfg = XCfgWorldMapMgr.SP.GetConfig((uint)XLogicWorld.SP.SceneManager.LoadedSceneId);
		if(cfg == null)
			return ;
		
		UIImageButton CurBtn = LogicUI.IconBtn[cfg.PosId - 1];
		
		//show xiao ren
		
		if(Effect == null)
			Effect = new XU3dEffect(EffectID);
		
		Effect.Layer	= GlobalU3dDefine.Layer_UI_2D;
		Effect.Parent	= CurBtn.gameObject.transform.parent;		
		Effect.LocalPosition	= new Vector3(CurBtn.gameObject.transform.localPosition.x,CurBtn.gameObject.transform.localPosition.y,CurBtn.gameObject.transform.localPosition.z - 1);
		CurPos			= Effect.LocalPosition;
	}
	
	public override void OnHide()
	{
		base.OnHide();
	}
	
	private void OnMainPlayerEnterGame(EEvent evt, params object[] args)
	{
		m_bMainPlayerEntered = true;	
	}
	
	private void RequireEnterScene(EEvent evt, params object[] args)
	{
		CoroutineManager.StartCoroutine(ReadyEnterScene((uint)args[0]));
	}
	
	private IEnumerator ReadyEnterScene(uint sceneID)
	{
		XCfgWorldMap cfg = XCfgWorldMapMgr.SP.GetConfig(sceneID);
		if(cfg == null)
			yield break;
		
		yield return new WaitForSeconds(0.5f);
		
		if(LogicUI == null)
			yield break;
		
		UIImageButton NextBtn = LogicUI.IconBtn[cfg.PosId - 1];
		if(NextBtn == null)
			yield break;
		
		Vector3 beginPos = CurPos;
		Vector3 nextPos = new Vector3(NextBtn.gameObject.transform.localPosition.x,NextBtn.gameObject.transform.localPosition.y,NextBtn.gameObject.transform.localPosition.z - 1);
		float dist = new Vector3(CurPos.x - nextPos.x, 0, CurPos.y - nextPos.y).magnitude;	
		float delayTime = dist / speed;
		
		float startTime	= Time.time;
		
		if(nextPos.x > beginPos.x)
		{
			Effect.Direction = new Vector3(0f,180.0f,0f);
		}
		else
		{
			Effect.Direction = new Vector3(0f,0.0f,0f);
		}
		
		while(Vector3.Distance(beginPos,nextPos) >= 0.01f)
		{
			float fracComplete = (Time.time - startTime) / delayTime;
			beginPos = Vector3.Slerp(CurPos, nextPos, fracComplete);
			Effect.LocalPosition	= beginPos;
			yield return 0;
		}			
		
		XLogicWorld.SP.SceneManager.RequireEnterScene(sceneID);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eWorldMap);
		
	}	
}

