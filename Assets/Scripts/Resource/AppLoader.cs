using resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using XGame.Client.Base.Pattern;

#if RES_DEBUG
using UnityEditor;
#endif

public enum LoadOrder
{
	LoadOrder_LoadNone	= 0,
	LoadOrder_LoadLang	= 1,
	LoadOrder_LoadResConfig = 2,
	LoadOrder_LoadDBConfig	= 3,
	LoadOrder_Num
}

public class AppLoader : XSingleton<AppLoader>,IProcessLoad
{
    private static List<DownloadItem> downloadList	= new List<DownloadItem>();
    public static string fontName;
    public static bool LoadInitFile1Done;
    public static bool LoadInitFile2Done;
	
	public static LoadOrder	CurLoadOrder	= LoadOrder.LoadOrder_LoadNone;

    private static string GetLanguageUrl(string name)
    {
        return string.Format("{0}data/language/{1}_{2}.txt.gz", ServiceInfo.res_path, name, Config.LANG);
    }

    public static float GetLoadingProgress1()
    {
        if (downloadList != null)
        {
            return DownloadItem.GetLoadingProgress(downloadList);
        }
        return 0f;
    }

    public static string GetShareResPath()
    {
        string str = ServiceInfo.res_path;
        while (str.EndsWith("/"))
        {
            str = str.Substring(0, str.Length - 1);
        }
        for (int i = 0; i < 2; i++)
        {
            str = str.Substring(0, str.LastIndexOf('/'));
        }
        return (str + "/Shared/");
    }
	
	public static IEnumerator LoadLanguage()
	{
		DownloadItem item = XDownloadManager.GetDownload(AppLoader.GetLanguageUrl("lang"));
		while(!item.IsDone)
		{
			yield return 0;
		}
		
		if(item.hasError)
		{
			if(item.www != null)
			{
				Log.Write(LogLevel.ERROR,"Load Language error {0}",item.www.error);
			}
			else
				Log.Write(LogLevel.ERROR,"Load Language error item.www is null");
		}
		else
		{
			
		}
	}
	
	public static IEnumerator LoadMaterial()
	{
		Log.Write(LogLevel.INFO,"LoadMaterial start");
#if RES_DEBUG
		Material XRayMat = AssetDatabase.LoadMainAssetAtPath("Assets/Other/Material/XRayMat.mat") as Material;
		XU3dModel.InitXRayMaterial(XRayMat);
		yield return 0;
#else
		XResourceBase resItem = XResourceManager.GetResource(XResourceMaterial.ResTypeName,4002);
		XResourceManager.StartLoadResource(XResourceMaterial.ResTypeName,4002);
		while(!resItem.IsLoadDone())
		{
			yield return 0;
		}
		
		XU3dModel.InitXRayMaterial(resItem.MainAsset.DownLoad.ab.mainAsset as Material);
#endif
		
		Log.Write(LogLevel.INFO,"LoadMaterial end");
	}
	
	public static IEnumerator LoadConfig()
	{		
		Log.Write(LogLevel.INFO,"LoadConfig start");
#if RES_DEBUG
		LoadResConfig_Debug();
		yield return 0;
#else
		yield return CoroutineManager.StartCoroutine(LoadResConfig());
#endif
		//XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eLoginFirst);
		
#if RES_DEBUG
		yield return CoroutineManager.StartCoroutine(LoadDBConfig_Debug());
#else
		yield return CoroutineManager.StartCoroutine(LoadDBConfig());
#endif
		
		Log.Write(LogLevel.INFO,"LoadConfig end");
	}	
	
#if RES_DEBUG	
	private static void LoadResConfig_Debug()
	{
		CurLoadOrder	= LoadOrder.LoadOrder_LoadResConfig;
		Log.Write(LogLevel.INFO,"LoadResConfig start");
		string[] configs = new string[]{"TextAsset.txt","Model.txt","Effect.txt","Scene.txt","UIAtlas.txt","UIPanel.txt","Material.txt","Audio.txt","Cursor.txt"};
		for(int i=0; i<configs.Length; i++)
		{
			TextAsset textAsset = AssetDatabase.LoadMainAssetAtPath("Assets/Other/Config/Resource/" + configs[i]) as TextAsset;
			XResourceManager.InitSingleResourceConfig(textAsset);
		}
		
		XEventManager.SP.SendEvent(EEvent.ResourceFileReady);
		Log.Write(LogLevel.INFO,"LoadResConfig end");
	}
#else
	private static IEnumerator LoadResConfig()
	{
		CurLoadOrder	= LoadOrder.LoadOrder_LoadResConfig;
		Log.Write(LogLevel.INFO,"LoadResConfig start");
		string resName = "wwwResource.ab";
		DownloadItem item = XDownloadManager.GetDownload(ServiceInfo.res_path + resName);
		item.LoadCompletedEvent	+= new DownloadItem.LoadCompletedDelegate(LoadResConfigCompleted);
		downloadList.Add(item);
		while(!item.IsDone)
		{
			yield return null;
		}
		
		Log.Write(LogLevel.INFO,"LoadResConfig end");
	}
#endif
	
#if RES_DEBUG	
	private static IEnumerator LoadDBConfig_Debug()
	{
		CurLoadOrder	= LoadOrder.LoadOrder_LoadDBConfig;
		Log.Write(LogLevel.INFO,"LoadDBConfig start");
		SortedList<uint,XResourceBase> mgr = XResourceManager.GetAllResourceByType(XResourceTextAsset.ResTypeName);
		if(mgr == null)
			yield break ;
		
		List<DownloadItem>	AllDBConfigList = new List<DownloadItem>();
		foreach(KeyValuePair<uint,XResourceBase> temp in mgr)
		{
			XResourceTextAsset res = XResourceManager.GetResource(XResourceTextAsset.ResTypeName,temp.Key) as XResourceTextAsset;
			XResourceManager.StartLoadResource(XResourceTextAsset.ResTypeName,temp.Key);
			res.ConfigType	= (EDBConfg_Item)temp.Key;
			res.ResLoadEvent	+= res.InitDBConfig;
			AllDBConfigList.AddRange(res.GatherDownloadItem());
		}
		
		while(!DownloadItem.IsAllDone(AllDBConfigList))
		{
			yield return null;
		}
		
		XEventManager.SP.SendEvent(EEvent.DBFileLoadReady);
		
		Log.Write(LogLevel.INFO,"LoadDBConfig end");
	}
#else
	private static IEnumerator LoadDBConfig()
	{
		CurLoadOrder	= LoadOrder.LoadOrder_LoadDBConfig;
		Log.Write(LogLevel.INFO,"LoadDBConfig start");
		
		SortedList<uint,XResourceBase> mgr = XResourceManager.GetAllResourceByType(XResourceTextAsset.ResTypeName);
		if(mgr == null)
			yield break;
		
		List<DownloadItem>	AllDBConfigList = new List<DownloadItem>();
		foreach(KeyValuePair<uint,XResourceBase> temp in mgr)
		{
			XResourceTextAsset res = XResourceManager.GetResource(XResourceTextAsset.ResTypeName,temp.Key) as XResourceTextAsset;
			XResourceManager.StartLoadResource(XResourceTextAsset.ResTypeName,temp.Key);
			res.ConfigType	= (EDBConfg_Item)temp.Key;
			res.ResLoadEvent	+= new XResourceBase.LoadCompletedDelegate(res.InitDBConfig);			
			AllDBConfigList.AddRange(res.GatherDownloadItem());
		}
		
		while(!DownloadItem.IsAllDone(AllDBConfigList))
		{
			yield return null;
		}
		
		XEventManager.SP.SendEvent(EEvent.DBFileLoadReady);
		
		
		Log.Write(LogLevel.INFO,"LoadDBConfig end");
	}
#endif
	
	public static void LoadResConfigCompleted(DownloadItem item)
	{
		XResourceManager.InitWWWResourceConfig(item.ab.mainAsset as TextAsset);
	}
	
	public string GetProcessText()
	{
		string temp = "";
		switch(CurLoadOrder)
		{
		case LoadOrder.LoadOrder_LoadLang:
			temp = CurLoadOrder.ToString();
			break;
		case LoadOrder.LoadOrder_LoadResConfig:
			temp = CurLoadOrder.ToString();
			break;
		case LoadOrder.LoadOrder_LoadDBConfig:
			temp = CurLoadOrder.ToString();
			break;
		}
		
		return temp;
	}
	public float  GetCurRate()
	{
#if RES_DEBUG
		float res = 0.0f;
		switch(CurLoadOrder)
		{
		case LoadOrder.LoadOrder_LoadLang:
			res	= 1.0f;
			break;
		case LoadOrder.LoadOrder_LoadResConfig:
			res	= 1.0f;
			break;
		case LoadOrder.LoadOrder_LoadDBConfig:
			res	= 1.0f;
			break;
		}
		
		return res;
#else
		float res = 0.0f;
		switch(CurLoadOrder)
		{
		case LoadOrder.LoadOrder_LoadLang:
			res	= 1.0f;
			break;
		case LoadOrder.LoadOrder_LoadResConfig:
			res	= 1.0f;
			break;
		case LoadOrder.LoadOrder_LoadDBConfig:
			res	= 1.0f;
			break;
		}
		
		return res;
#endif
	}
}

