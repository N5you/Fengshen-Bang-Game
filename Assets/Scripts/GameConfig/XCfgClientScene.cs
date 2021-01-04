
//============================================
//--4>:
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================

using System;
using UnityEngine;

partial class XCfgClientSceneMgr : CCfg1KeyMgrTemplate<XCfgClientSceneMgr, uint, XCfgClientScene> { };

partial class XCfgClientScene : ITabItemWith1Key<uint>
{
	public static readonly string _KEY_Index = "Index";
	public static readonly string _KEY_Name = "Name";
	public static readonly string _KEY_PreIndex = "PreIndex";
	public static readonly string _KEY_PostIndex = "PostIndex";
	public static readonly string _KEY_SceneID = "SceneID";
	public static readonly string _KEY_RequireLevel = "RequireLevel";
	public static readonly string _KEY_RequireMissionID = "RequireMissionID";
	public static readonly string _KEY_BossAtlasID = "BossAtlasID";
	public static readonly string _KEY_BossSpriteName = "BossSpriteName";
	public static readonly string _KEY_AtlasID = "AtlasID";
	public static readonly string _KEY_SpriteName = "SpriteName";
	public static readonly string _KEY_LuckTC_3_0 = "LuckTC_3_0";
	public static readonly string _KEY_LuckTC_3_1 = "LuckTC_3_1";
	public static readonly string _KEY_LuckTC_3_2 = "LuckTC_3_2";
	public static readonly string _KEY_LevelNeed_3_0 = "LevelNeed_3_0";
	public static readonly string _KEY_LevelNeed_3_1 = "LevelNeed_3_1";
	public static readonly string _KEY_LevelNeed_3_2 = "LevelNeed_3_2";
	public static readonly string _KEY_EnterPos = "EnterPos";
	public static readonly string _KEY_EnterDir = "EnterDir";
	public static readonly string _KEY_RoundNum = "RoundNum";
	public static readonly string _KEY_GroupID_6_0 = "GroupID_6_0";
	public static readonly string _KEY_GroupPos_6_0 = "GroupPos_6_0";
	public static readonly string _KEY_GroupDir_6_0 = "GroupDir_6_0";
	public static readonly string _KEY_GroupScene_6_0 = "GroupScene_6_0";
	public static readonly string _KEY_GroupID_6_1 = "GroupID_6_1";
	public static readonly string _KEY_GroupPos_6_1 = "GroupPos_6_1";
	public static readonly string _KEY_GroupDir_6_1 = "GroupDir_6_1";
	public static readonly string _KEY_GroupScene_6_1 = "GroupScene_6_1";
	public static readonly string _KEY_GroupID_6_2 = "GroupID_6_2";
	public static readonly string _KEY_GroupPos_6_2 = "GroupPos_6_2";
	public static readonly string _KEY_GroupDir_6_2 = "GroupDir_6_2";
	public static readonly string _KEY_GroupScene_6_2 = "GroupScene_6_2";
	public static readonly string _KEY_GroupID_6_3 = "GroupID_6_3";
	public static readonly string _KEY_GroupPos_6_3 = "GroupPos_6_3";
	public static readonly string _KEY_GroupDir_6_3 = "GroupDir_6_3";
	public static readonly string _KEY_GroupScene_6_3 = "GroupScene_6_3";
	public static readonly string _KEY_GroupID_6_4 = "GroupID_6_4";
	public static readonly string _KEY_GroupPos_6_4 = "GroupPos_6_4";
	public static readonly string _KEY_GroupDir_6_4 = "GroupDir_6_4";
	public static readonly string _KEY_GroupScene_6_4 = "GroupScene_6_4";
	public static readonly string _KEY_GroupID_6_5 = "GroupID_6_5";
	public static readonly string _KEY_GroupPos_6_5 = "GroupPos_6_5";
	public static readonly string _KEY_GroupDir_6_5 = "GroupDir_6_5";
	public static readonly string _KEY_GroupScene_6_5 = "GroupScene_6_5";
	public static readonly string _KEY_RebornPos_3_0 = "RebornPos_3_0";
	public static readonly string _KEY_RebornDir_3_0 = "RebornDir_3_0";
	public static readonly string _KEY_RebornPos_3_1 = "RebornPos_3_1";
	public static readonly string _KEY_RebornDir_3_1 = "RebornDir_3_1";
	public static readonly string _KEY_RebornPos_3_2 = "RebornPos_3_2";
	public static readonly string _KEY_RebornDir_3_2 = "RebornDir_3_2";
	public static readonly string _KEY_RoadFlagID_3_0 = "RoadFlagID_3_0";
	public static readonly string _KEY_RoadFlagPos_3_0 = "RoadFlagPos_3_0";
	public static readonly string _KEY_RoadFlagDir_3_0 = "RoadFlagDir_3_0";
	public static readonly string _KEY_RoadFlagID_3_1 = "RoadFlagID_3_1";
	public static readonly string _KEY_RoadFlagPos_3_1 = "RoadFlagPos_3_1";
	public static readonly string _KEY_RoadFlagDir_3_1 = "RoadFlagDir_3_1";
	public static readonly string _KEY_RoadFlagID_3_2 = "RoadFlagID_3_2";
	public static readonly string _KEY_RoadFlagPos_3_2 = "RoadFlagPos_3_2";
	public static readonly string _KEY_RoadFlagDir_3_2 = "RoadFlagDir_3_2";
	public static readonly string _KEY_CutSceneStart = "CutSceneStart";
	public static readonly string _KEY_CutScenePre_3_0 = "CutScenePre_3_0";
	public static readonly string _KEY_CutSceneSuf_3_0 = "CutSceneSuf_3_0";
	public static readonly string _KEY_CutScenePre_3_1 = "CutScenePre_3_1";
	public static readonly string _KEY_CutSceneSuf_3_1 = "CutSceneSuf_3_1";
	public static readonly string _KEY_CutScenePre_3_2 = "CutScenePre_3_2";
	public static readonly string _KEY_CutSceneSuf_3_2 = "CutSceneSuf_3_2";
	public static readonly string _KEY_EnterCameraPos = "EnterCameraPos";
	public static readonly string _KEY_EnterCameraRot = "EnterCameraRot";

	public uint Index { get; private set; }				// 关卡ID(1-256)
	public string Name { get; private set; }				// 关卡名
	public uint PreIndex { get; private set; }				// 前置关卡ID
	public uint PostIndex { get; private set; }				// 后置关卡ID
	public uint SceneID { get; private set; }				// 场景ID
	public uint RequireLevel { get; private set; }				// 所需要的等级
	public uint RequireMissionID { get; private set; }				// 进入关卡所需要接受的任务ID
	public uint BossAtlasID { get; private set; }				// BOSS的图集ID
	public string BossSpriteName { get; private set; }				// BOSS的图片名称
	public uint AtlasID { get; private set; }				// 关卡图集ID
	public string SpriteName { get; private set; }				// 关卡图片名称（字符串类型）
	public uint[] LuckTC { get; private set; }				// 关卡抽奖TC1
	public uint[] LevelNeed { get; private set; }				// 普通难度场景等级
	public Vector3 EnterPos { get; private set; }				// 关卡进入点坐标(X;Y;Z;)
	public float EnterDir { get; private set; }				// 进入点朝向
	public byte RoundNum { get; private set; }				// 回合数(注意, 此值必须>=后面配置的有效群组数, 别且不可以超出总群组数)
	public uint[] GroupID { get; private set; }				// 怪物群组1的ID(注意有效群组信息必须是连续的)
	public Vector3[] GroupPos { get; private set; }				// 怪物群组1的坐标(X;Y;Z;)
	public float[] GroupDir { get; private set; }				// 怪物群组1的朝向
	public uint[] GroupScene { get; private set; }				// 怪物群组1的战斗场景ID
	public Vector3[] RebornPos { get; private set; }				// 怪物1战斗失败后重生点
	public float[] RebornDir { get; private set; }				// 朝向
	public uint[] RoadFlagID { get; private set; }				// 路标1模型ID
	public Vector3[] RoadFlagPos { get; private set; }				// 路标1坐标
	public float[] RoadFlagDir { get; private set; }				// 路标1朝向
	public uint CutSceneStart { get; private set; }				// 进入副本开始的场景动画 0表示为没有
	public uint[] CutScenePre { get; private set; }				// 第一个怪物之前的动画 0 为没有动画
	public uint[] CutSceneSuf { get; private set; }				// 第一个怪物之后的动画 0 为没有动画
	public Vector3 EnterCameraPos { get; private set; }				// 进入副本摄像头位置
	public Vector3 EnterCameraRot { get; private set; }				// 进入副本摄像头朝向

	public XCfgClientScene()
	{
		LuckTC = new uint[3];
		LevelNeed = new uint[3];
		GroupID = new uint[6];
		GroupPos = new Vector3[6];
		GroupDir = new float[6];
		GroupScene = new uint[6];
		RebornPos = new Vector3[3];
		RebornDir = new float[3];
		RoadFlagID = new uint[3];
		RoadFlagPos = new Vector3[3];
		RoadFlagDir = new float[3];
		CutScenePre = new uint[3];
		CutSceneSuf = new uint[3];
	}

	public uint GetKey1() { return Index; }

	public bool ReadItem(TabFile tf)
	{
		Index = tf.Get<uint>(_KEY_Index);
		Name = tf.Get<string>(_KEY_Name);
		PreIndex = tf.Get<uint>(_KEY_PreIndex);
		PostIndex = tf.Get<uint>(_KEY_PostIndex);
		SceneID = tf.Get<uint>(_KEY_SceneID);
		RequireLevel = tf.Get<uint>(_KEY_RequireLevel);
		RequireMissionID = tf.Get<uint>(_KEY_RequireMissionID);
		BossAtlasID = tf.Get<uint>(_KEY_BossAtlasID);
		BossSpriteName = tf.Get<string>(_KEY_BossSpriteName);
		AtlasID = tf.Get<uint>(_KEY_AtlasID);
		SpriteName = tf.Get<string>(_KEY_SpriteName);
		LuckTC[0] = tf.Get<uint>(_KEY_LuckTC_3_0);
		LuckTC[1] = tf.Get<uint>(_KEY_LuckTC_3_1);
		LuckTC[2] = tf.Get<uint>(_KEY_LuckTC_3_2);
		LevelNeed[0] = tf.Get<uint>(_KEY_LevelNeed_3_0);
		LevelNeed[1] = tf.Get<uint>(_KEY_LevelNeed_3_1);
		LevelNeed[2] = tf.Get<uint>(_KEY_LevelNeed_3_2);
		EnterPos = XUtil.String2Vector3(tf.Get<string>(_KEY_EnterPos));
		EnterDir = tf.Get<float>(_KEY_EnterDir);
		RoundNum = tf.Get<byte>(_KEY_RoundNum);
		GroupID[0] = tf.Get<uint>(_KEY_GroupID_6_0);
		GroupPos[0] = XUtil.String2Vector3(tf.Get<string>(_KEY_GroupPos_6_0));
		GroupDir[0] = tf.Get<float>(_KEY_GroupDir_6_0);
		GroupScene[0] = tf.Get<uint>(_KEY_GroupScene_6_0);
		GroupID[1] = tf.Get<uint>(_KEY_GroupID_6_1);
		GroupPos[1] = XUtil.String2Vector3(tf.Get<string>(_KEY_GroupPos_6_1));
		GroupDir[1] = tf.Get<float>(_KEY_GroupDir_6_1);
		GroupScene[1] = tf.Get<uint>(_KEY_GroupScene_6_1);
		GroupID[2] = tf.Get<uint>(_KEY_GroupID_6_2);
		GroupPos[2] = XUtil.String2Vector3(tf.Get<string>(_KEY_GroupPos_6_2));
		GroupDir[2] = tf.Get<float>(_KEY_GroupDir_6_2);
		GroupScene[2] = tf.Get<uint>(_KEY_GroupScene_6_2);
		GroupID[3] = tf.Get<uint>(_KEY_GroupID_6_3);
		GroupPos[3] = XUtil.String2Vector3(tf.Get<string>(_KEY_GroupPos_6_3));
		GroupDir[3] = tf.Get<float>(_KEY_GroupDir_6_3);
		GroupScene[3] = tf.Get<uint>(_KEY_GroupScene_6_3);
		GroupID[4] = tf.Get<uint>(_KEY_GroupID_6_4);
		GroupPos[4] = XUtil.String2Vector3(tf.Get<string>(_KEY_GroupPos_6_4));
		GroupDir[4] = tf.Get<float>(_KEY_GroupDir_6_4);
		GroupScene[4] = tf.Get<uint>(_KEY_GroupScene_6_4);
		GroupID[5] = tf.Get<uint>(_KEY_GroupID_6_5);
		GroupPos[5] = XUtil.String2Vector3(tf.Get<string>(_KEY_GroupPos_6_5));
		GroupDir[5] = tf.Get<float>(_KEY_GroupDir_6_5);
		GroupScene[5] = tf.Get<uint>(_KEY_GroupScene_6_5);
		RebornPos[0] = XUtil.String2Vector3(tf.Get<string>(_KEY_RebornPos_3_0));
		RebornDir[0] = tf.Get<float>(_KEY_RebornDir_3_0);
		RebornPos[1] = XUtil.String2Vector3(tf.Get<string>(_KEY_RebornPos_3_1));
		RebornDir[1] = tf.Get<float>(_KEY_RebornDir_3_1);
		RebornPos[2] = XUtil.String2Vector3(tf.Get<string>(_KEY_RebornPos_3_2));
		RebornDir[2] = tf.Get<float>(_KEY_RebornDir_3_2);
		RoadFlagID[0] = tf.Get<uint>(_KEY_RoadFlagID_3_0);
		RoadFlagPos[0] = XUtil.String2Vector3(tf.Get<string>(_KEY_RoadFlagPos_3_0));
		RoadFlagDir[0] = tf.Get<float>(_KEY_RoadFlagDir_3_0);
		RoadFlagID[1] = tf.Get<uint>(_KEY_RoadFlagID_3_1);
		RoadFlagPos[1] = XUtil.String2Vector3(tf.Get<string>(_KEY_RoadFlagPos_3_1));
		RoadFlagDir[1] = tf.Get<float>(_KEY_RoadFlagDir_3_1);
		RoadFlagID[2] = tf.Get<uint>(_KEY_RoadFlagID_3_2);
		RoadFlagPos[2] = XUtil.String2Vector3(tf.Get<string>(_KEY_RoadFlagPos_3_2));
		RoadFlagDir[2] = tf.Get<float>(_KEY_RoadFlagDir_3_2);
		CutSceneStart = tf.Get<uint>(_KEY_CutSceneStart);
		CutScenePre[0] = tf.Get<uint>(_KEY_CutScenePre_3_0);
		CutSceneSuf[0] = tf.Get<uint>(_KEY_CutSceneSuf_3_0);
		CutScenePre[1] = tf.Get<uint>(_KEY_CutScenePre_3_1);
		CutSceneSuf[1] = tf.Get<uint>(_KEY_CutSceneSuf_3_1);
		CutScenePre[2] = tf.Get<uint>(_KEY_CutScenePre_3_2);
		CutSceneSuf[2] = tf.Get<uint>(_KEY_CutSceneSuf_3_2);
		EnterCameraPos = XUtil.String2Vector3(tf.Get<string>(_KEY_EnterCameraPos));
		EnterCameraRot = XUtil.String2Vector3(tf.Get<string>(_KEY_EnterCameraRot));
		return true;
	}
}

