using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USSetPlaybackRateEvent))]
public class USSetPlaybackRateEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USSetPlaybackRateEvent setPlaybackRateEvent = thisEvent as USSetPlaybackRateEvent;

		if (!setPlaybackRateEvent)
			Debug.LogWarning("Trying to render an event as a setPlaybackRateEvent, but it is a : " + thisEvent.GetType().ToString());

		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);
			if(setPlaybackRateEvent)
			{
				GUILayout.Label("Set Playback Rate for : " + (setPlaybackRateEvent.sequence?setPlaybackRateEvent.sequence.name:"null"), defaultBackground);			
				GUILayout.Label(setPlaybackRateEvent.sequence?setPlaybackRateEvent.sequence.name:"null", defaultBackground);
			}
			if (setPlaybackRateEvent)
				GUILayout.Label("Playback Rate : " + setPlaybackRateEvent.playbackRate, defaultBackground);
		GUILayout.EndArea();

		return myArea;
	}
}
