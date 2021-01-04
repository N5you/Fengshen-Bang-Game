using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
using System;

[AddComponentMenu("UILogic/XSaoDang")]
public class XSaoDang : XDefaultFrame
{
	public static readonly int MAX_MONSTER_TYPE_NUM = 6;
	//每次扫荡扣除体力
	public static readonly int 	SD_COST_TI_LI = 2;
	//每次扫荡花费时间
	public static readonly int    SD_COST_TIME = 30;
	
	
	[System.Serializable]
	public class SaoDangPrepare
	{
		public GameObject m_PrepareGo = null;
		public GameObject m_ProcessGo = null;
		
		public UIImageButton BtnStart;
		public UIImageButton BtnAdd;
		public UIImageButton BtnDec;
		
		public UILabel LabSceneName;
		public UILabel[] LabListMonsterName  =  new UILabel[MAX_MONSTER_TYPE_NUM];
		public UILabel LabCost;
		public UILabel LabCnt;
		public UILabel LabCanGet;
		
		public UIInput SaoDangCount;
			
		int m_InputCnt = 1;
		public int InputCnt
		{
		    get
		    {
		        return m_InputCnt;
		    }
		    set 
			{ 
				m_InputCnt = value;
				UpdateCost();
			}
		}
		
		
		public void Init()
		{
			UIEventListener ls1 = UIEventListener.Get(BtnStart.gameObject);
			ls1.onClick	+= StartSaoDang;
			
			
			UIEventListener ls2 = UIEventListener.Get(SaoDangCount.gameObject);
			ls2.onInput += OnInput;
			
			UIEventListener ls3 = UIEventListener.Get(BtnAdd.gameObject);
			ls3.onClick	+= ClickAdd;
			
			UIEventListener ls4 = UIEventListener.Get(BtnDec.gameObject);
			ls4.onClick	+= ClickDec;
			
			for(int i=0; i<MAX_MONSTER_TYPE_NUM; i++)
			{
				LabListMonsterName[i].text ="";
			}
		}
		
		public void Show()
		{
			m_PrepareGo.SetActive(true);
			UpdateUI();	
		}
		
		public void UpdateUI()
		{
			int ClientSceneID = XSaoDangManager.SP.ClientSceneID;
			int ClientSceneLevel = XSaoDangManager.SP.ClientSceneLevel;
			int Count = XSaoDangManager.SP.LeftCnt;
			
			InputCnt = Count;
			
			SaoDangCount.text = InputCnt.ToString();
			XCfgClientScene cfgClient = XCfgClientSceneMgr.SP.GetConfig((uint)ClientSceneID);
			if(cfgClient == null)
				return;
			LabSceneName.text = cfgClient.Name;
			
			 SortedList<string, UInt32> monsterList = new SortedList<string, UInt32>();
			
			for(int i = 0; i < 6; i++)
			{
				UInt32 groupID = cfgClient.GroupID[i ];
				XCfgMonsterGroup cfgGroup = XCfgMonsterGroupMgr.SP.GetConfig((uint)groupID);
				if(cfgGroup == null)
					continue;
				for (int batPos = 1; batPos < (int)XBattleDefine.BATTLE_POS_COUNT; ++batPos)
				{
					XCfgMonsterBase cfgMon = XCfgMonsterBaseMgr.SP.GetConfig(cfgGroup.MonID[batPos]);
					if (cfgMon == null)
						continue;
					if(monsterList.ContainsKey(cfgMon.Name))
					{
						monsterList[cfgMon.Name] ++;
					}
					else
					{
						monsterList.Add(cfgMon.Name,1);
					}
				}
			}
			
			for(int i=0; i<monsterList.Count; i++)
			{
				if(i >= MAX_MONSTER_TYPE_NUM)
					return;
				string name = monsterList.Keys[i];
				string num = monsterList.Values[i].ToString();
				LabListMonsterName[i].text = name + " * " + num;
			}
		}
		
		public void UpdateCost()
		{
			string costStr = "";
			if(InputCnt != 0)
			{
				int costTiLi = InputCnt * 2;
				int costTime = InputCnt * 3 * (int)SD_COST_TIME;
				string Timetext = XUtil.GetTimeStrByInt((int)costTime, 3);
				costStr = string.Format(XStringManager.SP.GetString(550),costTiLi,Timetext) ;
			}
			LabCost.text = costStr;
		}
		public void Hide()
		{
			m_PrepareGo.SetActive(false);
		}
		
		public void	StartSaoDang(GameObject go)
		{
			m_PrepareGo.SetActive(false);
			m_ProcessGo.SetActive(true);
			
			XSaoDangManager.SP.LeftCnt = InputCnt;
			XSaoDangManager.SP.ApplyStartSaoDang();
		}
		
		public void	ClickAdd(GameObject go)
		{
		    string numStr = SaoDangCount.text;
			int inputNum=0;
			if(int.TryParse(numStr,out inputNum))
			{
				UInt32 MaxNum = (UInt32)XLogicWorld.SP.MainPlayer.Power / (UInt32)SD_COST_TI_LI;
				if(inputNum >= MaxNum)
					inputNum = (int)MaxNum;
				else
					inputNum++;
				SaoDangCount.text = inputNum.ToString();
				InputCnt = inputNum;
			}	
		}
		
		public void	ClickDec(GameObject go)
		{
		    string numStr = SaoDangCount.text;
			int inputNum=0;
			if(int.TryParse(numStr,out inputNum))
			{
				if(inputNum <= 0)
					inputNum = 0;
				else
					inputNum--;
				SaoDangCount.text = inputNum.ToString();
				InputCnt = inputNum;
			}	
		}
		public void	OnInput(GameObject go, string inputStr)
		{
			UInt32 MaxNum = (UInt32)XLogicWorld.SP.MainPlayer.Power / (UInt32)SD_COST_TI_LI;
			
		    string numStr = SaoDangCount.text;
			int inputNum=0;
			if(int.TryParse(numStr,out inputNum))
			{
				if(inputNum < 0 || inputNum > MaxNum)
				{
					inputNum =(int) MaxNum;
					SaoDangCount.text = inputNum.ToString();
				}
				InputCnt = inputNum;
			}
			//int inputNum = System.Convert.ToInt32(numStr);
		}
		
	}
	[System.Serializable]
	public class SaoDangProcess
	{
		public GameObject m_PrepareGo = null;
		public GameObject m_ProcessGo = null;
		
		public UIImageButton BtnSpeedUp;
		public UIImageButton BtnEnd;
		
		public UILabel LabLeftTimeStr;
		public UILabel LabLeftTime;
		
		public UILabel LabEnd;
		
		public UILabel LabLeftCnt;
		public UILabel LabTotalExp;
		public UILabel LabTotalMoney;
		
		public UILabel LabBattleResult;
		
		public void Init()
		{
			LabLeftCnt.text = "";
			LabTotalExp.text = "";
			LabTotalMoney.text = "";
			//UIEventListener ls1 = UIEventListener.Get(BtnStartKill.gameObject);
			//ls1.onClick	+= StartKillMonster;
		}
		
		public void Show()
		{
			m_ProcessGo.SetActive(true);
		}

		public void Hide()
		{
			m_ProcessGo.SetActive(false);
		}
		
		public void UpdateInfo()
		{
			SC_BattleResult result = XSaoDangManager.SP.m_Result;
			if(result.HasBonus)
			{
				uint addEXP = result.Bonus.BonusExp;
				uint addMoney = result.Bonus.GameMoney;
				LabTotalExp.text = addEXP.ToString();
				LabTotalMoney.text = addMoney.ToString();
			}
		}
	}
	
	public SaoDangPrepare m_PrepareSaoDang = null;
	public SaoDangProcess m_ProcessSaoDang = null;
	
	
	public override bool Init()
	{
        base.Init();
		m_PrepareSaoDang.Init();
		m_ProcessSaoDang.Init();
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		//UIEventListener ls1 = UIEventListener.Get(BtnStartKill.gameObject);
		//ls1.onClick	+= StartKillMonster;
		return true;
	}
	
	public override void Show()
	{
		base.Show();
		m_PrepareSaoDang.Show();
		m_ProcessSaoDang.Hide();
	}
	
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eSaoDang);
	}
	
	public void Update_Battle_Result()
	{
		m_ProcessSaoDang.UpdateInfo();
	}
}