
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgBattleFailGuideMgr : CCfg1KeyMgrTemplate<XCfgBattleFailGuideMgr, int, XCfgBattleFailGuide> { };

partial class XCfgBattleFailGuide : ITabItemWith1Key<int>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_Unlock = "Unlock";
	public static readonly string _KEY_LinkUI = "LinkUI";

	public int Index { get; private set; }				// //
	public string Name { get; private set; }				// //
	public int Unlock { get; private set; }				// //
	public int LinkUI { get; private set; }				// //

	public XCfgBattleFailGuide()
	{
	}

	public int GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<int>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		Unlock = tf.Get<int>(_KEY_Unlock);
		LinkUI = tf.Get<int>(_KEY_LinkUI);
		return true;
	}
}

