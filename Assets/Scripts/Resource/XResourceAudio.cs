using UnityEngine;
using System.Collections;

namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class XResourceAudio : XResourceBase
	{
		public static string ResTypeName	= "Audio";
		public int m_AudioID;
		public AudioClip m_AudioClip;
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourceAudio) );
		}
		
		public override void Load()
		{
			MainAssetPath	= ServiceInfo.res_path + ResTypeName + '/';
			DependAssetPath	= ServiceInfo.res_path + "Shared/Shared";
			
			base.Load();
		}
		
		public void LoadCompleted(DownloadItem item)
		{
#if RES_DEBUG
			m_AudioClip = item.go as AudioClip;
#else
			m_AudioClip = item.ab.mainAsset as AudioClip;
#endif
			
		}
		
	}
}
