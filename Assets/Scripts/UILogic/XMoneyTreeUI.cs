using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[AddComponentMenu("UILogic/XMoneyTree")]

public class XMoneyTreeUI : XDefaultFrame
{

    public GameObject ButtonShake;
    public GameObject position;

    public UILabel CostMoneyLabel;
    public UILabel GetGameMoneyLabel;
    public UILabel LeftCountLabel;
    public AudioClip ShakeAudio;
    public float volume = 1f;
    public float pitch = 1f;

    public override bool Init()
    {
        base.Init();
        if (ButtonShake == null)
        {
            Log.Write(LogLevel.ERROR, "ButtonShake not found!");
            return false;
        }

        UIEventListener listenerButtonShake = UIEventListener.Get(ButtonShake);
        listenerButtonShake.onClick += SubmitShake;

        if (CostMoneyLabel == null)
        {
            Log.Write(LogLevel.ERROR, "CostMoneyLable not found!!");
            return false;
        }

        if (GetGameMoneyLabel == null)
        {
            Log.Write(LogLevel.ERROR, "GetGameMoneyLable not found!!");
            return false;
        }

        CostMoneyLabel.text = string.Format(XStringManager.SP.GetString(84), XMoneyTreeManager.SP.GetCostRealMoney());
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;

        return true;
    }
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eMoneyTree);
	}

    public void SetCostRealMoneyLabel(string text)
    {
        CostMoneyLabel.text = text;
    }

    public void SetGetMoneyLabel(string text)
    {
        GetGameMoneyLabel.text = text;
        this.setAlpha(GetGameMoneyLabel.gameObject, 10, 2, 0, UITweener.Method.BounceOut);
    }

    public void SetLeftCountLabel(string text)
    {
        this.LeftCountLabel.text = text;
    }

    private void SubmitShake(GameObject go)
    {
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_MoneyTree_Click);
		
        XEventManager.SP.SendEvent(EEvent.MoneyTree_GoShake);
    }

    public override void Show()
    {
        base.Show();
        XEventManager.SP.SendEvent(EEvent.MoneyTree_Update);
		
		// 新手引导
		Vector3 showPos = new Vector3(-150, 10, 0);
		XNewPlayerGuideManager.SP.pushGuideData2Queue(EEvent.PlayerGuide_Start, (int)XNewPlayerGuideManager.GuideType.Guide_MoneyTree_Click,
			(int)XNewPlayerGuideManager.GuideType.Guide_MoneyTree_Click,
			2, showPos, ButtonShake, true);
    }

    public bool ShowObject(GameObject go)
    {
        if (go == null && go is GameObject)
        {
            Log.Write(LogLevel.ERROR, "ERROR:Could'nt found the GameObject to SetActive");
            return false;
        }
        if (go.gameObject.activeSelf)
            return false;
        go.gameObject.SetActive(true);
        return true;
    }

    private void setAlpha(GameObject alphaObject, float duration, float from, float to, TweenAlpha.Method method)
    {
        TweenAlpha talpha = null;
        if (alphaObject.GetComponent<TweenAlpha>() == null)
        {
            alphaObject.AddComponent<TweenAlpha>();
        }
        talpha = alphaObject.GetComponent<TweenAlpha>();
        talpha.duration = duration;
        talpha.from = from;
        talpha.to = to;
        talpha.Reset();
        talpha.enabled = true;
    }
	
	public void PlayAudio(AudioClip sound)
	{
		if(sound.isReadyToPlay)
		{
			NGUITools.PlaySound(sound, volume, pitch);
		}
	}
}
