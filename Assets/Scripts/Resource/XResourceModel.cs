namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class XResourceModel : XResourceBase
	{
		public static string ResTypeName	= "Model";
		public XResourceModel()
		{
			
		}
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourceModel));
		}
		
		public override void Load()
		{
			MainAssetPath	= ServiceInfo.res_path + ResTypeName + '/';
			DependAssetPath	= ServiceInfo.res_path + "Shared/Shared";
			
			base.Load();
		}
	}
}



