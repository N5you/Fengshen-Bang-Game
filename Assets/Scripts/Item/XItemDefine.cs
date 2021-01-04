using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

//--4>: 服务器和客户端应该统一
public enum EItemBoxType
{
	BoxType_Invaid = -1,
    Begin = 0,
    Bag = Begin,
    Equip,
    Bank,
	PetEquip,
    End,
}

//装备槽位类型
public enum EQUIP_SLOT_TYPE
{
	EQUIP_SLOT_NONE,				// 不可装备
	EQUIP_SLOT_WEAPON,				// 武器
	EQUIP_SLOT_ARMOUR,				// 铠甲
	EQUIP_SLOT_AMULET,				// 护符
	EQUIP_SLOT_RING,				// 戒指 
	EQUIP_SLOT_MAGIC_WEAPON,		// 法宝
	EQUIP_SLOT_BOARD,				// 令牌	
	EQUIP_SLOT_FUYIN,				// 符印
	EQUIP_SLOP_FASHION,				// 时装
	EQUIP_SLOT_NUM,
};

public enum ITEM_BING_TYPE
{
	ITEM_BIND_NONE,					// 不绑定
	ITEM_BIND_RELEASE,				// 解封后绑定
	ITEM_BIND_GET,					// 获取绑定
	ITEM_BIND_NUM
};

public enum EQUIP_POS
{
	EQUIP_POS_INVALID = -1,
	EQUIP_POS_WEAPON,
	EQUIP_POS_ARMOUR,
	EQUIP_POS_AMULET,
	EQUIP_POS_RING,
	EQUIP_POS_MAGIC_WEAPON,
	EQUIP_POS_BOARD,
	EQUIP_POS_FASHION,
	EQUIP_POS_FUYIN_START,
	EQUIP_POS_FUYIN_NUM = 6,
	EQUIP_POS_FUYIN_END = EQUIP_POS.EQUIP_POS_FUYIN_START + EQUIP_POS.EQUIP_POS_FUYIN_NUM -1,
	EQUIP_POS_NUM
}

public enum EBAG_DATA
{
	MAX_COM_BAG_NUM=3,
	ONE_COM_BAG_SLOT_NUM=36,
	ROLE_MAX_COM_BAG_ITEM_NUM=3*36,
	
	MAX_BANK_BAG_NUM=3,
	ONE_BANK_BAG_SLOT_NUM=10,
	ROLE_BANK_ITEM_NUM=3*10,
	
	ROLE_MAX_EQUIP_NUM=13,
	ROLE_MAX_PET_EQUIP_NUM = ROLE_MAX_EQUIP_NUM*20,
};

public enum EBAG_DATA_POS
{
	ROLE_COM_BAG_ITEM_START= 0,
	ROLE_COM_BAG_ITEM_END	= ROLE_COM_BAG_ITEM_START + EBAG_DATA.ROLE_MAX_COM_BAG_ITEM_NUM - 1,
	ROLE_EQUIP_START		= ROLE_COM_BAG_ITEM_END + 1,
	ROLE_EQUIP_END			= ROLE_EQUIP_START + EBAG_DATA.ROLE_MAX_EQUIP_NUM - 1 ,
	ROLE_BANK_ITEM_START	= ROLE_EQUIP_END + 1,
	ROLE_BANK_ITEM_END		= ROLE_BANK_ITEM_START + EBAG_DATA.ROLE_BANK_ITEM_NUM - 1,
	ROLE_PET_EQUIP_START = ROLE_BANK_ITEM_END + 1,
	ROLE_PET_EQUIP_END =  ROLE_PET_EQUIP_START + EBAG_DATA.ROLE_MAX_PET_EQUIP_NUM -1,
	ROLE_MAX_ITEM_NUM		= ROLE_PET_EQUIP_END + 1,
};

////物品大类型
//public enum EItem_Type
//{
//	EITEM_TYPE_NONE,
//	EITEM_TYPE_WEAPON,		//1武器
//	EITEM_TYPE_ARMOR,		//2防具
//	EITEM_TYPE_MOUNT,		//3坐骑
//	EITEM_TYPE_MAGIC_WEAPON,//4法宝
//	EITEM_TYPE_MATERIAL,	//5材料
//	EITEM_TYPE_GOODS,		//6商品
//	EITEM_TYPE_RECIPE,		//7配方
//	EITEM_TYPE_CONSUMABLE,	//8消耗品
//	EITEM_TYPE_GEM,			//9宝石
//	EITEM_TYPE_CARD,		//10卡牌
//	EITEM_TYPE_CONTAINER,	//11容器
//	EITEM_TYPE_QUEST,		//12任务物品
//	EITEM_TYPE_PET,			//13宠物
//	EITEM_TYPE_NUM
//};
//
//public enum EItem_Armour_Sub_Type
//{
//	EQUIP_ARMOUR_ARMOUR			=1,		//铠甲
//	EQUIP_ARMOUR_AMULET			=2,		//护符
//	EQUIP_ARMOUR_RING			=3,		//戒指 
//	EQUIP_ARMOUR_MAGIC_WEAPON	=4,		//法宝
//	EQUIP_ARMOUR_BOARD			=5,		//令牌
//}

public enum EItem_Quality
{
	EITEM_QUALITY_INVALID = -1,
	EITEM_QUALITY_WHITE   = 1,		//1凡品--白色
	EITEM_QUALITY_GREEN,			//2良品--绿色
	EITEM_QUALITY_BLUE,				//3上品--蓝色
	EITEM_QUALITY_PURPLE,			//4极品--紫色
	EITEM_QUALITY_ORANGE,			//5神品--橙色
	EITEM_QUALITY_GOLD,				//6圣品--金色
	EITEM_QUALITY_NUM
};

public enum EItem_Gem_Type
{
	Gem_Type_Gold = 1,
	Gem_Type_Wood = 2,
	Gem_Type_Water= 3,
	Gem_Type_Fire = 4,
	Gem_Type_Soil = 5,
	Gem_Type_Num  = 5,
}

public enum EItem_Equip
{
	MAX_INLAY_GEM_NUM	= 5,
	MAX_RANDOM_ATTR_NUM = 5,
}

public class XItemDefine
{
	// 物品大类对应的子类枚举定义
    public static readonly System.Type[] ITEM_SUB_TYPE_ENUM = {
		null,
		typeof(EITEM_WEAPON_SUB_TYPE), 
		typeof(EITEM_ARMOR_SUB_TYPE),
		typeof(EITEM_FUYIN_SUB_TYPE),
		typeof(EITEM_MATERIAL_SUB_TYPE),
		typeof(EITEM_GOODS_SUB_TYPE),
		typeof(EITEM_GEM_SUB_TYPE),
		typeof(EITEM_RECIPE_SUB_TYPE),
		typeof(EITEM_MOUNT_SUB_TYPE),
		typeof(EITEM_MAGIC_WEAPON_SUB_TYPE),
		typeof(EITEM_CARD_SUB_TYPE),
		typeof(EITEM_CONTAINER_SUB_TYPE),
		typeof(EITEM_QUEST_SUB_TYPE),
		typeof(EITEM_PET_SUB_TYPE),
	};

	// 物品类别名称的字符串对应关系, 注意顺序需要和 EItem_Type 保持一致
    public static readonly uint[] STR_ITEM_MAIN_TYPE = {
		10001,
		100000, 100001, 100002, 100003, 100004, 100005, 100006, 100007, 100008, 100009, 100010, 100011, 100012,
	};

	// 物品子类名称字符串关系对应, 注意顺序需要和各个小类别的枚举一致
	// 每种子类预留 10 个
    public static readonly uint[] STR_ITEM_SUB_TYPE = {
		10001,
		// 武器
		101001, 101002, 101003, 101004, 10001, 10001, 10001, 10001, 10001, 10001,
		// 防具
		101011, 101012, 101013, 101014, 101015, 10001, 10001, 10001, 10001, 10001,
		// 符印
		101021, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001,
		// 材料
		101031, 101032, 101033, 101034, 101035, 101036, 10001, 10001, 10001, 10001,
		// 货物
		101041, 101042, 101043, 101044, 101045, 101046, 10001, 10001, 10001, 10001,
		// 宝石
		101051, 101052, 101053, 101054, 101055, 10001, 10001, 10001, 10001, 10001,
		// 配方
		101061, 101062, 101063, 10001, 10001, 10001, 10001, 10001, 10001, 10001,
		// 坐骑
		101071, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001,
		// 法宝
		101081, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001,
		// 卡牌
		101091, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001,
		// 容器
		101101, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001,
		// 任务
		101111, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001,
		// 宠物
		101121, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001, 10001,
	};

    public static string GetItemMainTypeStr(int tid)
    {
        if (tid < 0 || tid >= STR_ITEM_MAIN_TYPE.Length)
        {
            return string.Empty;
        }
		XCfgString cfg = XCfgStringMgr.SP.GetConfig(STR_ITEM_MAIN_TYPE[tid]);
        return null == cfg ? string.Empty : cfg.Content;
    }

    public static string GetItemSubTypeStr(int tid)
    {
        if (tid < 0 || tid >= STR_ITEM_SUB_TYPE.Length)
        {
            return string.Empty;
        }
		XCfgString cfg = XCfgStringMgr.SP.GetConfig(STR_ITEM_SUB_TYPE[tid]);
		if (cfg == null)
        {
			return string.Empty;
        }
        return cfg.Content;
    }

	public static System.Type GetSubTypeByID(int tid)
    {
        if (tid < 0 || tid >= ITEM_SUB_TYPE_ENUM.Length)
        {
            return null;
        }
        return ITEM_SUB_TYPE_ENUM[tid];
    }

	// 物品类型表示, 要求所有类型务必从 1 下标开始, 0 表示无效.
	// 主类型为 X, 子类型为 Y, 表示为 Y * FACTOR_OF_SUBTYPE + X
	// 如果子类型为 0 表示的是一个大类
	public static readonly uint FACTOR_ITEM_TYPE = 10000;
    public static void SplitItemType(uint tp, out ushort mainType, out ushort subType)
    {
        mainType = (ushort)(tp % FACTOR_ITEM_TYPE);
        subType = (ushort)(tp / FACTOR_ITEM_TYPE);
    }

    public static uint CombineItemType(ushort mainType, ushort subType)
    {
        return mainType + subType * FACTOR_ITEM_TYPE;
    }
}
