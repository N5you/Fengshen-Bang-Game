// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("FXMaker/NcEffect/Common/NcDisableDelayActive")]
public class NcDisableDelayActive : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	// Property -------------------------------------------------------------------------
	public static void HideNcDelayActive(GameObject tarObj)
	{
		tarObj.SetActive(false);
		NcDelayActive[]	coms = tarObj.GetComponentsInChildren<NcDelayActive>(true);
		foreach (NcDelayActive com in coms)
			com.CancelDelayActive();
	}
	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
		HideNcDelayActive(gameObject);
	}

	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
}


