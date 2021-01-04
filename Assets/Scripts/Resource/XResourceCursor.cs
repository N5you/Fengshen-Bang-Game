
using UnityEngine;
using System.Collections;

namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class XResourceCursor : XResourceBase
	{
		public static string ResTypeName	= "Cursor";
		public Texture2D m_Texture2D;
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourceCursor) );
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
			m_Texture2D = item.go as Texture2D;
#else
			m_Texture2D = item.ab.mainAsset as Texture2D;
#endif
			
		}
		
	}
}
