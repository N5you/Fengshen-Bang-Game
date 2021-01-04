using System;
using System.Collections.Generic;

public class HyperLinkMgr
{
	private static HyperLinkMgr m_this = new HyperLinkMgr();
	public static HyperLinkMgr SP {get { return m_this; }}
	
	private SortedDictionary<ELinkType,HyperLinkBase>	mDirectory = new SortedDictionary<ELinkType,HyperLinkBase>();
	
	private HyperLinkMgr()
	{
		Init();
	}
	
	public void Init()
	{
		HyperLinkName	linkName = new HyperLinkName();
		mDirectory.Add(ELinkType.ELink_Type_Name,linkName);
		
		HyperLinkItem	linkItem = new HyperLinkItem();
		mDirectory.Add(ELinkType.ELink_Type_Item,linkItem);
		
		HyperLinkNavGo linkObject = new HyperLinkNavGo();
		mDirectory.Add(ELinkType.ELink_Type_NavGo, linkObject);
		
		HyperLinkNavPos linkPos = new HyperLinkNavPos();
		mDirectory.Add(ELinkType.ELink_Type_NavPos, linkPos);
		
		HyperLinkDailyPlaySign linkSign = new HyperLinkDailyPlaySign();
		mDirectory.Add(ELinkType.ELink_Type_DailyPlaySign, linkSign);
	}
	
	public void Process(string linkStr)
	{
		HyperLinkBase linkBase = ParseLinkData(linkStr);
		if(linkBase != null)
			linkBase.HandleClickLink();
	}
	
	public HyperLinkBase ParseLinkData(string linkData)
	{
		string baseStr = linkData;
		string tempStr = "";
		
		ELinkType type = ELinkType.ELink_Type_None;
		
		if(HyperLinkBase.GetClampStr(ref baseStr,ref tempStr,"T(",")",0))
			type	= (ELinkType)Convert.ToInt32(tempStr);
		
		if(mDirectory.Count == 0)
			return null;
		
		if(mDirectory.ContainsKey(type))
		{
			mDirectory[type].ParseLinkInfo(baseStr);
			return mDirectory[type];
		}		
		return null;
	}
	
}