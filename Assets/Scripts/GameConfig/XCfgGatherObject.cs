
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgGatherObjectMgr : CCfg1KeyMgrTemplate<XCfgGatherObjectMgr, int, XCfgGatherObject> { };

partial class XCfgGatherObject : ITabItemWith1Key<int>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_ModelId = "ModelId";
	public static readonly string _KEY_NeedDistance = "NeedDistance";
	public static readonly string _KEY_NeedCareer = "NeedCareer";
	public static readonly string _KEY_NeedExp = "NeedExp";
	public static readonly string _KEY_CostTime = "CostTime";
	public static readonly string _KEY_CostStrength = "CostStrength";
	public static readonly string _KEY_ItemId_3_0 = "ItemId_3_0";
	public static readonly string _KEY_ItemId_3_1 = "ItemId_3_1";
	public static readonly string _KEY_ItemId_3_2 = "ItemId_3_2";

	public int ID { get; private set; }				// 采集物ID
	public string Name { get; private set; }				// 采集物名称
	public uint ModelId { get; private set; }				// 模型ID
	public float NeedDistance { get; private set; }				// 采集最大范围（米）
	public byte NeedCareer { get; private set; }				// 需求专业
	public ushort NeedExp { get; private set; }				// 需要熟练度值
	public uint CostTime { get; private set; }				// 采集消耗时长(单位秒)
	public uint CostStrength { get; private set; }				// 消耗的体力值
	public uint[] ItemId { get; private set; }				// 1物品ID

	public XCfgGatherObject()
	{
		ItemId = new uint[3];
	}

	public int GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<int>(_KEY_ID);
		Name = tf.Get<string>(_KEY_Name);
		ModelId = tf.Get<uint>(_KEY_ModelId);
		NeedDistance = tf.Get<float>(_KEY_NeedDistance);
		NeedCareer = tf.Get<byte>(_KEY_NeedCareer);
		NeedExp = tf.Get<ushort>(_KEY_NeedExp);
		CostTime = tf.Get<uint>(_KEY_CostTime);
		CostStrength = tf.Get<uint>(_KEY_CostStrength);
		ItemId[0] = tf.Get<uint>(_KEY_ItemId_3_0);
		ItemId[1] = tf.Get<uint>(_KEY_ItemId_3_1);
		ItemId[2] = tf.Get<uint>(_KEY_ItemId_3_2);
		return true;
	}
}

