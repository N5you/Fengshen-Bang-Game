using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USMatchObjectEvent))]
public class USMatchObjectEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USMatchObjectEvent matchObjectEvent = thisEvent as USMatchObjectEvent;

		if (!matchObjectEvent)
			Debug.LogWarning("Trying to render an event as a USMatchObjectEvent, but it is a : " + thisEvent.GetType().ToString());
		
		thisEvent.Duration = matchObjectEvent.inCurve[matchObjectEvent.inCurve.length-1].time;
		
		// Draw our Whole Box.
		if (thisEvent.Duration > 0)
		{
			float endPosition = USControl.convertTimeToEventPanePosition(thisEvent.Firetime + thisEvent.Duration);
			myArea.width = endPosition - myArea.x;
		}
		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);
		{
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
			
			if(matchObjectEvent.objectToMatch != null)
				GUILayout.Label(matchObjectEvent.objectToMatch.name, defaultBackground);
			else
				GUILayout.Label("NULL", defaultBackground);
		}
		GUILayout.EndArea();

		return myArea;
	}
}
