using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USWarpToObject))] 
public class USWarpToObjectEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USWarpToObject warpEvent = thisEvent as USWarpToObject;

		if (!warpEvent)
			Debug.LogWarning("Trying to render an event as a USWarpToObject, but it is a : " + thisEvent.GetType().ToString());
		
		myArea.width += 10.0f;
		DrawDefaultBox(myArea, thisEvent);
		
		GUILayout.BeginArea(myArea);
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
			if (warpEvent)
			{
				GUILayout.Label(warpEvent.objectToWarpTo?warpEvent.objectToWarpTo.name:"null", defaultBackground);
				GUILayout.Label(warpEvent.useObjectRotation?"Using Warp Rotation":"Keep Original Rotation", defaultBackground);
			}
		GUILayout.EndArea();

		return myArea;
	}
}
