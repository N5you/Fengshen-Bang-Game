
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgPetBaseMgr : CCfg1KeyMgrTemplate<XCfgPetBaseMgr, uint, XCfgPetBase> { };

partial class XCfgPetBase : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_AptitudeName = "AptitudeName";
	public static readonly string _KEY_AptitudeDes = "AptitudeDes";
	public static readonly string _KEY_Name_4_0 = "Name_4_0";
	public static readonly string _KEY_Name_4_1 = "Name_4_1";
	public static readonly string _KEY_Name_4_2 = "Name_4_2";
	public static readonly string _KEY_Name_4_3 = "Name_4_3";
	public static readonly string _KEY_ModelId_4_0 = "ModelId_4_0";
	public static readonly string _KEY_ModelId_4_1 = "ModelId_4_1";
	public static readonly string _KEY_ModelId_4_2 = "ModelId_4_2";
	public static readonly string _KEY_ModelId_4_3 = "ModelId_4_3";
	public static readonly string _KEY_Race = "Race";
	public static readonly string _KEY_Sex = "Sex";
	public static readonly string _KEY_ClassType = "ClassType";
	public static readonly string _KEY_ClassType2 = "ClassType2";
	public static readonly string _KEY_ClassLevel = "ClassLevel";
	public static readonly string _KEY_InitLevel = "InitLevel";
	public static readonly string _KEY_Aptitude = "Aptitude";
	public static readonly string _KEY_Loyal = "Loyal";
	public static readonly string _KEY_BattleSkill_3_0 = "BattleSkill_3_0";
	public static readonly string _KEY_BattleSkillLevel_3_0 = "BattleSkillLevel_3_0";
	public static readonly string _KEY_BattleSkill_3_1 = "BattleSkill_3_1";
	public static readonly string _KEY_BattleSkillLevel_3_1 = "BattleSkillLevel_3_1";
	public static readonly string _KEY_BattleSkill_3_2 = "BattleSkill_3_2";
	public static readonly string _KEY_BattleSkillLevel_3_2 = "BattleSkillLevel_3_2";
	public static readonly string _KEY_PassiveSkill_5_0 = "PassiveSkill_5_0";
	public static readonly string _KEY_PassiveSkillLevel_5_0 = "PassiveSkillLevel_5_0";
	public static readonly string _KEY_PassiveSkill_5_1 = "PassiveSkill_5_1";
	public static readonly string _KEY_PassiveSkillLevel_5_1 = "PassiveSkillLevel_5_1";
	public static readonly string _KEY_PassiveSkill_5_2 = "PassiveSkill_5_2";
	public static readonly string _KEY_PassiveSkillLevel_5_2 = "PassiveSkillLevel_5_2";
	public static readonly string _KEY_PassiveSkill_5_3 = "PassiveSkill_5_3";
	public static readonly string _KEY_PassiveSkillLevel_5_3 = "PassiveSkillLevel_5_3";
	public static readonly string _KEY_PassiveSkill_5_4 = "PassiveSkill_5_4";
	public static readonly string _KEY_PassiveSkillLevel_5_4 = "PassiveSkillLevel_5_4";
	public static readonly string _KEY_WuLi = "WuLi";
	public static readonly string _KEY_LingQiao = "LingQiao";
	public static readonly string _KEY_TiZhi = "TiZhi";
	public static readonly string _KEY_ShuFa = "ShuFa";
	public static readonly string _KEY_TongLing = "TongLing";
	public static readonly string _KEY_Growth = "Growth";
	public static readonly string _KEY_RandRate = "RandRate";
	public static readonly string _KEY_WuLiRate = "WuLiRate";
	public static readonly string _KEY_LingQiaoRate = "LingQiaoRate";
	public static readonly string _KEY_TiZhiRate = "TiZhiRate";
	public static readonly string _KEY_ShuFaRate = "ShuFaRate";
	public static readonly string _KEY_TongLingRate = "TongLingRate";

	public uint Index { get; private set; }				// ID(唯一标识符, 不能为0)
	public string AptitudeName { get; private set; }				// 特质名
	public string AptitudeDes { get; private set; }				// 特质描述
	public string[] Name { get; private set; }				// 宠物阶段等级为1时所使用的名字
	public uint[] ModelId { get; private set; }				// 阶段等级为1时所使用的模型ID
	public byte Race { get; private set; }				// 种族(1神仙、2鬼煞、3精怪、4妖魔、5圣灵、6凡人、7猛兽、8动物、9机关)
	public byte Sex { get; private set; }				// 性别(0无性别，1女，2男)
	public byte ClassType { get; private set; }				// 职业(4近战防御,5近战攻击，6远程攻击，7远程辅助，8术法攻击，9术法治疗)
	public byte ClassType2 { get; private set; }				// 职业(1战2法3弓手)
	public uint ClassLevel { get; private set; }				// 职业等级（境界等级）
	public uint InitLevel { get; private set; }				// 初始等级（初始化的等级值，默认为1）
	public byte Aptitude { get; private set; }				// 资质(0随机，1凡品，2良品，3上品，4极品，5神品，6圣品)
	public short Loyal { get; private set; }				// 忠诚值（初始化的忠诚值，默认为1）
	public ushort[] BattleSkill { get; private set; }				// 普通攻击技能
	public byte[] BattleSkillLevel { get; private set; }				// 技能等级
	public ushort[] PassiveSkill { get; private set; }				// 被动技能1
	public byte[] PassiveSkillLevel { get; private set; }				// 技能等级
	public int WuLi { get; private set; }				// 初始武力值
	public int LingQiao { get; private set; }				// 初始灵巧值
	public int TiZhi { get; private set; }				// 初始体质值
	public int ShuFa { get; private set; }				// 初始术法值
	public int TongLing { get; private set; }				// 初始统领值,此列要求都为0
	public float Growth { get; private set; }				// 初始成长值
	public byte RandRate { get; private set; }				// 是否随机分布成长率(0不随机,1随机)
	public int WuLiRate { get; private set; }				// 武力成长率(*10000)
	public int LingQiaoRate { get; private set; }				// 灵巧成长率(*10000)
	public int TiZhiRate { get; private set; }				// 体质成长率(*10000)
	public int ShuFaRate { get; private set; }				// 术法成长率(*10000)
	public int TongLingRate { get; private set; }				// 统领成长率(*10000),此列要求都为0

	public XCfgPetBase()
	{
		Name = new string[4];
		ModelId = new uint[4];
		BattleSkill = new ushort[3];
		BattleSkillLevel = new byte[3];
		PassiveSkill = new ushort[5];
		PassiveSkillLevel = new byte[5];
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		AptitudeName = tf.Get<string>(_KEY_AptitudeName);
		AptitudeDes = tf.Get<string>(_KEY_AptitudeDes);
		Name[0] = tf.Get<string>(_KEY_Name_4_0);
		Name[1] = tf.Get<string>(_KEY_Name_4_1);
		Name[2] = tf.Get<string>(_KEY_Name_4_2);
		Name[3] = tf.Get<string>(_KEY_Name_4_3);
		ModelId[0] = tf.Get<uint>(_KEY_ModelId_4_0);
		ModelId[1] = tf.Get<uint>(_KEY_ModelId_4_1);
		ModelId[2] = tf.Get<uint>(_KEY_ModelId_4_2);
		ModelId[3] = tf.Get<uint>(_KEY_ModelId_4_3);
		Race = tf.Get<byte>(_KEY_Race);
		Sex = tf.Get<byte>(_KEY_Sex);
		ClassType = tf.Get<byte>(_KEY_ClassType);
		ClassType2 = tf.Get<byte>(_KEY_ClassType2);
		ClassLevel = tf.Get<uint>(_KEY_ClassLevel);
		InitLevel = tf.Get<uint>(_KEY_InitLevel);
		Aptitude = tf.Get<byte>(_KEY_Aptitude);
		Loyal = tf.Get<short>(_KEY_Loyal);
		BattleSkill[0] = tf.Get<ushort>(_KEY_BattleSkill_3_0);
		BattleSkillLevel[0] = tf.Get<byte>(_KEY_BattleSkillLevel_3_0);
		BattleSkill[1] = tf.Get<ushort>(_KEY_BattleSkill_3_1);
		BattleSkillLevel[1] = tf.Get<byte>(_KEY_BattleSkillLevel_3_1);
		BattleSkill[2] = tf.Get<ushort>(_KEY_BattleSkill_3_2);
		BattleSkillLevel[2] = tf.Get<byte>(_KEY_BattleSkillLevel_3_2);
		PassiveSkill[0] = tf.Get<ushort>(_KEY_PassiveSkill_5_0);
		PassiveSkillLevel[0] = tf.Get<byte>(_KEY_PassiveSkillLevel_5_0);
		PassiveSkill[1] = tf.Get<ushort>(_KEY_PassiveSkill_5_1);
		PassiveSkillLevel[1] = tf.Get<byte>(_KEY_PassiveSkillLevel_5_1);
		PassiveSkill[2] = tf.Get<ushort>(_KEY_PassiveSkill_5_2);
		PassiveSkillLevel[2] = tf.Get<byte>(_KEY_PassiveSkillLevel_5_2);
		PassiveSkill[3] = tf.Get<ushort>(_KEY_PassiveSkill_5_3);
		PassiveSkillLevel[3] = tf.Get<byte>(_KEY_PassiveSkillLevel_5_3);
		PassiveSkill[4] = tf.Get<ushort>(_KEY_PassiveSkill_5_4);
		PassiveSkillLevel[4] = tf.Get<byte>(_KEY_PassiveSkillLevel_5_4);
		WuLi = tf.Get<int>(_KEY_WuLi);
		LingQiao = tf.Get<int>(_KEY_LingQiao);
		TiZhi = tf.Get<int>(_KEY_TiZhi);
		ShuFa = tf.Get<int>(_KEY_ShuFa);
		TongLing = tf.Get<int>(_KEY_TongLing);
		Growth = tf.Get<float>(_KEY_Growth);
		RandRate = tf.Get<byte>(_KEY_RandRate);
		WuLiRate = tf.Get<int>(_KEY_WuLiRate);
		LingQiaoRate = tf.Get<int>(_KEY_LingQiaoRate);
		TiZhiRate = tf.Get<int>(_KEY_TiZhiRate);
		ShuFaRate = tf.Get<int>(_KEY_ShuFaRate);
		TongLingRate = tf.Get<int>(_KEY_TongLingRate);
		return true;
	}
}

