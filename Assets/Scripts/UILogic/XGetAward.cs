using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XGetAward")]
public class XGetAward : XUIBaseLogic
{
	private static uint SINGLE_ITEM_WIDTH	= 70;
	private static float 	StartX = 36.0f;	
	public GameObject		mBtnRoot;
	public GameObject		BtnExample;
	public UISprite			BKSprite;
	
	private class SingleFeature
	{
		public uint			mIndex;
		public GameObject	mGO;
		public EAwardType  	mType;
		
		public SingleFeature()
		{
			FeatureList.Add(EAwardType.EAwardType_XianDH,new UIEventListener.VoidDelegate(XianDHClickDelegate));
			FeatureList.Add(EAwardType.EAwardType_ZhanYaoLu,new UIEventListener.VoidDelegate(ZhanYaoLuClickDelegate));
		}
		
		private SortedList<EAwardType, UIEventListener.VoidDelegate> FeatureList		= new SortedList<EAwardType, UIEventListener.VoidDelegate>();
		
		public void Init()
		{
			UIEventListener ls = UIEventListener.Get(mGO.gameObject);
			ls.onClick	= FeatureList[mType];
		}
		
		public void XianDHClickDelegate (GameObject go)
		{
			uint EmptyPosCount = XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag);
			if(EmptyPosCount == 0)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Chat,204);
				return ;
			}
			
			FeatureDataUnLockMgr.SP.ShowAward(mIndex);
			
			//get Award
			CS_Int.Builder msg = CS_Int.CreateBuilder();
			msg.SetData((int)mType);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_AddAward,msg.Build());			
			
			XEventManager.SP.SendEvent(EEvent.UI_FreshAward);
		} 
		
		public void ZhanYaoLuClickDelegate (GameObject go)
		{
			
			FeatureDataUnLockMgr.SP.ShowAward(mIndex);
			
			//get Award
			CS_Int.Builder msg = CS_Int.CreateBuilder();
			msg.SetData((int)mType);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_AddAward,msg.Build());			
			
			XEventManager.SP.SendEvent(EEvent.UI_FreshAward);
		} 
		
	}
	
	private List<SingleFeature>	mList = new List<SingleFeature>();
	
	public void AddAward(uint index,EAwardType type)
	{		
		XCfgAddAward cfg = XCfgAddAwardMgr.SP.GetConfig((uint)type);
		if(cfg == null)
			return ;
		
		SingleFeature	newsfo = new SingleFeature();
		newsfo.mGO	= XUtil.Instantiate(BtnExample.gameObject,mBtnRoot.transform,new Vector3(0,0,0),new Vector3(0,0,0));
		newsfo.mType	= type;
		newsfo.mIndex	= index;
		newsfo.mGO.SetActive(true);
		
		
		UIImageButton imageBtn = newsfo.mGO.GetComponent<UIImageButton>();
		if(imageBtn == null)
			return ;
		
		XUIDynamicAtlas.SP.SetSprite(imageBtn.target, (int)cfg.AtlasID, cfg.BtnCom, true, null);
		imageBtn.normalSprite	= cfg.BtnCom;
		imageBtn.hoverSprite	= cfg.BtnHover;
		imageBtn.pressedSprite	= cfg.BtnPress;
		
		newsfo.mGO.transform.localPosition	= new Vector3(StartX + mList.Count * SINGLE_ITEM_WIDTH ,-25.0f,mBtnRoot.transform.position.z);
		newsfo.Init();
		
		UIEventListener listen = UIEventListener.Get(imageBtn.gameObject);
		mList.Add(newsfo);
		
		//adjust Pos	
		Bounds bound = NGUIMath.CalculateRelativeWidgetBounds(mBtnRoot.transform);
		mBtnRoot.transform.localPosition = new Vector3(-(mList.Count * SINGLE_ITEM_WIDTH) / 2,mBtnRoot.transform.localPosition.y,mBtnRoot.transform.localPosition.z);
		
		//adjust bg width
		BKSprite.transform.localScale	= new Vector3(bound.extents.x * 2,BKSprite.transform.localScale.y,BKSprite.transform.localScale.z);
		
		//listen.onClick	+= new UIEventListener.VoidDelegate(mFuncEffect[(EFeatureID)temp1.Value].ClickFuncBtn);
		//XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT,(uint)999,(uint)900039, 1);
		
	}
	
	public void Clear()
	{
		//clear All
		foreach(SingleFeature obj in mList)
		{
			NGUITools.Destroy(obj.mGO);
		}
		
		mList.Clear();
	}
	
}
