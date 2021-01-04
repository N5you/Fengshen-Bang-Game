using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

public class XDayActivityAward
{
    public enum EAwardStatus
    {
        NotAvailable = 0,
        Available = 1,
        Received = 2
    }

    private byte awardID;
    private EAwardStatus status;

    public XDayActivityAward(byte _awardID, EAwardStatus _status)
    {
        awardID = _awardID;
        status = _status;
    }

    public EAwardStatus Status
    {
        get { return this.status; }
        private set { this.status = value; }
    }

    public byte ID
    {
        get { return this.awardID; }
        private set { this.awardID = value; }
    }

    public void SetAwardStatus(EAwardStatus _status)
    {
        if (_status == Status)
            return;
        Status = _status;
    }
}

public class XDayActivityItem
{
    public enum EActivityStatus
    {
        Available = 0,
        Complated = 1,
        NotAvailable = 2
    }

    private XCfgDayActivityBase XCfg;
    private uint itemID;
    private uint curProgress;
    private int mActivityValue;
    private EActivityStatus status;

    public uint ItemID { get { return this.itemID; } }

    public EActivityStatus Status { get { return status; } }

    public uint CurProgress { get { return curProgress; } }

    public int ActivityValue
    {
        get
        {
            mActivityValue = XCfgDayActivityBaseMgr.SP.GetConfig(this.itemID).VitalityValue;
            return mActivityValue;
        }
    }

    public XDayActivityItem(uint _itemID, EActivityStatus _status, ushort _curProgress)
    {
        itemID = _itemID;
        status = _status;
        curProgress = _curProgress;
    }

    public void SetStatus(EActivityStatus _status)
    {
        if (_status == status)
            return;
        status = _status;
    }

    public void SetCurProgress(uint _progress)
    {
        if (curProgress == _progress)
            return;
        curProgress = _progress;
    }
}

public class XDayActivityManager : XSingleton<XDayActivityManager>
{
    private SortedList<uint, XDayActivityItem> mActivityItemList;
    private Dictionary<uint, XDayActivityAward> mAwardList;
    private int mCurActivityValue;

    public int CurActivityValue
    {
        get
        {
            return mCurActivityValue;
        }

        private set { mCurActivityValue = value; }
    }

    public XDayActivityManager()
    {
        mActivityItemList = new SortedList<uint, XDayActivityItem>();
        mAwardList = new Dictionary<uint, XDayActivityAward>();
        mCurActivityValue = 60;

        //mActivityItemList.Add(1, new XDayActivityItem(1, XDayActivityItem.EActivityStatus.Available, 1));
        //mActivityItemList.Add(2, new XDayActivityItem(2, XDayActivityItem.EActivityStatus.Available, 2));
        //mActivityItemList.Add(3, new XDayActivityItem(3, XDayActivityItem.EActivityStatus.NotAvailable, 5));


        //mAwardList.Add(1, new XDayActivityAward(1, XDayActivityAward.EAwardStatus.Available));
        //mAwardList.Add(2, new XDayActivityAward(2, XDayActivityAward.EAwardStatus.Available));
        //mAwardList.Add(3, new XDayActivityAward(3, XDayActivityAward.EAwardStatus.Received));
        //mAwardList.Add(4, new XDayActivityAward(4, XDayActivityAward.EAwardStatus.Received));
        //mAwardList.Add(5, new XDayActivityAward(5, XDayActivityAward.EAwardStatus.NotAvailable));
    }

    public SortedList<uint, XDayActivityItem> GetActivityItemList()
    {
        return mActivityItemList;
    }

    public Dictionary<uint, XDayActivityAward> GetAwardList()
    {
        return mAwardList;
    }

    public XDayActivityItem GetActivityItem(uint itemID)
    {
        if (mActivityItemList.ContainsKey(itemID))
        {
            return mActivityItemList[itemID];
        }
        return null;
    }

    public XDayActivityAward GetAwardInfo(uint _awardID)
    {

        if (mAwardList.ContainsKey(_awardID))
        {
            return mAwardList[_awardID];
        }
        return null;
    }

    public void AddAwardInfo(XDayActivityAward award)
    {
        XCfgDayActivityAward xcfg = XCfgDayActivityAwardMgr.SP.GetConfig(award.ID);
        if (xcfg == null)
        {
            Debug.LogError("AddAwardInfo, xcfg not have awardID" + award.ID);
            return;
        }

        if (mAwardList.ContainsKey(award.ID))
        {
            mAwardList[award.ID].SetAwardStatus(award.Status);
        }
        else
        {
            mAwardList.Add(award.ID, award);
        }
    }

    public void AddItemInfo(XDayActivityItem item)
    {
        XCfgDayActivityBase xcfg = XCfgDayActivityBaseMgr.SP.GetConfig(item.ItemID);
        if (xcfg == null)
        {
            Debug.LogError("AddItemInfo, xcfg not have item" + item.ItemID);
            return;
        }
        if (mActivityItemList.ContainsKey(item.ItemID))
        {
            mActivityItemList[item.ItemID].SetCurProgress(item.CurProgress);
            mActivityItemList[item.ItemID].SetStatus(item.Status);
        }
        else
        {
            mActivityItemList.Add(item.ItemID, item);
        }
    }

    public void RequestGetReward(uint awardID)
    {
        if (!mAwardList.ContainsKey(awardID))
        {
            Debug.LogWarning("Not Have awardID");
        }

        if (mAwardList[awardID].Status != XDayActivityAward.EAwardStatus.Available)
        {
            //SendNotice the item not avilable;
            Debug.LogWarning("EAwardStatus Not at Available");
        }
        CS_UInt.Builder msg = CS_UInt.CreateBuilder();
        msg.SetData(awardID);
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_DayActivity_GetAward, msg.Build());
    }

    public void ResetDayActivity()
    {
        XCfgDayActivityBase xcfg = null;
        //重置每项的状态为默认数值
        foreach (KeyValuePair<uint, XDayActivityItem> s in mActivityItemList)
        {
            s.Value.SetStatus(XDayActivityItem.EActivityStatus.Available);
            s.Value.SetCurProgress(0);
        }

        foreach (KeyValuePair<uint, XDayActivityAward> s in mAwardList)
        {
            s.Value.SetAwardStatus(XDayActivityAward.EAwardStatus.NotAvailable);
        }

        if (XUIManager.SP.GetUIControl(EUIPanel.eDayActivity).IsLogicShow)
            XEventManager.SP.SendEvent(EEvent.DayActivity_ResetInfo);
    }

#region Server To Client 
    public void ON_SC_ReciveValueActivity(SC_DayActivity_ActivityValue msg)
    {
        //CurActivityValue = msg.ActivityValue;

        if (XUIManager.SP.GetUIControl(EUIPanel.eDayActivity).IsLogicShow)
            XEventManager.SP.SendEvent(EEvent.DayActivity_UpdateActivityValue);
    }

    public void ON_SC_ReciveItemInfo(SC_DayActivity_ItemInfo msg)
    {
        XDayActivityItem activityItem = new XDayActivityItem(msg.ItemID, (XDayActivityItem.EActivityStatus)msg.Status, (ushort)msg.CurProcess);
        AddItemInfo(activityItem);

        if (XUIManager.SP.GetUIControl(EUIPanel.eDayActivity).IsLogicShow)
            XEventManager.SP.SendEvent(EEvent.DayActivity_UpdateItem);
    }

    public void ON_SC_ReciveAwardInfo(SC_DayActivity_AwardInfo msg)
    {
        XDayActivityAward award = new XDayActivityAward((byte)msg.AwardID, (XDayActivityAward.EAwardStatus)msg.Status);
        AddAwardInfo(award);

        if (XUIManager.SP.GetUIControl(EUIPanel.eDayActivity).IsLogicShow)
            XEventManager.SP.SendEvent(EEvent.DayActivity_UpdateAward);
    }

    public void ON_SC_ReciveAllData(SC_DayActivity_AllData msg)
    {
        CurActivityValue = msg.ActivityValue;

        foreach (SC_DayActivity_AwardInfo s in msg.AwardInfoList)
        {
            XDayActivityAward award = new XDayActivityAward((byte)s.AwardID, (XDayActivityAward.EAwardStatus)s.Status);
            AddAwardInfo(award);
        }

        foreach (SC_DayActivity_ItemInfo s in msg.ItemInfoList)
        {
            XDayActivityItem itemInfo = new XDayActivityItem(s.ItemID, (XDayActivityItem.EActivityStatus)s.Status, (ushort)s.CurProcess);
            AddItemInfo(itemInfo);
        }
    }

    public void ON_SC_ReciveResetInfo()
    {
        ResetDayActivity();
    }
#endregion
}
