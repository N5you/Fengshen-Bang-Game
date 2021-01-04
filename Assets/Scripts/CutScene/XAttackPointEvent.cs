using UnityEngine;
using System.Collections;

[USequencerEvent("37Game/Battle Attack Point")]
[USequencerFriendlyName("Attack Point")]
public class XAttackPointEvent : USEventBase {
	
	public override void FireEvent()
	{
		if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_NONE == XBattleManager.SP.BattleType )
			return;
		
		if(null == XCutSceneMgr.SP.m_curBattleAction )
			return;
		
		XCutSceneMgr.SP.m_curBattleAction.Action_DamageHandle(1,false );
		
	}
	
	public override void ProcessEvent(float deltaTime)
	{
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
