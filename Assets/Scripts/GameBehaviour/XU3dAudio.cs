using UnityEngine;
using System.Collections;
using resource;

public class XU3dAudio : XU3dDynObject {
	
	public delegate void OnAudioLoadDone(XU3dAudio self);
	
	private OnAudioLoadDone m_loadDoneCallBack = null;
	
	private AudioClip m_AudioClip = null;
	
	public uint ID{
		get{ return m_nId;}
	}
	
	public AudioClip audioClip{
		get{ return m_AudioClip; }
	}
	
	public XU3dAudio(uint nId)
	{
		RefreshDynObject(nId);
	}
	
	public XU3dAudio(uint nId,OnAudioLoadDone loadDoneFunction)
	{
		m_loadDoneCallBack = loadDoneFunction;
		RefreshDynObject(nId);
	}
	
	public override void RefreshDynObject (uint id)
	{
		if(m_nId == id) 
			return;

		m_nId = id;
		doRefreshDynObject();
	}
	
	private void doRefreshDynObject()
	{
		NewGameObject("Audio_" + m_nId);		
		m_DynObject	= XResourceManager.GetResource(XResourceAudio.ResTypeName,m_nId );
		if(null == m_DynObject)
		{
			Log.Write(LogLevel.WARN, "XU3dEffect, not found resource: {0}", m_nId);
			return;
		}
		
		if(m_DynObject.IsLoadDone())
		{
			LoadCompleted(m_DynObject.MainAsset.DownLoad);
		}
		else
		{
			if(m_DynObject != null)
				m_DynObject.ResLoadEvent	-= LoadCompleted;
			XResourceManager.StartLoadResource(XResourceAudio.ResTypeName,m_nId );
			m_DynObject.ResLoadEvent	+= new XResourceBase.LoadCompletedDelegate(LoadCompleted);
		}

	}
	
	private void LoadCompleted( DownloadItem itemLoad )
	{
		#if RES_DEBUG
			AudioClip go = itemLoad.go as AudioClip;
		#else
			AudioClip go = itemLoad.ab.mainAsset as AudioClip;
		#endif
		
		onAudioDone(go);
	}
	
	private void onAudioDone(AudioClip audio )
	{
		if(null == audio)
		{
			Log.Write(LogLevel.WARN, "XU3dAudio, wrong resource: {0}", m_nId);
			return;
		}
		
		m_AudioClip = Object.Instantiate(audio) as AudioClip;
		
		if( null != m_loadDoneCallBack )
		{
			m_loadDoneCallBack(this);
		}
	}
	
}
