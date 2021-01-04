using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
class XUTMoneyTreeUI : XUICtrlTemplate<XMoneyTreeUI>
{
	private static int ShakeAnimID = 900014;

	public XUTMoneyTreeUI ()
	{
		XEventManager.SP.AddHandler (SendShake, EEvent.MoneyTree_GoShake);
		XEventManager.SP.AddHandler (OnShakeResualt, EEvent.MoneyTree_Resualt);
		XEventManager.SP.AddHandler (OnMaxShake, EEvent.MoneyTree_MaxShake);
		XEventManager.SP.AddHandler (UpdateCost, EEvent.MoneyTree_Update);
		XEventManager.SP.AddHandler (OnSetShackTime, EEvent.MoneyTree_ShakeTimes);
	}

	private void SendShake(EEvent evt, params object[] args)
	{
		if (LogicUI == null) {
			Log.Write (LogLevel.ERROR, "LogicUI is null!!");
			return;
		}
		if (!XMoneyTreeManager.SP.SendShake ())
			return;
		LogicUI.PlayAudio(LogicUI.ShakeAudio);
		XU3dEffect effect = new XU3dEffect ((uint)ShakeAnimID, EffectLoadedHandle);
	}

	private void OnShakeResualt(EEvent evt, params object[] args)
	{
		int getGameMoney = (int)args [0];
		int crit = (int)args [1];

		if (crit == 0) {
			string resualtText = string.Format (XStringManager.SP.GetString (88), getGameMoney);
			LogicUI.SetGetMoneyLabel (resualtText);
		}
		else {
			string resualtText = string.Format (XStringManager.SP.GetString (85), crit, getGameMoney); //暴击tip
			LogicUI.SetGetMoneyLabel (resualtText);
		}
		LogicUI.ShowObject (LogicUI.GetGameMoneyLabel.gameObject);

		string costRealMoneyText = string.Format (XStringManager.SP.GetString (84), XMoneyTreeManager.SP.GetCostRealMoney ());
		LogicUI.SetCostRealMoneyLabel (costRealMoneyText);

		string leftCountText = XMoneyTreeManager.SP.LeftCount.ToString ();
		LogicUI.SetLeftCountLabel (leftCountText);
	}

	private void OnMaxShake(EEvent evt, params object[] args)
	{
		string costRealMoneyText = string.Format (XStringManager.SP.GetString (82));
		LogicUI.SetCostRealMoneyLabel (costRealMoneyText);
	}

	private void UpdateCost(EEvent evt, params object[] args)
	{
		string costRealMoneyText;
		if (XMoneyTreeManager.SP.IsMaxShake ()) {
			costRealMoneyText = string.Format (XStringManager.SP.GetString (82));
		}
		else {
			costRealMoneyText = string.Format (XStringManager.SP.GetString (84), XMoneyTreeManager.SP.GetCostRealMoney ());
		}
		LogicUI.SetCostRealMoneyLabel (costRealMoneyText);

		string leftCountText = XMoneyTreeManager.SP.LeftCount.ToString ();
		LogicUI.SetLeftCountLabel (leftCountText);
	}
	
	private void OnSetShackTime(EEvent evt, params object[] args)
	{
		if (LogicUI != null) {
			LogicUI.SetLeftCountLabel (XMoneyTreeManager.SP.LeftCount.ToString());
		}
	}

	//UI金钱动画
	void EffectLoadedHandle(XU3dEffect effect)
	{
		effect.Layer = GlobalU3dDefine.Layer_UI_2D;
		effect.Parent = LogicUI.position.transform;
		effect.LocalPosition = new Vector3 (0, -350, -100);
		effect.Scale = new Vector3 (120, 120, 1);
	}
}

