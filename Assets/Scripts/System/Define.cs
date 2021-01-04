using System;
using UnityEngine;

[System.Serializable]
public class Define
{
    public static readonly int POLICY_SERVER_PORT = 44444;

	public static int			MAX_ACCOUNT_NAME_LEN			= 21;	// max account length, contain '\0'
	public static int			MAX_ACCOUNT_PASSWORD_LEN		= 21;	// max password length, contain '\0'
    public static int 			MAX_PLAYER_NAME_LEN = 20;
    public static int 			MIN_PLAYER_NAME_LEN = 2;
	
	public static float			CONFIG_RATE_BASE = 10000.0f;
	
    // 默认同屏最多显示其他玩家数量
    public static readonly uint DEFAULT_SHOW_PLAYER_NUM = 100;

    // 场景分块 cell 的大小
    public static readonly float CELL_SIZE = 1.0f;

    // 聊天信息最大长度
    public static readonly int MAX_CHAT_DATA_LEN = 200;

    // 和 NPC 对话的有效距离
    public static readonly float GOOD_NPC_CLICK_DISTANCE = 3.0f;
    // 离开 NPC 一定距离后自动隐藏对话框
    public static readonly float MAX_NPC_CLICK_DISTANCE = 4.0f;

    // 进入传送等可触发区域的距离
    public static readonly float REGION_ENTER_DISTANCE = 3.0f;
    // 离开传送等可触发区域的距离
    public static readonly float REGION_LEAVE_DISTANCE = 4.0f;

    // 单机副本战斗时, 玩家的朝向以及玩家和怪物的相对位置
    public static readonly Vector3 PLAYER_FIGHT_DIRECTION = new Vector3(0, 90, 0);
    public static readonly Vector3 PLAYER_FIGHT_POSITION = new Vector3(-14.0f, 0.0f, 0.0f);
	
	public static readonly uint MAX_CLASS_LEVEL = 7;
	
	// 死亡消失时间
	public static readonly float  DEAD_DESTROY_DELAY = 2.0f;
	
	 // 重力加速度
	public static readonly float  GRAVITATIONAL_ACCELERATION = -10f;
	
	// 摄像机相关
	public float MainCameraRotateSpeed = 150f;					// 摄像机转动速度
	public AnimationCurve MainCameraYCurve;						// 滚轮 -> 主摄像机运动轨迹
	public float MainCameraCurveSpeed = 30f;					// 滚轮 -> 主摄像机运动速度
	public float MainCameraScrollSpeed = 3f;					// 滚轮 -> 校准主摄像机滚轮速度
	
	public const string Money_Icon_Id  		= "12000540";
	public const string RealMoney_Icon_Id  = "12000541";
}

class GlobalU3dDefine
{
    // Tags
    public static readonly string Tag_Road = "Road";
    public static readonly string Tag_Block = "Block";
	
	// Layer
    public static readonly int Layer_Default = 0;
    public static readonly int Layer_IgnoreRaycast = 2;
    public static readonly int Layer_UI_2D = 8;
	public static readonly int Layer_GameObject	= 9;
	public static readonly int Layer_Decal = 10;
	public static readonly int Layer_ModelRTT = 12;
	public static readonly int Layer_BattleObject = 13;
	public static readonly int Layer_TerrainObject = 14;
	
	public static readonly string[] JobWeaponSlot = {"","Bone_Sword","Bone_Staff","Bone_Bow"};
	public static readonly string[] JobName = {"","魔君","青巫","风灵"};
	public static readonly string[] EquipPos = {"不可装备","武器","铠甲","护符","戒指","法宝","令牌","符印"};
	public static readonly string[] BindType = {"不绑定","解封后绑定","获取后绑定"};
	public static readonly string[] MajorName = {"","园艺","垂钓","勘采","烹饪","修道","天工"};
	
	public static uint[] FuYinOpenLevel = new uint[6]
		{
			20,
			30,
			40,
			50,
			60,
			70,
		};
	
	
}

public enum EJob_Type
{
	JobClass_ZhanShi 	=1,
	JobClass_FaShi		=2,
	JobClass_GongJianShou=3,
	JobClass_Num,
}

//宠物忠诚度描述
public enum Pet_Loyal_Type
{
	Pet_Logyal_None,
	Pet_Logyal_FanKang,
	Pet_Logyal_ShunCong,
	Pet_Logyal_XinLai,
	Pet_Logyal_ZhongCheng,
}

//宠物资质类型
public enum Pet_Aptitude_Type
{
	Pet_Aptitude_Com,
	Pet_Aptitude_LiangHao,
	Pet_Aptitude_YouXiu,
	Pet_Aptitude_ZhuoYue,
	Pet_Aptitude_WanMei,
	Pet_Aptitude_ShenWu,
}
