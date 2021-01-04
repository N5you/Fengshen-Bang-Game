using UnityEngine;
using System.Collections;

public class XAudio : MonoBehaviour {
	
	public int m_iAudioID = 0;
	
	public bool m_bLoop = false;
	
	private XU3dAudio m_u3dAudio = null;
	// Use this for initialization
	void Start () {
		m_u3dAudio = new XU3dAudio((uint)m_iAudioID,onAudioLoaded );
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	private void onAudioLoaded( XU3dAudio audio )
	{
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		
		audioSource.loop = m_bLoop;
		
		audioSource.clip = audio.audioClip;
		audioSource.Play();
	}
	
}
