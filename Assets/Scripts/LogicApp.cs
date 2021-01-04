using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using resource;


class LogicApp : MonoBehaviour
{
	//test:
	public bool						TestScene = false;
	public GameObject				TestBlock = null;
	public GameObject				TestRoad = null;
	//------

	public Camera					MainCamera = null;
	public Camera					UICamera = null;
	
	public string					ApacheServer = "0.0.0.0:0";
	public string					LoginServer = "0.0.0.0:0";
	public Define					UserDefine = null;
	
	public XWeUIRoot				UIRoot;
	
    public static LogicApp SP { get; private set; }

	public void Start()
	{
		LogicApp.SP = this;
        DontDestroyOnLoad(this);
		CoroutineManager.Init(this);
		
		CoroutineManager.StartCoroutine(StartDataInit());
	}
	
	private IEnumerator StartDataInit()
	{
		Log.Write("LogicApp StartDataInit Start");
		
		Input.imeCompositionMode = IMECompositionMode.On;
        Application.runInBackground = true;
        //Application.RegisterLogCallback(new Application.LogCallback(MyExpception.OnLogCallback));
        MyExpception.NewErrorEvent += new Action<string>(Statistics.ReportBug);
		Log.Write(DumpApplication());
		//CachingAuthorize();
		XDownloadManager.Init(true);
		InitGameMgr();
		//XDownloadHelper.Init();
		//MachineInfo.AutoChooseQualityLevel();
		XUTFirstLogin.CurLoading	= AppLoader.SP;
		yield return StartCoroutine(InitStartupParams());
		//yield return StartCoroutine(AppLoader.LoadLanguage());
		yield return StartCoroutine(AppLoader.LoadConfig());
		yield return StartCoroutine(AppLoader.LoadMaterial());
		
		Log.Write("LogicApp StartDataInit End");
		
		//XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eLoginFirst);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eLoginUI);
	}
	
	private void InitGameMgr()
	{
		XDBConfigManager.SP.Init();
		// 事件初始化
		XEventManager.SP.Init();
		// UI相关初始化
		XUIManager.SP.Init();
		// 逻辑初始化
		XLogicWorld.SP.Init();
		
		XResourceManager.Init();
		XNoticeManager.SP.Init();
	
		// 硬件消息初始化
		XKeyEventGate.SP.Init();
		XMouseEventGate.SP.Init();
		FeatureDataUnLockMgr.SP.Init();
	}
	
	private IEnumerator InitStartupParams()
	{
		ServiceInfo.SetDefault();
		//if ((Application.platform != RuntimePlatform.WindowsWebPlayer) && (Application.platform != RuntimePlatform.OSXWebPlayer))
  //      {
  //          if (Application.platform != RuntimePlatform.WindowsEditor)
  //          {
  //              if (Application.platform == RuntimePlatform.WindowsPlayer)
  //              {
		//		}
		//	}
		//}
		yield return 0;
	}	

	public void Update()
	{
		//XEventCtrlPool.SP.Breathe();
		XHardWareGate.SP.Breathe();
        XLogicWorld.SP.Breathe();
		XCameraLogic.SP.Breathe();
		//XDynamicObjectManager.SP.Breathe();
		XDownloadManager.Update();
		XUIManager.SP.Breathe();
	}
	
	void LateUpdate() {
        XCameraLogic.SP.LateBreathe();
    }
	
	void OnGUI() {
    	XCameraLogic.SP.OnRenderObject();
    }
	
    public void OnApplicationQuit()
    {
        //--4>TODO: 目前是单个场景, 离开直接退出
        XLogicWorld.SP.OnDestroy();
    }

    public void OnLevelWasLoaded(int level)
    {		
		int levelId = XLogicWorld.SP.SceneManager.LoadedSceneId;
		string levelName = XLogicWorld.SP.SceneManager.LoadedSceneName;
		ESceneType levelType = XLogicWorld.SP.SceneManager.LoadedSceneType;
		XLogicWorld.SP.OnLevelLoaded(levelId, levelName, levelType);
    }
	
	private static string DumpApplication()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("DumpApplication\n", new object[0]);
        //builder.AppendFormat("  isWebPlayer: {0}\n", Application.isWebPlayer);
        builder.AppendFormat("  isPlaying: {0}\n", Application.isPlaying);
        builder.AppendFormat("  isLoadingLevel: {0}\n", Application.isLoadingLevel);
        builder.AppendFormat("  isEditor: {0}\n", Application.isEditor);
        builder.AppendFormat("  platform: {0}\n", Application.platform);
        builder.AppendFormat("  absoluteURL: {0}\n", Application.absoluteURL);
        builder.AppendFormat("  dataPath: {0}\n", Application.dataPath);
        builder.AppendFormat("  persistentDataPath: {0}\n", Application.persistentDataPath);
        builder.AppendFormat("  temporaryCachePath: {0}\n", Application.temporaryCachePath);
        //builder.AppendFormat("  webSecurityHostUrl: {0}\n", Application.webSecurityHostUrl);
        //builder.AppendFormat("  webSecurityEnabled: {0}\n", Application.webSecurityEnabled);
        //builder.AppendFormat("  srcValue: {0}\n", Application.srcValue);
        builder.AppendFormat("  targetFrameRate: {0}\n", Application.targetFrameRate);
        builder.AppendFormat("  unityVersion: {0}\n", Application.unityVersion);
        return builder.ToString();
    }
	
	private void CachingAuthorize()
	{
		XDownloadManager.CachingAuthorize("The Legendary", "http://shenqi.com/", 0x7fffffffL, 0x5556887f, "83bc14abfd4fe4eec7a6be6547348603bb5471fa33c8a157442fff0134b9856910f2c77f1cc3423bc1cd2deb1d1495eeb6c1115c31034a0cc064162f26b958123241c0adcb55db19a84cca60eaa03ea8c3b675be330ef49c71718e3b219afea3759f5d8d910356296c6b035c1bfa01b03e9b4247ba592604ed34bb35774bc5df");
	}
}
