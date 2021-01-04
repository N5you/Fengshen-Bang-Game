using UnityEngine;
using System;

class XUTPassInfo : XUICtrlTemplate<XPassInfo>
{
	public XUTPassInfo()
	{
		XEventManager.SP.AddHandler(RetSceneHandle,EEvent.PassInfo_RetScene);
	}
	
	
	public  void RetSceneHandle(EEvent evt, params object[] args)
	{
		XLogicWorld.SP.SubSceneManager.LeaveClientScene();
	}	
	
	public override void OnShow()
	{
		base.OnShow();
		
		string name = XLogicWorld.SP.SubSceneManager.GetSubSceneName();
		uint LeftMonster = XLogicWorld.SP.SubSceneManager.GetLeftMonster();
		
		LogicUI.PassName.text 			= name;
		LogicUI.PassLeftMonster.text	= XStringManager.SP.GetString(30)+ Convert.ToString(LeftMonster);
	}
	
}
