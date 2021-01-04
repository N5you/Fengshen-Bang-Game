
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgUIRelationShipMgr : CCfg1KeyMgrTemplate<XCfgUIRelationShipMgr, ushort, XCfgUIRelationShip> { };

partial class XCfgUIRelationShip : ITabItemWith1Key<ushort>
{
	public static readonly string _KEY_UIPanelID = "UIPanelID";
	public static readonly string _KEY_SharePanelID = "SharePanelID";
	public static readonly string _KEY_OpenPanelID = "OpenPanelID";
	public static readonly string _KEY_SubPanelID = "SubPanelID";
	public static readonly string _KEY_SortByDESC = "SortByDESC";

	public ushort UIPanelID { get; private set; }				// UI面板ID
	public string SharePanelID { get; private set; }				// 可以共同存在的UI面板ID
	public string OpenPanelID { get; private set; }				// 打个面板的同时打开另一个面板，如果另一个面板处于未打开状态
	public string SubPanelID { get; private set; }				// 子UI面板ID
	public ushort SortByDESC { get; private set; }				// 排序降序

	public XCfgUIRelationShip()
	{
	}

	public ushort GetKey1() { return UIPanelID; }

	public bool ReadItem(TabFile tf)
	{
		UIPanelID = tf.Get<ushort>(_KEY_UIPanelID);
		SharePanelID = tf.Get<string>(_KEY_SharePanelID);
		OpenPanelID = tf.Get<string>(_KEY_OpenPanelID);
		SubPanelID = tf.Get<string>(_KEY_SubPanelID);
		SortByDESC = tf.Get<ushort>(_KEY_SortByDESC);
		return true;
	}
}

