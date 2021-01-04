using UnityEngine;

[AddComponentMenu("UILogic/XPassInfo")]
public class XPassInfo : XUIBaseLogic
{
	public UILabel PassName;
	public UILabel PassLeftMonster;
	
	public UISprite ReturnSprite;
	
	public override bool Init()
	{	
       	
		UIEventListener retListen = UIEventListener.Get(ReturnSprite.gameObject);
		retListen.onClick	= RetScene;
		
		return base.Init();
	}
	
	public void RetScene(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.PassInfo_RetScene);
	}
	
}