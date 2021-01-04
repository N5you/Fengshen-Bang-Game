
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgWorldMapMgr : CCfg1KeyMgrTemplate<XCfgWorldMapMgr, uint, XCfgWorldMap> { };

partial class XCfgWorldMap : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_SceneId = "SceneId";
	public static readonly string _KEY_CloseSprite = "CloseSprite";
	public static readonly string _KEY_OpenSpriteNormal = "OpenSpriteNormal";
	public static readonly string _KEY_OpenSpriteHover = "OpenSpriteHover";
	public static readonly string _KEY_PosId = "PosId";
	public static readonly string _KEY_RequireQuestID = "RequireQuestID";

	public uint SceneId { get; private set; }				// 跳转场景ID
	public string CloseSprite { get; private set; }				// 未开启状态图标
	public string OpenSpriteNormal { get; private set; }				// 开启状态图标
	public string OpenSpriteHover { get; private set; }				// 开启状态鼠标移上
	public int PosId { get; private set; }				// 世界地图上的位置ID
	public uint RequireQuestID { get; private set; }				// 场景开启需要完成的任务ID

	public XCfgWorldMap()
	{
	}

	public uint GetKey1() { return SceneId; }

	public bool ReadItem(TabFile tf)
	{
		SceneId = tf.Get<uint>(_KEY_SceneId);
		CloseSprite = tf.Get<string>(_KEY_CloseSprite);
		OpenSpriteNormal = tf.Get<string>(_KEY_OpenSpriteNormal);
		OpenSpriteHover = tf.Get<string>(_KEY_OpenSpriteHover);
		PosId = tf.Get<int>(_KEY_PosId);
		RequireQuestID = tf.Get<uint>(_KEY_RequireQuestID);
		return true;
	}
}

