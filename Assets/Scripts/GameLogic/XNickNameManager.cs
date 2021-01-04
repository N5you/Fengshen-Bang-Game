using UnityEngine;
using System;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
public class XNickNameInfo
{
	public uint nID;
	public string name;
	public int colorID;
}

public class XNickNameManager : XSingleton<XNickNameManager>
{
	//最大称号个数，需要与服务器同步，可以考虑放置公共的配置管理中
	public static readonly uint MAX_NICKNAME_LIST_NUM = 500;

	//已获得的称号列表
	private SortedList<uint, XNickNameInfo> m_nickNameList;

	//当前使用的称号ID
	private uint m_curNickNameID;

	//当前使用的称号名称
	private XNickNameInfo m_curNickName;

	public XNickNameManager ()
	{
		m_curNickNameID = 0;
		m_nickNameList = new SortedList<uint, XNickNameInfo> ();
	}

	private void addNickNameInfo(XNickNameInfo info)
	{
		if (info == null)
			return;
		if (!m_nickNameList.ContainsKey (info.nID)) {
			m_nickNameList.Add (info.nID, info);
		}
		else {
			Log.Write (LogLevel.WARN, "XNickNameManager, addNickNameInfo, the nID is in m_NickNameList");
		}
	}

	private void delNickNameInfo(uint nID)
	{
		if (m_nickNameList.ContainsKey (nID)) {
			m_nickNameList.Remove (nID);
		}
		else {
			Log.Write (LogLevel.WARN, "delNickNameInfo, the nID is not in m_NickNameList");
		}
	}

	public XNickNameInfo GetNickNameInfoFromCfg(uint nID)
	{
		XCfgNickName m_XCfgNickName = XCfgNickNameMgr.SP.GetConfig (nID);
		if (m_XCfgNickName == null) {
			Log.Write (LogLevel.ERROR, "XNickNameManager, GetNickNameInfoFromCfg, XCfgNickName not have nID:" + nID);
			return null;
		}
		else {
			XNickNameInfo tempInfo = new XNickNameInfo ();
			tempInfo.colorID = m_XCfgNickName.ColorID;
			tempInfo.name = m_XCfgNickName.NickName;
			tempInfo.nID = m_XCfgNickName.nID;
			return tempInfo;
		}
	}

	public XNickNameInfo GetNickNameInfoFromlist(uint nID)
	{
		XNickNameInfo tempNickNameInfo = new XNickNameInfo ();
		if (m_nickNameList != null) {
			if (m_nickNameList.ContainsKey (nID)) {
				tempNickNameInfo.nID = nID;
				tempNickNameInfo.name = m_nickNameList [nID].name;
				tempNickNameInfo.colorID = m_nickNameList [nID].colorID;
				return tempNickNameInfo;
			}else
			{
				Log.Write (LogLevel.ERROR, "XNickNameManager, GetNickNameInfoFromlist, m_nickNameList not have nID");
				return null;
			}
		}
		return null;
	}
	
	public SortedList<uint, XNickNameInfo> GetNickNameList()
	{
		return m_nickNameList;
	}
	
	public string GetCurNickNameStr()
	{
		string temp = "";
		XNickNameInfo s = new XNickNameInfo();
		s = this.GetNickNameInfoFromCfg(m_curNickNameID);
		temp = s.name;
		return temp;
	}

	//设置当前当前使用的称号
	public void HandleSetCurNickName(uint nID)
	{
		XCfgNickName m_XCfgNickName = XCfgNickNameMgr.SP.GetConfig (nID);
		if (m_XCfgNickName == null) {
			Log.Write (LogLevel.WARN, "配置文件XCfgNickName中不存在ID");
			return;
		}
		
		if (nID != 0) {
			if (!m_nickNameList.ContainsKey (nID)) {
				Log.Write (LogLevel.WARN, "curNickName nID not in nickNameList");
				return;
			}
		}
		
		if (this.m_curNickNameID == nID) {
			//Log.Write (LogLevel.INFO, string.Format ("NID:{0} is equal to CurNickNameID", nID));
			return;
		}

		CS_NickName_Info.Builder builder = CS_NickName_Info.CreateBuilder ();
		builder.SetNID (nID);
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_NickName_SetCurNickName, builder.Build ());
	}
	
	public void SetCurNickName(uint nID)
	{
		if (nID == 0) {
			XLogicWorld.SP.MainPlayer.NickName = "";
		}
		else {
			XNickNameInfo infoData = this.GetNickNameInfoFromlist (nID);
			if (infoData != null)
				try {
					string colorname = XGameColorDefine.Quality_Color [infoData.colorID];
					XLogicWorld.SP.MainPlayer.NickName = string.Format("{0}{1}",colorname,infoData.name);
				}
				catch {
					Log.Write (LogLevel.ERROR, "XNickNameManager, the index is out of Quality_Color num");
				}
		}
		m_curNickNameID = nID;
	}

	public void On_SC_ReciveAddNickName(uint nID)
	{
		XNickNameInfo infoData = this.GetNickNameInfoFromCfg (nID);
		if (infoData != null)
			this.addNickNameInfo (infoData);
	}

	public void On_SC_ReciveDelNickName(uint nID)
	{
		this.delNickNameInfo (nID);
		SetCurNickName(0);
	}

	public void On_SC_ReciveAllData(SC_NickName_AllData msg)
	{
		for (int cnt = 0; cnt != msg.AwardedNickNameListCount; ++cnt) {
			XNickNameInfo infoData = this.GetNickNameInfoFromCfg (msg.AwardedNickNameListList [cnt].NID);
			if (infoData == null)
				continue;
			this.addNickNameInfo (infoData);
		}

		if (msg.HasCurID) {
			uint nid = msg.CurID;
			this.SetCurNickName (nid);
		}
		else {
			Log.Write (LogLevel.DEBUG, "XNickNameManager, the msg.curID not have value");
		}
	}

	public void ON_SC_ReciveSetCurNickName(uint nID)
	{
		this.SetCurNickName (nID);
	}
}
