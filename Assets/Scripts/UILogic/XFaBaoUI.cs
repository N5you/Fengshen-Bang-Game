using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UILogic/XMountUI")]
public class XFaBaoUI : XDefaultFrame
{
	
	public class ZuoQiSelectItem
	{
		public UISprite SelSprite;
		private int _pos = -1;
		private XFaBaoUI _parent;
		public static int CurrentSelPos = -1;
		public ZuoQiSelectItem(XFaBaoUI parent, UISprite sprite)
		{
			SelSprite = sprite;
			_parent = parent;
			
			UIEventListener lis = UIEventListener.Get(SelSprite.gameObject);
			lis.onClick += onclick;
		}
		
		public void SetPos(int pos)
		{
			_pos = pos;
		}
		
		private void onclick(GameObject go)
		{
			CurrentSelPos = _pos;
			
			_parent.ShowMountModel(_pos);
		}
	}
	
	// 坐骑
	public GameObject ZuoQiPanel;
	
	public GameObject LevelPanel;
	public UILabel Label_Mount_Level;						// 坐骑等级
	
	public UILabel Label_Mount_PeiYangCount;				// 可培养次数
	public GameObject Obj_NormalMountPeiYang;				// 普通培养按钮
	public GameObject Obj_SuperMountPeiYang;				// 超级培养按钮
	
	public GameObject BaoJiPanel;							
	public UILabel Label_exp;
	
	public GameObject NormalPanel;							
	public UILabel LabelNormal_exp;
	
	public GameObject MountExpPanel;
	public UISlider MountExpSlider;							// 经验条
	public UILabel MountExpLabel;							// 经验值
	
	public GameObject JiaChengPanbel;
	public UILabel[] Label_WuLi = new UILabel[2];			// 武力
	public UILabel[] Label_LingQiao = new UILabel[2];		// 灵巧
	public UILabel[] Label_TiZhi = new UILabel[2];			// 体质
	public UILabel[] Label_ShuFa = new UILabel[2];			// 术法
	public UILabel[] Label_TongLing = new UILabel[2];		// 统领
	public UILabel[] Label_SuDu = new UILabel[2];			// 速度
	
	public GameObject HuanHua;								// 幻化按钮
	public GameObject QiCheng;								// 骑乘按钮
	public GameObject JieCheng;								// 解乘按钮
	
	public GameObject ZuoQiObj;
	public GameObject ZuoQiBtnDown;
	public GameObject ZuoQiBtnUp;
	
	public ZuoQiSelectItem[] ZuoQiItems = new ZuoQiSelectItem[6];
	public UISprite[] bk = new UISprite[6];
	public UISprite[] zqsprite = new UISprite[6];
	
	public UITexture ObjectModelParent;
	private XObjectModel m_objectModel = null;
	private SingleModelRTT m_rtt = null;
	
	struct ExpStruct
	{
		public int expType;
		public int exp;
	}
	private LinkedList<ExpStruct> m_allBaoJiShowValue = new LinkedList<ExpStruct>();
	
	// 法宝
	public GameObject FaBaoPabel;
	
	public UISpriteGroup ZuoQiGroup;
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener lis = UIEventListener.Get(Obj_NormalMountPeiYang);
		lis.onClick += OnClickNormalMountPeiYang;
		
		lis = UIEventListener.Get(Obj_SuperMountPeiYang);
		lis.onClick += OnClickSuperMountPeiYang;
		
		lis = UIEventListener.Get(HuanHua);
		lis.onClick += OnClickHuanHua;
		
		lis = UIEventListener.Get(QiCheng);
		lis.onClick += OnClickQiCheng;
		
		lis = UIEventListener.Get(JieCheng);
		lis.onClick += OnClickJieCheng;
		
		lis = UIEventListener.Get(ZuoQiBtnDown);
		lis.onClick += OnClickShowZuoQiDown;
		
		lis = UIEventListener.Get(ZuoQiBtnUp);
		lis.onClick += OnClickShowZuoQiUp;
		
		for ( int i = 0; i < 6; i++ )
		{
			ZuoQiItems[i] = new ZuoQiSelectItem(this, bk[i]);
		}
		
		LevelPanel.SetActive(false);
		BaoJiPanel.SetActive(false);
		MountExpPanel.SetActive(false);
		JiaChengPanbel.SetActive(false);
		HuanHua.SetActive(false);
		QiCheng.SetActive(false);
		JieCheng.SetActive(false);
		Obj_NormalMountPeiYang.SetActive(false);
		Obj_SuperMountPeiYang.SetActive(false);
		ZuoQiObj.SetActive(false);
		
		ObjectModelParent.transform.localScale = new Vector3(250f, 250f, 1f);
		ObjectModelParent.transform.localPosition = new Vector3(ObjectModelParent.transform.localPosition.x, 
			60f, ObjectModelParent.transform.localPosition.z);
		
		m_objectModel = new XObjectModel();
		m_objectModel.mainModelLoaded += loadModelFinish;
		m_objectModel.mountLoaded += loadMountFinish;
		
		m_objectModel.HandleEvent(EModelEvent.evtModelId, XLogicWorld.SP.MainPlayer.ModelId);

		ZuoQiGroup.SetSelect(0, false);
		
		TweenScale scaleEffect = BaoJiPanel.GetComponent<TweenScale>();
		if ( scaleEffect )
		{
			scaleEffect.onFinished += ScaleFinished;
		}
		
		scaleEffect = NormalPanel.GetComponent<TweenScale>();
		if ( scaleEffect )
		{
			scaleEffect.onFinished += ScaleFinished;
		}
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return true;
	}
	
	public override void Show()
    {
        base.Show();
		
		if ( ZuoQiSelectItem.CurrentSelPos > 0 )
			ShowMountModel(ZuoQiSelectItem.CurrentSelPos);
		
		Vector3 showPos = new Vector3(0, -72, Mathf.Round(Obj_NormalMountPeiYang.transform.localPosition.z));
		XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_StartPeiYang, 2, showPos, Obj_NormalMountPeiYang);
    }
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFaBao);
	}

    public override void Hide()
    {
        base.Hide();
    }
	
	private void OnClickNormalMountPeiYang(GameObject go)
	{
		XMountManager.SP.MountPeiYang(PeiYangType.PeiYang_Normal);
		
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_StartPeiYang);
	}
	
	private void OnClickSuperMountPeiYang(GameObject go)
	{
		XMountManager.SP.MountPeiYang(PeiYangType.PeiYang_Super);
	}
	
	private void OnClickHuanHua(GameObject go)
	{
		if ( ZuoQiSelectItem.CurrentSelPos == XMountManager.SP.m_nEquipMount )
			return;
		
		XMountManager.SP.EquipMount(ZuoQiSelectItem.CurrentSelPos, true);
		
		QiCheng.gameObject.SetActive(false);
		JieCheng.gameObject.SetActive(true);
	}
	
	private void OnClickQiCheng(GameObject go)
	{
		if ( XMountManager.SP.m_nPrevEquipMount <= 0 )
			return;
		
		XMountManager.SP.EquipMount(XMountManager.SP.m_nPrevEquipMount, true);
		
		QiCheng.gameObject.SetActive(false);
		JieCheng.gameObject.SetActive(true);
	}
	
	private void OnClickJieCheng(GameObject go)
	{
		if ( XMountManager.SP.m_nEquipMount <= 0 )
			return;
		
		XMountManager.SP.EquipMount(0, true);
		
		QiCheng.gameObject.SetActive(true);
		JieCheng.gameObject.SetActive(false);
	}
	
	public void HideBaojiPanel()
	{
		//ShowBaoJi();
	}
	
	private void OnClickShowZuoQiUp(GameObject go)
	{
		XMountManager.SP.ShowNextPage(1);
	}
	
	private void OnClickShowZuoQiDown(GameObject go)
	{
		XMountManager.SP.ShowNextPage(-1);
	}
	
	public void ShowMountModel(int pos)
	{
		ushort index = XMountManager.SP.GetMountModeId(pos);
		if ( 0 == index )
			return;
		m_objectModel.HandleEvent(EModelEvent.evtMountIndex, index);
	}
	
	public void loadModelFinish (XObjectModel model)
	{
		m_rtt = XModelRTTMgr.SP.AddObjectRTT(m_objectModel.mainModel, ObjectModelParent, 1.5f, new Vector3(0f, -1.5f, 2f), 
			Vector3.zero, Vector3.one, false);
		
		if ( XMountManager.SP.m_nEquipMount > 0 )
		{
			ShowMountModel(XMountManager.SP.m_nEquipMount);
		}
	}
	
	void Update()
	{
		if ( m_objectModel.mountModel != null && m_objectModel.mountModel.LocalPosition.x < 0 )
		{
			m_objectModel.mountModel.LocalPosition = new Vector3(0, -1.5f, 2f);
		}
	}
	
	public void loadMountFinish(XU3dModel mountModel)
	{
		if ( null == mountModel )
			return;
		
		ObjectModelParent.transform.localScale = new Vector3(280f, 280f, 1f);
		
		float defaultSize = 2f;
		float defaultY = 40f;
		if ( mountModel.m_mountModelType == XU3dModel.EMountModelType.eMountZhan )
		{
			defaultSize = 3f;
			defaultY = -12f;
		}
		else if ( mountModel.m_mountModelType == XU3dModel.EMountModelType.eMountFei )
		{
			defaultY = 0f;
		}
		
		ObjectModelParent.transform.localPosition = new Vector3(ObjectModelParent.transform.localPosition.x, 
			defaultY, ObjectModelParent.transform.localPosition.z);
		
		m_rtt.ShowObjModel(mountModel, ObjectModelParent, defaultSize, new Vector3(0f, -1.5f, 2f), new Vector3(0f, 300f, 0f), Vector3.one, false);
	}
	
	public void ShowExpAnima(int exptype, int exp)
	{
		ExpStruct t = new ExpStruct();
		t.expType = exptype;
		t.exp = exp;
		m_allBaoJiShowValue.AddLast(t);
		
		ShowExp();
	}
	
	private void ShowExp()
	{
		if ( m_allBaoJiShowValue.Count > 0 )
		{
			GameObject go = null;
			UILabel label = null;
			if ( 1 ==  m_allBaoJiShowValue.First.Value.expType )
			{
				BaoJiPanel.SetActive(true);
				NormalPanel.SetActive(false);
				go = BaoJiPanel;
				label = Label_exp;
			}
			else
			{
				BaoJiPanel.SetActive(false);
				NormalPanel.SetActive(true);
				go = NormalPanel;
				label = LabelNormal_exp;
			}
			if ( 1 ==  m_allBaoJiShowValue.First.Value.expType )
				label.text = "+ " + m_allBaoJiShowValue.First.Value.exp.ToString();
			else
				label.text = m_allBaoJiShowValue.First.Value.exp.ToString();
			
			m_allBaoJiShowValue.RemoveFirst();
			TweenScale scaleEffect = go.GetComponent<TweenScale>();
			if(scaleEffect != null)
			{
				scaleEffect.Reset();
				scaleEffect.enabled	= true;
			}
		}
		else
		{
			BaoJiPanel.SetActive(false);
			NormalPanel.SetActive(false);
		}
	}
	
	private void ScaleFinished(UITweener tween)
	{
		ShowExp();
	}
}
