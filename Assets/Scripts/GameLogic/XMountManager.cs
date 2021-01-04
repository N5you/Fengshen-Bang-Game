using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

public enum EMountType
{
	Zhan = 1,
	Zuo ,
	Fei
}

public enum PeiYangResult
{
	Result_Success					= 0,			// 培养成功

	Result_ConfigNull				= 1,			// 当前等级配置为空
	Result_CountZero				= 2,			// 当前可培养次数为0
	Result_NotEnough_GameMoney		= 3,			// 游戏币不足
	Result_NotEnough_RealMoney		= 4,			// 元宝不足
	Result_MountExist				= 5,			// 坐骑已存在，添加失败

	Result_UnKnow					= 10,			// 未知错误
};

public enum CritResult
{
	Crite_False						= 0,			//未暴击
	Crite_True						= 1,			// 暴击
};

public enum PeiYangType
{
	PeiYang_Normal					= 1,			// 普通培养
	PeiYang_Super					= 2				// 超级培养			
};

public class XMount
{
	public XMount(ushort wIndex)
	{
		MountIndex = wIndex;
	}
	public ushort MountIndex;
}

public class XMountManager : XSingleton<XMountManager>
{
	public int m_nEquipMount = 0;
	public int m_nPrevEquipMount = 0;
	public int m_canPeiYangCount = 0;
	public int m_nEquipPos = 0;
	public int m_nMountLevel = 0;
	public int m_nCurrExp = 0;
	public SortedList<int, XMount> m_Mounts;
	public int m_nCurrentPage = 0;
	
	public int MountPeiYangCount
	{
		get { return m_canPeiYangCount;}
		set
		{
			if(m_canPeiYangCount != value)
			{
				m_canPeiYangCount = value;
			}
			XDailyPlaySignMgr.SP.DoHandleShowPlaySign(DailyPlayKey.DailyPlay_ZuoQi, m_canPeiYangCount, 10);
		}
	}
		
	public XMountManager()
	{
		m_Mounts = new SortedList<int, XMount>();
	}
	
	public void On_SC_ActiveMount(SC_ActiveMount msg)
	{
		for(int i=0; i<msg.MountsCount; i++)
		{
			SC_OneMount mount = msg.GetMounts(i);
			m_Mounts.Add(mount.Pos, new XMount((ushort)(mount.Index)));
		}
		if(msg.HasEquipMount)
		{
			EquipMount(msg.EquipMount, true);
			
			// 设置当前坐骑为选中状态
			m_nCurrentPage = ((m_nEquipMount - 1) / 6);
			
			XEventManager.SP.SendEvent(EEvent.Mount_ShowZuoQiSelect);
		}
		
		if ( m_nMountLevel != msg.Level )
		{
			XEventManager.SP.SendEvent(EEvent.Mount_ShowJiaChengInfo, (uint)msg.Level);
		}
		
		MountPeiYangCount = msg.Count;
		m_nMountLevel = msg.Level;
		m_nCurrExp = msg.Exp;
		
		XEventManager.SP.SendEvent(EEvent.Mount_SetLevel, m_nMountLevel);
		XEventManager.SP.SendEvent(EEvent.Mount_SetPeiYangCount, MountPeiYangCount);
		
		XCfgMountAttr mountAttr = XCfgMountAttrMgr.SP.GetConfig((ushort)m_nMountLevel);
		if ( mountAttr == null )
			return;
		XEventManager.SP.SendEvent(EEvent.Mount_SetExp, (uint)m_nCurrExp, mountAttr.NeedExp);
		
		XEventManager.SP.SendEvent(EEvent.Mount_ShowZuoQiInfo);
	}
	
	public ushort GetMountModeId(int pos)
	{
		if ( !m_Mounts.ContainsKey(pos) )
			return 0;
		
		return m_Mounts[pos].MountIndex;
	}
	
	public XMount GetMount(int pos)
	{
		if ( !m_Mounts.ContainsKey(pos) )
			return null;
		
		return m_Mounts[pos];
	}
	
	public void On_SC_AddMount(SC_OneMount msg)
	{	
		if(m_Mounts.ContainsKey(msg.Pos))
		{
			Log.Write(LogLevel.WARN, "XMountManager: 客户端比服务器先增加了一个坐骑? {0}, {1}", msg.Pos, msg.Index);
			return;
		}
		m_Mounts.Add(msg.Pos, new XMount((ushort)msg.Index));
		
		MountPeiYangCount = msg.PeiYangCount;
		XEventManager.SP.SendEvent(EEvent.Mount_SetPeiYangCount, MountPeiYangCount);
		if ( 0 == m_nMountLevel )
		{
			m_nMountLevel = 1;
			XEventManager.SP.SendEvent(EEvent.Mount_SetLevel, m_nMountLevel);
			
			XEventManager.SP.SendEvent(EEvent.Mount_ShowJiaChengInfo, 1u);
		}
		
		//if ( m_Mounts.Count == 1 )
			EquipMount(msg.Pos, true);
		
		XEventManager.SP.SendEvent(EEvent.Mount_ShowZuoQiInfo);
	}
	
	public void On_SC_DelMount(SC_Int msg)
	{
		if(!m_Mounts.ContainsKey(msg.Data))
		{
			Log.Write(LogLevel.WARN, "XMountManager: 服务器命令客户端删除一个它没有的坐骑? {0}", msg.Data);
			return;
		}
		m_Mounts.Remove(msg.Data);
	}
	
	public void On_SC_PeiYangCount(SC_Int msg)
	{
		MountPeiYangCount = msg.Data;
		XEventManager.SP.SendEvent(EEvent.Mount_SetPeiYangCount, MountPeiYangCount);
	}
	
	public void On_SC_PeiYangResult(SC_PeiYangResult msg)
	{
		if ( m_nMountLevel != msg.Level )
		{
			m_nMountLevel = msg.Level;
			XEventManager.SP.SendEvent(EEvent.Mount_SetLevel, m_nMountLevel);
			XEventManager.SP.SendEvent(EEvent.Mount_ShowJiaChengInfo, (uint)msg.Level);
			m_nCurrExp = msg.Exp;
		}
		else
		{
			m_nCurrExp += msg.Exp;
		}

		switch ( (PeiYangResult)msg.Result )
		{
		case PeiYangResult.Result_ConfigNull:
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 10053);
			
			break;
		case PeiYangResult.Result_CountZero:
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 10050);
			
			break;
		case PeiYangResult.Result_NotEnough_GameMoney:
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 10051);
			
			break;
		case PeiYangResult.Result_NotEnough_RealMoney:
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 10052);
			
			break;
		case PeiYangResult.Result_MountExist:
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 10056);
			
			break;
		case PeiYangResult.Result_Success:
			{
				MountPeiYangCount = MountPeiYangCount--;
				XEventManager.SP.SendEvent(EEvent.Mount_SetPeiYangCount, m_canPeiYangCount);
			
				if ( CritResult.Crite_True == (CritResult)msg.Crite )
					XEventManager.SP.SendEvent(EEvent.Mount_SetBaoJiValue, msg.Exp);
				else
					XEventManager.SP.SendEvent(EEvent.Mount_SetNormalValue, msg.Exp);
			
				XCfgMountAttr mountAttr = XCfgMountAttrMgr.SP.GetConfig((ushort)m_nMountLevel);
				if ( mountAttr == null )
					return;
				XEventManager.SP.SendEvent(EEvent.Mount_SetExp, (uint)m_nCurrExp, mountAttr.NeedExp);
			}
			
			break;
		default:
			break;
		}
	}
	
	public void EquipMount(int nPos, bool syncToServer)
	{
		if(m_nEquipMount == nPos)
		{
			Log.Write(LogLevel.WARN, "XMountManager: 客户端出现了再次骑乘同一个坐骑 {0}", nPos);
			return;
		}
		if( nPos == 0 )
		{
			XLogicWorld.SP.MainPlayer.MountIndex = 0;
		}
		else if(m_Mounts.ContainsKey(nPos))
		{
			if ( XMeditationManager.SP.IsMeditationStart )
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 9922);
				return;
			}
			XLogicWorld.SP.MainPlayer.MountIndex = m_Mounts[nPos].MountIndex;
		}
		
		if ( syncToServer )
		{
			CS_UInt.Builder msg = CS_UInt.CreateBuilder();
			msg.SetData((uint)nPos);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_MountEquip, msg.Build());
		}
		m_nEquipMount = nPos;
		
		if ( m_nEquipMount != 0 )
			m_nPrevEquipMount = m_nEquipMount;
	}
	
	// 坐骑培养
	public void MountPeiYang(PeiYangType type)
	{
		if ( 0 == m_canPeiYangCount )
		{
			XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 10050);
			return;
		}
		
		CS_UInt.Builder msg = CS_UInt.CreateBuilder();
		msg.SetData((uint)type);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_MountPeiYang, msg.Build());
	}
	
	public void ShowNextPage(int moveNum)
	{
		int maxPage = m_Mounts.Count / 6;
		if ( m_Mounts.Count % 6 > 0 )
			maxPage += 1;
		if ( -1 == moveNum && 0 == m_nCurrentPage )
			return;
		if ( 1 == moveNum && maxPage == m_nCurrentPage + 1 )
			return;
		if ( 0 == maxPage )
			return;
		
		m_nCurrentPage += moveNum;
		XEventManager.SP.SendEvent(EEvent.Mount_ShowZuoQiInfo);
	}
}

