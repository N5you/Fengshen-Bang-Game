using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USEnableObjectEvent))]
public class USEnableObjectEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USEnableObjectEvent toggleEvent = thisEvent as USEnableObjectEvent;

		if (!toggleEvent)
			Debug.LogWarning("Trying to render an event as a USEnableObjectEvent, but it is a : " + thisEvent.GetType().ToString());

		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);
			if (toggleEvent)
			{
				GUILayout.Label(toggleEvent.enable?"Enable : ":"Disable : ", defaultBackground);
				GUILayout.Label(toggleEvent.AffectedObject.name, defaultBackground);
			}
		GUILayout.EndArea();

		return myArea;
	}
}
