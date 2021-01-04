using UnityEngine;

public class XUTSingleTalk : XUICtrlTemplate<XSingleTalk>
{
	private string content;
	
	public XUTSingleTalk()
	{
		XEventManager.SP.AddHandler(SetContentHandler,EEvent.SingleTalk_Content);
	}
	
    public void SetContentHandler(EEvent evt, params object[] args)
	{
		content	= (string)args[0];
	}
	
	
	public override void OnShow()
	{
		LogicUI.labelContent.text	= content;
	}
}
