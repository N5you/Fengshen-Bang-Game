
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgTrainMgr : CCfg1KeyMgrTemplate<XCfgTrainMgr, uint, XCfgTrain> { };

partial class XCfgTrain : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Level = "Level";
	public static readonly string _KEY_CostGameMoney = "CostGameMoney";
	public static readonly string _KEY_TrainMin = "TrainMin";
	public static readonly string _KEY_TrainMax = "TrainMax";
	public static readonly string _KEY_CostRealMoney = "CostRealMoney";
	public static readonly string _KEY_RTrainMin = "RTrainMin";
	public static readonly string _KEY_RTrainMax = "RTrainMax";

	public uint Level { get; private set; }				// 等级
	public uint CostGameMoney { get; private set; }				// 普通培养花费铜币
	public float TrainMin { get; private set; }				// 培养最小值
	public float TrainMax { get; private set; }				// 培养最大值
	public uint CostRealMoney { get; private set; }				// 高级培养花费元宝
	public float RTrainMin { get; private set; }				// 培养最小值
	public float RTrainMax { get; private set; }				// 培养最大值

	public XCfgTrain()
	{
	}

	public uint GetKey1() { return Level; }

	public bool ReadItem(TabFile tf)
	{
		Level = tf.Get<uint>(_KEY_Level);
		CostGameMoney = tf.Get<uint>(_KEY_CostGameMoney);
		TrainMin = tf.Get<float>(_KEY_TrainMin);
		TrainMax = tf.Get<float>(_KEY_TrainMax);
		CostRealMoney = tf.Get<uint>(_KEY_CostRealMoney);
		RTrainMin = tf.Get<float>(_KEY_RTrainMin);
		RTrainMax = tf.Get<float>(_KEY_RTrainMax);
		return true;
	}
}

