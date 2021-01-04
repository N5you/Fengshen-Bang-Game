using UnityEngine;
using System.Collections;

[USequencerFriendlyName("Black Ground")]
[USequencerEvent("37Game/Black Ground")]
public class XBlackGroundEffect : USEventBase {
	public USEventBase.UILayer uiLayer = USEventBase.UILayer.Front;
	
	public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f), new Keyframe(3.0f, 1.0f), new Keyframe(4.0f, 0.0f));
	public Color fadeColour = Color.black;
	
	private float currentCurveSampleTime = 0.0f;
	static public Texture2D texture = null;
	
	public string m_strWord;
	
	public override void FireEvent()
	{
		XEventManager.SP.SendEvent(EEvent.CutScene_BackWord,m_strWord);
	}
	
	public override void ProcessEvent(float deltaTime)
	{		
		currentCurveSampleTime = deltaTime;
		float alpha = fadeCurve.Evaluate(currentCurveSampleTime);
		alpha = Mathf.Min(Mathf.Max(0.0f, alpha), 1.0f);
		
		if(XCutSceneMgr.SP.isStarted){
			XEventManager.SP.SendEvent(EEvent.CutScene_BackAlpha,alpha );
		}
		else{
			if(!texture)
				texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			
	    	texture.SetPixel(0, 0, new Color(fadeColour.r, fadeColour.g, fadeColour.b, alpha));
			texture.Apply();
		}
	}
	
	public override void EndEvent()
	{	
		float alpha = fadeCurve.Evaluate(fadeCurve.keys[fadeCurve.length - 1].time);
		alpha = Mathf.Min(Mathf.Max(0.0f, alpha), 1.0f);
		
		if(XCutSceneMgr.SP.isStarted )
		{
			XEventManager.SP.SendEvent(EEvent.CutScene_BackAlpha,alpha );
		}else
		{
			if(!texture)
			texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			
	    	texture.SetPixel(0, 0, new Color(fadeColour.r, fadeColour.g, fadeColour.b, alpha));
			texture.Apply();
		}
		
	}
	
	public override void StopEvent()
	{
		UndoEvent();
	}
	
	public override void UndoEvent()
	{	
		currentCurveSampleTime = 0.0f;
		
		if(XCutSceneMgr.SP.isStarted )
		{
			XEventManager.SP.SendEvent(EEvent.CutScene_BackAlpha,0.0f );
		}else{
			if(!texture)
			texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			
	    	texture.SetPixel(0, 0, new Color(fadeColour.r, fadeColour.g, fadeColour.b, 0.0f));
			texture.Apply();
		}
		
	}
	
	void OnGUI()
	{	
		if(!Sequence.IsPlaying)
			return;
		if(XCutSceneMgr.SP.isStarted )
			return;
		
		float maxTime = 0.0f;
		foreach(Keyframe key in fadeCurve.keys)
		{
			if(key.time > maxTime)
				maxTime = key.time;
		}
		
		Duration = maxTime;
		
		if(!texture)
			return;
		
		int previousDepth = GUI.depth;
		
		GUI.depth = (int)uiLayer;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
		
		GUI.depth = previousDepth;
	}
	
	void OnEnable()
	{
		if(XCutSceneMgr.SP.isStarted )
		{
			XEventManager.SP.SendEvent(EEvent.CutScene_BackAlpha,0.0f );	
		}else{
			if(texture == null)
				texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		
	    	texture.SetPixel(0, 0, new Color(fadeColour.r, fadeColour.g, fadeColour.b, 0.0f));
			texture.Apply();	
		}
		

	}
}
