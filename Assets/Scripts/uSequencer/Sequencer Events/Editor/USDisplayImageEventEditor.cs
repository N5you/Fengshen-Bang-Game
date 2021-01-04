using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USDisplayImageEvent))]
public class USDisplayImageEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USDisplayImageEvent DisplayImageEvent = thisEvent as USDisplayImageEvent;

		if (!DisplayImageEvent)
			Debug.LogWarning("Trying to render an event as a USDisplayImageEvent, but it is a : " + thisEvent.GetType().ToString());
		
		float endPosition = USControl.convertTimeToEventPanePosition(thisEvent.Firetime + (thisEvent.Duration<=0.0f?3.0f:thisEvent.Duration));
		myArea.width = endPosition - myArea.x;
		
		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);	
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
		GUILayout.EndArea();

		return myArea;
	}
}
