using UnityEngine;
using System.Collections;
using XGame.Client.Packets;
using XGame.Client.Base;

[USequencerEvent("37Game/Battle Layer")]
[USequencerFriendlyName("Battle Layer")]
public class XChangeLayer : USEventBase {
	
	public int m_layer = GlobalU3dDefine.Layer_BattleObject;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public override void FireEvent()
	{
		Transform[] transList = this.TimelineContainer.AffectedObject.gameObject.GetComponentsInChildren<Transform>(true);
		
		foreach(Transform t in transList )
		{
			t.gameObject.layer = m_layer;
		}
	}
	
	private void OnEffectLoaded(XU3dEffect self)
	{
		
	}
	
	public override void ProcessEvent( float deltaTime )
	{
		
	}
	
}

