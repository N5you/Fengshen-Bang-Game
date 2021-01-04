
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMountMgr : CCfg1KeyMgrTemplate<XCfgMountMgr, ushort, XCfgMount> { };

partial class XCfgMount : ITabItemWith1Key<ushort>
{
	public static readonly string _KEY_MountIndex = "MountIndex";
	public static readonly string _KEY_MountName = "MountName";
	public static readonly string _KEY_MountColorLevel = "MountColorLevel";
	public static readonly string _KEY_MountAtlas = "MountAtlas";
	public static readonly string _KEY_MountIcon = "MountIcon";
	public static readonly string _KEY_ModelId = "ModelId";
	public static readonly string _KEY_MountType = "MountType";
	public static readonly string _KEY_MountTips = "MountTips";

	public ushort MountIndex { get; private set; }				// 坐骑ID
	public string MountName { get; private set; }				// 坐骑名字
	public uint MountColorLevel { get; private set; }				// 坐骑的成色值，影响名字的字色
	public int MountAtlas { get; private set; }				// 坐骑ICON的图集
	public string MountIcon { get; private set; }				// 坐骑图片ID
	public uint ModelId { get; private set; }				// 模型ID
	public uint MountType { get; private set; }				// 坐骑的骑乘类型，1站立骑乘，2坐式骑乘，3翅膀
	public string MountTips { get; private set; }				// 坐骑的tips说明文字

	public XCfgMount()
	{
	}

	public ushort GetKey1() { return MountIndex; }

	public bool ReadItem(TabFile tf)
	{
		MountIndex = tf.Get<ushort>(_KEY_MountIndex);
		MountName = tf.Get<string>(_KEY_MountName);
		MountColorLevel = tf.Get<uint>(_KEY_MountColorLevel);
		MountAtlas = tf.Get<int>(_KEY_MountAtlas);
		MountIcon = tf.Get<string>(_KEY_MountIcon);
		ModelId = tf.Get<uint>(_KEY_ModelId);
		MountType = tf.Get<uint>(_KEY_MountType);
		MountTips = tf.Get<string>(_KEY_MountTips);
		return true;
	}
}

