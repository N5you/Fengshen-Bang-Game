using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USFadeScreenEvent))]
public class USFadeScreenEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USFadeScreenEvent FadeScreenEvent = thisEvent as USFadeScreenEvent;

		if (!FadeScreenEvent)
			Debug.LogWarning("Trying to render an event as a USFadeScreenEvent, but it is a : " + thisEvent.GetType().ToString());
		
		float endPosition = USControl.convertTimeToEventPanePosition(thisEvent.Firetime + (thisEvent.Duration<=0.0f?3.0f:thisEvent.Duration));
		myArea.width = endPosition - myArea.x;
		
		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);	
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
		GUILayout.EndArea();

		return myArea;
	}
}
