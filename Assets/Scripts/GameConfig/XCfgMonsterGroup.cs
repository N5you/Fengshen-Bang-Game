
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMonsterGroupMgr : CCfg1KeyMgrTemplate<XCfgMonsterGroupMgr, uint, XCfgMonsterGroup> { };

partial class XCfgMonsterGroup : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_Title = "Title";
	public static readonly string _KEY_ModelId = "ModelId";
	public static readonly string _KEY_Zoom = "Zoom";
	public static readonly string _KEY_Chartlet = "Chartlet";
	public static readonly string _KEY_Color = "Color";
	public static readonly string _KEY_MoveType = "MoveType";
	public static readonly string _KEY_MoveSpeed = "MoveSpeed";
	public static readonly string _KEY_RunSpeed = "RunSpeed";
	public static readonly string _KEY_TurnSpeed = "TurnSpeed";
	public static readonly string _KEY_Height = "Height";
	public static readonly string _KEY_FollowRadius = "FollowRadius";
	public static readonly string _KEY_AttackRadius = "AttackRadius";
	public static readonly string _KEY_SeeRadius = "SeeRadius";
	public static readonly string _KEY_GroupType = "GroupType";
	public static readonly string _KEY_MonID_10_0 = "MonID_10_0";
	public static readonly string _KEY_MonID_10_1 = "MonID_10_1";
	public static readonly string _KEY_MonID_10_2 = "MonID_10_2";
	public static readonly string _KEY_MonID_10_3 = "MonID_10_3";
	public static readonly string _KEY_MonID_10_4 = "MonID_10_4";
	public static readonly string _KEY_MonID_10_5 = "MonID_10_5";
	public static readonly string _KEY_MonID_10_6 = "MonID_10_6";
	public static readonly string _KEY_MonID_10_7 = "MonID_10_7";
	public static readonly string _KEY_MonID_10_8 = "MonID_10_8";
	public static readonly string _KEY_MonID_10_9 = "MonID_10_9";

	public uint Index { get; private set; }				// 类型ID(唯一标识符)
	public string Name { get; private set; }				// 名字
	public string Title { get; private set; }				// 称号
	public uint ModelId { get; private set; }				// 模型ID
	public float Zoom { get; private set; }				// 模型缩放比例
	public uint Chartlet { get; private set; }				// 贴图索引
	public uint Color { get; private set; }				// 染色值
	public byte MoveType { get; private set; }				// 行动方式(1原地不动；2范围内随机移动；3巡逻)
	public float MoveSpeed { get; private set; }				// 在场景中的随机行走速度
	public float RunSpeed { get; private set; }				// 发现敌人后的追击移动速度
	public uint TurnSpeed { get; private set; }				// 转向速度
	public uint Height { get; private set; }				// 高度(距离地面的高度-飞行类生物)
	public uint FollowRadius { get; private set; }				// 追击范围
	public uint AttackRadius { get; private set; }				// 攻击范围
	public uint SeeRadius { get; private set; }				// 视野范围
	public uint GroupType { get; private set; }				// 群组类型(1为普通怪物群组还是2BOSS怪物群组)
	public uint[] MonID { get; private set; }				// 战斗位置的对象ID(0为法宝, 1-9为怪物)

	public XCfgMonsterGroup()
	{
		MonID = new uint[10];
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
		Color = tf.Get<uint>(_KEY_Color);
		MoveType = tf.Get<byte>(_KEY_MoveType);
		MoveSpeed = tf.Get<float>(_KEY_MoveSpeed);
		RunSpeed = tf.Get<float>(_KEY_RunSpeed);
		TurnSpeed = tf.Get<uint>(_KEY_TurnSpeed);
		Height = tf.Get<uint>(_KEY_Height);
		FollowRadius = tf.Get<uint>(_KEY_FollowRadius);
		AttackRadius = tf.Get<uint>(_KEY_AttackRadius);
		SeeRadius = tf.Get<uint>(_KEY_SeeRadius);
		GroupType = tf.Get<uint>(_KEY_GroupType);
		MonID[0] = tf.Get<uint>(_KEY_MonID_10_0);
		MonID[1] = tf.Get<uint>(_KEY_MonID_10_1);
		MonID[2] = tf.Get<uint>(_KEY_MonID_10_2);
		MonID[3] = tf.Get<uint>(_KEY_MonID_10_3);
		MonID[4] = tf.Get<uint>(_KEY_MonID_10_4);
		MonID[5] = tf.Get<uint>(_KEY_MonID_10_5);
		MonID[6] = tf.Get<uint>(_KEY_MonID_10_6);
		MonID[7] = tf.Get<uint>(_KEY_MonID_10_7);
		MonID[8] = tf.Get<uint>(_KEY_MonID_10_8);
		MonID[9] = tf.Get<uint>(_KEY_MonID_10_9);
		return true;
	}
}

