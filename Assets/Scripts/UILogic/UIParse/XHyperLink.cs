using UnityEngine;
using System;
using XGame.Client.Packets;

public enum ELinkType
{
	ELink_Type_None,			// 空
	ELink_Type_Item,			// 物品超链接, 格式:开启[link=1][linkdata=T(1)D1(userid)D2(itemid)]结束[link=0]
	ELink_Type_Name,			// 名称超链接,格式:开启[link=1][linkdata=T(2)N(name)D(id)] 结束[link=0]
	ELink_Type_NavGo,			// 导航对象，格式: 开启[link=1][linkdata=T(3)D(sceneID,ObjectType,id,posX,posY,posZ)]...[link=0]
	ELink_Type_NavPos,			// 导航坐标,	格式:开启[link=1][linkdata=T(4)D(sceneID,posX,posY,posZ)](posX,posY,posZ)[link=0]
	ELink_Type_DailyPlaySign,	// 每日玩法提醒链接,	格式:开启[link=1][linkdata=T(5)D(key)[link=0]
	ELink_Type_Num
}
	

public class HyperLinkBase
{
	public virtual void HandleClickLink() {}
	public virtual void ParseLinkInfo(string linkData) {}
	
	public static bool GetClampStr(ref string baseStr,ref string clampStr,string findStart,string findEnd, int startPos )
	{
		int beginPos 	= baseStr.IndexOf(findStart, startPos);
		int endPos		= 0;
		
		if(beginPos == -1)
			return false;
		
		beginPos	+= findStart.Length;
		endPos	= baseStr.IndexOf(findEnd,beginPos);
		
		if(endPos == -1)
			return false;
		
		clampStr	= baseStr.Substring(beginPos,endPos - beginPos);
		baseStr		= baseStr.Substring(0, beginPos - findStart.Length) + baseStr.Substring(endPos + findEnd.Length);
		
		return true;
	}
}


public class HyperLinkName :  HyperLinkBase
{
	public override void HandleClickLink()
	{
		if ( mId == XLogicWorld.SP.MainPlayer.ID )
			return;
		
		XUIPopMenu.isOper = true;
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.ePopMenu);
		XEventManager.SP.SendEvent(EEvent.PopMenu_NameData, mName, mId);
		
	}
	public override void ParseLinkInfo(string linkData)
	{
		mName = "";
		string tempStr = "";
		if(GetClampStr(ref linkData,ref tempStr,"N(",")",0))
			mName= tempStr;
		if(GetClampStr(ref linkData, ref tempStr, "D(", ")",0))
			mId = ulong.Parse(tempStr);
	}
	
	private ulong mId = 0ul;
	private string mName	= "";
}

public class HyperLinkItem : HyperLinkBase
{
	private ulong userid = 0;
	private ulong itemid = 0;
	
	public override void HandleClickLink()
	{	
		XUIPopMenu.isOper = true;
		CS_ItemShow.Builder build = CS_ItemShow.CreateBuilder();
		build.SetUid(userid);
		build.SetItemid(itemid);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ItemShow, build.Build());
	}
	
	public override void ParseLinkInfo(string linkData)
	{
		string useridStr = "";
		string itemidStr = "";
		if ( GetClampStr(ref linkData, ref useridStr,"D1(", ")",0) )
			userid = ulong.Parse(useridStr);
		
		if ( GetClampStr(ref linkData, ref itemidStr, "D2(", ")",0) )
			itemid = ulong.Parse(itemidStr);
	}
}

public class HyperLinkNavGo : HyperLinkBase
{
	private uint sceneId = 0;
	private uint duplicateId = 0;
	private EObjectType objectType = EObjectType.Begin;
	private int id = 0;
	private float fx;
	private float fy;
	private float fz;
	
	public override void HandleClickLink ()
	{
		if( 0!=duplicateId && duplicateId == XLogicWorld.SP.SubSceneManager.SubSceneID ) //need navigate kill mode
		{
			XLogicWorld.SP.MainPlayer.NavigateKill(duplicateId );
		}else
		{
			XLogicWorld.SP.MainPlayer.NavigateTo(new XMainPlayerStateNavigate.NavigateInfo(sceneId, objectType, id, new Vector3(fx, fy, fz),duplicateId));
		}
		
	}
	
	public override void ParseLinkInfo (string linkData)
	{
		string tempStr = "";
		if(GetClampStr(ref linkData, ref tempStr, "D(",")",0))
		{
			string[] content = tempStr.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
			uint uintVal;
			if(!uint.TryParse(content[0], out uintVal))
				return;
			sceneId = uintVal;
			
			if(!uint.TryParse(content[1], out uintVal))
				return;
			duplicateId = uintVal;
			
			int intVal;
			if(!int.TryParse(content[2], out intVal))
				return;
			objectType = (EObjectType)(intVal);
			
			if(!int.TryParse(content[3], out intVal))
				return;
			id = intVal;
			
			float fVal;
			if(!float.TryParse(content[4], out fVal))
				return;
			fx = fVal;
			
			if(!float.TryParse(content[5], out fVal))
				return;
			fy = fVal;
			
			if(!float.TryParse(content[6], out fVal))
				return;
			fz = fVal;
		}
	}
	
}

public class HyperLinkNavPos : HyperLinkBase
{
	private uint sceneId;
	private float fx;
	private float fy;
	private float fz;
	
	public override void HandleClickLink ()
	{
		XLogicWorld.SP.MainPlayer.NavigateTo(new XMainPlayerStateNavigate.NavigateInfo(sceneId, new Vector3(fx, fy, fz)));
	}
	
	public override void ParseLinkInfo (string linkData)
	{
		string tempStr = "";
		if(GetClampStr(ref linkData, ref tempStr, "D(",")",0))
		{
			string[] content = tempStr.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
			uint uintVal;
			if(!uint.TryParse(content[0], out uintVal))
				return;
			sceneId = uintVal;
			
			float fVal;
			if(!float.TryParse(content[1], out fVal))
				return;
			fx = fVal;
			
			if(!float.TryParse(content[2], out fVal))
				return;
			fy = fVal;
			
			if(!float.TryParse(content[3], out fVal))
				return;
			fz = fVal;
		}

	}
}

public class HyperLinkDailyPlaySign : HyperLinkBase
{
	private uint key = 0;
	
	public override void HandleClickLink ()
	{
		XDailyPlaySignMgr.SP.HandleClickLink(key);
	}
	
	public override void ParseLinkInfo (string linkData)
	{
		string tempStr = "";
		if(GetClampStr(ref linkData, ref tempStr, "D(",")",0))
		{
			key = uint.Parse(tempStr);
		}
	}
}
