using System;
using UnityEngine;
using System.Collections.Generic;


class AutoAdjustCamSize:MonoBehaviour{
		
		private Camera m_camera = null;
		private XU3dModel m_Xmodel = null;
		
		private uint currentFrame = 0;	
		
		private float m_fDistanceTotal = 0.0f;	
		
		private uint m_uiComFrameCount = 6;
		
		private bool m_bIsWhole = false;
		
		private bool m_bIsCareZ = false;
		
		private float m_fHalfHight = 0.0f;
		
		public Camera camera{
			set{m_camera = value;
			}
		}
		
		public bool isCareZ{
			set{
				m_bIsCareZ = value;
			}
		}
	
		public XU3dModel xModel{
			set{ 
				
				m_Xmodel = value;
				
				SkinnedMeshRenderer[] renders = m_Xmodel.m_gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				foreach(SkinnedMeshRenderer rend in renders )
				{
					rend.updateWhenOffscreen = true;
				}
			}
		}
		
		public bool isWhole
		{
			set
			{
				m_bIsWhole = value;
			}
		}
	
		void Start() 
		{
        	currentFrame = 0;
    	}
		
		void Update()
		{
			if(null == m_camera||null == m_Xmodel )
				return;
			
			currentFrame++;
			
			//start from frame 2 stated animation
			if( 1 >=currentFrame || 20 <= currentFrame )
				return;
			updateCameSize();
		}
		
		private void updateCameSize()
		{
			
			SkinnedMeshRenderer[] renders = m_Xmodel.m_gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			Bounds rootBounds = new Bounds(m_Xmodel.Position,new Vector3(0.1f,0.1f,0.1f) );
			foreach(SkinnedMeshRenderer rend in renders )
			{
				rootBounds.Encapsulate( rend.bounds );
			}
			
			//
			float widthHalfBase = rootBounds.size.x/2.0f;
			
			if( m_bIsCareZ )
			{
				widthHalfBase = Mathf.Max(widthHalfBase,rootBounds.size.z/2.0f );
			}
			
			float heightHalfBase = widthHalfBase/m_camera.aspect;
			//
			
			
			float widthHalfFinal = widthHalfBase/2.0f;
		
			//need show all the model move the model against to camera half Y
			if( m_bIsWhole )
			{
				Vector3 localPos = m_Xmodel.LocalPosition;
				
				localPos.y = - rootBounds.size.y/2.0f;
				
				m_Xmodel.LocalPosition = localPos;
			}
			
			float heightHalfFinal = rootBounds.max.y - m_camera.transform.position.y;
			
			if( heightHalfFinal < heightHalfBase )
			{
				heightHalfFinal = heightHalfBase;
			}
			
			m_fHalfHight = Mathf.Max(m_fHalfHight,heightHalfFinal );
			
			m_camera.orthographicSize = m_fHalfHight;
		}
}


public class SingleModelRTT
{
	public static UInt64 Count = 0;
	public float CameraSize = 0.2f;
	
	private XU3dModel 	m_ObjectModel;
	private GameObject	m_RTTGo;
	
	private RenderTexture	rt;
	
	private UITexture mTempTexture;
	private Vector3 TexturePos;
	private uint mNPCID;
	
	private bool bIsWhole = false;
	
	public SingleModelRTT()
	{
		string name = "ModelRTTGo" + Convert.ToString(Count);
		rt = new RenderTexture(512,512,24,RenderTextureFormat.ARGB32);
		rt.name	= "RTTTexture" + Convert.ToString(Count);
		
		Count++;		
		m_RTTGo = new GameObject(name);
		m_RTTGo.transform.parent	= LogicApp.SP.transform;
		m_RTTGo.AddComponent<Camera>();
		Camera camera = m_RTTGo.GetComponent<Camera>();
		camera.orthographic		= true;
		camera.orthographicSize	= CameraSize;
		camera.backgroundColor		= new Color(0f,0f,0f,0f);
		XUtil.SetLayer(m_RTTGo, GlobalU3dDefine.Layer_ModelRTT);
		int layerMask = 1 << GlobalU3dDefine.Layer_ModelRTT;
		camera.cullingMask	= layerMask;
		m_RTTGo.transform.localPosition	= new Vector3(0f,0f,0f);
		m_RTTGo.SetActive(false);
		
		MonoBehaviour.DontDestroyOnLoad(m_RTTGo);
	}
	
	public void SetCameraSize(float size)
	{
		Camera camera = m_RTTGo.GetComponent<Camera>();
		camera.orthographicSize	= size;
	}
	
	public void ShowSingleNPCModel(uint npcID,UITexture texture,float x,float y,float z)
	{
		mNPCID			= npcID;
		mTempTexture	= texture;
		TexturePos		= new Vector3(x,y,z);
		
		XCfgNpcBase npcBase = XCfgNpcBaseMgr.SP.GetConfig(mNPCID);
		if(null==npcBase)
			return;
		
		m_ObjectModel = new XU3dModel("ModelRTT_NPC",npcBase.ModelId,actShowSingleModel);
	}
	
	public void ShowSingleModel(uint modelID,UITexture texture,float x,float y,float z)
	{
		mTempTexture	= texture;
		TexturePos		= new Vector3(x,y,z);
		
		m_ObjectModel = new XU3dModel("ModelRTT_NPC",modelID,actShowSingleModel);
	}
	
	public void ShowSingleModelWhole(uint modelID,UITexture texture,float x,float y,float z)
	{
		mTempTexture	= texture;
		TexturePos		= new Vector3(x,y,z);
		
		bIsWhole = true;
		
		m_ObjectModel = new XU3dModel("ModelRTT_NPC",modelID,actShowSingleModel);
	}
	
	private void actShowSingleModel(XU3dModel self)
	{
		//对象可能在构造函数里就直接完成，这时候m_ObjectModel还是空的，在这里做下赋值
		if(m_ObjectModel == null)
			m_ObjectModel = self;
		
		Transform chestTF = m_ObjectModel.GetSkeleton(ESkeleton.eHeadCenter);
		
		m_ObjectModel.Parent = m_RTTGo.transform;
		m_ObjectModel.Layer = GlobalU3dDefine.Layer_ModelRTT;
		//m_ObjectModel.Direction = new Vector3(0,180,0);
		m_ObjectModel.Position = m_RTTGo.transform.TransformPoint(new Vector3(TexturePos.x,-chestTF.position.y,TexturePos.z));
		m_ObjectModel.PlayAnimation(EAnimName.Idle, 1.0f, false);
		
		Camera cameraCom = m_RTTGo.GetComponent<Camera>();
		cameraCom.targetTexture	= rt;
		
		//add auto adjust cam size
		AutoAdjustCamSize camFllow = m_RTTGo.AddComponent<AutoAdjustCamSize>();
		camFllow.camera = cameraCom;
		camFllow.xModel = self;
		camFllow.isWhole = bIsWhole;
		
		cameraCom.orthographicSize = 0.8f;
		cameraCom.nearClipPlane = 0.01f;
		mTempTexture.mainTexture = rt;
		m_RTTGo.SetActive(true);
		
		m_RTTGo.transform.localPosition	= new Vector3((Count++) * 10.0f,0f,0f);
	}
	
	public void ShowModel(uint modelID, UITexture texture, float x, float y, float z)
	{
		mTempTexture	= texture;
		TexturePos		= new Vector3(x,y,z);
		m_ObjectModel 	= new XU3dModel("ModelRTT_Model",modelID,_ShowModel);
	}
	
	public void ShowObjModel(XU3dModel objmodel, UITexture texture, float cameraSize, Vector3 pos, Vector3 rotate, Vector3 scale, bool useCameraCom)
	{
		mTempTexture = texture;
		CameraSize = cameraSize;
		SetCameraSize(cameraSize);
		TexturePos	= new Vector3(pos.x, pos.y, pos.z);
		m_ObjectModel = objmodel;
		m_ObjectModel.Scale = new Vector3(scale.x, scale.y, scale.z);
		_ShowModel(m_ObjectModel, rotate, TexturePos, useCameraCom);
	}
	
	private void _ShowModel(XU3dModel self, Vector3 rotate, Vector3 pos, bool useCameraCom)
	{
		if(m_ObjectModel == null)
			m_ObjectModel = self;
		
		CharacterController control = m_ObjectModel.m_gameObject.GetComponentInChildren<CharacterController>();
		if(control == 	null)
			return ;
		
		m_RTTGo.transform.localPosition	= new Vector3((Count++) * 10.0f,0f,0f);
		m_ObjectModel.Parent = m_RTTGo.transform;
		m_ObjectModel.Layer = GlobalU3dDefine.Layer_ModelRTT;
		m_ObjectModel.LocalPosition = pos;
		m_ObjectModel.Direction = rotate;
		m_ObjectModel.PlayAnimation(EAnimName.Idle, 1.0f, false);
		
		Camera cameraCom = m_RTTGo.GetComponent<Camera>();
		cameraCom.targetTexture		= rt;
		mTempTexture.mainTexture	= rt;
		m_RTTGo.SetActive(true);	
		
		//add auto adjust cam size
		if ( useCameraCom )
		{
			AutoAdjustCamSize camFllow = m_RTTGo.GetComponent<AutoAdjustCamSize>();
			if(camFllow == null)
				camFllow = m_RTTGo.AddComponent<AutoAdjustCamSize>();
			
			camFllow.camera = cameraCom;
			camFllow.xModel = self;
			camFllow.isWhole = true;
			camFllow.isCareZ = true;
		}
	}
	
	private void _ShowModel(XU3dModel self)
	{
		if(m_ObjectModel == null)
			m_ObjectModel = self;
		
		CharacterController control = m_ObjectModel.m_gameObject.GetComponentInChildren<CharacterController>();
		if(control == null)
			return ;
		
		Transform chestTF = m_ObjectModel.GetSkeleton(ESkeleton.eHeadCenter);
		
		m_ObjectModel.Parent = m_RTTGo.transform;
		m_ObjectModel.Layer = GlobalU3dDefine.Layer_ModelRTT;
		//m_ObjectModel.Direction = new Vector3(0,180,0);
		m_ObjectModel.Position = m_RTTGo.transform.TransformPoint(new Vector3(TexturePos.x,-chestTF.position.y,TexturePos.z));
		m_ObjectModel.PlayAnimation(EAnimName.Idle, 1.0f, false);
		
		Camera cameraCom = m_RTTGo.GetComponent<Camera>();
		cameraCom.targetTexture		= rt;
		mTempTexture.mainTexture	= rt;
		m_RTTGo.SetActive(true);
		
		m_RTTGo.transform.localPosition	= new Vector3((Count++) * 10.0f,0f,0f);
	}
	
	public void DestoryModelRtt()
	{
		if(null!=m_ObjectModel)
		{
			m_ObjectModel.Destroy();
			if(m_RTTGo)
				GameObject.Destroy(m_RTTGo);
		}		
	}
	
	public void DestoryNotModel()
	{
		if(m_RTTGo)
				GameObject.Destroy(m_RTTGo);
	}
	
	public void RotLeft()
	{
		if(m_ObjectModel != null)
			m_ObjectModel.Direction	= new Vector3(m_ObjectModel.Direction.x,m_ObjectModel.Direction.y + 5.0f,m_ObjectModel.Direction.z);
	}
	
	public void RotRight()
	{
		if(m_ObjectModel != null)
			m_ObjectModel.Direction	= new Vector3(m_ObjectModel.Direction.x,m_ObjectModel.Direction.y - 5.0f,m_ObjectModel.Direction.z);
	}
}

public class XModelRTTMgr
{
	private XModelRTTMgr() {}
	
	private static XModelRTTMgr mgr = new XModelRTTMgr();
	public static XModelRTTMgr SP {get { return mgr;} }
	
	private List<SingleModelRTT>	mRTTList	= new List<SingleModelRTT>();
	
	private SingleModelRTT m_SingleModelRtt = null;
	
	public void AddNpcRTT(uint npcID,UITexture texture)
	{
		if(null!=m_SingleModelRtt)
		{
			m_SingleModelRtt.DestoryModelRtt();
		}
		
		m_SingleModelRtt = new SingleModelRTT();
		
		m_SingleModelRtt.ShowSingleNPCModel(npcID,texture,0f,-20.8f,1f);
	}
	
	public void AddSingleModel(uint modelID,UITexture texture )
	{
		if(null!=m_SingleModelRtt)
		{
			m_SingleModelRtt.DestoryModelRtt();
		}
		
		m_SingleModelRtt = new SingleModelRTT();
		
		m_SingleModelRtt.ShowSingleModel(modelID,texture,0f,-20.8f,1f);
	}
	
	public SingleModelRTT AddSingleModelWhole(uint modelID,UITexture texture )
	{
		SingleModelRTT rtt = new SingleModelRTT();
		
		rtt.ShowSingleModelWhole(modelID,texture,0f,-20.8f,5f);
		
		mRTTList.Add(rtt);
		
		return rtt;
	}
	
	public SingleModelRTT AddModelRTT(uint petModelID,UITexture texture,float x,float y,float z)
	{		
		SingleModelRTT rtt = new SingleModelRTT();
		rtt.ShowModel(petModelID,texture,x,y,z);
		
		mRTTList.Add(rtt);
		
		return rtt;
	}
	
	public SingleModelRTT AddObjectRTT(XU3dModel model, UITexture texture, float cameraSize, Vector3 pos, Vector3 rotate, Vector3 scale, bool useCameraCom)
	{
		SingleModelRTT rtt = new SingleModelRTT();
		rtt.ShowObjModel(model, texture, cameraSize, pos, rotate, scale, useCameraCom);
		
		mRTTList.Add(rtt);
		return rtt;
	}
}
