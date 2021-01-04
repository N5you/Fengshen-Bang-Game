using UnityEngine;
using System.Collections;

[AddComponentMenu("FXMaker/NcEffect/NcProjector")]
public class NcProjector : NcEffectBehaviour
{
	private float m_fScaleX = 0.0f;
	private float m_fScaleY = 0.0f;
	
	public void Awake()
	{
		Projector projector = GetComponent<Projector>();
		if(null == projector)
		{
			projector = gameObject.AddComponent<Projector>();
			projector.nearClipPlane = 0.1f;
			projector.farClipPlane = 1.0f;
			projector.aspectRatio = 1.0f;
			projector.orthographic = true;
			projector.orthographicSize = 1.0f;
			projector.ignoreLayers = ~( 1 << GlobalU3dDefine.Layer_Decal );
			transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0f, 0f));
			transform.localPosition = new Vector3(0.0f, 0.5f, 0f);
		}
		m_fScaleX = transform.lossyScale.x;
		m_fScaleY = transform.lossyScale.y;
	}
	
	public void Update()
	{
		if(transform.lossyScale.x == m_fScaleX && transform.lossyScale.y == m_fScaleY)
			return;
		
		Projector projector = GetComponent<Projector>();
		if(null == projector)
			return;
		
		float x = transform.lossyScale.x;
		float y = transform.lossyScale.y;
		
		projector.orthographicSize *= (x > y) ? (x / m_fScaleX) : (y / m_fScaleY);
		projector.aspectRatio = x / y;

		m_fScaleX = x;
		m_fScaleY = y;
	}
	
	public override int GetAnimationState()
	{
		if (enabled == false || gameObject.activeSelf == false)
			return -1;
		
		Projector projector = GetComponent<Projector>();
		if(null == projector || !projector.enabled)
			return -1;
		
		return 1;
	}
}
