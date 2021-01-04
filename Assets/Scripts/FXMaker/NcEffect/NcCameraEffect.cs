using UnityEngine;
using System.Collections;

public class NcCameraEffect : NcEffectBehaviour
{	
	private NcCurveAnimation m_Curve;
	private Vector3 m_CameraPostion = Vector3.zero;
	private Vector3 m_CameraRotation = Vector3.zero;
	private Vector3 m_CameraScale = Vector3.zero;
	
	public override int GetAnimationState()
	{
		if(null == m_Curve) return -1;
		return m_Curve.GetAnimationState();
	}
	
	void Awake()
	{
		Camera mainCamera = Camera.main;
		if(null == mainCamera) return;
		// 保存一下摄像机的初始状态
		m_CameraPostion = mainCamera.transform.localPosition;
		m_CameraScale = mainCamera.transform.localScale;
		m_CameraRotation = mainCamera.transform.localRotation.eulerAngles;
		
		NcCurveAnimation curve = GetComponent<NcCurveAnimation>();
		if(null != curve)
		{
			m_Curve = mainCamera.gameObject.AddComponent<NcCurveAnimation>();
			curve.CopyTo(m_Curve, false);
			m_Curve.m_bLoop = false;
			m_Curve.m_bAutoDestruct = false;
		}
	}
	
	void OnDestroy()
	{
		if(null != m_Curve) 
		{
			Destroy(m_Curve);
			Camera mainCamera = Camera.main;
			if(null != mainCamera)
			{
				// 还原摄像机状态
				mainCamera.transform.localPosition = m_CameraPostion;
				mainCamera.transform.localScale = m_CameraScale;
				mainCamera.transform.localRotation = Quaternion.Euler(m_CameraRotation);
			}
		}
	}
}

