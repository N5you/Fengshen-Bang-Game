namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	
	public class XResourceEffect : XResourceBase
	{
		public static string ResTypeName	= "Effect";
		
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourceEffect));
		}
		
		public override void Load()
		{
			MainAssetPath	= ServiceInfo.res_path + ResTypeName + '/';
			DependAssetPath	= ServiceInfo.res_path + "Shared/Shared";
			
			base.Load();
		}
		
	}
}


