using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;
using XGame.Client.Base.Pattern;

class  GuidePosInfo
{
	public GuidePosInfo(EEvent e1, Vector3 p, int k, int k1, int k2, GameObject obj)
	{
		e = e1;
		pos = p;
		key = k;
		key1 = k1;
		key2 = k2;
		parent = obj;
	}
	
	public GuidePosInfo(Vector3 p, int k, int k1, int k2, GameObject obj)
	{
		e = EEvent.PlayerGuide_Start;
		pos = p;
		key = k;
		key1 = k1;
		key2 = k2;
		parent = obj;
	}
	
	public EEvent e;
	public Vector3 pos;
	public int key;
	public int key1;
	public int key2;
	public GameObject parent;
}

public class XNewPlayerGuideManager  : XSingleton<XNewPlayerGuideManager>
{
	public enum GuideType
	{
		Guide_DialogWithNpc 		= 1,			// 点击NPC对话
		Guide_Navi_Pos				= 2,			// 点击自动寻路
		Guide_Mission_Finish 		= 3,			// 点击完成任务
		Guide_FightScense			= 4,			// 点击挑战副本
		Guide_OpenBag_FinishMission = 5,			// 打开背包，完成任务   
		Guide_OpenBag_GetPet		= 6,			// 第1次获得宠物激活道具, 打开背包
		Guide_OpenBag_GetMount		= 7,			// 第1次获得坐骑激活道具 打开背包
		Guide_OpenProduct			= 10,			// 打开生产界面
		Guide_ItemEquip				= 11,			// 点击后选择装备
		
		// Guide_PetUse				= 12,			// 点击后选择使用道具获得宠物
		Guide_OpenBuZhen			= 133,			// 打开布阵界面
		Guide_BuZhenPetUse			= 13,			// 宠物拖拉上布阵
		
		// 技能
		Guide_OpenSkill				= 115,			// 打开星图界面
		Guide_Skill_Select			= 14,			// 点击学习技能
		Guide_Skill_Use				= 15,			// 点击装备技能
		
		// 强化
		Guide_OpenStrength			= 116,			// 打开强化界面
		Guide_Strength_Click		= 16,			// 点击按钮强化
		
		// 坐骑
		// Guide_Mount_Equip			= 17,		// 点击装备坐骑
		Guide_MoneyTree_Open		= 119,			// 点击打开摇钱树
		Guide_MoneyTree_Click		= 19,			// 点击摇一摇
		
		// 生产
		Guide_Product_Select		= 120,			// 点击选择一个采集技能
		Guide_Product_Learn			= 20,			// 点击学习选中的采集技能
		Guide_Product_SelMake		= 140,			// 选择制造技能分页
		Guide_Product_SelMakeItem	= 143,			// 请选择一项制造技能
		Guide_Product_SelMakeLearn	= 40,			// 学习制造技能
		
		//Guide_FuYin_Select			= 121,			// 点击符印插槽
		//Guide_FuYin_Drag			= 21,			// 拖入符印插槽
		
		// 灵丹
		Guide_LingDan_OpenRole		= 1022,			// 点击打开角色面板
		Guide_LingDan_Select		= 122,			// 选择灵丹分页
		Guide_LingDan_Use			= 22,			// 点击使用灵丹
		
		// 化灵
		Guide_HuaLing_OpenRole		= 10244,		// 点击打开角色面板
		Guide_HuaLing_Select		= 1024,			// 点击选择培养页签
		Guide_HuaLing_Click			= 124,			// 点击培养按钮
		Guide_HuaLing_Use			= 24,			// 点击化灵按钮
		
		// 符印
		Guide_FuYin_OpenRole		= 1025,			// 符印提示打开角色属性界面
		Guide_FuYin_Select			= 125,			// 点击选择符印页签
		Guide_FuYin_Equip			= 25,			// 装备此符印
		//Guide_OpenRoleInfo_LingDan	= 26,		// 灵丹提示打开角色属性界面
		//Guide_OpenRoleInfo_HuaLing	= 27,		// 化灵提示打开角色属性界面
		//Guide_LingDan_Item_Click	= 28,			// 点击灵丹页签
		//Guide_PeiYang_Item_Click	= 29,			// 点击培养页签
		//Guide_HuaLing_Item_Click	= 30,			// 点击化灵页签
		//Guide_OpenStrength_XiangQ	= 18,			// 镶嵌打开强化界面引导
		
		// 镶嵌
		Guide_XiangQ_OpenStrength	= 10322,		// 点击打开强化面板
		Guide_XiangQ_Item_Sel		= 1032,			// 点击镶嵌页签
		Guide_XiangQ_Drag			= 132,			// 将宝石拖入镶嵌槽
		Guide_XiangQ_Click			= 32,			// 点击镶嵌
		
		// 强化
		//Guide_Open_Strength_Item	= 33,			// 点击打开强化界面
		//Guide_Strength_Item_Click	= 34,			// 点击强化页签
		
		// 仙道会
		Guide_OpenXianDaoHui		= 135,			// 点击仙道会
		Guide_XianDaoHuiStart		= 35,			// 点击角色挑战
		
		// 山河图
		Guide_OpenShanHeTu			= 37,			// 点击山河图
		Guide_ShanHeTuStartStep		= 138,			// 点击进入挑战
		Guide_ShanHeTuStart			= 38,			// 点击开始挑战
		
		// Guide_OpenMoneyTree			= 39,			// 点击摇钱树
		
		Guide_OpenSealHill			= 141,			// 点击打开山海经
		Guide_StartSealHill			= 41,			// 开始挑战山海经
		
		Guide_OpenZhanYaoLu		 	= 142,			// 点击打开斩妖录
		Guide_StartZhanYaoLu 		= 42,			// 点击挑战斩妖录
		
		Guide_OpenMount				= 144,			// 打开坐骑
		Guide_StartPeiYang			= 43,			// 点击培养按钮
		
		Guide_StrengthOpenBag		= 1044,			// 点击打开背包
		Guide_StrengthEquip			= 44,			// 装备此装备
		
		Guide_Friend				= 48,			// 好友引导
		Guide_Auction				= 49,			// 拍卖行引导引导
		Guide_Mouse					= 60,			// 键盘使用引导
		Guide_KeyBord				= 61,			// 鼠标使用引导
		
		Guide_FirstGuide			= 62,			// 开场强制引导
		Guide_FirstGuide_Step1		= 1063,			// 开场强制引导
		Guide_FirstGuide_Step2		= 1064,			// 开场强制引导
		Guide_FirstGuide_Step3		= 1065,			// 开场强制引导
		Guide_FirstGuide_Step4		= 1066,			// 开场强制引导
	}
	
	// 保存所有所有需要进行引导的数据，每次只进行一个引导，按队列进行
	private LinkedList<GuidePosInfo> m_all2ShowData = new LinkedList<GuidePosInfo>();
	
	// 背包gameobject，一些需要多次重复出现引导的数据存放在本节点上
	private SortedList<int, GameObject> m_allMutilShowObj = new SortedList<int, GameObject>();
	private List<int> m_cached2showObj = new List<int>();
	
	public uint StrengthMissionID = 0;
	public int ItemId = 0;
	public bool OnItemGuide = false;
	
	private ulong _status;
	public XNewPlayerGuideManager()
	{
	}
	
	public void ON_SC_NewPlayerGuideData_Coming(SC_UInt64 guideData)
	{
		int cls = XLogicWorld.SP.MainPlayer.Class;
		switch (cls)
		{
		case 1:
			ItemId = 10700;
		
			break;
		case  2:
			ItemId = 107001;
		
			break;;
		case 3:
			ItemId = 10702;
		
			break;
		default:
			break;
		}
		
		FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig((int)EFeatureID.EFeatureID_Strengthen);
		StrengthMissionID = unLock.RequireQuest;
		_status = guideData.Data;
	}
	
	public void UpdateGuideStatus(int pos)
	{
		SC_Int.Builder msg =SC_Int.CreateBuilder();
		msg.SetData(pos);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_NewPlayerGuide, msg.Build());
		
		ulong id = 1ul;
		if ( pos > 1 )
			id = id << (pos -1);
		_status = _status | id; 
	}
	
	public bool getGuideFinish(int pos)
	{
		ulong id = 1ul;
		ulong status = _status;
		if ( pos > 1 )
			id = id << (pos - 1);
		status = _status & id; 
		return 0 != status;
	}
	
	public bool CanShowHint(XItem item)
	{
		if ( item.DataID == ItemId  && !getGuideFinish((int)GuideType.Guide_StrengthEquip) )
			return false;
		return true;
	}
	
	// 类型引导，此引导引导完成以后更新到服务器, 放到队列尾
	public void handleShowGuide(int key, int key2, Vector3 pos, GameObject parent)
	{
		if ( key <= 4  || key == 60 || key == 61)
		{
			// 任务引导后不再引导
			if ( getGuideFinish(key) )
				return;

			XEventManager.SP.SendEvent(EEvent.PlayerGuide_Start, key, key, key2, pos, parent);
		}
		else
		{
			if ( key == (int)GuideType.Guide_StrengthEquip )
				OnItemGuide = true;
			pushGuideData2Queue(EEvent.PlayerGuide_Start, key, key, key2, pos, parent, false);
	
		}
	}
	// 类型引导，此引导引导完成以后更新到服务器, 放到队列的首位
	public void handleShowGuide2(int key, int key2, Vector3 pos, GameObject parent)
	{
		if ( key <= 4  || key == 60 || key == 61)
		{
			// 任务引导后不再引导
			if ( getGuideFinish(key) )
				return;

			XEventManager.SP.SendEvent(EEvent.PlayerGuide_Start, key, key, key2, pos, parent);
		}
		else
		{
			if ( key == (int)GuideType.Guide_StrengthEquip )
				OnItemGuide = true;
			pushGuideData2Queue(EEvent.PlayerGuide_Start, key, key, key2, pos, parent, true);
	
		}
	}
	
	// 开场强制引导
	public void StartFirstForceGuide()
	{
		pushGuideData2Queue(EEvent.PlayerGuide_StepStart, (int)GuideType.Guide_FirstGuide_Step1, (int)GuideType.Guide_FirstGuide, 1, 
			new Vector3(0, 0, 0), null, true);
	}
	
	// 步骤引导，此引导只有在其所属类型引导完成后才不会再显示,放到队列的尾
	public void handleShowGuideExt(int key, int key1, int key2, Vector3 pos, GameObject parent)
	{
//		// 任务引导后不再引导
//		if ( getGuideFinish(key1) )
//			return;
//		
//		XEventManager.SP.SendEvent(EEvent.PlayerGuide_StepStart, key, key1, key2, pos, parent);
		
		pushGuideData2Queue(EEvent.PlayerGuide_StepStart, key, key1, key2, pos, parent, false);
	}
	
		// 步骤引导，此引导只有在其所属类型引导完成后才不会再显示,放到队列的首位
	public void handleShowGuideExt2(int key, int key1, int key2, Vector3 pos, GameObject parent)
	{
//		// 任务引导后不再引导
//		if ( getGuideFinish(key1) )
//			return;
//		
//		XEventManager.SP.SendEvent(EEvent.PlayerGuide_StepStart, key, key1, key2, pos, parent);
		
		pushGuideData2Queue(EEvent.PlayerGuide_StepStart, key, key1, key2, pos, parent, true);
	}
	
	// 按照队列处理引导
	public void pushGuideData2Queue(EEvent e, int key, int key1, int key2, Vector3 pos, GameObject parent, bool pushHead)
	{
		// 任务引导后不再引导
		if ( getGuideFinish(key1) )
			return;
		
		LinkedListNode<GuidePosInfo> tmpinfo = m_all2ShowData.First;
		while ( tmpinfo != null )
		{
			if ( tmpinfo.Value.key == key )
				return;
			tmpinfo = tmpinfo.Next;
		}

		if ( pushHead )
			m_all2ShowData.AddFirst(new GuidePosInfo(e, pos, key, key1, key2, parent));
		else
			m_all2ShowData.AddLast(new GuidePosInfo(e, pos, key, key1, key2, parent));
		
		handleGuideDataQueue();
//		
//		GuidePosInfo posData = m_all2ShowData.First.Value;
//		m_all2ShowData.RemoveFirst();
//		
//		XEventManager.SP.SendEvent(posData.e, posData.key, posData.key1, posData.key2, posData.pos, posData.parent);
	}
	
	public void handleGuideDataQueue()
	{
		if ( XForceGuide.OnShowIng )
			return;
		
		if ( m_all2ShowData.Count <= 0 )
		{
			if ( !DailySignManager.SP.GetCanFinish() )
				XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eDailySign);
			return;
		}
		
		GuidePosInfo posData = m_all2ShowData.First.Value;
		XEventManager.SP.SendEvent(posData.e, posData.key, posData.key1, posData.key2, posData.pos, posData.parent);
	}
	
	public void RemoveLastShowedData()
	{
		if ( m_all2ShowData.Count <= 0 )
			return;
		
		GuidePosInfo posData = m_all2ShowData.First.Value;
		
		m_all2ShowData.RemoveFirst();
		if ( posData.e == EEvent.PlayerGuide_StepStart || posData.key1 == (int)XNewPlayerGuideManager.GuideType.Guide_Skill_Select )
			return;
		
		handleGuideDataQueue();
	}
	
	// 类型引导完成
	public void handleGuideFinish(int key)
	{
		if ( getGuideFinish(key) )
			return;
	
		XEventManager.SP.SendEvent(EEvent.PlayerGuide_Finish, key);
		
		// 当强化装备完成以后，立即执行强化引导
		if ( key == (int)XNewPlayerGuideManager.GuideType.Guide_StrengthEquip )
		{ 
			OnItemGuide = false;
		}
	}
	
	// 当前步骤引导完成
	public void handleGuideFinishExt(int key)
	{
		XEventManager.SP.SendEvent(EEvent.PlayerGuide_StepFinish, key);
	}
	
	// 任务引导特殊，需要特殊处理
	public void handleMissionGuide(uint missionid)
	{
		// 只引导第一个任务
		if ( 999u != missionid )
			return;
		
		// 任务引导后不再引导
		if ( getGuideFinish((int)GuideType.Guide_DialogWithNpc) )
			return;
		
		XGameObject gameObject = XNpc.GetByKindIndex(2);
		if( null == gameObject ) 
			return;
		
		Vector3 pos = new Vector3();
		GameObject parent = new GameObject();
		gameObject.GetHeadPosInfo(ref pos, ref parent);
		pos.x = pos.x - 140;
		
		handleShowGuide((int)GuideType.Guide_DialogWithNpc, 1, pos, parent);
	}
	public void handleMissionFinish(uint missionid)
	{
		if ( 1003u == missionid )
		{
			handleGuideFinish((int)GuideType.Guide_Navi_Pos);
		}
		else if ( 999u == missionid )
		{
			handleGuideFinish((int)GuideType.Guide_Mission_Finish);
			handleGuideFinish((int)GuideType.Guide_DialogWithNpc);
		}
		else if ( StrengthMissionID == missionid )
		{
			if ( !m_allMutilShowObj.ContainsKey((int)GuideType.Guide_StrengthOpenBag))
			{
				m_cached2showObj.Add((int)GuideType.Guide_StrengthEquip);
			}
			else
			{
				Vector3	showPos1 = new Vector3(0, 88, -50);
				handleShowGuideExt((int)GuideType.Guide_StrengthOpenBag, 
					(int)GuideType.Guide_StrengthEquip, 1, showPos1, m_allMutilShowObj[(int)GuideType.Guide_StrengthOpenBag]);
			}
		}
	}
	public void handleHideMissionGuide()
	{
		XEventManager.SP.SendEvent(EEvent.PlayerGuide_Hide, (int)GuideType.Guide_Navi_Pos);
		XEventManager.SP.SendEvent(EEvent.PlayerGuide_Hide, (int)GuideType.Guide_DialogWithNpc);
	}
	
	// 物品引导
	public void handleItemGuide(XItem item)
	{
		int key = -1;
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
		if(cfgItem == null)
			return ;
		switch ( (EItem_Type)cfgItem.ItemType )
		{
//		case EItem_Type.EITEM_TYPE_PET:
//			key = (int)GuideType.Guide_OpenBag_GetPet;
//			break;
			
//		case EItem_Type.EITEM_TYPE_MOUNT:
//			key = (int)GuideType.Guide_OpenBag_GetMount;
//			break;
			
		default:
			break;
		}
		
		if (  -1 == key )
		{
			return;
		}
		if (!m_allMutilShowObj.ContainsKey(key) )
		{
			m_cached2showObj.Add(key);
			return;
		}
		Vector3	showPos = new Vector3(0, 88, -50);
		
		handleShowGuide(key, 1, showPos, m_allMutilShowObj[key]);
	}
	
	// NPC消失的时候，引导UI被摧毁，所有需要清除
	public void handleNpcDisapper(int npcIndex)
	{
 		if ( npcIndex != 2 )
			return;
		XEventManager.SP.SendEvent(EEvent.PlayerNpc_Disapper, (int)GuideType.Guide_DialogWithNpc);
	}
	
	// 处理functionbutton引导
	public void handleFunBtnGuide(uint unlockid, GameObject obj)
	{
		Vector3 showPos;
		if ( obj == null )
		{
			return;
		}
		showPos = new Vector3(-145, 1, -50);
		int key = -1;
		int key1 = 0;
		int key2 = 0;
		EEvent e = EEvent.PlayerGuide_Start;
		
		uint id2showEffect = unlockid;
		// 强化
		switch ( (EFeatureID)unlockid )
		{
		case EFeatureID.EFeatureID_Char:
			{
				if ( FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_lingdan) )
				{
					key = (int)GuideType.Guide_LingDan_OpenRole;
					key1 = (int)GuideType.Guide_LingDan_Use;
					key2 = 1;
					e = EEvent.PlayerGuide_StepStart;
				}
				if ( FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_hualing) )
				{
					key = (int)GuideType.Guide_HuaLing_OpenRole;
					key1 = (int)GuideType.Guide_HuaLing_Use;
					key2 = 1;
					e = EEvent.PlayerGuide_StepStart;
				}
				if ( FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_fuyin) )
				{
					key = (int)GuideType.Guide_FuYin_OpenRole;
					key1 = (int)GuideType.Guide_FuYin_Equip;
					key2 = 1;
					e = EEvent.PlayerGuide_StepStart;
				}
				
				m_allMutilShowObj[(int)GuideType.Guide_LingDan_OpenRole] = obj;
				m_allMutilShowObj[(int)GuideType.Guide_HuaLing_OpenRole] = obj;
				m_allMutilShowObj[(int)GuideType.Guide_FuYin_OpenRole]	 = obj;
			}
			
			break;
		case EFeatureID.EFeatureID_lingdan:
			id2showEffect = (uint)EFeatureID.EFeatureID_Char;
			obj = m_allMutilShowObj[(int)GuideType.Guide_LingDan_OpenRole];
			e = EEvent.PlayerGuide_StepStart;
			key = (int)GuideType.Guide_LingDan_OpenRole;
			key1 = (int)GuideType.Guide_LingDan_Use;
			key2 = 1;
			
			break;
		case EFeatureID.EFeatureID_hualing:
			id2showEffect = (uint)EFeatureID.EFeatureID_Char;
			obj = m_allMutilShowObj[(int)GuideType.Guide_HuaLing_OpenRole];
			key = (int)GuideType.Guide_HuaLing_OpenRole;
			key1 = (int)GuideType.Guide_HuaLing_Use;
			key2 = 1;
			e = EEvent.PlayerGuide_StepStart;
			
			break;
		case EFeatureID.EFeatureID_fuyin:				// 符印
			id2showEffect = (uint)EFeatureID.EFeatureID_Char;
			obj = m_allMutilShowObj[(int)GuideType.Guide_FuYin_OpenRole];
			key = (int)GuideType.Guide_FuYin_OpenRole;
			key1 = (int)GuideType.Guide_FuYin_Equip;
			key2 = 1;
			e = EEvent.PlayerGuide_StepStart;
			
			break;
		case EFeatureID.EFeatureID_Strengthen: 			// 强化
			{
				if ( getGuideFinish((int)GuideType.Guide_StrengthEquip) )
				{
					key = (int)GuideType.Guide_OpenStrength;
					key1 = (int)GuideType.Guide_Strength_Click;
					e = EEvent.PlayerGuide_StepStart;
					key2 = 1;
				}
				else
				{
					m_allMutilShowObj[(int)GuideType.Guide_OpenStrength] = obj;
				}
			}
			
//			break;
//		case EFeatureID.EFeatureID_xiangqian:
//			id2showEffect = (uint)EFeatureID.Guide_XiangQ_OpenRole;
//			obj = m_allMutilShowObj[(int)GuideType.Guide_XiangQ_OpenRole];
//			key = (int)GuideType.Guide_XiangQ_OpenRole;
//			key1 = (int)GuideType.Guide_XiangQ_Click;
//			key2 = 1;
//			e = EEvent.PlayerGuide_StepStart;
			
			
			break;
		case EFeatureID.EFeatureID_Bag:					// 背包
			{
				if ( XMissionManager.SP.hasReferMissionInList(StrengthMissionID) )
				{
					key = (int)GuideType.Guide_StrengthOpenBag;
					key1 = (int)GuideType.Guide_StrengthEquip;
					key2 = 1; 
					e = EEvent.PlayerGuide_StepStart;
				}
				//m_allMutilShowObj[(int)GuideType.Guide_OpenBag_FinishMission] = obj;
				//m_allMutilShowObj[(int)GuideType.Guide_OpenBag_GetPet] = obj;
				//m_allMutilShowObj[(int)GuideType.Guide_OpenBag_GetMount] = obj;
				m_allMutilShowObj[(int)GuideType.Guide_StrengthEquip] = obj;
			}
//			key = 5;
//			key1 = 5;
//			key2 = 1;
			
			break;
		case EFeatureID.EFeatureID_Skill:				// 星图
			key = (int)GuideType.Guide_OpenSkill;
			key1 = (int)GuideType.Guide_Skill_Use;
			key2 = 1;
			e = EEvent.PlayerGuide_StepStart;
			
			break;
		case EFeatureID.EFeatureID_Product:				// 生产
			key = (int)GuideType.Guide_OpenProduct;
			key1 = (int)GuideType.Guide_OpenProduct;
			key2 = 1;
			
			break;
		case EFeatureID.EFeatureID_FaBao:				// 坐骑
			key = (int)GuideType.Guide_OpenMount;
			key1 = (int)GuideType.Guide_StartPeiYang;
			key2 = 1;
			e = EEvent.PlayerGuide_StepStart;
			
			break;
		case EFeatureID.EFeatureID_Friend:				// 好友
			key = (int)GuideType.Guide_Friend;
			key1 = (int)GuideType.Guide_Friend;
			key2 = 1;
			
			break;
		case EFeatureID.EFeatureID_Formation:			// 布阵
			key = (int)GuideType.Guide_OpenBuZhen;
			key1 = (int)GuideType.Guide_BuZhenPetUse;
			key2 = 1;
			
			break;
		default:
			break;
		}
		if ( key1 > 0 && !getGuideFinish(key1) )
		{
			XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, id2showEffect, 900049u, 1);
			
			pushGuideData2Queue(e, key, key1, key2, showPos, obj, false);
			
			//XEventManager.SP.SendEvent(e, key1, key, key2, showPos, obj);
		}
		else
		{
			handleCachedShowObj();
		}
	}
	
	public void ShowStrengthGuide()
	{
		if ( !m_allMutilShowObj.ContainsKey((int)GuideType.Guide_OpenStrength) )
			return;
		
		XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)EFeatureID.EFeatureID_Strengthen, 900049u, 1);
		
		Vector3 showPos = new Vector3(-145, 1, -50);
		pushGuideData2Queue(EEvent.PlayerGuide_StepStart, (int)GuideType.Guide_OpenStrength, 
			(int)GuideType.Guide_Strength_Click, 1, showPos, m_allMutilShowObj[(int)GuideType.Guide_OpenStrength], true);
	}
	
	private void handleCachedShowObj()
	{
		for( int i = 0; i < m_cached2showObj.Count; i++ )
		{
			int key = m_cached2showObj[i];
			Vector3 showPos = new Vector3(-145, 1, -50);
			if ( !getGuideFinish(key) && m_allMutilShowObj.ContainsKey(key) )
			{
				int keyGuide1 = key;
				int keyGuide2 = key;
				EEvent e = EEvent.PlayerGuide_Start;
				
				if ( (GuideType)key == GuideType.Guide_StrengthEquip )
				{
					XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)EFeatureID.EFeatureID_Bag, 900049u, 1);
					keyGuide1 = (int)GuideType.Guide_StrengthOpenBag;
					keyGuide2 = (int)GuideType.Guide_StrengthEquip;
					e = EEvent.PlayerGuide_StepStart;
				}			
				
				pushGuideData2Queue(e, keyGuide1, keyGuide2, 1, showPos, m_allMutilShowObj[key], false);
				// XEventManager.SP.SendEvent(e, keyGuide1, keyGuide2, 1, showPos, m_allMutilShowObj[key]);
				
				m_cached2showObj.RemoveAt(i);
				i = 0;
			}
		}
	}
	
	public void handleFunBtnTr(EFeatureID unlockid, GameObject go)
	{
		Vector3 showPos = new Vector3(-145, 9, 0);
		switch ( unlockid )
		{
		case EFeatureID.EFeatureID_XianDaoHui:
			if ( !getGuideFinish((int)GuideType.Guide_XianDaoHuiStart) )
			{
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)unlockid, 900039u, 1);
				handleShowGuideExt((int)GuideType.Guide_OpenXianDaoHui, (int)GuideType.Guide_XianDaoHuiStart, 1, showPos, go);
			}
			
			break;
		case EFeatureID.EFeatureID_ShanHeTu:
			if ( !getGuideFinish((int)GuideType.Guide_OpenShanHeTu) )
			{
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)unlockid, 900039u, 1);
				
				handleShowGuide((int)GuideType.Guide_OpenShanHeTu, 1, showPos, go);
			}
			
			break;
		case EFeatureID.EFeatureID_MoneyTree:
			if ( !getGuideFinish((int)GuideType.Guide_MoneyTree_Open) )
			{
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)unlockid, 900039u, 1);
				handleShowGuideExt((int)GuideType.Guide_MoneyTree_Open, (int)GuideType.Guide_MoneyTree_Click, 1, showPos, go);
			}
			
			break;
		case EFeatureID.EFeatureID_HillSeal:
			if ( !getGuideFinish((int)GuideType.Guide_StartSealHill) )
			{
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)unlockid, 900039u, 1);
				handleShowGuideExt((int)GuideType.Guide_OpenSealHill, (int)GuideType.Guide_StartSealHill, 1, showPos, go);
			}
			
			break;
		case EFeatureID.EFeatureID_KillMonster:
			if ( !getGuideFinish((int)GuideType.Guide_StartZhanYaoLu) )
			{
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)unlockid, 900039u, 1);
				handleShowGuideExt((int)GuideType.Guide_OpenZhanYaoLu, (int)GuideType.Guide_StartZhanYaoLu, 1, showPos, go);
			}			
			
			break;
		case EFeatureID.EFeatureID_Auciton:
			if ( !getGuideFinish((int)GuideType.Guide_Auction) )
			{
				XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT, (uint)unlockid, 900039u, 1);
				handleShowGuide((int)GuideType.Guide_Auction, 1, showPos, go);
			}
			
			break;
		default:
			break;
		}
	}
	
	public void handleFunStrengthBtn()
	{
		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_OpenStrength) )
			handleGuideFinishExt((int)GuideType.Guide_OpenStrength);
	}
	
	public void handleRoleInfoBtn()
	{
		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_LingDan_OpenRole) )
			handleGuideFinishExt((int)GuideType.Guide_LingDan_OpenRole);
		
		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_HuaLing_OpenRole) )
			handleGuideFinishExt((int)GuideType.Guide_HuaLing_OpenRole);
		
		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_FuYin_OpenRole) )
			handleGuideFinishExt((int)GuideType.Guide_FuYin_OpenRole);
	}
	
	public void handleRoleStrengthOrInlay(int type)
	{
		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_XiangQ_Item_Sel) )
			XUTNewPlayerGuide.m_allShowGuides[(int)GuideType.Guide_XiangQ_Item_Sel].SetActive(false);
	}
	
	public void handleFunBagBtn()
	{
//		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_OpenBag_FinishMission) )
//			handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_OpenBag_FinishMission);
				
//		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_OpenBag_GetPet) )
//			handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_OpenBag_GetPet);
						
//		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_OpenBag_GetMount) )
//			handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_OpenBag_GetMount);
		
		if ( XUTNewPlayerGuide.m_allShowGuides.ContainsKey((int)GuideType.Guide_StrengthOpenBag) )
			handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_StrengthOpenBag);
	}
	
	// 打开背包新手引导
	public void handleOpenBagGuide()
	{
		if ( !getGuideFinish((int)GuideType.Guide_XiangQ_Click) )
			XEventManager.SP.SendEvent(EEvent.Bag_Guide, EItem_Type.EITEM_TYPE_GEM, (int)GuideType.Guide_XiangQ_Click);		
		
		if ( !getGuideFinish((int)GuideType.Guide_StrengthEquip) )
			XEventManager.SP.SendEvent(EEvent.Bag_Guide, EItem_Type.EITEM_TYPE_GEM, (int)GuideType.Guide_StrengthEquip);
	}
	
	// 鼠标方向引导
	public void handleDirection(int id)
	{
		Transform refer = XMissionTip.PlayerGuideMoveRefernce;
		if(refer == null)
			return ;

		Vector3 showPos = new Vector3(Mathf.Round(refer.localPosition.x - 208), 
				Mathf.Round(refer.localPosition.y - 8), 
				Mathf.Round(refer.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuide(id, 1, showPos, refer.parent.gameObject);
	}
	// 键盘 w a s d方向引导
	public void handleKeybord(int id)
	{
		Transform refer = XChatWindow.PlayerGuideKeybordRefernce;
		Vector3 showPos = new Vector3(Mathf.Round(refer.localPosition.x + 200), 
				Mathf.Round(refer.localPosition.y + 350), 
				Mathf.Round(refer.localPosition.z));
			XNewPlayerGuideManager.SP.handleShowGuide(id, 1, showPos, refer.parent.gameObject);
	}
	public void handleKeybordFinish(int id)
	{
		if ( !XUTNewPlayerGuide.m_allDeltaUI.ContainsKey(id) )
			return;
		handleDirectionDelay(id);
	}
	public void handleKeybordDisapper(int id)
	{
		if ( !XUTNewPlayerGuide.m_allDeltaUI.ContainsKey(id) )
			return;
		XUTNewPlayerGuide.m_allDeltaUI[id].gameObject.SetActive(false);
	}
	public void handleEnterNormalScene(int id)
	{
		if ( !XUTNewPlayerGuide.m_allDeltaUI.ContainsKey(id) )
			return;
		handleGuideFinish(id);
	}
	
	// 延迟消失
	public void handleDirectionDelay(int key)
	{
		XEventManager.SP.SendEvent(EEvent.PlayerMove_DisapperDelta, 3f, key);
	}
}
   