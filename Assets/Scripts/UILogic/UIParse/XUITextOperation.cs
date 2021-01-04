using UnityEngine;

public class XUITextOperation
{
	private bool mIsMouseOnHyperLink;
	private RenderStr mRenderStr;
	private UIWidget mUIWidget;
	private RenderStrTextComponent mPreRSTC;
	public  bool IsInit	{get;private set;}
	
	public XUITextOperation()
	{
		mIsMouseOnHyperLink	= false;
		mRenderStr 	= null;
		mUIWidget	= null;
		mPreRSTC	= null;
		IsInit		= false;
	}
	
	public void Init(RenderStr rs,UIWidget widget)
	{
		mRenderStr	= rs;
		mUIWidget	= widget;
		IsInit		= true;		
	}
	
	private RenderStrTextComponent GetSelComponent()
	{
		Vector3 vec = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
//		if(mUIWidget.pivot != UIWidget.Pivot.BottomLeft)
//		{
//			mUIWidget.pivot	= UIWidget.Pivot.BottomLeft;
//			NGUITools.AddWidgetCollider(mUIWidget.gameObject);
//		}
		
		Vector3 relativePos = mUIWidget.cachedTransform.worldToLocalMatrix.MultiplyPoint3x4(vec);		
		
		RenderStrTextComponent rstc = GetLinkTextComponent(new Vector2(relativePos.x,relativePos.y));
		
		return rstc;
	}
	
	public bool OnMouseMove()
	{
		bool linkStateChanged = false;
		RenderStrTextComponent rstc	= GetSelComponent();
		
		if(!mIsMouseOnHyperLink)
		{
			if(rstc != null)
			{				
				mUIWidget.MarkAsChangedLite();
				CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_Link);
				linkStateChanged	= true;
				mIsMouseOnHyperLink	= true;
				mPreRSTC	= rstc;
				mUIWidget.SendMessage("OnHyperLinkStateChange",linkStateChanged,SendMessageOptions.DontRequireReceiver);
				return true;
			}
		}
		else if(rstc == null)
		{
			CursorMgr.SP.SetCurSor(Cursor_Type.Cursor_Type_None);
			mIsMouseOnHyperLink	= false;
			mPreRSTC.HyperLinkColor = Color.white;
			mUIWidget.SendMessage("OnHyperLinkStateChange",linkStateChanged,SendMessageOptions.DontRequireReceiver);
			return true;
		}
		
		return false;
	}
	
	public void OnClick()
	{
		RenderStrTextComponent rstc	= GetSelComponent();
		if(mUIWidget != null && rstc != null)
		{
			mUIWidget.SendMessage("OnClickHyperLink",rstc.HyperLinkData,SendMessageOptions.DontRequireReceiver);
		}
	}
	
	RenderStrTextComponent GetLinkTextComponent(Vector2 mousePos)
	{
		RenderStrTextComponent Result = mRenderStr.GetLinkTextComponent(mousePos);
		return Result;
	}
	
	public void ReSet()
	{
		mIsMouseOnHyperLink	= false;
		mPreRSTC	= null;
		IsInit		= false;
	}
}