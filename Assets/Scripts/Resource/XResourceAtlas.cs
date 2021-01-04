namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	
	public class XResourceAtlas : XResourceBase
	{
		public static string ResTypeName	= "UIAtlas";
		public int AtlasID;
		
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourceAtlas));
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
			GameObject go = item.go as GameObject;
#else
			GameObject go = item.ab.mainAsset as GameObject;
#endif
			XUIDynamicAtlas.SP.onAtlasDone(AtlasID,go);
		}
		
	}
}



