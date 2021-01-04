using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[AddComponentMenu("UILogic/XGMWindow")]

public class XGMWindow : XDefaultFrame
{
    public readonly static int MAX_NUMBER = 12;

    [System.Serializable]
    public class XCMDBtn
    {
        public string m_name;
        public string m_content;
        public GameObject m_btn;

        public XCMDBtn(string name, string content, GameObject go)
        {
            this.m_name = name;
            this.m_content = content;
            this.m_btn = go;
            this.m_btn.name = "Button_" + name;
            m_btn.transform.Find("Label").GetComponent<UILabel>().text = name;
            UIEventListener listener = UIEventListener.Get(m_btn);
            listener.onClick += SetText;
        }


        public XCMDBtn(string name, string content, GameObject go, Vector2 localPostion)
        {
            this.m_name = name;
            this.m_content = content;
            this.m_btn = go;
            this.m_btn.name = "Button_" + name;
            m_btn.transform.Find("Label").GetComponent<UILabel>().text = name;
            m_btn.transform.localPosition = new Vector3(localPostion.x, localPostion.y, 0);
            UIEventListener listener = UIEventListener.Get(m_btn);
            listener.onClick += SetText;
        }

        private void SetText(GameObject go)
        {
            XEventManager.SP.SendEvent(EEvent.Chat_SetChatData, m_content);
        }
    }

    public GameObject BtnPrefebs;
    public List<GameObject> Buttons;
    public List<XCMDBtn> CMDBtns;
    public GameObject MoneyTreeBTN;
    public GameObject OnlineRewardBTN;
    public GameObject FriendBTN;
	public GameObject FormationBTN;
	private GameObject MeditationBTN;
	private GameObject MeditationStartBtn;
	private GameObject DayActivityBtn;
	private GameObject XDHBtn;

    public override bool Init()
    {
        base.Init();

        Buttons = new List<GameObject>();
        for (int cnt = 0; cnt != MAX_NUMBER; ++cnt)
        {
            Buttons.Add(XUtil.Instantiate(BtnPrefebs, transform) as GameObject);
            Buttons[cnt].transform.parent = transform;
        }

        CMDBtns = new List<XCMDBtn>();
        CMDBtns.Add(new XCMDBtn("设置等级", "@xgm setlevel ", Buttons[0], new Vector2(-240, 80)));
        CMDBtns.Add(new XCMDBtn("设置游戏币", "@xgm setgamemoney ", Buttons[1], new Vector2(-240, 20)));
        CMDBtns.Add(new XCMDBtn("设置元宝", "@xgm setrealmoney ", Buttons[2], new Vector2(-240, -40)));
        CMDBtns.Add(new XCMDBtn("进入副本", "@xgm enterclientscene ", Buttons[3], new Vector2(-240, -100)));
        CMDBtns.Add(new XCMDBtn("增加技能点", "@xgm addskillpoint ", Buttons[4], new Vector2(-120, 80)));
        CMDBtns.Add(new XCMDBtn("学习技能", "@xgm learnskill ", Buttons[5], new Vector2(-120, 20)));
        CMDBtns.Add(new XCMDBtn("增加BUFF", "@xgm addbuff ", Buttons[6], new Vector2(-120, -40)));
        CMDBtns.Add(new XCMDBtn("增加宠物", "@xgm addpet ", Buttons[7], new Vector2(-120, -100)));
        CMDBtns.Add(new XCMDBtn("添加道具", "@xgm additem ", Buttons[8], new Vector2(0, 80)));
        CMDBtns.Add(new XCMDBtn("增加生产熟练", "@xgm AddCareerExp ", Buttons[9], new Vector2(0, 20)));
        CMDBtns.Add(new XCMDBtn("设置强化", "@xgm SetStrength ", Buttons[10], new Vector2(0, -40)));
	CMDBtns.Add(new XCMDBtn("添加任务", "@xgm addMission ", Buttons[11], new Vector2(0, -100)));
		
		UIEventListener ls = UIEventListener.Get(ButtonExit.gameObject);
		ls.onClick	= ClickExit;

        OtherButton();
        return true;
    }
	
	public void ClickExit(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eGMWindow);
	}


    private void OtherButton()
    {
        MoneyTreeBTN = XUtil.Instantiate(BtnPrefebs, transform) as GameObject;
        MoneyTreeBTN.transform.localPosition = new Vector3(220, 80, 0);
        MoneyTreeBTN.transform.Find("Label").GetComponent<UILabel>().text = "摇钱树";
        UIEventListener listenerYaoqianshu = UIEventListener.Get(MoneyTreeBTN);
        listenerYaoqianshu.onClick += OpenWindow;


        OnlineRewardBTN = XUtil.Instantiate(BtnPrefebs, transform) as GameObject;
        OnlineRewardBTN.transform.localPosition = new Vector3(220, 40, 0);
        OnlineRewardBTN.transform.Find("Label").GetComponent<UILabel>().text = "在线奖励";
        UIEventListener listenerOnlineReward = UIEventListener.Get(OnlineRewardBTN);
        listenerOnlineReward.onClick += OnlineReward;
		
	FriendBTN = XUtil.Instantiate(BtnPrefebs, transform) as GameObject;
        FriendBTN.transform.localPosition = new Vector3(220, 0, 0);
        FriendBTN.transform.Find("Label").GetComponent<UILabel>().text = "我的好友";
        UIEventListener listenerFriend = UIEventListener.Get(FriendBTN);
        listenerFriend.onClick += Friend;

		FormationBTN = XUtil.Instantiate(BtnPrefebs, transform) as GameObject;
		FormationBTN.transform.localPosition = new Vector3(220, -40, 0);
		FormationBTN.transform.Find("Label").GetComponent<UILabel>().text = "布阵";
		UIEventListener listenerFormationBTN = UIEventListener.Get(FormationBTN);
		listenerFormationBTN.onClick += Formation;

		MeditationBTN = XUtil.Instantiate(BtnPrefebs, transform) as GameObject;
		MeditationBTN.transform.localPosition = new Vector3(220, -80, 0);
		MeditationBTN.transform.Find("Label").GetComponent<UILabel>().text = "打坐UI";
		UIEventListener listenerMeditationBTN = UIEventListener.Get(MeditationBTN);
		listenerMeditationBTN.onClick += OnMeditation;

		MeditationStartBtn = XUtil.Instantiate(BtnPrefebs, transform) as GameObject;
		MeditationStartBtn.transform.localPosition = new Vector3(220, -120, 0);
		MeditationStartBtn.transform.Find("Label").GetComponent<UILabel>().text = "打坐开始";
		UIEventListener listenerMeditationStartBtn = UIEventListener.Get(MeditationStartBtn);
		listenerMeditationStartBtn.onClick += OnMeditationStart;
		
		DayActivityBtn = XUtil.Instantiate(BtnPrefebs, transform) as GameObject;
		DayActivityBtn.transform.localPosition = new Vector3(220, -160, 0);
		DayActivityBtn.transform.Find("Label").GetComponent<UILabel>().text = "活跃度奖励";
		UIEventListener listenerDayActivityBtn = UIEventListener.Get(DayActivityBtn);
		listenerDayActivityBtn.onClick += OnDayActivityBtn;
		
		XDHBtn = XUtil.Instantiate(BtnPrefebs, transform) as GameObject;
		XDHBtn.transform.localPosition = new Vector3(220, -200, 0);
		XDHBtn.transform.Find("Label").GetComponent<UILabel>().text = "仙道会";
		UIEventListener listenerXDHBtn = UIEventListener.Get(XDHBtn);
		listenerXDHBtn.onClick += OnXDHBtn;
    }

    private void OpenWindow(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.UI_Toggle, (int)EUIPanel.eMoneyTree);
        Log.Write("摇钱树UI事件触发");
    }


    private void OnlineReward(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.UI_Toggle, (int)EUIPanel.eOnlineReawrd);
    }
	
    private void Friend(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.UI_Toggle, (int)EUIPanel.eFriend);
    }

	private void Formation(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, (int)EUIPanel.eFormation);
	}

	private void OnMeditation(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, (int)EUIPanel.eMeditation);
	}

	private void OnMeditationStart(GameObject go)
	{
		XMeditationManager.SP.MeditationRequest();
	}
	
	private void OnDayActivityBtn(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, (int)EUIPanel.eDayActivity);
	}
	
	private void OnXDHBtn(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, (int)EUIPanel.eXianDH);
	}
	
}
