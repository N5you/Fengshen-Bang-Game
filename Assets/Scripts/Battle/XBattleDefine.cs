using System;
using XGame.Client.Packets;

enum EBattleObjectType
{
    ebot_Begin = 0,
    ebot_Player = ebot_Begin,		// 玩家
    ebot_Monster,					// 怪物
    ebot_Magic,						// 法宝
    ebot_Pet,						// 宠物
    //--4>: ============================================
    ebot_End,
};

public class XBattleDefine
{
    public static readonly int BATTLE_MAGIC_POS = 0;
    public static readonly int BATTLE_POS_COUNT = (9+1);
}

public class XBattlePosition
{
    public EBattleGroupType Group { get; private set; }
    public uint Position { get; private set; }

    public uint ToUInt()
    {
        return (((uint)Group) << 5) | Position;
    }

    public static bool IsValid(EBattleGroupType e, uint pos)
    {
        if (e < EBattleGroupType.eBattleGroup_Begin || e >= EBattleGroupType.eBattleGroup_End)
        {
            return false;
        }
        if (pos < 0 || pos >= XBattleDefine.BATTLE_POS_COUNT)
        {
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        return string.Format("BattlePosition {0}-{1}", Group, Position);
    }

    private XBattlePosition(EBattleGroupType e, uint pos)
    {
        Group = e;
        Position = pos;
    }

    public static XBattlePosition Create(EBattleGroupType e, uint pos)
    {
        if (XBattlePosition.IsValid(e, pos))
        {
            return new XBattlePosition(e, pos);
        }
        return null;
    }

    public static XBattlePosition Create(uint netPos)
    {
        uint nGroup = netPos >> 5;
        // 网络包位置信息保存8位, 1-5 保存位置信息, 6-8 保存分组信息
        uint pos = netPos & 0x1F;
        return Create((EBattleGroupType)nGroup, pos);
    }
}
