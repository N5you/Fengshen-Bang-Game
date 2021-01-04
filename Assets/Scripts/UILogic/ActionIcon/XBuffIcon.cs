using UnityEngine;
using System.Collections;

/* 一种Icon类型, 只要功能需求和buffIcon的功能一样,都可以使用 
 */
[AddComponentMenu("UILogic/Icon/XBuffIcon")]
public class XBuffIcon : MonoBehaviour
{
	public delegate void BuffIconDelegate(uint BuffId);
	
	public uint BuffId;
	public UISprite SpriteBuff;
	public UISprite SpriteFrame;
	public UILabel	LabelLayer;
	public BuffIconDelegate onMouseEnter;
	public BuffIconDelegate onMouseExit;
	public BuffIconDelegate onRightClick;
	
	public void Start()
	{
		SpriteFrame.gameObject.SetActive(false);
		NGUITools.AddWidgetCollider(gameObject);
		// NGUI 暂时没有发送右键点击消息, 用左键替代
		UIEventListener listen = UIEventListener.Get(gameObject);
		listen.onPress += OnRightClick;
		listen.onHover += OnMouseHover;
	}
	
	public void OnMouseHover(GameObject go, bool bHover)
	{
		if(bHover)
		{
			SpriteFrame.gameObject.SetActive(true);
			onMouseEnter(BuffId);
		}
		else
		{
			SpriteFrame.gameObject.SetActive(false);
			onMouseExit(BuffId);
		}
	}
		
	public void OnRightClick(GameObject go, bool isPressed)
	{
		if(isPressed || - 2 != UICamera.currentTouchID)
			return;
		onRightClick(BuffId);
	}
	
	public void SetLayer(byte layer)
	{
		LabelLayer.text = "" + layer;
	}
	
	public void SetSprite(int nAtlasId, string strSpriteName)
	{
		XUIDynamicAtlas.SP.SetSprite(SpriteBuff, nAtlasId, strSpriteName);
	}
}