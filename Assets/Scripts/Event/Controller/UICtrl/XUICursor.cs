using UnityEngine;

class XUICursor : XUICtrlTemplate<XCursor>
{
	private XU3dModel m_mainModel	= null;
	private XU3dModel m_weaponModel = null;
	
	
	public XUICursor()
	{
		//RegEventAgent_CheckCreated(EEvent.Cursor_UpdateSprite, OnUpdateSprite);
		//XEventManager.SP.AddHandler(OnClearSprite,EEvent.Cursor_ClearSprite);
		RegEventAgent_CheckCreated(EEvent.Cursor_UpdateModel, OnUpdateModel);
		XEventManager.SP.AddHandler(OnClearModel,EEvent.Cursor_ClearModel);
		RegEventAgent_CheckCreated(EEvent.Cursor_UpdateIcon, OnUpdateIcon);
		RegEventAgent_CheckCreated(EEvent.Cursor_ClearIcon, OnClearIcon);
		RegEventAgent_CheckCreated(EEvent.Cursor_UpdateIconData, OnUpdateIconData);
		
	}

	public override void OnShow()
	{
		base.OnShow();
		LogicUI.ItemIcon.gameObject.SetActive(false);
	}
	
	
	
	
	public void OnUpdateSprite(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		uint uAtlasId	= (uint)args[1];
		string strSpriteName	= (string)args[2];	
		if(string.IsNullOrEmpty(strSpriteName))
			return ;
		LogicUI.SetSprite(uAtlasId,strSpriteName);
	}
	
	public void OnUpdateIcon(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		XActionIcon icon = (XActionIcon)args[1];
		LogicUI.SetSprite(icon);
	}
	
	public void OnUpdateIconData(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		uint uAtlasId			= (uint)args[1];
		string strSpriteName	= (string)args[2];
		if(string.IsNullOrEmpty(strSpriteName))
			return ;
		EItem_Quality curQuality = (EItem_Quality)args[3];
		ushort count 			= (ushort)args[4];
		
		LogicUI.SetIconData(uAtlasId,strSpriteName,curQuality,count);
	}
	
	public void OnClearIcon(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		LogicUI.ItemIcon.Reset();
	}
	
	public void OnClearSprite(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		LogicUI.Clear();
	}
	
	public void OnUpdateModel(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
	
		uint modelID 	= (uint)args[0];
		bool isMain		= (bool)args[1];
		
		UpdateUIModel(modelID,isMain);
	}
	
	public void OnClearModel(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return ;
		
		if(m_mainModel != null)
		{
			m_mainModel.Destroy();
			m_mainModel	= null;
		}
		
		if(m_weaponModel != null)
		{
			m_weaponModel.Destroy();
			m_weaponModel	= null;
		}
	}
	
	private void UpdateUIModel(uint modelID,bool isMainPlayer)
	{
		if(LogicUI == null || !LogicUI.gameObject.activeSelf)
			return ;
		
		if(m_mainModel != null)
		{
			m_mainModel.Destroy();
			m_mainModel	= null;
		}
		
		if(m_weaponModel != null)
		{
			m_weaponModel.Destroy();
			m_weaponModel	= null;
		}
			
		
		//显示模型
		if(null == m_mainModel)
		{
			m_mainModel = new XU3dModel("CurSor_Model",modelID);
			m_mainModel.Layer			= GlobalU3dDefine.Layer_UI_2D;
			m_mainModel.Parent			= LogicUI.mSprite.transform;
			m_mainModel.Scale			= new Vector3(12,12,12);
			m_mainModel.LocalPosition	= new Vector3(0,-10,-10);
			m_mainModel.Direction		= new Vector3(0,90,0);
			
			m_mainModel.PlayAnimation(EAnimName.Fight,1.0f,false);

			CharacterController control = m_mainModel.m_gameObject.GetComponentInChildren<CharacterController>();
			if(control == null)
				return ;
			
			control.enabled	= false;
		}

		if(isMainPlayer && null == m_weaponModel)
		{
			m_weaponModel = new XU3dModel("CurSor_Weapon", XLogicWorld.SP.MainPlayer.WeaponItemID);
			m_mainModel.AttachU3dModel(ESkeleton.eWeapon, m_weaponModel, ESkeleton.eMainObject);
		}		
	}
}
