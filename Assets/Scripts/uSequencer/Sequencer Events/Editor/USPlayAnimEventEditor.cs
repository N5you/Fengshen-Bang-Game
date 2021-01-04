using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USPlayAnimEvent))]
public class USPlayAnimEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USPlayAnimEvent animEvent = thisEvent as USPlayAnimEvent;

		if(!animEvent)
			Debug.LogWarning("Trying to render an event as a USPlayAnimEvent, but it is a : " + thisEvent.GetType().ToString());

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
