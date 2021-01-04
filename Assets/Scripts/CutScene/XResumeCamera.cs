using UnityEngine;
using System.Collections;

[USequencerFriendlyName("resume camera") ]
[USequencerEvent("37Game/resume game camera") ]
public class XResumeCamera : USEventBase {
	
	public override void FireEvent()
	{
		//not in the battle
		if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_NONE == XBattleManager.SP.BattleType )
			return;
		
		resumeCamera();
	}
	
	private void resumeCamera()
	{
		XCutSceneMgr.SP.resumeCamera();
	}
	
	private void cameraResumeEndListen(EEvent evt, params object[] args)
	{
		resumeCamera();
	}
	
	public override void ProcessEvent(float deltaTime)
	{
		
	}
	
	// Use this for initialization
	void Start () {
		XEventManager.SP.AddHandler(cameraResumeEndListen,EEvent.CutScene_BattleAnimationEnd );
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
}
