using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UILogic/XSelectScene")]
public class XSelectScene : XDefaultFrame
{
	private static readonly int MAX_SEL_SCENE_NUM = 8;
	private static readonly int MAX_STAR_NUM = 5;
	
	[System.Serializable]
	public class SceneChild
	{
		public GameObject 					Root;
		public GameObject					mStartList;
		public UILabel 						Name;
		private uint						mPassID;
		private bool						IsLock;
		private int							StarLevel;
		private UISprite					LockSprite;
		private int							SceneLevel;
		private UISprite[]					StarSpriteArray	= new UISprite[MAX_STAR_NUM];
		private GameObject					StarRoot;
		
		private UIAtlas						OldLockAtlas;
		private string						OldLockSpriteName;
		
		private int m_nIndex;
		
		public void SceneChildInit(int nIndex, string strName)
		{			
			Name = Root.transform.Find("Label_ChildName").GetComponent<UILabel>();
			Name.text 	= strName;
			m_nIndex 	= nIndex;
			IsLock		= true;
			StarLevel	= 0;
			SceneLevel	= 0;
			StarRoot	= mStartList;
			
			LockSprite	= Root.transform.Find("Sprite").GetComponent<UISprite>();	
			OldLockAtlas= LockSprite.atlas;
			OldLockSpriteName	= LockSprite.spriteName;
			
			StarSpriteArray[0]  = StarRoot.transform.Find("Star_Sprite1").GetComponent<UISprite>();
			StarSpriteArray[1]	= StarRoot.transform.Find("Star_Sprite2").GetComponent<UISprite>();
			StarSpriteArray[2]	= StarRoot.transform.Find("Star_Sprite3").GetComponent<UISprite>();
			StarSpriteArray[3]	= StarRoot.transform.Find("Star_Sprite4").GetComponent<UISprite>();
			StarSpriteArray[4]	= StarRoot.transform.Find("Star_Sprite5").GetComponent<UISprite>();
			
			for(int i = 0; i < 5;i++)
			{
				StarSpriteArray[i].gameObject.SetActive(false);
			}
			
			UIEventListener lis = UIEventListener.Get(Root); 
			lis.onClick += OnClick;
		}
		
		private void SetStarSprite(int level)
		{
			for(int i = 0; i < StarSpriteArray.Length; i++)
			{
				if(i >= level)
					NGUITools.SetActive(StarSpriteArray[i].gameObject,false);
				else
					NGUITools.SetActive(StarSpriteArray[i].gameObject,true);
			}
		}
		
		private void SetVisibleLock(bool isVisible)
		{
			if(isVisible)
			{
				LockSprite.atlas		= OldLockAtlas;
				LockSprite.spriteName	= OldLockSpriteName;
			}
			else
			{
				XCfgClientScene cfg = XCfgClientSceneMgr.SP.GetConfig(mPassID);
				if(cfg != null)
				{
					XUIDynamicAtlas.SP.SetSprite(LockSprite, (int)cfg.AtlasID, cfg.SpriteName);
					LockSprite.layer	= 3;
				}
			}
		}
		
		public void SetVisible(bool IsVisible)
		{
			NGUITools.SetActiveChildren(Root,IsVisible);
			NGUITools.SetActiveSelf(Root,IsVisible);
		}
		
		public void OnClick(GameObject go)
		{
			if(IsLock)
				return ;
			
			XCfgClientScene cfg = XCfgClientSceneMgr.SP.GetConfig(mPassID);
			if(cfg != null)
				XEventManager.SP.SendEvent(EEvent.SelectScene_SendData,m_nIndex,cfg.AtlasID,cfg.SpriteName,Name.text,IsLock,SceneLevel,StarLevel);
			XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eSelectScene_Pop);
		}
		
		public void Init(uint passID,string strName,bool isLock,int SceneLevel,int StarLevel)
		{
			mPassID			= passID;
			Name.text 		= strName;
			this.IsLock		= isLock;
			this.StarLevel	= StarLevel;
			this.SceneLevel	= SceneLevel;
			SetVisible(true);
			
			SetStarSprite(StarLevel);
			SetVisibleLock(isLock);	
		}
	}

	public UILabel LabelSceneName = null;	
	public SceneChild[] m_Children	= new SceneChild[MAX_SEL_SCENE_NUM];	
	
	public override bool Init()
	{
		base.Init();
		for(int i = 0; i < MAX_SEL_SCENE_NUM;i++)
		{
			m_Children[i].SceneChildInit(i,"");
		}
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		
		return true;
	}
	
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eSelectScene);
	}
	
	public override void Show()
	{		
		base.Show();
		Clear();
	}
	
	public void SetName(string strName)
	{
		//LabelSceneName.text = strName;
	}

	public void AddScene(int nIndex,uint passID, string strName,bool isLock,int sceneLevel,int starLevel)
	{
		if(MAX_SEL_SCENE_NUM <= nIndex)
			return ;
		m_Children[nIndex].Init(passID,strName,isLock,sceneLevel,starLevel);
	}
	
	public void Clear()
	{
		for(int i = 0; i < MAX_SEL_SCENE_NUM; i++)
		{
			m_Children[i].SetVisible(false);
		}
	}
}