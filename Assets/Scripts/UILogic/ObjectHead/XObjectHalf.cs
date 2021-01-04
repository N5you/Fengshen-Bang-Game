using UnityEngine;
using System.Collections;

public enum EObjectHalfHintType
{
	eHalfHint_Up = 0,
	eHalfHint_Down,
	eHalfHint_Count,
}

// 挂在角色的腰部, 可以用来提示一些信息
[AddComponentMenu("UILogic/ObjectHead/XObjectHalf")]
public class XObjectHalf : XUIBaseLogic
{
	public UILabel[] HalfHint;
	public Vector3 originalScale = Vector3.zero;
	
	public override bool Init ()
	{
		base.Init ();
		originalScale = transform.localScale;
		return true;
	}
	
	public void FlyHalfHint(EObjectHalfHintType ht, string str)
	{
		if(ht >= EObjectHalfHintType.eHalfHint_Count)
			return;
		
		UILabel label = XUtil.Instantiate<UILabel>(HalfHint[(int)ht]);
		label.text = str;
		NcCurveAnimation cur = label.GetComponent<NcCurveAnimation>();
		if(null != cur) cur.enabled = true;
	}
	
	public void SetScale(float f)
	{
		transform.localScale = originalScale * f;
	}
}

