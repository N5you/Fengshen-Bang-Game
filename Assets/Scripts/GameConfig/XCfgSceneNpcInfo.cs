
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgSceneNpcInfoMgr : CCfg1GroupMgrTemplate<XCfgSceneNpcInfoMgr, uint, XCfgSceneNpcInfo> { };

partial class XCfgSceneNpcInfo : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_SceneID = "SceneID";
	public static readonly string _KEY_NpcID = "NpcID";
	public static readonly string _KEY_BornPos = "BornPos";

	public uint SceneID { get; private set; }				// 场景ID
	public uint NpcID { get; private set; }				// NPCID
	public Vector3 BornPos { get; private set; }				// 出生点坐标

	public XCfgSceneNpcInfo()
	{
	}

	public uint GetKey1() { return SceneID; }

	public bool ReadItem(TabFile tf)
	{
		SceneID = tf.Get<uint>(_KEY_SceneID);
		NpcID = tf.Get<uint>(_KEY_NpcID);
		BornPos = XUtil.String2Vector3(tf.Get<string>(_KEY_BornPos));
		return true;
	}
}

