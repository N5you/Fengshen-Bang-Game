using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XForceGuide")]
public class XForceGuide : XDefaultFrame
{
	bool DailySignClosedByHere = false;
	
	public static bool OnShowIng = false;
	
	public static Vector3 RepostionPos;
		
	public static bool OnFirstForceGuideIng = false;
	public static int FirstForceGuideStepCount = 1;
	
	public GameObject ClickObj;
	public GameObject BkGround;
	
	public GameObject centerShow;
	public GameObject LabelTip;
	
	public override bool Init ()
	{
		base.Init();
		
		this.afterReposition += onReposition;
		
		UIEventListener ls = UIEventListener.Get(ClickObj);
		ls.onClick += OnClickObj;
		
		ls = UIEventListener.Get(BkGround);
		ls.onClick += OnClckBk;
		
		return true;
	}
	
	public override void Show ()
	{
		OnShowIng = true;
		
		XMainPlayerAutoMove.SP.Stop(true);
		
		centerShow.SetActive(OnFirstForceGuideIng && 1 == FirstForceGuideStepCount);
		LabelTip.SetActive(OnFirstForceGuideIng && 1 != FirstForceGuideStepCount);
		
		if ( XUIDailySign.OnShowIng )
			XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eDailySign);
		
		//XHardWareGate.SP.LockKeyBoard = true;
		base.Show ();
	}
	
	public void OnClickObj(GameObject go)
	{
		if ( !OnFirstForceGuideIng )
			return;
		
		FirstForceGuideStepCount++;
		centerShow.SetActive(false);
		
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step1);
		XNewPlayerGuideManager.SP.pushGuideData2Queue(EEvent.PlayerGuide_StepStart, (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step2, 
			(int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide, 2, new Vector3(380, -190, 0), XMainPlayerInfo.RootParent, true);
	}
	
	public void OnClckBk(GameObject go)
	{
		if ( !OnFirstForceGuideIng || 1 == FirstForceGuideStepCount )
			return;
		
		if ( 2 == FirstForceGuideStepCount )
		{
			XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step2);
			XNewPlayerGuideManager.SP.pushGuideData2Queue(EEvent.PlayerGuide_StepStart, (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step3, 
				(int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide, 3, new Vector3(-550, -146, 0), XFunctionBottomTR.RootParent, true);
		}
		else if ( 3 == FirstForceGuideStepCount )
		{
			XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step3);
			XNewPlayerGuideManager.SP.pushGuideData2Queue(EEvent.PlayerGuide_StepStart, (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step4, 
				(int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide, 4, new Vector3(360, 115, 0), XChatWindow.RootParent, true);
		}
		else if ( 4 == FirstForceGuideStepCount )
		{
			XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step4);
			XNewPlayerGuideManager.SP.pushGuideData2Queue(EEvent.PlayerGuide_Start, (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide, 
				(int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide, 0, new Vector3(0, 0, 0), null, true);
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide);
		}
		FirstForceGuideStepCount++;
	}
	
	public void onReposition(Vector3 v)
	{
		RepostionPos = new Vector3(v.x, v.y, v.z);
		
		XEventManager.SP.SendEvent(EEvent.PlayerGuide_ReposPanel, v.x, v.y, v.z);
	}
	
	public override void Hide ()
	{
		OnShowIng = false;
		
		//XHardWareGate.SP.LockKeyBoard = false;
		base.Hide ();
		
		//XNewPlayerGuideManager.SP.RemoveLastShowedData();
	}
}
