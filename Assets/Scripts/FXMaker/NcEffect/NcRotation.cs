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

[AddComponentMenu("FXMaker/NcEffect/NcRotation")]
public class NcRotation : NcEffectBehaviour
{
	public enum RotationMode
	{
		ZiZhuan,			// 自转
		GongZhuan			// 公转
	}
	
	// Attribute ------------------------------------------------------------------------
	public 	RotationMode	m_RotaionMode	= RotationMode.ZiZhuan;
	public 	bool		m_bLoop				= false;
	public	bool		m_bWorldSpace		= false;
	public	Vector3		m_vRotationValue	= new Vector3(0, 360, 0);
	private float		m_fStartTime 		= 0.0f;
	
	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (GetComponent<NcBillboard>() != null)
			return "SCRIPT_CLASH_ROTATEBILL";
		return "";	// no error
	}
#endif
	
	public override int GetAnimationState()
	{
		if(!m_bLoop && Time.time  - m_fStartTime > 1.0f)
		{
			return -1;
		}
		return 1;
	}
	
	// --------------------------------------------------------------------------
	void Start()
	{
		m_fStartTime = Time.time;
	}
	
	void Update()
	{
		if(!m_bLoop && Time.time - m_fStartTime > 1.0f)
			return;
		
		switch(m_RotaionMode)
		{
		case RotationMode.ZiZhuan:
		 	transform.Rotate(Time.deltaTime * m_vRotationValue.x, 
				Time.deltaTime * m_vRotationValue.y, 
				Time.deltaTime * m_vRotationValue.z, 
				(m_bWorldSpace ? Space.World : Space.Self));
			break;
			
		case RotationMode.GongZhuan:
			Vector3 point = Vector3.zero;
			Vector3 x = Vector3.right;
			Vector3 y = Vector3.up;
			Vector3 z = Vector3.forward;
			if(!m_bWorldSpace && null != transform.parent)
			{
				point = transform.parent.position;
				x = transform.parent.TransformDirection(x);
				y = transform.parent.TransformDirection(y);
				z = transform.parent.TransformDirection(z);
			}
			transform.RotateAround(point, x, Time.deltaTime * m_vRotationValue.x);
			transform.RotateAround(point, y, Time.deltaTime * m_vRotationValue.y);
			transform.RotateAround(point, z, Time.deltaTime * m_vRotationValue.z);
			break;
		}
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_vRotationValue		*= fSpeedRate;
	}
}
