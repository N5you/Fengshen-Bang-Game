namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class XResourceTextAsset : XResourceBase
	{
		public EDBConfg_Item ConfigType;
		
		public XResourceTextAsset()
		{
			
		}
		
		public static string ResTypeName	= "TextAsset";
		
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourceTextAsset));
		}
		
		public override void Load()
		{
			MainAssetPath	= ServiceInfo.res_path + ResTypeName + '/';
			DependAssetPath	= ServiceInfo.res_path + "Shared/Shared";
			
			base.Load();
		}
		
		public void InitDBConfig(DownloadItem item)
		{
			IConfigManager mgr = XDBConfigManager.SP.GetConfigManager(ConfigType);
			if(mgr == null)
				return ;
#if RES_DEBUG
			mgr.Init(item.go as TextAsset);
#else
			mgr.Init(item.ab.mainAsset as TextAsset);
#endif
		}
	}
}



