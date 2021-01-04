using System;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;
using UnityEngine;

public class XCharmRankInfo
{
	public ulong Rank;
	public string PlayerName;
	public ulong Flowers;
	
	public XCharmRankInfo ()
	{
		this.Rank = 0;
		this.PlayerName = "";
		this.Flowers = 0;
	}	
	public XCharmRankInfo (ulong rank, string name, uint flowers)
	{
		this.Rank = rank;
		this.PlayerName = name;
		this.Flowers = flowers;
	}
}

public class XCharmRankManager : XSingleton<XCharmRankManager> , IComparer<XCharmRankInfo>
{
	public static int MAX_RANKINFO_DATA_NUM;
	public List<XCharmRankInfo> m_rankInfoList;
	
	public XCharmRankManager ()
	{
		m_rankInfoList = new List<XCharmRankInfo> ();
		/*
		dataList.Add (new XCharmRankInfo (1, "Peter", 122));
		dataList.Add (new XCharmRankInfo (2, "NeedForSpeed", 2131));
		dataList.Add (new XCharmRankInfo (3, "RealRacing3", 324));
		dataList.Add (new XCharmRankInfo (4, "PinYIN", 234234));
		dataList.Add (new XCharmRankInfo (5,"HWHWHW", 234234));
		dataList.Add (new XCharmRankInfo (6, "AAAAFFF", 1232131));
		dataList.Add (new XCharmRankInfo (7, "QingZuiHao", 99999));
		dataList.Add (new XCharmRankInfo (8, "BSERBSER", 34234));
		dataList.Add (new XCharmRankInfo (9, "ASPPSA", 100000));
		dataList.Add (new XCharmRankInfo (10, "YUNSI", 23141));
		dataList.Add (new XCharmRankInfo (11, "HAHAHAHA", 1234));
		dataList.Add (new XCharmRankInfo (12, "王码五笔", 1123412));
		dataList.Add (new XCharmRankInfo (13, "Unity3D", 1341234));
		dataList.Add (new XCharmRankInfo (14, "Invictus", 723214));
		dataList.Add (new XCharmRankInfo (15, "Racer", 341234));
		if (dataList.Count > 0) {
			Comparison<XCharmRankData> sorter = new Comparison<XCharmRankData> (this.Compare);
			dataList.Sort (sorter);
		}
		*/
	}
	
	public uint GetDataCount()
	{
		if (this.m_rankInfoList != null) {
			if (this.m_rankInfoList.Count > 0)
				return (uint)this.m_rankInfoList.Count;
			else
				return 0;
		}
		else {
			Log.Write (LogLevel.ERROR, "XCharmRankManager GetDataCount, rankInfoList is null");
			return 0;
		}
	}
	
	public XCharmRankInfo GetData(int idx) 
	{
		XCharmRankInfo rankInfo = new XCharmRankInfo();
		if (idx < 0) {
			Log.Write (LogLevel.ERROR, "XCharmRankManager GetData, Index不能小于0");
		}
		if (this.m_rankInfoList != null) {
			
			rankInfo.Rank = m_rankInfoList [idx].Rank;
			rankInfo.PlayerName = m_rankInfoList[idx].PlayerName;
			rankInfo.Flowers = m_rankInfoList[idx].Flowers;
			return rankInfo;
		}
		else {
			Log.Write (LogLevel.ERROR, "XCharmRankManager GetData, rankInfoList is null");
			return null;
		}
	}
	

	public void On_SC_ReciveData(SC_Friend_Flower_Rank msg)
	{
		
		if(msg.RankListCount>0)
		{
			m_rankInfoList.Clear();
			for(int cnt = 0;cnt != msg.RankListCount; ++cnt)
			{
				XCharmRankInfo rankInfo = new XCharmRankInfo();
				rankInfo.PlayerName = msg.RankListList[cnt].Name;
				rankInfo.Flowers = msg.RankListList[cnt].Flower;
				rankInfo.Rank = msg.RankListList[cnt].Rank;
				m_rankInfoList.Add(rankInfo);
			}
			
			XEventManager.SP.SendEvent(EEvent.Friend_UpdateRankInfo);
		}
	}
	
	#region IComparer[XCharmRankData] implementation
	public int Compare(XCharmRankInfo x, XCharmRankInfo y)
	{

		if (x is XCharmRankInfo && y is XCharmRankInfo) {
			if (x.Flowers > y.Flowers) {
				return -1;
			}
			else if (x.Flowers < y.Flowers) {
					return 1;
				}
			return 0;
		}
		throw new NotImplementedException ();
	}
	#endregion
}


