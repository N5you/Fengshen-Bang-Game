namespace resource
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class AssetBundleDestroyer
	{
		private static List<AssetBundle> abs = new List<AssetBundle>();

		public static void Add(AssetBundle ab)
		{
			List<AssetBundle> abs = AssetBundleDestroyer.abs;
			lock (abs)
			{
				AssetBundleDestroyer.abs.Add(ab);
			}
		}

		public static void DoDestroy()
		{
			List<AssetBundle> abs = AssetBundleDestroyer.abs;
			lock (abs)
			{
				if (AssetBundleDestroyer.abs.Count > 0)
				{
					foreach (AssetBundle bundle in AssetBundleDestroyer.abs)
					{
						if (bundle != null)
						{
							bundle.Unload(false);
						}
					}
					AssetBundleDestroyer.abs.Clear();
				}
			}
		}
	}
}


