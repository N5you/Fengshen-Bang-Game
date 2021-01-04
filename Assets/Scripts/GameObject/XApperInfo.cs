using System;
using UnityEngine;
using XGame.Client.Packets;

public abstract class XAppearInfoBase
{
	public ulong ID { get; protected set; }
	public Vector3 Position { get; protected set; }
	public Vector3 Direction { get; protected set; } 
}

public class XMonsterAppearInfo : XAppearInfoBase
{
	private static ulong m_genMonsterId = 1;
	public uint MonsterBaseID {get;set;}
	public uint MonsterGroupID {get;set;}
	//public XCfgMonsterBase cfgInfo { get; private set; }
	//public XCfgMonsterGroup cfgGroup { get; private set; }
	
	public XMonsterAppearInfo(Vector3 pos, Vector3 dir)
	{
		ID = m_genMonsterId++;
		Position = pos;
		Direction = dir;
		
		MonsterBaseID	= 0;
		MonsterGroupID	= 0;
	}
	
	public void SetMonsterBase(uint id)
	{
		MonsterBaseID	= id;
	}
	
	public void SetMonsterGroupID(uint id)
	{
		MonsterGroupID	= id;
	}
	
//	public XMonsterAppearInfo(Vector3 pos, Vector3 dir, XCfgMonsterBase cfg)
//		: this(pos, dir)
//	{
//		cfgInfo = cfg;
//	}
//	
//	public XMonsterAppearInfo(Vector3 pos, Vector3 dir, XCfgMonsterGroup cfg)
//		: this(pos, dir)
//	{
//		cfgGroup = cfg;
//	}
}

public class XPetAppearInfo : XAppearInfoBase
{
	private static ulong m_genPetId = 1;
	public SC_OnePet petInfo { get; private set; }
	
	public XPetAppearInfo(Vector3 pos, Vector3 dir, SC_OnePet info)
	{
		ID = m_genPetId++;
		Position = pos;
		Direction = dir;
		petInfo = info;
	}
}

public class XTransPointAppearInfo
{
	private static ulong m_genTransPointId = 1;
	public ulong ID { get; private set; }
	public uint TransID {get; private set;}
	
	public XTransPointAppearInfo(uint transID)
	{
		TransID = transID;
		ID = m_genTransPointId++;
	}
}


public class XGatherObjectAppearInfo : XAppearInfoBase
{
	private static ulong m_genGOId = 1;
	public int GatherObjectId;
	
	public XGatherObjectAppearInfo(int nGoId, Vector3 pos, float dir)
	{
		ID = m_genGOId++;
		GatherObjectId = nGoId;
		Position = pos;
		Direction = new Vector3(0f, dir, 0f);
	}
}


/*abstract class XApperInfoBase
{
    public ulong ID { get; protected set; }
    public uint ModelID { get; protected set; }
    public Vector3 Position { get; protected set; }
    public Vector3 Direction { get; protected set; }
    public string Name { get; protected set; }
    public string Title { get; protected set; }
    public uint Level { get; protected set; }
    public int MaxHp { get; protected set; }
}

class XApperInfoMonster : XApperInfoBase
{
    public XApperInfoMonster(ulong id, Vector3 pos, Vector3 dir, XCfgMonsterBase info)
    {
        ID = id;
        Position = pos;
        Direction = dir;
        ModelID = info.ModelId;
        Name = info.Name;
        Title = info.Title;
        Level = info.Level;
        MaxHp = info.MaxHp;
    }

    public XApperInfoMonster(ulong id, Vector3 pos, Vector3 dir, XCfgMonsterGroup info)
    {
        ID = id;
        Position = pos;
        Direction = dir;
        ModelID = info.ModelId;
        Name = info.Name;
        Title = info.Title;
		//--4>TEST:
        Level = 10;
        MaxHp = 4444;
    }
}

class XAppearInfoPet : XApperInfoBase
{
    public XAppearInfoPet(ulong id, Vector3 pos, Vector3 dir, SC_OnePet info)
    {
        ID = id;
        Position = pos;
        Direction = dir;
        ModelID = info.Model;
        Name = info.Name;
        Title = info.Title;
        Level = info.Level;
		//--4>TEST:
		// MaxHp = info.MaxHp;
        MaxHp = 4444;
    }
}*/
