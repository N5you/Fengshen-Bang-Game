using UnityEngine;
using System.Collections;

public class XAreaActive : MonoBehaviour {
	public GameObject m_targetObj = null;
	
	// Use this for initialization
	void Start () {
		if(null == m_targetObj )
			return;
		m_targetObj.SetActive(false );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(null == m_targetObj || null == UICamera.mainCamera )
			return;
		
		Vector3 v = UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
		Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(transform);
		if ( bounds.min.x < v.x && bounds.max.x > v.x &&
			bounds.min.y < v.y && bounds.max.y > v.y)
		{
			m_targetObj.SetActive(true);
		}
		else
		{
			m_targetObj.SetActive(false);
		}
	}
	
}
