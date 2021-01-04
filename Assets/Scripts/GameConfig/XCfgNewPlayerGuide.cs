
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgNewPlayerGuideMgr : CCfg2KeyMgrTemplate<XCfgNewPlayerGuideMgr, int, int, XCfgNewPlayerGuide> { };

partial class XCfgNewPlayerGuide : ITabItemWith2Key<int, int>
{
	public static readonly string _KEY_ClassId = "ClassId";
	public static readonly string _KEY_StepId = "StepId";
	public static readonly string _KEY_UIResourcesID = "UIResourcesID";
	public static readonly string _KEY_Description = "Description";

	public int ClassId { get; private set; }				// 类型id
	public int StepId { get; private set; }				// 步骤id
	public uint UIResourcesID { get; private set; }				// 资源ID
	public string Description { get; private set; }				// 引导描述

	public XCfgNewPlayerGuide()
	{
	}

	public int GetKey1() { return ClassId; }

	public int GetKey2() { return StepId; }

	public bool ReadItem(TabFile tf)
	{
		ClassId = tf.Get<int>(_KEY_ClassId);
		StepId = tf.Get<int>(_KEY_StepId);
		UIResourcesID = tf.Get<uint>(_KEY_UIResourcesID);
		Description = tf.Get<string>(_KEY_Description);
		return true;
	}
}

