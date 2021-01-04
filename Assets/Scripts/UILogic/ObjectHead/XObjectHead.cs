using UnityEngine;
using System.Collections;
[AddComponentMenu("UILogic/ObjectHead/XObjectHead")]
public class XObjectHead : XUIBaseLogic
{
	public int KinderIndex = 0;
	public UILabel Name = null;
	public UILabel NickNameLable = null;
	public GameObject GuideObj;
	public Vector3 NamePosion = Vector3.zero;
	public Vector3 NickNamePosOff = Vector3.zero;
	public Vector3 originalScale = Vector3.zero;
	
	public override bool Init()
	{
		base.Init ();
		originalScale = transform.localScale;
		if (Name == null)
			Debug.Log ("NameIsNull");
		if (NickNameLable == null)
			Debug.Log ("NickNameLable is Null");

		Name.gameObject.transform.position = NamePosion;
		NickNameLable.gameObject.transform.position = Name.gameObject.transform.position + NickNamePosOff;
		 
		return true;
	}
	
	public override void Show()
	{
		for (int i = 0, imax = transform.childCount; i < imax; ++i) {
			NGUITools.SetActive (transform.GetChild (i).gameObject, true);
		}
		XEventManager.SP.SendEvent (EEvent.UI_OnShow, this);
	}
	
	public override void Hide()
	{
		for (int i = 0, imax = transform.childCount; i < imax; ++i) {
			NGUITools.SetActive (transform.GetChild (i).gameObject, false);
		}
		XEventManager.SP.SendEvent (EEvent.UI_OnHide, this);
	}
	
	public void SetName(string str)
	{
		Name.text = str;
	}

	public virtual void SetNickName(string str)
	{
		if (str == NickNameLable.text)
			return;
		
		NickNameLable.text = str;
	}
	
	public virtual Vector3 GetRelativePosition()
	{
		return new Vector3 (0f, 0.2f, 0f);
	}
	
	//由于XGameObject和XObjectHead的平行集成, 这里实现一些虚函数, 派生类实现, 好调用
	
	// -> XFightCharHead
	public virtual void FlyString(EFlyStrType ft, string str)
	{
	}

	public virtual void ShowBlood(bool b)
	{
	}

	public virtual void SetHp(int nHp, int nMaxHp)
	{
	}
	
	// -> XPlayerHead
	public virtual void SetFactionName(string str)
	{
	}

	public virtual void SetFactionIcon(string spriteid)
	{
	}
	
	// -> XNpc
	public virtual void SetNpcHead(string spriteName)
	{
	}
	
	// 获取名字的位置和父窗口信息
	public virtual void GetHeadPosInfo(ref Vector3 pos, ref GameObject parent)
	{
		pos = GuideObj.transform.localPosition;
		parent = GuideObj;
	}
	
	public void SetVisible(bool b)
	{ 
		if (b)
			Show ();
		else
			Hide (); 
	}
	
	
	public virtual void SetNickNameVisible(bool isShow)
	{
		if(isShow)
			NickNameLable.gameObject.SetActive(isShow);
		else
			NickNameLable.gameObject.SetActive(isShow);	
	}
	
	public void SetScale(float f)
	{
		transform.localScale = originalScale * f;
	}
}
