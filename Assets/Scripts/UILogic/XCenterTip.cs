using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ECenterTipStyle
{
	Up = 0,
	Down,
	BigFont_Up,
	Count,
}

[AddComponentMenu("UILogic/XCenterTip")]
public class XCenterTip : XUIBaseLogic
{
	public UILabel[] Tips = new UILabel[(int)ECenterTipStyle.Count];
	Queue<GameObject>	CenterTipQueue = new Queue<GameObject>();
	private float LastTime;
	
	public void OnCenterTip(ECenterTipStyle style, string tipContent, float scale)
	{
		if(ECenterTipStyle.Count == style) return;
		UILabel label = XUtil.Instantiate<UILabel>(Tips[(int)style]);
		label.text = tipContent;
		label.transform.localScale	= Tips[(int)style].transform.localScale;
		//label.gameObject.GetComponent<NcCurveAnimation>().enabled = true;
		label.transform.localScale *= scale;
		label.gameObject.SetActive(false);
		CenterTipQueue.Enqueue(label.gameObject);
	}
	
	void Update() 
	{		
		if(CenterTipQueue.Count == 0)
			return ;
		
		float deltaTime = Time.time - LastTime;
		if(deltaTime < 0.42f)
			return ;
		
		GameObject go = CenterTipQueue.Peek();
		go.SetActive(true);
		NcCurveAnimation NcCurve = go.GetComponent<NcCurveAnimation>();
		if(NcCurve != null)
			NcCurve.enabled = true;
		
		TweenColor color = go.GetComponent<TweenColor>();
		if(color != null)
		{
			color.Reset();
			color.enabled = true;
		}
		
		TweenScale scale = go.GetComponent<TweenScale>();
		if(scale != null)
		{
			scale.Reset();
			scale.enabled = true;
		}		
		
		CenterTipQueue.Dequeue();
		
		LastTime	= Time.time;
	}
}
