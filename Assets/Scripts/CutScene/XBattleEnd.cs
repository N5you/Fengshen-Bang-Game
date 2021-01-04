using UnityEngine;
using System.Collections;

[USequencerEvent("37Game/Battle End")]
[USequencerFriendlyName("Battle End")]
public class XBattleEnd : USEventBase {
	
	public GameObject m_CutSceneObj = null;
	
	public override void FireEvent()
	{
		if(null == XCutSceneMgr.SP.m_curBattleAction )
			return;
		
		actionBattleAnimationEnd();
	}
	
	private void battleAnimationEnd()
	{
		if(null != m_CutSceneObj)
		{
			m_CutSceneObj.SetActive(false);
			GameObject.Destroy(m_CutSceneObj );
		}
		
		XCutSceneMgr.SP.endBattleCutScene();
	}
	
	public override void ProcessEvent( float deltaTime )
	{
		
	}
	
	public void actionBattleAnimationEnd()
	{
		this.Timeline.Sequence.Stop();
		
		//in the battle animation cut scene in side event
		XEventManager.SP.SendEvent(EEvent.CutScene_BattleAnimationEnd );
		
		XEventManager.SP.ClearHandler(EEvent.CutScene_BattleAnimationEnd );
		
		battleAnimationEnd();
	}
	
	// Use this for initialization
	void Start () {
		
		if(!XCutSceneMgr.SP.isStarted )
			return;
		
		XCutSceneMgr.SP.BattleEnd = this;
		
		if( null == m_CutSceneObj )
			return;
		
		if( null == XLogicWorld.SP.SceneManager )
			return;
		
		m_CutSceneObj.transform.position = XLogicWorld.SP.SceneManager.GetCenterPos();
		
		XCutSceneMgr.SP.battleScene = m_CutSceneObj;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
}
