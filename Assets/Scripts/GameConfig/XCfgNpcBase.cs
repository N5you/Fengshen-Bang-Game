
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgNpcBaseMgr : CCfg1KeyMgrTemplate<XCfgNpcBaseMgr, uint, XCfgNpcBase> { };

partial class XCfgNpcBase : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_Title = "Title";
	public static readonly string _KEY_ModelId = "ModelId";
	public static readonly string _KEY_Sex = "Sex";
	public static readonly string _KEY_Race = "Race";
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_MaxHp = "MaxHp";
	public static readonly string _KEY_Radius = "Radius";
	public static readonly string _KEY_Dist = "Dist";
	public static readonly string _KEY_Zoom = "Zoom";
	public static readonly string _KEY_PicId = "PicId";
	public static readonly string _KEY_Function = "Function";
	public static readonly string _KEY_Talk_4_0 = "Talk_4_0";
	public static readonly string _KEY_Talk_4_1 = "Talk_4_1";
	public static readonly string _KEY_Talk_4_2 = "Talk_4_2";
	public static readonly string _KEY_Talk_4_3 = "Talk_4_3";
	public static readonly string _KEY_Color = "Color";
	public static readonly string _KEY_MoveType = "MoveType";
	public static readonly string _KEY_MoveSpeed = "MoveSpeed";
	public static readonly string _KEY_Height = "Height";
	public static readonly string _KEY_CamPos = "CamPos";
	public static readonly string _KEY_headIcon = "headIcon";

	public uint Index { get; private set; }				// 类型ID(唯一标识符)
	public string Name { get; private set; }				// 名字
	public string Title { get; private set; }				// 称号
	public uint ModelId { get; private set; }				// 模型ID
	public byte Sex { get; private set; }				// 性别(1女,2男)
	public byte Race { get; private set; }				// 种族
	public uint Level { get; private set; }				// 默认等级
	public int MaxHp { get; private set; }				// 生命上限值
	public float Radius { get; private set; }				// 半径
	public float Dist { get; private set; }				// 距离
	public uint Zoom { get; private set; }				// 缩放比例
	public byte PicId { get; private set; }				// 贴图
	public uint Function { get; private set; }				// 功能(占位标记,//仓库=1,拍卖=2,任务=4，商店=8，外部链接=16)
	public string[] Talk { get; private set; }				// 随机对话
	public ushort Color { get; private set; }				// 染色
	public byte MoveType { get; private set; }				// 移动类型
	public ushort MoveSpeed { get; private set; }				// 默认移动速度
	public byte Height { get; private set; }				// 高度
	public Vector3 CamPos { get; private set; }				// 观察起点
	public string headIcon { get; private set; }				// //NPC头顶图标提示，图集固定，这里只写图片名

	public XCfgNpcBase()
	{
		Talk = new string[4];
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		Title = tf.Get<string>(_KEY_Title);
		ModelId = tf.Get<uint>(_KEY_ModelId);
		Sex = tf.Get<byte>(_KEY_Sex);
		Race = tf.Get<byte>(_KEY_Race);
		Level = tf.Get<uint>(_KEY_Level);
		MaxHp = tf.Get<int>(_KEY_MaxHp);
		Radius = tf.Get<float>(_KEY_Radius);
		Dist = tf.Get<float>(_KEY_Dist);
		Zoom = tf.Get<uint>(_KEY_Zoom);
		PicId = tf.Get<byte>(_KEY_PicId);
		Function = tf.Get<uint>(_KEY_Function);
		Talk[0] = tf.Get<string>(_KEY_Talk_4_0);
		Talk[1] = tf.Get<string>(_KEY_Talk_4_1);
		Talk[2] = tf.Get<string>(_KEY_Talk_4_2);
		Talk[3] = tf.Get<string>(_KEY_Talk_4_3);
		Color = tf.Get<ushort>(_KEY_Color);
		MoveType = tf.Get<byte>(_KEY_MoveType);
		MoveSpeed = tf.Get<ushort>(_KEY_MoveSpeed);
		Height = tf.Get<byte>(_KEY_Height);
		CamPos = XUtil.String2Vector3(tf.Get<string>(_KEY_CamPos));
		headIcon = tf.Get<string>(_KEY_headIcon);
		return true;
	}
}

