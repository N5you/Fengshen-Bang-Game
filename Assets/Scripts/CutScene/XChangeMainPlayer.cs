using UnityEngine;
using System.Collections;

[USequencerEvent("37Game/useMainPlayer")]
[USequencerFriendlyName("MainPlayer")]
public class XChangeMainPlayer : USEventBase {
	public bool m_bMainPlayerShow = true;
	private XU3dModel m_mainPlayer = null;
	private XU3dModel m_weapon = null;
	
	private uint m_ModelIdCacheID = 0;
	private uint m_WeaponModelIdCache = 0;
	
	private bool m_bMainPlayerDone = false;
	private bool m_bWeaponDone = false;
	public override void FireEvent()
	{
		if(!XCutSceneMgr.SP.isStarted )
		{
			TimelineContainer.AffectedObject.gameObject.SetActive(m_bMainPlayerShow );
			return;
		}
		
		if(null != XCutSceneMgr.SP.mainPlayer )
		{
			m_bMainPlayerDone = true;
			m_bWeaponDone = true;
			
			this.TimelineContainer.AffectedObject = XCutSceneMgr.SP.mainPlayer.transform;
			XCutSceneMgr.SP.mainPlayer.SetActive(m_bMainPlayerShow );
		}else
		{
			m_mainPlayer = new XU3dModel("cutScene_mainPlayer",XLogicWorld.SP.MainPlayer.ModelId,mainPlayerDone);
			
			m_weapon = new XU3dModel("cutScene_weapon",XLogicWorld.SP.MainPlayer.WeaponModelId,weaponDone);
		}
		
	}
	
	public override void ProcessEvent( float deltaTime )
	{
		
	}
	
	private void mainPlayerDone(XU3dModel self)
	{	
		m_mainPlayer = self;
		m_bMainPlayerDone = true;
		
		Animation[] animations = self.m_gameObject.GetComponentsInChildren<Animation>(true);
		
		GameObject gameMainPlayer = animations[0].gameObject;
		
		gameMainPlayer.transform.parent = this.TimelineContainer.AffectedObject.transform.parent;
		gameMainPlayer.transform.position = this.TimelineContainer.AffectedObject.transform.position;
		gameMainPlayer.transform.rotation = this.TimelineContainer.AffectedObject.transform.rotation;
		
		this.TimelineContainer.AffectedObject.gameObject.SetActive(false);
		
		this.TimelineContainer.AffectedObject = gameMainPlayer.transform;
		
		XCutSceneMgr.SP.mainPlayer = gameMainPlayer;
		XCutSceneMgr.SP.mainPlayer.SetActive(m_bMainPlayerShow );
		
		attWeapon();
	}
	
	private void weaponDone(XU3dModel self)	
	{
		m_weapon = self;
		m_bWeaponDone = true;
		attWeapon();
	}
	
	private void attWeapon()
	{
		if(m_bWeaponDone&&m_bMainPlayerDone)
		{
			m_mainPlayer.AttachU3dModel(ESkeleton.eWeapon,m_weapon,ESkeleton.eMainObject);
		}
		
	}
	
}
