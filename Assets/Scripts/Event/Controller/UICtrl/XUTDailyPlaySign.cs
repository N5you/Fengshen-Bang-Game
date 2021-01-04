using UnityEngine;
using XGame.Client.Packets;
using System.Collections.Generic;

class XUTDailyPlaySign : XUICtrlTemplate<XDailyPlaySign>
{
	public XUTDailyPlaySign()
	{
		RegEventAgent_CheckCreated(EEvent.DailyPlaySign_RefreshUI, OnRefreshUI);
		
		RegEventAgent_CheckCreated(EEvent.DailyPlaySign_UpdateText, OnUpdateShowText);
		
		m_allShowData = new SortedList<uint, int>();
	}

	private void OnRefreshUI(EEvent evt, params object[] args)
    {
        if (null == LogicUI) 
			return;
		
		LogicUI.Reset();
		m_allShowData.Clear();
		
		int leftCount = XDailyPlaySignMgr.SP.m_allShowData.Count;
		int showCount = 0;
		for ( int i = 0; i < leftCount; i++ )
		{
			if ( showCount >= 9 )
				return;
			
			SItemDailyPlay item = XDailyPlaySignMgr.SP.m_allShowData.Values[i];
			
			if ( !XDailyPlaySignMgr.SP.CheckCanShow(item.key) )
				continue;
			
			LogicUI.SetLabelShowData(showCount, item.text);
			m_allShowData[item.key] = showCount;
			showCount++;
		}
    }
	
	private void OnUpdateShowText(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		
		uint key = (uint)args[0];
		string text = (string)args[1];
		
		if ( !XDailyPlaySignMgr.SP.CheckCanShow(key) )
				return;
		
		if ( m_allShowData.ContainsKey(key) )
		{
			LogicUI.LabelTestShowObj[m_allShowData[key]].text = text;
		}
	}
	
	public SortedList<uint, int> m_allShowData;
}
