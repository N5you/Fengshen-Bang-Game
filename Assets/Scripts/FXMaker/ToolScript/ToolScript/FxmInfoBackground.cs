// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("FXMaker/ToolScript/FxmInfoBackground")]
public class FxmInfoBackground : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public enum SETTING_TYPE		{ ModelPrefab=0, GridPrefab, LightPrefab, CameraPrefab, Count };

	public		GameObject[]	m_ChildObjects			= new GameObject[FXMakerBackground.m_CloneTypeCount+FXMakerBackground.m_ReferenceTypeCount];
	public		string[]		m_CloneThumbFilenames	= new string[FXMakerBackground.m_CloneTypeCount];
	public		GameObject[]	m_ReferenceObjects		= new GameObject[FXMakerBackground.m_ReferenceTypeCount];		// 	m_ModelPrefab1, m_ModelPrefab2, 

	//	[HideInInspector]

	// Property -------------------------------------------------------------------------
	public void SetActive()
	{
		gameObject.SetActive(true);
	}

	public void SetPingObject(int nChildIndex)
	{
		if (nChildIndex < FXMakerBackground.m_CloneTypeCount)
			FXMakerAsset.SetPingObject(m_ChildObjects[nChildIndex]);
		else FXMakerAsset.SetPingObject(m_ReferenceObjects[nChildIndex - FXMakerBackground.m_CloneTypeCount]);
	}

	public GameObject GetChildObject(int nChildIndex)
	{
		return m_ChildObjects[nChildIndex];
	}

	public string GetClildThumbFilename(int nChildIndex)
	{
		if (nChildIndex < FXMakerBackground.m_CloneTypeCount)
			return m_CloneThumbFilenames[nChildIndex];
		return NgAsset.GetPrefabThumbFilename(m_ReferenceObjects[nChildIndex-FXMakerBackground.m_CloneTypeCount]);
	}

	public GameObject GetReferenceObject(int nRefIndex)
	{
		return m_ReferenceObjects[nRefIndex];
	}

	public void ShowBackground(bool bShow)
	{
		for (int nIndex=0; nIndex < m_ReferenceObjects.Length; nIndex++)
		{
			int nChildIndex = FXMakerBackground.m_CloneTypeCount + nIndex;

			// 扁粮 巴 昏力
			if (m_ChildObjects[nChildIndex] != null)
			{
				Object.DestroyImmediate(m_ChildObjects[nChildIndex]);
				m_ChildObjects[nChildIndex]	= null;
			}

			if (bShow && m_ReferenceObjects[nIndex] != null)
			{
				m_ChildObjects[nChildIndex] = NgAsset.LoadPrefab(m_ReferenceObjects[nIndex], gameObject);
				m_ChildObjects[nChildIndex].name = m_ChildObjects[nChildIndex].name.Replace("(Clone)", "");
	 			m_ChildObjects[nChildIndex].SetActive(true);
			}
		}
	}

	public void SetCloneObject(int nCloneIndex, GameObject prefab)
	{
		// 扁粮 巴 昏力
		if (m_ChildObjects[nCloneIndex] != null)
		{
			Object.DestroyImmediate(m_ChildObjects[nCloneIndex]);
			m_ChildObjects[nCloneIndex]			= null;
			m_CloneThumbFilenames[nCloneIndex]	= "";
		}

		// 货肺款 瞒老靛 殿废
		if (prefab != null)
		{
			m_ChildObjects[nCloneIndex] = NgAsset.LoadPrefab(prefab, gameObject);
 			m_ChildObjects[nCloneIndex].SetActive(true);
			m_ChildObjects[nCloneIndex].name = m_ChildObjects[nCloneIndex].name.Replace("(Clone)", "");

			// 荤侩等 prefab苞 GameObject甫 急琶茄促.
			FXMakerAsset.SetPingObject(prefab, m_ChildObjects[nCloneIndex]);

			// thumb 免仿阑 困秦辑 盔夯prefab狼 guid甫 历厘秦 敌促.
			m_CloneThumbFilenames[nCloneIndex] = NgAsset.GetPrefabThumbFilename(prefab);
		}
		FXMakerBackground.inst.SaveBackgroundPrefab();
	}

	public void SetReferenceObject(int nRefIndex, GameObject prefab)
	{
		m_ReferenceObjects[nRefIndex]	= prefab;
		ShowBackground(true);
		FXMakerBackground.inst.SaveBackgroundPrefab();
	}

	// Control --------------------------------------------------------------------------

	// UpdateLoop -----------------------------------------------------------------------
	void Awake()
	{
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void FixedUpdate()
	{
	}

	public void LateUpdate()
	{
	}

	// Event -------------------------------------------------------------------------
	void OnDrawGizmos()
	{
	}

	// Function ----------------------------------------------------------------------
}
#endif
