using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USLookAtObjectEvent))]
public class USLookAtObjectEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USLookAtObjectEvent lookAtObjectEvent = thisEvent as USLookAtObjectEvent;

		if (!lookAtObjectEvent)
			Debug.LogWarning("Trying to render an event as a USLookAtObjectEvent, but it is a : " + thisEvent.GetType().ToString());
		
		float fadeInStartTime = lookAtObjectEvent.Firetime;
		float fadeInEndTime = lookAtObjectEvent.Firetime + lookAtObjectEvent.inCurve[lookAtObjectEvent.inCurve.length-1].time;
		
		float fadeOutStartTime = fadeInEndTime + lookAtObjectEvent.lookAtTime;
		float fadeOutEndTime = fadeOutStartTime + lookAtObjectEvent.outCurve[lookAtObjectEvent.outCurve.length-1].time;
		
		thisEvent.Duration = fadeOutEndTime - fadeInStartTime;
		
		lookAtObjectEvent.lookAtTime = Mathf.Max(lookAtObjectEvent.lookAtTime, 0.0f);
		
		// Draw our Whole Box.
		if (thisEvent.Duration > 0)
		{
			float endPosition = USControl.convertTimeToEventPanePosition(thisEvent.Firetime + thisEvent.Duration);
			myArea.width = endPosition - myArea.x;
		}
		DrawDefaultBox(myArea, thisEvent);
		
		Rect FadeInBox = myArea;
		// Draw our FadeInBox
		if (thisEvent.Duration > 0)
		{
			float endPosition = USControl.convertTimeToEventPanePosition(fadeInEndTime - fadeInStartTime);
			FadeInBox.width = endPosition;
		}
		DrawDefaultBox(FadeInBox, thisEvent);
		
		Rect FadeOutBox = myArea;
		// Draw our FadeOutBox
		if (thisEvent.Duration > 0)
		{
			float startPosition = USControl.convertTimeToEventPanePosition(fadeOutStartTime);
			float endPosition = USControl.convertTimeToEventPanePosition(fadeOutEndTime);
			
			FadeOutBox.x = startPosition;
			FadeOutBox.width = endPosition - startPosition;
		}
		DrawDefaultBox(FadeOutBox, thisEvent);

		GUILayout.BeginArea(myArea);
		{
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
			
			if(lookAtObjectEvent.objectToLookAt != null)
				GUILayout.Label(lookAtObjectEvent.objectToLookAt.name, defaultBackground);
			else
				GUILayout.Label("NULL", defaultBackground);
		}
		GUILayout.EndArea();

		return myArea;
	}
}
