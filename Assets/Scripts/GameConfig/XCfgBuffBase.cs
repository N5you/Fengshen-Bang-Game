
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgBuffBaseMgr : CCfg1KeyMgrTemplate<XCfgBuffBaseMgr, uint, XCfgBuffBase> { };

partial class XCfgBuffBase : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_BuffType = "BuffType";
	public static readonly string _KEY_EffectType = "EffectType";
	public static readonly string _KEY_LifeType = "LifeType";
	public static readonly string _KEY_LifeValue = "LifeValue";
	public static readonly string _KEY_LifeOverlayType = "LifeOverlayType";
	public static readonly string _KEY_MaxOverLayNum = "MaxOverLayNum";
	public static readonly string _KEY_DisperseNum = "DisperseNum";
	public static readonly string _KEY_DisappearType = "DisappearType";
	public static readonly string _KEY_AffectPriority = "AffectPriority";
	public static readonly string _KEY_ShowIcon = "ShowIcon";
	public static readonly string _KEY_AtlasId = "AtlasId";
	public static readonly string _KEY_SpriteName = "SpriteName";
	public static readonly string _KEY_EffectOnAdd = "EffectOnAdd";
	public static readonly string _KEY_EffectSkeOnAdd = "EffectSkeOnAdd";
	public static readonly string _KEY_EffectPosOnAdd = "EffectPosOnAdd";
	public static readonly string _KEY_EffectNumOnAdd = "EffectNumOnAdd";
	public static readonly string _KEY_EffectOnAttach = "EffectOnAttach";
	public static readonly string _KEY_EffectDelayOnAttach = "EffectDelayOnAttach";
	public static readonly string _KEY_EffectSkeOnAttach = "EffectSkeOnAttach";
	public static readonly string _KEY_EffectPosOnAttach = "EffectPosOnAttach";
	public static readonly string _KEY_EffectNumOnAttach = "EffectNumOnAttach";
	public static readonly string _KEY_EffectOnDisperse = "EffectOnDisperse";
	public static readonly string _KEY_EffectSkeOnDisperse = "EffectSkeOnDisperse";
	public static readonly string _KEY_EffectNumOnDisperse = "EffectNumOnDisperse";
	public static readonly string _KEY_EffectOnRemove = "EffectOnRemove";
	public static readonly string _KEY_EffectSkeOnRemove = "EffectSkeOnRemove";
	public static readonly string _KEY_EffectNumOnRemove = "EffectNumOnRemove";

	public uint Index { get; private set; }				// buff_id
	public string Name { get; private set; }				// buff名称
	public byte BuffType { get; private set; }				// 0:BUFF/1:DEBUFF
	public byte EffectType { get; private set; }				// 效果类型
	public byte LifeType { get; private set; }				// 生存类型
	public uint LifeValue { get; private set; }				// 生存值(单位是(秒)或者次数)
	public byte LifeOverlayType { get; private set; }				// 不变/相加/重置
	public byte MaxOverLayNum { get; private set; }				// 最大叠加数量
	public byte DisperseNum { get; private set; }				// 驱散层数
	public byte DisappearType { get; private set; }				// 在何时强行删除(无/死亡后/战斗结束后/both)
	public int AffectPriority { get; private set; }				// 作用优先级
	public int ShowIcon { get; private set; }				// 0:NoShow/1:sShow
	public int AtlasId { get; private set; }				// 图集ID
	public string SpriteName { get; private set; }				// 精灵名称
	public uint EffectOnAdd { get; private set; }				// buff增加时特效
	public int EffectSkeOnAdd { get; private set; }				// buff增加时特效绑点
	public Vector3 EffectPosOnAdd { get; private set; }				// 与绑点的相对坐标
	public int EffectNumOnAdd { get; private set; }				// buff增加时特效数量
	public uint EffectOnAttach { get; private set; }				// buff绑定时特效
	public float EffectDelayOnAttach { get; private set; }				// buff绑定特效播放延时
	public int EffectSkeOnAttach { get; private set; }				// buff绑定时特效绑点
	public Vector3 EffectPosOnAttach { get; private set; }				// 与绑点的相对坐标
	public int EffectNumOnAttach { get; private set; }				// buff绑定时特效数量
	public uint EffectOnDisperse { get; private set; }				// 驱散时特效
	public int EffectSkeOnDisperse { get; private set; }				// 驱散时特效绑点
	public int EffectNumOnDisperse { get; private set; }				// 驱散时特效数量
	public uint EffectOnRemove { get; private set; }				// 删除时特效
	public int EffectSkeOnRemove { get; private set; }				// 删除时特效绑点
	public int EffectNumOnRemove { get; private set; }				// 删除时特效数量

	public XCfgBuffBase()
	{
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		BuffType = tf.Get<byte>(_KEY_BuffType);
		EffectType = tf.Get<byte>(_KEY_EffectType);
		LifeType = tf.Get<byte>(_KEY_LifeType);
		LifeValue = tf.Get<uint>(_KEY_LifeValue);
		LifeOverlayType = tf.Get<byte>(_KEY_LifeOverlayType);
		MaxOverLayNum = tf.Get<byte>(_KEY_MaxOverLayNum);
		DisperseNum = tf.Get<byte>(_KEY_DisperseNum);
		DisappearType = tf.Get<byte>(_KEY_DisappearType);
		AffectPriority = tf.Get<int>(_KEY_AffectPriority);
		ShowIcon = tf.Get<int>(_KEY_ShowIcon);
		AtlasId = tf.Get<int>(_KEY_AtlasId);
		SpriteName = tf.Get<string>(_KEY_SpriteName);
		EffectOnAdd = tf.Get<uint>(_KEY_EffectOnAdd);
		EffectSkeOnAdd = tf.Get<int>(_KEY_EffectSkeOnAdd);
		EffectPosOnAdd = XUtil.String2Vector3(tf.Get<string>(_KEY_EffectPosOnAdd));
		EffectNumOnAdd = tf.Get<int>(_KEY_EffectNumOnAdd);
		EffectOnAttach = tf.Get<uint>(_KEY_EffectOnAttach);
		EffectDelayOnAttach = tf.Get<float>(_KEY_EffectDelayOnAttach);
		EffectSkeOnAttach = tf.Get<int>(_KEY_EffectSkeOnAttach);
		EffectPosOnAttach = XUtil.String2Vector3(tf.Get<string>(_KEY_EffectPosOnAttach));
		EffectNumOnAttach = tf.Get<int>(_KEY_EffectNumOnAttach);
		EffectOnDisperse = tf.Get<uint>(_KEY_EffectOnDisperse);
		EffectSkeOnDisperse = tf.Get<int>(_KEY_EffectSkeOnDisperse);
		EffectNumOnDisperse = tf.Get<int>(_KEY_EffectNumOnDisperse);
		EffectOnRemove = tf.Get<uint>(_KEY_EffectOnRemove);
		EffectSkeOnRemove = tf.Get<int>(_KEY_EffectSkeOnRemove);
		EffectNumOnRemove = tf.Get<int>(_KEY_EffectNumOnRemove);
		return true;
	}
}

