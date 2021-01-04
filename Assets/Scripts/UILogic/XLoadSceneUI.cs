using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XLoadSceneUI")]
public class XLoadSceneUI : XUIBaseLogic
{
	public UISlider SliderLoadProgress = null;
//	public GameObject xiaorenPos;
//	private XU3dEffect xiaorenEffect;
//	public static uint XiaoRenEffectID = 900024;
	public UILabel	LabelProgress;
//	
	public override bool Init()
	{
		base.Init();
//		xiaorenEffect 					= new XU3dEffect(XiaoRenEffectID);
//		xiaorenEffect.Layer 			= GlobalU3dDefine.Layer_UI_2D;
//		xiaorenEffect.Parent			= xiaorenPos.transform;
//		xiaorenEffect.LocalPosition 	= Vector3.back * 3f;
		return true;
	}
	
	public void SetDiscription(string str)
	{
		//LabelDiscription.text = str;
	}
	
	public void SetProgress(float progress)
	{
		LabelProgress.text = "" + (progress * 100).ToString("N1") + "%";
		if(progress > 1.0f) progress = 1f;
		if(progress < 0f) progress = 0f;
		SliderLoadProgress.sliderValue = progress;
	}
}

