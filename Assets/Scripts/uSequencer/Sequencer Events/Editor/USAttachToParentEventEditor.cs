using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USAttachToParentEvent))]
public class USAttachToParentEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USAttachToParentEvent attachEvent = thisEvent as USAttachToParentEvent;

		if (!attachEvent)
			Debug.LogWarning("Trying to render an event as a USAttachToParent, but it is a : " + thisEvent.GetType().ToString());

		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);	
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
			if (attachEvent)
				GUILayout.Label(attachEvent.parentObject?attachEvent.parentObject.name:"null", defaultBackground);
		GUILayout.EndArea();

		return myArea;
	}
}
