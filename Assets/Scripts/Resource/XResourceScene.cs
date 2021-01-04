namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class XResourceScene : XResourceBase
	{
		public static string ResTypeName	= "Scene";
		
		public string SceneName
		{
			get
			{
				if(MainAsset == null)
					return "";
				return MainAsset.ResName;
			}
		}
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourceScene));
		}
		
		public override void Load()
		{
			MainAssetPath	= ServiceInfo.res_path + ResTypeName + '/';
			DependAssetPath	= ServiceInfo.res_path + "Shared/Shared";
			
			base.Load();
		}
		
		public void GetAllResource(List<DownloadItem> list)
		{
			list.Add(MainAsset.DownLoad);
			foreach(SingleDependAsset dep in mDependList)
			{
				list.Add(dep.DownLoad);
			}
		}
	}
}


