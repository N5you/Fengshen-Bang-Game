using UnityEngine;
using System.Collections;

/*
 * 这个类设计用来索引静态UI, 当使用静态UI模式的时候, 不需要重新索引太多的UIPanel
 */ 
[AddComponentMenu("UILogic/XWeStaticUIRef")]
[System.Serializable]
public class XWeStaticUIRef : MonoBehaviour
{
	public GameObject[] Panels = new GameObject[(int)EUIPanel.eCount];
}

