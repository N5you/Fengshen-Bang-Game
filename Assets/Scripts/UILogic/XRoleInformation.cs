using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XRoleInformation")]
public class XRoleInformation : XDefaultFrame
{	
	public bool IsFirstOpen { get; set; }
	
	public static readonly int MAX_CHAR_ICON_NUM = 7;
	public static readonly int MAX_CHAR_ATTR_NUM = 21;
	public static readonly int MAX_CHAR_EQUIP_NUM = 7;
	private static readonly int MAX_ROLE_NUM = 11;
	public static readonly int MAX_FUNC_SEL_NUM = 4;
	public static readonly int MAX_ITEM_SEL_NUM = 18;
	
	
	//顶部标题	
	public UISpriteGroup	LSpriteGroup;
	public UISpriteGroup	TSpriteGroup;
	
	// 新手引导用到，获取窗口的位置
	public GameObject[]		UserSelectAttrObj;
	public GameObject[]		PeyYangObj;
	
	//界面左部信息，角色和宠物公用
	[System.Serializable]
	public class RoleInfoLeft
	{
		private static  readonly int RoleFuncBtnNum = 2;
		public UILabel 		CurSelLevel;
		//public GameObject 	mUIModelBK;
		public UITexture	mUITexture;
		
		public UILabel		ShengWangDes;
		public UILabel		ShengWangValue;
		
		public GameObject	ClothesGO;
		
		//宠物专有
		//public UISprite LoyalMark;
		//public UISprite AptitudeMark;
		//public UILabel	LabelL;
		//public UILabel	LabelR;
		
		//6个属性描述
		//1.职业
		public UILabel CareerName;
		//2.境界
		public UILabel StateName;
		//3.特质(无双)
		public UILabel	TeZHiDes;
		public UILabel	TeZhiName;
		//4.灵力(成长)
		public UILabel	GrowValue;
		//5.绝招
		public UILabel	JueZhaoName;
		//6.战力
		public UILabel BattleValue;
		
		//经验条
		public UISlider	ExpValue;
		public UILabel	ExpRate;
		
		//称号
		public UIPopupList NickName;
		public UILabel	ZizhiName;
		
		//下排按钮
		public UIImageButton[] FuncBtn = new UIImageButton[RoleFuncBtnNum];
		public XActionIcon[] ActionIconArray = new XActionIcon[MAX_CHAR_ICON_NUM];
		public UIImageButton BtnLeft;
		public UIImageButton BtnRight;
		
		
		//时装显示勾选
		public UISprite FashionSel;
	}
	
	public GameObject[] GOFuncBtn = new GameObject[2];
	
	[System.Serializable]
	public class RoleAttr
	{
		public UILabel[] AttrArray = new UILabel[MAX_CHAR_ATTR_NUM];
		public UISprite[] AttrMarkSprite = new UISprite[2];
		public UISprite[] AttrSprite = new UISprite[5];
	}
	
	[System.Serializable]
	public class HuaLing
	{
		public UIImageButton	PeiYangBtn;
		public GameObject	GuideObj;
		public UIRadioButton TrainRadio;
		//public UILabel ComTrain;
		public UILabel TrainDes;
		public UILabel CurGrow;
		public UILabel NeedMoney;
		public UILabel NeedLevel;
		public UILabel CurClassLevel;
		public UILabel NextClassLevel;
		public UILabel NeedGrow;
		public UIImageButton HLBtn;
		public UILabel PeiYValue;
	}
	
	[System.Serializable]
	public class FuYin
	{
		public XActionIcon[] action = new XActionIcon[6];
		public UILabel[] attrName = new UILabel[16];
		public UILabel[] attrValue = new UILabel[16];
		
		public void Clear()
		{
			for (int i = 0; i < 16; i++) {
				attrName [i].text = "";
				attrValue [i].text = "";
			}
		}
	}
	
	[System.Serializable]
	public class LingDan
	{
		public UISlider[]	AttrSlider = new UISlider[4];
		public UILabel[] 	AttrValue = new UILabel[4];
		public UIImageButton[] AddBtn = new UIImageButton[4]; 
		public UIImageButton	RandomAddBtn;
	}
	
	
	public RoleInfoLeft UIRoleInfoLeft = new RoleInfoLeft ();
	public RoleAttr		UIRoleAttr = new RoleAttr ();
	public LingDan		UILingDan = new LingDan ();
	public HuaLing		UIHuaLing = new HuaLing ();
	public FuYin		UIFuYin = new FuYin ();
	
	public XActionIcon[] ItemSelIconList = new XActionIcon[MAX_ITEM_SEL_NUM];
	public UIImageButton LeftBtn;
	public UIImageButton RightBtn;
	public UILabel		 LabelPage;
	
	public override bool Init()
	{
		base.Init ();
		
		UIEventListener listenerL = UIEventListener.Get (UIRoleInfoLeft.BtnLeft.gameObject);
		listenerL.onClick += OnClickBtnLeft;
		listenerL.onPressing += OnPressingLeft;
		
		UIEventListener listenerR = UIEventListener.Get (UIRoleInfoLeft.BtnRight.gameObject);
		listenerR.onClick += OnClickBtnRight;
		listenerR.onPressing += OnPressingRight;
		
		UIEventListener listener1 = UIEventListener.Get (UIRoleInfoLeft.FuncBtn [0].gameObject);
		listener1.onClick += OnClickBagBtn;
		
		UIEventListener listener2 = UIEventListener.Get (UIRoleInfoLeft.FuncBtn [1].gameObject);
		listener2.onClick += OnClickSkillBtn;
		
		UIEventListener listener4 = UIEventListener.Get (UILingDan.AddBtn [0].gameObject);
		listener4.onClick += OnClickWuLi;
		
		UIEventListener listener5 = UIEventListener.Get (UILingDan.AddBtn [1].gameObject);
		listener5.onClick += OnClickLingQiao;
		
		UIEventListener listener6 = UIEventListener.Get (UILingDan.AddBtn [2].gameObject);
		listener6.onClick += OnClickTiZhi;
		
		UIEventListener listener7 = UIEventListener.Get (UILingDan.AddBtn [3].gameObject);
		listener7.onClick += OnClickFaShu;
		
		UIEventListener listener8 = UIEventListener.Get (UIHuaLing.HLBtn.gameObject);
		listener8.onClick += OnClickHL;
		
		UIEventListener listener9 = UIEventListener.Get (UIHuaLing.PeiYangBtn.gameObject);
		listener9.onClick += OnClickPeiY;
		
		for (int i = 0; i < PeyYangObj.Length; i++) {
			UIEventListener listener10 = UIEventListener.Get (PeyYangObj [i]);
			listener10.onClick += OnClickPeyYSelObj;
		}
		
		UIEventListener RandomAdd = UIEventListener.Get (UILingDan.RandomAddBtn.gameObject);
		RandomAdd.onClick += OnRandomAddLD;
		
		//称号
		UIRoleInfoLeft.NickName.onSelectionChange += OnSelectListData;
		UIRoleInfoLeft.NickName.eventReceiver = this.gameObject;
		
		// 时装
		UIEventListener listenerF = UIEventListener.Get (UIRoleInfoLeft.FashionSel.gameObject);
		listenerF.onClick += OnClickFashionState;
		
//		Vector3 showPos2 = new Vector3 (Mathf.Round (PeyYangObj [0].transform.localPosition.x - 146),
//				Mathf.Round (PeyYangObj [0].transform.localPosition.y),
//				Mathf.Round (PeyYangObj [0].transform.localPosition.z));
//		XNewPlayerGuideManager.SP.handleShowGuideExt ((int)XNewPlayerGuideManager.GuideType.Guide_PeiYang_Select, 
//			(int)XNewPlayerGuideManager.GuideType.Guide_PeiYang_Click, 1, showPos2, PeyYangObj [0].transform.parent.gameObject);
		
		UIEventListener leftPageBtnLS = UIEventListener.Get(LeftBtn.gameObject);
		leftPageBtnLS.onClick += OnClickLeftPageBtn;
		
		UIEventListener RightPageBtnLS = UIEventListener.Get(RightBtn.gameObject);
		RightPageBtnLS.onClick += OnClickRightPageBtn;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;	
		
		return true;
	}
	
	public override void Show ()
	{
		base.Show ();
		
		// 灵丹页签选择框
		if ( FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_lingdan) )
		{
			Vector3 showPos = new Vector3 (Mathf.Round (UserSelectAttrObj [1].transform.localPosition.x + 25),
				Mathf.Round (UserSelectAttrObj [1].transform.localPosition.y + 80), 
				Mathf.Round (UserSelectAttrObj [1].transform.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuideExt2((int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Select, (int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Use,
				2, showPos, UserSelectAttrObj[1].transform.parent.gameObject);
		}
		
		// 化灵页签选择框
		if ( FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_hualing) )
		{
			Vector3 showPos1 = new Vector3 (Mathf.Round (UserSelectAttrObj [2].transform.localPosition.x + 25),
				Mathf.Round (UserSelectAttrObj [2].transform.localPosition.y + 80), 
				Mathf.Round (UserSelectAttrObj [2].transform.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuideExt2((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Select, (int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Use,
				2, showPos1, UserSelectAttrObj[2].transform.parent.gameObject);
		}
		
		// 符印页签选择框
		if ( FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_fuyin) )
		{
			Vector3 showPos1 = new Vector3 (Mathf.Round (UserSelectAttrObj [3].transform.localPosition.x + 25),
				Mathf.Round (UserSelectAttrObj [3].transform.localPosition.y + 80), 
				Mathf.Round (UserSelectAttrObj [3].transform.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuideExt2((int)XNewPlayerGuideManager.GuideType.Guide_FuYin_Select, (int)XNewPlayerGuideManager.GuideType.Guide_FuYin_Equip,
				2, showPos1, UserSelectAttrObj[3].transform.parent.gameObject);
		}
	}
	
	public void Exit(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eRoleInformation);
	}
	
	public void OnClickLeftPageBtn(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.CharInfo_LeftPageBtn);
	}
	
	public void OnClickRightPageBtn(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.CharInfo_RightPageBtn);
	}
	
	private void OnSelectListData(string str)
	{
		XEventManager.SP.SendEvent(EEvent.CharInfo_NickNameSelect,str);
	}
	
	private void OnRandomAddLD(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.CharInfo_RandomAddLD);
	}
	
	private void OnClickPeyYSelObj(GameObject go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt ((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Click);
		
		// 新手引导
		Vector3 showPos = new Vector3(0, -70, 0);
		XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Use, 
			4, showPos, UIHuaLing.PeiYangBtn.gameObject);
	}
	
	public void ShowFuncSel(int index)
	{
		for (int i = 0; i < MAX_FUNC_SEL_NUM; i++) {
			if (i == index)
				GOFuncBtn [i].gameObject.SetActive (true);
			else
				GOFuncBtn [i].gameObject.SetActive (false);
		}
	}
	
	void OnPressingLeft(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_TurnLeft, EUIPanel.eRoleInformation);
	}
	
	void OnPressingRight(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_TurnRight, EUIPanel.eRoleInformation);
	}
	
	void OnClickBtnLeft(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_TurnLeft, EUIPanel.eRoleInformation);
	}
	
	void OnClickFashionState(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.CharInfo_FashionChange, EUIPanel.eRoleInformation);
	}
	
	void OnClickBtnRight(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_TurnRight, EUIPanel.eRoleInformation);
	}
	
	void OnClickBagBtn(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_BottomBtn1);
	}
	
	void OnClickSkillBtn(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_BottomBtn2);
	}
	
	void OnClickFoodBtn(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_BottomBtn3);
	}
	
	void OnClickWuLi(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_WuLi);
	}
	
	void OnClickLingQiao(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_LingQiao);
	}
	
	void OnClickTiZhi(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_TiZhi);
	}
	
	void OnClickFaShu(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_FaShu);
	}
	
	void OnClickHL(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_HL);
	}
	
	void OnClickPeiY(GameObject go)
	{
		XEventManager.SP.SendEvent (EEvent.CharInfo_PY);
	}
}
