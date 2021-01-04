using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XFirstLogin")]
public class XFirstLogin : XUIBaseLogic
{
	public UILabel 	LabelDiscription;
	public UILabel	LabelProgress;
	
	public UISlider SliderLoadProgressTop = null;
	public UISlider SliderLoadProgressBottom = null;
	
	public void SetDiscription(string str)
	{
		LabelDiscription.text = str;
	}
	
	public void SetProgress(float progress)
	{
		LabelProgress.text = "" + progress;
		if(progress > 1.0f) progress = 1f;
		if(progress < 0f) progress = 0f;
		SliderLoadProgressTop.sliderValue = progress;
		SliderLoadProgressBottom.sliderValue = progress;
	}
}

