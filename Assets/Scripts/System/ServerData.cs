using System;

//--4>TODO: 务必保证客户端和服务器一致
public enum EPlayerSex
{
	Begin = 0,
	Unknown = Begin,
	Female = 1,
	Male = 2,
	Both,		// 双性, 用于男女公用的判断
	End
}

// 职业类别
public enum EPlayerClass
{
	Begin = 1,
	ZhanShi = Begin,
	FaShi,
	GongJianShou,
    End,
};

public class ServerData
{
    public static readonly uint QUEST_MAX_ID = 32768;
}
