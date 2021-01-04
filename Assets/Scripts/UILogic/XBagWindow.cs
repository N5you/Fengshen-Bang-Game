using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UILogic/XBagWindow")]
public class XBagWindow : XDefaultFrame
{
	public XBagWindow()
	{
		
	}
	
	public static readonly int MAX_BAG_ITEM_NUM = 6 * 6;	
	
	public UISprite[] BagPage = new UISprite[3];
	public int	CurPageID {get;set;}
	
	public XActionIcon[] 	ActionIconArray = new XActionIcon[MAX_BAG_ITEM_NUM];
	
	public UISpriteGroup	ItemSort;
	public UIImageButton[]	BtnList	= new UIImageButton[3];
	public UILabel			CountLabel;
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener Listener1 = UIEventListener.Get(BagPage[0].gameObject);
		Listener1.onClick += ClickShowPage1;
		UIEventListener Listener2 = UIEventListener.Get(BagPage[1].gameObject);
		Listener2.onClick += ClickShowPage2;
		UIEventListener Listener3 = UIEventListener.Get(BagPage[2].gameObject);
		Listener3.onClick += ClickShowPage3;
		
		UIEventListener Listener4 = UIEventListener.Get(BtnList[0].gameObject);
		Listener4.onClick += ClickSeal;
		UIEventListener Listener5 = UIEventListener.Get(BtnList[2].gameObject);
		Listener5.onClick += ClickArrange;
		UIEventListener Listener6 = UIEventListener.Get(BtnList[1].gameObject);
		Listener6.onClick += ClickBank;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		
		CurPageID	= 0;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eBagWindow);
	}
	
	public override void Show()
	{
		base.Show();
		
		XNewPlayerGuideManager.SP.handleOpenBagGuide();
	}
	
	public override void Hide()
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Drag);
		base.Hide();
	}
	
	public void ClickShowPage1(GameObject _go)
	{		
		CurPageID	= 0;
		XEventManager.SP.SendEvent(EEvent.Bag_ChangePage,EUIPanel.eBagWindow,CurPageID);
	}
	
	public void ClickShowPage2(GameObject _go)
	{
		CurPageID	= 1;		
		XEventManager.SP.SendEvent(EEvent.Bag_ChangePage,EUIPanel.eBagWindow,CurPageID);
	}
	
	public void ClickShowPage3(GameObject _go)
	{
		CurPageID	= 2;		
		XEventManager.SP.SendEvent(EEvent.Bag_ChangePage,EUIPanel.eBagWindow,CurPageID);
	}
	
	public void ClickSeal(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.Bag_ItemSeal);
	}
	
	public void ClickArrange(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.Bag_Arrange);
	}
	
	public void ClickBank(GameObject _go)
	{
		//XEventManager.SP.SendEvent(EEvent.Bag_Bank);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eStorage);
	}
	
	// 物品高亮特效显示
	private SortedList<ulong, XU3dEffect>	m_effect = new SortedList<ulong, XU3dEffect>();
	private LinkedList<GameObject> m_objs = new LinkedList<GameObject>();
	public void ShowItemEffect(int itemType, int subitem, uint effectid)
	{
		int beginIndex 	= XItemManager.GetBeginIndex(EItemBoxType.Bag);
		int endIndex	= XItemManager.GetEndIndex(EItemBoxType.Bag);
		for(int i = beginIndex; i <= endIndex; i++)
		{ 
			XItem LogicItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)i);
			if(LogicItem == null || LogicItem.IsEmpty())
				continue;
				
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
			if(cfgItem == null)
				continue;
				
			if ( itemType == cfgItem.ItemType && subitem == cfgItem.ItemSubType && cfgItem.AddHealth > 0)
			{
				StartEffect(LogicItem.GUID, effectid, ActionIconArray[i].gameObject);
			}  
		}	
	}
	public void StartEffect(ulong key, uint effectid, GameObject go)
	{
		m_objs.AddLast(go);
		
		XU3dEffect effect = new XU3dEffect(effectid, EffectLoadedHandle);
		m_effect[key] = effect;
	}
	private void EffectLoadedHandle(XU3dEffect effect)
    {
		if ( 0 == m_objs.Count )
			return;
		
        effect.Layer = GlobalU3dDefine.Layer_UI_2D;
        effect.Parent = m_objs.First.Value.transform;
        effect.LocalPosition =  new Vector3(0, 0, -10);
        effect.Scale = new Vector3(450, 450, 1);
		m_objs.RemoveFirst();
		
		if ( 0 == m_objs.Count )
			Invoke("DestroyAllEffect", 3f);
    }
	public void DestroyAllEffect()
	{
		foreach( KeyValuePair<ulong, XU3dEffect> item in m_effect )
		{
			XU3dEffect effect = item.Value;
			effect.Destroy();
		}
		m_effect.Clear();
	}
}
