using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame.Client.Packets;
using Google.ProtocolBuffers;

class XObjectManager
{
	// diaappear超过这个值的不再维护
	private static float DISAPPEAR_REMOVE_DELAY = 120f;		
	private SortedList<ulong, XGameObject>[] m_AllObject;
	
	public XObjectManager()
	{
		m_AllObject = new SortedList<ulong, XGameObject>[(int)EObjectType.End];
		for(int i=0; i<m_AllObject.Length; i++)
		{
			m_AllObject[i] = new SortedList<ulong, XGameObject>();
		}
	}	
	
	public void Breathe()
	{
		for(int i=0; i<(int)EObjectType.End; i++)
		{
			List<ulong> removed = new List<ulong>();
			foreach(KeyValuePair<ulong, XGameObject> kvp in m_AllObject[i])
			{
				if(!kvp.Value.IsWaitAppearData && !kvp.Value.IsAppear && Time.time - kvp.Value.DisAppearRockon >= DISAPPEAR_REMOVE_DELAY)
				{
					removed.Add(kvp.Key);
					continue;
				}
				if(kvp.Value.IsActive)
					kvp.Value.Breathe();
			}
			foreach(ulong id in removed)
			{
				m_AllObject[i].Remove(id);
			}
		}
	}
	
	public void OnLevelLoaded(int id, string name, ESceneType st)
	{
		for(int i=0; i<(int)EObjectType.End; i++)
		{
			foreach(KeyValuePair<ulong, XGameObject> kvp in m_AllObject[i])
			{
				kvp.Value.OnLevelLoaded(id, st);
			}
		}
	}
	
	public XPlayer GetPlayer(ulong id)
	{
		if(XLogicWorld.SP.MainPlayer.ID == id)
			return XLogicWorld.SP.MainPlayer;
		
		XPlayer player = GetObject<XPlayer>(EObjectType.OtherPlayer, id);
		return player;
	}
	
	public XGameObject GetObject(EObjectType objectType, ulong id)
	{
		SortedList<ulong, XGameObject> objectList = m_AllObject[(int)objectType];
		if(null == objectList || !objectList.ContainsKey(id)) return null;
		return objectList[id];
	}
	
	public T GetObject<T>(EObjectType objectType, ulong id) where T : XGameObject
	{
		XGameObject gameObject = GetObject(objectType, id);
		if(gameObject is T) 
			return gameObject as T;
		return null;
	}
	
	public void RemoveObject(EObjectType objectType, ulong id)
	{
		SortedList<ulong, XGameObject> objectList = m_AllObject[(int)objectType];
		if(null == objectList || !objectList.ContainsKey(id)) return;
		objectList.Remove(id);
	}
	
	public void SetAllObjectActive(bool isActive)
	{
		for(int i=0; i<(int)EObjectType.End; i++)
		{
			foreach(XGameObject gameObject in m_AllObject[i].Values)
			{
				gameObject.Visible	= isActive;
				gameObject.IsActive	= isActive;
			}			
		}
	}
	
	public void RemoveAllObject()
	{
		for(int i=0; i<(int)EObjectType.End; i++)
		{
			foreach(XGameObject gameObject in m_AllObject[i].Values)
			{
				gameObject.DisAppear();
			}
			m_AllObject[i].Clear();
		}
	}

	public void RemoveAllObjectNotLogic()
	{
		for(int i=0; i<(int)EObjectType.End; i++)
		{
			if(i == (int)EObjectType.MainPlayer || i == (int)EObjectType.OtherPlayer || i == (int)EObjectType.Npc)
				continue;

			foreach(XGameObject gameObject in m_AllObject[i].Values)
			{
				gameObject.DisAppear();
			}
			m_AllObject[i].Clear();
		}
	}

	public void RemoveObjectByType(EObjectType type)
	{
		foreach(XGameObject gameObject in m_AllObject[(int)type].Values)
		{
			gameObject.DisAppear();
		}
		m_AllObject[(int)type].Clear();
	}

	#region Appear相关
	// 取消最大玩家显示数量, 如之后策划有需求, 再增加
    public void On_SC_PlayerAppear(SC_PlayerAppear msg)
    {
        ulong playerId = msg.PlayerId;
		XPlayer player = GetObject<XPlayer>(EObjectType.OtherPlayer, playerId);
		if(null == player)
		{
			player = new XPlayer(playerId);
			m_AllObject[(int)EObjectType.OtherPlayer].Add(playerId, player);
		}
		player.IsWaitAppearData = true;
		
		CS_RequestPlayerAppear.Builder builder = CS_RequestPlayerAppear.CreateBuilder();
		builder.SetPlayerId(player.ID);
		builder.SetVersion(player.Version);
		CS_RequestPlayerAppear req = builder.Build();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_RequestPlayerAppear, req);
    }

    public void On_SC_NpcAppear(SC_NpcAppear msg)
    {
        ulong npcId = msg.NpcId;
		XNpc npc = GetObject<XNpc>(EObjectType.Npc, npcId);
        if (null == npc)
        {
			npc = new XNpc(npcId);
			m_AllObject[(int)EObjectType.Npc].Add(npcId, npc);
        }
		npc.IsWaitAppearData = true;
		
		Log.Write(LogLevel.WARN,"npcId is {0}",npcId);
		
		CS_RequestNpcAppear.Builder builder = CS_RequestNpcAppear.CreateBuilder();
        builder.SetNpcId(npcId);
        builder.SetVersion(npc.Version);
        CS_RequestNpcAppear req = builder.Build();
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_RequestNpcAppear, req);
    }
    
	public void On_SC_ObjectAppearData(EObjectType objectType, ulong id, IMessage msg)
    {
		XGameObject gameObject = GetObject<XGameObject>(objectType, id);		
		if(null == gameObject)
		{
			Log.Write(LogLevel.WARN,"objectType is {0} objectID is {1}",objectType,id);
			return;
		}
		gameObject.SetAppearData(msg);
		gameObject.Appear();
		gameObject.IsWaitAppearData = false;
    }
	
	public XMonster AppearMonster(XMonsterAppearInfo info)
	{
		XMonster monster = GetObject<XMonster>(EObjectType.Monster, info.ID);
		if(null == monster)
		{
			monster = new XMonster(info.ID);
			m_AllObject[(int)EObjectType.Monster].Add(info.ID, monster);
		}
		monster.SetAppearData(info);
		monster.Appear();
		return monster;
	}
	
    public XPet AppearPet(XPetAppearInfo info)
    {
		XPet pet = GetObject<XPet>(EObjectType.Pet, info.ID);
		if(null == pet)
		{
			pet = new XPet(info.ID);
			m_AllObject[(int)EObjectType.Pet].Add(info.ID, pet);
		}
		pet.SetAppearData(info);
		pet.Appear();
		return pet;
    }
	
	public XTransPoint AppearTransPoint(XTransPointAppearInfo info)
	{
		XTransPoint point = GetObject<XTransPoint>(EObjectType.TransPoint, info.ID);
		if(null == point)
		{
			point = new XTransPoint(info.ID);
			m_AllObject[(int)EObjectType.TransPoint].Add(info.ID, point);
		}
		point.SetAppearData(info);
		point.Appear();
		return point;
	}
	
	public XGatherObject AppearGatherObject(XGatherObjectAppearInfo info)
	{
		XGatherObject gatherObject = GetObject<XGatherObject>(EObjectType.GatherObject, info.ID);
		if(null == gatherObject)
		{
			gatherObject = new XGatherObject(info.ID);
			m_AllObject[(int)EObjectType.GatherObject].Add(info.ID, gatherObject);
		}
		gatherObject.SetAppearData(info);
		gatherObject.Appear();
		return gatherObject;
	}
	
	public void DisappearObject(EObjectType objectType, ulong id)
    {
		XGameObject gameObject = GetObject<XGameObject>(objectType, id);
		if(null == gameObject) return;
		gameObject.DisAppear();
		gameObject.IsWaitAppearData = false;
    }
	#endregion
}


/*class XObjectManager
{
    #region Propertys

    // 客户端动态创建的对象 ID 区域
    internal static readonly ulong CLIENT_DYNAMIC_OBJECT_ID_BEGIN = 1000;

    private ulong m_NextID = CLIENT_DYNAMIC_OBJECT_ID_BEGIN ;

    // 客户端动态创建的对象 ID 创建函数
    internal ulong GetNextID()
    {
        return m_NextID++;
    }

	// 将对象按照 EObjectType 进行分类保存
    private Hashtable[] m_AllGameObject;
    // 标记某类对象列表是否变动(用于解决 Breathe 内更改列表导致寻坏错误的问题)
    private bool[] m_TypeObjectChange;
    public uint LastBreathe { get; private set; }

    private uint NowShowPlayerNum = 0;
    private uint MaxShowPlayerNum = Define.DEFAULT_SHOW_PLAYER_NUM;
    public bool IsShowOtherPlayer { get; private set; }

    #endregion

    public XObjectManager()
    {
        m_AllGameObject = new Hashtable[(int)EObjectType.End];
        m_TypeObjectChange = new bool[(int)EObjectType.End];
        for (int i = (int)EObjectType.Begin; i < (int)EObjectType.End; ++i)
        {
            m_AllGameObject[i] = new Hashtable(1);
			m_TypeObjectChange[i] = true;
        }
        LastBreathe = 0;
        IsShowOtherPlayer = true;
        NowShowPlayerNum = 0;
    }

    ~XObjectManager()
    {
        RemoveAllObject(true, true);
    }

    #region Common Functions
	
    public void Breathe()
    {
        ++LastBreathe;

        Hashtable ht = null;
        for (int i = (int)EObjectType.Begin; i < (int)EObjectType.End; ++i)
        {
			ht = m_AllGameObject[i];
            while (m_TypeObjectChange[i])
            {
                m_TypeObjectChange[i] = false;
                foreach (XGameObject xgo in ht.Values)
                {
                    if (xgo.LastBreathe != LastBreathe)
                    {
                        xgo.Breathe();
						if (m_TypeObjectChange[i])
						{
							break;
						}
                    }
                }
            }
			m_TypeObjectChange[i] = true;
        }
    }

    public void ToggleHideOtherPlayer()
    {
        IsShowOtherPlayer = !IsShowOtherPlayer;

        Hashtable tb = GetTableFromType((int)EObjectType.OtherPlayer);
        if (null != tb)
        {
            foreach (XPlayer player in tb.Values)
            {
                player.SetVisible(IsShowOtherPlayer);
            }
        }
    }

    public XGameObject CreateObject(EObjectType type)
    {
        return CreateObject(type, this.GetNextID());
    }

    public XGameObject CreateObject(EObjectType type, ulong id)
    {
        //--4>: 因为增加了物体操作列表, 所以增加的时候会发现原来以为删除的还存在
        //      所以不管是否存在都增加. 只是如果增加多次可能会有问题
        /*
        XGameObject obj = GetObject(type, id);
        if (obj != null)
        {
            return null;
        }
        
        XGameObject obj = null;
        switch (type)
        {
            case EObjectType.MainPlayer:
                obj = new XMainPlayer(id);
                break;
            case EObjectType.Npc:
                obj = new XNpc(id);
                break;
            case EObjectType.OtherPlayer:
                obj = new XPlayer(id);
                break;
			case EObjectType.Monster:
				obj = new XMonster(id);
				break;
            case EObjectType.Pet:
                obj = new XPet(id);
                break;
            case EObjectType.TransPoint:
                obj = new XTransPoint(id);
                break;
            default:
                return null;
        }
        if (null != obj && obj.Init())
        {
			m_AllGameObject[(int)type].Add(id, obj);
            m_TypeObjectChange[(int)type] = true;
            return obj;
        }
        return null;
    }

    public XPlayer GetPlayer(ulong id)
    {
        XGameObject go = GetObject(EObjectType.OtherPlayer, id);
        if (go == null)
        {
            go = GetObject(EObjectType.MainPlayer, id);
        }
        return go as XPlayer;
    }

    public XGameObject GetObject(ulong id)
    {
        XGameObject xgo = null;
        for (int i = (int)EObjectType.Begin; i < (int)EObjectType.End; ++i)
        {
            xgo = doGetObject(i, id);
            if (null != xgo)
            {
                break;
            }
        }
        return xgo;
    }

    public XGameObject GetObject(EObjectType type, ulong id)
    {
        return doGetObject((int)type, id);
    }

    private XGameObject doGetObject(int type, ulong id)
    {
        Hashtable tb = GetTableFromType(type);
        if (tb == null || !tb.Contains(id))
        {
            return null;
        }
        return tb[id] as XGameObject;
    }

    public bool RemoveObject(EObjectType type, ulong id)
    {
        //--4>: 因为增加了物体操作列表, 所以逻辑上如果对一个物体删除多次可能会有问题
        Hashtable tb = GetTableFromType((int)type);
        if (tb == null || !tb.Contains(id))
        {
            return false;
        }
        XGameObject obj = (tb[id] as XGameObject);
        this.DisappearObject(obj);
        m_AllGameObject[(int)type].Remove(id);
		m_TypeObjectChange[(int)type] = true;
        return true;
    }

    public void RemoveAllObject()
    {
        this.RemoveAllObject(false, false);
    }

    public void RemoveAllObject(bool bRemoveMainPlayer, bool bRemovePet)
    {
        for (int i = (int)EObjectType.Begin; i < (int)EObjectType.End; ++i)
        {
            if (i == (int)EObjectType.MainPlayer && false == bRemoveMainPlayer)
            {
                continue;
            }
			if (i == (int)EObjectType.Pet && false == bRemovePet)
            {
                continue;
            }
			foreach (XGameObject xgo in m_AllGameObject[i].Values)
			{
				this.DisappearObject(xgo);
            }
            m_AllGameObject[i].Clear();
            m_TypeObjectChange[i] = true;
        }
        NowShowPlayerNum = 0;
    }

    private bool RequestPlayerAppear(XPlayer player)
    {
        if (player == null || !player.IsAppear)
        {
            return false;
        }

        if (NowShowPlayerNum < MaxShowPlayerNum)
        {
            // 请求获得或者更新详细信息
            CS_RequestPlayerAppear.Builder builder = CS_RequestPlayerAppear.CreateBuilder();
            builder.SetPlayerId(player.ID);
            builder.SetVersion(player.Version);
            CS_RequestPlayerAppear req = builder.Build();
            XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_RequestPlayerAppear, req);
        }
        return true;
    }

    private void DisappearObject(XGameObject obj)
    {
        if (obj != null)
        {
            if (obj.ObjectType == EObjectType.OtherPlayer && obj.HasModel() && NowShowPlayerNum > 0)
            {
                --NowShowPlayerNum;
            }
            obj.OnDisappear();
        }
    }

    #endregion

    #region Packet Process Functions

    public void On_SC_PlayerAppear(SC_PlayerAppear msg)
    {
        ulong playerId = msg.PlayerId;
        XPlayer player = GetObject(EObjectType.OtherPlayer, playerId) as XPlayer;
        if (player == null)
        {
            player = CreateObject(EObjectType.OtherPlayer, playerId) as XPlayer;
        }
        player.OnAppear();
        this.RequestPlayerAppear(player);
    }

    public void On_SC_NpcAppear(SC_NpcAppear msg)
    {
        ulong npcId = msg.NpcId;
        XNpc npc = GetObject(EObjectType.Npc, npcId) as XNpc;
        if (npc == null)
        {
            npc = CreateObject(EObjectType.Npc, npcId) as XNpc;
        }
        npc.OnAppear();
        // 请求获得或者更新详细信息
        CS_RequestNpcAppear.Builder builder = CS_RequestNpcAppear.CreateBuilder();
        builder.SetNpcId(npcId);
        builder.SetVersion(npc.Version);
        CS_RequestNpcAppear req = builder.Build();
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_RequestNpcAppear, req);
    }
    
	public void On_SC_ObjectAppearData(EObjectType type, ulong id, IMessage msg)
    {
        XGameObject gameObj = GetObject(type, id);
        //--4>: 如果传送场景, 进入副本等情况, 清空所有对象, 即使数据包晚到了也没事
        if (gameObj != null)
        {
            if (gameObj.ObjectType == EObjectType.OtherPlayer)
            {
                if (NowShowPlayerNum >= MaxShowPlayerNum || gameObj.HasModel())
                {
                    gameObj.SetAppearData(msg, false);
                }
                else
                {
                    gameObj.SetAppearData(msg, true);
                    if (gameObj.HasModel())
                    {
                        ++NowShowPlayerNum;
                    }
                }
            }
            else
            {
                gameObj.SetAppearData(msg, true);
            }
        }
    }

    public void On_SC_ObjectDisappear(EObjectType type, ulong id)
    {
        Hashtable tb = GetTableFromType((int)type);
        if (tb != null && tb.Contains(id))
        {
            this.DisappearObject(tb[id] as XGameObject);
        }
    }

	public XMonster On_Monster_Appear(XApperInfoMonster info)
	{
		XMonster monster = GetObject(EObjectType.Monster, info.ID) as XMonster;
        if (null == monster)
        {
            monster = CreateObject(EObjectType.Monster, info.ID) as XMonster;
        }
        monster.OnAppear();
        monster.SetAppearData(info);
        return monster;
	}

    public XPet On_SC_PetAppearData(SC_OnePet info)
    {
		ulong id = GetNextID();
		XPet pet = CreateObject(EObjectType.Pet, id) as XPet;
        pet.SetAppearData(info, false);
        return pet;
    }
	
    #endregion

    #region Internal Functions

    private Hashtable GetTableFromType(int type)
    {
        if (type < (int)EObjectType.Begin || type >= (int)EObjectType.End)
        {
            return null;
        }
        return m_AllGameObject[type];
    }

    #endregion
}*/

