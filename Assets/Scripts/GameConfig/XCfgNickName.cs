
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgNickNameMgr : CCfg1KeyMgrTemplate<XCfgNickNameMgr, uint, XCfgNickName> { };

partial class XCfgNickName : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_nID = "nID";
	public static readonly string _KEY_NickName = "NickName";
	public static readonly string _KEY_ColorID = "ColorID";

	public uint nID { get; private set; }				// 称号ID
	public string NickName { get; private set; }				// 领取时间Sec
	public int ColorID { get; private set; }				// 成色ID

	public XCfgNickName()
	{
	}

	public uint GetKey1() { return nID; }

	public bool ReadItem(TabFile tf)
	{
		nID = tf.Get<uint>(_KEY_nID);
		NickName = tf.Get<string>(_KEY_NickName);
		ColorID = tf.Get<int>(_KEY_ColorID);
		return true;
	}
}

