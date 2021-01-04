using UnityEngine;
using System.Collections;
using resource;

public class XU3dEffect : XU3dDynObject
{
	private bool bDone;			// 标记特效是否加载完成
	private bool bPlayOver;		// 标记特效是否播放结束
	private NcDestroyEvent m_DestroyEvt;
	public NcDestroyEvent.OnDestroyEvt onEffectPlayOver;
	public delegate void OnEffectLoaded(XU3dEffect self);
	private static uint mRunNumber = 0;
	OnEffectLoaded	mEffectLoaded;
	private GameObject mEffGO;
	
	public XU3dEffect(uint nId)
	{
		RefreshDynObject(nId);
	}
	
	public XU3dEffect(uint nId,OnEffectLoaded handle)
	{
		mEffectLoaded	= handle;
		RefreshDynObject(nId);
	}
	
	public override void RefreshDynObject (uint id)
	{
		if(m_nId == id) 
			return;

		m_nId = id;
		//releaseDynObject();
		doRefreshDynObject();
	}
	
	private void doRefreshDynObject()
	{
		NewGameObject("Effect_" + m_nId + "_" + mRunNumber);
		mRunNumber++;
		bDone = false;
		bPlayOver = false;
		onEffectPlayOver = null;
		
		m_DynObject	= XResourceManager.GetResource(XResourceEffect.ResTypeName,m_nId);
		if(m_DynObject == null)
		{
			Log.Write(LogLevel.ERROR,"cant find Effect ID {0}",m_nId);
			return ;
		}

		if(m_DynObject.IsLoadDone())
		{
			LoadCompleted(m_DynObject.MainAsset.DownLoad);
		}
		else
		{
			if(m_DynObject != null)
				m_DynObject.ResLoadEvent	-= LoadCompleted;
			XResourceManager.StartLoadResource(XResourceEffect.ResTypeName,m_nId);
			m_DynObject.ResLoadEvent	+= new XResourceBase.LoadCompletedDelegate(LoadCompleted);
		}
	}
	
	public void LoadCompleted(DownloadItem item)
	{
#if RES_DEBUG
			GameObject go = item.go as GameObject;
#else
			GameObject go = item.ab.mainAsset as GameObject;
#endif
		onEffectDone(m_nId,go);
	}
	
	private void onEffectDone(uint nEffectId, GameObject go)
	{
		bDone = true;
		if(!bPlayOver)
		{
			if(null == go)
			{
				Log.Write(LogLevel.WARN, "XU3dEffect, wrong resource: {0}", m_nId);
				return;
			}
			
			if(mEffGO != null)
				return ;
			
			if(m_gameObject == null)
				return;
			
			mEffGO = XUtil.Instantiate(go, m_gameObject.transform, Vector3.zero, Vector3.zero);
			Object.DontDestroyOnLoad(mEffGO);			
			XUtil.SetLayer(mEffGO, Layer);
#pragma warning disable
			mEffGO.SetActiveRecursively(true);
#pragma warning restore
			
			if(mEffectLoaded != null)
				mEffectLoaded(this);
			
			NcAutoDestruct autoDes = mEffGO.GetComponent<NcAutoDestruct>();
			if(autoDes == null)			
				return ;
				
			if(autoDes.m_fLifeTime  == 0)
					return ;
			autoDes.enabled	= true;	
			if(mEffGO)
				mEffGO.AddComponent<NcDestroyEvent>().onDestroyEvt += onEffectDestroy;
		}
	}
	
	private void onEffectError(int nEffectId)
	{
		bDone = true;
		Log.Write(LogLevel.WARN, "XU3dEffect, wrong resource: {0}", m_nId);
	}
	
	private void onEffectDestroy()
	{
		bPlayOver = true;
		if(null != onEffectPlayOver)
		{
			onEffectPlayOver();
			onEffectPlayOver = null;
		}
		Destroy(true);
		//releaseDynObject();
	}
	
//	private void releaseDynObject()
//	{
//		if(null != m_DynObject)
//		{
//			if(!bDone)
//			{
//				m_DynObject.onDone -= onEffectDone;
//				m_DynObject.onError -= onEffectError;
//			}
//			m_DynObject.refCount--;
//			m_DynObject = null;
//		}		
//	}
	
	public override void Destroy (bool bDestroyAll)
	{
		//releaseDynObject();
		base.Destroy(bDestroyAll);
	}
	
	public void FlyFromTo(Transform fromTran, Transform toTran,float speed,float HWRate)
	{
		if(null == fromTran || null == toTran) return;
		if(null == m_gameObject) return;
		NcEffectFlying fly = m_gameObject.AddComponent<NcEffectFlying>();
		fly.FromPos = fromTran.position;
		fly.ToPos 	= toTran.position;
		fly.Speed	= speed;
		fly.HWRate	= HWRate;
	}
	
	public void FlyFromTo(Vector3 fromPos, Vector3 toPos,float speed,float HWRate)
	{
		if(null == m_gameObject) return;
		NcEffectFlying fly = m_gameObject.AddComponent<NcEffectFlying>();
		fly.FromPos = fromPos;
		fly.ToPos 	= toPos;
		fly.Speed	= speed;
		fly.HWRate	= HWRate;
	}
}

