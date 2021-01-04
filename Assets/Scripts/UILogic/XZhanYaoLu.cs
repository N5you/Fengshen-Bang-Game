using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using System;

[AddComponentMenu("UILogic/XZhanYaoLu")]
public class XZhanYaoLu : XDefaultFrame
{
	public static readonly int MAX_FIGHT_CNT = 10;
	public static readonly int MAX_BUY_CNT = 10;
	public static readonly int CD_TIME = 300;
	
	
	public UILabel LabMonsterName;
	public UILabel LabLeftCnt;
	public UILabel LabCDTime;
	public UILabel LabCurComboSingle;
	public UILabel LabCurComboTens;
	public UILabel LabMaxComboCnt;
	public UILabel LabMoney;
	public UILabel LabShengWang;
	
	public UIImageButton BtnBuyCnt;
	public UIImageButton BtnStartKill;
	public UIImageButton BtnClearCD;
	
	public UISprite SprCombo;
	
	public UISprite SprKilled;
	public UILabel  LabCDString;
	public UILabel  LabGetString;
	
	public 	UITexture ModelTex;
	private SingleModelRTT m_rtt = null;		
	private XObjectModel m_objectModel = null;

	
	public GameObject guideObj = null;
	
	public override bool Init()
	{
        base.Init();
		
		UIEventListener ls1 = UIEventListener.Get(BtnStartKill.gameObject);
		ls1.onClick	+= StartKillMonster;
		
		UIEventListener ls2 = UIEventListener.Get(BtnBuyCnt.gameObject);
		ls2.onClick	+= BuyFightCnt;
		
		UIEventListener ls3 = UIEventListener.Get(BtnClearCD.gameObject);
		ls3.onClick	+= ClearCD;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		m_objectModel = new XObjectModel();
		m_objectModel.mainModelLoaded += loadModelFinish;
		
		
		return true;
	}
	public override void Show()
	{
		base.Show();
		
		Vector3 showPos = new Vector3(10, -240, Mathf.Round(guideObj.transform.localPosition.z));
		XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_StartZhanYaoLu, 2, showPos, guideObj);
	}
	
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eZhanYaoLu);
	}
	
	public void loadModelFinish (XObjectModel model)
	{
		if(m_rtt != null)
			m_rtt.DestoryNotModel();
		m_rtt = XModelRTTMgr.SP.AddObjectRTT(m_objectModel.mainModel, ModelTex, 1.4f, new Vector3(0f, -1.5f, 2f), 
			Vector3.zero, Vector3.one, false);
	}
	
	public void	StartKillMonster(GameObject go)
	{
		XZhanYaoLuManager.SP.ApplyKillMonster();
		
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_StartZhanYaoLu);
	}
	
	public void	BuyFightCnt(GameObject go)
	{
		XZhanYaoLuManager.SP.AskBuyFightCnt();
	}
	
	public void	ClearCD(GameObject go)
	{
		XZhanYaoLuManager.SP.AskClearCD();
	}
	public void Update_Time()
	{
		int leftCDTime = (int)XZhanYaoLuManager.SP.LeftCDTime;
		string text = XUtil.GetTimeStrByInt((int)leftCDTime, 2);
		LabCDTime.text = text;
		if(leftCDTime == 0)
		{
			LabCDTime.enabled = false;
			BtnClearCD.gameObject.SetActive(false);
			LabCDString.enabled = false;
		}
		else
		{
			LabCDTime.enabled = true;
			BtnClearCD.gameObject.SetActive(true);
			LabCDString.enabled = true;
		}
	}
	public void UpdateInfoUI()
	{
		int guanKaID = XZhanYaoLuManager.SP.GuanKaID;
		int comboCnt = XZhanYaoLuManager.SP.ComboCnt;
		int leftFightCnt = XZhanYaoLuManager.SP.LeftFightCnt;
		int maxComboCnt = XZhanYaoLuManager.SP.MaxComboCnt;
		bool isKilled = XZhanYaoLuManager.SP.IsAllreadyKilled;
		
		if(isKilled)
		{
			SprKilled.enabled = true;
			LabGetString.text = XStringManager.SP.GetString(534);
		}
		else
		{
			SprKilled.enabled = false;
			LabGetString.text = XStringManager.SP.GetString(533);
		}
		
		XCfgZhanYaoLu cfg = XCfgZhanYaoLuMgr.SP.GetConfig((uint)guanKaID);
		if(cfg == null)
			return;
		
		int colorLevel = System.Convert.ToInt32(cfg.NameColor);
		LabMonsterName.text = XGameColorDefine.Quality_Color[colorLevel] + cfg.Name;
		
		LabLeftCnt.text = leftFightCnt.ToString() + XStringManager.SP.GetString(420);

		Update_Time();
		
		SprCombo.enabled = true;
		LabCurComboSingle.enabled = true;
		int tensNum = comboCnt / 10;
		int singleNum = comboCnt % 10;
		if(tensNum != 0)
		{
			LabCurComboTens.enabled = true;
			LabCurComboTens.text = tensNum.ToString();
		}
		else
		{
			LabCurComboTens.enabled = false;
		}
		LabCurComboSingle.text = singleNum.ToString();

		
		m_objectModel.HandleEvent(EModelEvent.evtModelId, cfg.MonsterModel);
		//m_rtt.ShowObjModel(mountModel, ObjectModelParent, defaultSize, new Vector3(0f, -1.5f, 2f), new Vector3(0f, 300f, 0f), Vector3.one, false);
		
		LabMaxComboCnt.text = maxComboCnt.ToString();
		
		XCfgLianZhan cfgCombo = XCfgLianZhanMgr.SP.GetConfig((uint)comboCnt);
		if(cfgCombo == null)
			return;
		
		uint money = (uint)(cfg.Money * cfgCombo.MoneyRate);
		uint shengWang = (uint)(cfg.Reputation * cfgCombo.ShengWangRate);
		
		LabMoney.text = money.ToString()  + XStringManager.SP.GetString(421);
		LabShengWang.text = shengWang.ToString() + XStringManager.SP.GetString(422);
	}
}