using UnityEngine;
using System.Collections;

// 读条提示, 效果参照生产系统采集功能
[AddComponentMenu("UILogic/XReadTip")]
public class XReadTip : XUIBaseLogic
{
	public UILabel Label_Discription;
	public UILabel Label_Progress;
	public UISlider Slider_Progress;
	
	public void SetDiscription(string str)
	{
		Label_Discription.text = str;
	}
	
	public void SetProgress(string str)
	{
		Label_Progress.text = str;
	}
	
	public void SetProgress(float now, float max)
	{
		if(max <=0) max = 0.0001f;
		if(now > max) now = max;
		Slider_Progress.sliderValue = now / max;
	}
}

