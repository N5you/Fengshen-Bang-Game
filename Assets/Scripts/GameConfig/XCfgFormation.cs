
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgFormationMgr : CCfg1KeyMgrTemplate<XCfgFormationMgr, uint, XCfgFormation> { };

partial class XCfgFormation : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_UID = "UID";
	public static readonly string _KEY_StrName = "StrName";
	public static readonly string _KEY_AttackType = "AttackType";
	public static readonly string _KEY_On_CamSize = "On_CamSize";
	public static readonly string _KEY_On_ScaleX = "On_ScaleX";
	public static readonly string _KEY_On_ScaleY = "On_ScaleY";
	public static readonly string _KEY_On_ScaleZ = "On_ScaleZ";
	public static readonly string _KEY_On_PositionX = "On_PositionX";
	public static readonly string _KEY_On_PositionY = "On_PositionY";
	public static readonly string _KEY_On_PositionZ = "On_PositionZ";
	public static readonly string _KEY_On_RotationX = "On_RotationX";
	public static readonly string _KEY_On_RotationY = "On_RotationY";
	public static readonly string _KEY_On_RotationZ = "On_RotationZ";
	public static readonly string _KEY_Off_CamSize = "Off_CamSize";
	public static readonly string _KEY_Off_ScaleX = "Off_ScaleX";
	public static readonly string _KEY_Off_ScaleY = "Off_ScaleY";
	public static readonly string _KEY_Off_ScaleZ = "Off_ScaleZ";
	public static readonly string _KEY_Off_PositionX = "Off_PositionX";
	public static readonly string _KEY_Off_PositionY = "Off_PositionY";
	public static readonly string _KEY_Off_PositionZ = "Off_PositionZ";
	public static readonly string _KEY_Off_RotationX = "Off_RotationX";
	public static readonly string _KEY_Off_RotationY = "Off_RotationY";
	public static readonly string _KEY_Off_RotationZ = "Off_RotationZ";

	public uint UID { get; private set; }				// 宠物ID
	public string StrName { get; private set; }				// 宠物名称
	public ushort AttackType { get; private set; }				// 宠物攻击类型
	public float On_CamSize { get; private set; }				// 摄像机大小
	public float On_ScaleX { get; private set; }				// 大小X
	public float On_ScaleY { get; private set; }				// 大小Y
	public float On_ScaleZ { get; private set; }				// 大小Z
	public float On_PositionX { get; private set; }				// 位置X
	public float On_PositionY { get; private set; }				// 位置Y
	public float On_PositionZ { get; private set; }				// 位置Z
	public float On_RotationX { get; private set; }				// 方向X
	public float On_RotationY { get; private set; }				// 方向Y
	public float On_RotationZ { get; private set; }				// 方向Z
	public float Off_CamSize { get; private set; }				// 摄像机大小
	public float Off_ScaleX { get; private set; }				// 大小X
	public float Off_ScaleY { get; private set; }				// 大小Y
	public float Off_ScaleZ { get; private set; }				// 大小Z
	public float Off_PositionX { get; private set; }				// 位置X
	public float Off_PositionY { get; private set; }				// 位置Y
	public float Off_PositionZ { get; private set; }				// 位置Z
	public float Off_RotationX { get; private set; }				// 方向X
	public float Off_RotationY { get; private set; }				// 方向Y
	public float Off_RotationZ { get; private set; }				// 方向Z

	public XCfgFormation()
	{
	}

	public uint GetKey1() { return UID; }

	public bool ReadItem(TabFile tf)
	{
		UID = tf.Get<uint>(_KEY_UID);
		StrName = tf.Get<string>(_KEY_StrName);
		AttackType = tf.Get<ushort>(_KEY_AttackType);
		On_CamSize = tf.Get<float>(_KEY_On_CamSize);
		On_ScaleX = tf.Get<float>(_KEY_On_ScaleX);
		On_ScaleY = tf.Get<float>(_KEY_On_ScaleY);
		On_ScaleZ = tf.Get<float>(_KEY_On_ScaleZ);
		On_PositionX = tf.Get<float>(_KEY_On_PositionX);
		On_PositionY = tf.Get<float>(_KEY_On_PositionY);
		On_PositionZ = tf.Get<float>(_KEY_On_PositionZ);
		On_RotationX = tf.Get<float>(_KEY_On_RotationX);
		On_RotationY = tf.Get<float>(_KEY_On_RotationY);
		On_RotationZ = tf.Get<float>(_KEY_On_RotationZ);
		Off_CamSize = tf.Get<float>(_KEY_Off_CamSize);
		Off_ScaleX = tf.Get<float>(_KEY_Off_ScaleX);
		Off_ScaleY = tf.Get<float>(_KEY_Off_ScaleY);
		Off_ScaleZ = tf.Get<float>(_KEY_Off_ScaleZ);
		Off_PositionX = tf.Get<float>(_KEY_Off_PositionX);
		Off_PositionY = tf.Get<float>(_KEY_Off_PositionY);
		Off_PositionZ = tf.Get<float>(_KEY_Off_PositionZ);
		Off_RotationX = tf.Get<float>(_KEY_Off_RotationX);
		Off_RotationY = tf.Get<float>(_KEY_Off_RotationY);
		Off_RotationZ = tf.Get<float>(_KEY_Off_RotationZ);
		return true;
	}
}

