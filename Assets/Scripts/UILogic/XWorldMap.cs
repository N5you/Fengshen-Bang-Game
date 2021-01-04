using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UILogic/XWorldMap")]
public class XWorldMap : XUIBaseLogic
{
	[System.Serializable]
	public class MapIcon
	{
		private UIImageButton	mBtn;
		private uint SceneId = 0;
		private string mCloseSpriteName;
		private string mOpenSpriteNameN;
		private string mOpenSpriteNameH;
		private bool	mIsOpen	= true;
		
		public MapIcon(uint sceneId, UIImageButton Btn,string CloseSpriteName,string OpenSpriteNameN,string OpenSpriteNameH)
		{
			SceneId 			= sceneId;
			mBtn 				= Btn;
			mCloseSpriteName	= CloseSpriteName;
			mOpenSpriteNameN	= OpenSpriteNameN;
			mOpenSpriteNameH	= OpenSpriteNameH;
			
			XCfgWorldMap cfgWM = XCfgWorldMapMgr.SP.GetConfig(SceneId);
			if(cfgWM == null)
				 return;
			
			if(cfgWM.RequireQuestID > 0)
			{
				if(!XMissionManager.SP.hasReferMissionInList(cfgWM.RequireQuestID))
					mIsOpen = false;
			}
			else
			{
				mIsOpen	= true;
			}
			
			SetState(mIsOpen);
			
			NGUITools.AddWidgetCollider(mBtn.gameObject);
			UIEventListener listen = UIEventListener.Get(mBtn.gameObject);
			listen.onClick += OnClickIcon;
		}
		
		public void SetState(bool IsOpen)
		{
			mIsOpen	= IsOpen;
			if(mIsOpen)
			{
				mBtn.normalSprite	= mOpenSpriteNameN;
				mBtn.hoverSprite	= mOpenSpriteNameH;
				mBtn.pressedSprite	= mOpenSpriteNameN;
				
				mBtn.UpdateImage();
			}
			else
			{
				mBtn.normalSprite	= mCloseSpriteName;
				mBtn.hoverSprite	= mCloseSpriteName;
				mBtn.pressedSprite	= mCloseSpriteName;
				
				mBtn.UpdateImage();
			}
		}
		
		private void OnClickIcon(GameObject go)
		{
			if(mIsOpen)
				XEventManager.SP.SendEvent(EEvent.WorldMap_RequireLoadScene, SceneId);
		}
		
		public void ReflashIcon()
		{
			XCfgWorldMap cfgWM = XCfgWorldMapMgr.SP.GetConfig(SceneId);
			if(cfgWM == null)
				 return;
			
			if(cfgWM.RequireQuestID > 0)
			{
				if(!XMissionManager.SP.hasReferMissionInList(cfgWM.RequireQuestID))
					mIsOpen = false;
			}
			else
			{
				mIsOpen	= true;
			}
			
			SetState(mIsOpen);
		}
	}
	
	public UIAtlas WorldMapAtlas = null;	
	public UIImageButton[] IconBtn;
	public GameObject Exit = null;
	private List<MapIcon> m_icons = new List<MapIcon>();
	
	public override bool Init()
	{
		base.Init();
		UIEventListener listen = UIEventListener.Get(Exit.gameObject);
		listen.onClick += OnClickExit;
		return true;
	}
	
	public void OnAddMapIcon(int nPosId, uint nSceneId, string CloseSpriteName,string OpenSpriteNameN,string OpenSpriteNameH)
	{
		if(nPosId < 1 || nPosId > IconBtn.Length)
			return;
		
		m_icons.Add(new MapIcon(nSceneId, IconBtn[nPosId-1],CloseSpriteName,OpenSpriteNameN,OpenSpriteNameH));
	}
	
	private void OnClickExit(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eWorldMap);
	}
	
	public void ReflashIcon()
	{
		foreach(MapIcon icon in m_icons)
		{
			icon.ReflashIcon();
		}
	}
}

