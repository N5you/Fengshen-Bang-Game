using UnityEngine;
using System.Collections;


[AddComponentMenu("UILogic/XStrengthenWindow")]
public class XStrengthenWindow : XDefaultFrame
{
	public static readonly int SHOW_ICON_NUM = 6;
	public static readonly int MAX_PET_NUM = 8;
	public static readonly int MAX_MATERIAL_NUM = 2;
	public static readonly int MAX_ITEM_SEL_NUM = 18;
	
	//一个条目
	[System.Serializable]
	public class XStrengthenItem
	{
		public XActionIcon	ActionIcon;
		public UILabel		mLabel;
		public UISprite		mSprite;
		public int			Index;
		
		public GameObject	Parent;
		
		//保存图片资源
		private UIAtlas		mAtlas;
		private string		mSpriteName;
		private bool		mIsShow	= false;
		
		public void Init()
		{
			if(mSprite == null)
				return ;
			mAtlas		= mSprite.atlas;
			mSpriteName	= mSprite.spriteName;

			//制空
			mSprite.spriteName	= "11000306";
			mIsShow	= false;
			
			UIEventListener ls = UIEventListener.Get(mSprite.gameObject);
			ls.onClick	= ClickDelegate;
		}
		
		public void ClickDelegate (GameObject go)
		{
			XEventManager.SP.SendEvent(EEvent.Strengthen_ItemClick,Index);
		}
		
		public void SetActive(bool isActive)
		{
			if(mIsShow == isActive)
				return ;
			mIsShow	= isActive;
			
			if(mIsShow)
			{
				mSprite.atlas	= mAtlas;
				mSprite.spriteName	= mSpriteName;
			}
			else
			{
				mSprite.spriteName	= "11000306";
			}
		}
	}
	
	//强化界面元素
	[System.Serializable]
	public class StrengthenUIObj
	{
		//根节点
		public GameObject	mStrenghtenUI;
		//当前操作的物品图标
		public XActionIcon  OperItemIcon;
		//强化所需材料
		public XActionIcon[] materialList = new XActionIcon[2];
		public UILabel[] materialNum = new UILabel[2];
		
		public UILabel StrengthenItemName;
		public UILabel StrengthenItemDes;
		
		public UILabel	Money;
		public UISprite	MoneySprite;
	
		public UIImageButton BtnStrengthen;
		public UIImageButton BtnDirectStrengthen;
		
			//强化文本描述
		public UILabel[]	AttrLabelL = new UILabel[6];
		public UILabel[]    AttrLabelR = new UILabel[6];
		
		public UISprite	IconBKSprite;
		public UISprite	SeperatorMid;
		public UISprite	SeperatorBottom;
		public UILabel	EmptyLabel;
		
		public void Init()
		{
			UIEventListener Listener6 = UIEventListener.Get(BtnStrengthen.gameObject);
			Listener6.onClick = ClickStrengthen;
			UIEventListener Listener7 = UIEventListener.Get(BtnDirectStrengthen.gameObject);
			Listener7.onClick = ClickDirectStrengthen;			
		}
		
		public void ClearStrengthenInfo()
		{
			OperItemIcon.Reset();
			StrengthenItemName.text	= "";
			for(int i = 0;i < MAX_MATERIAL_NUM;i++)
			{
				materialList[i].Reset();
				materialNum[i].text	= "";
			}
			Money.text	= "";
		}
		
		public void ClearAttrLabel()
		{
			for(int i = 0; i < 6; i++)
			{
				AttrLabelL[i].text	= "";
				AttrLabelR[i].text  = "";
			}
		}
		
		public void ClickStrengthen(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.Strengthen_StartShtrengthen);
		}
		
		public void ClickDirectStrengthen(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.Strengthen_StartDirectStr);
		}
	}	
	
	//镶嵌界面元素
	[System.Serializable]
	public class InlayUIObj
	{
		//根节点
		public GameObject		mInlayUI;
		public UIAtlas			StrengthenAtlas;
		public UILabel			EmptyLabel;
		
		public UISprite			IconBKSprite;
		public UILabel 			StrengthenItemName;
		public UILabel 			StrengthenItemDes;
		//当前操作的物品图标		 	
		public XActionIcon  	OperItemIcon;
		public UISprite[]		mGemSprite	= new UISprite[3];
		public UILabel[]		mGemLabel	= new UILabel[3];
		public string			mGemBKName;	
		public XActionIcon[] 	mUIInlayGemIcon = new XActionIcon[3];
		public UIImageButton[]	mUIInlayBtn	= new UIImageButton[4];
		public UISprite[]		UnLockSprite = new UISprite[3];
		public UILabel[]		FreeGemLabel = new UILabel[3];
		
		public void Init()
		{
			UIEventListener Listener10 = UIEventListener.Get(mUIInlayBtn[0].gameObject);
			Listener10.onClick = ClickBtnInlay1;
			
			UIEventListener Listener11 = UIEventListener.Get(mUIInlayBtn[1].gameObject);
			Listener11.onClick = ClickBtnInlay2;
			
			UIEventListener Listener12 = UIEventListener.Get(mUIInlayBtn[2].gameObject);
			Listener12.onClick = ClickBtnInlay3;
			
			UIEventListener Listener13 = UIEventListener.Get(mUIInlayBtn[3].gameObject);
			Listener13.onClick = ClickBtnInlayRemoveAll;
		}
		
		public void ClickBtnInlay1(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.Strengthen_InlayBtn,0);
			
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click);
		}
	
		public void ClickBtnInlay2(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.Strengthen_InlayBtn,1);
		}
	
		public void ClickBtnInlay3(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.Strengthen_InlayBtn,2);
		}
	
		public void ClickBtnInlay4(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.Strengthen_InlayBtn,3);
		}
	
		public void ClickBtnInlayRemoveAll(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.Strengthen_InlayRemoveAll);
		}
		
		public void ClearInlayInfo()
		{
			for(int i = 0; i < 3; i++)
			{
				mGemSprite[i].atlas		 = StrengthenAtlas;
				mGemSprite[i].spriteName = mGemBKName;
				mGemLabel[i].text		 = "";
				mUIInlayGemIcon[i].Reset();
				UILabel label = mUIInlayBtn[i].gameObject.GetComponentInChildren<UILabel>();
				if(label == null)
					continue;
				label.text = "镶嵌";	
			}
			StrengthenItemName.text	= "";
			//StrengthenItemDes.text	= "";
		}
	}
	
	//强化6个条目
	public XStrengthenItem[] StrengthenItemList = new XStrengthenItem[SHOW_ICON_NUM];
	//强化界面左侧对象选择按钮
	public UISpriteGroup 	SpriteGroupObj;
	public StrengthenUIObj	Strengthen;
	public InlayUIObj		Inlay;
	
	//功能选择
	public UISpriteGroup	FuncSpriteGroup;
	
	public XActionIcon[] ItemSelIconList = new XActionIcon[MAX_ITEM_SEL_NUM];
	public UIImageButton LeftBtn;
	public UIImageButton RightBtn;
	public UILabel		 LabelPage;
	
	
	public override bool Init()
	{
		base.Init();
	
//		Vector3 showPos = new Vector3(Mathf.Round(InlaySprite.transform.localPosition.x),
//			Mathf.Round(InlaySprite.transform.localPosition.y + 70),
//			Mathf.Round(InlaySprite.transform.localPosition.z));
//		XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click_item, 
//			1, showPos, InlaySprite.transform.parent.gameObject);
		
		UIEventListener leftPageBtnLS = UIEventListener.Get(LeftBtn.gameObject);
		leftPageBtnLS.onClick += OnClickLeftPageBtn;
		
		UIEventListener RightPageBtnLS = UIEventListener.Get(RightBtn.gameObject);
		RightPageBtnLS.onClick += OnClickRightPageBtn;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		
		return true;
	}
	
	public override void Show()
	{
		base.Show();
		
		if ( Strengthen.BtnStrengthen.enabled )
		{
			Vector3 showPos = new Vector3(-150, 10, 0);
			XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_Strength_Click, 2, showPos, 
				Strengthen.BtnStrengthen.gameObject);
		}
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eStrengthenWindow);
	}
	
	public void OnClickLeftPageBtn(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.Strengthen_LeftPageBtn);
	}
	
	public void OnClickRightPageBtn(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.Strengthen_RightPageBtn);
	}
	
	public void ActiveItem(int index)
	{
		for(int i = 0;i < SHOW_ICON_NUM; i++)
		{
			if(index == i)
				StrengthenItemList[i].SetActive(true);
			else
				StrengthenItemList[i].SetActive(false);
		}
	}
}
