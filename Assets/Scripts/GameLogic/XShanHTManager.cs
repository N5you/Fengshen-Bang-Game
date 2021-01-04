using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Packets;

class XShanHTManager
{
	private SC_ShanHT_Info		mCurInfoMsg;
	public  SC_ShanHT_Rank   mRnakInfo;
	private uint	mLeftTimes;
	private uint	mMaxLevel;
	private uint 	mLeftLife;
	private uint	mCurLevel;
	private uint	mLeftBuyCnt;
	//上一次挑战的关卡
	public  uint 	mLastLevel;
	
	private bool	mAutoFight;
	
	public XShanHTManager()
	{
        SP = this;
		mLeftTimes = 0;
		mMaxLevel = 0;	
		mLeftLife = 0;
		mCurLevel = 0;
		mLastLevel = 0;
	}
	public static XShanHTManager SP { get; private set; }
	
    public uint LefTimes
    {
        get
        {
            return mLeftTimes;
        }
        set 
		{ 
			mLeftTimes = value;
			
			XDailyPlaySignMgr.SP.DoHandleShowPlaySign(DailyPlayKey.DailyPlay_ShanHe, (int)mLeftTimes, 0);
		}
    }
	
    public uint CurLevel
    {
        get
        {
            return mCurLevel;
        }
        set 
		{ 
			mLastLevel = mCurLevel;
			mCurLevel = value;
		}
    }
    public uint LeftBuyCnt
    {
        get
        {
            return mLeftBuyCnt;
        }
        set { mLeftBuyCnt = value; }
    }
    public bool AutoFight
    {
        get
        {
            return mAutoFight;
        }
        set 
		{ 
			mAutoFight = value;
		}
    }
	
    public uint LeftLife
    {
        get
        {
            return mLeftLife;
        }
        set 
		{ 
			mLeftLife = value;
		}
    }
	
    public uint MaxLevel
    {
        get
        {
            return mMaxLevel;
        }
        set 
		{ 
			mMaxLevel = value;
		}
    }
	
	public uint GetShanHeID(uint level)
	{
		for( uint i = 0; i < 5; ++i )
		{
			XCfgShanHTBase curConfig = XCfgShanHTBaseMgr.SP.GetConfig((uint)i + 1);
			if(curConfig.EndIndex >= level)
				return i + 1;
		}
		return 0;
	}
	public uint  GetCurShanHe()
	{
		return GetShanHeID(CurLevel);
	}
	public void BattleWin()
	{
		if(CurLevel > MaxLevel)
			MaxLevel = CurLevel;
		if(CurLevel % 20 == 0)
			CurLevel = 0;
		else
			CurLevel++;
	}
	public uint	getMaxLevel()
	{
		return mMaxLevel;
	}
	
	
		
    #region packets
    public void ON_SC_ShanHT_Info(SC_ShanHT_Info msg)
    {
		mCurInfoMsg = msg;
		LefTimes = msg.LeftTimes;
		mMaxLevel = msg.MaxLevel;
		mLeftLife = msg.LeftLife;
		mCurLevel = msg.CurLevel;
		mLeftBuyCnt = msg.LeftBuyTimes;
		
		XEventManager.SP.SendEvent(EEvent.ShanHe_Init_Info, msg);
    }
	
	public void ON_SC_ShanHT_Rank(SC_ShanHT_Rank msg)
	{
		mRnakInfo = msg;
		XEventManager.SP.SendEvent(EEvent.ShanHe_Rank, msg);
	}
    #endregion
}