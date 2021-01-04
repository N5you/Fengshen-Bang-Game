
//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Selectable sprite that follows the mouse.
/// </summary>

//[RequireComponent(typeof(UISprite))]
[AddComponentMenu("UILogic/XCursor")]
public class XCursor : XUIBaseLogic
{
	//Camera used to draw this cursor
	public Camera uiCamera;

	Transform mTrans;
	public UISprite mSprite;
	public XActionIcon ItemIcon;

	UIAtlas mAtlas;
	string mSpriteName;
	
	/// <summary>
	/// Cache the expected components and starting values.
	/// </summary>

	public override bool Init()
	{		
		mTrans = transform;
		//mSprite = GetComponentInChildren<UISprite>();
		mAtlas = mSprite.atlas;
		mSpriteName = mSprite.spriteName;
		mSprite.depth = 100;
		if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);	
		
		return base.Init();
	}

	/// <summary>
	/// Reposition the sprite.
	/// </summary>

	void Update ()
	{
		if (mSprite.atlas != null)
		{
			Vector3 pos = Input.mousePosition;

			if (uiCamera != null)
			{
				// Since the screen can be of different than expected size, we want to convert
				// mouse coordinates to view space, then convert that to world position.
				pos.x = Mathf.Clamp01(pos.x / Screen.width);
				pos.y = Mathf.Clamp01(pos.y / Screen.height);
				mTrans.position = uiCamera.ViewportToWorldPoint(pos);
				Vector3 temp = mTrans.position;
				temp.z += -1000;
				mTrans.position = temp;

				// For pixel-perfect results
				if (uiCamera.orthographic)
				{
					mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mTrans.localPosition, mTrans.localScale);
				}
			}
			else
			{
				// Simple calculation that assumes that the camera is of fixed size
				pos.x -= Screen.width * 0.5f;
				pos.y -= Screen.height * 0.5f;
				mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(pos, mTrans.localScale);
			}
		}
	}

	/// <summary>
	/// Clear the cursor back to its original value.
	/// </summary>

	public void Clear ()
	{
		mSprite.atlas = mAtlas;
		mSprite.spriteName = mSpriteName;
		mSprite.MakePixelPerfect();
		Update();
	}

	/// <summary>
	/// Override the cursor with the specified sprite.
	/// </summary>	
	public void SetSprite(uint uAtlasId, string strSpriteName)
	{		
		XUIDynamicAtlas.SP.SetSprite(mSprite, (int)uAtlasId, strSpriteName);
		mSprite.MakePixelPerfect();
		Update();
	}
	
	public void SetSprite(XActionIcon icon)
	{		
		if(ItemIcon == null)
			return ;
		ItemIcon.gameObject.SetActive(true);
		ItemIcon.SetSprite(icon.AtlasID,icon.ItemIcon.spriteName,icon.CurQuality,icon.CurCount);
		Update();
	}
	
	public void SetIconData(uint uAtlasId,string spriteName,EItem_Quality quality,ushort count)
	{
		if(ItemIcon == null)
			return ;
		ItemIcon.gameObject.SetActive(true);
		ItemIcon.SetSprite(uAtlasId,spriteName,quality,count);
		Update();
	}	
	
	void OnDrop(GameObject go)
	{
		if (XDragMgr.SP.IsDraging)
		{
			XDragMgr.SP.IsDraging = false;
			Clear();
		}	
	}
}