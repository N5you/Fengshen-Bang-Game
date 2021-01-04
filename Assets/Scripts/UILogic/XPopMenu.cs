using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[AddComponentMenu("UILogic/XPopMenu")]
public class XPopMenu : XDefaultFrame
{
	//操作基类
	public class XBtnOperBase
	{
		~XBtnOperBase()
		{
			
		}
		
		public void DestoryUIObject()
		{
			if(UIBtn != null)
			{
				NGUITools.Destroy(UIBtn.gameObject);
			}
		}
		
		public void InitSprite(UIImageButton btn)
		{
			mDemoBtn	= btn;
		}
		//添加UIButton
		public UIImageButton AddBtn(string labelName,GameObject BtnParent)
		{
			if(BtnParent == null)
				return null;
			
			GameObject imgBtnGO = XUtil.Instantiate(mDemoBtn.gameObject,BtnParent.transform);
			imgBtnGO.SetActive(true);
			imgBtnGO.transform.parent = BtnParent.transform;

			imgBtnGO.transform.localPosition = new Vector3(imgBtnGO.transform.localPosition.x,-CurNum * 20,0);
			CurNum++;
			
			UIImageButton imgBtn = imgBtnGO.GetComponent<UIImageButton>();
			UILabel[] labels = imgBtnGO.GetComponentsInChildren<UILabel>(true);
			UILabel label = labels[0];
			if(label != null)
				label.text	= labelName;			
			
			return imgBtn;
		}
		
		public virtual void Init(GameObject go)
		{
				
		}
		
		protected string 	BtnName;
		protected UIImageButton	UIBtn	= null;
		protected UIImageButton mDemoBtn;
		
		public static int CurNum {get;set;}
	}
	
	//1.装备按钮
	public class XBtnEquip : XBtnOperBase
	{
		public XBtnEquip(string name)
		{
			BtnName	= name;
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickEquip;			
		}
		
		public void ClickEquip(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.PopMenu_Equip, EUIPanel.ePopMenu);
		}
	}
	
	//2.合成按钮
	public class XBtnCompose : XBtnOperBase
	{
		public XBtnCompose(string name)
		{
			BtnName	= name;
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickCompose;			
		}
		
		public void ClickCompose(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.PopMenu_Compose, EUIPanel.ePopMenu);
		}
	}
	
	//3.丢弃按钮
	public class XBtnDrop : XBtnOperBase
	{
		public XBtnDrop(string name)
		{
			BtnName	= name;			
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickDrop;			
		}
		
		public void ClickDrop(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.PopMenu_Drop, EUIPanel.ePopMenu);
		}
	}	
	//4.使用按钮
	public class XBtnUse : XBtnOperBase
	{
		public XBtnUse(string name)
		{
			BtnName	= name;			
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickUse;
		}
		
		public void ClickUse(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.PopMenu_Use, EUIPanel.ePopMenu);
		}		
	}
	
	//5.拆分按钮
	public class XBtnSplit : XBtnOperBase
	{
		public XBtnSplit(string name)
		{
			BtnName	= name;	
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickSplit;
		}
		
		public void ClickSplit(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.PopMenu_Split, EUIPanel.ePopMenu);
		}
	}
	
	//6.分解按钮
	public class XBtnDecompose : XBtnOperBase
	{
		public XBtnDecompose(string name)
		{
			BtnName	= name;	
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickDecompose;
		}
		
		public void ClickDecompose(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.PopMenu_Decompose, EUIPanel.ePopMenu);
		}	
	}
	
	//7.上传按钮，用于把物品数据发送到聊天框中
	public class XBtnUpload : XBtnOperBase
	{
		public XBtnUpload(string name)
		{
			BtnName	= name;	
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickUpload;
		}
		
		public void ClickUpload(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.PopMenu_Upload, EUIPanel.ePopMenu);
		}	
	}
	
	//8.上架
	public class XBtnAuction : XBtnOperBase
	{
		public XBtnAuction(string name)
		{
			BtnName	= name;	
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickAuction;
		}
		
		public void ClickAuction(GameObject _go)
		{
			XEventManager.SP.SendEvent(EEvent.PopMenu_Auction, EUIPanel.ePopMenu);
		}	
	}
	
	//出售
	public class XBtnSell : XBtnOperBase
	{
		private int m_uiItemIndex=-1;
		public XBtnSell(string name,int uiItemIndex)
		{
			BtnName	= name;
			m_uiItemIndex = uiItemIndex;
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickDecompose;
		}
		
		public void ClickDecompose(GameObject _go)
		{
			//XEventManager.SP.SendEvent(EEvent.PopMenu_Decompose, EUIPanel.ePopMenu);
			XShopItemMgr.SP.requestSellItem( (uint)m_uiItemIndex );
		}
		
	}
	
	//购买
	public class XBtnBuy : XBtnOperBase
	{
		private uint m_itemID=0;
		private ActionIcon_Type m_iActionType = 0;
		public XBtnBuy(string name,uint uiItemID , ActionIcon_Type actionType)
		{
			BtnName	= name;
			m_itemID = uiItemID;
			m_iActionType = actionType;
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickDecompose;
		}
		
		public void ClickDecompose(GameObject _go)
		{
			XShopItemMgr.SP.requestBuyItem(m_itemID, m_iActionType);
		}
		
	}
	
	//回购
	public class XBtnBuyBack : XBtnOperBase
	{
		private uint m_itemBackIndex=0;
		public XBtnBuyBack(string name,uint uiItemIndex )
		{
			BtnName	= name;
			m_itemBackIndex = uiItemIndex;
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName,go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = ClickDecompose;
		}
		
		public void ClickDecompose(GameObject _go)
		{
			XShopItemMgr.SP.requestBuyBackItem(m_itemBackIndex);
		}
		
	}
	
	// 用户名鼠标左击相应
	public class XBtnName : XBtnOperBase
	{
		public XBtnName(string name, EEvent type)
		{
			eventType = type;
			BtnName	= name;
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName, go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = onClickSendMsg;			
		}
		
		public void onClickSendMsg(GameObject go)
		{
			XEventManager.SP.SendEvent(eventType, EUIPanel.ePopMenu);
		}
		
		private EEvent eventType;
	}
	
	public class XButGuildKickMem : XBtnOperBase
	{
		public XButGuildKickMem(string name,  UInt64 uMemID)
		{
			m_MemId	= uMemID;
			BtnName	= name;
		}
		
		public override void Init(GameObject go)
		{
			base.Init(go);
			UIBtn =  AddBtn(BtnName, go);
			
			UIEventListener Listener = UIEventListener.Get(UIBtn.gameObject);
			Listener.onClick = onClickSendMsg;			
		}
		
		public void onClickSendMsg(GameObject go)
		{
			//XEventManager.SP.SendEvent(eventType, EUIPanel.ePopMenu);
			if(XLogicWorld.SP.MainPlayer.GuildId == 0 && XGuildManager.SP.m_stGuildBaseInfo.uMasterId != XLogicWorld.SP.MainPlayer.ID)
				return;
			XGuildManager.SP.RequestRemove(m_MemId);
		}
		
		UInt64 m_MemId;
		//private EEvent eventType;
	}
	
	private List<XBtnOperBase> mBtnList = new List<XBtnOperBase>();
	public GameObject   BtnParent;
	public UISprite		BKSprite;
	public UIImageButton	DemoBtn;
	
	public override bool Init()
	{		
		return base.Init();
	}
	
	public override void Show()
	{
		// 在鼠标指针所在处显示
		Vector3 vec = LogicApp.SP.UICamera.ScreenToWorldPoint(Input.mousePosition);
		vec.z = transform.position.z;
		transform.position = new Vector3(vec.x + 0.5f,vec.y + 0.5f,vec.z);
		
		//摧毁控件
		for(int i = 0; i < mBtnList.Count; i++)
		{
			mBtnList[i].DestoryUIObject();
		}
		mBtnList.Clear();		
		XBtnOperBase.CurNum = 0;

		base.Show();
	}
	
	private void AdjustHeight()
	{
		//背景高度
		float x =  BKSprite.cachedTransform.localScale.x;
		BKSprite.cachedTransform.localScale	= new Vector3(x,XBtnOperBase.CurNum*20 + 2,1);
	}
	
	public void AddBtn(XBtnOperBase baseOp)
	{
		baseOp.InitSprite(DemoBtn);
		baseOp.Init(BtnParent);
		AdjustHeight();
		mBtnList.Add(baseOp);
		
	}
}
