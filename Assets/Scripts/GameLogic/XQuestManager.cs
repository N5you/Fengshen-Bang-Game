using System;
using UnityEngine;
using XGame.Client.Packets;
using System.Collections;
using System.Collections.Generic;

class XQuestManager
{
    //--4>TODO: 服务器和客户端应该统一
    static readonly int MAX_QUEST_STATE_NUM = 7;

    class XActiveQuest
    {
        public uint Id;
        public uint Flag;
        public List<uint> State = new List<uint>(MAX_QUEST_STATE_NUM);
    }

    #region property and init

    public Hashtable ActiveQuest { get; private set; }
    public List<uint> MagicData { get; private set; }
    public List<uint> AllQuestBit { get; private set; }

    public XQuestManager()
    {
        ActiveQuest = new Hashtable();
        MagicData = new List<uint>();
        AllQuestBit = new List<uint>();
    }

    ~XQuestManager()
    {
        ActiveQuest = null;
        MagicData = null;
        AllQuestBit = null;
    }

    #endregion

    #region Packet Process Functions

    public void On_SC_ActiveQuestList(SC_ActiveQuestList msg)
    {
        ActiveQuest.Clear();
        for (int i = 0; i < msg.QuestListCount; ++i)
        {
            XActiveQuest quest = new XActiveQuest();
            quest.Id = msg.GetQuestList(i).QuestId;
            quest.Flag = msg.GetQuestList(i).QuestFlag;
            quest.State.AddRange(msg.QuestListList[i].QuestStateList);
            ActiveQuest.Add(quest.Id, quest);
        }
    }

    public void On_SC_QuestMagic(SC_QuestMagic msg)
    {
        MagicData.Clear();
        MagicData.AddRange(msg.DataList);
    }

    public void On_SC_AllQuestBit(SC_AllQuestBit msg)
    {
        AllQuestBit.Clear();
        AllQuestBit.AddRange(msg.DataList);
    }

    internal void On_SC_UpdateQuestFlag(SC_UpdateQuestFlag msg)
    {
        if (ActiveQuest.Contains(msg.QuestId))
        {
            XActiveQuest quest = ActiveQuest[msg.QuestId] as XActiveQuest;
            quest.Flag = msg.QuestFlag;
        }
    }

    internal void On_SC_SetQuestState(SC_SetQuestState msg)
    {
        if (ActiveQuest.Contains(msg.QuestId))
        {
            XActiveQuest quest = ActiveQuest[msg.QuestId] as XActiveQuest;
            int index = (int)msg.StateIndex;
            if (index >= 0 && index < quest.State.Count)
            {
                quest.State[index] = msg.StateValue;
                Log.Write("[TEST] SetQuestState id:{0} index:{1} value:{2}", quest.Id, index, msg.StateValue);
            }
        }
    }

    internal void On_SC_SetQuestMagic(SC_SetQuestMagic msg)
    {
        int index = msg.MagicIndex;
        if (index >= 0 && index < MagicData.Count)
        {
            MagicData[index] = msg.MagicValue;
            Log.Write("[TEST] SetQuestMagic index:{0} value:{1}", index, msg.MagicValue);
        }
    }

    internal void On_SC_AcceptQuest(SC_AcceptQuest msg)
    {
        if (!ActiveQuest.Contains(msg.QuestId))
        {
            XActiveQuest quest = new XActiveQuest();
            quest.Id = msg.QuestId;
            for (int i = 0; i < MAX_QUEST_STATE_NUM; ++i)
            {
                quest.State.Add(0);
            }
            ActiveQuest.Add(quest.Id, quest);
        }
    }

    internal void On_SC_FinishQuest(SC_FinishQuest msg)
    {
        if (ActiveQuest.Contains(msg.QuestId))
        {
            ActiveQuest.Remove(msg.QuestId);
        }
    }

    internal void On_SC_DelQuest(SC_DelQuest msg)
    {
        if (ActiveQuest.Contains(msg.QuestId))
        {
            ActiveQuest.Remove(msg.QuestId);
        }
    }

    #endregion

}
