using UnityEngine;
using System.Collections;

[USequencerEvent("37Game/Change Battle Player")]
[USequencerFriendlyName("Change Battle Player")]
public class XBattleChangePlayer : USEventBase {
	public bool m_bMainPlayerShow = true;
	
	private XBattleObject m_curPlayer = null;
	
	public EBattlePepoleType m_battlePepoleType = EBattlePepoleType.eBattleDriver;
	
	private Vector3 m_v3SourcePos = Vector3.one;
	
	private Quaternion m_qtSourceRot = Quaternion.identity;
	
	private bool m_bInit = false;
	
	private Animation m_animation = null;
	
	public override void FireEvent()
	{
		//not in the battle
		if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_NONE== XBattleManager.SP.BattleType )
		{
			TimelineContainer.AffectedObject.gameObject.SetActive(m_bMainPlayerShow );
			return;
		}
		
		XBattlePosition battlePos = null;
		switch(m_battlePepoleType )
		{
		case EBattlePepoleType.eBattleDriver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.AttackBattlePos;
			m_v3SourcePos = BattleDisplayerMgr.GetFighterPos(battlePos.Group,(int)battlePos.Position );
			m_qtSourceRot = Quaternion.Euler(BattleDisplayerMgr.GetFighterDir(battlePos.Group) );
			break;
		case EBattlePepoleType.eBattlePassiver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.MainTargetPos;
			m_v3SourcePos = BattleDisplayerMgr.GetFighterPos(battlePos.Group,(int)battlePos.Position );
			m_qtSourceRot = Quaternion.Euler(BattleDisplayerMgr.GetFighterDir(battlePos.Group) );
			break;
		}
		
		if( null != battlePos)
		{
			XCutSceneMgr.SP.m_BatTargetPosList.Add(battlePos);
		}
		
		m_curPlayer = BattleDisplayerMgr.SP.m_BattleObjects[(int)(battlePos.Group) ,(int)(battlePos.Position)];
		
		Animation[] animations = m_curPlayer.ObjectModel.mainModel.m_gameObject.GetComponentsInChildren<Animation>(true);
		
		GameObject gameMainPlayer = animations[0].gameObject;
		
		m_animation = animations[0];
		
		this.TimelineContainer.AffectedObject.gameObject.SetActive(false);
		
		this.TimelineContainer.AffectedObject = gameMainPlayer.transform;
		
		m_curPlayer.SendModelEvent(EModelEvent.evtSetHeadVisiable,false);
		
		gameMainPlayer.SetActive(m_bMainPlayerShow );
		gameMainPlayer.transform.position = m_v3SourcePos;
		gameMainPlayer.transform.rotation = m_qtSourceRot;
		
		m_bInit = true;
	}
	
	private void resumePlayer()
	{
		if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_NONE == XBattleManager.SP.BattleType )
			return;
		
		if(false == m_bInit )
			return;
		
		Vector3 resumePos = Vector3.one;
		Quaternion resumeQua = Quaternion.identity ;
		
		XBattlePosition battlePos = null;
		switch(m_battlePepoleType)
		{
		case EBattlePepoleType.eBattleDriver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.AttackBattlePos;
			resumePos = BattleDisplayerMgr.GetFighterPos(battlePos.Group,(int)battlePos.Position );
			resumeQua = Quaternion.Euler(BattleDisplayerMgr.GetFighterDir(battlePos.Group) );
			break;
		case EBattlePepoleType.eBattlePassiver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.MainTargetPos;
			resumePos = BattleDisplayerMgr.GetFighterPos(battlePos.Group,(int)battlePos.Position );
			resumeQua = Quaternion.Euler(BattleDisplayerMgr.GetFighterDir(battlePos.Group) );
			break;
		}
		
		AffectedObject.transform.position = resumePos;
		AffectedObject.transform.rotation = resumeQua;
		
		Vector3 pos = AffectedObject.transform.localPosition;
		
		AnimationState state = m_animation["Fight"];
		if(!state)
			return;
		m_animation.CrossFade("Fight");
		state.speed = 1.0f;
		m_animation.wrapMode = WrapMode.Default;
		
		
		if(null!=m_curPlayer)
		{
			m_curPlayer.SendModelEvent(EModelEvent.evtSetHeadVisiable,true );
		}
	}
	
	public override void ProcessEvent( float deltaTime )
	{
		
	}
	
	private void cameraResumeEndListen(EEvent evt, params object[] args)
	{
		resumePlayer();
	}
	
	// Use this for initialization
	void Start () {
		XEventManager.SP.AddHandler(cameraResumeEndListen,EEvent.CutScene_BattleAnimationEnd );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
