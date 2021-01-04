using UnityEngine;
using System.Collections;

class XUTInputMessageBox : XUICtrlTemplate<XInputMessageBox>
{
	private int mMaxValue;
	public XUTInputMessageBox()
	{
		XEventManager.SP.AddHandler(PreInputMessageBox, EEvent.InputMessageBox);
		XEventManager.SP.AddHandler(SaveData, EEvent.InputMessageBoxSetMaxValue);
		RegEventAgent_CheckCreated(EEvent.InputMessageBox, OnInputMessageBox);
	}
	
	public override void OnHide()
	{
		LogicUI.InputContent.MaxValidValue	= 0;
	}
	
	private void PreInputMessageBox(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eInputMessageBox);
	}
	
	private void SaveData(EEvent evt, params object[] args)
	{
		mMaxValue	= (int)args[0];
	}
	
	private void OnInputMessageBox(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eInputMessageBox);
		LogicUI.InputContent.MaxValidValue	= mMaxValue;
		LogicUI.InputContent.text	= "1";
		LogicUI.MessageBox(args[0], args[1], args[2]);
		
		try{
			UIInputValidator validator = LogicUI.InputContent.gameObject.GetComponent<UIInputValidator>();
			validator.logic = (UIInputValidator.Validation)args[3];
			LogicUI.InputContent.text = (string)args[4];
			LogicUI.InputContent.maxChars = (int)args[5];
		}
		catch{
			//nothing to do;
		}
	}
}
