using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USPauseResumeAudioEvent))]
public class USPauseResumeAudioEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USPauseResumeAudioEvent audioEvent = thisEvent as USPauseResumeAudioEvent;

		if (!audioEvent)
			Debug.LogWarning("Trying to render an event as a USPauseResumeAudioEvent, but it is a : " + thisEvent.GetType().ToString());

		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);
			if (audioEvent)
				GUILayout.Label(audioEvent.pause?"Pause Audio":"Resume Audio", defaultBackground);
		GUILayout.EndArea();

		return myArea;
	}
}
