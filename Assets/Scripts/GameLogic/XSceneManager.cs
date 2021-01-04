using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Base.Map;
using XGame.Client.Packets;
using System.Timers;
using resource;

// 场景类型
public enum ESceneType
{
	CharScene,			// 选角场景
    NormalScene,		// 营地
    ClientScene,		// 副本
    FightScenePVE,		// 战斗场景
	FightScenePVP,		// 
	FightCutScene,		// 战斗特效
	CutScene,			// 动画场景
	NormalCutScene,			// 片头动画
}

public class XSceneManager : IBehaviourListener,IProcessLoad
{
	private static readonly float MAX_HEIGHT = 4444.0f;
	private static readonly uint BossLoadingEffectID = 900057;
	private static readonly uint ComLoadingEffectID  = 900058;
	
	// SceneId. SceneName. SceneType 皆是逻辑数据, 并非真正的场景资源数据
	public int LoadedSceneId { get; private set; }
	public string LoadedSceneName { get; private set; }
	public ESceneType LoadedSceneType { get; private set; }
	
	public int PreSceneId {get;private set;}
	public ESceneType PreSceneType {get; private set;}
	
	public Vector3 PrePlayerPos { get;  set; }
	public Vector3 PrePlayerDir { get;  set; }
	// 从DynLoad开始和ApplicationLoad->扫描地形数据结束 都是IsLoading状态
	public bool IsLoading { get; private set; }
	
	private XResourceScene m_dynLevel;
	// 场景地形数据
    private int m_CellNumX;
    private int m_CellNumZ;
    private XCellData[,] m_CellMap;
	private Terrain m_Terrain;	
	
	//private Timer mTimer  = new Timer(1000 * 1.8);
	private Timer mTimer  = new Timer(1000 * 1.8);
	public  bool	IsPlayingEffect = true;
	
	private Timer mDeltaTimer = new Timer(500);
	public  bool    IsDeltaTime	= true;
	
	// 寻路信息
    private XFourPath m_FourPath;
	private Dictionary<ESceneType,XSceneDataBase>	mSceneDataMap = new Dictionary<ESceneType, XSceneDataBase>();
	
	public XSceneManager()
	{
		LoadedSceneId = 0;
		LoadedSceneName = string.Empty;
		LoadedSceneType = ESceneType.NormalScene;
		PreSceneType	= ESceneType.NormalScene;
		m_dynLevel = null;
		IsLoading = false;
		PrePlayerPos 	= Vector3.zero;
		PrePlayerDir 	= Vector3.zero;
		
	}
	
	~XSceneManager()
	{
		reset();
	}
	
	public static void FadeOutOnComplete()
	{
		XCameraLogic.SP.mainCamera.gameObject.SetActive(true);
	}
	
	public static void FadeInOnComplete()
	{
		XLogicWorld.SP.MainPlayer.Position = XLogicWorld.SP.MainPlayer.CacheSceneInitPos;
		XLogicWorld.SP.MainPlayer.Direction = XLogicWorld.SP.MainPlayer.CacheSceneInitDir;
		XCameraLogic.SP.mainCamera.gameObject.SetActive(false);
		XLogicWorld.SP.SceneManager.IsPlayingEffect = false;
		
		if(XBattleManager.SP.IsReadyLeaveFight)
		{
			XBattleManager.SP.LeaveFight();
			XBattleManager.SP.IsReadyLeaveFight	= false;
		}
		
		if(XLogicWorld.SP.MainPlayer.IsInSubScene)
		{			
			XCameraLogic.SP.AdjustCamera(XLogicWorld.SP.SubSceneManager.EnterCameraPos,XLogicWorld.SP.SubSceneManager.EnterCameraRot);
		}
		
		
	}
	
	private void TimeReach(object sender, ElapsedEventArgs e)
	{
		IsPlayingEffect = false;
		
		if(mTimer != null)
			mTimer.Stop();
	}
	
	private void DeltaTimeReach(object sender, ElapsedEventArgs e)
	{
		IsDeltaTime = false;
		
		if(mDeltaTimer != null)
			mDeltaTimer.Stop();
	}
	
	
	
	public void Init()
	{
		AddSceneData(ESceneType.CharScene,new XSceneData_CharScene());
		AddSceneData(ESceneType.NormalScene,new XSceneData_Normal());
		AddSceneData(ESceneType.ClientScene,new XSceneData_SubScene());
		AddSceneData(ESceneType.FightScenePVE,new XSceneData_FightScenePVE());
		AddSceneData(ESceneType.FightScenePVP,new XSceneData_FightScenePVP());
		AddSceneData(ESceneType.CutScene,new XSceneData_CutScene());
		AddSceneData(ESceneType.NormalCutScene,new XSceneData_CutSceneNormal());
	}
	
	public void AddSceneData(ESceneType type,XSceneDataBase data)
	{
		if(mSceneDataMap.ContainsKey(type))
			return ;
		
		mSceneDataMap.Add(type,data);
	}
	
	public void Breathe()
	{
		if(!IsLoading) return;
		//XEventManager.SP.SendEvent(EEvent.LoadScene_Progress, m_dynLevel.progress);
	}
	
	public string GetProcessText()
	{
		string sceneName = Path.GetFileNameWithoutExtension(m_dynLevel.SceneName);
		return "Loading Scene " + sceneName;
	}
	
	public float  GetCurRate()
	{
		if(m_dynLevel == null)
			return 0;
		
		List<DownloadItem> list = new List<DownloadItem>();
		//list.Add(m_dynLevel.MainAsset.DownLoad);
		m_dynLevel.GetAllResource(list);	
		
		
		return DownloadItem.GetLoadingProgress(list);
	}
	
	public static float CellSize = 1.0f;
	
	public IEnumerator _loadScene(int nSceneId, ESceneType sceneType)
	{
		if(sceneType == ESceneType.FightScenePVE || sceneType == ESceneType.FightScenePVP)
		{
			CoroutineManager.StartCoroutine(CreateLoadingEffect());
			//CreateLoadingEffect();
		}
		else
		{			
			if(sceneType == ESceneType.NormalScene || sceneType == ESceneType.NormalCutScene ||
				sceneType == ESceneType.CutScene)
			{
				XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eLoadSceneUI);
				XEventManager.SP.SendEvent(EEvent.LoadScene_Progress, 0f);
				XEventManager.SP.SendEvent(EEvent.LoadScene_Discription, XStringManager.SP.GetString(23));
				IsPlayingEffect	= false;
				if(LoadedSceneType == ESceneType.FightScenePVE || LoadedSceneType == ESceneType.FightScenePVP)
				{
					if(XBattleManager.SP.IsReadyLeaveFight)
					{
						XBattleManager.SP.LeaveFight();
						XBattleManager.SP.IsReadyLeaveFight	= false;
					}
				}
			}
			else
			{
				IsPlayingEffect	= true;
				CameraFade.StartAlphaFade(Color.black,CameraFade.EFADE_TYPE.EFADE_TYPE_FADE_IN,1f,0f,FadeInOnComplete,FadeOutOnComplete);
			}
		}
		
		if(sceneType == ESceneType.FightScenePVE || sceneType == ESceneType.FightScenePVP)
		{
			while(IsDeltaTime)
			{
				yield return 1;
			}
		}
		else
		{
			while(IsPlayingEffect)
			{
				yield return 1;
			}
			
		}
		
		yield return 1;
		
		Log.Write(LogLevel.INFO,"start load Scene {0}",nSceneId);
		if(!mSceneDataMap.ContainsKey(sceneType))
			yield break ;
		XSceneDataBase	sceneData = mSceneDataMap[LoadedSceneType];
			
		if(sceneData != null)
			sceneData.OnLeaveScene();
		
		XHardWareGate.SP.Lock = true;		
		PreSceneId		= LoadedSceneId;
		PreSceneType	= LoadedSceneType;
		LoadedSceneId 	= nSceneId;
		LoadedSceneType = sceneType;
		PrePlayerPos	= XLogicWorld.SP.MainPlayer.Position;
		PrePlayerDir	= XLogicWorld.SP.MainPlayer.Direction;
		
		XUTLoadScene.CurLoading	= this;
		
		//Debug
		int numItems = 0;
		int numLoading = 0;
		int numWaiting = 0;
		XDownloadManager.DebugGetDownloadInfo(out numItems,out numLoading,out numWaiting);		
		Log.Write(LogLevel.DEBUG,"num Items is {0} num Loading is {1} num Waiting is {2}",numItems,numLoading,numWaiting);
		
		IsLoading = true;
		reset();

		m_dynLevel	= XResourceManager.GetResource(XResourceScene.ResTypeName,(uint)nSceneId) as XResourceScene;
		if(m_dynLevel == null)
		{
			Log.Write(LogLevel.ERROR,"cant find Scene ID {0}",nSceneId);
			yield break ;
		}
		if(m_dynLevel.IsLoadDone())
		{
			LoadCompleted(m_dynLevel.MainAsset.DownLoad);
		}
		else
		{
			if(m_dynLevel != null)
				m_dynLevel.ResLoadEvent	-= LoadCompleted;
			XResourceManager.StartLoadResource(XResourceScene.ResTypeName,(uint)nSceneId);
			m_dynLevel.ResLoadEvent	+= new XResourceBase.LoadCompletedDelegate(LoadCompleted);
		}
	}	
	
	private IEnumerator CreateLoadingEffect()
	{
		XEventManager.SP.SendEvent(EEvent.UI_HideAllUI);
		
		uint effectID = 0;
		if(XLogicWorld.SP.SubSceneManager.IsLastOne())
			effectID	= BossLoadingEffectID;
		else
			effectID	= ComLoadingEffectID;
			
		XU3dEffect effect = new XU3dEffect(effectID);
		effect.Parent	= LogicApp.SP.transform;
		effect.Layer	= GlobalU3dDefine.Layer_UI_2D;
		
		
		IsPlayingEffect	= true;
		mTimer.Elapsed += new ElapsedEventHandler(TimeReach);
		mTimer.Start();		
		
		IsDeltaTime	= true;
		mDeltaTimer.Elapsed += new ElapsedEventHandler(DeltaTimeReach);
		mDeltaTimer.Start();
		
		//BattleDisplayerMgr.SP.SetLoadingCamera();
		
		//XCameraLogic.SP.mainCamera.gameObject.SetActive(false);
		XLogicWorld.SP.MainPlayer.Visible = false;
		
		yield return 0;
	}
	
	public void LoadCompleted(DownloadItem item)
	{
#if RES_DEBUG
		string sceneName = Path.GetFileNameWithoutExtension(m_dynLevel.SceneName);
		onDynLevelLoaded(LoadedSceneId,sceneName);
#else
		onDynLevelLoaded(LoadedSceneId,m_dynLevel.SceneName);
#endif
		
	}
	
	private void onDynLevelLoaded(int levelId, string levelName)
	{
		if(!IsLoading) return;		
		LoadedSceneName = levelName;
		XEventManager.SP.SendEvent(EEvent.LoadScene_Progress, 1f);
		Application.LoadLevelAsync(levelName);
		Log.Write(LogLevel.INFO,"Load Level {0}",levelName);
	}
	
	private void onDynLevelError(int levelId)
	{
		// dump!
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eLoadSceneUI);
		XHardWareGate.SP.Lock = false;
	}
	
	public IEnumerator OnLevelLoaded(int sceneId, string sceneName, ESceneType sceneType)
	{
		if(sceneType == ESceneType.FightScenePVE || sceneType == ESceneType.FightScenePVP)
		{
			while(IsPlayingEffect)
			{
				yield return 0;
			}
		}
		else
		{
			while(IsPlayingEffect)
			{
				yield return 0;
			}
		}
		
		XSceneDataBase	sceneData = mSceneDataMap[sceneType];
		sceneData.OnLoaded(sceneId,sceneName);
		
		XHardWareGate.SP.Lock = false;
		IsLoading = false;
		
		// 如果是战斗场景，需要进行键盘移动引导
		if ( sceneType == ESceneType.ClientScene )
		{
			XNewPlayerGuideManager.SP.handleKeybord((int)XNewPlayerGuideManager.GuideType.Guide_KeyBord);
		}
		else if ( sceneType == ESceneType.NormalScene )
		{
			XNewPlayerGuideManager.SP.handleEnterNormalScene((int)XNewPlayerGuideManager.GuideType.Guide_KeyBord);
		}
		else
		{
			XNewPlayerGuideManager.SP.handleKeybordDisapper((int)XNewPlayerGuideManager.GuideType.Guide_KeyBord);
		}
		
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eLoadSceneUI);	
		
		XLogicWorld.SP.MainPlayer.OnLevelLoaded(sceneId, sceneType);
        XLogicWorld.SP.ObjectManager.OnLevelLoaded(sceneId, sceneName, sceneType);
	}
	
	public void SetTerrainRoad()
	{
		m_Terrain = Terrain.activeTerrain;
		//  扫描格子信息, 目前是动态扫描, 以后要改成直接导入预先准备的静态逻辑地图数据
		ScanTerrain(m_Terrain, out m_CellMap, out m_CellNumX, out m_CellNumZ);
		m_FourPath = new XFourPath(m_CellMap, m_CellNumX, m_CellNumZ);	// 设置寻路
		setRoadAndBlock();	// 设置Road和Block
	}
	
	private void reset()
	{
		m_CellNumX = 0;
		m_CellNumZ = 0;
		m_CellMap = null;
		m_FourPath = null;
		m_Terrain = null;
	}
	
	// 扫描地形, 生成格子数据
	public static void ScanTerrain(Terrain terrain, out XCellData[,] cellMap, out int cellNumX, out int cellNumZ)
	{
		cellNumX = Mathf.CeilToInt(terrain.terrainData.size.x / CellSize);
		cellNumZ = Mathf.CeilToInt(terrain.terrainData.size.z / CellSize);
		
		cellMap = new XCellData[cellNumX, cellNumZ];
		RaycastHit hitInfo;
        Vector3 origin = new Vector3(0.0f, MAX_HEIGHT, 0.0f);
		
		for(int x = 0; x < cellNumX; ++x)
        {
            for(int z = 0; z < cellNumZ; ++z)
            {
				if ( x == 46 && z == 23)
				{
					int a = 3;
				}
				origin.x = x * CellSize;
				origin.z = z * CellSize;
				
                cellMap[x, z] = new XCellData();
				cellMap[x, z].BarrierType = 0;
				cellMap[x, z].Height = terrain.SampleHeight(origin);
				
				float step = 0.3f;
				bool bBlock = false;
				bool bRoad = false;
				for(; origin.x < (x + 1) * CellSize; origin.x += CellSize * step)
				{
					for(origin.z = z * CellSize; origin.z < (z + 1) * CellSize; origin.z += CellSize * step)
					{
						if(Physics.Raycast(origin, Vector3.down, out hitInfo))
						{
							if(hitInfo.collider.tag == GlobalU3dDefine.Tag_Block)
							{
								cellMap[x, z].BarrierType = 1;
								//if(x == 0)
								//	cellMap[x, z].Height = 0f;
								//else
								//	cellMap[x, z].Height = cellMap[x - 1, z].Height;
								bBlock = true;
								// test
								if(LogicApp.SP.TestScene && null != LogicApp.SP.TestBlock)
								{
									GameObject go = GameObject.Instantiate(LogicApp.SP.TestBlock) as GameObject;
									go.transform.parent = null;
									go.transform.localScale = new Vector3(CellSize, 2f, CellSize);
									go.transform.position = new Vector3(x + CellSize / 2f, 1f + cellMap[x, z].Height, z + CellSize / 2f);
									XUtil.SetLayer(go, GlobalU3dDefine.Layer_IgnoreRaycast);
								}
							}
							else if(!bRoad && hitInfo.collider.tag == GlobalU3dDefine.Tag_Road)
							{
								cellMap[x, z].Height = hitInfo.point.y;
								bRoad = true;
								// test:
								if(LogicApp.SP.TestScene && null != LogicApp.SP.TestRoad)
								{
									GameObject go = GameObject.Instantiate(LogicApp.SP.TestRoad) as GameObject;
									go.transform.parent = null;
									go.transform.localScale = new Vector3(CellSize, 0.1f, CellSize);
									go.transform.position = new Vector3(x + CellSize / 2f, cellMap[x, z].Height, z + CellSize / 2f);
									XUtil.SetLayer(go, GlobalU3dDefine.Layer_IgnoreRaycast);
								}
							}
							if(bBlock) break;
						}
						
					}
					if(bBlock) break;
				}
					
            }
        }
	}

	//block -> ignore , road & terrain -> decal, 增加XBehaviour
	private void setRoadAndBlock()
	{
		Terrain terrain = Terrain.activeTerrain;
		// 获得所有阻挡包围盒 Wall, 设置 layer
        Component[] allCollider = terrain.GetComponentsInChildren(typeof(Collider), true);
        foreach (Component c in allCollider)
        {
            if (c.tag == GlobalU3dDefine.Tag_Block)
            {
				XUtil.SetLayer(c.gameObject, GlobalU3dDefine.Layer_IgnoreRaycast);
            }
			
			else if (c.tag == GlobalU3dDefine.Tag_Road)
			{
				XUtil.SetLayer(c.gameObject, GlobalU3dDefine.Layer_Decal);
				XBehaviour behaviour = c.gameObject.AddComponent<XBehaviour>();
				behaviour.BehaType = EBehaviourType.e_BehaviourType_Terrain;
				behaviour.AddListener(this);
			}
        }
		terrain.gameObject.layer = GlobalU3dDefine.Layer_TerrainObject;
		XBehaviour beha = terrain.gameObject.AddComponent<XBehaviour>();
		beha.BehaType = EBehaviourType.e_BehaviourType_Terrain;
		beha.AddListener(this);
	}
	
	
	// 获取自动寻路中途点
	public ArrayList FindPath(Vector3 posFrom, Vector3 posTo)
	{
        if (m_FourPath != null)
        {
			ArrayList list = m_FourPath.FindPath(posFrom / CellSize, posTo / CellSize);
			if(null != list)
			{
				for(int i=0; i<list.Count; i++)
				{
					list[i] = (Vector3)(list[i]) * CellSize;
				}
			}
			return list;
        }
        return null;
    }
	
	// 获取直线行走能到达的最远点
    public Vector3 GetLineReachablePoint(Vector3 posFrom, Vector3 posTo)
    {
        if (m_FourPath != null)
        {
            return m_FourPath.GetLineReachablePoint(posFrom / CellSize, posTo / CellSize) * CellSize;
        }
        return Vector3.zero;
    }
	
	// 获取X,Z平面的中心点
	public Vector3 GetCenterPos()
	{
		if(null == m_Terrain)
			return Vector3.zero;
		float fx = m_Terrain.terrainData.size.x / 2f;
		float fz = m_Terrain.terrainData.size.z / 2f;
		return new Vector3(fx, GetTerrainHeight(fx, fz), fz);
	}
	
	public bool IsCellWalkable(float x, float z)
	{
		int cx = Mathf.CeilToInt(x / CellSize);
		int cz = Mathf.CeilToInt(z / CellSize);
     	if(null == m_CellMap || 0 > cx || cx >= m_CellNumX || 0 > cz || cz >= m_CellNumZ)
			return false;
		
		//if(m_CellMap[cx, cz] == null)
		//	return false;
		
		return 0 == m_CellMap[cx, cz].BarrierType;
	}
	
	// 获取点高度, 直接取点所在格子的高度
    public float GetTerrainHeight(float x, float z)
    {
		int cx = Mathf.CeilToInt(x / CellSize);
		int cz = Mathf.CeilToInt(z / CellSize);
     	if(null == m_CellMap || 0 > cx || cx >= m_CellNumX || 0 > cz || cz >= m_CellNumZ)
			return 0f;
		
		if(m_CellMap[cx, cz] == null)
			return 0f;
		
		if(m_CellMap[cx, cz].Height == 0)
		{
			//Log.Write(LogLevel.INFO,"GetTerrainHeight height == 0 {0}{1}", x, z);;
		}
		return m_CellMap[cx, cz].Height;
    }
	
	public void FixTerrainHeight(ref Vector3 pos)
    {
		pos.y = GetTerrainHeight(pos.x, pos.z);
    }
	
	// 根据性插值算法取该坐标的地形高度
	public float GetExactHeightAt(float x, float z) 
	{
		int xLower = (int)(x);
		int xHigher = xLower + 1; 
		float xRelative = (x - xLower) / ((float)xHigher - (float)xLower); 
		int zLower = (int)(z); 
		int zHigher = zLower + 1; 
		float zRelative = (z - zLower) / ((float)zHigher - (float)zLower);
	
		float heightLxLz = GetTerrainHeight(xLower, zLower);
		float heightLxHz = heightLxLz;
		float heightHxLz = heightLxLz;
		float heightHxHz = heightLxLz;
		bool bLxHz = IsCellWalkable(xLower, zHigher);
		bool bHxLz = IsCellWalkable(xHigher, zLower);
		bool bHxHz = IsCellWalkable(xHigher, zHigher);
		if(bLxHz) heightLxHz = GetTerrainHeight(xLower, zHigher);
		if(bHxLz) heightHxLz = GetTerrainHeight(xHigher, zLower);
		if(bHxHz) heightHxHz = GetTerrainHeight(xHigher, zHigher);
		if(!bLxHz && !bHxLz)
		{
			return heightLxLz;
		}
		else if(!bLxHz && !bHxHz)	// 单线性
		{
			return heightLxLz + xRelative * (heightHxLz - heightLxLz);	
		}
		else if(!bHxLz && !bHxHz)
		{
			return heightLxLz + zRelative * (heightLxHz - heightLxLz);
		}
		
		// 双线性
		bool pointAboveLowerTriangle = (xRelative + zRelative < 1); 
		float finalHeight; 
		if(pointAboveLowerTriangle) 
		{ 
		    finalHeight = heightLxLz; 
		    finalHeight += zRelative * (heightLxHz - heightLxLz); 
		    finalHeight += xRelative * (heightHxLz - heightLxLz); 
		}
		else 
		{
		    finalHeight = heightHxHz; 
		    finalHeight += (1.0f - zRelative) * (heightHxLz - heightHxHz); 
		    finalHeight += (1.0f - xRelative) * (heightLxHz - heightHxHz); 
		}
		return finalHeight; 
	}
	
	// 插值
	public void FixExactHeight(ref Vector3 pos)
	{
		pos.y = GetExactHeightAt(pos.x, pos.z);
	}
	
	public void ShowTransPoint()
	{
		SortedList<uint,XCfgLevelEntry> list = XCfgLevelEntryMgr.SP.ItemTable;
		foreach(KeyValuePair<uint,XCfgLevelEntry> item in list)
		{
			if(item.Value.SceneID == LoadedSceneId)
			{
				XCfgLevelEntry cfg = XCfgLevelEntryMgr.SP.GetConfig(item.Key);
				if(cfg != null)
				{
					XTransPointAppearInfo	info = new XTransPointAppearInfo(cfg.Index);
		        	XTransPoint xtp = XLogicWorld.SP.ObjectManager.AppearTransPoint(info);
					xtp.SetHudVisible(false);
				}
			}
		}
	}
	
	public void ShowGatherObject()
	{
		SortedList<int, XCfgGatherObjectBorn> list = XCfgGatherObjectBornMgr.SP.GetGroup((uint)LoadedSceneId);
		if(list == null)
			return ;
		foreach(KeyValuePair<int, XCfgGatherObjectBorn> kvp in list)
		{
			XGatherObjectAppearInfo info = new XGatherObjectAppearInfo(kvp.Key, kvp.Value.BornPos, kvp.Value.Direction);
			XLogicWorld.SP.ObjectManager.AppearGatherObject(info);
		}
	}
	
	public void RequireEnterScene(uint sceneId)
	{
		CS_UInt.Builder builder = CS_UInt.CreateBuilder();
		builder.SetData(sceneId);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ChangeScene, builder.Build());
	}
	
	#region IBehaviourListener
	public void OnMouseDown(int mouseCode, Vector3 clickPoint)
	{
		if(0 != mouseCode) return;
		
		// 营地或者副本中需要自动寻路
		if(ESceneType.NormalScene == LoadedSceneType
			|| ESceneType.ClientScene == LoadedSceneType)
		{
			XLogicWorld.SP.MainPlayer.AutoMoveTo(clickPoint);
			
			XNewPlayerGuideManager.SP.handleDirectionDelay((int)XNewPlayerGuideManager.GuideType.Guide_Mouse);
		}
	}
	public void OnMouseUp(int mouseCode){}
	public void OnMouseUpAsButton(int mouseCode){}
	public void OnMouseEnter(){}
	public void OnMouseExit(){}
	public void OnControllerColliderHit(ControllerColliderHit hit){}
	public void OnCancelSelect() {}
    #endregion
}




