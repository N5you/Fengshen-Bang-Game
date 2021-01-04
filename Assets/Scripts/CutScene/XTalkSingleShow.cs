using UnityEngine;
using System.Collections;

[USequencerEvent("37Game/TalkSingleShow")]
[USequencerFriendlyName("Talk Single Show")]
[System.Serializable]
public class TalkSingleShow : USEventBase 
{	
	public string Content;
	
	public override void FireEvent()
	{
		XEventManager.SP.SendEvent(EEvent.SingleTalk_Content,Content);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eSingleTalk);
	}
	
	public override void EndEvent()
    {
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eSingleTalk);
    }
	
	public override void ProcessEvent( float deltaTime )
	{
		
	}
}
