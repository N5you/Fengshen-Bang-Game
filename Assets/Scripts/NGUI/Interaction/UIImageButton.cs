//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright ?2011-2013 Tasharen Entertainment
//----------------------------------------------
using UnityEngine;
/// <summary>
/// Sample script showing how easy it is to implement a standard button that swaps sprites.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	public UISprite target;
	public string normalSprite;
	public string hoverSprite;
	public string pressedSprite;
	public string disabledSprite;
	public GameObject TipObject;
	public string CommonTips;
	public int userdata = -1;
	
	// 掩藏回调
	public delegate void OnHide();
	public OnHide onHide;
	
	public bool isEnabled {
		get {
			Collider col = GetComponent<Collider>();
			return col && col.enabled;
		}
		set {
			Collider col = GetComponent<Collider>();
			if (!col)
				return;

			if (col.enabled != value) {
				col.enabled = value;				
			}
		}
	}

	void OnEnable()
	{
		UpdateImage ();		
	}
	
	void OnDisable()
	{
		UpdateImage ();
	}
	
	void Start()
	{
		if (target == null)
			target = GetComponentInChildren<UISprite> ();
		
		if (TipObject != null)
			TipObject.SetActive (false);
	}
	
	void OnHover(bool isOver)
	{
		if (isEnabled && target != null) {
			target.spriteName = isOver ? hoverSprite : normalSprite;
			target.MakePixelPerfect ();
		}
		
		if (isEnabled && TipObject != null) {
			if (TipObject != null && target != null) {
				Vector3 v = new Vector3 (target.transform.localPosition.x, 
					target.transform.localPosition.y,
					TipObject.transform.localPosition.z);	
				TipObject.transform.localPosition = v;
			}
			TipObject.SetActive (isOver);
		}
		
//		if (enabled)
//		{
//			UpdateImage();
//		}
	}

	void OnTooltip(bool isOver)
	{
		//添加按钮的公共TIPS提示
		if (isOver) 
		{
			if (isEnabled && !string.IsNullOrEmpty(CommonTips)) 
			{
				XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eToolTipC);
				XEventManager.SP.SendEvent (EEvent.ToolTip_C, CommonTips);
			}
		}
		else 
		{
			if (isEnabled && !string.IsNullOrEmpty(CommonTips)) 
			{
				XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipC);
			}
		}
	}

	void OnPress(bool pressed)
	{
		if (pressed) {
			target.spriteName = pressedSprite;
			target.MakePixelPerfect ();
		}
		else 
			UpdateImage ();
		
		//UpdateImage();
//			if(target != null)
//			{
//				target.spriteName	= pressedSprite;
//			}
		//}
	}
	
	public void UpdateImage()
	{
		if (target != null) {		
			if (isEnabled)
				target.spriteName = UICamera.IsHighlighted (gameObject) ? hoverSprite : normalSprite;
			else
				target.spriteName = disabledSprite;
				
			target.MakePixelPerfect ();
			
			XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipB);
		}
	}
	public void Hide()
	{
		if (gameObject != null)
		{
			gameObject.SetActive(false);
			if ( onHide != null )
				onHide();
		}
	}
	public void Show()
	{
		if (gameObject != null)
			gameObject.SetActive(true);
	}
	public void SetTipString(string tipString)
	{
		Transform labelForm = TipObject.transform.Find ("Label");
		if (null == labelForm)
			return;
		labelForm.GetComponent<UILabel> ().text = tipString;
		
		Vector2 size = labelForm.GetComponent<UILabel> ().font.CalculatePrintedSize (tipString, true, UIFont.SymbolStyle.Colored);
		Transform sprite = TipObject.transform.Find ("Sprite");
		Vector3 spritesize = sprite.localScale;
		spritesize.x = size.x * labelForm.GetComponent<UILabel> ().transform.localScale.x + 4f;
		sprite.localScale = spritesize;
	}
}