using System;

class XUTFightWin : XUICtrlTemplate<XFightWin>	
{
	private uint mExp;
	private uint mGameMoney;
	private uint mFlag;
	
	private uint[] TempDropItemList = new uint[XFightWin.MAX_ITEM_ICON_NUM];
	private int CurTempItemCount;
	
	private static string[] strSprite = new string[4]{"11007001", "11007002","11007003","11007004"};
	
    public XUTFightWin()
    {
		CurTempItemCount	= 0;
		// XEventManager.SP.AddHandler(WinData,EEvent.CopySceneResult_WinLevel);
		RegEventAgent_CheckCreated(EEvent.CopySceneResult_WinLevel, WinData);
		
		// 新版本中战斗结果界面不再出现物品奖励信息 add by ljh
		// XEventManager.SP.AddHandler(WinItemData,EEvent.CopySceneResult_Win_SetItem);
		RegEventAgent_CheckCreated(EEvent.CopySceneResult_Win_Reset, WinReset);
    }
	
	public void Reset()
	{
		// 新版本中战斗结果界面不再出现物品奖励信息,只显示星星数目 add by ljh
		for( int i = 0; i < LogicUI.Stars.Length; i++ )
		{
			LogicUI.Stars[i].gameObject.SetActive(false);
		}
		LogicUI.StarShowCount = 0;
		return;
		
		CurTempItemCount	= 0;
		for(int i = 0; i < XFightWin.MAX_ITEM_ICON_NUM; i++)
		{
			TempDropItemList[i]	= 0;
		}
	}

    public override void OnCreated(object arg)
    {
        base.OnCreated(arg);
    }
	
	public override void OnShow()
	{
		base.OnShow();
		
		if(mExp > 0)
			XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,505,mExp);
		if(mGameMoney > 0)
			XNoticeManager.SP.Notice(ENotice_Type.ENoitce_Type_CenterTip,507,mGameMoney);
		
		/*
		LogicUI.ResetUIData();
		
		LogicUI.ExpLabel.text	= Convert.ToString(mExp);
		//LogicUI.RepLabel.text	= Convert.ToString(mRep);
		LogicUI.MoneyLabel.text	= Convert.ToString(mGameMoney);	
		
		for(int i = 0; i < CurTempItemCount; i++)
		{
			//清空图标
			LogicUI.LogicIcon[i].ResetUIAndLogic();
			XItem tempItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(TempDropItemList[i]);
			if(tempItem == null || tempItem.IsEmpty())
				continue;
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(tempItem.DataID);
			if(cfgItem == null)
				continue;
			
			EItemBoxType outType;
			ushort outIndex;
			XItemManager.GetContainerType((ushort)TempDropItemList[i],out outType,out outIndex);
			
			LogicUI.LogicIcon[i].SetLogicData((ActionIcon_Type)outType,outIndex);			
			LogicUI.LogicIcon[i].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,tempItem.Color,tempItem.ItemCount);
		}
		*/
	}
	
	public void WinData(EEvent evt, params object[] args)
	{
		mExp		= (uint)args[0];
		mFlag		= (uint)args[1];
		mGameMoney	= (uint)args[2];
		
		if(LogicUI != null)
		{
			LogicUI.ExpLabel.text	= Convert.ToString(mExp);
			//LogicUI.RepLabel.text	= Convert.ToString(mRep);
			LogicUI.MoneyLabel.text	= Convert.ToString(mGameMoney);
			LogicUI.WinSprite.spriteName	= strSprite[mFlag];
			if ( 0u == mFlag )
				LogicUI.StarShowCount = 3;
			else if ( 1u== mFlag || 2 == mFlag )
				LogicUI.StarShowCount = 2;
			else if ( 3u == mFlag )
				LogicUI.StarShowCount = 1;
			else
				LogicUI.StarShowCount = 0;
		}		
	}
	
	public void WinItemData(EEvent evt, params object[] args)
	{
		int index 		= (int)args[0];
		uint BagIndex	= (uint)args[1];
		
		if(index < XFightWin.MAX_ITEM_ICON_NUM)
		{
			TempDropItemList[index] = BagIndex;
			CurTempItemCount = index + 1;
		}
	}
	
	public void WinReset(EEvent evt, params object[] args)
	{
		Reset();
	}
}
