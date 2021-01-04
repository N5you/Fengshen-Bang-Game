using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

public enum EGuildConstant
{
	eMAX_GUILD_SHOW_COUNT			= 8,
	eMAX_GUILDMEM_SHOW_COUNT 	= 8,
	eMAX_GUILDAPPLY_SHOW_COUNT 	= 8,
	
	eApplyState_Normal	= 1,
	eApplyState_Apply		= 2,
	
	eGET_ALL_GUILD_INFO   = 1 << 0,
	eGET_BAS_GUILD_INFO  = 1 << 1, 
	eGET_MEM_GUILD_INFO = 1 << 2,
	eGET_APY_GUILD_INFO  = 1 << 3,
	
	ePlayerId_null 					= 0,
	MAX_GUILD_CONST 			= 3,
	MAX_GUILD_INTER  			= 3,
	MAX_PLAYER_NAME_LEN 	= 40,
	MAX_GUILD_APPLY_NUM 	= 50,
	MAX_GUILD_MEM_NUM		= 50,
	
	eGUILD_CREATE_SUCCESS		= 1,
	eGUILD_CREATE_FAILED		    = 2,
	eGUILD_REFUSALED_APPLY		= 3,
	eGUILD_DISBAND_GUILD		    = 4,
	eCREATE_GUILD_MONEY			= 100000,
	
	eGUILD_AGREE_JOIN   			= 1,			// 同意加入
	eGUILD_REFUSAL_JOIN 			= 2,			// 拒绝加入
	
	GJ_NORMAL = 1,
	GJ_BRAINMAN = 2,	
	GJ_HEADER = 3,	
	
	eGUILDINFO_SELECT_PAGE_MEM = 0,
	eGUILDINFO_SELECT_PAGE_APPLY = 3,
	
	eGUILDMaintain_SELECT_PAGE_ANNO2 = 0,
	eGUILDMaintain_SELECT_PAGE_ANNO1 = 1
}

