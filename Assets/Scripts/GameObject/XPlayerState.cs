using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;

public abstract class XPlayerStateBase : XState<XPlayer>
{
	public XPlayerStateBase(EStateId id, XPlayer owner)
		: base((int)id, owner)
	{
	}
	
	public abstract void OnAppear();
}

//暂时直接集成XCharStateSit
//后面考虑直接全面写自己的状态机
public class XPlayerStateSit : XCharStateSit
{
	private XU3dEffect effect = null;
	private XU3dEffect effectMeditationLive = null;

	public XPlayerStateSit(XPlayer owner)
		: base(owner)
	{
		
	}
	
	public override void Enter(params object[] args)
	{
		m_Owner._playAnimation(EAnimName.Sit, 1f, false);
		
		effect = new XU3dEffect(900420);
		effectMeditationLive = new XU3dEffect(900061);
		
		effect.Position = m_Owner.Position;
		//effectMeditationLive.Parent = XLogicWorld.SP.MainPlayer.ObjectModel.mainModel.GetSkeleton(ESkeleton.eCapsuleTop).transform;
		effectMeditationLive.Position = m_Owner.Position + new Vector3(0f,2f,0);//XLogicWorld.SP.MainPlayer.ObjectModel.mainModel.GetSkeleton(ESkeleton.eCapsuleTop).position + new Vector3(0.0f,-2.0f,0.0f);
	}
	
	public override bool OnEvent(int evt, params object[] args)
	{
		EStateEvent ESE =(EStateEvent)evt;
		
		switch(ESE)
		{
			case(EStateEvent.esMoveTo):
				Machine.TranslateToState((int)EStateId.esMove, args);
				return true;
				
			case(EStateEvent.esJump):
				Machine.TranslateToState((int)EStateId.esMove, args);
				return true;
				
			case(EStateEvent.esCancelSit):
				Machine.TranslateToState((int)EStateId.esIdle);
				return true;
		}
		return false;
	}
	
	public override void OnAppear()
	{
		m_Owner._playAnimation(EAnimName.Sit, 1f, false);
	}

	public override void Exit()
	{
		destroyEffect();
	}

	private void destroyEffect()
	{
		if(null != effect)
		{
			effect.Destroy();
			effectMeditationLive.Destroy();
			effect = null;
			effectMeditationLive = null;
		}		
	}
}
