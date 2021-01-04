
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgConditionMgr : CCfg1KeyMgrTemplate<XCfgConditionMgr, int, XCfgCondition> { };

partial class XCfgCondition : ITabItemWith1Key<int>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_Name = "Name";

	public int ID { get; private set; }				// 条件ID
	public string Name { get; private set; }				// 条件名称

	public XCfgCondition()
	{
	}

	public int GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<int>(_KEY_ID);
		Name = tf.Get<string>(_KEY_Name);
		return true;
	}
}

