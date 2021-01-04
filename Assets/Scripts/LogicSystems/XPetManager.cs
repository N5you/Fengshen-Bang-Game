using System;
using UnityEngine;
using XGame.Client.Packets;
using XGame.Client.Network;
using System.Collections;
using System.Collections.Generic;

public class XPetManager
{
    public static readonly uint MAX_PLAYER_PET_NUM = 20;
	//--4>: 宠物位置下边从 1 开始, 0 无效, 直接忽略
    public XPet[] AllPet { get; private set; }
    public static readonly uint PET_INDEX_BEGIN = 1;
    public static readonly uint PET_INDEX_END = MAX_PLAYER_PET_NUM + 1;

    public XPetManager()
    {
        AllPet = new XPet[MAX_PLAYER_PET_NUM + 1];
    }

    public void Clear()
    {
        Array.Clear(AllPet, 0, AllPet.Length);
    }

    public void On_SC_AllPetData(SC_AllPetData msg)
    {
        this.Clear();
        foreach (SC_OnePet pet in msg.PetsList)
        {
            this.SetPet(pet);
        }
    }
	
	public int GetPetCount()
	{
		int petCount = 0;
		for(uint i = PET_INDEX_BEGIN; i < PET_INDEX_END; i++)
		{
			if(AllPet[i] != null)
				petCount++;
		}
		
		return petCount;
	}
	
	public void SetName(uint index,string name)
	{
		if (XUtil.IsInRange<uint>(index, PET_INDEX_BEGIN, PET_INDEX_END))
		{
			 XPet pet = AllPet[index];
			pet.Name	= name;
			
			XEventManager.SP.SendEvent(EEvent.CharInfo_ChangeName,index,name);
			
		}
	}

    public void SetPet(SC_OnePet info)
    {
        if (XUtil.IsInRange<uint>(info.Index, PET_INDEX_BEGIN, PET_INDEX_END))
        {
			uint idx = info.Index;
            XPet pet = AllPet[idx];
            if (null != pet)
            {
                XLogicWorld.SP.ObjectManager.RemoveObject(EObjectType.Pet, pet.ID);
            }
			pet = XLogicWorld.SP.ObjectManager.AppearPet(new XPetAppearInfo(Vector3.zero, Vector3.zero, info));
            AllPet[idx] = pet;
			
			AllPet[idx].UpdateDynAttrs(info.AttrsList);
			
			XEventManager.SP.SendEvent(EEvent.CharInfo_AddPet,idx);
        }
        else
        {
            //--4>TODO: log error
        }
    }

    public void DelPet(uint idx)
    {
        if (XUtil.IsInRange<uint>(idx, PET_INDEX_BEGIN, PET_INDEX_END))
        {
            AllPet[idx] = null;
			XEventManager.SP.SendEvent(EEvent.CharInfo_DelPet,idx);
        }
        else
        {
            //--4>TODO: log error
        }
    }

    public void On_Sync_BattlePos(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        uint idx = (uint)msg.Uid;
        if (XUtil.IsInRange(idx, PET_INDEX_BEGIN, PET_INDEX_END) && AllPet[idx] != null)
        {
            AllPet[idx].BattlePos = msg.Data;
        }
    }

    public void On_Sync_Loyal(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        uint idx = (uint)msg.Uid;
        if (XUtil.IsInRange(idx, PET_INDEX_BEGIN, PET_INDEX_END) && AllPet[idx] != null)
        {
            AllPet[idx].Loyal = msg.Data;
        }
    }
	
    public void On_Sync_Exp(NetPacket packet)
    {
        SC_UInt msg = SC_UInt.ParseFrom(packet.Message);
        uint idx = (uint)msg.Uid;
        if (XUtil.IsInRange(idx, PET_INDEX_BEGIN, PET_INDEX_END) && AllPet[idx] != null)
        {
            AllPet[idx].Exp = msg.Data;
        }
    }
}
