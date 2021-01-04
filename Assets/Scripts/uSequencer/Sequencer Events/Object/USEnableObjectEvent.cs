using UnityEngine;
using System.Collections;

[USequencerFriendlyName("Toggle Object")]
[USequencerEvent("Object/Toggle Object")]
public class USEnableObjectEvent : USEventBase
{
    public bool enable = false;
	private bool prevEnable = false;
	
#if (UNITY_4_0 || UNITY_4_1)
#else
	public bool enableRecursively = true;
#endif
	
	public override void FireEvent()
	{
#if (UNITY_4_0 || UNITY_4_1)
		prevEnable = AffectedObject.activeSelf;
#else
		prevEnable = AffectedObject.active;
#endif
		
#if (UNITY_4_0 || UNITY_4_1)
		AffectedObject.SetActive(enable);
#else
		if(enableRecursively)
	        AffectedObject.SetActiveRecursively(enable);
		else
			AffectedObject.active = enable;
#endif
	}
	
	public override void ProcessEvent(float deltaTime)
	{
		
	}
	
	public override void StopEvent()
	{
		UndoEvent();
	}
	
	public override void UndoEvent()
	{
		if(!AffectedObject)
			return;
		
#if (UNITY_4_0 || UNITY_4_1)
		AffectedObject.SetActive(prevEnable);
#else
		if(enableRecursively)
	        AffectedObject.SetActiveRecursively(prevEnable);
		else
			AffectedObject.active = prevEnable;
#endif
	}
}
