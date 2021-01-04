using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum EModelEvent
{	
	// XGameObject
	evtName,
	evtHeadBoardType,
	evtModelId,
	evtPosition,
	evtDirection,
	evtVisible,
	evtDestroy,
	evtAttachGo,
	evtScale,
	evtHudVisible,
	evtMatColor,
	
	// XCharacter
	evtTitle,
	evtHp,
	evtMaxHp,
	evtWeaponId,
	evtAnimation,
	evtShowBlood,
	evtFlyString,
	evtFlyStringHalf,
	evtNickName,
	
	// XPlayer
	evtUColor,
	evtMountIndex,
	
	// XNpc
	evtNpcFuncHead,
	
	evtSelect,	
	evtSetHeadVisiable,
	
	// XMainPlayer,
	evtAttachMainCamera,
	evtFadeOut,
}

//头顶板子的类型
public enum EHeadBoard_Type
{
	EHeadBoard_Type_None,
	EHeadBoard_Type_Monster,
	EHeadBoard_Type_Player,
	EHeadBoard_Type_NPC,
	EHeadBoard_Type_Num
}

public enum EMoveTarget_Type
{
	EMoveTarget_Object,
	EMoveTarget_Pos,
	EMoveTarget_Num	
}

public class XObjectModel
{
	public int index = 0;
	private static readonly Vector3	CAMERA_DEFAULT_POS = new Vector3 (0, 20, -20);
	private static uint FootEffectID = 900038;
	private static Transform m_ObjectManager = null;
	private static int ObjectIndex = 0;

	private static Transform ObjectParent {
		get {
			if (null == m_ObjectManager) {
				m_ObjectManager = new GameObject ("ObjectManager").transform;
				m_ObjectManager.parent = LogicApp.SP.transform;
				m_ObjectManager.position = Vector3.zero;
			}
			return m_ObjectManager;
		}
	}
	
	public delegate void LoadModelComplete (XObjectModel model);
	public LoadModelComplete mainModelLoaded;
	public event LoadModelComplete	LoadModelEvent;
	
	public delegate void LoadMountModelComplete(XU3dModel model);
	public LoadMountModelComplete mountLoaded;
		
	//private XGameObject m_xobj;
	private XU3dModel m_MainModel;
	private XU3dModel m_WeaponModel;
	private XU3dModel m_MountModel;
	private bool mountLoadFinish = false;
	
	EHeadBoard_Type	mHeadBoardType	= EHeadBoard_Type.EHeadBoard_Type_None;
	private XObjectHead m_Head;
	private string 		m_Name;
	
	private XU3dEffect	mSelectEffect;
	
	
	private XObjectHalf m_Half;
	
	public XU3dModel mainModel {
		get{ return m_MainModel; }
	}
	
	public XU3dModel mountModel {
		get{ return m_MountModel; }
	}
	
	private static EAnimName[] m_mountIdleAnim = new EAnimName[]{EAnimName.Idle, EAnimName.RideIdle, EAnimName.FloatIdle};
	private static EAnimName[] m_mountRunAnim = new EAnimName[]{ EAnimName.Idle, EAnimName.RideRun, EAnimName.FloatRun };
	private static ESkeleton[] m_syncSkeleton = new ESkeleton[] { ESkeleton.eCapsuleBottom, ESkeleton.eCapsuleHalf, ESkeleton.eCapsuleTop };
	
	// 信息记录
	private EAnimName m_curAnim;		// 切换动画时记录, 在上下坐骑的时候使用
	private float m_curAnimSpeed;
	private bool m_bIsVisible;			// 记录是否可见, Head资源到时利用
	private bool m_bShowBlood;			// 记录血条是否可见, Head资源到时利用
	private int m_nHp;					// 记录血量, Head到时利用
	private int m_nMaxHp;				// 记录最大血量, Head到时利用
	private string m_nNpcFuncName;		// 记录npc头顶面板图片， 资源到时使用
	private string m_nNickName;
	
	private EMoveTarget_Type	mMoveTType;
	private GameObject	m_MoveTarget;
	private XU3dEffect	m_FootEffect;
	private Vector3		m_MoveTargetPos;
	
	public interface IModelListener
	{
		void OnModelLoaded();
		void OnWeaponLoaded();
	}
	
	public void AddModelListener(IModelListener pML)
	{
		mListenerList.Add (pML);
	}
	
	List<IModelListener> mListenerList = new List<IModelListener> ();
	
	public XObjectModel ()
	{
		m_MainModel = null;
		m_WeaponModel = null;
		m_MountModel = null;
		m_Head = null;
		
		m_bIsVisible = true;
		m_bShowBlood = false;
		m_nHp = 0;
		m_nMaxHp = 1;
	}	
	
	public void HandleEvent(EModelEvent evt, params object[] args)
	{
		switch (evt)
		{
		case(EModelEvent.evtName):
			onName ((string)args [0]);
		
			break;
		case(EModelEvent.evtModelId):
			onModelId ((uint)args [0]);
			
			break;
		case(EModelEvent.evtWeaponId):
			onWeaponID ((uint)args [0]);
		
			break;
		case(EModelEvent.evtPosition):
			onPosition ((Vector3)args [0]);
		
			break;
		case(EModelEvent.evtDirection):
			onDirection ((Vector3)args [0]);
		
			break;
		case(EModelEvent.evtMatColor):
			onMatColor ((Color)args [0]);
		
			break;
		case(EModelEvent.evtVisible):
			onVisible ((bool)args [0]);
		
			break;
		case(EModelEvent.evtDestroy):
			onDestroy ();
		
			break;			
		case(EModelEvent.evtAttachGo):
			onAttachGo ((ESkeleton)args [0], (GameObject)args [1], (Vector3)args [2], (Vector3)args [3]);
		
			break;
		case(EModelEvent.evtScale):
			onScale ((float)(args [0]));
		
			break;
		case(EModelEvent.evtMountIndex):
			onMountIndex ((ushort)args [0]);
		
			break;
		case(EModelEvent.evtAttachMainCamera):
			onAttachMainCamera ();
		
			break;
		case(EModelEvent.evtHp):
			onHp ((int)(args [0]));
		
			break;
		case(EModelEvent.evtMaxHp):
			onMaxHp ((int)(args [0]));
		
			break;
		case(EModelEvent.evtNpcFuncHead):
			onNpcFuncPic ((string)(args [0]));
		
			break;
		case(EModelEvent.evtSetHeadVisiable):
			onHeadVisiable ((bool)(args [0]));
		
			break;
		case(EModelEvent.evtAnimation):
			onAnimation ((EAnimName)(args [0]), (float)(args [1]), (bool)(args [2]));
		
			break;
		case(EModelEvent.evtShowBlood):
			onShowBlood ((bool)(args [0]));
		
			break;
		case EModelEvent.evtFlyString:
			onFlyString ((EFlyStrType)args [0], (string)args [1]);
		
			break;
		case EModelEvent.evtFlyStringHalf:
			onFlyStringHalf ((EObjectHalfHintType)args [0], (string)args [1]);
		
			break;
		case EModelEvent.evtHudVisible :
			onHudVisible ((bool)args [0]);
		
			break;
		case EModelEvent.evtFadeOut :
			FadeOut ();
		
			break;
		case EModelEvent.evtNickName:
			onNickName ((string)args [0]);
		
			break;
		case EModelEvent.evtHeadBoardType:
			onHeadBoardType((EHeadBoard_Type)args[0]);
			break;
		case EModelEvent.evtSelect:
			onSelect((uint)args[0],(bool)args[1]);
			break;
		}
	}
	
	public void GetHeadPosInfo(ref Vector3 pos, ref GameObject parent)
	{
		if (null == m_Head)
			return;
		
		m_Head.GetHeadPosInfo (ref pos, ref parent);
		return;
	}
	
	private void onHudVisible(bool isVisible)
	{
		if (m_Head == null)
			return ;
		
		m_Head.SetVisible (isVisible);
	}
	
	private void onHeadBoardType(EHeadBoard_Type type)
	{
		if(mHeadBoardType	== type)
			return ;
		
		mHeadBoardType	= type;	
	}
	
	private void onSelect(uint effectID,bool isSelect)
	{
		if(m_MainModel == null)
			return ;
		
		if(isSelect)
		{
			if(mSelectEffect == null)
				mSelectEffect	= new XU3dEffect(effectID);
			
			mSelectEffect.Visible	= true;
			onAttachGo(ESkeleton.eMainObject,mSelectEffect.m_gameObject,Vector3.zero,Vector3.zero);
		}
		else
		{
			if(mSelectEffect == null)
				return ;
			
			mSelectEffect.Visible = false;
		}		
	}
	
	private void onName(string name)
	{
		m_Name	= name;
		
		if (null == m_Head)
			return;		
		m_Head.SetName (m_Name);
	}

	private void onNickName(string str)
	{
		m_nNickName = str;
		if (null == m_Head)
			return;
		m_Head.SetNickName (m_nNickName);
	}
	
	private void onModelId(uint modelId)
	{
		if (0 == modelId) 
		{
			if (null != m_MainModel) 
			{
				m_MainModel.Destroy (true);
				m_MainModel = null;
				if (null != m_Head) 
				{
					Object.Destroy (m_Head.gameObject);
					m_Head = null;
				}
				if (null != m_Half) 
				{
					Object.Destroy (m_Half.gameObject);
					m_Half = null;
				}
			}
		}
		else 
		{
			if (null == m_MainModel) 
			{
				m_MainModel = new XU3dModel(m_Name, modelId, ObjectParent, StartLoadHead);
			}
			else
			{
				m_MainModel.RefreshDynObject(modelId);
				
				if ( modelId == m_MainModel.m_nId && null != mainModelLoaded )
					mainModelLoaded(this);
			}
		}
	}
	
	private void onWeaponID(uint weaponID)
	{
		if (null != m_WeaponModel) {
			m_WeaponModel.Destroy ();
			m_WeaponModel = null;
		}		
		m_WeaponModel = new XU3dModel ("role_weapon", weaponID,OnWeaponLoadDone);
	}
	
	public void OnWeaponLoadDone(XU3dModel self)
	{
		m_WeaponModel = self;
		
		m_WeaponModel.Scale = m_MainModel.Scale;
		m_MainModel.AttachU3dModel (ESkeleton.eWeapon, m_WeaponModel, ESkeleton.eMainObject);
		
		foreach (IModelListener ls in mListenerList) {
			ls.OnWeaponLoaded();
		}
	}
	
	public void ReAttachWeapon()
	{
		if (m_WeaponModel == null)
			return ;
		
		m_MainModel.AttachU3dModel (ESkeleton.eWeapon, m_WeaponModel, ESkeleton.eMainObject);
	}
	
	public void ReAttachMount()
	{
		return;
		
		if (m_MountModel == null)
			return ;
		
		ESkeleton ske = ESkeleton.eMountZuo;
		if (XU3dModel.EMountModelType.eMountZhan == m_MountModel.m_mountModelType) 
		{
			ske = ESkeleton.eMountZhan;
		}
		else if (XU3dModel.EMountModelType.eMountFei == m_MountModel.m_mountModelType) 
		{
			ske = ESkeleton.eMountFei;
		}
		m_MainModel.ModelDirection = new Vector3(0, 180, 0);
		m_MountModel.AttachU3dModel(ESkeleton.eMount, m_MainModel, ske);
		onAnimation(m_curAnim, m_curAnimSpeed, false);
	}
	
	private void onPosition(Vector3 pos)
	{
		if (null != m_MountModel) {
			m_MountModel.Position = pos;
		}
		else if (null != m_MainModel) {
				m_MainModel.Position = pos;
			}
	}
	
	private void onDirection(Vector3 dir)
	{
		if (null != m_MountModel) {
			m_MountModel.ModelDirection = dir;
		}
		else if (null != m_MainModel) {
			m_MainModel.ModelDirection = dir;
		}
	}
	
	private void onMatColor(Color color)
	{
		if(m_MainModel != null)
		{
			m_MainModel.ChangeMatColor(color);
		}
	}
	
	private void onVisible(bool b)
	{
		m_bIsVisible = b;
		if (null != m_MountModel)
			m_MountModel.Visible = b;
		else if (null != m_MainModel)
				m_MainModel.Visible = b;
		//if (null != m_Head)
		//	m_Head.SetVisible (b);
		if (null != m_Head)
		{
			m_Head.gameObject.SetActive(b);
		}
	}
	
	private void onDestroy()
	{
		if (null != m_MountModel) {
			m_MountModel.Destroy ();
			m_MountModel = null;
		}
		if (null != m_MainModel) {
			m_MainModel.Destroy ();
			m_MainModel = null;
		}
		if (null != m_Head) {
			Object.Destroy (m_Head.gameObject);
			m_Head = null;
		}
		if (null != m_Half) {
			Object.Destroy (m_Half.gameObject);
			m_Half = null;
		}
		if (null != m_WeaponModel) {
			m_WeaponModel.Destroy ();
			m_WeaponModel = null;
		}
	}
	
	private void onAttachGo(ESkeleton ske, GameObject go, Vector3 localPos, Vector3 localRot)
	{
		// 如果有坐骑, 可能会绑定到坐骑上
		for (int i=0; i<m_syncSkeleton.Length; i++) {
			if (ske == m_syncSkeleton [i]) {
				if (null != m_MountModel) {
					m_MountModel.AttachGameObject (ske, go, localPos, localRot);
				}
				else if (null != m_MainModel) {
						m_MainModel.AttachGameObject (ske, go, localPos, localRot);
					}
				return;
			}
		}
		
		if (null == m_MainModel) 
			return;
		m_MainModel.AttachGameObject (ske, go, localPos, localRot);
	}
	
	private void onScale(float f)
	{
		if (null != m_MountModel) {
			m_MountModel.Scale = Vector3.one * f;
		}
		else if (null != m_MainModel) {
				m_MainModel.Scale = Vector3.one * f;
			}
		if (null != m_Head) {
			m_Head.SetScale (1.0f / f);
		}
		if (null != m_Half) {
			m_Half.SetScale (1.0f / f);
		}
	}
	
	private void onHp(int nHp)
	{
		m_nHp = nHp;
		if (null != m_Head) {
			m_Head.SetHp (m_nHp, m_nMaxHp);
		}
	}
	
	private void onNpcFuncPic(string spriteName)
	{
		m_nNpcFuncName = spriteName;
		if (null != m_Head) {
			m_Head.KinderIndex = index;
			m_Head.SetNpcHead (spriteName);
		}
	}
	
	private void onHeadVisiable(bool bVisiable)
	{
		m_Head.SetVisible (bVisiable);
	}
	
	private void onMaxHp(int nMaxHp)
	{
		m_nMaxHp = nMaxHp;
		if (null != m_Head) {
			m_Head.SetHp (m_nHp, m_nMaxHp);
		}
	}
	
	private void onAnimation(EAnimName anim, float fSpeed, bool bIsPush)
	{
		m_curAnim = anim;
		m_curAnimSpeed = fSpeed;
		if (null != m_MountModel) 
		{
			m_MountModel.PlayAnimation (anim, fSpeed, bIsPush);
			if (null != m_MainModel) 
			{
				if (EAnimName.Idle == anim) 
				{
					EAnimName mAnim = m_mountIdleAnim [(int)m_MountModel.m_mountModelType - (int)XU3dModel.EMountModelType.eMountZhan];
					m_MainModel.PlayAnimation (mAnim, fSpeed, bIsPush);
				}
				else if (EAnimName.Run == anim) 
				{
					EAnimName mAnim = m_mountRunAnim [(int)m_MountModel.m_mountModelType - (int)XU3dModel.EMountModelType.eMountZhan];
					m_MainModel.PlayAnimation (mAnim, fSpeed, bIsPush);
				}
			}
		}
		else if (null != m_MainModel) 
		{
			m_MainModel.PlayAnimation (anim, fSpeed, bIsPush);
			
		}
	}
	
	private void onShowBlood(bool b)
	{
		m_bShowBlood = b;
		if (null != m_Head) 
			m_Head.ShowBlood (b);
	}
	
	private void onFlyString(EFlyStrType ft, string str)
	{
		if (null != m_Head)
			m_Head.FlyString (ft, str);
	}
	
	private void onFlyStringHalf(EObjectHalfHintType ht, string str)
	{
		if (null != m_Half)
			m_Half.FlyHalfHint (ht, str);
	}
	
	private void onMountIndex(ushort mountIndex)
	{
		// 使用坐骑前，先销毁上个坐骑，避免坐骑与角色之间的父子关系紊乱
		if (null != m_MountModel && mountLoadFinish ) 
		{
			if (null != m_MainModel) 
			{
				m_MainModel.Parent = m_MountModel.Parent;
				m_MainModel.ModelDirection = m_MountModel.ModelDirection;
				m_MainModel.m_gameObject.transform.localRotation	= Quaternion.identity;
				m_MainModel.Position = m_MountModel.Position;
					
				m_MainModel.PlayAnimation (m_curAnim, m_curAnimSpeed, false);
				for (int i=0; i<m_syncSkeleton.Length; i++)
					m_MainModel.SyncChildObject (m_MountModel, m_syncSkeleton [i], m_syncSkeleton [i]);
				if (m_MountModel.IsMainCameraAttach)
					m_MainModel.AttachMainCamera (ESkeleton.eCameraBind);
			}
			m_MountModel.Destroy();
			m_MountModel = null;
		}
		else if ( null != m_MountModel && !mountLoadFinish )
			return;
		
 		XCfgMount cfgMount = XCfgMountMgr.SP.GetConfig (mountIndex);
		if (null == cfgMount || 0 == cfgMount.ModelId) 
		{
			return;
		}
		else 
		{
			mountLoadFinish = false;
			Vector3 temp = new Vector3(m_MainModel.Position.x, m_MainModel.Position.y, m_MainModel.Position.z);
			m_MountModel = new XU3dModel ("_mount_" + cfgMount.MountName, cfgMount.ModelId, 
				(XU3dModel.EMountModelType)cfgMount.MountType, OnMountModelLoadDone);
			m_MountModel.Position = temp;
		}
	}
	
	public void OnMountModelLoadDone(XU3dModel self)
	{
		mountLoadFinish = true;
		ESkeleton ske = ESkeleton.eMountZuo;
		if (XU3dModel.EMountModelType.eMountZhan == self.m_mountModelType) 
		{
			ske = ESkeleton.eMountZhan;
		}
		else if (XU3dModel.EMountModelType.eMountFei == self.m_mountModelType) 
		{
			ske = ESkeleton.eMountFei;
		}
		
		self.Parent = m_MainModel.Parent;
		self.ModelDirection = m_MainModel.ModelDirection;
				
		self.AttachU3dModel (ESkeleton.eMount, m_MainModel, ske);
		self.Scale = Vector3.one;
		m_MainModel.Scale = Vector3.one;
		
		m_MainModel.ModelDirection = new Vector3(0, 180, 0);

		for (int i=0; i<m_syncSkeleton.Length; i++) 
		{
			self.SyncSkeleton (m_MainModel, m_syncSkeleton [i]);
			self.SyncChildObject (m_MainModel, m_syncSkeleton [i], m_syncSkeleton [i]);
		}
		if (m_MainModel.IsMainCameraAttach)
			self.AttachMainCamera (ESkeleton.eCameraBind);
		
		m_MountModel = self;
		onAnimation (m_curAnim, m_curAnimSpeed, false);
		
		if ( null != mountLoaded )
			mountLoaded(self);
	}
	
	private void onAttachMainCamera()
	{
		bool success = false;
		if (null != m_MountModel) 
		{
			success = m_MountModel.AttachMainCamera (ESkeleton.eCameraBind, CAMERA_DEFAULT_POS);
		}
		if (null != m_MainModel && !success ) 
		{
			m_MainModel.AttachMainCamera (ESkeleton.eCameraBind, CAMERA_DEFAULT_POS);
		}
	}
	
	public void _onObjectHeadReady(XObjectHead head)
	{
		if (null == m_MainModel || !m_MainModel.isLoaded || null != m_Head)
		{
			if( m_MainModel != null && m_MainModel.isLoaded )
			{
				Transform top1 = m_MainModel.GetSkeleton (ESkeleton.eCapsuleTop);
				UIFollowTarget uiFT1 = m_Head.GetComponent<UIFollowTarget> ();
				uiFT1.target = top1;
			}
			return;	
		}
			
		
		GameObject child = NGUITools.AddChild (HUDRoot.go, head.gameObject);
		m_Head = child.GetComponentInChildren<XObjectHead> ();
		m_Head.Init ();
		m_Head.KinderIndex = index;
		Transform top = m_MainModel.GetSkeleton (ESkeleton.eCapsuleTop);
		UIFollowTarget uiFT = child.AddComponent<UIFollowTarget> ();
		uiFT.target = top;
		uiFT.SelfHead = m_Head;
		if (uiFT.target == null) {
			Log.Write (LogLevel.WARN, "null");
		}
		XUtil.SetLayer (m_Head.gameObject, GlobalU3dDefine.Layer_UI_2D);
		m_Head.SetName (m_Name);
		m_Head.SetHp (m_nHp, m_nMaxHp);
		m_Head.SetVisible (m_bIsVisible);
		m_Head.ShowBlood (m_bShowBlood);
		m_Head.SetNpcHead (m_nNpcFuncName);
		m_Head.SetNickName(m_nNickName);
		
		_ReFindHeadPos (m_MainModel);
	}
	
	private void StartLoadHead(XU3dModel model)
	{
		m_MainModel = model;
		EUIPanel headType = EUIPanel.eCount;
		switch (mHeadBoardType) 
		{
		case(EHeadBoard_Type.EHeadBoard_Type_Player):
			headType = EUIPanel.ePlayerHead;
		
			break;			
		case(EHeadBoard_Type.EHeadBoard_Type_NPC):
			headType = EUIPanel.eNpcHead;
		
			break;			
		default:
			headType = EUIPanel.eMonsterHead;
			break;
		}
		
		if (EUIPanel.eCount != headType) 
		{
			XEventManager.SP.SendEvent (EEvent.UI_ReqOriginal, headType, this);
		}
		XEventManager.SP.SendEvent (EEvent.UI_ReqOriginal, EUIPanel.eObjectHalf, this);
		
		if ( null != mainModelLoaded )
			mainModelLoaded(this);
	}
	
	private void _ReFindHeadPos(XU3dModel model)
	{
		if (m_Head == null)
			return ;
		UIFollowTarget UIFT = m_Head.GetComponent<UIFollowTarget> ();
		if (UIFT == null)
			return ;
		
		UIFT.target = model.GetSkeleton (ESkeleton.eCapsuleTop);
		
		foreach (IModelListener ls in mListenerList) {
			ls.OnModelLoaded ();
		}
		
		if (LoadModelEvent != null)
			LoadModelEvent (this);
	}
	
	public void _onObjectHalfReady(XObjectHalf half)
	{
		if (null == m_MainModel || null != m_Half)
			return;
		
		m_Half = XUtil.Instantiate<XObjectHalf> (half);
		m_Half.Init ();
		XUtil.SetLayer (m_Half.gameObject, m_MainModel.Layer);
		m_MainModel.AttachGameObject (ESkeleton.eCapsuleHalf, m_Half.gameObject);
	}
	
//	private EBehaviourType getBehaviourType()
//	{
//		switch (m_xobj.ObjectType) {
//			case(EObjectType.MainPlayer):
//				return EBehaviourType.e_BehaviourType_MainPlayer;
//			
//			case(EObjectType.OtherPlayer):
//				return EBehaviourType.e_BehaviourType_OtherPlayer;
//			
//			case(EObjectType.Monster):
//				return EBehaviourType.e_BehaviourType_Monster;
//			
//			case(EObjectType.Npc):
//				return EBehaviourType.e_BehaviourType_Npc;
//			
//			case(EObjectType.TransPoint):
//				return EBehaviourType.e_BehaviourType_TransPoint;
//			
//			case(EObjectType.GatherObject):
//				return EBehaviourType.e_BehaviourType_GatherObject;
//		}
//		
//		return EBehaviourType.e_BehaviourType_Other;
//	}
	
	public Transform _getSkeleton(ESkeleton ske)
	{
		if (null == m_MainModel) 
			return null;
		return m_MainModel.GetSkeleton (ske);
	}
	
	public float _animLength(EAnimName name)
	{
		if (null == m_MainModel)
			return 0f;
		return m_MainModel.AnimLength (name);
	}
	
	public void HoverIn()
	{
		if (m_MainModel != null)
			m_MainModel.HoverIn ();
		
		if (m_WeaponModel != null)
			m_WeaponModel.HoverIn ();
		
		if (m_MountModel != null)
			m_MountModel.HoverIn ();
	}
	
	public void HoverOut()
	{
		if (m_MainModel != null)
			m_MainModel.HoverOut ();
		
		if (m_WeaponModel != null)
			m_WeaponModel.HoverOut ();
		
		if (m_MountModel != null)
			m_MountModel.HoverOut ();
	}
	
	public void FadeOut()
	{
		CoroutineManager.StartCoroutine (UpdateAlpha ());
	}
	
	public IEnumerator UpdateAlpha()
	{
		for (int i = 0; i < 100; i++) {
			if (m_MainModel != null)
				m_MainModel.FadeOut (0.01f);
		
			if (m_WeaponModel != null)
				m_WeaponModel.FadeOut (0.01f);
			
			if (m_MountModel != null)
				m_MountModel.FadeOut (0.01f);
			
			yield return 0;
		}
	}
	
	public float _getRadius()
	{
		if (null != m_MountModel)
			return m_MountModel.Radius ();
		if (null != m_MainModel)
			return m_MainModel.Radius ();
		return 0f;
	}
	
	public EXRayMatType XRayType
	{
		get 
		{
			if(m_MainModel == null)
				return EXRayMatType.EXRayMatType_None;
			
			return m_MainModel.XRayType;
		}
		
		set
		{
			if(m_MainModel == null)
				return ;
			
			m_MainModel.XRayType	= value;
		}
	}
	
	public EXRayMatType	WeaponXRayType
	{
		get 
		{
			if(m_WeaponModel == null)
				return EXRayMatType.EXRayMatType_None;
			
			return m_WeaponModel.XRayType;
		}
		
		set
		{
			if(m_WeaponModel == null)
				return ;
			
			m_WeaponModel.XRayType	= value;
		}
	}
	
	public void AddBehaviourListener(EBehaviourType type,IBehaviourListener listener)
	{
		if(m_MainModel == null)
			return ;
		
		m_MainModel.AddBehaviourListener(type,listener);
	}
	
	public void SetMoveTarget(EMoveTarget_Type type,GameObject go ,Vector3 targetPos)
	{
		mMoveTType	= type;
		if(type == EMoveTarget_Type.EMoveTarget_Object)
		{
			if(go == null)
				return ;
			m_MoveTarget	= go;
		}
		else
		{
			m_MoveTargetPos	= targetPos;
		}		
		
		if(m_FootEffect == null)
		{
			m_FootEffect	= new XU3dEffect(FootEffectID);
			onAttachGo(ESkeleton.eMainObject,m_FootEffect.m_gameObject,Vector3.zero,Vector3.zero);
		}
		else
		{
			m_FootEffect.m_gameObject.SetActive(true);
		}
	}
	
	public GameObject	GetCurMoveTarget() 
	{
		return m_MoveTarget;
	}
	
	public void CancelMoveTarget()
	{
		m_MoveTarget	= null;
		m_MoveTargetPos	= Vector3.zero;
		if(m_FootEffect != null)
			m_FootEffect.m_gameObject.SetActive(false);
		
	}
	
	public void Breathe()
	{		
		if(m_FootEffect != null)
		{
			Vector3 targetPos = Vector3.zero;
			if(mMoveTType == EMoveTarget_Type.EMoveTarget_Object)
			{				
				if(m_MoveTarget != null)
				{
					targetPos	= m_MoveTarget.transform.position;					
				}
			}
			else
			{
				targetPos	= m_MoveTargetPos;
			}
			XU3dModel model = m_MainModel;
			if ( mountModel != null )
				model = mountModel;
			Vector3 deltaPos = targetPos - model.Position;
			deltaPos.Normalize();
			float deltaAngle = Mathf.Atan2(deltaPos.x,deltaPos.z);
			m_FootEffect.Direction	= new Vector3(0,Mathf.Rad2Deg * deltaAngle,0);
			
		}
	}
}
