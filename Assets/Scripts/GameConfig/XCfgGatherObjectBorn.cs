
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgGatherObjectBornMgr : CCfg2KeyMgrTemplate<XCfgGatherObjectBornMgr, uint, int, XCfgGatherObjectBorn> { };

partial class XCfgGatherObjectBorn : ITabItemWith2Key<uint, int>
{
	public static readonly string _KEY_SceneId = "SceneId";
	public static readonly string _KEY_GatherObjectId = "GatherObjectId";
	public static readonly string _KEY_BornPos = "BornPos";
	public static readonly string _KEY_Direction = "Direction";

	public uint SceneId { get; private set; }				// 场景ID
	public int GatherObjectId { get; private set; }				// 采集物ID
	public Vector3 BornPos { get; private set; }				// 采集物出生点
	public float Direction { get; private set; }				// 朝向

	public XCfgGatherObjectBorn()
	{
	}

	public uint GetKey1() { return SceneId; }

	public int GetKey2() { return GatherObjectId; }

	public bool ReadItem(TabFile tf)
	{
		SceneId = tf.Get<uint>(_KEY_SceneId);
		GatherObjectId = tf.Get<int>(_KEY_GatherObjectId);
		BornPos = XUtil.String2Vector3(tf.Get<string>(_KEY_BornPos));
		Direction = tf.Get<float>(_KEY_Direction);
		return true;
	}
}

