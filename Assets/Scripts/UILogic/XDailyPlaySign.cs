using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XDailyPlaySign")]
public class XDailyPlaySign : XUIBaseLogic
{
	public UILabel[] LabelTestShowObj;
	
	public override bool Init()
	{	
		for ( int i = 0; i < LabelTestShowObj.Length; i++ )
		{
			UIEventListener lis = UIEventListener.Get(LabelTestShowObj[i].gameObject);
			lis.onClickHyperLink	+= ClickName;
		}
		return true;
	}
	
	private void ClickName(GameObject go,string str)
	{
		HyperLinkMgr.SP.Process(str);
	}
	
	public override void Reset()
	{
		for ( int i = 0; i < LabelTestShowObj.Length; i++ )
		{
			LabelTestShowObj[i].text = "";
			LabelTestShowObj[i].gameObject.SetActive(false);
		}
	}
	
	public void SetLabelShowData(int index, string text)
	{
		if ( index < 0 || index > 8 )
			return;
		
		LabelTestShowObj[index].gameObject.SetActive(true);
		LabelTestShowObj[index].text = text;
		
		NGUITools.AddWidgetCollider(LabelTestShowObj[index].gameObject);
	}
}
