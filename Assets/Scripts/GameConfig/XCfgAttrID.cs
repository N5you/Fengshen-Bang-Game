
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgAttrIDMgr : CCfg1KeyMgrTemplate<XCfgAttrIDMgr, uint, XCfgAttrID> { };

partial class XCfgAttrID : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_AttrID = "AttrID";
	public static readonly string _KEY_EffectDes = "EffectDes";
	public static readonly string _KEY_AttrDes = "AttrDes";
	public static readonly string _KEY_ChangeType = "ChangeType";
	public static readonly string _KEY_AttrSortID = "AttrSortID";
	public static readonly string _KEY_AttrDefineID = "AttrDefineID";

	public uint AttrID { get; private set; }				// 属性ID
	public string EffectDes { get; private set; }				// 效果说明
	public string AttrDes { get; private set; }				// 属性描述
	public byte ChangeType { get; private set; }				// 加成类型(0-值加成,1-基础率加成)
	public uint AttrSortID { get; private set; }				// 属性排序ID  在物品上的显示顺序
	public uint AttrDefineID { get; private set; }				// 与程序定义的枚举对应值

	public XCfgAttrID()
	{
	}

	public uint GetKey1() { return AttrID; }

	public bool ReadItem(TabFile tf)
	{
		AttrID = tf.Get<uint>(_KEY_AttrID);
		EffectDes = tf.Get<string>(_KEY_EffectDes);
		AttrDes = tf.Get<string>(_KEY_AttrDes);
		ChangeType = tf.Get<byte>(_KEY_ChangeType);
		AttrSortID = tf.Get<uint>(_KEY_AttrSortID);
		AttrDefineID = tf.Get<uint>(_KEY_AttrDefineID);
		return true;
	}
}

