using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomUSEditor(typeof(USSpawnPrefabEvent))]
public class USSpawnPrefabEventEditor : USEventBaseEditor
{
	new public Rect RenderEvent(Rect myArea, USEventBase thisEvent)
	{
		USSpawnPrefabEvent spawnEvent = thisEvent as USSpawnPrefabEvent;

		if (!spawnEvent)
			Debug.LogWarning("Trying to render an event as a USSpawnPrefabEvent, but it is a : " + thisEvent.GetType().ToString());

		DrawDefaultBox(myArea, thisEvent);

		GUILayout.BeginArea(myArea);
			GUILayout.Label("Spawn : ", defaultBackground);
			if (spawnEvent)
				GUILayout.Label(spawnEvent.spawnPrefab?spawnEvent.spawnPrefab.name:"null", defaultBackground);
			GUILayout.Label("At : ", defaultBackground);
			GUILayout.Label(spawnEvent.spawnTransform?spawnEvent.spawnTransform.name:"Identity", defaultBackground);
		GUILayout.EndArea();

		return myArea;
	}
}
