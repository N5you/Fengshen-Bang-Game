using UnityEngine;
using System.Collections;
[AddComponentMenu("UILogic/ObjectHead/XPlayerHead")]
public class XPlayerHead : XFightCharHead
{
	public UILabel FactionNameLable;
	public UISprite FactionIconSprite;
	public Vector3 factionNamePosOff;
	public Vector3 factionIconPosOff;
	
	public override bool Init()
	{
		base.Init ();

		if (FactionNameLable != null && FactionIconSprite != null) {
			FactionNameLable.gameObject.transform.position = Name.gameObject.transform.position + factionNamePosOff;
			FactionIconSprite.gameObject.transform.position = Name.gameObject.transform.position + factionIconPosOff;
		}
	
		return true;
	}

	public override void Show()
	{
		base.Show ();
		if (FactionNameLable.text == "")
			SetFactionNameVisible(false);
	}
		
	public void SetFactionName(string str)
	{
		if (str == FactionNameLable.text)
			return;
		FactionNameLable.text = str;
	}

	public void SetFactionIcon(string spriteid)
	{
		if (spriteid == FactionIconSprite.spriteName)
			return;
		FactionIconSprite.spriteName = spriteid;
	}

	public void SetFactionNameVisible(bool isShow)
	{
		if (null == this.FactionNameLable)
			return;
		this.FactionNameLable.gameObject.SetActive (isShow);
		this.FactionIconSprite.gameObject.SetActive (isShow);
	}

}
