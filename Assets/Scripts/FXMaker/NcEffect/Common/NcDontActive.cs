// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

// Attribute ------------------------------------------------------------------------
// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("FXMaker/NcEffect/Common/NcDontActive")]
public class NcDontActive : NcEffectBehaviour
{
	// --------------------------------------------------------------------------
	// --------------------------------------------------------------------------
	void Awake()
	{
		gameObject.SetActive(false);
#if UNITY_EDITOR
		if (IsCreatingEditObject() == false)
#endif
		{
// 			Component[] coms = GetComponents<NcEffectBehaviour>();
// 			foreach (Component com in coms)
// 				Destroy(com);
		}
	}

	void OnEnable()
	{
		gameObject.SetActive(false);
	}

	// --------------------------------------------------------------------------
}
