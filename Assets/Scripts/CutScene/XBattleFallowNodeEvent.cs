using UnityEngine;
using System.Collections;

[USequencerFriendlyName("Battle Fallow Node") ]
[USequencerEvent("37Game/BattleFallowNodeEvent") ]

public enum EBattlePepoleType
{
	eBattleDriver,
	eBattlePassiver,
	eBattleCam,
}

public class XBattleFallowNodeEvent : USEventBase {
 	private GameObject objectToMatch = null;
	
	public AnimationCurve inCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
	
	public EBattlePepoleType m_battlePepoleType = EBattlePepoleType.eBattleDriver;
	
	private Quaternion sourceRotation = Quaternion.identity;
	
	private Vector3 sourcePosition = Vector3.zero;
	
	private Vector3 destinationPosition = Vector3.zero;
	
	private Quaternion destinationRotation = Quaternion.identity;
	
	public bool m_bUseRotation = true;
	
	private bool m_bInited = false;
	
    public override void FireEvent()
    {
		attachObjByBattle();
		
		if(!objectToMatch)
		{
			Debug.LogWarning("The XBattleFallowNodeEvent event does not provice a object to match", this);
			return;
		}
		
		sourceRotation = AffectedObject.transform.rotation;
		sourcePosition = AffectedObject.transform.position;
		
		m_bInited = true;
    }

    public override void ProcessEvent(float deltaTime)
    {
		if(!objectToMatch)
		{
			Debug.LogWarning("The XBattleFallowNodeEvent event does not provice a object to look at", this);
			return;
		}
		
		float ratio = 1.0f;
		ratio = Mathf.Clamp(inCurve.Evaluate(deltaTime), 0.0f, 1.0f);
		
		checkTransform();
		
		if(m_bUseRotation)
		{
			AffectedObject.transform.rotation = Quaternion.Slerp(sourceRotation, destinationRotation, ratio);
		}
		
		AffectedObject.transform.position = Vector3.Slerp(sourcePosition, destinationPosition, ratio);
    }
	
	private void attachObjByBattle()
	{
		//not in the battle
		if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_NONE == XBattleManager.SP.BattleType )
			return;
		
		XBattlePosition battlePos = null;
		XBattleObject curPlayer;
		
		switch(m_battlePepoleType )
		{
		case EBattlePepoleType.eBattleDriver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.AttackBattlePos;
			curPlayer = BattleDisplayerMgr.SP.m_BattleObjects[(int)(battlePos.Group) ,(int)(battlePos.Position)];
			objectToMatch = curPlayer.ObjectModel.mainModel.m_gameObject;
			break;
		case EBattlePepoleType.eBattlePassiver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.MainTargetPos;
			curPlayer = BattleDisplayerMgr.SP.m_BattleObjects[(int)(battlePos.Group) ,(int)(battlePos.Position)];
			objectToMatch = curPlayer.ObjectModel.mainModel.m_gameObject;
			break;
		case EBattlePepoleType.eBattleCam:
			objectToMatch = LogicApp.SP.MainCamera.gameObject;
			break;
		}
		
	}
	
	private void checkTransform()
	{
		XBattlePosition battlePos = null;
		switch(m_battlePepoleType)
		{
		case EBattlePepoleType.eBattleDriver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.AttackBattlePos;
			destinationPosition = BattleDisplayerMgr.GetFighterPos(battlePos.Group,(int)battlePos.Position );
			destinationRotation = Quaternion.Euler(BattleDisplayerMgr.GetFighterDir(battlePos.Group) );
			break;
		case EBattlePepoleType.eBattlePassiver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.MainTargetPos;
			destinationPosition = BattleDisplayerMgr.GetFighterPos(battlePos.Group,(int)battlePos.Position );
			destinationRotation = Quaternion.Euler(BattleDisplayerMgr.GetFighterDir(battlePos.Group) );
			break;
		case EBattlePepoleType.eBattleCam:
			destinationPosition = LogicApp.SP.MainCamera.transform.position;
			destinationRotation = LogicApp.SP.MainCamera.transform.rotation;
			break;
		}
		
		//destinationPosition = objectToMatch.transform.position;
		//destinationRotation = objectToMatch.transform.rotation;
	}
	
	public override void StopEvent()
	{
		UndoEvent();
	}
	
	public override void UndoEvent()
	{
		if(!AffectedObject)
			return;
		
		if(!m_bInited )
			return;
		
		AffectedObject.transform.rotation = sourceRotation;
		AffectedObject.transform.position = sourcePosition;
	}
}
