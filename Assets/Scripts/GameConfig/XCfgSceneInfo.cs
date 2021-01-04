
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgSceneInfoMgr : CCfg1KeyMgrTemplate<XCfgSceneInfoMgr, uint, XCfgSceneInfo> { };

partial class XCfgSceneInfo : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_SceneID = "SceneID";
	public static readonly string _KEY_Name = "Name";

	public uint SceneID { get; private set; }				// 场景ID
	public string Name { get; private set; }				// 场景名字

	public XCfgSceneInfo()
	{
	}

	public uint GetKey1() { return SceneID; }

	public bool ReadItem(TabFile tf)
	{
		SceneID = tf.Get<uint>(_KEY_SceneID);
		Name = tf.Get<string>(_KEY_Name);
		return true;
	}
}

