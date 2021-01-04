using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTSelectScene_Pop : XUICtrlTemplate<XSelectScene_Pop>
{	
	private string	mName;
	private bool	IsLock;
	private int		mSceneLevel;
	private int		mStarLevel;
	private int		mIndex;
	
	private uint	mAtlasID;
	private string	mSpriteName;
	
	public XUTSelectScene_Pop()
	{
		XEventManager.SP.AddHandler(OnGetSceneData, EEvent.SelectScene_SendData);		
	}
	
	public override void OnShow()
	{
		//LogicUI.Name.text	= mName;
		LogicUI.SetStarLevel(mStarLevel);
		LogicUI.Select(0);
		LogicUI.mHardLevel	= 1;
		LogicUI.SetIndex(mIndex);
		if(IsLock)
			LogicUI.Lock(0);
		else
		{
			if(mSceneLevel >= 1 && mStarLevel > 0)
				LogicUI.Lock(mSceneLevel + 1);
			else
				LogicUI.Lock(mSceneLevel);
		}
			
		
		if(mSpriteName != null)
		{
			XUIDynamicAtlas.SP.SetSprite(LogicUI.LockSprite, (int)mAtlasID, mSpriteName);
			LogicUI.LockSprite.layer	= 3;
		}
	}
	
	public void OnGetSceneData(EEvent evt, params object[] args)
	{
		mIndex		= (int)args[0];
		mAtlasID	= (uint)args[1];
		mSpriteName	= (string)args[2];
		mName		= (string)args[3];
		IsLock		= (bool)args[4];
		mSceneLevel	= (int)args[5];
		mStarLevel	= (int)args[6];
	}
	
	
};
