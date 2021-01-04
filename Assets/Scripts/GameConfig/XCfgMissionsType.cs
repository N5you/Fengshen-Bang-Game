
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgMissionsTypeMgr : CCfg1KeyMgrTemplate<XCfgMissionsTypeMgr, uint, XCfgMissionsType> { };

partial class XCfgMissionsType : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_typeID = "typeID";
	public static readonly string _KEY_typeName = "typeName";

	public uint typeID { get; private set; }				// //类型ID
	public string typeName { get; private set; }				// //类型名称

	public XCfgMissionsType()
	{
	}

	public uint GetKey1() { return typeID; }

	public bool ReadItem(TabFile tf)
	{
		typeID = tf.Get<uint>(_KEY_typeID);
		typeName = tf.Get<string>(_KEY_typeName);
		return true;
	}
}

