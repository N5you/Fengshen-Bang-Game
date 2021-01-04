
using System;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;
using System.Collections.Generic;
using UnityEngine;

// 帮会建筑
public class STGuildConstInfo
{
		public uint iConsLvl;
		public uint iConsTime;
}

public class STGuildBaseInfo
{
	public UInt64			uGuildId;
	public UInt64			uMasterId;	
	public String			cMasterName;
	public String			cGuildName;
	public String			cAnno;
	public String			cAnno2;
	public uint				uCreationTime;
	public uint				uMoney;
	public uint				uExp;
	public uint				uLvl;	
	public uint				uMemCount;
}

// 帮会申请信息
public class STApply
{
		public UInt64		uPlayerId;
		public String		cName;
		public uint 			uApplyTime;
		public uint			uLvl;
		public uint			uRank;
		public uint			uCombat;
	    public uint			uLeftTime;
}

// 帮会成员信息
public class STGuildMember
{
		public UInt64		uPlayerId;											
		public String		cName;												
		public uint			uRank;												
		public uint			uCurContr;	
		public uint			uTotalContr;
		public uint			uLeftTime;	
		public byte			cPos;	
}

// 帮会概要信息
public class STGuildSynInfo
{
		public UInt64		uGuildId;											
		public UInt64		uMasterId;											
		public String		cGuildName;										
		public String		cMasterName;
		public String		cAnno;
		public uint			uLvl;													
		public uint			uMemLen;	
}


public class XGuildManager : XSingleton<XGuildManager>
{
	public		uint 														m_GotGuildDataState					=		0;
	
	public		STGuildBaseInfo										m_stGuildBaseInfo					= 		new STGuildBaseInfo();

	// 帮会建筑数据
	public 		STGuildConstInfo[] 								m_stGuildConstInfo 				= 		new STGuildConstInfo [(uint)EGuildConstant.MAX_GUILD_CONST];
	// 帮会接口
	public		byte[] 													m_iInter 								= 		new byte[(uint)EGuildConstant.MAX_GUILD_INTER];
	// 申请
	public 		SortedList<UInt64, STApply> 					m_ApplyList							= 		new SortedList<UInt64, STApply>();
	// 所有成员
	public 		SortedList<UInt64, STGuildMember> 		m_MemberList						= 		new SortedList<UInt64, STGuildMember>();
	// 所有帮会概要信息
	public 		SortedList<UInt64,STGuildSynInfo> 			m_GuildSysList 		 				=		new SortedList<UInt64, STGuildSynInfo>();
	// 
	public 		ArrayList 												m_SelfApplyList 						= 		new ArrayList();
	
	public		SortedList<UInt64, UInt64>						m_GuildList							= 		new SortedList<UInt64, UInt64>();
	
	public XGuildManager()
	{
		for(int i = 0; i < (int)EGuildConstant.MAX_GUILD_CONST; i++)
		{
			m_stGuildConstInfo[i] = new STGuildConstInfo();
		}
		
	}
	// Request server==================================================================================================
	public void RequestAllGuildSynInfo()
	{
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestAllSynInfo, msg.Build() );
	}
	
	public void RequestGuildBaseInfo()
	{
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eSC_Guild_RequestBaseInfo, msg.Build() );
	}
	
	public void RequestGuildAllMemInfo()
	{
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eSC_Guild_RequestAllMemInfo, msg.Build() );
	}
	
	public void RequestGuildAllApplyInfo()
	{
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eSC_Guild_RequestAllApplyInfo, msg.Build() );
	}
	
	public void RequestGuildCreate(string guildName)
	{
		if(XLogicWorld.SP.MainPlayer.GuildId != 0)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid, 721);
			return;
		}
		if(XLogicWorld.SP.MainPlayer.GameMoney < (long)EGuildConstant.eCREATE_GUILD_MONEY)
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_SystemMid, 722);
			return;
		}
		
		CS_Guild_RequestCreate.Builder msg = CS_Guild_RequestCreate.CreateBuilder();
		msg.GuildName = guildName;
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestCreate, msg.Build() );
	}
	
	public void RequestApplyJoinGuild(UInt64 uGuildId)
	{
		m_SelfApplyList.Add(uGuildId);
		CS_UInt64.Builder msg = CS_UInt64.CreateBuilder();
		msg.SetData(uGuildId);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestApplyJoin, msg.Build() );
	}
	
	public void RequestCancelApplyJoin(UInt64 uGuildId)
	{
		m_SelfApplyList.Remove(uGuildId);
		CS_UInt64.Builder msg = CS_UInt64.CreateBuilder();
		msg.SetData(uGuildId);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestCancelApplyJoin, msg.Build() );
	}
	
	public void RequestHandleApply(UInt64 uPlayerId, uint uResult)
	{
		CS_Guild_RequestHandleApply.Builder msg = CS_Guild_RequestHandleApply.CreateBuilder();
		msg.SetPlayerId(uPlayerId);
		msg.SetResult(uResult);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestHandleApply, msg.Build() );
	}
	
	public void RequestLeave()
	{
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestLeave, msg.Build() );
	}	
	
	public void RequestRemove(UInt64 uPlayerId)
	{
		CS_UInt64.Builder msg = CS_UInt64.CreateBuilder();
		msg.SetData(uPlayerId);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestRemove, msg.Build() );
	}		
	
	public void RequestDisband()
	{
		if(XLogicWorld.SP.MainPlayer.GuildId == 0 && m_stGuildBaseInfo.uMasterId != XLogicWorld.SP.MainPlayer.ID)
			return;
		
		CS_Empty.Builder msg = CS_Empty.CreateBuilder();
		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestDisband, msg.Build() );
	}	
	
	public void RequestBroadAnno(uint uType, string Anno)
	{
		if(XLogicWorld.SP.MainPlayer.GuildId == 0 && m_stGuildBaseInfo.uMasterId != XLogicWorld.SP.MainPlayer.ID)
			return;
		
		CS_Guild_RequestBroadAnno.Builder msg = CS_Guild_RequestBroadAnno.CreateBuilder();
		msg.SetType(uType);
		msg.SetAnno(Anno);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestBroadAnno, msg.Build() );
	}	
	
	public void RequestTran(UInt64 uPlayerId)
	{
		if(XLogicWorld.SP.MainPlayer.GuildId == 0 && m_stGuildBaseInfo.uMasterId != XLogicWorld.SP.MainPlayer.ID)
			return;
		
		CS_Guild_RequestTran.Builder msg = CS_Guild_RequestTran.CreateBuilder();
		msg.SetPlayerId(uPlayerId);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Guild_RequestBroadAnno, msg.Build() );
	}	
	
	//====================================================================================================================
	
	//Recv Server============================================================================================================
	public void ON_SC_RecvAllSynInfo(SC_Guild_RespondAllSynInfo msg)
	{
		STGuildSynInfo 		stGuildSynInfo 	= null;
		SC_Guild_SynInfo   synInfo		  		= null;
		
		for (int i = 0; i < msg.GuildSynInfoCount; ++i)
        {
			synInfo 			 = msg.GetGuildSynInfo(i); 
			stGuildSynInfo = new STGuildSynInfo();
			
			stGuildSynInfo.uGuildId 			= synInfo.GuildId;
			stGuildSynInfo.uMasterId 		= synInfo.MasterId;
			stGuildSynInfo.cGuildName 		= synInfo.GuildName;
			stGuildSynInfo.cMasterName 	= synInfo.MasterName;
			stGuildSynInfo.cAnno				= synInfo.Anno;
			stGuildSynInfo.uLvl					= synInfo.ULvl;
			stGuildSynInfo.uMemLen			= synInfo.UMemLen;
			
			m_GuildSysList.Add(stGuildSynInfo.uGuildId, stGuildSynInfo);
        }
		SetGuildDataState((uint)EGuildConstant.eGET_ALL_GUILD_INFO);
		XEventManager.SP.SendEvent(EEvent.GuildList_UpdateGuildSynInfo);
	}
		
	public void ON_SC_RecvBroadAddSynInfo(SC_Guild_BroadAddSynInfo msg)
	{
		SC_Guild_SynInfo	guildInfo			= msg.GuildSynInfo;
		STGuildSynInfo 		stGuildSynInfo	= new STGuildSynInfo();
		
		stGuildSynInfo.uGuildId 	  			= guildInfo.GuildId;
		stGuildSynInfo.uMasterId		  		= guildInfo.MasterId;
		stGuildSynInfo.cGuildName		 	= guildInfo.GuildName;
		stGuildSynInfo.cMasterName	  	= guildInfo.MasterName;
		stGuildSynInfo.cAnno  				= guildInfo.Anno;
		stGuildSynInfo.uLvl	  					= guildInfo.ULvl;
		stGuildSynInfo.uMemLen				= guildInfo.UMemLen;
		
		m_GuildSysList.Add(stGuildSynInfo.uGuildId, stGuildSynInfo);
		XEventManager.SP.SendEvent(EEvent.GuildList_UpdateGuildSynInfo);
	}
	
	public void ON_SC_RecvBroadRemoveSynInfo(SC_UInt64 msg)
	{
		UInt64 uGuildId = msg.Data;
		
		m_MemberList.Remove(uGuildId);
		XEventManager.SP.SendEvent(EEvent.GuildList_UpdateGuildSynInfo);
	}
	
	public void ON_SC_RecvUpdateSynInfoList()
	{

	}
	
	public void ON_SC_RecvBaseInfo(SC_Guild_RespondBaseInfo msg)
	{
		m_stGuildBaseInfo.uGuildId 				= msg.GuildId;
		m_stGuildBaseInfo.uMasterId			= msg.MasterId;
		m_stGuildBaseInfo.cMasterName		= msg.MasterName;
		m_stGuildBaseInfo.cGuildName		= msg.GuildName;
		m_stGuildBaseInfo.cAnno				= msg.Anno;
		m_stGuildBaseInfo.cAnno2				= msg.Anno2;
		m_stGuildBaseInfo.uCreationTime 	= msg.CreationTime;
		m_stGuildBaseInfo.uMoney				= msg.Money;
		m_stGuildBaseInfo.uExp					= msg.Exp;
		m_stGuildBaseInfo.uLvl					= msg.Lvl;
		m_stGuildBaseInfo.uMemCount		= msg.MemCount;
		for(int i = 0; i < msg.InterCount; i++)
		{
			m_iInter[i] = (byte)msg.GetInter(i);
		}
		
		SC_Guild_ConstInfo constInfo = null;
		for(int i = 0; i < msg.GuildConstInfoCount; i++)
		{
			constInfo									=	msg.GetGuildConstInfo(i);
			m_stGuildConstInfo[i].iConsLvl 	= constInfo.ConsLvl;
			m_stGuildConstInfo[i].iConsTime = constInfo.ConsTime;
		}
		SetGuildDataState((uint)EGuildConstant.eGET_BAS_GUILD_INFO);
		XEventManager.SP.SendEvent(EEvent.GuildMain_UpdateInfo);
	}
	
	public void ON_SC_RecvAllMemInfo(SC_Guild_RespondAllMemInfo msg)
	{
		SC_Guild_MemInfo memInfo  = null;
		STGuildMember		guildMem = null;
		for(int i = 0; i < msg.GuildMemInfoCount; i++)
		{
			memInfo = msg.GetGuildMemInfo(i);
			guildMem = new STGuildMember();
			
			guildMem.uPlayerId 	  	= memInfo.PlayerId;
			guildMem.cName		  	= memInfo.Name;
			guildMem.uRank		 	  	= memInfo.Rank;
			guildMem.uCurContr	  	= memInfo.CurContr;
			guildMem.uTotalContr  	= memInfo.TotalContr;
			guildMem.uLeftTime	  	= memInfo.LeftTime;
			guildMem.cPos				= (byte)memInfo.Pos;
			
			m_MemberList.Add(guildMem.uPlayerId, guildMem);
		}
		SetGuildDataState((uint)EGuildConstant.eGET_MEM_GUILD_INFO);
		XEventManager.SP.SendEvent(EEvent.GuildInfo_UpdateMemInfo);
	}
	

	public void ON_SC_RecvAllApplyInfo(SC_Guild_RespondAllApplyInfo msg)
	{
		STApply	 					stApply 	= null;
		SC_Guild_Apply 			scApply = null;
		for(int i = 0; i < msg.ApplyCount; i++)
		{
			scApply 		= msg.GetApply(i);
			stApply 		= new STApply();
			
			stApply.uPlayerId		= scApply.PlayerId;
			stApply.cName			= scApply.Name;
			stApply.uApplyTime 	= scApply.ApplyTime;
			stApply.uLvl  				= scApply.Lvl;
			stApply.uRank  			= scApply.Rank;
			stApply.uCombat  		= scApply.Combat;
			stApply.uLeftTime  		= scApply.LeftTime;
			
			m_ApplyList.Add(stApply.uPlayerId, stApply);
		}
		SetGuildDataState((uint)EGuildConstant.eGET_APY_GUILD_INFO);
		XEventManager.SP.SendEvent(EEvent.GuildInfo_UpdateApplyInfo);
	}
	
	public void ON_SC_RecvBroadAddMem(SC_Guild_BroadAddMem msg)
	{
		SC_Guild_MemInfo memInfo  = msg.GuildMemInfo;;
		STGuildMember		guildMem = new STGuildMember();
		
		guildMem.uPlayerId 	  	= memInfo.PlayerId;
		guildMem.cName		  	= memInfo.Name;
		guildMem.uRank		 	  	= memInfo.Rank;
		guildMem.uCurContr	  	= memInfo.CurContr;
		guildMem.uTotalContr  	= memInfo.TotalContr;
		guildMem.uLeftTime	  	= memInfo.LeftTime;
		guildMem.cPos				= (byte)memInfo.Pos;
		
		m_MemberList.Add((uint)guildMem.uPlayerId, guildMem);
		XEventManager.SP.SendEvent(EEvent.GuildInfo_UpdateMemInfo);
	}
		
	public void ON_SC_RecvBroadRemoveMem(SC_Guild_BroadRemoveMem msg)
	{
		UInt64 	uPlayerId 	= msg.PlayerId;
		uint 		Type 		= msg.Type;
		m_MemberList.Remove(uPlayerId);
		
		XEventManager.SP.SendEvent(EEvent.GuildInfo_UpdateMemInfo);
	}

	public void ON_SC_RecvUpdateMemInfoList()
	{
		
	}
	
	public void ON_SC_RecvSelfApplyList(SC_Guild_SendSelfApplyList msg)
	{
		for(int i = 0; i < msg.GuildIdCount; i++)
		{
			m_SelfApplyList.Add(msg.GetGuildId(i));
		}
	}
	
	public void ON_SC_RecvBroadAddApply(SC_Guild_BroadAddApply msg)
	{
		SC_Guild_Apply  scApply 	= msg.Apply;
		STApply				stApply 		= new STApply();
		
		stApply.uPlayerId		= scApply.PlayerId;
		stApply.cName			= scApply.Name;
		stApply.uApplyTime 	= scApply.ApplyTime;
		stApply.uLvl  				= scApply.Lvl;
		stApply.uRank  			= scApply.Rank;
		stApply.uCombat  		= scApply.Combat;
		stApply.uLeftTime  		= scApply.LeftTime;
		
		m_ApplyList.Add(stApply.uPlayerId, stApply);
		
		XEventManager.SP.SendEvent(EEvent.GuildInfo_UpdateApplyInfo);
	}
	
	public void ON_SC_RecvBroadRemoveApply(SC_UInt64 msg)
	{
		UInt64 uPlayerId = (UInt64)msg.Data;
		m_ApplyList.Remove(uPlayerId);
		
		XEventManager.SP.SendEvent(EEvent.GuildInfo_UpdateApplyInfo);
	}
	
	public void ON_SC_RecvUpdateApplyInfoList()
	{
		
	}
	
	public void ON_SC_RecvFeedback(SC_UInt msg)
	{
		uint uValue = msg.Data;
		switch(uValue)
		{
		case (uint)EGuildConstant.eGUILD_CREATE_SUCCESS:
			
			break;
		case (uint)EGuildConstant.eGUILD_CREATE_FAILED:
				
			break;
		case (uint)EGuildConstant.eGUILD_REFUSALED_APPLY:
				
			break;
		case (uint)EGuildConstant.eGUILD_DISBAND_GUILD:
			
			break;
		default:    break;
		}
	}
	
	
	public void ON_SC_Guild_BroadAnno(SC_Guild_BroadAnno msg)
	{
		uint type = msg.Type;
		string anno = msg.Anno;
		
		if(type == (uint)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO1)
		{
			m_stGuildBaseInfo.cAnno = anno;
		}
		else if(type == (uint)EGuildConstant.eGUILDMaintain_SELECT_PAGE_ANNO2)
		{
			m_stGuildBaseInfo.cAnno2 = anno;
		}
		
		XEventManager.SP.SendEvent(EEvent.GuildMaintain_UpdateAnno);
		XEventManager.SP.SendEvent(EEvent.GuildMain_UpdateInfo);
	}
	
	public void ON_SC_Guild_BroadTran(SC_Guild_BroadTran msg)
	{
		UInt64 uPlayerId = (UInt64)msg.PlayerId;
		string name = msg.MasterName;
		
		m_stGuildBaseInfo.uMasterId = uPlayerId;
		m_stGuildBaseInfo.cMasterName = name;
		
		XEventManager.SP.SendEvent( EEvent.GuildMain_UpdateInfo);
	}
	
	//====================================================================================================================
	public bool CheckGuildDataState(uint eState)
	{
		if((m_GotGuildDataState & eState) != 0)
			return true;
		
		return false;
	}
	
	public void SetGuildDataState(uint eState)
	{
		m_GotGuildDataState |= eState;
	}
	
	public void RemoveGuildDataState(uint eState)
	{
		m_GotGuildDataState &= ~eState;
	}
	
	public void ClearGuildDataState()
	{
		m_GotGuildDataState = 0;
	}
	
	public bool HasApplyId(UInt64 uGuildId)
	{
		return m_SelfApplyList.Contains(uGuildId);
	}
	
	
	public UInt64 GetGuildMemId(string name)
	{
		STGuildMember  stGuildMember = null;
		foreach(KeyValuePair<UInt64, STGuildMember> kvpItem in m_MemberList)
		{
			stGuildMember = kvpItem.Value;
			if(stGuildMember.cName == name)
				return stGuildMember.uPlayerId;
		}
		
		return 0;
	}
	
}


