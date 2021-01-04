using UnityEngine;
using System.Collections;

public interface IProcessLoad
{
	string GetProcessText();
	float  GetCurRate();
}

class XUTLoadScene : XUICtrlTemplate<XLoadSceneUI>
{
	public static IProcessLoad	CurLoading;
	public XUTLoadScene()
	{
		RegEventAgent_CheckCreated(EEvent.LoadScene_Discription, OnDiscription);
		RegEventAgent_CheckCreated(EEvent.LoadScene_Progress, OnLoadProgress);
	}
	
	private void OnDiscription(EEvent evt, params object[] args)
	{
		LogicUI.SetDiscription((string)(args[0]));
	}
	
	private void OnLoadProgress(EEvent evt, params object[] args)
	{
		LogicUI.SetProgress((float)(args[0]));
	}
	
	public override void Breathe()
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf )
			return ;
		
		if(CurLoading == null)
			return ;
		
		LogicUI.SetDiscription(CurLoading.GetProcessText());
		LogicUI.SetProgress((float)CurLoading.GetCurRate());
		
	}
}

