using System;
using UnityEngine;
using XGame.Client.Packets;

public class XPet : XCharacter
{
    public XPet(ulong id) : base(id)
    {
        ObjectType = EObjectType.Pet;
    }

    public override void SetAppearData(object data)
    {
		XPetAppearInfo info = data as XPetAppearInfo;
        if (null == info)
        {
            return;
        }
		Position = info.Position;
		Direction = info.Direction;		
		Name = info.petInfo.Name;
        Title = info.petInfo.Title;
		SetModel(EModelCtrlType.eModelCtrl_Original, info.petInfo.Model);
		Level = (int)info.petInfo.Level;
		
		Exp = info.petInfo.Exp;
		//--4>TODO: test
        Hp = MaxHp = 4444; //info.MaxHp;

        Index = info.petInfo.Index;
        PetID = info.petInfo.PetID;
        DynSet(EShareAttr.esa_Class, (byte)info.petInfo.PetClass);
        ClassLevel = info.petInfo.ClassLevel;
        Race = info.petInfo.Race;
        UColor = info.petInfo.Color;
        DynSet(EShareAttr.esa_Grow, (int)(info.petInfo.Grow * Define.CONFIG_RATE_BASE));
        Aptitude = info.petInfo.Aptitude;
        Loyal = info.petInfo.Loyal;
        BattlePos = info.petInfo.BattlePos;
		
		WuLi	= info.petInfo.WuLi;
		LingQiao= info.petInfo.LingQiao;
		TiZhi	= info.petInfo.TiZhi;
		ShuFa	= info.petInfo.ShuFa;
		
    }
	
	public  override void OnModelLoaded()
	{
		base.OnModelLoaded();
		if(m_ObjectModel != null)
			m_ObjectModel.AddBehaviourListener(EBehaviourType.e_BehaviourType_Monster,this);
	}
	
	public static Pet_Loyal_Type	GetLoyalType(XPet pet)
	{
		uint Loyal	= pet.Loyal;
		if(Loyal >= 1 && Loyal <= 99)
			return Pet_Loyal_Type.Pet_Logyal_FanKang;
		else if(Loyal >= 100 && Loyal <= 299 )
			return Pet_Loyal_Type.Pet_Logyal_ShunCong;
		else if(Loyal >= 100 && Loyal <= 299)
			return Pet_Loyal_Type.Pet_Logyal_XinLai;
		else if(Loyal >= 100 && Loyal <= 299)
			return Pet_Loyal_Type.Pet_Logyal_ZhongCheng;
		
		return Pet_Loyal_Type.Pet_Logyal_None;
	}
	
    #region attr set
    private XAttrPet m_AttrPet = new XAttrPet();
	
    public uint Index
    {
        get { return m_AttrPet.Index; }
        set
        {
			if(Index != value)
			{
            	m_AttrPet.Index = value;
			}
        }
    }

    public uint PetID
    {
        get { return m_AttrPet.PetID; }
        set
        {
			if(PetID != value)
			{
            	m_AttrPet.PetID = value;
			}
        }
    }

    public uint ClassLevel
    {
        get { return m_AttrPet.ClassLevel; }
        set
        {
			if(ClassLevel != value)
			{
            	m_AttrPet.ClassLevel = value;
			}
        }
    }
	public uint Exp
    {
        get { return m_AttrPet.Exp; }
        set
        {
			if(Exp != value)
			{
            	m_AttrPet.Exp = value;
			}
        }
    }
    public uint Race
    {
        get { return m_AttrPet.Race; }
        set
        {
			if(Race != value)
			{
            	m_AttrPet.Race = value;
			}
        }
    }

    public uint UColor
    {
        get { return m_AttrPet.UColor; }
        set
        {
			if(UColor != value)
			{
            	m_AttrPet.UColor = value;
			}
        }
    }

	//--4>TODO: 建议增加角色和宠物公用的基类对象, 方便对公用属性的设置
    public uint Aptitude
    {
        get { return m_AttrPet.Aptitude; }
        set
        {
			if(Aptitude != value)
			{
            	m_AttrPet.Aptitude = value;
			}
        }
    }
    public uint Loyal
    {
        get { return m_AttrPet.Loyal; }
        set
        {
			if(Loyal != value)
			{
            	m_AttrPet.Loyal = value;
			}
        }
    }
	
    public uint BattlePos
    {
        get { return m_AttrPet.BattlePos; }
        set
        {
			if(BattlePos != value)
			{
            	m_AttrPet.BattlePos = value;				
			}
        }
    }
	
	public uint WuLi
	{
		get
		{
			return m_AttrPet.WuLiValue;
		}
		set
		{
			if(m_AttrPet.WuLiValue != value)
			{
				m_AttrPet.WuLiValue	= value;
				XEventManager.SP.SendEvent(EEvent.CharInfo_UpdateLingDan);
			}
		}
	}
	
	public uint LingQiao
	{
		get
		{
			return m_AttrPet.LingQiaoValue;
		}
		set
		{
			if(m_AttrPet.LingQiaoValue != value)
			{
				m_AttrPet.LingQiaoValue	= value;
				XEventManager.SP.SendEvent(EEvent.CharInfo_UpdateLingDan);
			}
		}
	}
	
	public uint TiZhi
	{
		get
		{
			return m_AttrPet.TiZhiValue;
		}
		set
		{
			if(m_AttrPet.TiZhiValue != value)
			{
				m_AttrPet.TiZhiValue	= value;
				XEventManager.SP.SendEvent(EEvent.CharInfo_UpdateLingDan);
			}
		}
	}
	
	public uint ShuFa
	{
		get
		{
			return m_AttrPet.ShuFaValue;
		}
		set
		{
			if(m_AttrPet.ShuFaValue != value)
			{
				m_AttrPet.ShuFaValue	= value;
				XEventManager.SP.SendEvent(EEvent.CharInfo_UpdateLingDan);
			}
		}
	}
	
    #endregion
}
