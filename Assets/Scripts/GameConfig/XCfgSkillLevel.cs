
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgSkillLevelMgr : CCfgListMgrTemplate<XCfgSkillLevelMgr, XCfgSkillLevel> { };

partial class XCfgSkillLevel : ITabItem
{
	public static readonly string _KEY_SkillID = "SkillID";
	public static readonly string _KEY_SkillEffectType = "SkillEffectType";
	public static readonly string _KEY_SkillLevel = "SkillLevel";
	public static readonly string _KEY_LearnLevel = "LearnLevel";
	public static readonly string _KEY_Kill = "Kill";
	public static readonly string _KEY_UseRegion = "UseRegion";
	public static readonly string _KEY_TextLearn = "TextLearn";
	public static readonly string _KEY_TextEffect = "TextEffect";
	public static readonly string _KEY_TextUpgrade = "TextUpgrade";
	public static readonly string _KEY_ReviveRate = "ReviveRate";
	public static readonly string _KEY_SuckBloodRate = "SuckBloodRate";
	public static readonly string _KEY_SummonID = "SummonID";
	public static readonly string _KEY_MoraleValue = "MoraleValue";
	public static readonly string _KEY_CRIRate = "CRIRate";
	public static readonly string _KEY_CRIHPRate = "CRIHPRate";
	public static readonly string _KEY_AttackCount = "AttackCount";
	public static readonly string _KEY_AttackTime_3_0 = "AttackTime_3_0";
	public static readonly string _KEY_AttackEffect_3_0 = "AttackEffect_3_0";
	public static readonly string _KEY_AttackDamageRate_3_0 = "AttackDamageRate_3_0";
	public static readonly string _KEY_AttackEffectBind_3_0 = "AttackEffectBind_3_0";
	public static readonly string _KEY_AttackTime_3_1 = "AttackTime_3_1";
	public static readonly string _KEY_AttackEffect_3_1 = "AttackEffect_3_1";
	public static readonly string _KEY_AttackDamageRate_3_1 = "AttackDamageRate_3_1";
	public static readonly string _KEY_AttackEffectBind_3_1 = "AttackEffectBind_3_1";
	public static readonly string _KEY_AttackTime_3_2 = "AttackTime_3_2";
	public static readonly string _KEY_AttackEffect_3_2 = "AttackEffect_3_2";
	public static readonly string _KEY_AttackDamageRate_3_2 = "AttackDamageRate_3_2";
	public static readonly string _KEY_AttackEffectBind_3_2 = "AttackEffectBind_3_2";

	public ushort SkillID { get; private set; }				// 技能ID
	public uint SkillEffectType { get; private set; }				// 技能效果类型
	public int SkillLevel { get; private set; }				// 技能等级
	public int LearnLevel { get; private set; }				// 学习最低等级
	public int Kill { get; private set; }				// 消耗怒气值
	public int UseRegion { get; private set; }				// 作用区域
	public string TextLearn { get; private set; }				// 学习时文本
	public string TextEffect { get; private set; }				// 效果文本
	public string TextUpgrade { get; private set; }				// 升级文本
	public float ReviveRate { get; private set; }				// 复活恢复气血百分比
	public float SuckBloodRate { get; private set; }				// 吸血百分比
	public uint SummonID { get; private set; }				// 召唤怪物ID
	public int MoraleValue { get; private set; }				// 增加或降低杀气值
	public int CRIRate { get; private set; }				// 一击必杀概率，万分比
	public int CRIHPRate { get; private set; }				// 一击必杀气血万分比
	public int AttackCount { get; private set; }				// 攻击段数
	public float[] AttackTime { get; private set; }				// 多段攻击命中时间点1
	public int[] AttackEffect { get; private set; }				// 多段攻击命中特效
	public int[] AttackDamageRate { get; private set; }				// 多段攻击伤害万分比
	public int[] AttackEffectBind { get; private set; }				// 多段伤害攻击绑定点

	public XCfgSkillLevel()
	{
		AttackTime = new float[3];
		AttackEffect = new int[3];
		AttackDamageRate = new int[3];
		AttackEffectBind = new int[3];
	}

	public bool ReadItem(TabFile tf)
	{
		SkillID = tf.Get<ushort>(_KEY_SkillID);
		SkillEffectType = tf.Get<uint>(_KEY_SkillEffectType);
		SkillLevel = tf.Get<int>(_KEY_SkillLevel);
		LearnLevel = tf.Get<int>(_KEY_LearnLevel);
		Kill = tf.Get<int>(_KEY_Kill);
		UseRegion = tf.Get<int>(_KEY_UseRegion);
		TextLearn = tf.Get<string>(_KEY_TextLearn);
		TextEffect = tf.Get<string>(_KEY_TextEffect);
		TextUpgrade = tf.Get<string>(_KEY_TextUpgrade);
		ReviveRate = tf.Get<float>(_KEY_ReviveRate);
		SuckBloodRate = tf.Get<float>(_KEY_SuckBloodRate);
		SummonID = tf.Get<uint>(_KEY_SummonID);
		MoraleValue = tf.Get<int>(_KEY_MoraleValue);
		CRIRate = tf.Get<int>(_KEY_CRIRate);
		CRIHPRate = tf.Get<int>(_KEY_CRIHPRate);
		AttackCount = tf.Get<int>(_KEY_AttackCount);
		AttackTime[0] = tf.Get<float>(_KEY_AttackTime_3_0);
		AttackEffect[0] = tf.Get<int>(_KEY_AttackEffect_3_0);
		AttackDamageRate[0] = tf.Get<int>(_KEY_AttackDamageRate_3_0);
		AttackEffectBind[0] = tf.Get<int>(_KEY_AttackEffectBind_3_0);
		AttackTime[1] = tf.Get<float>(_KEY_AttackTime_3_1);
		AttackEffect[1] = tf.Get<int>(_KEY_AttackEffect_3_1);
		AttackDamageRate[1] = tf.Get<int>(_KEY_AttackDamageRate_3_1);
		AttackEffectBind[1] = tf.Get<int>(_KEY_AttackEffectBind_3_1);
		AttackTime[2] = tf.Get<float>(_KEY_AttackTime_3_2);
		AttackEffect[2] = tf.Get<int>(_KEY_AttackEffect_3_2);
		AttackDamageRate[2] = tf.Get<int>(_KEY_AttackDamageRate_3_2);
		AttackEffectBind[2] = tf.Get<int>(_KEY_AttackEffectBind_3_2);
		return true;
	}
}

