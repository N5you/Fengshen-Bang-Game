using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XProductUI")]
public class XProductUI : XDefaultFrame
{
	// 采集物显示UI
	private class XCollectionItemInfo : XBaseActionIcon
	{
		private bool m_bEnable = false;						// 是否已采集过，如果以采集过，可以进行自动寻路
		private UILabel GatherInfoLabel;					// 采集物名称显示UI
		private uint expectScenseid = 0;					// 采集物所在场景ID
		private Vector3 expectPos;							// 采集物坐标
		
		public XCollectionItemInfo(UILabel label, XActionIcon actionIcon)
		{
			expectPos = new Vector3(0, 0, 0);
			IsCanDrag = false;
			IsCanDrop = false;
			IsCanPopMenu = false;
			GatherInfoLabel = label;
			SetUIIcon(actionIcon);
			UIEventListener.Get(actionIcon.gameObject).onClick += onNavigation;
		}
		
		// 设置采集物显示信息
		public void setGatherInfo(bool collected, uint itemId, XCfgItem cfgItem, uint scenseid, Vector3 pos)
		{
			m_bEnable = collected;
			SetSprite(cfgItem.IconAtlasID, cfgItem.IconID, (EItem_Quality)(cfgItem.QualityLevel), 1);
			GatherInfoLabel.text = XGameColorDefine.Quality_Color[cfgItem.QualityLevel] + cfgItem.Name;
			IconData = (int)itemId;
			expectScenseid = scenseid;
			expectPos.Set(pos.x, pos.y, pos.z);
			
			// 变色处理
			if ( m_bEnable )
				mUIIcon.ItemIcon.color = Color.white;				
			else
				mUIIcon.ItemIcon.color = Color.gray;
		}
		
		// 设置采集物是否可寻路
		public void Enable()
		{
			m_bEnable = true;
			mUIIcon.ItemIcon.color = Color.white;
		}
		
		// 寻路
		private void onNavigation(GameObject _go)
		{
			if( !m_bEnable ) 
				return;
			if ( UICamera.currentTouchID == -1)
			{
				XLogicWorld.SP.MainPlayer.NavigateTo(new XMainPlayerStateNavigate.NavigateInfo(expectScenseid, EObjectType.GatherObject, (int)IconData, expectPos));
				Debug.Log("auto move over scene");
			}
		}
		
		// 隐藏，显示当前采集物信息
		public void Show(bool active)
		{
			GatherInfoLabel.gameObject.SetActive(active);
			mUIIcon.gameObject.SetActive(active);
		}
	}
	
	private class XFormulaItem
	{
		public GameObject itemObject;
		public XCfgFormula cfgFormula;
		public XProductUI productUI;
		
		public XFormulaItem(XCfgFormula cfg, GameObject go, XProductUI ui)
		{
			cfgFormula = cfg;
			itemObject = go;
			productUI = ui;
			NGUITools.AddWidgetCollider(itemObject, true);
			UIEventListener lis = UIEventListener.Get(itemObject);
			lis.onClick += onClick;
		}
		
		private void onClick(GameObject go)
		{
			productUI.onSelectFormulaItem(cfgFormula);
		}
	}
	
	private class XFormulaList
	{
		public static UITree rootTree;

		private GameObject listObject;
		private SortedList<uint, XFormulaItem> formulaList;

		public XFormulaList(string name)
		{
			listObject = rootTree.insertNode(name);
			formulaList = new SortedList<uint, XFormulaItem>();
		}
		
		public void InsertFormula(XCfgFormula cfg, XProductUI productUI)
		{
			if ( formulaList.ContainsKey(cfg.ID) ) 
				return;
			
			string color = "";
			string spriteName = "";
			string itemName = "";
			getColorSprite(cfg, out color, out spriteName, out itemName);
			GameObject itemObject = rootTree.insertItem(color + cfg.Name, listObject);
			itemObject.name = itemName;
			
			for ( int i=0; i<itemObject.transform.childCount; i++ )
			{
				UISprite sprite = itemObject.transform.GetChild(i).GetComponent<UISprite>();
				if ( sprite && "CenterPoint" == sprite.name )
				{
					sprite.spriteName = spriteName;
					break;
				}
			}
			formulaList.Add(cfg.ID, new XFormulaItem(cfg, itemObject, productUI));
		}
		
		public void updateBKShow(XCareerInfo info)
		{
			foreach( KeyValuePair<uint, XFormulaItem> item in formulaList )
			{
				GameObject itemObject = item.Value.itemObject;
				string color = "";
				string spriteName = "";
				string itemName = "";
				getColorSprite(item.Value.cfgFormula,out color, out spriteName, out itemName);
				for( int i=0; i<itemObject.transform.childCount; i++ )
				{
					while ( true )
					{
						UISprite sprite = itemObject.transform.GetChild(i).GetComponent<UISprite>();
						if ( sprite && "CenterPoint" == sprite.name )
						{
							sprite.spriteName = spriteName;
							break;
						}
						UILabel label = itemObject.transform.GetChild(i).GetComponent<UILabel>();
						if ( label )
						{
							label.text = color + item.Value.cfgFormula.Name;
							break;
						}
						break;
					}
					
				}
			}
		}
		
		void getColorSprite(XCfgFormula cfg, out string color, out string spriteName, out string itemName)
		{
			color = "";
			spriteName ="";
			itemName = "";
			
			// 排序用
			int plusExpCount = cfg.Exp.Length;
			if ( plusExpCount < 3 )
				return;
			
			int exp = 10000 - cfg.LearnExp;
			int plus3 = 10000 - cfg.Exp[2]; 
			int plus2 = 10000 - cfg.Exp[1];
			int plus1 = 10000 - cfg.Exp[0];
			

			XCareerInfo careerInfo = XProductManager.SP.GetCareerInfo(1);
			if ( careerInfo.Exp < cfg.Exp[2] )
			{
				color = "[color=FEBF02]";								// 金色
				spriteName ="11000503";
			}
			else if ( cfg.Exp[2] <= careerInfo.Exp && cfg.Exp[1] > careerInfo.Exp )
			{
				color = "[color=FFF337]";								// 橙色
				spriteName ="11000502";
			}
			else if ( cfg.Exp[1] <= careerInfo.Exp && cfg.Exp[0] > careerInfo.Exp )
			{
				color = "[color=00FF00]";								// 绿色
				spriteName ="11000501";
			}
			else if ( careerInfo.Exp >= cfg.Exp[0] )
			{
				color = "[color=ACA48D]";								// 灰色
				spriteName ="11000500";
			}
			
			itemName = exp.ToString()+ "_" + plus3.ToString() + "_" + plus2.ToString() + "_"  + plus1.ToString()+ cfg.Name;
		}
		
	}
	
	private class XFormulaTgtIcon : XBaseActionIcon
	{
		private GameObject gameObject;
		private UILabel ItemName;
		private UILabel ItemCount;
		private UILabel ItemExp;
		private UILabel ItempTili;
		
		private XCfgFormula cfgFormula;
		private XCfgItem cfgItem;
		
		public XFormulaTgtIcon(GameObject go, UILabel labelName, UILabel labelCount, UILabel labelExp, UILabel labelTili)
		{
			IsCanDrag = false;
			IsCanDrop = false;
			IsCanPopMenu = false;

			gameObject = go;
			SetUIIcon(gameObject.GetComponent<XActionIcon>());
			ItemName = labelName;
			ItemCount = labelCount;
			ItemExp = labelExp;
			ItempTili = labelTili;
			
			Clear();
		}

		public void Clear()
		{
			cfgFormula = null;
			cfgItem = null;
			gameObject.SetActive(false);
			ItemName.gameObject.SetActive(false);
			ItemCount.gameObject.SetActive(false);
			ItemExp.gameObject.SetActive(false);
			ItempTili.gameObject.SetActive(false);
		}
		
		public void SetItem(XCfgFormula cfg)
		{
			if(null == cfg) 
				return;
			if(0 == cfg.OutputItemId) 
				return;
			
			IconData = (int)(cfg.OutputItemId);
			cfgItem = XCfgItemMgr.SP.GetConfig(cfg.OutputItemId);
			if(null == cfgItem)
				return;
			cfgFormula = cfg;
			SetSprite(cfgItem.IconAtlasID, cfgItem.IconID, (EItem_Quality)(cfgItem.QualityLevel), 1);
			ItemName.text =  XGameColorDefine.Quality_Color[cfgItem.QualityLevel] + cfgItem.Name;
			ItempTili.text = string.Format(XStringManager.SP.GetString(289u), cfg.CostStrength);
			ItemCount.text = "x" + cfg.OutputItemNum;
			UpdateTargetMulInfo();
			
			
			gameObject.SetActive(true);
			ItemName.gameObject.SetActive(true);
			ItemCount.gameObject.SetActive(true);
			ItemExp.gameObject.SetActive(true);
			ItempTili.gameObject.SetActive(true);
		}
		
		public void SetMul(int nMul)
		{
			if(null == cfgItem || null == cfgFormula)
				return;
			
			// ItemDis.text = cfgItem.Name + "x" + (cfgFormula.OutputItemNum * nMul);
		}
		
		public void UpdateTargetMulInfo()
		{
			string color = "";
			XCareerInfo careerInfo = XProductManager.SP.GetCareerInfo(1);
			if ( cfgFormula == null || careerInfo == null )
				return;
			if ( careerInfo.Exp < cfgFormula.Exp[2] )
			{
				color = "[color=FEBF02]";								// 金色
			}
			else if ( cfgFormula.Exp[2] <= careerInfo.Exp && cfgFormula.Exp[1] > careerInfo.Exp )
			{
				color = "[color=FFF337]";								// 橙色
			}
			else if ( cfgFormula.Exp[1] <= careerInfo.Exp && cfgFormula.Exp[0] > careerInfo.Exp )
			{
				color = "[color=00FF00]";								// 绿色
			}
			else if ( careerInfo.Exp >= cfgFormula.Exp[0] )
			{
				color = "[color=ACA48D]";								// 灰色
			}
			
			int exp = 0;
			if ( careerInfo.Exp > cfgFormula.Exp[0]  )
				exp = 0;
			else if ( careerInfo.Exp >= cfgFormula.Exp[1] )
				exp = 1;
			else if ( careerInfo.Exp >= cfgFormula.Exp[2])
				exp = 3;
			else
				exp = 5;
			
			ItemExp.text = color + string.Format(XStringManager.SP.GetString(288), exp);
		}
	}
	
	private class XFormulaMatIcon : XBaseActionIcon
	{
		private GameObject gameObject;
		private UILabel ItemDis;
		private UILabel ItemCount;
		private XCfgItem cfgItem;
		private uint nItemId;
		private int nCurItemNum;
		private ushort nNeedItemNum;
		private int	nCurMul;
		
		public XFormulaMatIcon(GameObject sam, GameObject parent, uint itemId, ushort needItemNum)
		{
			IsCanDrag = false;
			IsCanDrop = false;
			IsCanPopMenu = false;
			
			gameObject = XUtil.Instantiate(sam);
			gameObject.SetActive(true);
			gameObject.transform.parent = parent.transform;
			GameObject actionObject = gameObject.transform.Find("Matier_Icon").gameObject;
			SetUIIcon(actionObject.GetComponent<XActionIcon>());
			ItemDis = gameObject.transform.Find("Label_Name").GetComponent<UILabel>();
			ItemCount = gameObject.transform.Find("Label_Count").GetComponent<UILabel>();
			
			nItemId = itemId;
			nNeedItemNum = needItemNum;
			nCurMul = 1;
			nCurItemNum = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(itemId);
			IconData = (int)itemId;
			
			cfgItem = XCfgItemMgr.SP.GetConfig(nItemId);
			if(null != cfgItem)
				SetSprite(cfgItem.IconAtlasID, cfgItem.IconID, (EItem_Quality)(cfgItem.QualityLevel), 1);

			refreshItemDis();
		}
		
		public void SetMul(int nMul)
		{
			if( nMul < 0 || nMul ==nCurMul ) 
				return;
			nCurMul	= nMul;
			refreshItemDis();
		}
		
		public void OnItemNumChanged()
		{
			nCurItemNum = XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(nItemId);
			refreshItemDis();
		}
		
		public bool IsOk { get { return nNeedItemNum < nCurItemNum * nCurMul; }}
		
		private void refreshItemDis()
		{
			if(null == cfgItem) 
				return;
			
			if( nCurItemNum < nNeedItemNum )
			{
				ItemCount.text = "[color=ff0000]";
			}
			else
			{
				ItemCount.text = "[color=FFF337]";
			}
			ItemCount.text += nCurItemNum + "[color=FFF337]/" + nNeedItemNum;
			ItemDis.text = XGameColorDefine.Quality_Color[cfgItem.QualityLevel] + cfgItem.Name;
		}
		
		public void Destroy()
		{
			// 特殊处理
			gameObject.transform.parent = gameObject.transform.parent.parent;
			GameObject.Destroy(gameObject);
		}
	}
	
	//=================================================================================================
	public UIRadioButton Tab_Radio_UserSel;						// 标签页的单选框
	
	public UIRadioButton Learn_Radio_CareerType;				// 学习面板上的单选框(选择一个专业进行学习)
	
	public GameObject Object_Learn_Display_Panel;				// 生产系统机能学习界面
	public GameObject Object_Product_Displya_Panel;				// 生产系统机能展示界面
	
	public UISprite Sprite_Collection_Icon;						// 生产系统采集机能图标
	public UILabel	Label_Collection_xuexi;						// 显示“学习一个采集机能”控件
	public UILabel	Label_Collection_Name;						// 显示“技能名称”控件
	public UILabel	Label_Collection_Level;						// 显示“技能等级”控件
	
	public UISprite Sprite_Manufacture_Icon;					// 生产系统制造机能图标
	public UILabel	Label_Manufacture_xuexi;					// 显示“学习一个制造机能”控件
	public UILabel	Label_Manufacture_Name;						// 显示“制造名称”控件
	public UILabel	Label_Manufacture_Level;					// 显示“制造等级”控件
	
	public GameObject[] LearnObj;
	public UILabel	   LearnButtonTip;
	public UISprite[] Sprite_Career_Icon;						// 技能图标控件
	public UISprite[] Sprite_Career_ReaIcon;					// 相关技能图标
	public UILabel[] Sprite_Career_ReaIName;					// 相关技能名称
	public UILabel[]  Label_Career_Description;					// 技能描述
	public UILabel[]  Label_Career_Name;						// 技能名称
	public UILabel[]  Label_Career_RealName;					// 相关技能名称
	public GameObject Learn_Button;								// 学习按钮
	
	// 体力值
	public UISlider		Dp_Slider_Strength;			// 体力条
	public UILabel		Dp_Label_Strength;			// 体力值
	
	// 通用按钮
	public GameObject		Dp_Gather_IconUp;			// 向上翻页
	public GameObject		Dp_Gather_IconDown;			// 向下翻页
	public GameObject 		Dp_Button_Forget;			// 忘却按钮
	public UIImageButton	Dp_Button_Upgrade;			// 升级按钮
	
	// 显示采集物品
	public GameObject Object_Collection_Panel;
	public GameObject Object_manufacture_Panel;
	SortedList<int, XCfgGatherObjectBorn> m_AllGatherObjectBorn = new SortedList<int, XCfgGatherObjectBorn>();			// 所有物品
	private SortedList<uint, bool>	m_allMaterierCollected;		// 物品是否已采集
	private int m_curGatherShowPoint;							// 当前展示游标标记
	public UISprite[] Sprite_Sense_Icon;						// 采集物品所在场景图标 
	public UILabel[] Sprite_Sense_Name;							// 采集物品所在场景名称
	public XActionIcon[] Sprite_material_Icon;					// 采集到的材料图标
	public UILabel[] Sprite_material_Name;						// 采集到的材料名称
	private XCollectionItemInfo[] Collection_items;				// 采集物显示UI
	public UISprite[]  Sprite_Collection_Split;					// 采集物分割线
	
	
	// 显示制造物品
	// 所有配方Item
	private SortedList<string, XFormulaList> m_AllFormulaList = new SortedList<string, XFormulaList>();
	private XCfgFormula m_curCfgFormula = null;		// 当前选中配方
	private int m_curProduceMul = 1;				// 当前制造个数
	private XFormulaTgtIcon m_formulaTgtIcon;		// 包装配方目标产品的Icon
	private SortedList<uint, XFormulaMatIcon> m_AllFormulaMatIcon = new SortedList<uint, XFormulaMatIcon>();	// 所有配方需要材料的icon
	
	public UISlider			Dp_Slider_Exp;					// 专业经验条
	public UILabel  		Dp_Label_Exp;					// 专业经验值
	public GameObject		Dp_Produce_ButtonStart;			// 开始制作
	public GameObject		Dp_Produce_ButtonStartMax;		// 全部制作
	public GameObject		Dp_Produce_AddNum;				// 增加制作数量
	public GameObject		Dp_Produce_DecNum;				// 减少制作数量
	public UITree			Dp_Produce_FormulaTree;			// 所有配方展开树
	public GameObject       DP_Produce_Scroll_Up;			// 向上滚动按钮
	public GameObject       DP_Produce_Scroll_Down;			// 向下滚动按钮
	public UIScrollBar 		DP_Produce_ScrollBar;			// 配方树滚动条
	
	public GameObject		Dp_Produce_TargetIcon;			// 配方目标产品
	public UILabel			Dp_Produce_TargetName;			// 配方目标产品名称
	public UILabel			Dp_Produce_TargetCount;			// 配方目标产品名称
	public UILabel			Dp_Produce_TargetExp;			// 配方目标产品名称
	public UILabel			Dp_Produce_TargetTiLi;			// 配方目标产品名称
	public GameObject		Dp_ActionIcon_Sample;			// 材料图标样本	
	public UIGrid			Dp_Produce_GridMaterail;		// 配方所需要的所有材料格子
	
	public UIInput			Dp_Produce_PNum;				// 制造数量
	
	public UITable			TAB_PANEL;		
	
	private bool loadFinish = false;
	private bool firstOpen = false;
		
	public override bool Init()
	{
		base.Init ();
		
		Dp_Produce_FormulaTree.SetScrollBar(DP_Produce_ScrollBar);
		
		int showItemCount = Sprite_material_Name.Length;
		m_allMaterierCollected = new SortedList<uint, bool>();
		Collection_items = new XCollectionItemInfo[showItemCount];
		for (int i = 0; i < showItemCount; i++)
		{
			Collection_items[i] = new XCollectionItemInfo(Sprite_material_Name[i], Sprite_material_Icon[i]);
		}
		
		Tab_Radio_UserSel.onRadioChanged += new UIRadioButton.OnRadioChanged(onProductTypeChanged);
		Learn_Radio_CareerType.onRadioChanged += new UIRadioButton.OnRadioChanged(DisplayLearnPanel);
		
		for ( int i = 0; i < LearnObj.Length; i++ )
		{
			UIEventListener.Get(LearnObj[i]).onClick += onClickLearnItem;
		}
		// Learn_Radio_CareerType.onClickRadioItem += new UIRadioButton.OnClickRadioItem(onClickLearnItem);
		
		UIEventListener lis = UIEventListener.Get(Learn_Button);
		lis.onClick += onLearnButtonClick;
		
		lis = UIEventListener.Get(Dp_Gather_IconUp);
		lis.onClick += onCollectionShowUpClick;
		
		lis = UIEventListener.Get(Dp_Gather_IconDown);
		lis.onClick += onCollectionShowDownClick;
		
		lis = UIEventListener.Get(Dp_Button_Forget);
		lis.onClick += onForgetButtonClick;
		
		lis = UIEventListener.Get(Dp_Button_Upgrade.gameObject);
		lis.onClick += onUpgradeButtonClick;
		
		lis = UIEventListener.Get(DP_Produce_Scroll_Up);
		lis.onClick += onProduceScrollUpClick;
		
		lis = UIEventListener.Get(DP_Produce_Scroll_Down);
		lis.onClick += onProduceScrollDownClick;
		
		lis = UIEventListener.Get(Dp_Produce_AddNum);
		lis.onClick += onProduceAddNumClick;
		
		lis = UIEventListener.Get(Dp_Produce_DecNum);
		lis.onClick += onProduceDecNumClick;
		
		lis = UIEventListener.Get(Dp_Produce_ButtonStart);
		lis.onClick += onProduceButtonClick;
		
		lis = UIEventListener.Get(Dp_Produce_ButtonStartMax);
		lis.onClick += onProduceButtonMaxClick;
		
		lis = UIEventListener.Get(Dp_Produce_PNum.gameObject);
		lis.onSelect += OnToggleChat;
		
		XFormulaList.rootTree = Dp_Produce_FormulaTree;	

		m_formulaTgtIcon = new XFormulaTgtIcon(Dp_Produce_TargetIcon, Dp_Produce_TargetName,
			Dp_Produce_TargetCount, Dp_Produce_TargetExp, Dp_Produce_TargetTiLi);
		
		return true;
	}
	
	public override void Show()
	{		
		base.Show();
		
		InitCurrLearn(1);
		InitCurrLearn(0);
		
		loadFinish = true;
	}
	
	public override void Close()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eProduct);
	}
	
	
	public void OnToggleChat(GameObject go, bool bSelected)
	{
		XEventManager.SP.SendEvent(EEvent.Chat_ToggleChat, bSelected);
	}
	
	public void InitCurrLearn(int index)
	{
		byte btType = (byte)index;
		XCareerInfo info = XProductManager.SP.GetCareerInfo(btType);
		if(null == info)
		{
			ShowLearnPanel(index);
		}
		else
		{
			ShowProductDisplayPanel(index, info);
			
			refreshExp(XCfgProductCareerMgr.SP.GetConfig(info.CareerId, info.Level), info);
		}
		
		if ( 1 == index 
			&& XProductManager.SP.GetCareerInfo(0) != null
			&& XProductManager.SP.GetCareerInfo(1) == null )
		{
			GameObject makeItemGo = Tab_Radio_UserSel.transform.Find("CheckBox_Manufacture").gameObject;
			Vector3 showPos = new Vector3(Mathf.Round(makeItemGo.transform.localPosition.x - 542),
				Mathf.Round(makeItemGo.transform.localPosition.y + 184),
				Mathf.Round(makeItemGo.transform.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuideExt((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMake, 
				(int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeLearn, 1, showPos, makeItemGo);
		}
		if ( 0 == index && null == XProductManager.SP.GetCareerInfo(0) )
		{
			Vector3 showPos = new Vector3(Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.x + 340),
				Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.y - 30),
				Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuideExt((int)XNewPlayerGuideManager.GuideType.Guide_Product_Select, 
					(int)XNewPlayerGuideManager.GuideType.Guide_Product_Learn, 1, showPos, Object_Learn_Display_Panel);
		}
	}
	
	// 采集，制造面板切换
	private void onProductTypeChanged(int nIndex)
	{
		byte btType = (byte)nIndex;
		XCareerInfo info = XProductManager.SP.GetCareerInfo(btType);
		if(null == info)
		{
			ShowLearnPanel(nIndex);
		}
		else
		{
			ShowProductDisplayPanel(nIndex, info);
			
			refreshExp(XCfgProductCareerMgr.SP.GetConfig(info.CareerId, info.Level), info);
		}
		
		if ( 1 == nIndex )
		{
			XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMake);
			
			Vector3 showPos = new Vector3(Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.x + 340),
			Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.y - 30),
			Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuideExt2((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeItem, 
				(int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeLearn, 2, showPos, Object_Learn_Display_Panel.gameObject);
		}
	}
	
	// 向下显示采集物
	private void onCollectionShowUpClick(GameObject go)
	{
		if(0 >= m_curGatherShowPoint) 
			return;
		
		m_curGatherShowPoint--;
		ShowCurrentCollection();
	}
	
	private void onProduceScrollUpClick(GameObject go)
	{
		DP_Produce_ScrollBar.scrollValue -= 0.1f;
	}
	
	private void onProduceScrollDownClick(GameObject go)
	{
		DP_Produce_ScrollBar.scrollValue += 0.1f;
	}
	
	// 向上显示采集物
	private void onCollectionShowDownClick(GameObject go)
	{
		int totalCount = m_AllGatherObjectBorn.Count;
		if ( 3 * (m_curGatherShowPoint + 1) >= totalCount )
			return;
		
		m_curGatherShowPoint++;
		ShowCurrentCollection();
	}
	
	// 配方制造+1
	private void onProduceAddNumClick(GameObject go)
	{
		refreshProduceMulNum(m_curProduceMul + 1);
	}
	
	// 配方制造-1
	private void onProduceDecNumClick(GameObject go)
	{
		refreshProduceMulNum(m_curProduceMul - 1);
	}
	
	// 进行配方制造
	private void onProduceButtonClick(GameObject go)
	{
		if(null == m_curCfgFormula || 0 >= m_curProduceMul ) 
			return;
		int maxnum = 0;
		XProductManager.SP.ApplyProduceFormula(m_curCfgFormula, m_curProduceMul,out maxnum);
	}
	
	// 制造所有配方
	private void onProduceButtonMaxClick(GameObject go)
	{
		if(null == m_curCfgFormula) 
			return;
		int maxnum = 0;
		XProductManager.SP.ApplyProduceFormula(m_curCfgFormula, 0, out maxnum);
		
		refreshProduceMulNum(maxnum);
	}
	
	// 选择配方
	internal void onSelectFormulaItem(XCfgFormula cfgFormula)
	{
		if(null == cfgFormula || cfgFormula == m_curCfgFormula)
			return;
		
		if(null != m_curCfgFormula)
		{
			m_formulaTgtIcon.Clear();
			foreach(XFormulaMatIcon icon in m_AllFormulaMatIcon.Values)
			{
				icon.Destroy();
			}
			m_AllFormulaMatIcon.Clear();
		}
		m_curCfgFormula = cfgFormula;
		m_formulaTgtIcon.SetItem(m_curCfgFormula);
		for(int i=0; i<m_curCfgFormula.NeedItemId.Length; i++)
		{
			uint nItemId = m_curCfgFormula.NeedItemId[i];
			ushort wItemNum = m_curCfgFormula.NeedItemNum[i];
			if(nItemId > 0 && wItemNum > 0 && !m_AllFormulaMatIcon.ContainsKey(nItemId))
			{
				m_AllFormulaMatIcon.Add(nItemId, new XFormulaMatIcon(Dp_ActionIcon_Sample, Dp_Produce_GridMaterail.gameObject, nItemId, wItemNum));
				Dp_Produce_GridMaterail.repositionNow = true;
			}
		}
		
		refreshProduceMulNum(1);
	}
	
	// 学习技能
	private void onLearnButtonClick(GameObject go)
	{
		XProductManager.SP.ApplyLearnCareer((byte)(Tab_Radio_UserSel.CurrentSelect * 3 + Learn_Radio_CareerType.CurrentSelect + 1));
		
		int guideID = (int)XNewPlayerGuideManager.GuideType.Guide_Product_Learn;
		if ( 1 == Tab_Radio_UserSel.CurrentSelect )
			guideID = (int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeLearn;
		XNewPlayerGuideManager.SP.handleGuideFinish(guideID);
		 
		if ( XProductManager.SP.GetCareerInfo(1) == null )
		{
			GameObject makeItemGo = Tab_Radio_UserSel.transform.Find("CheckBox_Manufacture").gameObject;
			Vector3 showPos = new Vector3(Mathf.Round(makeItemGo.transform.localPosition.x - 542),
				Mathf.Round(makeItemGo.transform.localPosition.y + 184),
				Mathf.Round(makeItemGo.transform.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuideExt((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMake,
				(int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeLearn,
				1, showPos, makeItemGo);
		}
	}
	
	// 技能升级
	private void onUpgradeButtonClick(GameObject go)
	{
		XProductManager.SP.ApplyUpgradeCareer((byte)(Tab_Radio_UserSel.CurrentSelect));
	}
	
	// 删除技能
	private void onForgetButtonClick(GameObject go)
	{
		XProductManager.SP.ApplyForgetCareer((byte)(Tab_Radio_UserSel.CurrentSelect));
	}
	
	// 刷新熟练值显示(刷新前确定为当前显示标签页)
	private void refreshExp(XCfgProductCareer cfg, XCareerInfo info)
	{
		XCfgProductCareer cfgCurr = XCfgProductCareerMgr.SP.GetConfig(info.CareerId, (byte)info.Level);
		Dp_Slider_Exp.sliderValue = (float)(info.Exp - cfgCurr.NeedExp) / (float)(cfg.MaxExpLimit - cfgCurr.NeedExp);
		Dp_Label_Exp.text = info.Exp + "/" + cfg.MaxExpLimit;
		XCfgProductCareer next = XCfgProductCareerMgr.SP.GetConfig(info.CareerId, (byte)(info.Level + 1));
		if(null == next || next.NeedExp > info.Exp || next.NeedPlayerLevel > XLogicWorld.SP.MainPlayer.Level)
		{
			Dp_Button_Upgrade.enabled = false;
		}
		else
		{
			Dp_Button_Upgrade.enabled = true;
		}
		
		// 制造技能需要根据当前的经验值更新字体的颜色和背景颜色
		if ( 1 == next.Kind )
		{
			foreach ( KeyValuePair<string, XFormulaList> formulaItem in m_AllFormulaList )
				formulaItem.Value.updateBKShow(info);
		}
		if ( null != m_formulaTgtIcon )
			m_formulaTgtIcon.UpdateTargetMulInfo();
	}
	
	// 更新当前所要制造的配方目标产品数量
	private void refreshProduceMulNum(int n)
	{
		if(n < 0) 
			return;
		
		m_curProduceMul = n;
		Dp_Produce_PNum.text = "" + m_curProduceMul;
		m_formulaTgtIcon.SetMul(m_curProduceMul);
		foreach(XFormulaMatIcon icon in m_AllFormulaMatIcon.Values)
		{
			icon.SetMul(m_curProduceMul);
		}
	}
	
	// 技能显示
	public void OnLearnCareer(byte btType, XCareerInfo info)
	{
		if( Tab_Radio_UserSel.CurrentSelect == btType )
		{
			m_curGatherShowPoint = 0;
			m_AllGatherObjectBorn.Clear();
			ShowProductDisplayPanel((int)btType, info);
		}
	}
	
	// 显示技能面板
	public void ShowLearnPanel(int nIndex)
	{
		Object_Learn_Display_Panel.SetActive(true);
		Object_Product_Displya_Panel.SetActive(false);
		
		if ( 0 == nIndex )
		{
			Sprite_Collection_Icon.gameObject.SetActive(false);
			Label_Collection_xuexi.gameObject.SetActive(true);
			Label_Collection_Name.gameObject.SetActive(false);
			Label_Collection_Level.gameObject.SetActive(false);
		}
		if ( 1 == nIndex )
		{
			Sprite_Manufacture_Icon.gameObject.SetActive(false);
			Label_Manufacture_xuexi.gameObject.SetActive(true);
			Label_Manufacture_Name.gameObject.SetActive(false);
			Label_Manufacture_Level.gameObject.SetActive(false);
		}
		DisplayLearnPanel(Learn_Radio_CareerType.CurrentSelect);
	}
	
	public void onClickLearnItem(GameObject go)
	{
		if ( !loadFinish )
			return;
		
		// 新手引导
		int guideID = (int)XNewPlayerGuideManager.GuideType.Guide_Product_Learn;
		int finishext = (int)XNewPlayerGuideManager.GuideType.Guide_Product_Select;
		int key2 = 2;
		if ( 1 == Tab_Radio_UserSel.CurrentSelect )
		{
			key2 = 3;
			guideID = (int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeLearn;
			finishext = (int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeItem;
		}
		
		XNewPlayerGuideManager.SP.handleGuideFinishExt(finishext);
		
		Vector3 showPos = new Vector3(Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.x),
			Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.y - 277),
			Mathf.Round(Object_Learn_Display_Panel.transform.localPosition.z));
		XNewPlayerGuideManager.SP.handleShowGuide2(guideID, key2, showPos, Object_Learn_Display_Panel);
	}
	
	// 显示制造面板
	public void ShowProductDisplayPanel(int nIndex, XCareerInfo info)
	{
		Object_Learn_Display_Panel.SetActive(false);
		Object_Product_Displya_Panel.SetActive(true);
		
		if ( 0 == nIndex )
		{
			XCfgProductCareer cfg = XCfgProductCareerMgr.SP.GetConfig(info.CareerId, info.Level);
			Label_Collection_Name.text = cfg.Name;
			Label_Collection_Name.gameObject.SetActive(true);
			Label_Collection_Level.text = XStringManager.SP.GetString((uint)info.Level + 1009);
			Label_Collection_Level.gameObject.SetActive(true);
			Sprite_Collection_Icon.spriteName = cfg.SelectIcon;
			Sprite_Collection_Icon.gameObject.SetActive(true);
			Label_Collection_xuexi.gameObject.SetActive(false);
		
			Object_Collection_Panel.SetActive(true);
			Object_manufacture_Panel.SetActive(false);
			ShowCollectionPanel(info);
		}
		if ( 1 == nIndex )
		{
			XCfgProductCareer cfg = XCfgProductCareerMgr.SP.GetConfig(info.CareerId, info.Level);
			Label_Manufacture_Name.text = cfg.Name;
			Label_Manufacture_Name.gameObject.SetActive(true);
			Label_Manufacture_Level.text = XStringManager.SP.GetString((uint)info.Level + 1009);
			Label_Manufacture_Level.gameObject.SetActive(true);
			Sprite_Manufacture_Icon.spriteName = cfg.SelectIcon;
			Sprite_Manufacture_Icon.gameObject.SetActive(true);
			Label_Manufacture_xuexi.gameObject.SetActive(false);
		
			Object_Collection_Panel.SetActive(false);
			Object_manufacture_Panel.SetActive(true);
		}
	}
	
	// 展示学校技能面板
	public void DisplayLearnPanel(int selIndex)
	{
		int curSelIndex = Tab_Radio_UserSel.CurrentSelect;
		int index = Learn_Radio_CareerType.CurrentSelect;
		for ( int i = 0; i < 3; i++ )
		{
			byte careerId = (byte)(3 * curSelIndex + i + 1);
			XCfgProductCareer cfgSelf = XCfgProductCareerMgr.SP.GetConfig(careerId, 1);
			if(	null == cfgSelf	) 
				continue;
			
			XCfgProductCareer cfgRela1 = XCfgProductCareerMgr.SP.GetConfig(cfgSelf.RelativeID1, 1);
			Sprite_Career_ReaIName[i].text = cfgRela1.Name;
			if( index == i )
			{
				Sprite_Career_Icon[i].spriteName = cfgSelf.SelectIcon;
				Sprite_Career_ReaIcon[i].spriteName = cfgRela1.SelectIcon;
			}
			else
			{
				Sprite_Career_Icon[i].spriteName = cfgSelf.DefaultIcon;
				Sprite_Career_ReaIcon[i].spriteName = cfgRela1.DefaultIcon;
			}
			Label_Career_Name[i].text = cfgSelf.Name;
			Label_Career_Description[i].text = cfgSelf.Discription;
		}
		if ( 0 == curSelIndex )
		{
			LearnButtonTip.text = XStringManager.SP.GetString(290u);
		}
		else
		{
			LearnButtonTip.text = XStringManager.SP.GetString(291u);
		}
	}
	
	// 更新采集物场景信息
	public void ShowCollectionPanel(XCareerInfo info)
	{
		foreach(KeyValuePair<uint, SortedList<int, XCfgGatherObjectBorn> > kvpBorn in XCfgGatherObjectBornMgr.SP.ItemTable)
		{
			foreach(KeyValuePair<int, XCfgGatherObjectBorn> kvpGO in kvpBorn.Value)
			{
				XCfgGatherObject cfgGO = XCfgGatherObjectMgr.SP.GetConfig(kvpGO.Key);
				if ( info.CareerId != cfgGO.NeedCareer )
					continue;
				m_AllGatherObjectBorn[kvpGO.Key] =kvpGO.Value;
			}
		}
		ShowCurrentCollection();
	}
	
	// 显示采集物信息
	public void ShowCurrentCollection()
	{
		int leftCount = m_AllGatherObjectBorn.Count - 3 * m_curGatherShowPoint;
		if ( 0 >= leftCount )
			return;
		else if ( leftCount < 3 )
		{
			int matierShowCount = Collection_items.Length;
			for ( int i = 0; i < matierShowCount; i++ )
				Collection_items[i].Show(false);
			
			// 隐藏场景名称控件
			int count1 = Sprite_Sense_Name.Length;
			for ( int i = 0; i < count1; i++ )
				Sprite_Sense_Name[i].gameObject.SetActive(false);
			
			// 隐藏场景图标控件
			int count2 = Sprite_Sense_Icon.Length;
			for ( int i = 0; i < count2; i++ )
				Sprite_Sense_Icon[i].gameObject.SetActive(false);
			
			// 隐藏场景分割线控件
			int count3 = Sprite_Collection_Split.Length;
			for ( int i = 0; i < count3; i++ )
				Sprite_Collection_Split[i].gameObject.SetActive(false);
		}
		
		
		for ( int i = 0; i < leftCount && i < 3; i++ )
		{
			int key = m_AllGatherObjectBorn.Keys[3 * m_curGatherShowPoint + i];
			XCfgGatherObjectBorn cfgBorn = m_AllGatherObjectBorn[key];
			
			// 显示场景图标和名称
			XCfgSceneInfo sceneInfo = XCfgSceneInfoMgr.SP.GetConfig(cfgBorn.SceneId);
			if(null != sceneInfo)
			{
				Sprite_Sense_Name[i].text = sceneInfo.Name;
				
			}
			Sprite_Sense_Name[i].gameObject.SetActive(true);
			Sprite_Sense_Icon[i].gameObject.SetActive(true);
			Sprite_Collection_Split[i*2].gameObject.SetActive(true);
			Sprite_Collection_Split[i*2 + 1].gameObject.SetActive(true);
			XCfgWorldMap wmCfg = XCfgWorldMapMgr.SP.GetConfig(sceneInfo.SceneID);
			if(null != wmCfg)
			{
				Sprite_Sense_Icon[i].spriteName = wmCfg.CloseSprite;
			}
			// 显示对应场景名称的物品
			XCfgGatherObject cfgGO = XCfgGatherObjectMgr.SP.GetConfig(key);
			// 如果当前采集物不是角色所选技能，则不显示
			//if ( cfgGO.NeedCareer !=  info.CareerId )
			//	continue;
			int n = 0;
			foreach(uint itemId in cfgGO.ItemId)
			{
				if ( !m_allMaterierCollected.ContainsKey(itemId) )
					m_allMaterierCollected[itemId] = false;
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemId);
				Collection_items[i * 3 + n].setGatherInfo(m_allMaterierCollected[itemId], itemId, cfgItem, cfgBorn.SceneId, cfgBorn.BornPos);
				Collection_items[i * 3 + n].Show(true);
				if ( ++n >=3 )
					break;
			}
		}
	}
	
	// 技能升级结果
	public void OnUpgradeCareer(byte btType, XCareerInfo info)
	{
		XCfgProductCareer cfg = XCfgProductCareerMgr.SP.GetConfig(info.CareerId, info.Level);
		if(Tab_Radio_UserSel.CurrentSelect == btType)
		{
			refreshExp(cfg, info);
		}
		
		if ( 1 == cfg.Kind )
			Label_Manufacture_Level.text = XStringManager.SP.GetString((uint)info.Level + 1009);
		else
			Label_Collection_Level.text = XStringManager.SP.GetString((uint)info.Level + 1009);
	}
	
	// 删除技能结果
	public void OnForgetCareer(byte btType)
	{
		if ( Tab_Radio_UserSel.CurrentSelect == btType )
		{
			ShowLearnPanel((int)btType);
		}
		
		if((byte)EProductCareerType.eProductCareerType_Gather == btType)
		{
			m_AllGatherObjectBorn.Clear();
		}
		else
		{
			m_curCfgFormula = null;
			Dp_Produce_FormulaTree.clearTree();
			m_AllFormulaList.Clear();
			m_formulaTgtIcon.Clear();
			foreach(XFormulaMatIcon icon in m_AllFormulaMatIcon.Values)
				icon.Destroy();
			m_AllFormulaMatIcon.Clear();
		}
	}
	
	// 更新当前角色体力
	public void OnSetStrength(uint nStrength, uint nMaxStrength)
	{
		float sliderValue = (float)nStrength / (float)nMaxStrength;
		Dp_Slider_Strength.sliderValue = sliderValue;
		Dp_Label_Strength.text = nStrength + "/" + nMaxStrength;
	}
	
	// 更新当前角色节能经验
	public void OnSetExp(byte btType, XCareerInfo info)
	{
		if ( Tab_Radio_UserSel.CurrentSelect == btType )
		{
			XCfgProductCareer cfg = XCfgProductCareerMgr.SP.GetConfig(info.CareerId, info.Level);
			refreshExp(cfg, info);
		}
	}
	
	// 增加已采集物信息
	public void OnAddGatherRec(uint nItemId)
	{
		if(!m_allMaterierCollected.ContainsKey(nItemId))
		{
			Log.Write(LogLevel.WARN, "XProductUI, logic error, try to enable a XGatherIcon which does not exist : {0}", nItemId);
			return;
		}
		m_allMaterierCollected[nItemId] = true;
		
		// 如果当前采集物正显示在UI上，则需要马上更新UI
		int showCount = Collection_items.Length;
		for ( int i = 0; i < showCount; i++ )
		{
			if ( Collection_items[i].IconData == (int)nItemId )
				Collection_items[i].Enable();
		}
	}
	
	// 添加配方
	public void OnAddFormula(uint nFormulaId)
	{	
		XCfgFormula cfgFormula = XCfgFormulaMgr.SP.GetConfig(nFormulaId);
		XFormulaList formulaList = null;
		if(!m_AllFormulaList.ContainsKey(cfgFormula.ItemType))
		{
			formulaList = new XFormulaList(cfgFormula.ItemType);
			m_AllFormulaList.Add(cfgFormula.ItemType, formulaList);
		}
		else
		{
			formulaList = m_AllFormulaList[cfgFormula.ItemType];
		}
		formulaList.InsertFormula(cfgFormula, this);
		TAB_PANEL.Reposition();
	}
	
	// 配方信息更新
	public void OnItemUpdate(uint itemId)
	{
		if(null == m_AllFormulaMatIcon)
			return;
		
		if(!m_AllFormulaMatIcon.ContainsKey(itemId))
			return;
		
		m_AllFormulaMatIcon[itemId].OnItemNumChanged();
	}
	
	// 角色升级，需要更新技能经验
	public void OnLevelUp()
	{
		XCareerInfo info = XProductManager.SP.GetCareerInfo((byte)Tab_Radio_UserSel.CurrentSelect);
		if(null == info) 
			return;
		
		XCfgProductCareer cfg = XCfgProductCareerMgr.SP.GetConfig(info.CareerId, info.Level);
		if(null == cfg) 
			return;
		refreshExp(cfg, info);
	}
}

