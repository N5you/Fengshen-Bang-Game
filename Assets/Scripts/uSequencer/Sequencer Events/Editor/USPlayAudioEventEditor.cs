using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USPlayAudioEvent))]
public class USPlayAudioEventEditor : USEventBaseEditor
{
	Texture2D audioWaveTexture = null;
	
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USPlayAudioEvent audioEvent = thisEvent as USPlayAudioEvent;

		if (!audioEvent)
			Debug.LogWarning("Trying to render an event as a USPlayAudioEvent, but it is a : " + thisEvent.GetType().ToString());
		
		if (thisEvent.Duration > 0)
		{
			float endPosition = USControl.convertTimeToEventPanePosition(thisEvent.Firetime + thisEvent.Duration);
			myArea.width = endPosition - myArea.x;
		}

		if (audioEvent && audioEvent.loop)
		{
			float endPosition = USControl.convertTimeToEventPanePosition(thisEvent.Sequence.Duration);
			myArea.width = endPosition - myArea.x;
		}

		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);
#if (UNITY_4_0 || UNITY_4_1)
		
#else
			if (audioEvent && audioEvent.audioClip)
				audioWaveTexture = AssetPreview.GetAssetPreview(audioEvent.audioClip);
#endif

			if(audioWaveTexture)
			{
				audioWaveTexture.filterMode = FilterMode.Point;
				audioWaveTexture.anisoLevel = 1;
			
				Rect imageArea = new Rect(0.0f, 0.0f, audioWaveTexture.width, myArea.height);
				GUI.DrawTexture(imageArea, audioWaveTexture, ScaleMode.StretchToFill);
			}
		
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
			if (audioEvent)
				GUILayout.Label("Audio : " + audioEvent.audioClip, defaultBackground);
		GUILayout.EndArea();

		return myArea;
	}
}
