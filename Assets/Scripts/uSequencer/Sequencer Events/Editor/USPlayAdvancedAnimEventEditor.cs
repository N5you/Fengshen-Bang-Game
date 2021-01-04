using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USPlayAdvancedAnimEvent))]
public class USPlayAdvancedAnimEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USPlayAdvancedAnimEvent animEvent = thisEvent as USPlayAdvancedAnimEvent;

		if(!animEvent)
			Debug.LogWarning("Trying to render an event as a USPlayAdvancedAnimEvent, but it is a : " + thisEvent.GetType().ToString());

		float endPosition = USControl.convertTimeToEventPanePosition(thisEvent.Firetime + (thisEvent.Duration<=0.0f?3.0f:thisEvent.Duration));
		myArea.width = endPosition - myArea.x;

		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
			if(animEvent)
				GUILayout.Label("Animation : " + animEvent.animationClip, defaultBackground);
		GUILayout.EndArea();

		return myArea;
	}
}
