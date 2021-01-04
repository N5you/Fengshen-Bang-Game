using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTAuction : XUICtrlTemplate<XAuctionUI>
{
	public XUTAuction()
	{
		RegEventAgent_CheckCreated(EEvent.auction_UpdateMyAuction,  OnUpdateMyAuction);
		RegEventAgent_CheckCreated(EEvent.auction_UpdateHistroyInfo, OnUpdateMyHistroyInfo);
		RegEventAgent_CheckCreated(EEvent.auction_RealMoney_Change, OnUserRealMoneyChange);

		XEventManager.SP.AddHandler(OnDragPublish, EEvent.auction_DragPublish);
		XEventManager.SP.AddHandler(OnUpdateAllAuction, EEvent.auction_UpdateAuction);
		XEventManager.SP.AddHandler(OnUserSelectAuction, EEvent.auction_UserSelectAuction);
		XEventManager.SP.AddHandler(OnUpdatePrice, EEvent.auction_Update_Price);
		XEventManager.SP.AddHandler(OnSetSoldNoDataSee, EEvent.auction_SetSoldNoDataVisible);
		XEventManager.SP.AddHandler(OnSetMyNoDataSee, EEvent.auction_SetMyNoDataVisible);
		XEventManager.SP.AddHandler(OnDelete2ShowItem, EEvent.auction_DeleteOnSoldItem);
	}
	
	public void OnUpdateAllAuction(EEvent evt, params object[] args)
    {
		LogicUI.ShowAllAuctionInfo(args[0] as List<XAuctionInfo>, args[1] as string);
    }

    public void OnUpdateMyAuction(EEvent evt, params object[] args)
    {
		LogicUI.ShowMyAuctionInfo(args[0] as SortedList<XMyAuction, XMyAuction>);
    }
	
	private void OnUpdateMyHistroyInfo(EEvent evt, params object[] args)
	{
		LogicUI.ShowHistroyInfo((AuctionHistyoyInfo)args[0]);
	}
	
	private void OnUserRealMoneyChange(EEvent evt, params object[] args)
	{
		LogicUI.UserRealMoneyChange((long)args[0]);
	}
	
	private void OnDragPublish(EEvent evt, params object[] args)
	{
		if ( args.Length < 3 )
			return;
		
		if ( EUIPanel.eBagWindow != (EUIPanel)args[0] )
			return;
		
		uint price = XLogicWorld.SP.AuctionManager.GetItemPrice((uint)args[2]);
		LogicUI.GragPublish((uint)args[2], price, XLogicWorld.SP.AuctionManager.m_HistroyInfo.LeftPubCount);
	}
	
	public void OnUserSelectAuction(EEvent evt, params object[] args)
    {
		LogicUI.UserSelectAuction(args[0] as XAuctionInfo);
    }
	
	public void OnUpdatePrice(EEvent evt, params object[] args)
    {
		LogicUI.UpdateSoldPrice((uint)args[0], (SItemColorID)args[1]);
    }
	
	public void OnSetSoldNoDataSee(EEvent evt, params object[] args)
    {
		bool hide = ( 0 == (int)args[0] );
		LogicUI.SetSoldDataNoDataVisible(hide);
    }
	
	public void OnSetMyNoDataSee(EEvent evt, params object[] args)
    {
		bool hide = ( 0 == (int)args[0] );
		LogicUI.SetMyDataNoDataVisible(hide);
    }
	
	public void OnDelete2ShowItem(EEvent evt, params object[] args)
	{
		LogicUI.ReSet2SoldInfo();
	}
	
// 测试代码----------------------------------------------------------------------------
/*	public override void OnCreated (object arg)
	{
		base.OnCreated(arg);
		// 增加购买物品信息
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10029	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10031	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10032	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10033	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10034	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10035	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10036	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10037	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10038	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10039	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	10040	, 1, 103, 123, false, 10);
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90100	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90101	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90102	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90103	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90104	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90105	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90106	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90107	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90108	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90200	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90201	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90202	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90203	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90204	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90205	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90206	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90207	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90208	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90300	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90301	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90302	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90303	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90304	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90305	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90306	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90307	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90308	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90400	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90401	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90402	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90403	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90404	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90405	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90406	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90407	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90408	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90500	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90501	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90502	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90503	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90504	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90505	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90506	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90507	, 1, 103, 123, false, 10);		
		XAuctionManager.SP.On_SC_AddBuyAuctionItem(	90508	, 1, 103, 123, false, 10);		
	}*/
//------------------------------------------------------------------------------------------
}