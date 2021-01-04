using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XSelectScene_Pop")]
public class XSelectScene_Pop : XDefaultFrame
{
	private static readonly int MAX_STAR_NUM = 5;
	private static readonly int MAX_BTN_NUM = 3;
	public UILabel Name;
	public UISprite LockSprite;
	private int mIndex;
	public int mHardLevel;
	
	public UISprite[] SelectArray = new UISprite[MAX_BTN_NUM];
	public UISprite[] LockArray = new UISprite[MAX_BTN_NUM];
	private bool[] mIsEnable = new bool[MAX_BTN_NUM];
	
	public UISprite[] StarArray = new UISprite[MAX_STAR_NUM];
	
	public UIImageButton[] HardList = new UIImageButton[MAX_BTN_NUM];
	public UIImageButton  UIBtn;	
	
	public override bool Init()
	{
		mHardLevel	= 0;
		base.Init();
		UIEventListener Listener1 = UIEventListener.Get(UIBtn.gameObject);
		Listener1.onClick += OnOK;
		
		UIEventListener Listener2 = UIEventListener.Get(HardList[0].gameObject);
		Listener2.onClick += ClickNormal;
		
		UIEventListener Listener3 = UIEventListener.Get(HardList[1].gameObject);
		Listener3.onClick += ClickHard;
		
		UIEventListener Listener4 = UIEventListener.Get(HardList[2].gameObject);
		Listener4.onClick += ClickVeryHard;
		
		mIsEnable[0] = false;
		mIsEnable[1] = false;
		mIsEnable[2] = false;
		
		return true;		
	}
	
	public void OnOK(GameObject _go)
	{
		Hide();
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eSelectScene);		
		XLogicWorld.SP.MainPlayer.ChangeState((int)EStateId.esPreEnterScene,mIndex,mHardLevel);		
	}	
	public void ClickNormal(GameObject go)
	{
		if(!mIsEnable[0])
			return ;
		
		mHardLevel	= 1;
		Select(0);
	}
	
	public void ClickHard(GameObject go)
	{
		if(!mIsEnable[1])
			return ;
		mHardLevel	= 2;
		Select(1);
	}
	
	public void ClickVeryHard(GameObject go)
	{
		if(!mIsEnable[2])
			return ;
		mHardLevel	= 3;
		Select(2);
	}
	
	public void SetIndex(int index)
	{
		mIndex	= index;
	}
	
	public void Select(int index)
	{
		for(int i = 0; i < MAX_BTN_NUM; i++)
		{
			if(i == index)
			{
				NGUITools.SetActive(SelectArray[i].gameObject,true);
			}
			else
				NGUITools.SetActive(SelectArray[i].gameObject,false);
		}
	}
	
	public void Lock(int index)
	{
		for(int i = 0; i < MAX_BTN_NUM; i++)
		{
			if(i < index)
			{
				NGUITools.SetActive(LockArray[i].gameObject,false);
				mIsEnable[i] = true;
			}
			else
			{
				NGUITools.SetActive(LockArray[i].gameObject,true);
				mIsEnable[i] = false;
			}
				
		}
	}
	
	public void SetStarLevel(int Level)
	{
		for(int i = 0;i < MAX_STAR_NUM; i++)
		{
			if(i >= Level)
				NGUITools.SetActive(StarArray[i].gameObject,false);
			else
				NGUITools.SetActive(StarArray[i].gameObject,true);
		}
	}
}
