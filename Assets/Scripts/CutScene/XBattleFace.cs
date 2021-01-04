using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base;

[USequencerEvent("37Game/Battle Face")]
[USequencerFriendlyName("Battle Face")]
public class XBattleFace : USEventBase {
	
	private XU3dEffect m_FaceEffect = null;
	enum FaceEffect{
		efe_FashiMan = 900742,
		efe_FashiWoman = 900743,
		efe_GonJianShouMan = 900744,
		efe_GonJianShouWoman = 900745,
		efe_ZhanShiMan = 900746,
		efe_ZhanShiWoman = 900747,
	}
	
	public EBattlePepoleType m_battlePepoleType = EBattlePepoleType.eBattleDriver;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public override void FireEvent()
	{
		if(XGame.Client.Packets.BATTLE_TYPE.BATTLE_TYPE_NONE == XBattleManager.SP.BattleType )
		{
			return;
		}
		
		FaceEffect effectID = FaceEffect.efe_ZhanShiMan;
		
		XBattlePosition battlePos = null;
		switch(m_battlePepoleType )
		{
		case EBattlePepoleType.eBattleDriver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.AttackBattlePos;
			break;
		case EBattlePepoleType.eBattlePassiver:
			battlePos = XCutSceneMgr.SP.m_curBattleAction.MainTargetPos;
			break;
		}
		
		if( null == battlePos)
			return;
		
		XBattleObject player = BattleDisplayerMgr.SP.m_BattleObjects[(int)(battlePos.Group) ,(int)(battlePos.Position)];
		
		switch((EShareClass)player.Class)
		{
		case EShareClass.eshClass_ZhanShi:
			
			if( EShareSex.eshSex_Male == (EShareSex)player.Sex )
			{
				effectID = FaceEffect.efe_ZhanShiMan;
			}else
			{
				effectID = FaceEffect.efe_ZhanShiWoman;
			}
			break;
		case EShareClass.eshClass_FaShi:
			if( EShareSex.eshSex_Male == (EShareSex)player.Sex )
			{
				effectID = FaceEffect.efe_FashiMan;
			}else
			{
				effectID = FaceEffect.efe_FashiWoman;
			}
			break;
		case EShareClass.eshClass_GongJianShou:
			if( EShareSex.eshSex_Male == (EShareSex)player.Sex )
			{
				effectID = FaceEffect.efe_GonJianShouMan;
			}else
			{
				effectID = FaceEffect.efe_GonJianShouWoman;
			}
			break;
		}
		uint ID = (uint)effectID;
		m_FaceEffect = new XU3dEffect((uint)effectID ,OnEffectLoaded );
		
		m_FaceEffect.m_gameObject.transform.parent = XCutSceneMgr.SP.battleScene.transform;
		
		m_FaceEffect.m_gameObject.transform.position = Vector3.zero;
	}
	
	private void OnEffectLoaded(XU3dEffect self)
	{
		
		Transform[] transList = self.m_gameObject.GetComponentsInChildren<Transform>(true);
		
		foreach(Transform t in transList)
		{
			t.gameObject.layer = GlobalU3dDefine.Layer_UI_2D;
		}
		
	
	}
	
	public override void ProcessEvent( float deltaTime )
	{
		
	}
	
}
