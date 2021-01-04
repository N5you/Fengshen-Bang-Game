using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/ObjectHead/XNpcHead")]
public class XNpcHead : XObjectHead
{
	public UISprite NpcFuncPic;
	
	public override void SetNpcHead (string spriteName)
	{
		if(string.IsNullOrEmpty(spriteName))
		{
			if(NpcFuncPic == null)
				return ;
			NpcFuncPic.atlas = null;
			NpcFuncPic.spriteName = "";
			
			NpcFuncPic.alpha = 0.0f;
		}
		else
		{
			// 新手引导之任务引导
			if ( 2 == KinderIndex )
				XNewPlayerGuideManager.SP.handleMissionGuide(999u);
			
			NpcFuncPic.alpha = 1.0f;
			XUIDynamicAtlas.SP.SetSprite(NpcFuncPic,1201, spriteName, true, null);
		}
	}
}
