
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgBuyChallengeMgr : CCfg1KeyMgrTemplate<XCfgBuyChallengeMgr, uint, XCfgBuyChallenge> { };

partial class XCfgBuyChallenge : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Count = "Count";
	public static readonly string _KEY_GameMoney = "GameMoney";

	public uint Count { get; private set; }				// 次数
	public uint GameMoney { get; private set; }				// 游戏币

	public XCfgBuyChallenge()
	{
	}

	public uint GetKey1() { return Count; }

	public bool ReadItem(TabFile tf)
	{
		Count = tf.Get<uint>(_KEY_Count);
		GameMoney = tf.Get<uint>(_KEY_GameMoney);
		return true;
	}
}

