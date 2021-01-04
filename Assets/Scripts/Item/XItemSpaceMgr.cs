using System;
using UnityEngine;
using System.Collections;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

public class XItemSpaceMgr : XSingleton<XItemSpaceMgr>
{	
	private BitArray	mSpaceArray = null;
	
	public XItemSpaceMgr()
	{
		XEventManager.SP.AddHandler(OnItemSpaceOpen,EEvent.Bag_ItemSpace);
	}
	
	private ushort curWillPos;
	private uint   needMoney;
	private ushort curOpenPos;

	
	private void OnItemSpaceOpen(EEvent evt, params object[] args)
	{
		EItemBoxType IconType	= (EItemBoxType)args[0];
		int 		IconData	= (int)args[1];
		
		if(IconType != EItemBoxType.Bag)
			return ;
		
		ushort realPos = XItemManager.GetItemIndex(IconType,(ushort)IconData);
		//发现开启过，或者前一个还没有开启都不触发效果
		if(IsSet((short)realPos))
			return ;

		uint totalMoney = GetNeedMoney(realPos);

		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(OnClickOK);
		UIEventListener.VoidDelegate	funcCancel = new UIEventListener.VoidDelegate(OnClickCancel);
		
		string text = "开启槽位需要";
		if(IconType == EItemBoxType.Bag)
		{
			text += Convert.ToString(totalMoney);
			text += "金币";
			
			needMoney	= totalMoney;
		}
		else if(IconType == EItemBoxType.Bank)
		{
			XCfgBankSpace cfgBankSpace = XCfgBankSpaceMgr.SP.GetConfig((uint)(realPos+1 - XItemManager.GetBeginIndex(EItemBoxType.Bank)));
			if(cfgBankSpace == null)
				return ;
			text += Convert.ToString(cfgBankSpace.PagePrice);
			text += "金币";
			
			needMoney	=	cfgBankSpace.PagePrice;
		}			
		
		curWillPos	= realPos;	
		
		XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,funcCancel,text);
		
	}

	private uint GetNeedMoney(uint itemIndex)
	{
		ushort startIndex = XItemManager.GetBeginIndex(EItemBoxType.Bag);
		ushort endIndex   = XItemManager.GetEndIndex(EItemBoxType.Bag);
		
		uint totalMoney = 0;
		for(ushort i = curOpenPos; i <= itemIndex; i++)
		{
			if(!IsSet((short)i))
			{
				XCfgBagSpace cfgBankSpace = XCfgBagSpaceMgr.SP.GetConfig((uint)(i + 1 - startIndex));
				totalMoney += cfgBankSpace.Price;
			}

		}
		return totalMoney;
	}
	
	private void OnClickOK(GameObject go)
	{
		//金钱判断
		long money = XLogicWorld.SP.MainPlayer.GameMoney;
		if(money < needMoney)
		{
			string text = "金钱不足";
			XEventManager.SP.SendEvent(EEvent.MessageBox,null,null,text);
			return ;
		}
		
		//客户端直接开启，服务器做验证，免去服务器正确发包
		EItemBoxType tempType;
		ushort tempIndex;
		XItemManager.GetContainerType(curWillPos,out tempType,out tempIndex);
		if(tempType == EItemBoxType.Bag)
		{
			XCfgBagSpace cfgBagSpace = XCfgBagSpaceMgr.SP.GetConfig((uint)(curWillPos+1));
			if(cfgBagSpace == null)
				return ;

			for (uint i = curOpenPos; i <= curWillPos ;i++)
			{
				mSpaceArray.Set((int)i,true);
			}

			XEventManager.SP.SendEvent(EEvent.Bag_UpdateItemSpace,curOpenPos,curWillPos);
			//XLogicWorld.SP.MainPlayer.GameMoney	-= needMoney;
		}
		else if(tempType == EItemBoxType.Bank)
		{
			//XCfgBankSpace cfgBankSpace = XCfgBankSpaceMgr.SP.GetConfig((uint)(realPos+1 - XItemManager.GetBeginIndex(EItemBoxType.Equip)));
			//if(cfgBankSpace == null)
			//	return ;
			
			//ushort pageStartIndex	= (pBagSpace->PageID - 1) * EBAG_DATA.ONE_COM_BAG_SLOT_NUM;
			//ushort pageEndIndex		= pBagSpace->PageID * EBAG_DATA.ONE_COM_BAG_SLOT_NUM;
			//for (UINT16 i = pageStartIndex; i < pageEndIndex - 1;i++)
			//{
			//	mSpaceArray.Set(curWillPos,true);
			//	XEventManager.SP.SendEvent(EEvent.Bag_UpdateItemSpace,curWillPos,true);	
			//}
			//XLogicWorld.SP.MainPlayer.GameMoney	-= needMoney;
		}
		
		CS_ItemSpace.Builder builder = CS_ItemSpace.CreateBuilder();
		builder.SetItemIndex(curWillPos);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemSpace, builder.Build());

		curOpenPos	= curWillPos;
		curWillPos	= 0;
		needMoney	= 0;
	}
	
	private void OnClickCancel(GameObject go)
	{
		//none
	}
	
	public void Init(int[] array)
	{
		mSpaceArray	= new BitArray(array);	

		ushort startIndex = XItemManager.GetBeginIndex(EItemBoxType.Bag);
		ushort endIndex   = XItemManager.GetEndIndex(EItemBoxType.Bag);
		
		int count = 0;
		for(ushort i = startIndex; i <= endIndex; i++)
		{
			if(!IsSet((short)i))
			{
				curOpenPos = i;
				break;
			}	
		}
	}
	
	public bool IsSet(short pos)
	{
		if(mSpaceArray == null || pos >= mSpaceArray.Length)
			return false;
		
		return mSpaceArray[pos];
	}
	
	public bool IsSetPre(ushort pos)
	{		
		if(pos < (ushort)EBAG_DATA.ONE_COM_BAG_SLOT_NUM )
			return false;
		
		int pageNo = pos / (int)EBAG_DATA.ONE_COM_BAG_SLOT_NUM;
		int preIndex = pageNo * (int)EBAG_DATA.ONE_COM_BAG_SLOT_NUM;
		return mSpaceArray[preIndex - 1];		
	}
	
	public int GetSpace(EItemBoxType type)
	{
		ushort startIndex = XItemManager.GetBeginIndex(type);
		ushort endIndex   = XItemManager.GetEndIndex(type);
		
		int count = 0;
		for(ushort i = startIndex; i <= endIndex; i++)
		{
			if(IsSet((short)i))
				count++;
		}
		
		return count;
	}
}
