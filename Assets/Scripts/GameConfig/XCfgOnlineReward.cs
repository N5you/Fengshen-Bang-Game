
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgOnlineRewardMgr : CCfg1KeyMgrTemplate<XCfgOnlineRewardMgr, uint, XCfgOnlineReward> { };

partial class XCfgOnlineReward : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_ID = "ID";
	public static readonly string _KEY_GetTime = "GetTime";
	public static readonly string _KEY_RewardItemID = "RewardItemID";

	public uint ID { get; private set; }				// 领取ID
	public uint GetTime { get; private set; }				// 领取时间Sec
	public uint RewardItemID { get; private set; }				// 获得奖励物品ID

	public XCfgOnlineReward()
	{
	}

	public uint GetKey1() { return ID; }

	public bool ReadItem(TabFile tf)
	{
		ID = tf.Get<uint>(_KEY_ID);
		GetTime = tf.Get<uint>(_KEY_GetTime);
		RewardItemID = tf.Get<uint>(_KEY_RewardItemID);
		return true;
	}
}

