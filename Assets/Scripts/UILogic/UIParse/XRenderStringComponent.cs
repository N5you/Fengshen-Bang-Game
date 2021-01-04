using UnityEngine;

public class RenderStrComponent
{
	protected string mOriginalText;
	protected UIFont 	mFont;
	public UILabel		Label;
	protected UIWidget.Pivot  mPivot;
	public Vector2 Pos;
	public int type = 0;
	
	public RenderStrComponent(string text,UIFont font)
	{
		mOriginalText	= text;
		mFont			= font;
		Pos = new Vector3(0, 0);
	}
	
	public virtual void Clear()	{}
	
	public string GetText()	{ return mOriginalText; }
	
	public virtual Vector2 GetLocalSize() { return new Vector2(); }	
	public virtual void Draw(Vector2 startPos,BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols) {}
}

public class RenderStrTextComponent : RenderStrComponent
{
	public RenderStrTextComponent(string text,UIFont font) : base(text,font)
	{
		FontColor	= new Color(1.0f,0.0f,0.0f,1.0f);
		OutLineColor= new Color(0.0f,1.0f,0.0f,1.0f);
	}	

	public Color FontColor {get;set;}
	
	public bool IsEnableOutLine {get;set;}
	public Color OutLineColor {get;set;}
	
	public bool IsEnableUnderLine {get;set;}
	public Color UnderLineColor {get;set;}
	
	public bool IsEnableShadow {get;set;}
	public Color ShadowColor {get;set;}
	
	public bool EnCoding {get;set;}
	
	public bool IsEnableHyperLink {get;set;}
	public Color HyperLinkColor {get;set;}
	public string HyperLinkData {get;set;}
	
	public Vector2 EffectDistance = new Vector2(1,1);
	
	public override Vector2 GetLocalSize()
	{
		return mFont.CalculatePrintedSize(mOriginalText,true,UIFont.SymbolStyle.Colored);
	}
	
	public override void Draw(Vector2 startPos,BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if(mFont == null)
			return ;
		
		int startFont = verts.size;
		if(IsEnableHyperLink)
		{
			mFont.PrintParseStr(startPos,mOriginalText,HyperLinkColor,verts,uvs,cols);
		}
		else
		{
			mFont.PrintParseStr(startPos,mOriginalText,FontColor,verts,uvs,cols);
		}
		
		float pixel =  1f / mFont.size;
		float fx = pixel * EffectDistance.x;
		float fy = pixel * EffectDistance.y;
		
		if(IsEnableUnderLine)
		{
			int endFont = startFont + mOriginalText.Length * 4;
			ApplyUnderLine(verts,uvs,cols,startFont,endFont,0.1f);
		}
		
		if(IsEnableShadow)
		{
			int endFont = startFont + mOriginalText.Length * 4;
			ApplyShadow(verts,uvs,cols,startFont,endFont,fx,-fy);
		}
		
		if(IsEnableOutLine)
		{
			int endFont = startFont + mOriginalText.Length * 4;
			ShadowColor	= OutLineColor;
			ApplyOutLine(verts,uvs,cols,startFont,endFont,fx,fy);
		}
	}
	
	//下划线
	void ApplyUnderLine(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end,float height)
	{		
		if(end - 4 < 0)
			return ;
			
		Color c = UnderLineColor;
		//c.a *= alpha * mPanel.alpha;
		Color32 col = (mFont.premultipliedAlpha) ? NGUITools.ApplyPMA(c) : c;
		
		Vector3 posR = new Vector3(verts.buffer[end-4].x,verts.buffer[end-4].y,verts.buffer[end-4].z);
		posR.y = posR.y - 0.2f;
		Vector3 posL = new Vector3(verts.buffer[start+1].x,posR.y - height ,verts.buffer[start+1].z);
		
		//定点
		verts.Add(new Vector3(posR.x, posL.y,posR.z));
		verts.Add(new Vector3(posL.x, posL.y,posL.z));
		verts.Add(new Vector3(posL.x, posR.y,posL.z));
		verts.Add(new Vector3(posR.x, posR.y,posR.z));
		
//		CharacterInfo CharInfo;
//		int mDynamicFontSize = 50;
//		FontStyle mDynamicFontStyle = FontStyle.Normal;
//		
//		if (!mFont.mDynamicFont.GetCharacterInfo('_', out CharInfo, mDynamicFontSize, mDynamicFontStyle))
//			return ;
//		
//		Vector2 u0 = Vector2.zero, u1 = Vector2.zero;
//
//		u0.x = CharInfo.uv.xMin;
//		u0.y = CharInfo.uv.yMin;
//		u1.x = CharInfo.uv.xMax;
//		u1.y = CharInfo.uv.yMax;
//		
//		if (CharInfo.flipped)
//		{
//			uvs.Add(new Vector2(u0.x, u1.y));
//			uvs.Add(new Vector2(u0.x, u0.y));
//			uvs.Add(new Vector2(u1.x, u0.y));
//			uvs.Add(new Vector2(u1.x, u1.y));
//		}
//		else
//		{
//			uvs.Add(new Vector2(u1.x, u0.y));
//			uvs.Add(new Vector2(u0.x, u0.y));
//			uvs.Add(new Vector2(u0.x, u1.y));
//			uvs.Add(new Vector2(u1.x, u1.y));
//		}
		
		Vector2 u0 = Vector2.zero, u1 = Vector2.zero;
		bool isFlipped = false;
		if(mFont.GetCharUV('_',ref u0,ref u1,ref isFlipped))
		{
			if(isFlipped)
			{
				uvs.Add(new Vector2(u0.x, u1.y));
				uvs.Add(new Vector2(u0.x, u0.y));
				uvs.Add(new Vector2(u1.x, u0.y));
				uvs.Add(new Vector2(u1.x, u1.y));
			}
			else
			{
				uvs.Add(new Vector2(u1.x, u0.y));
				uvs.Add(new Vector2(u0.x, u0.y));
				uvs.Add(new Vector2(u0.x, u1.y));
				uvs.Add(new Vector2(u1.x, u1.y));
			}
		}
		else
		{
			uvs.Add(new Vector2(u1.x, u0.y));
			uvs.Add(new Vector2(u0.x, u0.y));
			uvs.Add(new Vector2(u0.x, u1.y));
			uvs.Add(new Vector2(u1.x, u1.y));
		}
		
		//color
		cols.Add (col);
		cols.Add (col);
		cols.Add (col);
		cols.Add (col);
	}
	
	//阴影
	void ApplyShadow (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end, float x, float y)
	{
		//Color c = mEffectColor;
		//c.a *= alpha * mPanel.alpha;
		//Color32 col = (font.premultipliedAlpha) ? NGUITools.ApplyPMA(c) : c;

		for (int i = start; i < end; ++i)
		{
			verts.Add(verts.buffer[i]);
			uvs.Add(uvs.buffer[i]);
			cols.Add(cols.buffer[i]);

			Vector3 v = verts.buffer[i];
			v.x += x;
			v.y += y;
			verts.buffer[i] = v;
			cols.buffer[i] = ShadowColor;
		}
	}
	
	//描边
	void ApplyOutLine(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int offset, int end, float x, float y)
	{
		ApplyShadow(verts, uvs, cols, offset, end, x, -y);
		
		offset  = end;
		end		= verts.size;
		ApplyShadow(verts, uvs, cols, offset, end, -x, y);
		
		offset  = end;
		end		= verts.size;
		ApplyShadow(verts, uvs, cols, offset, end, x, y);
		
		offset  = end;
		end		= verts.size;
		ApplyShadow(verts, uvs, cols, offset, end, -x, -y);
		
	}
}

//UI控件组件
public class RenderStrWidgetComponent : RenderStrComponent
{
	public RenderStrWidgetComponent(string text,UIFont font) : base(text,font)
	{
		
	}
	
	public void SetGameObject(GameObject go)
	{
		//mGo	= go;
	}
	
	//private UISprite mWidget;
	//private GameObject mGo;
}

public class RenderStrSpriteComponent : RenderStrComponent
{
	public RenderStrSpriteComponent(string text,UIFont font) : base(text,font)
	{
		type = 1;
	}
	
	public override void Clear()
	{
		//NGUITools.Destroy(Sprite.gameObject);
		if(Label == null)
			return ;
		
		Label.panel.AddDelWidget(Sprite);
		
	}
	
	public override Vector2 GetLocalSize()
	{
		if(Sprite == null || Label == null)
			return Vector2.zero;
		
		float x = Sprite.cachedTransform.localScale.x / Label.cachedTransform.localScale.x;
		float y = Sprite.cachedTransform.localScale.y / Label.cachedTransform.localScale.y;
		
		return new Vector2(x,y);
	}
	
	public void LoadSprite(int xwidth, int ywidth)
	{
		if(UIRoot.list.Count == 0)
			return ;
		
		Sprite			= NGUITools.AddWidget<UISprite>(Label.gameObject.transform.parent.gameObject);
		Sprite.gameObject.layer	= GlobalU3dDefine.Layer_UI_2D;
		Sprite.layer	= 5;
		Sprite.transform.localPosition	= Label.cachedTransform.localPosition;
		if ( xwidth > 0 && ywidth > 0 )
		{
			Vector3 scale = Sprite.transform.localScale;
			scale.x = xwidth;
			scale.y = ywidth;
			Sprite.transform.localScale = scale;
		}
		XUIDynamicAtlas.SP.SetSprite(Sprite, (int)AtlasID,SpriteName,true,OnSpriteDone);
	}	
	
	public override void Draw(Vector2 startPos,BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if(Sprite == null)
			return ;
		
		mSpritePos	= startPos;
		OnSpriteDone();
	}
	
	public void OnSpriteDone()
	{
		if(Sprite == null)
			return ;
		
		float yDelta = 0;
		float xDelta = 0;
		
		if(Label.pivot == UIWidget.Pivot.Left || Label.pivot == UIWidget.Pivot.Center || Label.pivot == UIWidget.Pivot.Right)
			yDelta = mSpritePos.y * Label.cachedTransform.localScale.y + Sprite.cachedTransform.localScale.y / 2 - Label.cachedTransform.localScale.y / 2;
		else if(Label.pivot == UIWidget.Pivot.Top || Label.pivot == UIWidget.Pivot.TopLeft || Label.pivot == UIWidget.Pivot.TopRight)
		{
			yDelta = -mSpritePos.y * Label.cachedTransform.localScale.y - 2f;
		}
		else
		{
			yDelta = (Label.relativeSize.y - mSpritePos.y) * Label.cachedTransform.localScale.y  + Sprite.cachedTransform.localScale.y / 2 + Label.cachedTransform.localPosition.y * 2;
		}
		
		if ( Label.pivot == UIWidget.Pivot.TopRight )
			xDelta = Label.cachedTransform.localPosition.x - mSpritePos.x * Label.cachedTransform.localScale.x + Sprite.cachedTransform.localScale.x / 2;
		else
			xDelta = Label.cachedTransform.localPosition.x + mSpritePos.x * Label.cachedTransform.localScale.x + Sprite.cachedTransform.localScale.x / 2;
		
		Sprite.cachedTransform.localPosition	= new Vector3(xDelta,
			Label.cachedTransform.localPosition.y + yDelta,
			Label.cachedTransform.localPosition.z);		

	}
	
	public string SpriteName {get;set;}
	public uint   AtlasID	 {get;set;}
	private UISprite	Sprite	= null;
	private Vector2 mSpritePos;
	
	
}