//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("NGUI/Tween/ColorEx")]
public class TweenColorEx : UITweener
{
	public Color src = Color.white;
	public Color from = Color.white;
	public Color to = Color.white;
	
	public int	repeat = 3;

	Transform mTrans;
	UIWidget mWidget;
	Material mMat;
	Light mLight;

	/// <summary>
	/// Current color.
	/// </summary>

	public Color color
	{
		get
		{
			if (mWidget != null) return mWidget.color;
			if (mLight != null) return mLight.color;
			if (mMat != null) return mMat.color;
			return Color.black;
		}
		set
		{
			if (mWidget != null) mWidget.color = value;
			if (mMat != null) mMat.color = value;

			if (mLight != null)
			{
				mLight.color = value;
				mLight.enabled = (value.r + value.g + value.b) > 0.01f;
			}
		}
	}
	
	/// <summary>
	/// Find all needed components.
	/// </summary>

	void Awake ()
	{
		mWidget = GetComponentInChildren<UIWidget>();
		if(mWidget)
			src =  mWidget.color;
		Renderer ren = GetComponent<Renderer>();
		if (ren != null) mMat = ren.material;
		mLight = GetComponent<Light>();
	}
	
	
	
	/// <summary>
	/// Interpolate and update the color.
	/// </summary>
	/// 
	

	override protected void OnUpdate(float factor, bool isFinished) 
	{
		if(isFinished)
		{
			color = src;
			return;
		}

		int  index = Mathf.FloorToInt(factor / (1f / repeat));
		if(index % 2 == 0)
			color = to;
		else
			color = from;
		//color = Color.Lerp(from, to, factor);
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenColor Begin (GameObject go, float duration, Color color)
	{
		TweenColor comp = UITweener.Begin<TweenColor>(go, duration);
		comp.from = comp.color;
		comp.to = color;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}