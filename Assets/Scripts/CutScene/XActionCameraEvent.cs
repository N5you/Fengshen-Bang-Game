using UnityEngine;
using System.Collections;

[USequencerEvent("37Game/Active Camera")]
[USequencerFriendlyName("Active Camera")]
public class XActionCameraEvent : USEventBase {
	
	public Camera m_ActiveCamera = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void FireEvent()
	{
		if(null == m_ActiveCamera )
		{
			Debug.Log( "there is null active camera" );
			return;
		}
		
		if(XCutSceneMgr.SP.IsPlayEnd)
		{
			Debug.Log( "there is cut scene already finish" );
			return ;
		}
		
		activeCamera();
	}
	
	private void activeCamera()
	{
		for( int i=0;i<Camera.allCameras.Length;i++ )
		{
			if("MainCamera" == Camera.allCameras[i].gameObject.tag)
			{
				if(Camera.allCameras[i].gameObject.activeSelf)
				{
					Camera.allCameras[i].gameObject.SetActive(false);
				}
			}
		}
		m_ActiveCamera.gameObject.SetActive(true);
	}
	
	public override void ProcessEvent( float deltaTime )
	{
		
	}
	
}
