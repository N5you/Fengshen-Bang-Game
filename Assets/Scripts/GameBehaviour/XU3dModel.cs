using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using resource;

//不允许在这几个对象中出现逻辑相关的数据!!!!!!
//XU3dModel 只是一个渲染显示部件，在游戏中例如单个模型，武器，坐骑
//XObjectModel 是一个游戏内逻辑对象的与引擎的交互部分，他可以包含多个XU3dModel

public enum EXRayMatType
{
	EXRayMatType_None,
	EXRayMatType_MainPlayer,
	EXRayMatType_MainPlayerWeapon,
	EXRayMatType_Num
}

public class XU3dModel : XU3dDynObject
{
	private class ChildObject
	{
		public ESkeleton Ske;
		public GameObject go;
		public Vector3 localPosition;
		public Vector3 localRotation;
		
		public ChildObject(ESkeleton ske, GameObject go, Vector3 p, Vector3 r)
		{
			this.Ske = ske;
			this.go = go;
			this.localPosition = p;
			this.localRotation = r;
		}
	}
	
	private class ChildModel
	{
		public ESkeleton srcSke;
		public XU3dModel tgtModel;
		public ESkeleton tgtSke;
		
		public ChildModel(ESkeleton srcSke, XU3dModel tgtModel, ESkeleton tgtSke)
		{
			this.srcSke = srcSke;
			this.tgtModel = tgtModel;
			this.tgtSke = tgtSke;
		}
	}
	
	private class BindSkeleton
	{
		public XU3dModel model;
		public ESkeleton ske;
		
		public BindSkeleton(XU3dModel model, ESkeleton ske)
		{
			this.model = model;
			this.ske = ske;
		}
	}
	
	public string ResourceName
	{
		get
		{
			if(m_realModel != null)
				return m_realModel.name;
			
			return "";
		}
	}
	
	public enum EMountModelType
	{
		eMountZhan = 1,
		eMountZuo,
		eMountFei
	}
	public EMountModelType m_mountModelType;
	
	
	public string m_name;	
	public EAnimName CurAnimName { get; private set; }
	public float CurAnimSpeed { get; private set; }
	private Animation m_animation;
	private XSkeleton m_skeleton;
	private GameObject m_realModel;
	private Vector3 m_modelRotation;				// 相对旋转
	private bool	m_IsRotChanged = false;
	private List<ChildObject> m_childObject;
	private ESkeleton m_mainCameraSke;				// 摄像机绑定的骨骼
	private List<ChildModel> m_childModel;
	private ChildModel m_fatherModel;
	private List<BindSkeleton> m_childSkeleton;		// 需要同步的骨骼信息
	private List<BindSkeleton> m_fatherSkeleton;	// 被同步的骨骼信息
	private bool bfilter = false;
	private string[] filterStr;
	
	private Color mOrginalColor;
	private bool  mIsInitColor;
	private Color HoverColor = new Color(1.0f,1.0f,1.0f,1.0f);
	private EXRayMatType	mXRayType = EXRayMatType.EXRayMatType_None;
	
	private CharacterController m_CharCtrl;			// 包围盒
	private XBehaviour m_behaviour;					// 存储XBehaviour信息
	private EBehaviourType m_behaType;
	private List<IBehaviourListener> m_listens;
	
	public delegate void OnModelLoadDone(XU3dModel self);
	public OnModelLoadDone	SendLoadDone;

	private bool bDone = false;
	
	public bool isLoaded{
		private set{}
		get{return bDone;}
	}
	
	public static void InitXRayMaterial(Material mat)
	{
		m_XRayMaterialMainPlayer = makeXRayMaterialName(mat,new Color(0.15f,0.4f,0.6f));
		m_XRayMaterialOtherPlayer= makeXRayMaterialName(mat,new Color(0.5f,0.6f,0.6f));
		m_XRayMaterialEnemy		 = makeXRayMaterialName(mat,new Color(1f,0.4f,0.6f));		
	}	

	private static Material m_XRayMaterialMainPlayer;
	private static Material m_XRayMaterialOtherPlayer;
	private static Material m_XRayMaterialEnemy;
	
	private List<Material> m_dynamicMaterialList = new List<Material>();
	
	private static Material makeXRayMaterialName(Material mat,Color color)
	{
		Material m = GameObject.Instantiate(mat) as Material;
		if(m == null)
			return null;
		
		m.SetColor("_Color",color);
		
		return m;
	}
	
	public XU3dModel(string strName,uint modelId)
	{
		initialize(strName);
		RefreshDynObject(modelId);
	}
	
	public XU3dModel(string strName,uint modelId,OnModelLoadDone LogicDone)
	{
		initialize(strName);
		SendLoadDone = LogicDone;
		RefreshDynObject(modelId);
	}
	
	public XU3dModel(string strName,uint modelId, Transform	 parent,OnModelLoadDone LogicDone)
	{
		initialize(strName);
		Parent = parent;
		SendLoadDone = LogicDone;
		RefreshDynObject(modelId);
	}
	
	public XU3dModel(string strName,uint modelId, EMountModelType mountType, OnModelLoadDone LogicDone)
	{
		m_mountModelType = mountType;
		initialize(strName);
		SendLoadDone = LogicDone;
		RefreshDynObject(modelId);
	}
	
	private void initialize(string strName)
	{
		m_name = strName;
		Layer = GlobalU3dDefine.Layer_GameObject;
		m_childObject = new List<ChildObject>();
		m_mainCameraSke = ESkeleton.eCount;
		m_childModel = new List<ChildModel>();
		m_fatherModel = null;
		m_childSkeleton = new List<BindSkeleton>();
		m_fatherSkeleton = new List<BindSkeleton>();
		clear();
		ModelDirection = Vector3.zero;
		CurAnimName = EAnimName.Idle;
		CurAnimSpeed = 1.0f;

		m_behaviour = null;
		m_listens = new List<IBehaviourListener>();
	}
	
	public override void RefreshDynObject (uint id)
	{
		if(m_nId == id) 
			return;

		m_nId = id;
		releaseDynObject();
		doRefreshDynObject();
	}
	
	private void doRefreshDynObject()
	{
		bDone = false;
		
		m_DynObject	= XResourceManager.GetResource(XResourceModel.ResTypeName,m_nId);
		if(m_DynObject == null)
		{
			Log.Write(LogLevel.ERROR,"cant find Model ID {0}",m_nId);
			return ;
		}
		
		if(m_DynObject.IsLoadDone())
			LoadCompleted(m_DynObject.MainAsset.DownLoad);
		else
		{
			//对于同一个对象，不应该出现多个完成事件
			if(m_DynObject != null)
			{
				m_DynObject.ResLoadEvent	-= LoadCompleted;
			}

			XResourceManager.StartLoadResource(XResourceModel.ResTypeName,m_nId);
			m_DynObject.ResLoadEvent	+= LoadCompleted;
			setDefaultModel();
		}
	}
	
	public void LoadCompleted(DownloadItem item)
	{
#if RES_DEBUG
			GameObject go = item.go as GameObject;
#else
			GameObject go = item.ab.mainAsset as GameObject;
#endif
		if(item == null)
		{
			Log.Write(LogLevel.WARN,"load Model failed item is null");
			return ;
		}
		if(go == null)
		{
			Log.Write(LogLevel.WARN,"load Model failed name is {0}",item.url);
			return ;
		}
		Log.Write(LogLevel.DEBUG,"resName is {0}",go.name);
		OnModelDone(m_nId,go);
	}
	
	private void OnModelDone(uint modelId, GameObject go)
	{
		if(modelId != m_nId || bDone) 
			return;
		bDone = true;
		
		setModel(go);
		
		if(SendLoadDone != null)
			SendLoadDone(this);
		
	}
	
	private void OnModelError(int modelId)
	{
		bDone = true;
		Log.Write(LogLevel.WARN, "XU3dModel, id 为 {0} 的动态模型资源加载出错 -> 还原默认模型", modelId);
	}
	
	protected void setDefaultModel()
	{
		//setModel(StaticResourceManager.SP.DefaultModel[(int)m_modelType]);
	}
	
	protected void setModel(GameObject go)
	{
		if(null == go) return;
		detachChildObject();
		detachMainCamera();
		detachChildModel();
		NewGameObject(m_name);
		Quaternion OrignalRot = go.transform.localRotation;
		Vector3 OrignalScale = go.transform.localScale;
		m_realModel = XUtil.Instantiate(go, m_gameObject.transform);

		m_realModel.transform.localRotation = Quaternion.Euler(OrignalRot.eulerAngles);
		m_realModel.transform.localScale	= OrignalScale;
		m_animation = m_realModel.GetComponent<Animation>();
		
		if(m_IsRotChanged)
			m_realModel.transform.localRotation = Quaternion.Euler(m_modelRotation);
		else
			m_modelRotation	= OrignalRot.eulerAngles;
			
		m_CharCtrl = m_realModel.GetComponent<CharacterController>();
		m_skeleton = new XSkeleton(m_gameObject, m_CharCtrl);
		reSyncChildSkeleton();
		reAttachChildObject();
		reAttachMainCamera();
		reAttachChildModel();
		XUtil.SetLayer(m_realModel, Layer);
		PlayAnimation(CurAnimName, CurAnimSpeed, false);

		reAttachFatherModel();
		reSyncFatherSkeleton();
		reAddXBehaviour();
		
		if(mXRayType != EXRayMatType.EXRayMatType_None)
		{
			EXRayMatType old = mXRayType;
			mXRayType	= EXRayMatType.EXRayMatType_None;
			setXRayMaterial(old);
		}
		
		if(!mIsInitColor)
		{
			SkinnedMeshRenderer[] renderList = m_realModel.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach(SkinnedMeshRenderer smr in renderList)
			{
				if(smr.materials.Length > 0)
				{
					if(smr.materials[0].HasProperty("_Color" ) )
						mOrginalColor = smr.materials[0].GetColor("_Color");
					
					if(mOrginalColor != new Color(0.0f,0.0f,0.0f,0.0f) )
						break ;
				}
				
			}
			mIsInitColor	= true;
		}	
	}
	
	private void setXRayMaterial(EXRayMatType type)
	{
		if(mXRayType == type)
			return ;
		
		if(m_realModel == null)
			return ;
		
		mXRayType	= type;
		
		Material currentMaterial;
		switch(mXRayType)
		{
			case EXRayMatType.EXRayMatType_MainPlayer:
				currentMaterial = m_XRayMaterialMainPlayer;
				break;
			
			case EXRayMatType.EXRayMatType_MainPlayerWeapon:
				currentMaterial = m_XRayMaterialMainPlayer;
				break;
			
//			case EBehaviourType.e_BehaviourType_OtherPlayer:
//				currentMaterial = m_XRayMaterialOtherPlayer;
//				break;
//			
//			case EBehaviourType.e_BehaviourType_OtherWeapon:
//				currentMaterial = m_XRayMaterialOtherPlayer;
//				break;
//			
//			case EBehaviourType.e_BehaviourType_Monster:
//				currentMaterial = m_XRayMaterialEnemy;
//				break;
			
			default:
					return; //not need material
				//break;
		}
		
		if(EXRayMatType.EXRayMatType_MainPlayerWeapon == mXRayType)//model type special
		{
			MeshRenderer[] meshRenders = m_realModel.GetComponentsInChildren<MeshRenderer>(true);
			foreach( MeshRenderer render in meshRenders )
			{
				setXRayRender(render,currentMaterial);
			}
		}
		
		SkinnedMeshRenderer[] someM = m_realModel.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		foreach( SkinnedMeshRenderer render in someM )
		{
			setXRayRender(render,currentMaterial);
		}
	}
	
	private void setXRayRender(Renderer render,Material XRayMaterial)
	{	
		//XRayMaterial
		Material[] materials = render.materials;
		Material[] newMaterials = new Material[materials.Length+1];
		
		for(int i=0;i<materials.Length;i++)
		{
			if(materials[i].shader.name.Contains("Particles") )  //exception particle
			{
				return;
			}
		}
		
		for(int i=0;i<materials.Length;i++)//copy the materials from old
		{
			newMaterials[i] = materials[i];
			if(newMaterials[i].renderQueue<XRayMaterial.renderQueue )
			{
				newMaterials[i].renderQueue = XRayMaterial.renderQueue+1;
				//newMaterials[i].renderQueue = 2400;
			}
			
			
			m_dynamicMaterialList.Add(newMaterials[i] );
		}
					
		newMaterials[materials.Length] = XRayMaterial;
		render.sharedMaterials = newMaterials;
	}
	
	public override bool Visible {
		get 
		{
			return base.Visible;
		}
		set 
		{
			bool bV = Visible;
			base.Visible = value;
			if(!bV && Visible)
				PlayAnimation(CurAnimName, CurAnimSpeed, false);
		}
	}
	
	private void releaseDynObject()
	{
//		if(null != m_DynObject)
//		{
//			if(!bDone)
//			{
//				m_DynObject.onDone -= OnModelDone;
//				m_DynObject.onError -= OnModelError;
//			}
//			m_DynObject.refCount--;
//			m_DynObject = null;
//		}
	}
	
	public override void Destroy(bool bDestroyAll)
	{
		releaseDynObject();
		
		if(!bDestroyAll) 
			detachChildObject();

		// 绑上来的摄像机和XU3dModel不负责销毁
		detachMainCamera();
		detachChildModel();
		
		clear();
		base.Destroy(bDestroyAll);
		
//		foreach(Material m in m_dynamicMaterialList)
//		{
//			GameObject.Destroy(m);
//		}
	}
	
	private void clear()
	{
		CurAnimName = EAnimName.Idle;
		m_animation = null;
		m_skeleton = null;
		m_CharCtrl = null;
		m_realModel = null;
		filterStr = null;
		m_childObject.Clear();
		m_mainCameraSke = ESkeleton.eCount;
		m_childModel.Clear();
		m_childSkeleton.Clear();
		m_fatherSkeleton.Clear();
	}
	
	// 缩放过程中需要保证摄像机的相对位置绝对(就是不能改变摄像机的滑动操作感觉)
	public override Vector3 Scale 
	{
		get 
		{
			Transform cameraBind = GetSkeleton(ESkeleton.eCameraBind);
			Vector3 oldLossyScale = Vector3.one;
			Vector3 newLossyScale = Vector3.one;
			
			if(null != cameraBind)
				oldLossyScale = cameraBind.lossyScale;

			Vector3 scale = base.Scale;
			
			if(null != cameraBind)
			{
				newLossyScale = cameraBind.lossyScale;
				cameraBind.transform.localScale /= (newLossyScale.x / oldLossyScale.x);
			}
			
			return scale;
		}
		set 
		{
			Transform cameraBind = GetSkeleton(ESkeleton.eCameraBind);
			Vector3 oldLossyScale = Vector3.one;
			Vector3 newLossyScale = Vector3.one;
			
			if(null != cameraBind)
				oldLossyScale = cameraBind.lossyScale;
			base.Scale = value;
			
			if(null != cameraBind)
			{
				newLossyScale = cameraBind.lossyScale;
				cameraBind.transform.localScale /= (newLossyScale.x / oldLossyScale.x);
			}
		}
	}
	
	public Vector3 ModelDirection 		// 相对旋转
	{
		get 
		{
			if(null != m_realModel)
				m_modelRotation = m_realModel.transform.localEulerAngles;
			return m_modelRotation;
		}
		set 
		{
			if(m_modelRotation != value)
			{
				m_modelRotation = value;
				m_IsRotChanged	= true;
				if(null != m_realModel)
				{
					m_realModel.transform.localRotation = Quaternion.Euler(m_modelRotation);
				}
			}
		}
	}
	
	public float AnimLength(EAnimName name)
	{
		if(null == m_animation)
			return 0f;
		return m_animation[name.ToString()].length;
	}

	// 已经计算了缩放
	public float Radius()
	{
		if(null == m_CharCtrl)
			return 0f;
		return m_CharCtrl.radius * Scale.x;
	}
	
	public void PlayAnimation(EAnimName name, float fSpeed, bool bIsPush)
	{
		XAnimationManager.SP.PlayAnimation(m_animation, CurAnimName, name, fSpeed, bIsPush);
		CurAnimName = name;
		CurAnimSpeed = fSpeed;
	}

	public Transform GetSkeleton(ESkeleton ske)
	{
		if(null == m_skeleton) return null;
		return m_skeleton.GetPoint(ske);
	}
	
	private void detachChildObject()
	{
		for(int i=0; i<m_childObject.Count;)
		{
			ChildObject child = m_childObject[i];
			if(null != child.go && child.go.transform.parent == GetSkeleton(child.Ske))
			{
				child.go.transform.parent = null;
				i++;
				continue;
			}
			m_childObject.RemoveAt(i);
		}
	}
	
	private void reAttachChildObject()
	{
		for(int i=0; i<m_childObject.Count; i++)
		{
			ChildObject child = m_childObject[i];
			child.go.transform.parent = GetSkeleton(child.Ske);
			child.go.transform.localPosition = child.localPosition;
			child.go.transform.localRotation = Quaternion.Euler(child.localRotation);
		}
	}
	
	private void detachMainCamera()
	{
		if(IsMainCameraAttach)
			XCameraLogic.SP.Detach();
	}
	
	private void reAttachMainCamera()
	{
		if(ESkeleton.eCount != m_mainCameraSke)
		{
			XCameraLogic.SP.AttachTo(GetSkeleton(m_mainCameraSke));
		}
	}
	
	private void detachChildModel()
	{
		for(int i=0; i<m_childModel.Count;)
		{
			ChildModel cm = m_childModel[i];
			if(null == cm 
				|| null == cm.tgtModel 
				|| null == cm.tgtModel.m_gameObject 
				|| null == cm.tgtModel.m_skeleton)
			{
				m_childModel.RemoveAt(i);
				continue;
			}
			Transform t = GetSkeleton(cm.srcSke);
			if(cm.tgtModel.Parent != t)
			{
				m_childModel.RemoveAt(i);
				continue;
			}
			cm.tgtModel.Parent = null;
			i++;
		}
	}
	
	private void reAttachChildModel()
	{
		for(int i=0; i<m_childModel.Count; i++)
		{
			AttachU3dModel(m_childModel[i]);
		}
	}
	
	private void reAttachFatherModel()
	{
		// 如果有父XU3dModel, 需要告诉父XU3dModel重新Attach自己一次
		if(null != m_fatherModel)
		{
			if(null == m_fatherModel.tgtModel)
			{
				m_fatherModel = null;
			}
			else
			{
				Transform srcSke = m_fatherModel.tgtModel.GetSkeleton(m_fatherModel.srcSke);
				if(null == srcSke || Parent != srcSke)
				{
					m_fatherModel = null;
				}
				else
				{
					m_fatherModel.tgtModel.AttachU3dModel(new ChildModel(m_fatherModel.srcSke, this, m_fatherModel.tgtSke));
				}
			}
		}
	}
	
	private void reSyncChildSkeleton()
	{
		for(int i=0; i<m_childSkeleton.Count;)
		{
			BindSkeleton childSke = m_childSkeleton[i];
			if(null == childSke.model || null == childSke.model.m_skeleton)
			{
				m_childSkeleton.RemoveAt(i);
				continue;
			}
			SyncSkeleton(childSke);
			i++;
		}
	}
	
	private void reSyncFatherSkeleton()
	{
		for(int i=0; i<m_fatherSkeleton.Count;)
		{
			BindSkeleton fatherSke = m_fatherSkeleton[i];
			if(null == fatherSke.model || null == fatherSke.model.m_skeleton)
			{
				m_fatherSkeleton.RemoveAt(i);
				continue;
			}
			fatherSke.model.SyncSkeleton(new BindSkeleton(this, fatherSke.ske));
			i++;
		}
	}
	
	private void reAddXBehaviour()
	{
		m_behaviour = m_realModel.AddComponent<XBehaviour>();
		m_behaviour.BehaType = m_behaType;
		for(int i=0; i<m_listens.Count; i++)
		{
			m_behaviour.AddListener(m_listens[i]);
		}
	}
	
	public void AttachGameObject(ESkeleton ske, GameObject go)
	{
		AttachGameObject(ske, go, Vector3.zero, Vector3.zero);
	}
	
	public void AttachGameObject(ESkeleton ske, GameObject go, Vector3 localPosition, Vector3 localRotation)
	{
		ChildObject child = new ChildObject(ske, go, localPosition, localRotation);		
		m_childObject.Add(child);
		go.transform.parent = GetSkeleton(ske);
		go.transform.localPosition = localPosition;
		go.transform.localRotation = Quaternion.Euler(localRotation);
	}
	
	public void AttachU3dModel(ESkeleton srcSke, XU3dModel tgtModel, ESkeleton tgtSke)
	{
		if(null == tgtModel || null == tgtModel.m_skeleton)
			return;
		tgtModel.m_fatherModel = new ChildModel(srcSke, this, tgtSke);
		ChildModel childModel = new ChildModel(srcSke, tgtModel, tgtSke);
		
		// 如果发现之前已经有同样的Attach需求, 删掉
		for(int i=0; i<m_childModel.Count;)
		{
			ChildModel cm = m_childModel[i];
			if(null == cm || cm.tgtModel == tgtModel)
			{
				m_childModel.RemoveAt(i);
				continue;
			}
			i++;
		}
		
		m_childModel.Add(childModel);
		AttachU3dModel(childModel);
	}
	
	private void AttachU3dModel(ChildModel childModel)
	{
		Transform srcTran = GetSkeleton(childModel.srcSke);
		Transform tgtTran = childModel.tgtModel.GetSkeleton(childModel.tgtSke);
		Transform temp = tgtTran.parent;
		
		tgtTran.parent = srcTran;
		childModel.tgtModel.Parent = tgtTran;
		
		tgtTran.localPosition = Vector3.zero;
		tgtTran.localRotation = Quaternion.Euler(Vector3.zero);
		
		childModel.tgtModel.Parent = srcTran;
		tgtTran.parent = temp;
	}
	
	public bool AttachMainCamera(ESkeleton ske)
	{
		m_mainCameraSke = ske;
		Transform form = GetSkeleton(m_mainCameraSke);
		if ( null != form )
		{
			XCameraLogic.SP.AttachTo(form);
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public bool AttachMainCamera(ESkeleton ske, Vector3 localPosition)
	{
		m_mainCameraSke = ske;
		Transform form = GetSkeleton(m_mainCameraSke);
		if ( null != form )
		{
			XCameraLogic.SP.AttachTo(form, localPosition);
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public bool IsMainCameraAttach
	{
		get
		{
			if(ESkeleton.eCount == m_mainCameraSke) 
				return false;
			
			if(XCameraLogic.SP.mainCamera.transform.parent != GetSkeleton(m_mainCameraSke))
			{
				m_mainCameraSke = ESkeleton.eCount;
				return false;
			}
			return true;
		}
	}
	
	public EXRayMatType XRayType
	{
		get 
		{
			return mXRayType;
		}
		
		set
		{
			setXRayMaterial(value);
		}
	}
	
	// 从别的u3dModel拷贝一个插槽过来(只拷贝position和rotation), 机制上会保证在模型变更的情况下插槽的信息仍然一致
	public void SyncSkeleton(XU3dModel tgtModel, ESkeleton ske)
	{
		if(null == tgtModel || null == tgtModel.m_skeleton)
			return;
		
		if(null == m_skeleton) return;
		BindSkeleton bs = new BindSkeleton(tgtModel, ske);
		m_childSkeleton.Add(bs);
		SyncSkeleton(bs);
		m_fatherSkeleton.Add(new BindSkeleton(this, ske));
	}
	
	private void SyncSkeleton(BindSkeleton bs)
	{
		Transform tgtSke = bs.model.GetSkeleton(bs.ske);
		m_skeleton.AddPoint(bs.ske, tgtSke);
	}
	
	// 把目标u3dModel挂加载tgtSke插槽下面所有的ChildObject以相同相对信息挂件到selfSke下面
	public void SyncChildObject(XU3dModel tgtModel, ESkeleton tgtSke, ESkeleton selfSke)
	{
		if(null == tgtModel || null == tgtModel.m_skeleton)
			return;
		
		for(int i=0; i<tgtModel.m_childObject.Count;)
		{
			ChildObject child = tgtModel.m_childObject[i];
			if(null == child.go || child.go.transform.parent != tgtModel.GetSkeleton(child.Ske))
			{
				tgtModel.m_childObject.RemoveAt(i);
				continue;
			}
			if(child.Ske == tgtSke)
			{
				tgtModel.m_childObject.RemoveAt(i);
				AttachGameObject(selfSke, child.go, child.localPosition, child.localRotation);
				continue;
			}
			i++;
		}
	}
	
	public void FadeOut(float alphaValue)
	{		
		if(m_realModel == null)
			return ;
		
		SkinnedMeshRenderer[] renderList = m_realModel.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(SkinnedMeshRenderer smr in renderList)
		{
			foreach(Material mat in smr.materials)
			{
				float tempAlpha = mat.color.a - alphaValue;
				if(tempAlpha < 0)
					tempAlpha = 0;
				
				mat.color	=	new Color(mat.color.r,mat.color.g,mat.color.b,tempAlpha);
				
			}
		}
		
		for(int i=0; i<m_childModel.Count; i++)
		{
			ChildModel child = m_childModel[i];
			child.tgtModel.FadeOut(alphaValue);
		}
	}
	
	public void ChangeMatColor(Color color)
	{
		if(m_realModel == null)
			return ;
		
		if(color == Color.white)
			color = mOrginalColor;
		SkinnedMeshRenderer[] renderList1 = m_realModel.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(SkinnedMeshRenderer smr in renderList1)
		{
			foreach(Material mat in smr.materials)
			{
				mat.color	=	color;
			}
		}
		
		for(int i=0; i<m_childModel.Count; i++)
		{
			ChildModel child = m_childModel[i];
			child.tgtModel.ChangeMatColor(color);
		}
	}
	
	public void HoverIn()
	{
		if(m_realModel == null)
			return ;	
			
		SkinnedMeshRenderer[] renderList1 = m_realModel.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(SkinnedMeshRenderer smr in renderList1)
		{
			foreach(Material mat in smr.materials)
			{
				mat.color	=	HoverColor;
			}
		}
		
		for(int i=0; i<m_childModel.Count; i++)
		{
			ChildModel child = m_childModel[i];
			child.tgtModel.HoverIn();
		}
		
	}
	
	public void HoverOut()
	{		
		if(m_realModel == null)
			return ;
		
		SkinnedMeshRenderer[] renderList = m_realModel.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(SkinnedMeshRenderer smr in renderList)
		{
			foreach(Material mat in smr.materials)
			{
				if(mat.HasProperty("_Color" ) )
					mat.color	=	mOrginalColor;
			}
		}
		
		for(int i=0; i<m_childModel.Count; i++)
		{
			ChildModel child = m_childModel[i];
			child.tgtModel.HoverOut();
		}
	}
	
	
	
	public void AddBehaviourListener(EBehaviourType type,IBehaviourListener listen)
	{
		m_listens.Add(listen);
		if(null != m_behaviour)
		{
			m_behaviour.BehaType	= type;
			m_behaviour.AddListener(listen);
		}
	}
}
