using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class ResourceDeploy
{
	private class MainAsset
	{		
		public string m_strAssetType;
		public int m_nId;
		public string m_strFilePath;
		public string m_Name;
		public string OrignalName;
		public string m_Version;
		public UnityEngine.Object m_obj;		

		public XmlElement m_xmlParentEle = null;
		public XmlElement m_xmlSelfEle = null;
		
		public MainAsset(string strAssetType, int nId, string strFilePath, UnityEngine.Object obj,string name)
		{
			m_strAssetType 	= strAssetType;
			m_nId 			= nId;
			m_strFilePath 	= strFilePath;
			m_obj 			= obj;	
			OrignalName		= name;
			m_Version = GenVersion().ToString();
			m_Name			= name + '_' + m_Version;
		}
		
		
		
		public static int GenVersion()
		{
			int temp 	= (int) (((ulong) (((DateTime.Now.Ticks / 0x2710L) / 0x3e8L) / 60L)) % 0x80000000L);
			return temp;
		}
	}
	
	private class objectCompare : IComparer<UnityEngine.Object>
	{
		public int Compare(UnityEngine.Object x, UnityEngine.Object y)
		{
			return x.GetInstanceID() - y.GetInstanceID();
		}
	}
	
	private class assetListCompare : IComparer<List<MainAsset>>
	{
		public int Compare(List<MainAsset> x, List<MainAsset> y)
		{
			if(x.Count != y.Count)
				return y.Count - x.Count;
			
			for(int i=0; i<x.Count; i++)
			{
				MainAsset mx = x[i];
				MainAsset my = y[i];
				if(mx.m_strFilePath != my.m_strFilePath)
				{
					return string.Compare(mx.m_strFilePath, my.m_strFilePath);
				}
			}	
			return 0;
		}
	}
	
	public static string buildDir
	{
		get
		{
			switch(EditorUserBuildSettings.activeBuildTarget)
			{
			case(BuildTarget.StandaloneWindows):
			//case(BuildTarget.WebPlayer):
				return Application.dataPath + "/../Deploy/WebPlayer";
				
			default:
				return null;
			}
		}
	}
	
	public static string SrcResPath
	{
		get
		{
			return Application.dataPath + "Assets/";
		}
	}
	
	[MenuItem("Tools/Deploy")]
	public static void Deploy()
	{
		string[] resConfigs = new string[]{
			"Other/Config/Resource/Audio.txt",
			"Other/Config/Resource/TextAsset.txt",
			"Other/Config/Resource/Model.txt",
			"Other/Config/Resource/Effect.txt",
			"Other/Config/Resource/Scene.txt",
			"Other/Config/Resource/UIAtlas.txt",
			"Other/Config/Resource/UIPanel.txt",
			"Other/Config/Resource/Material.txt",
			"Other/Config/Resource/Cursor.txt",
		};
		
		// 生成MainAsset的List, 并验证配置文件是否合理
		bool isMissFile = false;
		int MissCount = 0;
		List<MainAsset> mainAssets = new List<MainAsset>();
		for(int i=0; i<resConfigs.Length; i++)
		{
			TextAsset textAsset = AssetDatabase.LoadMainAssetAtPath("Assets/" + resConfigs[i]) as TextAsset;
			if(null == textAsset)
			{
				Log.Write(LogLevel.WARN, "config is null {0}", resConfigs[i]);
				return;
			}
			
			string strAssetType = Path.GetFileNameWithoutExtension(resConfigs[i]);
			TabFile tabFile = new TabFile(textAsset.name, textAsset.text);
			while(tabFile.Next())
			{
				int nId = tabFile.Get<int>("id");
				string strFilePath = tabFile.Get<string>("FilePath");
				for(int j=0; j<mainAssets.Count; j++)
				{
					MainAsset mainAsset = mainAssets[j];
					if(mainAsset.m_strFilePath == strFilePath)
					{
						Log.Write(LogLevel.WARN, "exist two same path {0} {1}, {2}", strFilePath, mainAsset.m_strAssetType, strAssetType);
						return;
					}
					if(mainAsset.m_strAssetType == strAssetType && mainAsset.m_nId == nId)
					{
						Log.Write(LogLevel.WARN, "{0} repleat id {1}", strAssetType, nId);
						return;
					}
				}
				UnityEngine.Object fileObj = AssetDatabase.LoadMainAssetAtPath("Assets/" + strFilePath);
				if(null == fileObj)
				{
					isMissFile	= true;
					Log.Write(LogLevel.WARN, "null id:{0} name:{1}", nId,strFilePath);
					MissCount++;
					continue;
				}
				
				//UnityEngine.Object[] dependObjects = EditorUtility.CollectDependencies(new UnityEngine.Object[] { fileObj });
				
				string fileName = Path.GetFileNameWithoutExtension(strFilePath);
				MainAsset asset = new MainAsset(strAssetType, nId, strFilePath, fileObj,fileName);
				mainAssets.Add(asset);				
			}
		}
		
		if(isMissFile)
		{
			Log.Write(LogLevel.WARN, "has Miss File Count {0}",MissCount);
		}
		
		// 分析依赖关系
		SortedList<UnityEngine.Object, List<MainAsset>> depends = new SortedList<UnityEngine.Object, List<MainAsset>>(new objectCompare());
		for(int i=0; i<mainAssets.Count; i++)
		{
			MainAsset mainAsset = mainAssets[i];
			UnityEngine.Object[] dependObjects = EditorUtility.CollectDependencies(new UnityEngine.Object[] { mainAsset.m_obj });
			for(int j=0; j<dependObjects.Length; j++)
			{
				UnityEngine.Object dependObj = dependObjects[j];
				if(!depends.ContainsKey(dependObj))
					depends.Add(dependObj, new List<MainAsset>());
				depends[dependObj].Add(mainAsset);
			}	
		}
		
		// 根据依赖关系, 分包
		SortedList<List<MainAsset>, List<UnityEngine.Object>> assets = new SortedList<List<MainAsset>, List<UnityEngine.Object>>(new assetListCompare());
		foreach(KeyValuePair<UnityEngine.Object, List<MainAsset>> kvp in depends)
		{
			List<UnityEngine.Object> list = null;
			if(!assets.ContainsKey(kvp.Value))
			{
				list = new List<UnityEngine.Object>();
				assets.Add(kvp.Value, list);
			}
			if(null == list) list = assets[kvp.Value];
			list.Add(kvp.Key);
		}
		
		// 准备XML配置文件
		string strXmlFile = "Other/Config/Resource/wwwResource.xml";
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(Application.dataPath + "/" + strXmlFile);
		xmlDoc.RemoveAll();	
		XmlElement root = xmlDoc.CreateElement("AssetList");
		xmlDoc.AppendChild((XmlNode)root);
		root.SetAttribute("TotalSize", "0");
		
		SortedList<string, XmlElement> typeXmlEle = new SortedList<string, XmlElement>();
		for(int i=0; i<mainAssets.Count; i++)
		{
			MainAsset mainAsset = mainAssets[i];
			if(!typeXmlEle.ContainsKey(mainAsset.m_strAssetType))
			{
				XmlElement typeEle = xmlDoc.CreateElement(mainAsset.m_strAssetType);
				typeEle.SetAttribute("TotalSize", "0");
				root.AppendChild(typeEle);
				typeXmlEle.Add(mainAsset.m_strAssetType, typeEle);
			}
			mainAsset.m_xmlParentEle = typeXmlEle[mainAsset.m_strAssetType];
			mainAsset.m_xmlSelfEle = xmlDoc.CreateElement("Asset");
			mainAsset.m_xmlSelfEle.SetAttribute("Id", "0");
			mainAsset.m_xmlSelfEle.SetAttribute("Version", mainAsset.m_Version.ToString());
			mainAsset.m_xmlSelfEle.SetAttribute("TotalSize", "0");
			mainAsset.m_xmlParentEle.AppendChild(mainAsset.m_xmlSelfEle);
		}
		
		//----------------------------------------------------------------------打包
		BuildPipeline.PushAssetDependencies();
		// 打包客户端
//		string errorHead = BuildPipeline.BuildPlayer(new string[]{"Assets/Scene/JueSe.unity"}, buildDir, BuildTarget.WebPlayer, BuildOptions.None);
//		if(!string.IsNullOrEmpty(errorHead))
//		{
//			Log.Write(LogLevel.ERROR,"{0}",errorHead);
//			return ;
//		}
		
		// 打包Shared
		int k = 0;
		foreach(KeyValuePair<List<MainAsset>, List<UnityEngine.Object>> kvp in assets)
		{
			List<MainAsset> listMainAsset = kvp.Key;
			if(listMainAsset.Count <= 1)
			{
				//MainAsset te1 = listMainAsset[0];
				//Log.Write(LogLevel.INFO,"objectName is {0}",te1.m_Name);
				continue;
			}
			
			List<UnityEngine.Object> listObject = kvp.Value;
			int version = MainAsset.GenVersion();
			string strTgtPath = "Shared/Shared" + (++k) + '_' + version.ToString() + ".asset";			
			bool isSucess = BuildPipeline.BuildAssetBundle(null, listObject.ToArray(), buildDir + "/" + strTgtPath, 
				BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, EditorUserBuildSettings.activeBuildTarget);
			
			if(!isSucess)
			{
				Log.Write(LogLevel.DEBUG,"build {0} failed",strTgtPath);
				continue;
			}
			
			// 设置大小观察信息
			FileInfo fileInfo = new FileInfo(buildDir  + "/" +  strTgtPath);
			double fileSize = fileInfo.Length / 1024f;
			root.SetAttribute("TotalSize", (fileSize + Convert.ToDouble(root.GetAttribute("TotalSize"))).ToString("0.00"));
			
			for(int j=0; j<listMainAsset.Count; j++)
			{
				MainAsset mainAsset = listMainAsset[j];
				XmlElement ele = xmlDoc.CreateElement("Depend");
				ele.SetAttribute("Id", "" + k);
				ele.SetAttribute("Version", version.ToString());
				ele.SetAttribute("Size", fileSize.ToString("0.00"));
				mainAsset.m_xmlSelfEle.AppendChild(ele);
				mainAsset.m_xmlSelfEle.SetAttribute("TotalSize", (fileSize + Convert.ToDouble(mainAsset.m_xmlSelfEle.GetAttribute("TotalSize"))).ToString("0.00"));
			}
		}
		
		//打包共享
//		UnityEngine.Object temp1 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Atlases/Common/CommonAtlas.prefab");
//		BuildPipeline.BuildAssetBundle(temp1,null,buildDir + "/Shared/" + "CommonAtlas.asset",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
//		
//		UnityEngine.Object temp4 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Font/DySongTi/songti12.prefab");
//		BuildPipeline.BuildAssetBundle(temp4,null,buildDir + "/Shared/" + "Font12.asset",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
//		
//		UnityEngine.Object temp5 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Font/DySongTi/songti14.prefab");
//		BuildPipeline.BuildAssetBundle(temp5,null,buildDir + "/Shared/" + "Font14.asset",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
//		
//		UnityEngine.Object temp6 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Font/DySongTi/songti16.prefab");
//		BuildPipeline.BuildAssetBundle(temp6,null,buildDir + "/Shared/" + "Font16.asset",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
//		
		
		// 打包mainAsset
		for(int i=0; i<mainAssets.Count; i++)
		{
			MainAsset mainAsset = mainAssets[i];
			// todo: 是否会出现mainAsset.m_strFilePath不一样但是strTgtPath一样的情况?
			
			string strTgtName = "" + mainAsset.m_nId + '_' + mainAsset.m_Version.ToString() + ".asset";
			string strTgtPath = mainAsset.m_strAssetType + "/" + strTgtName;
			if("Scene" == mainAsset.m_strAssetType)
			{
				BuildPipeline.PushAssetDependencies();
				//string errorScene = BuildPipeline.BuildPlayer(new string[]{ "Assets/" + mainAsset.m_strFilePath }, buildDir  + "/" +  strTgtPath, 
				//			EditorUserBuildSettings.activeBuildTarget, BuildOptions.BuildAdditionalStreamedScenes);
				BuildPipeline.PopAssetDependencies();
				
				//if(!string.IsNullOrEmpty(errorScene))
				//{
				//	Log.Write(LogLevel.ERROR,"{0}",errorScene);
				//	return ;
				//}
			}
			else
			{
				BuildPipeline.PushAssetDependencies();
				bool isSuccess = BuildPipeline.BuildAssetBundle(mainAsset.m_obj, null, buildDir  + "/" +  strTgtPath,
							BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, EditorUserBuildSettings.activeBuildTarget);
				BuildPipeline.PopAssetDependencies();
				
				if(!isSuccess)
				{
					Log.Write(LogLevel.DEBUG,"build {0} failed",strTgtPath);
					continue;
				}
			}
			
			// 设置大小观察信息
			FileInfo fileInfo = new FileInfo(buildDir  + "/" +  strTgtPath);
			double fileSize = fileInfo.Length / 1024f;
			root.SetAttribute("TotalSize", (fileSize + Convert.ToDouble(root.GetAttribute("TotalSize"))).ToString("0.00"));
			mainAsset.m_xmlSelfEle.SetAttribute("Id", "" + mainAsset.m_nId);
			mainAsset.m_xmlSelfEle.SetAttribute("Size",fileSize.ToString("0.00"));
			mainAsset.m_xmlSelfEle.SetAttribute("Name",mainAsset.OrignalName);
			
			mainAsset.m_xmlSelfEle.SetAttribute("TotalSize", (fileSize + Convert.ToDouble(mainAsset.m_xmlSelfEle.GetAttribute("TotalSize"))).ToString("0.00"));
			mainAsset.m_xmlParentEle.SetAttribute("TotalSize", (Convert.ToDouble(mainAsset.m_xmlParentEle.GetAttribute("TotalSize"))
				+ Convert.ToDouble(mainAsset.m_xmlSelfEle.GetAttribute("TotalSize"))).ToString("0.00"));
		}
		BuildPipeline.PopAssetDependencies();
		//----------------------------------------------------------------------
		
		xmlDoc.Save(Application.dataPath + "/" + strXmlFile);
		Log.Write(LogLevel.INFO,"pack end");
	}	
	
	[MenuItem("Tools/Test")]
	public static void Test()
	{
		//BuildPipeline.BuildAssetBundle(Selection.activeObject, null, "E:/wwwResource.ab", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
	}
	
	[MenuItem("Tools/TestDepend")]
	public static void TestDepend()
	{
		BuildPipeline.PushAssetDependencies();
		UnityEngine.Object temp1 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Atlases/Common/CommonAtlas.prefab");
		//BuildPipeline.BuildAssetBundle(temp1,null,"E:/TestAB/com.ab",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
		
		
		UnityEngine.Object temp4 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Font/DySongTi/songti12.prefab");
		//BuildPipeline.BuildAssetBundle(temp4,null,"E:/TestAB/Font12.ab",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
		
		UnityEngine.Object temp5 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Font/DySongTi/songti14.prefab");
		//BuildPipeline.BuildAssetBundle(temp5,null,"E:/TestAB/Font14.ab",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
		
		UnityEngine.Object temp6 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Font/DySongTi/songti16.prefab");
		//BuildPipeline.BuildAssetBundle(temp6,null,"E:/TestAB/Font16.ab",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
		
//		BuildPipeline.PushAssetDependencies();
//		UnityEngine.Object temp2 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Atlases/Login/LoginAtlas.prefab");
//		BuildPipeline.BuildAssetBundle(temp2,null,"E:/TestAB/LoginUI.ab",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
//		BuildPipeline.PopAssetDependencies();
//		
//		BuildPipeline.PushAssetDependencies();
//		UnityEngine.Object temp2 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Prefab/LoginUI.prefab");
//		BuildPipeline.BuildAssetBundle(temp2,null,"E:/TestAB/LoginUI.ab",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
//		BuildPipeline.PopAssetDependencies();

		
		BuildPipeline.PopAssetDependencies();
		
		//BuildPipeline.PushAssetDependencies();
		UnityEngine.Object temp2 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Prefab/BagWindow.prefab");
		//BuildPipeline.BuildAssetBundle(temp2,null,"E:/TestAB/BagWindow.ab",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
		//BuildPipeline.PopAssetDependencies();
		
		//BuildPipeline.PushAssetDependencies();
		UnityEngine.Object temp3 = AssetDatabase.LoadMainAssetAtPath("Assets/UIResources/Prefab/ChatWindow.prefab");
		//BuildPipeline.BuildAssetBundle(temp3,null,"E:/TestAB/ChatWindow.ab",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
		//BuildPipeline.PopAssetDependencies();
		
		//BuildPipeline.BuildAssetBundle(Selection.activeObject, null, "E:/222.asset", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
	}
	
	
	
	[MenuItem("Tools/TestBuildPlayer")]
	public static void TestBuildPlayer()
	{
		//string error = BuildPipeline.BuildPlayer(new string[]{"Assets/Scene/JueSe.unity"},"E:/scene.asset", BuildTarget.WebPlayer, BuildOptions.None);
		//Log.Write(LogLevel.WARN,"{0}",error);
	}
	
	[MenuItem("Tools/PackDBConfig")]
	public static void PackDBConfig()
	{
		string[] resConfigs = new string[]{
			"Other/Config/Resource/TextAsset.txt",
		};
		
		// 生成MainAsset的List, 并验证配置文件是否合理
		bool isMissFile = false;
		int MissCount = 0;
		List<MainAsset> mainAssets = new List<MainAsset>();
		for(int i=0; i<resConfigs.Length; i++)
		{
			TextAsset textAsset = AssetDatabase.LoadMainAssetAtPath("Assets/" + resConfigs[i]) as TextAsset;
			if(null == textAsset)
			{
				Log.Write(LogLevel.WARN, "config is null {0}", resConfigs[i]);
				return;
			}
			
			string strAssetType = Path.GetFileNameWithoutExtension(resConfigs[i]);
			TabFile tabFile = new TabFile(textAsset.name, textAsset.text);
			while(tabFile.Next())
			{
				int nId = tabFile.Get<int>("id");
				string strFilePath = tabFile.Get<string>("FilePath");
				for(int j=0; j<mainAssets.Count; j++)
				{
					MainAsset mainAsset = mainAssets[j];
					if(mainAsset.m_strFilePath == strFilePath)
					{
						Log.Write(LogLevel.WARN, "{0} 同时出现在{1}, {2} 这两个配置中, 放弃本次发布", strFilePath, mainAsset.m_strAssetType, strAssetType);
						return;
					}
					if(mainAsset.m_strAssetType == strAssetType && mainAsset.m_nId == nId)
					{
						Log.Write(LogLevel.WARN, "{0} 配置出现了Id重复 {1}, 放弃本次发布", strAssetType, nId);
						return;
					}
				}
				UnityEngine.Object fileObj = AssetDatabase.LoadMainAssetAtPath("Assets/" + strFilePath);
				if(null == fileObj)
				{
					isMissFile	= true;
					Log.Write(LogLevel.WARN, "资源为空: {0}, 放弃本次发布", strFilePath);
					MissCount++;
					continue;
				}
	
				string fileName = Path.GetFileNameWithoutExtension(strFilePath);
				MainAsset asset = new MainAsset(strAssetType, nId, strFilePath, fileObj,fileName);
				mainAssets.Add(asset);				
			}
		}
		
		if(isMissFile)
		{
			Log.Write(LogLevel.WARN, "资源为空文件数目{0}, 放弃本次发布",MissCount);
		}
		
		// 准备XML配置文件
		string strXmlFile = "Other/Config/Resource/wwwResource.xml";
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(Application.dataPath + "/" + strXmlFile);
		xmlDoc.RemoveAll();	
		XmlElement root = xmlDoc.CreateElement("AssetList");
		xmlDoc.AppendChild((XmlNode)root);
		root.SetAttribute("TotalSize", "0");
		
		SortedList<string, XmlElement> typeXmlEle = new SortedList<string, XmlElement>();
		for(int i=0; i<mainAssets.Count; i++)
		{
			MainAsset mainAsset = mainAssets[i];
			if(!typeXmlEle.ContainsKey(mainAsset.m_strAssetType))
			{
				XmlElement typeEle = xmlDoc.CreateElement(mainAsset.m_strAssetType);
				typeEle.SetAttribute("TotalSize", "0");
				root.AppendChild(typeEle);
				typeXmlEle.Add(mainAsset.m_strAssetType, typeEle);
			}
			mainAsset.m_xmlParentEle = typeXmlEle[mainAsset.m_strAssetType];
			mainAsset.m_xmlSelfEle = xmlDoc.CreateElement("Asset");
			mainAsset.m_xmlSelfEle.SetAttribute("Id", "0");
			mainAsset.m_xmlSelfEle.SetAttribute("TotalSize", "0");
			mainAsset.m_xmlParentEle.AppendChild(mainAsset.m_xmlSelfEle);
		}
		
		BuildPipeline.PushAssetDependencies();
		
		// 打包mainAsset
		for(int i=0; i<mainAssets.Count; i++)
		{
			MainAsset mainAsset = mainAssets[i];
			// todo: 是否会出现mainAsset.m_strFilePath不一样但是strTgtPath一样的情况?
			string strTgtName = "" + mainAsset.m_Name + ".asset";
			//string strTgtName = "" + mainAsset.m_nId + ".asset";
			string strTgtPath = mainAsset.m_strAssetType + "/" + strTgtName;
			if("Scene" == mainAsset.m_strAssetType)
			{
				//string errorScene = BuildPipeline.BuildPlayer(new string[]{ "Assets/" + mainAsset.m_strFilePath }, buildDir  + "/" +  strTgtPath, 
				//			EditorUserBuildSettings.activeBuildTarget, BuildOptions.BuildAdditionalStreamedScenes);
				
				//if(!string.IsNullOrEmpty(errorScene))
				//{
				//	Log.Write(LogLevel.ERROR,"{0}",errorScene);
				//	return ;
				//}
			}
			else
			{
				BuildPipeline.BuildAssetBundle(mainAsset.m_obj, null, buildDir  + "/" +  strTgtPath,
							BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, EditorUserBuildSettings.activeBuildTarget);
			}
			
			// 设置大小观察信息
			FileInfo fileInfo = new FileInfo(buildDir  + "/" +  strTgtPath);
			double fileSize = fileInfo.Length / 1024f;
			root.SetAttribute("TotalSize", (fileSize + Convert.ToDouble(root.GetAttribute("TotalSize"))).ToString("0.00"));
			mainAsset.m_xmlSelfEle.SetAttribute("Id", "" + mainAsset.m_nId);
			mainAsset.m_xmlSelfEle.SetAttribute("TotalSize", (fileSize + Convert.ToDouble(mainAsset.m_xmlSelfEle.GetAttribute("TotalSize"))).ToString("0.00"));
			mainAsset.m_xmlParentEle.SetAttribute("TotalSize", (Convert.ToDouble(mainAsset.m_xmlParentEle.GetAttribute("TotalSize"))
				+ Convert.ToDouble(mainAsset.m_xmlSelfEle.GetAttribute("TotalSize"))).ToString("0.00"));
		}
		BuildPipeline.PopAssetDependencies();
		//----------------------------------------------------------------------
		
		xmlDoc.Save(Application.dataPath + "/" + strXmlFile);
	}
}