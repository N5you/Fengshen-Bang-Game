
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgSkillBaseMgr : CCfg1KeyMgrTemplate<XCfgSkillBaseMgr, ushort, XCfgSkillBase> { };

partial class XCfgSkillBase : ITabItemWith1Key<ushort>
{
	public static readonly string _KEY_SkillID = "SkillID";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_FuncType = "FuncType";
	public static readonly string _KEY_LevelLimit = "LevelLimit";
	public static readonly string _KEY_Class = "Class";
	public static readonly string _KEY_AttackTarget = "AttackTarget";
	public static readonly string _KEY_UseWay = "UseWay";
	public static readonly string _KEY_SkillTime = "SkillTime";
	public static readonly string _KEY_AttackAnim = "AttackAnim";
	public static readonly string _KEY_AttackAnimSpeed = "AttackAnimSpeed";
	public static readonly string _KEY_AttackEffectDelay = "AttackEffectDelay";
	public static readonly string _KEY_AttackEffectID = "AttackEffectID";
	public static readonly string _KEY_AttackEffectBind = "AttackEffectBind";
	public static readonly string _KEY_IsFollowBone = "IsFollowBone";
	public static readonly string _KEY_AttackEffectNum = "AttackEffectNum";
	public static readonly string _KEY_AttackEffectLife = "AttackEffectLife";
	public static readonly string _KEY_TranslateDelay = "TranslateDelay";
	public static readonly string _KEY_UseObject = "UseObject";
	public static readonly string _KEY_BulletID = "BulletID";
	public static readonly string _KEY_BulletSrcBind = "BulletSrcBind";
	public static readonly string _KEY_BulletTgtBind = "BulletTgtBind";
	public static readonly string _KEY_BulletRate = "BulletRate";
	public static readonly string _KEY_BulletVelocity = "BulletVelocity";
	public static readonly string _KEY_BulletFlyTrack = "BulletFlyTrack";
	public static readonly string _KEY_RegionEffectID = "RegionEffectID";
	public static readonly string _KEY_DefeatDelay = "DefeatDelay";
	public static readonly string _KEY_HitEffectID = "HitEffectID";
	public static readonly string _KEY_HitEffectBind = "HitEffectBind";
	public static readonly string _KEY_HitEffectNum = "HitEffectNum";
	public static readonly string _KEY_AttackShockID = "AttackShockID";
	public static readonly string _KEY_AttackShockDelay = "AttackShockDelay";
	public static readonly string _KEY_HitShockID = "HitShockID";
	public static readonly string _KEY_ShowCutSceneAnimation = "ShowCutSceneAnimation";

	public ushort SkillID { get; private set; }				// 技能ID
	public string Name { get; private set; }				// 技能名称
	public int FuncType { get; private set; }				// 技能功能类型
	public int LevelLimit { get; private set; }				// 等级上限
	public byte Class { get; private set; }				// 职业许可
	public int AttackTarget { get; private set; }				// 攻击目标
	public int UseWay { get; private set; }				// 作用方式
	public float SkillTime { get; private set; }				// 技能时间
	public int AttackAnim { get; private set; }				// 施展动画
	public float AttackAnimSpeed { get; private set; }				// 施展动画速率
	public float AttackEffectDelay { get; private set; }				// 攻击特效延时(1.0s)
	public int AttackEffectID { get; private set; }				// 攻击特效ID
	public int AttackEffectBind { get; private set; }				// 攻击特效绑点
	public int IsFollowBone { get; private set; }				// 特效是否跟随骨骼运动
	public int AttackEffectNum { get; private set; }				// 攻击特效数量
	public float AttackEffectLife { get; private set; }				// 攻击特效生存时长
	public float TranslateDelay { get; private set; }				// 技能传递延时(s)
	public int UseObject { get; private set; }				// 作用对象
	public int BulletID { get; private set; }				// 子弹ID
	public int BulletSrcBind { get; private set; }				// 子弹发射源绑定点
	public int BulletTgtBind { get; private set; }				// 子弹发射目标绑定点
	public float BulletRate { get; private set; }				// 子弹缩放比率
	public float BulletVelocity { get; private set; }				// 子弹飞行速度
	public float BulletFlyTrack { get; private set; }				// 子弹的飞行轨迹(H/W)
	public int RegionEffectID { get; private set; }				// 区域特效ID
	public float DefeatDelay { get; private set; }				// 生效延时(1.0s)
	public int HitEffectID { get; private set; }				// 命中特效ID
	public int HitEffectBind { get; private set; }				// 命中特效绑点
	public int HitEffectNum { get; private set; }				// 命中特效数量
	public int AttackShockID { get; private set; }				// 攻击时震屏特效索引
	public float AttackShockDelay { get; private set; }				// 攻击震屏特效延时
	public int HitShockID { get; private set; }				// 被击时震屏特效索引
	public int ShowCutSceneAnimation { get; private set; }				// 播放特殊场景动画

	public XCfgSkillBase()
	{
	}

	public ushort GetKey1() { return SkillID; }

	public bool ReadItem(TabFile tf)
	{
		SkillID = tf.Get<ushort>(_KEY_SkillID);
		Name = tf.Get<string>(_KEY_Name);
		FuncType = tf.Get<int>(_KEY_FuncType);
		LevelLimit = tf.Get<int>(_KEY_LevelLimit);
		Class = tf.Get<byte>(_KEY_Class);
		AttackTarget = tf.Get<int>(_KEY_AttackTarget);
		UseWay = tf.Get<int>(_KEY_UseWay);
		SkillTime = tf.Get<float>(_KEY_SkillTime);
		AttackAnim = tf.Get<int>(_KEY_AttackAnim);
		AttackAnimSpeed = tf.Get<float>(_KEY_AttackAnimSpeed);
		AttackEffectDelay = tf.Get<float>(_KEY_AttackEffectDelay);
		AttackEffectID = tf.Get<int>(_KEY_AttackEffectID);
		AttackEffectBind = tf.Get<int>(_KEY_AttackEffectBind);
		IsFollowBone = tf.Get<int>(_KEY_IsFollowBone);
		AttackEffectNum = tf.Get<int>(_KEY_AttackEffectNum);
		AttackEffectLife = tf.Get<float>(_KEY_AttackEffectLife);
		TranslateDelay = tf.Get<float>(_KEY_TranslateDelay);
		UseObject = tf.Get<int>(_KEY_UseObject);
		BulletID = tf.Get<int>(_KEY_BulletID);
		BulletSrcBind = tf.Get<int>(_KEY_BulletSrcBind);
		BulletTgtBind = tf.Get<int>(_KEY_BulletTgtBind);
		BulletRate = tf.Get<float>(_KEY_BulletRate);
		BulletVelocity = tf.Get<float>(_KEY_BulletVelocity);
		BulletFlyTrack = tf.Get<float>(_KEY_BulletFlyTrack);
		RegionEffectID = tf.Get<int>(_KEY_RegionEffectID);
		DefeatDelay = tf.Get<float>(_KEY_DefeatDelay);
		HitEffectID = tf.Get<int>(_KEY_HitEffectID);
		HitEffectBind = tf.Get<int>(_KEY_HitEffectBind);
		HitEffectNum = tf.Get<int>(_KEY_HitEffectNum);
		AttackShockID = tf.Get<int>(_KEY_AttackShockID);
		AttackShockDelay = tf.Get<float>(_KEY_AttackShockDelay);
		HitShockID = tf.Get<int>(_KEY_HitShockID);
		ShowCutSceneAnimation = tf.Get<int>(_KEY_ShowCutSceneAnimation);
		return true;
	}
}

