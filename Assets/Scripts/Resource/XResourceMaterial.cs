namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class XResourceMaterial : XResourceBase
	{
		public static string ResTypeName	= "Material";
		public Material	ResMaterial;
		public XResourceMaterial()
		{
			
		}
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourceMaterial));
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
			Material temp = item.go as Material;
#else
			Material temp = item.ab.mainAsset as Material;
#endif
			ResMaterial = GameObject.Instantiate(temp) as Material;
		}
	}
}




