using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USApplyForceEvent))]
[CustomEditor(typeof(USApplyForceEvent))]
public class USApplyForceEventEditor : USEventBaseEditor
{
	private float HandleLength = 1.2f;
	private float HandleSize = 0.2f;
	
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USApplyForceEvent forceEvent = thisEvent as USApplyForceEvent;

		if (!forceEvent)
			Debug.LogWarning("Trying to render an event as a USApplyForceEvent, but it is a : " + thisEvent.GetType().ToString());
		
		DrawDefaultBox(myArea, thisEvent);
		
		GUILayout.BeginArea(myArea);
			GUILayout.Label(GetReadableEventName(thisEvent), defaultBackground);
			if(forceEvent)
			{
				GUILayout.Label(forceEvent.type.ToString(), defaultBackground);
				GUILayout.Label("Strength : " + forceEvent.strength, defaultBackground);
			}
		GUILayout.EndArea();

		return myArea;
	}
	
	void OnSceneGUI()
	{
		USApplyForceEvent forceEvent = target as USApplyForceEvent;

		if (!forceEvent)
			Debug.LogWarning("Trying to render an event as a USApplyForceEvent, but it is a : " + forceEvent.GetType().ToString());
		
		if(forceEvent.AffectedObject)
			forceEvent.transform.position = forceEvent.AffectedObject.transform.position;
		
        Undo.SetSnapshotTarget(forceEvent, "Change Force Event Direction");
		
		Vector3 vPosition 	= forceEvent.transform.position;
		
        float width		 	= HandleUtility.GetHandleSize(vPosition) * HandleLength;
		Vector3 vEnd	 	= vPosition + (forceEvent.direction * width);
		
        width 				= HandleUtility.GetHandleSize(vEnd) * HandleSize;
        vEnd 				= Handles.FreeMoveHandle(vEnd, Quaternion.identity, width, Vector3.zero, Handles.CubeCap);
		
		Vector3 vDifference = vEnd - vPosition;
		vDifference.Normalize();
		forceEvent.direction = vDifference;
		
		Handles.color = Color.red;
		Handles.DrawLine(vPosition, vEnd);
        
		if (GUI.changed)
        	EditorUtility.SetDirty (forceEvent);
	}
}
