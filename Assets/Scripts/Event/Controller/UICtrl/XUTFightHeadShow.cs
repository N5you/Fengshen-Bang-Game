using System;
using UnityEngine;

class XUTFightHeadShow : XUICtrlTemplate<XFightHeadShow>
{
	private uint m_uiSoulValue=0;
	
	public XUTFightHeadShow()
	{
		//XEventManager.SP.AddHandler(SetLBloodValue,EEvent.FightHead_Show_LBlood_Value);
		//XEventManager.SP.AddHandler(SetRBloodValue,EEvent.FightHead_Show_RBlood_Value);
		//XEventManager.SP.AddHandler(SetBattleValue,EEvent.FightHead_Show_Battle_Value);
		RegEventAgent_CheckCreated(EEvent.FightHead_Show_LBlood_Value,SetLBloodValue);
		RegEventAgent_CheckCreated(EEvent.FightHead_Show_RBlood_Value,SetRBloodValue);
		RegEventAgent_CheckCreated(EEvent.FightHead_Show_Battle_Value,SetBattleValue);
		RegEventAgent_CheckCreated(EEvent.FightHead_Show_Soul_Value,SetSoulValue);
		RegEventAgent_CheckCreated(EEvent.FightHead_Show_Soul_Fire,FireSoulValue);
		RegEventAgent_CheckCreated(EEvent.FightHead_Show_Soul_Explosion,ExplosionSoulValue);
		RegEventAgent_CheckCreated(EEvent.FightHead_Show_Soul_Empty,EmptySoulValue);
		
		RegEventAgent_CheckCreated(EEvent.Fight_Anim_Start,FightCutSceneStart );
		RegEventAgent_CheckCreated(EEvent.Fight_Anim_End,FightCutSceneEnd );
		
	}
	
	public override bool Show()
	{
		SetSoulValueReal();
		return base.Show();
	}
	
	public void SetLBloodValue(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		
		float leftValue = (float)(args[0]);
		
		LogicUI.LeftSlider.sliderValue = leftValue;
	}
	
	public void SetRBloodValue(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		
		float rightValue = (float)(args[0]);
		
		LogicUI.RightSlider.sliderValue	= rightValue;
	}
	
	public void SetSoulValue(EEvent evt, params object[] args )
	{
		m_uiSoulValue = (uint)args[0];
		
		SetSoulValueReal();
	}
	
	private void SetSoulValueReal()
	{
		if(LogicUI == null)
			return;
		
		LogicUI.SoulSlider.sliderValue	= (float)m_uiSoulValue/12.0f;
	}
	
	private void FightCutSceneStart(EEvent evt, params object[] args )
	{
		if(null == LogicUI )
			return;
		
		LogicUI.SetBattleCutState(true );
	}
	
	private void FightCutSceneEnd(EEvent evt, params object[] args )
	{
		if(null == LogicUI )
			return;
		
		LogicUI.SetBattleCutState(false );
	}
	
	private void EmptySoulValue(EEvent evt, params object[] args )
	{
		GameObject emptyEffect = GameObject.Instantiate(LogicUI.m_EmptyDemo,
			LogicUI.SoulSlider.transform.position,
			LogicUI.SoulSlider.transform.rotation ) as GameObject;
		
		emptyEffect.SetActive(true);
		
		LogicUI.SoulSlider.sliderValue	= 0.0f;
	}
	
	private void ExplosionSoulValue(EEvent evt, params object[] args )
	{
		GameObject explosionEffect = GameObject.Instantiate(LogicUI.m_ExplosionDemo,
			LogicUI.SoulSlider.transform.position,
			LogicUI.SoulSlider.transform.rotation ) as GameObject;
		
		explosionEffect.SetActive(true);
		LogicUI.SoulSlider.sliderValue	= ((uint)args[1])/12.0f;	//show soul at explosion
		
		//need play full effect
		if(12<=(uint)args[1] )
		{
			GameObject fullEffect = GameObject.Instantiate(LogicUI.m_SoulFullDemo,
			LogicUI.SoulSlider.transform.position,
			LogicUI.SoulSlider.transform.rotation ) as GameObject;
			
			fullEffect.SetActive(true);
		}
		
	}
	
	private void FireSoulValue(EEvent evt, params object[] args )
	{
		Camera ui2dCamera = UICamera.FindCameraForLayer(GlobalU3dDefine.Layer_UI_2D).gameObject.GetComponent<Camera>();
		
		Camera currentCam = null;
		foreach(Camera cam in Camera.allCameras)
		{
			//not ui camera but is active
			if(0 != (cam.cullingMask & (1 << GlobalU3dDefine.Layer_GameObject ) )||
				0 != (cam.cullingMask & (1 << GlobalU3dDefine.Layer_BattleObject ) )
				)
			{
				currentCam = cam;
			}
		}
		Vector3 cameraUIPos = currentCam.WorldToScreenPoint((Vector3)args[0] );
		cameraUIPos.x -= ui2dCamera.pixelWidth/2.0f;
		cameraUIPos.y -= ui2dCamera.pixelHeight/2.0f;
		cameraUIPos.z = -30.0f;
		
		cameraUIPos = ui2dCamera.transform.localToWorldMatrix.MultiplyPoint(cameraUIPos);
		
		GameObject objEffect = GameObject.Instantiate(LogicUI.m_SoulEffectDemo,
			cameraUIPos ,
			Quaternion.identity ) as GameObject ;
		
		FlyEffect flyEffect = objEffect.GetComponent<FlyEffect>();
		Vector3 v3TargetPos = LogicUI.SoulSlider.gameObject.transform.position;
		v3TargetPos.z -= 20.0f;     //must above the ui target so -20
		flyEffect.fireEffect( LogicUI.SoulSlider.gameObject.transform.position,(uint)args[1] );
		
		objEffect.SetActive(true );
	}
	
	public void SetBattleValue(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return;
		
		int leftValue = (int)(args[0]);
		int rightValue = (int)(args[1]);
		
		LogicUI.LeftBattleValue.text	= XStringManager.SP.GetString(31) + Convert.ToString(leftValue);
		LogicUI.RightBattleValue.text	= XStringManager.SP.GetString(31) + Convert.ToString(rightValue);
	}
}
