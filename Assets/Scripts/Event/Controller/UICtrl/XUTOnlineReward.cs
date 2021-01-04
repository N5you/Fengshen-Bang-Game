using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;
using System.Timers;
class XUTOnlineReward : XUICtrlTemplate<XOnlineReward>
{
	private int roundedRestSeconds;
	private float secondTime;
	private float minutesTime;
	private float hourTime;
	private int countDownSeconds;
	private uint m_GetID;
	private Timer m_Timer;
	private bool m_IsTimeToGet;
	private static int effectID = 900039;
	private XU3dEffect m_Effect;
	private bool isTimeFirstStart;

	UIEventListener.VoidDelegate OpenBag;
	
	public XUTOnlineReward ()
	{
		XEventManager.SP.AddHandler (ClickButton, EEvent.OnlineReward_GetItem);
		RegEventAgent_CheckCreated (EEvent.OnlineReward_NewEvent, NewEvent);
	
		m_GetID = 1;
		m_Timer = new Timer (1000);
		m_Timer.Elapsed += new ElapsedEventHandler (CalcTime);
		m_IsTimeToGet = true;
		isTimeFirstStart = true;
		
		OpenBag = new UIEventListener.VoidDelegate (MessageBox_IsOpenItemBag);
	}

	private bool m_openEffect = false;

	public override void Breathe()
	{
		base.Breathe ();
		if (m_openEffect == true) {
			CreateEffect ();
		}
	}

	private void NewEvent(EEvent evt, params object[] args)
	{
		if (isTimeFirstStart) {
			uint cdtime = XOnlineRewardManager.SP.GetTime ();
			TimerStart (cdtime);
			isTimeFirstStart = false;
			LogicUI.gameObject.SetActive(true);
		}
	}

	private void ClickButton(EEvent evt, params object[] args)
	{
		
		if (!m_IsTimeToGet || !XOnlineRewardManager.SP.IsCanGet) {
			XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 22);
			return;
		}
		
		if (!XOnlineRewardManager.SP.HandleGetItem ()) {
			return;
		}
		uint nextTime = XOnlineRewardManager.SP.GetNextTime ();
		string tname = XOnlineRewardManager.SP.GetCurRewardItemName ();
		
		uint second = nextTime % 60;
		uint mins = (uint)(nextTime / 60);
			
		
		
		if (XOnlineRewardManager.SP.IsLastGetID ()) {
			XEventManager.SP.SendEvent (EEvent.MessageBox, OpenBag, null, string.Format (XStringManager.SP.GetString (199), tname, "\n"));
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eOnlineReawrd);
		}
		else {
			TimerStart (nextTime);
			XEventManager.SP.SendEvent (EEvent.MessageBox, OpenBag, null, string.Format (XStringManager.SP.GetString (205), tname, "\n", mins, second,"\n"));
		}
	}
	
	private void MessageBox_IsOpenItemBag(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eBagWindow);
	}

	private void CalcTime(object sender, ElapsedEventArgs e)
	{
		countDownSeconds--;
		hourTime = (int)countDownSeconds / 3600;
		minutesTime = (int)(countDownSeconds - hourTime * 3600) / 60;
		secondTime = (int)(countDownSeconds - hourTime * 3600 - minutesTime * 60);
		LogicUI.Tips.text = string.Format (XStringManager.SP.GetString (200), minutesTime, secondTime);
        	

		if (countDownSeconds <= 0) {
			TimerStop ();
		}
	}

	private void TimerStart(uint setGetTime)
	{
		countDownSeconds = (int)setGetTime;
		m_IsTimeToGet = false;
		m_Timer.Start ();
		if (m_Effect != null) {
			m_Effect.Destroy ();
		}
	}

	private void TimerStop()
	{
		m_Timer.Stop ();
		m_openEffect = true;
		m_IsTimeToGet = true;
		LogicUI.Tips.text = XStringManager.SP.GetString (203);
	}

	private void CreateEffect()
	{
		m_Effect = new XU3dEffect ((uint)effectID, EffectLoadedHandle);
		m_openEffect = false;
	}

	private void EffectLoadedHandle(XU3dEffect effect)
	{
		effect.Layer = GlobalU3dDefine.Layer_UI_2D;
		effect.Parent = LogicUI.EffactePostion.transform;
		effect.LocalPosition = new Vector3 (0, -10, 0);
		effect.Scale = new Vector3 (500, 500, 1);
	}
}
