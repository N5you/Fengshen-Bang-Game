using UnityEngine;
using System.Collections;

class XUTLoginUI : XUICtrlTemplate<XLoginUI>
{
	public XUTLoginUI()
	{
		XEventManager.SP.AddHandler(CheckAccount, EEvent.Login_CheckAccount);
	}
	
	void CheckAccount(EEvent evt, params object[] args)
	{
		if(!XLogicWorld.SP.LoginProc.ConnectToLoginServer())
		{
			return;
		}
		XLogicWorld.SP.LoginProc.ApplyCheckAccount((string)args[0], (string)args[1]);
	}
}

