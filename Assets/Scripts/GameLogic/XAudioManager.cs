using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using XGame.Client.Base.Map;

public class XAudioManager : XSingleton<XAudioManager> {
	
	GameObject m_audioRoot = null;
	
	class audioNode{
		public AudioSource m_audioSource = null;
		
		public XU3dAudio m_xAudio = null;
	}
	
	Dictionary<uint,audioNode> m_audioList = new Dictionary<uint, audioNode>();
	
	public void init()
	{
		m_audioRoot = new GameObject("AudioRoot");
		m_audioRoot.transform.parent = LogicApp.SP.transform;
	}
	
	public void preLoadAudio( uint audioID )
	{
		audioNode audioN = null;
		if(!m_audioList.TryGetValue(audioID,out audioN ) )
		{
			//add to root
			audioN = new audioNode();
			m_audioList.Add(audioID,audioN );
			audioN.m_audioSource = m_audioRoot.AddComponent<AudioSource>();
			audioN.m_audioSource.volume = 1.0f;
			audioN.m_xAudio = new XU3dAudio(audioID,loadedCompleted );
		}
		
	}
	
	private void loadedCompleted(XU3dAudio self)
	{
		m_audioList[ self.ID ].m_audioSource.clip = self.audioClip;
	}
	
	public void playAudio( uint audioID )
	{
		audioNode audioN = null;
		if(m_audioList.TryGetValue(audioID,out audioN ) )
		{
			audioN.m_audioSource.Play();
		}else
		{
			//add to root
			audioN = new audioNode();
			m_audioList.Add(audioID,audioN );
			audioN.m_audioSource = m_audioRoot.AddComponent<AudioSource>();
			audioN.m_audioSource.volume = 1.0f;
			audioN.m_xAudio = new XU3dAudio(audioID,loadedCompleted );
		}
	}
	
}
