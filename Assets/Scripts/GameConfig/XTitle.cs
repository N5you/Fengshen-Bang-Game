
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XTitleMgr : CCfg1KeyMgrTemplate<XTitleMgr, uint, XTitle> { };

partial class XTitle : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_TitleID = "TitleID";
	public static readonly string _KEY_TitleName = "TitleName";

	public uint TitleID { get; private set; }				// 称号ID
	public string TitleName { get; private set; }				// 称号名称

	public XTitle()
	{
	}

	public uint GetKey1() { return TitleID; }

	public bool ReadItem(TabFile tf)
	{
		TitleID = tf.Get<uint>(_KEY_TitleID);
		TitleName = tf.Get<string>(_KEY_TitleName);
		return true;
	}
}

