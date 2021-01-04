using System;
using UnityEngine;
using System.Collections;

public class XActionIcon : MonoBehaviour {
	
	public static readonly string[] colorValueName	= new string[(int)EItem_Quality.EITEM_QUALITY_NUM];
	private static readonly uint EffectID = 900040;
	public float DragUIAlpha = 100.0f/255;
	public float DefaultAlpha;
		
	public UISprite 	ItemIcon;
	public UISprite 	IconBK;	
	public UISprite		IconBoard;
	public UILabel		IconNum;
	private string		UnOpenPicName = "11000102";

	public uint			AtlasID;	
	private	string 		OriginalSpriteName	= "";
	private UIAtlas		OriginalAtlas;
	
	private string		OriginalBKName;
	public EItem_Quality		CurQuality = EItem_Quality.EITEM_QUALITY_INVALID;
	public  ushort		CurCount;
	private	XU3dEffect	mEffect;
	
	public static void InitColor()
	{
//		colorValue[0]	= new Color32(255,255,255,255);
//		colorValue[1]	= new Color32(36,139,1,255);
//		colorValue[2]	= new Color32(17,171,242,255);
//		colorValue[3]	= new Color32(153,101,202,255);
//		colorValue[4]	= new Color32(242,154,75,255);
//		colorValue[5]	= new Color32(255,243,55,255);
		
		colorValueName[0]	= "11000161";
		colorValueName[1]	= "11000156";
		colorValueName[2]	= "11000157";
		colorValueName[3]	= "11000158";
		colorValueName[4]	= "11000159";
		colorValueName[5]	= "11000160";			
	}
	
	public void EnableEffect(bool isEnable)
	{
		if(isEnable)
		{
			if(mEffect == null)
			{
				mEffect	= new XU3dEffect(EffectID,OnEffectLoaded);
			}
			else
			{
				mEffect.Visible	= true;
			}
		}
		else
		{
			if(mEffect == null)
				return ;
			mEffect.Visible	= false;
		}
	}
	
	public  void OnEffectLoaded(XU3dEffect self)
	{
		self.Layer	= GlobalU3dDefine.Layer_UI_2D;
		self.Parent	= gameObject.transform;
		self.LocalPosition	= new Vector3(0,0,-50);
		self.Scale	= new Vector3(450,450,1);
	}
	
	public void ShowUnOpen(bool isShow)
	{
		if(!isShow)
			IconBK.spriteName	= UnOpenPicName;
		else
			IconBK.spriteName	= OriginalBKName;
	}
	
	public bool IsChange()
	{
		return ItemIcon.spriteName != OriginalSpriteName;
	}
	
	public void OnSetSpriteDone()
	{
		NGUITools.AddWidgetCollider(gameObject, true);
	}
	
	public void SetSpriteByID(uint itemID)
	{
		XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(itemID);
		
		SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,1);
	}
	
	public void SetSprite(uint uAtlasId, string strSpriteName,EItem_Quality quality,ushort num)
	{
		SetSprite(uAtlasId,strSpriteName,quality,num,true,1);
	}
	
	public void SetSprite(uint uAtlasId, string strSpriteName,EItem_Quality quality,ushort num,bool isAddCollider,ushort strengthenLevel)
	{
		if(null == ItemIcon) return;
		AtlasID = uAtlasId;
		if(isAddCollider)
			XUIDynamicAtlas.SP.SetSprite(ItemIcon, (int)uAtlasId, strSpriteName, true,OnSetSpriteDone);	
		else
			XUIDynamicAtlas.SP.SetSprite(ItemIcon, (int)uAtlasId, strSpriteName, true,null);
			
		DragReset();
		
		ItemIcon.color = new Color(255, 255, 255, 255);
		
		if(IconBK != null)
		{
			IconBK.spriteName	= "11000161";			
		}
		CurCount	= num;
		if(num > 1)
		{
			if(IconNum != null)
				IconNum.text = Convert.ToString(num);
		}
		else
		{
			if(IconNum != null)
				IconNum.text = "";
		}
		if(quality != EItem_Quality.EITEM_QUALITY_INVALID && quality < EItem_Quality.EITEM_QUALITY_NUM)
		{
			CurQuality	= quality;
			IconBoard.gameObject.SetActive(true);
			IconBoard.spriteName	= colorValueName[(int)quality - 1];
		}
		
		if(strengthenLevel > 1)
		{
			EnableEffect(true);
		}
		else
		{
			EnableEffect(false);
		}
	}
	
	public void SetSprite(UIAtlas atlas, string strSpriteName)
	{
		if(null == ItemIcon) return;
		ItemIcon.atlas = atlas;
		ItemIcon.spriteName = strSpriteName;
	}
	
	void Awake() 
	{	
		//OriginalSpriteName 	= ItemIcon.spriteName;
		if(IconBK != null)
		{
			OriginalBKName		= IconBK.spriteName;
			DefaultAlpha		= IconBK.alpha;
		}
		
		OriginalAtlas		= ItemIcon.atlas;
	}
	
	public void DragShow()
	{
		if(ItemIcon != null)
			ItemIcon.alpha	= DragUIAlpha;
		if(IconBK != null)
			IconBK.alpha	= DragUIAlpha;
		if(IconBoard != null)
			IconBoard.alpha	= DragUIAlpha;
		if(IconNum != null)
			IconNum.alpha	= DragUIAlpha;
		
	}
	
	public void DragReset()
	{
		if(ItemIcon != null)
			ItemIcon.alpha	= DefaultAlpha;
		if(IconBK != null)
			IconBK.alpha	= DefaultAlpha;
		if(IconBoard != null)
			IconBoard.alpha	= DefaultAlpha;
		if(IconNum != null)
			IconNum.alpha	= DefaultAlpha;
	}
	
	public void HoverShow(bool state)
	{
		if(IconBK.spriteName != OriginalBKName)
			return ;
		
		if(state)
			IconBK.alpha	= 1.0f;
		else
			IconBK.alpha	= DefaultAlpha;
	}
	
	public void Reset()
	{
		ItemIcon.atlas		= null;
		ItemIcon.spriteName = OriginalSpriteName;
		
		IconBoard.spriteName	= colorValueName[0];
		if(IconBK != null)
		{
			if(IconBK.spriteName == UnOpenPicName)
			{
				IconBK.alpha		= DefaultAlpha;
			}
			else
			{
				IconBK.spriteName	= OriginalBKName;
				IconBK.alpha		= DefaultAlpha;
			}
		}
		if(IconNum != null)
			IconNum.text		= "";
		IconBoard.gameObject.SetActive(false);
		EnableEffect(false);
	}
}
