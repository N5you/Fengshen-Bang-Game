using System;
using UnityEngine;
using System.Collections;

public enum EMouseState
{
	EMouseState_None,
	EMouseState_Split,
	EMouseState_Seal,
	EMouseState_Num,
}

public class XMouseOp
{
	public virtual void OnClick(ushort itemIndex) {}
}

public class XItemSplit : XMouseOp
{
	public override void OnClick(ushort itemIndex)
	{
		XItem targetItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(itemIndex);
		if(targetItem.IsEmpty())
		{
			XItemPacketProcess.SP.SendItemSplit(SrcItemIndex,SplitItemCount,itemIndex);
			return ;
		}
		else
		{
			bool isCan = XLogicWorld.SP.MainPlayer.ItemManager.IsCanPile(SrcItemIndex,itemIndex,(ushort)SplitItemCount);
			if(isCan)
			{
				XItemPacketProcess.SP.SendItemSplit(SrcItemIndex,SplitItemCount,itemIndex);
				return ;
			}
		}
		
		XMouseStateMgr.SP.mouseState	= EMouseState.EMouseState_None;
		CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_None);
		XEventManager.SP.SendEvent(EEvent.Cursor_ClearIcon);
	}
	
	public ushort 	SrcItemIndex;
	public int		SplitItemCount;
}

public class XItemSeal : XMouseOp
{
	public override void OnClick(ushort itemIndex)
	{
		XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem(itemIndex);
		if(item.IsEmpty())
			return ;
		
		if(!item.IsSeal)
		{
			XEventManager.SP.SendEvent(EEvent.Chat_Notice,XStringManager.SP.GetString(61));
			return ;
		}
		
		XItemPacketProcess.SP.SendItemSeal(itemIndex);
	}
}

public class XMouseStateMgr
{
	private static XMouseStateMgr	mgr	= new XMouseStateMgr();
	public static XMouseStateMgr	SP {get{ return mgr; }}	
	
	public EMouseState mouseState	= EMouseState.EMouseState_None;	
	
	private XMouseStateMgr()
	{
		Init();		
	}
	
	private XMouseOp[]	clickEventObj	= new XMouseOp[(int)EMouseState.EMouseState_Num];
	
	private void Init()
	{
		clickEventObj[(int)EMouseState.EMouseState_None]	= new XMouseOp();
		clickEventObj[(int)EMouseState.EMouseState_Split]	= new XItemSplit();
		clickEventObj[(int)EMouseState.EMouseState_Seal]	= new XItemSeal();
	}
	
	public void Click(ushort itemIndex)
	{
		if(mouseState > EMouseState.EMouseState_None && mouseState < EMouseState.EMouseState_Num)
			clickEventObj[(int)mouseState].OnClick(itemIndex);
	}
	
	
	public XMouseOp	GetMouseOp(EMouseState state)
	{
		if(state > EMouseState.EMouseState_None && state < EMouseState.EMouseState_Num)
			return clickEventObj[(int)state];
		
		return null;
	}
	
	
}
