
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMonsterBaseMgr : CCfg1KeyMgrTemplate<XCfgMonsterBaseMgr, uint, XCfgMonsterBase> { };

partial class XCfgMonsterBase : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_Title = "Title";
	public static readonly string _KEY_ModelId = "ModelId";
	public static readonly string _KEY_Zoom = "Zoom";
	public static readonly string _KEY_Chartlet = "Chartlet";
	public static readonly string _KEY_Head = "Head";
	public static readonly string _KEY_Color = "Color";
	public static readonly string _KEY_Radius = "Radius";
	public static readonly string _KEY_Race = "Race";
	public static readonly string _KEY_Sex = "Sex";
	public static readonly string _KEY_Type = "Type";
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_OfficeLevel = "OfficeLevel";
	public static readonly string _KEY_Exp = "Exp";
	public static readonly string _KEY_IsDynamic = "IsDynamic";
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
	public static readonly string _KEY_MaxHp = "MaxHp";
	public static readonly string _KEY_InitAngerValue = "InitAngerValue";
	public static readonly string _KEY_MaxAngerValue = "MaxAngerValue";
	public static readonly string _KEY_AngerGet = "AngerGet";
	public static readonly string _KEY_QuestDrop_3_0 = "QuestDrop_3_0";
	public static readonly string _KEY_QuestDrop_3_1 = "QuestDrop_3_1";
	public static readonly string _KEY_QuestDrop_3_2 = "QuestDrop_3_2";
	public static readonly string _KEY_TCDrop = "TCDrop";
	public static readonly string _KEY_FirstBossTCDrop = "FirstBossTCDrop";

	public uint Index { get; private set; }				// ID(唯一标识符)
	public string Name { get; private set; }				// 名字
	public string Title { get; private set; }				// 称号
	public uint ModelId { get; private set; }				// 模型ID
	public float Zoom { get; private set; }				// 模型缩放比例
	public uint Chartlet { get; private set; }				// 贴图索引
	public uint Head { get; private set; }				// 头像
	public uint Color { get; private set; }				// 染色值
	public uint Radius { get; private set; }				// 半径
	public byte Race { get; private set; }				// 种族
	public byte Sex { get; private set; }				// 性别(0无性别，1女，2男)
	public byte Type { get; private set; }				// 职业类型(1近战防御；2近战攻击;3远程攻击；4法术攻击，5治疗辅助)
	public uint Level { get; private set; }				// 等级
	public byte OfficeLevel { get; private set; }				// 实力等级(1普通；2BOSS；3超级BOSS)
	public uint Exp { get; private set; }				// 经验值(杀死获得)
	public byte IsDynamic { get; private set; }				// 是否调整属性(是否受场景难度选择影响，0为不受）
	public ushort[] BattleSkill { get; private set; }				// 普通攻击技能
	public byte[] BattleSkillLevel { get; private set; }				// 技能等级
	public ushort[] PassiveSkill { get; private set; }				// 被动技能1
	public byte[] PassiveSkillLevel { get; private set; }				// 技能等级
	public int MaxHp { get; private set; }				// 生命上限值
	public int InitAngerValue { get; private set; }				// 初始怒气值
	public int MaxAngerValue { get; private set; }				// 怒气上限值
	public int AngerGet { get; private set; }				// 怒气获取值
	public uint[] QuestDrop { get; private set; }				// 任务掉落1
	public uint TCDrop { get; private set; }				// 怪物TC掉落
	public uint FirstBossTCDrop { get; private set; }				// 首次击杀BOSS掉落TCID

	public XCfgMonsterBase()
	{
		BattleSkill = new ushort[3];
		BattleSkillLevel = new byte[3];
		PassiveSkill = new ushort[5];
		PassiveSkillLevel = new byte[5];
		QuestDrop = new uint[3];
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		Title = tf.Get<string>(_KEY_Title);
		ModelId = tf.Get<uint>(_KEY_ModelId);
		Zoom = tf.Get<float>(_KEY_Zoom);
		Chartlet = tf.Get<uint>(_KEY_Chartlet);
		Head = tf.Get<uint>(_KEY_Head);
		Color = tf.Get<uint>(_KEY_Color);
		Radius = tf.Get<uint>(_KEY_Radius);
		Race = tf.Get<byte>(_KEY_Race);
		Sex = tf.Get<byte>(_KEY_Sex);
		Type = tf.Get<byte>(_KEY_Type);
		Level = tf.Get<uint>(_KEY_Level);
		OfficeLevel = tf.Get<byte>(_KEY_OfficeLevel);
		Exp = tf.Get<uint>(_KEY_Exp);
		IsDynamic = tf.Get<byte>(_KEY_IsDynamic);
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
		MaxHp = tf.Get<int>(_KEY_MaxHp);
		InitAngerValue = tf.Get<int>(_KEY_InitAngerValue);
		MaxAngerValue = tf.Get<int>(_KEY_MaxAngerValue);
		AngerGet = tf.Get<int>(_KEY_AngerGet);
		QuestDrop[0] = tf.Get<uint>(_KEY_QuestDrop_3_0);
		QuestDrop[1] = tf.Get<uint>(_KEY_QuestDrop_3_1);
		QuestDrop[2] = tf.Get<uint>(_KEY_QuestDrop_3_2);
		TCDrop = tf.Get<uint>(_KEY_TCDrop);
		FirstBossTCDrop = tf.Get<uint>(_KEY_FirstBossTCDrop);
		return true;
	}
}

