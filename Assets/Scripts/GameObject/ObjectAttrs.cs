using System;
using UnityEngine;
using XGame.Client.Packets;
public enum EObjectType
{
	Begin = 0,
	GameObject = 0,
	Character,
	Npc,
	Monster,
	Pet,
	OtherPlayer,
	MainPlayer,
	Item,
	TransPoint,
	GatherObject,
	End,
}

public class XAttrGameObject
{
	internal String Name = string.Empty;
	internal uint ModelId = 0;
	internal Vector3 Position = Vector3.zero;
	internal Vector3 Direction = Vector3.zero;
	internal float Scale = 1.0f;
	internal Color MatColor = Color.white;
}

public class XAttrCharacter
{
	internal float Speed = 0f;
	internal String Title = string.Empty;
	internal String NickName = string.Empty;
	internal byte Sex = 0;
	internal int Hp = 0;
	public uint ArmourItemID = 0;
	internal uint ShowFashion = 0;
	internal uint FashionId = 0;
	internal uint WeaponItemID = 0;
	internal uint WeaponModelID = 0;
	internal XDynamicAttrMgr DynamicAttrs = new XDynamicAttrMgr ();
}

public class XAttrNpc
{
}

public class XAttrMonster
{
}

public class XAttrPet
{
	internal uint Index = 0;
	internal uint PetID = 0;
	internal uint ClassLevel = 0;
	internal uint Race = 0;
	internal uint UColor = 0;
	internal uint Aptitude = 0;
	internal uint Loyal = 0;
	internal uint Exp = 0;
	internal uint BattlePos = 0;
	internal uint WuLiValue = 0;
	internal uint LingQiaoValue = 0;
	internal uint TiZhiValue = 0;
	internal uint ShuFaValue = 0;
}

public class XAttrPlayer
{
	internal uint UColor = 0;
	internal uint ClassLevel = 0;
	internal ushort MountIndex = 0;
}

public class XAttrMainPlayer
{
	internal long GameMoney = 0;
	internal long RealMoney = 0;
	internal uint Exp = 0;
	internal uint BagSize = 0;
	internal uint BankSize = 0;
	internal uint Power = 0;
	internal uint FightRemain = 0;
	internal uint WuLiValue = 0;
	internal uint LingQiaoValue = 0;
	internal uint TiZhiValue = 0;
	internal uint ShuFaValue = 0;
	internal uint HealthBuyCount = 0;
	internal uint VIPLevel = 0;
	internal uint EatFoodCount = 0;
	internal uint ShengWangValue = 0;
	internal uint TotalShengWangValue =0;
	internal uint DayPeiYangCount = 0;
	internal uint ShowFashion	  = 0;					// 0 显示服装，1不显示服装
	internal ulong GuildId			  = 0;
	internal ulong Recharge		  = 0;
	internal uint VipAward		  = 0;
	// 战场布局站位
	internal uint BattlePos = 0;
}
